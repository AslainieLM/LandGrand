# 01: Kinds and themes

**What to build** (spec sections 2 and 4):

1. `Rules/LevelLayout.cs`: append `CandabaWetland`, `LaharPlain`, `RiverDelta`, `FloatingVillage` and `FloatingMarket` at the **end** of `LevelTheme`.
2. `Rules/LevelData.cs` `ItemCatalog`:
   - Append `peat` and `laterite` at the **end** of `All`. They are Plains only and `IsDraft`, with the text from spec section 6. Peat is stored as a rock.
   - Add `Episode.Plains` to pumice, dacite, andesite and gold, keeping their existing landforms and comments.
3. **Tests:**
   - The Plains shelf has 16 kinds: assert the exact list `Collection` gives. `Collection.TotalCount` is 56.
   - The new ids come last; no other kind, theme or badge id moved.
   - Shared kinds still sit on their old shelves.
   - Update the tests that expected 10 Plains kinds or a total of 54.

**Checklist**
- [ ] Enum and kind ids only appended
- [ ] EditMode tests pass (report the count; 423 before)

**Blocked by:** none

**Status:** not started

**Builder to run:** none
