# 08: Profile: third Collection page, 5×3 badges, four new badges

**What to build:**

1. **Collection pages.** Mountains now has 34 kinds, so the existing paging shows 3 pages (12, 12, 10) with "1/3". No layout change; check that the third page and the arrows work.
2. **Four new badges** in `AchievementCatalog.All`, after Rock Scholar, worked out from saved progress only (the save file does not change):
   - `brave_hiker`, "Brave Hiker", silver: "Clear any Mountains Hard level."
   - `world_peaks`, "World Peaks", silver: "Clear the Matterhorn, Kilimanjaro and Mauna Kea (Hard 5 to 7)."
   - `hard_star`, "Hard Star", gold: "Get 3 stars on all 7 Mountains Hard levels."
   - `mountain_master`, "Mountain Master", gold: "Clear all 15 Mountains levels."
   - The pictures `badge_<id>.png` and `badge_<id>_locked.png` already exist (`make_profile_art.py`).
   - Mountain Collector now needs all 34 kinds automatically. Do not change its rule.
3. **Achievements grid 5×3.**
   - `ProfileBuilder` lays the badges out in rows of 5 and fits 3 rows (15 cells) in the page panel.
   - Starting point: x = 56 + column × 92, rows at y 104, 162 and 220, name width 88.
   - Check that the names do not overlap; shrink the spacing, not the font.
   - Update the "4 by 2" comment and the "Badges 0/11" placeholder; the count shows "Badges n/15".
   - The Result Screen's "New badge!" notice picks up the new badges automatically.

**Blocked by:** 03

**Status:** done

- [x] Paging tests: 34 kinds give 3 pages; the last page has 10
- [x] Badge rule tests for the four new badges; existing badge tests pass (update counts from 11 to 15)
- [x] The Profile builds; check the 5×3 grid and page 3 in a screenshot, or ask the user
- [x] Basic-English comments

**Verified:** Unity EditMode 228 passed. Reviewed 480×270 renders of all 15 badge names at Andika 15 and all ten items on Collection page 3.

**Builder to run:** `python Tools/Art/make_profile_art.py`, then **LandGrand > Build Profile Scene**.

Spec: `../spec.md` ("Profile changes").
