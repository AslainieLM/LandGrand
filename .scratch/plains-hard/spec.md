# Plains Hard 1–7 (D-071)

Planned on 2026-11-22 in a grilling session (two rounds, eight questions). The user accepted every recommendation. It follows the D-069 outline. Plains Easy and Plains Normal are built, reviewed and played ("all passed"; `f0b8ff6`, `7af9350`). This is the last part of Plains: when all 15 Plains levels are cleared, Plateaus opens.

**Build split (changed 2026-11-22):** Claude made the art, backgrounds, sound and bot-checked layouts. A Claude Code session builds the C#, one ticket per session (`/implement`, then `/code-review`), from `SESSION-PROMPT.md`; there is no Codex hand-over. Then the user plays the checklist.

## 1. Rules that are already settled (do not change)

- **Hard sizes** (as Mountains and Hills Hard): width 9600, start (40, 0), goal 9440, checkpoints 2400, 4800 and 7200, 240 s, 9 pickups with exactly one secret on a high ledge (y ≥ 72).
- Pits and spikes send the explorer back to the furthest flag reached.
- Level differences are data (D-061). Rules have no Unity types and are tested. Every theme has its own six-layer background (D-063).
- **Walk speed:** 84 px/s everywhere.
- **Shared kinds:** one kind can sit on several shelves; it is counted once (D-066).
- **Drafts:** all lesson, review and kind text is a draft for teacher review (Q-04).
- **Badges (already built, D-069):** Prairie Ranger (clear any Plains Hard level), World Plains (clear Plains Hard 5–7), Steady Plains (3 stars on all 7), Plain Master (all 15 Plains levels), Plain Collector (the whole Plains shelf, now 24). No new badges.
- **Unlocking Plateaus:** generic, as Plains opened after Hills: the one-time map animation, "Plateaus is now open!", then "Coming soon".
- **Ambience:** the Plains Hard table entry `amb_dry_hard` (dry wind and far thunder), except H4 (section 2).
- **No regressions:** Mountains, Hills, Plains Easy and Plains Normal must play exactly as before.

## 2. New decisions (D-071)

1. **Places (the D-069 outline, finale chosen):** H1 Great Plains (USA), H2 Serengeti (Tanzania), H3 Pampas (Argentina), H4 West Siberian Plain (Russia), H5 Nullarbor Plain (Australia), H6 Indo-Gangetic Plain (South Asia), H7 **Mongolian Steppe** (the finale).
2. **No new rules.** Three new looks on old rules:
   - **Hay bale** = the rolling-log rule (`RollingLog`, same numbers), with hay frames and `sfx_hay_roll`.
   - **Herd animal** = the bat rule (`Bat`, same push, rest and size) **on the ground**: centre y 6, up-down radius 0, left-right radius 48. It walks 96 px back and forth, pushes the explorer 24 px away on a touch and never hurts. Art: wildebeest (H2), cattle (H3), horse (H7); touch sound `sfx_herd_hooves`.
   - **Frozen ground** = the icy-ground rule (`IceZone`, same numbers and `sfx_ice_slide`), with a new `frost_top` tile.
3. **Objects per level:**

| Level | Objects |
|---|---|
| H1 | hay bales (first one practice, rolling toward the player) and 3 straw piles |
| H2 | wildebeest herds only (first one practice) |
| H3 | hay bales and cattle herds |
| H4 | frozen ground and two peat-bog floats over water pits |
| H5 | crumbling ledges and bats |
| H6 | floats over flood ponds and river currents (all pits water) |
| H7 | everything: hay, horse herds, frozen ground, crumbling bridges and currents |

4. **Kinds** (only kinds a source ties to the place; `kinds-research.md`):
   - **New, Plains only:** `chalk` (H1), `loess` (H3) and `flint` (H5). Lignite was dropped: no source places it on the West Siberian Plain.
   - **Shared kinds that join the Plains shelf:** `granite` and `tuff` (H2), `calcite` (H1, H5), `mica` and `feldspar` (H3).
   - **Reviews:** peat (H4), laterite (H6), and earlier Plains kinds. H7 uses reviews only.
   - The Plains shelf goes from 16 to **24** kinds; the total goes from 56 to **59**.
   - Loess is wind-blown sediment, not a true rock. Like peat, it is stored as a rock (`IsRock` true) so it shows on the shelf, and its card says it is not a true rock (teacher check).
