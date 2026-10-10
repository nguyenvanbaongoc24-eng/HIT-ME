import {test} from 'node:test';
import assert from 'node:assert/strict';
import {Store} from '../src/store.js';
import {Rooms} from '../src/rooms.js';
import {createServer} from '../src/main.js';
import {WebSocket} from 'ws';
test('public queue is idempotent, cancels, reconnects and automatically starts with real minimum',()=>{
 const store=new Store(':memory:');let now=0;const rooms=new Rooms(store,()=>now,true,100,2);
 try{const a=store.guest('An'),b=store.guest('Bảo'),c=store.guest('Chi');
  const room=rooms.quick(a.id);assert.equal(rooms.quick(a.id).id,room.id);assert.equal(room.players.length,1);assert.equal(room.phase,'Waiting');
  rooms.quick(b.id);assert.equal(rooms.room(b.id).id,room.id);assert.equal(room.phase,'Countdown');
  rooms.leave(b.id);assert.equal(room.phase,'Waiting');assert.equal(rooms.membership.has(b.id),false);
  rooms.quick(c.id);assert.equal(room.phase,'Countdown');rooms.disconnect(c.id);assert.equal(room.phase,'Waiting');rooms.reconnect(c.id);assert.equal(room.phase,'Countdown');
  now=3001;rooms.tick();assert.equal(room.phase,'Placement');assert.equal(room.players.length,2);assert.equal(rooms.snapshot(a.id)?.players[1].action,null);
 }finally{store.close();}
});
test('authenticated identity cannot overwrite a different player or legacy economy',()=>{
 const store=new Store(':memory:');try{const legacy=store.guest('Legacy');store.db.prepare('UPDATE profiles SET coins=73 WHERE id=?').run(legacy.id);
  const a=store.authenticated({id:'aaaaaaaa-aaaa-4aaa-aaaa-aaaaaaaaaaaa',name:'An',avatar:'Char01_Player'});
  const b=store.authenticated({id:'bbbbbbbb-bbbb-4bbb-bbbb-bbbbbbbbbbbb',name:'Bảo',avatar:'Char03_BotFemale'});
  store.authenticated({id:'aaaaaaaa-aaaa-4aaa-aaaa-aaaaaaaaaaaa',name:'An mới',avatar:'Char02_BotMale'});
  assert.equal(store.profile(a.id).name,'An mới');assert.equal(store.profile(b.id).name,'Bảo');assert.equal(store.profile(legacy.id).coins,73);
 }finally{store.close();}
});
test('public queue expires without creating bots or awarding coins',()=>{
 const store=new Store(':memory:');let now=0;const rooms=new Rooms(store,()=>now,true,30000,2,1000);
 try{const player=store.guest('Waiting');rooms.quick(player.id);now=1001;rooms.tick();assert.equal(rooms.snapshot(player.id),null);assert.equal(rooms.membership.size,0);assert.equal(store.profile(player.id).coins,0);}finally{store.close();}
});
test('production auth rejects invalid bearer and never welcomes before verification',async()=>{
 const store=new Store(':memory:');const app=createServer(store,{requireIdentity:true,verifyIdentity:async()=>{throw Error('invalid_session');}});
 await new Promise<void>(resolve=>app.server.listen(0,'127.0.0.1',resolve));
 const ws=new WebSocket(`ws://127.0.0.1:${(app.server.address() as any).port}/play`);
 try{await new Promise(resolve=>ws.once('open',resolve));const message=new Promise<any>(resolve=>ws.once('message',raw=>resolve(JSON.parse(raw.toString()))));
  ws.send(JSON.stringify({type:'hello',request:'auth-test',accessToken:'invalid'}));assert.equal((await message).message,'invalid_session');assert.equal(app.rooms.membership.size,0);
 }finally{await app.close();store.close();}
});
