# 01: Shared kinds, per-landform audio table and summary data

**What to build** (spec sections 2, 4 and 7):

1. `ItemKind` (`Assets/Scripts/Rules/LevelData.cs:16-36`): replace `HomeLandform` with `Landforms` (an array of `Episode`) and add `bool BelongsTo(Episode e)`.
   - The 34 Mountains kinds keep `{ Mountains }`, except limestone, calcite, clay, andesite, quartz, magnetite and feldspar, which become `{ Mountains, Hills }`.
   - Update every reader of `HomeLandform` (`Collection.cs`, `Shelf`, `Achievements.cs` `mountain_collector`, the Profile) to use `BelongsTo`.
   - A kind found once counts once everywhere (save `found` ids are unchanged).
2. Add the three Hills kinds to `ItemCatalog`: `coral_stone`, `limonite` and `kaolinite`, with the names, descriptions, rock or mineral flags and pictures in spec section 7. `isDraft: true`, `Landforms = { Hills }`.
3. New Rules table `Assets/Scripts/Rules/LandformAudio.cs`:
   - `MusicFor(Episode)` and `AmbienceFor(Episode, Difficulty)` return the file names in spec section 9 (no extension).
   - Plains and Plateaus return the Mountains names.
   - `PixelSceneKit.LevelMusic` and `LevelAmbience` take the episode and load `<name>.ogg` from these. `LevelBuilder.cs:173` passes `level.Episode`. The error text names the script for each file (`make_hills_audio.py` for the Hills ones).
4. `LandformSummary.For(Hills)` returns the 5 draft lines and the `bg_hills` picture (spec section 3). Plains and Plateaus stay "Coming soon".
5. Tests:
   - Shared kinds sit on both shelves; Mountains shelf totals are still 34; the Hills shelf has the 10 Easy kinds once ticket 02 adds the levels.
   - `BelongsTo`; the audio table for every episode and difficulty; the Hills summary is 5 lines and draft.
   - Update `CollectionTests.cs:38-44,53` and `LandformSummaryTests.cs:19-27`, which assumed Hills is empty.

**Checklist**
- [ ] No Unity types in Rules; every new line commented (D-033)
- [ ] Mountains Collection still shows 34 kinds on 3 pages
- [ ] EditMode tests pass (report the count)

**Blocked by:** none

**Status:** not started

**Builder to run:** none for the rules alone. **LandGrand > Build Levels** after ticket 02 (the audio change shows up in the level scenes).
