using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using UnityEngine;
#if !UNITY_WEBGL || UNITY_EDITOR
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endif
namespace HitMe.UI
{
    [Serializable] public sealed class NetPoint { public double x,y; }
    [Serializable] public sealed class NetAction { public string id; public NetPoint position,direction; public bool canThrow; }
    [Serializable] public sealed class NetPlayer { public string id,name,weapon,avatar; public int hp,seq; public bool ready,connected,locked; public NetAction action; }
    [Serializable] public sealed class NetThrow { public string thrower,target; public NetPoint origin,end; public int damage; }
    [Serializable] public sealed class NetHealth { public string id; public int before,after; }
    [Serializable] public sealed class NetResolution { public string outcome,winner; public NetThrow[] throws; public NetHealth[] health; }
    [Serializable] public sealed class NetMatchStats {public string id;public int throws,hits,misses,received;}
    [Serializable] public sealed class NetRoom { public string id,code,owner,phase,match,timeoutPolicy; public int round; public double deadline,serverTime; public bool @private; public NetPlayer[] players; public NetResolution resolution; public NetMatchStats[] stats; }
    [Serializable] public sealed class NetItem { public string item; public int quantity; }
    [Serializable] public sealed class NetQuest { public string period,kind,state; public int target,progress; }
    [Serializable] public sealed class NetReward { public string match,outcome;public int coins,xp;public double created; }
    [Serializable] public sealed class NetProfile { public string id,name,avatar,equipped; public int xp,coins,level; public NetItem[] inventory; public NetQuest[] quests; public NetReward[] history; }
    [Serializable] public sealed class NetEnvelope { public string type,id,token,message,request; public NetRoom room; public NetProfile profile; public double serverTime; }
    [Serializable] sealed class NetRequest { public string type,request,name,avatar,token,code,item,period,kind; public bool ready; public int seq,round; public NetPoint value; }
    public sealed class NetworkSession : MonoBehaviour
    {
        public static NetworkSession Instance {get;private set;}
        public NetRoom Room {get;private set;} public NetProfile Profile {get;private set;}
        public string PlayerId {get;private set;} public string Error {get;private set;}="";
        public bool Connected {get;private set;} public bool OnlineBattle {get;set;}
        public string ErrorText=>Error.Length==0?"":Strings.Get("error"+Error).StartsWith("[")?Strings.Get(Error):Strings.Get("error"+Error);
        public int Revision {get;private set;} int seq; string token="",endpoint="",guestName="Quest";
        double serverOffset,nextRetry; int attempts; bool intentionalClose;
        readonly ConcurrentQueue<string> incoming=new ConcurrentQueue<string>();
        public double ServerNow=>DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()+serverOffset;
        public NetPlayer Self=>Room?.players==null?null:Array.Find(Room.players,p=>p.id==PlayerId);
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] static extern void HitMeNetConnect(string url);
        [DllImport("__Internal")] static extern void HitMeNetSend(string json);
        [DllImport("__Internal")] static extern void HitMeNetSaveToken(string value);
#else
        ClientWebSocket socket; readonly SemaphoreSlim sendLock=new SemaphoreSlim(1);
#endif
        public static NetworkSession Ensure(){if(Instance!=null)return Instance;return new GameObject("NetworkSession").AddComponent<NetworkSession>();}
        void Awake(){if(Instance!=null){Destroy(gameObject);return;}Instance=this;DontDestroyOnLoad(gameObject);}
        public void Connect(string url,string name){if(!NetworkEndpointSettings.IsAllowed(url)){Error="public_wss_required";Revision++;return;}endpoint=url;guestName=name;intentionalClose=false;attempts=0;Open();}
        void Open(){Error="";nextRetry=double.PositiveInfinity;
#if UNITY_WEBGL && !UNITY_EDITOR
            HitMeNetConnect(endpoint);
#else
            _=OpenEditor();
#endif
        }
#if !UNITY_WEBGL || UNITY_EDITOR
        async Task OpenEditor(){try{socket?.Dispose();socket=new ClientWebSocket();await socket.ConnectAsync(new Uri(endpoint),CancellationToken.None);incoming.Enqueue("{\"type\":\"opened\"}");var buffer=new byte[16384];
            while(socket.State==WebSocketState.Open){var result=await socket.ReceiveAsync(new ArraySegment<byte>(buffer),CancellationToken.None);if(result.MessageType==WebSocketMessageType.Close)break;if(!result.EndOfMessage)throw new Exception("message_too_large");incoming.Enqueue(Encoding.UTF8.GetString(buffer,0,result.Count));}
        }catch(Exception e){incoming.Enqueue(JsonUtility.ToJson(new NetEnvelope{type="transport_error",message=e.Message}));}finally{incoming.Enqueue("{\"type\":\"closed\"}");}}
        async Task SendEditor(string json){await sendLock.WaitAsync();try{if(socket?.State==WebSocketState.Open)await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)),WebSocketMessageType.Text,true,CancellationToken.None);}catch(Exception e){incoming.Enqueue(JsonUtility.ToJson(new NetEnvelope{type="transport_error",message=e.Message}));}finally{sendLock.Release();}}
