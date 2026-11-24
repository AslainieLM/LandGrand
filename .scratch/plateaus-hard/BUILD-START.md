# Build: start here (Plateaus Hard 1–7 and the Completion Screen)

You are building **Plateaus Hard 1–7 and the Completion Screen** for LandGrand, a Unity 6000.6.9f1 2D pixel-art game for Grade 5 pupils. **The art, backgrounds, sounds and bot-checked layouts are already made by Claude. A Claude Code session builds the C# code, one ticket per session, from `SESSION-PROMPT.md`.** This is the last episode.

## Read first

1. `AGENTS.md`.
2. `.scratch/plateaus-hard/spec.md`: decisions, levels, code changes, objects, content, the Completion Screen, assets, theme table and props.
3. `.scratch/plateaus-hard/layouts.md`: the exact level data and the bot check.
4. These show the patterns to copy:
   - `.scratch/plateaus-normal/HANDOVER.md` and `.scratch/plains-hard/HANDOVER.md`
   - `CONTEXT.md`
   - in code: the Plains Hard and Plateaus Normal levels, `ThemeArt` optional fields with null defaults (`PlatformSprite`, `BatFrames`, `IceTop` and their script fields), the vent build in `LevelBuilder.cs`, the gust build in `LevelObjects.cs`, the strips in `HardLevelObjects.cs`, `PlaceTrailProps`, `LandformDescriptionController`/`LandformDescriptionBuilder`, `LandformMapController` (`LandGrand.UnlockSeen.`), `LandformMapBuilder`
5. The ticket in `.scratch/plateaus-hard/issues/`.

## House rules

- **D-061:** level differences are data. No per-level, per-landform or per-theme `if` in play scripts.
- **Rules:** plain C# with no Unity types, with NUnit tests. Do not change the `SteamVent`, `WindZone`, `SandZone`, `IceZone`, `Bat`, `Updraft`, `CrumbleLedge` or `MovingPlatform` rules.
- **Level parts:** use `Begin(LevelController)` and `IsBuilt`. Timed things read `LevelClock.Now`, and sounds go through `PlaySfx`.
- **Save safety:** never reorder `Episode`, `LevelTheme`, badge ids or kind ids; add at the end. No new badges. The save file format does not change.
- **Walking:** 84 px/s everywhere.
- **Do not change:**
  - Mountains, Hills, Plains, Plateaus Easy or Plateaus Normal behaviour, numbers or text. Old vents, gusts, sand, herds, ice and platforms must look and move exactly as before.
  - Anything under `Tools/Art` or `Tools/Audio`, or any PNG, OGG or WAV. Only run the scripts, and report any missing or wrong asset.
- **Comments:** every new code line ends with a short basic-English comment (D-033).
- **Layout numbers:** use `layouts.md` exactly. If a number cannot work, stop and report.
- **Commits and vault:** do not commit or push; do not edit the vault (`D:\LandGrand\LandGrand-Sources`).
- **Changes:** anything not in the spec is a change. Ask first, and list it in `HANDOVER.md` under "Changes not in the spec".
- **End each ticket** with the Unity builder command(s) to run, or "none".
- **Ticket status:** when a ticket is finished, set its Status line to `done (<date>, <test count>)`.

## Commands (Editor closed)

```
"C:/Program Files/Unity/Hub/Editor/6000.6.9f1/Editor/Unity.exe" -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Temp/editmode.xml -logFile Temp/editmode.log
"C:/Program Files/Unity/Hub/Editor/6000.6.9f1/Editor/Unity.exe" -batchmode -projectPath . -executeMethod LevelBuilder.BuildAll -quit -logFile Temp/build.log
```

Run the other builders the same way, one at a time: `LessonBuilder.Build`, `SelectionBuilder.Build`, `LandformMapBuilder.Build`, `LandformDescriptionBuilder.Build`, `ProfileBuilder.Build` and (after ticket 04) `CompletionBuilder.Build`.

## Ticket order

| # | Ticket | Blocked by |
|---|---|---|
| 01 | Kinds, themes, `IsGameComplete`, `CompletionSummary` | none |
| 02 | Plateaus Hard 1–7 level data | 01 |
| 03 | Theme fields, seven themes, props; build every level scene | 01, 02 |
| 04 | Completion Screen, last Description Screen, map flag | 01 |
| 05 | Final check and handover | all |
