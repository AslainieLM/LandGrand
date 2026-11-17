# Codex: start here (Hills Hard 1–7)

You are building **Hills Hard 1–7** for LandGrand, a Unity 6000.6.8f1 2D pixel-art game for Grade 5 pupils. **The art, backgrounds, sounds and bot-checked layouts are already made by Claude. Your job is the C# code.**

## Read first

1. `AGENTS.md`.
2. `.scratch/hills-hard/spec.md`: decisions, the kind table, the content, the theme table and the props.
3. `.scratch/hills-hard/layouts.md`: the rule constants and the exact level data.
4. `.scratch/hills-normal/HANDOVER.md` and `CONTEXT.md`: the patterns (`FloatWatcher`, `BatFlock`, `CurrentWatcher`, `BalloonView`, `HillsPropFits`, `LandformAudio`, `ZoneFooting`, `IceWatcher`, the rolling-boulder part).
5. The ticket in `.scratch/hills-hard/issues/`.

## House rules

- **D-061:** level differences are data. No per-level, per-landform or per-theme `if` in play scripts.
- **Rules:** plain C# with no Unity types, with NUnit tests. There are 360 EditMode tests now.
- **Level parts:** new jobs are parts with `Begin(LevelController)` and `IsBuilt`. Moving things read `LevelClock.Now`, and sounds go through `PlaySfx`.
- **Save safety:** never reorder `Episode`, `LevelTheme`, badge ids or kind ids. New values go at the end.
- **Walking:** 84 px/s everywhere. Do not add level speeds.
- **Do not change:**
  - Mountains, Hills Easy or Hills Normal behaviour. `RollingBoulder` and ice must stay identical.
  - Any file under `Tools/Art` or `Tools/Audio`, or any PNG, OGG or WAV. Only run the scripts; if an asset is missing or wrong, report it.
- **Comments:** every new code line ends with a short basic-English comment (D-033).
- **Layout numbers:** use `layouts.md` exactly. If a number can't work, stop and report.
- **Commits and vault:** do not commit or push, and do not edit the vault (`D:\LandGrand\LandGrand-Sources`).
- **Manuscript (D-036):** anything not in the spec is a change. Ask first.
- **End each ticket** with the Unity builder command(s) to run, or "none".

## Commands (Editor closed)

```
"C:/Program Files/Unity/Hub/Editor/6000.6.8f1/Editor/Unity.exe" -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Temp/editmode.xml -logFile Temp/editmode.log
"C:/Program Files/Unity/Hub/Editor/6000.6.8f1/Editor/Unity.exe" -batchmode -projectPath . -executeMethod LevelBuilder.BuildAll -quit -logFile Temp/build.log
```

## Ticket order

| # | Ticket | Blocked by |
|---|---|---|
| 01 | Rules: mud zone, rolling log with a direction, layout lists, ambience script names | none |
| 02 | Level data: Hills Hard 1–7, six kinds, seven themes, ambience overrides | 01 |
| 03 | Play parts and builders: mud, logs, themes, props; build every scene | 01, 02 |
| 04 | Final check and handover | all |
