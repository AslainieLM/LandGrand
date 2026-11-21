# 03: Straw bounce part, themes, props, map and menu life, build every scene

**What to build** (spec sections 5 and 8):

1. `Play/StrawBounceWatcher.cs`: a new level part with `Begin(LevelController)` and `IsBuilt`, like `SandWatcher`.
   - When the explorer is grounded and `IsOnTop` is true for a pile, it does four things, all timed with `LevelClock.Now`:
     - calls `ExplorerMotor.Launch(StrawPile.LaunchSpeed)`
     - plays `PlaySfx(sfx_straw_bounce, 0.7)`
     - shows the squash sprite for 0.15 s
     - shows the straw bits (two frames, 0.1 s each)
   - No per-level or per-theme `if`.
2. **Builders:**
   - **Straw piles:** each pile is a solid like a small boulder, with the box x+1 … x+47 across and 0 … 15 up. It uses the `straw_pile` sprite, sits with its bottom on the ground line, and is registered with the watcher.
   - **Themes:** the three theme entries from the spec section 8 table, with their backgrounds and living props.
   - **Props:** placed through `HillsPropFits` or the same fit test, and they also avoid straw piles. Report any prop moved more than 64 px.
   - **Audio:** music and ambience come from `LandformAudio`; nothing is hard-coded.
   - **Missing-asset errors** name `Tools/Art/make_plains_easy_art.py`, `Tools/Art/make_plains_easy_backdrops.py` or `Tools/Audio/make_plains_audio.py`.
   - **Menu and map life:**
     - `SelectionBuilder`: add `PlainsMenuLife()` (spec section 8).
     - `LandformMapBuilder`: add `PlainsLife()` at the spots in `layouts.md` "Plains map life", shown only while Plains is open.
     - `LandformLife`: load the `Plains/` sprites. Hills sprites must load as before.
3. **Build:**
   - Run the four asset scripts: `make_plains_easy_art.py`, `make_plains_easy_backdrops.py`, `make_profile_art.py` and `make_plains_audio.py`. They must reproduce the same files; report any difference. OGG files may differ in bytes if the encoder is not byte-stable; report that too.
   - Then run every builder in batch mode, one at a time: `LevelBuilder.BuildAll`, `LessonBuilder.Build`, `SelectionBuilder.Build`, `LandformMapBuilder.Build`, `LandformDescriptionBuilder.Build` and `ProfileBuilder.Build`. Each log must show no "Missing" and no errors.

**Checklist**
- [ ] `PlainsEasy1`–`PlainsEasy3` are built and in Build Settings
- [ ] Plains scenes link `bgm_plains` and `amb_grass_easy`; Mountains and Hills scenes link what they did before
- [ ] No per-level or per-theme `if` in play scripts; boulders, vents, sand and ice are unchanged
- [ ] EditMode tests pass (report the count)

**Blocked by:** 01, 02

**Status:** not started

**Builder to run:** **LandGrand > Build Levels**, **Build Lesson Scene**, **Build Selection Scenes**, **Build Landform Map Scene**, **Build Landform Description Scene**, **Build Profile Scene**
