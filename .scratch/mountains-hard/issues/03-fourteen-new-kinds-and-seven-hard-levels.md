# 03: Fourteen new kinds and the seven Hard levels' data

**What to build:** The content, as data only.

1. **Fourteen new `ItemKind`s** in `ItemCatalog` (`LevelData.cs`), with the names, rock/mineral flags and descriptions from the spec table. All `isDraft: true`, home landform Mountains, picture name = id. Add them to `ItemCatalog.All`, so Mountains has 34 kinds.
2. **Seven Hard levels** in `LevelCatalog.Levels`, after Normal 5, using `new LevelData(Episode.Mountains, Difficulty.Hard, n, …)` exactly like the Normal levels:
   - lesson title, four sentences and picture (`lesson_mayon` … `lesson_mauna_kea`), marked draft;
   - 9 pickups, start (40, 0), checkpoints (2400, 0), (4800, 0), (7200, 0), goal (9440, 0);
   - hazards (pits and spikes);
   - the review text (three sentences);
   - `new LevelLayout(9600f, LevelTheme.…, boulders, ledges, vents:, crumbleLedges:, rockfalls:, movingPlatforms:, windZones:, rollingBoulders:, iceZones:)`;
   - the banner name ("Mount Mayon", "Sierra Madre", "Kitanglad Range", "Mount Hamiguitan", "Matterhorn", "Mount Kilimanjaro", "Mauna Kea").
   Copy every number from the spec's "Level data". A bridge at x is three crumbling ledges at x, x + 32 and x + 64 with top 0.
3. **Data tests** (a new `MountainsHardTests.cs`, like `MountainsNormalTests`):
   - 7 Hard levels, numbered 1–7, each 9600 wide, goal 9440, three checkpoints at 2400/4800/7200, 9 pickups;
   - each level has 2 new kinds × 4 pickups plus 1 review pickup of an older kind;
   - every rolling-boulder run is ≤ 243 px and over plain ground: no pit, spikes, boulder or vent between `End − 8` and `Spawn + 8`, and no checkpoint within 60 px of it;
   - no pit or spikes within 100 px of a checkpoint; no falling rock within 120 px of a checkpoint; no ice on a checkpoint; no falling rock within 30 px of ice;
   - every lesson sentence is at most 44 characters;
   - Mountains has 34 kinds; every new kind is used in a Hard level.

**Blocked by:** 01, 02

**Status:** done

- [x] All Easy and Normal tests still pass (update any test that counts 20 kinds or 8 levels)
- [x] The new data tests pass
- [x] `LevelCatalog.TimeLimitFor(Hard)` is still 240
- [x] Basic-English comments on every new line (the level lists can use one comment per line, as in Normal)

**Verified:** Unity EditMode 216 passed, including the seven layouts, safe hazard clearances, exact counts, 240-second limit, and draft lesson lengths.

**Builder to run:** none yet (the scenes come in ticket 04).

Spec: `../spec.md` ("Level data", "Content").
