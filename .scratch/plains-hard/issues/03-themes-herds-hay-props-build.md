# 03: Themes, hay, herds, frost, props, build every scene

**What to build** (spec sections 4 and 8):

1. **`ThemeArt`** optional fields (null or 0 keeps the current art, so old levels do not change):
   - `RollerFrames` (null = `log`), `RollerWarnFrames` (null = `log_warn`), `RollerSound` (null = `sfx_log_roll`)
   - `BatFrames` (null = `bat`), `BatFrameCount` (0 = 3), `BatSound` (null = `sfx_bat_flap`), `BatArtLift` (0)
   - `IceTop` (null = `ice_top`)
   - the script names for the missing-asset messages, set in data (like `PlatformLandSoundScript`)
2. **Builders:**
   - The roller build (`HardLevelObjects`) uses the theme's roller frames and sound.
   - The ice strip uses `IceTop`.
   - The bat build (`HillsLevelObjects`) uses the theme's bat frames, frame count and sound, and draws each picture `BatArtLift` px above the rule's position.
3. **`BatFlock`:** keep the lift when it moves the pictures. The touch box and the `Bat` rule do not change. No per-theme `if`.
4. **The seven theme entries** from the spec section 8 table: backdrops, goals, roller, bat and ice art.
5. **Living props** (spec section 8): placed with the shared trail prop helper, which also avoids floats, pools, zones, crumbles and straw piles. Report any prop moved more than 64 px.
6. **Missing-asset errors** name `make_plains_hard_art.py`, `make_plains_hard_backdrops.py` or `make_plains_hard_audio.py`, taken from theme data.
7. **Build:**
   - Run the three new asset scripts. They must reproduce the same files; report any difference.
   - Then run every builder one at a time: `LevelBuilder.BuildAll`, `LessonBuilder.Build`, `SelectionBuilder.Build`, `LandformMapBuilder.Build`, `LandformDescriptionBuilder.Build` and `ProfileBuilder.Build`. Each log must show no "Missing" and no errors.
8. **Regression check:** compare the Hills Hard scenes (logs), the Hills Normal scenes (bats) and the Mountains Hard scenes (ice) with the last commit. Log, bat and ice positions, sprites and timings must be identical (ignore the order the scene file lists objects in). Report the result.

**Checklist**
- [x] `PlainsHard1`–`PlainsHard7` are built and in Build Settings, and link `bgm_plains` and `amb_dry_hard` (H4: `amb_cold_plain`)
- [x] No per-level or per-theme `if` in play scripts
- [x] Old logs, bats and ice are unchanged
- [x] EditMode tests pass (report the count)

**Blocked by:** 01, 02

**Status:** done (2026-11-23, 475 tests)

**Builder to run:** **LandGrand > Build Levels**, **Build Lesson Scene**, **Build Selection Scenes**, **Build Landform Map Scene**, **Build Landform Description Scene**, **Build Profile Scene**
