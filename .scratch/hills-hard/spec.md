# Hills Hard 1–7 (D-068)

This spec builds **Hills Hard 1–7**, the last part of Hills. When all 15 Hills levels are cleared, Plains opens.

- **Claude made, ready to use:** the art, the backgrounds, the sounds and the bot-checked layouts.
- **ChatGPT (Codex) writes:** the C# rules, the play parts, the builders and the tests.

Hills Easy and Normal are built (D-066, D-067). Before this build, run 360 EditMode tests.

## 1. Rules that are already settled (do not change)

- **Hard sizes (Mountains Hard, D-065):**
  - 9600 px wide, 3 checkpoints, 9 pickups (one secret on a high ledge) and 240 s.
  - Pits and spikes send the explorer back to the furthest flag reached.
- **Walking:** 84 px/s, the same as every level. Do not add a separate ground speed.
- **Structure (D-061):**
  - One script runs every level, and every difference between levels is data in `LevelCatalog`.
  - Rules are plain C# with NUnit tests.
  - New jobs on the level screen are level parts that read `LevelClock.Now`.
  - Sounds go through `LevelController.PlaySfx`.
- **Badges, already built:**
  - Slope Ranger: clear any Hills Hard level.
  - World Hills: clear Hills Hard 5–7, using `LastThreeHardLevels`.
  - Steady Star: 3 stars on every Hills Hard level.
  - Hill Master: every Hills level.
  - Hill Collector follows the whole Hills shelf, which becomes 23 kinds.
  - No new badges.
- **Unlocking Plains:** when Hills is complete, Plains unlocks the generic way: the one-time map animation, then "Coming soon".
- **Content:** all text is a draft for teacher review (Q-04).

## 2. New decisions (D-068)

1. **Slippery mud:** exactly the icy-ground rule (`IceSlide`, the same numbers), applied in `MudZone` stretches. Only the art and the sound differ:
   - The ground top is swapped to `mud_top`.
   - `mud_splash_00/01` shows while sliding.
   - `sfx_mud_squelch` plays when a slide starts, the way `sfx_ice_slide` does on ice.
2. **Rolling log:** the rolling-boulder rule with a direction (+1 right, −1 left).
   - **Art:** `log_00–03` frames, with the `log_warn_00/01` dust warning.
   - **Sound:** `sfx_log_roll`.
   - **Right-rolling logs** stay at 90 px/s. They only catch a player who stops, turns back or is slowed: a "don't stop on the slope" lesson.
3. **Where each object goes:**

| Level | Objects |
|---|---|
| H1 | mud only, with a safe first stretch |
| H2 | logs only, with a safe first log |
| H3 | mud, bats and crumbling ledges |
| H4 | wind, logs and balloons |
| H5 | mud, current and logs |
| H6 | logs, horizontal moving platforms and floats |
| H7 | everything (the finale) |

4. **Ambience set for one level:**
   - H4 uses `amb_wind_turbines`.
   - H6 uses `amb_city_hills`.
   - The other levels use `amb_rain_hard` from the table.
5. **H2 safety lesson:** it teaches the warning signs of a landslide and what to do, and names no disaster. Antipolo was chosen because a 2018 UP Diliman study rates about 40% of the city as possibly at risk of landslides.
6. **Kinds:**
   - Six new Hills-only kinds: peridotite, chlorite, dacite, quartzite, travertine and leucite. The Hills shelf grows from 17 to 23.
   - H3 and H7 use reviews only.
   - Review kinds are not added to the Hills shelf. This is the same rule as tuff and andesite in Normal.

## 3. Level outline

| Level | Banner | Theme | Goal | Objects | New kinds | Reviews |
|---|---|---|---|---|---|---|
| H1 | Tarsier Forest | TarsierForest | Tarsier Lookout | mud | peridotite | limestone |
| H2 | Antipolo Slopes | RainySlope | Barangay Hall | logs | chlorite | basalt |
| H3 | Masungi Georeserve | KarstRidge | Spider-Web Bridge | mud, bats, crumbling ledges | — | limestone, calcite |
| H4 | Pililla Wind Farm | WindHills | Turbine Base | wind, logs, balloons | dacite | andesite |
| H5 | Clew Bay | DrumlinBay | Stone Pier | mud, current, logs | quartzite | schist |
| H6 | Seven Hills of Rome | CityHills | Temple Ruins | logs, platforms, floats | travertine, leucite | tuff |
| H7 | Guilin Karst | KarstTowers | Raft Dock | everything | — | limestone, calcite |

