# 04: Final check and handover

**What to do:**

1. Re-run the asset scripts, all six builders and the EditMode tests. Report the final count.
2. `CONTEXT.md`: add **Straw bounce pile** ("a low pile of rice straw; landing on its top bounces the explorer high; walking into its side does nothing"). Keep the existing words.
3. Write `.scratch/plains-easy/HANDOVER.md` with:
   - every file built or changed
   - the final test count
   - every changed number, with its reason
   - every prop moved more than 64 px
   - everything not checked in the Editor
   - the play checklist below

**Play checklist for the user**

- [ ] **Unlock:** with a save where Hills is complete, the map plays the one-time Plains unlock. Then the Plains region shows rice, egrets and a carabao, and Plateaus says "Finish Plains first".
- [ ] **Menus:** Plains Easy 1 opens, and Normal and Hard say "Coming soon". The menu screens show egrets and grass.
- [ ] **E1 Central Luzon Plain:**
  - rice ground, the Rice Granary goal, carabao and egrets
  - `bgm_plains`, with grass and crickets
  - the three-step climb reaches the siltstone secret
- [ ] **E2 Pangasinan Salt Beds:**
  - salt ground, salt heaps and palms, and the Salt Hut
  - halite and gypsum show their first-find pop-ups
- [ ] **E3 Isabela River Plain:**
  - The first straw pile, at 600, is safe to try.
  - Landing on a pile bounces you high with a soft "fwump", a squash and straw bits.
  - Walking into a pile's side does nothing.
  - The pile at 1200 reaches the mudstone ledge.
  - The pile at 3300 plus a double jump reaches the limestone secret.
- [ ] **Length:** each level takes about 1 minute (the bot took 57, 62 and 62 s; the limit is 120 s).
- [ ] **After Plains Easy 3:**
  - Next is hidden.
  - The Profile's Plains shelf shows 10 kinds and "Found here n/10".
  - The Plains badge page shows 10 badges, with Salt Maker and Straw Jumper earned.
- [ ] **Regression:** Mountains and Hills still play as before. Spot-check one level of each and the Hills badge pages.

**Blocked by:** 01, 02, 03

**Status:** not started

**Builder to run:** none (everything is built in ticket 03)
