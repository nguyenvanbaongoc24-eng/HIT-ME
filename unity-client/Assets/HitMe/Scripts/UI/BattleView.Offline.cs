using System;
using System.Collections.Generic;
using System.Linq;
using HitMe.Core;
using HitMe.Characters;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace HitMe.UI
{
    public sealed partial class BattleView
    {
        public OfflineMatch Match { get; private set; }
        static readonly string[] Hearts={"","♥","♥♥","♥♥♥"};
        readonly List<(string id,BotDecision decision,BotContext context)> scheduledBots=new List<(string,BotDecision,BotContext)>(); int scheduledRound=-1;
        readonly Dictionary<string,BotController> bots = new Dictionary<string,BotController>();
        Button offlineLaunch; readonly List<RectTransform> projectiles = new List<RectTransform>();
        double presentationThrowSeconds;
        MatchPhase paintedPhase = (MatchPhase)(-1); int paintedRound=-1; bool resultNavigated;
        public void AddOfflineLauncher()
        {
            if(Match!=null)return;
            offlineLaunch=Button("PlayOffline",hud,Strings.Get("playOffline"),new Vector2(240,44),new Vector2(0,-root.rect.height/2+210),()=>StartOffline(OfflineRunContext.Settings),C(31,140,129));
        }
        public void StartOffline(MatchSettings settings)
        {
            Initialize(); presentationThrowSeconds=settings.ThrowSeconds; Match=new OfflineMatch(Config,settings,Time.unscaledTimeAsDouble);
            scheduledRound=-1;scheduledBots.Clear();bots.Clear(); for(int i=1;i<=settings.BotCount;i++) bots.Add("bot-"+i,new BotController(settings.Seed+(uint)i*7919,settings.Difficulty));
            actors=new RectTransform[settings.BotCount+1]; portraitItems=new RectTransform[actors.Length*3]; portraitPositions=new Vector2[portraitItems.Length];
            paintedPhase=(MatchPhase)(-1); paintedRound=-1; resultNavigated=false; Rebuild();
            if(offlineLaunch!=null) Destroy(offlineLaunch.gameObject); offlineLaunch=null;
        }
        void PumpBots(double now)
        {
            if(Match.Phase!=MatchPhase.Placement)return;
            if(scheduledRound!=Match.Round){
                scheduledRound=Match.Round;scheduledBots.Clear();
                foreach(var item in bots){var context=Match.BotContextFor(item.Key);var decision=item.Value.Decide(context,Config,Match.PhaseStarted);if(decision!=null)scheduledBots.Add((item.Key,decision,context));}
                scheduledBots.Sort((a,b)=>{int order=a.decision.LockAt.CompareTo(b.decision.LockAt);return order!=0?order:StringComparer.Ordinal.Compare(a.id,b.id);});
            }
            while(scheduledBots.Count>0 && now>=scheduledBots[0].decision.LockAt){var item=scheduledBots[0];scheduledBots.RemoveAt(0);bots[item.id].Submit(item.context,item.decision.LockAt,Match.Place,Match.Aim,Match.Lock);}

        }
        void MatchPlace(Vector2 screen)
        {
            double now=Time.unscaledTimeAsDouble; PumpBots(now); Match.Tick(now);
            Point p=FromPointer(screen); if(!Config.ArenaGeometry.PointInside(p))return;
            Match.Place("player",Config.ArenaGeometry.ClampPosition(p,Config.playerRadius),now); PaintMatch();
        }
        void MatchAim(Vector2 screen)
        {
            var self=Match.ViewFor("player")[0]; if(self.Action==null)return;
            var p=FromPointer(screen); Match.Aim("player",new Point(p.X-self.Action.Position.X,p.Y-self.Action.Position.Y),Time.unscaledTimeAsDouble); PaintMatch();
        }
        void UpdateMatch()
        {
            double now=Time.unscaledTimeAsDouble; PumpBots(now); Match.Tick(now); Match.DrainEvents(); PaintMatch();
            if(Match.Phase==MatchPhase.MatchResult && !resultNavigated)
            {
                resultNavigated=true; OfflineRunContext.Result=Match.Resolution; OfflineRunContext.Rounds=Match.Round; SceneManager.LoadScene("Result");
            }
        }
        void PaintMatch()
        {
            floor.Find("ArenaLabel").gameObject.SetActive(ArenaMaps.Selected!=0);
            var view=Match.ViewFor("player"); bool placement=Match.Phase==MatchPhase.Placement;
            bool changed=paintedPhase!=Match.Phase || paintedRound!=Match.Round;
            if(changed) { foreach(var p in projectiles) if(p!=null)Destroy(p.gameObject); projectiles.Clear(); }
            VisibleActorCount=0;
            for(int i=0;i<actors.Length;i++)
            {
                var f=view[i]; bool visible=f.Action!=null || (!placement && Match.History.Count>0 && Match.History[Match.History.Count-1].Actions.Any(a=>a.Id==f.Id));
                actors[i].gameObject.SetActive(visible);
                if(visible)
                {
                    VisibleActorCount++; var action=f.Action ?? Match.History[Match.History.Count-1].Actions.First(a=>a.Id==f.Id);
                    actors[i].anchoredPosition=ToCanvas(action.Position);
                    var body=actors[i].Find("CharacterSpritePlaceholder").GetComponent<Image>();
                    body.color=f.Hp==0?C(95,95,95):Match.Phase==MatchPhase.RoundResult && Match.Resolution.Health.Any(h=>h.Id==f.Id && h.HPAfter<h.HPBefore)?C(255,92,83):i==0?C(43,157,147):i%2==0?C(121,105,182):C(218,109,76);
                    CharacterPose pose=f.Hp==0?CharacterPose.Dead:Match.Phase==MatchPhase.RoundResult && Match.Resolution.Outcome==MatchOutcome.Winner && Match.Resolution.Winner==f.Id?CharacterPose.Win:Match.Phase==MatchPhase.RoundResult && Match.Resolution.Health.Any(h=>h.Id==f.Id && h.HPAfter<h.HPBefore)?CharacterPose.Hit:Match.Phase==MatchPhase.Throw?CharacterPose.Throw:placement && f.Action!=null && f.Action.CanThrow?CharacterPose.Aim:CharacterPose.Idle;
                    var presentation=body.GetComponent<CharacterPresentation>();
                    presentation.Visual.SetFacing(new Vector2((float)action.Direction.X,(float)action.Direction.Y));
                    presentation.Present(pose,body.color);
                    var spriteLabel=body.GetComponentInChildren<Text>(); if(spriteLabel!=null)spriteLabel.text=f.Hp==0?"X":"PH\n"+(i==0?"P":"B"+i);
                    actors[i].Find("NameHealth/HealthSymbols").GetComponent<Text>().text=Hearts[f.Hp];
                }
                portraitItems[i*3+2].GetComponent<Text>().text=f.Hp==0?"X":Hearts[f.Hp];
                portraitItems[i*3].GetComponent<EllipseGraphic>().color=f.Hp==0?Color.gray:i==0?C(43,157,147):C(190,100,89);
            }
            // Sort every visible/revealed group, including rounds after the human is eliminated.
            SortActorsByFeet();
            if(view[0].Action!=null)AdaptHudToPlayer();
            LayoutOpponentLabels();
            var roundLabel=hud.Find("Round").GetComponent<Text>(); roundLabel.text=Strings.Get("round")+" "+Match.Round;
            timer.text=placement?Math.Max(0,(int)Math.Ceiling(Match.Deadline-Time.unscaledTimeAsDouble)).ToString():"0";
            ready.interactable=CanPlace;
            readyText.text=view[0].Stage==InputStage.Locked?Strings.Get("locked"):view[0].Hp==0?Strings.Get("spectating"):Strings.Get("ready");
            status.text=Match.NeedsTimeoutConfirmation?Strings.Get("timeoutUnconfirmed"):placement?Strings.Get(view[0].Hp==0?"spectating":"hint"):Strings.Get("phase"+Match.Phase);
            if(Match.Phase==MatchPhase.RoundResult)
            {
                var self=Match.Resolution.Throws.FirstOrDefault(t=>t.Thrower=="player");
                status.text=Strings.Get(self==null?"skipThrow":self.Hit?"hit":"miss");
                var eliminated=Match.Resolution.Health.Where(h=>h.Eliminated).ToArray();
                if(eliminated.Length>0)status.text+=" · "+Strings.Get("eliminated")+" "+eliminated.Length.ToString();
            }
            aim.gameObject.SetActive(placement && view[0].Action!=null && view[0].Action.CanThrow);
            if(aim.gameObject.activeSelf)
            {
                var a=view[0].Action; var start=ToCanvas(a.Position); var end=ToCanvas(Config.ArenaGeometry.ProjectileCollision(a.Position,a.Direction,Config.projectileRadius)); var delta=end-start;
                aim.anchoredPosition=(start+end)/2; aim.sizeDelta=new Vector2(delta.magnitude,2); aim.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg);
            }
            if(Match.Phase==MatchPhase.Throw)
            {
                if(projectiles.Count==0)foreach(var t in Match.Resolution.Throws)projectiles.Add(CreateWeaponProjectile(t.Thrower,t.Origin));
                float progress=Mathf.Clamp01((float)((Time.unscaledTimeAsDouble-Match.PhaseStarted)/presentationThrowSeconds));
                for(int i=0;i<projectiles.Count;i++)projectiles[i].anchoredPosition=Vector2.Lerp(ToCanvas(Match.Resolution.Throws[i].Origin),ToCanvas(Match.Resolution.Throws[i].End),progress);
            }
            paintedPhase=Match.Phase; paintedRound=Match.Round;
        }
        readonly List<Rect> occupiedBounds=new List<Rect>(48);
        readonly Vector3[] boundsCorners=new Vector3[4];
        static readonly int[] LabelSides={0,-1,1};
        static readonly string[] HudWidgets={"Round","Timer","Settings","Ready","Chat","Weapon","Status"};
        void LayoutOpponentLabels()
        {
            // Keep HUD clear for all actors and separate name plates even when logical hitboxes overlap.
            occupiedBounds.Clear();var occupied=occupiedBounds;
            float hudBottom=float.PositiveInfinity;
            foreach(var item in portraitItems){var bounds=LocalBounds(item);occupied.Add(bounds);hudBottom=Mathf.Min(hudBottom,bounds.yMin);}
            foreach(string widget in HudWidgets)occupied.Add(LocalBounds(hud.Find(widget).GetComponent<RectTransform>()));
            for(int i=0;i<actors.Length;i++) if(actors[i].gameObject.activeSelf)
            {
                var body=actors[i].Find("CharacterSpritePlaceholder").GetComponent<RectTransform>();

                var visual=body.GetComponent<CharacterVisual>();
                float maxHeight=Mathf.Clamp(hudBottom-actors[i].anchoredPosition.y-7,8,visual.DesiredHeight);
                if(visual.HasSprite)visual.Fit(maxHeight,2*(root.rect.width/2-Mathf.Abs(actors[i].anchoredPosition.x))-8);
                else { body.sizeDelta=new Vector2(38,maxHeight); body.anchoredPosition=new Vector2(0,maxHeight/2+1); }
                if(maxHeight<35 && body.GetComponent<CharacterPresentation>().Pose==CharacterPose.Dead)body.localRotation=Quaternion.identity;
                var bodyLabel=body.GetComponentInChildren<Text>();
                if(bodyLabel!=null) { bodyLabel.rectTransform.sizeDelta=new Vector2(36,Mathf.Max(8,maxHeight-4)); bodyLabel.fontSize=maxHeight<20?5:maxHeight<35?8:12; if(maxHeight<35 && body.GetComponent<CharacterPresentation>().Pose!=CharacterPose.Dead)bodyLabel.text="PH"; }
                occupied.Add(LocalBounds(body)); occupied.Add(LocalBounds(actors[i].Find("FeetHitbox").GetComponent<RectTransform>()));
            }
            for(int i=0;i<actors.Length;i++)if(actors[i].gameObject.activeSelf)
            {
                var label=actors[i].Find("NameHealth").GetComponent<RectTransform>(); Vector2 foot=actors[i].anchoredPosition;
                bool found=false;
                for(int row=0;row<18 && !found;row++)foreach(int side in LabelSides)
                {
                    Vector2 candidate=new Vector2(Mathf.Clamp(foot.x+side*132,root.rect.xMin+65,root.rect.xMax-65),Mathf.Clamp(foot.y+Mathf.Max(72,actors[i].Find("CharacterSpritePlaceholder").GetComponent<RectTransform>().rect.height+18)-row*26,root.rect.yMin+108,root.rect.yMax-110));
                    Rect bounds=new Rect(candidate-new Vector2(63,12),new Vector2(126,24));
                    bool overlaps=false;for(int k=0;k<occupied.Count;k++)if(occupied[k].Overlaps(bounds)){overlaps=true;break;}
                    if(overlaps)continue;
                    label.anchoredPosition=candidate-foot; occupied.Add(bounds); found=true; break;
                }
                label.gameObject.SetActive(found); // HP remains public in roster if there is no collision-free plate.
            }
        }
        void SortActorsByFeet()
        {
            // Fix ranks in ascending order; later moves cannot disturb already sorted siblings.
            for(int rank=0;rank<actors.Length;rank++)for(int i=0;i<actors.Length;i++){
                int targetRank=0;for(int j=0;j<actors.Length;j++)if(j!=i && (actors[j].anchoredPosition.y>actors[i].anchoredPosition.y || actors[j].anchoredPosition.y==actors[i].anchoredPosition.y && j<i))targetRank++;
                if(targetRank==rank){if(actors[i].GetSiblingIndex()!=rank)actors[i].SetSiblingIndex(rank);break;}
            }

        }
        RectTransform CreateWeaponProjectile(string thrower,Point origin)
        {
            int index=Match.Ids.ToList().IndexOf(thrower);
            var definition=actors[index].Find("CharacterSpritePlaceholder").GetComponent<CharacterVisual>().Definition;
            var weapon=definition!=null?definition.defaultWeapon:null;
            if(weapon==null || weapon.FlightSprite==null)
                return Ellipse("ProjectilePH",root,Vector2.one*(float)(Config.projectileRadius*Viewport.Scale*2),ToCanvas(origin),C(65,107,104)).rectTransform;
            var image=Box("ProjectilePH",root,Vector2.one*weapon.visualSize,ToCanvas(origin),Color.white);
            image.sprite=weapon.FlightSprite;image.preserveAspect=true;return image.rectTransform;
        }
        Rect LocalBounds(RectTransform r)
        {
            var corners=boundsCorners; r.GetWorldCorners(corners);
            float minX=float.PositiveInfinity,minY=float.PositiveInfinity,maxX=float.NegativeInfinity,maxY=float.NegativeInfinity;
            foreach(var corner in corners) { var p=root.InverseTransformPoint(corner); minX=Mathf.Min(minX,p.x);minY=Mathf.Min(minY,p.y);maxX=Mathf.Max(maxX,p.x);maxY=Mathf.Max(maxY,p.y); }
            return UnityEngine.Rect.MinMaxRect(minX,minY,maxX,maxY);
        }
    }
}
