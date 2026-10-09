# RELEASE BASELINE — 2026-10-09

Status: PASS for local baseline; BLOCKED for public release prerequisites.

- Existing project: `unity-client`, Unity 6000.6.4f1; Web Build Support installed. No new project.
- Preserved known-good commit: `22dae6da3e26474f1df5ceea89537b86234213c8`; branch `codex/release-baseline-20261009`.
- Before edits, fresh executions: Unity EditMode 83/83, PlayMode 32/32, backend 16/16. Existing tested Web build remains recoverable from the baseline commit.
- Browser before replacement: localhost 8791, viewport 445×751, canvas x=49.2578125,width=346.484375. Brown side gutters observed; template max-width is the direct cause.
- HTTP server listener verified: 127.0.0.1:8791. Local authoritative backend listener verified: 127.0.0.1:8788. These are local development services.
- Requested production `https://hit-me-eta.vercel.app/` returned HTTP 404. Connected Vercel account does not expose the target project.
- Supabase Auth settings probe for the supplied project/public key returned HTTP 401. Anonymous-auth configuration, existing rows/schema and RLS have NOT been verified. No database migration applied.
- A secret supplied in chat was not used or stored. Rotate it and configure the replacement directly on the backend host.
- Source inventory: `RELEASE_SOURCE_AUDIT.json`, 9 Picture reference images and 20 WAV files. Original source files preserved.
- Existing art uses separate static sprites and code motion; concept sheets are references, not production animation sheets. No complete frame animation pack exists.
- Physical iPhone/Safari/Zalo, public multiplayer and production persistence: NOT_TESTED.
