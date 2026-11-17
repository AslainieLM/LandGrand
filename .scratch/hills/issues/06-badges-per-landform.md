# 06: Badges per landform, the Profile badge pages and the Description totals

**What to build** (spec sections 2, 3 and 8):

1. `Achievements.cs`:
   - Give each badge a `Landform`, which is its page.
   - The 15 existing badges are on the Mountains page, with ids and order unchanged.
   - Replace the Mountains-only rules with small helpers that take the episode and level numbers. `world_peaks` reads `{5, 6, 7}` as data, and the same helpers make the Hills rules.
   - Add the 10 Hills badges (ids, tiers, rules and hints in spec section 3) after the Mountains ones.
   - Add `ForLandform(Episode)`.
2. Profile (`ProfileBuilder.cs:151-153`, `ProfileController.cs`):
   - The Achievements tab gets the four landform tabs, with the same sprites and lock rules as the Collection, using `RegionGuide.ForLocked`. Remove the copy at `ProfileController.cs:148`.
   - Each landform page is the same 5×3 grid. "Badges n/N" counts the shown page.
   - Plains and Plateaus show "Coming soon" when open.
3. `LandformDescriptionController.cs:48`: the badge total counts this landform's page only.
4. The "New badge!" notice on the Result Screen works for Hills badges.
5. Tests:
   - Update `AchievementTests.cs:14,35` and `HardProfileTests.cs:9`; the Mountains list is unchanged.
   - Hills rules: Pasture Pal after Hills E2; Dune Surfer after E3; Rolling Star with 3 stars on all three; Hill Collector with all 10 Hills kinds; the Normal and Hard badges stay locked without data.
   - `world_peaks` still needs Mountains Hard 5–7.

**Checklist**
- [ ] The Mountains badge page looks exactly as before (15 badges, 5×3); the four landform tabs now sit above it, so check the row spacing in Unity
- [x] The Hills badge page shows 10 badges in two rows; earned ones are in colour (built in code)
- [x] The Description Screen after Mountains Hard 7 still says badges n/15 (built in code)
- [ ] EditMode tests pass (report the count); not run yet

**Blocked by:** 01

**Status:** built by Claude (Haiku), not yet run in Unity

**Builder to run:** **LandGrand > Build Profile Scene**, **LandGrand > Build Landform Description Scene**
