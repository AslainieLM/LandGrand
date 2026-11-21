# Plains Easy 1–3 handover

Built on 2026-11-21 with Unity 6000.6.9f1. Central Luzon Plain, Pangasinan Salt Beds and Isabela River Plain are enabled in Build Settings.

## Verification

| Ticket | EditMode result | Net new cases |
| --- | --- | ---: |
| 01 Rules | 412 passed | 17 |
| 02 Level data | 417 passed | 5 |
| 03 Play parts and builders | 417 passed | 0 |
| 04 Final check | **417 passed, 0 failed, 0 skipped** | 0 |
| Review fixes | **423 passed, 0 failed, 0 skipped** | 6 |

- All six builders exited successfully. The logs contain zero missing assets and zero project build errors. Unity reported an access-token refresh error and then successfully updated its license. Three builder sessions also logged a cURL callback abort. Git flags trailing spaces that Unity writes for empty fields in its generated scene YAML.
- The four generators reproduced **158 files byte for byte** in a temporary copy, including all four OGG files, the WAV and three preview sheets.
- The bot reached all five pickups in each trail and finished in **56.8 s, 62.4 s and 62.2 s**. E3 used four bounces. The bot reached 128 px with a bounce and double jump. A side collision stopped it at x=595 without a bounce.
- The saved Plains scenes use a 480×270 camera, 84 px/s walk, Plains music and Easy ambience. All four E3 pile boxes match their authored positions. The three visible goal bases align with the ground.
- Profile references all 20 Plains achievement pictures and all four new item pictures. Lesson references all three Plains lesson pictures.
- The map's rice, carabao and full egret path clear the region border, label and trail. The four shared menu scenes reference the Plains sprites.
- All 30 rebuilt Mountains and Hills levels match baseline dc4f2a9 on fixed collision shapes and music and ambience references.
- The standards review found no blocking issues. The spec review caught the missing-art message for the reused Salt Beds palm. The builder now names the Hills Easy generator for that sprite.
- Raw builder logs and the final test report are in Logs/PlainsEasy-final/. The test XML is plains-04-editmode.xml.

## Changed numbers

- All gameplay and map coordinates match the supplied layouts. The user reconfirmed keeping the Farm Shed at (24, 22) so the visible base meets the ground. The original spec used (24, 24). The bottom two rows are transparent, so the approved 2 px drop puts the visible base on the ground. The x offset is 24 px and the goal trigger is at x=5240.
- The item catalog grows from 50 to 54, as specified. The Plains shelf has 10 kinds. The badge catalog grows from 25 to 35 with the ten Plains badges.
- Every Plains prop fit within 64 px of its planned position. The largest shifts were E1 rice from x=3870 to 3931 (61 px), E2 salt mound from x=3120 to 3161 (41 px), and E3 corn from x=3380 to 3422 (42 px). All E3 props clear the piles by at least 8 px.
- Fixed animals use 1.5, 2 and 2.5 s per frame where applicable. Corn cycles through 1.2, 1.5 and 1.8 frames per second. Prop placement uses the existing 90 px approach to each goal and allows at least half the planned spacing between repeats.

## Review fixes

| Item | Source | Result |
| --- | --- | --- |
| 0. Farm Shed | `Assets/Editor/LevelThemes.cs:209` | Reconfirmed `(24, 22)`. The two transparent rows at the base explain the 2 px adjustment from the original spec. |
| 1. Comments and volume | `Assets/Editor/LevelThemes.cs:450`, `Assets/Editor/LevelThemes.cs:523`, `Assets/Editor/LevelThemes.cs:594`, `Assets/Scripts/Play/StrawBounceWatcher.cs:23`, `Assets/Scripts/Play/StrawBounceWatcher.cs:59` | Named the frame time and bounce volume. The corn rate belongs to the corn caller. A single-frame prop skips the animation component. |
| 2. Shared helpers | `Assets/Editor/LevelThemes.cs:462`, `Assets/Editor/LevelThemes.cs:516`, `Assets/Editor/LevelThemes.cs:535` | Hills Hard and Plains share the theme, repeat placement and frame-loading helpers with the right generator paths. |
| 3. Fit names | `Assets/Editor/LevelThemes.cs:573`, `Assets/Editor/LevelThemes.cs:621` | Renamed the two fit helpers to reflect their use across landforms. |
| 4. Shelf comments | `Assets/Scripts/Rules/LevelData.cs:155`, `Assets/Scripts/Rules/LevelData.cs:169`, `Assets/Scripts/Rules/LevelData.cs:170`, `Assets/Scripts/Rules/LevelData.cs:171`, `Assets/Scripts/Rules/LevelData.cs:172`, `Assets/Scripts/Rules/LevelData.cs:177` | Restored the six original comments and added the Plains shelf to each. |
| 5. Glossary | `CONTEXT.md:47` | Uses the ticket wording for walking into the side. |
| 6. Rule tests | `Assets/Tests/EditMode/AchievementTests.cs:47`, `Assets/Tests/EditMode/PlainsEasyLevelsTests.cs:8`, `Assets/Tests/EditMode/PlainsEasyLevelsTests.cs:45` | Added six badge cases. The leftmost pile gets the existing 80 px safety distance and the other piles use 60 px. |
| 7. Layout default | `Assets/Scripts/Rules/LevelLayout.cs:80` | Uses the same empty-array style as the nearby lists. |

