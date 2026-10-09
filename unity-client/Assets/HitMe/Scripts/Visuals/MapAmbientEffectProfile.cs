using UnityEngine;
namespace HitMe.Visuals {
[CreateAssetMenu(menuName="HIT ME/Map Ambient FX Profile")]
public sealed class MapAmbientEffectProfile:ScriptableObject {
 public Sprite[] sprites=new Sprite[0];
 [Range(0,32)] public int capacity=8;
 [Min(.1f)] public float lifetime=4;
 [Min(0)] public float particlesPerSecond=1;
 public Vector2 drift=new Vector2(0,-4);
 public Vector2 spriteSize=new Vector2(8,8);
}
}
