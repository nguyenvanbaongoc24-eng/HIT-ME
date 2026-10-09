using UnityEngine;
namespace HitMe.Visuals {
[CreateAssetMenu(menuName="HIT ME/Map Environment Motion Profile")]
public sealed class EnvironmentMotionProfile:ScriptableObject {
 public bool pauseDuringAimThrow=true;
 [Min(0)] public int lowLayers=1,mediumLayers=2,highLayers=8;
 public float parallaxPixels=0; // Enable only for genuine independent depth layers.
 public int particleCapacity=8;
}
}
