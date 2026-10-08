using UnityEngine;
namespace HitMe.UI
{
    // Decoration only. Gameplay geometry and pointer handling stay in the existing BattleView.
    [CreateAssetMenu(menuName="HIT ME/Arena Art Definition")]
    public sealed class ArenaArtDefinition : ScriptableObject
    {
        public Sprite backdrop,sand;
    }
}
