# Plains Normal 1–5 (D-070)

Planned on 2026-11-22 in a grilling session (one round, four questions). The user accepted every recommendation. It follows the D-069 outline. Plains Easy is built, reviewed and played (`f0b8ff6`, "all passed").

**Build split (same as Plains Easy):** Claude made the art, backgrounds, sound and bot-checked layouts. ChatGPT/Codex writes the C# only. Claude reviews the build, then the user plays the checklist.

## 1. Rules that are already settled (do not change)

- **Normal sizes** (as Mountains and Hills Normal): width 7200, start (40, 0), goal 7040, checkpoints 2400 and 4800, 180 s, 7 pickups with exactly one secret on a high ledge (y ≥ 72).
- Pits send the explorer back to the furthest flag reached, or to the start before the first flag.
- Level differences are data (D-061). Rules have no Unity types and are tested. Every theme has its own six-layer background (D-063).
- **Walk speed:** 84 px/s everywhere.
- **Shared kinds:** one kind can sit on several shelves; it is counted once (D-066).
- **Drafts:** all lesson, review and kind text is a draft for teacher review (Q-04).
- **Badges:** the Plains badges already exist (D-069). Philippine Plains (clear Normal 1–4) and Delta Boater (clear Normal 5) become earnable now; no new badges.
- **Ambience:** every Plains Normal level uses the table entry `amb_wetland_normal` (frogs and water birds). No level override.
- **No regressions:** Mountains, Hills and Plains Easy must play exactly as before.

## 2. New decisions (D-070)

1. **Places (the D-069 outline):** N1 Candaba Swamp, N2 Pinatubo lahar plain, N3 Agno River delta, N4 Agusan Marsh, N5 Mekong Delta (Vietnam).
2. **Objects reuse existing rules; only the art is new:**
   - N1 sinking floats over water pits, with the practice float over a safe pool (`SinkingFloat`, `ShallowPool`)
   - N2 soft lahar sand (`SandZone`) and two crumbling-ledge bridges over pits (`CrumbleLedge`)
   - N3 river currents (`CurrentZone`); the first one pushes with the player
   - N4 one float, one sand patch and one current
   - N5 four **market boats**: horizontal `MovingPlatform` (48 px, 40 px/s) with boat art. The first boat crosses a safe pool.
3. **Market boat look and sound:**
   - Each theme may name its own moving-platform sprite and a landing sound (data on the theme, default: the old `platform` sprite and no sound). FloatingMarket uses `market_boat` and `sfx_boat_bump`.
   - Every other theme keeps `platform` with no sound, so Mountains is unchanged.
4. **Kinds** (only kinds a source ties to the place):
   - **New, Plains only:** `peat` (N1, N4) and `laterite` (N5).
   - **Shared kinds that join the Plains shelf:** `pumice`, `dacite`, `andesite` (N2) and `gold` (N3).
   - **Reviews:** clay, siltstone, mudstone and quartz.
   - The Plains shelf goes from 10 to **16** kinds; the total goes from 54 to **56**.
   - Peat is not a true rock. It is stored as a rock (`IsRock` true) so it shows on the shelf, and its card says it is not a true rock yet (teacher check, Q-04).
5. **Water pits:** N1, N3, N4 and N5 pits are water (`IsWater`). N2 pits are dry (grey lahar gaps under crumbling bridges).

## 3. Level outline

| Level | Banner | Theme | Goal | Objects | Pits / spikes | Kinds (review) | Bot time |
|---|---|---|---|---|---|---|---|
| N1 | Candaba Swamp | CandabaWetland | Bird Hide | 4 floats (practice over a pool) | 3 water / 2 | **peat**; (clay, siltstone, mudstone) | 83.8 s |
| N2 | Pinatubo Lahar Plain | LaharPlain | Lahar Marker | 4 sand zones, 2 crumbling bridges | 3 dry / 3 | pumice, dacite, andesite (shared, new to Plains) | 93.0 s |
| N3 | Agno River Delta | RiverDelta | Fish Pen Hut | 4 currents (2 against) | 3 water / 3 | gold (shared, new to Plains); (quartz, siltstone, mudstone) | 85.8 s |
| N4 | Agusan Marsh | FloatingVillage | Floating House | 1 float, 1 sand, 1 current | 3 water / 3 | (peat, clay, mudstone) | 85.2 s |
| N5 | Mekong Delta | FloatingMarket | Market Pier | 4 market boats (first over a pool) | 3 water / 3 | **laterite**; (clay, siltstone, mudstone) | 98.4 s |

The exact numbers are in `layouts.md`. The Hills Normal bot times are 83.8–91.1 s at the same walk speed.

## 4. Code changes

