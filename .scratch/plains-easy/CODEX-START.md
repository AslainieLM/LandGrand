# Codex: start here (Plains Easy 1–3)

You are building **Plains Easy 1–3** for LandGrand, a Unity 6000.6.9f1 2D pixel-art game for Grade 5 pupils. **The art, backgrounds, badges, sounds and bot-checked layouts are already made by Claude. Your job is the C# code.**

## Read first

1. `AGENTS.md`.
2. `.scratch/plains-easy/spec.md`: the decisions, outline, code-change table, straw pile rules, level data, content, theme table, props and life.
3. `.scratch/plains-easy/layouts.md`: the rule constants, the exact level data and the map life spots.
4. These show the patterns to copy:
   - `.scratch/hills/spec.md` and `.scratch/hills-hard/HANDOVER.md`
   - `CONTEXT.md`
   - in code: the Hills Easy levels, `SandWatcher`, `HillsPropFits`, `HillsMenuLife`, `HillsLife`, `LandformAudio` and the Hills badges
5. The ticket in `.scratch/plains-easy/issues/`.

## House rules

- **D-061:** level differences are data. No per-level, per-landform or per-theme `if` in play scripts.
- **Rules:** plain C# with no Unity types, with NUnit tests.
- **Level parts:** new jobs are parts with `Begin(LevelController)` and `IsBuilt`. Moving things read `LevelClock.Now`, and sounds go through `PlaySfx`.
- **Save safety:** never reorder `Episode`, `LevelTheme`, badge ids or kind ids. New values go at the end.
- **Walking:** 84 px/s everywhere. Do not add level speeds.
- **Do not change:**
  - Mountains or Hills behaviour, numbers or text. Boulders, vents, sand, ice, mud and logs must stay identical.
  - Any file under `Tools/Art` or `Tools/Audio`, or any PNG, OGG or WAV. Only run the scripts; if an asset is missing or wrong, report it.
- **Comments:** every new code line ends with a short basic-English comment (D-033).
- **Layout numbers:** use `layouts.md` exactly. If a number cannot work, stop and report.
- **Commits and vault:** do not commit or push, and do not edit the vault (`D:\LandGrand\LandGrand-Sources`).
- **Manuscript (D-036):** anything not in the spec is a change. Ask first.
- **End each ticket** with the Unity builder command(s) to run, or "none".

## Commands (Editor closed)

```
"C:/Program Files/Unity/Hub/Editor/6000.6.9f1/Editor/Unity.exe" -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Temp/editmode.xml -logFile Temp/editmode.log
"C:/Program Files/Unity/Hub/Editor/6000.6.9f1/Editor/Unity.exe" -batchmode -projectPath . -executeMethod LevelBuilder.BuildAll -quit -logFile Temp/build.log
```

Run the other builders the same way, one at a time: `LessonBuilder.Build`, `SelectionBuilder.Build`, `LandformMapBuilder.Build`, `LandformDescriptionBuilder.Build`, `ProfileBuilder.Build`. If the Editor path differs, find the installed 6000.6.9f1 editor in the Unity Hub folder.

## Ticket order

| # | Ticket | Blocked by |
|---|---|---|
| 01 | Rules: straw pile, layout list, themes, Plains kinds, audio row, summary, badges | none |
| 02 | Level data: Plains Easy 1–3 | 01 |
| 03 | Straw bounce part, themes, props, map and menu life; build every scene | 01, 02 |
| 04 | Final check and handover | all |
