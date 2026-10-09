using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace HitMe.UI {
// No navigation or networking inside reusable panels.
public sealed class HitMePanel : MonoBehaviour {
 public TMP_Text heading,body,value;
 public TMP_Text roundLabel,timerLabel;
 public bool selectOnClick;
 public HitMeWidget[] items;
 public Button confirm,cancel;
 public TMP_InputField input;
 public UnityEvent confirmed=new UnityEvent(),cancelled=new UnityEvent();
 public UnityEvent<string> submitted=new UnityEvent<string>();
 public string headingKey;
 string language;
 void Awake(){if(confirm!=null)confirm.onClick.AddListener(()=>{if(input!=null)submitted.Invoke(input.text);else confirmed.Invoke();});if(cancel!=null)cancel.onClick.AddListener(()=>cancelled.Invoke());if(selectOnClick&&items!=null)for(int i=0;i<items.Length;i++){int index=i;items[i].activated.AddListener(()=>Select(index));}Refresh();}
 void Update(){if(language!=Strings.Language)Refresh();}
 void Refresh(){language=Strings.Language;if(heading!=null&&!string.IsNullOrEmpty(headingKey))heading.text=Strings.Get(headingKey);}
 public void Open(string title,string message){if(heading!=null)heading.text=title;if(body!=null)body.text=message;gameObject.SetActive(true);}
 public void Close(){gameObject.SetActive(false);}
 public void BindValue(string data){if(value!=null)value.text=data??"—";}
 public void Select(int index){if(items==null)return;for(int i=0;i<items.Length;i++)if(items[i].State!=WidgetState.Locked&&items[i].State!=WidgetState.Disabled)items[i].SetState(i==index?WidgetState.Selected:WidgetState.Normal);}
 public void BindHealth(string playerName,int hp,bool eliminated){if(body!=null)body.text=playerName;if(value!=null)value.text=new string('♥',Mathf.Clamp(hp,0,3));if(items!=null)foreach(var item in items)item.SetState(eliminated?WidgetState.Eliminated:WidgetState.Normal);}
 public void BindBattle(int round,float seconds){if(roundLabel!=null)roundLabel.text=Strings.Get("round")+" "+round;if(timerLabel!=null)timerLabel.text=Mathf.Max(0,seconds).ToString("0.0",System.Globalization.CultureInfo.InvariantCulture);}
 public void SetChatAvailable(bool available,string reason){if(input!=null)input.interactable=available;if(confirm!=null)confirm.interactable=available;if(body!=null)body.text=reason??"";}
}
}
