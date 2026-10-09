using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace HitMe.UI {
// Debounce actual pointer/submit events; gameplay/service owners retain their existing listeners.
public sealed class DebouncedButton : Button {
 public float clickInterval=.18f;float last=-100;
 bool Accept(){if(!IsActive()||!IsInteractable()||Time.unscaledTime-last<clickInterval)return false;last=Time.unscaledTime;return true;}
 public override void OnPointerClick(PointerEventData e){if(e.button==PointerEventData.InputButton.Left&&Accept())base.OnPointerClick(e);}
 public override void OnSubmit(BaseEventData e){if(Accept())base.OnSubmit(e);}
}
}
