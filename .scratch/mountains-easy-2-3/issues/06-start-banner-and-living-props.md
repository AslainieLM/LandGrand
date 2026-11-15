# 06: Level start banner and living props

**What to build:** On entering any level, its banner name ("Mountain Trail", "Volcano Trail", "Folded Peaks") slides in on a plate for about 1.5 s in Micro 5 22 px; input and the timer start when it leaves. Add decoration-only living props per theme: Easy 1 keeps its current look; Volcanic gets puffing decorative vents and glowing lava cracks; Folded rock gets drifting snow near the summit and an eagle that flies off when the explorer comes near. Props have no collision and no effect on play. Props for a theme only appear once that theme exists (tickets 03 and 05).

**Blocked by:** Ticket 01

**Status:** to do

- [ ] The banner shows on every level start and on Retry; the countdown does not move until it leaves
- [ ] The banner text is the level's banner name from level data; the HUD still says "Mountains Easy n" (D-049)
- [ ] A level-run test confirms no time passes before the run starts
- [ ] Props animate smoothly, stay pixel-sharp, and never block or hide the explorer, pickups or hazards
- [ ] Frame rate stays steady on the target 2 GB phone profile with props on
- [ ] Every new line of code has a short basic-English comment (D-033)

**Builder to run:** `python Tools/Art/make_level_art.py`, then **LandGrand > Build Mountains Levels**.

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md` (D-055).
