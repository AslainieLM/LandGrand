# Plains Easy 1–3 and the Plains episode outline (D-069)

Planned on 2026-11-21 in a grilling session (two rounds, eight questions). The user accepted every recommendation. This spec covers:

- the **episode outline** for all 15 Plains levels (places, topics, themes, new objects, badges, Description Screen), and
- the **full build** of Plains Easy 1–3, including a new object, the **straw bounce pile**.

Plains Normal 1–5 and Hard 1–7 get their own specs after the user has played Easy, as with Mountains and Hills. Their kinds, exact object numbers and backgrounds are chosen then.

**Build split (same as Hills Hard):** Claude made the art, backgrounds, badges, sounds and bot-checked layouts. ChatGPT/Codex writes the C# only. Claude reviews the build, then the user plays the checklist.

## 1. Rules that are already settled (do not change)

- Episode order is Mountains → Hills → Plains → Plateaus. Plains opens when all 15 Hills levels are cleared; the unlock, the one-time map animation and "Finish Hills first" already work for any landform (D-039, D-065, D-066).
- Each episode has 3 Easy, 5 Normal and 7 Hard levels (Chapter III P066).
- Easy levels have 5 pickups, 1 checkpoint and a 120 s limit. Pits and spikes send the explorer a short way back (D-048, D-056).
- Stars: 0–40% gives 1 star, 41–99% gives 2, and 100% gives 3 (D-039).
- One secret-spot pickup per level, a 1.5 s start banner, living props, and teach-test-twist pacing (D-055).
- One script runs every level. Level differences are data in `LevelCatalog`. Rules live in `Assets/Scripts/Rules` with no Unity types and are covered by tests (D-061).
- Every level theme has its own six-layer background (D-063).
- Shared kinds: one kind can sit on several shelves; it is saved and counted once (D-066).
- All lesson, review, kind and summary text is a **draft** (`isDraft: true`, "Draft — needs teacher review"). Q-04 stays open (D-043).
- Walking speed is 84 px/s everywhere (D-067 review).
- Mountains and Hills must play exactly as before.

## 2. New decisions (D-069)

1. **Plan scope:** an outline for all 15 levels, and a full spec for Easy 1–3 only.
2. **Easy places:** E1 Central Luzon Plain, Nueva Ecija (what a plain is); E2 Pangasinan salt beds (plains by the sea, salt from seawater); E3 Isabela river plain, Cagayan Valley (rivers spread soil and make farm plains).
3. **New object, the straw bounce pile (E3):** a 48×16 golden rice-straw pile that stands on the ground like a boulder. Landing on its top launches the explorer at **400 px/s** (about 89 px up; feet reach about 104 px). The double jump is ready again after the launch, so bounce + double jump reaches about 128 px. Walking into its side does nothing (it is a solid wall 16 px tall). It never hurts. The first pile is on safe ground.
4. **Kinds:** the Plains shelf starts with 10 kinds: 4 new Plains-only kinds (siltstone, halite, gypsum, mudstone) and 6 shared kinds that gain `Episode.Plains` (clay, sandstone, shale, quartz, conglomerate, limestone).
5. **Badges:** ten on the Plains page, with the same rules as the ten Hills badges (section 3).
6. **Audio:** `bgm_plains` (D major, 4/4, 96 BPM, 40 s loop, bamboo-flute-like lead). Ambience: Easy grass wind and crickets (`amb_grass_easy`), Normal adds frogs and water birds (`amb_wetland_normal`), Hard dry wind and far thunder (`amb_dry_hard`). Effect `sfx_straw_bounce`.
7. **Map and menus:** the open Plains region on the Landform Map shows swaying rice, two white egrets and a small grazing carabao. The Plains menu screens show two egrets and swaying grass, plus the cloud every landform has. Plateaus stays locked with "Finish Plains first", then gets the one-time unlock animation and "Coming soon" (already generic).
8. **Plains Description Screen** data is added now (section 3). It is reached only after Plains Hard 7 exists.

## 3. Episode outline (all 15 levels)

