using System;
using UnityEngine;

namespace HitMe.UI
{
    public enum CharacterPose { Idle, Aim, Throw, Hit, Dead, Win }
    [Serializable] public sealed class SpriteState { public string state; public string[] resourcePaths; }
    [Serializable] public sealed class CharacterDefinition
    {
        public string id, gender;
        public string[] customizationSlots;
        public SpriteState[] states;
        public Vector2 feetAnchor;
    }
    [Serializable] public sealed class CharacterManifest
    {
        public int version;
        public CharacterDefinition[] characters;
        public static CharacterManifest Load()
        {
            return JsonUtility.FromJson<CharacterManifest>(Resources.Load<TextAsset>("Art/manifest").text);
        }
        public Sprite LoadSprite(string id, CharacterPose pose)
        {
            foreach (var character in characters)
                if (character.id == id)
                    foreach (var state in character.states)
                        if (state.state == pose.ToString() && state.resourcePaths.Length > 0)
                            return Resources.Load<Sprite>(state.resourcePaths[0]);
            return null; // BattleView labels missing sprites explicitly.
        }
    }
}
