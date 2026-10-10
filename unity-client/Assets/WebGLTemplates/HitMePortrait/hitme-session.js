// Public configuration only. Never put service-role/secret keys in a Web build.
window.hitMeSession = (() => {
  const url='https://ucvnnovebjawcmavgxyg.supabase.co';
  const key='sb_publishable_ZzZXMcXNDm3GHeVyBG83WA_5UPMETZ_';
  const storageKey='hitme.supabase.session.v1';
  let session, pending;
  async function request(path,options={}) {
    const controller=new AbortController(),timer=setTimeout(()=>controller.abort(),15000);
    let response;try{response=await fetch(url+path,{...options,headers:{apikey:key,'Content-Type':'application/json',...options.headers},signal:controller.signal});}finally{clearTimeout(timer);}
    if(!response.ok){const error=new Error(response.status===401||response.status===400?'invalid_session':'networkConnectionFailed');error.status=response.status;throw error;}
    return response.status===204?null:response.json();
  }
  function store(s){session=s;localStorage.setItem(storageKey,JSON.stringify(s));return s;}
  async function restore(){
    let saved;try{saved=JSON.parse(localStorage.getItem(storageKey)||'null');}catch{saved=null;}
    if(saved?.refresh_token){
      try{return store(await request('/auth/v1/token?grant_type=refresh_token',{method:'POST',body:JSON.stringify({refresh_token:saved.refresh_token})}));}
      catch(error){if(error.status!==400&&error.status!==401)throw error;localStorage.removeItem(storageKey);}
    }
    return store(await request('/auth/v1/signup',{method:'POST',body:JSON.stringify({data:{display_name:'Khách'}})}));
  }
  async function prepare(){
    if(session&&session.expires_at*1000>Date.now()+60000)return session;
    if(!pending)pending=restore().finally(()=>{pending=null;});return pending;
  }
  async function profile(){const s=await prepare();const rows=await request('/rest/v1/profiles?user_id=eq.'+encodeURIComponent(s.user.id)+'&select=display_name,avatar',{headers:{Authorization:'Bearer '+s.access_token}});if(!rows?.[0])throw Error('profile_unavailable');return rows[0];}
  async function saveProfile(name,avatar){
    const s=await prepare();
    if(!name.trim()||Array.from(name.trim()).length>24||/[<>\p{Cc}\p{Cf}]/u.test(name))throw Error('invalid_name');
    if(!['Char01_Player','Char02_BotMale','Char03_BotFemale'].includes(avatar))throw Error('invalid_avatar');
    await request('/rest/v1/profiles?user_id=eq.'+encodeURIComponent(s.user.id),{method:'PATCH',headers:{Authorization:'Bearer '+s.access_token,Prefer:'return=representation'},body:JSON.stringify({display_name:name.trim().normalize('NFC'),avatar})});
    localStorage.setItem('hitme.onboarded.'+s.user.id,'1');
  }
  return {prepare,profile,saveProfile,needsOnboarding:()=>!localStorage.getItem('hitme.onboarded.'+session?.user.id)};
})();
