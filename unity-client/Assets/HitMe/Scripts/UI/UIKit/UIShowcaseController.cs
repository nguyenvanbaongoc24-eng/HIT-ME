using UnityEngine;
using UnityEngine.UI;
namespace HitMe.UI {
// Development-only prefab, excluded from production scenes.
public sealed class UIShowcaseController : MonoBehaviour {
 void Awake(){var canvas=gameObject.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;var scaler=gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(390,844);gameObject.AddComponent<GraphicRaycaster>();transform.localScale=Vector3.one;}
 void Start(){Strings.Load(Strings.Language);foreach(var widget in GetComponentsInChildren<HitMeWidget>(true)){widget.Bind(widget.name,widget.initialState.ToString(),widget.icon.sprite);widget.SetState(widget.initialState);}}
}
}
