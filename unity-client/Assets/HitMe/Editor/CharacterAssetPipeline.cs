using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using HitMe.Characters;
using HitMe.UI;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Definition=HitMe.Characters.CharacterDefinition;
namespace HitMe.Editor
{
    public static class CharacterAssetPipeline
    {
        public const int PixelsPerUnit=100;
        public const string ArtRoot="Assets/HitMe/Art/Characters",WeaponRoot="Assets/HitMe/Art/Weapons";
        public const string DefinitionRoot="Assets/HitMe/ScriptableObjects/Characters",PrefabRoot="Assets/HitMe/Prefabs/Characters",AnimationRoot="Assets/HitMe/Animations/Characters";
        public static readonly string[] CharacterIds={"Char01_Player","Char02_BotMale","Char03_BotFemale"};
        static readonly string[] WeaponIds={"DepToOng","Chao","Vot"};
        static readonly string[] DisplayNames={"Nam áo ba lỗ","Nam đội mũ","Nữ thể thao"};
        static readonly string[] Briefs={"Nam, tóc đen, áo ba lỗ trắng, short xanh, dép xanh; dép tổ ong.","Nam, mũ lưỡi trai, trang phục đời thường; chảo.","Nữ, tóc buộc, trang phục thể thao; vợt."};
        [MenuItem("HIT ME/Characters/Import and Validate Library")]
        public static void BuildLibrary()
        {
            foreach(string dir in new[]{ArtRoot,WeaponRoot,DefinitionRoot,PrefabRoot,AnimationRoot,"Assets/HitMe/Resources/Characters"})Directory.CreateDirectory(dir);
            foreach(string id in CharacterIds)foreach(VisualState state in Enum.GetValues(typeof(VisualState)))Directory.CreateDirectory(ArtRoot+"/"+id+"/"+state);
            foreach(string id in WeaponIds)Directory.CreateDirectory(WeaponRoot+"/"+id);
            AssetDatabase.Refresh();
            var report=new StringBuilder("# Character asset validation\n\nReal PNG files only; no concept slicing or invented animation frames.\n\n");
            var definitions=new List<Definition>();var sprites=new List<Sprite>();
            for(int i=0;i<CharacterIds.Length;i++)
            {
                string id=CharacterIds[i];var definition=LoadOrCreate<Definition>(DefinitionRoot+"/"+id+".asset");
                if(string.IsNullOrEmpty(definition.characterId)) { definition.characterId=id;definition.displayName=DisplayNames[i];definition.artBrief=Briefs[i]; }
                var weapon=LoadOrCreate<WeaponDefinition>(DefinitionRoot+"/"+WeaponIds[i]+".asset");
                weapon.weaponId=WeaponIds[i];if(string.IsNullOrEmpty(weapon.displayName))weapon.displayName=WeaponIds[i];
                weapon.heldSprite=LoadPng(WeaponRoot+"/"+WeaponIds[i]+"/held.png",new Vector2(.5f,.5f),report);
                weapon.projectileSprite=LoadPng(WeaponRoot+"/"+WeaponIds[i]+"/projectile.png",new Vector2(.5f,.5f),report);
                weapon.icon=LoadPng(WeaponRoot+"/"+WeaponIds[i]+"/icon.png",new Vector2(.5f,.5f),report);
                definition.defaultWeapon=weapon;definition.portrait=LoadPng(ArtRoot+"/"+id+"/portrait.png",new Vector2(.5f,.5f),report);
                var entries=new List<CharacterAnimation>();
                foreach(VisualState state in Enum.GetValues(typeof(VisualState)))
                {
                    var entry=definition.Animation(state) ?? new CharacterAnimation {state=state,loop=state==VisualState.Idle || state==VisualState.Aim || state==VisualState.Victory};
                    string folder=ArtRoot+"/"+id+"/"+state;
                    var files=Directory.GetFiles(folder,"*.png").Where(path=>System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileName(path),"^frame_[0-9]{3}\\.png$")).OrderBy(path=>path,StringComparer.Ordinal).ToArray();
                    var frames=new List<Sprite>();Vector2? dimensions=null;
                    foreach(string file in files)
                    {
                        var sprite=LoadPng(file.Replace('\\','/'),definition.footPivot,report);if(sprite==null)continue;
                        if(dimensions.HasValue && sprite.rect.size!=dimensions.Value) {report.AppendLine("- REJECT dimensions differ: "+file);continue;}
                        dimensions=sprite.rect.size;frames.Add(sprite);
                    }
                    entry.frames=frames.ToArray();
                    if(frames.Count==0)report.AppendLine("- MISSING "+folder+"/frame_000.png (fallback)");
                    string clipPath=AnimationRoot+"/"+id+"/"+state+".anim";
                    Directory.CreateDirectory(Path.GetDirectoryName(clipPath));
                    if(frames.Count>=2)
                    {
                        AssetDatabase.Refresh();var clip=AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
                        if(clip==null){clip=new AnimationClip();AssetDatabase.CreateAsset(clip,clipPath);}
                        ConfigureClip(clip,entry);entry.clip=clip;EditorUtility.SetDirty(clip);
                    }
                    else entry.clip=null;
                    sprites.AddRange(frames);entries.Add(entry);
                }
                definition.animations=entries.ToArray();
                if(definition.portrait!=null)sprites.Add(definition.portrait);
                foreach(var sprite in new[]{weapon.heldSprite,weapon.projectileSprite,weapon.icon})if(sprite!=null)sprites.Add(sprite);
                string prefabPath=PrefabRoot+"/"+id+".prefab";
                var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if(prefab==null)
                {
                    var go=new GameObject(id,typeof(RectTransform),typeof(Image),typeof(CharacterVisual),typeof(CharacterPresentation));
                    var image=go.GetComponent<Image>();image.raycastTarget=false;image.sprite=definition.Frame(VisualState.Idle,0);image.enabled=image.sprite!=null;image.preserveAspect=true;
                    var rect=go.GetComponent<RectTransform>();rect.pivot=definition.footPivot;rect.sizeDelta=new Vector2(55.5f,74);
                    var serialized=new SerializedObject(go.GetComponent<CharacterVisual>());serialized.FindProperty("definition").objectReferenceValue=definition;serialized.ApplyModifiedPropertiesWithoutUndo();
                    prefab=PrefabUtility.SaveAsPrefabAsset(go,prefabPath);UnityEngine.Object.DestroyImmediate(go);
                }
                // Refresh only generated visual references; preserve the existing prefab hierarchy.
                var contents=PrefabUtility.LoadPrefabContents(prefabPath);
                try
                {
                    var preview=contents.GetComponent<Image>();preview.sprite=definition.Frame(VisualState.Idle,0);preview.enabled=preview.sprite!=null;preview.preserveAspect=true;
                    contents.GetComponent<RectTransform>().pivot=definition.footPivot;
                    PrefabUtility.SaveAsPrefabAsset(contents,prefabPath);
                }
                finally {PrefabUtility.UnloadPrefabContents(contents);}
                definition.visualPrefab=prefab;EditorUtility.SetDirty(definition);EditorUtility.SetDirty(weapon);definitions.Add(definition);
            }
            var catalog=LoadOrCreate<CharacterCatalog>("Assets/HitMe/Resources/Characters/CharacterCatalog.asset");catalog.characters=definitions.ToArray();EditorUtility.SetDirty(catalog);
            string atlasPath=ArtRoot+"/HitMeCharacters.spriteatlas";var atlas=AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
            if(atlas==null){atlas=new SpriteAtlas();AssetDatabase.CreateAsset(atlas,atlasPath);}
            atlas.Remove(atlas.GetPackables());atlas.Add(sprites.Distinct().Cast<UnityEngine.Object>().ToArray());
            var packing=atlas.GetPackingSettings();packing.enableRotation=false;packing.enableTightPacking=false;atlas.SetPackingSettings(packing);atlas.SetIncludeInBuild(true);EditorUtility.SetDirty(atlas);
            catalog.atlas=atlas;EditorUtility.SetDirty(catalog);
            var registry=LoadOrCreate<CharacterAtlasRegistry>("Assets/HitMe/Resources/Characters/CharacterAtlasRegistry.asset");registry.atlas=atlas;EditorUtility.SetDirty(registry);
            ArenaArtSetup.Configure();
            AssetDatabase.SaveAssets();
            if(sprites.Count>0)SpriteAtlasUtility.PackAtlases(new[]{atlas},EditorUserBuildSettings.activeBuildTarget);
            report.Insert(0,"Validated sprite references: "+sprites.Distinct().Count()+"\n\n");
            string output=Path.GetFullPath(Path.Combine(Application.dataPath,"../../docs/SPRINT3A_ASSET_VALIDATION.md"));File.WriteAllText(output,report.ToString());
            Debug.Log("HITME_CHARACTER_LIBRARY: definitions=3, validSprites="+sprites.Distinct().Count()+", report="+output);
        }
        static T LoadOrCreate<T>(string path) where T:ScriptableObject
        { var asset=AssetDatabase.LoadAssetAtPath<T>(path);if(asset==null){asset=ScriptableObject.CreateInstance<T>();AssetDatabase.CreateAsset(asset,path);}return asset; }
        public static bool HasTransparentPixels(byte[] bytes)
        {
            var texture=new Texture2D(2,2,TextureFormat.RGBA32,false);
            try { if(!ImageConversion.LoadImage(texture,bytes,false))return false;var pixels=texture.GetPixels32();return pixels.Any(pixel=>pixel.a<255) && pixels.Any(pixel=>pixel.a>0); }
            finally { UnityEngine.Object.DestroyImmediate(texture); }
        }
        static Sprite LoadPng(string path,Vector2 pivot,StringBuilder report)
        {
            if(!File.Exists(path)) {report.AppendLine("- MISSING "+path);return null;}
            if(!HasTransparentPixels(File.ReadAllBytes(path))) {report.AppendLine("- REJECT opaque/empty/invalid PNG: "+path);return null;}
            var importer=AssetImporter.GetAtPath(path) as TextureImporter;
            if(importer==null){report.AppendLine("- REJECT importer: "+path);return null;}
            var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);
            if(settings.spritePivot!=pivot || settings.spriteAlignment!=(int)SpriteAlignment.Custom){settings.spritePivot=pivot;settings.spriteAlignment=(int)SpriteAlignment.Custom;importer.SetTextureSettings(settings);importer.SaveAndReimport();}
            var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if(sprite!=null)report.AppendLine("- IMPORTED "+path);return sprite;
        }
        public static void ConfigureClip(AnimationClip clip,CharacterAnimation entry)
        {
            if(entry.frames==null || entry.frames.Length<2)throw new ArgumentException("Two real frames required for an authored clip.");
            clip.frameRate=Mathf.Max(1,entry.framesPerSecond);
            var keys=entry.frames.Select((sprite,index)=>new ObjectReferenceKeyframe {time=index/clip.frameRate,value=sprite}).ToList();
            // Extend the last real frame for one frame duration; no synthetic image is produced.
            keys.Add(new ObjectReferenceKeyframe {time=entry.frames.Length/clip.frameRate,value=entry.frames[entry.frames.Length-1]});
            AnimationUtility.SetObjectReferenceCurve(clip,new EditorCurveBinding {path="",type=typeof(Image),propertyName="m_Sprite"},keys.ToArray());
            var settings=AnimationUtility.GetAnimationClipSettings(clip);settings.loopTime=entry.loop;AnimationUtility.SetAnimationClipSettings(clip,settings);
        }
    }
}
