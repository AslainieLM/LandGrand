# 03: Collection shelves on the Profile

**What to build:** Under the explorer strip, four shelves, one per landform in episode order (Mountains, Hills, Plains, Plateaus), each about 48 px tall with the landform name in the pixel font on the left and up to 6 slots of 32 px, with no scrolling. Each slot shows an item kind from the ticket 01 model: found ones in colour with their name in Andika under them; unfound ones as a dark silhouette of the same picture with "?" as the name. A shelf with no item kinds shows "Coming soon". Slots are drawn each time the Profile opens, so a newly found item shows at once.

**Blocked by:** Tickets 01, 02

**Status:** built and reviewed, not committed; awaiting the Editor check

- [ ] With a fresh profile, the Mountains shelf shows four silhouettes with "?"; the other three shelves show "Coming soon"
- [ ] After finding quartz and mica in Mountains Easy 1, those two show in colour with their names; the others stay silhouettes
- [ ] Everything fits on the 480x270 screen with no overlap and no scrolling
- [ ] The silhouette does not reveal the name or the description

- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec, D-052 and the final manuscript (D-036); anything new is raised with the user first
- [ ] The last line of the report names the Unity builder to run

Spec: `../spec.md`. Glossary: `CONTEXT.md`. Design source: vault `Decisions/Decision Register.md` (D-052).