EditMode passed **423 tests, 0 failed, 0 skipped**, up from 417. Plains progress keeps landform-specific Mountains and Hills badges locked. The general badges shown on the Mountains page still count progress in Plains.

Git diff shows all seven Hills Hard scenes were regenerated. After normalizing Unity's local file IDs, document order and whitespace, the only differences from `dc4f2a9` are the shared result and pickup panels' references to 10 Plains badge pictures and four Plains item pictures from the original Plains implementation. All Hills Hard prop positions, sprites, frame order and timing match. The same comparison passed for all 30 Mountains and Hills levels.

The rebuilt Hills map and menu birds still use `Hills/bird_0` and `Hills/bird_1` at 3 frames per second, with 12 s and 15 s laps. Their map flight center is (372, 30) with radius (20, 20). The menu center is (330, 60) with radius (24, 10). This matches `dc4f2a9` in the map, both selection scenes, lesson and description.

All six builders succeeded with zero missing assets and zero project errors. The raw logs include Unity's recovered license refresh error and cURL callback aborts. See `Logs/PlainsEasy-review/` for the builder logs and `editmode.xml`.

## Source and notes

- `Assets/Scripts/Rules/StrawPile.cs` defines pile size, contact and launch.
- `Assets/Scripts/Rules/LevelLayout.cs` adds the pile list and appends three themes.
- `Assets/Scripts/Rules/LevelData.cs` adds the three layouts, draft lessons and reviews, four draft kinds and six shared kinds.
- `Assets/Scripts/Rules/LandformAudio.cs` maps the Plains music and ambience to their generator.
- `Assets/Scripts/Rules/LandformSummary.cs` adds the five draft Plains lines.
- `Assets/Scripts/Rules/Achievements.cs` appends the ten Plains badge rules.
- `Assets/Scripts/Play/StrawBounceWatcher.cs` launches the explorer, plays the effect and animates the pile.
- `Assets/Editor/LevelBuilder.cs` builds the pile visuals and solid boxes.
- `Assets/Editor/LevelThemes.cs` supplies theme art, goals and fitted props.
- `Assets/Editor/PixelSceneKit.cs` names the Plains generator for missing shared pictures.
- `Assets/Editor/LandformLife.cs` imports and loads Plains sprites and shares the bird flight setup.
- `Assets/Editor/LandformMapBuilder.cs` adds the Plains map animations.
- `Assets/Editor/SelectionBuilder.cs` adds the Plains menu animations.
- `Assets/Editor/LessonBuilder.cs` imports the Plains menu sprites before building the lesson.
- `Assets/Editor/LandformDescriptionBuilder.cs` imports the Plains menu sprites before building the review.
- `Assets/Tests/EditMode/StrawPileTests.cs` checks the edges, feet heights and launch rise.
- `Assets/Tests/EditMode/PlainsEasyLevelsTests.cs` checks trail data and pile clearance.
- `Assets/Tests/EditMode/NextPlayableTests.cs` checks the Hills finale unlock and the end of Plains Easy.
- `Assets/Tests/EditMode/LevelCatalogTests.cs` checks catalog bounds.
- `Assets/Tests/EditMode/CollectionTests.cs` checks the shelf, count and shared kinds.
- `Assets/Tests/EditMode/LandformAudioTests.cs` checks the audio row and generator names.
- `Assets/Tests/EditMode/LandformSummaryTests.cs` checks the Plains summary and Plateaus placeholder.
- `Assets/Tests/EditMode/AchievementTests.cs` checks badge order, tiers and pages.
- `Assets/Tests/EditMode/HillsHardLevelsTests.cs` keeps the order test scoped to the six Hills Hard additions.
- `CONTEXT.md` defines Straw bounce pile.
- `.scratch/plains-easy/HANDOVER.md` records the build, checks and play checklist.

