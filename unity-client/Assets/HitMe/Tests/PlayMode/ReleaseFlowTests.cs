using System.Collections;
using HitMe.Audio;
using HitMe.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace HitMe.Tests
{
    public class ReleaseFlowTests
    {
        [UnityTest] public IEnumerator CharacterSelectionRequiresConfirmationAndAudioPreferencesPersist(){
            foreach(var locale in new[]{"vi","en"}){Strings.Load(locale);foreach(var key in new[]{"menuConfirm","saveProfile","copyRoomCode","resultStatsHeader","offlineNoRewards","audioSettings","audioMaster","audioSfx","audioMusic","audioMute","errorpublic_wss_required","errorinvalid_name","errorinvalid_avatar"})Assert.That(Strings.Get(key),Does.Not.StartWith("["),locale+":"+key);}Strings.Load("vi");
            int saved=CosmeticPreview.Character;float volume=HitMeAudio.Master;
            SceneManager.LoadScene("MainMenu");yield return null;yield return null;yield return null;
            var view=Object.FindAnyObjectByType<MainMenuView>();view.Characters();yield return null;
            var selection=view.Content.GetComponentInChildren<HitMePanel>();int next=(saved+1)%3;
            selection.items[next].activated.Invoke();Assert.AreEqual(saved,CosmeticPreview.Character);
            view.Content.Find("MenuModal/Panel/ConfirmCharacter").GetComponent<Button>().onClick.Invoke();yield return null;Assert.AreEqual(next,CosmeticPreview.Character);
            view.AudioSettings();yield return null;
            var slider=view.Content.Find("MenuModal/Panel/MasterSlider").GetComponent<Slider>();slider.value=.41f;
            Assert.AreEqual(.41f,HitMeAudio.Master,.001f);Assert.AreEqual(.41f,PlayerPrefs.GetFloat("AudioMaster"),.001f);
            Assert.IsNotNull(Resources.Load<AudioClip>("Audio/sfx_throw"));
            var audio=HitMeAudio.Ensure();Assert.IsTrue(audio.Remember("release-test-event"));Assert.IsFalse(audio.Remember("release-test-event"));
            yield return new WaitForSecondsRealtime(.35f);Capture("Release-Audio-Settings");
            CosmeticPreview.Character=saved;HitMeAudio.Master=volume;view.CloseModal();LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator RealProfileRenameCharacterConfirmationAndLobbyPortrait(){
            var url=System.Environment.GetEnvironmentVariable("HITME_LIVE_TEST_URL");if(string.IsNullOrEmpty(url))Assert.Ignore("Local backend endpoint not supplied.");
            if(NetworkSession.Instance!=null)Object.Destroy(NetworkSession.Instance.gameObject);yield return null;
            var net=NetworkSession.Ensure();net.Connect(url,"Release UI");float end=Time.realtimeSinceStartup+8;
            while(net.Profile==null&&Time.realtimeSinceStartup<end)yield return null;Assert.IsNotNull(net.Profile);
            string id=net.PlayerId;net.SaveProfile("Ngọc kiểm thử","Char01_Player");end=Time.realtimeSinceStartup+5;
            while(net.Profile.name!="Ngọc kiểm thử"&&Time.realtimeSinceStartup<end)yield return null;Assert.AreEqual("Ngọc kiểm thử",net.Profile.name);
            SceneManager.LoadScene("MainMenu");yield return null;yield return null;yield return null;
            var view=Object.FindAnyObjectByType<MainMenuView>();view.Characters();yield return null;
            var selection=view.Content.GetComponentInChildren<HitMePanel>();selection.items[2].activated.Invoke();Assert.AreEqual("Char01_Player",net.Profile.avatar);
            view.Content.Find("MenuModal/Panel/ConfirmCharacter").GetComponent<Button>().onClick.Invoke();end=Time.realtimeSinceStartup+5;
            while(net.Profile.avatar!="Char03_BotFemale"&&Time.realtimeSinceStartup<end)yield return null;Assert.AreEqual("Char03_BotFemale",net.Profile.avatar);Assert.AreEqual(id,net.PlayerId);Assert.AreEqual(0,net.Profile.coins);
            net.Command("create");end=Time.realtimeSinceStartup+5;while(net.Room==null&&Time.realtimeSinceStartup<end)yield return null;Assert.IsNotNull(net.Room);
            SceneManager.LoadScene("Lobby");yield return null;yield return null;yield return null;
            Assert.IsNotNull(GameObject.Find("LobbyPortrait0").GetComponent<Image>().sprite);
            Assert.That(GameObject.Find("LobbyPlayer0").GetComponent<Text>().text,Does.Contain("Ngọc kiểm thử"));
            yield return null;Capture("Release-Lobby-Profile");
            net.Command("leave");end=Time.realtimeSinceStartup+5;while(net.Room!=null&&Time.realtimeSinceStartup<end)yield return null;
            Object.Destroy(net.gameObject);SceneManager.LoadScene("MainMenu");yield return null;yield return null;LogAssert.NoUnexpectedReceived();
        }
        static readonly string EvidenceDirectory=System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath,"../../docs/screenshots/Release-Run-"+System.DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")));
        static void Capture(string name){System.IO.Directory.CreateDirectory(EvidenceDirectory);var shot=ScreenCapture.CaptureScreenshotAsTexture();System.IO.File.WriteAllBytes(System.IO.Path.Combine(EvidenceDirectory,name+".png"),shot.EncodeToPNG());Object.Destroy(shot);}
        [UnityTearDown] public IEnumerator Cleanup(){if(NetworkSession.Instance!=null)Object.Destroy(NetworkSession.Instance.gameObject);yield return null;}
    }
}
