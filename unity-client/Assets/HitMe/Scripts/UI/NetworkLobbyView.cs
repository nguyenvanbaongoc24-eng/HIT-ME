using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace HitMe.UI
{
    public sealed class NetworkLobbyView:MonoBehaviour
    {
        NetworkSession net;Transform content;Text info;InputField endpoint,code,name;string page="lobby";bool weekly;int revision=-1;
        string L(string key)=>Strings.Get(key);
        void Start(){page=MenuNavigationService.ConsumePage();net=NetworkSession.Ensure();var go=new GameObject("OnlineCanvas",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));go.transform.SetParent(transform,false);go.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
            var scaler=go.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(390,844);scaler.matchWidthOrHeight=0;content=go.transform;var art=ArenaMaps.LoadSelected();if(art?.backdrop!=null){var background=go.AddComponent<Image>();background.sprite=art.backdrop;background.color=new Color(.36f,.30f,.26f);background.raycastTarget=false;}Build();}
        RectTransform R(string id,float y,float height=42){var r=new GameObject(id,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(content,false);r.sizeDelta=new Vector2(342,height);r.anchoredPosition=new Vector2(0,y);return r;}
        Text T(string id,string value,float y,int size=16,float height=42){var r=R(id,y,height);var t=r.gameObject.AddComponent<HitMeText>();t.font=FoundationFonts.Text;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;t.text=value;t.raycastTarget=false;return t;}
        void B(string key,float y,UnityEngine.Events.UnityAction action){var widget=HitMeWidgetFactory.Create("HitMeButtonTertiary",content);var r=widget.GetComponent<RectTransform>();r.name=key;r.sizeDelta=new Vector2(342,44);r.anchoredPosition=new Vector2(0,y);widget.title.gameObject.SetActive(false);widget.icon.gameObject.SetActive(false);widget.action.onClick.AddListener(action);var text=T(key+"Label",L(key),y);text.transform.SetParent(r,false);text.rectTransform.anchoredPosition=Vector2.zero;}
        InputField Input(string id,string value,float y){var r=R(id,y);r.gameObject.AddComponent<Image>().color=new Color(.24f,.20f,.16f);var input=r.gameObject.AddComponent<InputField>();var text=T(id+"Text",value,y);text.transform.SetParent(r,false);text.rectTransform.anchoredPosition=Vector2.zero;input.textComponent=text;input.characterLimit=256;input.text=value;return input;}
        void Build(){for(int i=content.childCount-1;i>=0;i--){content.GetChild(i).gameObject.SetActive(false);Destroy(content.GetChild(i).gameObject);}T("Title","HIT ME · "+L("online"),340,26);
            B("settings",290,()=>{Strings.Load(Strings.Language=="vi"?"en":"vi");Build();});
            if(!net.Connected||net.Profile==null){endpoint=Input("Endpoint",PlayerPrefs.GetString("NetworkEndpoint","ws://127.0.0.1:8788/play"),205);name=Input("GuestName","Quest",145);B("connect",85,()=>{PlayerPrefs.SetString("NetworkEndpoint",endpoint.text);net.Connect(endpoint.text,name.text);});info=T("NetworkStatus",L("guestDevelopment")+"\n"+net.ErrorText,-10,14,100);B("sceneMainMenu",-290,()=>SceneManager.LoadScene("MainMenu"));return;}
            if(net.Room!=null){var room=net.Room;T("RoomCode",L("roomCode")+": "+room.code,220,22);string roster="";foreach(var p in room.players)roster+=p.name+(p.id==room.owner?" ("+L("roomOwner")+")":"")+" · "+L(p.ready?"locked":"notReady")+(p.connected?"":" · "+L("networkDisconnected"))+"\n";T("Roster",roster,95,17,180);
                info=T("RoomPhase",L("network"+room.phase)+"\n"+net.ErrorText,-70,15,70);B("ready",-160,()=>net.Command("ready",ready:!net.Self.ready));B("leaveRoom",-220,()=>net.Command("leave"));return;}
            if(page=="profile"){var p=net.Profile;T("Profile",p.name+"\n"+L("level")+": "+p.level+" · XP "+p.xp+"\n"+L("coins")+": "+p.coins+"\n"+L("equipped")+": "+L("item"+p.equipped)+"\n"+L("history")+": "+(p.history?.Length??0),50,20,240);var avatar=R("ProfileAvatar",235,75);avatar.sizeDelta=new Vector2(68,75);var image=avatar.gameObject.AddComponent<Image>();image.sprite=HitMe.Characters.CharacterCatalog.Load()?.ForSlot(0)?.Frame(HitMe.Characters.VisualState.Idle,0);image.preserveAspect=true;image.raycastTarget=false;string history="";if(p.history!=null)for(int i=0;i<System.Math.Min(3,p.history.Length);i++){var match=p.history[i];history+=L(match.outcome=="draw"?"draw":match.outcome=="win"?"victory":"defeat")+" · "+match.coins+" "+L("coins")+" · XP "+match.xp+"\n";}T("MatchHistory",history,-135,14,86);}
            else if(page=="inventory"){T("CosmeticNotice",L("cosmeticWeapon"),230,14);float y=175;foreach(var item in net.Profile.inventory){var captured=item;bool weapon=item.item=="chao"||item.item=="vot"||item.item=="dep-to-ong";if(weapon){B("equip",y,()=>net.Command("equip",captured.item));content.GetChild(content.childCount-1).GetComponentInChildren<Text>().text=L("item"+item.item)+" × "+item.quantity;}else T("MapCollectible",L("map"+item.item)+" × "+item.quantity,y);y-=48;if(y< -210)break;}}
            else if(page=="quests"){float y=225;foreach(var q in net.Profile.quests){if((q.period.Length!=10)!=weekly)continue;var captured=q;B("claimQuest",y,()=>net.Claim(captured));content.GetChild(content.childCount-1).GetComponentInChildren<Text>().text=L("questEvent"+q.kind)+" "+q.progress+"/"+q.target+" · "+L("quest"+q.state);y-=60;}T("QuestNote",L("questPayoutPending"),-110,13,60);B("weeklyQuests",-180,()=>{weekly=!weekly;Build();});}
            else{T("ProfileSummary",net.Profile.name+" · "+L("coins")+" "+net.Profile.coins,235,18);B("quickMatch",165,()=>net.Command("quick"));B("createRoom",110,()=>net.Command("create"));code=Input("RoomCodeInput","",55);code.characterLimit=8;B("joinRoom",0,()=>net.Command("join",code.text.Trim()));B("profile",-65,()=>{page="profile";Build();});B("inventory",-120,()=>{page="inventory";Build();});B("dailyQuests",-175,()=>{page="quests";Build();});}
            info=T("Error",net.ErrorText,-245,12);B("back",-300,()=>{if(page!="lobby"){page="lobby";Build();}else SceneManager.LoadScene("MainMenu");});}
        void Update(){if(net==null)return;if(net.Room!=null&&net.Room.phase!="Waiting"&&net.Room.phase!="Countdown"){net.OnlineBattle=true;SceneManager.LoadScene("Battle");return;}
            if(revision==net.Revision)return;revision=net.Revision;
            // State updates at 10 Hz; rebuild only when room or displayed values change.
            string fingerprint=(net.Room?.id??"")+"|"+(net.Room?.phase??"")+"|"+JsonUtility.ToJson(net.Profile)+"|"+net.Error+"|"+page;
            if(net.Room?.players!=null)foreach(var p in net.Room.players)fingerprint+="|"+p.id+p.ready+p.connected;
            if(fingerprint!=lastFingerprint){lastFingerprint=fingerprint;if(net.Room!=null||net.Profile!=null)Build();else if(info!=null)info.text=L("guestDevelopment")+"\n"+net.ErrorText;}}
        string lastFingerprint="";
    }
}

