// Test anonymous signup and automatic profile/inventory trigger
const url = 'https://ucvnnovebjawcmavgxyg.supabase.co';
const key = 'sb_publishable_ZzZXMcXNDm3GHeVyBG83WA_5UPMETZ_';

async function main() {
  console.log('[1] Attempting Anonymous Signup...');
  const res = await fetch(`${url}/auth/v1/signup`, {
    method: 'POST',
    headers: {
      apikey: key,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({})
  });

  const data = await res.json();
  console.log('Signup HTTP status:', res.status);
  if (!res.ok) {
    console.error('Signup failed:', data);
    return;
  }

  const userId = data.user?.id;
  const token = data.access_token;
  console.log('[PASS] Created anonymous user:', userId);

  // Check profile created by trigger
  console.log('[2] Checking public.profiles for created user...');
  const profileRes = await fetch(`${url}/rest/v1/profiles?user_id=eq.${userId}`, {
    headers: {
      apikey: key,
      Authorization: `Bearer ${token}`
    }
  });
  console.log('Profile query status:', profileRes.status);
  const profiles = await profileRes.json();
  console.log('Profile data:', profiles);

  // Check inventory starter items created by trigger
  console.log('[3] Checking public.inventory for starter items...');
  const invRes = await fetch(`${url}/rest/v1/inventory?user_id=eq.${userId}`, {
    headers: {
      apikey: key,
      Authorization: `Bearer ${token}`
    }
  });
  console.log('Inventory query status:', invRes.status);
  const items = await invRes.json();
  console.log('Inventory items:', items);
}

main().catch(console.error);
