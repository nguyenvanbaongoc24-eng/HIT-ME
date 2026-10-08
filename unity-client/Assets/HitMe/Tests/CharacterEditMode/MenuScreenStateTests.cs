using NUnit.Framework;
using HitMe.UI;
public sealed class MenuScreenStateTests {
 [Test]public void RequestStatesRetainIntentUntilCompletion(){var s=new MenuScreenState();s.SelectIntent("quick");s.Begin();Assert.IsTrue(s.Loading);Assert.AreEqual(MenuRequestStatus.Connecting,s.Request);s.MarkSent();Assert.AreEqual("quick",s.PendingIntent);Assert.AreEqual(MenuRequestStatus.WaitingForRoom,s.Request);s.Complete();Assert.IsFalse(s.Loading);Assert.IsEmpty(s.PendingIntent);}
 [Test]public void CancelAfterSendRetainsCleanupIntent(){var s=new MenuScreenState();s.SelectIntent("quick");s.Begin();s.MarkSent();s.Cancel();Assert.IsTrue(s.AwaitingCancelledRoom);Assert.AreEqual(MenuPanel.Home,s.Panel);Assert.IsFalse(s.Loading);Assert.IsEmpty(s.PendingIntent);s.CancellationHandled();Assert.IsFalse(s.AwaitingCancelledRoom);}
 [Test]public void BackBeforeConnectDoesNotJoinRoom(){var s=new MenuScreenState();s.SelectIntent("private");s.OpenPanel(MenuPanel.Connection);s.Cancel();Assert.IsEmpty(s.PendingIntent);Assert.IsFalse(s.AwaitingCancelledRoom);Assert.AreEqual(MenuRequestStatus.Idle,s.Request);}
 [Test]public void FailedRequestRemainsCancellable(){var s=new MenuScreenState();s.SelectIntent("quick");s.Begin();s.MarkSent();s.Fail("offline");Assert.IsFalse(s.Loading);Assert.AreEqual("offline",s.Failure);s.Cancel();Assert.IsTrue(s.AwaitingCancelledRoom);Assert.IsEmpty(s.Failure);}
}
