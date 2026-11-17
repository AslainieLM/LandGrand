# Hills Normal 1–5 handover

Built and verified on 2026-11-17 in Unity 6000.6.8f1. All five Hills Normal scenes are enabled in Build Settings. The generators produced 75 sprite PNGs, 30 backdrop PNGs, four audio files and three reviewed previews. River Gorge uses the existing 16×12 fern at the specified path.

## Verification

| Ticket | EditMode passed | Added cases |
|---|---:|---:|
| 01 Rules | 334 | 38 |
| 02 Sprites | 334 | 0 |
| 03 Backdrops | 334 | 0 |
| 04 Audio | 334 | 0 |
| 05 Catalog | 352 | 18 |
| 06 Runtime and builders | 360 | 8 |
| 07 Final build | 360 | 0 |

Final run: **360 passed, 0 failed, 0 skipped** in `Temp/editmode.xml`. The run before Hills Normal had 296 passing tests. All three generators reproduced the reviewed previews. All six builders completed, with zero missing assets and zero refused props. Unity generated 23 level scenes. Lesson contains the five new illustrations and seven kind pictures, and Profile contains all seven kind pictures. Callao references `amb_cave_drips`. The other four Hills Normal trails reference `amb_stream_normal`.

A batch Play Mode smoke check opened all five scenes with progress kept in memory. It verified the N1 raft carrying, pause, recovery, safe pool floor and checkpoint return from a water pit. It checked the N2 24 px bat push, N3 running at 42 px/s against the current and launching at 84 px/s, and the N4 mix. In N5 it boarded a rising basket, paused and resumed while riding, rode it down, landed on the crossing ledge, opened the zeolite card and crossed the 160 px gap. The temporary smoke runner was removed after the check.

The 18 earlier level scenes were reserialized by the level builder. Their fixed geometry, named objects, original asset references and ambience match the baseline. Shared pickup panels now include the seven new item picture references. Existing PNG, WAV and OGG files have an empty Git diff.

### Review results

- **Standards.** Float transition state now lives in plain C# with tests. Balloon startup checks its serialized references. The added code was checked for D-033 comments.
- **Spec.** The motor compares both ends of a moving-top step, records actual landing support, and keeps riders aboard across pause and resume. The runtime checks and eight NUnit regressions cover the resulting edge cases.

## Approved choices and numeric differences

- All level dimensions, pickups, hazards, periods, phases, ledge heights, and checkpoint placements match the authored layouts. Hills Normal uses **72 px/s on the ground** as confirmed in chat, compared with the existing motor's 84 px/s default. The 30 px/s opposing current produces the specified 42 px/s walk. Air speed is 84 px/s.
- Seven fixed decoration x positions moved to the nearest places allowed by `HillsPropFits`. The table lists the original and final pixel positions. Ties took the left spot. Repeating decoration stays near the approximate spacing in the spec and fits around trail objects.

| Level | Decoration | Original x | Final x | Clearance |
|---|---|---:|---:|---|
| N1 | Seabird | 2600 | 2585 | Pickup |
| N4 | Bulul | 1500 | 1493 | Boulder |
| N4 | Bulul | 4800 | 4781 | Checkpoint flag |
| N5 | Cone house | 900 | 883 | Practice basket and pickup |
| N5 | Cone house | 5600 | 5633 | Secret ledge and pickup |
| N5 | Far balloon | 4200 | 4137 | Basket route, cliff bridge and pit |
| N5 | Far balloon | 6500 | 6493 | Spikes |

- Water pits use the Normal checkpoint return, as confirmed in chat. This resolves the short-return wording in the original planning notes. The practice pool is safe.
- Hill Collector still uses the full shelf. A save with the original ten kinds needs the seven new finds to earn it on the expanded shelf.
- The spec leaves several animation rates open. Mist, rice and distant balloon frames use 0.3 s, 1.2 s and 0.75 s per frame. A balloon's burner sound is heard within 240 px, half the reference view width.

## Previews

- [Sprite art](art-preview.png)
- [Backgrounds](backdrop-preview.png)
- [Audio waveforms and spectra](audio-preview.png)

The art sheets were opened and reviewed. The final generator run reproduced them byte for byte. The cave loop seam was measured on both decoded channels and is near the size of ordinary nearby sample changes.

## Checks remaining in the Editor

Run the manual play checklist below for complete trails, all 35 pickups and secret routes, the Selection and Profile flows, badge notices, in-game sound and visual checks. The smoke checks exercised selected locations. A device build and teacher review of the draft text were outside this check.

## Rebuild commands

All six builders ran for this handover. To repeat them in the Editor, run the following under **LandGrand** in order.

1. **Build Levels**
2. **Build Lesson Scene**
3. **Build Selection Scenes**
4. **Build Landform Map Scene**
5. **Build Landform Description Scene**
6. **Build Profile Scene**

