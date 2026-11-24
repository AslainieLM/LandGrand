# 03: Theme fields, seven themes, props; build every level scene

**What to build** (spec sections 4, 5 and 8):

1. **`ThemeArt`** new optional fields (null keeps today's art, so old levels do not change): `VentRim`, `VentSteamFrames`, `VentWarnFrames`, `GustFrames`, `GustSound`, `SandTop`, each with a script field for the missing-asset message (like `IceTopScript`). Extend the "old themes keep defaults" test to them.
2. **Builders:** the vent build (`LevelBuilder.cs`), the gust build (`LevelObjects.cs`) and the soft-sand strip (`HardLevelObjects.cs`) read those fields. No per-theme `if`.
3. **The seven theme entries** from the spec section 8 table (tiles, goals, backdrops, vent, gust, sand, ice, herd and platform art and sounds).
4. **Living props** (spec section 8) with the shared trail prop helper. Report any prop moved more than 64 px and any y you changed.
5. **Assets:** run `make_plateaus_hard_art.py`, `make_plateaus_hard_backdrops.py` and `make_plateaus_hard_audio.py` twice each; outputs must be identical and match the files on disk. Report any difference.
6. **Build:** run `LevelBuilder.BuildAll`, `LessonBuilder.Build`, `SelectionBuilder.Build` one at a time; each log has no "Missing" and no errors.
7. **Regression check:** compare against `HEAD` the Mountains Easy 2 and Mossy Forest scenes (vents), the Mountains Normal gust scenes, the Hills Easy 3 and Plateaus Normal 2 scenes (sand), Plains Hard (herds, frost) and Plateaus Normal (llamas, salt crust, cable cars). Positions, sprites and timings must be identical (ignore the order the scene file lists objects in). Report the result.

**Checklist**
- [ ] `PlateausHard1`–`PlateausHard7` are built and in Build Settings, with `bgm_plateaus` and `amb_highwind_hard`
- [ ] Old vents, gusts, sand, herds, ice and platforms unchanged
- [ ] No per-level or per-theme `if` in play scripts
- [ ] EditMode tests pass (report the count)

**Blocked by:** 01, 02

**Status:** open

**Builder to run:** **LandGrand > Build Levels**, **Build Lesson Scene**, **Build Selection Scenes**
