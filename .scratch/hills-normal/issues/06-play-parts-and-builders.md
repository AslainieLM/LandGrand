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
- [x] `HillsNormal1`–`HillsNormal5` scenes are built and in Build Settings
- [x] N2 links `amb_cave_drips`; the others link `amb_stream_normal`; Mountains and Hills Easy are unchanged
- [x] No per-level or per-theme `if` in play scripts
- [x] EditMode tests pass (report the count)

**Blocked by:** 01, 02, 03, 04, 05

**Status:** done

**Verified:** Unity built 23 levels with no missing assets or refused props. All five new scenes are enabled. Compared all level ambience references. EditMode passed 360 of 360, and all five scene smoke checks passed.

**Builder to run:** **LandGrand > Build Levels**, **LandGrand > Build Lesson Scene**, **LandGrand > Build Selection Scenes**, **LandGrand > Build Profile Scene**

## Files

- `Assets/Scripts/Play/FloatWatcher.cs` Starts float views and plays water-entry or submergence splashes.
- `Assets/Scripts/Play/FloatView.cs` Shows and moves each one-way raft.
- `Assets/Scripts/Play/BatFlock.cs` Animates bat loops, handles contacts and flashes the explorer.
- `Assets/Scripts/Play/CurrentWatcher.cs` Draws moving river marks and applies the grounded push.
- `Assets/Scripts/Play/BalloonView.cs` Moves the basket and its burner on the level clock.
- `Assets/Scripts/Play/LevelPropAnimation.cs` Animates the new decoration frames using level time.
- `Assets/Scripts/Play/ExplorerMotor.cs` Carries the rider vertically, resolves one-way landing and applies the two new pushes.
- `Assets/Scripts/Play/DynamicSolid.cs` Reports whether the previous frame had a usable support top.
- `Assets/Scripts/Play/LevelGeometry.cs` Exposes dynamic tops for ground queries, with the motor recording actual landing support.
- `Assets/Scripts/Play/BackdropLayer.cs` Lets the new theme entries set their cloud drift to the level clock.
- `Assets/Scripts/Play/LevelController.cs` Passes the authored ground speed to the motor during setup.
- `Assets/Scripts/Rules/LevelLayout.cs` Stores the optional ground speed.
- `Assets/Scripts/Rules/LevelData.cs` Sets 72 px/s grounded walking for Hills Normal.
- `Assets/Scripts/Rules/OneWayTop.cs` Checks relative landing from above, including reappearing tops.
- `Assets/Scripts/Rules/SinkingFloat.cs` Keeps attempt state and the float timing rules independent of Unity.
- `Assets/Tests/EditMode/HillsNormalRulesTests.cs` Adds eight recovery and moving-top regressions.
- `Assets/Tests/EditMode/HillsNormalLevelsTests.cs` Checks the 72 px/s value and the defaults on older trails.
- `Assets/Editor/LevelBuilder.cs` Builds water gaps and pools and assigns art and ambience.
- `Assets/Editor/HardLevelObjects.cs` Uses the current top tile over a current strip.
- `Assets/Editor/HillsLevelObjects.cs` Builds the new objects, views and sounds.
- `Assets/Editor/LevelObjects.cs` Adds balloons and rafts to the one-way solid list.
- `Assets/Editor/LevelThemes.cs` Adds five theme entries and living props checked by HillsPropFits.
- `Assets/Editor/PixelSceneKit.cs` Loads the chosen ambience and names the new art and sound generators in diagnostics.
- `ProjectSettings/EditorBuildSettings.asset` Adds the five enabled Hills Normal scenes.
- Unity saved all 23 generated level scenes and added the new scene and source import records. The handover lists these individually.

## Approved numbers and verification

- Grounded walk speed is 72 px/s for Hills Normal, so the 30 px/s current yields 42 px/s against it. Air speed remains 84 px/s. No layout coordinates changed.
- Fixed decoration x moves (px), nearest legal spots: seabird 2600 to 2585, bulul 1500 to 1493 and 4800 to 4781, cone house 900 to 883 and 5600 to 5633, far balloon 4200 to 4137 and 6500 to 6493.
- The batch gameplay audit covered raft support and recovery, pause, the safe pool, water checkpoints, bat push, river speed, the terrace mix, and the balloon crossing and zeolite card. It used temporary progress in memory.
- The Standards and Spec reviews found landing and pause edge cases. The motor now sweeps changing tops from the prior frame, remembers the actual support, and keeps a rider attached across pause. Added NUnit coverage and reran the scene audit.
