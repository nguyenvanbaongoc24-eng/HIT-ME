using UnityEngine;
namespace HitMe.Visuals {
[CreateAssetMenu(menuName="HIT ME/Character Motion")]
public sealed class CharacterMotionProfile : ScriptableObject {
 public float idleBob=.65f, breathing=.006f, aimAngle=3, throwAngle=8, hitShake=1.3f, victoryBounce=1.6f;
}
}
