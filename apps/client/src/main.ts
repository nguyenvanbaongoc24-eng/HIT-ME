import Phaser from "phaser";
import { CONFIG } from "@hit-me/shared-config";
import { clampToArena, distanceToArenaWall, resolveRound, type ArenaBounds, type PlayerState, type RoundAction, type Vector2 } from "@hit-me/game-core";
import { THEME, type Quality } from "./theme/theme";
import assetManifest from "./assets/manifest.json";
import vi from "./i18n/vi.json";
import "./styles.css";

type Phase = "placement" | "reveal" | "throw" | "resolve" | "ended";
type UiButton = Phaser.GameObjects.Container & { label: Phaser.GameObjects.Text };
type Actor = PlayerState & {
  name: string;
  color: number;
  sprite?: Phaser.GameObjects.Container;
  ring?: Phaser.GameObjects.Arc;
  hearts?: Phaser.GameObjects.Arc[];
  knockout?: Phaser.GameObjects.Text;
  ticket?: Phaser.GameObjects.Container;
  ticketHearts?: Phaser.GameObjects.Arc[];
  ticketName?: Phaser.GameObjects.Text;
};
type CrowdMember = { sprite: Phaser.GameObjects.Container; home: Vector2; phase: number };

const BOT_COUNT = 3;
const PLAYER_ID = "player";
const ARENA: ArenaBounds = CONFIG.ARENA_SHAPE === "ellipse"
  ? { shape: "ellipse", a: CONFIG.ARENA_A, b: CONFIG.ARENA_B }
  : { shape: "circle", radius: CONFIG.ARENA_RADIUS };

class HitMeScene extends Phaser.Scene {
  private actors: Actor[] = [];
  private phase: Phase = "placement";
  private roundNo = 1;
  private roundEndsAt = 0;
  private currentAction: RoundAction | null = null;
  private lastResultText = "";
  private arenaCenter: Vector2 = { x: 0, y: 0 };
  private arenaScalePx = 0;
  private arenaApx = 0;
  private arenaBpx = 0;
  private arenaGraphics?: Phaser.GameObjects.Graphics;
  private aimGraphics?: Phaser.GameObjects.Graphics;
  private timerFill?: Phaser.GameObjects.Rectangle;
  private timerLabel?: Phaser.GameObjects.Text;
  private phaseLabel?: Phaser.GameObjects.Text;
  private aliveLabel?: Phaser.GameObjects.Text;
  private roundLabel?: Phaser.GameObjects.Text;
  private banner?: Phaser.GameObjects.Text;
  private readyButton?: UiButton;
  private weaponHud?: Phaser.GameObjects.Container;
  private playerHealth?: Phaser.GameObjects.Arc[];
  private chatPanel?: Phaser.GameObjects.Container;
  private settingsPanel?: Phaser.GameObjects.Container;
  private projectiles: Phaser.GameObjects.Container[] = [];
  private crowd: CrowdMember[] = [];
  private particleCount = 0;
  private quality: Quality = CONFIG.QUALITY;
  private sfxEnabled = true;
  private settingsText?: Phaser.GameObjects.Text;
  private qualityText?: Phaser.GameObjects.Text;
  private slowFrames = 0;
  private lastHudTick = 0;

  constructor() {
    super("hit-me");
  }

  create(): void {
    this.scale.on("resize", () => this.layout());
    this.createUi();
    this.startMatch();
    this.input.on("pointerdown", (pointer: Phaser.Input.Pointer) => this.onPointerDown(pointer));
    this.input.on("pointermove", (pointer: Phaser.Input.Pointer) => this.onPointerMove(pointer));
    this.input.on("pointerup", (pointer: Phaser.Input.Pointer) => this.onPointerUp(pointer));
  }

  update(time: number, delta: number): void {
    this.observeFrameBudget(delta);
    if (this.phase === "placement" && time >= this.roundEndsAt) this.resolveAndAnimateRound();
    this.updateAmbientMotion(time);
    if (time - this.lastHudTick >= THEME.motion.hudTickMs) {
      this.lastHudTick = time;
      this.updateHud();
    }
  }

  private createUi(): void {
    this.arenaGraphics = this.add.graphics().setDepth(0);
    this.aimGraphics = this.add.graphics().setDepth(18);
    this.roundLabel = this.add.text(18, 20, "", this.hudStyle(14, "#fff6dc")).setDepth(41);
    this.aliveLabel = this.add.text(0, 20, "", this.hudStyle(15)).setOrigin(1, 0).setDepth(30).setVisible(false);
    this.phaseLabel = this.add.text(0, 64, "", this.hudStyle(12, "#6d3d35")).setOrigin(0.5).setDepth(30).setVisible(false);
    this.timerFill = this.add.rectangle(0, 49, 1, 5, THEME.colors.gold).setOrigin(0.5, 0.5).setDepth(41);
    this.timerLabel = this.add.text(0, 17, "", this.hudStyle(26, "#fff6dc")).setOrigin(0.5, 0).setDepth(41);
    this.banner = this.add.text(0, 0, "", { color: "#fff6dc", fontFamily: THEME.fontFamily, fontSize: "14px", fontStyle: "700", align: "center", wordWrap: { width: 320 } }).setOrigin(0.5).setDepth(30);

    const settings = this.createButton("⚙", 42, 38, () => this.togglePanel("settings"), 15);
    settings.setDepth(31).setData("dock", "top-right");
    const chat = this.createButton("💬", 42, 38, () => this.togglePanel("chat"), 15);
    chat.setDepth(31).setData("dock", "bottom-left");
    this.readyButton = this.createButton(vi.ready, 102, 34, () => this.resolveAndAnimateRound(), 13);
    this.readyButton.setDepth(31).setVisible(false);
    this.weaponHud = this.add.container().setDepth(30);
    this.weaponHud.add([this.add.circle(0, 0, 20, THEME.colors.paper, 0.96).setStrokeStyle(2, THEME.colors.paperShade, 0.9), this.add.rectangle(0, 0, 25, 9, THEME.colors.gold).setAngle(-14)]);
    this.playerHealth = Array.from({ length: CONFIG.MAX_HP }, (_, index) => this.add.circle(index * 13, 0, 4.5, THEME.colors.heartFull).setDepth(30).setVisible(false));
    this.chatPanel = this.createPanel(vi.chatHint);
    this.settingsPanel = this.createSettingsPanel();
    this.settingsPanel.setVisible(false);
    this.chatPanel.setVisible(false);
  }

