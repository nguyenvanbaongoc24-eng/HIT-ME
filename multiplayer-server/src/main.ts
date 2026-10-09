import http from 'node:http';
import https from 'node:https';
import {readFileSync,mkdirSync} from 'node:fs';
import {WebSocketServer,WebSocket} from 'ws';
import {Store,catalog,modes} from './store.js';
import {Rooms} from './rooms.js';
export function createServer(store:Store,options:{tls?:{key:Buffer;cert:Buffer};allowedOrigins?:string[];proposedTimeout?:boolean}={}){
  const rooms=new Rooms(store,undefined,options.proposedTimeout??true),clients=new Map<string,WebSocket>();
  const handler:http.RequestListener=(req,res)=>{if(req.url==='/health'){res.writeHead(200,{'Content-Type':'application/json'});res.end(JSON.stringify({service:'hit-me',transport:options.tls?'wss':'ws-dev',players:clients.size}));}else{res.writeHead(404);res.end();}};
  const server=options.tls?https.createServer(options.tls,handler):http.createServer(handler);
  const wss=new WebSocketServer({noServer:true,maxPayload:8192});
  const defaultOrigins = ['http://127.0.0.1:8791','http://localhost:8791','https://hit-me-game.vercel.app'];
  server.on('upgrade',(req,socket,head)=>{const origin=req.headers.origin;if(req.url!=='/play'||origin&&!(options.allowedOrigins??defaultOrigins).includes(origin)){socket.destroy();return;}wss.handleUpgrade(req,socket,head,ws=>wss.emit('connection',ws,req));});
  const send=(ws:WebSocket,data:unknown)=>{if(ws.readyState===WebSocket.OPEN)ws.send(JSON.stringify(data));};
  const broadcast=()=>{for(const [id,ws] of clients)send(ws,{type:'state',room:rooms.snapshot(id),profile:store.profile(id),serverTime:Date.now()});};
  wss.on('connection',ws=>{let player='';let alive=true;let budget=0;let windowStart=Date.now();const cache=new Map<string,unknown>();
    const authTimeout=setTimeout(()=>{if(!player)ws.close(4001,'auth_required');},10000);
    ws.on('pong',()=>{alive=true;});(ws as any).heartbeat=()=>{if(!alive){ws.terminate();return;}alive=false;ws.ping();};
    ws.on('message',raw=>{let request='';try{if(Date.now()-windowStart>1000){budget=0;windowStart=Date.now();}if(++budget>40)throw Error('rate_limited');const m=JSON.parse(raw.toString());request=m.request;
      if(typeof request!=='string'||request.length>80||request.length<1)throw Error('request_required');
      if(cache.has(request)){send(ws,cache.get(request));return;}
      if(m.type==='hello'){if(player)throw Error('already_authenticated');const session=store.guest(m.name,m.token);player=session.id;const old=clients.get(player);clients.set(player,ws);if(old&&old!==ws)old.close(4002,'session_replaced');rooms.reconnect(player);clearTimeout(authTimeout);send(ws,{type:'welcome',request,...session,catalog,modes});}
      else {if(!player)throw Error('auth_required');switch(m.type){
        case 'create':rooms.create(player,true);break;case 'join':if(typeof m.code!=='string')throw Error('invalid_code');rooms.join(player,m.code);break;
        case 'quick':rooms.quick(player);break;case 'ready':if(typeof m.ready!=='boolean')throw Error('invalid_ready');rooms.ready(player,m.ready);break;
        case 'leave':rooms.leave(player);break;case 'place':case 'aim':case 'lock':rooms.action(player,m.seq,m.type,m.value??null,m.round);break;
        case 'profile':{
          const room=rooms.membership.has(player)?rooms.room(player):null;
          if(room&&room.phase!=='Waiting')throw Error('match_started');
          store.updateProfile(player,m.name,m.avatar);
          if(room){const member=room.players.find(p=>p.id===player)!;const profile=store.profile(player);member.name=profile.name;member.avatar=profile.avatar;}
          break;
        }
        case 'equip':store.equip(player,m.item);if(rooms.membership.has(player))rooms.room(player).players.find(p=>p.id===player)!.weapon=m.item;break;case 'claim':store.claim(player,m.period,m.kind);break;case 'heartbeat':break;
        default:throw Error('unsupported_request'); // reward / balance / result writes rejected
      }}
      const response={type:'ack',request,serverTime:Date.now()};cache.set(request,response);if(cache.size>128)cache.delete(cache.keys().next().value!);send(ws,response);broadcast();
    }catch(e){send(ws,{type:'error',request,message:e instanceof Error?e.message:'invalid_message',serverTime:Date.now()});}});
    ws.on('close',()=>{clearTimeout(authTimeout);if(player&&clients.get(player)===ws){clients.delete(player);rooms.disconnect(player);broadcast();}});
    ws.on('error',()=>{});
  });
  const tick=setInterval(()=>{try{rooms.tick();broadcast();}catch(e){console.error('room tick failed',e);}},100);
  const heartbeat=setInterval(()=>{for(const ws of wss.clients)(ws as any).heartbeat();},15000);
  return {server,rooms,wss,close:async()=>{clearInterval(tick);clearInterval(heartbeat);for(const ws of wss.clients)ws.terminate();await new Promise<void>(resolve=>wss.close(()=>resolve()));await new Promise<void>(resolve=>server.close(()=>resolve()));}};
}
if(process.argv[1]?.endsWith('main.js')||process.argv[1]?.endsWith('main.ts')){
  mkdirSync('data',{recursive:true});const tls=process.env.TLS_CERT&&process.env.TLS_KEY?{cert:readFileSync(process.env.TLS_CERT),key:readFileSync(process.env.TLS_KEY)}:undefined;
  if(process.env.NODE_ENV==='production'&&!tls&&process.env.BEHIND_PROXY!=='true'&&process.env.RENDER!=='true')throw Error('WSS requires TLS_CERT and TLS_KEY');
  const service=createServer(new Store(process.env.DB_PATH??'data/hitme.sqlite'),{tls,proposedTimeout:process.env.PROPOSED_TIMEOUT!=="false",allowedOrigins:process.env.ALLOWED_ORIGINS?.split(',')});
  const behindProxy=process.env.BEHIND_PROXY==='true'||process.env.RENDER==='true';
  if(!tls&&!behindProxy&&process.env.HOST&&!['127.0.0.1','localhost','::1'].includes(process.env.HOST))throw Error('Non-local listener requires WSS');
  const host=process.env.HOST??(behindProxy?'0.0.0.0':'127.0.0.1');
  service.server.listen(Number(process.env.PORT??8788),host,()=>console.log(`HIT ME ${tls?'WSS':behindProxy?'WS (Behind Proxy)':'WS development'} listening on ${host}:${process.env.PORT??8788}`));
}
