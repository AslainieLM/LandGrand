# 03: Themes, market boats, props, build every scene

**What to build** (spec sections 4 and 8):

1. **`ThemeArt`:** two optional fields:
   - `PlatformSprite`: null means `platform`.
   - `PlatformLandSound`: null means no sound.
2. **`LevelObjects` moving-platform build:**
   - Use the theme's sprite, placed so its **top row is the platform top**: the centre y is `Top − spriteHeight / 2`.
   - The old 10 px `platform` therefore stays at `Top − 5`, so Mountains scenes do not move.
   - Give the view the landing sound.
3. **`MovingPlatformView`:** play the landing sound through `PlaySfx` at volume 0.6, once each time the explorer lands on it. No sound when it is empty. No per-theme `if`.
4. **The five theme entries** from the spec section 8 table: backdrops, goals, and FloatingMarket's `market_boat` and `sfx_boat_bump`.
5. **Living props** (spec section 8): placed with the trail prop helpers, which also avoid floats, pools, zones, crumbles and boats. Report any prop moved more than 64 px.
6. **Missing-asset errors** name `make_plains_normal_art.py`, `make_plains_normal_backdrops.py` or `make_plains_normal_audio.py`.
7. **Build:**
   - Run the three new asset scripts. They must reproduce the same files; report any difference.
   - Then run every builder one at a time: `LevelBuilder.BuildAll`, `LessonBuilder.Build`, `SelectionBuilder.Build`, `LandformMapBuilder.Build`, `LandformDescriptionBuilder.Build` and `ProfileBuilder.Build`. Each log must show no "Missing" and no errors.
8. **Regression check:** compare the Mountains scenes that have moving platforms (Mountains Normal and Hard) with the last commit. Platform positions, sprites and timings must be identical (ignore the order the scene file lists objects in). Report the result.

**Checklist**
- [ ] `PlainsNormal1`–`PlainsNormal5` are built and in Build Settings, and link `bgm_plains` and `amb_wetland_normal`
- [ ] No per-level or per-theme `if` in play scripts
- [ ] The Mountains moving platforms are unchanged
- [ ] EditMode tests pass (report the count)

**Blocked by:** 01, 02

**Status:** not started

**Builder to run:** **LandGrand > Build Levels**, **Build Lesson Scene**, **Build Selection Scenes**, **Build Landform Map Scene**, **Build Landform Description Scene**, **Build Profile Scene**
