# HIT ME multiplayer foundation

Requires Node 24 (local `node:sqlite` is experimental). From `D:/HIT ME/multiplayer-server`:

```powershell
npm ci
npm run build
npm test
npm start
```

Local endpoint: `ws://127.0.0.1:8788/play`, health `http://127.0.0.1:8788/health`. SQLite data lives in `data/hitme.sqlite` relative to the server working directory. Back up that file and its WAL before upgrades. No existing database is migrated automatically.

Run `../tools/serve-web.py --port 8791` to serve the Unity build. Open **two separate browser tabs**, choose Lobby, use distinct guest names, connect. One creates a room; the other joins its 8-character code. Both Ready starts a 3-second countdown. Tap position, drag aim, Ready locks each round. Server reveals positions, animates simultaneous throws, resolves HP and awards the verified terminal result. Both tabs need to act before the 5-second deadline. All players ready also starts Quick Match. There is no bot fill.

Guest resume tokens are in browser sessionStorage (separate per tab). Reload retains that tab's guest. Closing a tab loses its credential; production account linking is pending. Never log/share tokens. Origin allowlist is a browser boundary, not authentication; bearer sessions authenticate each player. HTTP/WS development binds loopback only. WSS is required for non-local environments: set NODE_ENV=production, TLS_CERT, TLS_KEY, HOST, ALLOWED_ORIGINS to reviewed deployment values. The server refuses production startup without TLS. Reverse-proxy deployments need a reviewed trusted TLS boundary; do not disable browser certificate validation.

Rewards 100/50, 40/20, 65/30 are trial coins/XP; cap 3 rewarded private matches per UTC day. Match+player primary key makes retries safe. Client reward requests are rejected. All rewards and balances are transactional. A match with insufficient submitted locked actions receives no reward. Inventory starter weapons are cosmetic. Map collectibles use existing six Vietnam IDs. Materials and recipes have types but no invented crafting economy.

Quests use verified round hits and match completion events; events deduplicate by ID. Daily and weekly progress is independent. Claim currently records completion with no unconfirmed quest payout. Old unclaimed periods are returned as Expired in profile.expiredQuests; UI displays current daily/weekly periods. Weekly trial periods are UTC seven-day buckets anchored to the Unix epoch.

Supabase credentials were absent. `migrations/001_sprint4.sql` is an isolated PostgreSQL/Supabase schema with RLS and no client balance writes. **Not applied or runtime-connected.** To enable: create a Supabase project, review migration against backup, configure Auth providers/anonymous login, apply reviewed migration, implement server-side JWT verification and transactional PostgreSQL repository matching Store's tests. Supply service-role key only to server secret environment. Anonymous-to-social linking and Google/Facebook/Apple need provider setup. Do not point current guest UUIDs at Auth foreign keys without a reviewed account-linking migration.

Unconfirmed policies: incomplete-input timeout remains labelled proposed stay/skip. AFK ban threshold, disconnect forfeiture, quest payouts, crafting recipes, ranking and production reward balance remain open. In-progress disconnected fighters keep normal combat HP until the match resolves; no invented HP penalty. Waiting-room disconnected members expire after 30 seconds.

Set PROPOSED_TIMEOUT=false to block incomplete placement rather than adopting the stay/skip proposal. Complete player inputs still resolve normally.
