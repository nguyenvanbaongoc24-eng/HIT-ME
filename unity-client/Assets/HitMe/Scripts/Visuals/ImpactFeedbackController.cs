using UnityEngine;
namespace HitMe.Visuals {
// Eight reusable bursts; trigger ONLY after confirmed combat resolution, never decides damage.
public sealed class ImpactFeedbackController : MonoBehaviour {
 struct Burst {public Vector2 point;public float started;public bool hit,active;}
 readonly Burst[] bursts=new Burst[8];int next;MotionParticleGraphic graphic;
 public int Capacity=>bursts.Length;
 public void ConfirmedImpact(Vector2 point,bool hit){bursts[next]=new Burst{point=point,started=Time.unscaledTime,hit=hit,active=true};next=(next+1)%bursts.Length;}
 void Awake(){graphic=gameObject.AddComponent<MotionParticleGraphic>();graphic.raycastTarget=false;}
 void Update(){graphic.ClearPoints();float strength=MotionSettings.Strength;for(int i=0;i<bursts.Length;i++){var b=bursts[i];if(!b.active)continue;float t=(Time.unscaledTime-b.started)/.38f;if(t>=1){bursts[i].active=false;continue;}if(strength==0)continue;int n=MotionSettings.Quality==MotionQuality.Low?3:6;for(int j=0;j<n;j++){float a=j*6.28318f/n;var offset=new Vector2(Mathf.Cos(a),Mathf.Sin(a))*t*17*strength;graphic.AddPoint(b.point+offset,1.8f*(1-t),b.hit?new Color(1,.72f,.25f,1-t):new Color(.8f,.68f,.44f,(1-t)*.6f));}}graphic.Flush();}
}
}
