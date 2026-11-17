# Hills Normal 1–5 (D-067)

This spec is the full build of **Hills Normal 1–5**: five levels, four new level objects, seven new kinds, five themes with their art and backgrounds, and four new sounds.

**Who does what:**
- **Codex (ChatGPT):** draws the art and makes the sounds with Python scripts, and writes the C# code.
- **Claude:** designed and bot-checked the layouts, wrote the content and reviews the build.

The Hills outline, the shared-kind rules, the badges and the audio table come from D-066 and `.scratch/hills/spec.md`. Hills Easy 1–3 was built and played on 2026-11-17 (`8d92fd5`, `f4e293b`).

## 1. Rules that are already settled (do not change)

- Episode order and unlocks (D-039, D-065). Hills Normal opens when all three Hills Easy levels are cleared. The unlock code is already generic.
- **Normal sizes (Mountains Normal, D-062):**
  - 7200 px wide, 2 checkpoints and a 180 s timer.
  - The Normal pickup count from `LevelData`.
  - Pits and spikes return the explorer to the furthest flag reached.
- Stars: 0–40% 1, 41–99% 2, 100% 3.
- One script runs every level. Every difference between levels is data in `LevelCatalog`. Rules are plain C# with NUnit tests. No per-level code (D-061).
- One music loop per landform (`bgm_hills`). Hills Normal ambience is `amb_stream_normal`, except in N2 (section 2, decision 6).
- **Badges are already built:**
  - Philippine Hills: clear Hills Normal 1–4.
  - Balloon Rider: clear Hills Normal 5.
  - No new badges.
- All content is a draft for teacher review (Q-04).
- **Art and audio:**
  - Made by Python scripts in `Tools/Art` and `Tools/Audio`, reusing the existing helper modules (`make_mainmenu_art as mm`, `make_landform_map_art as lm`, `make_level_art as la`, `make_lesson_art as ls`, `make_normal_art as na`, `make_normal_backdrops as nb`, `make_menu_music`).
  - Every script writes a preview PNG under `.scratch/hills-normal/`.
  - Backgrounds have six layers (L0–L5, each 960×270).

## 2. New decisions (D-067)

1. **N1 sinking float:**
   - A 48 px raft at the water line over a water pit.
   - **While the explorer stands on it:** it waits 0.6 s, then sinks at 8 px/s. When it is 16 px down (after 2 s of sinking), it no longer holds the explorer, and falling into the water counts as a pit.
   - **After the explorer leaves:** it rises back 1.5 s later.
   - **Warning:** it shakes and shows ripples, like the crumbling-ledge warning.
   - **Bot-check additions (`layouts.md`):**
     - A float rises at 16 px/s.
     - Floats and balloons are one-way tops: they hold from above only, like the dynamic slabs.
     - N1's practice float sits over a shallow safe **pool**: water 16 px deep with a floor, not a hazard. Draw it with `water_fill` too.
2. **N2 gentle bat:**
   - A bat flies a small ellipse loop over the path at body height.
   - **On a touch:** the explorer is pushed 24 px back and flashes, and that bat rests for 1 s.
   - **Push details:** the push is spread over 0.2 s, and walk input is ignored meanwhile. It goes away from the bat; when the explorer is straight under the bat, it goes against the walk.
   - **Never:** a return to a checkpoint, or any other penalty.
   - **Lesson line:** "Do not disturb bats in caves."
3. **N3 river current:**
   - A shallow water strip on solid ground, not a pit.
   - While the explorer is grounded in it, there is a steady push of 30 px/s, to the right or the left (data).
   - Walking against it still moves forward at 42 px/s.
   - Ripple marks show the direction.
4. **N4 Banaue mix:** one float, one bat loop and one current strip. No new object.
5. **N5 hot-air balloon:**
   - A vertical moving platform: a basket top 48 px wide that moves up and down at 40 px/s between a low and a high top.
   - It is a new option on `MovingPlatform`, so the old horizontal platforms are unchanged.
   - Only the basket top is solid.
   - One balloon is needed to cross a wide gap.
6. **Ambience set for one level:**
   - A level can name its own ambience, overriding the `LandformAudio` table entry.
   - N2 uses `amb_cave_drips`: drips and a far echo, with no birds or cicadas.
   - This is data on the level, not per-level code.
