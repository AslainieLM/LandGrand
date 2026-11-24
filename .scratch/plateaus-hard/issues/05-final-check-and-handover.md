# 05: Final check and handover

**What to do:**

1. Run every builder one at a time: `LevelBuilder.BuildAll`, `LessonBuilder.Build`, `SelectionBuilder.Build`, `LandformMapBuilder.Build`, `LandformDescriptionBuilder.Build`, `ProfileBuilder.Build`, `CompletionBuilder.Build`. Each log: no "Missing", no errors.
2. Run all EditMode tests; report the count.
3. Write `.scratch/plateaus-hard/HANDOVER.md` like `.scratch/plains-hard/HANDOVER.md`: every file added or changed, the test count, numbers changed from the spec, props moved more than 64 px, goal offsets, things not checked in the Editor, "Changes not in the spec", and the play checklist below.
4. Run `/code-review` against the commit that holds this plan (spec: `spec.md`, `layouts.md`, `issues/01`–`05`; standards: `AGENTS.md`, `BUILD-START.md`; ignore `.unity` scenes and binary assets). Fix the findings, then re-run the tests and builders.

**Play checklist (for the user):**
- [ ] H1–H7 each: lesson shows, banner names the place, goal sprite sits on the ground, all 9 pickups can be collected, time fits in 240 s
- [ ] H1 hot-spring vents puff and lift; H2 geladas nudge and chatter; H3 sand gusts push left with the strong gust sound; H4 slabs move up and down and the ladders are only pictures; H5 blue ice slides and snow gusts push; H6 loess sand slows; H7 everything works together
- [ ] Old vents (Mountains Easy 2), gusts (Mountains Normal), sand (Hills Easy 3), herds (Plains Hard) look and sound as before
- [ ] Collection: Plateaus shelf 36, coal, meteorite and opal cards
- [ ] After Plateaus Hard 7 with everything complete: Description says "You explored all four landforms!", then the Completion Screen (stars n/180, rocks and minerals n/67, badges), Back to Map shows the golden flag and ribbon; a second visit skips the Completion Screen; the book's See Ending opens it again
- [ ] Land Grand Champion "New badge!" shows on the Result Screen

**Blocked by:** 01, 02, 03, 04

**Status:** open

**Builder to run:** all seven builders (see step 1)
