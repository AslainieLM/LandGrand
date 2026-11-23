# Plateaus Easy 1–3: handover (D-072)

Complete (ticket 04, 2026-11-24). Built in four ticket sessions; this file is the full record. Nothing is committed. The user plays the checklist at the end, then commits; the vault gets the play result.

## Result

- Ticket 01 done 2026-11-23: EditMode 515/515 passing (475 before).
- Ticket 02 done 2026-11-24: EditMode 524/524 passing (515 before).
- Ticket 03 done 2026-11-24: EditMode 524/524 passing (no new tests; play code and builders). Six builders run; scenes built.
- Ticket 04 done 2026-11-24: **final EditMode 524/524 passing** (475 before ticket 01; 49 added). Final re-run: the four asset scripts twice (byte-identical to each other and to disk); all six builders exit 0 with no "Missing", errors or refused props (48 levels); all 45 old level scenes match `HEAD` apart from fileIDs; menu, map, Lesson, Description and Profile scenes differ only by the Plateaus life and pictures.

### Ticket 04

- `CONTEXT.md`: **Cliff updraft** (Levels) and **Plateau** (Landforms) added; existing words kept.
- `Assets/Editor/SelectionBuilder.cs`: removed an extra blank line left by ticket 03.
- Final `/code-review` against `HEAD` 78474eb (there is no Plateaus plan commit; the plan files are uncommitted). Standards: no hard violations; every new line has its comment; all enum, kind and badge appends are at the end; no Unity types in Rules. Spec: every number in `LevelCatalog` matches `layouts.md`, and the kinds, badges, audio, summary, updraft rule, play code, themes, props, map and menu life match the spec. Judgement calls left as they are (house pattern, low value now): `PlaceHillsNormalProp` is reused for the falls spray under its Hills name; the three Plateaus theme rows repeat their script arguments; the `LevelLayout` constructor now has 21 parameters; the `±0.5` tolerance in `Updraft.Contains` has no name.

## Files

### Ticket 01

- `Assets/Scripts/Rules/Updraft.cs` (new): the cliff updraft rule (spec section 5).
- `Assets/Scripts/Rules/LevelLayout.cs`: `PineapplePlateau`, `LakePlateau`, `FallsEdge` appended to `LevelTheme`; `Updraft[] updrafts = null` as the last constructor parameter; `Updrafts` list.
- `Assets/Scripts/Rules/LevelData.cs`: `halloysite`, `goethite`, `gibbsite`, `cristobalite` appended to `ItemCatalog.All`; `Episode.Plateaus` added to basalt, andesite, hematite, tuff, volcanic_breccia and laterite.
- `Assets/Scripts/Rules/LandformAudio.cs`: the Plateaus row and its scripts.
- `Assets/Scripts/Rules/LandformSummary.cs`: the Plateaus summary.
- `Assets/Scripts/Rules/Achievements.cs`: the 11 Plateaus badges after the Plains badges.
- `Assets/Tests/EditMode/PlateausRulesTests.cs` (new); updated `AchievementTests`, `CollectionTests`, `HillsNormalLevelsTests`, `LandformAudioTests`, `LandformSummaryTests`.

### Ticket 02

- `Assets/Scripts/Rules/LevelData.cs`: Plateaus Easy 1–3 appended at the end of `LevelCatalog`, numbers from `layouts.md` and text from spec section 7 (checked line by line).
- `Assets/Tests/EditMode/PlateausEasyLevelsTests.cs` (new, 9 tests): shape and content per trail, every layout number, kinds taught and reviewed, updraft fairness, only `PlateausEasy3` has updrafts.
- `Assets/Tests/EditMode/LevelCatalogTests.cs`: Plateaus Easy 1 now exists; Easy 4 and Normal 1 do not.
- `Assets/Tests/EditMode/PlainsHardLevelsTests.cs`: `HasLevels(Plateaus, Easy)` is now true; `EarlierTrailsKeepTheirCountsAndObjects` leaves out Plateaus (it counts the 38 trails before Plains Hard).

### Ticket 03

