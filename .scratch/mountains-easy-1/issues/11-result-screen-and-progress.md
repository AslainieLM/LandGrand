# 11: Result Screen and progress shown everywhere

**What to build:** Clearing the level opens the Result Screen: the stars pop in one by one with a chime, the collected items appear in a row with missed ones as grey outlines, the goals are shown with ticks, and two or three draft review sentences close the lesson. Replay starts the level again from its lesson, Continue goes to the next available level, and Back to Map returns to the Landform Map. The new stars show on the map, the difficulty cards and the trail.

**Blocked by:** Tickets 08, 10

**Status:** built 2026-11-13, awaiting the user's Editor check

- [ ] Stars, items and goals match what happened in the run
- [ ] Replay, Continue and Back to Map work; Continue goes to the next available level (Easy 2 shows as next, even if not built yet)
- [ ] Saved best stars show on the trail, the Easy card and the Mountains region
- [ ] Replaying with fewer items never lowers the shown stars (D-045)
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec and the final manuscript (D-036); anything new is raised with the user first

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md`.
