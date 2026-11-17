# 05: Hills Normal 1–5 level data, the seven kinds and the five themes

**What to build** (spec sections 3 and 7, `layouts.md`):

1. `Rules/LevelLayout.cs`: add `IslandHills`, `LimestoneCave`, `RiverGorge`, `RiceTerraces` and `FairyChimneys` **at the end** of `LevelTheme`.
2. `Rules/LevelData.cs`:
   - The seven new kinds from spec section 7 (Hills only, `IsDraft` true), appended **at the end** of `ItemCatalog.All`.
   - Hills Normal 1–5 in `LevelCatalog`, with **exactly** the numbers in `layouts.md`. Use the lessons, reviews and banners in spec section 7.
   - N2 has `AmbienceOverride = "amb_cave_drips"`.
3. **Tests** (`HillsNormalLevelsTests.cs`, new), with the same checks as `HillsEasyLevelsTests`:
   - The Normal rules and clearances.
   - The secret spot is on a high ledge.
   - The first new object of each level is in a safe practice spot.
   - The Hills shelf has 17 kinds.
   - N2's ambience is `amb_cave_drips`.
   - Mountains and Hills Easy are unchanged.
4. Update tests that assumed Hills has only Easy levels, for example `LevelCatalogTests` and `NextPlayableTests`. After Hills Easy 3, Next now opens Hills Normal 1.

**Checklist**
- [ ] `LevelCatalog` matches `layouts.md` number for number
- [ ] The Hills shelf has 17 kinds; Mountains shelf unchanged
- [ ] EditMode tests pass (report the count)

**Blocked by:** 01

**Status:** not started

**Builder to run:** none (ticket 06 builds the scenes)
