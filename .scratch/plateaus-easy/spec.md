# Plateaus Easy 1–3 and the Plateaus episode outline (D-072)

Planned on 2026-11-23 in a grilling session (three rounds, eleven questions). The user accepted every recommendation, and the E2 topic was changed after the kinds research found no source for "built by lava". This spec covers:

- the **episode outline** for all 15 Plateaus levels (places, topics, objects, badges, Description Screen data), and
- the **full build** of Plateaus Easy 1–3, including a new object, the **cliff updraft**.

Plateaus Normal 1–5 and Hard 1–7 get their own specs after the user has played Easy. **The ending screen is decided now (section 3) but built with Plateaus Hard**, because only Plateaus Hard 7 can reach it; this build adds only its badge rule.

**Build split (same as Plains Hard):** Claude made the art, backgrounds, badges, sounds and bot-checked layouts. A Claude Code session builds the C#, one ticket per session, from `SESSION-PROMPT.md`. Then `/code-review`, then the user plays the checklist.

## 1. Rules that are already settled (do not change)

- Episode order is Mountains → Hills → Plains → Plateaus. Plateaus opens when all 15 Plains levels are cleared; the unlock, the one-time map animation and "Finish Plains first" already work for any landform (D-039, D-065, D-066).
- Each episode has 3 Easy, 5 Normal and 7 Hard levels (Chapter III P066).
- Easy levels have 5 pickups, 1 checkpoint and a 120 s limit. Pits and spikes send the explorer a short way back (D-048, D-056).
- Stars: 0–40% gives 1 star, 41–99% gives 2, and 100% gives 3 (D-039).
- One secret-spot pickup per level, a 1.5 s start banner, living props, and teach-test-twist pacing (D-055).
- One script runs every level. Level differences are data in `LevelCatalog`. Rules live in `Assets/Scripts/Rules` with no Unity types and are covered by tests (D-061).
- Every level theme has its own six-layer background (D-063).
- Shared kinds: one kind can sit on several shelves; it is saved and counted once (D-066).
- All lesson, review, kind and summary text is a **draft** (`isDraft: true`, "Draft — needs teacher review"). Q-04 stays open (D-043).
- Walking speed is 84 px/s everywhere.
- Mountains, Hills and Plains must play exactly as before.

## 2. New decisions (D-072)

1. **Plan scope:** an outline for all 15 levels, and a full spec for Easy 1–3 only.
2. **Easy places:** E1 Bukidnon Plateau (what a plateau is); E2 Lake Lanao, about 700 m (lakes on plateaus: volcanoes helped make its basin); E3 Maria Cristina Falls on the Agus River (a river drops off the plateau edge; the falls run over basalt).
3. **New object, the cliff updraft (E3):** section 5.
4. **Kinds:** the Plateaus shelf starts with 10 kinds: 4 new Plateaus-only kinds (halloysite, goethite, gibbsite, cristobalite) and 6 shared kinds that gain `Episode.Plateaus` (basalt, andesite, hematite, tuff, volcanic_breccia, laterite). Total kinds 63.
5. **Badges:** eleven on the Plateaus page (section 3).
6. **Audio:** `bgm_plateaus` (A major, 6/8, 88 BPM, 40 s loop, flute-like lead). Ambience: Easy dry wind and a hawk (`amb_highland_easy`), Normal canyon wind, an echoing hawk and a far waterfall (`amb_canyon_normal`), Hard strong high wind with blowing sand (`amb_highwind_hard`). Effect `sfx_updraft` (a loop).
7. **Map and menus:** the open Plateaus region on the Landform Map shows a shining waterfall, swaying grass and two circling hawks. The Plateaus menu screens show the cloud, two hawks and swaying grass.
8. **Plateaus Description Screen** data is added now (section 3). It is reached only after Plateaus Hard 7 exists.

## 3. Episode outline (all 15 levels)

