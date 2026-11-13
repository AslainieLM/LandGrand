# 04: Landform Map

**What to build:** Play opens one connected pixel-art Landform Map in the user's 2×2 placement: Mountains top left, Hills top right, Plains bottom left, Plateaus bottom right. Mountains is unlocked and animated (blowing snow, a drifting cloud, a circling eagle). The other regions are grey with a padlock and no animation; tapping one shows a guide such as "Finish Mountains first" and stays on the map. Each region shows its star progress from the save, the player's name is at the top, and Back in the top-left corner returns to the Main Menu.

**Blocked by:** Tickets 02, 03

**Status:** done, committed by the user

- [ ] The Main Menu's Play opens the map with the screen fade
- [ ] Region looks follow the saved progress: unlocked animate, locked are grey with a padlock and still
- [ ] Tapping a locked region shows its guide text and stays on the map
- [ ] Star progress per region shows from the save (0/45 for a fresh profile)
- [ ] Back (top left) and the Android Back button return to the Main Menu
- [ ] Art uses the LandGrand palette at 480×270 and stays pixel-sharp
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec and the final manuscript (D-036); anything new is raised with the user first

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md`.
