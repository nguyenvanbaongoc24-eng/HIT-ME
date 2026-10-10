import {characterIds,validateNickname} from './store.js';
export interface AuthProfile {id:string;name:string;avatar:string}
// Only a public key is needed: /auth/v1/user verifies the bearer and RLS scopes the profile.
export function supabaseVerifier(url:string,key:string){
  if(!url.startsWith('https://')||!key)throw Error('auth_configuration_missing');
  return async(token:unknown):Promise<AuthProfile>=>{
    if(typeof token!=='string'||token.length<20||token.length>6000)throw Error('invalid_session');
    const headers={apikey:key,Authorization:`Bearer ${token}`};
    const userResponse=await fetch(`${url}/auth/v1/user`,{headers,signal:AbortSignal.timeout(8000)});
    if(!userResponse.ok)throw Error('invalid_session');
    const user=await userResponse.json() as {id?:string};
    if(!user.id||!/^[0-9a-f-]{36}$/i.test(user.id))throw Error('invalid_session');
    const response=await fetch(`${url}/rest/v1/profiles?user_id=eq.${user.id}&select=display_name,avatar`,{headers,signal:AbortSignal.timeout(8000)});
    if(!response.ok)throw Error('profile_unavailable');
    const rows=await response.json() as {display_name:string;avatar:string}[];
    const p=rows[0];if(!p||!(characterIds as readonly string[]).includes(p.avatar))throw Error('profile_unavailable');
    return {id:user.id,name:validateNickname(p.display_name),avatar:p.avatar};
  };
}
