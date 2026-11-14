# 04: Item card for a found item kind

**What to build:** Tapping a found item kind on a shelf opens an item card over the Profile, in the same layout as the in-level item pop-up: the picture in a slot, the name, a "Rock" or "Mineral" tag, the description in Andika and the "Draft — needs teacher review" note (Q-04). The text comes from the item catalog, so it is always the same as the in-level pop-up. An OK button or a tap outside the card closes it. Android's Back closes the card first; a second Back leaves the Profile. Tapping an unfound item kind does nothing.

**Blocked by:** Tickets 03

**Status:** built and reviewed, not committed; awaiting the Editor check

- [ ] Each of Granite, Quartz, Feldspar and Mica opens a card with the right picture, name, tag and description once found
- [ ] Unfound item kinds cannot be opened
- [ ] OK, a tap outside and Android Back each close the card; Back again returns to the Main Menu
- [ ] The longest description fits the card without overflow

- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec, D-052 and the final manuscript (D-036); anything new is raised with the user first
- [ ] The last line of the report names the Unity builder to run

Spec: `../spec.md`. Glossary: `CONTEXT.md`. Design source: vault `Decisions/Decision Register.md` (D-052).
