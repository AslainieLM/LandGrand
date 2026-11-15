# Codex: start here (Mountains Hard 1–7 and the Landform Description Screen)

You are building **Mountains Hard 1–7** and the **Landform Description Screen** for LandGrand, a Unity 6000.6.8f1 2D pixel-art game for Grade 5 pupils. The design is final. The art, audio and level layouts are already made and checked. **Your job is the C# code.**

## Read first, in this order

1. `AGENTS.md` in the repo root, and the project rules it points to.
2. `.scratch/mountains-hard/spec.md`: the design, the exact numbers and all the text.
3. `.scratch/mountains-normal/HANDOVER.md`: how the Normal work was built (same patterns).
4. The ticket you are working on, in `.scratch/mountains-hard/issues/`.

## House rules (every ticket)

- **Code structure (vault D-061):**
  - One script runs every level, and everything that differs between levels is data in `LevelCatalog` (`Assets/Scripts/Rules/LevelData.cs`). **No per-level code.**
  - Game rules live in `Assets/Scripts/Rules`: plain C# with **no Unity types**, covered by NUnit EditMode tests in `Assets/Tests/EditMode`.
  - The level screen is split into parts on the `Level Controller` object (`LevelController` plus `TrailProgressBar`, `PickupCollector`, `HazardWatcher`, `LevelTimer`, `LevelMenus`, `ResultScreen` and `WindWatcher`, each with `Begin(LevelController)` and `IsBuilt`). A new job becomes a **new part**, not more fields on `LevelController`.
  - Theme art is one entry per theme in `Assets/Editor/LevelThemes.cs`, and each theme names its own `Backdrop` folder (D-063).
  - Moving objects read `LevelClock.Now`, so they pause during the banner, the pause menu and the find pop-up.
  - Scenes are built only by the editor builders in `Assets/Editor`. Never hand-edit `.unity` files.
- **Comments (D-033):** every new code line ends with a short basic-English comment that says what the line is for. Look at any existing file for the style.
- **Do not commit or push.** The user makes the commits.
- **Do not edit the art or audio scripts or their PNG, WAV and OGG files**, unless a ticket says so. If an asset is missing or looks wrong, say so in your report.
- **Do not change rules that are already settled.** Easy and Normal levels must play exactly as before.
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

There are 198 EditMode tests now. If Unity cannot run where you are, still write the tests and say clearly in your report that they were not run.

## Ticket order

Work in this order, and finish each ticket's checklist before starting the next.

| # | Ticket | Blocked by |
|---|---|---|
| 01 | Three checkpoints and Hard layout data | none |
| 02 | Rules for the rolling boulder and icy ground | 01 |
| 03 | Fourteen new kinds and the seven Hard levels' data | 01, 02 |
| 04 | Play scripts and builder for the rolling boulder and icy ground | 02, 03 |
| 05 | Seven new themes, Hard wind and the new sounds | 03, 04 |
| 06 | Landform unlock rules and the map | 03 |
| 07 | Landform Description Screen | 06 |
| 08 | Profile: third Collection page, 5×3 badges, four new badges | 03 |
| 09 | Final check and handover | all |
