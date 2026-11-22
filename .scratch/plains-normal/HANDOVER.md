# Plains Normal 1–5 handover

Built on 2026-11-22 with Unity 6000.6.9f1. All five Plains Normal levels are enabled in Build Settings.

## Verification

| Ticket | EditMode result | New cases |
| --- | --- | ---: |
| 01 Kinds and themes | 425 passed | 2 |
| 02 Level data | 435 passed | 10 |
| 03 Themes, boats and props | 435 passed | 0 |
| 04 Final check | **435 passed, 0 failed, 0 skipped** | 0 |
| Review fixes | **440 passed, 0 failed, 0 skipped** | 5 |

- Both passes ran all six Unity builders. The logs show zero missing assets and zero project errors. Raw Unity logs include a recovered license refresh error and cURL callback aborts. Git may flag trailing spaces from the Unity scene serializer.
- The three generators reproduced **82 outputs byte for byte** in a temporary copy on both passes. These include 78 project PNGs, the WAV and three preview sheets.
- The layout and bot data match. The rerun cleared all five trails with all seven pickups each, in **83.8, 93.0, 85.8, 85.2 and 98.4 s**. All 65 shifted-start runs reached the goal. The bot failed the Mekong crossing without boats. The Hills Normal bot times matched the 84 px/s baseline.
- The saved Plains Normal scenes use a 480×270 camera, 84 px/s walk, six backdrop layers per theme, `bgm_plains`, and `amb_wetland_normal`. The Lesson scene references all five new illustrations. Profile references peat and laterite. All 112 decorative placements clear the specified trail objects.
- All four Mekong boat decks align with y=0, use the market boat sprite, and link the bump clip. The sound scales the player's effects volume by 0.6. The view checks for a new landing after the explorer moves.
- All 33 previous playable scenes match baseline `ed2cf05` after accounting for Unity serialization, the two additional pictures in the shared pickup panels, and empty optional platform sound fields. All 18 legacy horizontal platforms match their positions, sprites, and routes in Mountains Normal 3 and 4, Mountains Hard 3, 6 and 7, and Hills Hard 6.
- Standards and spec reviews found no actionable source issues. The play checklist checks the Normal and Hard unlock milestones separately.
- Final logs and `editmode.xml` are in `Logs/PlainsNormal/final/`.

## Review fixes

1. **Landing sound.** `Assets/Scripts/Play/MovingPlatformView.cs:44` samples platform support after the explorer moves. `Assets/Scripts/Rules/PlatformLanding.cs:7` records support even when the sound is muted by a freeze or pause. `Assets/Tests/EditMode/PlatformLandingTests.cs:7` covers landing, staying, leaving and two frozen cases. The freeze tests failed before the fix and passed afterward.
2. **Shared prop helper.** `Assets/Editor/LevelThemes.cs:543` places fixed props for Hills Hard and Plains Normal. `Assets/Editor/PlainsNormalProps.cs:17` passes its own art generator and asks for full zone clearance. The seven Hills Hard and five Plains Normal scenes matched their pre-fix versions after normalizing Unity's local IDs and ignoring scene object order. Prop positions, sprite references, animation frames and frame timings all matched.
3. **Missing sound message.** `Assets/Editor/LevelThemes.cs:24` holds the generator path next to the clip name. The Floating Market entry sets it at line 219. `Assets/Editor/LevelObjects.cs:41` uses it to report a missing clip.
4. **Small clean-ups.** `Assets/Scripts/Play/MovingPlatformView.cs:31` uses the saved controller link directly. `Assets/Editor/LevelThemes.cs:529` explains why only Plains Normal clears full zones.
5. **Regression tests.** `Assets/Tests/EditMode/AchievementTests.cs:12` pins all 35 badge IDs in save order. `Assets/Tests/EditMode/PlainsNormalLevelsTests.cs:49` pins Mountains Normal 1's width, goal and empty platform list, Mountains Normal 3's platform routes, Hills Normal 1's floats and pool, and Plains Easy 3's straw piles. Platform sprite overrides live in the Editor builder, so the Rules test omits them. The saved scene audit confirmed the positions, sprites and routes of all 18 earlier horizontal platforms.
6. **Earlier implementation choices.** The section below records the five choices made in the initial Plains Normal work and their reasons.

