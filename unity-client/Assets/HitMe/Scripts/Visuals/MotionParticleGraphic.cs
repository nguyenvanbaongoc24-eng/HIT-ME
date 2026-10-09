using UnityEngine;
using UnityEngine.UI;
namespace HitMe.Visuals {
// Fixed buffers and one Canvas mesh. Presentation positions only, no colliders or resolver.
public sealed class MotionParticleGraphic : MaskableGraphic {
 readonly Vector2[] points=new Vector2[64];readonly float[] sizes=new float[64];readonly Color[] colors=new Color[64];int count;
 public void ClearPoints(){count=0;}
 public void AddPoint(Vector2 position,float radius,Color tint){if(count>=points.Length)return;points[count]=position;sizes[count]=radius;colors[count++]=tint;}
 public void Flush(){SetVerticesDirty();}
 protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();for(int i=0;i<count;i++){int start=vh.currentVertCount;var p=points[i];float s=sizes[i];vh.AddVert(p+new Vector2(-s,-s),colors[i],Vector2.zero);vh.AddVert(p+new Vector2(-s,s),colors[i],Vector2.zero);vh.AddVert(p+new Vector2(s,s),colors[i],Vector2.zero);vh.AddVert(p+new Vector2(s,-s),colors[i],Vector2.zero);vh.AddTriangle(start,start+1,start+2);vh.AddTriangle(start,start+2,start+3);}}
}
}
