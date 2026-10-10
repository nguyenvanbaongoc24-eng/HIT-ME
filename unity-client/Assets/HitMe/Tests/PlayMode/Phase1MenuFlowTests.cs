using System;
using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using HitMe.UI;
namespace HitMe.Tests {
public sealed class Phase1MenuFlowTests {
 string savedEndpoint;
 [UnitySetUp]public IEnumerator IsolateConnection(){savedEndpoint=PlayerPrefs.GetString("NetworkEndpoint","");PlayerPrefs.SetString("NetworkEndpoint","ws://127.0.0.1:1/play");if(NetworkSession.Instance!=null)UnityEngine.Object.Destroy(NetworkSession.Instance.gameObject);yield return null;}
 [UnityTearDown]public IEnumerator RestoreConnection(){PlayerPrefs.SetString("NetworkEndpoint",savedEndpoint);if(NetworkSession.Instance!=null)UnityEngine.Object.Destroy(NetworkSession.Instance.gameObject);yield return null;}
 Button B(Transform root,string path)=>root.Find(path).GetComponent<Button>();
 static Rect Bounds(Transform t){var corners=new Vector3[4];((RectTransform)t).GetWorldCorners(corners);return Rect.MinMaxRect(corners[0].x,corners[0].y,corners[2].x,corners[2].y);}
 [UnityTest]public IEnumerator BackCancelCompactAndBotLaunch(){
  if(NetworkSession.Instance!=null)UnityEngine.Object.Destroy(NetworkSession.Instance.gameObject);yield return null;Strings.Load("vi");OfflineRunContext.Requested=false;SceneManager.LoadScene(MenuSceneIds.Home);yield return null;yield return null;yield return null;yield return null;
  var c=UnityEngine.Object.FindAnyObjectByType<MainMenuController>();var v=c.View;Assert.IsNull(v.Content.Find("PlayerStatusWidget/TrialCurrency"));Assert.IsNull(v.Content.Find("PracticeEntry/ArtFallback"));Assert.AreEqual(MenuPanel.Home,c.State.Panel);
  v.Settings();Assert.AreEqual(MenuPanel.Settings,c.State.Panel);c.Back();Assert.AreEqual(MenuPanel.Home,c.State.Panel);
  c.Online("private");Assert.IsTrue(c.Loading);c.Back();Assert.IsEmpty(c.Pending);Assert.AreEqual(MenuPanel.Home,c.State.Panel);
  c.Online("quick");c.Connect("ws://127.0.0.1:1/play","Phase1-transport-test");Assert.IsTrue(c.Loading);Assert.IsFalse(B(v.Content,"PrimaryActions/QuickMatch").interactable);Assert.IsTrue(v.Content.Find("PrimaryActions/CancelMatchmaking").gameObject.activeSelf);c.Cancel();Assert.IsFalse(c.Loading);Assert.IsEmpty(c.Pending);
  yield return null;UnityEngine.Object.Destroy(NetworkSession.Instance.gameObject);yield return null;
  // A fresh controller isolates the background reconnect timer from layout/bot checks.
  SceneManager.LoadScene(MenuSceneIds.Home);yield return null;yield return null;yield return null;yield return null;c=UnityEngine.Object.FindAnyObjectByType<MainMenuController>();v=c.View;
  v.SafeAreaOverride=new Rect(0,34,Screen.width,Screen.height*.72f);yield return null;yield return null;Assert.IsFalse(v.Content.Find("SecondaryFeatures").gameObject.activeSelf);Assert.IsFalse(Bounds(v.Content.Find("PrimaryActions")).Overlaps(Bounds(v.Content.Find("BottomNavigation"))));Assert.IsFalse(Bounds(v.Content.Find("PracticeEntry")).Overlaps(Bounds(v.Content.Find("BottomNavigation"))));v.SafeAreaOverride=null;yield return null;
  int originalCount=OfflineRunContext.Settings.BotCount;var originalDifficulty=OfflineRunContext.Settings.Difficulty;int originalMap=ArenaMaps.Selected;
  v.Practice();B(v.Content,"MenuModal/Panel/BotCount").onClick.Invoke();int chosen=OfflineRunContext.Settings.BotCount;B(v.Content,"MenuModal/Panel/Difficulty").onClick.Invoke();var chosenDifficulty=OfflineRunContext.Settings.Difficulty;c.Back();v.Practice();Assert.AreEqual(chosen,OfflineRunContext.Settings.BotCount);Assert.AreEqual(chosenDifficulty,OfflineRunContext.Settings.Difficulty);c.Back();
  v.Maps();B(v.Content,"MenuModal/Panel/Map0").onClick.Invoke();c.Back();v.Practice();B(v.Content,"MenuModal/Panel/PlayBots").onClick.Invoke();yield return null;yield return null;var battle=UnityEngine.Object.FindAnyObjectByType<BattleView>();Assert.IsNotNull(battle.Match);Assert.AreEqual(chosen+1,battle.Match.Ids.Count);
  SceneManager.LoadScene(MenuSceneIds.Home);yield return null;yield return null;OfflineRunContext.Settings.BotCount=originalCount;OfflineRunContext.Settings.Difficulty=originalDifficulty;ArenaMaps.Selected=originalMap;LogAssert.NoUnexpectedReceived();
 }
 [UnityTest]public IEnumerator RealServerPrivateQuickAndInventoryRoutes(){
  string url=Environment.GetEnvironmentVariable("HITME_LIVE_TEST_URL");if(string.IsNullOrEmpty(url)){Assert.Ignore("Opt-in: set HITME_LIVE_TEST_URL for local backend integration.");yield break;}
  if(NetworkSession.Instance!=null)UnityEngine.Object.Destroy(NetworkSession.Instance.gameObject);yield return null;Strings.Load("vi");OfflineRunContext.Requested=false;SceneManager.LoadScene(MenuSceneIds.Home);yield return null;yield return null;yield return null;yield return null;
  var c=UnityEngine.Object.FindAnyObjectByType<MainMenuController>();c.Session.Connect(url,"Phase1 UI "+Guid.NewGuid().ToString("N").Substring(0,6));c.Online("private");
  float until=Time.realtimeSinceStartup+10;while(SceneManager.GetActiveScene().name!=MenuSceneIds.Lobby&&Time.realtimeSinceStartup<until)yield return null;Assert.AreEqual(MenuSceneIds.Lobby,SceneManager.GetActiveScene().name);yield return null;yield return null;var net=NetworkSession.Instance;Assert.IsTrue(net.Connected);Assert.IsNotNull(net.Profile);
  GameObject.Find("createRoom").GetComponent<Button>().onClick.Invoke();until=Time.realtimeSinceStartup+5;while(net.Room==null&&Time.realtimeSinceStartup<until)yield return null;Assert.IsNotNull(net.Room);Assert.IsTrue(net.Room.@private);Assert.AreEqual("Waiting",net.Room.phase);net.Command("leave");until=Time.realtimeSinceStartup+5;while(net.Room!=null&&Time.realtimeSinceStartup<until)yield return null;Assert.IsNull(net.Room);
  SceneManager.LoadScene(MenuSceneIds.Home);yield return null;yield return null;yield return null;yield return null;c=UnityEngine.Object.FindAnyObjectByType<MainMenuController>();B(c.View.Content,"PrimaryActions/QuickMatch").onClick.Invoke();until=Time.realtimeSinceStartup+5;while(net.Room==null&&Time.realtimeSinceStartup<until)yield return null;Assert.AreEqual(MenuSceneIds.Home,SceneManager.GetActiveScene().name);Assert.IsNotNull(c.View.Content.Find("MenuModal/Panel/SearchStatus"));Assert.IsNotNull(net.Room);Assert.IsFalse(net.Room.@private);c.Cancel();until=Time.realtimeSinceStartup+5;while(net.Room!=null&&Time.realtimeSinceStartup<until)yield return null;Assert.IsNull(net.Room);
  SceneManager.LoadScene(MenuSceneIds.Home);yield return null;yield return null;yield return null;yield return null;c=UnityEngine.Object.FindAnyObjectByType<MainMenuController>();B(c.View.Content,"BottomNavigation/Inventory").onClick.Invoke();yield return null;yield return null;yield return null;Assert.AreEqual(MenuSceneIds.Lobby,SceneManager.GetActiveScene().name);Assert.IsNotNull(GameObject.Find("CosmeticNotice"));Assert.GreaterOrEqual(net.Profile.inventory.Length,3);Assert.AreEqual(0,net.Profile.coins,"Navigation alone cannot grant a reward.");foreach(var widget in UnityEngine.Object.FindObjectsByType<HitMeWidget>()){if(widget.name!="equip")continue;var card=Bounds(widget.transform);var title=Bounds(widget.title.transform);var selected=Bounds(widget.selection.transform);Assert.GreaterOrEqual(title.yMin,card.yMin-.1f,"Compact inventory title must stay inside the card.");Assert.LessOrEqual(title.yMax,card.yMax+.1f);Assert.GreaterOrEqual(selected.yMin,card.yMin-.1f);Assert.LessOrEqual(selected.yMax,card.yMax+.1f);}yield return null;var shot=ScreenCapture.CaptureScreenshotAsTexture();System.IO.File.WriteAllBytes(System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath,"../../docs/screenshots/Sprint6A-Lobby-Inventory-Actual.png")),shot.EncodeToPNG());UnityEngine.Object.Destroy(shot);UnityEngine.Object.Destroy(net.gameObject);SceneManager.LoadScene(MenuSceneIds.Home);yield return null;yield return null;LogAssert.NoUnexpectedReceived();
 }
}
}
