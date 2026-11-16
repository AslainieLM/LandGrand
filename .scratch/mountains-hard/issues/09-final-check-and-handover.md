# 09: Final check and handover

**What to do:**

1. Run all the asset scripts, then every builder:
   - `python Tools/Art/make_hard_art.py`
   - `python Tools/Art/make_hard_backdrops.py`
   - `python Tools/Art/make_profile_art.py`
   - `python Tools/Audio/make_hard_audio.py`
   - **Build Mountains Levels**
   - **Build Lesson Scene**
   - **Build Selection Scenes**
   - **Build Landform Map Scene**
   - **Build Landform Description Scene**
   - **Build Profile Scene**
2. Run the EditMode tests and report the count.
3. Add the new words to `CONTEXT.md`, the glossary, in its existing style: Rolling boulder, Icy ground, Hard checkpoints, Landform Description Screen, Episode complete and Unlock animation.
4. Write `.scratch/mountains-hard/HANDOVER.md` and a short handover in your final message:
   - what was built, file by file
   - test results
   - any number you changed from the spec, and why
   - anything not done or not checked in the Editor

   Do **not** edit the Obsidian vault (`D:\LandGrand\LandGrand-Sources`). Claude updates it from your report.

**Blocked by:** 01–08

**Status:** done; the user played the whole checklist on 2026-11-16 and every item passed (fixes in `0726daf`)

**Play checklist for the user** (copy it into the handover):
- [x] Each Hard level shows its banner (Mount Mayon to Mauna Kea), its lesson, its theme and its own background
- [x] Rolling boulders rumble first, roll left, can be jumped, never appear on top of the explorer, and send you to the last checkpoint
- [x] Ice makes the explorer slide; Mayon's first boulder and the Matterhorn's first ice patch are safe places to learn
- [x] All three checkpoint flags work, in the level and on the bar
- [x] The Result Screen shows 9 items; Next after Hard 7 opens the Landform Description Screen
- [x] The Description Screen shows the summary, stars n/45, kinds n/34, badges n/15 and "Hills is now open!"; the fanfare plays
- [x] To the Map: Hills unlocks with the padlock animation once; tapping Hills says "Coming soon"
- [x] The book button on the Mountains difficulty screen reopens the Description Screen; Back returns
- [x] The Profile shows 3 pages of Mountains kinds and 15 badges in a 5×3 grid
- [x] Easy and Normal still play as before

**Verified:** Unity EditMode 234 passed, 19 focused Play Mode checks passed, all six builders succeeded, and 191 regenerated assets matched in a temporary copy. The glossary and handover are complete.

**Builder to run:** all of the above.
