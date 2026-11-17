# 06: Play parts and builders — floats, bats, currents, balloons, themes and props

**What to build** (spec sections 4 and 8):

1. **New level parts**, each with `Begin(LevelController)` and `IsBuilt`, reading `LevelClock.Now`. Follow `SandWatcher`, `IceWatcher`, `MovingPlatformView` and the crumbling-ledge view.
   - **`FloatWatcher`:**
     - Each float is a `DynamicSolid` following `SinkingFloat.TopAt`.
     - It shakes and shows `float_ripple_00/01` while it waits and sinks.
     - It stops being solid once `Holds` is false.
     - Play `sfx_splash` when it goes under.
     - A fall into the water is the normal pit hazard.
   - **`BatFlock`:**
     - Draws each bat (`bat_00/01/02`) at `Bat.PositionAt`.
     - On a touch, it asks the motor for the push and plays `sfx_bat_flap`.
     - The explorer flashes, but no checkpoint return.
   - **`CurrentWatcher`:**
     - Tells the motor the `CurrentZone` push.
     - Draws `current_mark_00/01` moving with the flow (flipped for left).
   - **Balloons:**
     - Vertical `MovingPlatform`s drawn with `balloon` and a `balloon_flame_00/01` flicker.
     - Play `sfx_burner` when one starts to rise.
     - Only the basket top is solid.
2. `Play/ExplorerMotor.cs`:
   - Add the current push on the ground only, like the wind push.
   - Add a one-off bat push-back that keeps the checkpoint.
   - Air speed, jumps, sand and ice must not change.
3. **Builders** (`LevelBuilder.cs`, `HardLevelObjects.cs`, `LevelThemes.cs`):
   - The five theme entries from the spec section 8 theme table.
   - The living props from spec section 8, every one through `HillsPropFits`, which also refuses water and the first 16 px of a current strip. Report any refused prop.
   - `water_fill` in pits that hold a float, and in the shallow safe pool under N1's practice float. The pool has a solid floor 16 px down and is not a hazard.
   - `current_top` swapped onto the ground over current strips, like `soft_top`.
   - The level music component takes its ambience from `LandformAudio.AmbienceFor(level)`.
   - Missing-asset errors name the right new script.
4. Run **LandGrand > Build Levels** in batch mode. The log must show no "Missing" and no refused props.

**Checklist**
- [ ] `HillsNormal1`–`HillsNormal5` scenes are built and in Build Settings
- [ ] N2 links `amb_cave_drips`; the others link `amb_stream_normal`; Mountains and Hills Easy are unchanged
- [ ] No per-level or per-theme `if` in play scripts
- [ ] EditMode tests pass (report the count)

**Blocked by:** 01, 02, 03, 04, 05

**Status:** not started

**Builder to run:** **LandGrand > Build Levels**, **LandGrand > Build Lesson Scene**, **LandGrand > Build Selection Scenes**, **LandGrand > Build Profile Scene**
