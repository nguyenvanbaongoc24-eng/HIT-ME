using UnityEngine;
using UnityEngine.UI;
namespace HitMe.Visuals {
// Observes display changes only. It never computes health, rewards or match results.
public sealed class FeedbackPulse : MonoBehaviour {
 Text label; string previous; float began=-100; Vector3 rest;
 void Awake(){label=GetComponent<Text>();rest=transform.localScale;}
 void Update(){if(label!=null&&previous!=label.text){if(previous!=null)began=Time.unscaledTime;previous=label.text;}float t=Time.unscaledTime-began;transform.localScale=rest*(1+(t<.25f?Mathf.Sin(t/.25f*Mathf.PI)*.06f*MotionSettings.Strength:0));}
 void OnDisable(){transform.localScale=rest;}
}
}
