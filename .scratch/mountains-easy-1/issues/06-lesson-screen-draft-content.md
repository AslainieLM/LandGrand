# 06: Lesson Screen with draft content

**What to build:** Levels and item kinds are described as data. Mountains Easy 1 has a draft lesson (title, picture, three or four short sentences) and its items, granite, quartz, feldspar and mica, each with a picture, name and short description marked "draft — needs teacher review" (D-043). Tapping Easy 1 on the trail opens the Lesson Screen with the lesson, the item pictures and names, and the goals ("Collect 5/5", "Finish before time runs out"). Start begins the level; Back returns to the trail.

**Blocked by:** Tickets 05

**Status:** ready-for-agent

- [ ] Level data includes episode, difficulty, number, time limit (120 s), lesson, collectibles, checkpoint, hazards and goal
- [ ] Item kinds are shared data, so the same kind can appear in later levels
- [ ] The Lesson Screen shows the lesson, items and goals in a clear layout using the pixel font
- [ ] Draft content is visibly marked as needing teacher review
- [ ] Start opens the level scene (a placeholder is fine until ticket 07); Back returns to the trail
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec and the final manuscript (D-036); anything new is raised with the user first

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md`.