EditMode passed **440 tests**, up from 435. All six builders ran in sequence and rebuilt 44 scenes. There were no missing assets or project errors. Unity printed a license refresh error before successfully updating the license. Some builders also printed a cURL callback abort and still exited successfully. The results XML, six builder logs and pre/post scene comparison are in `Logs/PlainsNormal-review/`. This review changed no gameplay, layout, sprite offset or prop placement numbers.

## Changes not in the spec

- **Platform setup guard** (`Assets/Scripts/Play/MovingPlatformView.cs:19`). A platform view disables itself and reports a rebuild if its picture is missing or if it has a landing clip without a level controller link. Legacy silent platforms work with the picture alone.
- **Full prop clearance** (`Assets/Editor/LevelThemes.cs:529`). Plains Normal props avoid the entire sand and current strips. This moved three Pinatubo reed tufts by 107 px each and one Agusan hyacinth by 148 px, with the final hyacinth spot also respecting plant spacing. The table below records the positions.
- **Plains Easy badge test** (`Assets/Tests/EditMode/AchievementTests.cs:113`). The test now grants only kinds obtainable on the Easy trail. Granting all 16 kinds on the expanded Plains shelf can earn the shared Rock Scholar badge and would change what the test checks.
- **Bird Hide offset** (`Assets/Editor/LevelThemes.cs:215`). The picture uses (24, 23) so its visible base meets the ground despite one transparent bottom row. The goal trigger stays at x=7040.
- **Unlock checklist** (`.scratch/plains-normal/HANDOVER.md:231`). After Plains Easy 3, Hard asks the player to clear Normal first. After Normal 5, Next is hidden and the unlocked Hard card says “Coming soon.”

## Numbers and placement

- Gameplay data match `layouts.md`. Every trail is 7200 px wide. The goal trigger is x=7040. Each trail allows 180 s and has seven pickups, two checkpoints and one secret.
- Bird Hide's visual offset moved from **(24, 24) to (24, 23)**. The bottom row is transparent, so the 1 px drop puts the visible base on the ground. The other four goals use the offsets in the spec. The saved positions and sprite alpha bounds confirm that all five visible bases meet the ground.
- The Plains shelf grew from 10 to 16. The catalog grew from 54 to 56. Peat and laterite are the two new kinds. Pumice, dacite, andesite and gold also appear on the Plains shelf.
- The duck poses swap every 1.5, 2 and 2.5 s respectively. Egrets reuse the Plains Easy frames and timing pattern. Repeat placements use the existing half-spacing clearance and leave 90 px near the goal. Plains Normal props clear the full widths of sand and current strips.
- Four other fixed decorations moved to fit. In N3, fish stakes moved from x=4300 to 4313 and from x=6100 to 6113 to clear current edges. A bangka moved from x=5000 to 4975 to clear the quartz pickup. In N5, a fruit crate moved from x=2900 to 2884 to clear the siltstone pickup.

### Prop moves over 64 px

| Level | Prop | Planned x | Built x | Move | Clearance |
| --- | --- | ---: | ---: | ---: | --- |
| N1 | Reed tuft | 6100 | 6199 | 99 px | Water pit |
| N2 | Reed tuft | 600 | 493 | 107 px | Practice sand |
| N2 | Reed tuft | 2200 | 2307 | 107 px | Sand and andesite pickup |
| N2 | Reed tuft | 4200 | 4093 | 107 px | Sand |
| N4 | Water hyacinth | 3160 | 3308 | 148 px | Sand and spacing from the previous plant |

## Source and notes

