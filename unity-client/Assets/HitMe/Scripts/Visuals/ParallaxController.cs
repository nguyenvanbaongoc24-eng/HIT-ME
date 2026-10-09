using UnityEngine;
namespace HitMe.Visuals {
// Operates on an independent layer container only. Current maps have zero registered depth motion.
public sealed class ParallaxController:MonoBehaviour {
 RectTransform rect;float maximum;Vector2 offset;
 public void Configure(float pixels){rect=(RectTransform)transform;maximum=Mathf.Max(0,pixels);}
 public void SetViewOffset(Vector2 normalized){offset=new Vector2(Mathf.Clamp(normalized.x,-1,1),Mathf.Clamp(normalized.y,-1,1));}
 void LateUpdate(){if(rect!=null)rect.anchoredPosition=MotionSettings.Reduced?Vector2.zero:offset*maximum*MotionSettings.Strength;}
}
}
