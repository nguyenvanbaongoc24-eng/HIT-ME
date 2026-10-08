using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
namespace HitMe.Editor {
public static class TextMeshProSetup {
 [MenuItem("HIT ME/Prepare Sprint 3B")]
 public static void Configure(){
  if(!Directory.Exists("Assets/TextMesh Pro/Resources")){
   string package=Directory.GetFiles("Library/PackageCache","TMP Essential Resources.unitypackage",SearchOption.AllDirectories).First();throw new System.InvalidOperationException("Import TMP Essential Resources from installed UGUI package first: "+package);
  }
  Build("Nunito","NunitoSDF",48);Build("NotoSansSymbols2-Regular","SymbolsSDF",48);foreach(string id in new[]{"NunitoSDF","SymbolsSDF"}){var font=Resources.Load<TMP_FontAsset>("Fonts/"+id);if(font!=null && font.material.HasProperty("_OutlineWidth")){font.material.EnableKeyword("OUTLINE_ON");font.material.SetFloat("_FaceDilate",.1f);font.material.SetFloat("_OutlineWidth",.09f);font.material.SetColor("_OutlineColor",new Color(.13f,.08f,.07f,1));EditorUtility.SetDirty(font.material);}}AssetDatabase.SaveAssets();
  Debug.Log("HITME_TMP_CONFIGURED");
 }
 static void Build(string source,string name,int size){
  string path="Assets/HitMe/Resources/Fonts/"+name+".asset";var existing=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);if(existing!=null){if(source!="Nunito"||existing.HasCharacter('Ư'))return;AssetDatabase.DeleteAsset(path);}
  Font font=Resources.Load<Font>("Fonts/"+source);if(font==null)throw new System.Exception("Missing source font: "+source);
  var asset=TMP_FontAsset.CreateFontAsset(font,size,6,GlyphRenderMode.SDFAA,2048,2048,AtlasPopulationMode.Dynamic,false);asset.name=name;
  AssetDatabase.CreateAsset(asset,path);
  foreach(var texture in asset.atlasTextures)AssetDatabase.AddObjectToAsset(texture,asset);AssetDatabase.AddObjectToAsset(asset.material,asset);
  string chars=source=="Nunito"?new string(Enumerable.Range(32,95).Select(x=>(char)x).ToArray())+new string(Enumerable.Range(0xc0,0x180-0xc0).Select(x=>(char)x).ToArray())+new string(Enumerable.Range(0x1ea0,0x1efa-0x1ea0).Select(x=>(char)x).ToArray())+"ĐđƠơƯư–—…":"♥★✓✕";
  asset.TryAddCharacters(chars,out string missing); // Unsupported non-Vietnamese Latin glyphs are recorded; all VI glyphs verified separately.
  asset.atlasPopulationMode=AtlasPopulationMode.Static;EditorUtility.SetDirty(asset);
  Debug.Log("TMP_FONT "+name+" missing="+missing);
 }
}}
