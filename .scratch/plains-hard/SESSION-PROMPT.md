# Next session: build Plains Hard 1–7 (D-071)

Paste everything below the line into a fresh Claude Code session in `D:\LandGrand\LandGrand`. Paste it again after each `/clear`; the prompt finds the next ticket by itself.

---

Build Plains Hard 1–7 for LandGrand (D-071). You are the builder this time; there is no Codex hand-over. The plan is finished and committed (`0c81d8e` game repo, `d8ab44b` vault). The art, backgrounds, sound and bot-checked layouts are already made. Your job is the C# code, the scenes (through the builders only), the tests and the handover.

## 1. Load the context (read in this order, before any edit)

1. `AGENTS.md`, then the vault notes it names: `Current Focus.md`, `Decisions/Decision Register.md` (D-061, D-066, D-068, D-069, D-070, D-071), `Questions/Open Questions.md` (Q-04, Q-13) in `D:\LandGrand\LandGrand-Sources\LandGrand\`.
2. `.scratch/plains-hard/BUILD-START.md`: the house rules, batch commands and ticket order.
3. `.scratch/plains-hard/spec.md`: decisions (section 2), code changes (4), object rules (5), content (6), assets, the theme table and props (8).
4. `.scratch/plains-hard/layouts.md`: the exact numbers for H1–H7. Copy them exactly.
5. `CONTEXT.md`, then the patterns you will copy:
   - `.scratch/plains-normal/HANDOVER.md`: the last build, and its "Review fixes" and "Changes not in the spec" sections, which show what review catches.
   - `.scratch/hills-hard/HANDOVER.md`: rolling logs, and mud as the ice rule.
   - In code: `Assets/Editor/LevelThemes.cs` (`ThemeArt`, and the `PlatformSprite` / `PlatformLandSound` / `PlatformLandSoundScript` pattern: an optional field whose null keeps the old art), `Assets/Editor/HardLevelObjects.cs` (roller and ice builds), `Assets/Editor/HillsLevelObjects.cs` (bat build), `Assets/Scripts/Play/BatFlock.cs`, `Assets/Editor/PlainsNormalProps.cs` and the shared `PlaceTrailProps`, `Assets/Scripts/Rules/LevelData.cs` (`ItemCatalog`, `LevelCatalog`: copy the Hills Hard and Plains Normal entries), and `Assets/Tests/EditMode/PlainsNormalLevelsTests.cs` / `HillsHardLevelsTests.cs`.
6. The ticket you are about to build, in `.scratch/plains-hard/issues/`.

## 2. Pick the ticket

- Tickets: `01` kinds and themes, `02` level data, `03` themes, hay, herds, frost, props and building every scene, `04` final check and handover. Each ticket says what blocks it.
- Read each ticket's **Status** line. Build the first ticket that is not `done` and whose blockers are `done`. If `.scratch/plains-hard/HANDOVER.md` exists, read it too.
- Build **one ticket per session**. When it is finished, set its Status to `done (<date>, <test count>)`, report, and stop. The user runs `/clear` and pastes this prompt again for the next ticket.

## 3. How to build a ticket

Use `/implement` on the ticket. It drives `/tdd` (one red-green slice at a time) and ends with `/code-review`.

- **Rules first (01, 02):** write the failing EditMode test, then the data or code that makes it pass.
- **Model split (memory):** you plan and check on Opus. Mechanical work may go to a Haiku subagent with a tight brief naming the files, the exact numbers, the D-033 comment rule and "no other edits": for example, typing the seven `LevelCatalog` entries from `layouts.md`, or the seven theme entries from the spec section 8 table. Always check its result by reading the content, never by its success message.
- **Unity batch runs need the Editor closed.** Before the first batch run of the session, ask the user once to close Unity, and wait for a yes. Run tests and builders in the background (each takes minutes), one at a time, with the commands in `BUILD-START.md`. Read the log; the result counts only if it shows no "Missing", no errors, and the test XML shows every test passing.
- **Ticket 03 is the biggest.** Build it in this order, testing as you go:
  1. the `ThemeArt` fields, with a test that every old theme keeps its defaults
  2. the roller and ice builds
  3. the bat build, and the `BatFlock` lift
  4. the seven theme entries
  5. the props
  6. the asset scripts (run each twice and compare the outputs: identical)
  7. the six builders
  8. the regression scene comparison against `HEAD`: Hills Hard logs, Hills Normal bats, Mountains Hard ice. Compare positions, sprites and timings, ignoring the order the scene file lists objects in.

## 4. House rules (from AGENTS.md, BUILD-START.md and the user's standing rules)

- **D-061:** level differences are data. No per-level, per-landform or per-theme `if` in play scripts.
- **Rules:** plain C# with no Unity types, with NUnit tests. Do **not** change the `Bat`, `RollingLog` or `IceZone` rules: hay, herds and frost are new looks on old rules. A herd is `new Bat(cx, 6f, 48f, 0f, period, phase)`.
- **Save safety:** append only to `LevelTheme`, kind ids and badge ids. There are no new badges.
- **Walking:** 84 px/s everywhere.
- **Old levels:** Mountains, Hills, Plains Easy and Plains Normal must play exactly as before.
- **Comments:** every new or changed code line ends with a short basic-English comment (D-033).
- **Assets:** never edit `Tools/Art`, `Tools/Audio` or any PNG, OGG or WAV; only run the scripts. If an asset is missing or wrong, stop and tell the user. The Steppe eagles reuse `Props/eagle_00/01`.
- **Scenes:** made only by the editor builders. Never hand-edit `.unity` files.
- **Layout numbers:** exactly as `layouts.md`. If a number cannot work, stop and ask; never change it silently. A goal offset may be lowered if the sprite has clear rows at its base (as with the Farm Shed (24, 22)), but say so.
- **Decisions are the user's:** you settle facts. Anything not in the spec is a change: ask first, and list it in `HANDOVER.md` under "Changes not in the spec".
- **The game follows the manuscript (D-036):** surface differences; do not settle them yourself.
- **Content:** all lesson, review and kind text is a draft (Q-04). Use the spec section 6 text as written.
- **Git:** do not commit or push unless the user asks for that commit. A commit message is one sentence, with no `Co-Authored-By` or AI line. Never commit `.scratch/*/bot/bin`, `.scratch/*/bot/obj` or `Assets/Resources/GDKEditionAutoGen*`.
- **Vault:** do not edit it during a ticket. After the user plays the checklist, record the result in D-071 and Current Focus, but only when the user says so.
- **Builder:** end every report by naming the Unity builder(s) the user should run, or "none".

## 5. Report at the end of each ticket

- the files added or changed (one line each, with `file:line` where useful)
- the tests added, and the pass count (440 before ticket 01)
- any number changed from `spec.md` or `layouts.md`, and why
- anything not checked in the Editor
- the Unity builder(s) to run, or "none"

## 6. After ticket 04 (the last session)

1. `HANDOVER.md` is complete: every file, the final test count, changed numbers, props moved more than 64 px, goal offsets, things not checked in the Editor, "Changes not in the spec", and the play checklist from ticket 04.
2. Run `/code-review` against `0c81d8e` (spec: `.scratch/plains-hard/spec.md`, `layouts.md`, `issues/01`–`04`; standards: `AGENTS.md`, `BUILD-START.md`; ignore `.unity` scenes and binary assets). Fix the findings, then re-run the tests and the builders.
3. Tell the user to play the checklist in `HANDOVER.md`. After "all passed", the user commits, and the vault gets the play result.

Start now: load the context in section 1, pick the ticket in section 2, and say in one line which ticket you are building and why it is next.
