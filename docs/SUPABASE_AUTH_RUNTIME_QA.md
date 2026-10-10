# Supabase Auth Runtime QA

2026-10-10. Supabase API fixture checks **PASS**; Unity product Auth integration **FAIL**.

Verified project: `https://ucvnnovebjawcmavgxyg.supabase.co`. Publishable configuration read from existing project; no secret key used. Evidence: `STRICT_PUBLIC_SERVICE_PROBE.json`; reproducible command `cd multiplayer-server; npx tsx scripts/strict-production-probe.ts`.

Two distinct anonymous sessions each returned a valid session and independently verified identity. Trigger-created profile count=1. Both saved the same Vietnamese Unicode nickname and different canonical avatar IDs; profile survived token refresh with unchanged identity and zero coins. Cross-user read returned no rows; cross-user PATCH changed no rows. Direct coin PATCH denied. Invalid token rejected HTTP403. Logout returned204. Probe output omits tokens, keys and account IDs. Only dedicated QA profiles were touched; no existing user rows were changed or deleted. Initial column-name error in the probe was repaired from `id` to actual `user_id`.

These are independent HTTP bearer sessions, not two independently isolated browser UI contexts. Browser refresh/session persistence, expired-token expiry, interruption recovery, re-login after logout, email login and OAuth callback: **NOT_TESTED**.

Unity `NetworkSession` sends HIT ME `hello {name,token}`. Backend `main.ts` calls Store.guest and maintains SQLite identity. No Supabase access-token validation/binding exists in this active flow. Thus public Unity login, WSS identity association and Supabase-backed match/currency persistence are **FAIL** against the requested production requirement. API provider success cannot substitute for this integration.

No migration applied in this audit; existing table/RLS behavior was exercised through actual user-scoped requests. Existing reports claiming ENFORCED should be read within those tested operations, not as proof every policy/schema is secure. No service-role key observed in active Unity source; complete distributed-binary secret extraction NOT_TESTED.
