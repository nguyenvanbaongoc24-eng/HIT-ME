using System.Collections;
using System.IO;
using NUnit.Framework;
using HitMe.UI;
using HitMe.Characters;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using TMPro;
namespace HitMe.Tests {
public sealed class MasterProductionFlowTests {
 static readonly string CaptureDirectory=Path.GetFullPath(Path.Combine(Application.dataPath,"../../docs/screenshots/Master-Run-"+System.DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")));
 static void Capture(string name){Directory.CreateDirectory(CaptureDirectory);var texture=ScreenCapture.CaptureScreenshotAsTexture();try{File.WriteAllBytes(Path.Combine(CaptureDirectory,name+".png"),texture.EncodeToPNG());}finally{Object.Destroy(texture);}}
 [UnityTest] public IEnumerator ProductionGalleriesRenderWithoutEquippingUnconfirmedIds(){
  Strings.Load("vi");SceneManager.LoadScene("MainMenu");yield return null;yield return null;yield return null;var view=Object.FindAnyObjectByType<MainMenuView>();int character=CosmeticPreview.Character,weapon=CosmeticPreview.Weapon;
  view.ProductionRoster();yield return null;yield return new WaitForSecondsRealtime(.3f);Assert.AreEqual(7,view.Content.Find("MenuModal").GetComponentsInChildren<CharacterVisual>().Length);foreach(var t in view.Content.Find("MenuModal").GetComponentsInChildren<TMP_Text>())Assert.IsFalse(t.isTextOverflowing,t.text);Capture("Master-CharacterSelection");view.CloseModal();yield return new WaitForSecondsRealtime(.3f);
  view.ProductionWeapons();yield return null;yield return new WaitForSecondsRealtime(.3f);Assert.AreEqual(9,view.Content.Find("MenuModal").GetComponentsInChildren<HitMeWidget>().Length-1);foreach(var t in view.Content.Find("MenuModal").GetComponentsInChildren<TMP_Text>())Assert.IsFalse(t.isTextOverflowing,t.text);Capture("Master-WeaponSelection");Assert.AreEqual(character,CosmeticPreview.Character);Assert.AreEqual(weapon,CosmeticPreview.Weapon);view.CloseModal();yield return new WaitForSecondsRealtime(.3f);view.WeaponArtworkPreview(HitMe.Visuals.ProductionVisualPack.Load().weapons[6]);yield return new WaitForSecondsRealtime(.3f);Assert.IsNotNull(view.Content.Find("MenuModal/Panel/FlightPreview").GetComponent<HitMe.Visuals.ProjectileVisualController>());Capture("Master-WeaponMotionPreview");Assert.AreEqual(weapon,CosmeticPreview.Weapon);view.CloseModal();yield return null;LogAssert.NoUnexpectedReceived();
 }
}
}