  private startMatch(): void {
    this.tweens.killAll();
    for (const projectile of this.projectiles) projectile.destroy();
    this.projectiles = [];
    for (const actor of this.actors) {
      actor.sprite?.destroy();
      actor.ticket?.destroy();
    }
    this.actors = [{ id: PLAYER_ID, name: vi.you, hp: CONFIG.MAX_HP, alive: true, color: THEME.colors.player[0], position: { x: -220, y: 0 } }];
    for (let index = 1; index <= BOT_COUNT; index += 1) {
      const angle = (Math.PI * 2 * index) / (BOT_COUNT + 1);
      this.actors.push({
        id: `bot-${index}`,
        name: `${vi.bots} ${index}`,
        hp: CONFIG.MAX_HP,
        alive: true,
        color: THEME.colors.player[index],
        position: { x: Math.cos(angle) * 360, y: Math.sin(angle) * 360 }
      });
    }
    this.roundNo = 1;
    this.lastResultText = "";
    this.layout();
    this.startPlacement();
  }

  private layout(): void {
    const { width, height } = this.scale;
    this.cameras.main.setBackgroundColor("#2a1a2b");
    this.arenaCenter = { x: width / 2, y: height * 0.52 };
    const logicA = ARENA.shape === "ellipse" ? ARENA.a : ARENA.radius;
    const logicB = ARENA.shape === "ellipse" ? ARENA.b : ARENA.radius;
    this.arenaScalePx = Math.min(width * 0.42 / logicA, height * 0.36 / logicB);
    this.arenaApx = logicA * this.arenaScalePx;
    this.arenaBpx = logicB * this.arenaScalePx;
    this.drawArena();
    this.createOrPositionCrowd();
    this.roundLabel?.setPosition(18, 22);
    this.phaseLabel?.setPosition(width / 2, 62);
    this.timerFill?.setPosition(width / 2, 50);
    this.timerLabel?.setPosition(width / 2, 15);
    this.banner?.setPosition(width / 2, this.arenaCenter.y - this.arenaBpx * 0.56);
    this.positionDockedUi();
    this.chatPanel?.setPosition(width / 2, height - 152);
    this.settingsPanel?.setPosition(width / 2, 118);
    for (const actor of this.actors) {
      this.positionActor(actor);
      this.positionTicket(actor);
    }
    this.updateAimLine();
  }

  private drawArena(): void {
    const graphics = this.arenaGraphics;
    if (!graphics) return;
    const { x, y } = this.arenaCenter;
    const a = this.arenaApx;
    const b = this.arenaBpx;
    graphics.clear();
    const bands = 16;
    for (let index = 0; index < bands; index += 1) {
      const color = Phaser.Display.Color.Interpolate.ColorWithColor(Phaser.Display.Color.ValueToColor(THEME.colors.duskTop), Phaser.Display.Color.ValueToColor(THEME.colors.duskBottom), bands - 1, index).color;
      graphics.fillStyle(color, 1).fillRect(0, (this.scale.height / bands) * index, this.scale.width, this.scale.height / bands + 1);
    }
    graphics.fillStyle(THEME.colors.shadow, 0.38).fillEllipse(x, y, a * 2.28, b * 2.28);
    graphics.lineStyle(Math.max(15, a * 0.2), THEME.colors.wallInner, 1).strokeEllipse(x, y, a * 2.2, b * 2.2);
    graphics.lineStyle(Math.max(8, a * 0.09), THEME.colors.wallTop, 1).strokeEllipse(x, y, a * 2.08, b * 2.08);
    graphics.lineStyle(3, THEME.colors.gold, 0.9).strokeEllipse(x, y, a * 2, b * 2);
    graphics.fillStyle(THEME.colors.floorDark, 1).fillEllipse(x, y, a * 1.94, b * 1.94);
    graphics.fillStyle(THEME.colors.floorLight, 1).fillEllipse(x, y, a * 1.72, b * 1.72);
    graphics.lineStyle(2, THEME.colors.grout, 0.45).strokeEllipse(x, y, a * 1.44, b * 1.44).strokeEllipse(x, y, a * 0.98, b * 0.98).strokeEllipse(x, y, a * 0.5, b * 0.5);
    graphics.fillStyle(THEME.colors.floorLight, 0.12).fillEllipse(x - a * 0.18, y - b * 0.19, a * 1.08, b * 1.08);
    for (let index = 0; index < 12; index += 1) {
      const angle = (Math.PI * 2 * index) / 12;
      const dx = Math.cos(angle);
      const dy = Math.sin(angle);
      graphics.lineStyle(1, THEME.colors.grout, 0.25).lineBetween(x + dx * a * 0.18, y + dy * b * 0.18, x + dx * a * 0.9, y + dy * b * 0.9);
      const archX = x + dx * a * 1.08;
      const archY = y + dy * b * 1.08;
      graphics.fillStyle(THEME.colors.wallHighlight, 0.95).fillCircle(archX, archY, Math.max(4, a * 0.07));
      graphics.fillStyle(THEME.colors.wallInner, 0.9).fillCircle(archX, archY, Math.max(2, a * 0.04));
      if (index % 3 === 0) {
        graphics.fillStyle(THEME.colors.teal, 0.95).fillRect(archX - a * 0.042, archY - b * 0.1, a * 0.084, b * 0.1);
        graphics.fillStyle(THEME.colors.gold, 0.92).fillCircle(x + dx * a * 1.22, y + dy * b * 1.22, Math.max(4, a * 0.04));
      }
    }
    for (let index = 0; index < 4; index += 1) {
      const angle = (Math.PI * 2 * index) / 4 + Math.PI / 4;
      const potX = x + Math.cos(angle) * a * 1.2;
      const potY = y + Math.sin(angle) * b * 1.2;
      graphics.fillStyle(THEME.colors.wallTop, 1).fillCircle(potX, potY, Math.max(5, a * 0.06));
      graphics.fillStyle(THEME.colors.tealDark, 1).fillCircle(potX, potY - a * 0.07, Math.max(6, a * 0.09));
    }
  }

