# Mountains Normal 1–5 handover

Implemented all seven tickets’ code and reporting work. Ticket 04 retains its broad human play-through checks as pending; they are not falsely marked complete.

## Verification

- Final Unity 6000.6.8f1 EditMode suite: **198 passed, 0 failed** (45 test cases added over the 153-test baseline).
- Ticket totals: 01 = 155; 02 = 192; 03 = 194; 04 = 194; 05 = 194; 06 and final = 198. The initial ticket 03 failures were two outdated ten-kind expectations; both were updated and rerun successfully.
- Nine focused Play Mode checks passed: landing on a platform, platform carrying, first checkpoint, second checkpoint, ledge breaking, broken-bridge respawn to checkpoint 2, ledge return, rock respawn to checkpoint 2, and wind pushing the idle explorer left.
- All eight level scenes, the Lesson scene, and the Profile scene built successfully. Selection already obtains its five Normal stones from PlayerProgress and opens the catalog-driven Lesson screen; no selection change or rebuild was needed.
- Profile badges and both Collection pages rendered at 480×270 and visually inspected. An initial wrapped-name overlap was fixed; final renders have no reported badge text overflow.
- Every added nonblank C# line has an end comment. Rules have no Unity imports. Source/document whitespace audit passed; Unity-generated scene YAML contains its ordinary blank-value trailing spaces and was not hand-edited.
- No art/audio generator or PNG/WAV/OGG output changed. Builders updated sprite importer metadata and font assets. No commits, pushes, resets, stashes, or checkouts were run. The Obsidian vault was not edited.
- The delegated standards reviewer failed because its agent reached a usage limit. Direct source/spec review, compile checks, automated tests, live smoke checks, and render inspection were completed instead; no completed independent review is claimed.

## Numbers and layout adjustments

Gameplay coordinates, time limits, object timing/speeds, and lesson/review/item text are unchanged from the spec. The Pine Forest hut art offset is **(0,24)** instead of **(20,24)**, because the supplied sprite’s door is centered and should align with the goal. Ticket 05 explicitly permits goal-art offset adjustment. Other goal offsets remain (36,22), (16,20), and (28,28).

The badge rows use the proposed y positions 104, 162, and 220. Names stay on one line with automatic fitting (badge labels 10–15, Collection labels 9–15, Lesson item labels 8–15) to prevent longer Normal names overlapping adjacent cells. Pine frost uses 36 flakes across the trail; Snow Peak uses 192. These are presentation settings for the spec’s light frost and full-trail snow, not gameplay changes.

## Not checked or deferred

- No complete player-controlled run through all five Normal levels or every pickup route. The supplied physics-bot times were not independently rerun.
- No complete visual/play regression through Easy 1–3; existing rules tests passed and all Easy scenes rebuilt. Their data, hazard policy, themes, and audio selection remain unchanged.
- No human audio listening check, Android device/build test, or full runtime interaction check of Profile paging and the Result Screen. Profile wiring and static layout were checked.
- The live smoke check used checkpoint teleports and isolated object interactions; it does not prove every level is finishable.
- Hard retains Easy ambience as the requested placeholder. Hard levels and other landforms remain out of scope. Educational text remains draft pending teacher review.

## Builder commands, in order

Close the Unity Editor before running. Wait for each process to finish before starting the next.

```powershell
Set-Location 'D:\LandGrand\LandGrand'
$unityEditor = 'C:\Program Files\Unity\Hub\Editor\6000.6.8f1\Editor\Unity.exe'
& $unityEditor -batchmode -projectPath . -executeMethod LevelBuilder.BuildAll -quit -logFile Temp/build.log
& $unityEditor -batchmode -projectPath . -executeMethod LessonBuilder.Build -quit -logFile Temp/lesson-build.log
& $unityEditor -batchmode -projectPath . -executeMethod ProfileBuilder.Build -quit -logFile Temp/profile-build.log
& $unityEditor -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Temp/editmode.xml -logFile Temp/editmode.log
```

Do not rerun the art/audio generators: their outputs were already supplied and preserved. Optional repeatable validation commands are `NormalValidation.BuildAndInspect` (builders plus Profile renders) and `NormalPlayValidation.Begin` (live smoke checks; omit `-quit`, because the checker exits itself). Evidence is under `.scratch/mountains-normal/validation/`.

## User play checklist

