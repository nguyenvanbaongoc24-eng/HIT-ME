using System.IO;
using HitMe.UI;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
namespace HitMe.Editor {
public sealed class ProductionPngImporter : AssetPostprocessor {
 void OnPreprocessTexture(){if(!assetPath.StartsWith("Assets/HitMe/Art/Production/")||!assetPath.EndsWith(".png"))return;
  var i=(TextureImporter)assetImporter;i.textureType=TextureImporterType.Sprite;i.spriteImportMode=SpriteImportMode.Single;i.spritePixelsPerUnit=100;i.mipmapEnabled=false;i.alphaIsTransparency=true;i.filterMode=FilterMode.Bilinear;i.wrapMode=TextureWrapMode.Clamp;i.textureCompression=TextureImporterCompression.Uncompressed;i.maxTextureSize=assetPath.Contains("/MainMenu/village")||assetPath.EndsWith("battle-clean.png")?2048:assetPath.EndsWith("logo.png")?1024:512;
  if(assetPath.Contains("/Weapons/"))i.spritePixelsPerUnit=64;
  if((assetPath.Contains("/Characters/")||assetPath.Contains("/Animals/"))&&!assetPath.Contains("/Portraits/")){i.spritePixelsPerUnit=128;var settings=new TextureImporterSettings();i.ReadTextureSettings(settings);settings.spriteAlignment=(int)SpriteAlignment.Custom;settings.spritePivot=new Vector2(assetPath.EndsWith("NonLaBoy.png")?.28f:assetPath.EndsWith("CapBoy.png")?.23f:.5f,0);i.SetTextureSettings(settings);}
  if(assetPath.EndsWith("terracotta-tile.png")){i.spritePixelsPerUnit=256;i.textureCompression=TextureImporterCompression.Compressed;}
  if(assetPath.EndsWith("stone-tile.png"))i.spritePixelsPerUnit=64;
  if(assetPath.Contains("/UI/")&&!assetPath.EndsWith("logo.png"))i.spriteBorder=new Vector4(64,64,64,64);
 }
}
public static class ProductionArtSetup {
 public const string Root="Assets/HitMe/Art/Production/";
 public static Sprite Sprite(string relative)=>AssetDatabase.LoadAssetAtPath<Sprite>(Root+relative);
 public static void ApplyTheme(){
  if(!File.Exists(Root+"MainMenu/village-clean.png"))return;
  var theme=AssetDatabase.LoadAssetAtPath<UIThemeDefinition>("Assets/HitMe/Resources/UI/MainMenuTheme.asset");if(theme==null)return;
  theme.paper=Sprite("UI/gold.png");theme.brick=Sprite("UI/red.png");theme.card=Sprite("UI/card.png");theme.ink=Sprite("UI/nav.png");theme.logo=Sprite("UI/logo.png");theme.background=Sprite("MainMenu/village-clean.png");
  theme.staticCharacterPreview=Sprite("Characters/pink-static-preview.png");
  theme.featureArtwork=new[]{Sprite("MainMenu/feature-map.png"),Sprite("MainMenu/feature-character.png"),Sprite("MainMenu/feature-collection.png"),Sprite("MainMenu/feature-ranking.png")};
  const string path=Root+"UI/Production.spriteatlas";var atlas=AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);if(atlas==null){atlas=new SpriteAtlas();AssetDatabase.CreateAsset(atlas,path);}atlas.Remove(atlas.GetPackables());atlas.Add(new Object[]{theme.paper,theme.brick,theme.card,theme.ink,theme.logo,theme.shadow,AssetDatabase.LoadAssetAtPath<Object>("Assets/HitMe/Art/UI/Icons")});atlas.SetPackingSettings(new SpriteAtlasPackingSettings{padding=4,enableRotation=false,enableTightPacking=false});atlas.SetTextureSettings(new SpriteAtlasTextureSettings{generateMipMaps=false,filterMode=FilterMode.Bilinear,sRGB=true});atlas.SetIncludeInBuild(true);theme.atlas=atlas;EditorUtility.SetDirty(atlas);EditorUtility.SetDirty(theme);
 }
 [MenuItem("HIT ME/Configure Production Artwork")]
 public static void Configure(){AssetDatabase.Refresh();UIKitSetup.Configure();ArenaArtSetup.Configure();AssetDatabase.SaveAssets();}
 public static void Build(){Configure();HitMeWebBuild.BuildArtwork();}
}
}
