using HitMe.Core;
using HitMe.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace HitMe.UI
{
    public sealed partial class BattleView : MonoBehaviour
    {
        public FoundationConfig Config { get; private set; }
        public PlacementSession Session { get; private set; }
        public ArenaViewport Viewport { get; private set; }
        public int VisibleActorCount { get; private set; }
        public Rect? SafeAreaOverride { get; set; }
        public Rect CurrentSafeArea => SafeAreaOverride ?? WebMobileBridge.SafeArea;
        public bool CanPlace => online ? NetworkSession.Instance.Connected && NetworkSession.Instance.Room?.phase=="Placement" && NetworkSession.Instance.Self!=null && NetworkSession.Instance.Self.hp>0 && !NetworkSession.Instance.Self.locked : Match != null ? Match.Phase == MatchPhase.Placement && Match.ViewFor("player")[0].Hp > 0 && Match.ViewFor("player")[0].Stage != InputStage.Locked : Session.Phase == BattlePhase.Placement && !Session.Locked;
        public float DragThreshold => (float)Config.aimDragPixels * canvas.scaleFactor;
        Canvas canvas;
        RectTransform root, hud, floor, aim;
        RectTransform[] actors = new RectTransform[3];
        RectTransform[] portraitItems = new RectTransform[9];
        Vector2[] portraitPositions = new Vector2[9];
        readonly Point[] demoPositions = { new Point(0, -550), new Point(-410, 520), new Point(420, 70) };
        Text timer, status, readyText;
        Button ready;
        Font font;
        Vector2 previousSize;
        Rect previousSafeArea;
        float revealAt;
        int lastTimerSecond = -1;
        bool initialized;
        string language = "vi";
        GameObject panel;
        Color ink = new Color(.20f, .12f, .10f);
        static Color C(float r, float g, float b) => new Color(r / 255, g / 255, b / 255);

        public void Initialize()
        {
            if (initialized) return;
            initialized = true;
            language=Strings.Language;
            Config = JsonUtility.FromJson<FoundationConfig>(Resources.Load<TextAsset>("foundation-config").text);
            WebMobileBridge.Ensure(); Config.Validate(); Session = new PlacementSession(Config); Strings.Load(language);
            FoundationFonts.Validate(); font = FoundationFonts.Text;
            GameObject go = new GameObject("BattleCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.transform.SetParent(transform, false);
            canvas = go.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(Config.referenceWidth, Config.referenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight; scaler.matchWidthOrHeight = 0;
            root = go.GetComponent<RectTransform>();
            Build();
        }
        void Start() { Initialize(); }
        RectTransform Rect(string name, Transform parent, Vector2 size, Vector2 pos)
        {
            var r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            r.SetParent(parent, false); r.anchorMin = r.anchorMax = new Vector2(.5f, .5f); r.sizeDelta = size; r.anchoredPosition = pos; return r;
        }
        void Stretch(RectTransform r) { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero; }
        Image Box(string name, Transform parent, Vector2 size, Vector2 pos, Color color)
        {
            var r = Rect(name, parent, size, pos); var image = r.gameObject.AddComponent<Image>(); image.color = color; image.raycastTarget = false; return image;
        }
        EllipseGraphic Ellipse(string name, Transform parent, Vector2 size, Vector2 pos, Color color, float inner = 0)
        {
            var r = Rect(name, parent, size, pos); var g = r.gameObject.AddComponent<EllipseGraphic>(); g.color = color; g.innerRatio = inner; g.raycastTarget = false; return g;
        }
        MaskableGraphic ArenaSurface(string name, Transform parent, Vector2 size, Vector2 pos, Color color, float inner=0)
        {
            if(Config.arenaShape!="roundedRectangle") return Ellipse(name,parent,size,pos,color,inner);
            if(parent==root)pos+=ArenaOffset;
            var r=Rect(name,parent,size,pos); var g=r.gameObject.AddComponent<RoundedRectangleGraphic>();
            g.cornerRadius=(float)(Config.cornerRadius*Viewport.Scale)+Mathf.Max(0,(size.x-(float)(Config.arenaWidth*Viewport.Scale))/2);
            g.color=color;g.innerRatio=inner;g.raycastTarget=false;return g;
        }
        Text Label(string name, Transform parent, string value, Vector2 size, Vector2 pos, int fontSize = 14)
        {
            var r = Rect(name, parent, size, pos); var t = r.gameObject.AddComponent<HitMeText>();
            t.font = font; t.text = value; t.fontSize = fontSize; t.fontStyle = FontStyle.Bold; t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white; t.raycastTarget = false; t.horizontalOverflow = HorizontalWrapMode.Wrap;
            var outline = r.gameObject.AddComponent<Outline>(); outline.effectColor = ink; outline.effectDistance = new Vector2(1, -1);
            return t;
        }
        Button Button(string name, Transform parent, string text, Vector2 size, Vector2 pos, UnityEngine.Events.UnityAction click, Color color)
        {
            var widget=HitMeWidgetFactory.Create(name=="Ready"?"HitMeButtonPrimary":"HitMeButtonTertiary",parent);widget.name=name;var image=widget.background;image.color=color;var r=widget.GetComponent<RectTransform>();r.sizeDelta=size;r.anchoredPosition=pos;widget.title.gameObject.SetActive(false);widget.detail.gameObject.SetActive(false);widget.icon.gameObject.SetActive(false);var b=widget.action;b.onClick.AddListener(click);
            Label("Label", image.transform, text, size-new Vector2(6,4),Vector2.zero,14);
            return b;
        }
        void Build()
        {
            Canvas.ForceUpdateCanvases();
            float w = root.rect.width, h = root.rect.height;
            if (w <= 0 || h <= 0) { w = Config.referenceWidth; h = Config.referenceHeight; }
            Viewport = new ArenaViewport(w, h, Config,(Screen.height-CurrentSafeArea.yMax)/canvas.scaleFactor,CurrentSafeArea.yMin/canvas.scaleFactor);
            var arenaArt=ArenaMaps.LoadSelected();
            bool hasBackdrop=arenaArt!=null && arenaArt.backdrop!=null;
            var bg = Box("StandsPlaceholder", root, new Vector2(w, h), Vector2.zero, C(107, 66, 57)); Stretch(bg.rectTransform);
            if(hasBackdrop) {bg.sprite=arenaArt.backdrop;bg.color=Color.white;}
            for (int i = 0; !hasBackdrop && i < 4; i++)
                ArenaSurface("StandTierPlaceholder", root, new Vector2((float)Viewport.OuterWidth + 38 + i * 70, (float)Viewport.OuterHeight + 40 + i * 80), Vector2.zero, i % 2 == 0 ? C(139, 86, 66) : C(179, 114, 77), .965f);
            ArenaSurface("WallShadow", root, new Vector2((float)Viewport.OuterWidth + 8, (float)Viewport.OuterHeight + 10), new Vector2(0, -4), ink);
            ArenaSurface("WallPlaceholder", root, new Vector2((float)Viewport.OuterWidth, (float)Viewport.OuterHeight), Vector2.zero, C(223, 162, 79));
            ArenaSurface("WallInnerPlaceholder", root, new Vector2((float)Viewport.OuterWidth - 9, (float)Viewport.OuterHeight - 9), Vector2.zero, C(146, 84, 48));
            var arena = ArenaSurface("ArenaSandPlaceholder", root, new Vector2((float)(2 * Config.ArenaGeometry.Bounds.MaxX * Viewport.Scale), (float)(2 * Config.ArenaGeometry.Bounds.MaxY * Viewport.Scale)), Vector2.zero, C(238, 192, 121));
            floor = arena.rectTransform; arena.raycastTarget = true;
            arena.gameObject.AddComponent<ArenaInput>().view = this;
            if(arenaArt!=null && arenaArt.sand!=null)
            {
                arena.gameObject.AddComponent<Mask>().showMaskGraphic=false;
                var sand=Box("ArenaSandSprite",floor,floor.sizeDelta,Vector2.zero,Color.white);Stretch(sand.rectTransform);sand.sprite=arenaArt.sand;
            }
            ArenaSurface("FloorMark", floor, floor.sizeDelta * .94f, Vector2.zero, C(215, 157, 89), .993f);
            Label("ArenaLabel", floor, hasBackdrop?"":ArenaMaps.DisplayName+"\n"+Strings.Get("arenaPlaceholder"), new Vector2(200, 48), new Vector2(0, -110), 14).color = C(160, 100, 55);
            CharacterManifest manifest = CharacterManifest.Load();
            var catalog = CharacterCatalog.Load();
            var actorLayer = Rect("ActorLayer", root, Vector2.zero, Vector2.zero); Stretch(actorLayer);
            for (int i = 0; i < actors.Length; i++)
            {
                var actor = Rect("ActorFeet" + i, actorLayer, Vector2.zero, ToCanvas(demoPositions[i % 3])); actors[i] = actor;
                float radius = (float)(Config.playerRadius * Viewport.Scale);
                Ellipse("FootShadow", actor, new Vector2(radius * 2, radius), Vector2.zero, new Color(0,0,0,.18f));
                Ellipse("FeetHitbox", actor, new Vector2(radius * 2, radius * 2), Vector2.zero, C(52, 61, 49), .78f);
                Color tint = i == 0 ? C(43, 157, 147) : (i == 1 ? C(218, 109, 76) : C(121, 105, 182));
                int visualSlot=online&&onlinePlayers!=null?onlinePlayers[i].avatar=="Char02_BotMale"?1:onlinePlayers[i].avatar=="Char03_BotFemale"?2:0:i;
                var definition=!online&&i==0?CosmeticPreview.Definition:catalog!=null?catalog.ForSlot(visualSlot):null;
                Image body;
                if(definition!=null && definition.visualPrefab!=null)
                {
                    body=Instantiate(definition.visualPrefab,actor,false).GetComponent<Image>();body.name="CharacterSpritePlaceholder";
                    body.rectTransform.pivot=new Vector2(.5f,.5f);body.rectTransform.sizeDelta=new Vector2(38,58);body.rectTransform.anchoredPosition=new Vector2(0,30);body.color=tint;
                }
                else body=Box("CharacterSpritePlaceholder",actor,new Vector2(38,58),new Vector2(0,30),tint);
                var presentation=body.GetComponent<CharacterPresentation>();if(presentation==null)presentation=body.gameObject.AddComponent<CharacterPresentation>();
                presentation.Initialize(manifest,i%2==0 && i>0?"female-placeholder":"male-placeholder",definition);
                if(!online&&i==0)presentation.Visual.SetCosmeticWeapon(CosmeticPreview.WeaponDefinition);
                Label("MissingSpriteLabel",body.transform,"PH\n"+(i==0?"P":"B"+i),new Vector2(36,48),Vector2.zero,12);
                presentation.Present(CharacterPose.Idle,tint);
                if(presentation.Visual.HasSprite)presentation.Visual.Fit(presentation.Visual.DesiredHeight);
                string name = i == 0 ? Strings.Get("you") : Strings.Get("bot") + " " + i;
                var nameHealth = Rect("NameHealth", actor, new Vector2(126, 24), new Vector2(0, presentation.Visual.HasSprite?presentation.Visual.DesiredHeight+18:72));
                Label("Name", nameHealth, name, new Vector2(70, 24), new Vector2(-24, 0), 12);
                var health = Label("HealthSymbols", nameHealth, "♥♥♥", new Vector2(40, 24), new Vector2(34, 0), 12);
                health.font = FoundationFonts.Symbols;
                health.gameObject.AddComponent<HitMe.Visuals.FeedbackPulse>();
            }
            aim = Box("AimLine", root, new Vector2(1, 2), Vector2.zero, C(18, 109, 103)).rectTransform; aim.gameObject.SetActive(false);
            hud = Rect("SafeAreaHUD", root, Vector2.zero, Vector2.zero); Stretch(hud); UpdateSafeArea();
            float top = hud.rect.height / 2 - 23, bottom = -hud.rect.height / 2 + 34;
            float safeW = hud.rect.width;
            Label("Round", hud, Strings.Get("round") + " 1", new Vector2(100, 24), new Vector2(-safeW / 2 + 59, top), 17);
            timer = Label("Timer", hud, "5", new Vector2(70, 28), new Vector2(0, top), 22);
            BuildKitHUD(safeW,top);hud.Find("Round").GetComponent<Text>().enabled=false;timer.enabled=false;
            Button("Settings", hud, Strings.Get("settings"), new Vector2(84, 28), new Vector2(safeW / 2 - 50, top), Settings, C(47, 102, 103));
            for (int i = 0; i < actors.Length; i++)
            {
                float x = (i - (actors.Length - 1) / 2f) * Mathf.Min(66, (safeW - 24) / actors.Length);
                portraitItems[i * 3] = Ellipse("PortraitPlaceholder" + i, hud, new Vector2(Match == null ? 30 : 24, Match == null ? 30 : 24), new Vector2(x, top - (Match == null ? 27 : 27)), i == 0 ? C(43, 157, 147) : C(190, 100, 89)).rectTransform;
                var portraitDefinition=!online&&i==0?CosmeticPreview.Definition:catalog!=null?catalog.ForSlot(online&&onlinePlayers!=null?onlinePlayers[i].avatar=="Char02_BotMale"?1:onlinePlayers[i].avatar=="Char03_BotFemale"?2:0:i):null;
                if(portraitDefinition!=null && portraitDefinition.Frame(HitMe.Characters.VisualState.Idle,0)!=null)
                {
                    var portrait=Box("PortraitSprite",portraitItems[i*3],portraitItems[i*3].sizeDelta,Vector2.zero,Color.white);
                    portrait.sprite=portraitDefinition.portrait!=null?portraitDefinition.portrait:portraitDefinition.Frame(HitMe.Characters.VisualState.Idle,0);portrait.preserveAspect=true;
                }
                portraitItems[i * 3 + 1] = Label("PortraitNumber", hud, portraitDefinition!=null && portraitDefinition.Frame(HitMe.Characters.VisualState.Idle,0)!=null?"":(i + 1).ToString(), new Vector2(30, Match == null ? 28 : 24), new Vector2(x, top - (Match == null ? 27 : 27)), 13).rectTransform;
                var hearts = Label("Hearts", hud, "♥♥♥", new Vector2(60, Match == null ? 24 : 18), new Vector2(x, top - (Match == null ? 49 : 49)), Match == null ? 12 : 10); hearts.color = C(255, 165, 139); hearts.font = FoundationFonts.Symbols;
                portraitItems[i * 3 + 2] = hearts.rectTransform;
                for (int k = 0; k < 3; k++) portraitPositions[i * 3 + k] = portraitItems[i * 3 + k].anchoredPosition;
            }
            Button("Chat", hud, Strings.Get("chat"), new Vector2(62, 46), new Vector2(-safeW / 2 + 40, bottom), OpenChat, C(48, 104, 107));
            ready = Button("Ready", hud, Strings.Get("ready"), new Vector2(164, 48), new Vector2(0, bottom), OnReady, C(247, 186, 58));
            readyText = ready.GetComponentInChildren<Text>(); readyText.color=ink;
            var equipped=online?OnlineWeapon(NetworkSession.Instance.Self?.weapon):CosmeticPreview.WeaponDefinition;
            var weaponButton=Button("Weapon",hud,Strings.Get("weapon"),new Vector2(70,46),new Vector2(safeW/2-44,bottom),()=>Message(equipped!=null?equipped.displayName+"\n"+Strings.Get("cosmeticWeapon"):Strings.Get("weaponMock")),C(159,95,56));
            if(equipped!=null && equipped.heldSprite!=null){weaponButton.GetComponentInChildren<Text>().text="";var icon=Box("EquippedWeaponIcon",weaponButton.transform,new Vector2(34,34),Vector2.zero,Color.white);icon.sprite=equipped.icon!=null?equipped.icon:equipped.heldSprite;icon.preserveAspect=true;}

            status = Label("Status", hud, Strings.Get("preview"), new Vector2(safeW - 20, 34), new Vector2(0, bottom + 49), 12);
            lastTimerSecond = -1;
            SortActorsByFeet();
            VisibleActorCount = actors.Length; previousSize = root.rect.size; previousSafeArea = CurrentSafeArea;
            BuildMotion();
        }
        void UpdateSafeArea()
        {
            Rect safe = CurrentSafeArea;
            hud.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            hud.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            hud.offsetMin = hud.offsetMax = Vector2.zero;
            Canvas.ForceUpdateCanvases();
        }
        Vector2 ArenaOffset=>new Vector2(0,root.rect.height/2-(float)Viewport.CenterY);
        Vector2 ToCanvas(Point p) => new Vector2((float)(p.X * Viewport.Scale), (float)(p.Y * Viewport.Scale))+ArenaOffset;
        Point FromPointer(Vector2 screen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, screen, null, out Vector2 p);
            p-=ArenaOffset;return new Point(p.x / Viewport.Scale, p.y / Viewport.Scale);
        }
        public void Place(Vector2 screen)
        {
            if (online) { OnlinePlace(screen); return; }
            if (Match != null) { MatchPlace(screen); return; }
            if (Session.Place(FromPointer(screen), Time.unscaledTimeAsDouble)) { actors[0].gameObject.SetActive(true); actors[0].anchoredPosition = ToCanvas(Session.Position); aim.gameObject.SetActive(false); AdaptHudToPlayer(); }
        }
        void AdaptHudToPlayer()
        {
            // Keep the logical foot position unchanged. Adapt placeholder presentation near the HUD.
            float footY = hud.InverseTransformPoint(actors[0].position).y;
            bool upper = footY + 84 > hud.rect.yMax - 104;
            float top = hud.rect.yMax - 23;
            for (int i = 0; i < portraitItems.Length; i++)
            {
                Vector2 p = portraitPositions[i];
                if (upper && Match == null) p.y = hud.rect.yMin + 150 - (i % 3 == 2 ? 22 : 0);
                portraitItems[i].anchoredPosition = p;
            }
            var body = actors[0].Find("CharacterSpritePlaceholder").GetComponent<RectTransform>();
            float height = upper ? Mathf.Clamp(top - 22 - footY - 6, 8, 58) : 58;
            var visual=body.GetComponent<CharacterVisual>();
            if(visual!=null && visual.HasSprite)visual.Fit(upper?height:visual.DesiredHeight,2*(root.rect.width/2-Mathf.Abs(actors[0].anchoredPosition.x))-8);
            else { body.sizeDelta = new Vector2(38, height); body.anchoredPosition = new Vector2(0, height / 2 + 1); }
            var bodyLabel = body.GetComponentInChildren<Text>();
            if (bodyLabel != null) { bodyLabel.rectTransform.sizeDelta = new Vector2(36, Mathf.Max(8, height - 4)); bodyLabel.fontSize = height < 35 ? 8 : 12; }
            float footX = hud.InverseTransformPoint(actors[0].position).x;
            float nameX = root.InverseTransformPoint(hud.position).x + Mathf.Clamp(footX, hud.rect.xMin + 71, hud.rect.xMax - 71) - actors[0].anchoredPosition.x;
            actors[0].Find("NameHealth").GetComponent<RectTransform>().anchoredPosition = new Vector2(nameX, upper ? -32 : visual!=null && visual.HasSprite?visual.DesiredHeight+18:72);
            bool lower = footY - (float)(Config.playerRadius * Viewport.Scale) < hud.rect.yMin + 104;
            status.rectTransform.anchoredPosition = new Vector2(0, lower ? hud.rect.yMax - 145 : hud.rect.yMin + 83);
            SortActorsByFeet();
        }
        public void Aim(Vector2 screen)
        {
            if (online) { OnlineAim(screen); return; }
            if (Match != null) { MatchAim(screen); return; }
            if (!Session.Aim(FromPointer(screen), Time.unscaledTimeAsDouble)) return;
            Point end = Config.ArenaGeometry.ProjectileCollision(Session.Position, Session.Direction, Config.projectileRadius);
            Vector2 a = ToCanvas(Session.Position), b = ToCanvas(end), delta = b - a;
            aim.gameObject.SetActive(true); aim.anchoredPosition = (a + b) / 2;
            aim.sizeDelta = new Vector2(delta.magnitude, 2); aim.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        }
        public void StartPlacement()
        {
            Session.Start(Time.unscaledTimeAsDouble);
            foreach (var actor in actors) actor.gameObject.SetActive(false);
            VisibleActorCount = 0; aim.gameObject.SetActive(false); status.text = Strings.Get("hint"); readyText.text = Strings.Get("ready");
        }
        void OnReady()
        {
            if (online) { NetworkSession.Instance.Action("lock"); return; }
            if (Match != null) { PumpBots(Time.unscaledTimeAsDouble); Match.Tick(Time.unscaledTimeAsDouble); Match.Lock("player", Time.unscaledTimeAsDouble); PaintMatch(); return; }
            if (Session.Phase == BattlePhase.LayoutPreview || Session.Phase == BattlePhase.AwaitingRules) StartPlacement();
            else if (Session.Ready(Time.unscaledTimeAsDouble)) readyText.text = Strings.Get("locked");
        }
        void Update()
        {
            if (!initialized) return;
            SyncKitHUD();
            if (root.rect.size != previousSize || CurrentSafeArea != previousSafeArea) { Rebuild(); return; }
            if (offlineLaunch != null) offlineLaunch.gameObject.SetActive(Session.Phase == BattlePhase.LayoutPreview);
            if (online) { PaintOnline(); return; }
            if (Match != null) { UpdateMatch(); return; }
            BattlePhase old = Session.Phase; Session.Tick(Time.unscaledTimeAsDouble);
            if (Session.Phase == BattlePhase.Placement)
            {
                double seconds = System.Math.Max(0, Session.Deadline - Time.unscaledTimeAsDouble);
                int second = (int)System.Math.Ceiling(seconds);
                if (second != lastTimerSecond) { timer.text = second.ToString(); lastTimerSecond = second; }
                timer.color = seconds < 2 ? C(255, 118, 90) : Color.white;
                VisibleActorCount = Session.HasPosition ? 1 : 0;
            }
            if (old != Session.Phase && Session.Phase == BattlePhase.Reveal)
            {
                for (int i = 1; i < 3; i++) actors[i].gameObject.SetActive(true);
                VisibleActorCount = 3; aim.gameObject.SetActive(false); revealAt = Time.unscaledTime; status.text = Strings.Get("reveal");
            }
            if (Session.Phase == BattlePhase.Reveal && Time.unscaledTime - revealAt >= Config.revealSeconds) Session.FinishReveal();
            if (Session.Phase == BattlePhase.AwaitingRules)
            {
                timer.text = "0"; status.text = Strings.Get("awaitingRules"); readyText.text = Strings.Get("retry");
            }
        }
        void Rebuild()
        {
            // Rebuild only on a viewport/safe-area change; never allocate the scene in normal Update.
            if(online)onlineResultShown=false;
            for (int i = root.childCount - 1; i >= 0; i--) { var child = root.GetChild(i); child.gameObject.SetActive(false); Destroy(child.gameObject); }
            panel = null; projectiles.Clear(); Build();
            if (online) { PaintOnline(); return; }
            if (Match != null) { PaintMatch(); return; }
            AddOfflineLauncher();
            if (Session.Phase != BattlePhase.LayoutPreview)
            {
                bool reveal = Session.Phase == BattlePhase.Reveal || (Session.Phase == BattlePhase.AwaitingRules && Session.HasAim);
                actors[0].gameObject.SetActive(Session.HasPosition); actors[0].anchoredPosition = ToCanvas(Session.Position);
                actors[1].gameObject.SetActive(reveal); actors[2].gameObject.SetActive(reveal);
                VisibleActorCount = (Session.HasPosition ? 1 : 0) + (reveal ? 2 : 0);
                if (Session.HasPosition) AdaptHudToPlayer();
            }
        }
        void Message(string message){ShowKitPanel("HitMePopup",Strings.Get("weapon"),message);}
        void Settings()
        {
            language = language == "vi" ? "en" : "vi"; Strings.Load(language); Rebuild();
        }
    }
}

