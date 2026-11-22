You are building Plains Hard 1–7 for LandGrand, a Unity 6000.6.9f1 2D pixel-art platformer for Grade 5 pupils in the Philippines. Repo: D:\LandGrand\LandGrand. 480x270 pixel-perfect camera. The ART, BACKGROUNDS, SOUND and bot-checked LAYOUTS are already made by Claude. Your job is the C# code only.

=== READ FIRST, IN THIS ORDER ===
1. AGENTS.md (repo root).
2. .scratch/plains-hard/CODEX-START.md — house rules, batch commands, ticket order.
3. .scratch/plains-hard/spec.md — decisions (section 2), outline (3), code changes (4), object rules (5), content (6), assets, theme table and props (8).
4. .scratch/plains-hard/layouts.md — the exact data for H1–H7 (bot-checked at walk 84; bot files .scratch/plains-hard/bot-check-*.cs.txt). Copy the numbers exactly.
5. .scratch/plains-normal/HANDOVER.md, .scratch/hills-hard/HANDOVER.md and CONTEXT.md — patterns to reuse: rolling logs, Hills Normal bats (BatFlock), Mountains Hard ice, the Plains Normal optional theme fields PlatformSprite / PlatformLandSound / PlatformLandSoundScript, PlaceTrailProps / RepeatTrailProp / TrailPropFits.
6. Each ticket in .scratch/plains-hard/issues/ right before you start it.

=== WHAT YOU ARE BUILDING ===
- Seven Hard levels (width 9600, start (40, 0), goal 9440, checkpoints 2400, 4800 and 7200, 240 s, 9 pickups with one secret at y >= 72):
  H1 Great Plains (Prairie, goal Hay Barn) — rolling hay bales (first one practice, rolling toward the player) and 3 straw piles.
  H2 Serengeti (Savanna, goal Kopje) — wildebeest herds only (first one practice).
  H3 Pampas (Pampas, goal Estancia Gate) — hay bales and cattle herds.
  H4 West Siberian Plain (FrozenPlain, goal Winter Hut) — frozen ground (the ice rule) and two bog floats over water pits; ambience override amb_cold_plain.
  H5 Nullarbor Plain (LimestonePlain, goal Cave Entrance) — crumbling bridges and steps, and bats.
  H6 Indo-Gangetic Plain (GangesPlain, goal Stilt House) — floats over water pits and river currents; every pit is water.
  H7 Mongolian Steppe (Steppe, goal Ger) — the finale: hay, horse herds, frozen ground, crumbling bridges and currents.
- NO new rules. Three new LOOKS on old rules:
  Hay bale = RollingLog, unchanged numbers, with frames hay_00–03, warning hay_warn_00/01, sound sfx_hay_roll.
  Herd = Bat on the ground: new Bat(cx, 6f, 48f, 0f, period, phase), unchanged push, rest and touch box; frames wildebeest / cattle / horse (3 each, 32x16, facing left, feet on the bottom row), drawn 2 px above the rule's centre (BatArtLift 2), touch sound sfx_herd_hooves.
  Frozen ground = IceZone, unchanged numbers and sfx_ice_slide, with the top tile frost_top.
- The looks are theme DATA: new optional ThemeArt fields RollerFrames / RollerWarnFrames / RollerSound, BatFrames / BatFrameCount / BatSound / BatArtLift, IceTop, plus their script names for missing-asset messages. Null or 0 keeps the current art ("log", "log_warn", "sfx_log_roll", "bat", 3, "sfx_bat_flap", 0, "ice_top"), so Mountains, Hills and Plains Normal do not change.
- Kinds: append chalk, loess and flint at the END of ItemCatalog.All (Plains only, IsDraft, all stored as rocks; the loess card says it is not a true rock). Add Episode.Plains to granite, tuff, calcite, mica and feldspar. Plains shelf 16 -> 24; Collection.TotalCount 56 -> 59.
- Themes appended at the END of LevelTheme: Prairie, Savanna, Pampas, FrozenPlain, LimestonePlain, GangesPlain, Steppe.
- No new badges (Prairie Ranger, World Plains, Steady Plains and Plain Master already exist). After all 15 Plains levels, Plateaus opens the generic way.

=== TICKETS (follow blocking edges) ===
01 Kinds and themes (no blockers), with tests (shelf 24, total 59, ids appended).
02 Level data (needs 01): Plains Hard 1–7 exactly as layouts.md; water pits for H4 (two) and H6 (all); the H4 ambience override; text from spec section 6; PlainsHardLevelsTests.cs; progression and badge tests.
03 Themes, hay, herds, frost, props, build (needs 01, 02): the optional ThemeArt fields and their use in the roller, ice and bat builds and BatFlock (keep the lift when moving pictures), seven theme entries, living props via the shared trail prop helper (report any moved > 64 px), missing-asset errors naming make_plains_hard_art.py / make_plains_hard_backdrops.py / make_plains_hard_audio.py from data. Run the three scripts (identical output) and all six builders (no "Missing", no errors). Check the Hills Hard logs, Hills Normal bats and Mountains Hard ice scenes are unchanged.
04 Final check (needs all): scripts, builders, tests, CONTEXT.md words (Hay bale, Herd), .scratch/plains-hard/HANDOVER.md with the play checklist from ticket 04.

=== HARD RULES ===
- D-061: level differences are data. No per-level, per-landform or per-theme if-branches in play scripts.
- Rules are plain C# with NO Unity types and NUnit EditMode tests. Do not change the Bat, RollingLog or IceZone rules. 440 tests pass now; report the new count.
- Timed things read LevelClock.Now; sounds via LevelController.PlaySfx.
- Never reorder Episode, LevelTheme, badge ids or kind ids; add at the end.
- Walk speed is 84 px/s everywhere.
- Mountains, Hills, Plains Easy and Plains Normal must play exactly as before.
- Do NOT edit anything in Tools/Art or Tools/Audio or any PNG/OGG/WAV. Only run the scripts. If an asset is missing or looks wrong, stop and report.
- Use layouts.md numbers exactly. If one cannot work, stop and report; never change it silently. If a goal sprite has clear rows at its base, you may lower its offset (as with the Farm Shed (24, 22)) but report it.
- Every new code line ends with a short basic-English comment (D-033).
- Scenes are made only by the editor builders. Never hand-edit .unity files.
- Anything not in the spec is a change: write it down in HANDOVER.md under "Changes not in the spec" and ask.
- Do NOT commit or push. Do NOT edit the Obsidian vault (D:\LandGrand\LandGrand-Sources).
- Batch Unity needs the Editor closed. If Unity cannot run, still write the tests and say clearly they were not run.

=== AFTER EACH TICKET, REPORT ===
Files added/changed (one line each); tests added and the pass count (or "not run" and why); any number changed from spec.md or layouts.md and why; the Unity builder command(s) to run, or "none".

=== FINAL HANDOVER (ticket 04) ===
In .scratch/plains-hard/HANDOVER.md and your last message: every file built, final test count, every changed number with reason, every prop moved > 64 px, any goal offset changed, everything not checked in the Editor, "Changes not in the spec", and the user play checklist.
