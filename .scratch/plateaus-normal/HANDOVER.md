# Plateaus Normal 1–5 handover

Built on 2026-11-24 with Unity 6000.6.9f1 from the approved D-073 spec and bot-checked layouts. The five trails reuse the existing platform, sand, bridge, updraft, current, float, ice, and herd rules. All five use `amb_canyon_normal`.

## Verification

| Ticket | EditMode results | Coverage added |
| --- | --- | --- |
| 01 | 539 passed | 15 cases for appended IDs, ulexite, and retained shelf memberships |
| 02 | 558 passed | 19 cases for level data, progression, and badges |
| 03 | 563 passed | 5 cases for theme defaults, moving art placement, and SaltFlat art |
| 04 | 563 passed | Full rerun |

Each of the six builders exited successfully in ticket 03 and again in ticket 04. No project script errors or missing-asset messages appeared in the successful builder logs. Unity logged a licensing refresh error during local startup and continued with its local license. The initial sandboxed test could not initialize licensing, so the verified runs used the local license outside the sandbox.

The three approved generators ran in isolated copies. Both passes in each verification round reproduced all 91 PNG and WAV files byte-for-byte against the project originals, including previews. The original asset files and scripts match the baseline.

The scene audit checked the five goal sprites and positions, all 30 backdrop layers, shared ambience, the four cable strips and car sprites, and Grand Canyon prop clearance along the currents. The 48 earlier gameplay scenes matched `e1bbd09` after normalizing Unity file IDs and disregarding the zero art offset and unused ulexite picture entry. The retained earlier scene files match the baseline byte-for-byte. The shared Lesson and Profile scenes contain the new pictures.

The standards review found no hard violations. The missing-cable check now stops the level builder with the generator command. The spec review found no actionable mismatches. The independent Claude review described in D-073 remains pending.

## Layout numbers and goals

Every authored coordinate, width, count, speed, and timing used by the five layouts matches the checked layout. The start is `(40, 0)`, checkpoints are `2400` and `4800`, the goal is at `7040`, and the limit is `180 s`. The Plateaus shelf is 24 kinds and the full catalog is 64 kinds as specified. Monument Valley and Mount Roraima retain other high pickups alongside the one secret marked in each layout.

All five goal sprites have visible pixels on the bottom row. Their offset stays at `(24, 24)` relative to the goal line and ground. The batch scene audit checked the positions. Visual alignment still needs a play check.

## Decorations moved more than 64 px

The existing prop-fit checks keep whole sprites clear of the trail objects. Grand Canyon also uses the existing option to clear the full current zones. Coordinates are horizontal level pixels.

| Trail | Prop | Planned x | Built x | Distance |
| --- | --- | ---: | ---: | ---: |
| Normal 1 | `fynbos` | 2140 | 2207 | 67 px |
| Normal 1 | `fynbos` | 3700 | 3807 | 107 px |
| Normal 1 | `fynbos` | 5980 | 6107 | 127 px |
| Normal 1 | `fynbos` | 6100 | 6167 | 67 px |
| Normal 3 | `pinyon_pine` | 700 | 591 | 109 px |
| Normal 3 | `pinyon_pine` | 5900 | 5791 | 109 px |
| Normal 3 | `raven` | 2000 | 1893 | 107 px |
| Normal 5 | `salt_cone` | 800 | 909 | 109 px |
| Normal 5 | `salt_cone` | 4100 | 4309 | 209 px |
| Normal 5 | `salt_cone` | 6200 | 6373 | 173 px |
| Normal 5 | `flamingo` | 6200 | 6371 | 171 px |

## Changes not in the spec

None. Decoration moves use the clearance rule in section 8. The six builders produced the required Unity import metadata and scene registration.

## Checks pending in the Editor

The manual checklist below covers the unlock and result flow, the appearance and movement of the new art, the sounds, player timing, and older-level play. The bot times of 83.8–98.4 s are from the approved layout checks. The batch runs did not time a human playthrough, listen to audio in play, or test on Android. Teacher review of draft lesson and item text is still open.

## Play checklist for the user

