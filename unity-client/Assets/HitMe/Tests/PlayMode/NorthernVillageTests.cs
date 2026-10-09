using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using HitMe.UI;
using HitMe.Core;
using HitMe.Visuals;
namespace HitMe.Tests {
public sealed class NorthernVillageTests {
 static readonly string EvidenceDirectory=Path.GetFullPath(Path.Combine(Application.dataPath,"../../docs/screenshots/Northern-Run-"+System.DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")));
 static void Capture(string name){Directory.CreateDirectory(EvidenceDirectory);var image=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(EvidenceDirectory,name+".png"),image.EncodeToPNG());Object.Destroy(image);}
 [UnityTest] public IEnumerator LayersPreserveSixFeetResponsivePrivacyAndRealCombat(){int saved=ArenaMaps.Selected;ArenaMaps.Selected=0;OfflineRunContext.Requested=false;SceneManager.LoadScene("Battle");yield return null;yield return null;
  var view=Object.FindAnyObjectByType<BattleView>();
  foreach(var size in new[]{new Vector2Int(360,800),new Vector2Int(375,812),new Vector2Int(390,844),new Vector2Int(402,874),new Vector2Int(430,932)}){
   typeof(BattleSmokeTests).GetMethod("SetGameViewSize",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{size.x,size.y});yield return null;yield return null;
   view.StartOffline(new MatchSettings{BotCount=5,Timeout=TimeoutPolicy.ProposedStayAndSkip});yield return null;yield return null;
   var village=Object.FindAnyObjectByType<NorthernVillageArenaView>();Assert.IsNotNull(village);Assert.AreEqual(8,village.transform.childCount);Assert.AreEqual(village.arenaFloor,GameObject.Find("ArenaSandPlaceholder").transform.parent);Assert.AreEqual(village.gameplayVisualRoot,GameObject.Find("ActorLayer").transform.parent);Assert.AreEqual(0,view.VisibleActorCount);
   view.Match.Place("player",new Point(0,-700),view.Match.PhaseStarted+.01);yield return null;yield return new WaitForSecondsRealtime(.05f);Capture("Idle-"+size.x+"x"+size.y);
   view.Match.Aim("player",new Point(1,0),view.Match.PhaseStarted+.01);yield return null;yield return new WaitForSecondsRealtime(.05f);Capture("Aim-"+size.x+"x"+size.y);
  }
  view.StartOffline(new MatchSettings{BotCount=1,Timeout=TimeoutPolicy.ProposedStayAndSkip});yield return null;var m=view.Match;
  for(int round=0;round<3;round++){double at=m.PhaseStarted+.01;m.Place("player",new Point(-200,0),at);m.Aim("player",new Point(1,0),at);m.Lock("player",at);m.Place("bot-1",new Point(200,0),at);m.Aim("bot-1",new Point(1,0),at);m.Lock("bot-1",at);yield return null;if(round==0){yield return new WaitForSecondsRealtime(.05f);Capture("Reveal");};m.Tick(at+.31);yield return null;Assert.AreEqual(MatchPhase.Throw,m.Phase);if(round==0){yield return new WaitForSecondsRealtime(.05f);Capture("Throw");};m.Tick(at+1.01);yield return null;Assert.AreEqual(MatchPhase.RoundResult,m.Phase);if(round==0){yield return new WaitForSecondsRealtime(.05f);Capture("Hit");};m.Tick(at+1.61);yield return null;}
  yield return null;Assert.AreEqual("Result",SceneManager.GetActiveScene().name);yield return new WaitForSecondsRealtime(.05f);Capture("Victory");ArenaMaps.Selected=saved;LogAssert.NoUnexpectedReceived();
 }
}
}
