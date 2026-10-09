using System.Collections;
using HitMe.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace HitMe.Tests {
public sealed class UIKitFlowTests {
 [UnityTest]public IEnumerator ShowcaseRendersAndPreservesDisabledStates(){
  SceneManager.LoadScene("MainMenu");yield return null;yield return null;
  foreach(var canvas in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))canvas.gameObject.SetActive(false);
  // Showcase prefab is deliberately not in Resources/production scenes; load in Editor only.
#if UNITY_EDITOR
  var asset=UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HitMe/Prefabs/UI/Showcase/UIShowcase.prefab");var showcase=Object.Instantiate(asset);yield return null;yield return null;Canvas.ForceUpdateCanvases();yield return null;
  int disabled=0;foreach(var w in showcase.GetComponentsInChildren<HitMeWidget>()){if(w.initialState==WidgetState.Locked||w.initialState==WidgetState.Disabled||w.initialState==WidgetState.Loading){Assert.IsFalse(w.action.interactable);disabled++;}Assert.IsNotNull(w.title.font);Assert.IsFalse(w.loading.GetComponent<TMPro.TMP_Text>().text.Contains("[menuLoading]"));}Assert.Greater(disabled,20);
  var first=showcase.GetComponentInChildren<HitMeWidget>().GetComponent<RectTransform>();var corners=new Vector3[4];first.GetWorldCorners(corners);Assert.Greater(Vector3.Distance(corners[0],corners[2]),40,"Showcase has a visible render size");
  var folder=System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath,"../../docs/screenshots"));var image=ScreenCapture.CaptureScreenshotAsTexture();System.IO.File.WriteAllBytes(System.IO.Path.Combine(folder,"UIKit-Showcase.png"),image.EncodeToPNG());Object.Destroy(image);Object.Destroy(showcase);
#endif
  SceneManager.LoadScene("MainMenu");yield return null;
 }
 [UnityTest]public IEnumerator IndependentPopupChatAndRealScreenReuse(){
  Strings.Load("vi");var popup=Object.Instantiate(Resources.Load<GameObject>("UI/Kit/HitMePopup")).GetComponent<HitMePanel>();int confirmations=0;popup.confirmed.AddListener(()=>confirmations++);popup.cancelled.AddListener(popup.Close);popup.Open("Kiểm tra","Dữ liệu từ owner");popup.confirm.onClick.Invoke();Assert.AreEqual(1,confirmations);popup.cancel.onClick.Invoke();Assert.IsFalse(popup.gameObject.activeSelf);Object.Destroy(popup.gameObject);
  var chat=Object.Instantiate(Resources.Load<GameObject>("UI/Kit/HitMeChatPanel")).GetComponent<HitMePanel>();string submitted=null;chat.submitted.AddListener(text=>submitted=text);chat.input.text="Xin chào";chat.confirm.onClick.Invoke();Assert.AreEqual("Xin chào",submitted);chat.SetChatAvailable(false,"Offline");Assert.IsFalse(chat.confirm.interactable);Assert.IsFalse(chat.input.interactable);Object.Destroy(chat.gameObject);
  SceneManager.LoadScene("MainMenu");yield return null;yield return null;Assert.Greater(Object.FindAnyObjectByType<MainMenuView>().GetComponentsInChildren<HitMeWidget>().Length,10);
  MenuNavigationService.Lobby();yield return null;yield return null;Assert.Greater(Object.FindAnyObjectByType<NetworkLobbyView>().GetComponentsInChildren<HitMeWidget>().Length,0);SceneManager.LoadScene("MainMenu");yield return null;
 }
}
}
