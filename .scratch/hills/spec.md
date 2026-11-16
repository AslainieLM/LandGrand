# Hills Easy 1–3 and the Hills episode outline (D-066)

Planned on 2026-11-16 in a grilling session (rounds Q1–Q10). The user accepted every recommendation. This spec covers:

- the **episode outline** for all 15 Hills levels (hills, topics, themes, badges, Description Screen), and
- the **full build** of Hills Easy 1–3, plus the code changes that make LandGrand work with more than one landform.

Hills Normal 1–5 and Hard 1–7 get their own specs after the user has played Easy (same as Mountains). Their kinds, exact object numbers and backgrounds are chosen then.

## 1. Rules that are already settled (do not change)

- Episode order is Mountains → Hills → Plains → Plateaus. Hills opens when all 15 Mountains levels are cleared (D-039, D-065). The unlock code is already generic.
- Each episode has 3 Easy, 5 Normal and 7 Hard levels (Chapter III P066).
- Easy levels have 5 pickups, 1 checkpoint and a 120 s limit. Pits and spikes send the explorer a short way back (D-048, D-056).
- Stars: 0–40% gives 1 star, 41–99% gives 2, and 100% gives 3 (D-039).
- One secret-spot pickup per level, a 1.5 s start banner, living props, and teach-test-twist pacing (D-055).
- One script runs every level. Level differences are data in `LevelCatalog`. Rules live in `Assets/Scripts/Rules`, with no Unity types, and are covered by tests (D-061).
- Every level theme has its own six-layer background (D-063).
- All lesson, review, kind and summary text is a **draft** (`isDraft: true`, "Draft — needs teacher review"). Q-04 stays open (D-043).
- Mountains Easy, Normal and Hard must play exactly as before.

## 2. New decisions (D-066)

1. **Plan scope:** an outline for all 15 levels, and a full spec for Easy 1–3 only.
2. **Shared kinds:** a kind can belong to more than one landform's shelf. It is saved once and counted once by Rock Scholar, but it shows on every shelf it belongs to. `ItemKind.HomeLandform` becomes a list of landforms.
3. **Per-landform audio:** the level music depends on the landform, and the ambience depends on the landform and the difficulty.
4. **Soft sand** (Easy 3): while the explorer is on soft sand, walking speed drops to 60%. Jumps and speed in the air do not change. Sand never hurts the explorer and never sends them back.
5. **Badges per landform:** the Achievements tab gets landform tabs like the Collection, with one 5×3 page per landform. Mountains keeps its 15 badges. The four general badges and Rock Scholar stay on the Mountains page. Hills adds 10 badges.
6. **Description Screen badges** count only that landform's page (Mountains n/15, Hills n/10).
7. **Map:** the open Hills region shows swaying trees, a shining stream and two birds (D-041). The cloud the Mountains Hard build added over Hills is removed. Plains follows the same rule Hills did: "Finish Hills first" with a padlock, then a one-time unlock animation and "Coming soon".
8. **Menu screen life:** behind the difficulty, level and lesson screens, Mountains keeps its snow and eagle. Hills shows two birds and swaying grass. The cloud shows for every landform.
9. **A difficulty with no level data** shows "Coming soon" when it is open (for now, Hills Normal and Hard after Easy is cleared). On the Result Screen, **Next is hidden** when the next level has no data; Back to Map and Replay stay.
10. **The builder menu** "LandGrand > Build Mountains Levels" is renamed **"LandGrand > Build Levels"**, because it builds every level in `LevelCatalog`.
11. **N2 bats** (outline only): touching a bat pushes the explorer back 24 px gently and never sends them to a checkpoint. The lesson says "Do not disturb bats in caves."

## 3. Episode outline (all 15 levels)

