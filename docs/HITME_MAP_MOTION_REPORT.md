# Vietnam map motion foundation

Status: **PARTIAL**. Extended existing ArenaArtDefinition rather than adding a competing map-data class. Existing backdrop/sand references remain intact; floorSprite aliases sand. Added IDs, VI/EN names, preview/border/center/shadow references, background/foreground arrays, motion/ambient/audio/lighting/quality metadata and artwork availability state.

MapMotionController reuses EnvironmentLayerView. Existing MotionSettings provides Low/Medium/High and Reduced Motion; per-map profile controls layer count and focus pause. Motion stops while aiming/throwing and disabled scenes do not update. Images do not intercept touch. No background deformation, deterministic math changes or collision edits. Confirmed crowd reaction remains presentation-only.

AmbientVFXController has a fixed bounded sprite pool (max 32), quality limits, reduced-motion suppression, no per-frame allocation/instantiation and no particles if profile/sprites are missing. No dust/leaf/wave/bird art is supplied yet, so ambient production effects are **BLOCKED_BY_ARTWORK**, not completed merely by having a pool. True parallax is disabled pending registered depth layers; parallaxPixels=0. Audio/lighting metadata is not represented as authored ambience/lighting implementation.

Bắc Bộ: real isolated flag, leaf branch, lantern and audience layers. Hội An/Hạ Long: no location layers are available; those maps render explicitly labeled shared-floor fallback, without borrowing village motion and calling it coastal/lantern-town animation.

Map selection affects actual offline Battle art loading. Online Battle deliberately uses default map visuals; local selection is not sent as synchronized room config. Backend map protocol remains MISSING. Existing gameplay, bot AI, multiplayer protocol and Phaser source are preserved.

Performance: existing sprite atlas remains in use for eligible decorations. Quality culling is by layer count; full spatial culling/registered parallax and texture/device tuning remain partial. Existing Web quality supports target 60 / fallback 30; no measured real-device FPS claim.

ParallaxController is a dormant independent-container foundation. Current map motion profile uses zero parallax pixels; no current scene is claimed to have real depth parallax. Future registered far/mid artwork can use the bounded offset API. Configure pipeline preserves manually assigned map floor/preview/layer/profile references, so importing real artwork does not require a competing controller or data system. Select the ArenaArtDefinition asset, assign real sprites/layers/profile in Inspector, and replace shared floor with the location export. Do not activate depth offsets on the flattened composite background.

Validation: 83 EditMode / 30 PlayMode pass. Motion policy test actually verifies high layers, focus freeze, reduced-motion freeze, low layer culling and non-raycasting Images. Scene/map flow test confirms floor use and missing-art labeling in actual Battle render. Empty ambient pool test confirms no invented sprite particles. No real-device performance benchmark or frame/rig animation is claimed.

Final Web Build passes, 0 errors / 4 existing TMP warnings. Backend 16/16 rerun passes. Real browser responsive/VI-EN/preview/selection smoke completed; detailed evidence and limits in HITME_MAP_VISUAL_QA.md. Default online presentation is isolated from local offline selection; no room map support is fabricated.
