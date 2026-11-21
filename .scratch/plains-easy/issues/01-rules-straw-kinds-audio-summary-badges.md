# 01: Rules for the straw pile, Plains kinds, audio, summary and badges

**What to build** (spec sections 2, 3, 4 and 5):

1. `Rules/StrawPile.cs` (new): `X`, `Width = 48`, `Height = 16`, `LaunchSpeed = 400`, `SquashSeconds = 0.15`, and `IsOnTop(explorerMiddleX, explorerHalfWidth, feetY)` exactly as spec section 5.
2. `Rules/LevelLayout.cs`:
   - Add `StrawPile[] strawPiles = null` at the **end** of the constructor, and a `StrawPiles` list that is empty by default.
   - Add `RicePlain`, `SaltBeds` and `RiverPlain` at the **end** of `LevelTheme`.
   - Existing callers must compile unchanged.
3. `Rules/LevelData.cs` `ItemCatalog`:
   - Append `siltstone`, `halite`, `gypsum` and `mudstone` at the **end** of `All`. They are Plains only and `IsDraft`, with the text from spec section 7.
   - Add `Episode.Plains` to clay, sandstone, shale, quartz, conglomerate and limestone.
4. `Rules/LandformAudio.cs`: add the Plains row and its script names (all `make_plains_audio.py`).
5. `Rules/LandformSummary.cs`: add the Plains summary (5 draft lines, picture `bg_plains`).
6. `Rules/Achievements.cs`: append the 10 Plains badges after the Hills badges, on the `Episode.Plains` page.
7. **Tests:**
   - `IsOnTop` at the edges and heights in spec section 5. The rise from `LaunchSpeed` is about 88.9 px.
   - The Plains shelf has 10 kinds: assert the exact list `Collection` gives.
   - `Collection.TotalCount` is 54, and a shared kind counts once.
   - The Plains summary and audio rows. Plateaus is still "Coming soon" and still uses the Mountains audio.
   - The 10 Plains badges:
     - their ids, in order, after `hill_master`
     - on the Plains page
     - locked on a fresh profile
     - Mountains and Hills badge ids and order unchanged
   - Update the existing tests that say Plains is empty (the list in spec section 4).

**Checklist**
- [ ] No Unity types in Rules; the constants match spec section 5
- [ ] Enum, kind and badge ids are only appended
- [ ] EditMode tests pass (report the count; the Hills Hard handover reports 395 before)

**Blocked by:** none

**Status:** not started

**Builder to run:** none