  private createOrPositionCrowd(): void {
    const count = this.quality === "low" || CONFIG.LOW_POWER ? 24 : Math.min(THEME.layout.crowdMobile, CONFIG.MAX_CROWD);
    while (this.crowd.length < count) {
      const marker = this.add.container().setDepth(6);
      marker.add([this.add.ellipse(0, 4, 12, 5, THEME.colors.shadow, 0.28), this.add.circle(0, 0, 5, THEME.colors.player[this.crowd.length % THEME.colors.player.length]), this.add.circle(0, -5, 3.5, THEME.colors.skin)]);
      this.crowd.push({ sprite: marker, home: { x: 0, y: 0 }, phase: this.crowd.length * 0.73 });
    }
    for (const [index, member] of this.crowd.entries()) {
      const row = Math.floor(index / 16);
      const slot = index % 16;
      const angle = (Math.PI * 2 * slot) / 16 + row * 0.13;
      const crowdA = Math.min(this.arenaApx * (1.08 + row * 0.025), this.scale.width / 2 - 9);
      const crowdB = Math.min(this.arenaBpx * (1.08 + row * 0.025), this.arenaCenter.y - 12, this.scale.height - this.arenaCenter.y - 12);
      member.home = { x: this.arenaCenter.x + Math.cos(angle) * crowdA, y: this.arenaCenter.y + Math.sin(angle) * crowdB };
      member.sprite.setPosition(member.home.x, member.home.y).setVisible(index < count);
    }
  }

  private drawActor(actor: Actor): void {
    const radius = this.logicDistanceToScreen(CONFIG.PLAYER_RADIUS);
    const container = this.add.container(0, 0).setDepth(actor.id === PLAYER_ID ? 14 : 12);
    actor.ring = this.add.ellipse(0, radius * 0.64, radius * 2.04, radius * 0.82, THEME.colors.shadow, 0.38).setStrokeStyle(1, THEME.colors.shadow, 0.32) as unknown as Phaser.GameObjects.Arc;
    const body = this.add.ellipse(0, radius * 0.18, radius * 1.55, radius * 1.48, actor.color, 1).setStrokeStyle(2, THEME.colors.paperShade, 0.55);
    const face = this.add.circle(0, -radius * 0.52, radius * 0.82, THEME.colors.skin).setStrokeStyle(1, THEME.colors.paperShade, 0.36);
    const hair = this.add.arc(0, -radius * 0.78, radius * 0.74, 194, 346, false, THEME.colors.hair).setStrokeStyle(Math.max(2, radius * 0.27), THEME.colors.hair);
    const tunic = this.add.rectangle(0, radius * 0.38, radius * 1.2, radius * 0.62, actor.color).setStrokeStyle(1, THEME.colors.paper, 0.55);
    const badge = this.add.circle(0, radius * 0.36, Math.max(4, radius * 0.2), THEME.colors.gold).setStrokeStyle(1, THEME.colors.paperShade, 0.45);
    const badgeNo = this.add.text(0, radius * 0.36, actor.id === PLAYER_ID ? "1" : actor.id.replace("bot-", ""), this.hudStyle(8, "#6d3d35")).setOrigin(0.5);
    const weapon = this.add.rectangle(radius * 0.82, radius * 0.26, radius * 0.86, Math.max(4, radius * 0.24), THEME.colors.gold).setAngle(-24);
    const eyes = this.add.text(0, -radius * 0.5, "• •", this.hudStyle(Math.max(7, radius * 0.52), "#6d3d35")).setOrigin(0.5);
    const label = this.add.text(0, -radius * 1.72, actor.name, this.hudStyle(10, "#fff6dc")).setOrigin(0.5).setVisible(actor.id !== PLAYER_ID);
    actor.hearts = Array.from({ length: CONFIG.MAX_HP }, (_, index) => this.add.circle((index - 1) * 8, -radius * 1.38, 3, THEME.colors.heartFull));
    actor.knockout = this.add.text(0, -radius * 1.9, "×", { color: "#fff6dc", fontFamily: THEME.fontFamily, fontStyle: "700", fontSize: "22px" }).setOrigin(0.5).setVisible(false);
    container.setData("assetSlot", assetManifest.slots["character.body"].width);
    container.add([actor.ring, body, tunic, face, hair, eyes, badge, badgeNo, weapon, label, ...actor.hearts, actor.knockout]);
    actor.sprite = container;
    this.createTicket(actor);
  }

