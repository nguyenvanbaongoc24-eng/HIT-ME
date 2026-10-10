const url = 'https://ucvnnovebjawcmavgxyg.supabase.co';
const key = 'sb_publishable_ZzZXMcXNDm3GHeVyBG83WA_5UPMETZ_';

async function run() {
  console.log('[1] Creating anonymous test player...');
  const signup = await (await fetch(`${url}/auth/v1/signup`, {
    method: 'POST',
    headers: { apikey: key, 'Content-Type': 'application/json' },
    body: JSON.stringify({})
  })).json();
  const token = signup.access_token;
  const userId = signup.user.id;

  // 1. Legal update: display_name and avatar
  console.log('[2] Testing legal update (display_name & avatar)...');
  const updateRes = await fetch(`${url}/rest/v1/profiles?user_id=eq.${userId}`, {
    method: 'PATCH',
    headers: { apikey: key, Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
    body: JSON.stringify({ display_name: 'HitMeMaster', avatar: 'Char02_BotMale' })
  });
  console.log('Legal update HTTP status:', updateRes.status);

  // 2. Illegal update: attempt to alter coins or xp directly from client
  console.log('[3] Testing illegal client balance manipulation (coins=99999)...');
  const illegalRes = await fetch(`${url}/rest/v1/profiles?user_id=eq.${userId}`, {
    method: 'PATCH',
    headers: { apikey: key, Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
    body: JSON.stringify({ coins: 99999 })
  });
  console.log('Illegal update HTTP status:', illegalRes.status);
  const illegalBody = await illegalRes.text();
  console.log('Illegal update response:', illegalBody);

  // 3. Verify actual profile in database
  console.log('[4] Verifying profile state...');
  const check = await (await fetch(`${url}/rest/v1/profiles?user_id=eq.${userId}`, {
    headers: { apikey: key, Authorization: `Bearer ${token}` }
  })).json();
  console.log('Profile verified:', check);
}

run().catch(console.error);
