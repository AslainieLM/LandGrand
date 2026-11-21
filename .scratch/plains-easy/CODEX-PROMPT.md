You are building Plains Easy 1–3 for LandGrand, a Unity 6000.6.9f1 2D pixel-art platformer for Grade 5 pupils in the Philippines. Repo: D:\LandGrand\LandGrand. 480x270 pixel-perfect camera. The ART, BACKGROUNDS, BADGES, SOUNDS and bot-checked LAYOUTS are already made by Claude. Your job is the C# code only.

=== READ FIRST, IN THIS ORDER ===
1. AGENTS.md (repo root).
2. .scratch/plains-easy/CODEX-START.md — house rules, batch commands, ticket order.
3. .scratch/plains-easy/spec.md — decisions (section 2), episode outline and the 10 Plains badges (section 3), code-change table (section 4), straw pile rules (section 5), level data (section 6), lessons/reviews/kinds/banners (section 7), assets, theme table, props, map and menu life, audio table (section 8).
4. .scratch/plains-easy/layouts.md — rule constants, the exact data for E1–E3 (bot-checked at walk 84; bot files .scratch/plains-easy/bot-check-*.cs.txt) and the Plains map life spots. Copy the numbers exactly.
5. .scratch/hills/spec.md, .scratch/hills-hard/HANDOVER.md and CONTEXT.md — the patterns to reuse: Hills Easy level data, SandWatcher, HillsPropFits / NearestHillsPropSpot, HillsMenuLife, HillsLife, LandformLife, LandformAudio, LandformSummary, the Hills badges, ExplorerMotor.Launch (the steam vent launch).
6. Each ticket in .scratch/plains-easy/issues/ right before you start it.

=== WHAT YOU ARE BUILDING ===
- Three Easy levels (start (40, 0), 1 checkpoint, 120 s, 5 pickups with one secret):
  E1 Central Luzon Plain (RicePlain, goal Rice Granary) — what a plain is; no new object.
  E2 Pangasinan Salt Beds (SaltBeds, goal Salt Hut) — plains by the sea, salt from seawater; no new object.
  E3 Isabela River Plain (RiverPlain, goal Farm Shed) — rivers spread soil; NEW straw bounce pile.
- STRAW BOUNCE PILE: 48x16 solid like a small boulder (box x+1..x+47, 0..15). Landing on its top (grounded, feet at 15 ±0.5, body overlapping across) calls ExplorerMotor.Launch(400) (about 89 px rise; double jump ready again). Walking into its side does nothing. Squash sprite 0.15 s, straw bits 2 frames x 0.1 s, sfx_straw_bounce at volume 0.7 via PlaySfx, timing on LevelClock.Now. E3 piles at 600, 1200, 3300, 4500.
- Four new Plains-only kinds (IsDraft), appended at the END of ItemCatalog.All: siltstone, halite, gypsum, mudstone. Add Episode.Plains to clay, sandstone, shale, quartz, conglomerate, limestone. Plains shelf = 10 kinds; Collection.TotalCount 50 -> 54.
- Three LevelTheme values at the END: RicePlain, SaltBeds, RiverPlain. StrawPile[] strawPiles = null at the END of the LevelLayout constructor.
- LandformAudio Plains row: bgm_plains; amb_grass_easy / amb_wetland_normal / amb_dry_hard; script make_plains_audio.py.
- LandformSummary Plains: 5 draft lines, bg_plains. Plateaus stays "Coming soon".
- Ten Plains badges appended after hill_master (salt_maker, straw_jumper, flat_star, plain_collector, philippine_plains, delta_boater, prairie_ranger, world_plains, steady_plains, plain_master), page Episode.Plains, same helper rules as the Hills badges.
- Map: PlainsLife() (rice, egrets, carabao) only while Plains is open. Menus: PlainsMenuLife() (cloud, two egrets, six grass tufts).

=== TICKETS (follow blocking edges) ===
01 Rules (no blockers): StrawPile.cs, the layout list, themes, kinds, audio row, summary, badges, tests; update tests that say Plains is empty.
02 Level data (needs 01): Plains Easy 1–3 exactly as layouts.md, content from spec section 7, PlainsEasyLevelsTests.cs, progression tests (Hills Hard 7 -> Plains Easy 1; Plains Normal "Coming soon"; Next hidden after E3).
03 Play part and builders (needs 01, 02): StrawBounceWatcher level part; straw pile solids and sprites; 3 theme entries (spec section 8 table); living props through the prop-fit test (also refuse straw piles; report any moved > 64 px); PlainsMenuLife; PlainsLife; LandformLife loads Plains/ sprites; missing-asset errors naming make_plains_easy_art.py / make_plains_easy_backdrops.py / make_plains_audio.py. Run the four asset scripts (they must reproduce identical files) and every builder; logs must have no "Missing" and no errors.
04 Final check (needs all): scripts, all six builders, EditMode tests, CONTEXT.md word (Straw bounce pile), .scratch/plains-easy/HANDOVER.md with the play checklist from ticket 04.

=== HARD RULES ===
- D-061: level differences are data in LevelCatalog. No per-level, per-landform or per-theme if-branches in play scripts.
- Rules are plain C# with NO Unity types and NUnit EditMode tests. The Hills Hard handover reports 395 tests; report the new count.
- New jobs are level parts with Begin(LevelController) and IsBuilt; timed things read LevelClock.Now; sounds via LevelController.PlaySfx.
- Never reorder Episode, LevelTheme, badge ids or kind ids; add at the end.
- Walk speed is 84 px/s everywhere. Do NOT add a per-level or ground speed.
- Mountains and Hills must play exactly as before. Boulders, vents, sand, ice, mud and logs must be unchanged (tests).
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
In .scratch/plains-easy/HANDOVER.md and your last message: every file built, final test count, every changed number with reason, every prop moved > 64 px, everything not checked in the Editor, and the user play checklist.
