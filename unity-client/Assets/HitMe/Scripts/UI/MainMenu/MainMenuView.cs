using System;
using TMPro;
using HitMe.Characters;
using UnityEngine;
using UnityEngine.UI;
namespace HitMe.UI {
public sealed class MainMenuView : MonoBehaviour {
 public Rect? SafeAreaOverride {get=>safe?.Override;set{if(safe!=null){safe.Override=value;safe.Apply();Layout();}}}
 public RectTransform Content=>content;
 public UIThemeDefinition Theme=>theme;
 MainMenuController controller; UIThemeDefinition theme; RectTransform content,header,brand,hero,actions,features,practice,nav,modal;
 SafeAreaAdapter safe; TextMeshProUGUI nameText,balanceText,levelText,quickLabel,privateLabel; Button quick,privateButton,cancel;
 Image avatar; CharacterVisual character; RectTransform characterRect; float lastHeight; string locale; InputField endpoint,guest; bool compact;
 string L(string key)=>Strings.Get(key);
 public void Initialize(MainMenuController owner){controller=owner;theme=Resources.Load<UIThemeDefinition>("UI/MainMenuTheme");if(theme==null){Debug.LogError("MainMenuTheme missing. Run HIT ME/Configure Main Menu Assets.");return;}
  var canvas=new GameObject("PremiumMenuCanvas",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));canvas.transform.SetParent(transform,false);canvas.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
  var scaler=canvas.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(390,844);scaler.matchWidthOrHeight=0;
  var bg=R(canvas.transform,"VillageBackground",0,0,390,844);Stretch(bg);var image=bg.gameObject.AddComponent<Image>();image.sprite=theme.background;image.preserveAspect=false;image.raycastTarget=false; // controlled arena-art fallback
  var shade=Panel(canvas.transform,"Readability",theme.ink,new Color(.12f,.17f,.13f,.36f),0,0,390,844);Stretch(shade);
  content=R(canvas.transform,"SafeArea",0,0,390,844);Stretch(content);safe=content.gameObject.AddComponent<SafeAreaAdapter>();safe.Apply();
  Build();Canvas.ForceUpdateCanvases();Layout();RefreshStatus();
 }
 RectTransform R(Transform parent,string id,float x,float y,float w,float h){var r=new GameObject(id,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=new Vector2(.5f,1);r.pivot=new Vector2(.5f,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
 void Stretch(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;}
 RectTransform Panel(Transform parent,string id,Sprite sprite,Color color,float x,float y,float w,float h){var r=R(parent,id,x,y,w,h);var im=r.gameObject.AddComponent<Image>();im.sprite=sprite;im.type=Image.Type.Sliced;im.color=color;im.raycastTarget=false;return r;}
 TextMeshProUGUI T(Transform parent,string id,string value,float x,float y,float w,float h,float size,Color color,TextAlignmentOptions alignment=TextAlignmentOptions.Center){var r=R(parent,id,x,y,w,h);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=Resources.Load<TMP_FontAsset>("Fonts/NunitoSDF");t.text=value;t.fontSize=size;t.enableAutoSizing=true;t.fontSizeMax=size;t.fontSizeMin=size*.8f;t.fontStyle=FontStyles.Bold;t.color=color;t.alignment=alignment;t.raycastTarget=false;t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Truncate;return t;}
 Image Icon(Transform p,string id,string icon,float x,float y,float size){var r=R(p,id,x,y,size,size);var im=r.gameObject.AddComponent<Image>();im.sprite=theme.Icon(icon);im.preserveAspect=true;im.raycastTarget=false;return im;}
 Button B(Transform p,string id,Sprite bg,Color tint,float x,float y,float w,float h,Action action){var widget=HitMeWidgetFactory.Create("HitMeButtonTertiary",p);var r=widget.GetComponent<RectTransform>();r.name=id;r.anchorMin=r.anchorMax=new Vector2(.5f,1);r.pivot=new Vector2(.5f,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);widget.background.sprite=bg;widget.background.color=tint;widget.title.gameObject.SetActive(false);widget.icon.gameObject.SetActive(false);widget.loading.SetActive(false);widget.action.onClick.AddListener(()=>action());return widget.action;}
 void Clear(Transform p){for(int i=p.childCount-1;i>=0;i--){var g=p.GetChild(i).gameObject;g.SetActive(false);Destroy(g);}}
 void Build(){Clear(content);locale=Strings.Language;modal=null;controller.State.OpenPanel(MenuPanel.Home);
  header=Panel(content,"PlayerStatusWidget",theme.ink,Color.white,0,8,370,68);
  var profile=B(header,"Profile",theme.card,Color.white,-151,7,52,52,()=>controller.OpenAccount("profile"));avatar=Icon(profile.transform,"Avatar","people",0,3,46);
  nameText=T(header,"PlayerName","",-66,9,108,25,15,theme.cream,TextAlignmentOptions.Left);
  levelText=T(header,"PlayerLevel","",-66,35,108,21,11,theme.muted,TextAlignmentOptions.Left);
  Icon(header,"CoinIcon","coin",14,12,24);balanceText=T(header,"CurrencyWidget","",49,13,48,24,14,theme.cream);
  T(header,"CurrencyLabel",L("menuCurrencyLabel"),42,38,64,16,9,theme.muted);
  var inbox=B(header,"Inbox",theme.card,Color.white,100,10,48,48,()=>Notice("menuInbox",L("menuUnavailable")));Icon(inbox.transform,"MailIcon","mail",0,9,30);
  var settings=B(header,"Settings",theme.card,Color.white,151,10,48,48,Settings);Icon(settings.transform,"SettingsIcon","settings",0,9,30);
  brand=R(content,"Branding",0,84,360,86);var logo=R(brand,"IndependentLogo",0,0,222,59);var li=logo.gameObject.AddComponent<Image>();li.sprite=theme.logo;li.preserveAspect=true;li.raycastTarget=false;T(brand,"Tagline",L("menuTagline"),0,62,260,24,16,theme.cream);
  hero=R(content,"CharacterPresentation",0,179,370,220);var shadow=R(hero,"FootShadow",0,175,148,44);var si=shadow.gameObject.AddComponent<Image>();si.sprite=theme.shadow;si.raycastTarget=false;
  characterRect=R(hero,"SelectedCharacter",0,0,150,197);characterRect.pivot=new Vector2(.5f,0);characterRect.anchorMin=characterRect.anchorMax=new Vector2(.5f,0);characterRect.anchoredPosition=new Vector2(0,20);characterRect.gameObject.AddComponent<Image>();character=characterRect.gameObject.AddComponent<CharacterVisual>();character.Initialize(CharacterCatalog.Load()?.ForSlot(0));
  var quests=B(hero,"Quests",theme.ink,Color.white,148,32,48,48,()=>controller.OpenAccount("quests"));Icon(quests.transform,"QuestIcon","quest",0,8,30);
  T(hero,"QuestCaption",L("dailyQuests"),148,82,66,30,11,theme.cream);
  var badge=Panel(quests.transform,"NotificationBadge",theme.brick,Color.white,16,0,14,14);badge.gameObject.SetActive(false);
  actions=R(content,"PrimaryActions",0,400,370,110);
  quick=B(actions,"QuickMatch",theme.paper,Color.white,-94,0,182,108,()=>controller.Online("quick"));quickLabel=T(quick.transform,"Label",L("menuQuick"),-7,16,150,30,22,theme.dark);T(quick.transform,"Description",L("menuQuickDesc"),-7,49,145,31,12,theme.dark);Icon(quick.transform,"Arrow","arrow",66,78,18).color=theme.dark;
  privateButton=B(actions,"PrivateRoom",theme.brick,Color.white,94,0,182,108,()=>controller.Online("private"));privateLabel=T(privateButton.transform,"Label",L("menuPrivate"),-4,17,158,28,21,theme.cream);T(privateButton.transform,"Description",L("menuPrivateDesc"),-4,50,150,30,12,theme.cream);Icon(privateButton.transform,"Arrow","arrow",66,78,18);
  cancel=B(actions,"CancelMatchmaking",theme.ink,Color.white,0,110,156,44,controller.Cancel);T(cancel.transform,"Label",L("menuCancel"),0,0,150,44,14,theme.cream);cancel.gameObject.SetActive(false);
  features=R(content,"SecondaryFeatures",0,522,370,119);
  Feature(0,"Maps","menuMaps","map",Maps);Feature(1,"Characters","menuCharacters","people",Characters);Feature(2,"Collection","menuCollection","bag",()=>controller.OpenAccount("inventory"));Feature(3,"Ranking","menuRanking","trophy",()=>Notice("menuRanking",L("menuUnavailable")),true);
  practice=R(content,"PracticeEntry",0,650,370,48);var train=B(practice,"Practice",theme.ink,Color.white,-66,0,236,48,Practice);Icon(train.transform,"Icon","practice",-89,10,28);T(train.transform,"Label",L("menuPractice"),12,0,179,48,15,theme.cream);
  var details=B(practice,"Help",theme.card,Color.white,124,0,112,48,()=>Notice("menuAbout",L("menuAboutBody")));T(details.transform,"Label",L("menuAbout"),0,0,104,48,13,theme.cream);
  nav=Panel(content,"BottomNavigation",theme.ink,Color.white,0,744,390,78);
  Nav(0,"Home","menuHome","home",()=>{CloseModal();},false);Nav(1,"Shop","menuShop","shop",()=>Notice("menuShop",L("menuUnavailable")),true);Nav(2,"Inventory","inventory","bag",()=>controller.OpenAccount("inventory"),false);Nav(3,"Travel","menuTravel","map",Maps,false);Nav(4,"Community","menuCommunity","people",()=>Notice("menuCommunity",L("menuUnavailable")),true);
 }
 void Feature(int i,string id,string key,string icon,Action action,bool locked=false){var b=B(features,id,theme.card,Color.white,-141+i*94,0,88,119,action);Icon(b.transform,"Icon",icon,0,10,39);T(b.transform,"Title",L(key),0,54,82,34,13,theme.cream);T(b.transform,"State",L(locked?"menuSoon":"menuExplore"),0,91,82,18,10,theme.muted);}
 void Nav(int i,string id,string key,string icon,Action action,bool locked){var b=B(nav,id,null,new Color(0,0,0,0),-156+i*78,0,76,76,action);var im=Icon(b.transform,"Icon",icon,0,9,29);im.color=i==0?new Color(1,.8f,.32f):theme.muted;T(b.transform,"Label",L(key),0,41,75,20,10,i==0?new Color(1,.8f,.32f):theme.muted);if(locked)T(b.transform,"Soon",L("menuSoon"),0,61,75,14,8,theme.muted);if(i==0)Panel(b.transform,"ActiveIndicator",theme.paper,Color.white,0,72,42,3);}
 public void Layout(){if(content==null||hero==null)return;Canvas.ForceUpdateCanvases();float h=content.rect.height;lastHeight=h;compact=h<720;float heroHeight=Mathf.Clamp(h-(compact?500:620),h<590?52:116,270);hero.sizeDelta=new Vector2(370,heroHeight);float foot=Mathf.Clamp(heroHeight-20,32,230);characterRect.sizeDelta=new Vector2(foot*.78f,foot);hero.Find("FootShadow").GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-heroHeight+43);
  float primaryY=179+heroHeight+8;actions.anchoredPosition=new Vector2(0,-primaryY);features.anchoredPosition=new Vector2(0,-primaryY-122);practice.anchoredPosition=new Vector2(0,-primaryY-(compact?122:251));nav.anchoredPosition=new Vector2(0,-h+78);features.gameObject.SetActive(!compact&&!controller.Loading); // Footer remains inside safe area; secondary features compact first.
 }
 public void RefreshStatus(){if(theme==null)return;var p=controller.Profile;nameText.text=p?.name??L("menuGuest");levelText.text=p!=null?L("level")+" "+p.level+" · XP "+p.xp:L("menuNotConnected");balanceText.text=p!=null?p.coins.ToString():"—";
  var def=CharacterCatalog.Load()?.ForSlot(0);if(p!=null){var catalog=CharacterCatalog.Load();if(catalog?.characters!=null)foreach(var d in catalog.characters)if(d!=null&&d.characterId==p.avatar)def=d;}avatar.sprite=def!=null&&def.portrait!=null?def.portrait:def?.Frame(VisualState.Idle,0);if(avatar.sprite==null)avatar.sprite=theme.Icon("people");character.Initialize(def);
  if(p!=null){string id=p.equipped=="chao"?"Chao":p.equipped=="vot"?"Vot":"DepToOng";var catalog=CharacterCatalog.Load();if(catalog?.characters!=null)foreach(var d in catalog.characters){var w=d?.defaultWeapon;if(w!=null&&w.weaponId==id)character.SetCosmeticWeapon(w);}}
  quick.interactable=privateButton.interactable=!controller.Loading;quickLabel.text=L(controller.Loading?"menuLoading":"menuQuick");cancel.gameObject.SetActive(controller.Loading);
  features.gameObject.SetActive(!compact&&!controller.Loading);practice.gameObject.SetActive(!controller.Loading);var badge=hero.Find("Quests/NotificationBadge");bool completed=false;if(p?.quests!=null)foreach(var q in p.quests)if(q.state=="Completed")completed=true;badge.gameObject.SetActive(completed);
  var spin=actions.Find("Spinner");if(controller.Loading&&spin==null){var image=Icon(actions,"Spinner","settings",0,159,24);image.gameObject.AddComponent<MenuSpinner>();}else if(!controller.Loading&&spin!=null)Destroy(spin.gameObject);
 }
 void LateUpdate(){if(theme==null)return;if(Mathf.Abs(lastHeight-content.rect.height)>1)Layout();if(locale!=Strings.Language){Build();Layout();RefreshStatus();}if(character!=null)character.Present(VisualState.Idle,Color.white,Time.unscaledTime);}
 public void CloseModal(){if(modal!=null){modal.gameObject.SetActive(false);Destroy(modal.gameObject);}modal=null;controller.State.OpenPanel(MenuPanel.Home);}
 RectTransform Modal(string title,float height=440,MenuPanel screen=MenuPanel.Notice){CloseModal();controller.State.OpenPanel(screen);var veil=R(content,"MenuModal",0,0,390,844);Stretch(veil);var shade=veil.gameObject.AddComponent<Image>();shade.color=new Color(.02f,.04f,.03f,.86f);modal=veil;var panel=Panel(veil,"Panel",theme.ink,Color.white,0,Mathf.Max(30,(content.rect.height-height)/2),360,height);T(panel,"Title",L(title),-18,17,282,39,22,theme.cream);var close=B(panel,"Close",theme.card,Color.white,151,12,48,48,controller.Back);Icon(close.transform,"Icon","cancel",0,12,24);return panel;}
 Button Row(Transform p,string id,string value,float y,Action action){var b=B(p,id,theme.card,Color.white,0,y,326,48,action);T(b.transform,"Label",value,0,0,312,48,15,theme.cream);return b;}
 public void Notice(string title,string body){var p=Modal(title,280);T(p,"Message",body,0,75,314,123,16,theme.cream);Row(p,"Dismiss",L("back"),213,controller.Back);}
 public void Settings(){bool diagnostics=Application.isEditor||Debug.isDebugBuild;var p=Modal("settings",diagnostics?415:340,MenuPanel.Settings);Row(p,"Language",L("menuLanguage")+": "+Strings.Language.ToUpperInvariant(),78,()=>{Strings.Load(Strings.Language=="vi"?"en":"vi");Build();Layout();RefreshStatus();Settings();});Row(p,"Graphics",L(WebMobileBridge.LowQuality?"qualityLow":"qualityHigh"),139,()=>{WebMobileBridge.SetQuality(!WebMobileBridge.LowQuality);Settings();});T(p,"SoundStatus",L("menuAudioPending"),0,201,310,70,14,theme.muted);if(diagnostics)Row(p,"Diagnostics",L("menuDiagnostics"),291,()=>Notice("menuDiagnostics",L("menuArtFallback")+"\n"+L("menuTrial")+"\n"+L("timeoutProposal")));}
 public void Practice(){var p=Modal("menuPractice",398,MenuPanel.Practice);Row(p,"BotCount",L("botCount")+": "+OfflineRunContext.Settings.BotCount,75,()=>{OfflineRunContext.Settings.BotCount=OfflineRunContext.Settings.BotCount%5+1;Practice();});Row(p,"Difficulty",L("difficulty")+": "+L(OfflineRunContext.Settings.Difficulty==HitMe.Core.BotDifficulty.Easy?"easy":"normal"),133,()=>{OfflineRunContext.Settings.Difficulty=OfflineRunContext.Settings.Difficulty==HitMe.Core.BotDifficulty.Easy?HitMe.Core.BotDifficulty.Normal:HitMe.Core.BotDifficulty.Easy;Practice();});T(p,"Policy",L("menuPracticeTip"),0,192,306,70,12,theme.muted);Row(p,"PlayBots",L("playOffline"),280,MenuNavigationService.Practice);}
 public void Maps(){var p=Modal("menuMaps",490,MenuPanel.Maps);for(int i=0;i<ArenaMaps.Ids.Length;i++){int index=i;Row(p,"Map"+i,(ArenaMaps.Selected==i?"• ":"")+L("map"+ArenaMaps.Ids[i])+(i==0?"":" · "+L("missingArenaArt")),73+i*58,()=>{ArenaMaps.Selected=index;Maps();});}T(p,"MapPolicy",L("menuMapPolicy"),0,424,314,53,11,theme.muted);}
 public void Characters(){var p=Modal("menuCharacters",400,MenuPanel.Characters);var catalog=CharacterCatalog.Load();for(int i=0;i<3;i++){var d=catalog?.ForSlot(i);var r=R(p,"Character"+i,-106+i*106,78,92,120);var image=r.gameObject.AddComponent<Image>();image.sprite=d?.Frame(VisualState.Idle,0);image.preserveAspect=true;image.raycastTarget=false;}
  T(p,"CharacterNote",L("menuCharacterPolicy"),0,218,314,69,14,theme.muted);Row(p,"Weapons",L("inventory"),319,()=>controller.OpenAccount("inventory"));}
 InputField Input(Transform p,string id,string value,float y){var r=Panel(p,id,theme.card,Color.white,0,y,326,48);r.GetComponent<Image>().raycastTarget=true;var field=r.gameObject.AddComponent<InputField>(); // Existing project text input bridge provides Web mobile keyboard.
  var label=R(r,"Text",0,0,310,48);var t=label.gameObject.AddComponent<HitMeText>();t.font=FoundationFonts.Text;t.fontSize=15;t.color=theme.cream;t.alignment=TextAnchor.MiddleCenter;field.textComponent=t;field.characterLimit=256;field.text=value;return field;}
 public void Connection(){var p=Modal("online",426,MenuPanel.Connection);T(p,"GuestPolicy",L("menuGuestConsent"),0,70,310,63,14,theme.muted);endpoint=Input(p,"Endpoint",PlayerPrefs.GetString("NetworkEndpoint","ws://127.0.0.1:8788/play"),143);guest=Input(p,"GuestName","Quest",205);guest.characterLimit=24;Row(p,"Connect",L("connect"),274,()=>controller.Connect(endpoint.text.Trim(),guest.text.Trim()));Row(p,"Cancel",L("menuCancel"),336,controller.Cancel);}
}
}

