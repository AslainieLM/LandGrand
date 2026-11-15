# 01: Three checkpoints and Hard layout data

**What to build:** The rules side for Hard levels, with no new behaviour yet.

1. **Three checkpoints.** `LevelData.Checkpoints` is already a list (Normal ticket 01). Check that `LevelRun` keeps the **furthest** of three checkpoints, and that the trail bar and `HazardWatcher` handle any number of flags. Add tests with three checkpoints.
2. **New themes.** `LevelTheme` (in `LevelLayout.cs`) gets `LavaSlope`, `StormRidge`, `EagleForest`, `BonsaiForest`, `GlacierHorn`, `EquatorSummit` and `SeaPeak`, after `SnowPeak`.
3. **New layout data on `LevelLayout`**, each optional with an empty default (named arguments, like `windZones:`), so Easy and Normal are untouched:
   - `RollingBoulders`: `RollingBoulder(float spawn, float end, float offset = 0f)`.
   - `IceZones`: `IceZone(float x, float width)`.
   Each is a sealed class in its own file in `Assets/Scripts/Rules`, like `Rockfall` and `WindZone`. Ticket 02 adds their behaviour.

**Blocked by:** none

**Status:** done

- [x] Easy 1–3 and Normal 1–5 data and behaviour unchanged; all 198 existing tests pass
- [x] New tests: with three checkpoints, the respawn point moves forward only, to the furthest flag reached
- [x] New tests: the two new layout lists default to empty; a layout keeps what it was given
- [x] No Unity types in `Assets/Scripts/Rules`
- [x] Every new line has a basic-English comment (D-033)

**Verified:** Unity EditMode 200 passed, including all 198 existing cases. The world and trail flag loops accept all three flags.

**Builder to run:** none (rules only).

Spec: `../spec.md` ("New decisions" 1–3, "Level data").