**Pickup kinds:** the `k1`–`k9` placeholders in `layouts.md` take these kinds, in order. **S** marks the secret pickup.

| Level | k1 | k2 | k3 | k4 | k5 | k6 | k7 | k8 | k9 |
|---|---|---|---|---|---|---|---|---|---|
| H1 | limestone | peridotite | limestone | peridotite | limestone | limestone | **peridotite (S)** | limestone | peridotite |
| H2 | basalt | chlorite | basalt | chlorite | **chlorite (S)** | basalt | basalt | chlorite | basalt |
| H3 | limestone | calcite | limestone | calcite | limestone | calcite | **calcite (S)** | limestone | calcite |
| H4 | andesite | dacite | andesite | dacite | andesite | dacite | **dacite (S)** | andesite | andesite |
| H5 | schist | quartzite | schist | quartzite | schist | quartzite | schist | **quartzite (S)** | schist |
| H6 | tuff | travertine | leucite | tuff | travertine | **leucite (S)** | tuff | travertine | leucite |
| H7 | limestone | calcite | limestone | calcite | limestone | calcite | **calcite (S)** | limestone | calcite |

## 4. Code changes

| Where | Change |
|---|---|
| `Rules/MudZone.cs` (new) | X and Width with `Contains`, like `IceZone`. Footing uses `ZoneFooting`; the slide uses `IceSlide`. |
| `Rules/RollingLog.cs` (new) | Spawn, End, Direction (±1) and Offset. It uses the `RollingBoulder` timing (cycle, warning, speed, size, skip distance), with x = Spawn + Direction × elapsed × Speed, and it is gone when it passes End. Do not change `RollingBoulder`: Mountains boulders must be identical (test). |
| `Rules/LevelLayout.cs` | Optional `MudZones` and `Logs`, added at the end of the constructor. Existing callers compile unchanged. |
| `Rules/LevelData.cs` | The six kinds, appended at the end of `ItemCatalog.All` (Hills only, `IsDraft`). Hills Hard 1–7 exactly as in `layouts.md`, with section 3's kinds and section 7's text. `AmbienceOverride` for H4 and H6. |
| `Rules/LevelLayout.cs` enum | Seven themes **at the end**: TarsierForest, RainySlope, KarstRidge, WindHills, DrumlinBay, CityHills and KarstTowers. |
| `Rules/LandformAudio.cs` | The script-name table maps `amb_wind_turbines` and `amb_city_hills` to `make_hills_hard_audio.py`. |
| `Play/` | **Mud:** extend the ice part, or add a `MudWatcher` part, so the motor's ice slide is on over ice **or** mud. Mud plays its own squelch and splash, with no per-theme `if`: a mud zone carries its own look. **Logs:** a `LogRoller` part (like the rolling-boulder part) that draws, warns, rolls in its direction and plays `sfx_log_roll`. A touch is a trap, like the boulder. |
| `Editor/` builders | `mud_top` over mud zones (swapped like `soft_top`), the logs, the seven themes and props (section 8), and the ambience from the level. Missing-asset errors name `make_hills_hard_art.py`, `make_hills_hard_backdrops.py` or `make_hills_hard_audio.py`. |
| Tests | Mud equals ice. Logs in both directions, the skip distance, and Mountains boulders unchanged. Hills Hard data tests (the same checks as `HillsNormalLevelsTests`). The shelf is 23. H4/H6 ambience. After Hills Normal 5, Next opens Hills Hard 1. After Hills Hard 7, Next Episode opens the Hills Description Screen and Plains unlocks. |

## 5. Object rules

See `layouts.md`, the "Rule constants" table. The mud uses the ice numbers. The log uses the boulder numbers plus a direction, with a maximum run of 243 px.

## 6. Level data

See `layouts.md`, H1–H7: the exact positions and the bot results. The bot files are `.scratch/hills-hard/bot-check-*.cs.txt`. `LevelCatalog` must match `layouts.md` exactly.

## 7. Content (drafts, Q-04)

