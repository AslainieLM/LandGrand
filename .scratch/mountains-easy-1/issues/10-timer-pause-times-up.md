# 10: Timer, pause menu and Time's up

**What to build:** A 2:00 countdown shows at the top centre and stops while the game is paused or a pop-up is open. A pause button at the top right opens Resume, Retry and Back to Main Menu. When the timer reaches zero, "Time's up!" offers Retry, which starts again from the Lesson Screen, or Back to Main Menu.

**Blocked by:** Tickets 07

**Status:** ready-for-agent

- [ ] Tests: time counts down while playing, stops while paused and during the pop-up, and reaching zero gives time up; retry resets the attempt
- [ ] The pause menu's three buttons work; the Android Back button opens the pause menu during play
- [ ] Time's up shows Retry and Back to Main Menu; Retry goes to the Lesson Screen
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec and the final manuscript (D-036); anything new is raised with the user first

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md`.
