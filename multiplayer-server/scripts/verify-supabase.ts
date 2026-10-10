// Script to verify Supabase connection, Auth configuration, and Schema tables
const supabaseUrl = process.env.NEXT_PUBLIC_SUPABASE_URL || 'https://ucvnnovebjawcmavgxyg.supabase.co';
const apiKey = process.env.NEXT_PUBLIC_SUPABASE_PUBLISHABLE_KEY || 'sb_publishable_ZzZXMcXNDm3GHeVyBG83WA_5UPMETZ_';

async function check() {
  console.log(`[1] Probing Supabase URL: ${supabaseUrl}`);

  // 1. Auth Settings check
  try {
    const authRes = await fetch(`${supabaseUrl}/auth/v1/settings`, {
      headers: { apikey: apiKey }
    });
    if (authRes.ok) {
      const authSettings = await authRes.json();
      const anonEnabled = authSettings.external?.anonymous_users ?? false;
      console.log(`[PASS] Supabase API key is VALID.`);
      console.log(`[INFO] Anonymous Login enabled: ${anonEnabled ? 'YES' : 'NO (Action needed: enable in Dashboard)'}`);
    } else {
      console.log(`[FAIL] Auth settings failed with HTTP ${authRes.status}: ${await authRes.text()}`);
    }
  } catch (err) {
    console.log(`[ERROR] Failed to fetch auth settings:`, err);
  }

  // 2. Public Tables Schema check
  const tables = ['profiles', 'inventory', 'matches', 'reward_transactions', 'quests', 'quest_events'];
  console.log(`\n[2] Checking public schema tables:`);
  for (const table of tables) {
    try {
      const res = await fetch(`${supabaseUrl}/rest/v1/${table}?limit=1`, {
        headers: {
          apikey: apiKey,
          Authorization: `Bearer ${apiKey}`
        }
      });
      if (res.status === 200) {
        console.log(`  - public.${table}: [EXISTS]`);
      } else if (res.status === 404) {
        console.log(`  - public.${table}: [NOT_FOUND] (Migration 002 not yet applied)`);
      } else {
        const body = await res.json().catch(() => ({}));
        console.log(`  - public.${table}: [HTTP ${res.status}] ${body.message || ''}`);
      }
    } catch (err) {
      console.log(`  - public.${table}: [ERROR]`, err);
    }
  }
}

check();