7. **Kinds:**
   - Seven new kinds: aragonite, coquina, flowstone, dripstone, volcanic breccia, ignimbrite and zeolite. They are Hills only.
   - The Hills shelf goes from 10 to 17 kinds.
   - N4 has no new kind, because no source names the bedrock at Banaue. Its pickups are clay and kaolinite reviews.
8. **First objects are safe:** the first float, bat, current and balloon in each level sit on a safe practice spot.
9. **New sounds:** `amb_cave_drips`, `sfx_splash` (the float goes under, or the explorer drops into the water), `sfx_bat_flap` (a bat push) and `sfx_burner` (a balloon starts to rise).
10. **Facts kept within their sources:**
    - No age is given for Hundred Islands.
    - The Cappadocia caps are ignimbrite, not basalt.
    - The N3 lesson does not name the gorge's rock.

## 3. Level outline

| Level | Banner | Theme | Goal | New object | Pits / spikes | New kinds | Reviews |
|---|---|---|---|---|---|---|---|
| N1 | Hundred Islands | IslandHills | Boat Dock | 4 sinking floats | 3 / 2 | aragonite, coquina | coral stone |
| N2 | Callao Cave | LimestoneCave | Cave Mouth | 3 bat loops | 3 / 2 | flowstone, dripstone | calcite |
| N3 | Pagsanjan Gorge | RiverGorge | Falls Landing | 3 current strips (one pushes the other way) | 3 / 3 | volcanic breccia | tuff, andesite |
| N4 | Banaue Rice Terraces | RiceTerraces | Ifugao House | 1 float, 1 bat loop, 1 current | 3 / 3 | none | clay, kaolinite |
| N5 | Cappadocia | FairyChimneys | Balloon Field | 4 balloons | 4 / 3 | ignimbrite, zeolite | tuff |

## 4. Code changes

| Where | Now | Change |
|---|---|---|
| `Rules/SinkingFloat.cs` (new) | none | Placement (x, top, which pit it covers), the sink rule (`TopAt(standingSeconds, leftSeconds)`) and `Holds(...)`. Constants are in section 5. |
| `Rules/Bat.cs` (new) | none | Centre, radius, period and phase; `PositionAt(seconds)`; touch test against the explorer body; push 24 px; rest 1 s. |
| `Rules/WindZone.cs` or `Rules/CurrentZone.cs` (new) | `WindZone` pushes left in gusts | Add `CurrentZone` (x, width, direction ±1, steady 30 px/s while grounded in it), using `ZoneFooting` for the grounded test. Do not change `WindZone`. |
| `Rules/MovingPlatform.cs` | horizontal only | Add a vertical option: x is fixed and the top moves between `Top0` and `Top1` at the same 40 px/s. Horizontal platforms must stay exactly the same (tests). |
| `Rules/LevelLayout.cs` | sand, ice, wind, platforms | Optional lists `Floats`, `Bats`, `Currents`, and vertical entries in the platforms. Optional `AmbienceOverride` (string, null = use the table). |
| `Rules/LandformAudio.cs` | per landform and difficulty | `AmbienceFor(LevelData)` returns the level's override when set, or else the table entry. Add `amb_cave_drips` to the script-name table (`make_hills_normal_audio.py`). |
| `Rules/LevelData.cs` | Hills Easy only | Hills Normal 1–5 data (section 6), the 7 new kinds (section 7) and 5 new `LevelTheme` values **at the end**: IslandHills, LimestoneCave, RiverGorge, RiceTerraces and FairyChimneys. |
| `Play/FloatWatcher.cs`, `Play/BatFlock.cs`, `Play/CurrentWatcher.cs` (new level parts) | none | Each has `Begin(LevelController)` and `IsBuilt` and reads `LevelClock.Now`. Floats and balloons are `DynamicSolid`s like `MovingPlatformView`. Sounds go through `LevelController.PlaySfx`. |
| `Play/ExplorerMotor.cs` | sand and ice | Adds the current push (like the wind push) and the bat push-back (a 24 px impulse, without moving the checkpoint). |
| `Editor/LevelBuilder.cs`, `HardLevelObjects.cs`, `LevelThemes.cs` | Hills Easy | Builds water in the float pits (`water_fill`), floats, bats, current strips (`current_top`, swapped like `soft_top`), balloons, the 5 themes and their props. Ambience comes from the level. |
| Tests | 296 | Rules tests for every new object, Hills Normal data tests (the same checks as `HillsEasyLevelsTests`), the ambience override, the shelf at 17, and Mountains platforms unchanged. |

