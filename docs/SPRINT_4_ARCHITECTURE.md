# Sprint 4 architecture

Source of gameplay truth: current Unity Core, Sprint 2 rules and Sprint 3B rounded arena. Phaser `apps/` and `packages/` remain unchanged. New server is isolated in `multiplayer-server/`.

Node 24 + TypeScript + ws: one authoritative room state machine, per-viewer snapshots, immutable locked actions, simultaneous damage, server clock, bounded requests and message sizes. Unity sends intent only. Shared fixtures verify the C# / TS resolver contract. Positions are omitted for opponents during Placement, including reconnect snapshots.

SQLite local repository: transactionally durable guest profiles, session token hashes, inventory, match reward ledger and deduplicated quest events. Guest tokens are bearer credentials and must travel over WSS outside localhost. Production refuses startup without TLS. Origin allowlist is explicit. Supabase credentials were not found; PostgreSQL migration is prepared separately, not applied to an existing database. Social login/account linking are pending credentials and provider configuration.

Trial reward amounts come from Sprint 4 examples, not the older canonical economy. Private daily cap is a development policy. Quest payouts and crafting costs remain undefined; no invented payouts or recipes. Cosmetics never modify combat. Server rewards require actual locked inputs and a terminal match. Client cannot submit results or currency changes.

Timeout keeps existing labelled PROPOSED_STAY_SKIP policy. Disconnect grace is a configurable 30-second proposal; waiting-room sessions expire, but in-progress HP/forfeit/AFK bans are not invented. Matchmaking has no bot fill until room bot policy is confirmed. No sudden death, nudge or ranking changes.

Unity integration extends the existing BattleView and CharacterVisual; networking presentation does not resolve damage locally. Existing six scenes and offline play remain. Local WS is strictly a localhost development option; deployed clients require WSS with a valid certificate.
