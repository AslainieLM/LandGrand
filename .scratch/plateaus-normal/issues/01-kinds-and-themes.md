# 01: Kinds and themes

**What to build** (spec sections 2 and 4):

1. `Rules/LevelLayout.cs`: append `TableTop`, `MesaValley`, `CanyonRim`, `TepuiTop` and `SaltFlat` at the **end** of `LevelTheme`.
2. `Rules/LevelData.cs` `ItemCatalog`: append `ulexite` at the **end** of `All` (Plateaus only, mineral, `IsDraft`, text from spec section 6); add `Episode.Plateaus` to sandstone, quartzite, granite, shale, siltstone, conglomerate, limestone, schist, quartz, gneiss, halite, gypsum and ignimbrite (keep their landforms and comments).
3. **Tests:** the Plateaus shelf has 24 kinds (assert the exact list); `Collection.TotalCount` is 64; the new ids come last; shared kinds keep their old shelves; update the tests that expected 10 Plateaus kinds or a total of 63.

**Checklist**
- [x] Enum and kind ids only appended
- [x] EditMode tests pass (report the count; 524 before)

**Blocked by:** none

**Status:** done (2026-11-24, 563 tests at the end of the build; restored after a planning script reset this line)

**Builder to run:** none
