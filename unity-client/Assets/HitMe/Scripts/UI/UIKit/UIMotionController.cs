using System;
using HitMe.Visuals;
using UnityEngine;
namespace HitMe.UI {
[DisallowMultipleComponent]
public sealed class UIMotionController : MonoBehaviour {
 public float duration=.16f;
 CanvasGroup group;float began;bool running,exiting;Action finished;Vector3 rest=Vector3.one;
 public bool Transitioning=>running;
 void Awake(){group=GetComponent<CanvasGroup>();if(group==null)group=gameObject.AddComponent<CanvasGroup>();rest=transform.localScale;}
 void OnEnable(){Enter();}
 public void Enter(){exiting=false;finished=null;Begin();}
 public void Exit(Action callback){exiting=true;finished=callback;Begin();}
 void Begin(){began=Time.unscaledTime;running=true;group.interactable=false;group.blocksRaycasts=!exiting;if(MotionSettings.Reduced)Complete();}
 void Update(){if(!running)return;float t=Mathf.Clamp01((Time.unscaledTime-began)/Mathf.Max(.01f,duration));float eased=1-Mathf.Pow(1-t,3);float visible=exiting?1-eased:eased;group.alpha=visible;transform.localScale=rest*Mathf.Lerp(.98f,1,visible);if(t>=1)Complete();}
 void Complete(){running=false;group.alpha=exiting?0:1;transform.localScale=rest;group.interactable=!exiting;group.blocksRaycasts=!exiting;var callback=finished;finished=null;callback?.Invoke();}
 public void Pulse(){if(!running&&!MotionSettings.Reduced)Enter();}
}
}
