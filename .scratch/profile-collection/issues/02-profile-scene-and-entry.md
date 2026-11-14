# 02: Profile scene, Main Menu portrait and explorer strip

**What to build:** A thin path from the Main Menu to a working Profile. A round 32x32 explorer portrait (the Cave Spelunker's face) sits in the top-right corner of the Main Menu with the explorer's name in small Andika text under it; the four main buttons do not move (LG-16). Tapping it fades to a new Profile scene, built by `ProfileBuilder` (LandGrand > Build Profile Scene) and added to the build list after the Main Menu. The scene reuses the Main Menu backdrop (no title), the calm menu music, the Back icon in the top-left corner and the shared font fix before saving. At the top is the explorer strip: portrait, "Explorer: Ana" (or "Explorer: —" before a name is set) and the three totals from ticket 01 (stars earned, item kinds found n/total, levels cleared). Back and Android's Back return to the Main Menu with the usual fade. The area under the strip is left empty for ticket 03.

**Blocked by:** Tickets 01

**Status:** built and reviewed, not committed; awaiting the Editor check

- [ ] Portrait and name show on the Main Menu without overlapping the title or the buttons at 480x270
- [ ] Tapping the portrait opens the Profile; Back and Android Back return to the Main Menu
- [ ] The strip shows the saved name and the right totals for a fresh and a partly played profile
- [ ] Headings and numbers use Micro 5 at 22 px (D-050); the name uses Andika (D-051)
- [ ] The scripts compile outside Unity, and the Main Menu and Profile builders run without errors

- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec, D-052 and the final manuscript (D-036); anything new is raised with the user first
- [ ] The last line of the report names the Unity builder to run

Spec: `../spec.md`. Glossary: `CONTEXT.md`. Design source: vault `Decisions/Decision Register.md` (D-052).