5. **Sound:** new `sfx_hay_roll` and `sfx_herd_hooves`; new ambience `amb_cold_plain` for H4 only (`AmbienceOverride`). The other six use `amb_dry_hard`.
6. **Water pits:** H4 pits (3000, 104) and (6500, 104) and every H6 pit are water (`IsWater`). All other pits are dry.

## 3. Level outline

| Level | Banner | Theme | Goal | Objects | Kinds (review) | Bot time |
|---|---|---|---|---|---|---|
| H1 | Great Plains | Prairie | Hay Barn | 9 hay bales, 3 straw piles | **chalk**, calcite; (limestone, shale, sandstone) | 112.6 s |
| H2 | Serengeti | Savanna | Kopje | 6 wildebeest herds | granite, tuff; (sandstone, siltstone, shale) | 115.7 s |
| H3 | Pampas | Pampas | Estancia Gate | 5 hay bales, 3 cattle herds | **loess**, mica, feldspar; (quartz, clay) | 115.0 s |
| H4 | West Siberian Plain | FrozenPlain | Winter Hut | 7 frozen stretches, 2 bog floats | (peat, clay, siltstone, mudstone, sandstone) | 112.6 s |
| H5 | Nullarbor Plain | LimestonePlain | Cave Entrance | 3 crumbling bridges, 2 steps, 6 bats | **flint**; (limestone, calcite, chalk) | 115.5 s |
| H6 | Indo-Gangetic Plain | GangesPlain | Stilt House | 12 floats, 6 currents | (laterite, clay, siltstone, sandstone, mudstone) | 123.8 s |
| H7 | Mongolian Steppe | Steppe | Ger | 4 hay, 2 horse herds, 4 frozen, 3 bridges, 2 currents | (granite, chalk, loess, flint, tuff, mica, feldspar) | 118.0 s |

The exact numbers are in `layouts.md`. The Hills Hard bot times are 112.6–132.0 s at the same walk speed.

## 4. Code changes

