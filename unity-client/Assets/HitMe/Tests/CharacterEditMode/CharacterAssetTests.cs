using System;
using System.IO;
using System.Linq;
using HitMe.Characters;
using HitMe.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace HitMe.Tests
{
    public sealed class CharacterAssetTests
    {
        [Test] public void ThreeDefinitionsPrefabsAndAtlasExistWithoutFakeArt()
        {
            var catalog=CharacterCatalog.Load();Assert.IsNotNull(catalog);Assert.AreEqual(3,catalog.characters.Length);
            foreach(var d in catalog.characters)
            {
                Assert.IsNotNull(d);Assert.IsNotNull(d.visualPrefab);Assert.IsNotNull(d.defaultWeapon);
                Assert.AreEqual(6,d.animations.Length);Assert.IsNotEmpty(d.characterId);Assert.IsNotEmpty(d.displayName);
                Assert.IsNotNull(d.visualPrefab.GetComponent<CharacterVisual>());
                Assert.That(d.referenceHeight,Is.InRange(70,75));Assert.That(d.footPivot.x,Is.InRange(0,1));Assert.That(d.footPivot.y,Is.InRange(0,1));
                var idle=d.Frame(VisualState.Idle,0);if(idle!=null)Assert.That(Vector2.Distance(idle.pivot/idle.rect.size,d.footPivot),Is.LessThan(.005f));
                foreach(var state in d.animations) if(state.frames.Length==0)Assert.IsNull(state.clip);
            }
            Assert.AreEqual("Char01_Player",catalog.ForSlot(0).characterId);Assert.AreEqual("Char02_BotMale",catalog.ForSlot(1).characterId);Assert.AreEqual("Char03_BotFemale",catalog.ForSlot(2).characterId);
            Assert.IsNotNull(AssetDatabase.LoadAssetAtPath<UnityEngine.U2D.SpriteAtlas>(CharacterAssetPipeline.ArtRoot+"/HitMeCharacters.spriteatlas"));
        }
        [Test] public void PngImporterUsesSingleSpritePpuAndFootPivotAndRealClipReferences()
        {
            const string folder="Assets/HitMe/Art/Characters/__PipelineTest";
            Assert.IsFalse(AssetDatabase.IsValidFolder(folder),"Never replace an existing user folder.");
            Texture2D texture=null;AnimationClip clip=null;
            try
            {
                Directory.CreateDirectory(folder);
                // 2x2 QA swatch only; never a character asset or deliverable art.
                texture=new Texture2D(2,2,TextureFormat.RGBA32,false);texture.SetPixels(new[]{Color.clear,Color.white,Color.white,Color.clear});texture.Apply();
                byte[] bytes=texture.EncodeToPNG();Assert.IsTrue(CharacterAssetPipeline.HasTransparentPixels(bytes));
                File.WriteAllBytes(folder+"/frame_000.png",bytes);File.WriteAllBytes(folder+"/frame_001.png",bytes);AssetDatabase.Refresh();
                var importer=(TextureImporter)AssetImporter.GetAtPath(folder+"/frame_000.png");
                Assert.AreEqual(TextureImporterType.Sprite,importer.textureType);Assert.AreEqual(SpriteImportMode.Single,importer.spriteImportMode);Assert.AreEqual(100,importer.spritePixelsPerUnit);Assert.IsFalse(importer.mipmapEnabled);
                var a=AssetDatabase.LoadAssetAtPath<Sprite>(folder+"/frame_000.png");var b=AssetDatabase.LoadAssetAtPath<Sprite>(folder+"/frame_001.png");
                Assert.AreEqual(new Vector2(.5f,0),a.pivot/a.rect.size);
                var custom=new TextureImporterSettings();importer.ReadTextureSettings(custom);custom.spritePivot=new Vector2(.5f,.25f);importer.SetTextureSettings(custom);importer.SaveAndReimport();
                a=AssetDatabase.LoadAssetAtPath<Sprite>(folder+"/frame_000.png");Assert.AreEqual(new Vector2(.5f,.25f),a.pivot/a.rect.size,"Reimport must preserve definition foot pivot.");
                var animation=new CharacterAnimation {state=VisualState.Throw,frames=new[]{a,b},framesPerSecond=8};
                clip=new AnimationClip();CharacterAssetPipeline.ConfigureClip(clip,animation);
                var binding=AnimationUtility.GetObjectReferenceCurveBindings(clip).Single();Assert.AreEqual(typeof(Image),binding.type);Assert.AreEqual("m_Sprite",binding.propertyName);
                var keys=AnimationUtility.GetObjectReferenceCurve(clip,binding);Assert.AreSame(a,keys[0].value);Assert.AreSame(b,keys[1].value);Assert.IsTrue(keys.All(k=>k.value==a || k.value==b));
                Assert.AreSame(a,animation.Sample(0));Assert.AreSame(b,animation.Sample(.125f));Assert.AreSame(b,animation.Sample(100));animation.loop=true;Assert.AreSame(a,animation.Sample(.25f));
                Assert.Throws<ArgumentException>(()=>CharacterAssetPipeline.ConfigureClip(clip,new CharacterAnimation {frames=new[]{a}}));
                texture.SetPixels(new[]{Color.white,Color.white,Color.white,Color.white});texture.Apply();Assert.IsFalse(CharacterAssetPipeline.HasTransparentPixels(texture.EncodeToPNG()));
                texture.SetPixels(new[]{Color.clear,Color.clear,Color.clear,Color.clear});texture.Apply();Assert.IsFalse(CharacterAssetPipeline.HasTransparentPixels(texture.EncodeToPNG()));
            }
            finally { if(texture!=null)UnityEngine.Object.DestroyImmediate(texture);if(clip!=null)UnityEngine.Object.DestroyImmediate(clip);AssetDatabase.DeleteAsset(folder); }
        }
    }
}
