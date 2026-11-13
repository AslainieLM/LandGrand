# 12: Mountains audio

**What to build:** The Mountains level has its own original music loop, quieter than the menu, and a light-breeze ambience for Easy (D-042). Both loop without a gap, fade in and out with the screen fade, and follow the Settings sliders; pickups, hazards and buttons use the sound-effects slider.

**Blocked by:** Tickets 07

**Status:** built 2026-11-13, awaiting the user's Editor check. Decision: while paused or after Time up the music and breeze play on at 30% loudness (smooth change); the first-find pop-up does not lower them.

- [ ] Music and ambience are original, made with the project's music script approach, and loop seamlessly
- [ ] Settings sliders change the level music, ambience and sound effects
- [ ] Audio fades with scene changes and stops while paused (music may continue quietly; decide and note it)
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec and the final manuscript (D-036); anything new is raised with the user first

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md`.
