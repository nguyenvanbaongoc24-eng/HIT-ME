using UnityEngine;
using UnityEngine.UI;
namespace HitMe.Visuals {
// Only isolated artwork moves. No camera, gameplay root, collider or baked backdrop is animated.
public sealed class EnvironmentLayerView : MonoBehaviour {
 EnvironmentLayerDefinition definition;RectTransform rect;float phase,cheerUntil;
 public void Celebrate(){if(definition!=null&&definition.motion==LayerMotion.Crowd)cheerUntil=Time.unscaledTime+.8f;}
 public void Initialize(EnvironmentLayerDefinition data,int index){definition=data;rect=(RectTransform)transform;phase=index*1.7f;var image=gameObject.AddComponent<Image>();image.sprite=data.sprite;image.preserveAspect=true;image.raycastTarget=false;rect.anchorMin=rect.anchorMax=data.anchor;rect.pivot=data.pivot;rect.sizeDelta=data.referenceSize;rect.anchoredPosition=Vector2.zero;}
 void LateUpdate(){if(definition==null)return;float strength=MotionSettings.Strength,t=Time.unscaledTime*definition.speed+phase;rect.localRotation=Quaternion.Euler(0,0,definition.motion==LayerMotion.Sway||definition.motion==LayerMotion.Flutter?Mathf.Sin(t)*definition.amplitude*strength:0);rect.localScale=new Vector3(definition.motion==LayerMotion.Flutter?1+Mathf.Sin(t*1.6f)*.015f*strength:1,1,1);rect.anchoredPosition=definition.motion==LayerMotion.Crowd?new Vector2(0,Mathf.Abs(Mathf.Sin(t*(Time.unscaledTime<cheerUntil?5:1)))*definition.amplitude*(Time.unscaledTime<cheerUntil?3:1))*strength:definition.motion==LayerMotion.Float?new Vector2(Mathf.Sin(t)*definition.amplitude,Mathf.Cos(t*.7f)*definition.amplitude)*strength:Vector2.zero;}
 public static RectTransform Create(Transform parent,EnvironmentLayerDefinition[] layers){var root=new GameObject("IndependentEnvironmentLayers",typeof(RectTransform)).GetComponent<RectTransform>();root.SetParent(parent,false);root.anchorMin=Vector2.zero;root.anchorMax=Vector2.one;root.offsetMin=root.offsetMax=Vector2.zero;if(layers!=null)for(int i=0;i<layers.Length;i++){var data=layers[i];if(data==null||data.sprite==null)continue;var child=new GameObject(data.layerId,typeof(RectTransform));child.transform.SetParent(root,false);child.AddComponent<EnvironmentLayerView>().Initialize(data,i);}return root;}
}
}
