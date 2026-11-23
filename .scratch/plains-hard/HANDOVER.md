# Plains Hard 1–7: handover (D-071)

Built on 2026-11-23 by a Claude Code session from `SESSION-PROMPT.md`. Tickets 01–04 are done. Nothing is committed: the user makes the commits.

## Result

- **EditMode tests:** 475 pass, 0 fail (440 before ticket 01).
  - Ticket 01: 443.
  - Ticket 02: 467.
  - Ticket 03: 475.
- **Asset scripts:** `make_plains_hard_art.py`, `make_plains_hard_backdrops.py` and `make_plains_hard_audio.py` were each run twice.
  - Both runs wrote the same 125 files, byte for byte.
  - The files match the committed copies: git shows no change.
- **Builders:** all six ran one at a time with the Editor closed, each with exit 0 and no "Missing", error or exception lines: `LevelBuilder.BuildAll`, `LessonBuilder.Build`, `SelectionBuilder.Build`, `LandformMapBuilder.Build`, `LandformDescriptionBuilder.Build` and `ProfileBuilder.Build`.
- **Scenes:** `PlainsHard1`–`PlainsHard7` are built and in Build Settings.
  - Every level links `bgm_plains`.
  - H1–H3 and H5–H7 link `amb_dry_hard`; H4 links `amb_cold_plain`.
  - Theme art per level:
    - **H1:** hay frames, `sfx_hay_roll`.
    - **H2:** wildebeest frames, `sfx_herd_hooves`, lift 2, faces left.
    - **H3:** hay and cattle.
    - **H4:** `frost_top`.
    - **H5:** bat frames and `sfx_bat_flap`.
    - **H7:** hay, horses and `frost_top`.
  - Goals: hay_barn, kopje, estancia_gate, winter_hut, nullarbor_cave, stilt_house and ger.

## Regression check (ticket 03, item 8)

Each rebuilt scene file gives every object a new fileID, so a plain diff is useless. A comparison script read every old scene and the `HEAD` copy into canonical form:
- each object is named by its hierarchy path, not its fileID
- each reference is replaced by the path it points to
- the order the scene file lists objects in is ignored

**Result for all 38 old scenes** (Mountains 15, Hills 15, Plains Easy 3, Plains Normal 5):
- No object was added or removed.
- No position, sprite, timing or flip changed.
- Only two kinds of lines differ:
  - 114 lines: every level scene's item-card sprite list now also holds `chalk`, `loess` and `flint` (3 per scene). This is expected because every level carries all card pictures.
  - 8 lines: the 4 Hills Normal and Hills Hard bat scenes store the two new `BatFlock` fields at their defaults, `artLift: 0` and `artFacesLeft: 0`.

So Hills Hard logs, Hills Normal bats and Mountains Hard ice are unchanged.

## Files

