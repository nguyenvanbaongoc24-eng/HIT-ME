import {writeFileSync} from 'node:fs';
import assert from 'node:assert/strict';
import {WebSocket} from 'ws';
import {Store} from '../src/store.js';
import {createServer} from '../src/main.js';
import {supabaseVerifier} from '../src/supabase-auth.js';
const url='https://ucvnnovebjawcmavgxyg.supabase.co',key='sb_publishable_ZzZXMcXNDm3GHeVyBG83WA_5UPMETZ_';
const evidence:any={checkedAt:new Date().toISOString(),status:'FAIL',identity:'Two separate real Supabase anonymous users',uiCoverage:'Protocol/REST evidence; not two physical browser sessions'};
const sessions:any[]=[],clients:any[]=[];let app:ReturnType<typeof createServer>|undefined,store:Store|undefined;
async function rest(path:string,session?:any,body?:any,method?:string){const response=await fetch(url+path,{method:method??(body?'POST':'GET'),headers:{apikey:key,'Content-Type':'application/json',...(session?{Authorization:'Bearer '+session.access_token}:{}),Prefer:'return=representation'},body:body?JSON.stringify(body):undefined,signal:AbortSignal.timeout(15000)});if(!response.ok)throw Error('Supabase HTTP '+response.status);return response.status===204?null:response.json();}
async function client(endpoint:string,session:any){const ws=new WebSocket(endpoint,{origin:'https://hit-me-game.vercel.app'});const c:any={ws,state:null,welcome:null,errors:[],serial:0};clients.push(c);
 ws.on('message',raw=>{const message=JSON.parse(raw.toString());if(message.type==='state')c.state=message;if(message.type==='welcome')c.welcome=message;if(message.type==='error')c.errors.push(message.message);});
 await new Promise<void>((resolve,reject)=>{const timer=setTimeout(()=>reject(Error('transport timeout')),45000);ws.once('open',()=>{clearTimeout(timer);resolve();});ws.once('error',()=>{clearTimeout(timer);reject(Error('transport failed'));});});
 c.send=(type:string,body:any={})=>ws.send(JSON.stringify({type,request:'ux-'+(++c.serial),version:'hitme-v1',...body}));
 c.wait=async(predicate:()=>boolean)=>{const end=Date.now()+20000;while(!predicate()){if(Date.now()>end)throw Error('state timeout: '+c.state?.room?.phase);await new Promise(resolve=>setTimeout(resolve,20));}};
 c.send('hello',{accessToken:session.access_token});await c.wait(()=>!!c.welcome);assert.equal(c.welcome.auth,'supabase');await c.wait(()=>!!c.state?.profile);return c;
}
try{
 for(let i=0;i<2;i++){const s:any=await rest('/auth/v1/signup',undefined,{data:{display_name:'QA Mobile '+i}});sessions.push(s);await rest('/rest/v1/profiles?user_id=eq.'+s.user.id,s,{display_name:i?'QA Bảo UX':'QA Ngọc UX',avatar:i?'Char03_BotFemale':'Char01_Player'},'PATCH');}
 assert.notEqual(sessions[0].user.id,sessions[1].user.id);evidence.distinctIdentities=true;
 let endpoint=process.env.HITME_PROBE_ENDPOINT;
 if(!endpoint){store=new Store(':memory:');app=createServer(store,{verifyIdentity:supabaseVerifier(url,key),requireIdentity:true});await new Promise<void>(resolve=>app!.server.listen(0,'127.0.0.1',resolve));endpoint=`ws://127.0.0.1:${(app.server.address() as any).port}/play`;}
 evidence.endpoint=endpoint;evidence.public=endpoint.startsWith('wss://');
 const a=await client(endpoint,sessions[0]),b=await client(endpoint,sessions[1]);
 assert.equal(a.state.profile.name,'QA Ngọc UX');assert.equal(b.state.profile.avatar,'Char03_BotFemale');evidence.namesAndAvatarsVerified=true;
 a.send('quick');await a.wait(()=>!!a.state.room);const first=a.state.room.id;a.send('quick');await a.wait(()=>a.state.room?.id===first);assert.equal(a.state.room.players.length,1);evidence.duplicateQueuePrevented=true;
 a.send('leave');await a.wait(()=>!a.state.room);evidence.cancelVerified=true;
 a.send('quick');await a.wait(()=>!!a.state.room);b.send('quick');await b.wait(()=>b.state.room?.players.length===2);await a.wait(()=>a.state.room?.players.length===2);
 assert.equal(a.state.room.id,b.state.room.id);assert.deepEqual(a.state.room.players.map((p:any)=>p.name).sort(),['QA Bảo UX','QA Ngọc UX'].sort());evidence.sameAuthoritativeRoom=true;
 for(let round=1;round<=3;round++){
  await a.wait(()=>a.state.room?.phase==='Placement'&&a.state.room.round===round);await b.wait(()=>b.state.room?.phase==='Placement'&&b.state.room.round===round);
  a.send('place',{round,seq:round*3-2,value:{x:-400,y:0}});a.send('aim',{round,seq:round*3-1,value:{x:1,y:0}});a.send('lock',{round,seq:round*3});
  await a.wait(()=>a.state.room.players.find((p:any)=>p.id===a.welcome.id).locked);assert.equal(a.state.room.players.find((p:any)=>p.id===b.welcome.id).action,null);
  b.send('place',{round,seq:round*3-2,value:{x:400,y:0}});b.send('aim',{round,seq:round*3-1,value:{x:-1,y:0}});b.send('lock',{round,seq:round*3});
 }
 await a.wait(()=>a.state.room?.phase==='MatchResult');await b.wait(()=>b.state.room?.phase==='MatchResult');assert.deepEqual(a.state.room.resolution,b.state.room.resolution);assert.equal(a.state.room.resolution.outcome,'Draw');assert.equal(a.state.profile.history.length,1);assert.equal(b.state.profile.history.length,1);assert.equal(a.errors.length+b.errors.length,0);
 evidence.sameResult=true;evidence.outcome='Draw';evidence.privacyVerified=true;evidence.rounds=3;evidence.serverLedgerVerified=true;
 const refreshed:any=await rest('/auth/v1/token?grant_type=refresh_token',undefined,{refresh_token:sessions[0].refresh_token});sessions[0]=refreshed;
 const resumed=await client(endpoint,refreshed);assert.equal(resumed.welcome.id,a.welcome.id);assert.equal(resumed.state.profile.name,'QA Ngọc UX');assert.equal(resumed.state.profile.avatar,'Char01_Player');evidence.refreshRestoresIdentityAndProfile=true;
 resumed.send('leave');b.send('leave');evidence.status='PASS';
}catch(error){evidence.error=error instanceof Error?error.message:'probe failed';process.exitCode=1;}
finally{for(const c of clients)c.ws.close();if(app)await app.close();store?.close();for(const session of sessions)try{await rest('/auth/v1/logout',session,undefined,'POST');}catch{} }
writeFileSync(process.env.HITME_PROBE_REPORT??'../docs/MOBILE_ONLINE_LOCAL_PROBE.json',JSON.stringify(evidence,null,2));console.log(JSON.stringify(evidence,null,2));