- `Assets/Scripts/Play/ExplorerMotor.cs`: `UpdraftTop` (NaN by default) and `Ride(dt)`. Inside an updraft a jump press and the jump buffer are dropped; after the gravity line `Ride` sets `grounded = false`, `StandingOn = null`, `jumpedFromGround = true`, `airJumpUsed = false` and `velocity.y = Updraft.RiseVelocity`. With `UpdraftTop` NaN every line runs as before.
- `Assets/Scripts/Play/UpdraftWatcher.cs` (new level part, `Begin`, `IsBuilt`, execution order −50 like `WindWatcher`): sets `UpdraftTop` from `Updraft.Contains` each frame (NaN while paused, frozen or not playing); loops `sfx_updraft.wav` at 0.5 through the level sfx volume and stops it on leaving and during a pause; draws the wind frames at 8 per second and moves two leaves and two dust specks up each column on `LevelClock.Now`.
- `Assets/Scripts/Play/LevelController.cs`: `PlaySfx(AudioSource loop, float volume = 1f)` (see "Changes not in the spec").
- `Assets/Editor/LevelBuilder.cs`: `BuildUpdrafts` builds each column (base 0–8 px, tiled wind 8 px up to `top + 26`, two leaves, two dust specks) and adds the watcher only on levels with updrafts; missing art names `make_plateaus_easy_art.py`, a missing sound names `make_plateaus_audio.py`.
- `Assets/Editor/LevelThemes.cs`: the `PineapplePlateau`, `LakePlateau` and `FallsEdge` entries (spec section 8 table, goal offset (24, 24)); `PineapplePlateauProps`, `LakePlateauProps`, `FallsEdgeProps`; `TrailPropFits` keeps props 8 px from every updraft column (no old level has updrafts).
- `Assets/Editor/PixelSceneKit.cs`: `PlateausEasyArtScript` and its lesson and item picture names for missing-file messages.
- `Assets/Editor/LandformLife.cs`: `ConfigurePlateausSprites`, `Hawk`, `PlateausStandingLife`; the Plains standing life now shares a private `StandingLife` (same output).
- `Assets/Editor/LandformMapBuilder.cs`: `PlateausLife()`, shown only while Plateaus is open.
- `Assets/Editor/SelectionBuilder.cs`: `PlateausMenuLife()` (cloud, two hawks at (330, 60) radius (24, 10), 12 s and 15 s, six grass tufts on row 268); `LessonBuilder.cs` and `LandformDescriptionBuilder.cs` import the Plateaus life sprites too, because they reuse the menu backdrop.
- Scenes rebuilt by the six builders: `PlateausEasy1`–`3` (new) and every old scene.

### Ticket 03 checks

