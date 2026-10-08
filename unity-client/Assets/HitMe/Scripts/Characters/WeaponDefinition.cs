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
        public Sprite FlightSprite => projectileSprite!=null?projectileSprite:heldSprite;
    }
}
