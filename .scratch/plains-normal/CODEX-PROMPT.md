You are building Plains Normal 1–5 for LandGrand, a Unity 6000.6.9f1 2D pixel-art platformer for Grade 5 pupils in the Philippines. Repo: D:\LandGrand\LandGrand. 480x270 pixel-perfect camera. The ART, BACKGROUNDS, SOUND and bot-checked LAYOUTS are already made by Claude. Your job is the C# code only.

=== READ FIRST, IN THIS ORDER ===
1. AGENTS.md (repo root).
2. .scratch/plains-normal/CODEX-START.md — house rules, batch commands, ticket order.
3. .scratch/plains-normal/spec.md — decisions (section 2), outline (3), code changes (4), object rules (5), content (6), assets, theme table and props (8).
4. .scratch/plains-normal/layouts.md — the exact data for N1–N5 (bot-checked at walk 84; bot files .scratch/plains-normal/bot-check-*.cs.txt). Copy the numbers exactly.
5. .scratch/plains-easy/HANDOVER.md, .scratch/hills-normal/HANDOVER.md and CONTEXT.md — patterns to reuse: Hills Normal floats/pools/currents, Mountains Normal moving platforms and crumbling bridges, soft sand, TrailTheme / RepeatTrailProp / TrailPropFits.
6. Each ticket in .scratch/plains-normal/issues/ right before you start it.

=== WHAT YOU ARE BUILDING ===
- Five Normal levels (width 7200, start (40, 0), goal 7040, checkpoints 2400 and 4800, 180 s, 7 pickups with one secret at y >= 72):
  N1 Candaba Swamp (CandabaWetland, goal Bird Hide) — sinking floats; practice float over a safe pool; water pits.
  N2 Pinatubo Lahar Plain (LaharPlain, goal Lahar Marker) — soft sand and two crumbling bridges; dry pits.
  N3 Agno River Delta (RiverDelta, goal Fish Pen Hut) — four currents, first one pushes right; water pits.
  N4 Agusan Marsh (FloatingVillage, goal Floating House) — one float, one sand patch, one current; water pits.
  N5 Mekong Delta (FloatingMarket, goal Market Pier) — four market boats = horizontal MovingPlatform (48 px, 40 px/s, top 0); first boat over a safe pool; water pits.
- NO new rules. All objects reuse existing rules and numbers.
- Market boat look: new optional ThemeArt fields PlatformSprite (null = "platform") and PlatformLandSound (null = none). The platform art's top row is the platform top: centre y = Top − spriteHeight/2 (old 10 px platform stays at Top − 5, so Mountains is unchanged). MovingPlatformView plays the landing sound via PlaySfx at 0.6 once per landing. FloatingMarket uses market_boat + sfx_boat_bump.
- Kinds: append peat and laterite at the END of ItemCatalog.All (Plains only, IsDraft; peat stored as a rock, its card says it is not a true rock yet). Add Episode.Plains to pumice, dacite, andesite and gold. Plains shelf 10 -> 16; Collection.TotalCount 54 -> 56.
- Themes appended at the END of LevelTheme: CandabaWetland, LaharPlain, RiverDelta, FloatingVillage, FloatingMarket.
- Ambience: table entry amb_wetland_normal for all five (no override). No new badges (Philippine Plains and Delta Boater already exist).

=== TICKETS (follow blocking edges) ===
01 Kinds and themes (no blockers), with tests (shelf 16, total 56, ids appended).
02 Level data (needs 01): Plains Normal 1–5 exactly as layouts.md; water pits for N1, N3, N4, N5; text from spec section 6; PlainsNormalLevelsTests.cs; progression and badge tests.
03 Themes, boats, props, build (needs 01, 02): ThemeArt platform fields, platform placement and landing sound, five theme entries, living props via the trail prop helpers (report any moved > 64 px), missing-asset errors naming make_plains_normal_art.py / make_plains_normal_backdrops.py / make_plains_normal_audio.py. Run the three scripts (identical output) and all six builders (no "Missing", no errors). Check Mountains moving-platform scenes are unchanged.
04 Final check (needs all): scripts, builders, tests, CONTEXT.md word (Market boat), .scratch/plains-normal/HANDOVER.md with the play checklist from ticket 04.

=== HARD RULES ===
- D-061: level differences are data. No per-level, per-landform or per-theme if-branches in play scripts.
- Rules are plain C# with NO Unity types and NUnit EditMode tests. 423 tests pass now; report the new count.
- Timed things read LevelClock.Now; sounds via LevelController.PlaySfx.
- Never reorder Episode, LevelTheme, badge ids or kind ids; add at the end.
- Walk speed is 84 px/s everywhere.
- Mountains, Hills and Plains Easy must play exactly as before.
- Do NOT edit anything in Tools/Art or Tools/Audio or any PNG/OGG/WAV. Only run the scripts. If an asset is missing or looks wrong, stop and report.
- Use layouts.md numbers exactly. If one cannot work, stop and report; never change it silently. If a goal sprite has clear rows at its base, you may lower its offset (as with the Farm Shed (24, 22)) but report it.
- Every new code line ends with a short basic-English comment (D-033).
- Scenes are made only by the editor builders. Never hand-edit .unity files.
- Anything not in the spec is a change: write it down and ask.
- Do NOT commit or push. Do NOT edit the Obsidian vault (D:\LandGrand\LandGrand-Sources).
- Batch Unity needs the Editor closed. If Unity cannot run, still write the tests and say clearly they were not run.

=== AFTER EACH TICKET, REPORT ===
Files added/changed (one line each); tests added and the pass count (or "not run" and why); any number changed from spec.md or layouts.md and why; the Unity builder command(s) to run, or "none".

=== FINAL HANDOVER (ticket 04) ===
In .scratch/plains-normal/HANDOVER.md and your last message: every file built, final test count, every changed number with reason, every prop moved > 64 px, any goal offset changed, everything not checked in the Editor, and the user play checklist.
