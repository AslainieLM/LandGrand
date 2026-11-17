# 03: Play parts and builders for mud, logs, themes and props

**What to build** (spec sections 4 and 8):

1. **Mud:**
   - Make the motor's ice slide turn on over ice **or** mud. Extend `IceWatcher`, or add a `MudWatcher` part that sets the same state; choose whichever keeps ice identical.
   - On mud, play `sfx_mud_squelch` when a slide starts, and show `mud_splash_00/01` at the feet while sliding. Use `LevelClock.Now`.
   - No per-theme `if`: the mud zone data carries its own look and sound.
2. **Logs:**
   - A `LogRoller` level part, modelled on the rolling-boulder part.
   - It shows the `log_warn_00/01` warning at Spawn, rolls `log_00–03` in its direction (flip the frame order for right), and plays `sfx_log_roll` through `PlaySfx`.
   - A touch is the same trap as a boulder.
3. **Builders** (`LevelBuilder.cs`, `HardLevelObjects.cs`, `HillsLevelObjects.cs`, `LevelThemes.cs`):
   - The seven theme entries from the spec section 8 table.
   - `mud_top` swapped onto the ground over mud zones.
   - The logs.
   - Reuse `water_fill` and `current_top` for H5, H6 and H7.
   - The living props from spec section 8, all through `HillsPropFits`, which also refuses mud zones and log runs. Report any prop that moved more than 64 px.
   - The ambience comes from `LandformAudio.AmbienceFor(level)`.
   - Missing-asset errors name the right `make_hills_hard_*` script.
4. **Build:**
   - Run the three asset scripts. They must reproduce the same files; report any difference.
   - Then run every builder in batch mode. The log must show no "Missing" and no errors.

**Checklist**
- [ ] `HillsHard1`–`HillsHard7` are built and in Build Settings
- [ ] H4 links `amb_wind_turbines`, H6 `amb_city_hills`, the others `amb_rain_hard`; Mountains, Hills Easy and Hills Normal scenes link what they did before
- [ ] No per-level or per-theme `if` in play scripts; ice and boulders unchanged
- [ ] EditMode tests pass (report the count)

**Blocked by:** 01, 02

**Status:** not started

**Builder to run:** **LandGrand > Build Levels**, **Build Lesson Scene**, **Build Selection Scenes**, **Build Landform Map Scene**, **Build Landform Description Scene**, **Build Profile Scene**
