# 02: Plains Easy 1–3 level data

**What to build** (spec sections 6 and 7; the numbers are also in `layouts.md`):

1. `LevelCatalog`: add Plains Easy 1, 2 and 3 in the same shape as Hills Easy 1–3: start (40, 0), 120 s, 1 checkpoint, and 5 pickups with exactly one secret. Use the numbers exactly.
2. **Themes and banners:**
   - Themes `RicePlain`, `SaltBeds` and `RiverPlain`.
   - Banners "Central Luzon Plain", "Pangasinan Salt Beds" and "Isabela River Plain".
3. **Lessons, reviews and kinds** from spec section 7, all marked draft:
   - Lesson titles "Plains", "Coastal Plains" and "River Plains".
   - Pictures `lesson_central_luzon`, `lesson_pangasinan` and `lesson_isabela`.
4. Plains Easy 3 gets the four straw piles at 600, 1200, 3300 and 4500.
5. **Tests** (`PlainsEasyLevelsTests.cs`, with the same checks as the Hills Easy level tests):
   - **Level basics:** widths, goals, checkpoints, timers, pickup counts, one secret per level, and the kinds and their order.
   - **Straw piles:**
     - only Plains Easy 3 has them, and it has 4
     - the first is safe
     - none is within 40 px of the checkpoint, or within 60 px of a pit or spikes
   - **Content:** every pickup kind exists in `ItemCatalog`, and every lesson line is at most 44 characters.
   - **Progression:**
     - clearing Hills Hard 7 opens Plains Easy 1
     - after Plains Easy 3 is cleared, Plains Normal shows "Coming soon"
     - Next is hidden after Plains Easy 3 (no data)
   - **Regression:** Mountains and Hills level data are unchanged (the existing tests still pass).

**Checklist**
- [ ] Numbers exactly as in `layouts.md`; any change is reported, never made silently
- [ ] EditMode tests pass (report the count)

**Blocked by:** 01

**Status:** not started

**Builder to run:** none
