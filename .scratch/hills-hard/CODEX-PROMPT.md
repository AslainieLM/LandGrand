You are building Hills Hard 1–7 for LandGrand, a Unity 6000.6.8f1 2D pixel-art platformer for Grade 5 pupils in the Philippines. Repo: D:\LandGrand\LandGrand. 480x270 pixel-perfect camera. The ART, BACKGROUNDS, SOUNDS and bot-checked LAYOUTS are already made by Claude. Your job is the C# code only.

=== READ FIRST, IN THIS ORDER ===
1. AGENTS.md (repo root).
2. .scratch/hills-hard/CODEX-START.md — house rules, batch commands, ticket order.
3. .scratch/hills-hard/spec.md — decisions (section 2), level outline and the k1–k9 pickup-kind table (section 3), code-change table (section 4), lessons/reviews/kinds/banners (section 7), asset list, theme table and living props (section 8).
4. .scratch/hills-hard/layouts.md — rule constants and the exact data for H1–H7 (bot-checked at walk 84; bot files .scratch/hills-hard/bot-check-*.cs.txt). Copy the numbers exactly.
5. .scratch/hills-normal/HANDOVER.md and CONTEXT.md — the patterns to reuse: FloatWatcher, BatFlock, CurrentWatcher, BalloonView, IceWatcher, the rolling-boulder part, HillsPropFits / NearestHillsPropSpot, LandformAudio, ZoneFooting.
6. Each ticket in .scratch/hills-hard/issues/ right before you start it.

=== WHAT YOU ARE BUILDING ===
- Seven Hard levels (9600 px, start x 40, 3 checkpoints, 240 s, 9 pickups with one secret at y ≥ 72):
  H1 Tarsier Forest (TarsierForest, goal Tarsier Lookout) — slippery mud only, first mud safe.
  H2 Antipolo Slopes (RainySlope, Barangay Hall) — rolling logs only, first log safe; calm landslide-safety lesson.
  H3 Masungi Georeserve (KarstRidge, Spider-Web Bridge) — mud, bats, crumbling ledges.
  H4 Pililla Wind Farm (WindHills, Turbine Base) — wind, logs, vertical balloons; ambience amb_wind_turbines.
  H5 Clew Bay (DrumlinBay, Stone Pier) — mud, current, logs.
  H6 Seven Hills of Rome (CityHills, Temple Ruins) — logs, horizontal moving platforms, floats; ambience amb_city_hills.
  H7 Guilin Karst (KarstTowers, Raft Dock) — everything (finale).
- SLIPPERY MUD = exactly the icy-ground rule (IceSlide, same numbers) inside MudZone stretches; only art (mud_top, mud_splash_00/01) and sound (sfx_mud_squelch) differ.
- ROLLING LOG = the RollingBoulder rule (3.5 s cycle, 0.8 s warning, 90 px/s, 14x14 box, 48 px skip) plus Direction (+1 right / −1 left); x = Spawn + Direction·elapsed·Speed; gone past End; run ≤ 243 px. Art log_00–03 (16x16), log_warn_00/01 (24x4); sound sfx_log_roll. RollingBoulder itself must NOT change.
- Six new Hills-only kinds (IsDraft), appended at the END of ItemCatalog.All: peridotite, chlorite, dacite, quartzite, travertine, leucite. Hills shelf 17 → 23. Review kinds (limestone, basalt, calcite, andesite, schist, tuff) are NOT added to the Hills shelf.
- Seven LevelTheme values at the END: TarsierForest, RainySlope, KarstRidge, WindHills, DrumlinBay, CityHills, KarstTowers.
- No new badges. Hills becomes complete after H7, so the existing generic flow opens the Hills Description Screen and unlocks Plains (one-time map animation, then "Coming soon").

=== TICKETS (follow blocking edges) ===
01 Rules (no blockers): Rules/MudZone.cs, Rules/RollingLog.cs, optional MudZones and Logs at the END of the LevelLayout constructor (existing callers unchanged), LandformAudio script names amb_wind_turbines / amb_city_hills -> make_hills_hard_audio.py. Tests: mud == ice, logs both directions incl. skip, Mountains boulders unchanged.
02 Level data (needs 01): 7 themes, 6 kinds, Hills Hard 1–7 exactly as layouts.md with the k1–k9 kinds from spec section 3, lessons/reviews/banners from spec section 7, AmbienceOverride on H4 and H6. HillsHardLevelsTests.cs (same checks as HillsNormalLevelsTests). Update progression tests: Normal 5 -> Hard 1; Hard 7 -> Description Screen; Hills complete -> Plains unlocked; Plains still "Coming soon".
03 Play parts and builders (needs 01, 02): mud turns on the motor's ice slide (extend IceWatcher or a MudWatcher part; ice must stay identical) with squelch + splash; LogRoller part like the rolling-boulder part (warning, roll in its direction, sfx_log_roll via PlaySfx, touch = boulder trap). Builders: 7 theme entries (spec section 8 table, turbine_base offset (24, 32)), mud_top swapped over mud zones, logs, reuse water_fill/current_top, living props through HillsPropFits (also refuse mud and log runs; report any moved > 64 px), ambience via LandformAudio.AmbienceFor(level), missing-asset errors naming make_hills_hard_art.py / make_hills_hard_backdrops.py / make_hills_hard_audio.py. Run the three scripts (they must reproduce identical files) and every builder; log must have no "Missing" and no errors.
04 Final check (needs all): scripts, all six builders, EditMode tests, CONTEXT.md words (Slippery mud, Rolling log, Hills complete), .scratch/hills-hard/HANDOVER.md with the play checklist from ticket 04.

=== HARD RULES ===
- D-061: level differences are data in LevelCatalog. No per-level, per-landform or per-theme if-branches in play scripts.
- Rules are plain C# with NO Unity types and NUnit EditMode tests. 360 tests pass now; report the new count.
- New jobs are level parts with Begin(LevelController) and IsBuilt; moving things read LevelClock.Now; sounds via LevelController.PlaySfx.
- Never reorder Episode, LevelTheme, badge ids or kind ids; add at the end.
- Walk speed is 84 px/s everywhere. Do NOT add a per-level or ground speed.
- Mountains, Hills Easy and Hills Normal must play exactly as before. Ice and RollingBoulder must be unchanged (tests).
- Do NOT edit anything in Tools/Art or Tools/Audio or any PNG/OGG/WAV. Only run the scripts. If an asset is missing or looks wrong, stop and report.
- Use layouts.md numbers exactly. If one cannot work, stop and report; never change it silently.
- Every new code line ends with a short basic-English comment (D-033).
- Scenes are made only by the editor builders. Never hand-edit .unity files.
- Anything not in the spec is a change: write it down and ask.
- Do NOT commit or push. Do NOT edit the Obsidian vault (D:\LandGrand\LandGrand-Sources).
- Batch Unity needs the Editor closed (commands in CODEX-START.md). If Unity cannot run, still write the tests and say clearly they were not run.

=== AFTER EACH TICKET, REPORT ===
Files added/changed (one line each); tests added and the pass count (or "not run" and why); any number changed from spec.md or layouts.md and why; the Unity builder command(s) to run, or "none".

=== FINAL HANDOVER (ticket 04) ===
In .scratch/hills-hard/HANDOVER.md and your last message: every file built, final test count, every changed number with reason, every prop moved > 64 px, everything not checked in the Editor, and the user play checklist.
