mergeInto(LibraryManager.library, {
  HitMeNetConnect: function(urlPtr) {
    var url=UTF8ToString(urlPtr),generation=(Module.hitMeGeneration||0)+1;Module.hitMeGeneration=generation;
    var receive=function(event){if(Module.hitMeGeneration===generation)SendMessage('NetworkSession','Receive',JSON.stringify(event));};
    if(!/^wss:\/\//.test(url)&&!/^ws:\/\/(127\.0\.0\.1|localhost):/.test(url)){receive({type:'transport_error',message:'public_wss_required'});return;}
    if(Module.hitMeSocket){Module.hitMeSocket.onclose=null;Module.hitMeSocket.close();}
    if(!window.hitMeSession){receive({type:'auth_failed',message:'auth_configuration_missing'});return;}
    window.hitMeSession.prepare().then(function(session){
      if(Module.hitMeGeneration!==generation)return;
      Module.hitMeAuth=session;receive({type:'auth_ready',needsOnboarding:window.hitMeSession.needsOnboarding(),code:(function(){var code=new URLSearchParams(location.search).get("room")||"";return /^[0-9a-f]{8}$/i.test(code)?code:"";})()});
      var socket=new WebSocket(url);Module.hitMeSocket=socket;
      socket.onopen=function(){receive({type:'opened'});};
      socket.onmessage=function(e){if(Module.hitMeGeneration===generation)SendMessage('NetworkSession','Receive',e.data);};
      socket.onerror=function(){receive({type:'transport_error',message:'networkConnectionFailed'});};
      socket.onclose=function(){receive({type:'closed'});};
    }).catch(function(error){receive({type:'auth_failed',message:error.message==='invalid_session'?'invalid_session':'networkConnectionFailed'});});
  },
  HitMeNetSend: function(ptr){
    var json=UTF8ToString(ptr),message=JSON.parse(json),socket=Module.hitMeSocket;
    if(!socket||socket.readyState!==1)return;
    if(message.type==='hello'){message.accessToken=Module.hitMeAuth.access_token;message.token='';socket.send(JSON.stringify(message));return;}
    if(message.type==='profile'){
      window.hitMeSession.saveProfile(message.name,message.avatar).then(function(){return window.hitMeSession.prepare();}).then(function(session){if(Module.hitMeSocket===socket&&socket.readyState===1){message.accessToken=session.access_token;socket.send(JSON.stringify(message));SendMessage('NetworkSession','Receive','{"type":"profile_saved"}');}})
      .catch(function(error){SendMessage('NetworkSession','Receive',JSON.stringify({type:'error',message:error.message}));});return;
    }
    socket.send(json);
  },
  HitMeNetSaveToken: function(ptr){/* Supabase refresh session is persisted by hitme-session.js. */}
});
