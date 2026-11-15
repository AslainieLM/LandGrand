# 04: Play scripts and builder for the level objects

**What to build:** Make the four objects work in the level scenes. Each new job is a **new part or view** (D-061).

1. **Explorer motor.**
   - `ExplorerMotor` and `LevelGeometry` must support **solids that move or switch off**: platforms and crumbling ledges.
   - Suggested shape: `LevelGeometry` keeps the static solids, plus a list of dynamic solids that each give their current `Rect` and how far they moved this frame.
   - Standing on a moving platform carries the explorer by the platform's movement.
   - `HasGroundAt` (used by the Easy return rule) still works.
   - The motor gets an extra horizontal speed from the wind, set each frame (for example `ExternalPushX`).
2. **Views** (`Assets/Scripts/Play`), each asking the ticket 02 rules:
   - `CrumbleLedgeView`: whole, cracking frames, breaking, gone, back. It spawns 3–4 falling `crumble_bit` chips and plays `sfx_crumble.wav` once when it breaks.
   - `RockfallView`: the warning shadow (2 frames), the pebble trickle, the falling rock, the landing with `sfx_rockfall.wav` (only when the explorer is within 260 px) and the dust (the start-screen `Dust` frames).
   - `MovingPlatformView`: draws `platform` at `LeftAt(t)`.
   - `WindZoneView`: during the warning and push it draws `gust_00..03` streaks across the zone at about 6 positions, and plays `sfx_gust.wav` once at the warning when the explorer is in or near the zone.
3. **Parts on the Level Controller.**
   - `HazardWatcher` treats a rock hit as a trap and a fall through a broken bridge as a pit, with the Normal checkpoint rule.
   - A new small part, for example `WindWatcher`, sets the motor's push.
   - Checkpoints:
     - Show **one flag per checkpoint** in the level and on the trail bar.
     - Each flag turns green when reached.
     - The guide says "Checkpoint reached!" each time.
4. **Builder.** `LevelBuilder` builds every object from the layout data. Sorting orders:
   - crumbling ledges 6, like ledges
   - platforms 6
   - the warning shadow 7 and the rock 19
   - wind streaks 22, in front of the explorer
5. **Normal rules.** The Easy short-return rules stay Easy-only. Normal uses the checkpoint for spikes, pits and rocks.

**Blocked by:** 02, 03

**Status:** to do

- [ ] Easy 1–3 rebuild and play exactly as before (vents, banner, spikes, pits, the Easy return rule)
- [ ] Each Normal level builds; the explorer can ride a platform, crosses a crumbling bridge by walking, is sent back to the right checkpoint by a rock, and is pushed back by gusts
- [ ] Rules tests still pass; add tests for any new rules code
- [ ] No per-level code anywhere: the builder reads everything from the layout data
- [ ] Basic-English comments

**Builder to run:** `python Tools/Art/make_normal_art.py`, `python Tools/Audio/make_normal_audio.py`, then **LandGrand > Build Mountains Levels**.

Spec: `../spec.md` ("Level objects: exact rules").
