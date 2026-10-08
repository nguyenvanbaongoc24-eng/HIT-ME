mergeInto(LibraryManager.library, {
  HitMeNetConnect: function(urlPtr) {
    var url=UTF8ToString(urlPtr);
    if(!/^wss:\/\//.test(url)&&!/^ws:\/\/(127\.0\.0\.1|localhost):/.test(url)){SendMessage('NetworkSession','Receive',JSON.stringify({type:'transport_error',message:'WSS required'}));return;}
    var token=sessionStorage.getItem('hitme.session')||'';
    SendMessage('NetworkSession','Receive',JSON.stringify({type:'stored_token',token:token}));
    if(Module.hitMeSocket){Module.hitMeSocket.onclose=null;Module.hitMeSocket.close();}
    var socket=new WebSocket(url);Module.hitMeSocket=socket;
    socket.onopen=function(){SendMessage('NetworkSession','Receive','{"type":"opened"}');};
    socket.onmessage=function(e){SendMessage('NetworkSession','Receive',e.data);};
    socket.onerror=function(){SendMessage('NetworkSession','Receive','{"type":"transport_error","message":"networkConnectionFailed"}');};
    socket.onclose=function(){SendMessage('NetworkSession','Receive','{"type":"closed"}');};
  },
  HitMeNetSend: function(ptr){var s=Module.hitMeSocket;if(s&&s.readyState===1)s.send(UTF8ToString(ptr));},
  HitMeNetSaveToken: function(ptr){sessionStorage.setItem('hitme.session',UTF8ToString(ptr));}
});
