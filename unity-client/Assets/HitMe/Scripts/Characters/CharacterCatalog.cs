using UnityEngine;
namespace HitMe.Characters
{
    public sealed class CharacterCatalog : ScriptableObject
    {
        public CharacterDefinition[] characters;
        public UnityEngine.U2D.SpriteAtlas atlas;
        public CharacterDefinition ForSlot(int slot)
        { if(characters==null || characters.Length<3)return null; return characters[slot==0?0:slot%2==1?1:2]; }
        public static CharacterCatalog Load() => Resources.Load<CharacterCatalog>("Characters/CharacterCatalog");
    }
}
