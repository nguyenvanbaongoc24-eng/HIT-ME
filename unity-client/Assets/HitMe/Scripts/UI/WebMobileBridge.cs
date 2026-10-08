using System.Runtime.InteropServices;
using UnityEngine;
namespace HitMe.UI {
public sealed class WebMobileBridge : MonoBehaviour {
 static WebMobileBridge instance;static Vector4 insets;
 public static bool LowQuality=>PlayerPrefs.GetInt("LowQuality",0)==1;
 public static Rect SafeArea {get{if(insets==Vector4.zero)return Screen.safeArea;return new Rect(insets.x*Screen.width,insets.w*Screen.height,(1-insets.x-insets.z)*Screen.width,(1-insets.y-insets.w)*Screen.height);}}
 public static void Ensure(){if(instance!=null)return;var go=new GameObject("WebMobileBridge");instance=go.AddComponent<WebMobileBridge>();DontDestroyOnLoad(go);SetQuality(LowQuality);}
 public static void SetQuality(bool low){PlayerPrefs.SetInt("LowQuality",low?1:0);Application.targetFrameRate=low?30:60;QualitySettings.vSyncCount=0;PlayerPrefs.Save();}
 [System.Serializable]class Insets {public float left,top,right,bottom;}
 public void SetSafeArea(string json){var i=JsonUtility.FromJson<Insets>(json);insets=new Vector4(Mathf.Clamp01(i.left),Mathf.Clamp01(i.top),Mathf.Clamp01(i.right),Mathf.Clamp01(i.bottom));if(insets.x+insets.z>=1||insets.y+insets.w>=1)insets=Vector4.zero;}
 int frames;float elapsed;float worst;
 #if UNITY_WEBGL && !UNITY_EDITOR
 [DllImport("__Internal")]static extern void HitMePerformance(float fps,float maxMs,int target);
 #endif
 void Update(){frames++;elapsed+=Time.unscaledDeltaTime;worst=Mathf.Max(worst,Time.unscaledDeltaTime);if(elapsed<2)return;
 #if UNITY_WEBGL && !UNITY_EDITOR
 HitMePerformance(frames/elapsed,worst*1000,Application.targetFrameRate);
 #endif
 frames=0;elapsed=0;worst=0;}
}
}
