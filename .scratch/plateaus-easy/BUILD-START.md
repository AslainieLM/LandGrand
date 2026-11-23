# Build: start here (Plateaus Easy 1–3)

You are building **Plateaus Easy 1–3** for LandGrand, a Unity 6000.6.9f1 2D pixel-art game for Grade 5 pupils. **The art, backgrounds, badges, sound and bot-checked layouts are already made by Claude. A Claude Code session builds the C# code, one ticket per session, from `SESSION-PROMPT.md`.**

## Read first

1. `AGENTS.md`.
2. `.scratch/plateaus-easy/spec.md`: decisions, outline, code changes, updraft rules, content, assets, theme table and props.
3. `.scratch/plateaus-easy/layouts.md`: the exact level data, the bot check and the map life.
4. These show the patterns to copy:
   - `.scratch/plains-hard/HANDOVER.md` and `.scratch/plains-easy/HANDOVER.md`
   - `CONTEXT.md`
   - in code: the Plains Easy straw pile (`Rules/StrawPile.cs`, `Play/StrawBounceWatcher.cs`, its builder and tests), the wind's `ExternalPushX` (how a watcher feeds the motor each frame), `PlaceTrailProps`, and the Plains map and menu life
5. The ticket in `.scratch/plateaus-easy/issues/`.

## House rules

- **D-061:** level differences are data. No per-level, per-landform or per-theme `if` in play scripts.
- **Rules:** plain C# with no Unity types, with NUnit tests.
- **Level parts:** use `Begin(LevelController)` and `IsBuilt`. Timed things read `LevelClock.Now`, and sounds go through `PlaySfx` or the level's sfx volume.
- **Save safety:** never reorder `Episode`, `LevelTheme`, badge ids or kind ids; add at the end.
- **Walking:** 84 px/s everywhere.
- **Do not change:**
  - Mountains, Hills or Plains behaviour, numbers or text. With no updraft the explorer must move exactly as before.
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

Run the other builders the same way, one at a time: `LessonBuilder.Build`, `SelectionBuilder.Build`, `LandformMapBuilder.Build`, `LandformDescriptionBuilder.Build` and `ProfileBuilder.Build`.

## Ticket order

| # | Ticket | Blocked by |
|---|---|---|
| 01 | Rules: updraft, kinds, themes, audio, summary, badges | none |
| 02 | Plateaus Easy 1–3 level data | 01 |
| 03 | Updraft in play, themes, props, map and menu life; build every scene | 01, 02 |
| 04 | Final check and handover | all |
