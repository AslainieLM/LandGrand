# 02: Plateaus Easy 1–3 level data

**What to build** (spec section 6, `layouts.md`):

1. `Rules/LevelData.cs` `LevelCatalog`: add Plateaus Easy 1, 2 and 3 with every number from `layouts.md` copied exactly: width, goal, checkpoint, pits, spikes, boulders, ledges, updrafts (E3 only), pickups with the one secret, banner title, goal name, theme, lesson (title, 4 sentences, picture) and review (3 sentences) from spec section 7. Copy the shape of the Plains Easy entries.
2. **Tests** (a new `Assets/Tests/EditMode/PlateausEasyLevelsTests.cs`, like `PlainsEasyLevelsTests.cs`):
   - each level: width, goal, one checkpoint, 5 pickups, exactly one secret, 120 s, theme, banner and goal names
   - every pickup kind is on the Plateaus shelf; E1 teaches basalt, andesite, hematite and halloysite; E2 has the halloysite review; E3 has the goethite review
   - E3 has 4 updrafts; the first is safe (no pit or spikes within 80 px); none within 40 px of the checkpoint or 60 px of a pit or spikes; each has its landing ledge at `x + 40`, `top − 4`; the hovering head (`top + 26`) is at or below 196
   - no other level in the catalog has updrafts
   - every lesson sentence is at most 44 characters; all text is a draft

**Checklist**
- [x] Numbers match `layouts.md` exactly
- [x] EditMode tests pass (report the count)

**Blocked by:** 01

**Status:** done (2026-11-24, 524)

**Builder to run:** none
