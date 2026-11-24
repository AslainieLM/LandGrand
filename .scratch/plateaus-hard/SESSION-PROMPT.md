# Next session: build Plateaus Hard 1–7 and the Completion Screen (D-074)

Paste everything below the line into a fresh Claude Code session in `D:\LandGrand\LandGrand`. Paste it again after each `/clear`; the prompt finds the next ticket by itself.

---

Build Plateaus Hard 1–7 and the Completion Screen for LandGrand (D-074), the last episode. You are the builder; there is no Codex hand-over. The plan is finished. The art, backgrounds, sounds and bot-checked layouts are already made. Your job is the C# code, the scenes (through the builders only), the tests and the handover.

## 1. Load the context (read in this order, before any edit)

1. `AGENTS.md`, then the vault notes it names: `Current Focus.md`, `Decisions/Decision Register.md` (D-036, D-061, D-065, D-071, D-072, D-073, D-074), `Questions/Open Questions.md` (Q-04, Q-13) in `D:\LandGrand\LandGrand-Sources\LandGrand\`.
2. `.scratch/plateaus-hard/BUILD-START.md`: the house rules, batch commands and ticket order.
3. `.scratch/plateaus-hard/spec.md`: decisions (section 2), code changes (4), objects (5), content (6), the Completion Screen (7), assets, the theme table and props (8).
4. `.scratch/plateaus-hard/layouts.md`: the exact numbers for H1–H7. Copy them exactly.
5. `CONTEXT.md`, then the patterns you will copy:
   - `.scratch/plateaus-normal/HANDOVER.md` and `.scratch/plains-hard/HANDOVER.md`: the last builds, with their review fixes and "Changes not in the spec".
   - In code: `Assets/Editor/LevelThemes.cs` (`ThemeArt` optional fields and the "old themes keep defaults" test), the vent build in `Assets/Editor/LevelBuilder.cs`, the gust build in `Assets/Editor/LevelObjects.cs`, the strips in `Assets/Editor/HardLevelObjects.cs`, `Assets/Scripts/Rules/LevelData.cs` (`ItemCatalog`, `LevelCatalog`, `PlainsHard(...)`, `PlateausNormal(...)`), `Assets/Scripts/Common/LandformDescriptionController.cs`, `Assets/Scripts/LandformMap/LandformMapController.cs`, and `Assets/Tests/EditMode/PlainsHardLevelsTests.cs`.
6. The ticket you are about to build, in `.scratch/plateaus-hard/issues/`.

## 2. Pick the ticket

- Tickets: `01` kinds, themes and the completion rules, `02` level data, `03` theme fields, themes, props and building the level scenes, `04` the Completion Screen, the last Description Screen and the map flag, `05` final check and handover. Each ticket says what blocks it.
- Read each ticket's **Status** line. Build the first ticket that is not `done` and whose blockers are `done`. If `.scratch/plateaus-hard/HANDOVER.md` exists, read it too.
- Build **one ticket per session**. When it is finished, set its Status to `done (<date>, <test count>)`, report, and stop. The user runs `/clear` and pastes this prompt again for the next ticket.

## 3. How to build a ticket

Use `/tdd` (one red-green slice at a time) and end with `/code-review` on the ticket's changes.

- **Rules first (01, 02, 04):** write the failing EditMode test, then the data or code that makes it pass.
- **Model split (memory):** you plan and check on Opus. Mechanical work may go to a Haiku subagent with a tight brief naming the files, the exact numbers, the D-033 comment rule and "no other edits": for example, typing the seven `LevelCatalog` entries from `layouts.md`, or the seven theme entries from the spec section 8 table. Always check its result by reading the content, never by its success message.
- **Unity batch runs need the Editor closed.** Before the first batch run of the session, ask the user once to close Unity, and wait for a yes. Run tests and builders in the background (each takes minutes), one at a time, with the commands in `BUILD-START.md`. Read the log; the result counts only if it shows no "Missing", no errors, and the test XML shows every test passing.
- **Ticket 03** in this order, testing as you go: the `ThemeArt` fields with the defaults test; the vent, gust and sand builds; the seven theme entries; the props; the asset scripts (run each twice, identical output); the builders; the regression scene comparison against `HEAD`.
- **Ticket 04:** keep the route choice in a plain C# rule with tests; the controller only asks it.

## 4. House rules (from AGENTS.md, BUILD-START.md and the user's standing rules)

- **D-061:** level differences are data. No per-level, per-landform or per-theme `if` in play scripts.
- **Rules:** plain C# with no Unity types, with NUnit tests. Do **not** change the vent, wind, sand, ice, herd, updraft, crumble or platform rules: every Plateaus Hard object is a new look on an old rule. A herd is `new Bat(cx, 6f, 48f, 0f, period, phase)`. Gusts push left only.
- **Save safety:** append only to `LevelTheme` and kind ids. There are no new badges. The save file does not change; "Completion seen" is a device setting.
- **Walking:** 84 px/s everywhere.
- **Old levels:** Mountains, Hills, Plains, Plateaus Easy and Plateaus Normal must play exactly as before.
- **Comments:** every new or changed code line ends with a short basic-English comment (D-033).
- **Assets:** never edit `Tools/Art`, `Tools/Audio` or any PNG, OGG or WAV; only run the scripts. If an asset is missing or wrong, stop and tell the user.
- **Scenes:** made only by the editor builders. Never hand-edit `.unity` files.
- **Layout numbers:** exactly as `layouts.md`. If a number cannot work, stop and ask; never change it silently. A goal offset may be lowered if the sprite has clear rows at its base, but say so.
- **Decisions are the user's:** you settle facts. Anything not in the spec is a change: ask first, and list it in `HANDOVER.md` under "Changes not in the spec".
- **The game follows the manuscript (D-036):** P069 and Figure 4E name the Completion Screen; surface differences, do not settle them yourself.
- **Content:** all lesson, review, kind and screen text is a draft (Q-04). Use the spec text as written.
- **Git:** do not commit or push unless the user asks for that commit. A commit message is one sentence, with no `Co-Authored-By` or AI line. Never commit `.scratch/*/bot/bin`, `.scratch/*/bot/obj`, `__pycache__` or `Assets/Resources/GDKEditionAutoGen*`.
- **Vault:** do not edit it during a ticket. After the user plays the checklist, record the result in D-074 and Current Focus, but only when the user says so.
- **Builder:** end every report by naming the Unity builder(s) the user should run, or "none".

## 5. Report at the end of each ticket

- the files added or changed (one line each, with `file:line` where useful)
- the tests added, and the pass count (563 before ticket 01)
- any number changed from `spec.md` or `layouts.md`, and why
- anything not checked in the Editor
- the Unity builder(s) to run, or "none"

## 6. After ticket 05 (the last session)

1. `HANDOVER.md` is complete: every file, the final test count, changed numbers, props moved more than 64 px, goal offsets, things not checked in the Editor, "Changes not in the spec", and the play checklist from ticket 05.
2. Run `/code-review` against the commit that holds this plan (spec: `.scratch/plateaus-hard/spec.md`, `layouts.md`, `issues/01`–`05`; standards: `AGENTS.md`, `BUILD-START.md`; ignore `.unity` scenes and binary assets). Fix the findings, then re-run the tests and the builders.
3. Tell the user to play the checklist in `HANDOVER.md`. After "all passed", the user commits, and the vault gets the play result.

Start now: load the context in section 1, pick the ticket in section 2, and say in one line which ticket you are building and why it is next.
