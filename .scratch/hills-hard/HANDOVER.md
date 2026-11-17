# Hills Hard 1–7 handover

Built in Unity 6000.6.8f1 on 2026-11-18. Hills Hard 1–7 are enabled in Build Settings. All seven use the 480×270 camera and the shared 84 px/s walk.

## Verification

| Ticket | EditMode results | New cases |
| --- | --- | --- |
| 01 Rules | 32 focused passed | 15 |
| 02 Catalog | 84 focused passed | 20 |
| 03 Play parts and builders | 106 focused passed | 0 |
| 04 Final build | **395 passed, 0 failed, 0 skipped** | 0 |

- All six builders completed in both passes. Their logs contain zero missing assets and zero scene-generation errors. The raw Unity logs include an access-token refresh error followed by a successful license update. Some Unity sessions also logged a cURL callback abort and a D3D12 info-queue diagnostic.
- The generators reproduced 133 files byte for byte in a temporary copy, including 84 sprite PNGs, 42 backdrop PNGs, four audio files and three previews. The copy kept the prepared media in the project intact.
- The spec review matched 401 layout and pickup-kind entries across Hills Hard 1–7 to the authored data. The standards review checked D-033 comments, plain C# rules and D-061. Log startup now reports incomplete references. The actual mud slide and sound still need the play check.
- All 36 rebuilt scenes are enabled. The 23 earlier level scenes match the baseline fixed collision shapes and music and ambience references. Hills Hard scene references match the authored mud and log counts. H4 uses `amb_wind_turbines`, H6 uses `amb_city_hills`, and H1–H3, H5 and H7 use `amb_rain_hard`.
- Test and builder logs are under `Logs/HillsHard-final/`. The test report is `editmode.xml` and prop moves are in `levels.log`.

## Numeric changes and build choices

All authored gameplay values match `layouts.md` and the spec. The Hills shelf grows from 17 to 23 as specified. The item catalog has 50 kinds. Limestone, Calcite and Andesite retain their existing Hills slots.

Section 8 counts 19 prop PNGs. The generator supplies 20 named files from its 10 still props and five two-frame sets. The scene builder uses all the named props.

The log view uses the boulder roll's 0.1 s frame time and 260 px hearing distance. Mud splashes use the sand puff's 0.15 s frame time while the explorer slides. The builder reads the specified 2.5 s tarsier and 0.5 s turbine times as time per frame, consistent with the Normal prop helper. Wind grass, sheep and cormorants use that helper's existing 1 s frame default.

The fit check uses the widest frame. It searches by whole pixels, takes the left spot on a tie and uses the Normal spacing margin of half the authored interval between repeats. The table records every decoration move greater than 64 px. Positions and distances are in level pixels.

| Trail | Prop | Planned x | Built x | Distance | Clearance |
| --- | --- | ---: | ---: | ---: | --- |
| H1 | `vine` | 660 | 807 | 147 | Mud |
| H1 | `vine` | 1960 | 2207 | 247 | Mud |
| H1 | `vine` | 2220 | 2337 | 117 | Spacing from the shifted vine |
| H1 | `vine` | 3260 | 3193 | 67 | Mud |
| H1 | `vine` | 3520 | 3707 | 187 | Mud and boulder |
| H1 | `vine` | 5080 | 4993 | 87 | Mud |
| H1 | `vine` | 5340 | 5407 | 67 | Mud |
| H1 | `vine` | 6120 | 5993 | 127 | Mud and ledge |
| H1 | `vine` | 7680 | 7290 | 390 | Mud |
| H1 | `vine` | 7940 | 8107 | 167 | Mud |
| H1 | `vine` | 8980 | 9207 | 227 | Mud |
| H1 | `vine` | 9240 | 9337 | 97 | Spacing from the shifted vine |
| H1 | `tarsier` | 7700 | 7493 | 207 | Mud |
| H2 | `cracked_ground` | 5200 | 5321 | 121 | Log route |
| H3 | `karst_spike` | 840 | 907 | 67 | Mud |
| H3 | `karst_spike` | 1860 | 1693 | 167 | Mud |
| H3 | `karst_spike` | 3560 | 3443 | 117 | Mud |
| H3 | `karst_spike` | 5260 | 5457 | 197 | Mud |
| H3 | `karst_spike` | 5600 | 5703 | 103 | Pit and crumbling bridge |
| H3 | `karst_spike` | 8320 | 8557 | 237 | Mud |
| H3 | `karst_spike` | 8660 | 8727 | 67 | Spikes |
| H4 | `wind_grass` | 4400 | 4515 | 115 | Log route |
| H4 | `wind_grass` | 6020 | 5937 | 83 | Pit and bridge |
| H4 | `wind_grass` | 6560 | 6635 | 75 | Log route |
| H5 | `stone_wall` | 900 | 1017 | 117 | Mud |
| H5 | `stone_wall` | 3300 | 3233 | 67 | Mud and spikes |
| H5 | `stone_wall` | 5700 | 5825 | 125 | Log route |
| H5 | `stone_wall` | 6300 | 6183 | 117 | Mud |
| H6 | `cypress` | 3720 | 3545 | 175 | Log route |

The other adjusted fixed placements are listed below. Repeating decoration uses approximate spacing as specified.

