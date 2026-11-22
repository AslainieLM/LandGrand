You built Plains Normal 1–5 for LandGrand (repo D:\LandGrand\LandGrand, Unity 6000.6.9f1). Claude reviewed your uncommitted work against commit ed2cf05. No hard violations were found, the level data matches layouts.md, and 435 EditMode tests pass. Please fix the small findings below. Keep every house rule in .scratch/plains-normal/CODEX-START.md:
- no behaviour change for Mountains, Hills or Plains Easy
- numbers from layouts.md
- a short basic-English comment on every new or changed code line (D-033)
- no edits to Tools/Art, Tools/Audio or any asset
- no hand-edited .unity files
- no commit or push
- no vault edits

=== 1. BOAT THUNK AFTER A FREEZE (behaviour bug) ===
In Assets/Scripts/Play/MovingPlatformView.cs, the landing check (`riding && !wasRiding`) does not update `wasRiding` while the explorer is Frozen. After a respawn, resume or banner freeze while standing on a boat, the bump can play again with no new landing.
- Fix it so the sound plays only on a real landing. For example, keep `wasRiding` in step every frame, even while frozen or paused, or reset it on respawn so a respawn never counts as a landing.
- Add a short comment saying why.
- If you can test the rule without Unity types, add an EditMode test: move the "did the explorer just land?" edge check into a tiny plain C# helper in Rules, and test landing, staying, leaving, and a frozen stretch.

=== 2. ONE HELPER INSTEAD OF A COPY ===
PlacePlainsNormalStills (Assets/Editor/PlainsNormalProps.cs:41) is a near-copy of PlaceHillsHardProps (LevelThemes.cs ~538). Make one shared helper that takes the art script path and the clearFullZones flag. Both callers should use it.
- Hills Hard and Plains Normal scenes must come out the same.
- After rebuilding, compare their prop positions, sprites and frame timings with your current build, ignoring the order objects are listed in the scene file. Report the result.

=== 3. THE SOUND'S REPAIR MESSAGE COMES FROM DATA ===
LevelObjects.cs:41 always names "Tools/Audio/make_plains_normal_audio.py" for the theme's landing sound. Add the script path to ThemeArt next to PlatformLandSound, for example `PlatformLandSoundScript`. Set it in the FloatingMarket entry, and use it in the missing-file message. Fix the comment so it no longer says "boat bump" in shared code.

=== 4. SMALL CLEAN-UPS ===
- MovingPlatformView.cs:24: remove the self-assignment `level = owner` (or whichever line assigns the field to itself), keeping the behaviour.
- `clearFullZones` is passed through five functions. Leave the behaviour as it is. If a small named option type or a single overload makes it clearer without touching Hills behaviour, do it; otherwise, add one comment at RepeatTrailProp that explains what the flag does and why only Plains Normal uses it.

=== 5. MISSING TESTS ===
a) Ticket 01 asked that "no other kind, theme or badge id moved". Add a badge-order test that asserts the full ordered id list of AchievementCatalog.All (35 ids) exactly as it is now.
b) Ticket 02 asked for a regression check on old data. Add one test that pins key facts of earlier levels:
   - Mountains Normal 1: width, goal and moving-platform routes
   - Hills Normal 1: floats and pool
   - Plains Easy 3: straw piles
   - and that no Mountains, Hills or Plains Easy level uses a theme with a custom PlatformSprite, if this is testable from Rules; otherwise skip that last part and say so

=== 6. REPORT THE CHANGES YOU MADE ON YOUR OWN ===
In HANDOVER.md, add a "Changes not in the spec" section that lists and explains:
- the MovingPlatformView IsBuilt guard, which turns a platform off when its picture or sound link is missing
- clearFullZones, and why it moves reeds and hyacinths 107–148 px
- the narrowed AchievementTests.cs:112 test
- the Bird Hide offset (24, 23)
- the checklist line change: after Plains Easy 3, Plains Hard asks you to clear Normal first, and "Coming soon" shows after N5

=== RUN AND REPORT ===
1. Close the Editor, then batch-run the EditMode tests and all six builders one at a time (commands in CODEX-START.md). Each log must show no "Missing" and no errors. Save the logs under Logs/PlainsNormal-review/.
2. Update .scratch/plains-normal/HANDOVER.md with a "Review fixes" section that lists items 1–6 with file:line, the new test count (435 before), and the scene check from item 2.
3. In your last message, give: the files changed (one line each), the test count, any number changed and why, and the Unity builder command(s) the user should run, or "none".
