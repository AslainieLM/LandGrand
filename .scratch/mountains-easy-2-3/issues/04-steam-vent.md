# 04: Steam vent launch (Easy 2)

**What to build:** A steam vent level object, described in level data like other layout parts. It puffs on a fixed cycle (about every 2 s) with a short warning wisp first. Standing on it during a puff launches the explorer upward higher than a double jump; the explorer may still double jump after the launch. It never hurts, never moves the explorer back, and does not touch the timer, items, respawn point or stars. The puff-and-launch rule lives in the rules core and is tested there. Place it in Easy 2 at the two spots ticket 03 marked: once over flat ground (safe to try), and once to reach the pumice secret ledge, which then moves up so only the vent reaches it.

**Blocked by:** Ticket 01 (Easy 2 placement also needs ticket 03)

**Status:** to do

- [ ] The vent shows a warning wisp, then a puff, on a steady cycle a learner can read
- [ ] Standing on it during a puff launches; standing on it between puffs does not
- [ ] A launch never changes time, collected items, respawn point or stars (rules tests)
- [ ] After ticket 03, the pumice ledge can only be reached with the vent, and the level stays finishable without it
- [ ] A puff sound plays at the Settings sound volume
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] D-036: the vent is a level object the manuscript does not name; it is recorded in D-055 for the panel

**Builder to run:** `python Tools/Art/make_level_art.py`, then **LandGrand > Build Mountains Levels**.

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md` (D-055).