  private createTicket(actor: Actor): void {
    const ticket = this.add.container().setDepth(40);
    const card = this.add.rectangle(0, 0, 82, THEME.layout.ticketHeight, THEME.colors.paper, 1).setStrokeStyle(2, actor.id === PLAYER_ID ? THEME.colors.gold : THEME.colors.paperShade, 0.9).setAngle(actor.id === PLAYER_ID ? -1 : 1);
    const portrait = this.add.circle(-30, -5, 9, actor.color).setStrokeStyle(1, THEME.colors.paperShade, 0.5);
    const badge = this.add.text(-30, -5, actor.id === PLAYER_ID ? "1" : actor.id.replace("bot-", ""), this.hudStyle(9, "#6d3d35")).setOrigin(0.5);
    actor.ticketHearts = Array.from({ length: CONFIG.MAX_HP }, (_, index) => this.add.circle(-10 + index * 10, 10, 3.3, THEME.colors.heartFull));
    ticket.add([card, portrait, badge, ...actor.ticketHearts]);
    actor.ticket = ticket;
  }

  private positionTicket(actor: Actor): void {
    if (!actor.ticket) return;
    const index = this.actors.indexOf(actor);
    const gap = THEME.layout.ticketGap;
    const ticketWidth = Math.min(88, (this.scale.width - 16 - gap * (this.actors.length - 1)) / this.actors.length);
    actor.ticket.setScale(ticketWidth / 82);
    actor.ticket.setPosition(8 + ticketWidth / 2 + index * (ticketWidth + gap), 78);
  }

  private startPlacement(): void {
    this.phase = "placement";
    this.roundEndsAt = this.time.now + CONFIG.PLACEMENT_TIME_MS;
    const playerPosition = this.getPlayer().position;
    this.currentAction = { playerId: PLAYER_ID, position: { ...playerPosition }, angle: this.angleToArenaCenter(playerPosition) };
    this.lastResultText = this.roundNo <= 2 ? vi.dragHint : "";
    this.readyButton?.setVisible(true);
    this.chatPanel?.setVisible(false);
    this.settingsPanel?.setVisible(false);
    this.updateActorSprites();
    this.updateAimLine();
  }

  private resolveAndAnimateRound(): void {
    if (this.phase !== "placement") return;
    this.phase = "reveal";
    this.readyButton?.setVisible(false);
    this.aimGraphics?.clear();
    const actions = this.getRoundActions();
    const result = resolveRound({ players: this.actors, actions, config: CONFIG });
    const hitsByTarget = new Map<string, number>();
    for (const hit of result.hits) hitsByTarget.set(hit.targetId, (hitsByTarget.get(hit.targetId) ?? 0) + 1);
    for (const actor of this.actors) {
      const action = actions.find((candidate) => candidate.playerId === actor.id);
      if (action) actor.position = { ...action.position };
      actor.hp = result.hpAfter[actor.id] ?? actor.hp;
      actor.alive = actor.hp > 0;
    }
    this.lastResultText = result.hits.length ? vi.hit : vi.missed;
    this.updateActorSprites();
    for (const actor of this.actors.filter((actor) => actor.id !== PLAYER_ID && actor.sprite)) {
      actor.sprite?.setScale(0).setAlpha(1);
      this.tweens.add({ targets: actor.sprite, scale: 1, duration: CONFIG.REVEAL_DURATION_MS, ease: "Back.out" });
      this.emitBurst(this.worldToScreen(actor.position), THEME.colors.floorDark, 8);
    }
    this.playSfx(470, 0.04);
    this.time.delayedCall(CONFIG.REVEAL_DURATION_MS + 80, () => {
      this.phase = "throw";
      for (const action of actions) {
        const actor = this.actors.find((candidate) => candidate.id === action.playerId);
        if (actor?.sprite) this.tweens.add({ targets: actor.sprite, scaleX: 1.16, scaleY: 0.82, yoyo: true, duration: 120 });
      }
      this.playSfx(650, 0.05);
        this.animateThrows(actions, result.hits, () => {
        this.phase = "resolve";
        this.pulseHitTargets(hitsByTarget);
        this.time.delayedCall(CONFIG.RESOLVE_PAUSE_MS, () => this.afterRound());
      });
    });
  }

  private getRoundActions(): RoundAction[] {
    const actions: RoundAction[] = this.currentAction ? [this.currentAction] : [];
    for (const actor of this.actors) if (actor.alive && actor.id !== PLAYER_ID) actions.push(this.makeBotAction(actor));
    return actions;
  }