| Level | Place (banner) | Topic | Theme | New object | Kinds |
|---|---|---|---|---|---|
| E1 | Central Luzon Plain, Nueva Ecija ("Central Luzon Plain") | What a plain is | RicePlain | none | clay\*, sandstone\*, shale\*, siltstone |
| E2 | Pangasinan salt beds ("Pangasinan Salt Beds") | Plains by the sea; salt from seawater | SaltBeds | none | halite, gypsum, quartz\*, + siltstone review |
| E3 | Isabela, Cagayan Valley ("Isabela River Plain") | Rivers spread soil and make farm plains | RiverPlain | **straw bounce pile** | conglomerate\*, limestone\*, mudstone, + halite review |
| N1 | Candaba Swamp, Pampanga | Wetland plains and their birds | CandabaWetland | sinking float (reused) | chosen in the Normal spec |
| N2 | Pinatubo lahar plain, Pampanga–Tarlac | Volcanoes can make plains | LaharPlain | soft sand and steam vents (reused) | later |
| N3 | Agno River delta, Pangasinan | Rivers build deltas at the sea | RiverDelta | river current (reused) | later |
| N4 | Agusan Marsh, Agusan del Sur | People living on wetlands | FloatingVillage | mix | later |
| N5 | Mekong Delta, Vietnam | Floating markets on a delta | FloatingMarket | moving market boats (horizontal platforms, new art) | later |
| H1 | Great Plains, USA | Grassland plains and wind | Prairie | rolling hay bales (the rolling-log rule, new art) | later |
| H2 | Serengeti, Tanzania | Animal migration on plains | Savanna | gentle grazing herd push (the bat push rule) | later |
| H3 | Pampas, Argentina | Plains for farming and ranching | Pampas | mix | later |
| H4 | West Siberian Plain, Russia | Cold plains and frozen ground | FrozenPlain | ice (reused) | later |
| H5 | Nullarbor Plain, Australia | A limestone plain with caves | LimestonePlain | crumbling ledges and bats (reused) | later |
| H6 | Indo-Gangetic Plain, India | Great river plains and floods | GangesPlain | floats and current (reused) | later |
| H7 | finale (place chosen in the Hard spec) | Plains of the world | mix | everything | later |

\* = a shared kind that is already on the Mountains shelf (and some on the Hills shelf).

The outline is tentative after E3: each Normal and Hard place, object and kind needs its own check when its spec is written. Each new kind needs a geological source (Chapter II P026).

### Badges (all 10 get their rules and art now)

New entries go **after** the ten Hills badges in `Achievements.All`. Ids and order are saved; never reorder.

| Id | Name | Tier | Rule | Hint |
|---|---|---|---|---|
| `salt_maker` | Salt Maker | silver | clear Plains Easy 2 | Clear Plains Easy 2. |
| `straw_jumper` | Straw Jumper | silver | clear Plains Easy 3 | Clear Plains Easy 3. |
| `flat_star` | Flat Star | gold | 3 stars on all 3 Plains Easy levels | Get 3 stars on every Plains Easy level. |
| `plain_collector` | Plain Collector | gold | find every kind on the Plains shelf | Find every Plains rock and mineral. |
| `philippine_plains` | Philippine Plains | silver | clear Plains Normal 1–4 | Clear Plains Normal 1 to 4. |
| `delta_boater` | Delta Boater | gold | clear Plains Normal 5 | Clear Plains Normal 5. |
| `prairie_ranger` | Prairie Ranger | silver | clear any Plains Hard level | Clear a Plains Hard level. |
| `world_plains` | World Plains | silver | clear Plains Hard 5, 6 and 7 | Clear Plains Hard 5 to 7. |
| `steady_plains` | Steady Plains | gold | 3 stars on all 7 Plains Hard levels | Get 3 stars on every Plains Hard level. |
| `plain_master` | Plain Master | gold | clear all 15 Plains levels (`IsEpisodeComplete(Plains)`) | Clear every Plains level. |

Use the existing helpers exactly as the Hills badges do (`ClearedAll`, `StarsAll`, `KindsAll`, `ClearedAny`, `Numbers(...)`, `LastThreeHardLevels`). Badges whose levels have no data yet stay locked. Art: `badge_<id>.png` and `badge_<id>_locked.png` in `Assets/Art/MainMenu/Sprites/Profile/` (made by `make_profile_art.py`).

### Plains Landform Description Screen (data only)

