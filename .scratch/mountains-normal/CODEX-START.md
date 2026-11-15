# Codex: start here (Mountains Normal 1–5)

You are building **Mountains Normal 1–5** for LandGrand, a Unity 6000.6.8f1 2D pixel-art game for Grade 5 pupils. The design is final. The art, audio and level layouts are already made and checked. **Your job is the C# code.**

## Read first, in this order

1. `AGENTS.md` in the repo root, and the project rules it points to.
2. `.scratch/mountains-normal/spec.md`: the design, the exact numbers and all the text.
3. The ticket you are working on, in `.scratch/mountains-normal/issues/`.

## House rules (every ticket)

- **Code structure (vault D-061):**
  - One script runs every level, and everything that differs between levels is data in `LevelCatalog` (`Assets/Scripts/Rules/LevelData.cs`).
  - Game rules live in `Assets/Scripts/Rules`: plain C# with **no Unity types**, covered by NUnit EditMode tests in `Assets/Tests/EditMode`.
  - The level screen is split into parts on the `Level Controller` object (`LevelController` plus `TrailProgressBar`, `PickupCollector`, `HazardWatcher`, `LevelTimer`, `LevelMenus` and `ResultScreen`). A new job becomes a **new part**, not more fields on `LevelController`.
  - Theme art is one entry per theme in `Assets/Editor/LevelThemes.cs`.
  - Scenes are built only by the editor builders in `Assets/Editor`. Never hand-edit `.unity` files.
- **Comments (D-033):** every new code line ends with a short basic-English comment that says what the line is for. Look at any existing file for the style.
- **Do not commit or push.** The user makes the commits.
- **Do not edit the art or audio scripts or their PNG, WAV and OGG files**, unless a ticket says so. If an asset is missing or looks wrong, say so in your report.
- **Do not change rules that are already settled.** Easy levels must play exactly as before.
- **Manuscript (D-036):** anything not in the spec is a change. Ask before adding it.
- **End each ticket by naming the Unity builder command(s) the user must run** (for example **LandGrand > Build Mountains Levels**).

## Tests

- **Unity EditMode tests (needs the Unity Editor closed):**

  ```
  "C:/Program Files/Unity/Hub/Editor/6000.6.8f1/Editor/Unity.exe" -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Temp/editmode.xml
  ```
- **Build the level scenes in batch mode (Editor closed):**

  ```
  "C:/Program Files/Unity/Hub/Editor/6000.6.8f1/Editor/Unity.exe" -batchmode -projectPath . -executeMethod LevelBuilder.BuildAll -quit -logFile Temp/build.log
  ```

If Unity cannot run where you are, still write the tests and say clearly in your report that they were not run.

## Ticket order

Work in this order, and finish each ticket's checklist before starting the next.

| # | Ticket | Blocked by |
|---|---|---|
| 01 | Two checkpoints and Normal layout data | none |
| 02 | Rules for the four level objects | 01 |
| 03 | Ten new kinds and the five Normal levels' data | 01, 02 |
| 04 | Play scripts and builder for the level objects | 02, 03 |
| 05 | Four new themes, Normal wind and the new sounds | 03, 04 |
| 06 | Profile: Collection pages, 4×3 badges, three new badges | 03 |
| 07 | Final check and handover | all |
