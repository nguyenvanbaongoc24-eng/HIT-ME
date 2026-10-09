using UnityEngine;
namespace HitMe.Visuals {
// Ambient fallback on the stands' edges. Composite PNG leaves/crowd are never animated as rigs.
public sealed class EnvironmentMotionController : MonoBehaviour {
 MotionParticleGraphic graphic;RectTransform area;
 public int ParticleCount=>MotionSettings.AmbientCount;
 void Awake(){area=GetComponent<RectTransform>();graphic=gameObject.AddComponent<MotionParticleGraphic>();graphic.raycastTarget=false;}
 void Update(){graphic.ClearPoints();int n=ParticleCount;float time=Time.unscaledTime;for(int i=0;i<n;i++){float y=Mathf.Repeat(i*.137f+time*.006f,1);float side=i%2==0?-1:1;float x=side*area.rect.width*.465f+Mathf.Sin(time*.7f+i)*2*MotionSettings.Strength;graphic.AddPoint(new Vector2(x,(y-.5f)*area.rect.height),.7f,new Color(1,.9f,.6f,.18f));}graphic.Flush();}
}
}
