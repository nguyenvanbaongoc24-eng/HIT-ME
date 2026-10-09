using UnityEngine;
using UnityEngine.UI;
namespace HitMe.UI {
public enum AimVisualState {Hidden,Aiming,Locked,Throwing}
// UI mesh only. Authoritative direction, collision endpoint and input remain owned by BattleView.
[RequireComponent(typeof(CanvasRenderer))]
public sealed class ProductionAimIndicator:MaskableGraphic {
 public AimVisualState State {get;private set;}
 public Vector2 GameplayOrigin {get;private set;}
 public Vector2 Endpoint {get;private set;}
 public Vector2 LaunchOrigin {get;private set;}
 public Vector2 VisualDirection {get;private set;}
 Vector2 targetDirection;float distance,opacity,lockedAt,throwAt;bool snap;
 protected override void Awake(){base.Awake();raycastTarget=false;}
 public void Present(Vector2 feet,Vector2 endpoint,bool locked,bool immediate=false){bool appeared=State==AimVisualState.Hidden;GameplayOrigin=feet;Endpoint=endpoint;var delta=endpoint-feet;distance=delta.magnitude;targetDirection=distance>.001f?delta/distance:Vector2.right;if(appeared||locked||immediate)VisualDirection=targetDirection;if(locked&&State!=AimVisualState.Locked)lockedAt=Time.unscaledTime;State=locked?AimVisualState.Locked:AimVisualState.Aiming;opacity=1;snap=locked;SetVerticesDirty();}
 public void Snap(){VisualDirection=targetDirection;snap=true;SetVerticesDirty();}
 public void Hide(bool throwing=false){if(throwing&&(State==AimVisualState.Aiming||State==AimVisualState.Locked)){State=AimVisualState.Throwing;throwAt=Time.unscaledTime;}else if(!throwing){State=AimVisualState.Hidden;opacity=0;}SetVerticesDirty();}
 void LateUpdate(){if(State==AimVisualState.Hidden)return;if(State==AimVisualState.Throwing){opacity=Mathf.Clamp01(1-(Time.unscaledTime-throwAt)/.14f);if(opacity<=0)State=AimVisualState.Hidden;}else if(!snap){float angle=Mathf.Atan2(VisualDirection.y,VisualDirection.x)*Mathf.Rad2Deg;float desired=Mathf.Atan2(targetDirection.y,targetDirection.x)*Mathf.Rad2Deg;angle=Mathf.LerpAngle(angle,desired,1-Mathf.Exp(-65*Time.unscaledDeltaTime));VisualDirection=new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad),Mathf.Sin(angle*Mathf.Deg2Rad));}snap=State==AimVisualState.Locked;SetVerticesDirty();}
 protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();if(State==AimVisualState.Hidden||distance<2)return;float scale=Mathf.Clamp(rectTransform.rect.width/390f,.7f,1.5f);float inset=Mathf.Min(24*scale,distance*.4f);LaunchOrigin=GameplayOrigin+VisualDirection*inset;float length=distance-inset;var normal=new Vector2(-VisualDirection.y,VisualDirection.x);float width=Mathf.Min(18*scale,length*.16f);float confirm=State==AimVisualState.Locked?1+.15f*Mathf.Exp(-(Time.unscaledTime-lockedAt)*10):1;
  // Feathered outer ribbons and cream inner gradient. All geometry remains floor-mask clipped.
  for(int i=0;i<12;i++){float a=i/12f,b=(i+1)/12f;var p=LaunchOrigin+VisualDirection*(length*a);var q=LaunchOrigin+VisualDirection*(length*b);float wa=width*(.08f+.92f*a),wb=width*(.08f+.92f*b);Color ca=C(.2f*(1-a*.5f)*confirm),cb=C(.2f*(1-b*.5f)*confirm);Quad(vh,p-normal*wa,q-normal*wb,q+normal*wb,p+normal*wa,ca,cb,cb,ca);Quad(vh,p-normal*(wa+3*scale),q-normal*(wb+3*scale),q-normal*wb,p-normal*wa,C(0),C(0),cb,ca);Quad(vh,p+normal*wa,q+normal*wb,q+normal*(wb+3*scale),p+normal*(wa+3*scale),ca,cb,C(0),C(0));}
  for(int i=0;i<6;i++){float t=(i+.6f)/7f;if(!HitMe.Visuals.MotionSettings.Reduced)t+=(Time.unscaledTime*.12f)%(.1f);var p=LaunchOrigin+VisualDirection*(length*t);Disc(vh,p,(2.4f-i*.17f)*scale,C(.8f-i*.06f));}
  float pulse=HitMe.Visuals.MotionSettings.Reduced?0:Mathf.Sin(Time.unscaledTime*3)*.6f;Ring(vh,LaunchOrigin+VisualDirection*length,(7+pulse)*scale,1.3f*scale,C(.65f));
 }
 Color C(float a)=>new Color(1,.965f,.84f,a*opacity);
 static void Quad(VertexHelper v,Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color ca,Color cb,Color cc,Color cd){int n=v.currentVertCount;v.AddVert(a,ca,Vector2.zero);v.AddVert(b,cb,Vector2.zero);v.AddVert(c,cc,Vector2.zero);v.AddVert(d,cd,Vector2.zero);v.AddTriangle(n,n+1,n+2);v.AddTriangle(n,n+2,n+3);}
 static void Disc(VertexHelper v,Vector2 p,float radius,Color color){for(int i=0;i<12;i++){float a=i*Mathf.PI/6,b=(i+1)*Mathf.PI/6;Quad(v,p,p+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius,p+new Vector2(Mathf.Cos(b),Mathf.Sin(b))*radius,p,color,color,color,color);}}
 static void Ring(VertexHelper v,Vector2 p,float radius,float thickness,Color color){for(int i=0;i<32;i++){float a=i*Mathf.PI/16,b=(i+1)*Mathf.PI/16;var x=new Vector2(Mathf.Cos(a),Mathf.Sin(a));var y=new Vector2(Mathf.Cos(b),Mathf.Sin(b));Quad(v,p+x*radius,p+y*radius,p+y*(radius-thickness),p+x*(radius-thickness),color,color,color,color);}}
}
}
