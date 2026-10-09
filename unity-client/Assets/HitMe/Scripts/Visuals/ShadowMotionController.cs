using UnityEngine;
namespace HitMe.Visuals {
public sealed class ShadowMotionController:MonoBehaviour {
 public bool Focused {get;set;}
 RectTransform rect;Vector2 origin;
 void Awake(){rect=(RectTransform)transform;origin=rect.anchoredPosition;}
 void LateUpdate(){if(Focused||MotionSettings.Reduced||MotionSettings.Quality==MotionQuality.Low)return;rect.anchoredPosition=origin+new Vector2(Mathf.Sin(Time.unscaledTime*.09f),Mathf.Cos(Time.unscaledTime*.07f))*.8f*MotionSettings.Strength;}
}
}
