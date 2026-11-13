# 08: Collectibles, HUD counter and first-find pop-up

**What to build:** The level holds 5 collectibles. Touching one collects it and updates the counter at the top left (for example 2/5). The first time the player ever finds a kind, the game and timer pause and a "You found an item!" pop-up shows its picture, name and short description with OK. Later finds of a kind already found, in this level or earlier, only play a chime and sparkle. Stars at the goal follow the collected count.

**Blocked by:** Tickets 07

**Status:** ready-for-agent

- [ ] Tests: each pickup counts once; a first find asks for the pop-up; a known kind does not; a kind found in an earlier session does not
- [ ] The pop-up pauses the game and blocks other input until OK
- [ ] Found kinds are saved
- [ ] Reaching the goal gives stars by the star rule from ticket 01
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec and the final manuscript (D-036); anything new is raised with the user first

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md`.
