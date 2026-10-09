using UnityEngine;
namespace HitMe.Visuals {
public enum LayerMotion { Static, Sway, Flutter, Float, Crowd }
[CreateAssetMenu(menuName="HIT ME/Environment Layer")]
public sealed class EnvironmentLayerDefinition : ScriptableObject {
 public string layerId;
 public Sprite sprite;
 public Vector2 anchor=new Vector2(.5f,.5f),referenceSize=new Vector2(40,60),pivot=new Vector2(.5f,1);
 public LayerMotion motion;
 public float amplitude=2,speed=1;
 [TextArea] public string provenance;
}
}
