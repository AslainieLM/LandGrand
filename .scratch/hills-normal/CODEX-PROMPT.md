You are building Hills Normal 1–5 for LandGrand, a Unity 6000.6.8f1 2D pixel-art game for Grade 5 pupils, in D:\LandGrand\LandGrand. This time you also make the art and the sounds, with Python scripts.

Read, in this order:
1. AGENTS.md in the repo root.
2. .scratch/hills-normal/CODEX-START.md (house rules, commands, ticket order).
3. .scratch/hills-normal/spec.md (decisions, content, every sprite name and size, theme table, props, audio).
4. .scratch/hills-normal/layouts.md (rule constants and the exact level data; bot-checked by Claude).
5. .scratch/hills/HANDOVER.md and CONTEXT.md (how Hills Easy was built; the glossary).
6. Each ticket in .scratch/hills-normal/issues/ before you start it.

Build tickets 01 to 07, respecting the blocking edges:
01 rules: SinkingFloat, Bat, CurrentZone, vertical MovingPlatform, level ambience override (no blockers)
02 art script make_hills_normal_art.py + preview (no blockers)
03 backdrops script make_hills_normal_backdrops.py + preview (no blockers)
04 audio script make_hills_normal_audio.py + preview (no blockers)
05 Hills Normal 1–5 data, 7 kinds, 5 themes (needs 01)
06 play parts (FloatWatcher, BatFlock, CurrentWatcher, balloons), motor, builders, themes, props (needs 01–05)
07 final check: run scripts and every builder, EditMode tests, CONTEXT.md words, .scratch/hills-normal/HANDOVER.md with the play checklist (needs all)

Rules:
- One script runs every level; level differences are data in LevelCatalog; no per-level or per-theme if-branches in play scripts (D-061).
- Rules stay plain C# with no Unity types and get NUnit EditMode tests. 296 tests now; report the new count.
- New level jobs are new parts with Begin(LevelController) and IsBuilt; moving things read LevelClock.Now; sounds go through LevelController.PlaySfx.
- Never reorder Episode, LevelTheme, badge ids or kind ids; add new values at the end.
- Mountains and Hills Easy must play exactly as before. Do not change existing art or audio files.
- Art: exact names and sizes from spec section 8, palette from make_mainmenu_art and the existing helpers, no anti-aliasing. Look at every preview PNG and fix what looks wrong before reporting.
- Every new code line, C# and Python, ends with a short basic-English comment (D-033).
- Use the numbers in layouts.md exactly. If one cannot work, stop and report it instead of changing it.
- Do not commit or push. Do not edit the Obsidian vault (D:\LandGrand\LandGrand-Sources).
- Anything not in the spec is a change: ask before adding it.
- End every ticket by naming the Unity builder command(s) the user must run, or "none".
- In the final handover list every file built, the test count, every number you changed and why, everything not checked in the Editor, and the three preview PNG paths.