- [x] **Unlock** — Finish Plateaus Easy 3 and open Plateaus Normal 1.
- [x] **N1 Table Mountain** — Check the hanging cable cars and use them to cross the gaps, including the first safe pool. Listen for the soft landing clunk and open the quartzite card.
- [x] **N2 Monument Valley** — Check the red mesas and slower walking on sand. Cross both crumbling bridges and watch them return. Ride the updraft to the hematite ledge.
- [x] **N3 Grand Canyon** — Check the layered walls and river pits. Verify the first current pushes forward and open the schist card.
- [x] **N4 Mount Roraima** — Cross the summit pools on the floats and use the high updraft to reach the secret quartz.
- [x] **N5 Salar de Uyuni** — Check that the white salt crust slides like ice. The llamas should walk, hum softly, and nudge the explorer without harm. Open the ulexite card.
- [x] **Length** — Aim for about 1.5 minutes per level. The bot took 83.8–98.4 s and the timer allows 180 s.
- [x] **Profile** — Confirm 24 Plateaus kinds, Table Top after N1, and Canyon Explorer after N3.
- [x] **Regression** — Check the appearance and movement of Plains Normal 5 boats, Plains Hard 2 herds, Plains Hard 4 frost, and Plateaus Easy 3 updrafts.

## Rebuild commands

With the Editor closed, run one command for each method listed below. In the Editor, use **LandGrand > Build Levels** if Unity shows stale play scenes.

```powershell
unity run . --no-tail -- -executeMethod LevelBuilder.BuildAll -nographics
unity run . --no-tail -- -executeMethod LessonBuilder.Build -nographics
unity run . --no-tail -- -executeMethod SelectionBuilder.Build -nographics
unity run . --no-tail -- -executeMethod LandformMapBuilder.Build -nographics
unity run . --no-tail -- -executeMethod LandformDescriptionBuilder.Build -nographics
unity run . --no-tail -- -executeMethod ProfileBuilder.Build -nographics
```

Batch logs and the NUnit results are under `Logs/PlateausNormal/`.

## Scene files built

Paths in this list are relative to the Unity project. The builders saved 53 play scenes and six shared scenes.

- `Assets/Scenes/MountainsEasy1.unity`
- `Assets/Scenes/MountainsEasy2.unity`
- `Assets/Scenes/MountainsEasy3.unity`
- `Assets/Scenes/MountainsNormal1.unity`
- `Assets/Scenes/MountainsNormal2.unity`
- `Assets/Scenes/MountainsNormal3.unity`
- `Assets/Scenes/MountainsNormal4.unity`
- `Assets/Scenes/MountainsNormal5.unity`
- `Assets/Scenes/MountainsHard1.unity`
- `Assets/Scenes/MountainsHard2.unity`
- `Assets/Scenes/MountainsHard3.unity`
- `Assets/Scenes/MountainsHard4.unity`
- `Assets/Scenes/MountainsHard5.unity`
- `Assets/Scenes/MountainsHard6.unity`
- `Assets/Scenes/MountainsHard7.unity`
- `Assets/Scenes/HillsEasy1.unity`
- `Assets/Scenes/HillsEasy2.unity`
- `Assets/Scenes/HillsEasy3.unity`
- `Assets/Scenes/HillsNormal1.unity`
- `Assets/Scenes/HillsNormal2.unity`
- `Assets/Scenes/HillsNormal3.unity`
- `Assets/Scenes/HillsNormal4.unity`
- `Assets/Scenes/HillsNormal5.unity`
- `Assets/Scenes/HillsHard1.unity`
- `Assets/Scenes/HillsHard2.unity`
- `Assets/Scenes/HillsHard3.unity`
- `Assets/Scenes/HillsHard4.unity`
- `Assets/Scenes/HillsHard5.unity`
- `Assets/Scenes/HillsHard6.unity`
- `Assets/Scenes/HillsHard7.unity`
- `Assets/Scenes/PlainsEasy1.unity`
- `Assets/Scenes/PlainsEasy2.unity`
- `Assets/Scenes/PlainsEasy3.unity`
- `Assets/Scenes/PlainsNormal1.unity`
- `Assets/Scenes/PlainsNormal2.unity`
- `Assets/Scenes/PlainsNormal3.unity`
- `Assets/Scenes/PlainsNormal4.unity`
- `Assets/Scenes/PlainsNormal5.unity`
- `Assets/Scenes/PlainsHard1.unity`
- `Assets/Scenes/PlainsHard2.unity`
- `Assets/Scenes/PlainsHard3.unity`
- `Assets/Scenes/PlainsHard4.unity`
- `Assets/Scenes/PlainsHard5.unity`
- `Assets/Scenes/PlainsHard6.unity`
- `Assets/Scenes/PlainsHard7.unity`
- `Assets/Scenes/PlateausEasy1.unity`
- `Assets/Scenes/PlateausEasy2.unity`
- `Assets/Scenes/PlateausEasy3.unity`
- `Assets/Scenes/PlateausNormal1.unity`
- `Assets/Scenes/PlateausNormal2.unity`
- `Assets/Scenes/PlateausNormal3.unity`
- `Assets/Scenes/PlateausNormal4.unity`
- `Assets/Scenes/PlateausNormal5.unity`
- `Assets/Scenes/Lesson.unity`
- `Assets/Scenes/DifficultySelection.unity`
- `Assets/Scenes/LevelSelection.unity`
- `Assets/Scenes/LandformMap.unity`
- `Assets/Scenes/LandformDescription.unity`
- `Assets/Scenes/Profile.unity`

