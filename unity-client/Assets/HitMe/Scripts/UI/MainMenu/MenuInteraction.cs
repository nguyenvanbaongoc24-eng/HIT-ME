using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace HitMe.UI {
// All motion is decorative, unscaled and completed in roughly 160ms.
public sealed class MenuInteraction : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerExitHandler,IPointerEnterHandler,ISelectHandler,IDeselectHandler {
 float target=1; bool held; Button button;
 void Awake(){button=GetComponent<Button>();}
 public void OnPointerDown(PointerEventData e){if(button.interactable){held=true;target=.97f;}}
 public void OnPointerUp(PointerEventData e){held=false;target=1;}
 public void OnPointerExit(PointerEventData e){held=false;target=1;}
 public void OnPointerEnter(PointerEventData e){if(button.interactable&&!held)target=1.015f;}
 public void OnSelect(BaseEventData e){if(button.interactable)target=1.015f;}
 public void OnDeselect(BaseEventData e){target=1;}
 void Update(){transform.localScale=Vector3.Lerp(transform.localScale,Vector3.one*target,1-Mathf.Exp(-Time.unscaledDeltaTime/ .055f));}
}
public sealed class MenuSpinner : MonoBehaviour {void Update(){transform.Rotate(0,0,-Time.unscaledDeltaTime*240);}}
}
