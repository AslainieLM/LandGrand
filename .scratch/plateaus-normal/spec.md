# Plateaus Normal 1–5 (D-073)

Planned on 2026-11-24 in a grilling session (two rounds, five questions), from the D-072 outline. The user accepted every recommendation and chose: **Claude plans, Codex builds.**

**Build split:** Claude made the art, backgrounds, sounds and bot-checked layouts. Codex writes the C# from `CODEX-PROMPT.md` / `CODEX-START.md`. Claude reviews the build, then the user plays the checklist.

## 1. Rules that are already settled (do not change)

- Normal levels: width 7200, start (40, 0), goal 7040, checkpoints 2400 and 4800, 180 s, 7 pickups with one secret on a high ledge (y ≥ 72). Pits and spikes send the explorer a short way back.
- One script runs every level; level differences are data in `LevelCatalog`; rules are plain C# with tests (D-061).
- Every level theme has its own six-layer background (D-063). Shared kinds count once (D-066).
- All lesson, review and kind text is a draft (Q-04). Walking 84 px/s.
- The cliff updraft, crumbling ledges, soft sand, currents, floats, moving platforms, ice and herds already exist; **no new rules** in this build.
- Mountains, Hills, Plains and Plateaus Easy must play exactly as before.
- The two Plateaus Normal badges already exist: `table_top` (clear Plateaus Normal 1) and `canyon_explorer` (clear Plateaus Normal 3).

## 2. New decisions (D-073)

1. **N1 cable cars:** horizontal moving platforms (48 px, 40 px/s, top 0) drawn as cable cars, the Plains Normal market-boat way (theme `PlatformSprite` and `PlatformLandSound`). The first car crosses a safe pool.
2. **N5 salt crust and llamas:** salt crust uses the ice rule with a white salt top tile (theme `IceTop`), like the Plains Hard frosty ground. Llama herds use the herd rule (a `Bat` on the ground, push 24 px, never hurts), drawn as llamas with a soft hum (theme `BatFrames`, `BatSound`).
3. **Themes and goals:** N1 TableTop "Cable Station", N2 MesaValley "Trading Post", N3 CanyonRim "Canyon Lookout", N4 TepuiTop "Explorer Camp", N5 SaltFlat "Salt Hotel". Each has its own six-layer background.
4. **Sizes and audio:** the Normal sizes above; ambience `amb_canyon_normal` for all five (already the Plateaus Normal table entry); new effects `sfx_cable_car` and `sfx_llama`.
5. **Kinds:** one new Plateaus-only kind, **ulexite**; shared kinds gain `Episode.Plateaus`: sandstone, quartzite, granite, shale, siltstone, conglomerate, limestone, schist, quartz, gneiss, halite, gypsum, ignimbrite. The Plateaus shelf grows from 10 to 24 kinds; the total from 63 to 64.

## 3. Levels

| Level | Place (banner) | Topic | Theme, goal | Objects | Kinds |
|---|---|---|---|---|---|
| N1 | Table Mountain, South Africa ("Table Mountain") | Flat tops | TableTop, Cable Station | 4 cable cars, the first over a safe pool | sandstone, quartzite, granite, shale |
| N2 | Monument Valley, USA ("Monument Valley") | Mesas and buttes | MesaValley, Trading Post | soft sand, 2 crumbling bridges, 2 updrafts | siltstone, conglomerate; sandstone and hematite reviews |
| N3 | Grand Canyon, Colorado Plateau ("Grand Canyon") | Rivers cut canyons; rock layers | CanyonRim, Canyon Lookout | 4 river currents (first pushes right), water pits | limestone, schist; granite and sandstone reviews |
| N4 | Mount Roraima, Venezuela ("Mount Roraima") | Table-top mountains | TepuiTop, Explorer Camp | 2 floats in summit pools, 2 updrafts | quartz, gneiss; quartzite and sandstone reviews |
| N5 | Salar de Uyuni, Bolivia ("Salar de Uyuni") | A high plateau and a salt flat | SaltFlat, Salt Hotel | 3 salt-crust stretches, 3 llama herds | halite, gypsum, ignimbrite, ulexite; andesite review |

Exact numbers: `layouts.md`. Bot: 83.8–98.4 s at walk 84; every pickup reachable; lint clean.

## 4. Code changes

