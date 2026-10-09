using UnityEngine;
namespace HitMe.UI
{
    // Decoration only. Gameplay geometry and pointer handling stay in the existing BattleView.
    [CreateAssetMenu(menuName="HIT ME/Arena Art Definition")]
    public sealed class ArenaArtDefinition : ScriptableObject
    {
        public Sprite backdrop,sand;
        public GameObject environmentPrefab;
        public string mapId,displayNameVi,displayNameEn;
        public Sprite previewSprite,borderSprite,centerDecoration,shadowOverlay;
        public HitMe.Visuals.EnvironmentLayerDefinition[] backgroundLayers,foregroundLayers;
        public HitMe.Visuals.EnvironmentMotionProfile motionProfile;
        public HitMe.Visuals.MapAmbientEffectProfile ambientEffectProfile;
        public AudioClip audioProfile;
        public Color lightingProfile=Color.white;
        public HitMe.Visuals.MotionQuality performanceTier=HitMe.Visuals.MotionQuality.High;
        public MapUnlockState unlockState;
        public Sprite floorSprite=>sand;
        public bool ArtworkReady=>backdrop!=null&&sand!=null;
        public bool CanUseOffline=>sand!=null&&unlockState!=MapUnlockState.Locked;
        public string DisplayName=>Strings.Language=="en"?displayNameEn:displayNameVi;
    }
    public enum MapUnlockState {Available,ArtworkMissing,Locked}
}
