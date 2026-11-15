# 03: Ten new kinds and the five Normal levels' data

**What to build:**

1. **Items.** Add 10 `ItemKind`s to `ItemCatalog` with the ids, names, rock/mineral flags and descriptions from the spec table: sandstone, clay, conglomerate, shale, basalt, magnetite, sulfur, pyrite, limestone and gneiss.
   - All are `isDraft: true` and have home landform Mountains.
   - Put them in `ItemCatalog.All` after garnet, in that order.
   - The pictures already exist in `Assets/Art/Items/Sprites/`.
2. **Levels.** Add Mountains Normal 1–5 to `LevelCatalog` with the exact numbers in the spec ("Level data"):
   - width 7200, start (40, 0), checkpoints (2400, 0) and (4800, 0), goal (7040, 0), time limit 180 s
   - pickups, pits (with their widths), spikes (width 32), boulders, ledges, crumbling ledges, rockfalls, moving platforms, wind zones and the N4 steam vent
   - theme, banner name, lesson (title, 4 sentences, picture, draft) and review (3 sentences)
3. **Lesson builder.** The Lesson Screen picture list already comes from the catalog. Check that the 5 new lesson pictures (`lesson_pulag`, `lesson_pinatubo`, `lesson_matutum`, `lesson_apo`, `lesson_everest`) are picked up, and that the item boxes show each **kind** once, as Easy does.
4. **Level selection.** Check that the Normal trail shows 5 stones and opens the Lesson Screen for each Normal level.

**Blocked by:** 01, 02

**Status:** to do

- [ ] Catalog tests:
  - 5 Normal levels with 7 pickups each and 2 checkpoints
  - every pickup, hazard and object inside the width
  - no checkpoint inside a pit
  - every lesson sentence ≤ 44 characters
  - every kind used exists in `ItemCatalog`
- [ ] Mountains has 20 kinds; Mountain Collector now needs all 20 (that is expected)
- [ ] Easy data unchanged
- [ ] Basic-English comments

**Builder to run:** **LandGrand > Build Lesson Scene**. The level scenes build in ticket 04.

Spec: `../spec.md` ("Level data", "Content").