#endif
        public void Receive(string json){incoming.Enqueue(json);}
        void Update(){while(incoming.TryDequeue(out var json)){var e=JsonUtility.FromJson<NetEnvelope>(json);if(e==null)continue;
            switch(e.type){case "opened":Connected=true;Send(new NetRequest{type="hello",name=guestName,token=token});break;
                case "stored_token":token=e.token??"";break;
                case "welcome":PlayerId=e.id;token=e.token;attempts=0;Error="";
#if UNITY_WEBGL && !UNITY_EDITOR
                    HitMeNetSaveToken(token);
#endif
                    break;
                case "state":Room=string.IsNullOrEmpty(e.room?.id)?null:e.room;Profile=string.IsNullOrEmpty(e.profile?.id)?null:e.profile;
                    if(Room?.players!=null)foreach(var p in Room.players)if(string.IsNullOrEmpty(p.action?.id))p.action=null;
                    if(Room!=null&&string.IsNullOrEmpty(Room.resolution?.outcome))Room.resolution=null;serverOffset=e.serverTime-DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();if(Self!=null)seq=Math.Max(seq,Self.seq);Revision++;break;
                case "ack":Error="";Revision++;break;
                case "error":Error=e.message;Revision++;break;
                case "transport_error":Error=e.message;Revision++;break;
                case "closed":Connected=false;Error="networkDisconnected";nextRetry=Time.unscaledTimeAsDouble+Math.Min(10,1+attempts++);Revision++;break;
            }}if(!Connected&&!intentionalClose&&endpoint.Length>0&&Time.unscaledTimeAsDouble>=nextRetry){nextRetry=double.PositiveInfinity;Open();}}
        void Send(NetRequest r){r.request=Guid.NewGuid().ToString("N");var json=JsonUtility.ToJson(r);
#if UNITY_WEBGL && !UNITY_EDITOR
            HitMeNetSend(json);
#else
            _=SendEditor(json);
#endif
        }
        public void Command(string type,string argument="",bool ready=false){Send(new NetRequest{type=type,code=argument,item=argument,ready=ready});}
        public void SaveProfile(string name,string avatar){Send(new NetRequest{type="profile",name=name,avatar=avatar});}
        public void Action(string type,NetPoint point=null){if(Self==null||Self.locked||Room.phase!="Placement")return;Send(new NetRequest{type=type,value=point,round=Room.round,seq=++seq});}
        public void Claim(NetQuest q){Send(new NetRequest{type="claim",kind=q.kind,period=q.period});}
        void OnDestroy(){if(Instance==this)Instance=null;intentionalClose=true;
#if !UNITY_WEBGL || UNITY_EDITOR
            socket?.Dispose();
#endif
        }
    }
}
