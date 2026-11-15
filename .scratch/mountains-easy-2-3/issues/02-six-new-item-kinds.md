# 02: Six new item kinds and a Mountains shelf of 10

**What to build:** Add andesite, pumice, obsidian, slate, marble (rocks) and garnet (mineral) to the item catalog, each with home landform Mountains, draft flag on, and the descriptions in the spec's item table. Draw each item sprite with its own hand-written function in the lesson art script, next to granite, quartz, feldspar and mica, in the LandGrand palette at the same size as the existing four. The Profile's Mountains shelf and the totals then show 10 kinds with no other change (they already count every kind).

**Blocked by:** None

**Status:** to do

- [ ] The six kinds exist with the spec's names, Rock/Mineral flags, descriptions and home landform Mountains
- [ ] Each has a sprite that reads clearly at pickup size, in the lesson boxes and on the item card; obsidian looks glassy black, pumice light and holey, garnet dark red crystals
- [ ] The Profile shows 10 Mountains kinds; unfound ones appear as dark shapes with "?"; Found n/total shows the new total
- [ ] Item names in boxes and descriptions use Andika (D-051); the card fits the longest description
- [ ] Collection tests cover the shelf of 10 and the new total
- [ ] Text marked "Draft — needs teacher review" (Q-04)
- [ ] Every new line of code has a short basic-English comment (D-033)

**Builder to run:** `python Tools/Art/make_lesson_art.py`, then **LandGrand > Build Profile Scene**.

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md` (D-055).