| Level | Real hill | Topic | Theme | New object | Kinds |
|---|---|---|---|---|---|
| E1 | Chocolate Hills, Bohol | What a hill is | ChocolateHills | none | limestone\*, calcite\*, clay\*, coral stone |
| E2 | Vayang Rolling Hills, Batanes | Grass holds soil; grazing | RollingPasture | none (grazing animals as props) | andesite\*, limonite, kaolinite, + coral stone review |
| E3 | La Paz Sand Dunes, Ilocos Norte | Wind makes dunes | SandDunes | **soft sand** | quartz\*, magnetite\*, feldspar\*, + limonite review |
| N1 | Hundred Islands, Pangasinan | Hills from old coral reefs | IslandHills | sinking float | 2 kinds, chosen in the Normal spec |
| N2 | Callao Cave, Cagayan | Caves inside limestone hills | LimestoneCave | gentle bats | 2 kinds |
| N3 | Pagsanjan Gorge, Laguna | Rivers wear valleys | RiverGorge | river current (wind zone with a direction) | 2 kinds |
| N4 | Banaue Rice Terraces, Ifugao | Farming on slopes ("hill and mountain slopes") | RiceTerraces | mix of N1–N3 | 2 kinds |
| N5 | Cappadocia, Türkiye | Wind and rain carve fairy chimneys | FairyChimneys | hot-air balloons (vertical moving platforms) | 2 kinds |
| H1 | Tarsier forest, Corella, Bohol | Hills are homes for animals | TarsierForest | slippery mud (ice rule, mud art) | 2 kinds |
| H2 | Hills of Antipolo, Rizal (*source to check*) | Landslide warning signs and safety | RainySlope | rolling log (rolling boulder with a direction) | 2 kinds |
| H3 | Masungi Georeserve, Rizal | Rain shapes limestone; people protect hills | KarstRidge | mix | 2 kinds |
| H4 | Pililla Wind Farm, Rizal | Windy hills and wind power | WindHills | mix | 2 kinds |
| H5 | Clew Bay drumlins, Ireland | Glaciers leave hills | DrumlinBay | mix | 2 kinds |
| H6 | Seven Hills of Rome, Italy | Cities built on hills | CityHills | mix | 2 kinds |
| H7 | Guilin karst hills, China | Tower limestone hills | KarstTowers | mix | 2 kinds |

\* = a shared kind that is already on the Mountains shelf.

The Hills shelf has 10 kinds after Easy and about 34 after Hard, shared kinds included. Each Normal and Hard kind needs a geological source when its spec is written (Chapter II P026).

**Badges (all 10 get their rules and art now):**

| Id | Name | Tier | Rule | Hint |
|---|---|---|---|---|
| `pasture_pal` | Pasture Pal | silver | clear Hills Easy 2 | Clear Hills Easy 2 |
| `dune_surfer` | Dune Surfer | silver | clear Hills Easy 3 | Clear Hills Easy 3 |
| `rolling_star` | Rolling Star | gold | 3 stars on all 3 Hills Easy levels | Get 3 stars on every Hills Easy level |
| `hill_collector` | Hill Collector | gold | find every kind on the Hills shelf | Find every Hills rock and mineral |
| `philippine_hills` | Philippine Hills | silver | clear Hills Normal 1–4 | Clear Hills Normal 1 to 4 |
| `balloon_rider` | Balloon Rider | gold | clear Hills Normal 5 | Clear Hills Normal 5 |
| `slope_ranger` | Slope Ranger | silver | clear any Hills Hard level | Clear a Hills Hard level |
| `world_hills` | World Hills | silver | clear Hills Hard 5, 6 and 7 | Clear Hills Hard 5 to 7 |
| `steady_star` | Steady Star | gold | 3 stars on all 7 Hills Hard levels | Get 3 stars on every Hills Hard level |
| `hill_master` | Hill Master | gold | clear all 15 Hills levels (`IsEpisodeComplete(Hills)`) | Clear every Hills level |

Badges whose levels have no data yet simply stay locked.

