# 05: Move name editing from Settings to the Profile

**What to build:** The pencil next to "Explorer: name" in the Profile strip opens a name panel in the Profile scene that works like the Main Menu one: same name rules (1 to 12 letters, numbers or spaces), prefilled with the saved name, the phone's keyboard, OK saves at once and updates the strip, Back or Android Back cancels and returns to the Profile. The "Explorer" row and pencil are removed from the Settings panel, which keeps only the two volume sliders, the Back icon and Save. The first-Play name prompt on the Main Menu stays as it is. The Main Menu portrait shows the new name when the player returns.

**Blocked by:** Tickets 02

**Status:** built and reviewed, not committed; awaiting the Editor check

- [ ] Changing the name in the Profile saves it, updates the strip and the Main Menu portrait label
- [ ] A name that breaks the rules shows the same warning as on first Play and is not saved
- [ ] Back on the name panel returns to the Profile without changes
- [ ] Settings shows no name row; Save and Back still work as before
- [ ] First Play with no saved name still asks for the name before the Landform Map

- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec, D-052 and the final manuscript (D-036); anything new is raised with the user first
- [ ] The last line of the report names the Unity builder to run

Spec: `../spec.md`. Glossary: `CONTEXT.md`. Design source: vault `Decisions/Decision Register.md` (D-052).
