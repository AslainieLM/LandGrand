# 05: Difficulty selection and level trail (Mountains)

**What to build:** Tapping Mountains opens a separate difficulty screen with three large cards, Easy, Normal and Hard, each showing its stars (0/9, 0/15, 0/21). Locked cards are grey with a padlock and "Clear all Easy levels" or "Clear all Normal levels". Tapping an unlocked difficulty opens a level trail with one stone per level showing its stars; levels after the next playable one are locked. Back returns one step on each screen.

**Blocked by:** Tickets 04

**Status:** done, committed by the user

- [ ] Difficulty cards show stars and locks from the saved progress
- [ ] Tapping a locked card shows its guide and stays on the screen
- [ ] The Easy trail shows 3 stones; only available levels can be tapped
- [ ] Back and the Android Back button go back one step: trail to difficulties to map
- [ ] The mountains background animates slowly behind the cards
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec and the final manuscript (D-036); anything new is raised with the user first

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md`.
