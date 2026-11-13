# 03: Pixel font, name entry and Main Menu updates

**What to build:** The first time the player taps Play, they type their name with the phone's keyboard before the Landform Map opens. Text that the player types, and longer text later, uses a bundled Open Font License pixel font through TextMeshPro (proposed Pixelify Sans). Settings lets the player change the name. Tutorial page 1 mentions the double jump.

**Blocked by:** Tickets 02

**Status:** ready-for-agent

- [ ] The font's licence is checked and its licence file is kept in the project; if Pixelify Sans cannot be used, another OFL pixel font is chosen and noted in the vault
- [ ] First Play shows the name panel; names of 1–12 letters, numbers or spaces are accepted, others are refused with a clear message
- [ ] The name is saved at once; later taps on Play skip the panel
- [ ] Settings shows the name and lets the player change it
- [ ] Tutorial page 1 says to tap again in the air to double jump
- [ ] Works with the Android Back button and keeps the pixel-perfect look
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec and the final manuscript (D-036); anything new is raised with the user first

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md`.
