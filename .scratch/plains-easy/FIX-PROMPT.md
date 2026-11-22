You built Plains Easy 1–3 for LandGrand (repo D:\LandGrand\LandGrand, Unity 6000.6.9f1). Claude reviewed your uncommitted work against commit dc4f2a9. No hard violations were found and the 417 EditMode tests pass. Please fix the small findings below. Keep every house rule in .scratch/plains-easy/CODEX-START.md: no behaviour changes for Mountains or Hills, numbers from layouts.md, a short basic-English comment on every new or changed code line (D-033), no edits to Tools/Art, Tools/Audio or any asset, do not hand-edit .unity files, do not commit or push, and do not edit the vault.

=== 0. ASK THE USER FIRST ===
Your HANDOVER says the Farm Shed goal offset in LevelThemes.cs:206 is (24, 22), not the spec's (24, 24), "with user approval". Claude cannot confirm that approval. Before you change anything else, ask the user:
"Keep the Farm Shed goal at offset (24, 22), or use the spec's (24, 24)?"
Then do what they answer. Write the answer and the reason in HANDOVER.md under "Changed numbers".

=== 1. FIX WRONG COMMENTS ===
a) LevelThemes.cs ~483, RepeatPlainsProp: the comment says "Corn sways at 1.2, 1.5 or 1.8 frames per second", but the helper places every repeated Plains prop (rice tufts, salt mounds, palms, corn). Also, the variable `seconds` holds 1/fps. Rename it so the name matches what it holds (for example `secondsPerFrame`). Rewrite the comment to say what the helper really does. Still props with one frame must not get an animation component, or must get one that is clearly harmless; say which in the comment.
b) StrawBounceWatcher.cs:58: `PlaySfx(bounceSound, 0.7f); // Uses the player's sound effects volume` is wrong, because 0.7 is this sound's own fixed volume. Make 0.7 a named constant (for example `const float BounceVolume = 0.7f;` with a comment "How loud the bounce plays (spec section 5)"), and fix the comment the way SandWatcher.cs:65 words its own sound.

=== 2. REMOVE THE COPIED PROP HELPERS (no visible change) ===
PlainsPropFrames, RepeatPlainsProp and PlainsTheme (LevelThemes.cs ~475 onward) copy HillsHardPropFrames, RepeatHillsHardProp and HillsHardTheme. Only the art script name differs.
- Make one shared helper for each pair. Each shared helper takes the art script path (for the missing-asset error) as a parameter. Make both the Hills Hard and the Plains callers use them.
- Hills Hard scenes must build exactly as before: the same props, positions, frames and timings.
- Rebuild with LevelBuilder.BuildAll. Then compare the Hills Hard scenes with git diff. The only differences allowed are serialisation noise (for example fileIDs); report anything else.

=== 3. HONEST NAMES ===
FindAndPlaceTrailProp now serves Hills and Plains, but it still calls NearestHillsPropSpot and HillsPropFits. Rename those two to landform-neutral names, such as NearestPropSpot and PropFits, or TrailPropSpot and TrailPropFits if those names are taken. Update every caller. Behaviour must not change.

=== 4. RESTORE THE LOST SHELF COMMENTS ===
LevelData.cs ~155 and ~168–176: the old comments on the six shared kinds (quartz, sandstone, clay, conglomerate, shale, limestone) were replaced by one generic "Adds Plains to the shared shelves" line. For each kind, put back its old comment from commit dc4f2a9 (`git show dc4f2a9:Assets/Scripts/Rules/LevelData.cs`), and add "and Plains" where it now applies. For example: "A mineral, taught in Mountains and shared with Hills and Plains". Do not change any game text.

=== 5. CONTEXT.md WORDING ===
CONTEXT.md:47 says "Its side acts as a solid wall." Ticket 04 asked for: "a low pile of rice straw; landing on its top bounces the explorer high; walking into its side does nothing". Use the ticket wording. A short, clear sentence in the glossary style is fine, as long as it says that walking into the side does nothing.

=== 6. ADD MISSING TESTS ===
a) Badges earned (AchievementTests or a new Plains test):
   - clearing Plains Easy 2 earns salt_maker
   - clearing Plains Easy 3 earns straw_jumper
   - 3 stars on all three Plains Easy levels earns flat_star
   - finding the 10 Plains-shelf kinds earns plain_collector
   - on a fresh profile all 10 Plains badges are locked
   - the Mountains and Hills badges are not earned by Plains progress
b) The PlainsEasyLevelsTests.cs:45 check `pile.X == 600f ? 80f : 60f` hard-codes the practice pile. Name it: for example, take the first pile by x as the practice pile and use named constants `PracticePileSafeDistance = 80f` and `PileSafeDistance = 60f`, each with a comment that cites spec section 5.

=== 7. SMALL CONSISTENCY ===
LevelLayout.cs:80 uses System.Array.Empty for the straw piles, while the lines around it use `new X[0]`. Use the same style as its neighbours.

=== NOT IN THIS FIX ===
- Do not refactor the 20-parameter LevelLayout constructor (a known item; spec section 9 keeps it out of scope).
- Keep the six Plains Normal and Hard badges (the spec asks for all 10 now).
- Leave the FlyingBird refactor as it is, but check in the Editor or through the built scene that the Hills map and menu birds still use the same frames and flight. Write the result in HANDOVER.md.

=== RUN AND REPORT ===
1. Close-Editor batch runs (commands in CODEX-START.md): EditMode tests, then every builder one at a time: LevelBuilder.BuildAll, LessonBuilder.Build, SelectionBuilder.Build, LandformMapBuilder.Build, LandformDescriptionBuilder.Build, ProfileBuilder.Build. Each log must show no "Missing" and no errors.
2. Update .scratch/plains-easy/HANDOVER.md: a "Review fixes" section that lists each item 0–7 with the file:line changed, the new test count (417 before), and the Hills Hard scene check from item 2.
3. In your last message: the files changed (one line each), the test count, the Farm Shed answer, any number changed and why, and the Unity builder command(s) the user should run, or "none".
