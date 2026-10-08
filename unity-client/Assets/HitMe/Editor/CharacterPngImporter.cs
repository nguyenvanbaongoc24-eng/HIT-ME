using System;
using UnityEditor;
using UnityEngine;
namespace HitMe.Editor
{
    public sealed class CharacterPngImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if(!assetPath.EndsWith(".png",StringComparison.OrdinalIgnoreCase) || !(assetPath.StartsWith(CharacterAssetPipeline.ArtRoot+"/",StringComparison.Ordinal) || assetPath.StartsWith(CharacterAssetPipeline.WeaponRoot+"/",StringComparison.Ordinal)))return;
            var importer=(TextureImporter)assetImporter;
            importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
            importer.spritePixelsPerUnit=CharacterAssetPipeline.PixelsPerUnit;importer.alphaIsTransparency=true;
            importer.mipmapEnabled=false;importer.filterMode=FilterMode.Bilinear;importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=512;
            var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);
            // Preserve the custom foot pivot assigned by the library on subsequent imports.
            if(settings.spriteAlignment!=(int)SpriteAlignment.Custom)
            {
                settings.spriteAlignment=(int)SpriteAlignment.Custom;
                settings.spritePivot=assetPath.StartsWith(CharacterAssetPipeline.WeaponRoot+"/",StringComparison.Ordinal) || assetPath.EndsWith("/portrait.png",StringComparison.OrdinalIgnoreCase)?new Vector2(.5f,.5f):new Vector2(.5f,0);
            }
            importer.SetTextureSettings(settings);
        }
    }
}
