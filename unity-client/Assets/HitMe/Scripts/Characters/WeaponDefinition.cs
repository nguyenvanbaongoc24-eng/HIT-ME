using UnityEngine;
namespace HitMe.Characters
{
    [CreateAssetMenu(menuName="HIT ME/Weapon Visual Definition")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        public string weaponId,displayName;
        public Sprite heldSprite,projectileSprite,icon;
        public Vector2 handAnchor=new Vector2(.7f,.5f);
        public Vector2 gripPivot=new Vector2(.5f,.5f);
        [Min(1)] public float visualSize=14;
        public float flightSpinDegreesPerSecond=650;
        public bool trailEnabled=true;
        [Min(0)] public float trailWidth=1.6f;
        public Color trailColor=new Color(1,.8f,.4f,.28f);
        public Sprite impactSprite; // Artwork only; confirmed impacts remain controlled by combat events.
        public Sprite FlightSprite => projectileSprite!=null?projectileSprite:heldSprite;
    }
}
