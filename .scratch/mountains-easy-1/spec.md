---
status: ready-for-agent
created: 2026-11-13
sources:
  - Vault Project/Gameplay Design.md (D-039 to D-044)
  - Vault Sources/Final Manuscript Chapters I-III 2026-11-13.md (D-036, LG-26 to LG-32)
  - Vault Manuscript/Drafts/Land Grand - Chapter III - Game Procedure Flowchart - v2.drawio
---

# Spec: First playable level — Mountains, Easy 1

## Problem Statement

LandGrand has a finished start-up screen and Main Menu, but Play only shows "Coming soon". A Grade 5 learner cannot yet do the thing the game exists for: read a short lesson about a landform, explore it, collect rocks and minerals, read what they are, and see a result. The capstone manuscript (final Chapters I–III, which the game must strictly follow under D-036) describes that whole loop, and the user has now settled the gameplay details for it (D-039 to D-044). Until one complete level works on the device, nothing about the learning design, controls, scoring or saving can be tried with learners, teachers or panelists.

## Solution

Build the first complete, playable slice of the game, end to end, for one level: **Mountains, Easy, level 1**. From the Main Menu the player taps Play, enters a name the first time, sees the Landform Map with Mountains unlocked and the other three landforms locked, picks Mountains, picks Easy, picks level 1, reads the lesson, plays a short side-scrolling mountain trail with touch controls, collects five rocks and minerals (the first find of each kind pops up its name and description), avoids a pit and a spike trap (which return them to a checkpoint), reaches the cave goal before the 2-minute timer runs out, and sees a Result Screen with 1–3 stars, the items found, the goals and a short review. Progress is saved on the device and loaded the next time the game opens. Everything follows the redesigned flowchart (version 2), looks like the existing pixel-art screens, and keeps working offline.

## User Stories

### Launch and saving
1. As a learner, I want my progress to load automatically when I open the game, so that I can continue where I stopped.
2. As a learner, I want my results saved automatically after a level, so that I never have to press a save button.
3. As a learner, I want my progress to stay on this device without an account or internet, so that I can play at home offline.
4. As a parent, I want a saved game not to break if the phone switches off during a save, so that my child's progress is not lost.

### Name entry
5. As a first-time player, I want to type my name the first time I tap Play, so that the game feels like mine.
6. As a player, I want the name to accept 1–12 letters, numbers or spaces, so that I can type a short real name.
7. As a player, I want the phone's own keyboard to appear, so that typing works like other apps.
8. As a returning player, I want Play to skip the name step, so that I reach the map quickly.
9. As a player, I want to change my name later in Settings, so that a typo is not permanent.

### Landform Map
10. As a learner, I want one connected map with Mountains top left, Hills top right, Plains bottom left and Plateaus bottom right, so that I can see all four landforms of the game.
11. As a learner, I want Mountains to be the only unlocked region at the start, so that I know where to begin.
12. As a learner, I want the unlocked region to animate (blowing snow, a drifting cloud, a circling eagle), so that it feels alive and inviting.
13. As a learner, I want locked regions grey with a padlock and no animation, so that I can tell they are not ready yet.
14. As a learner, I want tapping a locked region to show a guide such as "Finish Mountains first" and keep me on the map, so that I understand what to do.
15. As a learner, I want to see my star progress for each region (for example 0/45), so that I know how far I am.
16. As a learner, I want my name shown on the map, so that the game greets me.
17. As a learner, I want a Back button in the top-left corner, and the Android Back button, to return to the Main Menu, so that I am never stuck.

### Difficulty and level selection
18. As a learner, I want a separate screen after tapping Mountains with Easy, Normal and Hard cards, so that I can choose a difficulty.
19. As a learner, I want each card to show its stars (0/9, 0/15, 0/21), so that I can track progress per difficulty.
20. As a learner, I want Normal and Hard greyed with a padlock and "Clear all Easy levels" or "Clear all Normal levels", so that I know how to unlock them.
21. As a learner, I want a level trail with one stone per level showing its stars, so that I can pick a level.
22. As a learner, I want only the next playable level and earlier ones to be open, so that I play in order as the manuscript requires.
23. As a learner, I want Back on these screens to return one step, so that I can change my mind.

### Lesson Screen
24. As a learner, I want a short lesson before the level with the landform's name and picture, three or four short sentences, and the level's rocks and minerals with pictures and names, so that I know what I will find.
25. As a learner, I want the level's goals shown on the lesson ("Collect 5/5", "Finish before time runs out"), so that I know what earns three stars.
26. As a learner, I want Start to begin the level and Back to return to the level trail, so that I control when play begins.
27. As a teacher, I want lesson and item text marked as draft until it is reviewed, so that unchecked content is not mistaken for final.

