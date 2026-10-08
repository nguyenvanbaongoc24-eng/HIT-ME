using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using HitMe.Core;
using HitMe.UI;
using HitMe.Characters;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace HitMe.Tests {
public class Sprint3BIntegrationTests {
 static Rect Bounds(RectTransform r){var corners=new Vector3[4];r.GetWorldCorners(corners);return Rect.MinMaxRect(corners[0].x,corners[0].y,corners[2].x,corners[2].y);}
 [UnityTest]public IEnumerator SixPortraitSizesKeepRoundedCornerActorsInsideScreenAndAwayFromHud(){
  ArenaMaps.Selected=0;OfflineRunContext.Requested=false;SceneManager.LoadScene("Battle");yield return null;yield return null;
  foreach(var size in new[]{new Vector2Int(360,800),new Vector2Int(390,844),new Vector2Int(393,852),new Vector2Int(402,874),new Vector2Int(412,915),new Vector2Int(430,932)}){
   typeof(BattleSmokeTests).GetMethod("SetGameViewSize",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{size.x,size.y});yield return null;yield return null;
   var view=Object.FindAnyObjectByType<BattleView>();view.SafeAreaOverride=new Rect(0,34,Screen.width,Screen.height-93);yield return null;yield return null;
   Assert.IsInstanceOf<RoundedRectangleArenaGeometry>(view.Config.ArenaGeometry);Assert.IsNotNull(GameObject.Find("ArenaSandPlaceholder").GetComponent<RoundedRectangleGraphic>());
   view.StartOffline(new MatchSettings{BotCount=2,Timeout=TimeoutPolicy.ProposedStayAndSkip});yield return null;
   var m=view.Match;double now=Time.unscaledTimeAsDouble;
   var points=new[]{new Point(0,-500),view.Config.ArenaGeometry.ClampPosition(new Point(-1000,1750),90),view.Config.ArenaGeometry.ClampPosition(new Point(1000,-1750),90)};
   for(int i=0;i<3;i++){Assert.IsTrue(m.Place(m.Ids[i],points[i],now));Assert.IsTrue(m.Aim(m.Ids[i],new Point(i==1?1:-1,0),now));Assert.IsTrue(m.Lock(m.Ids[i],now));}
   yield return null;yield return null;
   Assert.AreEqual(3,view.VisibleActorCount);
   foreach(var visual in Object.FindObjectsByType<CharacterVisual>()){
    Assert.IsTrue(visual.HasSprite);var bounds=Bounds(visual.GetComponent<RectTransform>());Assert.That(bounds.xMin,Is.GreaterThanOrEqualTo(-1));Assert.That(bounds.xMax,Is.LessThanOrEqualTo(Screen.width+1));Assert.That(bounds.yMin,Is.GreaterThanOrEqualTo(0));Assert.That(bounds.yMax,Is.LessThanOrEqualTo(Screen.height));
    foreach(string name in new[]{"Round","Timer","Ready","Chat","Weapon","Settings","Status"})Assert.IsFalse(bounds.Overlaps(Bounds(GameObject.Find(name).GetComponent<RectTransform>())),visual.name+" overlaps "+name);
   }
   foreach(var text in Object.FindObjectsByType<TextMeshProUGUI>()){Assert.IsNotNull(text.font);Assert.IsFalse(text.isTextOverflowing,"TMP overflow "+text.transform.parent.name);}
   var folder=Path.GetFullPath(Path.Combine(Application.dataPath,"../../docs/screenshots"));Directory.CreateDirectory(folder);yield return null;var image=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(folder,"Sprint3B-"+size.x+"x"+size.y+".png"),image.EncodeToPNG());Object.Destroy(image);
  }LogAssert.NoUnexpectedReceived();
 }
 [UnityTest]public IEnumerator MissingMapUsesExplicitFallbackWithoutChangingGeometry(){ArenaMaps.Selected=4;OfflineRunContext.Requested=false;SceneManager.LoadScene("Battle");yield return null;yield return null;var view=Object.FindAnyObjectByType<BattleView>();Assert.IsNull(ArenaMaps.LoadSelected());Assert.IsTrue(GameObject.Find("ArenaLabel").GetComponent<Text>().text.Contains("chưa có art"));Assert.AreEqual(2000,((RoundedRectangleArenaGeometry)view.Config.ArenaGeometry).Width);ArenaMaps.Selected=0;LogAssert.NoUnexpectedReceived();}
 [Test]public void StaticSdfFontsCoverAllVietnameseAndHearts(){var font=Resources.Load<TMP_FontAsset>("Fonts/NunitoSDF");Assert.AreEqual(AtlasPopulationMode.Static,font.atlasPopulationMode);foreach(char c in "SẴN SÀNGĐẤUƯỜƠẠỆLàng quê Bắc BộVịnh Hạ LongCố đô HuếChợ nổi Cái RăngTây Bắc")Assert.IsTrue(font.HasCharacter(c),"Missing "+c);Assert.IsTrue(Resources.Load<TMP_FontAsset>("Fonts/SymbolsSDF").HasCharacter('♥'));}
 [UnityTest]public IEnumerator LowQualityTargetSurvivesMenuToBattleSceneChange(){WebMobileBridge.Ensure();WebMobileBridge.SetQuality(true);SceneManager.LoadScene("MainMenu");yield return null;yield return null;Assert.AreEqual(30,Application.targetFrameRate);OfflineRunContext.Requested=false;SceneManager.LoadScene("Battle");yield return null;yield return null;Assert.AreEqual(30,Application.targetFrameRate);Assert.IsTrue(WebMobileBridge.LowQuality);WebMobileBridge.SetQuality(false);LogAssert.NoUnexpectedReceived();}

}}
