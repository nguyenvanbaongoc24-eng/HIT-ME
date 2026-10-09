# Supabase connection handoff

Current runtime: Node 24 WebSocket authoritative server, SQLite repository. Supabase migration exists at `multiplayer-server/migrations/001_sprint4.sql`, but has NOT been applied. Supabase Auth/PostgreSQL runtime adapter is NOT implemented. Adding keys alone will not activate it.

Needed information:

1. Supabase Project URL and project reference; publishable key (or legacy anon key) can be used by the client once Auth integration is implemented.
2. Desired first Auth mode: email, Google, or anonymous/guest. Configure providers in Supabase; production redirect URLs must match the actual deployed site.
3. Whether the Supabase database is new/empty or already contains user data, and whether the isolated `hitme_s4` migration has been applied. Review schema and RLS before applying; do not map current guest UUIDs directly into Auth foreign keys.
4. Configure server-only secret/service-role key and PostgreSQL connection string/password in backend secret environment, never in Unity resources, public Web files, Git or chat.
5. Public trusted WSS endpoint for the current real-time match server, plus allowed HTTPS site origin. Supabase does not automatically replace the existing WebSocket server/protocol. No hosting relocation or protocol rewrite is included in this deployment.

Next engineering work after configuration: server JWT verification, account linking, PostgreSQL transactional Store adapter, existing reward/idempotency/security tests, reviewed migration, then client Auth flow. Existing gameplay and trial reward rules remain unchanged.

This release is playable offline on static hosting. Online production and Supabase persistence remain pending those integrations.
