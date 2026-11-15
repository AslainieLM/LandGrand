# 06: Landform unlock rules and the map

**What to build:** Finishing all 15 Mountains levels opens Hills, saved through the clears that are already saved.

1. **Rules** (no Unity types), in `PlayerProgress` and `LevelCatalog`, with tests:
   - `IsEpisodeComplete(Episode e)`: every Easy, Normal and Hard level of `e` is cleared (3 + 5 + 7, from `LevelCount`).
   - `IsEpisodeUnlocked(Episode e)`: `e == Mountains`, or `IsEpisodeComplete(e − 1)`. This replaces today's `e == Episode.Mountains`.
   - `StarsFor(Episode e)` and `MaxStarsFor(Episode e)` (levels × 3 = 45 for Mountains).
   - `LevelCatalog.HasLevels(Episode e)`.
   - `NextAfter` keeps working: after Hard 7 of a complete landform it returns `NextEpisode` (as now).
2. **Landform Map** (`LandformMapController`):
   - Open regions come from the new `IsEpisodeUnlocked`, so Hills shows in colour and animated once Mountains is complete.
   - Tapping an open region with `HasLevels` false shows the guide text "Coming soon". Do not open the difficulty screen for it.
   - **One-time unlock animation** for each open landform other than Mountains whose device flag `LandGrand.UnlockSeen.<Episode>` (`PlayerPrefs`, like `GameSettings`) is not set:
     - padlock frames `Description/unlock_padlock_00..03` over 0.8 s at the region's padlock spot;
     - the grey `locked` image fades out over 0.7 s;
     - three `sparkle_00..02` sparkles;
     - `sfx_unlock.wav`;
     - the guide "Hills is now open!" for 2 s;
     - taps are blocked for the 1.5 s;
     - then set the flag.
   - Put the "should this landform play its unlock?" decision in a small rules function (open, not Mountains, not yet seen) with a test. Keep `PlayerPrefs` on the Unity side.
3. **Collection tabs** in the Profile already follow the open landforms. Check that the Hills tab opens and shows "Coming soon".

**Blocked by:** 03 (the Hard levels must exist for the full "complete" test)

**Status:** done

- [x] Tests: Hills is locked until all 15 Mountains levels are cleared, then unlocked; Plains stays locked; stars sum and max; `HasLevels` is true for Mountains and false for Hills
- [x] Tests: the unlock-animation rule plays once per landform and never for Mountains
- [x] Existing map and progress tests pass (update the old "only Mountains is open" test to the new rule)
- [x] The save file format does not change (`ProgressSaveTests` pass)
- [x] Basic-English comments

**Verified:** Unity EditMode 219 passed, including save-format regression tests. The map and Profile rebuilt, and live checks proved the one-time Hills unlock and Coming soon on both screens.

**Builder to run:** `python Tools/Art/make_hard_art.py` (Description sprites), then **LandGrand > Build Landform Map Scene** and **LandGrand > Build Profile Scene**.

Spec: `../spec.md` ("New decisions" 7–10, "Landform Description Screen", Flow).
