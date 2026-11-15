---
status: ready-for-agent
created: 2026-11-15
sources:
  - Vault Project/Gameplay Design.md (D-039 to D-055)
  - Vault Decisions/Decision Register.md (D-036, D-048, D-050, D-051, D-055)
  - .scratch/mountains-easy-1/spec.md (the slice this extends)
  - CONTEXT.md (Lesson topic, Level theme, Level layout, Review pickup)
---

# Spec: Mountains Easy 2 and Easy 3, and one data-driven level builder

## Problem Statement

Mountains Easy 1 is playable, but the Easy trail has three stones and only the first one leads anywhere: levels 2 and 3 open a "Coming soon" lesson. A learner cannot finish the Easy difficulty, so Normal never unlocks (LG-31) and the Mountains shelf of the Collection stays at four item kinds. The level builder is also hard-wired to Easy 1 (scene path, width, boulders, ledges and five `LevelCatalog.Find(Mountains, Easy, 1)` calls), so every new level would be a copy-paste of 375 lines.

## Solution

Add two new Mountains Easy levels, each with its own lesson topic, three new item kinds, a level theme and a harder layout, and make one builder produce every level from level data.

- **Easy 2, "Volcano Trail"**: lesson topic volcanic mountains. New kinds andesite, pumice, obsidian; review pickup quartz. Dark volcanic theme with steam vents that launch the explorer (D-055); goal at a lava-rock arch.
- **Easy 3, "Folded Peaks"**: lesson topic folded mountains (heat and squeezing change rock). New kinds slate, marble, garnet; review pickup mica. Grey folded-rock theme with a stepped climb; goal at a summit stone pile.
- Both levels keep the Easy rules: 5 pickups, 1 checkpoint, 120 s, star bands 0–2 / 3–4 / 5 (D-039), Easy pit rule (D-048), first-find pop-up (D-040), replays keep the best (D-045).
- Excitement comes from level design used by popular platformers, not new scoring rules: teach-test-twist pacing, one secret spot per level, a level start banner, living props, and (Easy 2 only) the steam vent launch.

All lesson, item and review text is draft, marked "Draft — needs teacher review" (D-043, Q-04 open). Text follows D-050 (Micro 5 22 px headings, buttons, numbers, HUD) and D-051 (Andika for sentences, descriptions, item names in boxes).

## Content (all draft)

### Easy 2 — Volcanic Mountains

Lesson heading "Level 2: Volcanic Mountains"; new lesson picture `lesson_volcano` (a volcano with a lava flow cooling to grey rock).

1. Some mountains are volcanoes.
2. Hot melted rock, called lava, flows out.
3. Cooled lava turns into rock like andesite.
4. Mayon and Apo are Philippine volcanoes.