  private makeBotAction(actor: Actor): RoundAction {
    const target = this.chooseBotTarget(actor);
    const position = this.randomPointInArena();
    return { playerId: actor.id, position, angle: Math.atan2(target.position.y - position.y, target.position.x - position.x) + Phaser.Math.FloatBetween(-0.18, 0.18) };
  }

  private chooseBotTarget(actor: Actor): Actor {
    const candidates = this.actors.filter((candidate) => candidate.alive && candidate.id !== actor.id);
    return candidates[Math.floor(Math.random() * candidates.length)] ?? this.getPlayer();
  }

  private animateThrows(actions: RoundAction[], hits: { attackerId: string; targetId: string }[], done: () => void): void {
    let remaining = actions.length;
    if (!remaining) return done();
    for (const action of actions) {
      const start = this.worldToScreen(action.position);
      const direction = { x: Math.cos(action.angle), y: Math.sin(action.angle) };
      const targetId = hits.find((hit) => hit.attackerId === action.playerId)?.targetId;
      const hitTarget = targetId ? this.actors.find((actor) => actor.id === targetId) : undefined;
      const distance = distanceToArenaWall(action.position, direction, ARENA);
      const end = hitTarget
        ? this.worldToScreen(hitTarget.position)
        : this.worldToScreen({ x: action.position.x + direction.x * distance, y: action.position.y + direction.y * distance });
      const projectile = this.add.container(start.x, start.y).setDepth(20);
      const projectileRadius = Math.max(5, this.logicDistanceToScreen(CONFIG.PROJECTILE_RADIUS));
      projectile.add([
        this.add.rectangle(-projectileRadius * 1.4, 0, projectileRadius * 2.8, Math.max(2, projectileRadius * 0.35), THEME.colors.teal, 0.33),
        this.add.circle(0, 0, projectileRadius, THEME.colors.gold),
        this.add.rectangle(0, 0, 16, 6, THEME.colors.wallInner)
      ]);
      projectile.setRotation(action.angle);
      this.projectiles.push(projectile);
      this.tweens.add({
        targets: projectile,
        x: end.x,
        y: end.y,
        rotation: projectile.rotation + Math.PI * 4,
        duration: CONFIG.THROW_DURATION_MS,
        ease: "Quad.easeOut",
        onComplete: () => {
          projectile.destroy();
          remaining -= 1;
          if (!remaining) {
            this.projectiles = [];
            done();
          }
        }
      });
    }
  }

  private pulseHitTargets(hitsByTarget: Map<string, number>): void {
    for (const actor of this.actors) {
      if (!hitsByTarget.has(actor.id) || !actor.sprite) continue;
      const damage = this.add.text(actor.sprite.x, actor.sprite.y - 38, "-1", { color: "#ff7168", fontFamily: THEME.fontFamily, fontSize: "20px", fontStyle: "700" }).setOrigin(0.5).setDepth(28);
      const impact = this.add.text(actor.sprite.x, actor.sprite.y - 58, vi.impact, { color: "#fff6dc", fontFamily: THEME.fontFamily, fontSize: "16px", fontStyle: "700" }).setOrigin(0.5).setDepth(29);
      if (this.quality !== "low") this.cameras.main.shake(THEME.motion.hitMs, 0.004);
      this.tweens.add({ targets: actor.sprite, alpha: 0.35, yoyo: true, repeat: 2, duration: THEME.motion.hitMs / 2 });
      this.tweens.add({ targets: damage, y: damage.y - 28, alpha: 0, duration: THEME.motion.damageFloatMs, onComplete: () => damage.destroy() });
      this.tweens.add({ targets: impact, y: impact.y - 18, scale: 1.35, alpha: 0, duration: THEME.motion.damageFloatMs, onComplete: () => impact.destroy() });
      this.emitBurst({ x: actor.sprite.x, y: actor.sprite.y }, THEME.colors.hit, 12);
      this.playSfx(180, 0.08);
      actor.hearts?.[actor.hp]?.setScale(1);
      if (actor.hearts?.[actor.hp]) this.tweens.add({ targets: actor.hearts[actor.hp], scale: 0, angle: 120, duration: 220 });
      if (!actor.alive) {
        actor.knockout?.setVisible(true);
        const exit = Phaser.Math.Angle.Between(this.arenaCenter.x, this.arenaCenter.y, actor.sprite.x, actor.sprite.y);
        this.tweens.add({ targets: actor.sprite, x: actor.sprite.x + Math.cos(exit) * 100, y: actor.sprite.y + Math.sin(exit) * 100, rotation: Math.PI * 2, alpha: 0, duration: THEME.motion.eliminationMs });
        this.emitBurst({ x: actor.sprite.x, y: actor.sprite.y }, THEME.colors.gold, 16);
        this.tweens.add({ targets: actor.ticket, y: actor.ticket?.y ? actor.ticket.y - 18 : 0, alpha: 0, duration: THEME.motion.eliminationMs });
        this.lastResultText = `${actor.name} ${vi.eliminated}`;
      }
    }
    this.reactCrowd();
    this.updateActorSprites();
  }

  private afterRound(): void {
    const alive = this.actors.filter((actor) => actor.alive);
    if (alive.length <= 1) {
      this.phase = "ended";
      this.lastResultText = alive.length === 1 && alive[0].id === PLAYER_ID ? vi.victory : vi.defeat;
      if (alive[0]?.id === PLAYER_ID) this.emitBurst(this.arenaCenter, THEME.colors.paper, 36);
      this.reactCrowd();
      return;
    }
    this.roundNo += 1;
    this.startPlacement();
  }

