# Mountains Hard 1–7 and Landform Description handover

All seven Hard trails are built. They add 14 item kinds, rolling boulders, icy ground, and three checkpoints per trail. Finishing all 15 Mountains levels opens the Landform Description Screen from Next Episode and opens Hills on the map. The save still stores the existing clears and found items. The map remembers its one-time animation separately in the device settings. The Mountains difficulty screen reopens the review through its book. Profile shows three Collection pages and 15 badges.

## Verification

| Ticket | Unity EditMode passed | Other verification |
| --- | ---: | --- |
| 01 | 200 | Three flags and independent copies of both layout lists. |
| 02 | 212 | Boulder timing, bounds and skip, plus ice speed and edges. |
| 03 | 216 | Hard data and safety margins. |
| 04 | 216 | Eight Easy/Normal scenes rebuilt before the Hard themes were wired. Hard builds and focused play followed. |
| 05 | 216 | All 15 levels built. Seven Hard scenes rendered at 480×270, and 174 placed ground props checked for clearance. The saved scenes reference the correct difficulty wind. |
| 06 | 219 | Map and Profile rebuilt. Unlock, star totals and the existing save format passed. |
| 07 | 223 | Summary text fits at 480×270. The Hard 7 result, map unlock and book route passed in Play Mode. |
| 08 | 228 | All 15 badge labels and ten items on page 3 inspected at 480×270. |
| 09 | 234 | All six builders completed. 19 focused Play Mode checks passed. |

All final EditMode cases passed with zero failures. The Play Mode checks used test progress in memory, reached selected positions by teleport, and used the actual gameplay and screen buttons. They verified the boulder warning, pause, skip, jump and checkpoint return, all three flags, the ice speed change and wind, the live Hard 7 clear and Next Episode route, the trip totals, the one-time Hills animation and Coming soon, book visibility and Back, all three Profile pages and 15 badges. The test restored the device's previous Hills animation setting.

Both review passes completed against `08ce224`. Standards review moved the slide sound threshold into the plain ice rule, confirmed ice still works if its sound is unavailable, and prompted one shared rule for the next episode. Spec review matched all seven Hard layouts and draft text and found that the warning sound could outlast a paused warning. The boulder warning and roll now have separately pausable sound players.

All four requested asset generators exited successfully in a temporary copy. All 191 regenerated media files matched the supplied outputs by PNG pixels, WAV bytes or decoded OGG samples. The repository's art and audio scripts and media files were preserved. The builders refreshed Unity import metadata. Production C# source and Markdown passed the diff whitespace check. The vault was read only, and the user retains all commits.

## Spec numbers

No specified gameplay, layout, theme offset or timing number changed. The summary keeps Andika at 15 with slightly tighter letter spacing. Badge names wrap at Andika 15 with tighter line spacing between wrapped lines. The grid uses the specified five columns, three rows and 88-pixel name width.

## Play checks still open

The focused editor run did not cover a full playthrough of all seven Hard trails, every pickup route, or the full Easy and Normal regression playthrough. Audio was checked for matching generated content, scene references and playback wiring. Listening to the volumes and the device build and touch experience remain open. The user checklist below remains open for acceptance. The lesson, item, and landform summary text remains marked draft pending teacher review.

## Builder commands in order

Run from the project root with the Unity Editor closed. The four asset scripts are listed for reproducibility. Their outputs already match, so run them again only in a temporary copy if comparing the final media.

```powershell
python Tools/Art/make_hard_art.py
python Tools/Art/make_hard_backdrops.py
python Tools/Art/make_profile_art.py
python Tools/Audio/make_hard_audio.py
```

The scene builders can be run from the LandGrand menu or in batch mode. Run them from the actual Unity project.

