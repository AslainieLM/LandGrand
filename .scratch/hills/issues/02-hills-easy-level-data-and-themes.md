# 02: Hills Easy 1–3 level data and the three themes

**What to build** (spec sections 6, 7 and 9):

1. Add `ChocolateHills`, `RollingPasture` and `SandDunes` to the **end** of the `LevelTheme` enum (`LevelLayout.cs:7`).
2. Add Hills Easy 1, 2 and 3 to `LevelCatalog`, with exactly the numbers in spec section 6:
   - width, goal, checkpoint, pits, spikes, boulders, ledges and pickups
   - lesson (title, 4 sentences, picture, draft), review (3 sentences) and start banner
   - Soft-sand zones need ticket 03's `SandZone` type. Add E3's four zones in ticket 03 and leave E3 without zones here.
3. `LevelThemes.cs`:
   - Add the three theme entries from the theme table in spec section 9.
   - Add their living props (spec section 9): palms and cogon tufts; goats, cows and stone fences, using the existing frame-loop helper; beach grass.
   - Props never sit over pits, ledges or boulders.
4. Rename the builder menu to **LandGrand > Build Levels** (`LevelBuilder.cs:15,17,43,72`) and update the log and error text (`LevelController.cs:59`). `BuildAll` keeps building every level in `LevelCatalog`, so it now also makes `HillsEasy1`–`3`.
5. Tests:
   - The Hills Easy levels have 5 pickups, 1 checkpoint and a secret spot; every pickup kind is on the Hills shelf; the lessons have 4 sentences of at most 44 characters; everything is draft.
   - Pit and spike clearances from the checkpoint are at least 100 px (same checks as `MountainsHardTests`).
   - Update `LevelCatalogTests.cs:27` and `EpisodeCompletionTests.cs:36` (Hills now has levels).

**Checklist**
- [x] The map's "Coming soon" on Hills is gone because `HasLevels(Hills)` is now true (no map code change)
- [x] The Hills Collection tab shows the 10 kinds (code: 10 Hills kinds, checked by `CollectionTests`; the screen itself not yet run)
- [ ] Mountains levels build and play unchanged (code unchanged; not run in Unity)
- [ ] EditMode tests pass (report the count) (not run: Unity Editor was open)

**Blocked by:** 01

**Status:** built by Claude (Haiku), not yet run in Unity

**Builder to run:** **LandGrand > Build Levels**, **LandGrand > Build Lesson Scene**, **LandGrand > Build Selection Scenes**