| Where | Change |
|---|---|
| `Rules/LevelLayout.cs` `LevelTheme` | Append `Prairie`, `Savanna`, `Pampas`, `FrozenPlain`, `LimestonePlain`, `GangesPlain`, `Steppe` at the **end**. |
| `Rules/LevelData.cs` `ItemCatalog` | Append `chalk`, `loess` and `flint` at the **end** of `All` (Plains only, `IsDraft`; all three `IsRock`). Add `Episode.Plains` to granite, tuff, calcite, mica and feldspar (keep their existing landforms). |
| `Rules/LevelData.cs` `LevelCatalog` | Plains Hard 1–7 exactly as `layouts.md`. Herds are `new Bat(cx, 6f, 48f, 0f, period, phase)`. Hay is `new RollingLog(spawn, end, direction, offset)`. Pits marked water as in section 2. H4 has `ambienceOverride: "amb_cold_plain"`. |
| `Editor/LevelThemes.cs` `ThemeArt` | New optional fields; **null keeps today's art**, so old levels do not change: `RollerFrames` (prefix of the 4 roll frames, null = `log`), `RollerWarnFrames` (prefix of the 2 warning frames, null = `log_warn`), `RollerSound` (null = `sfx_log_roll`), `BatFrames` (prefix, null = `bat`), `BatFrameCount` (0 = 3), `BatSound` (null = `sfx_bat_flap`), `BatArtLift` (pixels the picture sits above the rule's centre, default 0), `IceTop` (null = `ice_top`), and the art and sound script names for the error messages (like `PlatformLandSoundScript`). Seven theme entries (section 8). |
| `Editor/HardLevelObjects.cs` | The roller build reads the theme's roller frames and sound; the ice strip reads the theme's `IceTop`. |
| `Editor/HillsLevelObjects.cs` bat build | Reads the theme's bat frames, count and sound. The picture is drawn at the rule's position plus `BatArtLift` in y (the touch box is unchanged). |
| `Play/BatFlock.cs` | Keep the lift when it moves each picture (the rule's y plus the lift). It already faces each picture along its motion. No rule change. |
| Living props | The section 8 props through the shared trail prop helper (`PlaceTrailProps`, `RepeatTrailProp`, `TrailPropFits`). Report any prop moved more than 64 px. |
| Missing-asset errors | Name `Tools/Art/make_plains_hard_art.py`, `Tools/Art/make_plains_hard_backdrops.py` or `Tools/Audio/make_plains_hard_audio.py`, taken from theme data (no hard-coded script names in shared code). |
| Tests | New `PlainsHardLevelsTests.cs` (sizes, pickups, the secret, herds at y 6 with radius 0, hay directions, water pits, the H4 override). Update the shelf and total counts (24 and 59). A test that every old theme keeps the default roller, bat and ice art. Update any test that says Plains Hard is "Coming soon" or that Plateaus is locked after Plains Normal. |

Already generic, no change expected: rolling logs, bats, ice, floats, currents, crumbles, straw piles, the Hard star rule, badges, the Plateaus unlock and Description Screen flow, and Next to the next level.

## 5. Object rules (unchanged; for reference)

| Object | Numbers |
|---|---|
| Hay bale (rolling log) | 14 px hit box, 90 px/s, 3.5 s cycle, 0.8 s warning; direction −1 left or +1 right; skipped for one cycle if the explorer stands within 48 px of the spawn when it would start |
| Herd (bat on the ground) | touch box 16×12 (y 0–12); push 24 px over 0.2 s; 1 s rest; never hurts |
| Bat | as Hills Normal: loop around (cx, 16) with radii 24 and 12 |
| Frozen ground (ice) | speeds up at 160 px/s², slows at 100 px/s² |
| Straw pile | 48×16 solid; landing on top launches at 400 px/s |
| Sinking float | 48 px; waits 0.6 s, sinks 8 px/s, holds nothing at 16 px down, rises 1.5 s after the explorer leaves |
| Crumbling ledge | 32 px; breaks 0.5 s after a touch, back after 3 s |
| River current | 30 px/s on the ground in the strip, in its direction |

## 6. Content (drafts, Q-04: needs teacher review)

### Lessons (4 sentences, each at most 44 characters)

**H1 "Grassland Plains"** (picture `lesson_great_plains`):
1. The Great Plains are wide, flat grassland.
2. They lie in the middle of North America.
3. Long ago, a sea covered these plains.
4. Its tiny shells became white chalk.

**H2 "Plains of Animals"** (picture `lesson_serengeti`):
1. The Serengeti is a plain in Tanzania.
2. Its grass grows on old volcanic ash.
3. Millions of wildebeest walk across it.
4. Watch wild animals from far away.

**H3 "Farm and Ranch Plains"** (picture `lesson_pampas`):
1. The Pampas are grasslands of Argentina.
2. The soil there is rich for farming.
3. Gauchos ride horses and herd cattle.
4. Wind once spread fine dust called loess.

**H4 "Cold Plains"** (picture `lesson_siberia`):
1. The West Siberian Plain is in Russia.
2. It is the largest flat lowland on Earth.
3. Its winters are long and very cold.
4. Huge bogs there are full of peat.

**H5 "Limestone Plains"** (picture `lesson_nullarbor`):
1. The Nullarbor Plain is in Australia.
2. Its name means "no tree".
3. It is one huge sheet of limestone.
4. Long ago, people mined flint in a cave.

**H6 "River Flood Plains"** (picture `lesson_ganges`):
1. Three big rivers built this huge plain.
2. Their floods leave rich soil behind.
3. Many people farm rice and wheat here.
4. It is in India and nearby countries.

**H7 "Plains of the World"** (picture `lesson_steppe`):
1. The steppe is a grassland across Mongolia.
2. Herders live in round tents called gers.
3. Winters on the steppe are very cold.
4. You explored plains around the world!

### Reviews (3 sentences, shown on the Result Screen)

- **H1:** "Grassland plains are flat and wide." / "Chalk is made of tiny sea shells." / "Hay is dried grass for farm animals."
- **H2:** "Grass plains feed huge herds." / "Granite rocks stand on the Serengeti." / "Tuff is rock made of volcanic ash."
- **H3:** "Plains are good for farms and ranches." / "Loess is fine dust laid down by wind." / "Mica and feldspar are grains in loess."
- **H4:** "Cold plains freeze in winter." / "Bogs hold peat made of old plants." / "Peat can slowly turn into coal."
- **H5:** "Limestone forms from shells in old seas." / "Flint is very hard and makes sparks." / "Calcite is the mineral in limestone."
- **H6:** "Rivers build plains with their mud." / "Floods bring new soil to the farms." / "Laterite is red, iron-rich rock."
- **H7:** "Steppes are wide, grassy plains." / "Herders move their animals to new grass." / "Plains are lands that are mostly flat."

### New kinds (Plains only)

| Id | Name | Rock? | Description | Picture |
|---|---|---|---|---|
| `chalk` | Chalk | rock | A soft, white rock made of the tiny shells of sea plankton. An old sea once covered the Great Plains. | `chalk` |
| `loess` | Loess | rock (see note) | Fine yellow dust carried by the wind and laid down in thick layers. It is not a true rock, but it makes rich farm soil. | `loess` |
| `flint` | Flint | rock | A very hard stone made of tiny quartz grains. People made tools from it and struck it to make sparks. | `flint` |

Shared kinds keep their existing text and pictures.

### Banners and goals

H1 "Great Plains" (Hay Barn), H2 "Serengeti" (Kopje), H3 "Pampas" (Estancia Gate), H4 "West Siberian Plain" (Winter Hut), H5 "Nullarbor Plain" (Cave Entrance), H6 "Indo-Gangetic Plain" (Stilt House), H7 "Mongolian Steppe" (Ger).

### Fact sources

All are from Wikipedia, checked 2026-11-22; the details are in `kinds-research.md`.

- **Not confirmed (teacher check):**
  - Mongolian herders, gers and cold winters (from a page summary only).
  - "Loess makes rich farm soil" (not quoted).
  - "Huge bogs there are full of peat" (the page says "raised bogs").
  - "Calcite is the mineral in limestone" (the Chalk page names calcite; the Nullarbor page says "calcareous skeletons").
- The teacher should check these and every draft sentence.

## 7. Layout data

See `layouts.md`; the builder must copy the numbers exactly.

## 8. Assets (made by Claude; do not edit)

- `python Tools/Art/make_plains_hard_art.py` makes:
  - **Tiles:** `<x>_top`, `<x>_soil`, `ledge_<x>` and `boulder_small_<x>` / `boulder_big_<x>` for `prairie`, `savanna`, `pampas`, `siberia`, `nullarbor`, `ganges` and `steppe`.
  - **Goals:** `hay_barn`, `kopje`, `estancia_gate`, `winter_hut`, `nullarbor_cave`, `stilt_house` and `ger` (56×48, base on the bottom row).
  - **Hay:** `hay_00`–`hay_03` (16×16 roll frames, like `log_00`) and `hay_warn_00/01` (24×4 straw dust, like `log_warn_00`).
  - **Herds:** `wildebeest_00`–`02`, `cattle_00`–`02`, `horse_00`–`02` (32×16 walk frames facing left; the feet are on the bottom row).
  - **Frozen ground:** `frost_top` (16×8, like `ice_top`).
  - **Props:** `fence_post`, `hay_roll_prop`, `windpump`, `acacia_tree`, `zebra_00/01`, `pampas_grass`, `birch_tree`, `snow_tuft`, `saltbush`, `wheat_sheaf`, `steppe_grass` (the Steppe eagles reuse the existing `Props/eagle_00/01`).
  - **Items:** `chalk`, `loess`, `flint`.
  - **Lesson pictures:** `lesson_great_plains`, `lesson_serengeti`, `lesson_pampas`, `lesson_siberia`, `lesson_nullarbor`, `lesson_ganges`, `lesson_steppe`.
  - Preview: `.scratch/plains-hard/art-preview.png`.
- `python Tools/Art/make_plains_hard_backdrops.py` makes `Backdrop/<Theme>/L0–L5` for the seven themes. Preview: `.scratch/plains-hard/backdrop-preview.png`.
- `python Tools/Audio/make_plains_hard_audio.py` makes `sfx_hay_roll.wav`, `sfx_herd_hooves.wav` and `amb_cold_plain.ogg` (a loop). Preview: `.scratch/plains-hard/audio-preview.png`.
- Reused, already made: the straw pile art and `sfx_straw_bounce`; `bat_00`–`02` and `sfx_bat_flap`; `sfx_ice_slide`; the float raft and ripple; `water_fill` and `current_top`; the crumble frames; `Props/egret_00/01`, `Props/reed_tuft`, `Props/palm_tree`, `Props/eagle_00/01`.

### Theme table for `LevelThemes.cs`

| Theme | GroundTop / Soil / Ledge / BoulderEnding | Goal (name, sprite, offset) | Roller, bat and ice art |
|---|---|---|---|
| Prairie | `prairie_top` / `prairie_soil` / `ledge_prairie` / `_prairie` | "Hay Barn", `hay_barn`, (24, 24) | roller `hay`, `hay_warn`, `sfx_hay_roll` |
| Savanna | `savanna_top` / `savanna_soil` / `ledge_savanna` / `_savanna` | "Kopje", `kopje`, (24, 24) | bat `wildebeest`, 3 frames, `sfx_herd_hooves`, lift 2 |
| Pampas | `pampas_top` / `pampas_soil` / `ledge_pampas` / `_pampas` | "Estancia Gate", `estancia_gate`, (24, 24) | roller `hay`; bat `cattle`, 3, `sfx_herd_hooves`, lift 2 |
| FrozenPlain | `siberia_top` / `siberia_soil` / `ledge_siberia` / `_siberia` | "Winter Hut", `winter_hut`, (24, 24) | ice `frost_top` |
| LimestonePlain | `nullarbor_top` / `nullarbor_soil` / `ledge_nullarbor` / `_nullarbor` | "Cave Entrance", `nullarbor_cave`, (24, 24) | default (bats stay bats) |
| GangesPlain | `ganges_top` / `ganges_soil` / `ledge_ganges` / `_ganges` | "Stilt House", `stilt_house`, (24, 24) | default |
| Steppe | `steppe_top` / `steppe_soil` / `ledge_steppe` / `_steppe` | "Ger", `ger`, (24, 24) | roller `hay`; bat `horse`, 3, `sfx_herd_hooves`, lift 2; ice `frost_top` |

Each theme's backdrop is `Backdrop/<Theme>/`. Check each goal in the Editor; if the sprite has clear rows at its base, lower it as was done for the Farm Shed (24, 22), and report it.

### Living props (decoration only; the trail prop fit test keeps them off pits, pools, ledges, boulders, crumbles, platforms and zones)

- **Prairie:** fence posts every 240 px from x 140 (order 3, y 8); `hay_roll_prop` at 1400, 3400, 5800 and 8200 (order 2, y 8); `windpump` at 2200 and 6400 (order 1, y 24).
- **Savanna:** `acacia_tree` every 900 px from x 300 (order 1, y 24); zebras (`zebra_00/01`, a swap every 1.5–2.5 s) at 2600 and 7000 (order 2, y 8).
- **Pampas:** `pampas_grass` every 300 px from x 180 (order 3, y 10); fence posts every 500 px from x 400 (order 3, y 8).
- **FrozenPlain:** `birch_tree` every 700 px from x 250 (order 1, y 24); `snow_tuft` every 350 px from x 120 (order 3, y 4).
- **LimestonePlain:** `saltbush` every 450 px from x 220 (order 3, y 6). No trees: the name means "no tree".
- **GangesPlain:** `wheat_sheaf` every 600 px on dry ground from x 300 (order 3, y 8); egrets at 2800 and 6800 (order 4, y 5); `Props/reed_tuft` every 500 px from x 450 (order 3, y 7).
- **Steppe:** `steppe_grass` every 260 px from x 140 (order 3, y 6); eagles (`eagle_00/01`, a swap every 1.5–2.5 s) at 3000 and 7600 (order 1, y 120).

## 9. Not in scope

- Plateaus levels and the Completion Screen.
- Any change to Mountains, Hills, Plains Easy or Plains Normal numbers or text.
- New badges or new rules.
- Refactoring the `LevelLayout` constructor.