## Files added or changed

- `.scratch/plateaus-normal/HANDOVER.md`
- `.scratch/plateaus-normal/issues/01-kinds-and-themes.md`
- `.scratch/plateaus-normal/issues/02-plateaus-normal-level-data.md`
- `.scratch/plateaus-normal/issues/03-themes-cable-cars-llamas-props-build.md`
- `.scratch/plateaus-normal/issues/04-final-check-and-handover.md`
- `Assets/Art/Items/Sprites/ulexite.png.meta`
- `Assets/Art/Lesson/Sprites/lesson_grand_canyon.png.meta`
- `Assets/Art/Lesson/Sprites/lesson_monument_valley.png.meta`
- `Assets/Art/Lesson/Sprites/lesson_roraima.png.meta`
- `Assets/Art/Lesson/Sprites/lesson_table_mountain.png.meta`
- `Assets/Art/Lesson/Sprites/lesson_uyuni.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/CanyonRim.meta`
- `Assets/Art/Level/Sprites/Backdrop/CanyonRim/L0.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/CanyonRim/L1.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/CanyonRim/L2.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/CanyonRim/L3.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/CanyonRim/L4.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/CanyonRim/L5.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/MesaValley.meta`
- `Assets/Art/Level/Sprites/Backdrop/MesaValley/L0.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/MesaValley/L1.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/MesaValley/L2.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/MesaValley/L3.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/MesaValley/L4.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/MesaValley/L5.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltFlat.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltFlat/L0.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltFlat/L1.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltFlat/L2.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltFlat/L3.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltFlat/L4.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltFlat/L5.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/TableTop.meta`
- `Assets/Art/Level/Sprites/Backdrop/TableTop/L0.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/TableTop/L1.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/TableTop/L2.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/TableTop/L3.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/TableTop/L4.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/TableTop/L5.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/TepuiTop.meta`
- `Assets/Art/Level/Sprites/Backdrop/TepuiTop/L0.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/TepuiTop/L1.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/TepuiTop/L2.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/TepuiTop/L3.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/TepuiTop/L4.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/TepuiTop/L5.png.meta`
- `Assets/Art/Level/Sprites/Props/dassie_00.png.meta`
- `Assets/Art/Level/Sprites/Props/dassie_01.png.meta`
- `Assets/Art/Level/Sprites/Props/flamingo_00.png.meta`
- `Assets/Art/Level/Sprites/Props/flamingo_01.png.meta`
- `Assets/Art/Level/Sprites/Props/fynbos_00.png.meta`
- `Assets/Art/Level/Sprites/Props/fynbos_01.png.meta`
- `Assets/Art/Level/Sprites/Props/pinyon_pine.png.meta`
- `Assets/Art/Level/Sprites/Props/pitcher_plant_tepui.png.meta`
- `Assets/Art/Level/Sprites/Props/raven_00.png.meta`
- `Assets/Art/Level/Sprites/Props/raven_01.png.meta`
- `Assets/Art/Level/Sprites/Props/sagebrush.png.meta`
- `Assets/Art/Level/Sprites/Props/salt_cone.png.meta`
- `Assets/Art/Level/Sprites/Props/tepui_mist_00.png.meta`
- `Assets/Art/Level/Sprites/Props/tepui_mist_01.png.meta`
- `Assets/Art/Level/Sprites/boulder_big_canyon.png.meta`
- `Assets/Art/Level/Sprites/boulder_big_mesa.png.meta`
- `Assets/Art/Level/Sprites/boulder_big_saltflat.png.meta`
- `Assets/Art/Level/Sprites/boulder_big_tabletop.png.meta`
- `Assets/Art/Level/Sprites/boulder_big_tepui.png.meta`
- `Assets/Art/Level/Sprites/boulder_small_canyon.png.meta`
- `Assets/Art/Level/Sprites/boulder_small_mesa.png.meta`
- `Assets/Art/Level/Sprites/boulder_small_saltflat.png.meta`
- `Assets/Art/Level/Sprites/boulder_small_tabletop.png.meta`
- `Assets/Art/Level/Sprites/boulder_small_tepui.png.meta`
- `Assets/Art/Level/Sprites/cable_car.png.meta`
- `Assets/Art/Level/Sprites/cable_line.png.meta`
- `Assets/Art/Level/Sprites/cable_station.png.meta`
- `Assets/Art/Level/Sprites/canyon_lookout.png.meta`
- `Assets/Art/Level/Sprites/canyon_soil.png.meta`
- `Assets/Art/Level/Sprites/canyon_top.png.meta`
- `Assets/Art/Level/Sprites/explorer_camp.png.meta`
- `Assets/Art/Level/Sprites/ledge_canyon.png.meta`
- `Assets/Art/Level/Sprites/ledge_mesa.png.meta`
- `Assets/Art/Level/Sprites/ledge_saltflat.png.meta`
- `Assets/Art/Level/Sprites/ledge_tabletop.png.meta`
- `Assets/Art/Level/Sprites/ledge_tepui.png.meta`
- `Assets/Art/Level/Sprites/llama_00.png.meta`
- `Assets/Art/Level/Sprites/llama_01.png.meta`
- `Assets/Art/Level/Sprites/llama_02.png.meta`
- `Assets/Art/Level/Sprites/mesa_soil.png.meta`
- `Assets/Art/Level/Sprites/mesa_top.png.meta`
- `Assets/Art/Level/Sprites/salt_crust_top.png.meta`
- `Assets/Art/Level/Sprites/salt_hotel.png.meta`
- `Assets/Art/Level/Sprites/saltflat_soil.png.meta`
- `Assets/Art/Level/Sprites/saltflat_top.png.meta`
- `Assets/Art/Level/Sprites/tabletop_soil.png.meta`
- `Assets/Art/Level/Sprites/tabletop_top.png.meta`
- `Assets/Art/Level/Sprites/tepui_soil.png.meta`
- `Assets/Art/Level/Sprites/tepui_top.png.meta`
- `Assets/Art/Level/Sprites/trading_post.png.meta`
- `Assets/Audio/sfx_cable_car.wav.meta`
- `Assets/Audio/sfx_llama.wav.meta`
- `Assets/Editor/LevelObjects.cs`
- `Assets/Editor/LevelThemes.cs`
- `Assets/Editor/PixelSceneKit.cs`
- `Assets/Editor/PlainsHardProps.cs`
- `Assets/Editor/PlainsNormalProps.cs`
- `Assets/Editor/ThemeArtDefaultsTests.cs`
- `Assets/Scenes/Lesson.unity`
- `Assets/Scenes/PlateausNormal1.unity`
- `Assets/Scenes/PlateausNormal1.unity.meta`
- `Assets/Scenes/PlateausNormal2.unity`
- `Assets/Scenes/PlateausNormal2.unity.meta`
- `Assets/Scenes/PlateausNormal3.unity`
- `Assets/Scenes/PlateausNormal3.unity.meta`
- `Assets/Scenes/PlateausNormal4.unity`
- `Assets/Scenes/PlateausNormal4.unity.meta`
- `Assets/Scenes/PlateausNormal5.unity`
- `Assets/Scenes/PlateausNormal5.unity.meta`
- `Assets/Scenes/Profile.unity`
- `Assets/Scripts/Play/MovingPlatformView.cs`
- `Assets/Scripts/Rules/LevelData.cs`
- `Assets/Scripts/Rules/LevelLayout.cs`
- `Assets/Tests/EditMode/CollectionTests.cs`
- `Assets/Tests/EditMode/HillsHardLevelsTests.cs`
- `Assets/Tests/EditMode/HillsNormalLevelsTests.cs`
- `Assets/Tests/EditMode/LevelCatalogTests.cs`
- `Assets/Tests/EditMode/PlateausEasyLevelsTests.cs`
- `Assets/Tests/EditMode/PlateausNormalLevelsTests.cs`
- `Assets/Tests/EditMode/PlateausNormalLevelsTests.cs.meta`
- `Assets/Tests/EditMode/PlateausRulesTests.cs`
- `CONTEXT.md`
- `ProjectSettings/EditorBuildSettings.asset`