| File | Change |
|---|---|
| `Assets/Scripts/Rules/LevelLayout.cs` | `LevelTheme` gains Prairie, Savanna, Pampas, FrozenPlain, LimestonePlain, GangesPlain and Steppe at the end (ticket 01). |
| `Assets/Scripts/Rules/LevelData.cs` | Changes in `ItemCatalog` and `LevelCatalog`, listed below this table. |
| `Assets/Scripts/Rules/LandformAudio.cs` | `amb_cold_plain` names `make_plains_hard_audio.py` for the missing-file message. |
| `Assets/Scripts/Play/BatFlock.cs` | `artLift`: the picture is drawn this many px above the rule's position; the touch box is unchanged. `artFacesLeft` reverses the flip. `IsBuilt` accepts 2 or more frames. |
| `Assets/Editor/LevelThemes.cs` | New fields and helpers, listed below this table. |
| `Assets/Editor/PlainsHardProps.cs` (new) | The section 8 living props, placed with `RepeatTrailProp`, `PlaceTrailProps` and `PlacePlainsAnimals`. |
| `Assets/Editor/HardLevelObjects.cs` | The roller build reads the theme's frames, script and sound (`ThemeSound`); the ice strip reads `IceTop` and `IceTopScript`. |
| `Assets/Editor/HillsLevelObjects.cs` | The bat build reads the theme's frames, count, script, sound, lift and facing. |
| `Assets/Editor/PixelSceneKit.cs` | `PlainsHardArtScript`, and the lesson and item pictures that name it in missing-file messages. |
| `Assets/Editor/ThemeArtDefaultsTests.cs` (new) | 8 NUnit tests in the editor assembly: old themes set no new field and keep `log`, `bat` and `ice_top`; each Plains Hard theme names the spec section 8 art. |
| `Assets/Tests/EditMode/PlainsHardLevelsTests.cs` (new) | Sizes, pickups, secret, herds, hay, flags, water pits, H4 override and script, progression, badges and regression. |
| `Assets/Tests/EditMode/{Collection,Achievement}Tests.cs` | The Plains shelf has 24 kinds and the total is 59 (ticket 01). |
| `Assets/Tests/EditMode/{LevelCatalog,NextPlayable}Tests.cs` | Plains Hard now has data, and Plains Normal 5 shows Next to Hard 1. |
| `Assets/Tests/EditMode/HillsHardLevelsTests.cs` | The "no mud or logs" check covers only the trails before Hills Hard, because Plains Hard hay uses logs. |
| `Assets/Tests/EditMode/PlainsEasyLevelsTests.cs` | The "only one trail has straw piles" check skips Plains Hard, because H1 has three piles. |
| `CONTEXT.md` | Adds **Hay bale** and **Herd**. |
| `Assets/Scenes/PlainsHard1–7.unity` (new) | Built by `LevelBuilder.BuildAll`. |
| `Assets/Scenes/*.unity`, `ProjectSettings/EditorBuildSettings.asset` | Rebuilt by the builders (see the regression check). |
| `.scratch/plains-hard/issues/01–04` | Status lines set to done. |
| `.scratch/plains-hard/props-moved.txt` | The props log copied from the level build. |

**`LevelData.cs` changes:**
- `ItemCatalog`:
  - adds `chalk`, `loess` and `flint` at the end (ticket 01)
  - adds Plains to granite, tuff, calcite, mica and feldspar
- `LevelCatalog`:
  - adds `PlainsHard(1..7)` with the `layouts.md` numbers (ticket 02)
  - adds the `PlainsHard` helper, which sets the start, the three flags and the goal

**`LevelThemes.cs` changes:**
- Optional `ThemeArt` fields. A null or 0 value keeps the old art:
  - roller: `RollerFrames`, `RollerWarnFrames`, `RollerArtScript`, `RollerSound`, `RollerSoundScript`
  - bat: `BatFrames`, `BatFrameCount`, `BatArtScript`, `BatSound`, `BatSoundScript`, `BatArtLift`, `BatArtFacesLeft`
  - ice: `IceTop`, `IceTopScript`
- The seven theme entries, built by the new `PlainsHardTheme` helper.
- `OldThemesWithNewArt` and `ObjectArtNames`, used by the tests.
- `PlacePlainsAnimals` takes an optional `order` (default 4).

## Numbers changed from `spec.md` or `layouts.md`

None. A script compared every boulder, ledge, pit, spike, hay route, herd, bat, ice stretch, straw pile, float, current, crumbling ledge and pickup with `layouts.md`: 0 mismatches.

## Goal offsets

All seven goals use (24, 24). Each goal picture (56×48) has its bottom row filled, with no clear rows at the base. None was lowered.

## Props moved more than 64 px

The builder moves a prop to the nearest spot that passes the fit check. These are the moves larger than 64 px (from the level build log):