| Where | Change |
|---|---|
| `Rules/LevelLayout.cs` `LevelTheme` | Append `CandabaWetland`, `LaharPlain`, `RiverDelta`, `FloatingVillage`, `FloatingMarket` at the **end**. |
| `Rules/LevelData.cs` `ItemCatalog` | Append `peat` and `laterite` at the **end** of `All` (Plains only, `IsDraft`). Add `Episode.Plains` to pumice, dacite, andesite and gold (keep their existing landforms). |
| `Rules/LevelData.cs` `LevelCatalog` | Plains Normal 1–5 exactly as `layouts.md`. Pits marked water as in section 2. |
| `Editor/LevelThemes.cs` `ThemeArt` | Two optional fields: `PlatformSprite` (null means `platform`) and `PlatformLandSound` (null means none). Five theme entries (section 8). |
| `Editor/LevelObjects.cs` moving platform build | Use the theme's `PlatformSprite`. Place the art so its **top row is the platform top**: the centre y is `Top − spriteHeight / 2` (for the 10 px `platform`, that is the current `Top − 5`, so old levels do not move). Pass the landing sound to the view. |
| `Play/MovingPlatformView.cs` | If it has a landing sound, play it through `LevelController.PlaySfx` (volume 0.6) once each time the explorer lands on it (not every frame). No sound when the sound is empty. |
| Living props | The section 8 props through the trail prop helpers (`RepeatTrailProp`, `TrailPropFits`). Report any prop moved more than 64 px. |
| Missing-asset errors | Name `Tools/Art/make_plains_normal_art.py`, `Tools/Art/make_plains_normal_backdrops.py` or `Tools/Audio/make_plains_normal_audio.py`. |
| Tests | New `PlainsNormalLevelsTests.cs`. Update the shelf and total counts (16 and 56) and any test that says Plains Normal is "Coming soon". |

Already generic, no change expected: floats, pools, sand, crumbles, currents, moving platforms, the Normal "Coming soon" rule, Next to the next level, badges, Plains life on the map and menus, and the Description Screen (it needs Plains Hard).

## 5. Object rules (unchanged; for reference)

| Object | Numbers |
|---|---|
| Sinking float | 48 px; waits 0.6 s, sinks 8 px/s, holds nothing at 16 px down, rises 1.5 s after the explorer leaves at 16 px/s |
| Shallow pool | 16 px deep with a floor; not a hazard |
| Soft sand | walking at 60 %; jumps unchanged |
| Crumbling ledge | 32 px; breaks 0.5 s after a touch, back after 3 s |
| River current | 30 px/s on the ground in the strip, in its direction |
| Moving platform (boat) | 48 px wide, 40 px/s between X0 and X1 at top 0 over its pit or pool |

## 6. Content (drafts, Q-04: needs teacher review)

### Lessons (4 sentences, each at most 44 characters)

**N1 "Wetland Plains"** (picture `lesson_candaba`):
1. Some plains are wet and swampy.
2. Candaba Swamp floods in the wet season.
3. In the dry season, farmers plant there.
4. Wild ducks rest there on long trips.

**N2 "Volcanic Plains"** (picture `lesson_pinatubo_lahar`; `lesson_pinatubo` is the Mountains Normal picture):
1. Volcanoes can bury and build plains.
2. Mount Pinatubo erupted in 1991.
3. Lahars are rivers of volcanic mud.
4. Lahar mud covered towns and farms.

**N3 "Deltas"** (picture `lesson_agno`):
1. A delta forms where a river meets the sea.
2. The river drops its sand and mud there.
3. The Agno built the Pangasinan Plain.
4. People pan for gold along the Agno.

**N4 "Living on Wetlands"** (picture `lesson_agusan`):
1. Agusan Marsh is a huge wetland in Mindanao.
2. Some Manobo families live in floating homes.
3. The marsh is a protected wetland.
4. It was home to Lolong, a big crocodile.

**N5 "River Deltas of the World"** (picture `lesson_mekong`):
1. The Mekong Delta is in southern Vietnam.
2. It is flat land built from river mud.
3. People sell fruit from boats there.
4. Laterite is red rock of hot, wet lands.

### Reviews (3 sentences, shown on the Result Screen)

- **N1:** "Wetlands are plains that hold water." / "Dead plants pile up in wetlands as peat." / "Over a long time, peat can turn into coal."
- **N2:** "Lahars carry volcanic sand and rock." / "Pumice is a light rock full of holes." / "Pinatubo's rocks include andesite and dacite."
- **N3:** "Deltas are new land built by rivers." / "Gold can be panned from river sand." / "Quartz sand and silt fill the delta."
- **N4:** "Wetland soil is soft and full of old plants." / "Peat, clay and mud fill the marsh." / "Protecting wetlands keeps animals safe."
- **N5:** "A delta is flat, low land at a river's mouth." / "River mud makes deltas good for farming." / "Laterite is red because of its iron."

### New kinds (Plains only)

| Id | Name | Rock? | Description | Picture |
|---|---|---|---|---|
| `peat` | Peat | rock (see note) | Soft brown matter made of old, wet plants. It is not a true rock yet, but in time it can become coal. | `peat` |
| `laterite` | Laterite | rock | A red, iron-rich rock or soil of hot, wet places. People cut it into building blocks. | `laterite` |

Shared kinds keep their existing text and pictures.

### Banners and goals

N1 "Candaba Swamp" (Bird Hide), N2 "Pinatubo Lahar Plain" (Lahar Marker), N3 "Agno River Delta" (Fish Pen Hut), N4 "Agusan Marsh" (Floating House), N5 "Mekong Delta" (Market Pier).

