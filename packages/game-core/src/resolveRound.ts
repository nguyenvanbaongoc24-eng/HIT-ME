import type { ArenaBounds, Hit, PlayerState, ResolveRoundInput, ResolveRoundResult, RoundAction, Vector2 } from "./types.js";

const EPSILON = 1e-9;

type Candidate = { targetId: string; distanceAlongRay: number };

export function resolveRound(input: ResolveRoundInput): ResolveRoundResult {
  const { players, actions, config } = input;
  const arena = input.arena ?? (input.arenaRadius === undefined ? arenaFromConfig(config) : { shape: "circle", radius: input.arenaRadius });
  const alivePlayers = players.filter((player) => player.alive && player.hp > 0);
  const aliveById = new Map(alivePlayers.map((player) => [player.id, player]));
  const actionByPlayerId = getFirstActionByPlayerId(actions);
  const hitReach = config.PLAYER_RADIUS + config.PROJECTILE_RADIUS;
  const hits: Hit[] = [];

  for (const attacker of alivePlayers) {
    const action = actionByPlayerId.get(attacker.id);
    if (!action || !isFiniteAction(action) || !isInsideArena(action.position, arena)) continue;

    const direction = normalizeDirection(action.angle);
    const wallDistance = distanceToArenaWall(action.position, direction, arena);
    const candidates: Candidate[] = [];

    for (const target of alivePlayers) {
      if (target.id === attacker.id) continue;
      const targetAction = actionByPlayerId.get(target.id);
      const targetPosition = targetAction && isFiniteAction(targetAction) && isInsideArena(targetAction.position, arena) ? targetAction.position : target.position;
      const offset = subtract(targetPosition, action.position);
      const distanceAlongRay = dot(offset, direction);
      if (distanceAlongRay <= EPSILON || distanceAlongRay > wallDistance + EPSILON) continue;
      if (Math.abs(cross(offset, direction)) <= hitReach + EPSILON) candidates.push({ targetId: target.id, distanceAlongRay });
    }

    candidates.sort((left, right) => left.distanceAlongRay - right.distanceAlongRay);
    for (const candidate of candidates.slice(0, config.MAX_PIERCE_TARGETS)) hits.push({ attackerId: attacker.id, targetId: candidate.targetId });
  }

  const damageByPlayerId = new Map<string, number>();
  for (const hit of hits) {
    if (aliveById.has(hit.targetId)) damageByPlayerId.set(hit.targetId, (damageByPlayerId.get(hit.targetId) ?? 0) + 1);
  }

  const hpAfter: Record<string, number> = {};
  const eliminated: string[] = [];
  for (const player of players) {
    const nextHp = Math.max(0, player.hp - (damageByPlayerId.get(player.id) ?? 0));
    hpAfter[player.id] = nextHp;
    if (player.alive && player.hp > 0 && nextHp <= 0) eliminated.push(player.id);
  }
  return { hits, hpAfter, eliminated };
}

export function distanceToArenaWall(origin: Vector2, direction: Vector2, arena: ArenaBounds | number): number {
  const bounds = typeof arena === "number" ? { shape: "circle" as const, radius: arena } : arena;
  if (bounds.shape === "circle") {
    const b = 2 * dot(origin, direction);
    const c = dot(origin, origin) - bounds.radius * bounds.radius;
    const discriminant = b * b - 4 * c;
    return discriminant < 0 ? 0 : Math.max(0, (-b + Math.sqrt(discriminant)) / 2);
  }
  const aTerm = direction.x * direction.x / (bounds.a * bounds.a) + direction.y * direction.y / (bounds.b * bounds.b);
  const bTerm = 2 * (origin.x * direction.x / (bounds.a * bounds.a) + origin.y * direction.y / (bounds.b * bounds.b));
  const cTerm = origin.x * origin.x / (bounds.a * bounds.a) + origin.y * origin.y / (bounds.b * bounds.b) - 1;
  const discriminant = bTerm * bTerm - 4 * aTerm * cTerm;
  return discriminant < 0 || aTerm <= EPSILON ? 0 : Math.max(0, (-bTerm + Math.sqrt(discriminant)) / (2 * aTerm));
}

export function clampToArena(position: Vector2, arena: ArenaBounds, playerRadius = 0): Vector2 {
  if (arena.shape === "circle") {
    const maxRadius = Math.max(0, arena.radius - playerRadius);
    const length = Math.hypot(position.x, position.y);
    return length <= maxRadius || length <= EPSILON ? position : { x: position.x * maxRadius / length, y: position.y * maxRadius / length };
  }
  const a = Math.max(EPSILON, arena.a - playerRadius);
  const b = Math.max(EPSILON, arena.b - playerRadius);
  const normalized = position.x * position.x / (a * a) + position.y * position.y / (b * b);
  if (normalized <= 1 || normalized <= EPSILON) return position;
  const scale = 1 / Math.sqrt(normalized);
  return { x: position.x * scale, y: position.y * scale };
}

export function shrinkArena(arena: ArenaBounds, factor: number): ArenaBounds {
  const clampedFactor = Math.max(0, factor);
  return arena.shape === "circle"
    ? { shape: "circle", radius: arena.radius * clampedFactor }
    : { shape: "ellipse", a: arena.a * clampedFactor, b: arena.b * clampedFactor };
}

function arenaFromConfig(config: ResolveRoundInput["config"]): ArenaBounds {
  return config.ARENA_SHAPE === "ellipse" ? { shape: "ellipse", a: config.ARENA_A, b: config.ARENA_B } : { shape: "circle", radius: config.ARENA_RADIUS };
}

function getFirstActionByPlayerId(actions: readonly RoundAction[]): Map<string, RoundAction> {
  const result = new Map<string, RoundAction>();
  for (const action of actions) if (!result.has(action.playerId)) result.set(action.playerId, action);
  return result;
}

function isFiniteAction(action: RoundAction): boolean {
  return Number.isFinite(action.angle) && Number.isFinite(action.position.x) && Number.isFinite(action.position.y);
}

function isInsideArena(position: Vector2, arena: ArenaBounds): boolean {
  return arena.shape === "circle"
    ? dot(position, position) <= arena.radius * arena.radius + EPSILON
    : position.x * position.x / (arena.a * arena.a) + position.y * position.y / (arena.b * arena.b) <= 1 + EPSILON;
}

function normalizeDirection(angle: number): Vector2 { return { x: Math.cos(angle), y: Math.sin(angle) }; }
function subtract(left: Vector2, right: Vector2): Vector2 { return { x: left.x - right.x, y: left.y - right.y }; }
function dot(left: Vector2, right: Vector2): number { return left.x * right.x + left.y * right.y; }
function cross(left: Vector2, right: Vector2): number { return left.x * right.y - left.y * right.x; }
