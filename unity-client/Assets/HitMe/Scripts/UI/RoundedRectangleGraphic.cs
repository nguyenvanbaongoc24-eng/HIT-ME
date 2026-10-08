using UnityEngine;
using UnityEngine.UI;
namespace HitMe.UI {
[RequireComponent(typeof(CanvasRenderer))]
public sealed class RoundedRectangleGraphic : MaskableGraphic {
 public float cornerRadius; public float innerRatio;
 const int Steps=16;
 Vector2 Vertex(Rect r,int index,float inset=0) {
  float radius=Mathf.Clamp(cornerRadius-inset,0,Mathf.Min(r.width,r.height)/2-inset);
  int corner=index/(Steps+1), step=index%(Steps+1);
  float angle=(corner*90+step*90f/Steps)*Mathf.Deg2Rad;
  float sx=corner==0||corner==3?1:-1, sy=corner<2?1:-1;
  return r.center+new Vector2(sx*(r.width/2-inset-radius)+Mathf.Cos(angle)*radius,sy*(r.height/2-inset-radius)+Mathf.Sin(angle)*radius);
 }
 protected override void OnPopulateMesh(VertexHelper vh) {
  vh.Clear(); Rect r=rectTransform.rect; int count=4*(Steps+1);
  if(innerRatio<=0) {vh.AddVert(r.center,color,Vector2.zero);for(int i=0;i<count;i++)vh.AddVert(Vertex(r,i),color,Vector2.zero);for(int i=0;i<count;i++)vh.AddTriangle(0,i+1,(i+1)%count+1);}
  else {float inset=Mathf.Min(r.width,r.height)*(1-innerRatio)/2;for(int i=0;i<count;i++){vh.AddVert(Vertex(r,i),color,Vector2.zero);vh.AddVert(Vertex(r,i,inset),color,Vector2.zero);}for(int i=0;i<count;i++){int k=i*2,n=((i+1)%count)*2;vh.AddTriangle(k,n,k+1);vh.AddTriangle(n,n+1,k+1);}}
 }
 public override bool Raycast(Vector2 sp,Camera camera){if(!base.Raycast(sp,camera))return false;RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,sp,camera,out var p);Rect r=rectTransform.rect;float radius=Mathf.Clamp(cornerRadius,0,Mathf.Min(r.width,r.height)/2);Vector2 q=new Vector2(Mathf.Abs(p.x-r.center.x)-(r.width/2-radius),Mathf.Abs(p.y-r.center.y)-(r.height/2-radius));return new Vector2(Mathf.Max(q.x,0),Mathf.Max(q.y,0)).magnitude+Mathf.Min(Mathf.Max(q.x,q.y),0)<=radius;}
}}
