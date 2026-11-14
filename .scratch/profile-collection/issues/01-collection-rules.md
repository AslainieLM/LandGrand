# 01: Collection rules and Profile totals

**What to build:** The rules behind the Collection, in the pure C# rules assembly. Every item kind has a home landform (Granite, Quartz, Feldspar and Mica: Mountains). A small Collection model gives, for each landform, its shelf: the item kinds whose home landform it is, in lesson order, each marked found or not found from the player's progress, and whether the shelf is "Coming soon" (no item kinds yet). It also gives the Profile totals: stars earned (sum of best stars), levels cleared, item kinds found and the total number of item kinds. The future Completion Screen (LG-15) will use the same model. The save format does not change.

**Blocked by:** None

**Status:** built and reviewed, not committed; awaiting the Editor check

- [ ] Tests: every item kind has a home landform; the Mountains shelf is Granite, Quartz, Feldspar, Mica in that order
- [ ] Tests: Hills, Plains and Plateaus shelves are "Coming soon"
- [ ] Tests: found state follows the player's progress; found count and total count are right for a fresh and a partly played profile
- [ ] Tests: stars and cleared levels add up correctly, and a worse replay does not lower them (D-045)
- [ ] All existing rules tests still pass

- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec, D-052 and the final manuscript (D-036); anything new is raised with the user first
- [ ] The last line of the report names the Unity builder to run

Spec: `../spec.md`. Glossary: `CONTEXT.md`. Design source: vault `Decisions/Decision Register.md` (D-052).
