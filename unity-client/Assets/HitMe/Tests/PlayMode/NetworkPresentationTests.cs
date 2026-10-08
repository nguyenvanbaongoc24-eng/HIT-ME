using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using HitMe.UI;
public sealed class NetworkPresentationTests
{
    [UnityTest] public IEnumerator ServerSnapshotKeepsOpponentHiddenAndLockedInputDisabled(){
        var net=NetworkSession.Ensure();net.Receive("{\"type\":\"state\",\"room\":null}");yield return null;Assert.IsNull(net.Room);net.Receive("{\"type\":\"welcome\",\"id\":\"self\",\"token\":\"test\"}");
        net.Receive("{\"type\":\"state\",\"profile\":{\"id\":\"self\",\"coins\":0,\"xp\":0},\"room\":{\"id\":\"test-room\",\"phase\":\"Placement\",\"round\":1,\"players\":[{\"id\":\"self\",\"hp\":3,\"action\":{\"id\":\"self\",\"position\":{\"x\":0,\"y\":0},\"direction\":{\"x\":1,\"y\":0},\"canThrow\":true}},{\"id\":\"other\",\"hp\":3,\"action\":null}]}}");
        yield return null;var go=new GameObject("NetworkBattleTest");var view=go.AddComponent<BattleView>();view.Initialize();view.StartOnline();yield return null;
        Assert.AreEqual(1,view.VisibleActorCount);Assert.AreEqual(2,net.Room.players.Length);net.Room.players[0].locked=true;Assert.IsFalse(view.CanPlace);
        net.Room.phase="Reveal";net.Room.players[1].action=new NetAction{position=new NetPoint{x=400,y=0},direction=new NetPoint{x=-1,y=0},canThrow=true};yield return null;Assert.AreEqual(2,view.VisibleActorCount);
        Assert.AreEqual(3,net.Room.players[0].hp);
        net.Room.phase="MatchResult";net.Room.resolution=new NetResolution{outcome="Draw"};net.Profile.history=new NetReward[0];yield return null;
        Assert.IsNotNull(go.transform.Find("BattleCanvas/SafeAreaHUD/OnlineResult"));
        view.SafeAreaOverride=new Rect(0,20,Screen.width,Screen.height-60);yield return null;yield return null;
        Assert.IsNotNull(go.transform.Find("BattleCanvas/SafeAreaHUD/OnlineResult"),"Result must survive responsive rebuild.");
        UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(net.gameObject);yield return null;LogAssert.NoUnexpectedReceived();
    }
}
