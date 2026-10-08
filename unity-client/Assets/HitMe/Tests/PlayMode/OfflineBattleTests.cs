using System.Collections;
using System.IO;
using System.Reflection;
using System.Linq;
using HitMe.Core;
using HitMe.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace HitMe.Tests
{
    public sealed class OfflineBattleTests
    {
        static void Size(int w,int h) => typeof(BattleSmokeTests).GetMethod("SetGameViewSize",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{w,h});
        static void Capture(string name)
        { var image=ScreenCapture.CaptureScreenshotAsTexture(); var directory=Path.GetFullPath(Path.Combine(Application.dataPath,"../../docs/screenshots")); Directory.CreateDirectory(directory); File.WriteAllBytes(Path.Combine(directory,name),image.EncodeToPNG()); Object.Destroy(image); }
        [UnityTest] public IEnumerator OfflineInputPrivacyTouchAndFourSizes()
        {
            OfflineRunContext.Requested=false; SceneManager.LoadScene("Battle"); yield return null;yield return null;
            var view=Object.FindAnyObjectByType<BattleView>();
            foreach(var size in new[]{new Vector2Int(360,800),new Vector2Int(390,844),new Vector2Int(412,915),new Vector2Int(430,932)})
            {
                Size(size.x,size.y); yield return null;yield return null;
                view.SafeAreaOverride=new Rect(0,34,Screen.width,Screen.height-78);
                view.StartOffline(new MatchSettings { BotCount=5,Timeout=TimeoutPolicy.ProposedStayAndSkip }); yield return null;yield return null;
                Assert.IsNull(GameObject.Find("ArenaLabel"),"Decorative label must not cover gameplay actors."); Assert.AreEqual(6,view.Match.Ids.Count); Assert.AreEqual(0,view.VisibleActorCount);
                var floor=GameObject.Find("ArenaSandPlaceholder");
                var first=new PointerEventData(EventSystem.current){pointerId=11,position=new Vector2(Screen.width*.5f,Screen.height*.45f)};
                ExecuteEvents.Execute(floor,first,ExecuteEvents.pointerDownHandler);
                var position=view.Match.ViewFor("player")[0].Action.Position;
                var second=new PointerEventData(EventSystem.current){pointerId=12,position=new Vector2(Screen.width*.6f,Screen.height*.4f)};
                ExecuteEvents.Execute(floor,second,ExecuteEvents.pointerDownHandler);
                Assert.AreEqual(position.X,view.Match.ViewFor("player")[0].Action.Position.X,"Second touch must not place.");
                ExecuteEvents.Execute(floor,second,ExecuteEvents.pointerUpHandler);
                first.position+=new Vector2(60,30); ExecuteEvents.Execute(floor,first,ExecuteEvents.dragHandler); ExecuteEvents.Execute(floor,first,ExecuteEvents.pointerUpHandler);
                Assert.IsTrue(view.Match.ViewFor("player")[0].Action.CanThrow);
                GameObject.Find("Ready").GetComponent<Button>().onClick.Invoke(); yield return null;
                Assert.AreEqual(InputStage.Locked,view.Match.ViewFor("player")[0].Stage);
                Assert.AreEqual(1,view.VisibleActorCount); Assert.IsNull(GameObject.Find("ActorFeet1"));
                Assert.IsTrue(view.Match.ViewFor("player").Skip(1).All(f=>f.Action==null));
                foreach(string name in new[]{"Ready","Settings","Chat","Weapon"})
                { var rect=GameObject.Find(name).GetComponent<RectTransform>(); var corners=new Vector3[4];rect.GetWorldCorners(corners);foreach(var corner in corners)Assert.That(RectTransformUtility.WorldToScreenPoint(null,corner).y,Is.InRange(33,Screen.height-43)); }
                foreach(var t in Object.FindObjectsByType<Text>())Assert.LessOrEqual(t.preferredHeight,t.rectTransform.rect.height+2,t.name+" clipped");
                yield return new WaitForSecondsRealtime(.15f); Capture("Sprint2-Placement-"+size.x+"x"+size.y+".png");
            }
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator MatchThroughResultReplayAndMenu()
        {
            OfflineRunContext.Requested=false;SceneManager.LoadScene("Battle");yield return null;yield return null;Size(390,844);yield return null;yield return null;
            var view=Object.FindAnyObjectByType<BattleView>();view.StartOffline(new MatchSettings { BotCount=1,Timeout=TimeoutPolicy.ProposedStayAndSkip });yield return null;
            var m=view.Match;
            for(int round=0;round<3;round++)
            {
                double at=m.PhaseStarted+.01;
                Assert.IsTrue(m.Place("player",new Point(-200,0),at));Assert.IsTrue(m.Aim("player",new Point(1,0),at));Assert.IsTrue(m.Lock("player",at));
                Assert.IsTrue(m.Place("bot-1",new Point(200,0),at));Assert.IsTrue(m.Aim("bot-1",new Point(1,0),at));Assert.IsTrue(m.Lock("bot-1",at));
                yield return null;
                if(round==0){yield return new WaitForSecondsRealtime(.1f);Capture("Sprint2-Reveal-390x844.png");}
                m.Tick(at+.31);yield return null;Assert.AreEqual(MatchPhase.Throw,m.Phase);
                if(round==0){yield return null;Capture("Sprint2-Throw-390x844.png");}
                m.Tick(at+1.01);yield return null;Assert.AreEqual(MatchPhase.RoundResult,m.Phase);
                Assert.AreEqual(2-round,m.ViewFor("player")[1].Hp);
                m.Tick(at+1.61);yield return null;
            }
            yield return null;yield return null;Assert.AreEqual("Result",SceneManager.GetActiveScene().name);
            Assert.AreEqual(Strings.Get("victory"),GameObject.Find("ResultOutcome").GetComponent<Text>().text);
            Assert.AreEqual(3,OfflineRunContext.Rounds);yield return new WaitForSecondsRealtime(.15f);Capture("Sprint2-Result-390x844.png");
            GameObject.Find("Replay").GetComponent<Button>().onClick.Invoke();yield return null;yield return null;
            var replay=Object.FindAnyObjectByType<BattleView>().Match;Assert.IsNotNull(replay);Assert.AreEqual(1,replay.Round);Assert.IsTrue(replay.ViewFor("player").All(f=>f.Hp==3 && f.Action==null));Assert.IsEmpty(replay.History);
            OfflineRunContext.Menu();yield return null;yield return null;Assert.AreEqual("MainMenu",SceneManager.GetActiveScene().name);LogAssert.NoUnexpectedReceived();
        }
        static Rect Bounds(RectTransform r)
        { var corners=new Vector3[4];r.GetWorldCorners(corners);var points=corners.Select(c=>RectTransformUtility.WorldToScreenPoint(null,c)).ToArray();return Rect.MinMaxRect(points.Min(p=>p.x),points.Min(p=>p.y),points.Max(p=>p.x),points.Max(p=>p.y)); }
        [UnityTest] public IEnumerator BotsAtRimKeepCharactersAndHudSeparate()
        {
            OfflineRunContext.Requested=false;SceneManager.LoadScene("Battle");yield return null;yield return null;
            var view=Object.FindAnyObjectByType<BattleView>();
            foreach(var size in new[]{new Vector2Int(360,800),new Vector2Int(390,844),new Vector2Int(412,915),new Vector2Int(430,932)})
            {
                Size(size.x,size.y);yield return null;yield return null;view.SafeAreaOverride=new Rect(0,34,Screen.width,Screen.height-78);
                view.StartOffline(new MatchSettings { BotCount=5,Timeout=TimeoutPolicy.ProposedStayAndSkip });yield return null;
                var m=view.Match; var poses=new[]{new Point(0,-370),new Point(0,1660),new Point(0,-1660),new Point(910,0),new Point(-910,0),new Point(0,0)};
                double at=Time.unscaledTimeAsDouble;for(int i=0;i<6;i++){Assert.IsTrue(m.Place(m.Ids[i],poses[i],at));Assert.IsTrue(m.Aim(m.Ids[i],new Point(1,0),at));Assert.IsTrue(m.Lock(m.Ids[i],at));}
                yield return null;
                foreach(var actor in Object.FindObjectsByType<CharacterPresentation>())
                {
                    var bounds=Bounds(actor.GetComponent<RectTransform>());
                    foreach(string widget in new[]{"Round","Timer","Settings","Ready","Chat","Weapon","Status","PortraitPlaceholder0","PortraitPlaceholder1","PortraitPlaceholder2","PortraitPlaceholder3","PortraitPlaceholder4","PortraitPlaceholder5"})Assert.IsFalse(bounds.Overlaps(Bounds(GameObject.Find(widget).GetComponent<RectTransform>())),actor.transform.parent.name+" overlaps "+widget);
                    Assert.That(bounds.xMin,Is.GreaterThanOrEqualTo(-1));Assert.That(bounds.xMax,Is.LessThanOrEqualTo(Screen.width+1));
                }
                foreach(var t in Object.FindObjectsByType<Text>())Assert.LessOrEqual(t.preferredHeight,t.rectTransform.rect.height+2,"Clipped "+t.name);
            }
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator RealClockThrowAnimatesPrecomputedResult()
        {
            OfflineRunContext.Requested=false;SceneManager.LoadScene("Battle");yield return null;yield return null;
            var view=Object.FindAnyObjectByType<BattleView>();view.StartOffline(new MatchSettings { BotCount=1,Timeout=TimeoutPolicy.ProposedStayAndSkip });yield return null;
            var m=view.Match;double now=Time.unscaledTimeAsDouble;
            foreach(var id in m.Ids){var p=id=="player"?new Point(-200,0):new Point(200,0);Assert.IsTrue(m.Place(id,p,now));Assert.IsTrue(m.Aim(id,new Point(id=="player"?1:-1,0),now));Assert.IsTrue(m.Lock(id,now));}
            var computed=m.Resolution;Assert.AreEqual(2,computed.Health[0].HPAfter);Assert.AreEqual(3,m.ViewFor("player")[0].Hp);
            double limit=Time.unscaledTimeAsDouble+2;while(m.Phase==MatchPhase.Reveal && Time.unscaledTimeAsDouble<limit)yield return null;
            Assert.AreEqual(MatchPhase.Throw,m.Phase);yield return new WaitForSecondsRealtime(.2f);
            var projectile=GameObject.Find("ProjectilePH").GetComponent<RectTransform>();var origin=computed.Throws[0].Origin; Assert.That(Vector2.Distance(projectile.anchoredPosition,new Vector2((float)(origin.X*view.Viewport.Scale),(float)(origin.Y*view.Viewport.Scale))),Is.GreaterThan(1),"Projectile must actually move from its own start point.");
            Assert.AreSame(computed,m.Resolution);Assert.AreEqual(3,m.ViewFor("player")[0].Hp);Capture("Sprint2-Throw-RealClock.png");
            while(m.Phase==MatchPhase.Throw && Time.unscaledTimeAsDouble<limit)yield return null;
            Assert.AreEqual(MatchPhase.RoundResult,m.Phase);Assert.AreEqual(2,m.ViewFor("player")[0].Hp);LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator ResultVictoryDefeatDrawAreLocalized()
        {
            foreach(var winner in new[]{"player","bot-1",null})
            {
                var f=new[]{new FighterSnapshot("player",1),new FighterSnapshot("bot-1",1)};
                var actions=new[]{new LockedAction("player",new Point(-200,0),new Point(winner=="bot-1"? -1:1,0)),new LockedAction("bot-1",new Point(200,0),new Point(winner=="player"?1:-1,0))};
                OfflineRunContext.Result=CombatRules.Resolve(f,actions,new FoundationConfig());OfflineRunContext.Rounds=3;
                SceneManager.LoadScene("Result");yield return null;yield return null;yield return null;
                Assert.AreEqual(Strings.Get(winner==null?"draw":winner=="player"?"victory":"defeat"),GameObject.Find("ResultOutcome").GetComponent<Text>().text);
                Assert.IsNotNull(GameObject.Find("Replay"));Assert.IsNotNull(GameObject.Find("BackToMenu"));
            }
            OfflineRunContext.Menu();yield return null;LogAssert.NoUnexpectedReceived();
        }
    }
}