```powershell
Set-Location 'D:\LandGrand\LandGrand'
$unityEditor = 'C:\Program Files\Unity\Hub\Editor\6000.6.8f1\Editor\Unity.exe'
$builders = [ordered]@{
    'LevelBuilder.BuildAll' = 'Temp/levels-build.log'
    'LessonBuilder.Build' = 'Temp/lesson-build.log'
    'SelectionBuilder.Build' = 'Temp/selection-build.log'
    'LandformMapBuilder.Build' = 'Temp/map-build.log'
    'LandformDescriptionBuilder.Build' = 'Temp/description-build.log'
    'ProfileBuilder.Build' = 'Temp/profile-build.log'
}
foreach ($job in $builders.GetEnumerator()) {
    $unityArguments = @('-batchmode', '-projectPath', '.', '-executeMethod', $job.Key, '-quit', '-logFile', $job.Value)
    $unityProcess = Start-Process -FilePath $unityEditor -ArgumentList $unityArguments -WindowStyle Hidden -Wait -PassThru
    if ($unityProcess.ExitCode -ne 0) { throw "Builder failed. Check $($job.Value)." }
}
$unityArguments = @('-batchmode', '-projectPath', '.', '-runTests', '-testPlatform', 'EditMode', '-testResults', 'Temp/editmode.xml', '-logFile', 'Temp/editmode.log')
$unityProcess = Start-Process -FilePath $unityEditor -ArgumentList $unityArguments -WindowStyle Hidden -Wait -PassThru
if ($unityProcess.ExitCode -ne 0) { throw 'EditMode failed. Check Temp/editmode.log.' }
```

Menus in the same order are **Build Mountains Levels**, **Build Lesson Scene**, **Build Selection Scenes**, **Build Landform Map Scene**, **Build Landform Description Scene**, and **Build Profile Scene**.

## User play checklist

- [ ] Each Hard level shows its banner (Mount Mayon to Mauna Kea), its lesson, its theme and its own background
- [ ] Rolling boulders rumble first, roll left, can be jumped, never appear on top of the explorer, and send you to the last checkpoint
- [ ] Ice makes the explorer slide; Mayon's first boulder and the Matterhorn's first ice patch are safe places to learn
- [ ] All three checkpoint flags work, in the level and on the bar
- [ ] The Result Screen shows 9 items; Next after Hard 7 opens the Landform Description Screen
- [ ] The Description Screen shows the summary, stars n/45, kinds n/34, badges n/15 and "Hills is now open!"; the fanfare plays
- [ ] To the Map: Hills unlocks with the padlock animation once; tapping Hills says "Coming soon"
- [ ] The book button on the Mountains difficulty screen reopens the Description Screen; Back returns
- [ ] The Profile shows 3 pages of Mountains kinds and 15 badges in a 5×3 grid
- [ ] Easy and Normal still play as before

## Files added or changed

The supplied Hard media and generator changes already in the working tree are outside this inventory. Scenes are generated by the builders. Validation images are snapshots outside the game assets.

