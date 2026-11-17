# Hills Easy 1–3: handover (2026-11-17)

Tickets 01–06 were built by Claude (Haiku helpers, one per ticket, reviewed by Claude Opus). Ticket 07 (this file, the glossary) was done by Claude Opus.

**Run in Unity batch mode on 2026-11-17:** the code compiles, all **295 EditMode tests pass** (was 234; 280 before the review fixes), and all six builders ran with no errors and no missing-asset messages. Hills Easy 1–3 scenes were made and added to Build Settings; they link `bgm_hills` and `amb_stream_easy`, E3 has the sand watcher and `sfx_sand_step`, and Mountains scenes still link the Mountains music and breeze. Nothing has been played or listened to yet. Nothing is committed.

## What was built, file by file

**Rules (plain C#, tested)**
- `Rules/LevelData.cs`: `ItemKind.HomeLandform` is now `Landforms` (a list) with `BelongsTo(episode)`. Seven kinds are shared with Hills (limestone, calcite, clay, andesite, quartz, magnetite, feldspar). New kinds Coral Stone, Limonite and Kaolinite (drafts). Hills Easy 1–3 in `LevelCatalog` (spec section 6, same numbers as the bot file). `LevelCatalog.HasLevels(episode, difficulty)`.
- `Rules/LevelLayout.cs`: three new themes at the end of `LevelTheme` (ChocolateHills, RollingPasture, SandDunes); optional `SandZones` list.
- `Rules/SandZone.cs`, `Rules/SoftSand.cs` (new): sand zones and the sand rule (walk ×0.6, step sound every 0.3 s, puff frames 0.15 s).
- `Rules/LandformAudio.cs` (new): music and ambience per landform and difficulty. Plains and Plateaus fall back to Mountains.
- `Rules/LandformSummary.cs`: the five Hills summary lines and `bg_hills`.
- `Rules/Collection.cs`: shelves use `BelongsTo`.
- `Rules/Achievements.cs`: every badge has a `Landform`; 15 Mountains badges keep ids and order, 10 Hills badges follow. `ForLandform(episode)`. Rules are small helpers; an empty level list never earns a badge; `world_peaks` and `world_hills` read `{5, 6, 7}` as data.
- `Rules/PlayerProgress.cs`: `NextPlayableAfter(e, d, n)` returns None when the next level has no data. `NextAfter` is unchanged.

**Play and menus**
- `Play/SandWatcher.cs` (new level part): sets `ExplorerMotor.OnSand`, plays the step sound and draws the puffs on `LevelClock.Now`.
- `Play/ExplorerMotor.cs`: ground walk speed goes through `SoftSand.WalkSpeed`. Air speed and jumps unchanged.
- `Play/ResultScreen.cs`: Next is hidden when nothing playable follows. Back to Map and Replay stay.
- `Play/LevelController.cs`: error text names "Build Levels".
- `Selection/DifficultySelectionController.cs`: an open difficulty with no levels shows "Coming soon" in the guide panel.
- `Common/LandformBackdrop.cs`: `lifeSets[]` per landform replaces the single snow object.
- `Common/LandformDescriptionController.cs`: badge total counts this landform's page only.
- `Profile/ProfileController.cs`: Achievements tab has landform tabs and one 5×3 page per landform; Plains and Plateaus say "Coming soon".

**Editor builders**
- `LevelBuilder.cs`: menu is now **LandGrand > Build Levels**; soft_top tiles over sand, sand watcher, step sound and puffs.
- `HardLevelObjects.cs`: ground top drawing takes the sand zones.
- `LevelThemes.cs`: three Hills themes with their props; `HillsPropFits` keeps props off ledges and sand.
- `PixelSceneKit.cs`: `LevelMusic(episode, difficulty)` reads `LandformAudio`.
- `LandformMapBuilder.cs`: Hills cloud removed; Hills trees, stream ripples and birds.
- `SelectionBuilder.cs`: one life set per landform (Mountains: cloud, eagle, snow; Hills: cloud, 2 birds, 6 grass; Plains and Plateaus: cloud).
- `LandformLife.cs` (new): shared cloud, bird and grass helpers.
- `ProfileBuilder.cs`: badge tabs, 15-cell grid, "Coming soon" label.

**Tests**
- New: `HillsEasyLevelsTests`, `HillsBadgeTests`, `SoftSandTests`, `LandformAudioTests`, `NextPlayableTests`.
- Changed: `AchievementTests`, `CollectionTests`, `EpisodeCompletionTests`, `LandformSummaryTests`, `LevelCatalogTests`, `MountainsEasyLevelsTests`, `MountainsHardTests`, `MountainsNormalTests` (Hills is no longer empty; Mountains checks now look only at Mountains levels).
- **295 pass, 0 fail** (was 234). New after review: `ZoneFootingTests` and more `LandformAudioTests`.

**Glossary:** `CONTEXT.md` has Shared kind, Shelf (rewritten), Badge page, Soft sand, Coming soon difficulty and Landform audio table. "Home landform" is removed.

## Numbers changed from the spec, or guessed

- Spike width 32 px for all Hills spikes (the spec gives only positions; same as Mountains).
- "Standing on sand" uses the ice check's 0.01 px tolerance; the bot used 0.5 px. Only zone edges could feel different.
- Map and menu bird circles, tree and grass placement, and sway speeds: the spec gave positions only, so the circle sizes and speeds were chosen by eye in code.
- Profile badge rows moved to y 122 / 174 / 226 to make room for the tabs, so the Mountains page is not pixel-identical to before.
- Hills shelf order is the catalog order (shared kinds first, then the three new kinds).
- The ticket named `LevelCatalog.Next`; it became `PlayerProgress.NextPlayableAfter` because it needs saved progress.

## Not done or not checked

- The Mountains scenes were rebuilt too; their scene files changed. Commit them with the code.
- New `.cs` and scene `.meta` files were made by Unity; commit them with the code.
- The asset scripts were not re-run; the art and audio from commit `d3c24c5` are used as they are.
- Scene placeholders `0/34` and `0/25` in `LandformDescriptionBuilder.cs` stay; the controller overwrites them when the screen opens.
- No sound was listened to and no level was played.

## Builders (already run in batch mode; re-run only after further changes)

1. **LandGrand > Build Levels**
2. **LandGrand > Build Lesson Scene**
3. **LandGrand > Build Selection Scenes**
4. **LandGrand > Build Landform Map Scene**
5. **LandGrand > Build Landform Description Scene**
6. **LandGrand > Build Profile Scene**

EditMode tests: `Temp/editmode.xml`. Builder logs: `Temp/build-*.log`.

## Play checklist for the user

- [ ] Map: Hills opens with its trees, stream and birds (no cloud); tapping Hills opens the difficulty screen; Plains says "Finish Hills first"
- [ ] Hills screens show birds and grass, not snow; Hills music plays in the levels with the stream and birds
- [ ] Each Hills Easy level shows its banner (Chocolate Hills, Batanes Hills, La Paz Dunes), its lesson, its theme and its own background
- [ ] Batanes has grazing goats and cows; Chocolate Hills has palms; La Paz has dune grass
- [ ] Soft sand in La Paz: slower walking, same jumps, step sound and puffs; the first patch is a safe place to learn
- [ ] Pits and spikes send the explorer a short way back, as in Mountains Easy
- [ ] Each level has 5 pickups, and the secret spots are reachable; coral stone, limonite and kaolinite pop-ups show their draft text
- [ ] Collection: the Hills tab shows 10 kinds; shared kinds (limestone, quartz and others) show on both Mountains and Hills
- [ ] Achievements: Mountains and Hills badge tabs; Pasture Pal, Dune Surfer and Rolling Star can be earned; "New badge!" shows
- [ ] After Hills Easy 3, Next is hidden; Hills Normal says "Coming soon"
- [ ] Mountains Easy, Normal and Hard still play as before, with Mountains music and wind

## Review fixes (2026-11-17, after /code-review)

- Audio script names are data in `LandformAudio` (`MusicScriptFor`, `AmbienceScriptFor`); `PixelSceneKit` has no landform branch. Ambience is keyed by `Difficulty`, not by position.
- `LandformSummary` uses one table keyed by landform.
- `SandZone.Covers` is now `Contains`, like `IceZone`. New `Rules/ZoneFooting.cs` holds the one flat-ground check (0.01 px) used by both ice and sand; ice behaves as before.
- `SandWatcher`: step sound through `LevelController.PlaySfx(clip, volume)`; steps and puffs are timed from when a walk on sand starts (first step at once, then every 0.3 s), not from a fixed clock grid.
- `ExplorerMotor`: sand slows walking only while grounded, so a jump from sand is never slowed.
- Profile: one helper for the four landform tabs in both the builder and the controller.
- `LevelThemes.HillsPropFits`: one overlap test; beach grass now avoids only the first 16 px of each sand zone (spec section 9); goats and cows go through the fit check (none refused in the build log).
- `Achievements.WorldLevels` renamed `LastThreeHardLevels`.
- Level Selection also shows "Coming soon" when a difficulty has no levels.
- Kept on purpose: Profile badge rows at y 122/174/226 (room for the tabs); 0.01 px flat tolerance.
