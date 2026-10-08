# HIT ME working rules

- Preserve existing `apps/` and `packages/` Phaser/TypeScript source. Unity lives in `unity-client/`.
- Read `docs/HitMe_Spec_for_Antigravity.md`, `docs/HitMe_UI_Redesign_Prompt_1C.md`, and `docs/HitMe_UI_Prompts_for_Codex.md` before changes. Current user instructions take priority; 1C replaces 1B.
- Gameplay math/state must remain testable without UnityEngine. Do not infer unresolved rules in `docs/MISSING_REQUIREMENTS.md`.
- Opponent positions/directions stay hidden during placement. Layout preview must be explicitly identified as a preview.
- Default Vietnamese, i18n keys, Unicode, safe-area mobile portrait. Use clearly labeled placeholders for unavailable art.
- Do not claim test/build/screenshot/FPS success without running the real checks. No production multiplayer, economy, ranking, or shop in foundation.
- Run focused Unity EditMode/PlayMode checks and record material limitations before ending a sprint.
