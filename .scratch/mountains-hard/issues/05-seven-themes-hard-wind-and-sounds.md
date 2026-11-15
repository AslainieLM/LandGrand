# 05: Seven new themes, Hard wind and the new sounds

**What to build:**

1. **Seven theme entries** in `Assets/Editor/LevelThemes.cs`, one per new `LevelTheme`, from the spec's theme table: ground top, soil, ledge, boulder ending, goal name, goal sprite, goal offset, props, `Backdrop = "Backdrop/<Theme>/"` and `BackdropScript = "Tools/Art/make_hard_backdrops.py"`.
2. **Living props** per theme, decoration only, in the existing `…Props` style (for example `SnowProps`). Never place a prop on a hazard, a pickup or a checkpoint:
   - LavaSlope: `burnt_tree` here and there.
   - StormRidge: `bent_tree`; rain streaks (`rain_00/01`, alternating every 0.15 s) falling in front of the explorer at sorting order 22, see-through, following the camera.
   - EagleForest: `tall_tree` and `orchid`; the Easy `eagle` (its flying frames) gliding across now and then.
   - BonsaiForest: `bonsai_tree` and `pitcher_plant`.
   - GlacierHorn: `edelweiss`; the Normal Snow Peak snow flakes.
   - EquatorSummit: `groundsel`.
   - SeaPeak: `silversword`.
3. **Hard ambience.** In `PixelSceneKit`'s ambience table, Hard plays `amb_wind_hard.ogg` from `make_hard_audio.py`, replacing the Easy breeze stand-in (remove the TODO).
4. **Sounds** used by tickets 04 and 07 are already made: `sfx_boulder_rumble`, `sfx_boulder_roll`, `sfx_ice_slide`, `sfx_fanfare` and `sfx_unlock`.

**Blocked by:** 03, 04

**Status:** done

- [x] Every Hard level builds with its own ground, boulders, goal, props and six-layer background (no "no background" error)
- [x] Easy and Normal themes unchanged
- [x] Hard levels play the new Hard wind; Easy and Normal keep theirs
- [x] Props do not sit on hazards, pickups or flags (check one screenshot per level, or ask the user)
- [x] Basic-English comments

**Verified:** Unity EditMode 216 passed. All 15 levels built; each Hard scene has six theme layers and Hard wind. Seven 480×270 renders inspected, and all 174 ground props checked for clearance. Audio clips were checked in the scenes; listening remains on the play checklist.

**Builder to run:** `python Tools/Art/make_hard_art.py`, `python Tools/Art/make_hard_backdrops.py`, `python Tools/Audio/make_hard_audio.py`, then **LandGrand > Build Mountains Levels**.

Spec: `../spec.md` ("Assets", theme table).