  private onPointerDown(pointer: Phaser.Input.Pointer): void {
    if (this.phase === "ended") return this.startMatch();
    if (this.phase !== "placement" || !this.currentAction || !this.isPointerInArena(pointer)) return;
    const logic = this.clampToPlayableArena(this.screenToWorld(pointer.x, pointer.y));
    this.currentAction.position = logic;
    this.currentAction.angle = this.angleToArenaCenter(logic);
    this.getPlayer().position = { ...logic };
    this.updateActorSprites();
    this.updateAimLine();
    this.createRipple(this.worldToScreen(logic));
    this.playSfx(820, 0.025);
  }

  private onPointerMove(pointer: Phaser.Input.Pointer): void {
    if (this.phase !== "placement" || !this.currentAction || !pointer.isDown) return;
    const target = this.screenToWorld(pointer.x, pointer.y);
    const origin = this.worldToScreen(this.currentAction.position);
    if (Math.hypot(pointer.x - origin.x, pointer.y - origin.y) < CONFIG.AIM_DRAG_THRESHOLD_PX) return;
    this.currentAction.angle = Math.atan2(target.y - this.currentAction.position.y, target.x - this.currentAction.position.x);
    this.updateAimLine();
  }

  private onPointerUp(pointer: Phaser.Input.Pointer): void {
    this.onPointerMove(pointer);
  }

  private updateActorSprites(): void {
    for (const actor of this.actors) {
      if (!actor.sprite) this.drawActor(actor);
      this.positionActor(actor);
      const hideOpponent = this.phase === "placement" && actor.id !== PLAYER_ID && !CONFIG.SHOW_LAST_ROUND_GHOSTS;
      actor.sprite?.setVisible(!hideOpponent);
      actor.ring?.setStrokeStyle(2, actor.alive ? actor.color : 0x77706a, actor.alive ? 0.85 : 0.3);
      actor.hearts?.forEach((heart, index) => heart.setFillStyle(index < actor.hp ? THEME.colors.heartFull : THEME.colors.heartEmpty, index < actor.hp ? 1 : 0.65));
      actor.ticketHearts?.forEach((heart, index) => heart.setFillStyle(index < actor.hp ? THEME.colors.heartFull : THEME.colors.heartEmpty, index < actor.hp ? 1 : 0.65));
      actor.ticket?.setAlpha(actor.alive ? 1 : 0.42);
    }
  }

  private positionActor(actor: Actor): void {
    if (!actor.sprite) return;
    const screen = this.worldToScreen(actor.position);
    actor.sprite.setPosition(screen.x, screen.y).setDepth(10 + (screen.y / this.scale.height) * 7 + (actor.id === PLAYER_ID ? 0.2 : 0));
    if (!actor.alive) actor.sprite.setAlpha(0.2);
  }

  private updateAimLine(): void {
    const graphics = this.aimGraphics;
    if (!graphics) return;
    graphics.clear();
    if (this.phase !== "placement" || !this.currentAction) return;
    const direction = { x: Math.cos(this.currentAction.angle), y: Math.sin(this.currentAction.angle) };
    const distance = distanceToArenaWall(this.currentAction.position, direction, ARENA);
    const start = this.worldToScreen(this.currentAction.position);
    const end = this.worldToScreen({ x: this.currentAction.position.x + direction.x * distance, y: this.currentAction.position.y + direction.y * distance });
    const offset = Math.floor(this.time.now / 65) % 2;
    for (let index = offset; index < THEME.layout.aimSegments; index += 2) {
      const from = index / THEME.layout.aimSegments;
      const to = (index + 1) / THEME.layout.aimSegments;
      graphics.lineStyle(2, THEME.colors.teal, 0.95).lineBetween(Phaser.Math.Linear(start.x, end.x, from), Phaser.Math.Linear(start.y, end.y, from), Phaser.Math.Linear(start.x, end.x, to), Phaser.Math.Linear(start.y, end.y, to));
    }
    const pulse = 12 + Math.sin(this.time.now / 100) * 2;
    graphics.fillStyle(THEME.colors.teal, 1).fillTriangle(end.x, end.y, end.x - direction.x * pulse - direction.y * 6, end.y - direction.y * pulse + direction.x * 6, end.x - direction.x * pulse + direction.y * 6, end.y - direction.y * pulse - direction.x * 6);
  }

  private updateHud(): void {
    const alive = this.actors.filter((actor) => actor.alive).length;
    const remainingMs = Math.max(0, this.roundEndsAt - this.time.now);
    const ratio = this.phase === "placement" ? remainingMs / CONFIG.PLACEMENT_TIME_MS : 0;
    const lowTime = remainingMs <= CONFIG.LOW_TIME_THRESHOLD_MS;
    this.roundLabel?.setText(`${vi.round} ${this.roundNo}`);
    this.aliveLabel?.setText(`${vi.alive} ${alive}/${this.actors.length}`);
    this.phaseLabel?.setText(this.phase === "placement" ? vi.phasePlacement : this.phase === "reveal" ? vi.phaseReveal : this.phase === "throw" ? vi.phaseThrow : this.phase === "resolve" ? vi.phaseResolve : "");
    this.timerFill?.setDisplaySize(Math.max(0, (this.scale.width - 36) * ratio), 6).setFillStyle(lowTime ? THEME.colors.heartFull : THEME.colors.gold);
    this.timerLabel?.setText(this.phase === "placement" ? `${Math.ceil(remainingMs / 1000)}` : "").setColor(lowTime ? "#ff9a90" : THEME.colors.text);
    this.banner?.setText(this.phase === "ended" ? `${this.lastResultText}\n${vi.newMatch}` : this.lastResultText);
    const player = this.getPlayer();
    this.playerHealth?.forEach((heart, index) => heart.setFillStyle(index < player.hp ? THEME.colors.heartFull : THEME.colors.heartEmpty, index < player.hp ? 1 : 0.65));
  }

