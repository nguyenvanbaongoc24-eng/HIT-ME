import {test} from 'node:test';
import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import vm from 'node:vm';
const source=readFileSync('../unity-client/Assets/WebGLTemplates/HitMePortrait/hitme-session.js','utf8');
function fixture(initial?:any){
 const storage=new Map<string,string>();if(initial)storage.set('hitme.supabase.session.v1',JSON.stringify(initial));
 const requests:{url:string;method:string;body:any}[]=[];let failRefresh=false,failTransport=false;
 const localStorage={getItem:(key:string)=>storage.get(key)??null,setItem:(key:string,value:string)=>storage.set(key,value),removeItem:(key:string)=>storage.delete(key)};
 const context:any={window:{},localStorage,setTimeout,clearTimeout,AbortController,fetch:async(url:string,options:any)=>{
  requests.push({url,method:options.method??'GET',body:options.body?JSON.parse(options.body):null});
  if(failTransport)throw Error('offline');
  if(failRefresh&&url.includes('refresh_token'))return {ok:false,status:400};
  return {ok:true,status:200,json:async()=>url.includes('/profiles')?[{display_name:'Ngọc',avatar:'Char03_BotFemale'}]:{user:{id:'auth-user'},access_token:'test-access',refresh_token:'test-refresh',expires_at:Date.now()/1000+3600}};
 }};vm.createContext(context);vm.runInContext(source,context);return {api:context.window.hitMeSession,storage,requests,set failRefresh(value:boolean){failRefresh=value;},set failTransport(value:boolean){failTransport=value;}};
}
test('browser session uses real auth protocol, restores identity and writes only cosmetics',async()=>{
 const f=fixture({refresh_token:'existing'});await f.api.prepare();assert.equal(f.requests[0].body.refresh_token,'existing');assert.equal(f.api.needsOnboarding(),true);
 const p=await f.api.profile();assert.equal(p.avatar,'Char03_BotFemale');await f.api.saveProfile(' Nguyễn Ngọc ','Char03_BotFemale');
 const patch=f.requests.find(r=>r.method==='PATCH')!;assert.deepEqual(patch.body,{display_name:'Nguyễn Ngọc',avatar:'Char03_BotFemale'});assert.equal(f.api.needsOnboarding(),false);
 const count=f.requests.length;await f.api.prepare();assert.equal(f.requests.length,count);
 await assert.rejects(()=>f.api.saveProfile('<b>Bad</b>','Char03_BotFemale'),/invalid_name/);
});
test('expired refresh creates guest but transport failures do not silently replace identity',async()=>{
 const f=fixture({refresh_token:'expired'});f.failRefresh=true;await f.api.prepare();assert.ok(f.requests.some(r=>r.url.endsWith('/signup')));
 const unavailable=fixture({refresh_token:'keep-me'});unavailable.failTransport=true;await assert.rejects(()=>unavailable.api.prepare(),/offline/);assert.ok(unavailable.storage.has('hitme.supabase.session.v1'));assert.equal(unavailable.requests.some(r=>r.url.endsWith('/signup')),false);
});