## 5. Object rules: exact numbers

See `.scratch/hills-normal/layouts.md`, the "Rule constants" table. The bot used those numbers, so copy them into the Rules classes as named constants.

## 6. Level data: Hills Normal 1–5

See `.scratch/hills-normal/layouts.md`: the full data for each level and the bot results. The bot files are `.scratch/hills-normal/bot-check-*.cs.txt`. `LevelCatalog` must match `layouts.md` exactly.

## 7. Content (drafts, Q-04: needs teacher review)

### Lessons (4 sentences, each at most 44 characters)

- **N1 Hundred Islands** (picture `lesson_hundred_islands`):
  - Hundred Islands is in Pangasinan.
  - Its hills are old coral reefs.
  - Waves wore them into small hills.
  - Corals build reefs from aragonite.
- **N2 Callao Cave** (`lesson_callao_cave`):
  - Callao Cave is in a limestone hill.
  - Rain slowly dissolves limestone.
  - Drips build stalactites and flowstone.
  - Do not disturb bats in caves.
- **N3 Pagsanjan Gorge** (`lesson_pagsanjan`):
  - Pagsanjan Gorge is in Laguna.
  - A river cut this deep, narrow valley.
  - Water and stones wear the rock away.
  - Boatmen paddle up to the falls.
- **N4 Banaue Rice Terraces** (`lesson_banaue`):
  - Ifugao farmers built the rice terraces.
  - Terraces are steps cut into slopes.
  - Steps slow water and keep soil in place.
  - The terraces have clay-rich soil.
- **N5 Cappadocia** (`lesson_cappadocia`):
  - Cappadocia is in Türkiye.
  - Volcanoes once covered it in thick ash.
  - Wind and rain carved fairy chimneys.
  - Hard ignimbrite caps protect them.

### Reviews (3 sentences, shown on the Result Screen)

- **N1:** "Hundred Islands are hills made from old coral reefs." / "Corals build their reefs from aragonite." / "Coquina is a rock made of broken shells."
- **N2:** "Callao Cave formed inside a limestone hill." / "Dripping water leaves dripstone and flowstone." / "Calcite is the mineral in cave stone."
- **N3:** "Rivers slowly cut valleys into the land." / "Pagsanjan Gorge was cut by a river." / "Volcanic breccia is broken volcanic rock glued by ash."
- **N4:** "Rice terraces are steps on hill and mountain slopes." / "Terraces slow water and keep soil from washing away." / "Clay helps the terrace soil hold water."
- **N5:** "Fairy chimneys are cones of soft volcanic rock." / "Hard ignimbrite caps keep the rain off them." / "Zeolite forms inside volcanic ash."

### New kinds (Hills only, `IsDraft` true)

| Id | Name | Rock or mineral | Description (at most 80 characters) | Confidence |
|---|---|---|---|---|
| `aragonite` | Aragonite | mineral | A mineral that corals and shells use to build their hard parts. | medium |
| `coquina` | Coquina | rock | A rock made of broken shells pressed and stuck together. | weak: teacher check |
| `flowstone` | Flowstone | rock | Sheets of stone left by water flowing over cave walls. | strong |
| `dripstone` | Dripstone | rock | Stone made by dripping water. It forms stalactites. | strong |
| `volcanic_breccia` | Volcanic Breccia | rock | Sharp pieces of volcanic rock glued together by ash. | weak: regional source |
| `ignimbrite` | Ignimbrite | rock | A rock made when a hot cloud of volcanic ash settles and welds. | strong |
| `zeolite` | Zeolite | mineral | A mineral that forms in volcanic ash. It is full of tiny holes. | medium |

Item pictures are 24×24, in `Assets/Art/Items/Sprites/<id>.png`.

### Start banners

Hundred Islands, Callao Cave, Pagsanjan Gorge, Banaue Rice Terraces, Cappadocia.

