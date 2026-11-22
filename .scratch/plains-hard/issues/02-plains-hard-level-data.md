# 02: Plains Hard 1–7 level data

**What to build** (spec sections 2, 3 and 6; numbers in `layouts.md`):

1. `LevelCatalog`: add Plains Hard 1–7 in the same shape as Hills Hard 1–7:
   - width 9600, start (40, 0), goal 9440, checkpoints 2400, 4800 and 7200, 240 s
   - 9 pickups with exactly one secret
   - Use every number exactly as `layouts.md`.
2. **Objects (old rules only):**
   - hay bales = `RollingLog(spawn, end, direction, offset)` (H1, H3, H7)
   - straw piles (H1)
   - herds = `Bat(cx, 6, 48, 0, period, phase)` (H2, H3, H7)
   - ice zones (H4, H7)
   - floats (H4, H6), crumbling ledges (H5, H7), bats (H5), currents (H6, H7)
3. **Water pits:** H4 (3000, 104) and (6500, 104), and every H6 pit. All other pits are dry.
4. **Ambience:** H4 has `ambienceOverride: "amb_cold_plain"`; the others use the table (`amb_dry_hard`).
5. **Text:** themes, banners, goal names, lessons, reviews and lesson pictures from spec section 6, all marked draft.
6. **Tests** (`PlainsHardLevelsTests.cs`, the same checks as `HillsHardLevelsTests`):
   - **Level basics:** widths, goals, checkpoints, timer, 9 pickups, one secret at y ≥ 72, and the kinds and their order.
   - **Objects:**
     - the first H1 hay bale rolls left (toward the player)
     - every herd has centre y 6, up-down radius 0 and left-right radius 48
     - no pit or spikes within 100 px of a checkpoint
     - the H4 and H6 water pits as listed; the H4 override
   - **Content:** every lesson line is at most 44 characters, and every pickup kind exists.
   - **Progression:**
     - clearing Plains Normal 5 opens Plains Hard 1 (Hard no longer says "Coming soon")
     - Next is hidden after Hard 7
     - clearing all 15 Plains levels opens Plateaus (the generic unlock)
     - Prairie Ranger, World Plains, Steady Plains and Plain Master are earned by their rules
   - **Regression:** Mountains, Hills, Plains Easy and Plains Normal data are unchanged.

**Checklist**
- [ ] Numbers exactly as in `layouts.md`; any change is reported, never made silently
- [ ] EditMode tests pass (report the count)

**Blocked by:** 01

**Status:** not started

**Builder to run:** none
