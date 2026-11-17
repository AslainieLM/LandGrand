# 02: Hills Hard 1–7 level data, six kinds and seven themes

**What to build** (spec sections 3 and 7, `layouts.md`):

1. `LevelTheme`: add TarsierForest, RainySlope, KarstRidge, WindHills, DrumlinBay, CityHills and KarstTowers **at the end**.
2. `ItemCatalog`: add peridotite, chlorite, dacite, quartzite, travertine and leucite, Hills only and `IsDraft`, appended at the **end** of `All`. The text is in spec section 7.
3. `LevelCatalog`: add Hills Hard 1–7 with **exactly** the `layouts.md` numbers:
   - Pickup kinds come from the k1–k9 table in spec section 3.
   - Lessons, reviews and banners come from spec section 7.
   - H4 has `AmbienceOverride = "amb_wind_turbines"` and H6 has `"amb_city_hills"`.
4. **Tests:**
   - A new `HillsHardLevelsTests.cs` with the same checks as `HillsNormalLevelsTests`: sizes, 3 flags, 9 pickups with one secret at y ≥ 72, the safe first mud in H1 and the safe first log in H2, clearances, and the ambience.
   - The Hills shelf has 23 kinds; Mountains is unchanged.
   - Update the tests that assumed Hills Hard has no levels:
     - After Hills Normal 5, Next opens Hills Hard 1.
     - After Hills Hard 7, Next Episode opens the Description Screen.
     - Hills becomes complete, so Plains unlocks.
   - Plains Easy stays "Coming soon".

**Checklist**
- [ ] `LevelCatalog` matches `layouts.md` number for number
- [ ] Hills shelf is 23; Hill Collector needs all 23
- [ ] EditMode tests pass (report the count)

**Blocked by:** 01

**Status:** not started

**Builder to run:** none (ticket 03 builds the scenes)
