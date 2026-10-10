using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using HitMe.UI;
using HitMe.Characters;
namespace HitMe.Tests {
public sealed class MainMenuIntegrationTests {
 string savedEndpoint;
 [UnitySetUp]public IEnumerator IsolateLiveBroadcast(){savedEndpoint=PlayerPrefs.GetString("NetworkEndpoint","");PlayerPrefs.SetString("NetworkEndpoint","ws://127.0.0.1:1/play");if(NetworkSession.Instance!=null)Object.Destroy(NetworkSession.Instance.gameObject);yield return null;}
 [UnityTearDown]public IEnumerator RestoreEndpoint(){PlayerPrefs.SetString("NetworkEndpoint",savedEndpoint);if(NetworkSession.Instance!=null)Object.Destroy(NetworkSession.Instance.gameObject);yield return null;}
 static Rect Bounds(RectTransform r){var c=new Vector3[4];r.GetWorldCorners(c);return Rect.MinMaxRect(c[0].x,c[0].y,c[2].x,c[2].y);}
 static Button Button(Transform root,string path)=>root.Find(path).GetComponent<Button>();
 [UnityTest]public IEnumerator MenuSevenSizesSafeAreaLocalizationAndNavigation(){
  Strings.Load("vi");OfflineRunContext.Requested=false;if(NetworkSession.Instance!=null)Object.Destroy(NetworkSession.Instance.gameObject);yield return null;
  SceneManager.LoadScene("MainMenu");yield return null;yield return null;yield return null;yield return null;
  var view=Object.FindAnyObjectByType<MainMenuView>();Assert.IsNotNull(view);Assert.IsNotNull(view.Theme.entryPrefab);Assert.IsNotNull(view.Theme.atlas);Assert.Greater(view.Theme.paper.border.x,0);Assert.IsNotNull(view.Content.Find("PlayerStatusWidget/Profile/Avatar").GetComponent<Image>().sprite);
  foreach(var size in new[]{new Vector2Int(360,800),new Vector2Int(375,812),new Vector2Int(390,844),new Vector2Int(393,852),new Vector2Int(402,874),new Vector2Int(412,915),new Vector2Int(430,932)}){
   typeof(BattleSmokeTests).GetMethod("SetGameViewSize",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{size.x,size.y});yield return null;yield return null;
   view.SafeAreaOverride=new Rect(0,34,Screen.width,Screen.height-93);yield return null;yield return null;
   var safe=Bounds(view.Content);foreach(string path in new[]{"PlayerStatusWidget","PrimaryActions/QuickMatch","PrimaryActions/PrivateRoom","SecondaryFeatures","PracticeEntry","BottomNavigation"}){
    var b=Bounds(view.Content.Find(path).GetComponent<RectTransform>());Assert.GreaterOrEqual(b.xMin,safe.xMin-1,path);Assert.LessOrEqual(b.xMax,safe.xMax+1,path);Assert.GreaterOrEqual(b.yMin,safe.yMin-1,path);Assert.LessOrEqual(b.yMax,safe.yMax+1,path);
   }
   var quick=Bounds(view.Content.Find("PrimaryActions/QuickMatch").GetComponent<RectTransform>());var other=Bounds(view.Content.Find("PrimaryActions/PrivateRoom").GetComponent<RectTransform>());Assert.IsFalse(quick.Overlaps(other));
   var character=view.Content.Find("CharacterPresentation/SelectedCharacter").GetComponent<CharacterVisual>();Assert.IsTrue(character.HasSprite);Assert.IsFalse(Bounds(character.GetComponent<RectTransform>()).Overlaps(quick));
   foreach(var b in view.Content.GetComponentsInChildren<Button>()){var bounds=Bounds(b.GetComponent<RectTransform>());Assert.GreaterOrEqual(bounds.width,43.5f,b.name);Assert.GreaterOrEqual(bounds.height,43.5f,b.name);}
   foreach(var text in view.Content.GetComponentsInChildren<TextMeshProUGUI>()){Assert.IsNotNull(text.font);Assert.IsFalse(text.text.Contains("[menu"),text.name);Assert.IsFalse(text.isTextOverflowing,text.transform.parent.name+" "+text.text);}
   var folder=Path.GetFullPath(Path.Combine(Application.dataPath,"../../docs/screenshots"));Directory.CreateDirectory(folder);var image=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(folder,"Sprint5-Menu-SafeArea-"+size.x+"x"+size.y+".png"),image.EncodeToPNG());Object.Destroy(image);
  }
  view.SafeAreaOverride=null;view.Settings();yield return null;Button(view.Content,"MenuModal/Panel/Language").onClick.Invoke();yield return null;Assert.AreEqual("en",Strings.Language);Assert.AreEqual("QUICK PLAY",view.Content.Find("PrimaryActions/QuickMatch/Label").GetComponent<TextMeshProUGUI>().text);view.CloseModal();
  view.Practice();yield return null;int before=OfflineRunContext.Settings.BotCount;Button(view.Content,"MenuModal/Panel/BotCount").onClick.Invoke();yield return null;Assert.AreNotEqual(before,OfflineRunContext.Settings.BotCount);view.CloseModal();
  view.Maps();yield return null;Button(view.Content,"MenuModal/Panel/Map2").onClick.Invoke();yield return null;Button(view.Content,"MenuModal/Panel/ConfirmMap").onClick.Invoke();yield return null;Assert.AreEqual(2,ArenaMaps.Selected);view.CloseModal();ArenaMaps.Selected=0;
  Button(view.Content,"SecondaryFeatures/Ranking").onClick.Invoke();yield return null;Assert.IsTrue(view.Content.Find("MenuModal/Panel/Message").GetComponent<TextMeshProUGUI>().text.Contains("Coming soon"));view.CloseModal();
  Button(view.Content,"PrimaryActions/QuickMatch").onClick.Invoke();yield return null;Assert.IsNull(view.Content.Find("MenuModal/Panel/Endpoint"));Assert.IsNotNull(view.Content.Find("MenuModal/Panel/CancelSearch"));Button(view.Content,"MenuModal/Panel/CancelSearch").onClick.Invoke();yield return null;Assert.IsTrue(Button(view.Content,"PrimaryActions/QuickMatch").interactable);
  // A fixture snapshot verifies binding only; no development data is embedded in the product UI.
  var net=NetworkSession.Instance;net.Receive("{\"type\":\"state\",\"profile\":{\"id\":\"fixture\",\"name\":\"Fixture Profile\",\"level\":4,\"coins\":217,\"xp\":321,\"avatar\":\"Char01_Player\",\"equipped\":\"chao\"}}");yield return null;yield return null;
  Assert.AreEqual("217",view.Content.Find("PlayerStatusWidget/CurrencyWidget").GetComponent<TextMeshProUGUI>().text);Assert.AreEqual("Fixture Profile",view.Content.Find("PlayerStatusWidget/PlayerName").GetComponent<TextMeshProUGUI>().text);
  Object.Destroy(net.gameObject);Strings.Load("vi");SceneManager.LoadScene("Battle");yield return null;yield return null;Assert.IsNotNull(Object.FindAnyObjectByType<BattleView>());LogAssert.NoUnexpectedReceived();
 }
}
}