*(Shortened 2026-11-15 during the build: the first wording measured 308–334 px in Andika at UI size 15, wider than the 312 px lesson line; these measure at most 298 px, the width of Easy 1's longest line.)*

Review: "Volcanoes are mountains made by lava. Lava cools into rocks like andesite, pumice and obsidian. Fast cooling makes glassy obsidian."

### Easy 3 — Folded Mountains

Lesson heading "Level 3: Folded Mountains"; new lesson picture `lesson_folds` (rock layers bent into folds under a peak).

1. Land pushed together makes mountains.
2. Rock bends into folds, like a pushed rug.
3. Heat and squeezing turn old rock into new.
4. Mud rock becomes slate this way.

*(Shortened 2026-11-15 for the same reason (350–387 px before). Marble's limestone origin stays in its item description and pop-up.)*

Review: "Folded mountains form when land is pushed together. Heat and squeezing change rocks, like mud rock into slate. Garnet crystals can grow inside changed rock."

### New item kinds (home landform Mountains, all draft)

| Id | Name | Rock/Mineral | Description |
| --- | --- | --- | --- |
| andesite | Andesite | Rock | A grey rock made when lava cools. Many Philippine volcanoes are made of it. |
| pumice | Pumice | Rock | A light rock full of tiny holes from gas bubbles. Some pumice can float on water. |
| obsidian | Obsidian | Rock | A shiny black rock like glass. It forms when lava cools very fast. |
| slate | Slate | Rock | A dark rock that splits into flat, smooth sheets. It forms when mud rock is squeezed. |
| marble | Marble | Rock | A smooth rock, often white. It forms when limestone is heated and squeezed. |
| garnet | Garnet | Mineral | A dark red mineral that grows as small crystals inside squeezed rock. |

The Mountains shelf grows from 4 to 10 kinds. Basalt is deliberately not used: its home landform is to be Plateaus. Limestone and sandstone are kept for Hills and Plains.

### Pickups per level (5 each)

- Easy 2: andesite, quartz (review pickup), obsidian, pumice (secret spot), andesite.
- Easy 3: slate, mica (review pickup), marble, garnet (secret spot), slate.

## Level layouts (beats; exact x positions set in the ticket)

Both levels are about 5400 px wide (Easy 1 is 4800 px), with the checkpoint near the middle. Pacing is teach, test, twist: each obstacle appears first in a safe form, then with risk, then combined with another.

**Easy 2 — Volcano Trail.** Safe start → pit 1 (single jump) → steam vent introduced over flat ground (safe to try) → spike 1 → checkpoint → vent launch up to a high ledge holding pumice (secret spot) → pit 2 between two ledges → spike 2 beside a ledge → lava-rock arch with flag (goal). 2 pits, 2 spikes.

**Easy 3 — Folded Peaks.** Stepped climb up folded rock ledges → pit 1 → spike 1 → checkpoint → wide pit (64 px) that needs the double jump → spike pair → high off-path ledge with garnet (secret spot, reached by backtracking up the folds) → pit 3 → summit stone pile with flag (goal). 3 pits, 3 spikes. The climb must stay inside what the current camera shows; if the camera's fixed vertical framing (CameraFollow) cannot follow it, lower the climb rather than change the camera in this spec.

Both: one checkpoint, Easy pit rule (back about 24 px behind the last ground spot), spikes send to the checkpoint or start.

## Implementation Decisions

- **Level layout becomes level data.** Each level's data gains its width, ground pieces (from pits), boulders, ledges, steam vents, level theme (ground tiles, props, goal object), the level start banner name and the lesson picture. Item positions and the layout live in the same place, so the "keep LevelData.cs and LevelBuilder.cs in step" rule ends.
- **One builder for every level.** One menu command, **LandGrand > Build Mountains Levels**, builds every level in the level catalog into its scene (named from the existing scene name rule, e.g. `MountainsEasy2`), adds each to the build list, and sets the level identity (episode, difficulty, number) on the scene's level controller. The old Easy 1 menu item goes away. Easy 1 rebuilt this way must play and look the same as now.
- **Level theme.** Three themes: Mountains meadow (Easy 1, unchanged), Volcanic (dark rock ground with glowing lava cracks, puffing vents, lava-rock arch goal), Folded rock (grey layered ground with folded bands, drifting snow near the top, a startled eagle, summit stone pile goal). All share the six Mountains backdrop layers and parallax (D-049), the Mountains music and the Easy light-breeze ambience (D-042 is per difficulty).
- **Steam vent (D-055).** A level object, Easy 2 only for now. It puffs on a fixed cycle (about every 2 s, with a short warning wisp before the puff). Standing on it during a puff launches the explorer upward higher than a double jump (launch speed 480, about 128 px; a double jump reaches about 76 px); the explorer can still double jump after. The secret ledge starts 16 px right of the vent, so an explorer holding right rises above it before passing under it (checked with a physics bot on 2026-11-15). It never hurts, never moves the explorer back, and does not touch the timer, items or stars. Its launch is part of the rules core so it can be tested.
- **Level start banner.** On entering a level, its name ("Volcano Trail", "Folded Peaks"; Easy 1 gets "Mountain Trail") slides in for about 1.5 s in Micro 5 22 px. Input and the timer start when the banner leaves. The HUD level name keeps the "Mountains Easy 2" form (D-049).
- **Secret spot.** One pickup per level sits off the main path. It is a normal pickup; it is only placement. No marker, no extra reward beyond counting toward 3 stars.
- **Living props.** Decoration only: vent puffs (Easy 2 also uses them for the launch), glowing lava cracks, drifting summit snow, an eagle that flies off when the explorer comes near. No collision, no effect on play.
- **Art.** Six new item sprites, each a hand-written draw function in the lesson art script next to the existing four. New tiles and props for the two themes in the level art script (volcanic ground, lava cracks, vent with puff frames, lava-rock arch; folded-rock ground, fold bands, stone pile, flag, snow, eagle frames), two lesson pictures, and the banner plate. LandGrand palette, even sprite sizes, pixel-perfect rules from D-054.
- **Text rules.** Banner in Micro 5 22 px; the lesson heading stays in Andika as for Easy 1 (D-051 moved it to the reading font on 2026-11-14; the first draft of this spec said Micro 5 by mistake); lesson sentences, item names in the lesson boxes, pop-up and card descriptions, and review text in Andika (D-051). Check that the four lesson sentences and the review fit their boxes at those sizes; shorten wording only with the user.
- **Profile shelf paging.** With 10 Mountains kinds, a shelf shows 5 slots per page and a page arrow on the right (the existing Next and Prev icons); a shelf with 5 or fewer kinds shows no arrow.
- **Unchanged.** Star rule, time limit, checkpoint count, Easy pit rule, first-find pop-up, saving, Collection totals (they already count every item kind), level selection and Continue flow, HUD layout.
- **Normal after Easy.** Clearing Easy 3 unlocks Normal (LG-31). Normal level 1 then shows the existing "Coming soon" lesson; no Normal content is in this spec.
- **Code comments.** Every new line keeps a basic-English comment (D-033).

## Testing Decisions

- Tested through the rules core only, by public inputs and outputs (same rule as the Easy 1 spec).
- Level catalog: Easy 1, 2 and 3 exist with 5 pickups each, one checkpoint, a goal, time limit 120 s; every pickup's item kind is in the item catalog; every new kind has home landform Mountains and the right rock/mineral flag. The existing test that expects Easy 2 to be missing is replaced.
- Collection: the Mountains shelf holds 10 kinds; found count and total follow.
- Level run: clearing Easy 2 or 3 with 5/5 gives 3 stars; a review pickup of an already-found kind gives no pop-up; a review pickup of a never-found kind does give one.
- Player progress: clearing Easy 1–3 unlocks Normal; Continue after Easy 3 goes back to the level trail.
- Steam vent: a puff while standing on the vent launches; no puff, no launch; a launch never changes time, items or respawn point.
- Not automated: layouts, themes, banner, props, art, feel, camera framing. Checked by playing each level in the Editor at 1920×1080 and, when possible, on an Android phone. Easy 1 is played again after the builder change to confirm nothing moved.

## Out of Scope

- Mountains Normal and Hard levels, and the Hills, Plains and Plateaus episodes.
- Steam vents anywhere other than Easy 2.
- New hazard types (falling rocks, moving platforms) and any change to scoring, stars, timer or unlock rules.
- Final, teacher-validated content (Q-04); episode completion and the Landform Description Screen (Q-03).
- New music or ambience sets.

## Further Notes

- **D-036:** the steam vent is a level object the manuscript does not name; record it for the panel next to Q-13. Everything else here is content and presentation.
- **Suggested tickets** (blockers first): 1) level layout as data plus one builder, Easy 1 rebuilt unchanged; 2) new item kinds, their art and the Collection shelf of 10; 3) Easy 2 content, theme art and layout; 4) steam vent rule and object; 5) Easy 3 content, theme art and layout; 6) level start banner and living props. 2 needs nothing; 3 and 5 need 1 and 2; 4 needs 1; 6 needs 1.
- **Builder to run after the tickets:** `python Tools/Art/make_lesson_art.py`, `python Tools/Art/make_level_art.py`, then **LandGrand > Build Lesson Scene** and **LandGrand > Build Mountains Levels**.
