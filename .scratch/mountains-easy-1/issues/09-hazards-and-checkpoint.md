# 09: Hazards and checkpoint

**What to build:** The level has one pit, one spike trap and a checkpoint flag in the middle. Touching the trap or falling into the pit returns the explorer to the last checkpoint reached (or the start), with collected items kept and the timer still running (D-040). There is no health bar.

**Blocked by:** Tickets 07

**Status:** built 2026-11-13, awaiting the user's Editor check; the hazard-keeps-the-timer test is now in LevelRunTests (ticket 10)

- [ ] Tests: a hazard moves the respawn point to the last checkpoint or start; items and remaining time are unchanged
- [ ] Reaching the checkpoint flag shows it as reached
- [ ] Respawn has a short, clear effect so the player understands what happened
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec and the final manuscript (D-036); anything new is raised with the user first

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md`.