- `.scratch/mountains-hard/HANDOVER.md` — Records final verification, builders, inventory and the play checklist.
- `.scratch/mountains-hard/issues/01-three-checkpoints-and-hard-layout-data.md` — Records the ticket status, verification and builder commands.
- `.scratch/mountains-hard/issues/02-rules-for-rolling-boulder-and-icy-ground.md` — Records the ticket status, verification and builder commands.
- `.scratch/mountains-hard/issues/03-fourteen-new-kinds-and-seven-hard-levels.md` — Records the ticket status, verification and builder commands.
- `.scratch/mountains-hard/issues/04-play-scripts-and-builder-for-rolling-boulder-and-ice.md` — Records the ticket status, verification and builder commands.
- `.scratch/mountains-hard/issues/05-seven-themes-hard-wind-and-sounds.md` — Records the ticket status, verification and builder commands.
- `.scratch/mountains-hard/issues/06-landform-unlock-rules-and-map.md` — Records the ticket status, verification and builder commands.
- `.scratch/mountains-hard/issues/07-landform-description-screen.md` — Records the ticket status, verification and builder commands.
- `.scratch/mountains-hard/issues/08-profile-third-page-and-new-badges.md` — Records the ticket status, verification and builder commands.
- `.scratch/mountains-hard/issues/09-final-check-and-handover.md` — Records the ticket status, verification and builder commands.
- `.scratch/mountains-hard/ticket-01.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-01.xml` — Unity EditMode test results for this ticket.
- `.scratch/mountains-hard/ticket-02.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-02.xml` — Unity EditMode test results for this ticket.
- `.scratch/mountains-hard/ticket-03.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-03.xml` — Unity EditMode test results for this ticket.
- `.scratch/mountains-hard/ticket-04-existing-build.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-04.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-04.xml` — Unity EditMode test results for this ticket.
- `.scratch/mountains-hard/ticket-05-build.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-05-inspection-final.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-05-inspection.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-05.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-05.xml` — Unity EditMode test results for this ticket.
- `.scratch/mountains-hard/ticket-06-build.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-06.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-06.xml` — Unity EditMode test results for this ticket.
- `.scratch/mountains-hard/ticket-07-build-final.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-07-build.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-07-play-final.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-07-play.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-07.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-07.xml` — Unity EditMode test results for this ticket.
- `.scratch/mountains-hard/ticket-08-build-final.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-08-build.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-08-inspection.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-08.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-08.xml` — Unity EditMode test results for this ticket.
- `.scratch/mountains-hard/ticket-09-build-final.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-09-build.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-09-play.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-09.log` — Editor, builder or Play Mode validation log for this ticket.
- `.scratch/mountains-hard/ticket-09.xml` — Unity EditMode test results for this ticket.
- `.scratch/mountains-hard/validation/LandformDescription.png` — 480×270 inspection render from the built scene.
- `.scratch/mountains-hard/validation/MountainsHard1.png` — 480×270 inspection render from the built scene.
- `.scratch/mountains-hard/validation/MountainsHard2.png` — 480×270 inspection render from the built scene.
- `.scratch/mountains-hard/validation/MountainsHard3.png` — 480×270 inspection render from the built scene.
- `.scratch/mountains-hard/validation/MountainsHard4.png` — 480×270 inspection render from the built scene.
- `.scratch/mountains-hard/validation/MountainsHard5.png` — 480×270 inspection render from the built scene.
- `.scratch/mountains-hard/validation/MountainsHard6.png` — 480×270 inspection render from the built scene.
- `.scratch/mountains-hard/validation/MountainsHard7.png` — 480×270 inspection render from the built scene.
- `.scratch/mountains-hard/validation/ProfileBadges.png` — 480×270 inspection render from the built scene.
- `.scratch/mountains-hard/validation/ProfileCollection3.png` — 480×270 inspection render from the built scene.
- `.scratch/mountains-hard/validation/asset-check.txt` — Records the four successful generator runs and the 191 matching outputs.
- `Assets/Art/Description.meta` — Unity import or asset identity metadata.
- `Assets/Art/Description/Sprites.meta` — Unity import or asset identity metadata.
- `Assets/Art/Description/Sprites/btn_book_icon.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Description/Sprites/btn_book_icon_pressed.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Description/Sprites/btn_to_map.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Description/Sprites/btn_to_map_pressed.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Description/Sprites/scroll_panel.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Description/Sprites/sparkle_00.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Description/Sprites/sparkle_01.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Description/Sprites/sparkle_02.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Description/Sprites/unlock_padlock_00.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Description/Sprites/unlock_padlock_01.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Description/Sprites/unlock_padlock_02.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Description/Sprites/unlock_padlock_03.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/calcite.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/chromite.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/diorite.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/gabbro.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/gold.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/hematite.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/hornblende.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/olivine.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/schist.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/scoria.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/serpentinite.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/tanzanite.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/trachyte.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Items/Sprites/tuff.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Lesson/Sprites/lesson_hamiguitan.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Lesson/Sprites/lesson_kilimanjaro.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Lesson/Sprites/lesson_kitanglad.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Lesson/Sprites/lesson_matterhorn.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Lesson/Sprites/lesson_mauna_kea.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Lesson/Sprites/lesson_mayon.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Lesson/Sprites/lesson_sierra_madre.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/BonsaiForest.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/BonsaiForest/L0.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/BonsaiForest/L1.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/BonsaiForest/L2.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/BonsaiForest/L3.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/BonsaiForest/L4.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/BonsaiForest/L5.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EagleForest.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EagleForest/L0.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EagleForest/L1.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EagleForest/L2.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EagleForest/L3.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EagleForest/L4.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EagleForest/L5.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EquatorSummit.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EquatorSummit/L0.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EquatorSummit/L1.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EquatorSummit/L2.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EquatorSummit/L3.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EquatorSummit/L4.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/EquatorSummit/L5.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/GlacierHorn.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/GlacierHorn/L0.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/GlacierHorn/L1.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/GlacierHorn/L2.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/GlacierHorn/L3.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/GlacierHorn/L4.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/GlacierHorn/L5.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/LavaSlope.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/LavaSlope/L0.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/LavaSlope/L1.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/LavaSlope/L2.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/LavaSlope/L3.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/LavaSlope/L4.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/LavaSlope/L5.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/SeaPeak.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/SeaPeak/L0.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/SeaPeak/L1.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/SeaPeak/L2.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/SeaPeak/L3.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/SeaPeak/L4.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/SeaPeak/L5.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/StormRidge.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/StormRidge/L0.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/StormRidge/L1.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/StormRidge/L2.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/StormRidge/L3.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/StormRidge/L4.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Backdrop/StormRidge/L5.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/bent_tree.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/bonsai_tree.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/burnt_tree.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/edelweiss.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/groundsel.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/ice_glint_00.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/ice_glint_01.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/orchid.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/pitcher_plant.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/rain_00.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/rain_01.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/rumble_dust_00.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/rumble_dust_01.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/silversword.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/Props/tall_tree.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/alpine_hut.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/bonsai_soil.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/bonsai_top.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_big_bonsai.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_big_eagle.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_big_equator.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_big_glacier.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_big_lava.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_big_sea.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_big_storm.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_small_bonsai.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_small_eagle.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_small_equator.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_small_glacier.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_small_lava.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_small_sea.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/boulder_small_storm.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/eagle_nest.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/eagle_soil.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/eagle_top.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/equator_soil.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/equator_top.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/glacier_soil.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/glacier_top.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/ice_top.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/lava_soil.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/lava_top.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/ledge_bonsai.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/ledge_eagle.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/ledge_equator.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/ledge_glacier.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/ledge_lava.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/ledge_sea.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/ledge_storm.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/lookout_tower.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/roll_boulder_00.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/roll_boulder_01.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/roll_boulder_02.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/roll_boulder_03.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/sanctuary_sign.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/sea_soil.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/sea_top.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/storm_soil.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/storm_top.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/summit_board.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/telescope_dome.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/Level/Sprites/watch_station.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/MainMenu/Sprites/Profile/badge_brave_hiker.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/MainMenu/Sprites/Profile/badge_brave_hiker_locked.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/MainMenu/Sprites/Profile/badge_hard_star.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/MainMenu/Sprites/Profile/badge_hard_star_locked.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/MainMenu/Sprites/Profile/badge_mountain_master.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/MainMenu/Sprites/Profile/badge_mountain_master_locked.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/MainMenu/Sprites/Profile/badge_world_peaks.png.meta` — Unity import or asset identity metadata.
- `Assets/Art/MainMenu/Sprites/Profile/badge_world_peaks_locked.png.meta` — Unity import or asset identity metadata.
- `Assets/Audio/amb_wind_hard.ogg.meta` — Unity import or asset identity metadata.
- `Assets/Audio/sfx_boulder_roll.wav.meta` — Unity import or asset identity metadata.
- `Assets/Audio/sfx_boulder_rumble.wav.meta` — Unity import or asset identity metadata.
- `Assets/Audio/sfx_fanfare.wav.meta` — Unity import or asset identity metadata.
- `Assets/Audio/sfx_ice_slide.wav.meta` — Unity import or asset identity metadata.
- `Assets/Audio/sfx_unlock.wav.meta` — Unity import or asset identity metadata.
- `Assets/Editor/DescriptionArt.cs` — Loads the fixed review sprites and sounds, and builds the book, map button and shared lock effect.
- `Assets/Editor/DescriptionArt.cs.meta` — Unity import or asset identity metadata.
- `Assets/Editor/HardLevelObjects.cs` — Builds ice tiles and glints, connects the ice part, and wires each placed rolling boulder.
- `Assets/Editor/HardLevelObjects.cs.meta` — Unity import or asset identity metadata.
- `Assets/Editor/LandformDescriptionBuilder.cs` — Builds the 480×270 review screen and registers it directly after Lesson.
- `Assets/Editor/LandformDescriptionBuilder.cs.meta` — Unity import or asset identity metadata.
- `Assets/Editor/LandformMapBuilder.cs` — Connects the opening lock, sparkles and Hills cloud.
- `Assets/Editor/LevelBuilder.cs` — Builds the new level objects from layout data and keeps the review in the build list.
- `Assets/Editor/LevelThemes.cs` — Defines all seven themes, their backdrop folders and safe decoration placement.
- `Assets/Editor/PixelSceneKit.cs` — Binds the supplied Hard wind to Hard levels.
- `Assets/Editor/ProfileBuilder.cs` — Builds a five by three medal grid with 15-point labels and the three-page Collection.
- `Assets/Editor/SelectionBuilder.cs` — Mirrors the Back icon with a book button on the difficulty screen.
- `Assets/Fonts/Andika SDF.asset` — Dynamic TMP font data refreshed during the editor builds.
- `Assets/Fonts/Micro5 2x.asset` — Dynamic TMP font data refreshed during the editor builds.
- `Assets/Resources/GDKEditionAutoGen.meta` — Unity import or asset identity metadata.
- `Assets/Resources/GDKEditionAutoGen/GDKEdition.asset` — Unity-generated project data.
- `Assets/Resources/GDKEditionAutoGen/GDKEdition.asset.meta` — Unity import or asset identity metadata.
- `Assets/Scenes/DifficultySelection.unity` — Scene generated by the editor builder.
- `Assets/Scenes/LandformDescription.unity` — Scene generated by the editor builder.
- `Assets/Scenes/LandformDescription.unity.meta` — Unity import or asset identity metadata.
- `Assets/Scenes/LandformMap.unity` — Scene generated by the editor builder.
- `Assets/Scenes/Lesson.unity` — Scene generated by the editor builder.
- `Assets/Scenes/LevelSelection.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsEasy1.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsEasy2.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsEasy3.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsHard1.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsHard1.unity.meta` — Unity import or asset identity metadata.
- `Assets/Scenes/MountainsHard2.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsHard2.unity.meta` — Unity import or asset identity metadata.
- `Assets/Scenes/MountainsHard3.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsHard3.unity.meta` — Unity import or asset identity metadata.
- `Assets/Scenes/MountainsHard4.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsHard4.unity.meta` — Unity import or asset identity metadata.
- `Assets/Scenes/MountainsHard5.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsHard5.unity.meta` — Unity import or asset identity metadata.
- `Assets/Scenes/MountainsHard6.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsHard6.unity.meta` — Unity import or asset identity metadata.
- `Assets/Scenes/MountainsHard7.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsHard7.unity.meta` — Unity import or asset identity metadata.
- `Assets/Scenes/MountainsNormal1.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsNormal2.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsNormal3.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsNormal4.unity` — Scene generated by the editor builder.
- `Assets/Scenes/MountainsNormal5.unity` — Scene generated by the editor builder.
- `Assets/Scenes/Profile.unity` — Scene generated by the editor builder.
- `Assets/Scripts/Common/GameSession.cs` — Records whether the review was opened from the book.
- `Assets/Scripts/Common/LandformDescriptionController.cs` — Shows the draft review, saved trip totals, next landform and the proper Back route.
- `Assets/Scripts/Common/LandformDescriptionController.cs.meta` — Unity import or asset identity metadata.
- `Assets/Scripts/Common/PixelScreen.cs` — Lets the existing guide show the new map notice for two seconds.
- `Assets/Scripts/Common/UnlockAnimation.cs` — Animates the opening lock, three sparkles and optional grey region fade.
- `Assets/Scripts/Common/UnlockAnimation.cs.meta` — Unity import or asset identity metadata.
- `Assets/Scripts/LandformMap/LandformMapController.cs` — Shows unlocked regions, handles empty regions, and remembers when the unlock animation has played.
- `Assets/Scripts/Play/ExplorerMotor.cs` — Separates walking speed from wind, applies ice slip and resets walking speed at walls.
- `Assets/Scripts/Play/ForestEagle.cs` — Flies the existing eagle frames over the forest on the level clock.
- `Assets/Scripts/Play/ForestEagle.cs.meta` — Unity import or asset identity metadata.
- `Assets/Scripts/Play/HazardWatcher.cs` — Applies the existing checkpoint trap response when a live rolling boulder hits.
- `Assets/Scripts/Play/IceWatcher.cs` — Checks whether the player stands on ice and cues the start of a slide.
- `Assets/Scripts/Play/IceWatcher.cs.meta` — Unity import or asset identity metadata.
- `Assets/Scripts/Play/IceZoneView.cs` — Animates the icy ground glints on the level clock.
- `Assets/Scripts/Play/IceZoneView.cs.meta` — Unity import or asset identity metadata.
- `Assets/Scripts/Play/ResultScreen.cs` — Opens the review when a finished episode offers Next Episode.
- `Assets/Scripts/Play/RidgeRain.cs` — Draws camera-following rain using the shared level clock.
- `Assets/Scripts/Play/RidgeRain.cs.meta` — Unity import or asset identity metadata.
- `Assets/Scripts/Play/RollingBoulderView.cs` — Shows the warning, turning rock and end dust, and plays the pausable warning and roll.
- `Assets/Scripts/Play/RollingBoulderView.cs.meta` — Unity import or asset identity metadata.
- `Assets/Scripts/Play/RollingBoulderWatcher.cs` — Shares the timed skip decision between the hit check and the view.
- `Assets/Scripts/Play/RollingBoulderWatcher.cs.meta` — Unity import or asset identity metadata.
- `Assets/Scripts/Play/SummitSnow.cs` — Lets new Hard snow pause with the level while existing snow keeps its prior behavior.
- `Assets/Scripts/Profile/ProfileController.cs` — Updates the badge count examples to 15.
- `Assets/Scripts/Rules/Achievements.cs` — Adds Brave Hiker, World Peaks, Hard Star and Mountain Master.
- `Assets/Scripts/Rules/IceSlide.cs` — Sets the acceleration, slowing and slide sound rules in plain C#.
- `Assets/Scripts/Rules/IceSlide.cs.meta` — Unity import or asset identity metadata.
- `Assets/Scripts/Rules/IceZone.cs` — Defines an icy stretch and checks whether an x position falls inside it.
- `Assets/Scripts/Rules/IceZone.cs.meta` — Unity import or asset identity metadata.
- `Assets/Scripts/Rules/LandformSummary.cs` — Keeps the five draft Mountains sentences and the short placeholders for future landforms.
- `Assets/Scripts/Rules/LandformSummary.cs.meta` — Unity import or asset identity metadata.
- `Assets/Scripts/Rules/LevelData.cs` — Adds seven exact Hard catalog entries, fourteen item kinds and a playable-data query.
- `Assets/Scripts/Rules/LevelLayout.cs` — Adds the seven themes and optional rolling-boulder and ice lists.
- `Assets/Scripts/Rules/PlayerProgress.cs` — Derives episode completion, unlocks and stars from existing saved clears.
- `Assets/Scripts/Rules/RollingBoulder.cs` — Defines the cycle, speed, position and collision box.
- `Assets/Scripts/Rules/RollingBoulder.cs.meta` — Unity import or asset identity metadata.
- `Assets/Scripts/Rules/RollingBoulderRun.cs` — Records whether a particular roll was skipped.
- `Assets/Scripts/Rules/RollingBoulderRun.cs.meta` — Unity import or asset identity metadata.
- `Assets/Scripts/Selection/DifficultySelectionController.cs` — Opens the book review only after the landform is complete.
- `Assets/Tests/EditMode/AchievementTests.cs` — Checks the expanded badge order and tiers and preserves existing badge rules.
- `Assets/Tests/EditMode/CollectionTests.cs` — Checks the original item order and expanded Collection count.
- `Assets/Tests/EditMode/EpisodeCompletionTests.cs` — Tests the clear requirement, saved unlocks, map order, stars and animation decision.
- `Assets/Tests/EditMode/EpisodeCompletionTests.cs.meta` — Unity import or asset identity metadata.
- `Assets/Tests/EditMode/HardCheckpointTests.cs` — Tests the third flag and the two new optional object lists.
- `Assets/Tests/EditMode/HardCheckpointTests.cs.meta` — Unity import or asset identity metadata.
- `Assets/Tests/EditMode/HardProfileTests.cs` — Tests the three pages and four new badge conditions.
- `Assets/Tests/EditMode/HardProfileTests.cs.meta` — Unity import or asset identity metadata.
- `Assets/Tests/EditMode/IceSlideTests.cs` — Tests ice pace, turning, stopping, sound threshold and the zone edges.
- `Assets/Tests/EditMode/IceSlideTests.cs.meta` — Unity import or asset identity metadata.
- `Assets/Tests/EditMode/LandformSummaryTests.cs` — Tests the Mountains summary and future landform placeholders.
- `Assets/Tests/EditMode/LandformSummaryTests.cs.meta` — Unity import or asset identity metadata.
- `Assets/Tests/EditMode/LevelCatalogTests.cs` — Updates the unavailable Hard level test for the seven new entries.
- `Assets/Tests/EditMode/MountainsHardTests.cs` — Tests Hard trail dimensions, contents, timing and safe clearances.
- `Assets/Tests/EditMode/MountainsHardTests.cs.meta` — Unity import or asset identity metadata.
- `Assets/Tests/EditMode/MountainsNormalTests.cs` — Keeps the Normal kinds and order assertions as the catalog grows.
- `Assets/Tests/EditMode/NormalProfileTests.cs` — Keeps the original 20-kind paging case alongside the new 34-kind case.
- `Assets/Tests/EditMode/PlayerProgressTests.cs` — Checks the fresh-profile starting state under the new landform rule.
- `Assets/Tests/EditMode/RollingBoulderTests.cs` — Tests timing, hit bounds, offsets and one skip decision per cycle.
- `Assets/Tests/EditMode/RollingBoulderTests.cs.meta` — Unity import or asset identity metadata.
- `CONTEXT.md` — Defines the six Hard and landform terms in the game glossary.
- `ProjectSettings/EditorBuildSettings.asset` — Registers the new review directly after Lesson and Hard 1–7 after Normal 5.