- Asset scripts (`make_plateaus_easy_art.py`, `make_plateaus_easy_backdrops.py`, `make_profile_art.py`, `make_plateaus_audio.py`) each run twice: both runs byte-identical to each other and to the files already on disk.
- Builders: all six exit 0; no "Missing", no script errors (the only error line is Unity's licensing token notice). `Built 48 levels`.
- Regression: all 45 old level scenes compared with `HEAD`, ignoring object order and fileIDs: no difference in any of them (not only Plains Easy 3, Hills Hard 1 and Mountains Hard 1). Menu scenes gain only the Plateaus life; the Lesson, Description and Profile controllers gain the new Plateaus pictures.
- Props: no Plateaus prop was refused or moved more than 64 px. Counts: E1 46 pineapple plants and 3 hawks; E2 32 lake reeds and 2 hawks; E3 37 ferns, 2 falls sprays (exactly at 5100 and 5180) and 2 hawks.
- Goal offsets: `farm_lookout`, `torogan` and `falls_deck` have no clear rows at their base, so (24, 24) puts each on the ground. No change.
- Map life spots (map pixels; bottoms): waterfall (396, 168), on the lit face just under the top edge of the big mesa; grass (300, 158), (468, 200), (350, 240), (262, 252) on open ground, clear of the padlock, label and trail; hawks circle (410, 148), radius (20, 8), 14 s and 17 s per lap.

## Numbers changed from `spec.md` or `layouts.md`

- None so far.

## Changes not in the spec

- `Updraft.RiseVelocity` returns 0 when `dt <= 0`. The spec formula `Clamp((Top − feetY) / dt, 0, RiseSpeed)` gives NaN or infinity at `dt = 0`, which could reach the explorer's `velocity.y`. Tested by `RiseVelocity_IsZeroWhenNoTimePassed`. Kept by the user on 2026-11-23.
- Ticket 03: `LevelController.PlaySfx(AudioSource loop, float volume = 1f)` gains an optional volume, so the updraft loop plays at 0.5 through the level sfx volume as the spec asks. Volume 1 keeps the log roll exactly as before. Kept by the user on 2026-11-24.
- Ticket 03: the spec does not give these updraft view details, so they were chosen: the wind tiles start on top of the 8 px base (as in the art preview); draw orders wind 5, base 7, leaves and dust 18 (just behind the explorer); leaves at x + 10 and x + 22, dust at x + 6 and x + 26, a quarter loop apart; they rise at 90 px/s (the explorer's rise speed); the map hawks use radius (20, 8) and 14 s and 17 s laps, like the Plains egrets. Kept by the user on 2026-11-24.
- Ticket 03: the two falls sprays are placed at exactly 5100 and 5180, as the spec says. The usual row helper stops at goal − 90 (5150), which would have moved the second one.
- Two old tests were removed because no landform uses a fallback now: `LandformSummaryTests.OtherLandformsHaveOnlyAComingSoonLine` and `LandformAudioTests.AmbienceFor_OtherLandformsUseTheMountainsSounds`. The fallback code stays, untested.

## Review notes (ticket 01 `/code-review`)

- No hard violations and no blocking spec defects.
- Left open: the test name `CollectionTests.LandformsWithoutItemKinds_AreComingSoon` no longer fits (it now checks that every shelf has kinds); rename when that file is next touched.
- `land_grand_champion` sits on the Plateaus page (11 badges), as the spec says; check the 5×3 grid fits in ticket 03.

## Review notes (ticket 03 `/code-review`)

- Standards: no hard violations. Every new line has its comment; with `UpdraftTop` NaN the motor runs exactly as before; no per-level, per-landform or per-theme `if` in play code.
- Spec: no missing or wrong requirement.
- Fixed: `Tiled` now gives back its renderer, so `BuildUpdrafts` no longer finds the wind by the name "Wind"; `SelectionBuilder.CloudOnly` had no caller after `PlateausMenuLife` and was removed.
- Left as judgement calls (house pattern): `ExplorerMotor` takes the hover height as a float with NaN for "none" (as the spec names it) and makes its own `Updraft` for `RiseVelocity`; `ConfigurePlateausSprites` repeats `ConfigurePlainsSprites`, and three builders call both; `BuildUpdrafts` follows `BuildStrawPiles`.
- Note: `LessonBuilder`, `SelectionBuilder` and `LandformDescriptionBuilder` now stop with `DirectoryNotFoundException` naming `make_plateaus_easy_art.py` if `Assets/Art/LandformMap/Sprites/Plateaus/` is missing, the same way they already stop for the Plains folder.
- Editor check for ticket 04: the wind uses tiled draw mode centred on the column, so a part-tile may sit at the bottom or top of the column rather than at the top only. Look for a seam where the wind meets the base.

## Not checked in the Editor

- Ticket 01 needs no Editor check.
- Ticket 02 needs no Editor check. Until ticket 03 builds the scenes and themes, Plateaus Easy shows as playable in the menus but has no scenes and no `LevelThemes` entries; do not run `LevelBuilder.BuildAll` before ticket 03.
- The goal names (Farm Lookout, Torogan, Falls Viewing Deck) live in the `LevelThemes.cs` theme table, so they are set and tested in ticket 03, not here.
- Ticket 03, not checked in the Editor: the updraft feel (rise, hover, jump ignored, double jump after leaving, landing ledges), the wind loop and its stop on pause, the column, leaf and dust look, the goal pictures on the ground, the props and hawks, the map waterfall, grass and hawks, the Plateaus menu life, and the 11 badges on the Plateaus page of the 5×3 grid. These go in the ticket 04 play checklist.
- Ticket 03 adds no EditMode test: the new code is a `MonoBehaviour` and editor builders, and the rules it uses are covered by `PlateausRulesTests` and `PlateausEasyLevelsTests`.

## Play checklist (from ticket 04)

- [ ] **Unlock:** after all 15 Plains levels, the map plays the one-time Plateaus unlock; the Plateaus region shows a waterfall, grass and two hawks.
- [ ] **Menus:** the Plateaus difficulty and level screens show hawks and grass; Normal and Hard say "Coming soon".
- [ ] **Music:** Plateaus levels play the new Plateaus music and the dry wind with a hawk.
- [ ] **E1 Bukidnon Plateau:** pineapple farm look, Farm Lookout goal; basalt, andesite and hematite join the Plateaus shelf; the halloysite card opens on first find (secret spot on the three-step climb).
- [ ] **E2 Lake Lanao:** lake look, torogan goal; tuff, volcanic breccia and goethite cards; the halloysite review on the high ledge.
- [ ] **E3 Maria Cristina Falls:**
  - the first updraft is on safe ground; standing in it lifts you slowly with a soft whoosh, leaves and dust
  - at the top you hover; walking right puts you on the ledge; jumping inside does nothing
  - walking straight through only lifts you a little
  - the updraft at 1200 reaches the gibbsite ledge; the one at 3300 reaches the cristobalite secret
  - the goethite review sits between the two spikes
  - pausing freezes the wind, leaves and sound
- [ ] **Length:** each level takes about 1 minute (bot 57–63 s; the limit is 120 s).
- [ ] **Profile:** the Plateaus shelf shows 10 kinds; the Plateaus badge page shows 11 badges; Updraft Rider and Philippine Plateaus after E3; High Star after 3 stars on all three.
- [ ] **Regression:** Plains Easy 3 straw piles, Hills Hard logs and Mountains Hard ice look and move as before. Spot-check one Plains Hard level.

Also look at, from "Not checked in the Editor":

- [ ] The goal pictures (Farm Lookout, Torogan, Falls Viewing Deck) stand on the ground, not floating or sunk.
- [ ] The updraft wind column has no visible seam where it meets its base.
- [ ] The 11 Plateaus badges fit the 5×3 badge page without overflow.