| Trail | Prop | Planned x | Built x | Distance | Clearance |
| --- | --- | ---: | ---: | ---: | --- |
| H1 | `tarsier` | 4300 | 4293 | 7 | Pit |
| H2 | `warning_sign` | 3400 | 3363 | 37 | Log route |
| H2 | `tilted_post` | 2200 | 2194 | 6 | Boulder |
| H4 | `turbine_far` | 7200 | 7175 | 25 | Checkpoint flag |
| H5 | `sheep` | 4400 | 4391 | 9 | Spikes |
| H5 | `sheep` | 8100 | 8147 | 47 | Log route |
| H6 | `column_broken` | 2900 | 2935 | 35 | Log route |
| H6 | `column_broken` | 6900 | 6915 | 15 | Log route |
| H7 | `cormorant` | 5000 | 4993 | 7 | Mud |
| H7 | `cormorant` | 8400 | 8393 | 7 | Pit |

## Generated files

Unity saved these 36 scenes. The seven new Hills Hard scenes also have corresponding `.unity.meta` import records.

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
- `Assets/Scenes/Lesson.unity`
- `Assets/Scenes/DifficultySelection.unity`
- `Assets/Scenes/LevelSelection.unity`
- `Assets/Scenes/LandformMap.unity`
- `Assets/Scenes/LandformDescription.unity`
- `Assets/Scenes/Profile.unity`

- `ProjectSettings/EditorBuildSettings.asset` records the enabled Hills Hard scenes.
- Unity imported the 130 prepared media files and created their importer metadata plus seven backdrop folder records under `Assets/Art` and `Assets/Audio`.
- The 133 generator outputs include the prepared preview sheets `art-preview.png`, `backdrop-preview.png` and `audio-preview.png`.

## Source files

- `Assets/Scripts/Rules/MudZone.cs` places slippery mud.
- `Assets/Scripts/Rules/RollingLog.cs` defines directional rolls and reuses the boulder skip and hit rules.
- `Assets/Scripts/Rules/LevelLayout.cs` adds mud and log lists and seven theme values.
- `Assets/Scripts/Rules/LevelData.cs` adds the seven trails and six draft kinds.
- `Assets/Scripts/Rules/LandformAudio.cs` maps the two ambience files to their generator.
- `Assets/Scripts/Play/IceWatcher.cs` enables the same slide on mud with its own sound and splash.
- `Assets/Scripts/Play/LogRoller.cs` draws the warnings and rolls, checks contact and plays the loop.
- `Assets/Scripts/Play/HazardWatcher.cs` applies the same response to log and boulder hits.
- `Assets/Scripts/Play/LevelController.cs` plays the loop at the player's effects volume.
- `Assets/Editor/HardLevelObjects.cs` builds mud and logs from the layout.
- `Assets/Editor/LevelBuilder.cs` calls the shared object builder for all levels.
- `Assets/Editor/LevelThemes.cs` wires the theme art and living props and checks their clearance.
- `Assets/Editor/PixelSceneKit.cs` reports the Hard generator for missing Hard sprites.
- `Assets/Tests/EditMode/HillsHardRulesTests.cs` adds rule checks.
- `Assets/Tests/EditMode/HillsHardLevelsTests.cs` checks trail data, practice spots, clearance, ambience and kinds.
- `Assets/Tests/EditMode/CollectionTests.cs` checks the expanded catalog and shelves.
- `Assets/Tests/EditMode/HillsBadgeTests.cs` checks Hill Collector with all 23 kinds.
- `Assets/Tests/EditMode/HillsNormalLevelsTests.cs` keeps the seven-kind assertion scoped to Normal.
- `Assets/Tests/EditMode/LevelCatalogTests.cs` covers the Hard catalog bounds.
- `Assets/Tests/EditMode/NextPlayableTests.cs` covers the Normal-to-Hard route and episode completion.
- `CONTEXT.md` defines Slippery mud, Rolling log and Hills complete.
- `.scratch/hills-hard/HANDOVER.md` records the build and play checks.

Unity made a `.cs.meta` file for each new script and test. All work is uncommitted.

## Rebuild

Close the Editor before running batch commands from `D:\LandGrand\LandGrand`. Run `LevelBuilder.BuildAll`, `LessonBuilder.Build`, `SelectionBuilder.Build`, `LandformMapBuilder.Build`, `LandformDescriptionBuilder.Build` and `ProfileBuilder.Build` in that order. Pass each method to Unity with `-batchmode -projectPath . -executeMethod <method> -quit -logFile <log>`. For tests, use `-batchmode -projectPath . -runTests -testPlatform EditMode -testResults <xml> -logFile <log>`.

## Editor play checklist

Batch verification covered compilation, rules and saved scene references. Interactive play, in-game audio and visual checks, all secret routes, the full older-level regression, and an Android run remain open. The lesson and kind copy is draft for teacher review.

- [ ] After Hills Normal 5, Next opens Hills Hard 1; the Hard trail shows seven stones
- [ ] Each Hard level shows its banner, lesson, theme, background and goal
- [ ] H1: mud slides like ice, with a squelch and splashes; the first mud is safe
- [ ] H2: logs warn, then roll; the first log is safe; the lesson teaches warning signs calmly
- [ ] H3–H7: each mix is fair; H4 plays wind and turbines; H6 plays bells and the city; the others play rain
- [ ] Right-rolling logs only catch you if you stop; H6's log at the platform can be hopped
- [ ] Each level has 9 pickups, and the secret is reachable; the six new kinds show their draft text
- [ ] Collection: the Hills tab shows 23 kinds; "Found here" counts them
- [ ] Slope Ranger, World Hills, Steady Star, Hill Master and Hill Collector can be earned
- [ ] After Hills Hard 7: the Hills Description Screen, then the map unlocks Plains once and shows "Coming soon"
- [ ] Mountains (ice and boulders) and Hills Easy and Normal still play as before