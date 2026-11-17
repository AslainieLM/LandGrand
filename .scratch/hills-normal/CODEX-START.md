# Codex: start here (Hills Normal 1–5)

You are building **Hills Normal 1–5** for LandGrand, a Unity 6000.6.8f1 2D pixel-art game for Grade 5 pupils. This time **you also make the art and the sounds**, with Python scripts, the way the earlier scripts in `Tools/Art` and `Tools/Audio` do.

The design is final:
- Claude designed and bot-checked the level layouts.
- The content (lessons, reviews, kinds) is written. It is a draft for teacher review.

## Read first, in this order

1. `AGENTS.md` in the repo root.
2. `.scratch/hills-normal/spec.md`: the decisions, the content, and every sprite name and size.
3. `.scratch/hills-normal/layouts.md`: the rule constants and the exact level data.
4. `.scratch/hills/HANDOVER.md`: how Hills Easy was built (same patterns: `SandWatcher`, `HillsPropFits`, `LandformAudio`).
5. `CONTEXT.md`: the glossary words.
6. The ticket you are working on, in `.scratch/hills-normal/issues/`.

## House rules (every ticket)

- **Code structure (vault D-061):**
  - One script runs every level. Differences between levels are data in `LevelCatalog`.
  - No per-level code, and no per-landform or per-theme `if` branches in play scripts.
  - Rules live in `Assets/Scripts/Rules` as plain C# with **no Unity types**, covered by NUnit EditMode tests in `Assets/Tests/EditMode`.
  - A new job on the level screen is a **new part** with `Begin(LevelController)` and `IsBuilt`, like `SandWatcher` and `IceWatcher`.
  - Moving things read `LevelClock.Now`, so they pause with the game.
  - Sounds go through `LevelController.PlaySfx(clip, volume)`.
  - Scenes are built only by the editor builders. Never hand-edit `.unity` files.
- **Save safety:**
  - Never reorder the `Episode` or `LevelTheme` enums, or existing badge and kind ids.
  - New `LevelTheme` values go at the end.
- **Comments (D-033):** every new code line ends with a short basic-English comment that says what it is for. Python scripts too: follow `make_hills_easy_art.py`.
- **Art (D-028 practice):**
  - Pixel art at the exact sizes in spec section 8, built from the palette in `make_mainmenu_art` and its helpers.
  - No anti-aliasing, and no colours outside the palette mixes.
  - Each script writes a preview PNG under `.scratch/hills-normal/`. Look at it and fix what looks wrong before you report.
- **Do not change:**
  - Mountains, or Hills Easy 1–3. They must play exactly as before.
  - Existing art or audio files.
- **Do not commit or push.** The user makes the commits.
- **Manuscript (D-036):** anything not in the spec is a change. Write it down and ask; do not add it yourself.
- **End each ticket** by naming the Unity builder command(s) the user must run (for example **LandGrand > Build Levels**), or "none".

## Commands

The Unity Editor must be closed for batch mode.

```
"C:/Program Files/Unity/Hub/Editor/6000.6.8f1/Editor/Unity.exe" -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Temp/editmode.xml -logFile Temp/editmode.log
"C:/Program Files/Unity/Hub/Editor/6000.6.8f1/Editor/Unity.exe" -batchmode -projectPath . -executeMethod LevelBuilder.BuildAll -quit -logFile Temp/build.log
```

There are **296** EditMode tests now. If Unity cannot run where you are, still write the tests and say clearly that they were not run.

Python: `python Tools/Art/<script>.py` from the repo root, with Pillow and ffmpeg (already used by the earlier scripts).

## Ticket order

| # | Ticket | Blocked by |
|---|---|---|
| 01 | Rules: float, bat, current, vertical balloon, ambience set per level | none |
| 02 | Art: tiles, objects, goals, props, items, lesson pictures | none |
| 03 | Backgrounds: five themes, six layers each | none |
| 04 | Audio: cave drips, splash, bat flap, burner | none |
| 05 | Level data: Hills Normal 1–5, seven kinds, five themes | 01 |
| 06 | Play parts and builders: floats, bats, currents, balloons, themes, props | 01, 02, 03, 04, 05 |
| 07 | Final check and handover | all |

Tickets 01–04 can start at once. Finish each ticket's checklist before you mark it done.