- Title "Plains", with the draft summary: "Plains are wide, flat and low lands." / "Rivers, wind, ice and lava can make plains." / "Plains soil is often silt, sand and clay." / "Many people farm and live on plains." / "Some plains lie flat beside the sea."
- Picture `bg_plains` (already exists), draft note on.
- Totals: stars n/45, kinds n/(Plains shelf), badges n/10. "Plateaus is now open!", with "New levels coming soon" while Plateaus has no levels (already generic).

## 4. Code changes

| Where | Now | Change |
|---|---|---|
| `Rules/LevelLayout.cs` `LevelTheme` | ends with `KarstTowers` | Add `RicePlain`, `SaltBeds`, `RiverPlain` at the **end**. |
| `Rules/LevelLayout.cs` constructor | 19 optional lists, last `RollingLog[] logs = null` | Add `StrawPile[] strawPiles = null` at the **end**, and `IReadOnlyList<StrawPile> StrawPiles` (empty by default). Existing callers do not change. |
| new `Rules/StrawPile.cs` | — | `StrawPile(float x)` data, plus the rule constants and helpers in section 5. |
| `Rules/LevelData.cs` `ItemCatalog` | 50 kinds, last `leucite` | Append `siltstone`, `halite`, `gypsum`, `mudstone` at the **end** of `All` (Plains only, `IsDraft`). Add `Episode.Plains` to the `Landforms` of clay, sandstone, shale, quartz, conglomerate and limestone (keep their existing landforms). |
| `Rules/LevelData.cs` `LevelCatalog` | no Plains levels | Add Plains Easy 1–3 exactly as section 6. |
| `Rules/LandformAudio.cs` | Plains uses the Mountains files | Plains row: music `bgm_plains`; Easy `amb_grass_easy`, Normal `amb_wetland_normal`, Hard `amb_dry_hard`; all from `make_plains_audio.py`. |
| `Rules/LandformSummary.cs` | Plains returns "Coming soon" | Plains returns the 5 draft lines above, picture `bg_plains`, draft on. Plateaus stays "Coming soon". |
| `Rules/Achievements.cs` | 25 badges (15 Mountains, 10 Hills) | Append the 10 Plains badges (section 3), page `Episode.Plains`. |
| new `Play/StrawBounceWatcher.cs` (level part) | — | Section 5. |
| `Editor/LevelThemes.cs` | 29 themes | The three theme entries and props in section 8. |
| `Editor/LevelBuilder.cs` and the level object builders | — | Build each straw pile: a solid like a boulder (48 wide, 16 tall) with the `straw_pile` sprite, registered with the watcher. Missing-art errors name `Tools/Art/make_plains_easy_art.py`. |
| `Editor/SelectionBuilder.cs` `MenuLife` | `sets[Plains] = CloudOnly(...)` | A `PlainsMenuLife()` like `HillsMenuLife()` (section 8). |
| `Editor/LandformMapBuilder.cs` | Plains has no life | A `PlainsLife()` like `HillsLife()`, shown only while Plains is open (section 8). |
| `Editor/LandformLife.cs` | `HillsSprite(...)` helpers | Add the same kind of helper for the `Plains/` map sprites, or make the existing helper take the folder. Hills sprites must load exactly as before. |
| Tests that say Plains is empty | `CollectionTests` (`LandformsWithoutItemKinds_AreComingSoon`, `FreshProfile_HasNothingFound` total 50, `ShelfFoundCount_CountsOnlyThatShelf` Plains 0), `LandformSummaryTests`, `LevelCatalogTests`, `AchievementTests`, progression tests | Update them to the new facts: the Plains shelf has 10 kinds; the total is 54; Plateaus is still "Coming soon". Do not delete what they check about Mountains or Hills. |

Already generic, no change expected: the unlock order and lock guide (`RegionGuide`), `MaxStarsFor`, scene names (`PlainsEasy1`), the save format, the Collection and badge pages with their arrows, "Coming soon" for difficulties without levels, Next hidden when the next level has no data, and the Description Screen flow.

## 5. Straw bounce pile: exact rules

