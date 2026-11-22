# 02: Plains Normal 1–5 level data

**What to build** (spec sections 2, 3 and 6; numbers in `layouts.md`):

1. `LevelCatalog`: add Plains Normal 1–5 in the same shape as Hills Normal 1–5:
   - width 7200, start (40, 0), goal 7040, checkpoints 2400 and 4800, 180 s
   - 7 pickups with exactly one secret
   - Use every number exactly as `layouts.md`.
2. **Objects:**
   - floats and pools (N1, N4, and the N5 pool)
   - sand zones (N2, N4)
   - crumbling ledges (N2)
   - currents with directions (N3, N4)
   - horizontal moving platforms at top 0 (N5)
3. **Water pits:** N1, N3, N4 and N5 pits are water; N2 pits are dry.
4. **Text:** themes, banners, goal names, lessons, reviews and lesson pictures from spec section 6, all marked draft.
5. **Tests** (`PlainsNormalLevelsTests.cs`, the same checks as `HillsNormalLevelsTests`):
   - **Level basics:** widths, goals, checkpoints, timer, 7 pickups, one secret at y ≥ 72, and the kinds and their order.
   - **Objects:**
     - the first float of N1 is over a pool
     - the first current of N3 pushes right
     - each N5 boat spans its pit or pool (X0 = left edge, X1 + 48 ≥ right edge)
     - no pit or spikes within 100 px of a checkpoint
   - **Content:** every lesson line is at most 44 characters, and every pickup kind exists.
   - **Progression:**
     - clearing Plains Easy 3 opens Plains Normal 1 (Normal no longer says "Coming soon")
     - after Plains Normal 5 is cleared, Plains Hard still says "Coming soon"
     - Next is hidden after Normal 5
     - the Philippine Plains and Delta Boater badges are earned by their clears
   - **Regression:** Mountains, Hills and Plains Easy data are unchanged.

**Checklist**
- [ ] Numbers exactly as in `layouts.md`; any change is reported, never made silently
- [ ] EditMode tests pass (report the count)

**Blocked by:** 01

**Status:** not started

**Builder to run:** none
