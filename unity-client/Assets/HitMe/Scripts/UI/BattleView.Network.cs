using System;
using System.Linq;
using HitMe.Core;
using HitMe.Characters;
using UnityEngine;
using UnityEngine.UI;
namespace HitMe.UI
{
    public sealed partial class BattleView
    {
        bool online;NetPlayer[] onlinePlayers;string onlinePhase="";int onlineRound=-1;bool onlineResultShown;
        Point P(NetPoint p)=>new Point(p.x,p.y);
        WeaponDefinition OnlineWeapon(string id){var catalog=CharacterCatalog.Load();int slot=id=="chao"?1:id=="vot"?2:0;return catalog?.ForSlot(slot)?.defaultWeapon;}
        public void StartOnline(){online=true;var net=NetworkSession.Instance;onlinePlayers=net.Room.players.OrderBy(p=>p.id==net.PlayerId?0:1).ThenBy(p=>p.id,StringComparer.Ordinal).ToArray();
            actors=new RectTransform[onlinePlayers.Length];portraitItems=new RectTransform[actors.Length*3];portraitPositions=new Vector2[portraitItems.Length];Rebuild();PaintOnline();}
        void OnlinePlace(Vector2 screen){if(!CanPlace)return;var p=FromPointer(screen);if(!Config.ArenaGeometry.PointInside(p))return;p=Config.ArenaGeometry.ClampPosition(p,Config.playerRadius);NetworkSession.Instance.Action("place",new NetPoint{x=p.X,y=p.Y});}
        void OnlineAim(Vector2 screen){if(!CanPlace)return;var self=NetworkSession.Instance.Self;if(self?.action==null)return;var p=FromPointer(screen);NetworkSession.Instance.Action("aim",new NetPoint{x=p.X-self.action.position.x,y=p.Y-self.action.position.y});}
        void PaintOnline(){var net=NetworkSession.Instance;var room=net.Room;if(room==null)return;bool placement=room.phase=="Placement";bool changed=room.phase!=onlinePhase||room.round!=onlineRound;
            if(changed){ReleaseProjectiles();if(room.phase=="Throw")HitMe.Audio.HitMeAudio.Play("sfx_throw",room.match+":"+room.round+":throw");if(room.phase=="Reveal")HitMe.Audio.HitMeAudio.Play("sfx_reveal",room.match+":"+room.round+":reveal");if(room.phase=="RoundResult"&&room.resolution!=null){foreach(var t in room.resolution.throws){impactFeedback.ConfirmedImpact(ToCanvas(P(t.end)),t.damage>0);ConfirmedCrowdResponse(t.damage>0);HitMe.Audio.HitMeAudio.Play(t.damage>0?"sfx_hit_bop":"sfx_wall_thud",room.match+":"+room.round+":impact:"+t.thrower);}foreach(var h in room.resolution.health)if(h.before>0&&h.after==0)HitMe.Audio.HitMeAudio.Play("sfx_eliminate",room.match+":"+room.round+":eliminate:"+h.id);}}
            floor.Find("ArenaLabel").gameObject.SetActive(false);VisibleActorCount=0;
            for(int i=0;i<actors.Length;i++){var f=Array.Find(room.players,p=>p.id==onlinePlayers[i].id);if(f==null){actors[i].gameObject.SetActive(false);continue;}var action=f.action;bool visible=action!=null&&(!placement||f.id==net.PlayerId);actors[i].gameObject.SetActive(visible);
                portraitItems[i*3+2].GetComponent<Text>().text=Hearts[Mathf.Clamp(f.hp,0,3)];if(!visible)continue;VisibleActorCount++;actors[i].anchoredPosition=ToCanvas(P(action.position));
                var presentation=actors[i].Find("CharacterSpritePlaceholder").GetComponent<CharacterPresentation>();bool hit=room.phase=="RoundResult"&&room.resolution!=null&&Array.Exists(room.resolution.health,h=>h.id==f.id&&h.after<h.before);
                presentation.Visual.SetCosmeticWeapon(OnlineWeapon(f.weapon));presentation.Present(room.phase=="MatchResult"&&room.resolution?.winner==f.id?CharacterPose.Win:f.hp==0?CharacterPose.Dead:hit?CharacterPose.Hit:room.phase=="Throw"?CharacterPose.Throw:action.canThrow?CharacterPose.Aim:CharacterPose.Idle,Color.white);
                presentation.Visual.SetFacing(new Vector2((float)action.direction.x,(float)action.direction.y));actors[i].Find("NameHealth/Name").GetComponent<Text>().text=i==0?Strings.Get("you"):f.name;actors[i].Find("NameHealth/HealthSymbols").GetComponent<Text>().text=Hearts[Mathf.Clamp(f.hp,0,3)];}
            SortActorsByFeet();LayoutOpponentLabels();hud.Find("Round").GetComponent<Text>().text=Strings.Get("round")+" "+room.round;
            timer.text=placement?Math.Max(0,(int)Math.Ceiling((room.deadline-net.ServerNow)/1000)).ToString():"0";ready.interactable=CanPlace;readyText.text=net.Self?.locked==true?Strings.Get("locked"):Strings.Get("ready");
            status.text=!net.Connected?Strings.Get("networkDisconnected"):net.Error.Length>0?net.ErrorText:Strings.Get("network"+room.phase);if(placement&&net.Self?.action?.canThrow==true) ShowProductionAim(P(net.Self.action.position),P(net.Self.action.direction),net.Self.locked); else if(room.phase!="Reveal") aim.GetComponent<ProductionAimIndicator>().Hide(room.phase=="Throw");
            if(room.phase=="Throw"&&room.resolution!=null){if(projectiles.Count==0)foreach(var t in room.resolution.throws){int index=Array.FindIndex(onlinePlayers,p=>p.id==t.thrower);var weapon=actors[index].Find("CharacterSpritePlaceholder").GetComponent<CharacterVisual>().Weapon;projectiles.Add(RentProjectile(weapon.FlightSprite,weapon.visualSize,ToCanvas(P(t.origin))));}
                float progress=Mathf.Clamp01((float)(1-(room.deadline-net.ServerNow)/700));for(int i=0;i<projectiles.Count;i++){var t=room.resolution.throws[i];projectiles[i].anchoredPosition=Vector2.Lerp(ToCanvas(P(t.origin)),ToCanvas(P(t.end)),progress);}}
            onlinePhase=room.phase;onlineRound=room.round;
            if(room.phase=="MatchResult"&&!onlineResultShown){onlineResultShown=true;ShowOnlineResult();}}
        void ShowOnlineResult(){var net=NetworkSession.Instance;var result=net.Room.resolution;panel=Box("OnlineResult",hud,new Vector2(340,560),Vector2.zero,C(73,59,53)).gameObject;panel.AddComponent<UIMotionController>();
            if(result.outcome!="Draw")HitMe.Audio.HitMeAudio.Play(result.winner==net.PlayerId?"sfx_win":"sfx_lose",net.Room.match+":outcome");
            Label("ServerResult",Strings.Get(result.outcome=="Draw"?"draw":result.winner==net.PlayerId?"victory":"defeat"),new Vector2(320,50),new Vector2(0,235),24);
            Label("StatsHeader",Strings.Get("resultStatsHeader"),new Vector2(320,32),new Vector2(0,188),13);
            for(int i=0;i<net.Room.players.Length;i++){
                var player=net.Room.players[i];float y=145-i*40;
                var portrait=Box("ResultPortrait"+i,panel.transform,new Vector2(32,32),new Vector2(-140,y),Color.white);
                portrait.sprite=CharacterCatalog.Load()?.ForSlot(player.avatar=="Char02_BotMale"?1:player.avatar=="Char03_BotFemale"?2:0)?.portrait;portrait.preserveAspect=true;
                Label("ResultName"+i,player.name,new Vector2(104,36),new Vector2(-68,y),13);
                var stats=net.Room.stats==null?null:Array.Find(net.Room.stats,s=>s.id==player.id);
                string values=stats==null?"—":stats.throws+" / "+stats.hits+" / "+stats.misses+" / "+stats.received;
                Label("ResultStats"+i,values,new Vector2(154,36),new Vector2(73,y),13);
            }
            var reward=net.Profile.history==null?null:Array.Find(net.Profile.history,h=>h.match==net.Room.match);
            Label("ServerBalance",panel.transform,Strings.Get("serverConfirmed")+"\n"+Strings.Get("matchReward")+": "+(reward==null?"—":reward.coins+" · XP "+reward.xp)+"\n"+Strings.Get("balance")+": "+net.Profile.coins+" · XP "+net.Profile.xp+"\n"+Strings.Get("trialRewards"),new Vector2(320,105),new Vector2(0,-140),14);
            if(reward?.coins>0)HitMe.Audio.HitMeAudio.Play("sfx_coin",net.Room.match+":reward:"+net.PlayerId);
            panel.transform.Find("ServerBalance").gameObject.AddComponent<HitMe.Visuals.FeedbackPulse>();
            Button("ReturnLobby",panel.transform,Strings.Get("online"),new Vector2(240,44),new Vector2(0,-235),()=>{net.OnlineBattle=false;net.Command("leave");UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");},C(31,140,129));}
        Text Label(string id,string value,Vector2 size,Vector2 pos,int fontSize)=>Label(id,panel.transform,value,size,pos,fontSize);
    }
}
