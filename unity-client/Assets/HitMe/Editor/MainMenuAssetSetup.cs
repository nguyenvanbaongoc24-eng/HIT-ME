using System.IO;
using HitMe.UI;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
namespace HitMe.Editor {
public sealed class MainMenuPngImporter : AssetPostprocessor {
 void OnPreprocessTexture(){if(!assetPath.StartsWith("Assets/HitMe/Art/UI/")||!assetPath.EndsWith(".png"))return;var i=(TextureImporter)assetImporter;i.textureType=TextureImporterType.Sprite;i.spriteImportMode=SpriteImportMode.Single;i.spritePixelsPerUnit=100;i.mipmapEnabled=false;i.alphaIsTransparency=true;i.filterMode=FilterMode.Bilinear;i.wrapMode=TextureWrapMode.Clamp;i.textureCompression=TextureImporterCompression.Uncompressed;i.maxTextureSize=1024;if(assetPath.Contains("/Buttons/"))i.spriteBorder=new Vector4(32,32,32,32);}
}
public static class MainMenuAssetSetup {
 [MenuItem("HIT ME/Configure Main Menu Assets")]
 public static void Configure(){const string art="Assets/HitMe/Art/UI/";Directory.CreateDirectory("Assets/HitMe/Resources/UI");Directory.CreateDirectory("Assets/HitMe/ScriptableObjects/UI");Directory.CreateDirectory("Assets/HitMe/Prefabs/UI");AssetDatabase.Refresh();
  const string themePath="Assets/HitMe/Resources/UI/MainMenuTheme.asset";var theme=AssetDatabase.LoadAssetAtPath<UIThemeDefinition>(themePath);if(theme==null){theme=ScriptableObject.CreateInstance<UIThemeDefinition>();AssetDatabase.CreateAsset(theme,themePath);}
  theme.paper=AssetDatabase.LoadAssetAtPath<Sprite>(art+"Buttons/paper.png");theme.brick=AssetDatabase.LoadAssetAtPath<Sprite>(art+"Buttons/brick.png");theme.ink=AssetDatabase.LoadAssetAtPath<Sprite>(art+"Buttons/ink.png");theme.card=AssetDatabase.LoadAssetAtPath<Sprite>(art+"Buttons/card.png");theme.logo=AssetDatabase.LoadAssetAtPath<Sprite>(art+"MainMenu/logo.png");theme.shadow=AssetDatabase.LoadAssetAtPath<Sprite>(art+"MainMenu/shadow.png");theme.background=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/HitMe/Art/Arenas/LangQueBacBo/backdrop.png");
  theme.iconIds=new[]{"home","shop","bag","map","people","settings","mail","trophy","quest","arrow","cancel","coin","practice"};theme.icons=new Sprite[theme.iconIds.Length];for(int i=0;i<theme.icons.Length;i++)theme.icons[i]=AssetDatabase.LoadAssetAtPath<Sprite>(art+"Icons/"+theme.iconIds[i]+".png");
  const string atlasPath="Assets/HitMe/Art/UI/MainMenu/MainMenu.spriteatlas";var atlas=AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);if(atlas==null){atlas=new SpriteAtlas();AssetDatabase.CreateAsset(atlas,atlasPath);}atlas.Remove(atlas.GetPackables());atlas.Add(new Object[]{AssetDatabase.LoadAssetAtPath<Object>(art+"Buttons"),AssetDatabase.LoadAssetAtPath<Object>(art+"Icons"),theme.logo,theme.shadow});atlas.SetPackingSettings(new SpriteAtlasPackingSettings{padding=4,enableRotation=false,enableTightPacking=false});atlas.SetTextureSettings(new SpriteAtlasTextureSettings{generateMipMaps=false,filterMode=FilterMode.Bilinear,sRGB=true});atlas.SetIncludeInBuild(true);theme.atlas=atlas;EditorUtility.SetDirty(atlas);
  const string prefabPath="Assets/HitMe/Prefabs/UI/MainMenu.prefab";var root=new GameObject("MainMenuController");root.AddComponent<MainMenuController>();theme.entryPrefab=PrefabUtility.SaveAsPrefabAsset(root,prefabPath);Object.DestroyImmediate(root);EditorUtility.SetDirty(theme);ProductionArtSetup.ApplyTheme();AssetDatabase.SaveAssets();Debug.Log("Main menu theme/prefab/atlas configured. Environment and portrait art still fallback.");
 }
}
}
