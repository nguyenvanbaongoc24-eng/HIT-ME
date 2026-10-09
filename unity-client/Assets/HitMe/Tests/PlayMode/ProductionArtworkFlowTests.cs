using System.Collections;
using System.IO;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using HitMe.UI;
namespace HitMe.Tests {
public sealed class ProductionArtworkFlowTests {
 [UnityTest] public IEnumerator StaticPreviewDoesNotEquipCharacterOrWeapon(){
  int character=CosmeticPreview.Character,weapon=CosmeticPreview.Weapon;Strings.Load("vi");SceneManager.LoadScene("MainMenu");yield return null;yield return null;yield return null;
  var view=Object.FindAnyObjectByType<MainMenuView>();view.ArtworkPreview();yield return null;yield return null;
  Assert.AreSame(view.Theme.staticCharacterPreview,view.Content.Find("MenuModal/Panel/StaticArtwork").GetComponent<Image>().sprite);
  Assert.AreEqual(character,CosmeticPreview.Character);Assert.AreEqual(weapon,CosmeticPreview.Weapon);
  Assert.That(view.Content.Find("MenuModal/Panel/PreviewPolicy").GetComponent<TMP_Text>().text,Does.Contain("Chưa có rig"));
  yield return new WaitForSecondsRealtime(.25f);var image=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.GetFullPath(Path.Combine(Application.dataPath,"../../docs/screenshots/Artwork-Static-Preview.png")),image.EncodeToPNG());Object.Destroy(image);
  view.CloseModal();yield return null;LogAssert.NoUnexpectedReceived();
 }
}
}
