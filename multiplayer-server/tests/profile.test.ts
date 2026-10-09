import {test} from 'node:test';
import assert from 'node:assert/strict';
import {Store,validateNickname} from '../src/store.js';
import {WebSocket} from 'ws';
import {createServer} from '../src/main.js';
import {betaRewardProposal} from '../src/reward-policy.js';
import {mkdtempSync,unlinkSync,rmdirSync,existsSync} from 'node:fs';
import {tmpdir} from 'node:os';
import {join} from 'node:path';
test('Vietnamese nickname validation rejects markup, controls, invisible characters and excess length',()=>{
  assert.equal(validateNickname('  Nguyễn Văn An  '),'Nguyễn Văn An');
  assert.equal(validateNickname('Nguye\u0302\u0303n'),'Nguyễn');
  for(const name of ['', 'a'.repeat(25),'<b>An</b>','An\nBảo','An\u200bBảo'])assert.throws(()=>validateNickname(name),/invalid_name/);
});
test('real WS profile update is scoped to authenticated user and broadcasts lobby avatar',async()=>{
  const store=new Store(':memory:'),app=createServer(store);await new Promise<void>(resolve=>app.server.listen(0,'127.0.0.1',resolve));
  const ws=new WebSocket(`ws://127.0.0.1:${(app.server.address() as any).port}/play`),inbox:any[]=[];
  ws.on('message',raw=>inbox.push(JSON.parse(raw.toString())));
  await new Promise(resolve=>ws.once('open',resolve));let request=0;
  const send=(type:string,payload:any={})=>{inbox.length=0;ws.send(JSON.stringify({type,request:String(++request),...payload}));};
  const wait=async(predicate:(m:any)=>boolean)=>{const end=Date.now()+2000;while(Date.now()<end){const result=inbox.find(predicate);if(result)return result;await new Promise(resolve=>setTimeout(resolve,10));}throw Error('timeout');};
  try{
    send('hello',{name:'Ngọc'});const welcome=await wait(m=>m.type==='welcome');
    const other=store.guest('Bảo');send('create');await wait(m=>m.type==='state'&&m.room);
    send('profile',{id:other.id,name:'Ngọc mới',avatar:'Char03_BotFemale',coins:999999});
    const state=await wait(m=>m.type==='state'&&m.profile.name==='Ngọc mới');
    assert.equal(state.profile.id,welcome.id);assert.equal(state.profile.coins,0);assert.equal(store.profile(other.id).name,'Bảo');
    assert.equal(state.room.players[0].avatar,'Char03_BotFemale');assert.equal(state.room.players[0].name,'Ngọc mới');
    app.rooms.join(other.id,state.room.code);app.rooms.ready(welcome.id,true);app.rooms.ready(other.id,true);
    send('profile',{name:'Forbidden',avatar:'Char01_Player'});assert.equal((await wait(m=>m.type==='error')).message,'match_started');
    assert.equal(store.profile(welcome.id).name,'Ngọc mới');
  }finally{await app.close();store.close();}
});
test('profile update preserves identity, balance and ownership and rejects arbitrary avatar',()=>{
  const store=new Store(':memory:');try{
    const a=store.guest('Ngọc'),b=store.guest('Bảo');
    store.updateProfile(a.id,'  Nguyễn Ngọc  ','Char03_BotFemale');
    const p=store.profile(a.id);assert.equal(p.id,a.id);assert.equal(p.name,'Nguyễn Ngọc');assert.equal(p.avatar,'Char03_BotFemale');
    assert.equal(p.coins,0);assert.equal(p.xp,0);assert.equal(p.inventory.length,3);assert.equal(store.profile(b.id).name,'Bảo');
    assert.equal(store.guest('ignored',a.token).id,a.id);
    assert.throws(()=>store.updateProfile(a.id,'Valid','Premium'),/invalid_avatar/);
    assert.equal(store.profile(a.id).avatar,'Char03_BotFemale');
  }finally{store.close();}
});
test('trial reward ledger records version and inactive beta cannot silently replace payouts',()=>{
  const store=new Store(':memory:');try{const a=store.guest('Ngọc'),b=store.guest('Bảo');store.reward('versioned',[a.id,b.id],a.id,false,true);store.reward('versioned',[a.id,b.id],a.id,false,true);
    const p=store.profile(a.id);assert.equal(p.coins,100);assert.equal(p.history.length,1);assert.equal(p.history[0].policy_version,'trial-v1');assert.equal(betaRewardProposal.enabled,false);assert.equal(betaRewardProposal.tieRule,null);
  }finally{store.close();}
});
test('additive SQLite ledger upgrade preserves legacy balances and reward history',()=>{
  const dir=mkdtempSync(join(tmpdir(),'hitme-ledger-upgrade-')),path=join(dir,'test.sqlite');let store=new Store(path);
  try{const a=store.guest('Ngọc'),b=store.guest('Bảo');store.reward('legacy',[a.id,b.id],a.id,false,true);
    store.db.exec('ALTER TABLE rewards DROP COLUMN policy_version');store.close();store=new Store(path);
    const p=store.profile(a.id);assert.equal(p.coins,100);assert.equal(p.history[0].match,'legacy');assert.equal(p.history[0].policy_version,'trial-v1');
    store.reward('legacy',[a.id,b.id],a.id,false,true);assert.equal(store.profile(a.id).coins,100);
  }finally{store.close();for(const file of [path,path+'-wal',path+'-shm'])if(existsSync(file))unlinkSync(file);rmdirSync(dir);}
});
