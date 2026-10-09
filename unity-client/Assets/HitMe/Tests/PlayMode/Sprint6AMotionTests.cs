using System.Collections;
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
public sealed class Sprint6AMotionTests {
 [UnityTest]public IEnumerator ArtworkMovesWithoutMovingLogicalFeetAndReducedMotionStopsIt(){
  bool reduced=MotionSettings.Reduced;var quality=MotionSettings.Quality;int choice=CosmeticPreview.Character;
  try{MotionSettings.Reduced=false;MotionSettings.Quality=MotionQuality.High;CosmeticPreview.Character=0;OfflineRunContext.Requested=false;SceneManager.LoadScene("Battle");yield return null;yield return null;
   var visual=Object.FindObjectsByType<CharacterVisual>().First(v=>v.Definition.characterId=="Char01_Player");var feet=visual.transform.parent.GetComponent<RectTransform>();var ring=feet.Find("FeetHitbox");var position=feet.anchoredPosition;var matrix=ring.localToWorldMatrix;
   visual.Present(VisualState.Idle,Color.white,.4f);var art=visual.transform.Find("MotionArtwork").GetComponent<RectTransform>();Assert.Greater(art.anchoredPosition.y,0);Assert.AreEqual(position,feet.anchoredPosition);Assert.AreEqual(matrix,ring.localToWorldMatrix);
   MotionSettings.Reduced=true;foreach(VisualState state in System.Enum.GetValues(typeof(VisualState))){visual.Present(state,Color.white,1);Assert.AreEqual(Vector2.zero,art.anchoredPosition);Assert.AreEqual(Vector3.one,art.localScale);Assert.Less(Quaternion.Angle(Quaternion.identity,visual.transform.localRotation),.001f);Assert.AreEqual(matrix,ring.localToWorldMatrix);}
   MotionSettings.Reduced=false;yield return null;var image=ScreenCapture.CaptureScreenshotAsTexture();var path=Path.GetFullPath(Path.Combine(Application.dataPath,"../../docs/screenshots/Sprint6A-Battle-Actual.png"));File.WriteAllBytes(path,image.EncodeToPNG());Object.Destroy(image);
  }finally{MotionSettings.Reduced=reduced;MotionSettings.Quality=quality;CosmeticPreview.Character=choice;}LogAssert.NoUnexpectedReceived();
 }
 [UnityTest]public IEnumerator ConfirmedResolutionAndProjectileReuseCannotChangeHealth(){
  OfflineRunContext.Requested=false;SceneManager.LoadScene("Battle");yield return null;yield return null;var view=Object.FindAnyObjectByType<BattleView>();view.StartOffline(new MatchSettings{BotCount=1,Timeout=TimeoutPolicy.ProposedStayAndSkip});yield return null;var match=view.Match;ProjectileVisualController[] ids=null;
  for(int round=0;round<2;round++){double at=match.PhaseStarted+.01;foreach(string id in match.Ids){match.Place(id,new Point(id=="player"?-200:200,0),at);match.Aim(id,new Point(1,0),at);match.Lock(id,at);}match.Tick(at+.31);yield return null;var projectiles=Object.FindObjectsByType<ProjectileVisualController>();Assert.AreEqual(2,projectiles.Length);var current=projectiles;if(ids==null)ids=current;else CollectionAssert.AreEquivalent(ids,current,"Pooled visuals must be reused between rounds.");match.Tick(at+1.01);yield return null;Assert.AreEqual(0,Object.FindObjectsByType<ProjectileVisualController>().Length);int hp=match.ViewFor("player")[1].Hp;var impact=Object.FindAnyObjectByType<ImpactFeedbackController>();Assert.AreEqual(8,impact.Capacity);for(int j=0;j<20;j++)impact.ConfirmedImpact(Vector2.zero,true);yield return null;Assert.AreEqual(hp,match.ViewFor("player")[1].Hp,"VFX never apply damage.");match.Tick(at+1.61);yield return null;}LogAssert.NoUnexpectedReceived();
 }
 [UnityTest]public IEnumerator QualityLimitsAmbientAndTrailAndReducedMotion(){bool reduced=MotionSettings.Reduced;var quality=MotionSettings.Quality;try{MotionSettings.Reduced=false;MotionSettings.Quality=MotionQuality.Low;Assert.AreEqual(3,MotionSettings.AmbientCount);Assert.AreEqual(2,MotionSettings.TrailCount);MotionSettings.Quality=MotionQuality.High;Assert.AreEqual(10,MotionSettings.AmbientCount);Assert.AreEqual(6,MotionSettings.TrailCount);MotionSettings.Reduced=true;Assert.AreEqual(0,MotionSettings.AmbientCount);Assert.AreEqual(0,MotionSettings.TrailCount);Assert.AreEqual(0,MotionSettings.Strength);yield return null;}finally{MotionSettings.Reduced=reduced;MotionSettings.Quality=quality;}}
}
}
