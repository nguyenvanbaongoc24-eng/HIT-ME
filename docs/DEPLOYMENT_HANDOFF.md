# GitHub / Vercel release handoff — 09/10/2026

Unity source is `unity-client/`; Phaser `apps/` / `packages/` and multiplayer source are preserved. GitHub already contained an initial Unity project at root; it is retained as historical starter files. Open `unity-client/` for the current working game.

`web-release/` contains the actual tested Unity Web Build, copied from `unity-client/Builds/Web/`. `docs/AIM_WEB_BUILD.json`: Succeeded, 0 errors, 4 warnings. Latest checks: EditMode 83/83, PlayMode 32/32. The release includes production aim renderer and prior accumulated artwork/map integration, with limitations documented in the sprint reports.

Vercel: import this GitHub repository, Root Directory = repository root, Framework Preset = Other. `vercel.json` serves `web-release/` directly, with no npm or Unity build on Vercel. Unity decompression fallback is enabled, so `.unityweb` files must be delivered as binary without a manually supplied gzip Content-Encoding. Do not apply SPA rewrites to build-file URLs. An immutable build cache is avoided because Unity uses fixed output filenames.

Regenerate a release after Unity build by copying index.html, Build/, and StreamingAssets/ (when present) into web-release; commit generated release together with source. Never upload Library/, Temp/, SQLite databases or secret environment files.

Requested URL `https://hit-me-eta.vercel.app/` returned HTTP 404 at inspection. The connected Vercel account/team did not expose the matching project/alias. No deployment to a different project is claimed. Connect the account owning the intended project (or confirm the intended new project/team) to complete production deployment.

Supabase requirements and pending runtime work: `SUPABASE_CONNECTION_HANDOFF.md`.
