# 01: Rules for the sinking float, gentle bat, river current, vertical balloon and per-level ambience

**What to build** (spec sections 2 and 4; numbers from the `layouts.md` "Rule constants" table):

1. `Rules/SinkingFloat.cs` (new):
   - **Placement:** x, top, and the pit it covers.
   - **Sink rule:** the wait before sinking, the sink speed, the depth at which it stops holding, and the rise delay.
   - **Methods:** `TopAt(...)` and `Holds(...)`.
2. `Rules/Bat.cs` (new):
   - **Loop:** centre, radius x/y, period and phase; `PositionAt(seconds)`.
   - **Touch:** a test against the explorer's 12×26 body.
   - **Push:** the push distance and the rest after a push.
3. `Rules/CurrentZone.cs` (new):
   - x, width and direction (+1 right, −1 left).
   - A steady push only while standing in the strip. Use `ZoneFooting` for that check.
   - Do not change `WindZone`.
4. `Rules/MovingPlatform.cs`:
   - Add a vertical option: x is fixed and the top moves between a low and a high top at the same `Speed`.
   - Horizontal platforms must give exactly the same `LeftAt` values as now.
5. `Rules/LevelLayout.cs`:
   - Optional `Floats`, `Bats`, `Currents` and vertical platforms.
   - Optional `AmbienceOverride` (null = use the table).
   - Existing callers must compile unchanged.
6. `Rules/LandformAudio.cs`:
   - `AmbienceFor(LevelData level)` returns the override when set, or else the table entry.
   - The script-name table maps `amb_cave_drips` to `make_hills_normal_audio.py`.
7. **Tests:**
   - Each rule at its edges.
   - Mountains horizontal platforms unchanged.
   - The ambience override and the fallback to the table.

**Checklist**
- [x] Rules have no Unity types; every constant matches `layouts.md`
- [x] Mountains moving platforms give the same positions as before (test)
- [x] EditMode tests pass (report the count; 296 before)

**Blocked by:** none

**Status:** done

**Verified:** Unity 6000.6.8f1 EditMode, 334 passed, 0 failed, 0 skipped.

**Builder to run:** none
