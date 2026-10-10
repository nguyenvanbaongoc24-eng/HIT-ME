using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HitMe.Characters;
using HitMe.Core;
using HitMe.UI;
using HitMe.Visuals;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace HitMe.Tests {
public sealed class ProductionMotionEvidenceTests {
 [UnityTest] public IEnumerator RealCombatProducesTimestampedMotionEvidence(){
  bool reduced=MotionSettings.Reduced;int choice=CosmeticPreview.Character;
  var folder=Path.GetFullPath(Path.Combine(Application.dataPath,"../../docs/screenshots/ProductionMotion-"+System.DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")));
  Directory.CreateDirectory(folder);var samples=new List<string>();int index=0;
  System.Action<string> capture=label=>{var image=ScreenCapture.CaptureScreenshotAsTexture();string file=(index++).ToString("D3")+"-"+label+".png";File.WriteAllBytes(Path.Combine(folder,file),image.EncodeToPNG());Object.Destroy(image);samples.Add(Time.unscaledTimeAsDouble.ToString("F6",System.Globalization.CultureInfo.InvariantCulture)+" "+file);};
  try {MotionSettings.Reduced=false;CosmeticPreview.Character=0;OfflineRunContext.Requested=false;SceneManager.LoadScene("Battle");yield return null;yield return null;
   typeof(BattleSmokeTests).GetMethod("SetGameViewSize",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic).Invoke(null,new object[]{390,844});yield return null;yield return null;
   var view=Object.FindAnyObjectByType<BattleView>();view.StartOffline(new MatchSettings{BotCount=1,Timeout=TimeoutPolicy.ProposedStayAndSkip,RoundResultSeconds=2});((System.Collections.IDictionary)typeof(BattleView).GetField("bots",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(view)).Clear();yield return null;
   var match=view.Match;double at=match.PhaseStarted+.01;match.Place("player",new Point(-200,0),at);yield return null;
   var visual=GameObject.Find("ActorFeet0").GetComponentInChildren<CharacterVisual>();var feet=visual.transform.parent.GetComponent<RectTransform>();var foot=feet.anchoredPosition;var positions=new List<Vector2>();
   for(int i=0;i<7;i++){yield return new WaitForSecondsRealtime(.5f);Assert.AreEqual(VisualState.Idle,visual.State);positions.Add(visual.transform.Find("MotionArtwork").GetComponent<RectTransform>().anchoredPosition);Assert.AreEqual(foot,feet.anchoredPosition);capture("Idle");}
   Assert.Greater(positions.Distinct().Count(),1,"Actual runtime frames must move.");
   for(int round=1;round<=3;round++){at=System.Math.Max(match.PhaseStarted,Time.unscaledTimeAsDouble)+.01;match.Place("player",new Point(-200,0),at);match.Aim("player",new Point(1,0),at);yield return null;Assert.AreEqual(VisualState.Aim,visual.State);capture("Aim");match.Lock("player",at);yield return null;capture("Locked");match.Place("bot-1",new Point(200,0),at);match.Aim("bot-1",new Point(1,0),at);match.Lock("bot-1",at);yield return null;capture("Reveal");match.Tick(at+.31);yield return null;Assert.AreEqual(VisualState.Throw,visual.State);capture("Throw");yield return new WaitForSecondsRealtime(.15f);capture("FollowThrough");match.Tick(at+1.1);yield return null;yield return null;Assert.AreEqual(MatchPhase.RoundResult,match.Phase);
    var bot=GameObject.Find("ActorFeet1").GetComponentInChildren<CharacterVisual>();Assert.AreEqual(round==3?VisualState.Eliminated:VisualState.Hit,bot.State);capture(round==3?"EliminatedAndVictory":"Hit");if(round==3)Assert.AreEqual(VisualState.Victory,visual.State);else {match.Tick(at+3.1);yield return null;}
   }
   LogAssert.NoUnexpectedReceived();
  } finally {File.WriteAllLines(Path.Combine(folder,"timestamps.txt"),samples);MotionSettings.Reduced=reduced;CosmeticPreview.Character=choice;}
 }
}
}