### Fact sources

All sources, with their confidence, are in `.scratch/hills-normal/kinds-research.md`, which was checked on 2026-11-17. The weak ones are coquina at Hundred Islands, volcanic breccia at Pagsanjan (the JICA source covers the region only) and the Batanes-style travel facts. No source names the bedrock at Pagsanjan or Banaue. MGB's 1:50,000 sheets are sold on paper (PHP 500 each), so a note is added to Q-04.

## 8. Assets Codex must make (Python, with a preview each)

Use the same palette and helpers as `Tools/Art/make_hills_easy_art.py`. Every file name and size below is what the builders load, so keep the names exactly. Sizes are width × height in pixels.

### `Tools/Art/make_hills_normal_art.py` → `Assets/Art/Level/Sprites/`

**Tiles and boulders for each theme.** The ending is `_island`, `_cave`, `_gorge`, `_terrace` or `_chimney`.

| Sprite | Size | Notes |
|---|---|---|
| `<ending>_top` (for example `island_top`) | 16×8 | ground top: island grass on limestone, damp cave floor, mossy gorge rock, terrace mud with grass, pale tuff |
| `<ending>_soil` | 16×16 | soil under the top |
| `ledge_<ending>` | 16×10 | ledge slab |
| `boulder_small_<ending>` / `boulder_big_<ending>` | 32×24 / 48×36 | boulders |

**Objects:**

| Sprite | Size | Notes |
|---|---|---|
| `water_fill` | 16×16 | sea or pond water in a float pit; tiles like `pit_fill` |
| `float_raft` | 48×10 | bamboo raft; the top 2 px are its standing line |
| `float_ripple_00/01` | 16×4 | ripples that warn of sinking |
| `current_top` | 16×8 | shallow fast water over the ground top |
| `current_mark_00/01` | 12×6 | ripple chevrons; flip x for leftward |
| `bat_00/01/02` | 16×10 | three wing frames, dark brown, friendly face |
| `balloon` | 48×80 | envelope and basket; the basket top is the bottom 10 px |
| `balloon_flame_00/01` | 8×8 | burner flame |

**Goals:**

| Sprite | Size | Goal |
|---|---|---|
| `boat_dock` | 56×48 | Boat Dock |
| `cave_mouth` | 56×48 | Cave Mouth |
| `falls_landing` | 56×48 | Falls Landing |
| `ifugao_house` | 56×56 | Ifugao House |
| `balloon_field` | 56×48 | Balloon Field |

**Props** (`Props/`):

| Sprite | Size | Notes |
|---|---|---|
| `seabird_00/01` | 12×8 | |
| `bangka` | 40×14 | |
| `mangrove` | 24×20 | |
| `stalagmite` | 12×20 | |
| `bat_sign` | 20×20 | sign that reads "Do not disturb bats" in a pixel font, or a bat with a hush icon if text will not fit |
| `chapel_glow_00/01` | 24×24 | |
| `fern` | 16×12 | |
| `bangka_boatman` | 40×20 | |
| `falls_mist_00/01` | 24×16 | |
| `rice_stalk_00/01` | 10×12 | |
| `bulul` | 12×20 | |
| `farmer_00/01` | 16×20 | |
| `cone_house` | 32×48 | |
| `far_balloon_00/01` | 12×16 | |
| `dry_shrub` | 14×8 | |

**Items** (`Assets/Art/Items/Sprites/`, 24×24): aragonite, coquina, flowstone, dripstone, volcanic_breccia, ignimbrite and zeolite.

**Lesson pictures** (`Assets/Art/Lesson/Sprites/`, 120×82): `lesson_hundred_islands`, `lesson_callao_cave`, `lesson_pagsanjan`, `lesson_banaue` and `lesson_cappadocia`.

Preview: `.scratch/hills-normal/art-preview.png`.

### `Tools/Art/make_hills_normal_backdrops.py` → `Assets/Art/Level/Sprites/Backdrop/<Theme>/L0–L5.png` (960×270 each)

- **IslandHills:** bright sea sky, far islets with mushroom bases, nearer limestone islets with green tops, bangkas, near mangroves.
- **LimestoneCave:**
  - L0–L1: the cave mouth's light and a dark rock frame.
  - L2–L3: stalactite rows and columns.
  - L4–L5: a near cave wall and flowstone.
  - Darker overall, lit from the mouth side.