Batch commands for PowerShell, with Unity closed:

```powershell
Set-Location -LiteralPath 'D:\LandGrand\LandGrand'
python Tools/Art/make_hills_normal_art.py
python Tools/Art/make_hills_normal_backdrops.py
python Tools/Audio/make_hills_normal_audio.py
$unityExe = 'C:/Program Files/Unity/Hub/Editor/6000.6.8f1/Editor/Unity.exe'
Start-Process -FilePath $unityExe -ArgumentList '-batchmode -projectPath . -executeMethod LevelBuilder.BuildAll -quit -logFile Temp/build-levels.log' -WindowStyle Hidden -Wait
Start-Process -FilePath $unityExe -ArgumentList '-batchmode -projectPath . -executeMethod LessonBuilder.Build -quit -logFile Temp/build-lesson.log' -WindowStyle Hidden -Wait
Start-Process -FilePath $unityExe -ArgumentList '-batchmode -projectPath . -executeMethod SelectionBuilder.Build -quit -logFile Temp/build-selection.log' -WindowStyle Hidden -Wait
Start-Process -FilePath $unityExe -ArgumentList '-batchmode -projectPath . -executeMethod LandformMapBuilder.Build -quit -logFile Temp/build-map.log' -WindowStyle Hidden -Wait
Start-Process -FilePath $unityExe -ArgumentList '-batchmode -projectPath . -executeMethod LandformDescriptionBuilder.Build -quit -logFile Temp/build-description.log' -WindowStyle Hidden -Wait
Start-Process -FilePath $unityExe -ArgumentList '-batchmode -projectPath . -executeMethod ProfileBuilder.Build -quit -logFile Temp/build-profile.log' -WindowStyle Hidden -Wait
Start-Process -FilePath $unityExe -ArgumentList '-batchmode -projectPath . -runTests -testPlatform EditMode -testResults Temp/editmode.xml -logFile Temp/editmode.log' -WindowStyle Hidden -Wait
```

## Play checklist for the user

- [ ] After Hills Easy 3, Next opens Hills Normal 1; the Hills Normal trail shows five stones
- [ ] Each Normal level shows its banner, lesson, theme, background and goal (Boat Dock, Cave Mouth, Falls Landing, Ifugao House, Balloon Field)
- [ ] N1: floats shake, sink after a short wait and rise again. Practise over the safe pool. A fall into a water pit returns you to the furthest reached flag, or the start before the first flag.
- [ ] N2: bats loop gently; a touch pushes you back a little and never to a flag; the cave plays drips, not birds; the bat sign is visible
- [ ] N3: currents push the way their ripples point; the one against you is still walkable; jumps are unchanged
- [ ] N4: one float, one bat loop and one current, each fair
- [ ] N5: balloons rise and fall with a burner whoosh; one balloon is needed for the wide gap; only the basket is solid
- [ ] Each level has its pickups, and the secret spot is reachable; the seven new kinds show their draft text
- [ ] Collection: the Hills tab shows 17 kinds, and "Found here" counts them
- [ ] Philippine Hills (after N1–N4) and Balloon Rider (after N5) can be earned; "New badge!" shows
- [ ] Mountains and Hills Easy still play as before

## File inventory

321 added or changed project files, including Unity metadata. Paths are relative to the project root.

