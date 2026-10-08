namespace HitMe.UI {
public enum MenuPanel { Home, Connection, Settings, Practice, Maps, Characters, Notice }
public enum MenuRequestStatus { Idle, Connecting, WaitingForRoom, Failed }
// Presentation state only: no fighter, room, currency, inventory or bot data is written here.
public sealed class MenuScreenState {
 public MenuPanel Panel {get;private set;}=MenuPanel.Home;
 public MenuRequestStatus Request {get;private set;}=MenuRequestStatus.Idle;
 public string PendingIntent {get;private set;}="";
 public string Failure {get;private set;}="";
 public bool Sent {get;private set;}
 public bool AwaitingCancelledRoom {get;private set;}
 public bool Loading=>Request==MenuRequestStatus.Connecting||Request==MenuRequestStatus.WaitingForRoom;
 public void OpenPanel(MenuPanel panel){Panel=panel;}
 public void SelectIntent(string intent){if(Loading)return;PendingIntent=intent;Failure="";Request=MenuRequestStatus.Idle;}
 public void Begin(){Request=MenuRequestStatus.Connecting;Failure="";Sent=false;}
 public void MarkSent(){Sent=true;Request=MenuRequestStatus.WaitingForRoom;}
 public void Fail(string error){Failure=error;Request=MenuRequestStatus.Failed;}
 public void Complete(){PendingIntent="";Request=MenuRequestStatus.Idle;Sent=false;Failure="";}
 public void Cancel(){AwaitingCancelledRoom=Sent;Complete();Panel=MenuPanel.Home;}
 public void CancellationHandled(){AwaitingCancelledRoom=false;}
}
}