- New file `Rules/StrawPile.cs`:
  - `public readonly struct StrawPile` (or a small class like `SandZone`) with `public float X` (the left edge, in level pixels on the ground line).
  - `public const float Width = 48f;` and `public const float Height = 16f;`
  - `public const float LaunchSpeed = 400f;` (px/s straight up; about 89 px of rise with gravity 900).
  - `public const float SquashSeconds = 0.15f;` (how long the squashed picture shows after a bounce).
  - `public bool IsOnTop(float explorerMiddleX, float explorerHalfWidth, float feetY)`: true when the explorer's body overlaps the pile across (`explorerMiddleX + explorerHalfWidth > X + 1` and `explorerMiddleX − explorerHalfWidth < X + Width − 1`) and `|feetY − (Height − 1)| ≤ 0.5`. The `Height − 1` matches the solid box (top at 15, the way boulders use `height − 1`).
- The pile's solid box is `x + 1 … x + Width − 1` across and `0 … Height − 1` up, exactly like a small boulder's box rule.
- New level part `Play/StrawBounceWatcher.cs` (with `Begin(LevelController)` and `IsBuilt`, like `SandWatcher`):
  - Each step, when the explorer is grounded and `IsOnTop` is true for a pile, call the existing `ExplorerMotor.Launch(StrawPile.LaunchSpeed)` (the steam vent's launch; it keeps the double jump ready).
  - Play `sfx_straw_bounce.wav` through `LevelController.PlaySfx` at volume 0.7.
  - Swap that pile's sprite to `straw_pile_squash` for `SquashSeconds`, then back to `straw_pile`. Show `Props/straw_bits_00` then `_01` (0.1 s each) just above the pile's middle.
  - Read `LevelClock.Now` for the squash and bits timing, so they pause with the game.
- Fairness (checked by the bot lint): no pile within 60 px of a pit or spikes, within 40 px of a boulder or the checkpoint, before x 200 or after goal − 120. The first pile has no pit or spikes within 80 px.
- Tests (EditMode):
  - `IsOnTop` at both side edges, just outside them, at feet 15, and at feet 14 and 16.5 (false).
  - `LaunchSpeed` gives a rise of 400² / (2 · 900) ≈ 88.9 px.
  - Plains Easy 3 has 4 piles; the first is safe (no pit or spikes within 80 px); no pile is within 40 px of the checkpoint; no other level has piles.
  - Mountains and Hills levels have no piles.

## 6. Level data: Plains Easy 1–3

Coordinates are in pixels: x across from the level start, y up from the ground line. Every level starts at (40, 0) like Hills Easy. `S(x)` is a small boulder 32×24, `B(x)` a big boulder 48×36. `L(x, top, width)` is a ledge. Spikes are 32 wide. The exact numbers are in `layouts.md` too.

### Plains Easy 1: "Central Luzon Plain" (theme RicePlain)

- Width 4800. Goal at 4740 (Rice Granary). Checkpoint at 2400.
- Pits: 1500 (48 wide), 3300 (48). Spikes: 2900.
- Boulders: S(400), B(1000), S(1900), B(2700), S(3600), B(4200).
- Ledges: L(600, 32, 80), L(1200, 28, 64), L(2000, 40, 64), L(3000, 28, 80), L(3800, 28, 48), L(3860, 56, 64), L(3940, 84, 64).
- Pickups:
  - clay (416, 24), on the first boulder
  - sandstone (640, 32), on a low dike
  - shale (2032, 40)
  - siltstone (3972, 84), **secret spot**: the top of the three-step climb
  - clay (4600, 16), near the granary

### Plains Easy 2: "Pangasinan Salt Beds" (theme SaltBeds)

- Width 5400. Goal at 5240 (Salt Hut). Checkpoint at 2700.
- Pits: 800 (48), 3500 (56). Spikes: 1700, 4000.
- Boulders: S(400), B(1400), S(2300), S(4400), B(4800).
- Ledges: L(1100, 28, 80), L(2000, 40, 64), L(3000, 40, 64), L(3080, 84, 72), L(3250, 28, 64), L(4600, 28, 64).
- Pickups:
  - halite (416, 24)
  - gypsum (1132, 28)
  - quartz (2032, 40)
  - siltstone (3112, 84), **secret spot**: climb from the ledge at 40 to the one at 84 (review kind)
  - halite (4416, 24)

### Plains Easy 3: "Isabela River Plain" (theme RiverPlain)

- Width 5400. Goal at 5240 (Farm Shed). Checkpoint at 2500.
- Pits: 900 (48), 2100 (56), 3800 (56). Spikes: 1500, 3000, 3100 (a pair, with halite between them), 4300.
- Boulders: S(350), B(1700), S(2800), B(4050), S(4700).
- Ledges: L(1240, 96, 64) (above the second pile), L(1900, 40, 64), L(2300, 28, 80), L(3360, 120, 56) (the secret ledge above the third pile).
- Straw piles: 600 (learn on safe ground), 1200 (bounce up to the mudstone ledge), 3300 (bounce + double jump to the secret ledge), 4500 (a fun bounce before the goal).
- Pickups:
  - conglomerate (366, 24), on the first boulder
  - mudstone (1272, 96), on the ledge reached by bouncing on the pile at 1200
  - halite (3066, 0), between the spike pair (review kind)
  - limestone (3388, 120), **secret spot**: only a bounce plus a double jump reaches it
  - conglomerate (4716, 24)

### Bot check (2026-11-21)

The scratch bot (`.scratch/plains-easy/bot-check-*.cs.txt`; working copy `bot/`) is the Hills Hard sim with the straw pile added and the Easy driver. Copy `Sim.cs`, `Program.cs`, `PlainsLevels.cs`, `EasyLevels.cs` and `bot.csproj` into an empty folder and run `dotnet run -c Release`.

```
check: straw 600: bounces 1, highest feet 100.6 px (15 + 400^2/1800 = 103.9)
check: straw 600 plus a double jump at the top: highest feet 128.0 px (+ 230^2/1800 = 29.4)
check: walking into the pile's side: bounces 0, x 595.0 (stops at 595)
-- Regression, Hills Easy at walk 84 (expect E1 56.8, E2 62.4, E3 70.1):
E1 56.8  E2 62.4  E3 70.1
-- Plains Easy:
E1 Central Luzon Plain: bot GOAL 56.8s (goal); bounces 0; pickups 5/5 reachable lint clean
E2 Pangasinan Salt Beds: bot GOAL 62.4s (goal); bounces 0; pickups 5/5 reachable lint clean
E3 Isabela River Plain: bot GOAL 62.2s (goal); bounces 4; pickups 5/5 reachable lint clean
   without straw piles: needs a pile -> pickups [mudstone(1272,96) limestone(3388,120)], ledges [L(1240,96) L(3360,120)]
```

The last line shows the piles are needed: without them the mudstone ledge and the secret ledge cannot be reached. A 32 px pile was tried first; a walking jump from right next to it flew over it, so the pile is 48 px wide.

## 7. Content (drafts, Q-04: needs teacher review)

### Lessons (4 sentences, each at most 44 characters)

**E1 "Plains"** (picture `lesson_central_luzon`):
1. Plains are wide, flat and low lands.
2. Central Luzon has our biggest plain.
3. Nueva Ecija is the Rice Granary.
4. Flat land is easy to farm and build on.

**E2 "Coastal Plains"** (picture `lesson_pangasinan`):
1. Some plains lie flat beside the sea.
2. Pangasinan means place of salt.
3. Sea water dries in ponds and leaves salt.
4. That salt is a mineral called halite.

**E3 "River Plains"** (picture `lesson_isabela`):
1. Rivers can build plains, too.
2. The Cagayan is our longest river.
3. Floods spread silt, sand and clay.
4. Farmers grow crops on this rich soil.

### Reviews (3 sentences, shown on the Result Screen)

- **E1:** "Plains are wide, flat and low lands." / "Plain soil often has clay, silt and sand." / "Sand hardens into sandstone; silt into siltstone."
- **E2:** "Coastal plains lie flat beside the sea." / "Drying sea water leaves halite and gypsum." / "Beach sand is mostly quartz, a tough mineral."
- **E3:** "Rivers spread soil when they flood a plain." / "Hardened mud and clay become mudstone." / "River gravel can harden into conglomerate."

### New kinds (Plains only)

| Id | Name | Rock? | Description | Picture |
|---|---|---|---|---|
| `siltstone` | Siltstone | rock | A smooth rock made of silt, finer than sand. Silt settles in quiet water. | `siltstone` |
| `halite` | Halite | mineral | Rock salt. It forms when salty sea or lake water dries up. | `halite` |
| `gypsum` | Gypsum | mineral | A very soft white mineral left by drying sea water. It is used for plaster and chalk. | `gypsum` |
| `mudstone` | Mudstone | rock | A fine rock made from hardened mud and clay. | `mudstone` |

Shared kinds keep their existing text and pictures.

### Start banners and goals

E1 "Central Luzon Plain" (goal "Rice Granary"), E2 "Pangasinan Salt Beds" ("Salt Hut"), E3 "Isabela River Plain" ("Farm Shed").

### Fact sources (checked 2026-11-21, Wikipedia; see `kinds-research.md`)

- **Central Luzon:** "contains the largest plain in the country"; Nueva Ecija is the "Rice Granary of the Philippines" and has the largest rice yield (Central Luzon, Nueva Ecija pages).
- **Pangasinan:** the name means "place of salt" or "place of salt-making", from "asin"; a major salt producer (Pangasinan page).
- **Cagayan River:** the longest river in the Philippines (about 505 km); "subject to extensive flooding"; an alluvial plain from Ilagan to Tumauini (Isabela); tributaries deposited "mostly limestone sands and clays" (Cagayan River page). The limestone pickup in E3 rests on this.
- **Floodplain soils** "usually consist of clays, silts, sands, and gravels" (Floodplain page).
- **Plain:** "a flat expanse of land … few rapid or steep changes in elevation"; plains form from deposits by water, ice or wind, or from lava (Plain page).
- **Halite:** rock salt; "formed from the evaporation of seawater or salty lake water" (Halite page).
- **Gypsum:** Mohs 2, "a soft sulfate mineral"; "deposited from lake and sea water"; used in plaster and "blackboard or sidewalk chalk" (Gypsum page).
- **Siltstone:** "composed mostly of silt"; forms in "relatively quiet depositional environments" (Siltstone page).
- **Mudstone:** a fine-grained rock "whose original constituents were clays or muds" (Mudstone page). Where it forms is not stated on that page, so the description does not say.
- **Not confirmed:** that Isabela grows mostly corn (the E3 props show corn; the lesson says only "crops"). The teacher should check the Isabela farming fact and every draft sentence.

## 8. Assets (made by Claude; do not edit)

Scripts and their outputs:
- `python Tools/Art/make_plains_easy_art.py`
  - **Level sprites** (`Assets/Art/Level/Sprites/`): `rice_top`, `rice_soil`, `ledge_rice`, `rice_granary` (56×48), `salt_top`, `salt_soil`, `ledge_salt`, `salt_hut` (56×48), `river_top`, `river_soil`, `ledge_river`, `farm_shed` (56×48), `boulder_small_*` / `boulder_big_*` for `_rice`, `_salt` and `_river`, `straw_pile` (48×16), `straw_pile_squash` (48×16).
  - **Props** (`Props/`): `straw_bits_00/01` (12×6), `carabao_00/01` (24×16), `egret_00/01` (12×10), `rice_tuft` (12×10), `salt_mound` (16×8), `corn_stalk_00/01` (10×28).
  - **Items:** `siltstone`, `halite`, `gypsum`, `mudstone` (24×24).
  - **Lesson pictures:** `lesson_central_luzon`, `lesson_pangasinan`, `lesson_isabela` (120×82).
  - **Map and menu life** (`Assets/Art/LandformMap/Sprites/Plains/`): `rice_0/1` (7×6), `egret_0/1` (5×3), `carabao_0/1` (9×6), `grass_0/1` (7×6).
  - Preview: `.scratch/plains-easy/art-preview.png`.
- `python Tools/Art/make_plains_easy_backdrops.py`: `Backdrop/RicePlain/`, `Backdrop/SaltBeds/`, `Backdrop/RiverPlain/`, each L0–L5 (960×270). Preview: `.scratch/plains-easy/backdrop-preview.png`.
- `python Tools/Art/make_profile_art.py`: the 10 Plains badges, normal and locked. Preview: `.scratch/plains-easy/badge-preview.png`.
- `python Tools/Audio/make_plains_audio.py`: `bgm_plains.ogg` (40 s loop), `amb_grass_easy.ogg`, `amb_wetland_normal.ogg`, `amb_dry_hard.ogg` (24 s loops), `sfx_straw_bounce.wav`. Preview: `.scratch/plains-easy/audio-preview.png`.

### Theme table for `LevelThemes.cs`

| Theme | GroundTop / Soil / Ledge / BoulderEnding | Goal (name, sprite, offset) | Backdrop | Scripts |
|---|---|---|---|---|
| RicePlain | `rice_top` / `rice_soil` / `ledge_rice` / `_rice` | "Rice Granary", `rice_granary`, (24, 24) | `Backdrop/RicePlain/` | `make_plains_easy_art.py`, `make_plains_easy_backdrops.py` |
| SaltBeds | `salt_top` / `salt_soil` / `ledge_salt` / `_salt` | "Salt Hut", `salt_hut`, (24, 24) | `Backdrop/SaltBeds/` | same |
| RiverPlain | `river_top` / `river_soil` / `ledge_river` / `_river` | "Farm Shed", `farm_shed`, (24, 24) | `Backdrop/RiverPlain/` | same |

The goal offsets follow the Hills 56×48 goals (`viewing_deck` (24, 24)). Check each goal in the Editor and report if one floats or sinks.

### Living props (decoration only; keep them off pits, ledges, boulders and straw piles with the existing `OverPit` / `PropFits` / `HillsPropFits` helpers; report any prop moved more than 64 px)

- **RicePlain:**
  - A rice tuft about every 140 px from x = 90, order 7, centre y = 5.
  - Carabao at x = 700, 2200 and 4000. Order 4, centre y = 8, frames 00/01 swapping every 1.5–2.5 s (different for each).
  - Egrets at x = 1100, 2600 and 3500. Order 4, centre y = 5, the same frame swap.
- **SaltBeds:**
  - A salt mound about every 360 px from x = 240, order 3, centre y = 4.
  - A palm tree (`Props/palm_tree`, already made for Hills) about every 600 px from x = 450, order 3, centre y = 20.
  - Egrets at x = 1300 and 3900, as above.
- **RiverPlain:**
  - A corn stalk about every 120 px from x = 140, order 3, centre y = 14, frames 00/01 at 1.2–1.8 frames per second.
  - Carabao at x = 1000 and 4250, as above.
  - Never on a straw pile or within 8 px of one.

### Map life (`LandformMapBuilder`)

The Plains region is bottom left: label (60, 248), padlock (170, 236) in the map's canvas coordinates. Place the life inside the Plains area of the map picture the same way `HillsLife()` places the Hills life; the exact pixel spots are in `layouts.md` ("Plains map life").

- Six rice clumps (`rice_0/1`, 1.2–1.8 frames per second, each a little different).
- Two egrets (`egret_0/1`, 3 frames per second) on a slow `CircleFlight` above the region, radius about 20 px.
- One carabao (`carabao_0/1`, a swap every 2 s).
- All of it is active only while Plains is open, like the Hills life.

### Menu life (`SelectionBuilder`)

- **Plains:** the cloud, two egrets (frames `egret_0/1` on a `CircleFlight` around screen (330, 60), radius (24, 10), 12 s and 15 s per lap) and six grass tufts (`grass_0/1`, 1.5 frames per second) along the bottom at x = 30, 110, 190, 290, 370 and 450, on screen row 268.
- Mountains and Hills: as now.

### Audio table (`LandformAudio`)

| Episode | Music | Easy | Normal | Hard |
|---|---|---|---|---|
| Mountains | `bgm_mountains` | `amb_breeze_easy` | `amb_wind_normal` | `amb_wind_hard` |
| Hills | `bgm_hills` | `amb_stream_easy` | `amb_stream_normal` | `amb_rain_hard` |
| **Plains** | **`bgm_plains`** | **`amb_grass_easy`** | **`amb_wetland_normal`** | **`amb_dry_hard`** |
| Plateaus | not planned yet: use the Mountains files | | | |

## 9. Not in scope

- Plains Normal 1–5 and Hard 1–7: levels, lessons, kinds, objects, backgrounds.
- Plateaus content and the Completion Screen.
- Any change to Mountains or Hills levels, numbers or text.
- Refactoring the 19-parameter `LevelLayout` constructor (a known review finding; not part of this build).
