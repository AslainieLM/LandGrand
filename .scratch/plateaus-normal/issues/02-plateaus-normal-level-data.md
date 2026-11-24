# 02: Plateaus Normal 1–5 level data

**What to build** (spec sections 3, 6 and 7; `layouts.md`):

1. `Rules/LevelData.cs` `LevelCatalog`: Plateaus Normal 1–5 with every number from `layouts.md` copied exactly (pits with the water flag for N3 and N4 and the N1 safe pool, spikes, boulders, ledges, crumbling bridges, sand, updrafts, cable-car platforms, currents, floats, salt-crust ice, llama herds, pickups with one secret), banners, goals, themes, lessons and reviews. Copy the shape of the Plains Normal entries and the Plateaus Easy updrafts.
2. **Tests** (`Assets/Tests/EditMode/PlateausNormalLevelsTests.cs`, like `PlainsNormalLevelsTests.cs`): each level's size, checkpoints, 180 s, 7 pickups, one secret at y ≥ 72, theme, banner, goal; every pickup kind on the Plateaus shelf; every updraft has its landing ledge at `x + 40`, `top − 4`; N1 has 4 platforms with the first over the safe pool; N5 has 3 ice stretches and 3 herds; no herd, ice or platform in the other Plateaus Normal levels where the layout has none; lesson sentences at most 44 characters; all text draft. Progression: Plateaus Normal opens after Plateaus Easy; Table Top and Canyon Explorer badges earn on N1 and N3.

**Checklist**
- [x] Numbers match `layouts.md` exactly
- [x] EditMode tests pass (report the count)

**Blocked by:** 01

**Status:** done (2026-11-24, 563 tests at the end of the build; restored after a planning script reset this line)

**Builder to run:** none
