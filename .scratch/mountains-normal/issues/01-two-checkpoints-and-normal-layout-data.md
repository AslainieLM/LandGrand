# 01: Two checkpoints and Normal layout data

**What to build:** The rules side for Normal levels, with no new behaviour yet.

1. **Checkpoints become a list.**
   - `LevelData` gets `IReadOnlyList<Spot> Checkpoints`. The constructor's `Spot checkpoint` parameter becomes `Spot[] checkpoints`.
   - Easy 1–3 pass one checkpoint each, at the same positions as now.
   - `LevelRun` remembers the **furthest checkpoint reached**. `RespawnPoint` is that checkpoint, or the start.
   - Keep `CheckpointReached` (true when any was reached) so the guide texts still work.
2. **New themes.** `LevelTheme` gets `PineForest`, `AshGorge`, `MossyForest` and `SnowPeak`.
3. **New layout data on `LevelLayout`**, each optional with an empty default, so Easy is untouched:
   - `CrumbleLedges`: `CrumbleLedge(X, Top)`. Width 32 is a constant on the type.
   - `Rockfalls`: `Rockfall(X, Offset)`.
   - `MovingPlatforms`: `MovingPlatform(X0, X1, Top)`. Width 48 is a constant.
   - `WindZones`: `WindZone(X, Width, Offset)`.
   - Steam vents already exist.
4. **Crumbling bridges in pits.** The ground pieces are split at pits as today. A crumbling ledge with top 0 inside a pit is its own solid, which ticket 04 can switch off.

**Blocked by:** none

**Status:** done

- [x] Easy 1–3 data and behaviour unchanged; all existing tests pass after updating them to the list form
- [x] New tests: the respawn point moves to checkpoint 2 only after checkpoint 2 is reached; it never moves back; a level with two checkpoints reports both
- [x] New tests: the layout lists default to empty; a layout keeps what it was given
- [x] No Unity types in `Assets/Scripts/Rules`
- [x] Every new line has a basic-English comment (D-033)

**Builder to run:** none (rules only). The play scripts that read the old `Checkpoint` must still compile. `HazardWatcher` and `TrailProgressBar` use the first checkpoint for now; ticket 04 shows both.

Spec: `../spec.md` ("New decisions" 1, "Level data").

Verified: Unity EditMode 155 passed, 0 failed (2026-11-15). No builder required.
