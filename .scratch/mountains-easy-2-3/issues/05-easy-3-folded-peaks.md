# 05: Easy 3 "Folded Peaks" — lesson, theme and layout

**What to build:** Add Mountains Easy 3 to the level catalog with the spec's lesson (heading "Level 3: Folded Mountains", four sentences, new picture `lesson_folds`), review text and pickups (slate, mica review pickup, marble, garnet in the secret spot, slate). Add the Folded rock level theme: grey layered ground with folded bands in the cliffs and a summit stone pile with a flag as the goal, on the shared Mountains backdrop with the same music and Easy breeze. Lay out about 5400 px in teach-test-twist order: stepped climb up folded ledges, pit 1, spike 1, checkpoint, a wide pit that needs the double jump, a spike pair, the high off-path garnet ledge (reached by backtracking up the folds), pit 3, goal. The climb stays inside the current camera framing; lower it rather than change the camera.

**Blocked by:** Tickets 01, 02

**Status:** to do

- [ ] Level 3 on the Easy trail opens this lesson and Start opens the `MountainsEasy3` scene
- [ ] Lesson heading and sentences in Andika (D-051), all fitting; draft note shown
- [ ] 5 pickups, 1 checkpoint, 3 pits, 3 spikes, 120 s; the wide pit cannot be crossed with a single jump but always with a double jump
- [ ] First finds of slate, marble and garnet pop up; mica gives only a chime if already found
- [ ] The level is finishable in about 75–105 s without hazards; 5/5 gives 3 stars; the Result shows the review text
- [ ] Clearing Easy 3 unlocks Normal; Normal level 1 shows the existing "Coming soon" lesson; Continue after Easy 3 returns to the level trail
- [ ] Catalog and progress tests cover Easy 3 and the Normal unlock
- [ ] Every new line of code has a short basic-English comment (D-033)

**Builder to run:** `python Tools/Art/make_lesson_art.py`, `python Tools/Art/make_level_art.py`, then **LandGrand > Build Lesson Scene** and **LandGrand > Build Mountains Levels**.

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md` (D-055).