## Generated scene and build files

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
- `Assets/Scenes/Lesson.unity`
- `Assets/Scenes/DifficultySelection.unity`
- `Assets/Scenes/LevelSelection.unity`
- `Assets/Scenes/LandformMap.unity`
- `Assets/Scenes/LandformDescription.unity`
- `Assets/Scenes/Profile.unity`

`ProjectSettings/EditorBuildSettings.asset` enables Plains Easy 1–3.

### Unity import records

- `Assets/Art/Items/Sprites/gypsum.png.meta`
- `Assets/Art/Items/Sprites/halite.png.meta`
- `Assets/Art/Items/Sprites/mudstone.png.meta`
- `Assets/Art/Items/Sprites/siltstone.png.meta`
- `Assets/Art/LandformMap/Sprites/Plains.meta`
- `Assets/Art/LandformMap/Sprites/Plains/carabao_0.png.meta`
- `Assets/Art/LandformMap/Sprites/Plains/carabao_1.png.meta`
- `Assets/Art/LandformMap/Sprites/Plains/egret_0.png.meta`
- `Assets/Art/LandformMap/Sprites/Plains/egret_1.png.meta`
- `Assets/Art/LandformMap/Sprites/Plains/grass_0.png.meta`
- `Assets/Art/LandformMap/Sprites/Plains/grass_1.png.meta`
- `Assets/Art/LandformMap/Sprites/Plains/rice_0.png.meta`
- `Assets/Art/LandformMap/Sprites/Plains/rice_1.png.meta`
- `Assets/Art/Lesson/Sprites/lesson_central_luzon.png.meta`
- `Assets/Art/Lesson/Sprites/lesson_isabela.png.meta`
- `Assets/Art/Lesson/Sprites/lesson_pangasinan.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RicePlain.meta`
- `Assets/Art/Level/Sprites/Backdrop/RicePlain/L0.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RicePlain/L1.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RicePlain/L2.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RicePlain/L3.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RicePlain/L4.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RicePlain/L5.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverPlain.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverPlain/L0.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverPlain/L1.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverPlain/L2.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverPlain/L3.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverPlain/L4.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverPlain/L5.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltBeds.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltBeds/L0.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltBeds/L1.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltBeds/L2.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltBeds/L3.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltBeds/L4.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/SaltBeds/L5.png.meta`
- `Assets/Art/Level/Sprites/Props/carabao_00.png.meta`
- `Assets/Art/Level/Sprites/Props/carabao_01.png.meta`
- `Assets/Art/Level/Sprites/Props/corn_stalk_00.png.meta`
- `Assets/Art/Level/Sprites/Props/corn_stalk_01.png.meta`
- `Assets/Art/Level/Sprites/Props/egret_00.png.meta`
- `Assets/Art/Level/Sprites/Props/egret_01.png.meta`
- `Assets/Art/Level/Sprites/Props/rice_tuft.png.meta`
- `Assets/Art/Level/Sprites/Props/salt_mound.png.meta`
- `Assets/Art/Level/Sprites/Props/straw_bits_00.png.meta`
- `Assets/Art/Level/Sprites/Props/straw_bits_01.png.meta`
- `Assets/Art/Level/Sprites/boulder_big_rice.png.meta`
- `Assets/Art/Level/Sprites/boulder_big_river.png.meta`
- `Assets/Art/Level/Sprites/boulder_big_salt.png.meta`
- `Assets/Art/Level/Sprites/boulder_small_rice.png.meta`
- `Assets/Art/Level/Sprites/boulder_small_river.png.meta`
- `Assets/Art/Level/Sprites/boulder_small_salt.png.meta`
- `Assets/Art/Level/Sprites/farm_shed.png.meta`
- `Assets/Art/Level/Sprites/ledge_rice.png.meta`
- `Assets/Art/Level/Sprites/ledge_river.png.meta`
- `Assets/Art/Level/Sprites/ledge_salt.png.meta`
- `Assets/Art/Level/Sprites/rice_granary.png.meta`
- `Assets/Art/Level/Sprites/rice_soil.png.meta`
- `Assets/Art/Level/Sprites/rice_top.png.meta`
- `Assets/Art/Level/Sprites/river_soil.png.meta`
- `Assets/Art/Level/Sprites/river_top.png.meta`
- `Assets/Art/Level/Sprites/salt_hut.png.meta`
- `Assets/Art/Level/Sprites/salt_soil.png.meta`
- `Assets/Art/Level/Sprites/salt_top.png.meta`
- `Assets/Art/Level/Sprites/straw_pile.png.meta`
- `Assets/Art/Level/Sprites/straw_pile_squash.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_delta_boater.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_delta_boater_locked.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_flat_star.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_flat_star_locked.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_philippine_plains.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_philippine_plains_locked.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_plain_collector.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_plain_collector_locked.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_plain_master.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_plain_master_locked.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_prairie_ranger.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_prairie_ranger_locked.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_salt_maker.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_salt_maker_locked.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_steady_plains.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_steady_plains_locked.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_straw_jumper.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_straw_jumper_locked.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_world_plains.png.meta`
- `Assets/Art/MainMenu/Sprites/Profile/badge_world_plains_locked.png.meta`
- `Assets/Audio/amb_dry_hard.ogg.meta`
- `Assets/Audio/amb_grass_easy.ogg.meta`
- `Assets/Audio/amb_wetland_normal.ogg.meta`
- `Assets/Audio/bgm_plains.ogg.meta`
- `Assets/Audio/sfx_straw_bounce.wav.meta`
- `Assets/Scenes/PlainsEasy1.unity.meta`
- `Assets/Scenes/PlainsEasy2.unity.meta`
- `Assets/Scenes/PlainsEasy3.unity.meta`
- `Assets/Scripts/Play/StrawBounceWatcher.cs.meta`
- `Assets/Scripts/Rules/StrawPile.cs.meta`
- `Assets/Tests/EditMode/PlainsEasyLevelsTests.cs.meta`
- `Assets/Tests/EditMode/StrawPileTests.cs.meta`

