using UnityEngine;
namespace HitMe.UI {
public sealed class SafeAreaAdapter : MonoBehaviour {
 public Rect? Override; Rect previous; Vector2 screen;
 public void Apply(){var r=Override??WebMobileBridge.SafeArea;if(Screen.width<=0||Screen.height<=0)return;previous=r;screen=new Vector2(Screen.width,Screen.height);var rect=(RectTransform)transform;rect.anchorMin=new Vector2(r.x/Screen.width,r.y/Screen.height);rect.anchorMax=new Vector2(r.xMax/Screen.width,r.yMax/Screen.height);rect.offsetMin=rect.offsetMax=Vector2.zero;}
 void LateUpdate(){if(previous!=(Override??WebMobileBridge.SafeArea)||screen!=new Vector2(Screen.width,Screen.height))Apply();}
}
}