### Lessons (4 sentences, each at most 44 characters)

- **H1** (`lesson_tarsier_forest`):
  - Hills give homes to many animals.
  - Tarsiers live in Bohol's forest hills.
  - Trees on hills give them food and shade.
  - Keep forests safe so animals have homes.
- **H2** (`lesson_antipolo_slope`):
  - Heavy rain can loosen soil on slopes.
  - Watch for new cracks in the ground.
  - Tilting trees or posts are a warning.
  - Move away and tell an adult.
- **H3** (`lesson_masungi`):
  - Masungi has sharp limestone hills.
  - Rain slowly dissolves the limestone.
  - People made trails to protect it.
  - Stay on trails to keep hills safe.
- **H4** (`lesson_pililla`):
  - Hilltops are often very windy.
  - Pililla in Rizal has 27 wind turbines.
  - Wind turns them to make electricity.
  - Wind power is clean and never runs out.
- **H5** (`lesson_clew_bay`):
  - Long ago, ice sheets covered Ireland.
  - Moving ice left small rounded hills.
  - These hills are called drumlins.
  - Clew Bay has about a hundred of them.
- **H6** (`lesson_rome_hills`):
  - Rome was built on seven hills.
  - Hills were easy to defend long ago.
  - Romans built with travertine and tuff.
  - Many old buildings still stand today.
- **H7** (`lesson_guilin`):
  - Guilin in China has tower-shaped hills.
  - They are made of thick limestone.
  - Rain and rivers carved them over time.
  - They are part of a UNESCO karst site.

### Reviews (3 sentences)

- **H1:** "Hills are homes for animals like tarsiers." / "Forests on hills give food and shelter." / "Peridotite is a dark green rock from deep inside the Earth."
- **H2:** "Cracks, tilting trees and muddy springs can warn of a landslide." / "Move away from the slope and tell an adult." / "Chlorite is a soft green mineral found in old basalt."
- **H3:** "Rain slowly dissolves limestone into sharp shapes." / "Staying on trails protects the hills." / "Calcite is the mineral that makes limestone."
- **H4:** "Windy hills are good places for wind turbines." / "Wind power makes clean electricity." / "Dacite is a pale volcanic rock."
- **H5:** "Glaciers left rounded hills called drumlins." / "Clew Bay's islands are drumlins in the sea." / "Quartzite is a very hard rock made from sand."
- **H6:** "Rome grew on seven hills." / "Romans built with travertine from nearby springs." / "Leucite is a white mineral in some volcanic rock."
- **H7:** "Guilin's tower hills are made of limestone." / "Rain and rivers carved them over a long time." / "Limestone hills like these are called karst."

### New kinds (Hills only, `IsDraft` true)

| Id | Name | Rock or mineral | Description (at most 80 characters) | Confidence |
|---|---|---|---|---|
| `peridotite` | Peridotite | rock | A dark green rock from deep inside the Earth. It is full of olivine. | medium: found in east Bohol, not confirmed at Corella; teacher check |
| `chlorite` | Chlorite | mineral | A soft green mineral. It forms when rocks like basalt slowly change. | medium |
| `dacite` | Dacite | rock | A pale grey volcanic rock with small light crystals. | low-medium: regional; teacher check |
| `quartzite` | Quartzite | rock | A very hard rock made when sandstone is heated and squeezed. | high |
| `travertine` | Travertine | rock | A cream rock made by spring water. Romans built with it. | high |
| `leucite` | Leucite | mineral | A white mineral with round crystals, found in some volcanic rock. | medium |

Item pictures: `Assets/Art/Items/Sprites/<id>.png`, 24×24, already made.

### Start banners

Tarsier Forest, Antipolo Slopes, Masungi Georeserve, Pililla Wind Farm, Clew Bay, Seven Hills of Rome, Guilin Karst.

### Sources

`.scratch/hills-hard/kinds-research.md` holds the sources: Bohol and Rizal geology, the Antipolo landslide study, the Pililla facts (27 turbines, 54 MW, 2015), Clew Bay (about 100 islands, about 20,000 years) and UNESCO South China Karst (Guilin added in 2014).

## 8. Assets (made by Claude; do not edit, only run)

