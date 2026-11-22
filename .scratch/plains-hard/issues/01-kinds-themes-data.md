# 01: Kinds and themes

**What to build** (spec sections 2 and 4):

1. `Rules/LevelLayout.cs`: append `Prairie`, `Savanna`, `Pampas`, `FrozenPlain`, `LimestonePlain`, `GangesPlain` and `Steppe` at the **end** of `LevelTheme`.
2. `Rules/LevelData.cs` `ItemCatalog`:
   - Append `chalk`, `loess` and `flint` at the **end** of `All`. They are Plains only, `IsDraft` and stored as rocks, with the text from spec section 6.
   - Add `Episode.Plains` to granite, tuff, calcite, mica and feldspar, keeping their existing landforms and comments.
3. **Tests:**
   - The Plains shelf has 24 kinds: assert the exact list `Collection` gives. `Collection.TotalCount` is 59.
   - The new ids come last; no other kind, theme or badge id moved (the 35 badge ids test stays green).
   - Shared kinds still sit on their old shelves.
   - Update the tests that expected 16 Plains kinds or a total of 56.

**Checklist**
- [ ] Enum and kind ids only appended
- [ ] EditMode tests pass (report the count; 440 before)

**Blocked by:** none

**Status:** not started

**Builder to run:** none