| Level | Moves |
|---|---|
| H1 | fence_post 860→788 (72), 3500→3612 (112), 4460→4532 (72), 5180→5068 (112); hay_roll_prop 8200→8083 (117) |
| H2 | none |
| H3 | pampas_grass 7980→7879 (101), 8580→8459 (121) |
| H4 | birch_tree 5150→4987 (163), 7950→8113 (163); snow_tuft 1870→1693 (177), 3270→3193 (77), 3620→3707 (87), 6070→5993 (77), 7820→8107 (287), 8170→8282 (112), 8870→8793 (77) |
| H5 | none |
| H6 | wheat_sheaf 900→1006 (106), 2700→2494 (206), 3900→3794 (106), 4500→4706 (206), 5100→5198 (98), 6300→6194 (106), 6900→7106 (206), 8100→7994 (106), 8700→8906 (206); egret 2800→3057 (257), 6800→6493 (307); reed_tuft 2950→3057 (107), 4450→4707 (257), 5950→6107 (157), 6950→7107 (157), 8450→8293 (157) |
| H7 | steppe_grass 660→907 (247), 920→1039 (119), 2740→2887 (147), 3000→3071 (71), 4300→4557 (257), 4560→4687 (127), 5080→4993 (87), 5340→5407 (67), 7420→7293 (127), 8720→8907 (187), 8980→9185 (205), 9240→9315 (75); eagle 7600→7717 (117) |

H6 and H7 are crowded with water pits, currents, crumbling bridges and ice, so many props had to move. No prop was refused. The eagles fly at y 120 but use the same ground fit check, which is why one moved.

## Changes not in the spec

1. **Herd facing (asked and approved, 2026-11-23: "Theme flag").** The herd pictures face left, but `BatFlock` flips a picture when it moves left (made for the round bat). Without a fix, herds would walk backwards. Added `ThemeArt.BatArtFacesLeft` and `BatFlock.artFacesLeft` (default false), which reverse the flip. This is data only, and old bats are unchanged.
2. `BatFlock.IsBuilt` accepts 2 or more frames instead of exactly 3, so `BatFrameCount` can work. Every theme still uses 3.
3. Extra script fields `RollerArtScript`, `BatArtScript` and `IceTopScript`, so missing-art messages name the Plains Hard script from theme data. The spec asked for the script names in data; it only listed the sound scripts by name.
4. `PlacePlainsAnimals` gained an optional layer `order` (default 4, so older calls are unchanged). Zebras use order 2 and eagles order 1, as section 8 says.
5. `LandformAudio` lists `amb_cold_plain` with `make_plains_hard_audio.py` for the missing-file message.
6. The theme default test (`ThemeArtDefaultsTests`) lives in `Assets/Editor`, because the EditMode test assembly can only see the rules. Unity runs it with the other EditMode tests.
7. Four older tests were narrowed, because Plains Hard now uses hay (logs), straw piles and data: see Files.
8. The hay, herd and frozen-ground objects keep the old object names in the scene ("Rolling Log", "Gentle Bat", "Ice Top"), so the shared code needs no per-theme name.

## Not checked in the Editor

- No level was played: movement, hay timing, herd nudges, the frost slide and the floats are checked only by the rules tests and the layout bot.
- Not yet seen on screen:
  - the herd pictures with lift 2, and their facing
  - the goal pictures at (24, 24)
  - prop looks and layer order
  - the backdrops
- Sound levels of `sfx_hay_roll`, `sfx_herd_hooves` and `amb_cold_plain`.
- The Landform Map's Plateaus unlock after all 15 Plains levels (rules tests only).
- Android device check (still open from earlier landforms).

## Play checklist for the user

- [ ] **Unlock:** after Plains Normal 5, Plains Hard 1 opens.
- [ ] **H1 Great Plains:**
  - the practice hay bale rolls toward you in a safe spot, with a straw-dust warning and a soft rustle
  - right-rolling bales only catch you if you stop
  - straw piles still bounce
  - the chalk card opens on first find
