using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace HitMe.UI {
public enum WidgetState { Normal, Selected, Locked, Disabled, Loading, Unavailable, Hit, Eliminated }
// Presentation only: owners bind validated service/game data and handle events.
public sealed class HitMeWidget : MonoBehaviour {
 public UIThemeDefinition theme;
 public Button action;
 public Image background, icon;
 public TMP_Text title, detail;
 public GameObject badge, loading, selection;
 public string titleKey, detailKey;
 public UnityEvent activated=new UnityEvent();
 public WidgetState initialState;
 public WidgetState State {get;private set;}
 string language; bool boundTitle,boundDetail;
 void Awake(){if(action!=null)action.onClick.AddListener(()=>{HitMe.Audio.HitMeAudio.Play("sfx_ui_click");activated.Invoke();});RefreshLocalization();SetState(initialState);}
 void Update(){if(language!=Strings.Language)RefreshLocalization();}
 public void RefreshLocalization(){language=Strings.Language;if(!boundTitle&&title!=null)title.text=string.IsNullOrEmpty(titleKey)?"":Strings.Get(titleKey);if(!boundDetail&&detail!=null)detail.text=string.IsNullOrEmpty(detailKey)?"":Strings.Get(detailKey);if(loading!=null){var label=loading.GetComponent<TMP_Text>();if(label!=null)label.text=Strings.Get("menuLoading");}}
 public void Bind(string label,string description=null,Sprite sprite=null){boundTitle=label!=null;boundDetail=description!=null;if(title!=null&&label!=null)title.text=label;if(detail!=null&&description!=null)detail.text=description;if(icon!=null){icon.sprite=sprite;icon.enabled=sprite!=null;}}
 public void SetState(WidgetState state){State=state;if(action!=null)action.interactable=state!=WidgetState.Disabled&&state!=WidgetState.Locked&&state!=WidgetState.Loading&&state!=WidgetState.Unavailable&&state!=WidgetState.Eliminated;
  if(loading!=null)loading.SetActive(state==WidgetState.Loading);if(title!=null)title.enabled=state!=WidgetState.Loading;if(selection!=null)selection.SetActive(state==WidgetState.Selected);if(background!=null)background.color=state==WidgetState.Hit?new Color(1,.45f,.4f):state==WidgetState.Eliminated?new Color(.55f,.55f,.55f):Color.white;
  if(detail!=null&&!boundDetail&&(state==WidgetState.Locked||state==WidgetState.Unavailable))detail.text=Strings.Get("menuUnavailable");else RefreshLocalization();}
 public void SetBadge(bool visible){if(badge!=null)badge.SetActive(visible);}
}
public static class HitMeWidgetFactory {
 public static HitMeWidget Create(string prefab,Transform parent){var asset=Resources.Load<GameObject>("UI/Kit/"+prefab);if(asset==null)throw new InvalidOperationException("Missing UI Kit prefab: "+prefab+". Run HIT ME/Configure UI Kit.");return UnityEngine.Object.Instantiate(asset,parent,false).GetComponent<HitMeWidget>();}
}
}
