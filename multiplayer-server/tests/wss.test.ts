import test from 'node:test';import assert from 'node:assert/strict';
import selfsigned from 'selfsigned';import {WebSocket} from 'ws';import {Store} from '../src/store.js';import {createServer} from '../src/main.js';
test('WSS handshake verifies the test CA certificate',async()=>{
  const cert=selfsigned.generate([{name:'commonName',value:'localhost'}],{days:1,keySize:2048,algorithm:'sha256',extensions:[{name:'basicConstraints',cA:true},{name:'subjectAltName',altNames:[{type:2,value:'localhost'}]}]});
  const store=new Store(':memory:'),app=createServer(store,{tls:{key:Buffer.from(cert.private),cert:Buffer.from(cert.cert)}});
  await new Promise<void>(resolve=>app.server.listen(0,'127.0.0.1',resolve));
  try{const port=(app.server.address() as any).port;const socket=new WebSocket(`wss://localhost:${port}/play`,{ca:cert.cert});
    await new Promise<void>((resolve,reject)=>{socket.once('open',()=>resolve());socket.once('error',reject);});
    const welcome=new Promise<any>((resolve,reject)=>{socket.on('message',raw=>{const m=JSON.parse(raw.toString());if(m.type==='welcome')resolve(m);});setTimeout(()=>reject(Error('WSS welcome timeout')),2000).unref();});
    socket.send(JSON.stringify({type:'hello',request:'tls-1',name:'TLS Test'}));assert((await welcome).id);socket.close();
  }finally{await app.close();store.close();}
});
