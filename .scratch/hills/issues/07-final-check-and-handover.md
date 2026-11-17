# 07: Final check and handover

**What to do:**

1. Run all the asset scripts, then every builder:
   - `python Tools/Art/make_hills_easy_art.py`
   - `python Tools/Art/make_hills_easy_backdrops.py`
   - `python Tools/Art/make_profile_art.py`
   - `python Tools/Audio/make_hills_audio.py`
   - **LandGrand > Build Levels**
   - **LandGrand > Build Lesson Scene**
   - **LandGrand > Build Selection Scenes**
   - **LandGrand > Build Landform Map Scene**
   - **LandGrand > Build Landform Description Scene**
   - **LandGrand > Build Profile Scene**
2. Run the EditMode tests and report the count.
3. Add the new words to `CONTEXT.md`, the glossary, in its existing style: Soft sand, Shared kind, Landform audio table, Badge page, Coming soon difficulty.
4. Write `.scratch/hills/HANDOVER.md` and a short handover in your final message:
   - what was built, file by file
   - test results
   - any number you changed from the spec, and why
   - anything not done or not checked in the Editor

   Do **not** edit the Obsidian vault (`D:\LandGrand\LandGrand-Sources`). Claude updates it from your report.

**Blocked by:** 01–06

**Status:** done; the user played the whole checklist on 2026-11-17 and every item passed (commits `8d92fd5`, `f4e293b`)

**Play checklist for the user** (copy it into the handover):
- [x] Map: Hills opens with its trees, stream and birds (no cloud); tapping Hills opens the difficulty screen; Plains says "Finish Hills first"
- [x] Hills screens show birds and grass, not snow; Hills music plays in the levels with the stream and birds
- [x] Each Hills Easy level shows its banner (Chocolate Hills, Batanes Hills, La Paz Dunes), its lesson, its theme and its own background
- [x] Batanes has grazing goats and cows; Chocolate Hills has palms; La Paz has dune grass
- [x] Soft sand in La Paz: slower walking, same jumps, step sound and puffs; the first patch is a safe place to learn
- [x] Pits and spikes send the explorer a short way back, as in Mountains Easy
- [x] Each level has 5 pickups, and the secret spots are reachable; coral stone, limonite and kaolinite pop-ups show their draft text
- [x] Collection: the Hills tab shows 10 kinds; shared kinds (limestone, quartz and others) show on both Mountains and Hills
- [x] Achievements: Mountains and Hills badge tabs; Pasture Pal, Dune Surfer and Rolling Star can be earned; "New badge!" shows
- [x] After Hills Easy 3, Next is hidden; Hills Normal says "Coming soon"
- [x] Mountains Easy, Normal and Hard still play as before, with Mountains music and wind

**Builder to run:** all of the above.