### Fact sources

All are from Wikipedia, checked 2026-11-22; details are in `kinds-research.md`.

- **Not confirmed:**
  - Pinatubo's rocks as andesite and dacite (from a page summary only; the USGS pages could not be reached).
  - That the Candaba sediment is peat (the town page says "humus and decaying plant residues").
  - That Agusan has peat (the page says only "swamp forests").
- The teacher should check these and every draft sentence.

## 7. Layout data

See `layouts.md`; ChatGPT must copy the numbers exactly.

## 8. Assets (made by Claude; do not edit)

- `python Tools/Art/make_plains_normal_art.py` makes:
  - **Tiles and goals:** `<x>_top`, `<x>_soil`, `ledge_<x>` and `boulder_small_<x>` / `boulder_big_<x>` for `candaba`, `lahar`, `delta`, `agusan` and `mekong`. Goals `bird_hide`, `fish_pen_hut`, `floating_house` and `market_pier` (56×48), and `lahar_marker` (40×48).
  - **The boat:** `market_boat` (48×16, deck on row 0).
  - **Props:** `duck_00/01`, `water_lily`, `lahar_sign`, `reed_tuft`, `fish_stakes`, `bangka_plains`, `water_hyacinth`, `crocodile`, `fruit_crate`.
  - **Items:** `peat`, `laterite`.
  - **Lesson pictures:** `lesson_candaba`, `lesson_pinatubo_lahar`, `lesson_agno`, `lesson_agusan`, `lesson_mekong`.
  - Preview: `.scratch/plains-normal/art-preview.png`.
- `python Tools/Art/make_plains_normal_backdrops.py` makes `Backdrop/<Theme>/L0–L5` for the five themes. Preview: `.scratch/plains-normal/backdrop-preview.png`.
- `python Tools/Audio/make_plains_normal_audio.py` makes `sfx_boat_bump.wav` (0.30 s, about −26 dB).
- Reused, already made: `water_fill` and `current_top` (Hills Normal); the `float` raft art; the crumble frames; `soft_top` and the sand puff (Hills Easy); `Props/egret_00/01` and `Props/palm_tree`.

### Theme table for `LevelThemes.cs`

| Theme | GroundTop / Soil / Ledge / BoulderEnding | Goal (name, sprite, offset) | Platform sprite, landing sound |
|---|---|---|---|
| CandabaWetland | `candaba_top` / `candaba_soil` / `ledge_candaba` / `_candaba` | "Bird Hide", `bird_hide`, (24, 24) | default |
| LaharPlain | `lahar_top` / `lahar_soil` / `ledge_lahar` / `_lahar` | "Lahar Marker", `lahar_marker`, (8, 24) | default |
| RiverDelta | `delta_top` / `delta_soil` / `ledge_delta` / `_delta` | "Fish Pen Hut", `fish_pen_hut`, (24, 24) | default |
| FloatingVillage | `agusan_top` / `agusan_soil` / `ledge_agusan` / `_agusan` | "Floating House", `floating_house`, (24, 24) | default |
| FloatingMarket | `mekong_top` / `mekong_soil` / `ledge_mekong` / `_mekong` | "Market Pier", `market_pier`, (24, 24) | `market_boat`, `sfx_boat_bump` |

Each theme's backdrop is `Backdrop/<Theme>/`. Check each goal in the Editor; if the sprite has clear rows at its base, lower it as was done for the Farm Shed (24, 22), and report it.

### Living props (decoration only; the trail prop fit test keeps them off pits, pools, ledges, boulders, crumbles, platforms and zones)

- **CandabaWetland:**
  - reed tufts every 260 px from x 120 (order 3, y 7)
  - ducks (`duck_00/01`, a swap every 1.5–2.5 s) at 1200, 3000 and 5500 (order 4, y 5)
  - egrets (`egret_00/01`) at 2200 and 4600 (order 4, y 5)
- **LaharPlain:**
  - lahar signs at 450, 3000 and 5900 (order 3, y 10)
  - reed tufts every 400 px from x 200 (order 7, y 7)
- **RiverDelta:**
  - fish stakes at 900, 2500, 4300 and 6100 (order 2, y 8)
  - bangka (`Props/bangka_plains`; `Props/bangka` is the Hills Normal boat) at 1800 and 5000 (order 3, y 6)
  - egrets at 3300 (order 4, y 5)
- **FloatingVillage:**
  - water hyacinth every 300 px from x 160 (order 7, y 4)
  - crocodile at 2300 and 5400 (order 2, y 4)
  - reed tufts every 500 px from x 400 (order 3, y 7)
- **FloatingMarket:**
  - fruit crates at 600, 2900, 4700 and 6500 (order 4, y 5)
  - palm trees every 700 px from x 350 (order 3, y 20)

## 9. Not in scope

- Plains Hard 1–7, the Plains Description Screen flow, and Plateaus.
- Any change to Mountains, Hills or Plains Easy numbers or text.
- Refactoring the `LevelLayout` constructor.
