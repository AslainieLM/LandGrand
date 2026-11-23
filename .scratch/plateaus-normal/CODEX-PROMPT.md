You are building Plateaus Normal 1–5 for LandGrand, a Unity 6000.6.9f1 2D pixel-art platformer for Grade 5 pupils in the Philippines. Repo: D:\LandGrand\LandGrand. 480x270 pixel-perfect camera. The ART, BACKGROUNDS, SOUND and bot-checked LAYOUTS are already made by Claude. Your job is the C# code only.

=== READ FIRST, IN THIS ORDER ===
1. AGENTS.md (repo root).
2. .scratch/plateaus-normal/CODEX-START.md — house rules, batch commands, ticket order.
3. .scratch/plateaus-normal/spec.md — decisions (section 2), levels (3), code changes (4), objects (5), content (6), banners (7), assets, theme table and props (8).
4. .scratch/plateaus-normal/layouts.md — the exact data for N1–N5 (bot-checked at walk 84; bot files .scratch/plateaus-normal/bot-check-*.txt). Copy the numbers exactly.
5. .scratch/plateaus-easy/HANDOVER.md, .scratch/plains-normal/HANDOVER.md, .scratch/plains-hard/HANDOVER.md and CONTEXT.md — patterns to reuse: market boats (PlatformSprite / PlatformLandSound), herds and frost (BatFrames, BatSound, IceTop), updrafts, crumbling bridges, soft sand, currents, floats, trail prop helpers.
6. Each ticket in .scratch/plateaus-normal/issues/ right before you start it.

=== WHAT YOU ARE BUILDING ===
- Five Normal levels (width 7200, start (40, 0), goal 7040, checkpoints 2400 and 4800, 180 s, 7 pickups with one secret at y >= 72):
  N1 Table Mountain (TableTop, goal Cable Station) — four cable cars = horizontal MovingPlatform (48 px, 40 px/s, top 0); the first over a safe pool; dry pits.
  N2 Monument Valley (MesaValley, goal Trading Post) — soft sand, two crumbling bridges, two updrafts; dry pits.
  N3 Grand Canyon (CanyonRim, goal Canyon Lookout) — four currents, the first pushes right; water pits.
  N4 Mount Roraima (TepuiTop, goal Explorer Camp) — two floats in water pits, two updrafts; the secret is on the high updraft ledge.
  N5 Salar de Uyuni (SaltFlat, goal Salt Hotel) — three salt-crust stretches (the ice rule) and three llama herds (the herd rule).
- NO new rules. Every object reuses an existing rule and its numbers.
- Cable car look: two new optional ThemeArt fields. PlatformArtAbove (default 0): centre y = Top − spriteHeight/2 + PlatformArtAbove (cable_car is 48x48 with its roof at row 24, so 24; every old platform stays the same). PlatformCable (default null): a tileable strip (cable_line, 16x3) from X0 − 16 to X1 + 64 with its middle at Top + PlatformArtAbove − 1, behind the car. Landing sound sfx_cable_car.wav via the existing PlatformLandSound.
- Llama and salt look: SaltFlat sets BatFrames "llama" (3 frames, facing right: BatArtFacesLeft false, BatArtLift 2), BatSound "sfx_llama.wav", IceTop "salt_crust_top", with their art and audio scripts.
- Kinds: append ulexite at the END of ItemCatalog.All (Plateaus only, mineral, IsDraft). Add Episode.Plateaus to sandstone, quartzite, granite, shale, siltstone, conglomerate, limestone, schist, quartz, gneiss, halite, gypsum and ignimbrite. Plateaus shelf 10 -> 24; Collection.TotalCount 63 -> 64.
- Themes appended at the END of LevelTheme: TableTop, MesaValley, CanyonRim, TepuiTop, SaltFlat.
- Ambience: the existing Plateaus Normal entry amb_canyon_normal for all five. No new badges (Table Top and Canyon Explorer already exist).

=== TICKETS (follow blocking edges) ===
01 Kinds and themes (no blockers), with tests (shelf 24, total 64, ids appended).
02 Level data (needs 01): Plateaus Normal 1–5 exactly as layouts.md; water pits for N3 and N4 and the N1 safe pool; text from spec section 6; PlateausNormalLevelsTests.cs; progression and badge tests.
03 Themes, cable cars, llamas, salt crust, props, build (needs 01, 02): ThemeArt fields, platform placement, cable line and landing sound, five theme entries, living props via the trail prop helpers (report any moved > 64 px), missing-asset errors naming make_plateaus_normal_art.py / make_plateaus_normal_backdrops.py / make_plateaus_normal_audio.py. Run the three scripts (identical output) and all six builders (no "Missing", no errors). Check old platforms, herds, frost and updrafts are unchanged.
04 Final check (needs all): scripts, builders, tests, CONTEXT.md words (Cable car, Llama herd), .scratch/plateaus-normal/HANDOVER.md with the play checklist from ticket 04.

=== HARD RULES ===
- D-061: level differences are data. No per-level, per-landform or per-theme if-branches in play scripts.
- Rules are plain C# with NO Unity types and NUnit EditMode tests. 524 tests pass now; report the new count.
- Timed things read LevelClock.Now; sounds via LevelController.PlaySfx.
- Never reorder Episode, LevelTheme, badge ids or kind ids; add at the end.
- Walk speed is 84 px/s everywhere.
- Mountains, Hills, Plains and Plateaus Easy must play exactly as before.
- Do NOT edit anything in Tools/Art or Tools/Audio or any PNG/OGG/WAV. Only run the scripts. If an asset is missing or looks wrong, stop and report.
- Use layouts.md numbers exactly. If one cannot work, stop and report; never change it silently. If a goal sprite has clear rows at its base, you may lower its offset but report it.
- Every new code line ends with a short basic-English comment (D-033).
- Scenes are made only by the editor builders. Never hand-edit .unity files.
- Anything not in the spec is a change: write it down and ask.
- Do NOT commit or push. Do NOT edit the Obsidian vault (D:\LandGrand\LandGrand-Sources).
- Batch Unity needs the Editor closed. If Unity cannot run, still write the tests and say clearly they were not run.

=== AFTER EACH TICKET, REPORT ===
Files added/changed (one line each); tests added and the pass count (or "not run" and why); any number changed from spec.md or layouts.md and why; the Unity builder command(s) to run, or "none".

=== FINAL HANDOVER (ticket 04) ===
In .scratch/plateaus-normal/HANDOVER.md and your last message: every file built, final test count, every changed number with reason, every prop moved > 64 px, any goal offset changed, everything not checked in the Editor, and the user play checklist.
