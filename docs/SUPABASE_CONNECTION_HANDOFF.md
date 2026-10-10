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

## Selected Supabase project

User supplied dashboard: https://supabase.com/dashboard/project/ucvnnovebjawcmavgxyg
Project reference: `ucvnnovebjawcmavgxyg`.
Verified API URL: `https://ucvnnovebjawcmavgxyg.supabase.co`.
Verified Publishable Key: `sb_publishable_ZzZXMcXNDm3GHeVyBG83WA_5UPMETZ_` (HTTP 200 at `/auth/v1/settings`).

## Migration & Security Status: COMPLETED & VERIFIED (2026-10-10)

1. **Migration 002 (Schema & Triggers)**:
   - Các bảng trong schema `public`: `profiles`, `inventory`, `matches`, `reward_transactions`, `quests`, `quest_events` đã được tạo thành công trên database.
   - Trigger `on_auth_user_created` tự động tạo profile và cấp vũ khí khởi đầu (`dep-to-ong`, `chao`, `vot`) khi người chơi đăng ký mới.
2. **Anonymous Sign-ins**:
   - Đã được kích hoạt trên Supabase Auth (`anonymous_users: true`).
   - Đã kiểm tra luồng tạo Anonymous Guest Player qua script [test-anon-auth.ts](file:///d:/HIT%20ME/multiplayer-server/scripts/test-anon-auth.ts) -> Thành công tạo tài khoản và tự động sinh profile/inventory.
3. **Migration 003 (Bảo mật RLS & Chống hack số dư)**:
   - Đã áp dụng phân quyền cột: Người chơi chỉ được cập nhật `display_name`, `avatar`, `equipped_weapon`.
   - Đã kiểm tra qua [test-rls-security.ts](file:///d:/HIT%20ME/multiplayer-server/scripts/test-rls-security.ts): Thử nghiệm client hack xu (`coins: 99999`) bị PostgreSQL chặn thẳng với mã lỗi HTTP 403 (`permission denied for table profiles`).

