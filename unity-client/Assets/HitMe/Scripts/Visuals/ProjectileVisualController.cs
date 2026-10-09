using UnityEngine;
using UnityEngine.UI;
namespace HitMe.Visuals {
// Flight position/progress belongs to BattleView. This component only decorates that transform.
public sealed class ProjectileVisualController : MonoBehaviour {
 RectTransform body;MotionParticleGraphic trail;readonly Vector2[] history=new Vector2[8];int count;float started,lastSample;Vector2 restSize;
 float spinDegreesPerSecond=650;
 bool trailEnabled=true;float trailWidth=1.6f;Color trailColor=new Color(1,.8f,.4f,.28f);
 public void Configure(HitMe.Characters.WeaponDefinition definition){spinDegreesPerSecond=definition!=null?definition.flightSpinDegreesPerSecond:650;trailEnabled=definition==null||definition.trailEnabled;trailWidth=definition!=null?Mathf.Max(0,definition.trailWidth):1.6f;trailColor=definition!=null?definition.trailColor:new Color(1,.8f,.4f,.28f);}
 public int TrailSamples=>count;
 void Awake(){body=GetComponent<RectTransform>();var go=new GameObject("FlightTrail",typeof(RectTransform),typeof(MotionParticleGraphic));go.transform.SetParent(transform,false);go.transform.SetAsFirstSibling();trail=go.GetComponent<MotionParticleGraphic>();trail.raycastTarget=false;}
 public void ResetFlight(){count=0;started=lastSample=Time.unscaledTime;restSize=body.sizeDelta;body.localRotation=Quaternion.identity;body.localScale=Vector3.one;trail.ClearPoints();trail.Flush();}
 void LateUpdate(){float strength=MotionSettings.Strength;float elapsed=Time.unscaledTime-started;body.localRotation=Quaternion.Euler(0,0,elapsed*spinDegreesPerSecond*strength);float stretch=Mathf.Sin(elapsed*18)*.035f*strength;body.sizeDelta=new Vector2(restSize.x*(1+stretch),restSize.y*(1-stretch));
  if(Time.unscaledTime-lastSample>.035f){lastSample=Time.unscaledTime;for(int i=history.Length-1;i>0;i--)history[i]=history[i-1];history[0]=body.anchoredPosition;count=Mathf.Min(history.Length,count+1);}
  trail.transform.localRotation=Quaternion.Inverse(body.localRotation);trail.ClearPoints();int n=trailEnabled?Mathf.Min(count,MotionSettings.TrailCount):0;for(int i=1;i<n;i++){float fade=1-i/(float)n;var color=trailColor;color.a*=fade;trail.AddPoint(history[i]-body.anchoredPosition,trailWidth*fade,color);}trail.Flush();
 }
 void OnDisable(){if(body==null)return;body.localRotation=Quaternion.identity;body.sizeDelta=restSize;count=0;}
}
}
