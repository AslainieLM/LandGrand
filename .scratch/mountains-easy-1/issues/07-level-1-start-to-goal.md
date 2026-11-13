# 07: Level 1, playable from start to goal

**What to build:** The Mountains Easy 1 level can be played from start to finish. It is a side-scrolling mountain trail 2–3 screens wide in new palette pixel art, with boulders and ledges to jump and a cave entrance with a flag as the goal. The Cave Spelunker explorer walks while an arrow is held, jumps, and double jumps a little lower with a dust puff. Large on-screen buttons sit bottom left (arrows) and bottom right (jump); keyboard arrows and space also work in the Editor. The camera follows the explorer. Reaching the goal clears the level, saves it, and shows a basic result.

**Blocked by:** Tickets 06

**Status:** ready-for-agent

- [ ] The level runs at the target pixel-perfect look and frame rate in the Editor
- [ ] Walk, jump and double jump feel responsive with the touch buttons and the keyboard
- [ ] The level can be finished in about 90 s without hazards
- [ ] Reaching the goal records the clear in saved progress and opens a basic result
- [ ] A level-run part of the rules core handles start and goal, with tests
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec and the final manuscript (D-036); anything new is raised with the user first

**Built 2026-11-13, awaiting the user's Editor check (so boxes stay open):**

**Added 2026-11-13 after the first build (D-046, vault `Project/Gameplay Design.md` "Level look and feel"):**

- [ ] The background scrolls in layers (parallax) as the explorer walks, reusing the start screen's 8 mountain layers and its layer-scrolling script
- [ ] The double jump is a full forward somersault of about 0.3 s, drawn as 4–6 new pixel-art frames (no sprite rotation); the dust puff stays
- [ ] The level is longer: about 60–90 s to finish without hazards; the item places in `LevelData.cs` move with the new layout

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md`.
