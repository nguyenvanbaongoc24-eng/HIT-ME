using UnityEngine;
namespace HitMe.UI {
public sealed class MainMenuController : MonoBehaviour {
 public MainMenuView View {get;private set;}
 public bool Loading {get;private set;}
 public string Pending {get;private set;}="";
 NetworkSession net; float started; bool sent,cancelAwait; int revision=-1;
 void Start(){Strings.Load(Strings.Language);OfflineRunContext.LoadDefaults();WebMobileBridge.Ensure();net=NetworkSession.Ensure();View=gameObject.AddComponent<MainMenuView>();View.Initialize(this);}
 public NetProfile Profile=>net?.Profile;
 public bool Connected=>net!=null&&net.Connected&&net.Profile!=null;
 public void Online(string intent){if(Loading)return;if(net.Room!=null){MenuNavigationService.Lobby();return;}Pending=intent;
  if(!Connected){View.Connection();return;}Begin();}
 public void Connect(string endpoint,string guest){if(Loading)return;PlayerPrefs.SetString("NetworkEndpoint",endpoint);net.Connect(endpoint,guest);Begin();}
 void Begin(){Loading=true;started=Time.unscaledTime;sent=false;View.CloseModal();View.RefreshStatus();}
 public void Cancel(){cancelAwait=sent;Pending="";Loading=false;sent=false;if(net.Room!=null&&net.Room.phase=="Waiting"){net.Command("leave");cancelAwait=false;}View.CloseModal();View.RefreshStatus();}
 public void OpenAccount(string page){if(!Connected){Pending=page;View.Connection();}else MenuNavigationService.Lobby(page);}
 void Update(){if(net==null||View==null)return;if(net.Revision!=revision){revision=net.Revision;View.RefreshStatus();}
  if(cancelAwait&&net.Room?.phase=="Waiting"){net.Command("leave");cancelAwait=false;}
  if(!Loading)return;
  if(net.Error.Length>0){Loading=false;View.Notice("menuNetworkError",net.ErrorText);View.RefreshStatus();return;}
  if(Time.unscaledTime-started>15){Loading=false;View.Notice("menuNetworkError",Strings.Get("menuConnectTimeout"));View.RefreshStatus();return;}
  if(!Connected)return;
  if(!sent){sent=true;if(Pending=="quick")net.Command("quick");else{var page=Pending=="private"?"lobby":Pending;Loading=false;Pending="";MenuNavigationService.Lobby(page);return;}}
  if(net.Room!=null){Loading=false;Pending="";MenuNavigationService.Lobby();}
 }
 void OnDestroy(){// A cancel during an in-flight quick request must not silently join later.
  if(net!=null&&Loading&&sent&&net.Room?.phase=="Waiting")net.Command("leave");
 }
}
}
