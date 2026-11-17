# 01: Rules for slippery mud and the rolling log

**What to build** (spec sections 2 and 4; numbers from the `layouts.md` "Rule constants" table):

1. `Rules/MudZone.cs` (new):
   - X and Width, with `Contains(x)`, exactly like `IceZone`.
   - The slide is `IceSlide` with the same numbers, and the footing uses `ZoneFooting`.
2. `Rules/RollingLog.cs` (new):
   - Spawn, End, Direction (+1 right, −1 left) and Offset.
   - It reuses the `RollingBoulder` timing constants: a 3.5 s cycle, a 0.8 s warning, 90 px/s, a 14×14 box and a 48 px skip distance.
   - x = Spawn + Direction × elapsed × Speed.
   - It is gone once it passes End in its direction.
   - A run is at most 243 px.
   - Do **not** change `RollingBoulder`.
3. `Rules/LevelLayout.cs`: add optional `MudZones` and `Logs` at the **end** of the constructor. Existing callers must compile unchanged.
4. `Rules/LandformAudio.cs`: in the script-name table, map `amb_wind_turbines` and `amb_city_hills` to `make_hills_hard_audio.py`.
5. **Tests:**
   - Mud gives the same speeds as ice for the same inputs.
   - Logs in both directions: the position, the end, the warning, and the skip when the explorer is within 48 px of Spawn.
   - Mountains boulders give exactly the same positions as before.
   - The script names map correctly.

**Checklist**
- [ ] No Unity types in Rules; the constants match `layouts.md`
- [ ] `RollingBoulder` and ice behave the same as before (tests)
- [ ] EditMode tests pass (report the count; 360 before)

**Blocked by:** none

**Status:** not started

**Builder to run:** none