- **RiverGorge:** a narrow strip of sky, tall mossy cliff walls, a far waterfall, a green river, near ferns.
- **RiceTerraces:** misty mountains, terrace steps with water shining, Ifugao houses, near rice.
- **FairyChimneys:** a dawn sky with several balloons, far tuff cones, fairy chimneys with dark caps, near cone houses.

Preview: `.scratch/hills-normal/backdrop-preview.png`.

### `Tools/Audio/make_hills_normal_audio.py` → `Assets/Audio/`

| File | What | Length | Loudness target |
|---|---|---|---|
| `amb_cave_drips.ogg` | slow drips with an echo tail, a soft far rumble, no birds | 24 s loop, end folded onto start | about −21 dB mean, like the other ambience |
| `sfx_splash.wav` | short water splash | about 0.3 s | about −24 dB |
| `sfx_bat_flap.wav` | three quick soft wing flaps | about 0.25 s | about −26 dB |
| `sfx_burner.wav` | gas burner whoosh | about 0.5 s | about −24 dB |

Use the same encoding as `make_hills_audio.py`: ffmpeg ogg, and a low-pass filter so nothing hisses.

Preview: `.scratch/hills-normal/audio-preview.png`.

### Theme table for `LevelThemes.cs`

| Theme | Top / Soil / Ledge / Boulder ending | Goal (name, sprite, offset) | Backdrop |
|---|---|---|---|
| IslandHills | `island_top` / `island_soil` / `ledge_island` / `_island` | "Boat Dock", `boat_dock`, (24, 24) | `Backdrop/IslandHills/` |
| LimestoneCave | `cave_top` / `cave_soil` / `ledge_cave` / `_cave` | "Cave Mouth", `cave_mouth`, (24, 24) | `Backdrop/LimestoneCave/` |
| RiverGorge | `gorge_top` / `gorge_soil` / `ledge_gorge` / `_gorge` | "Falls Landing", `falls_landing`, (24, 24) | `Backdrop/RiverGorge/` |
| RiceTerraces | `terrace_top` / `terrace_soil` / `ledge_terrace` / `_terrace` | "Ifugao House", `ifugao_house`, (24, 28) | `Backdrop/RiceTerraces/` |
| FairyChimneys | `chimney_top` / `chimney_soil` / `ledge_chimney` / `_chimney` | "Balloon Field", `balloon_field`, (24, 24) | `Backdrop/FairyChimneys/` |

### Living props

These are decoration only. Every one goes through `HillsPropFits`: off pits, ledges, boulders, water and the first 16 px of any current strip.

- **IslandHills:**
  - Mangrove every ~380 px from x = 150 (order 3, y 10).
  - Seabirds (2 frames, 1.2–2 s) at x = 700, 2600 and 5200 (order 4, y 70).
- **LimestoneCave:**
  - Stalagmite every ~300 px from x = 120 (order 3, y 10).
  - `bat_sign` at x = 200 (order 3, y 10).
  - Chapel glow (2 frames, 1.5 s) at x = 3600 (order 2, y 30).
- **RiverGorge:**
  - Fern every ~220 px from x = 90 (order 7, y 6).
  - Falls mist (2 frames) near the goal at x = goal − 60 (order 3, y 20).
- **RiceTerraces:**
  - Rice stalks every ~120 px from x = 60 (order 7, y 6, 2 frames).
  - Bulul at x = 1500 and 4800 (order 3, y 10).
  - Farmer (2 frames, 2 s) at x = 2600 (order 3, y 10).
- **FairyChimneys:**
  - Dry shrub every ~260 px from x = 110 (order 7, y 4).
  - Cone house at x = 900 and 5600 (order 2, y 24).
  - Far balloons (2 frames) at x = 1800, 4200 and 6500 (order 1, y 120).

The background bangka and boatman are in the backdrops, not props.

## 9. Not in scope

- Hills Hard 1–7 (its own spec after Normal is played).
- Plains, and the Hills Description Screen changes (already built in D-066).
- Badge changes, and map or menu life changes.
- Changes to Mountains or Hills Easy.