| Level | Place (banner) | Topic | Theme | New object | Kinds |
|---|---|---|---|---|---|
| E1 | Bukidnon Plateau ("Bukidnon Plateau") | What a plateau is | PineapplePlateau | none | basalt\*, andesite\*, hematite\*, halloysite |
| E2 | Lake Lanao ("Lake Lanao") | Lakes on plateaus | LakePlateau | none | tuff\*, volcanic_breccia\*, goethite, + halloysite review |
| E3 | Maria Cristina Falls ("Maria Cristina Falls") | Rivers drop off plateau edges | FallsEdge | **cliff updraft** | gibbsite, laterite\*, cristobalite, + goethite review |
| N1 | Table Mountain, South Africa | Flat tops | later | cable cars (moving platforms, new art) | later |
| N2 | Monument Valley, USA | Mesas and buttes | later | crumbling ledges and updrafts | later |
| N3 | Grand Canyon, Colorado Plateau | Rivers cut canyons; rock layers | later | river current | later |
| N4 | Mount Roraima, Venezuela | Table-top mountains and waterfalls | later | updrafts and floats | later |
| N5 | Altiplano and Salar de Uyuni, Bolivia | A high plateau and a salt flat | later | salt crust (the ice rule) and llama herds (the herd rule) | later |
| H1 | Deccan Plateau, India | Lava plateaus | later | steam vents and crumbles | later |
| H2 | Ethiopian Highlands | Life on high plateaus | later | gelada troops (the herd rule) | later |
| H3 | Tassili n'Ajjer, Algeria | Wind carves desert plateaus | later | sideways wind gusts and soft sand | later |
| H4 | Mesa Verde, USA | Cliff homes | later | ledges, updrafts, moving platforms | later |
| H5 | Antarctic Plateau | An ice plateau | later | ice and wind gusts | later |
| H6 | Loess Plateau, China | Soil loss and terraces | later | soft sand and crumbling ledges | later |
| H7 | Tibetan Plateau (finale) | The roof of the world | later | everything | later |

\* = a shared kind already on another shelf.

The outline is tentative after E3: each Normal and Hard place, object and kind needs its own check when its spec is written. Each new kind needs a geological source (Chapter II P026).

### Badges (all 11 get their rules and art now)

New entries go **after** the ten Plains badges in `AchievementCatalog.All`. Ids and order are saved; never reorder.

| Id | Name | Tier | Rule | Hint |
|---|---|---|---|---|
| `updraft_rider` | Updraft Rider | silver | clear Plateaus Easy 3 | Clear Plateaus Easy 3. |
| `table_top` | Table Top | silver | clear Plateaus Normal 1 | Clear Plateaus Normal 1. |
| `high_star` | High Star | gold | 3 stars on all 3 Plateaus Easy levels | Get 3 stars on every Plateaus Easy level. |
| `plateau_collector` | Plateau Collector | gold | find every kind on the Plateaus shelf | Find every Plateaus rock and mineral. |
| `philippine_plateaus` | Philippine Plateaus | silver | clear Plateaus Easy 1–3 | Clear Plateaus Easy 1 to 3. |
| `canyon_explorer` | Canyon Explorer | gold | clear Plateaus Normal 3 | Clear Plateaus Normal 3. |
| `mesa_ranger` | Mesa Ranger | silver | clear any Plateaus Hard level | Clear a Plateaus Hard level. |
| `world_plateaus` | World Plateaus | silver | clear Plateaus Hard 5, 6 and 7 | Clear Plateaus Hard 5 to 7. |
| `steady_plateaus` | Steady Plateaus | gold | 3 stars on all 7 Plateaus Hard levels | Get 3 stars on every Plateaus Hard level. |
| `plateau_master` | Plateau Master | gold | clear all 15 Plateaus levels (`IsEpisodeComplete(Plateaus)`) | Clear every Plateaus level. |
| `land_grand_champion` | Land Grand Champion | gold | all four landforms complete (`IsEpisodeComplete` for each `Episode`) | Clear every level of every landform. |

Use the existing helpers exactly as the Plains badges do. Badges whose levels have no data yet stay locked. Art: `badge_<id>.png` and `badge_<id>_locked.png` in `Assets/Art/MainMenu/Sprites/Profile/` (made by `make_profile_art.py`). Check that the Plateaus badge page shows 11 badges in the 5×3 grid without overflow.

### Plateaus Landform Description Screen (data only)