## Editor play checklist

Interactive Editor play, visual inspection in motion, listening to the mix, older-level playthroughs and Android device checks remain open. The lesson, kind, review and summary copy awaits teacher review.

- [ ] **Unlock.** With a completed Hills save, check the one-time Plains unlock. Check the rice, egrets and grazing carabao after it opens. Tap Plateaus and check “Finish Plains first.”
- [ ] **Menus.** Open Plains Easy 1. Check the menu egrets and grass. Clear Easy 1–3 and tap Normal to see “Coming soon.” Hard shows “Clear all Normal levels.”
- [ ] **E1 Central Luzon Plain.** Check the rice ground, Rice Granary, carabao and egrets. Listen for the Plains tune, grass and crickets. Climb the three steps to the siltstone secret.
- [ ] **E2 Pangasinan Salt Beds.** Check the salt ground, salt heaps, palms and Salt Hut. Read the halite and gypsum first-find pop-ups.
- [ ] **E3 Isabela River Plain.** Check that the first pile at x=600 is safe. Land on top for a high bounce, soft “fwump,” squash and straw bits. Walk into its side to check the solid wall. Pause during a bounce effect and resume. Reach mudstone from x=1200 and the limestone secret from x=3300 with a bounce and double jump. Check that the Farm Shed meets the ground.
- [ ] **Length.** Allow about one minute per trail. Check the shared 120 s limit.
- [ ] **After E3.** Check that Next is hidden. Check the 10-kind Plains shelf with “Found here n/10,” and the ten Plains badges with Salt Maker and Straw Jumper earned.
- [ ] **Regression.** Spot-check one Mountains level, one Hills level and the Hills badge pages.

## Rebuild

Close the LandGrand Editor. From the project root, run each builder in its own Unity batch session. Run `LevelBuilder.BuildAll`, `LessonBuilder.Build`, `SelectionBuilder.Build`, `LandformMapBuilder.Build`, `LandformDescriptionBuilder.Build` and `ProfileBuilder.Build` in that order. Use `-batchmode -projectPath . -executeMethod <method> -quit -logFile <log>`. For EditMode, use `-batchmode -projectPath . -runTests -testPlatform EditMode -testResults <xml> -logFile <log>`.