  private createButton(label: string, width: number, height: number, onClick: () => void, fontSize: number): UiButton {
    const container = this.add.container();
    const backdrop = this.add.circle(0, 0, Math.max(width, height) / 2, THEME.colors.paper, 0.98).setStrokeStyle(2, THEME.colors.paperShade, 0.9).setDisplaySize(width, height).setInteractive({ useHandCursor: true });
    const text = this.add.text(0, 0, label, { color: "#6d3d35", fontFamily: THEME.fontFamily, fontSize: `${fontSize}px`, fontStyle: "700" }).setOrigin(0.5);
    backdrop.on("pointerdown", () => onClick());
    container.add([backdrop, text]);
    return Object.assign(container, { label: text });
  }

  private createPanel(text: string): Phaser.GameObjects.Container {
    const panel = this.add.container().setDepth(35);
    panel.add([this.add.rectangle(0, 0, 260, 42, THEME.colors.paper, 0.98).setStrokeStyle(2, THEME.colors.paperShade, 0.9), this.add.text(0, 0, text, this.hudStyle(12, "#6d3d35")).setOrigin(0.5)]);
    return panel;
  }

  private createSettingsPanel(): Phaser.GameObjects.Container {
    const panel = this.add.container().setDepth(35);
    const background = this.add.rectangle(0, 0, 260, 76, THEME.colors.paper, 0.98).setStrokeStyle(2, THEME.colors.paperShade, 0.9);
    const audioHit = this.add.rectangle(0, -19, 244, 30, THEME.colors.paper, 0).setInteractive({ useHandCursor: true });
    const qualityHit = this.add.rectangle(0, 20, 244, 30, THEME.colors.paper, 0).setInteractive({ useHandCursor: true });
    this.settingsText = this.add.text(0, -19, "", this.hudStyle(12, "#6d3d35")).setOrigin(0.5);
    this.qualityText = this.add.text(0, 20, "", this.hudStyle(12, "#6d3d35")).setOrigin(0.5);
    audioHit.on("pointerdown", () => {
      this.sfxEnabled = !this.sfxEnabled;
      this.updateSettingsLabels();
      if (this.sfxEnabled) this.playSfx(720, 0.04);
    });
    qualityHit.on("pointerdown", () => this.setQuality(this.quality === "high" ? "medium" : this.quality === "medium" ? "low" : "high"));
    panel.add([background, audioHit, qualityHit, this.settingsText, this.qualityText]);
    this.updateSettingsLabels();
    return panel;
  }

  private togglePanel(panel: "chat" | "settings"): void {
    const target = panel === "chat" ? this.chatPanel : this.settingsPanel;
    const other = panel === "chat" ? this.settingsPanel : this.chatPanel;
    other?.setVisible(false);
    target?.setVisible(!target.visible);
  }

  private updateAmbientMotion(time: number): void {
    if (this.phase === "placement") this.updateAimLine();
    if (CONFIG.LOW_POWER || this.quality === "low") return;
    for (const member of this.crowd) {
      if (!member.sprite.visible) continue;
      member.sprite.setY(member.home.y + Math.sin(time / THEME.motion.crowdBobMs + member.phase) * 2);
    }
  }

  private observeFrameBudget(delta: number): void {
    if (CONFIG.LOW_POWER || this.quality === "low") return;
    this.slowFrames = delta > 34 ? this.slowFrames + 1 : Math.max(0, this.slowFrames - 2);
    if (this.slowFrames >= 150) this.setQuality(this.quality === "high" ? "medium" : "low");
  }

  private setQuality(quality: Quality): void {
    if (this.quality === quality) return;
    this.quality = quality;
    this.slowFrames = 0;
    this.createOrPositionCrowd();
    this.updateSettingsLabels();
  }

  private updateSettingsLabels(): void {
    const qualityLabel = this.quality === "high" ? vi.qualityHigh : this.quality === "medium" ? vi.qualityMedium : vi.qualityLow;
    this.settingsText?.setText(this.sfxEnabled ? vi.settingsSfxOn : vi.settingsSfxOff);
    this.qualityText?.setText(`${vi.settingsQuality}: ${qualityLabel}`);
  }

  private reactCrowd(): void {
    if (CONFIG.LOW_POWER || this.quality === "low") return;
    for (const member of this.crowd) {
      this.tweens.add({ targets: member.sprite, scaleX: 1.13, scaleY: 1.13, yoyo: true, duration: 110, delay: (member.phase * 28) % 120 });
    }
  }

