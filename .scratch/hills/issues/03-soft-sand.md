# 03: Soft sand: rule, play part and builder

**What to build** (spec section 5):

1. `Assets/Scripts/Rules/SoftSand.cs` with `WalkShare = 0.6f`, `StepSeconds = 0.3f` and `WalkSpeed(walk, onSand)`.
2. `SandZone(x, width)` with `Covers(px)` (both edges included). Add it to `LevelLayout` as an optional `sandZones` array that defaults to empty.
3. Add Hills Easy 3's four zones to its data: (560, 160), (1900, 400), (2640, 150) and (4050, 330).
4. `Assets/Scripts/Play/SandWatcher.cs`, a new level part:
   - The explorer is "on sand" only on the flat ground line inside a zone, never on a ledge, boulder or moving solid (copy the `IceWatcher` test).
   - It sets the walking share on `ExplorerMotor` next to the ice code (`ExplorerMotor.cs:109-111`). Jumps and air speed are unchanged.
   - Every 0.3 s while walking on sand it plays `sfx_sand_step.wav` at volume 0.5 and shows the two `Props/sand_puff` frames at the feet. It reads `LevelClock.Now`.
5. Builder: swap the ground-top tile for `soft_top` over each zone, like the ice swap in `HardLevelObjects.cs:52-65`. Load `sfx_sand_step.wav`; if it is missing, the error names `Tools/Audio/make_hills_audio.py`.
6. Tests:
   - `WalkSpeed` on and off sand; `Covers` edges.
   - Hills E3: the first zone has no pit or spikes within 80 px; no zone is within 40 px of the checkpoint.
   - All Mountains levels have no zones.

**Checklist**
- [x] The explorer walks slower on the darker soft sand and jumps the same
- [x] Step sound and sand puffs appear only while walking on sand, and stop in the pause menu
- [ ] Easy 3 can be cleared in about 80 s at a steady walk (bot: 81.1 s)
- [ ] EditMode tests pass (report the count)

**Blocked by:** 02

**Status:** built by Claude (Haiku), not yet run in Unity

**Builder to run:** **LandGrand > Build Levels**
