# 01: Rules: updraft, kinds, themes, audio, summary and badges

**What to build** (spec sections 2 to 5):

1. `Rules/LevelLayout.cs`: append `PineapplePlateau`, `LakePlateau` and `FallsEdge` at the **end** of `LevelTheme`; add `Updraft[] updrafts = null` at the **end** of the constructor and `IReadOnlyList<Updraft> Updrafts` (empty by default).
2. New `Rules/Updraft.cs` exactly as spec section 5 (`X`, `Top`, `Width` 32, `RiseSpeed` 90, `Contains`, `RiseVelocity`).
3. `Rules/LevelData.cs` `ItemCatalog`:
   - Append `halloysite`, `goethite`, `gibbsite` and `cristobalite` at the **end** of `All`: Plateaus only, `IsDraft`, minerals, with the text from spec section 7.
   - Add `Episode.Plateaus` to basalt, andesite, hematite, tuff, volcanic_breccia and laterite, keeping their existing landforms and comments.
4. `Rules/LandformAudio.cs`: the Plateaus row (spec section 8 audio table).
5. `Rules/LandformSummary.cs`: the Plateaus summary (spec section 3).
6. `Rules/Achievements.cs`: append the 11 Plateaus badges after the Plains badges (spec section 3).
7. **Tests:**
   - `Updraft` rule tests from spec section 5.
   - The Plateaus shelf has 10 kinds: assert the exact list `Collection` gives. `Collection.TotalCount` is 63.
   - The new ids come last; no other kind, theme or badge id moved. 46 badges; the Plateaus page has 11.
   - `land_grand_champion` is earned only when all four landforms are complete; `plateau_master` only when all 15 Plateaus levels are cleared.
   - Plateaus summary lines, picture and draft flag; Plateaus audio files.
   - Update the tests that expected an empty Plateaus shelf, 59 kinds, 35 badges or a "Coming soon" Plateaus summary.

**Checklist**
- [ ] Enum, kind and badge ids only appended
- [ ] No Unity types in `Rules`
- [ ] EditMode tests pass (report the count; 475 before)

**Blocked by:** none

**Status:** done (2026-11-23, 515)

**Builder to run:** none
