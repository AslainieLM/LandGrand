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
- [x] `LevelCatalog` matches `layouts.md` number for number
- [x] The Hills shelf has 17 kinds; Mountains shelf unchanged
- [x] EditMode tests pass (report the count)

**Blocked by:** 01

**Status:** done

**Verified:** N1–N5 placements match the bot data, lesson and review text match section 7, and Unity EditMode passed 352 of 352 tests.

**Builder to run:** none (ticket 06 builds the scenes)

## Files

- `Assets/Scripts/Rules/LevelData.cs` appends the seven kinds and five levels and identifies which pits hold water.
- `Assets/Scripts/Rules/LevelLayout.cs` appends the five themes and stores optional safe pool data.
- `Assets/Tests/EditMode/HillsNormalLevelsTests.cs` verifies sizes, object counts, practice spots, clearance, secret ledges, checkpoint return and ambience.
- `Assets/Tests/EditMode/HillsNormalLevelsTests.cs.meta` is the Unity script import record.
- `Assets/Tests/EditMode/LevelCatalogTests.cs` recognizes Hills Normal and keeps Hills Hard unavailable.
- `Assets/Tests/EditMode/NextPlayableTests.cs` checks both the new Easy-to-Normal route and the end of Normal.
- `Assets/Tests/EditMode/CollectionTests.cs` checks the larger Hills shelf and overall kind count.
- `Assets/Tests/EditMode/HillsEasyLevelsTests.cs` verifies the original ten Easy kinds by name.
- `Assets/Tests/EditMode/HillsBadgeTests.cs` confirms the approved full-shelf Hill Collector requirement on a save with the original ten kinds.
- `Assets/Tests/EditMode/MountainsNormalTests.cs` scopes the existing five-level regression to Mountains.

No layout numbers changed. The user confirmed that water pits follow the Normal checkpoint return and Hill Collector still requires the full Hills shelf.
