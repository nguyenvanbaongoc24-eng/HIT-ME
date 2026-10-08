using System;
using UnityEngine;
using UnityEngine.U2D;
namespace HitMe.Characters
{
    // A Resources reference independent of character/prefab deserialization.
    public sealed class CharacterAtlasRegistry : ScriptableObject
    {
        public SpriteAtlas atlas;
    }
    public static class CharacterAtlasLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            SpriteAtlasManager.atlasRequested-=Request;
            SpriteAtlasManager.atlasRequested+=Request;
        }
        static void Request(string tag,Action<SpriteAtlas> callback)
        {
            var registry=Resources.Load<CharacterAtlasRegistry>("Characters/CharacterAtlasRegistry");
            if(registry!=null && registry.atlas!=null && registry.atlas.tag==tag)callback(registry.atlas);
        }
    }
}