- [ ] Each Normal level shows its banner (Mount Pulag to Mount Everest), its lesson and its theme
- [ ] Crumbling bridges break behind the explorer; stopping on one drops you to the last checkpoint
- [ ] Rocks warn for about a second before they fall
- [ ] Platforms carry the explorer
- [ ] Gusts push back and you can wait them out
- [ ] Both checkpoint flags work, in the level and on the bar
- [ ] The Result Screen shows 7 items; Next after Normal 5 leads to Hard 1 ("Coming soon")
- [ ] The Profile shows 2 pages of Mountains kinds and 11 badges

## Every added or changed project file

- `.scratch/mountains-normal/HANDOVER.md` — This file-by-file handover and remaining play checklist.
- `.scratch/mountains-normal/issues/01-two-checkpoints-and-normal-layout-data.md` — Records the implemented ticket work and verified or pending checklist items.
- `.scratch/mountains-normal/issues/02-rules-for-the-four-level-objects.md` — Records the implemented ticket work and verified or pending checklist items.
- `.scratch/mountains-normal/issues/03-ten-new-kinds-and-five-normal-levels.md` — Records the implemented ticket work and verified or pending checklist items.
- `.scratch/mountains-normal/issues/04-play-scripts-and-builder-for-level-objects.md` — Records the implemented ticket work and verified or pending checklist items.
- `.scratch/mountains-normal/issues/05-four-themes-wind-and-sounds.md` — Records the implemented ticket work and verified or pending checklist items.
- `.scratch/mountains-normal/issues/06-profile-pages-and-new-badges.md` — Records the implemented ticket work and verified or pending checklist items.
- `.scratch/mountains-normal/issues/07-final-check-and-handover.md` — Records the implemented ticket work and verified or pending checklist items.
- `.scratch/mountains-normal/validation/editmode.xml` — Durable final NUnit result: 198 passed, zero failed.
- `.scratch/mountains-normal/validation/profile-badges.png` — Rendered Profile inspection image at the game’s 480×270 resolution.
- `.scratch/mountains-normal/validation/profile-collection-1.png` — Rendered Profile inspection image at the game’s 480×270 resolution.
- `.scratch/mountains-normal/validation/profile-collection-2.png` — Rendered Profile inspection image at the game’s 480×270 resolution.
- `Assets/Art/Items/Sprites/basalt.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Items/Sprites/clay.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Items/Sprites/conglomerate.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Items/Sprites/gneiss.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Items/Sprites/limestone.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Items/Sprites/magnetite.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Items/Sprites/pyrite.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Items/Sprites/sandstone.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Items/Sprites/shale.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Items/Sprites/sulfur.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Lesson/Sprites/lesson_apo.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Lesson/Sprites/lesson_everest.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Lesson/Sprites/lesson_matutum.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Lesson/Sprites/lesson_pinatubo.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Lesson/Sprites/lesson_pulag.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/Props/crumble_bit.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/Props/fern.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/Props/gust_00.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/Props/gust_01.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/Props/gust_02.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/Props/gust_03.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/Props/pebbles_00.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/Props/pebbles_01.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/Props/pine_tree.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/Props/sulfur_crust.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/ash_soil.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/ash_top.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/boulder_big_ash.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/boulder_big_moss.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/boulder_big_pine.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/boulder_big_snow.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/boulder_small_ash.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/boulder_small_moss.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/boulder_small_pine.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/boulder_small_snow.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/bridge.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/crumble_00.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/crumble_01.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/crumble_02.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/crumble_03.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/hut.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/ledge_ash.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/ledge_moss.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/ledge_pine.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/ledge_snow.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/moss_soil.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/moss_top.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/pine_soil.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/pine_top.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/platform.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/rock_fall.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/rock_warn_00.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/rock_warn_01.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/snow_soil.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/snow_top.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/summit_flags.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/Level/Sprites/trail_sign.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/MainMenu/Sprites/Profile/badge_philippine_peaks.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/MainMenu/Sprites/Profile/badge_philippine_peaks_locked.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/MainMenu/Sprites/Profile/badge_rock_scholar.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/MainMenu/Sprites/Profile/badge_rock_scholar_locked.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/MainMenu/Sprites/Profile/badge_top_of_the_world.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Art/MainMenu/Sprites/Profile/badge_top_of_the_world_locked.png.meta` — Sprite import metadata configured by the existing editor builder; image pixels unchanged.
- `Assets/Editor/LessonBuilder.cs` — Fits long Normal item names and rebuilds the catalog-driven lesson picture list.
- `Assets/Editor/LevelBuilder.cs` — Builds both checkpoint flags and connects the data-driven objects and difficulty audio.
- `Assets/Editor/LevelObjects.cs` — Builds each placed object and loads its sound without per-level branches.
- `Assets/Editor/LevelObjects.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Editor/LevelThemes.cs` — Adds Pine Forest, Ash Gorge, Mossy Forest, and Snow Peak art and decoration.
- `Assets/Editor/NormalPlayValidation.cs` — Runs nine focused checks against live built scenes without collecting or clearing levels.
- `Assets/Editor/NormalPlayValidation.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Editor/NormalValidation.cs` — Provides repeatable scene builds and Profile inspection renders without saving inspection state.
- `Assets/Editor/NormalValidation.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Editor/PixelSceneKit.cs` — Chooses stronger wind for Normal; Easy and the Hard placeholder retain the breeze.
- `Assets/Editor/ProfileBuilder.cs` — Builds Collection arrows and page count, eleven badges in three rows, and fitting names.
- `Assets/Fonts/Andika SDF.asset` — Dynamic font asset data refreshed by the existing builders and text rendering.
- `Assets/Fonts/Micro5 2x.asset` — Dynamic font asset data refreshed by the existing builders and text rendering.
- `Assets/Resources/GDKEditionAutoGen.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Resources/GDKEditionAutoGen/GDKEdition.asset` — Supporting feature handover or Unity-generated build artifact.
- `Assets/Resources/GDKEditionAutoGen/GDKEdition.asset.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scenes/Lesson.unity` — Generated scene rebuilt by the editor builder; not edited by hand.
- `Assets/Scenes/MountainsEasy1.unity` — Generated scene rebuilt by the editor builder; not edited by hand.
- `Assets/Scenes/MountainsEasy2.unity` — Generated scene rebuilt by the editor builder; not edited by hand.
- `Assets/Scenes/MountainsEasy3.unity` — Generated scene rebuilt by the editor builder; not edited by hand.
- `Assets/Scenes/MountainsNormal1.unity` — Generated scene rebuilt by the editor builder; not edited by hand.
- `Assets/Scenes/MountainsNormal1.unity.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scenes/MountainsNormal2.unity` — Generated scene rebuilt by the editor builder; not edited by hand.
- `Assets/Scenes/MountainsNormal2.unity.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scenes/MountainsNormal3.unity` — Generated scene rebuilt by the editor builder; not edited by hand.
- `Assets/Scenes/MountainsNormal3.unity.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scenes/MountainsNormal4.unity` — Generated scene rebuilt by the editor builder; not edited by hand.
- `Assets/Scenes/MountainsNormal4.unity.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scenes/MountainsNormal5.unity` — Generated scene rebuilt by the editor builder; not edited by hand.
- `Assets/Scenes/MountainsNormal5.unity.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scenes/Profile.unity` — Generated scene rebuilt by the editor builder; not edited by hand.
- `Assets/Scripts/Play/CrumbleLedgeView.cs` — Shows cracking, break chips and sound, absence, and return while updating solidity.
- `Assets/Scripts/Play/CrumbleLedgeView.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scripts/Play/DynamicSolid.cs` — Defines current and previous shape information for moving or vanishing support.
- `Assets/Scripts/Play/DynamicSolid.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scripts/Play/ExplorerMotor.cs` — Collides with active dynamic solids, carries platform passengers, and adds wind speed.
- `Assets/Scripts/Play/FallingChipView.cs` — Draws four short-lived falling chips after a slab breaks.
- `Assets/Scripts/Play/FallingChipView.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scripts/Play/HazardWatcher.cs` — Raises both flags and applies Normal checkpoint returns to falling rocks and pits.
- `Assets/Scripts/Play/LevelGeometry.cs` — Exposes current active solid shapes and detects dynamic support under the feet.
- `Assets/Scripts/Play/MovingPlatformView.cs` — Moves its solid shape and art using the plain platform rule.
- `Assets/Scripts/Play/MovingPlatformView.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scripts/Play/RockfallView.cs` — Shows shadow warnings, pebbles, falling rock, landing sound, and dust.
- `Assets/Scripts/Play/RockfallView.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scripts/Play/TrailProgressBar.cs` — Raises the matching flag for each checkpoint.
- `Assets/Scripts/Play/WindWatcher.cs` — Sets the motor’s leftward push from active gust zones.
- `Assets/Scripts/Play/WindWatcher.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scripts/Play/WindZoneView.cs` — Draws six wind streaks and plays one nearby warning sound per gust.
- `Assets/Scripts/Play/WindZoneView.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scripts/Profile/ProfileController.cs` — Pages twelve kinds at a time, resets on a landform choice, and hides paging for one page.
- `Assets/Scripts/Rules/Achievements.cs` — Adds Philippine Peaks, Top of the World, and Rock Scholar from existing saved progress.
- `Assets/Scripts/Rules/Collection.cs` — Restores shelf PageCount and SlotsOnPage behavior from commit 5cc908b.
- `Assets/Scripts/Rules/CrumbleLedge.cs` — Plain 0.5-second break and 3-second return rule plus slab placement.
- `Assets/Scripts/Rules/CrumbleLedge.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scripts/Rules/LevelData.cs` — Adds checkpoint lists, ten item kinds, and all five exact Normal level records and texts.
- `Assets/Scripts/Rules/LevelLayout.cs` — Adds four themes and optional lists for the four new objects.
- `Assets/Scripts/Rules/LevelRun.cs` — Tracks the furthest checkpoint without moving the return point backward.
- `Assets/Scripts/Rules/MovingPlatform.cs` — Plain 40-pixel-per-second back-and-forth travel rule and placement.
- `Assets/Scripts/Rules/MovingPlatform.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scripts/Rules/Rockfall.cs` — Plain 3-second cycle, falling height, active hit box, and placement.
- `Assets/Scripts/Rules/Rockfall.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Scripts/Rules/WindZone.cs` — Plain 4-second gust cycle, zone coverage, and maximum 40-pixel-per-second push.
- `Assets/Scripts/Rules/WindZone.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Tests/EditMode/AchievementTests.cs` — Adds or updates public rule regression checks for the expanded Normal feature.
- `Assets/Tests/EditMode/CollectionTests.cs` — Adds or updates public rule regression checks for the expanded Normal feature.
- `Assets/Tests/EditMode/CrumbleLedgeTests.cs` — Adds or updates public rule regression checks for the expanded Normal feature.
- `Assets/Tests/EditMode/CrumbleLedgeTests.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Tests/EditMode/LevelCatalogTests.cs` — Adds or updates public rule regression checks for the expanded Normal feature.
- `Assets/Tests/EditMode/LevelRunTests.cs` — Adds or updates public rule regression checks for the expanded Normal feature.
- `Assets/Tests/EditMode/MountainsEasyLevelsTests.cs` — Adds or updates public rule regression checks for the expanded Normal feature.
- `Assets/Tests/EditMode/MountainsNormalTests.cs` — Adds or updates public rule regression checks for the expanded Normal feature.
- `Assets/Tests/EditMode/MountainsNormalTests.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Tests/EditMode/MovingPlatformTests.cs` — Adds or updates public rule regression checks for the expanded Normal feature.
- `Assets/Tests/EditMode/MovingPlatformTests.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Tests/EditMode/NormalCheckpointTests.cs` — Adds or updates public rule regression checks for the expanded Normal feature.
- `Assets/Tests/EditMode/NormalCheckpointTests.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Tests/EditMode/NormalProfileTests.cs` — Adds or updates public rule regression checks for the expanded Normal feature.
- `Assets/Tests/EditMode/NormalProfileTests.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Tests/EditMode/RockfallTests.cs` — Adds or updates public rule regression checks for the expanded Normal feature.
- `Assets/Tests/EditMode/RockfallTests.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `Assets/Tests/EditMode/WindZoneTests.cs` — Adds or updates public rule regression checks for the expanded Normal feature.
- `Assets/Tests/EditMode/WindZoneTests.cs.meta` — Unity-generated asset identity/import metadata for the corresponding file.
- `CONTEXT.md` — Adds the five requested domain definitions without implementation details.
- `ProjectSettings/EditorBuildSettings.asset` — Editor-generated configuration including the five Normal scenes in the build list.
- `.scratch/mountains-normal/validation/editmode.log` — Final Unity EditMode runner log (ignored by Git, retained locally).
- `.scratch/mountains-normal/validation/normal-play.log` — Nine passing live runtime checks (ignored by Git, retained locally).
