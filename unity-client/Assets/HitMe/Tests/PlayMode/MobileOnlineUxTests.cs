using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using HitMe.UI;
namespace HitMe.Tests {
public sealed class MobileOnlineUxTests {
 [UnityTest] public IEnumerator SocketOpenAloneDoesNotAuthenticate(){
  if(NetworkSession.Instance!=null)Object.Destroy(NetworkSession.Instance.gameObject);yield return null;
  var session=NetworkSession.Ensure();session.Receive("{\"type\":\"opened\"}");yield return null;Assert.IsFalse(session.Connected);
  session.Receive("{\"type\":\"welcome\",\"id\":\"qa-user\",\"token\":\"\",\"auth\":\"supabase\"}");yield return null;Assert.IsTrue(session.Connected);Assert.IsTrue(session.SupabaseAuthenticated);
  Object.Destroy(session.gameObject);yield return null;
 }
 [UnityTest] public IEnumerator ReconnectIsBoundedAndExposesFailure(){
  if(NetworkSession.Instance!=null)Object.Destroy(NetworkSession.Instance.gameObject);yield return null;
  var session=NetworkSession.Ensure();for(int i=0;i<4;i++){session.Receive("{\"type\":\"closed\"}");yield return null;}
  Assert.IsFalse(session.Connecting);Assert.IsFalse(session.Connected);Assert.AreEqual("connectionFailed",session.ConnectionStatus);
  Object.Destroy(session.gameObject);yield return null;
 }
 [UnityTest] public IEnumerator AvatarOpensProfileWithoutEndpointAndPracticeRemainsAvailable(){
  SceneManager.LoadScene("MainMenu");yield return null;yield return null;yield return null;
  var view=Object.FindAnyObjectByType<MainMenuView>();view.Content.Find("PlayerStatusWidget/Profile").GetComponent<Button>().onClick.Invoke();yield return null;
  Assert.IsNotNull(view.Content.Find("MenuModal/Panel/ProfilePortrait"));Assert.IsNotNull(view.Content.Find("MenuModal/Panel/DisplayName"));
  Assert.IsNull(view.Content.Find("MenuModal/Panel/Endpoint"));Assert.IsNull(view.Content.Find("MenuModal/Panel/Connect"));
  Assert.IsTrue(view.Content.Find("PracticeEntry").gameObject.activeSelf);view.CloseModal();
  Object.Destroy(NetworkSession.Ensure().gameObject);yield return null;LogAssert.NoUnexpectedReceived();
 }
}
}
