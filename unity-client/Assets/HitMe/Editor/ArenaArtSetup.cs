using System.IO;
using HitMe.UI;
using UnityEditor;
using UnityEngine;
namespace HitMe.Editor
{
    public sealed class ArenaPngImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if(!assetPath.StartsWith("Assets/HitMe/Art/Arenas/") || !assetPath.EndsWith(".png"))return;
            var importer=(TextureImporter)assetImporter;
            importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
            importer.spritePixelsPerUnit=100;importer.mipmapEnabled=false;importer.wrapMode=TextureWrapMode.Clamp;
            importer.filterMode=FilterMode.Bilinear;importer.textureCompression=TextureImporterCompression.Compressed;
            importer.maxTextureSize=assetPath.EndsWith("/sand.png")?512:2048;
        }
    }
    public static class ArenaArtSetup
    {
        public static void Configure()
        {
            const string art="Assets/HitMe/Art/Arenas/LangQueBacBo/";
            if(!File.Exists(art+"backdrop.png"))return;
            const string folder="Assets/HitMe/Resources/Arenas";Directory.CreateDirectory(folder);AssetDatabase.Refresh();
            const string path=folder+"/LangQueBacBo.asset";
            var definition=AssetDatabase.LoadAssetAtPath<ArenaArtDefinition>(path);
            if(definition==null){definition=ScriptableObject.CreateInstance<ArenaArtDefinition>();AssetDatabase.CreateAsset(definition,path);}
            definition.backdrop=AssetDatabase.LoadAssetAtPath<Sprite>(art+"backdrop.png");
            definition.sand=AssetDatabase.LoadAssetAtPath<Sprite>(art+"sand.png");
            EditorUtility.SetDirty(definition);
        }
    }
}
