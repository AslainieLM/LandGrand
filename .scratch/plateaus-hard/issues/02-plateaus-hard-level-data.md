# 02: Plateaus Hard 1–7 level data

**What to build** (spec sections 3 and 6, `layouts.md`):

1. A `PlateausHard(...)` helper like `PlainsHard(...)` (Hard sizes, `bgm_plateaus`, Plateaus Hard ambience from the table).
2. Seven `LevelCatalog` entries, copied exactly from `layouts.md`: pits, spikes, boulders, ledges, crumbles, vents, wind zones, sand, ice, herds (`new Bat(cx, 6f, 48f, 0f, period, phase)`), updrafts, moving platforms (horizontal and `MovingPlatform.Vertical`), pickups with their kind ids, banners, goal names, lessons and reviews.
3. `PlateausHardLevelsTests.cs` like `PlainsHardLevelsTests.cs`: sizes, checkpoints, 9 pickups, one secret at y >= 72, the kinds per level, banner and goal text, lesson lines at most 44 characters, the Plateaus Hard badges now reachable (`mesa_ranger`, `world_plateaus`, `steady_plateaus`, `plateau_master`, `land_grand_champion`).

**Checklist**
- [ ] Every number matches `layouts.md` (report any change and why; never change one silently)
- [ ] Next Level and Next Episode: Plateaus Hard 7 has no next landform, so the Result Screen shows no Next Episode button; check `NextPlayableAfter`
- [ ] EditMode tests pass (report the count)

**Blocked by:** 01

**Status:** open

**Builder to run:** none