- Title "Plateaus", with the draft summary: "Plateaus are high lands with flat tops." / "Their sides are steep, like a table." / "Lava, rivers and wind shape plateaus." / "Rivers can cut deep canyons in them." / "Many people farm on high plateaus."
- Picture `bg_plateaus` (already exists), draft note on.
- Totals: stars n/45, kinds n/(Plateaus shelf), badges n/11.

### Ending screen (decided now, built with Plateaus Hard)

After the Plateaus Description Screen, which says "You explored all four landforms!" instead of a next-landform banner, a one-time ending screen shows the explorer on a plateau looking over all four landforms (reusing the start-up screen layers), the title "Land Grand Explorer!", stars n/180, kinds n/total and badges n/total, a short thank-you line, and one Back to Map button, with the menu music. Replays stay allowed. "Shown once" follows the unlock-animation pattern (a device setting, like `LandGrand.UnlockSeen.`). **Not part of this build.**

## 4. Code changes

| Where | Now | Change |
|---|---|---|
| `Rules/LevelLayout.cs` `LevelTheme` | ends with `Steppe` | Add `PineapplePlateau`, `LakePlateau`, `FallsEdge` at the **end**. |
| `Rules/LevelLayout.cs` constructor | last optional parameter `StrawPile[] strawPiles = null` | Add `Updraft[] updrafts = null` at the **end**, and `IReadOnlyList<Updraft> Updrafts` (empty by default). Existing callers do not change. |
| new `Rules/Updraft.cs` | — | Section 5. |
| `Rules/LevelData.cs` `ItemCatalog` | 59 kinds | Append `halloysite`, `goethite`, `gibbsite`, `cristobalite` at the **end** of `All` (Plateaus only, `IsDraft`). Add `Episode.Plateaus` to the landforms of basalt, andesite, hematite, tuff, volcanic_breccia and laterite (keep their existing landforms). |
| `Rules/LevelData.cs` `LevelCatalog` | no Plateaus levels | Add Plateaus Easy 1–3 exactly as `layouts.md`. |
| `Rules/LandformAudio.cs` | Plateaus uses the Mountains files | Plateaus row: music `bgm_plateaus`; Easy `amb_highland_easy`, Normal `amb_canyon_normal`, Hard `amb_highwind_hard`; script `make_plateaus_audio.py`. |
| `Rules/LandformSummary.cs` | Plateaus returns "Coming soon" | Plateaus returns the 5 draft lines above, picture `bg_plateaus`, draft on. |
| `Rules/Achievements.cs` | 35 badges | Append the 11 Plateaus badges (section 3), page `Episode.Plateaus`. |
| `Play/ExplorerMotor.cs` | — | Section 5 (`UpdraftTop`). |
| new `Play/UpdraftWatcher.cs` (level part) | — | Section 5. |
| `Editor/LevelThemes.cs` | Plains themes last | The three theme entries and props in section 8. |
| `Editor/LevelBuilder.cs` and the level object builders | — | Build each updraft view (section 5), registered with the watcher. Missing-art errors name `Tools/Art/make_plateaus_easy_art.py`. |
| `Editor/SelectionBuilder.cs` `MenuLife` | `sets[Plateaus] = CloudOnly(...)` | A `PlateausMenuLife()` like `PlainsMenuLife()` (section 8). |
| `Editor/LandformMapBuilder.cs` | Plateaus has no life | A `PlateausLife()` like `PlainsLife()`, shown only while Plateaus is open (`layouts.md`). |
| `Editor/LandformLife.cs` | per-landform sprite helpers | Load the `Plateaus/` map sprites the same way; older landforms load exactly as before. |
| Tests that say Plateaus is empty | Collection, LandformSummary, LevelCatalog, Achievement and progression tests | Update them to the new facts: the Plateaus shelf has 10 kinds; the total is 63; badges 46. Do not delete what they check about other landforms. |

Already generic, no change expected: the unlock order and lock guide (`RegionGuide`), `MaxStarsFor`, scene names (`PlateausEasy1`), the save format, the Collection and badge pages with their arrows, "Coming soon" for difficulties without levels, Next hidden when the next level has no data.

## 5. Cliff updraft: exact rules