- **`python Tools/Art/make_hills_hard_art.py`** writes:
  - the 35 theme tiles: `<ending>_top/_soil`, `ledge_<ending>` and `boulder_small/big_<ending>`, for the endings `_tarsier`, `_slope`, `_karst`, `_wind`, `_drumlin`, `_city` and `_tower`
  - the mud: `mud_top` (16×8) and `mud_splash_00/01` (10×6)
  - the log: `log_00–03` (16×16) and `log_warn_00/01` (24×4)
  - the 7 goals
  - 19 `Props/` sprites and the 7 lesson pictures
  - the 6 item pictures

  Preview: `.scratch/hills-hard/art-preview.png`.
- **`python Tools/Art/make_hills_hard_backdrops.py`** writes `Backdrop/<Theme>/L0–L5` (960×270) for the 7 themes. Preview: `backdrop-preview.png`.
- **`python Tools/Audio/make_hills_hard_audio.py`** writes:
  - `amb_wind_turbines.ogg` (24 s, −20.1 dB)
  - `amb_city_hills.ogg` (24 s, −21.1 dB)
  - `sfx_mud_squelch.wav` (0.3 s)
  - `sfx_log_roll.wav` (0.8 s, a loop like `sfx_boulder_roll`)

  Preview: `audio-preview.png`.

### Theme table for `LevelThemes.cs`

| Theme | Ending | Goal (name, sprite, offset) | Backdrop |
|---|---|---|---|
| TarsierForest | `_tarsier` | "Tarsier Lookout", `tarsier_lookout`, (24, 24) | `Backdrop/TarsierForest/` |
| RainySlope | `_slope` | "Barangay Hall", `barangay_hall`, (24, 24) | `Backdrop/RainySlope/` |
| KarstRidge | `_karst` | "Spider-Web Bridge", `web_bridge`, (24, 24) | `Backdrop/KarstRidge/` |
| WindHills | `_wind` | "Turbine Base", `turbine_base`, (24, 32) | `Backdrop/WindHills/` |
| DrumlinBay | `_drumlin` | "Stone Pier", `stone_pier`, (24, 24) | `Backdrop/DrumlinBay/` |
| CityHills | `_city` | "Temple Ruins", `temple_ruins`, (24, 24) | `Backdrop/CityHills/` |
| KarstTowers | `_tower` | "Raft Dock", `raft_dock`, (24, 24) | `Backdrop/KarstTowers/` |

**Water:** the floats and current strips in H5, H6 and H7 reuse `water_fill` and `current_top` from Hills Normal.

### Living props

These are decoration only. Every one goes through `HillsPropFits`, plus mud zones and log runs, and moves to the nearest fitting spot, as in Normal. Report any prop that moved more than 64 px.

- **TarsierForest:**
  - Vine every ~260 px from x 140 (order 3, y 12).
  - Tarsier (2 frames, 2.5 s) at x 900, 4300 and 7700 (order 4, y 20).
- **RainySlope:**
  - `warning_sign` at x 600, 3400 and 6600 (order 3, y 10).
  - `cracked_ground` at x 1200 and 5200 (order 6, y 2).
  - `tilted_post` at x 2200 and 7400 (order 3, y 10).
- **KarstRidge:**
  - `karst_spike` every ~340 px from x 160 (order 3, y 8).
  - `rope_rail` at x 2600 and 5900 (order 3, y 6).
- **WindHills:**
  - `turbine_far_00/01` (2 frames, 0.5 s) at x 1000, 3900 and 7200 (order 1, y 40).
  - `wind_grass_00/01` every ~180 px from x 80 (order 7, y 5).
- **DrumlinBay:**
  - `stone_wall` every ~600 px from x 300 (order 3, y 5).
  - Sheep (2 frames) at x 1800, 4400 and 8100 (order 4, y 6).
- **CityHills:**
  - `cypress` every ~500 px from x 220 (order 3, y 16).
  - `column_broken` at x 2900 and 6900 (order 3, y 12).
- **KarstTowers:**
  - `bamboo_raft_far` at x 1300 and 6000 (order 1, y 30).
  - Cormorant (2 frames) at x 2300, 5000 and 8400 (order 4, y 10).

## 9. Not in scope

- Plains.
- New badges.
- Changes to Mountains, Hills Easy or Hills Normal.
- Any edit to the art or audio scripts or their files.
