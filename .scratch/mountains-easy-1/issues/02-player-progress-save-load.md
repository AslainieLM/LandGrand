# 02: Player progress, saving and loading at launch

**What to build:** The game remembers the player. Progress holds the player name, unlocked episodes, cleared levels with their best stars, and every item kind ever found. Its rules decide what is available: Mountains is unlocked at first, levels go in order, Normal unlocks after all 3 Easy levels, Hard after all 5 Normal levels, and unlocking never depends on stars. A replay never lowers saved stars (D-045). Progress is saved safely to one file on the device and loaded when the game opens, before the Main Menu (LG-26).

**Blocked by:** Tickets 01

**Status:** done, committed by the user

- [ ] Tests: only Mountains is unlocked in a fresh profile; only the first uncleared level and earlier ones are available
- [ ] Tests: Normal unlocks when all 3 Easy levels are cleared, Hard when all 5 Normal levels are cleared, whatever the stars
- [ ] Tests: a worse replay keeps the best stars; found item kinds are never removed
- [ ] Tests: save then load gives equal progress; a missing or corrupt file gives a fresh profile
- [ ] Saving writes to a temporary file first, so an interrupted save keeps the previous save
- [ ] Opening the game loads progress before the Main Menu appears; volume settings stay where they are
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec and the final manuscript (D-036); anything new is raised with the user first

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md`.