### Playing the level
28. As a learner, I want large left and right arrows at the bottom left and a large jump button at the bottom right, so that I can play comfortably with my thumbs.
29. As a learner, I want to hold an arrow to walk, so that movement is simple.
30. As a learner, I want to tap jump to jump and tap again in the air for a slightly lower double jump with a dust puff, so that I can recover a late jump.
31. As a learner, I want a visible 2-minute countdown at the top centre, so that I know how much time is left.
32. As a learner, I want a collectibles counter at the top left (for example 2/5), so that I know how many I have found.
33. As a learner, I want to collect an item just by touching it, so that I do not need extra buttons.
34. As a learner, I want the first time I ever find a kind of rock or mineral to pause the game and timer and show "You found an item!" with its picture, name and short description, so that I can read without losing time.
35. As a learner, I want later finds of the same kind to play only a chime and sparkle, so that the pop-up does not interrupt me again.
36. As a learner, I want touching a spike trap or falling into the pit to return me to the last checkpoint with my items kept and the timer still running, so that a mistake costs time, not everything.
37. As a learner, I want a checkpoint flag in the middle of the level, so that I do not have to restart from the beginning after a mistake.
38. As a learner, I want a pause button at the top right with Resume, Retry and Back to Main Menu, so that I can stop when someone calls me.
39. As a learner, I want "Time's up!" with Retry or Back to Main Menu when the timer reaches zero, so that I can try again.
40. As a learner, I want Retry to start the level again from its lesson, so that the flow matches the manuscript.
41. As a learner, I want reaching the cave entrance with the flag to complete the level, so that I know where the level ends.
42. As a learner, I want the level to feel like a mountain trail, with boulders and ledges to jump and the same Cave Spelunker explorer, so that it matches the start screen.
43. As a learner, I want a calm mountain music loop and a light-breeze ambience for Easy, so that the level sounds like the mountains.
44. As a learner, I want the Settings volume sliders to control the level music, ambience and sound effects, so that I can play quietly.

### Result Screen and progress
45. As a learner, I want 1 star for 0–40% of the items, 2 stars for 41–99%, and 3 stars for all of them, so that collecting is rewarded.
46. As a learner, I want stars to pop in one at a time with a chime, so that the result feels rewarding.
47. As a learner, I want to see the items I found in a row, with missed ones as grey outlines, so that I know what I missed.
48. As a learner, I want the goals shown with ticks and two or three review sentences about the landform, so that the level ends with a short review.
49. As a learner, I want Replay, Continue and Back to Map buttons, so that I choose what to do next.
50. As a learner, I want Continue to go to the next available level, so that I can keep playing in order.
51. As a learner, I want clearing a level to count even with 1 star, so that I am never blocked by stars.
52. As a learner, I want the stars and found items to appear on the map, difficulty cards and level trail afterwards, so that my progress is visible.

### Quality
53. As a learner on a 2 GB Android phone, I want the level to run smoothly, so that it is playable on the target devices.
54. As a learner, I want everything to stay landscape and pixel-sharp like the menu, so that the game looks consistent.
55. As the developer, I want every line of new code to carry a short plain-English comment (D-033), so that the code is easy to understand for the capstone.

## Implementation Decisions

- **One rules core, separate from Unity.** The rules live in a small plain-C# module with no dependency on Unity's engine types, in its own assembly definition so it can be tested in isolation. It has three parts:
  - **Star rule:** stars from collected and total collectibles: 0–40% → 1, 41–99% → 2, 100% → 3. With 5 items: 0–2 → 1, 3–4 → 2, 5 → 3. A cleared level always has at least 1 star.
  - **Level run:** the state of one attempt. Inputs: time passing, an item touched (with its kind), a hazard touched, a checkpoint reached, pause and resume, pop-up closed, goal reached, retry. Outputs: remaining time, collected count, current respawn point, whether a first-find pop-up is due, and an outcome (playing, paused, pop-up open, time up, cleared with stars). Time does not pass while paused or while a pop-up is open. A hazard keeps the collected items and the running timer and moves the player to the last checkpoint (or the start, if none was reached). Each collectible in a level counts once.
  - **Player progress:** player name, unlocked episodes, cleared levels and the best stars per level (a replay never lowers them, D-045), and the set of item kinds ever found. Rules: episodes unlock in the order Mountains, Hills, Plains, Plateaus, with Mountains unlocked from the start; Normal unlocks when all 3 Easy levels of that episode are cleared; Hard when all 5 Normal are cleared; within a difficulty only the first uncleared level and earlier ones are available; unlocking depends on clearing, never on stars.
