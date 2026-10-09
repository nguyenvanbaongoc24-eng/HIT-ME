using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using HitMe.UI;
using HitMe.Core;
namespace HitMe.Tests {
public sealed class ProductionAimTests {
 static void Capture(string name){var texture=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.GetFullPath(Path.Combine(Application.dataPath,"../../docs/screenshots/Aim-"+name+".png")),texture.EncodeToPNG());Object.Destroy(texture);}
 [UnityTest] public IEnumerator ActualTouchDragMatchesDirectionEndpointReleaseLockAndThrow(){int saved=ArenaMaps.Selected;ArenaMaps.Selected=0;OfflineRunContext.Requested=false;SceneManager.LoadScene("Battle");yield return null;yield return null;var view=Object.FindAnyObjectByType<BattleView>();
  foreach(var size in new[]{new Vector2Int(360,800),new Vector2Int(390,844),new Vector2Int(402,874),new Vector2Int(430,932)}){
   typeof(BattleSmokeTests).GetMethod("SetGameViewSize",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{size.x,size.y});yield return null;yield return null;
   view.StartOffline(new MatchSettings{BotCount=5,Timeout=TimeoutPolicy.ProposedStayAndSkip});yield return null;yield return null;
   Assert.AreEqual(6,view.Match.Ids.Count);Assert.IsNull(GameObject.Find("AimLine"));var indicator=Object.FindAnyObjectByType<ProductionAimIndicator>();Assert.AreEqual(1,Object.FindObjectsByType<ProductionAimIndicator>().Length);Assert.IsNotNull(indicator.GetComponent<CanvasRenderer>());Assert.IsFalse(indicator.raycastTarget);Assert.AreEqual(AimVisualState.Hidden,indicator.State);
   var directions=new[]{Vector2.up,Vector2.down,new Vector2(-1,1).normalized,new Vector2(1,1).normalized,Vector2.right};string[] labels={"Up","Down","DiagonalLeft","DiagonalRight","NearWall"};
   for(int i=0;i<directions.Length;i++){
    var floor=GameObject.Find("ArenaSandPlaceholder");var touch=new PointerEventData(EventSystem.current){pointerId=73,position=new Vector2(Screen.width*(i==4?.8f:.5f),Screen.height*.4f)};ExecuteEvents.Execute(floor,touch,ExecuteEvents.pointerDownHandler);touch.position+=directions[i]*45;ExecuteEvents.Execute(floor,touch,ExecuteEvents.dragHandler);ExecuteEvents.Execute(floor,touch,ExecuteEvents.pointerUpHandler);yield return null;yield return null;
    var action=view.Match.ViewFor("player")[0].Action;Assert.IsTrue(action.CanThrow);Assert.AreEqual(AimVisualState.Aiming,indicator.State);var d=new Vector2((float)action.Direction.X,(float)action.Direction.Y).normalized;Assert.That(Vector2.Dot(d,indicator.VisualDirection),Is.GreaterThan(.999f));Assert.That((indicator.LaunchOrigin-indicator.GameplayOrigin).magnitude,Is.GreaterThan(1));var endpoint=view.Config.ArenaGeometry.ProjectileCollision(action.Position,action.Direction,view.Config.projectileRadius);var expectedDelta=new Vector2((float)((endpoint.X-action.Position.X)*view.Viewport.Scale),(float)((endpoint.Y-action.Position.Y)*view.Viewport.Scale));Assert.That(Vector2.Distance(indicator.Endpoint-indicator.GameplayOrigin,expectedDelta),Is.LessThan(.02f));Assert.AreEqual(1,view.VisibleActorCount,"Opponents stay private during aim, including six-player matches.");yield return new WaitForSecondsRealtime(.08f);Capture(labels[i]+"-"+size.x+"x"+size.y);
   }
   GameObject.Find("Ready").GetComponent<Button>().onClick.Invoke();yield return null;Assert.AreEqual(InputStage.Locked,view.Match.ViewFor("player")[0].Stage);Assert.AreEqual(AimVisualState.Locked,indicator.State);view.Match.Tick(view.Match.Deadline+.01);yield return null;yield return null;Assert.AreEqual(6,view.VisibleActorCount);yield return new WaitForSecondsRealtime(.05f);Capture("SixCharacters-Locked-Reveal-"+size.x+"x"+size.y);view.Match.Tick(view.Match.PhaseStarted+.31);yield return null;Assert.AreEqual(MatchPhase.Throw,view.Match.Phase);Assert.AreEqual(AimVisualState.Throwing,indicator.State);yield return new WaitForSecondsRealtime(.2f);Assert.AreEqual(AimVisualState.Hidden,indicator.State);
  }ArenaMaps.Selected=saved;LogAssert.NoUnexpectedReceived();
 }
}
}
