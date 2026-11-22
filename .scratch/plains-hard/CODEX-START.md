# Codex: start here (Plains Hard 1–7)

You are building **Plains Hard 1–7** for LandGrand, a Unity 6000.6.9f1 2D pixel-art game for Grade 5 pupils. **The art, backgrounds, sound and bot-checked layouts are already made by Claude. Your job is the C# code.**

## Read first

1. `AGENTS.md`.
2. `.scratch/plains-hard/spec.md`: decisions, outline, code changes, content, assets, theme table and props.
3. `.scratch/plains-hard/layouts.md`: the exact level data and the bot check.
4. These show the patterns to copy:
   - `.scratch/plains-normal/HANDOVER.md` and `.scratch/hills-hard/HANDOVER.md`
   - `CONTEXT.md`
   - in code: the Hills Hard levels (rolling logs, mud as the ice rule), the Hills Normal bats (`BatFlock`), Mountains Hard ice, the Plains Normal `PlatformSprite` / `PlatformLandSound` theme fields (optional theme art with a null default), `PlaceTrailProps`, `RepeatTrailProp` and `TrailPropFits`
5. The ticket in `.scratch/plains-hard/issues/`.

## House rules

- **D-061:** level differences are data. No per-level, per-landform or per-theme `if` in play scripts.
- **Rules:** plain C# with no Unity types, with NUnit tests. Do not change the `Bat`, `RollingLog` or `IceZone` rules.
- **Level parts:** use `Begin(LevelController)` and `IsBuilt`. Timed things read `LevelClock.Now`, and sounds go through `PlaySfx`.
- **Save safety:** never reorder `Episode`, `LevelTheme`, badge ids or kind ids; add at the end.
- **Walking:** 84 px/s everywhere.
- **Do not change:**
  - Mountains, Hills, Plains Easy or Plains Normal behaviour, numbers or text. Old logs, bats and ice must look and move exactly as before.
  - Anything under `Tools/Art` or `Tools/Audio`, or any PNG, OGG or WAV. Only run the scripts, and report any missing or wrong asset.
- **Comments:** every new code line ends with a short basic-English comment (D-033).
- **Layout numbers:** use `layouts.md` exactly. If a number cannot work, stop and report.
- **Commits and vault:** do not commit or push; do not edit the vault (`D:\LandGrand\LandGrand-Sources`).
- **Changes:** anything not in the spec is a change. Ask first, and list it in `HANDOVER.md` under "Changes not in the spec".
- **End each ticket** with the Unity builder command(s) to run, or "none".

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
| 02 | Plains Hard 1–7 level data | 01 |
| 03 | Themes, hay, herds, frost, props; build every scene | 01, 02 |
| 04 | Final check and handover | all |