| Where | Change |
|---|---|
| `Rules/LevelLayout.cs` `LevelTheme` | Append `TableTop`, `MesaValley`, `CanyonRim`, `TepuiTop`, `SaltFlat` at the **end**. |
| `Rules/LevelData.cs` `ItemCatalog` | Append `ulexite` at the **end** of `All` (Plateaus only, mineral, `IsDraft`). Add `Episode.Plateaus` to the 13 shared kinds in section 2 (keep their landforms and comments). |
| `Rules/LevelData.cs` `LevelCatalog` | Add Plateaus Normal 1–5 exactly as `layouts.md`; N3 and N4 pits are water pits, and so is the N1 safe pool; text from section 6. |
| `Editor/LevelThemes.cs` `ThemeArt` | One new optional field, `PlatformArtAbove` (int, default 0): rows of platform art above the standing top. Placement becomes centre y = `Top − spriteHeight / 2 + PlatformArtAbove`, so every old platform (0) is unchanged. Plus a cable line: a new optional field `PlatformCable` (string, null = none) naming a tileable strip drawn from `X0 − 16` to `X1 + 64` with its middle at `Top + PlatformArtAbove − 1`, sorted behind the car. Five theme entries (section 8). |
| `Editor/LevelObjects.cs` moving platform build | Use `PlatformArtAbove` and `PlatformCable` as above. |
| Herd and ice | No code change: the SaltFlat theme sets `BatFrames = "llama"`, `BatFrameCount = 3`, `BatSound = "sfx_llama.wav"`, the art and sound scripts, `BatArtLift = 2`, `BatArtFacesLeft = false` (the llama frames face right), `IceTop = "salt_crust_top"` and its script. Old herds, bats and ice must look exactly as before. |
| Tests | Update tests that expect a Plateaus shelf of 10 or a total of 63; add `PlateausNormalLevelsTests.cs` like `PlainsNormalLevelsTests.cs`; test that `PlatformArtAbove = 0` keeps the old placement and that every old theme keeps its defaults. |

## 5. Objects (no new rules)

- **Cable car:** `cable_car` is 48×48; the cabin roof (the standing top) is row 24 from the top, so `PlatformArtAbove = 24`; a hanger arm rises from the roof to the top of the sprite. `cable_line` (16×3) tiles along the car's path. Landing plays `sfx_cable_car.wav` through `PlaySfx` at 0.6, once per landing (the market-boat rule).
- **Salt crust:** the ice rule and numbers; top tile `salt_crust_top`.
- **Llama herd:** the herd rule and numbers; frames `llama_00`–`_02` (32×16, facing right); touch sound `sfx_llama.wav`.
- **Updrafts, crumbling bridges, soft sand, currents, floats:** exactly as before.

## 6. Content (drafts, Q-04: needs teacher review)

### Lessons (4 sentences, each at most 44 characters)

**N1 "Table Tops"** (`lesson_table_mountain`): "Table Mountain has a long flat top." / "It stands above Cape Town by the sea." / "Its top is hard sandstone." / "A cloud called the tablecloth covers it."

**N2 "Mesas and Buttes"** (`lesson_monument_valley`): "Wind and water wear plateaus down." / "A smaller flat hill is a mesa." / "A thin tall one is called a butte." / "Monument Valley has many of both."

**N3 "Canyons"** (`lesson_grand_canyon`): "Rivers can cut deep into plateaus." / "The Colorado River made the Grand Canyon." / "Its walls show layers of old rock." / "The oldest layers lie at the bottom."

**N4 "Table-top Mountains"** (`lesson_roraima`): "A tepui is a table-top mountain." / "Mount Roraima has cliffs on all sides." / "Its top is often wet with rain and mist." / "Its old rocks are quartzite and sandstone."

**N5 "High Plateaus"** (`lesson_uyuni`): "The Altiplano is a very high plateau." / "Salar de Uyuni is a huge salt flat." / "It is the biggest salt flat on Earth." / "After rain it mirrors the sky."

### Reviews (3 sentences)

- **N1:** "Table Mountain has a flat sandstone top." / "Quartzite is sandstone made very hard." / "Granite lies deep under the mountain."
- **N2:** "Mesas and buttes are pieces of old plateaus." / "Sandstone and siltstone make their red walls." / "Hematite gives the rocks their red colour."
- **N3:** "The Colorado River cut the Grand Canyon." / "Limestone and sandstone make its upper walls." / "Schist and granite lie at the very bottom."
- **N4:** "Tepuis are table-top mountains." / "Roraima is made of quartzite and sandstone." / "Quartz crystals can be found on its top."
- **N5:** "The Altiplano is a high, flat plateau." / "Drying lakes left halite and gypsum." / "Ignimbrite comes from hot volcanic ash."

### New kind

| Id | Name | Rock? | Description | Picture |
|---|---|---|---|---|
| `ulexite` | Ulexite | mineral | A soft white mineral made of thin fibres. Light passes through it, so it is called TV rock. | `ulexite` |

### Sources and checks (see `kinds-research.md`)

The research (2026-11-24) used Wikipedia and survey summaries. **Not confirmed (teacher check):** hematite at Monument Valley (sources say "iron oxide"); quartzite and quartz crystals at Roraima; gypsum, ulexite and ignimbrite at Uyuni (search summaries only); basalt was not used at the Grand Canyon; the Table Mountain "tablecloth" cloud; "biggest salt flat on Earth"; "after rain it mirrors the sky"; elevations differ by source.

