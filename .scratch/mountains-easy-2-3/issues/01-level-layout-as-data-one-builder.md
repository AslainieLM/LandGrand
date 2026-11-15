# 01: Level layout as data, one builder for every level

**What to build:** Move Mountains Easy 1's layout out of the builder into its level data: width, ground pieces (split by pits), boulders, ledges, level theme (ground tiles, props, goal object), level start banner name ("Mountain Trail") and lesson picture. Replace the Easy 1 menu item with one command, **LandGrand > Build Mountains Levels**, that builds every level in the level catalog into the scene named by the existing scene-name rule, adds it to the build list, and sets the level identity (episode, difficulty, number) on the scene's level controller. Theme support starts with the Mountains meadow theme only; the Volcanic and Folded rock themes arrive in tickets 03 and 05. The five fixed `Find(Mountains, Easy, 1)` calls become reads of the level being built.

**Blocked by:** None

**Status:** to do

- [ ] Easy 1 built by the new command looks and plays exactly as before (same ground, boulders, ledges, pickups, pit, spikes, checkpoint, cave goal, HUD, backdrop, audio)
- [ ] The builder holds no level-specific numbers; the "keep LevelData.cs and LevelBuilder.cs in step" comment is gone
- [ ] Adding a level to the catalog is enough for the command to build its scene
- [ ] The level controller in each built scene carries that level's identity
- [ ] Rules tests still pass; a catalog test checks every level's layout data is complete (width, goal inside width, pickups inside width)
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec and the final manuscript (D-036); anything new is raised with the user first

**Builder to run:** `python Tools/Art/make_level_art.py`, then **LandGrand > Build Mountains Levels**.

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md` (D-055).
