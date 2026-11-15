# 03: Easy 2 "Volcano Trail" — lesson, theme and layout

**What to build:** Add Mountains Easy 2 to the level catalog with the spec's lesson (heading "Level 2: Volcanic Mountains", four sentences, new picture `lesson_volcano`), review text and pickups (andesite, quartz review pickup, obsidian, pumice in the secret spot, andesite). Add the Volcanic level theme: dark volcanic rock ground, glowing lava cracks (decoration), and a lava-rock arch with a flag as the goal, on the shared Mountains backdrop with the same music and Easy breeze. Lay out about 5400 px in teach-test-twist order: safe start, pit 1 (single jump), spike 1, checkpoint, pit 2 between two ledges, spike 2 beside a ledge, goal. Leave the two steam-vent spots and the vent-reached secret ledge marked in the layout; until ticket 04 lands, the pumice ledge is reachable by double jump so the level is completable.

**Blocked by:** Tickets 01, 02

**Status:** to do

- [ ] Level 2 on the Easy trail opens this lesson and Start opens the `MountainsEasy2` scene
- [ ] The lesson heading and sentences use Andika (D-051), all fitting their boxes; draft note shown
- [ ] 5 pickups, 1 checkpoint, 2 pits, 2 spikes, 120 s; the Easy pit rule (D-048) and spike-to-checkpoint rule work here
- [ ] First finds of andesite, obsidian and pumice pop up; quartz gives only a chime if already found
- [ ] The level is finishable in about 70–100 s without hazards; 5/5 gives 3 stars; the Result shows the review text
- [ ] The Volcanic theme reads clearly: explorer, pickups, ledges and hazards stand out from the dark ground
- [ ] Catalog tests cover Easy 2; the old "Easy 2 is missing" test is replaced
- [ ] Every new line of code has a short basic-English comment (D-033)

**Builder to run:** `python Tools/Art/make_lesson_art.py`, `python Tools/Art/make_level_art.py`, then **LandGrand > Build Lesson Scene** and **LandGrand > Build Mountains Levels**.

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md` (D-055).