  private createRipple(point: Vector2): void {
    if (this.quality === "low") return;
    const ripple = this.add.circle(point.x, point.y, 8, THEME.colors.teal, 0).setStrokeStyle(2, THEME.colors.teal, 0.86).setDepth(17);
    this.tweens.add({ targets: ripple, scale: 2.4, alpha: 0, duration: 320, onComplete: () => ripple.destroy() });
  }

  private emitBurst(point: Vector2, color: number, requestedCount: number): void {
    const allowed = Math.max(0, CONFIG.MAX_PARTICLES - this.particleCount);
    const count = Math.min(requestedCount, allowed, this.quality === "low" ? 5 : THEME.layout.particlesPerBurst);
    for (let index = 0; index < count; index += 1) {
      const particle = this.add.circle(point.x, point.y, 2 + (index % 3), color, 0.95).setDepth(27);
      const angle = (Math.PI * 2 * index) / count + Phaser.Math.FloatBetween(-0.2, 0.2);
      const distance = Phaser.Math.Between(16, 42);
      this.particleCount += 1;
      this.tweens.add({
        targets: particle,
        x: point.x + Math.cos(angle) * distance,
        y: point.y + Math.sin(angle) * distance,
        alpha: 0,
        scale: 0.4,
        duration: THEME.motion.particleMs,
        onComplete: () => {
          this.particleCount -= 1;
          particle.destroy();
        }
      });
    }
  }

  private playSfx(frequency: number, durationSeconds: number): void {
    if (!this.sfxEnabled) return;
    const AudioContextClass = window.AudioContext;
    if (!AudioContextClass) return;
    try {
      const context = new AudioContextClass();
      const oscillator = context.createOscillator();
      const gain = context.createGain();
      oscillator.frequency.value = frequency;
      gain.gain.setValueAtTime(0.035, context.currentTime);
      gain.gain.exponentialRampToValueAtTime(0.001, context.currentTime + durationSeconds);
      oscillator.connect(gain).connect(context.destination);
      oscillator.start();
      oscillator.stop(context.currentTime + durationSeconds);
      oscillator.addEventListener("ended", () => context.close());
    } catch {
      // Browsers may require a prior user gesture before audio can start.
    }
  }

  private positionDockedUi(): void {
    for (const child of this.children.list) {
      if (!(child instanceof Phaser.GameObjects.Container)) continue;
      if (child.getData("dock") === "top-right") child.setPosition(this.scale.width - 28, 38);
      if (child.getData("dock") === "bottom-left") child.setPosition(38, this.scale.height - 42);
    }
    this.readyButton?.setPosition(this.scale.width / 2, this.scale.height - 42);
    this.weaponHud?.setPosition(this.scale.width - 40, this.scale.height - 42);
    this.playerHealth?.forEach((heart, index) => heart.setPosition(this.scale.width - 66 + index * 13, this.scale.height - 42));
  }

  private hudStyle(fontSize: number, color: string = THEME.colors.text): Phaser.Types.GameObjects.Text.TextStyle {
    return { color, fontFamily: THEME.fontFamily, fontSize: `${fontSize}px`, fontStyle: "700" };
  }

  private getPlayer(): Actor {
    const player = this.actors.find((actor) => actor.id === PLAYER_ID);
    if (!player) throw new Error("Player actor is missing");
    return player;
  }

  private randomPointInArena(): Vector2 {
    const radius = Math.sqrt(Math.random());
    const angle = Math.random() * Math.PI * 2;
    const a = ARENA.shape === "ellipse" ? ARENA.a - CONFIG.PLAYER_RADIUS : ARENA.radius - CONFIG.PLAYER_RADIUS;
    const b = ARENA.shape === "ellipse" ? ARENA.b - CONFIG.PLAYER_RADIUS : ARENA.radius - CONFIG.PLAYER_RADIUS;
    return { x: Math.cos(angle) * radius * a, y: Math.sin(angle) * radius * b };
  }

  private isPointerInArena(pointer: Phaser.Input.Pointer): boolean {
    const x = (pointer.x - this.arenaCenter.x) / this.arenaApx;
    const y = (pointer.y - this.arenaCenter.y) / this.arenaBpx;
    return x * x + y * y <= 1;
  }

  private clampToPlayableArena(point: Vector2): Vector2 {
    return clampToArena(point, ARENA, CONFIG.PLAYER_RADIUS);
  }

  private worldToScreen(point: Vector2): Vector2 {
    return { x: this.arenaCenter.x + point.x * this.arenaScalePx, y: this.arenaCenter.y + point.y * this.arenaScalePx * CONFIG.VIEW_TILT_Y };
  }

  private screenToWorld(x: number, y: number): Vector2 {
    return { x: (x - this.arenaCenter.x) / this.arenaScalePx, y: (y - this.arenaCenter.y) / (this.arenaScalePx * CONFIG.VIEW_TILT_Y) };
  }

  private logicDistanceToScreen(distance: number): number {
    return distance * this.arenaScalePx;
  }

  private angleToArenaCenter(point: Vector2): number {
    return Math.atan2(-point.y, -point.x);
  }
}

new Phaser.Game({
  type: Phaser.AUTO,
  parent: "app",
  backgroundColor: "#1a1715",
  scale: { mode: Phaser.Scale.RESIZE, parent: "app", width: "100%", height: "100%" },
  render: { antialias: true, pixelArt: false, roundPixels: true, powerPreference: "high-performance" },
  scene: HitMeScene
});
