# Codex: start here (Hills Easy 1–3, and making LandGrand work with more than one landform)

You are building **Hills Easy 1–3** for LandGrand, a Unity 6000.6.8f1 2D pixel-art game for Grade 5 pupils. Hills is the second landform, after Mountains. You are also making the code work with more than one landform. The design is final. The art, audio and level layouts are already made and checked. **Your job is the C# code.**

## Read first, in this order

1. `AGENTS.md` in the repo root, and the project rules it points to.
2. `.scratch/hills/spec.md`: the design, the exact numbers and all the text.
3. `.scratch/mountains-hard/HANDOVER.md`: how the Mountains Hard work was built (same patterns).
4. The ticket you are working on, in `.scratch/hills/issues/`.

## House rules (every ticket)

- **Code structure (vault D-061):**
  - One script runs every level, and everything that differs between levels is data in `LevelCatalog` (`Assets/Scripts/Rules/LevelData.cs`). **No per-level code, and no per-landform branches in play scripts.** A difference between landforms is a data table in Rules.
  - Game rules live in `Assets/Scripts/Rules`: plain C# with **no Unity types**, covered by NUnit EditMode tests in `Assets/Tests/EditMode`.
  - The level screen is split into parts on the `Level Controller` object, each with `Begin(LevelController)` and `IsBuilt`. A new job becomes a **new part** (for example `SandWatcher`), not more fields on `LevelController`.
  - Theme art is one entry per theme in `Assets/Editor/LevelThemes.cs`, and each theme names its own `Backdrop` folder (D-063).
  - Moving objects read `LevelClock.Now`, so they pause during the banner, the pause menu and the find pop-up.
  - Scenes are built only by the editor builders in `Assets/Editor`. Never hand-edit `.unity` files.
  - **Never reorder the `Episode` enum or existing badge ids.** Their numbers and ids are in the save file.
- **Comments (D-033):** every new code line ends with a short basic-English comment that says what the line is for. Look at any existing file for the style.
- **Do not commit or push.** The user makes the commits.
- **Do not edit the art or audio scripts or their PNG, WAV and OGG files.** If an asset is missing or looks wrong, say so in your report.
- **Do not change rules that are already settled.** Mountains Easy, Normal and Hard must play exactly as before.
- **Manuscript (D-036):** anything not in the spec is a change. Ask before adding it.
- **End each ticket by naming the Unity builder command(s) the user must run** (for example **LandGrand > Build Levels**).

## Tests

- **Unity EditMode tests (needs the Unity Editor closed):**

  ```
  "C:/Program Files/Unity/Hub/Editor/6000.6.8f1/Editor/Unity.exe" -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Temp/editmode.xml
  ```
- **Build the level scenes in batch mode (Editor closed):**

  ```
  "C:/Program Files/Unity/Hub/Editor/6000.6.8f1/Editor/Unity.exe" -batchmode -projectPath . -executeMethod LevelBuilder.BuildAll -quit -logFile Temp/build.log
  ```

There are 234 EditMode tests now. If Unity cannot run where you are, still write the tests and say clearly in your report that they were not run.

## Ticket order

Work in this order, and finish each ticket's checklist before starting the next.

| # | Ticket | Blocked by |
|---|---|---|
| 01 | Shared kinds, per-landform audio table and summary data | none |
| 02 | Hills Easy 1–3 level data and the three themes | 01 |
| 03 | Soft sand: rule, play part and builder | 02 |
| 04 | "Coming soon" difficulties and the Result Screen's Next | 02 |
| 05 | Hills life on the map and the menu screens | none |
| 06 | Badges per landform, the Profile badge pages and the Description totals | 01 |
| 07 | Final check and handover | all |

Tickets 01 and 05 can start at once. After 02, tickets 03 and 04 can be done in any order. Ticket 06 needs only 01.