- `.scratch/hills-normal/art-preview.png` Shows every sprite at 3× with its name.
- `.scratch/hills-normal/audio-preview.png` Shows measured waveforms and spectra from the decoded assets.
- `.scratch/hills-normal/backdrop-preview.png` Shows each complete 480×270 background plus all of its layers.
- `.scratch/hills-normal/HANDOVER.md` Records the builds, tests, decisions, preview links, full inventory and play checklist.
- `.scratch/hills-normal/issues/01-rules-float-bat-current-balloon.md` Records the ticket’s completed work and verification.
- `.scratch/hills-normal/issues/02-art-tiles-objects-props-items.md` Records the ticket’s completed work and verification.
- `.scratch/hills-normal/issues/03-backdrops-five-themes.md` Records the ticket’s completed work and verification.
- `.scratch/hills-normal/issues/04-audio-cave-splash-bat-burner.md` Records the ticket’s completed work and verification.
- `.scratch/hills-normal/issues/05-level-data-kinds-themes.md` Records the ticket’s completed work and verification.
- `.scratch/hills-normal/issues/06-play-parts-and-builders.md` Records the ticket’s completed work and verification.
- `.scratch/hills-normal/issues/07-final-check-and-handover.md` Records the ticket’s completed work and verification.
- `Assets/Art/Items/Sprites/aragonite.png` The 24×24 `aragonite` collectible picture.
- `Assets/Art/Items/Sprites/aragonite.png.meta` Unity import record for `aragonite.png`.
- `Assets/Art/Items/Sprites/coquina.png` The 24×24 `coquina` collectible picture.
- `Assets/Art/Items/Sprites/coquina.png.meta` Unity import record for `coquina.png`.
- `Assets/Art/Items/Sprites/dripstone.png` The 24×24 `dripstone` collectible picture.
- `Assets/Art/Items/Sprites/dripstone.png.meta` Unity import record for `dripstone.png`.
- `Assets/Art/Items/Sprites/flowstone.png` The 24×24 `flowstone` collectible picture.
- `Assets/Art/Items/Sprites/flowstone.png.meta` Unity import record for `flowstone.png`.
- `Assets/Art/Items/Sprites/ignimbrite.png` The 24×24 `ignimbrite` collectible picture.
- `Assets/Art/Items/Sprites/ignimbrite.png.meta` Unity import record for `ignimbrite.png`.
- `Assets/Art/Items/Sprites/volcanic_breccia.png` The 24×24 `volcanic_breccia` collectible picture.
- `Assets/Art/Items/Sprites/volcanic_breccia.png.meta` Unity import record for `volcanic_breccia.png`.
- `Assets/Art/Items/Sprites/zeolite.png` The 24×24 `zeolite` collectible picture.
- `Assets/Art/Items/Sprites/zeolite.png.meta` Unity import record for `zeolite.png`.
- `Assets/Art/Lesson/Sprites/lesson_banaue.png` The 120×82 `lesson_banaue` lesson illustration.
- `Assets/Art/Lesson/Sprites/lesson_banaue.png.meta` Unity import record for `lesson_banaue.png`.
- `Assets/Art/Lesson/Sprites/lesson_callao_cave.png` The 120×82 `lesson_callao_cave` lesson illustration.
- `Assets/Art/Lesson/Sprites/lesson_callao_cave.png.meta` Unity import record for `lesson_callao_cave.png`.
- `Assets/Art/Lesson/Sprites/lesson_cappadocia.png` The 120×82 `lesson_cappadocia` lesson illustration.
- `Assets/Art/Lesson/Sprites/lesson_cappadocia.png.meta` Unity import record for `lesson_cappadocia.png`.
- `Assets/Art/Lesson/Sprites/lesson_hundred_islands.png` The 120×82 `lesson_hundred_islands` lesson illustration.
- `Assets/Art/Lesson/Sprites/lesson_hundred_islands.png.meta` Unity import record for `lesson_hundred_islands.png`.
- `Assets/Art/Lesson/Sprites/lesson_pagsanjan.png` The 120×82 `lesson_pagsanjan` lesson illustration.
- `Assets/Art/Lesson/Sprites/lesson_pagsanjan.png.meta` Unity import record for `lesson_pagsanjan.png`.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys.meta` Unity import record for `FairyChimneys`.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L0.png` Layer 0 of the FairyChimneys seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L0.png.meta` Unity import record for `L0.png`.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L1.png` Layer 1 of the FairyChimneys seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L1.png.meta` Unity import record for `L1.png`.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L2.png` Layer 2 of the FairyChimneys seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L2.png.meta` Unity import record for `L2.png`.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L3.png` Layer 3 of the FairyChimneys seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L3.png.meta` Unity import record for `L3.png`.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L4.png` Layer 4 of the FairyChimneys seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L4.png.meta` Unity import record for `L4.png`.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L5.png` Layer 5 of the FairyChimneys seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L5.png.meta` Unity import record for `L5.png`.
- `Assets/Art/Level/Sprites/Backdrop/IslandHills.meta` Unity import record for `IslandHills`.
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L0.png` Layer 0 of the IslandHills seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L0.png.meta` Unity import record for `L0.png`.
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L1.png` Layer 1 of the IslandHills seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L1.png.meta` Unity import record for `L1.png`.
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L2.png` Layer 2 of the IslandHills seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L2.png.meta` Unity import record for `L2.png`.
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L3.png` Layer 3 of the IslandHills seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L3.png.meta` Unity import record for `L3.png`.
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L4.png` Layer 4 of the IslandHills seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L4.png.meta` Unity import record for `L4.png`.
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L5.png` Layer 5 of the IslandHills seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L5.png.meta` Unity import record for `L5.png`.
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave.meta` Unity import record for `LimestoneCave`.
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L0.png` Layer 0 of the LimestoneCave seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L0.png.meta` Unity import record for `L0.png`.
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L1.png` Layer 1 of the LimestoneCave seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L1.png.meta` Unity import record for `L1.png`.
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L2.png` Layer 2 of the LimestoneCave seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L2.png.meta` Unity import record for `L2.png`.
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L3.png` Layer 3 of the LimestoneCave seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L3.png.meta` Unity import record for `L3.png`.
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L4.png` Layer 4 of the LimestoneCave seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L4.png.meta` Unity import record for `L4.png`.
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L5.png` Layer 5 of the LimestoneCave seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L5.png.meta` Unity import record for `L5.png`.
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces.meta` Unity import record for `RiceTerraces`.
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L0.png` Layer 0 of the RiceTerraces seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L0.png.meta` Unity import record for `L0.png`.
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L1.png` Layer 1 of the RiceTerraces seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L1.png.meta` Unity import record for `L1.png`.
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L2.png` Layer 2 of the RiceTerraces seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L2.png.meta` Unity import record for `L2.png`.
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L3.png` Layer 3 of the RiceTerraces seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L3.png.meta` Unity import record for `L3.png`.
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L4.png` Layer 4 of the RiceTerraces seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L4.png.meta` Unity import record for `L4.png`.
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L5.png` Layer 5 of the RiceTerraces seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L5.png.meta` Unity import record for `L5.png`.
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge.meta` Unity import record for `RiverGorge`.
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L0.png` Layer 0 of the RiverGorge seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L0.png.meta` Unity import record for `L0.png`.
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L1.png` Layer 1 of the RiverGorge seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L1.png.meta` Unity import record for `L1.png`.
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L2.png` Layer 2 of the RiverGorge seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L2.png.meta` Unity import record for `L2.png`.
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L3.png` Layer 3 of the RiverGorge seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L3.png.meta` Unity import record for `L3.png`.
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L4.png` Layer 4 of the RiverGorge seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L4.png.meta` Unity import record for `L4.png`.
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L5.png` Layer 5 of the RiverGorge seamless backdrop.
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L5.png.meta` Unity import record for `L5.png`.
- `Assets/Art/Level/Sprites/balloon_field.png` The `balloon_field` level sprite.
- `Assets/Art/Level/Sprites/balloon_field.png.meta` Unity import record for `balloon_field.png`.
- `Assets/Art/Level/Sprites/balloon_flame_00.png` The `balloon_flame_00` level sprite.
- `Assets/Art/Level/Sprites/balloon_flame_00.png.meta` Unity import record for `balloon_flame_00.png`.
- `Assets/Art/Level/Sprites/balloon_flame_01.png` The `balloon_flame_01` level sprite.
- `Assets/Art/Level/Sprites/balloon_flame_01.png.meta` Unity import record for `balloon_flame_01.png`.
- `Assets/Art/Level/Sprites/balloon.png` The `balloon` level sprite.
- `Assets/Art/Level/Sprites/balloon.png.meta` Unity import record for `balloon.png`.
- `Assets/Art/Level/Sprites/bat_00.png` The `bat_00` level sprite.
- `Assets/Art/Level/Sprites/bat_00.png.meta` Unity import record for `bat_00.png`.
- `Assets/Art/Level/Sprites/bat_01.png` The `bat_01` level sprite.
- `Assets/Art/Level/Sprites/bat_01.png.meta` Unity import record for `bat_01.png`.
- `Assets/Art/Level/Sprites/bat_02.png` The `bat_02` level sprite.
- `Assets/Art/Level/Sprites/bat_02.png.meta` Unity import record for `bat_02.png`.
- `Assets/Art/Level/Sprites/boat_dock.png` The `boat_dock` level sprite.
- `Assets/Art/Level/Sprites/boat_dock.png.meta` Unity import record for `boat_dock.png`.
- `Assets/Art/Level/Sprites/boulder_big_cave.png` The `boulder_big_cave` level sprite.
- `Assets/Art/Level/Sprites/boulder_big_cave.png.meta` Unity import record for `boulder_big_cave.png`.
- `Assets/Art/Level/Sprites/boulder_big_chimney.png` The `boulder_big_chimney` level sprite.
- `Assets/Art/Level/Sprites/boulder_big_chimney.png.meta` Unity import record for `boulder_big_chimney.png`.
- `Assets/Art/Level/Sprites/boulder_big_gorge.png` The `boulder_big_gorge` level sprite.
- `Assets/Art/Level/Sprites/boulder_big_gorge.png.meta` Unity import record for `boulder_big_gorge.png`.
- `Assets/Art/Level/Sprites/boulder_big_island.png` The `boulder_big_island` level sprite.
- `Assets/Art/Level/Sprites/boulder_big_island.png.meta` Unity import record for `boulder_big_island.png`.
- `Assets/Art/Level/Sprites/boulder_big_terrace.png` The `boulder_big_terrace` level sprite.
- `Assets/Art/Level/Sprites/boulder_big_terrace.png.meta` Unity import record for `boulder_big_terrace.png`.
- `Assets/Art/Level/Sprites/boulder_small_cave.png` The `boulder_small_cave` level sprite.
- `Assets/Art/Level/Sprites/boulder_small_cave.png.meta` Unity import record for `boulder_small_cave.png`.
- `Assets/Art/Level/Sprites/boulder_small_chimney.png` The `boulder_small_chimney` level sprite.
- `Assets/Art/Level/Sprites/boulder_small_chimney.png.meta` Unity import record for `boulder_small_chimney.png`.
- `Assets/Art/Level/Sprites/boulder_small_gorge.png` The `boulder_small_gorge` level sprite.
- `Assets/Art/Level/Sprites/boulder_small_gorge.png.meta` Unity import record for `boulder_small_gorge.png`.
- `Assets/Art/Level/Sprites/boulder_small_island.png` The `boulder_small_island` level sprite.
- `Assets/Art/Level/Sprites/boulder_small_island.png.meta` Unity import record for `boulder_small_island.png`.
- `Assets/Art/Level/Sprites/boulder_small_terrace.png` The `boulder_small_terrace` level sprite.
- `Assets/Art/Level/Sprites/boulder_small_terrace.png.meta` Unity import record for `boulder_small_terrace.png`.
- `Assets/Art/Level/Sprites/cave_mouth.png` The `cave_mouth` level sprite.
- `Assets/Art/Level/Sprites/cave_mouth.png.meta` Unity import record for `cave_mouth.png`.
- `Assets/Art/Level/Sprites/cave_soil.png` The `cave_soil` level sprite.
- `Assets/Art/Level/Sprites/cave_soil.png.meta` Unity import record for `cave_soil.png`.
- `Assets/Art/Level/Sprites/cave_top.png` The `cave_top` level sprite.
- `Assets/Art/Level/Sprites/cave_top.png.meta` Unity import record for `cave_top.png`.
- `Assets/Art/Level/Sprites/chimney_soil.png` The `chimney_soil` level sprite.
- `Assets/Art/Level/Sprites/chimney_soil.png.meta` Unity import record for `chimney_soil.png`.
- `Assets/Art/Level/Sprites/chimney_top.png` The `chimney_top` level sprite.
- `Assets/Art/Level/Sprites/chimney_top.png.meta` Unity import record for `chimney_top.png`.
- `Assets/Art/Level/Sprites/current_mark_00.png` The `current_mark_00` level sprite.
- `Assets/Art/Level/Sprites/current_mark_00.png.meta` Unity import record for `current_mark_00.png`.
- `Assets/Art/Level/Sprites/current_mark_01.png` The `current_mark_01` level sprite.
- `Assets/Art/Level/Sprites/current_mark_01.png.meta` Unity import record for `current_mark_01.png`.
- `Assets/Art/Level/Sprites/current_top.png` The `current_top` level sprite.
- `Assets/Art/Level/Sprites/current_top.png.meta` Unity import record for `current_top.png`.
- `Assets/Art/Level/Sprites/falls_landing.png` The `falls_landing` level sprite.
- `Assets/Art/Level/Sprites/falls_landing.png.meta` Unity import record for `falls_landing.png`.
- `Assets/Art/Level/Sprites/float_raft.png` The `float_raft` level sprite.
- `Assets/Art/Level/Sprites/float_raft.png.meta` Unity import record for `float_raft.png`.
- `Assets/Art/Level/Sprites/float_ripple_00.png` The `float_ripple_00` level sprite.
- `Assets/Art/Level/Sprites/float_ripple_00.png.meta` Unity import record for `float_ripple_00.png`.
- `Assets/Art/Level/Sprites/float_ripple_01.png` The `float_ripple_01` level sprite.
- `Assets/Art/Level/Sprites/float_ripple_01.png.meta` Unity import record for `float_ripple_01.png`.
- `Assets/Art/Level/Sprites/gorge_soil.png` The `gorge_soil` level sprite.
- `Assets/Art/Level/Sprites/gorge_soil.png.meta` Unity import record for `gorge_soil.png`.
- `Assets/Art/Level/Sprites/gorge_top.png` The `gorge_top` level sprite.
- `Assets/Art/Level/Sprites/gorge_top.png.meta` Unity import record for `gorge_top.png`.
- `Assets/Art/Level/Sprites/ifugao_house.png` The `ifugao_house` level sprite.
- `Assets/Art/Level/Sprites/ifugao_house.png.meta` Unity import record for `ifugao_house.png`.
- `Assets/Art/Level/Sprites/island_soil.png` The `island_soil` level sprite.
- `Assets/Art/Level/Sprites/island_soil.png.meta` Unity import record for `island_soil.png`.
- `Assets/Art/Level/Sprites/island_top.png` The `island_top` level sprite.
- `Assets/Art/Level/Sprites/island_top.png.meta` Unity import record for `island_top.png`.
- `Assets/Art/Level/Sprites/ledge_cave.png` The `ledge_cave` level sprite.
- `Assets/Art/Level/Sprites/ledge_cave.png.meta` Unity import record for `ledge_cave.png`.
- `Assets/Art/Level/Sprites/ledge_chimney.png` The `ledge_chimney` level sprite.
- `Assets/Art/Level/Sprites/ledge_chimney.png.meta` Unity import record for `ledge_chimney.png`.
- `Assets/Art/Level/Sprites/ledge_gorge.png` The `ledge_gorge` level sprite.
- `Assets/Art/Level/Sprites/ledge_gorge.png.meta` Unity import record for `ledge_gorge.png`.
- `Assets/Art/Level/Sprites/ledge_island.png` The `ledge_island` level sprite.
- `Assets/Art/Level/Sprites/ledge_island.png.meta` Unity import record for `ledge_island.png`.
- `Assets/Art/Level/Sprites/ledge_terrace.png` The `ledge_terrace` level sprite.
- `Assets/Art/Level/Sprites/ledge_terrace.png.meta` Unity import record for `ledge_terrace.png`.
- `Assets/Art/Level/Sprites/Props/bangka_boatman.png` The `bangka_boatman` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/bangka_boatman.png.meta` Unity import record for `bangka_boatman.png`.
- `Assets/Art/Level/Sprites/Props/bangka.png` The `bangka` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/bangka.png.meta` Unity import record for `bangka.png`.
- `Assets/Art/Level/Sprites/Props/bat_sign.png` The `bat_sign` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/bat_sign.png.meta` Unity import record for `bat_sign.png`.
- `Assets/Art/Level/Sprites/Props/bulul.png` The `bulul` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/bulul.png.meta` Unity import record for `bulul.png`.
- `Assets/Art/Level/Sprites/Props/chapel_glow_00.png` The `chapel_glow_00` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/chapel_glow_00.png.meta` Unity import record for `chapel_glow_00.png`.
- `Assets/Art/Level/Sprites/Props/chapel_glow_01.png` The `chapel_glow_01` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/chapel_glow_01.png.meta` Unity import record for `chapel_glow_01.png`.
- `Assets/Art/Level/Sprites/Props/cone_house.png` The `cone_house` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/cone_house.png.meta` Unity import record for `cone_house.png`.
- `Assets/Art/Level/Sprites/Props/dry_shrub.png` The `dry_shrub` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/dry_shrub.png.meta` Unity import record for `dry_shrub.png`.
- `Assets/Art/Level/Sprites/Props/falls_mist_00.png` The `falls_mist_00` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/falls_mist_00.png.meta` Unity import record for `falls_mist_00.png`.
- `Assets/Art/Level/Sprites/Props/falls_mist_01.png` The `falls_mist_01` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/falls_mist_01.png.meta` Unity import record for `falls_mist_01.png`.
- `Assets/Art/Level/Sprites/Props/far_balloon_00.png` The `far_balloon_00` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/far_balloon_00.png.meta` Unity import record for `far_balloon_00.png`.
- `Assets/Art/Level/Sprites/Props/far_balloon_01.png` The `far_balloon_01` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/far_balloon_01.png.meta` Unity import record for `far_balloon_01.png`.
- `Assets/Art/Level/Sprites/Props/farmer_00.png` The `farmer_00` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/farmer_00.png.meta` Unity import record for `farmer_00.png`.
- `Assets/Art/Level/Sprites/Props/farmer_01.png` The `farmer_01` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/farmer_01.png.meta` Unity import record for `farmer_01.png`.
- `Assets/Art/Level/Sprites/Props/mangrove.png` The `mangrove` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/mangrove.png.meta` Unity import record for `mangrove.png`.
- `Assets/Art/Level/Sprites/Props/rice_stalk_00.png` The `rice_stalk_00` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/rice_stalk_00.png.meta` Unity import record for `rice_stalk_00.png`.
- `Assets/Art/Level/Sprites/Props/rice_stalk_01.png` The `rice_stalk_01` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/rice_stalk_01.png.meta` Unity import record for `rice_stalk_01.png`.
- `Assets/Art/Level/Sprites/Props/seabird_00.png` The `seabird_00` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/seabird_00.png.meta` Unity import record for `seabird_00.png`.
- `Assets/Art/Level/Sprites/Props/seabird_01.png` The `seabird_01` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/seabird_01.png.meta` Unity import record for `seabird_01.png`.
- `Assets/Art/Level/Sprites/Props/stalagmite.png` The `stalagmite` living-prop sprite.
- `Assets/Art/Level/Sprites/Props/stalagmite.png.meta` Unity import record for `stalagmite.png`.
- `Assets/Art/Level/Sprites/terrace_soil.png` The `terrace_soil` level sprite.
- `Assets/Art/Level/Sprites/terrace_soil.png.meta` Unity import record for `terrace_soil.png`.
- `Assets/Art/Level/Sprites/terrace_top.png` The `terrace_top` level sprite.
- `Assets/Art/Level/Sprites/terrace_top.png.meta` Unity import record for `terrace_top.png`.
- `Assets/Art/Level/Sprites/water_fill.png` The `water_fill` level sprite.
- `Assets/Art/Level/Sprites/water_fill.png.meta` Unity import record for `water_fill.png`.
- `Assets/Audio/amb_cave_drips.ogg` The 24.000 s cave loop, measured at −21.9 dB mean.
- `Assets/Audio/amb_cave_drips.ogg.meta` Unity import record for `amb_cave_drips.ogg`.
- `Assets/Audio/sfx_bat_flap.wav` The 0.250 s wing sound, measured at −26.0 dB mean.
- `Assets/Audio/sfx_bat_flap.wav.meta` Unity import record for `sfx_bat_flap.wav`.
- `Assets/Audio/sfx_burner.wav` The 0.500 s balloon sound, measured at −24.0 dB mean.
- `Assets/Audio/sfx_burner.wav.meta` Unity import record for `sfx_burner.wav`.
- `Assets/Audio/sfx_splash.wav` The 0.300 s water sound, measured at −24.0 dB mean.
- `Assets/Audio/sfx_splash.wav.meta` Unity import record for `sfx_splash.wav`.
- `Assets/Editor/HardLevelObjects.cs` Uses the current top tile over a current strip.
- `Assets/Editor/HillsLevelObjects.cs` Builds the new objects, views and sounds.
- `Assets/Editor/HillsLevelObjects.cs.meta` Unity import record for `HillsLevelObjects.cs`.
- `Assets/Editor/LevelBuilder.cs` Builds water gaps and pools and assigns art and ambience.
- `Assets/Editor/LevelObjects.cs` Adds balloons and rafts to the one-way solid list.
- `Assets/Editor/LevelThemes.cs` Adds five theme entries and living props checked by HillsPropFits.
- `Assets/Editor/PixelSceneKit.cs` Loads the chosen ambience and names the new art and sound generators in diagnostics.
- `Assets/Fonts/Andika SDF.asset` Unity refreshed the reading font atlas while building and exercising the scenes.
- `Assets/Fonts/Micro5 2x.asset` Unity refreshed the pixel font atlas while building and exercising the scenes.
- `Assets/Scenes/DifficultySelection.unity` Rebuilt the shared difficulty selection scene.
- `Assets/Scenes/HillsEasy1.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/HillsEasy2.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/HillsEasy3.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/HillsNormal1.unity` Hundred Islands scene generated by LevelBuilder.
- `Assets/Scenes/HillsNormal1.unity.meta` Unity import record for `HillsNormal1.unity`.
- `Assets/Scenes/HillsNormal2.unity` Callao Cave scene generated by LevelBuilder.
- `Assets/Scenes/HillsNormal2.unity.meta` Unity import record for `HillsNormal2.unity`.
- `Assets/Scenes/HillsNormal3.unity` Pagsanjan Gorge scene generated by LevelBuilder.
- `Assets/Scenes/HillsNormal3.unity.meta` Unity import record for `HillsNormal3.unity`.
- `Assets/Scenes/HillsNormal4.unity` Banaue Rice Terraces scene generated by LevelBuilder.
- `Assets/Scenes/HillsNormal4.unity.meta` Unity import record for `HillsNormal4.unity`.
- `Assets/Scenes/HillsNormal5.unity` Cappadocia scene generated by LevelBuilder.
- `Assets/Scenes/HillsNormal5.unity.meta` Unity import record for `HillsNormal5.unity`.
- `Assets/Scenes/LandformDescription.unity` Rebuilt the shared description scene.
- `Assets/Scenes/LandformMap.unity` Rebuilt the landform map.
- `Assets/Scenes/Lesson.unity` Rebuilt with the five new lesson illustrations and seven kind sprites.
- `Assets/Scenes/LevelSelection.unity` Rebuilt the shared level selection scene.
- `Assets/Scenes/MountainsEasy1.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsEasy2.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsEasy3.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsHard1.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsHard2.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsHard3.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsHard4.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsHard5.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsHard6.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsHard7.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsNormal1.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsNormal2.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsNormal3.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsNormal4.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/MountainsNormal5.unity` Rebuilt by Unity. Fixed geometry, named scene objects and original asset references match the baseline.
- `Assets/Scenes/Profile.unity` Rebuilt with all seven new Collection sprites.
- `Assets/Scripts/Play/BackdropLayer.cs` Lets the new theme entries set their cloud drift to the level clock.
- `Assets/Scripts/Play/BalloonView.cs` Moves the basket and its burner on the level clock.
- `Assets/Scripts/Play/BalloonView.cs.meta` Unity import record for `BalloonView.cs`.
- `Assets/Scripts/Play/BatFlock.cs` Animates bat loops, handles contacts and flashes the explorer.
- `Assets/Scripts/Play/BatFlock.cs.meta` Unity import record for `BatFlock.cs`.
- `Assets/Scripts/Play/CurrentWatcher.cs` Draws moving river marks and applies the grounded push.
- `Assets/Scripts/Play/CurrentWatcher.cs.meta` Unity import record for `CurrentWatcher.cs`.
- `Assets/Scripts/Play/DynamicSolid.cs` Reports whether the previous frame had a usable support top.
- `Assets/Scripts/Play/ExplorerMotor.cs` Carries the rider vertically, resolves one-way landing and applies the two new pushes.
- `Assets/Scripts/Play/FloatView.cs` Shows and moves each one-way raft.
- `Assets/Scripts/Play/FloatView.cs.meta` Unity import record for `FloatView.cs`.
- `Assets/Scripts/Play/FloatWatcher.cs` Starts float views and plays water-entry or submergence splashes.
- `Assets/Scripts/Play/FloatWatcher.cs.meta` Unity import record for `FloatWatcher.cs`.
- `Assets/Scripts/Play/LevelController.cs` Passes the authored ground speed to the motor during setup.
- `Assets/Scripts/Play/LevelGeometry.cs` Exposes dynamic tops for ground queries, with the motor recording actual landing support.
- `Assets/Scripts/Play/LevelPropAnimation.cs` Animates the new decoration frames using level time.
- `Assets/Scripts/Play/LevelPropAnimation.cs.meta` Unity import record for `LevelPropAnimation.cs`.
- `Assets/Scripts/Rules/Bat.cs` Defines ellipse motion, body contact, push direction and rest.
- `Assets/Scripts/Rules/Bat.cs.meta` Unity import record for `Bat.cs`.
- `Assets/Scripts/Rules/CurrentZone.cs` Applies the authored 30 px/s current on flat ground.
- `Assets/Scripts/Rules/CurrentZone.cs.meta` Unity import record for `CurrentZone.cs`.
- `Assets/Scripts/Rules/LandformAudio.cs` Chooses a level's own ambience when supplied and resolves the generator.
- `Assets/Scripts/Rules/LevelData.cs` Appends seven draft kinds and describes five full trails and their lessons as catalog data.
- `Assets/Scripts/Rules/LevelLayout.cs` Appends five themes and holds optional floats, bats, currents, pools, ambience and ground speed.
- `Assets/Scripts/Rules/MovingPlatform.cs` Supports vertical baskets while preserving the existing horizontal route.
- `Assets/Scripts/Rules/OneWayTop.cs` Checks relative landing from above, including reappearing tops.
- `Assets/Scripts/Rules/OneWayTop.cs.meta` Unity import record for `OneWayTop.cs`.
- `Assets/Scripts/Rules/SinkingFloat.cs` Keeps attempt state and the float timing rules independent of Unity.
- `Assets/Scripts/Rules/SinkingFloat.cs.meta` Unity import record for `SinkingFloat.cs`.
- `Assets/Tests/EditMode/CollectionTests.cs` Verifies the 17-kind Hills shelf and 44 unique kinds overall.
- `Assets/Tests/EditMode/HillsBadgeTests.cs` Checks the full-shelf Hill Collector rule with the expanded shelf.
- `Assets/Tests/EditMode/HillsEasyLevelsTests.cs` Verifies the exact ten kinds taught in Hills Easy.
- `Assets/Tests/EditMode/HillsNormalLevelsTests.cs` Covers the catalog, practice spots, Normal checkpoints, shelf and older layout defaults.
- `Assets/Tests/EditMode/HillsNormalLevelsTests.cs.meta` Unity import record for `HillsNormalLevelsTests.cs`.
- `Assets/Tests/EditMode/HillsNormalRulesTests.cs` Covers float timing, raft support, bat contact, current footing, vertical travel and ambience.
- `Assets/Tests/EditMode/HillsNormalRulesTests.cs.meta` Unity import record for `HillsNormalRulesTests.cs`.
- `Assets/Tests/EditMode/LevelCatalogTests.cs` Verifies Hills Normal exists and Hills Hard still has no level data.
- `Assets/Tests/EditMode/MountainsNormalTests.cs` Keeps the original five-level check scoped to Mountains.
- `Assets/Tests/EditMode/NextPlayableTests.cs` Checks the Easy-to-Normal transition and the end of the playable Hills trails.
- `CONTEXT.md` Adds the five requested glossary terms.
- `ProjectSettings/EditorBuildSettings.asset` Adds the five enabled Hills Normal scenes.
- `Tools/Art/make_hills_normal_art.py` Generates the new sprite, item and lesson art and the labeled preview.
- `Tools/Art/make_hills_normal_backdrops.py` Generates five sets of six seamless 960×270 layers and the backdrop preview.
- `Tools/Audio/make_hills_normal_audio.py` Generates, measures and plots the four sounds.