- `Assets/Scripts/Rules/LevelData.cs` adds the five trails, two draft kinds and four shelf memberships.
- `Assets/Scripts/Rules/LevelLayout.cs` appends the five themes.
- `Assets/Scripts/Rules/PlatformLanding.cs` tracks when support changes to a platform.
- `Assets/Scripts/Play/MovingPlatformView.cs` aligns the deck and plays the optional landing sound.
- `Assets/Editor/LevelBuilder.cs` passes the chosen theme to the object builder.
- `Assets/Editor/LevelObjects.cs` uses the optional platform sprite and sound.
- `Assets/Editor/LevelThemes.cs` registers the themes and full zone clearance.
- `Assets/Editor/PixelSceneKit.cs` names the new generator for missing lesson pictures and item art.
- `Assets/Editor/PlainsNormalProps.cs` places decorations clear of the trail objects.
- `Assets/Tests/EditMode/CollectionTests.cs` checks 16 Plains kinds, 56 total kinds and the appended theme values.
- `Assets/Tests/EditMode/AchievementTests.cs` checks the expanded shelf and the existing Plains Normal badge rules.
- `Assets/Tests/EditMode/HillsHardLevelsTests.cs` checks that dacite keeps its Hills shelf spot.
- `Assets/Tests/EditMode/NextPlayableTests.cs` checks Normal progression and the Hard placeholder.
- `Assets/Tests/EditMode/LevelCatalogTests.cs` checks Plains Normal catalog bounds.
- `Assets/Tests/EditMode/PlainsNormalLevelsTests.cs` checks layouts, content, crossings and ambience.
- `Assets/Tests/EditMode/PlatformLandingTests.cs` checks the sound edge before, during and after a freeze.
- `CONTEXT.md` defines Market boat.
- `.scratch/plains-normal/HANDOVER.md` records verification, generated files and the play checklist.

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
- `Assets/Scenes/PlainsNormal1.unity`
- `Assets/Scenes/PlainsNormal2.unity`
- `Assets/Scenes/PlainsNormal3.unity`
- `Assets/Scenes/PlainsNormal4.unity`
- `Assets/Scenes/PlainsNormal5.unity`
- `Assets/Scenes/Lesson.unity`
- `Assets/Scenes/DifficultySelection.unity`
- `Assets/Scenes/LevelSelection.unity`
- `Assets/Scenes/LandformMap.unity`
- `Assets/Scenes/LandformDescription.unity`
- `Assets/Scenes/Profile.unity`

`ProjectSettings/EditorBuildSettings.asset` enables the five Plains Normal scenes.

### Unity import records

- `Assets/Art/Items/Sprites/laterite.png.meta`
- `Assets/Art/Items/Sprites/peat.png.meta`
- `Assets/Art/Lesson/Sprites/lesson_agno.png.meta`
- `Assets/Art/Lesson/Sprites/lesson_agusan.png.meta`
- `Assets/Art/Lesson/Sprites/lesson_candaba.png.meta`
- `Assets/Art/Lesson/Sprites/lesson_mekong.png.meta`
- `Assets/Art/Lesson/Sprites/lesson_pinatubo_lahar.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/CandabaWetland.meta`
- `Assets/Art/Level/Sprites/Backdrop/CandabaWetland/L0.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/CandabaWetland/L1.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/CandabaWetland/L2.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/CandabaWetland/L3.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/CandabaWetland/L4.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/CandabaWetland/L5.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingMarket.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingMarket/L0.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingMarket/L1.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingMarket/L2.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingMarket/L3.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingMarket/L4.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingMarket/L5.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingVillage.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingVillage/L0.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingVillage/L1.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingVillage/L2.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingVillage/L3.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingVillage/L4.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/FloatingVillage/L5.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/LaharPlain.meta`
- `Assets/Art/Level/Sprites/Backdrop/LaharPlain/L0.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/LaharPlain/L1.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/LaharPlain/L2.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/LaharPlain/L3.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/LaharPlain/L4.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/LaharPlain/L5.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverDelta.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverDelta/L0.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverDelta/L1.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverDelta/L2.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverDelta/L3.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverDelta/L4.png.meta`
- `Assets/Art/Level/Sprites/Backdrop/RiverDelta/L5.png.meta`
- `Assets/Art/Level/Sprites/Props/bangka_plains.png.meta`
- `Assets/Art/Level/Sprites/Props/crocodile.png.meta`
- `Assets/Art/Level/Sprites/Props/duck_00.png.meta`
- `Assets/Art/Level/Sprites/Props/duck_01.png.meta`
- `Assets/Art/Level/Sprites/Props/fish_stakes.png.meta`
- `Assets/Art/Level/Sprites/Props/fruit_crate.png.meta`
- `Assets/Art/Level/Sprites/Props/lahar_sign.png.meta`
- `Assets/Art/Level/Sprites/Props/reed_tuft.png.meta`
- `Assets/Art/Level/Sprites/Props/water_hyacinth.png.meta`
- `Assets/Art/Level/Sprites/Props/water_lily.png.meta`
- `Assets/Art/Level/Sprites/agusan_soil.png.meta`
- `Assets/Art/Level/Sprites/agusan_top.png.meta`
- `Assets/Art/Level/Sprites/bird_hide.png.meta`
- `Assets/Art/Level/Sprites/boulder_big_agusan.png.meta`
- `Assets/Art/Level/Sprites/boulder_big_candaba.png.meta`
- `Assets/Art/Level/Sprites/boulder_big_delta.png.meta`
- `Assets/Art/Level/Sprites/boulder_big_lahar.png.meta`
- `Assets/Art/Level/Sprites/boulder_big_mekong.png.meta`
- `Assets/Art/Level/Sprites/boulder_small_agusan.png.meta`
- `Assets/Art/Level/Sprites/boulder_small_candaba.png.meta`
- `Assets/Art/Level/Sprites/boulder_small_delta.png.meta`
- `Assets/Art/Level/Sprites/boulder_small_lahar.png.meta`
- `Assets/Art/Level/Sprites/boulder_small_mekong.png.meta`
- `Assets/Art/Level/Sprites/candaba_soil.png.meta`
- `Assets/Art/Level/Sprites/candaba_top.png.meta`
- `Assets/Art/Level/Sprites/delta_soil.png.meta`
- `Assets/Art/Level/Sprites/delta_top.png.meta`
- `Assets/Art/Level/Sprites/fish_pen_hut.png.meta`
- `Assets/Art/Level/Sprites/floating_house.png.meta`
- `Assets/Art/Level/Sprites/lahar_marker.png.meta`
- `Assets/Art/Level/Sprites/lahar_soil.png.meta`
- `Assets/Art/Level/Sprites/lahar_top.png.meta`
- `Assets/Art/Level/Sprites/ledge_agusan.png.meta`
- `Assets/Art/Level/Sprites/ledge_candaba.png.meta`
- `Assets/Art/Level/Sprites/ledge_delta.png.meta`
- `Assets/Art/Level/Sprites/ledge_lahar.png.meta`
- `Assets/Art/Level/Sprites/ledge_mekong.png.meta`
- `Assets/Art/Level/Sprites/market_boat.png.meta`
- `Assets/Art/Level/Sprites/market_pier.png.meta`
- `Assets/Art/Level/Sprites/mekong_soil.png.meta`
- `Assets/Art/Level/Sprites/mekong_top.png.meta`
- `Assets/Audio/sfx_boat_bump.wav.meta`
- `Assets/Editor/PlainsNormalProps.cs.meta`
- `Assets/Scenes/PlainsNormal1.unity.meta`
- `Assets/Scenes/PlainsNormal2.unity.meta`
- `Assets/Scenes/PlainsNormal3.unity.meta`
- `Assets/Scenes/PlainsNormal4.unity.meta`
- `Assets/Scenes/PlainsNormal5.unity.meta`
- `Assets/Tests/EditMode/PlainsNormalLevelsTests.cs.meta`
- `Assets/Scripts/Rules/PlatformLanding.cs.meta`
- `Assets/Tests/EditMode/PlatformLandingTests.cs.meta`

