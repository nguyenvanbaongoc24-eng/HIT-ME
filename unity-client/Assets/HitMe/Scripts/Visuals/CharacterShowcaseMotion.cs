using HitMe.Characters;
using UnityEngine;
namespace HitMe.Visuals {
// A visual-only sample of six states; no match input, health or damage events are produced.
[RequireComponent(typeof(CharacterVisual))]
public sealed class CharacterShowcaseMotion : MonoBehaviour {
 CharacterVisual visual;
 void Awake(){visual=GetComponent<CharacterVisual>();}
 void LateUpdate(){visual.Present((VisualState)((int)(Time.unscaledTime/2)%6),Color.white,Time.unscaledTime);visual.Fit(76,132);}
}
}
