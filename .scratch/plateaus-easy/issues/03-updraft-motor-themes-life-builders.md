# 03: Updraft in play, themes, props, map and menu life; build every scene

**What to build** (spec sections 4, 5 and 8). Build in this order, testing as you go:

1. `Play/ExplorerMotor.cs`: `UpdraftTop` (NaN by default) and the updraft step from spec section 5: jump presses do nothing inside, gravity is replaced by the rise velocity, the double jump is ready on leaving, walking works as normal. When `UpdraftTop` is NaN the motor must behave exactly as before.
2. New `Play/UpdraftWatcher.cs` level part (`Begin(LevelController)`, `IsBuilt`), like `StrawBounceWatcher`: sets `UpdraftTop`, loops `sfx_updraft.wav` at 0.5 while inside, stops it on leaving and on pause.
3. The updraft view: stacked `updraft_00`–`_03` frames up to `top + 26`, `updraft_base`, rising leaves and dust, 8 frames per second on `LevelClock.Now`.
4. The level object builder: build each updraft from the layout and register it with the watcher; missing-art errors name `Tools/Art/make_plateaus_easy_art.py`.
5. `Editor/LevelThemes.cs`: the three theme entries (spec section 8 theme table) and the living props.
6. `Editor/LandformLife.cs` and `LandformMapBuilder.cs`: `PlateausLife()` (waterfall, grass, two hawks), active only while Plateaus is open; report the spots chosen.
7. `Editor/SelectionBuilder.cs`: `PlateausMenuLife()` (cloud, two hawks, six grass tufts).
8. Run the asset scripts (each twice; outputs identical): `make_plateaus_easy_art.py`, `make_plateaus_easy_backdrops.py`, `make_profile_art.py`, `make_plateaus_audio.py`.
9. Run the builders one at a time: `LevelBuilder.BuildAll`, `LessonBuilder.Build`, `SelectionBuilder.Build`, `LandformMapBuilder.Build`, `LandformDescriptionBuilder.Build`, `ProfileBuilder.Build`. Logs must show no "Missing" and no errors.
10. Regression: compare the Plains Easy 3, Hills Hard 1 and Mountains Hard 1 scenes against `HEAD` (positions, sprites, timings; ignore object order). Only expected differences are allowed.

**Checklist**
- [ ] No per-level, per-landform or per-theme `if` in play scripts (D-061)
- [ ] Props kept off pits, ledges, boulders and updraft columns; props moved more than 64 px listed
- [ ] Goal offsets checked; any change reported
- [ ] EditMode tests pass (report the count)

**Blocked by:** 01, 02

**Status:** done (2026-11-24, 524)

**Builder to run:** the session runs the six builders in batch mode; the user re-runs **LandGrand > Build Levels** only if the Editor shows stale scenes
