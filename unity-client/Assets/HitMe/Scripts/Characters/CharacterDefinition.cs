using System;
using UnityEngine;
namespace HitMe.Characters
{
    public enum VisualState { Idle, Aim, Throw, Hit, Eliminated, Victory }
    public enum FacingDirection { Left = -1, Right = 1 }
    [Serializable] public sealed class CharacterAnimation
    {
        public VisualState state;
        public Sprite[] frames = Array.Empty<Sprite>();
        public AnimationClip clip;
        [Min(1)] public float framesPerSecond = 8;
        public bool loop;
        public Sprite Sample(float seconds)
        {
            if(frames == null || frames.Length == 0) return null;
            int index=Mathf.Max(0,Mathf.FloorToInt(seconds*Mathf.Max(1,framesPerSecond)));
            index=loop?index%frames.Length:Mathf.Min(index,frames.Length-1);
            return frames[index];
        }
    }
    [CreateAssetMenu(menuName="HIT ME/Character Definition")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        public string characterId;
        public string displayName;
        [TextArea] public string artBrief;
        public CharacterAnimation[] animations = Array.Empty<CharacterAnimation>();
        public WeaponDefinition defaultWeapon;
        [Tooltip("Artwork already includes a held weapon; suppress an overlapping cosmetic sprite.")] public bool bakedHeldWeapon;
        public Vector2 footPivot = new Vector2(.5f,0);
        [Min(.1f)] public float visualScale=1;
        [Range(70,75)] public float referenceHeight=74;
        public FacingDirection authoredFacing=FacingDirection.Right;
        public Sprite portrait,shadow;
        public HitMe.Visuals.CharacterMotionProfile motionProfile;
        public GameObject visualPrefab;
        public CharacterAnimation Animation(VisualState state)
        { foreach(var a in animations) if(a!=null && a.state==state)return a; return null; }
        public Sprite Frame(VisualState state,float elapsed)
        { var frame=Animation(state)?.Sample(elapsed); return frame!=null?frame:Animation(VisualState.Idle)?.Sample(elapsed); }
    }
}