**Hills Landform Description Screen** (its data is added now; it is reached only after Hills Hard 7 exists):
- Title "Hills", with the draft summary: "Hills are lower and rounder than mountains." / "Rain, rivers, wind and ice shape hills." / "Many hills are made of limestone or sand." / "Grass and trees hold hill soil in place." / "People farm, live and use wind on hills."
- Totals: stars n/45, kinds n/(Hills shelf), badges n/10.
- "Plains is now open!", with "New levels coming soon" while Plains has no levels.
- The `bg_hills` picture with the Hills birds and grass (no snow), and the same `sfx_fanfare`.

## 4. Code changes so more than one landform works

These places currently assume Mountains is the only landform (found 2026-11-16):

| Where | Now | Change |
|---|---|---|
| `Rules/LevelData.cs` `ItemKind` | `HomeLandform` (one episode) | `Landforms` (array of episodes), plus `BelongsTo(Episode)`. Mountains-only kinds keep `{ Mountains }`. limestone, calcite, clay, andesite, quartz, magnetite and feldspar become `{ Mountains, Hills }`. `Collection`/`Shelf` use `BelongsTo`. |
| `Rules/LevelLayout.cs` `LevelTheme` | 14 Mountains themes | Add `ChocolateHills`, `RollingPasture` and `SandDunes` at the **end** of the enum. |
| `Editor/PixelSceneKit.cs:62-82` | `LevelMusic` is always `bgm_mountains`; `LevelAmbience(Difficulty)` | Read the file names from a new Rules table, `LandformAudio.MusicFor(Episode)` and `LandformAudio.AmbienceFor(Episode, Difficulty)`. `LevelBuilder.cs:173` passes `level.Episode`. |
| `Rules/LandformSummary.cs` | Hills returns "Coming soon" | Hills returns the 5 draft lines above with picture `bg_hills`. Plains and Plateaus stay "Coming soon". |
| `Rules/Achievements.cs` | 15 badges; 11 hard-code Mountains; `world_peaks` uses `Enumerable.Range(5,3)` | Each badge has a `Landform` (its page). Add the 10 Hills badges. Mountains- and Hills-specific rules come from small helpers that take the episode and level numbers, so `world_peaks` reads `{5, 6, 7}` as data. Ids and their order in the save stay as they are. |
| `Editor/ProfileBuilder.cs:151-153`, `Profile/ProfileController.cs` | One 5×3 badge grid with no landform tabs | Achievements gets the same four landform tabs as the Collection (reuse the `land_*` sprites and the lock guide via `RegionGuide.ForLocked`). One 5×3 page per landform. "Badges n/N" counts the shown landform. |
| `Common/LandformDescriptionController.cs:48` | Badges = all earned / all | Badges for this landform's page only. |
| `Common/LandformBackdrop.cs:14,20`, `Editor/SelectionBuilder.cs` | Snow only for Mountains; cloud and eagle for all | Life per landform: Mountains = snow and eagle, Hills = birds and grass, cloud for every landform. |
| `Editor/LandformMapBuilder.cs:89-95` | Hills life is one reused Mountains cloud | Remove that cloud. Add the Hills life set (section 9). |
| `Editor/LevelBuilder.cs:15,17,43,72`, `Play/LevelController.cs:59` | Menu "Build Mountains Levels" | Rename it to "LandGrand/Build Levels". Update the log and error text. |
| Difficulty and Level Selection | Assume every open difficulty has levels | An open difficulty with no level data shows "Coming soon" (reuse the map's guide text). |
| `Play/ResultScreen.cs` | Next goes to the next level | Hide Next when the next level has no data. |
| Tests that check Hills is empty | `CollectionTests.cs:38-44,53`, `LevelCatalogTests.cs:27`, `EpisodeCompletionTests.cs:36`, `LandformSummaryTests.cs:19-27`, `AchievementTests.cs:14,35`, `HardProfileTests.cs:9`, `MountainsHardTests.cs:36` | Update them to the new facts. Do not delete what they check about Mountains. |

Already generic, no change: the unlock order, `MaxStarsFor` (45 for 15 levels), scene names (`HillsEasy1`), the save format (`e` = 1 for Hills), `LandGrand.UnlockSeen.<Episode>`, `RegionGuide`, and the Result Screen's Next Episode.

## 5. Soft sand: exact rules

- New rule `Rules/SoftSand.cs`:
  - `public const float WalkShare = 0.6f;` (60% of the walking speed is left on sand).
  - `public static float WalkSpeed(float walk, bool onSand)` returns `walk * WalkShare` on sand and `walk` otherwise.
  - `public const float StepSeconds = 0.3f;` (one step sound and one sand puff every 0.3 s while walking on sand).
- New data `SandZone(x, width)` in `LevelLayout`, as an optional `sandZones` array that defaults to empty (like `iceZones`). `Covers(px)` is true from `x` to `x + width`, both edges included.
- The explorer is "on sand" only while grounded on the flat ground line inside a zone. It is not on sand while standing on a ledge, a boulder or a moving solid (same test as `IceWatcher`).
- New level part `Play/SandWatcher.cs` (with `Begin(LevelController)` and `IsBuilt`):
  - It sets the walk-speed share on `ExplorerMotor`, next to the ice code at `ExplorerMotor.cs:109-111`. Air speed is not changed.
  - Every 0.3 s while walking on sand, it plays `sfx_sand_step.wav` at volume 0.5 and shows `Props/sand_puff_00` then `_01` (0.15 s each) at the explorer's feet.
  - It reads `LevelClock.Now`, so it pauses with the game.
- Builder: over each zone, swap the theme's ground-top tile for `soft_top` (same way as the ice tile swap in `HardLevelObjects.cs:52-65`).
- Fairness: the first zone is on safe ground, with no pit or spikes within 80 px. No zone is within 40 px of a checkpoint. Walking on sand loses time, so the bot run must stay well under 120 s.
- Tests: `WalkSpeed` on and off sand; `Covers` at both edges and just outside them; Hills Easy 3 zone checks (first zone safe, none near the checkpoint); Mountains levels have no zones.

## 6. Level data: Hills Easy 1–3

Coordinates are in pixels: x across from the level start, y up from the ground line. Every level starts at (32, 0). `B(x, size)` is a boulder (small 32×24, big 48×36). `L(x, top, width)` is a ledge.

### Hills Easy 1: "Chocolate Hills" (theme ChocolateHills)

- Width 4800. Goal at 4740 (the viewing deck). Checkpoint at 2400.
- Pits: 1880 (48 wide), 3150 (48). Spikes: 3450.
- Boulders: B(300, Small), B(900, Big), B(1400, Small), B(2150, Big), B(2700, Small), B(3300, Big), B(3900, Small), B(4300, Big).
- Ledges: L(520, 40, 80), L(1100, 28, 80), L(1600, 40, 64), L(2950, 28, 80), L(3560, 28, 48), L(3620, 56, 96), L(4000, 28, 80).
- Pickups:
  - limestone (316, 24), on the first boulder
  - calcite (560, 40), on a ledge
  - clay (1140, 28), on a low ledge
  - coral stone (3668, 56), **secret spot**: the high ledge after the step
  - limestone (4600, 16), near the deck

### Hills Easy 2: "Batanes Hills" (theme RollingPasture)

- Width 5400. Goal at 5240 (the stone house). Checkpoint at 2700.
- Pits: 700 (48), 3380 (56). Spikes: 1800, 3800.
- Boulders: B(360, Small), B(1550, Big), B(2350, Small), B(4300, Small), B(4700, Big).
- Ledges: L(1300, 28, 80), L(2050, 40, 64), L(3000, 40, 64), L(3080, 84, 72), L(3300, 28, 64), L(3452, 28, 64), L(4900, 28, 64).
- Pickups:
  - andesite (376, 24)
  - limonite (1332, 28)
  - kaolinite (2082, 40)
  - coral stone (3112, 84), **secret spot**: climb from the ledge at 40 to the one at 84
  - andesite (4316, 24)

### Hills Easy 3: "La Paz Dunes" (theme SandDunes)

- Width 5400. Goal at 5240 (the dune marker). Checkpoint at 2500.
- Pits: 800 (48), 2800 (64, the double-jump pit), 3900 (56). Spikes: 1300, 3200, 3300 (a pair, with limonite between them).
- Boulders: B(1050, Big), B(1800, Small), B(4400, Small), B(4700, Big), B(5000, Small).
- Ledges: L(300, 24, 64), L(380, 48, 64), L(460, 72, 80), L(1500, 40, 64), L(2000, 28, 80), L(3500, 28, 56), L(3580, 56, 56), L(3480, 84, 64).
- Soft sand: (560, 160) to learn on safe ground, (1900, 400), (2640, 150) just before the wide pit, and (4050, 330).
- Pickups:
  - quartz (500, 72), on top of the stepped climb
  - magnetite (1532, 40)
  - limonite (3266, 0), between the spike pair
  - feldspar (3512, 84), **secret spot**: climb back up to the high ledge
  - quartz (4416, 24)

### Bot check (2026-11-16)

The scratch bot (`.scratch/hills/bot-check-*.cs.txt`) is the Mountains Hard bot with soft sand added. To run it, copy the three files into an empty folder as `Sim.cs`, `Levels.cs` and `bot.csproj`, then run `dotnet run -c Release`.

```
E1 Chocolate Hills: bot GOAL 66.0s (goal); pickups 5/5 reachable; time on sand 0.0s
E2 Batanes: bot GOAL 72.7s (goal); pickups 5/5 reachable; time on sand 0.0s
E3 La Paz: bot GOAL 81.1s (goal); pickups 5/5 reachable; time on sand 21.1s
```

There were no lint warnings: no hazard within 100 px of a checkpoint, no pit at the start or the goal, the first sand zone is safe, and no sand is near the checkpoint. The Mountains Easy times were 66, 73 and 73 s.

## 7. Content (drafts, Q-04: needs teacher review)

### Lessons (4 sentences, each at most 44 characters)

**E1 "Hills"** (picture `lesson_chocolate_hills`):
1. Hills are lower and rounder than mountains.
2. Bohol has over 1,200 Chocolate Hills.
3. They are made of old sea limestone.
4. Dry grass turns them brown like chocolate.

**E2 "Rolling Hills"** (picture `lesson_batanes`):
1. Batanes has wide, rolling green hills.
2. Grass roots hold the hill soil in place.
3. Cows and goats eat grass on the hills.
4. Too much grazing can leave soil bare.

**E3 "Sand Dunes"** (picture `lesson_la_paz`):
1. Wind can pile sand into hills called dunes.
2. La Paz in Ilocos Norte has big sand dunes.
3. Sea and river bring the sand to the coast.
4. Plants on dunes help keep the sand still.

### Reviews (3 sentences, shown on the Result Screen)

- **E1:** "Hills are lower and rounder than mountains." / "The Chocolate Hills are limestone from an old sea floor." / "Limestone has calcite in it, and the hills sit on clay."
- **E2:** "Rolling hills have gentle, rounded slopes." / "Grass and roots keep hill soil from washing away." / "Feldspar in rock slowly breaks down into kaolinite clay."
- **E3:** "Dunes are hills of sand piled up by the wind." / "Most sand grains are quartz, a tough mineral." / "The black grains in Ilocos sand are magnetite."

### New kinds (Hills only)

| Id | Name | Rock? | Description | Picture |
|---|---|---|---|---|
| `coral_stone` | Coral Stone | rock | A pale rock made from old coral. The Ivatan build strong stone houses with it. | `coral_stone` |
| `limonite` | Limonite | mineral | A yellow-brown iron mineral. It gives many soils their rusty color. | `limonite` |
| `kaolinite` | Kaolinite | mineral | A soft, white clay mineral. It forms when feldspar slowly breaks down. | `kaolinite` |

Shared kinds keep their Mountains text and pictures.

### Start banners

E1 "Chocolate Hills", E2 "Batanes Hills", E3 "La Paz Dunes".

### Fact sources (checked 2026-11-16)

- **Chocolate Hills:** count estimates run from about 1,260 to 1,776. Most hills are 30–50 m tall and the tallest is about 120 m. They are marine limestone with coral and shell fossils, resting on hardened clay, and weathering shaped them. Sources: Wikipedia "Chocolate Hills"; the Bohol provincial tourism geologic history; the Bohol Island UNESCO Global Geopark page. The lesson says "over 1,200", which every count supports.
- **Batanes:** Batanes is on the Luzon volcanic arc (Mt. Iraya). Ivatan houses are built of limestone and coral, and cattle graze the rolling hills. Sources: travel guides only, so the teacher should check this. Vayang's own rock types were not found in a source.
- **La Paz:** the dunes were made by wind, sea and river working together. They are a National Geological Monument (1993), about 85 km². Sources: Laoag City tourism; MGB Ilocos Norte geosites.
- **Dune sand is mostly quartz:** Britannica "sand dune"; OneGeology Kids.
- **Ilocos Norte black sand is magnetite:** Inquirer reports on magnetite sand permits off Laoag, Bacarra and Paoay; the MGB statement on Region 1. **Ilmenite was not confirmed, so E3 uses feldspar** (the Q3 fallback).
- **Wording cautions:** say "over 1,200", not an exact count. "Hills are lower and rounder than mountains" is a general rule; there is no fixed height that separates hills from mountains.

## 8. Profile and Collection

- The Hills Collection tab now lists 10 kinds on one page (6×2 grid, 12 per page). The page arrows stay hidden.
- Achievements: landform tabs; Mountains page with 15 badges (5×3); Hills page with 10 (two rows). Plains and Plateaus show "Coming soon" when open, and the lock guide when locked.
- Badge art: `badge_<id>.png` and `badge_<id>_locked.png` in `Assets/Art/MainMenu/Sprites/Profile/` (already made).

## 9. Assets (already made and checked; do not edit)

Scripts and their outputs:
- `python Tools/Art/make_hills_easy_art.py`
  - **Level sprites:** `choc_top`, `choc_soil`, `ledge_choc`, `viewing_deck` (56×48), `pasture_top`, `pasture_soil`, `ledge_pasture`, `stone_house` (56×48), `dune_top`, `dune_soil`, `ledge_dune`, `dune_marker` (40×48), `soft_top`, and `boulder_small_*` / `boulder_big_*` for `_choc`, `_pasture` and `_dune`.
  - **Props:** `Props/goat_00/01` (16×12), `Props/cow_00/01` (24×16), `Props/palm_tree` (24×40), `Props/cogon_tuft` (12×8), `Props/stone_fence` (32×10), `Props/beach_grass` (12×12), `Props/sand_puff_00/01` (10×6).
  - **Items:** `coral_stone`, `limonite`, `kaolinite`.
  - **Lesson pictures:** `lesson_chocolate_hills`, `lesson_batanes`, `lesson_la_paz`.
  - **Map and menu life:** `LandformMap/Sprites/Hills/tree_0/1` (9×11), `ripple_0/1` (6×2), `bird_0/1` (5×3), `grass_0/1` (7×6).
  - Preview: `.scratch/hills/art-preview.png`.
- `python Tools/Art/make_hills_easy_backdrops.py`
  - `Backdrop/ChocolateHills/`, `Backdrop/RollingPasture/`, `Backdrop/SandDunes/`, each L0–L5.
  - Preview: `.scratch/hills/backdrop-preview.png`.
- `python Tools/Art/make_profile_art.py`: the 10 Hills badges, each normal and locked. Preview: `.scratch/hills/badge-preview.png`.
- `python Tools/Audio/make_hills_audio.py`
  - Music: `bgm_hills.ogg` (40 s loop).
  - Ambience: `amb_stream_easy.ogg`, `amb_stream_normal.ogg`, `amb_rain_hard.ogg` (24 s loops).
  - Effect: `sfx_sand_step.wav` (0.14 s).
  - Preview: `.scratch/hills/audio-preview.png`.

### Theme table for `LevelThemes.cs`

| Theme | GroundTop / Soil / Ledge / BoulderEnding | Goal (name, sprite, offset) | Backdrop | Script |
|---|---|---|---|---|
| ChocolateHills | `choc_top` / `choc_soil` / `ledge_choc` / `_choc` | "Viewing Deck", `viewing_deck`, (24, 24) | `Backdrop/ChocolateHills/` | `Tools/Art/make_hills_easy_backdrops.py` |
| RollingPasture | `pasture_top` / `pasture_soil` / `ledge_pasture` / `_pasture` | "Stone House", `stone_house`, (8, 24) | `Backdrop/RollingPasture/` | same |
| SandDunes | `dune_top` / `dune_soil` / `ledge_dune` / `_dune` | "Dune Marker", `dune_marker`, (8, 24) | `Backdrop/SandDunes/` | same |

### Living props (decoration only; keep them off pits, ledges and boulders with the existing `OverPit`/`PropFits` helpers)

- **ChocolateHills:**
  - A palm tree about every 420 px from x = 200, order 3, centre y = 20.
  - A cogon tuft about every 150 px from x = 90, order 7, centre y = 4.
- **RollingPasture:**
  - Goats at x = 600, 1700 and 3950. Order 4, centre y = 6, with frames 00/01 swapping every 1.5–2.5 s (different for each goat).
  - Cows at x = 1100, 2550 and 4550. Order 4, centre y = 8, with the same frame swap.
  - A stone fence about every 520 px from x = 260, order 3, centre y = 5.
- **SandDunes:** a beach-grass tuft about every 200 px from x = 120, order 7, centre y = 6. Never on a sand zone's first 16 px, so the soft tiles stay readable.

### Map life (LandformMapBuilder)

Positions are in map-picture pixels, with x to the right and y down from the top. Convert them the same way the existing cloud placement does.

- **Remove** the "Hills Cloud".
- **Trees:** six trees with `SpriteFlipbook` frames `tree_0`/`tree_1` at 1.2–1.8 frames per second (each a little different).
  - Four on the far-hill line `y = 72 + round(12·sin(x/27))`, at x = 290, 312, 405 and 440.
  - Two on the near-hill line `y = 94 + round(10·sin(x/19 + 1))`, at x = 270 and 455.
  - Each tree's bottom sits on the line.
- **Stream shine:** three ripples, `ripple_0`/`ripple_1` at 2 frames per second, at y = 104, 116 and 128, with x = `340 + round(9·sin(y/9))` − 3.
- **Birds:** two birds, `bird_0`/`bird_1` at 3 frames per second, on a slow `CircleFlight` around (372, 30) with a radius of about 20 px.
- All of it is active only while Hills is open, like the Mountains life.

### Menu life (SelectionBuilder and LandformBackdrop)

- **Hills:** two birds (the same frames, on a `CircleFlight` around screen (330, 60)) and six grass tufts (`grass_0`/`grass_1`, 1.5 frames per second) along the bottom of the picture at x = 30, 110, 190, 290, 370 and 450, sitting on screen row 268.
- **Mountains:** snow and eagle, as now.
- **Every landform:** the cloud, as now.

### Audio table (`LandformAudio`)

| Episode | Music | Easy | Normal | Hard |
|---|---|---|---|---|
| Mountains | `bgm_mountains` | `amb_breeze_easy` | `amb_wind_normal` | `amb_wind_hard` |
| Hills | `bgm_hills` | `amb_stream_easy` | `amb_stream_normal` | `amb_rain_hard` |
| Plains, Plateaus | not planned yet: use the Mountains files | | | |

The Hills Normal and Hard ambience files are made now so the table is complete, even though those levels come later.

## 10. Not in scope

- Hills Normal 1–5 and Hard 1–7: levels, lessons, kinds, objects, backgrounds and sounds.
- Building the Hills Description Screen flow (its data is added now and its flow already works for any landform).
- Plains and Plateaus content, and the Completion Screen.
- Any change to Mountains levels, numbers or text.