- [ ] **H2 Serengeti:**
  - wildebeest walk back and forth, face where they walk, and stand on the ground
  - a touch nudges you away with soft hooves and never hurts; you can jump over them
  - granite and tuff appear on the Plains shelf
- [ ] **H3 Pampas:** hay bales and cattle together; the loess card says it is not a true rock.
- [ ] **H4 West Siberian Plain:**
  - frosty ground slides like ice
  - bog floats over two water pits
  - cold wind ambience (not the dry wind)
- [ ] **H5 Nullarbor Plain:** crumbling bridges and bats; no trees; the flint card opens.
- [ ] **H6 Indo-Gangetic Plain:** water pits with floats; currents push both ways.
- [ ] **H7 Mongolian Steppe:** hay, horses, frost, crumbling bridges and currents; ger goal.
- [ ] **Length:** each level takes about 2 minutes (bot 113–124 s; the limit is 240 s).
- [ ] **Profile:**
  - the Plains shelf shows 24 kinds and "Found here n/24"
  - Prairie Ranger after any Hard clear; World Plains after H5–H7; Plain Master after all 15
- [ ] **Plateaus:** after all 15 Plains levels, the map plays the one-time unlock and Plateaus says "Coming soon".
- [ ] **Regression:** Hills Hard logs, Hills Normal bats and Mountains Hard ice look and move as before. Spot-check one Plains Normal level.

## Review fixes (`/code-review` against `0c81d8e`, 2026-11-23)

The Spec reviewer found no wrong numbers or text. The Standards reviewer found one documented-rule point and several judgement calls.

**Fixed:**
- **Duplicated defaults:** the `?? Default…` fallbacks were written in three places. `ThemeArt` now resolves them once (`UsedRollerFrames`, `UsedRollerWarnFrames`, `UsedRollerSound`, `UsedBatFrames`, `UsedBatFrameCount`, `UsedBatSound`, `UsedIceTop`), and the builders and `ObjectArtNames` read those.
- **Ticket 04 status** set to done. The final re-run is recorded below.

**Kept, with the reason:**
- `PixelSceneKit` names `make_plains_hard_art.py` for the seven lesson pictures and the three item pictures, in the same kind of list each landform already has. These pictures belong to the Lesson and Profile screens, not to a level theme. Issue 03 item 6 covers the theme art, backdrops and audio, and those take their script names from theme data. Moving every landform's list into data would be a refactor outside this build.
- `ThemeArt` has 14 loose optional fields instead of grouped roller, herd and ice records. This follows the spec's field list and the existing `PlatformSprite` / `PlatformLandSound` pattern.
- `ObjectArtNames` returns strings for a simple test comparison. It is only used by `ThemeArtDefaultsTests`.
- Herds keep the `Bat*` field names because a herd is the bat rule (CONTEXT.md **Herd**).
- The eagles go through the ground fit check like every prop, so the 7600 eagle moved to 7717. Many H6 and H7 props moved too (see the table above); check them on screen.
- The Plains shelf grows to 24, so Plain Collector needs 24 kinds. This is spec section 1: "Plain Collector (the whole Plains shelf, now 24)".

**Final re-run after the fixes:**
- Asset scripts: no change from the committed files.
- EditMode: **475 pass, 0 fail**.
- All six builders: exit 0, with no "Missing", error, exception or "refused" lines. The 45 prop moves in the table are the same as before.
- Scene comparison against `HEAD`: the 38 old level scenes still differ only in the new item-card sprites and the default `BatFlock` fields. The Lesson scene adds the seven lesson pictures, and the Profile scene adds the three item pictures.
- Logs and results are in `Logs/PlainsHard/`.

## Commit notes

Do not commit `.scratch/*/bot/bin`, `.scratch/*/bot/obj` or `Assets/Resources/GDKEditionAutoGen*`. The new PNG `.meta` files (items, lessons, backdrops) were made by Unity's import and belong with the art.
