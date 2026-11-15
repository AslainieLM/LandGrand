# 06: Profile: Collection pages, 4×3 badges, three new badges

**What to build:**

1. **Collection pages.**
   - The Mountains tab now has 20 kinds and the grid shows 6×2 = 12. Add page arrows inside the landform tab: Prev and Next icons with "1/2" between them.
   - Paging existed in commit `5cc908b` (`Collection.PageCount` and `SlotsOnPage`, removed in `3447c4c`). Reuse that rules code, its tests and the existing arrow icons.
   - Changing the landform tab resets to page 1.
   - The arrows hide when a landform has 12 kinds or fewer.
2. **Three new badges** in `AchievementCatalog.All`, after Mountain Collector, worked out from saved progress only (the save file does not change):
   - `philippine_peaks`, "Philippine Peaks", silver: "Clear Mountains Normal 1 to 4."
   - `top_of_the_world`, "Top of the World", gold: "Clear Mount Everest (Mountains Normal 5)."
   - `rock_scholar`, "Rock Scholar", silver: "Find 15 different rocks and minerals."
   - The pictures `badge_<id>.png` and `badge_<id>_locked.png` already exist.
3. **Achievements grid 4×3.**
   - `ProfileBuilder` lays the badges out in rows of 4 and fits 3 rows in the page panel (y 80 to 264 on screen), with each name under its medal. Rows at y 104, 162 and 220 are a starting point; check the names do not overlap.
   - "Badges n/11" counts all badges.
   - The "New badge!" notice on the Result Screen picks up the new badges automatically.

**Blocked by:** 03

**Status:** to do

- [ ] Paging tests: 20 kinds give 2 pages; 12 kinds give 1 page with the arrows hidden
- [ ] Badge rule tests for the three new badges
- [ ] Existing badge tests pass
- [ ] The Profile builds; check the 4×3 grid and the page arrows in a screenshot, or ask the user to
- [ ] Basic-English comments

**Builder to run:** `python Tools/Art/make_profile_art.py`, then **LandGrand > Build Profile Scene**.

Spec: `../spec.md` ("New decisions" 5 and 6).
