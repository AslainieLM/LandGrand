# 01: Kinds and themes

**What to build** (spec sections 2, 4 and 6):

1. Append the seven themes to `LevelTheme` at the **end**: `DeccanTraps, SimienHighlands, SandstoneArches, CliffDwellings, IcePlateau, LoessTerraces, RoofOfWorld`.
2. Append `coal` (rock), `meteorite` (rock) and `opal` (mineral) at the **end** of `ItemCatalog.All`, Plateaus only, `IsDraft`, with the spec section 6 text and pictures.
3. Add `Episode.Plateaus` to zeolite, calcite, olivine, clay, loess, feldspar, mica, chromite and serpentinite. Keep their other landforms and comments.
4. Add `PlayerProgress.IsGameComplete` and `CompletionSummary.For(progress)` (plain C#, spec section 4), test first.
5. Update tests that expect a Plateaus shelf of 24 or a total of 64 (now 36 and 67). The `plateau_collector` badge needs all 36 Plateaus kinds; check its test.

**Checklist**
- [ ] Old enum values, kind ids and badge ids keep their order
- [ ] Shelf counts: Plateaus 36, total 67, other shelves unchanged except the nine shared kinds now also on Plateaus
- [ ] `IsGameComplete` and `CompletionSummary` tests (an empty save, one landform complete, all four complete: stars 180)
- [ ] EditMode tests pass (563 before this ticket; report the count)

**Blocked by:** none

**Status:** open

**Builder to run:** none
