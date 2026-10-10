using UnityEngine;
using UnityEngine.InputSystem;
namespace HitMe.UI {
public sealed class MainMenuController : MonoBehaviour {
 public MainMenuView View {get;private set;}
 public MenuScreenState State {get;}=new MenuScreenState();
 public bool Loading=>State.Loading;
 public string Pending=>State.PendingIntent;
 public float SearchElapsed=>Mathf.Max(0,Time.unscaledTime-started);
 public NetworkSession Session=>net;
 NetworkSession net;float started;int revision=-1;bool onboardingShown,queueSeen;
 void Start(){Strings.Load(Strings.Language);OfflineRunContext.LoadDefaults();WebMobileBridge.Ensure();net=NetworkSession.Ensure();View=gameObject.AddComponent<MainMenuView>();View.Initialize(this);net.EnsureConnected();}
 public NetProfile Profile=>net?.Profile;
 public bool Connected=>net!=null&&net.Connected&&net.Profile!=null;
 public void Online(string intent){if(Loading)return;State.SelectIntent(intent);
  if(net.NeedsOnboarding&&Connected){View.PlayerProfile();return;}
  if(net.Room!=null&&Connected){State.Complete();MenuNavigationService.Lobby();return;}
  net.EnsureConnected();Begin();
 }
 public void Connect(string endpoint,string guest){if(Loading)return;net.Connect(endpoint,guest);Begin();}
 public void RetryConnection(){net.Connect(NetworkEndpointSettings.Current,PlayerPrefs.GetString("PlayerDisplayName",Strings.Language=="vi"?"Khách":"Guest"));View.CloseModal();View.RefreshStatus();}
 void Begin(){queueSeen=false;State.Begin();started=Time.unscaledTime;View.CloseModal();if(Pending=="quick")View.Searching();View.RefreshStatus();}
 public void Cancel(){bool sent=State.Sent;State.Cancel();if(sent&&net.Room!=null&&(net.Room.phase=="Waiting"||net.Room.phase=="Countdown")){net.Command("leave");State.CancellationHandled();}View.CloseModal();View.RefreshStatus();}
 public void Back(){if(Loading||State.Request==MenuRequestStatus.Failed)Cancel();else View.CloseModal();}
 public void OpenAccount(string page){if(page=="profile"){View.PlayerProfile();return;}if(Loading)return;State.SelectIntent(page);net.EnsureConnected();Begin();}
 void Update(){if(net==null||View==null)return;if(net.Revision!=revision){revision=net.Revision;View.RefreshStatus();}
  if(Keyboard.current!=null&&Keyboard.current.escapeKey.wasPressedThisFrame&&State.Panel!=MenuPanel.Home)Back();
  if(Connected&&net.NeedsOnboarding&&!onboardingShown&&!Loading){onboardingShown=true;View.PlayerProfile();}
  if(State.AwaitingCancelledRoom&&net.Room!=null){if(net.Room.phase=="Waiting"||net.Room.phase=="Countdown")net.Command("leave");State.CancellationHandled();}
    if(Connected&&!net.NeedsOnboarding&&!Loading&&net.Room==null&&net.InvitationCode.Length>0){var code=net.InvitationCode;net.InvitationCode="";net.Command("join",code);MenuNavigationService.Lobby("private");return;}
  if(!Loading)return;
  if(Pending=="quick")View.UpdateSearch();
  if(!Connected){if(!net.Connecting&&net.Error.Length>0){State.Fail(net.ErrorText);View.Connection();}return;}
  if(net.NeedsOnboarding){State.Complete();View.PlayerProfile();return;}
  if(Pending=="quick"){
    if(net.Room!=null)queueSeen=true;
    if(queueSeen&&net.Room==null){State.Complete();State.Fail(Strings.Get("matchQueueExpired"));View.Notice("matchSearching",Strings.Get("matchQueueExpired"));return;}
    if(!State.Sent){State.MarkSent();net.Command("quick");}
    View.UpdateSearch();
    if(net.Room!=null&&net.Room.phase!="Waiting"){State.Complete();MenuNavigationService.Lobby();}
    return;
  }
  var page=Pending=="private"?"private":Pending;State.Complete();MenuNavigationService.Lobby(page);
 }
 void OnDestroy(){if(net!=null&&Loading&&State.Sent&&net.Room?.phase=="Waiting")net.Command("leave");}
}
}