- New file `Rules/Updraft.cs`:
  - `public sealed class Updraft` with `public float X { get; }` (left edge) and `public float Top { get; }` (the feet height where the explorer hovers), constructor `Updraft(float x, float top)`.
  - `public const float Width = 32f;` and `public const float RiseSpeed = 90f;` (px/s).
  - `public bool Contains(float middleX, float feetY)`: `middleX >= X && middleX <= X + Width && feetY >= -0.5f && feetY <= Top + 0.5f`.
  - `public float RiseVelocity(float feetY, float dt)`: `Clamp((Top − feetY) / dt, 0, RiseSpeed)`, so the feet stop exactly at `Top`.
- `ExplorerMotor`: a new `public float UpdraftTop { get; set; } = float.NaN;` (NaN means "not in an updraft"), set every frame by the watcher before the motor moves (use the same ordering the wind's `ExternalPushX` uses). In `Update`, when it is not NaN:
  - a jump press does nothing (no ground jump, no double jump, no somersault or dust);
  - after the gravity line, set `grounded = false`, `StandingOn = null`, `jumpedFromGround = true`, `airJumpUsed = false`, and `velocity.y = Updraft rise velocity` (so gravity is off inside, and the double jump is ready on leaving, as after a launch);
  - walking left and right work as normal.
- New level part `Play/UpdraftWatcher.cs` (with `Begin(LevelController)` and `IsBuilt`, like `StrawBounceWatcher`): each frame, find the updraft that `Contains` the explorer's middle and feet and set `UpdraftTop` (or NaN). While the explorer is inside any column, loop `sfx_updraft.wav` at volume 0.5 through the level's sfx volume; stop it on leaving and when the game pauses.
- **View:** each column stacks `updraft_00`–`_03` (32×32, tileable) from the ground up to `Top + 26` (the hovering head), with `updraft_base` (32×8) on the ground; the frames step at 8 per second. Two leaves and two dust specks (`Props/updraft_leaf_00/01`, `Props/updraft_dust_00/01`) rise through each column on a loop. Animation reads `LevelClock.Now`, so it pauses with the game.
- Fairness (checked by the bot lint): no updraft within 60 px of a pit or spikes, 40 px of a boulder or the checkpoint, before x 200 or after goal − 120; the first updraft has no pit or spikes within 80 px; the hovering head stays at or below 196 px; no ledge in or over a column. Each landing ledge starts 8 px right of its column and sits 4 px below the updraft top.
- Tests (EditMode):
  - `Contains` at both side edges, just outside, at feet −0.5, at `Top + 0.5` and just above.
  - `RiseVelocity` is 90 far below the top, 0 at the top, and lands exactly on the top in one step when closer than 90 · dt.
  - Plateaus Easy 3 has 4 updrafts; the first is safe (no pit or spikes within 80 px); no updraft is within 40 px of the checkpoint; each has its landing ledge at `x + 40`, `top − 4`; no other level has updrafts.
  - Mountains, Hills and Plains levels have no updrafts.

## 6. Level data

Exactly as `layouts.md`. Banners and goals: E1 "Bukidnon Plateau" (goal "Farm Lookout"), E2 "Lake Lanao" ("Torogan"), E3 "Maria Cristina Falls" ("Falls Viewing Deck").

## 7. Content (drafts, Q-04: needs teacher review)

### Lessons (4 sentences, each at most 44 characters)

**E1 "Plateaus"** (picture `lesson_bukidnon`):
1. A plateau is high land with a flat top.
2. Its sides are steep, like a table.
3. Bukidnon is a plateau in Mindanao.
4. Farmers grow pineapples on its top.

**E2 "Plateau Lakes"** (picture `lesson_lanao`):
1. Some plateaus hold big lakes.
2. Lake Lanao is about 700 m high.
3. Volcanoes helped make its basin.
4. It is our second-largest lake.

**E3 "Plateau Rivers"** (picture `lesson_maria_cristina`):
1. Rivers flow off the edge of plateaus.
2. Lake Lanao drains into the Agus River.
3. It falls 98 m at Maria Cristina Falls.
4. The falls make power for Mindanao.

### Reviews (3 sentences, shown on the Result Screen)

- **E1:** "A plateau is high, flat land with steep sides." / "Basalt and andesite come from old volcanoes." / "Halloysite and hematite are in its red soil."
- **E2:** "Lake Lanao lies high on a plateau." / "Volcanic ash hardens into tuff." / "Goethite is a rusty mineral in the soil."
- **E3:** "Rivers fall off plateau edges as waterfalls." / "Maria Cristina Falls runs over basalt." / "Gibbsite and laterite form in warm, wet soil."

### New kinds (Plateaus only)

| Id | Name | Rock? | Description | Picture |
|---|---|---|---|---|
| `halloysite` | Halloysite | mineral | A soft white clay mineral. It forms when volcanic rock breaks down. | `halloysite` |
| `goethite` | Goethite | mineral | A brown iron mineral. It makes soil look rusty. | `goethite` |
| `gibbsite` | Gibbsite | mineral | A soft white mineral rich in aluminium. It forms in warm, wet soils. | `gibbsite` |
| `cristobalite` | Cristobalite | mineral | A form of silica, like quartz. It can make tiny white balls in volcanic rock. | `cristobalite` |

Shared kinds keep their existing text and pictures.

### Fact sources (checked 2026-11-23; see `kinds-research.md`)

- **Bukidnon:** average elevation 915 m; "mainly a volcanic zone of pyroclastic, basaltic, and andesitic cones"; pineapple plantations (Wikipedia, Bukidnon). Bukidnon soils contain halloysite, gibbsite, goethite, hematite and cristobalite, over Kitanglad basalt and andesite (Bukidnon soils paper, Univ. Liège).
- **Lake Lanao:** about 700 m; formed by "tectonic-volcanic damming of a basin between two mountain ranges, and the collapse of a large volcano"; second-largest lake in the Philippines; its only outlet is the Agus River (Wikipedia, Lake Lanao).
- **Maria Cristina Falls:** 98 m; the Agus River "flows over a basalt rock formation"; the Agus hydropower plants supply much of Mindanao's electricity (Wikipedia).
- **Not confirmed (teacher check):** the plateau definition (needs a source); tuff, volcanic breccia and laterite at these places (inferred from "pyroclastic", lahars and the soil types); no source names a rock at Lake Lanao itself; "the falls drop off the plateau edge" is inferred from the lake (about 700 m) and the sea; the cristobalite description; the halloysite "forms when volcanic rock breaks down" wording.

## 8. Assets (made by Claude; do not edit)

Scripts and their outputs:
- `python Tools/Art/make_plateaus_easy_art.py`
  - **Level sprites** (`Assets/Art/Level/Sprites/`): `pineapple_top`, `pineapple_soil`, `ledge_pineapple`, `farm_lookout` (56×48), `lake_top`, `lake_soil`, `ledge_lake`, `torogan` (56×48), `falls_top`, `falls_soil`, `ledge_falls`, `falls_deck` (56×48), `boulder_small_*` / `boulder_big_*` for `_pineapple`, `_lake` and `_falls`, `updraft_00`–`_03` (32×32), `updraft_base` (32×8).
  - **Props** (`Props/`): `updraft_leaf_00/01`, `updraft_dust_00/01` (6×6), `pineapple_plant` (12×12), `hawk_00/01` (16×10), `lake_reed_00/01` (8×16), `banca_00/01` (24×10, spare: not placed in Easy), `fern_00/01` (12×12), `falls_spray_00/01` (16×12).
  - **Items:** `halloysite`, `goethite`, `gibbsite`, `cristobalite` (24×24).
  - **Lesson pictures:** `lesson_bukidnon`, `lesson_lanao`, `lesson_maria_cristina` (120×82).
  - **Map and menu life** (`Assets/Art/LandformMap/Sprites/Plateaus/`): `hawk_0/1`, `waterfall_0/1`, `grass_0/1`.
  - Preview: `.scratch/plateaus-easy/art-preview.png`.
- `python Tools/Art/make_plateaus_easy_backdrops.py`: `Backdrop/PineapplePlateau/`, `Backdrop/LakePlateau/`, `Backdrop/FallsEdge/`, each L0–L5 (960×270). Preview: `.scratch/plateaus-easy/backdrop-preview.png`.
- `python Tools/Art/make_profile_art.py`: the 11 Plateaus badges, normal and locked. Preview: `.scratch/plateaus-easy/badge-preview.png`.
- `python Tools/Audio/make_plateaus_audio.py`: `bgm_plateaus.ogg` (a 40.9 s loop: 30 bars of 6/8 at 88 BPM, the nearest whole bars to 40 s), `amb_highland_easy.ogg`, `amb_canyon_normal.ogg`, `amb_highwind_hard.ogg` (24 s loops), `sfx_updraft.wav` (loop). Preview: `.scratch/plateaus-easy/audio-preview.png`.

### Theme table for `LevelThemes.cs`

| Theme | GroundTop / Soil / Ledge / BoulderEnding | Goal (name, sprite, offset) | Backdrop | Scripts |
|---|---|---|---|---|
| PineapplePlateau | `pineapple_top` / `pineapple_soil` / `ledge_pineapple` / `_pineapple` | "Farm Lookout", `farm_lookout`, (24, 24) | `Backdrop/PineapplePlateau/` | `make_plateaus_easy_art.py`, `make_plateaus_easy_backdrops.py` |
| LakePlateau | `lake_top` / `lake_soil` / `ledge_lake` / `_lake` | "Torogan", `torogan`, (24, 24) | `Backdrop/LakePlateau/` | same |
| FallsEdge | `falls_top` / `falls_soil` / `ledge_falls` / `_falls` | "Falls Viewing Deck", `falls_deck`, (24, 24) | `Backdrop/FallsEdge/` | same |

The goal offsets follow the Plains 56×48 goals. Check each goal in the Editor and report if one floats or sinks (a lower offset is fine when the sprite has clear rows at its base; say so).

### Living props (decoration only; keep them off pits, ledges, boulders and updraft columns with the existing prop-fit helpers; report any prop moved more than 64 px)

- **PineapplePlateau:** a pineapple plant about every 100 px from x = 80, order 7, centre y = 6; hawks (`hawk_00/01`, frame swap every 0.4 s) high in the sky at x = 900, 2600 and 4100, centre y = 150, order 2.
- **LakePlateau:** a lake reed (`lake_reed_00/01`, 1.2–1.8 frames per second) about every 160 px from x = 120, order 7, centre y = 8; hawks at x = 1300 and 3900 as above.
- **FallsEdge:** a fern (`fern_00/01`, 1.2–1.8 frames per second) about every 140 px from x = 100, order 7, centre y = 6; falls mist (`falls_spray_00/01`, 2 frames per second) at x = 5100 and 5180, order 3, centre y = 12; hawks at x = 1800 and 3900 as above. Never inside an updraft column or within 8 px of one.

### Menu life (`SelectionBuilder`)

- **Plateaus:** the cloud, two hawks (`hawk_0/1` on a `CircleFlight` around screen (330, 60), radius (24, 10), 12 s and 15 s per lap) and six grass tufts (`grass_0/1`, 1.5 frames per second) along the bottom at x = 30, 110, 190, 290, 370 and 450, on screen row 268. Other landforms: as now.

### Audio table (`LandformAudio`)

| Episode | Music | Easy | Normal | Hard |
|---|---|---|---|---|
| Mountains | `bgm_mountains` | `amb_breeze_easy` | `amb_wind_normal` | `amb_wind_hard` |
| Hills | `bgm_hills` | `amb_stream_easy` | `amb_stream_normal` | `amb_rain_hard` |
| Plains | `bgm_plains` | `amb_grass_easy` | `amb_wetland_normal` | `amb_dry_hard` |
| **Plateaus** | **`bgm_plateaus`** | **`amb_highland_easy`** | **`amb_canyon_normal`** | **`amb_highwind_hard`** |

## 9. Not in scope

- Plateaus Normal 1–5 and Hard 1–7: levels, lessons, kinds, objects, backgrounds.
- The ending screen, the "You explored all four landforms!" Description banner and its "shown once" setting (built with Plateaus Hard).
- Any change to Mountains, Hills or Plains levels, numbers or text.
