# 07: Final check and handover

**What to do:**

1. Run all the asset scripts, then every builder:
   - `python Tools/Art/make_normal_art.py`
   - `python Tools/Art/make_profile_art.py`
   - `python Tools/Audio/make_normal_audio.py`
   - **Build Mountains Levels**
   - **Build Lesson Scene**
   - **Build Profile Scene**
   - the selection builder, if level selection changed
2. Run the EditMode tests and report the count.
3. Add the new words to `CONTEXT.md`, the glossary, in its existing style: Crumbling ledge, Falling rock, Moving platform, Wind gust and Normal checkpoints.
4. Write a short handover in your final message:
   - what was built, file by file
   - test results
   - any number you changed from the spec, and why
   - anything not done or not checked in the Editor

   Do **not** edit the Obsidian vault (`D:\LandGrand\LandGrand-Sources`). Claude updates it from your report.

**Blocked by:** 01–06

**Status:** done; user play checklist remains open

**Play checklist for the user** (copy it into the handover):
- [ ] Each Normal level shows its banner (Mount Pulag to Mount Everest), its lesson and its theme
- [ ] Crumbling bridges break behind the explorer; stopping on one drops you to the last checkpoint
- [ ] Rocks warn for about a second before they fall
- [ ] Platforms carry the explorer
- [ ] Gusts push back and you can wait them out
- [ ] Both checkpoint flags work, in the level and on the bar
- [ ] The Result Screen shows 7 items; Next after Normal 5 leads to Hard 1 ("Coming soon")
- [ ] The Profile shows 2 pages of Mountains kinds and 11 badges

**Builder to run:** all of the above.

Handover: ../HANDOVER.md. All code, builders, glossary, automated tests, and Profile render checks complete. Asset generators intentionally not rerun, following the user’s explicit preservation rule. Selection was already data-driven and did not change. Full play-through and audio listening remain the user checks below.
