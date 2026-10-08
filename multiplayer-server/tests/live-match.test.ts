import test from 'node:test';import assert from 'node:assert/strict';import {WebSocket} from 'ws';import {Store} from '../src/store.js';import {createServer} from '../src/main.js';
test('two real WS clients play three rounds to server-confirmed draw and durable reward',async()=>{
  const store=new Store(':memory:'),app=createServer(store);await new Promise<void>(r=>app.server.listen(0,'127.0.0.1',r));const port=(app.server.address() as any).port;
  async function client(name:string){const ws=new WebSocket(`ws://127.0.0.1:${port}/play`);let latest:any;let welcome:any;let serial=0;const errors:any[]=[];ws.on('message',raw=>{const m=JSON.parse(raw.toString());if(m.type==='state')latest=m;if(m.type==='welcome')welcome=m;if(m.type==='error')errors.push(m);});await new Promise<void>(r=>ws.once('open',()=>r()));
    const send=(type:string,values:any={})=>ws.send(JSON.stringify({type,request:name+(++serial),...values}));const wait=async(check:()=>boolean)=>{const end=Date.now()+8000;while(!check()){if(Date.now()>end)throw Error('live match timeout '+JSON.stringify(latest?.room?.phase));await new Promise(r=>setTimeout(r,10));}};
    send('hello',{name});await wait(()=>!!welcome);return {ws,send,wait,get state(){return latest;},get id(){return welcome.id;},errors};}
  try{const a=await client('Live A'),b=await client('Live B');a.send('create');await a.wait(()=>!!a.state?.room);b.send('join',{code:a.state.room.code});await b.wait(()=>b.state?.room?.players.length===2);a.send('ready',{ready:true});b.send('ready',{ready:true});
    for(let round=1;round<=3;round++){await a.wait(()=>a.state?.room?.phase==='Placement'&&a.state.room.round===round);await b.wait(()=>b.state?.room?.phase==='Placement'&&b.state.room.round===round);
      a.send('place',{round,seq:round*3-2,value:{x:-400,y:0}});a.send('aim',{round,seq:round*3-1,value:{x:1,y:0}});a.send('lock',{round,seq:round*3});
      await a.wait(()=>a.state.room.players.find((p:any)=>p.id===a.id).locked);assert.equal(a.state.room.players.find((p:any)=>p.id===b.id).action,null);
      b.send('place',{round,seq:round*3-2,value:{x:400,y:0}});b.send('aim',{round,seq:round*3-1,value:{x:-1,y:0}});b.send('lock',{round,seq:round*3});
    }
    await a.wait(()=>a.state?.room?.phase==='MatchResult');await b.wait(()=>b.state?.room?.phase==='MatchResult');assert.equal(a.state.room.resolution.outcome,'Draw');assert.equal(a.state.profile.coins,65);assert.equal(b.state.profile.coins,65);assert.equal(a.state.profile.history.length,1);assert.equal(a.errors.length+b.errors.length,0);
  }finally{await app.close();store.close();}
});