## 7. Banners and goals

N1 "Table Mountain" ("Cable Station"), N2 "Monument Valley" ("Trading Post"), N3 "Grand Canyon" ("Canyon Lookout"), N4 "Mount Roraima" ("Explorer Camp"), N5 "Salar de Uyuni" ("Salt Hotel").

## 8. Assets (made by Claude; do not edit)

- `python Tools/Art/make_plateaus_normal_art.py`: theme tiles, boulders and goals for `tabletop`, `mesa`, `canyon`, `tepui`, `saltflat` (goals `cable_station`, `trading_post`, `canyon_lookout`, `explorer_camp`, `salt_hotel`, 56×48); `cable_car` (48×48), `cable_line` (16×3), `salt_crust_top` (16×8), `llama_00`–`_02` (32×16); props `fynbos_00/01`, `dassie_00/01`, `sagebrush`, `pinyon_pine`, `raven_00/01`, `pitcher_plant_tepui` (the name `pitcher_plant` belongs to Mountains Hard), `tepui_mist_00/01`, `flamingo_00/01`, `salt_cone`; item `ulexite`; lesson pictures `lesson_table_mountain`, `lesson_monument_valley`, `lesson_grand_canyon`, `lesson_roraima`, `lesson_uyuni`. Preview `.scratch/plateaus-normal/art-preview.png`.
- `python Tools/Art/make_plateaus_normal_backdrops.py`: `Backdrop/TableTop/`, `MesaValley/`, `CanyonRim/`, `TepuiTop/`, `SaltFlat/`, each L0–L5. Preview `.scratch/plateaus-normal/backdrop-preview.png`.
- `python Tools/Audio/make_plateaus_normal_audio.py`: `sfx_cable_car.wav`, `sfx_llama.wav`. Preview `.scratch/plateaus-normal/audio-preview.png`.

### Theme table

| Theme | GroundTop / Soil / Ledge / BoulderEnding | Goal (name, sprite, offset) | Backdrop | Extra |
|---|---|---|---|---|
| TableTop | `tabletop_top` / `tabletop_soil` / `ledge_tabletop` / `_tabletop` | "Cable Station", `cable_station`, (24, 24) | `Backdrop/TableTop/` | `PlatformSprite = "cable_car"`, `PlatformArtAbove = 24`, `PlatformCable = "cable_line"`, `PlatformLandSound = "sfx_cable_car.wav"` |
| MesaValley | `mesa_top` / `mesa_soil` / `ledge_mesa` / `_mesa` | "Trading Post", `trading_post`, (24, 24) | `Backdrop/MesaValley/` | — |
| CanyonRim | `canyon_top` / `canyon_soil` / `ledge_canyon` / `_canyon` | "Canyon Lookout", `canyon_lookout`, (24, 24) | `Backdrop/CanyonRim/` | — |
| TepuiTop | `tepui_top` / `tepui_soil` / `ledge_tepui` / `_tepui` | "Explorer Camp", `explorer_camp`, (24, 24) | `Backdrop/TepuiTop/` | — |
| SaltFlat | `saltflat_top` / `saltflat_soil` / `ledge_saltflat` / `_saltflat` | "Salt Hotel", `salt_hotel`, (24, 24) | `Backdrop/SaltFlat/` | llama herd and salt crust fields (section 4) |

Check each goal in the Editor; a lower offset is fine when the sprite has clear rows at its base; report it.

### Living props (decoration only; keep them off pits, water, ledges, boulders, updraft columns, floats and cable-car paths with the existing prop-fit helpers; report any prop moved more than 64 px)

- **TableTop:** fynbos (`fynbos_00/01`, 1.2–1.8 frames per second) about every 120 px from x = 100, order 7, centre y = 6; dassies (`dassie_00/01`, swap every 1.5–2.5 s) at x = 1700, 4600 and 6500, order 4, centre y = 4.
- **MesaValley:** sagebrush about every 180 px from x = 140, order 7, centre y = 5; ravens (`raven_00/01`, swap every 0.4 s) at x = 1500 and 4000, centre y = 150, order 2.
- **CanyonRim:** a pinyon pine about every 400 px from x = 300, order 3, centre y = 12; ravens at x = 2000 and 5200 as above.
- **TepuiTop:** a pitcher plant (`pitcher_plant_tepui`) about every 200 px from x = 150, order 7, centre y = 6; tepui mist (`tepui_mist_00/01`, 2 frames per second) at x = 1500, 3500 and 6000, order 3, centre y = 14.
- **SaltFlat:** a salt cone about every 300 px from x = 200, order 3, centre y = 4; flamingos (`flamingo_00/01`, swap every 1.5–2.5 s) at x = 2500, 4400 and 6200, order 4, centre y = 8. Never on salt crust within 8 px of a llama path.

## 9. Not in scope

Plateaus Hard 1–7 and the ending screen; any change to older levels; new badges (none are needed).
