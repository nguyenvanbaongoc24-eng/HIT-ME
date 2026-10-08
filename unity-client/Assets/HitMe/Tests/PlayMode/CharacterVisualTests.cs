using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HitMe.Characters;
using HitMe.Core;
using HitMe.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Definition=HitMe.Characters.CharacterDefinition;
namespace HitMe.Tests
{
    public sealed class CharacterVisualTests
    {
        [UnityTest] public IEnumerator ThreeCharacterSlotsUseOneRendererAndPreserveFeet()
        {
            OfflineRunContext.Requested=false;SceneManager.LoadScene("Battle");yield return null;yield return null;
            var visuals=Object.FindObjectsByType<CharacterVisual>();Assert.AreEqual(3,visuals.Length);
            CollectionAssert.AreEquivalent(new[]{"Char01_Player","Char02_BotMale","Char03_BotFemale"},visuals.Select(v=>v.Definition.characterId));
            var view=Object.FindAnyObjectByType<BattleView>();
            foreach(var visual in visuals)
            {
                var feet=visual.transform.parent.GetComponent<RectTransform>();var ring=feet.Find("FeetHitbox").GetComponent<RectTransform>();
                Assert.AreEqual(Vector2.zero,ring.anchoredPosition);Assert.That(ring.sizeDelta.x,Is.EqualTo(2*90*view.Viewport.Scale).Within(.01));
                Assert.IsTrue(visual.HasSprite,"Production Idle PNG must be connected.");
                Assert.IsFalse(visual.transform.Find("MissingSpriteLabel").gameObject.activeSelf);
                Assert.IsNotNull(visual.Definition.defaultWeapon.heldSprite);
                Assert.IsNotNull(feet.Find("FootShadow"));Assert.AreEqual(1,visual.GetComponents<CharacterVisual>().Length);
                Assert.AreSame(visual,visual.GetComponent<CharacterPresentation>().Visual);
            }
            var image=ScreenCapture.CaptureScreenshotAsTexture();var folder=Path.GetFullPath(Path.Combine(Application.dataPath,"../../docs/screenshots"));Directory.CreateDirectory(folder);File.WriteAllBytes(Path.Combine(folder,"Sprint3A-Battle-Sprites.png"),image.EncodeToPNG());Object.Destroy(image);
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest] public IEnumerator SpriteFramesFacingWeaponAndEveryPoseCannotMoveHitbox()
        {
            // In-memory QA swatches, deliberately not saved/imported as character art.
            var texture=new Texture2D(2,2);var first=Sprite.Create(texture,new Rect(0,0,2,2),new Vector2(.5f,0),100);var second=Sprite.Create(texture,new Rect(0,0,1,2),new Vector2(.5f,0),100);
            var definition=ScriptableObject.CreateInstance<Definition>();var weapon=ScriptableObject.CreateInstance<WeaponDefinition>();weapon.heldSprite=first;definition.defaultWeapon=weapon;
            definition.animations=System.Enum.GetValues(typeof(VisualState)).Cast<VisualState>().Select(state=>new CharacterAnimation {state=state,frames=new[]{first,second},framesPerSecond=8}).ToArray();
            var feet=new GameObject("QA Feet",typeof(RectTransform));var ring=new GameObject("QA Hitbox",typeof(RectTransform));ring.transform.SetParent(feet.transform,false);
            var body=new GameObject("QA Visual",typeof(RectTransform),typeof(Image),typeof(CharacterVisual));body.transform.SetParent(feet.transform,false);var visual=body.GetComponent<CharacterVisual>();
            try
            {
                feet.transform.position=new Vector3(40,50,0);var original=feet.transform.position;var hitbox=ring.transform.localToWorldMatrix;visual.Initialize(definition);
                var events=new List<VisualState>();visual.StateChanged+=events.Add;
                visual.SetFacing(Vector2.left);visual.Present(VisualState.Idle,Color.green,0);visual.Fit(74);
                Assert.AreEqual(FacingDirection.Left,visual.Facing);Assert.AreEqual(-1,body.transform.localScale.x);Assert.AreSame(first,body.GetComponent<Image>().sprite);
                visual.Present(VisualState.Idle,Color.green,.13f);Assert.AreSame(second,body.GetComponent<Image>().sprite);Assert.AreEqual(1,events.Count);
                visual.SetFacing(Vector2.right);visual.Present(VisualState.Aim,Color.green,.2f);visual.Fit(74);Assert.AreEqual(1,body.transform.localScale.x);Assert.IsTrue(body.transform.Find("HeldWeaponSprite").gameObject.activeSelf);
                foreach(VisualState state in System.Enum.GetValues(typeof(VisualState)))
                {
                    visual.Present(state,Color.white,1);visual.Fit(74);
                    Assert.AreEqual(original,feet.transform.position);Assert.AreEqual(hitbox,ring.transform.localToWorldMatrix);Assert.AreEqual(Vector2.zero,body.GetComponent<RectTransform>().anchoredPosition);
                    Assert.AreEqual(original,body.transform.TransformPoint(Vector3.zero),"Sprite pivot remains at feet.");
                    if(state==VisualState.Throw || state==VisualState.Eliminated)Assert.IsFalse(body.transform.Find("HeldWeaponSprite").gameObject.activeSelf);
                }
                definition.animations=new[]{new CharacterAnimation {state=VisualState.Idle,frames=new[]{first}}};
                visual.Present(VisualState.Hit,Color.white,2);Assert.AreSame(first,body.GetComponent<Image>().sprite,"Missing pose reuses real idle frame.");
                definition.animations=System.Array.Empty<CharacterAnimation>();visual.Present(VisualState.Idle,Color.green,3);Assert.IsFalse(visual.HasSprite);Assert.AreEqual(Color.green,body.GetComponent<Image>().color);
                yield return null;LogAssert.NoUnexpectedReceived();
            }
            finally {Object.Destroy(feet);Object.Destroy(definition);Object.Destroy(weapon);Object.Destroy(first);Object.Destroy(second);Object.Destroy(texture);}
        }
        [UnityTest] public IEnumerator CombatSnapshotDrivesThrowHitEliminationAndVictory()
        {
            OfflineRunContext.Requested=false;SceneManager.LoadScene("Battle");yield return null;yield return null;
            var view=Object.FindAnyObjectByType<BattleView>();view.StartOffline(new MatchSettings {BotCount=1,Timeout=TimeoutPolicy.ProposedStayAndSkip});yield return null;
            var human=Object.FindObjectsByType<CharacterVisual>(FindObjectsInactive.Include).Single(v=>v.Definition.characterId=="Char01_Player");var bot=Object.FindObjectsByType<CharacterVisual>(FindObjectsInactive.Include).Single(v=>v.Definition.characterId=="Char02_BotMale");
            var events=new List<VisualState>();bot.StateChanged+=events.Add;var m=view.Match;
            for(int round=0;round<3;round++)
            {
                double at=m.PhaseStarted+.01;
                foreach(string id in m.Ids) {m.Place(id,new Point(id=="player"?-200:200,0),at);m.Aim(id,new Point(1,0),at);m.Lock(id,at);}
                yield return null;m.Tick(at+.31);yield return null;Assert.AreEqual(VisualState.Throw,bot.State);Assert.AreEqual(VisualState.Throw,human.State);
                m.Tick(at+1.01);yield return null;Assert.AreEqual(round==2?VisualState.Eliminated:VisualState.Hit,bot.State);
                Assert.That(bot.transform.parent.GetComponent<RectTransform>().anchoredPosition.x,Is.EqualTo(200*view.Viewport.Scale).Within(.01));
                if(round==2){Assert.AreEqual(VisualState.Victory,human.State);Assert.IsTrue(bot.gameObject.activeInHierarchy);}
                else {m.Tick(at+1.61);yield return null;}
            }
            Assert.AreEqual(2,events.Count(s=>s==VisualState.Hit));Assert.AreEqual(1,events.Count(s=>s==VisualState.Eliminated));Assert.AreEqual(0,m.ViewFor("player")[1].Hp);LogAssert.NoUnexpectedReceived();
        }
    }
}
