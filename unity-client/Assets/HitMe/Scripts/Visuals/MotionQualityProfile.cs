using UnityEngine;
namespace HitMe.Visuals {
public enum MotionQuality { Low, Medium, High }
[CreateAssetMenu(menuName="HIT ME/Motion Quality")]
public sealed class MotionQualityProfile : ScriptableObject {public float strength=1;public int ambientCount=10,trailCount=6;}
public static class MotionSettings {
 static readonly MotionQualityProfile[] profiles=new MotionQualityProfile[3];
 static MotionQualityProfile Profile {get{int i=(int)Quality;if(profiles[i]==null)profiles[i]=Resources.Load<MotionQualityProfile>("Motion/"+Quality);return profiles[i];}}
 public static MotionQuality Quality {get=>(MotionQuality)Mathf.Clamp(PlayerPrefs.GetInt("MotionQuality",2),0,2);set=>PlayerPrefs.SetInt("MotionQuality",(int)value);}
 public static bool Reduced {get=>PlayerPrefs.GetInt("ReducedMotion",0)==1;set=>PlayerPrefs.SetInt("ReducedMotion",value?1:0);}
 public static float Strength=>Reduced?0:Profile!=null?Profile.strength:Quality==MotionQuality.Low?.4f:Quality==MotionQuality.Medium?.7f:1;
 public static int AmbientCount=>Reduced?0:Profile!=null?Profile.ambientCount:Quality==MotionQuality.Low?3:Quality==MotionQuality.Medium?6:10;
 public static int TrailCount=>Reduced?0:Profile!=null?Profile.trailCount:Quality==MotionQuality.Low?2:Quality==MotionQuality.Medium?4:6;
}
}
