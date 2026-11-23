# Codex: start here (Plateaus Normal 1–5)

You are building **Plateaus Normal 1–5** for LandGrand, a Unity 6000.6.9f1 2D pixel-art game for Grade 5 pupils. **The art, backgrounds, sound and bot-checked layouts are already made by Claude. Your job is the C# code.**

## Read first

1. `AGENTS.md`.
2. `.scratch/plateaus-normal/spec.md`: decisions, levels, code changes, objects, content, assets, theme table and props.
3. `.scratch/plateaus-normal/layouts.md`: the exact level data and the bot check.
4. These show the patterns to copy:
   - `.scratch/plateaus-easy/HANDOVER.md`, `.scratch/plains-normal/HANDOVER.md` and `.scratch/plains-hard/HANDOVER.md`
   - `CONTEXT.md`
   - in code: the Plains Normal market boats (`PlatformSprite`, `PlatformLandSound`), the Plains Hard herds and frost (`BatFrames`, `BatSound`, `IceTop`), the Plateaus Easy updrafts, and the trail prop helpers
5. The ticket in `.scratch/plateaus-normal/issues/`.

## House rules

- **D-061:** level differences are data. No per-level, per-landform or per-theme `if` in play scripts.
- **Rules:** plain C# with no Unity types, with NUnit tests. No new rules in this build.
- **Level parts:** use `Begin(LevelController)` and `IsBuilt`. Timed things read `LevelClock.Now`, and sounds go through `PlaySfx`.
- **Save safety:** never reorder `Episode`, `LevelTheme`, badge ids or kind ids; add at the end.
- **Walking:** 84 px/s everywhere.
- **Do not change:**
  - Mountains, Hills, Plains or Plateaus Easy behaviour, numbers or text. Old platforms, herds, bats and ice must look and move exactly as before.
  - Anything under `Tools/Art` or `Tools/Audio`, or any PNG, OGG or WAV. Only run the scripts, and report any missing or wrong asset.
- **Comments:** every new code line ends with a short basic-English comment (D-033).
- **Layout numbers:** use `layouts.md` exactly. If a number cannot work, stop and report.
- **Commits and vault:** do not commit or push; do not edit the vault (`D:\LandGrand\LandGrand-Sources`).
- **Changes:** anything not in the spec is a change. Ask first.
- **End each ticket** with the Unity builder command(s) to run, or "none".
- **Ticket status:** when a ticket is finished, set its Status line to `done (<date>, <test count>)`.

## Commands (Editor closed)

```
"C:/Program Files/Unity/Hub/Editor/6000.6.9f1/Editor/Unity.exe" -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Temp/editmode.xml -logFile Temp/editmode.log
"C:/Program Files/Unity/Hub/Editor/6000.6.9f1/Editor/Unity.exe" -batchmode -projectPath . -executeMethod LevelBuilder.BuildAll -quit -logFile Temp/build.log
```

Run the other builders the same way, one at a time: `LessonBuilder.Build`, `SelectionBuilder.Build`, `LandformMapBuilder.Build`, `LandformDescriptionBuilder.Build` and `ProfileBuilder.Build`.

## Ticket order

| # | Ticket | Blocked by |
|---|---|---|
| 01 | Kinds and themes | none |
| 02 | Plateaus Normal 1–5 level data | 01 |
| 03 | Themes, cable cars, llamas, salt crust, props; build every scene | 01, 02 |
| 04 | Final check and handover | all |