## Editor play checklist

Interactive Editor play, full pickup routes, in-game animation and audio, and Android device checks remain open. Teacher review of draft content is pending.

- [ ] **Unlock.** After Plains Easy 3, use Next to open Plains Normal 1. Check that the Normal trail has five levels and that Hard asks you to clear Normal first.
- [ ] **N1 Candaba Swamp.** Practise on the float over the safe shallow pool. Check that the floats sink and rise, the ducks and egrets appear, and the wetland loop plays. Open the peat card on first find. Check that the visible Bird Hide base meets the ground.
- [ ] **N2 Pinatubo Lahar Plain.** Check grey lahar ground and slower walking on soft sand. Cross both crumbling bridges and check that they break and return. Reach the secret dacite on the high ledge.
- [ ] **N3 Agno River Delta.** Check that the first current pushes with you and the second pushes against you. Look for water pits, fish stakes and bangka. Check that gold appears on the Plains shelf.
- [ ] **N4 Agusan Marsh.** Try the float, sand patch and current. Check the floating-house goal, crocodiles and water hyacinth.
- [ ] **N5 Mekong Delta.** Practise on the boat over the safe pool. Ride across the 200 px river pits. Hear one soft bump per landing. Pause and resume while standing on the boat and listen for silence. Then jump and land to hear a new bump. Open the laterite card.
- [ ] **Length.** Allow about 1.5 minutes per trail. The bot took about 84–98 s. The timer allows 180 s.
- [ ] **Profile.** Check the 16-kind Plains shelf and “Found here n/16.” Earn Philippine Plains after Normal 1–4 and Delta Boater after Normal 5.
- [ ] **After Normal 5.** Check that Next is hidden and the open Hard card says “Coming soon.”
- [ ] **Regression.** Check that Mountains Normal platforms look and move as before. Spot-check one Hills level and one Plains Easy level.
