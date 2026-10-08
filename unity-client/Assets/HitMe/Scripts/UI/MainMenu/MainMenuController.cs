using UnityEngine;
using UnityEngine.InputSystem;
namespace HitMe.UI {
public sealed class MainMenuController : MonoBehaviour {
 public MainMenuView View {get;private set;}
 public MenuScreenState State {get;}=new MenuScreenState();
 public bool Loading=>State.Loading;
 public string Pending=>State.PendingIntent;
 NetworkSession net; float started; int revision=-1;
 void Start(){Strings.Load(Strings.Language);OfflineRunContext.LoadDefaults();WebMobileBridge.Ensure();net=NetworkSession.Ensure();View=gameObject.AddComponent<MainMenuView>();View.Initialize(this);}
 public NetProfile Profile=>net?.Profile;
 public bool Connected=>net!=null&&net.Connected&&net.Profile!=null;
 public void Online(string intent){if(Loading)return;State.SelectIntent(intent);
  if(!Connected){View.Connection();return;}if(net.Room!=null){State.Complete();MenuNavigationService.Lobby();return;}Begin();}
 public void Connect(string endpoint,string guest){if(Loading)return;PlayerPrefs.SetString("NetworkEndpoint",endpoint);net.Connect(endpoint,guest);Begin();}
 void Begin(){State.Begin();started=Time.unscaledTime;View.CloseModal();View.RefreshStatus();}
 public void Cancel(){State.Cancel();if(net.Room!=null&&(net.Room.phase=="Waiting"||net.Room.phase=="Countdown")){net.Command("leave");State.CancellationHandled();}View.CloseModal();View.RefreshStatus();}
 public void Back(){if(State.Panel==MenuPanel.Connection||Loading||State.Request==MenuRequestStatus.Failed)Cancel();else View.CloseModal();}
 public void OpenAccount(string page){if(Loading)return;State.SelectIntent(page);if(!Connected)View.Connection();else{State.Complete();MenuNavigationService.Lobby(page);}}
 void Update(){if(net==null||View==null)return;if(net.Revision!=revision){revision=net.Revision;View.RefreshStatus();}
  if(Keyboard.current!=null&&Keyboard.current.escapeKey.wasPressedThisFrame&&State.Panel!=MenuPanel.Home){Back();}
  if(State.AwaitingCancelledRoom&&net.Room!=null){if(net.Room.phase=="Waiting"||net.Room.phase=="Countdown")net.Command("leave");else View.Notice("online",Strings.Get("menuMatchAlreadyStarted"));State.CancellationHandled();}
  if(!Loading)return;
  if(net.Error.Length>0){State.Fail(net.ErrorText);View.Notice("menuNetworkError",net.ErrorText);View.RefreshStatus();return;}
  if(Time.unscaledTime-started>15){State.Fail(Strings.Get("menuConnectTimeout"));View.Notice("menuNetworkError",State.Failure);View.RefreshStatus();return;}
  if(!Connected)return;
  if(net.Room!=null){State.Complete();MenuNavigationService.Lobby();return;}
  if(!State.Sent){State.MarkSent();if(Pending=="quick")net.Command("quick");else{var page=Pending=="private"?"lobby":Pending;State.Complete();MenuNavigationService.Lobby(page);return;}}
 }
 void OnDestroy(){// A cancel during an in-flight quick request must not silently join later.
  if(net!=null&&Loading&&State.Sent&&net.Room?.phase=="Waiting")net.Command("leave");
 }
}
}
