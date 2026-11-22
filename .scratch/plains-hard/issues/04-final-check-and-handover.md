# 04: Final check and handover

**What to do:**

1. Re-run the asset scripts, all six builders and the EditMode tests. Report the final count.
2. `CONTEXT.md`: add **Hay bale** ("a rolling log drawn as a round bale of hay") and **Herd** ("a gentle bat drawn as a grazing animal that walks on the ground and nudges the explorer away"). Keep the existing words.
3. Write `.scratch/plains-hard/HANDOVER.md` with:
   - every file built or changed
   - the final test count
   - every changed number, with its reason
   - every prop moved more than 64 px
   - any goal offset changed
   - everything not checked in the Editor
   - the play checklist below

**Play checklist for the user**

- [ ] **Unlock:** after Plains Normal 5, Plains Hard 1 opens.
- [ ] **H1 Great Plains:**
  - the practice hay bale rolls toward you in a safe spot, with a straw-dust warning and a soft rustle
  - right-rolling bales only catch you if you stop
  - straw piles still bounce
  - the chalk card opens on first find
- [ ] **H2 Serengeti:**
  - wildebeest walk back and forth, face where they walk, and stand on the ground
  - a touch nudges you away with soft hooves and never hurts; you can jump over them
  - granite and tuff appear on the Plains shelf
- [ ] **H3 Pampas:** hay bales and cattle together; the loess card says it is not a true rock.
- [ ] **H4 West Siberian Plain:**
  - frosty ground slides like ice
  - bog floats over two water pits
  - cold wind ambience (not the dry wind)
- [ ] **H5 Nullarbor Plain:** crumbling bridges and bats; no trees; the flint card opens.
- [ ] **H6 Indo-Gangetic Plain:** water pits with floats; currents push both ways.
- [ ] **H7 Mongolian Steppe:** hay, horses, frost, crumbling bridges and currents; ger goal.
- [ ] **Length:** each level takes about 2 minutes (bot 113–124 s; the limit is 240 s).
- [ ] **Profile:**
  - the Plains shelf shows 24 kinds and "Found here n/24"
  - Prairie Ranger after any Hard clear; World Plains after H5–H7; Plain Master after all 15
- [ ] **Plateaus:** after all 15 Plains levels, the map plays the one-time unlock and Plateaus says "Coming soon".
- [ ] **Regression:** Hills Hard logs, Hills Normal bats and Mountains Hard ice look and move as before. Spot-check one Plains Normal level.

**Blocked by:** 01, 02, 03

**Status:** not started

**Builder to run:** none (everything is built in ticket 03)
