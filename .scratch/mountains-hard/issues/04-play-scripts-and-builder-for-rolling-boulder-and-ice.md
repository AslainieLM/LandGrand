# 04: Play scripts and builder for the rolling boulder and icy ground

**What to build:** Make the two new objects work in the level scenes, and build the seven Hard scenes. Each new job is a **new part or view** (D-061).

1. **Explorer motor: icy ground.**
   - `ExplorerMotor` keeps a walking speed (`walkVelocity`) separate from the wind push. Today it sets `velocity.x = dir * walkSpeed + ExternalPushX`.
   - When the explorer is grounded at ground level (not on a ledge, crumbling ledge or platform) and its middle is inside an `IceZone`, use `IceSlide.Next(walkVelocity, dir * walkSpeed, dt)`. Otherwise `walkVelocity = dir * walkSpeed`, as now.
   - Then `velocity.x = walkVelocity + ExternalPushX`. Hitting a wall sets `walkVelocity` to 0.
   - The motor learns about the ice from a small new part, for example `IceWatcher` (like `WindWatcher`), that sets `OnIce` on the motor each frame from the level data.
   - It plays `sfx_ice_slide.wav` once each time a slide starts: on ice, moving faster than 20 px/s, and the held direction is 0 or the opposite way.
2. **Views** (`Assets/Scripts/Play`), each asking the ticket 02 rules and reading `LevelClock.Now`:
   - `RollingBoulderView`: the dust warning at `Spawn` (`rumble_dust_00/01`), the rolling boulder (`roll_boulder_00..03`, a new frame every 0.1 s), and the dust puff at `End` (the start-screen `Dust` frames). Sounds: `sfx_boulder_rumble.wav` once at the warning when the explorer is within 300 px; `sfx_boulder_roll.wav` looping while rolling within 260 px.
   - `IceZoneView`: draws `ice_top` tiles over the ground top from `X` to `X + Width`, and two or three twinkling `ice_glint_00/01` props.
3. **Parts on the Level Controller.**
   - A new `RollingBoulderWatcher` part calls `RollingBoulderRun.Update(LevelClock.Now, explorerX)` each frame, so the skip rule works. The views and `HazardWatcher` read `CurrentX` from it.
   - `HazardWatcher` treats a rolling-boulder hit as a trap, with the checkpoint rule (the same as a falling rock).
   - Checkpoints: three flags in the level and on the trail bar; each turns green when reached; "Checkpoint reached!" each time.
4. **Builder.** `LevelBuilder` builds every object from the layout data and builds the seven scenes `MountainsHard1`–`MountainsHard7`, adding them to the build list after `MountainsNormal5`. Sorting orders: rolling boulder 19, rumble dust 7, ice top like the ground top, ice glints 5.
5. **Hard rules.** The Easy short-return rules stay Easy-only. Hard uses the checkpoint for spikes, pits, rocks and rolling boulders.

Until ticket 05 adds the seven themes, the builder stops with a "no background" error for the Hard themes (D-063). Either do ticket 05 before building the Hard scenes, or build Easy and Normal only for now and say so.

**Blocked by:** 02, 03

**Status:** implemented and verified in focused Play Mode; full Easy/Normal play check remains

- [ ] Easy 1–3 and Normal 1–5 rebuild and play exactly as before (vents, banner, spikes, pits, the Easy return rule, wind, platforms, crumbling ledges)
- [x] Each Hard level builds; a rolling boulder warns, rolls, can be jumped, and sends the explorer to the right checkpoint when it hits; a boulder does not appear on top of the explorer
- [x] On ice the explorer speeds up and slides as specified; off ice, walking is instant; wind still pushes on ice
- [x] Rules tests still pass; add tests for any new rules code
- [x] No per-level code anywhere: the builder reads everything from the layout data
- [x] Basic-English comments

**Verified:** Unity EditMode 216 passed and all eight Easy/Normal scenes rebuilt. All seven Hard scenes built in ticket 05. Live checks passed for warnings, skips, jumping, checkpoint three, ice and wind in ticket 07.

**Builder to run:** `python Tools/Art/make_hard_art.py`, `python Tools/Audio/make_hard_audio.py`, then **LandGrand > Build Mountains Levels**.

Spec: `../spec.md` ("Level objects: exact rules", "Assets").