- **Saving.** Progress is stored as one JSON file in the app's persistent data folder, written to a temporary file first and then swapped in, so an interrupted save cannot corrupt the previous one. A missing or unreadable file starts a new profile. Volume settings stay in the existing device preferences (they are not progress). Saving happens after each level result and after name entry; loading happens once at launch, before the Main Menu (LG-26).
- **Level content as data.** Each level is described by data: episode, difficulty, level number, time limit (Easy 120 s), the lesson (title, sentences, picture), its collectibles (kind and position), checkpoint, hazards and goal. Item kinds (name, picture, short description, draft flag) are shared across levels so "first find" works across the game. Mountains Easy 1 uses granite, quartz, feldspar and mica (5 pickups; one kind appears twice), all marked "draft — needs teacher review" (D-043, Q-04 open).
- **Scenes.** New scenes follow the existing builder approach (a menu command creates each scene and adds it to the build list): Landform Map (the scene Play already looks for), Difficulty Selection, Level Selection, Lesson, the Mountains Easy 1 level, and Result. Name entry is a panel shown from the Main Menu's Play before the map loads. Scene changes use the existing screen fader; menu-style screens reuse the existing pixel UI approach (480×270 layout, whole-number UI zoom, palette colours, click sound).
- **Text.** Screens with typed or longer text use a bundled Open Font License pixel font (proposed Pixelify Sans) through TextMeshPro, with its licence file kept in the project. Its licence must be confirmed before it is added; if it cannot be, pick another OFL pixel font and note it in the vault.
- **Level play.** A side view with a camera following the explorer and pixel-perfect rendering like the start screen. Explorer movement: hold to walk, jump plus one lower air jump, collision with ground and ledges; touch controls are on-screen buttons that also accept keyboard arrows and space in the Editor. The level is 2–3 screens wide, finishable in about 90 s, with 5 collectibles, a mid-level checkpoint flag, one pit, one spike trap, boulders and ledges, and a cave entrance with a flag as the goal (D-043). Art is new pixel art in the LandGrand palette made with the project's art script approach; the explorer reuses the Cave Spelunker sprites, with jump frames added if needed.
- **Audio.** A Mountains music loop (original, made with the project's music script approach, quieter than the menu) and a Mountains-Easy light-breeze ambience; ambience sets are planned per landform and difficulty (D-042), but only this one is in scope. All audio respects the Settings volumes.
- **Main Menu change.** Play keeps its current behaviour (open the Landform Map if it is in the build), with the first-time name panel in front of it. Settings gains a name field. Tutorial page 1 text gains the double jump.
- **Code comments.** Every line of new code gets a concise basic-English comment (D-033).

## Testing Decisions

- **What a good test is here:** it drives the rules core only through its public inputs and checks its public outputs (stars, remaining time, outcome, availability, saved and loaded values). It never checks private fields or call order, so the core can be refactored freely.
- **Tested module:** the rules core only, with Unity Test Framework EditMode tests in a separate test assembly. Cases to cover:
  - Star rule boundaries: 0/5, 2/5, 3/5, 4/5, 5/5, and 40%, 41%, 99%, 100% on larger totals.
  - Level run: the timer counts down and reaches time up; time stops while paused and while a pop-up is open; a first find opens a pop-up, a repeat kind does not, a kind found in an earlier session does not; a hazard returns to the last checkpoint (or start), keeps items and keeps the timer; the same pickup cannot count twice; reaching the goal clears with the right stars; retry resets the attempt.
  - Player progress: Mountains is the only episode unlocked at first; level order inside a difficulty; Normal and Hard unlock rules; unlocking ignores stars; a worse replay keeps the best stars; save then load gives equal progress; a missing or corrupt save file gives a fresh profile.
- **Not automated:** scenes, art, animation, audio, touch feel and layout. They are checked by playing in the Unity Editor (Game view at 1920×1080) and, when possible, on an Android device.
- **Prior art:** none yet; the project has no tests. These are the first, and they set the pattern for later levels.

## Out of Scope

- All other levels (Mountains Easy 2–3, Normal, Hard) and the Hills, Plains and Plateaus episodes, beyond showing them locked on the map.
- The Landform Description Screen and the Completion Screen (manuscript Fig. 4E / flowchart Part 5).
- Ambience sets other than Mountains Easy, and music for other landforms.
- A health bar (only if panelists ask, D-040).
- Final, teacher-validated lesson and item content (Q-04).
- Several player profiles, progress reset, and cloud or cross-device saving.
- Redrawing the manuscript figures in draw.io (the user does that from the version 2 file).

## Further Notes

- **Replays (D-045, user confirmed 2026-11-13):** a level keeps the best stars ever earned there, and every item kind ever found stays found. A worse replay never lowers saved progress.
- **Open question Q-03:** what "required episode levels" means for unlocking the next episode. Not needed for this slice, because no episode completion happens yet.
- **Manuscript rule (D-036):** any behaviour not in the final manuscript or in D-037 to D-044 must be raised with the user before it is built.
- Pixel-perfect notes from the Main Menu work apply: the Game view should use an even resolution such as 1920×1080, and UI zoom must follow the camera's whole-number zoom.
- Context for the next steps: `/to-tickets` should split this into thin vertical slices (for example: rules core with tests; save and load; name entry; Landform Map; difficulty and level selection; lesson; level movement and hazards; pick-ups and pop-up; HUD, timer and pause; Result Screen; audio), each declaring what blocks it.
