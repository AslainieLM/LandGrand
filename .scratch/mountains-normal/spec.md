# Spec: Mountains Normal 1–5 (D-062)

Settled with the user on 2026-11-15 in a grilling interview ("Go with your recommendation and make the LandGrand best game ever"). The art and audio are already made and checked. The level layouts are already checked by a physics bot. **Codex builds the code from the tickets in `issues/`.** Read `CODEX-START.md` first.

## Goal

Mountains Normal has five levels, as the final manuscript requires (three Easy, five Normal and seven Hard per episode, Ch. III P066). Clearing all five unlocks Hard (P068; already in `PlayerProgress`). Each level is set on a **real mountain**, teaches one topic, and adds **one new level object** that later levels keep.

| # | Banner | Topic (lesson title) | Theme | New object | New kinds | Review kinds |
|---|---|---|---|---|---|---|
| N1 | Mount Pulag | Weathering | Pine Forest | Crumbling ledge | sandstone, clay | granite, feldspar |
| N2 | Mount Pinatubo | Erosion | Ash Gorge | Falling rocks | conglomerate, shale | pumice, quartz |
| N3 | Mount Matutum | Mountains and Water | Mossy Forest | Moving platform | basalt, magnetite | andesite |
| N4 | Mount Apo | Minerals from Mountains | Mossy Forest | (mix of N1–N3, plus the Easy 2 steam vent) | sulfur, pyrite | obsidian, quartz |
| N5 | Mount Everest | The Highest Mountain | Snow Peak | Wind gusts | limestone, gneiss | marble, garnet |

## Rules that are already settled (do not change)

- **Time limit:** 3 minutes (D-039). **Stars:** 0–40% = 1, 41–99% = 2, 100% = 3 (D-039).
- **Hazards on Normal:** spikes, pits, falling rocks and a crumbled ledge you fall through all send the explorer back to the **last checkpoint reached** (D-040). The Easy short-return rules (D-048, D-056) do **not** apply to Normal.
- **Ambience:** stronger wind for Normal (D-042): `amb_wind_normal.ogg`, same Mountains music.

## New decisions (D-062)

1. **7 pickups and 2 checkpoints per Normal level.** The checkpoints are at x 2400 and 4800. The respawn point is the last checkpoint reached.
2. **Each level is 7200 px wide** with the goal at x 7040. The bot walks it in 98–112 s against the 180 s limit.
3. **Four new themes:** Pine Forest, Ash Gorge, Mossy Forest and Snow Peak. Each has its own ground, soil, ledge, boulders, goal and living props.
4. **Goals:** a ranger hut (Pine Forest), a stone footbridge (Ash Gorge), a trail sign (Mossy Forest), and summit prayer flags (Snow Peak).
5. **Collection paging returns** inside a landform tab, because Mountains now has 20 kinds and the grid shows 12. The tab shows 12 per page with Prev and Next arrows and "1/2".
6. **Three new badges**, and the Achievements grid becomes 4×3:
   - Philippine Peaks (silver): clear Mountains Normal 1–4.
   - Top of the World (gold): clear Mountains Normal 5.
   - Rock Scholar (silver): find 15 different kinds. *Changed from "find all 20 Mountains kinds", because Mountain Collector already means that.*
7. **Manuscript additions (D-036, Q-13 d):** crumbling ledges, falling rocks, moving platforms and wind gusts are not in the manuscript. They are listed for the adviser and panel, like the steam vent and the achievements.

## Level objects: exact rules

All four go in the plain C# Rules assembly (D-061), with EditMode tests. The play scripts only show them.

**Crumbling ledge** (`CrumbleLedge`, 32 px wide, 10 px tall, solid like a ledge)
- Once the explorer stands on it, it breaks **0.5 s** later: it shakes, then falls as chips and stops being solid.
- It comes back **3.0 s** after breaking.
- Two uses:
  - **Bridges:** three ledges with top 0 filling a 96 px pit, so the explorer must keep walking.
  - **Steps:** raised ledges that lead to a pickup.
- Sprites: `crumble_00` (whole), `crumble_01` and `crumble_02` (cracking, shown during the 0.5 s), `crumble_03` (breaking, shown for 0.15 s), plus chips (`Props/crumble_bit`).
- Sound: `sfx_crumble.wav`.

**Falling rock** (`Rockfall`, landing spot X = middle)
- Cycle of **3.0 s**, shifted by its `Offset` in seconds:
  - **0–1.2 s:** warning. The landing shadow (`rock_warn_00`, then `rock_warn_01` from 0.6 s) shows, and pebbles trickle down (`Props/pebbles_00/01`).
  - **1.2–1.5 s:** the rock (`rock_fall`, 16×16) falls from y 270 to y 0 in a straight line.
  - **1.5–1.65 s:** it lies on the ground, then breaks into dust (reuse the start-screen `Dust` frames) and disappears.
- **Hit box:** 16×16 centred on X at the rock's current height. It is active only from 1.2 to 1.65 s.
- Touching the rock counts as a trap (checkpoint rule).
- Sound: `sfx_rockfall.wav` when it lands, if the explorer is within 260 px.

**Moving platform** (`MovingPlatform`, 48×10, `X0`, `X1`, `Top`)
- The left edge moves back and forth between X0 and X1 at **40 px/s**. It starts at X0 at level time 0.
- It is solid and **carries the explorer** standing on it by its own movement each frame.
- Over a pit the platform top is 0. X0 is the pit's left edge, and X1 = pit right edge − 48.
- Sprite: `platform`.

**Wind gust** (`WindZone`, `X`, `Width`, `Offset`)
- Cycle of **4.0 s**:
  - **0–2.0 s:** calm.
  - **2.0–2.6 s:** warning. Wind streaks (`Props/gust_00..03`) blow through the zone and `sfx_gust.wav` plays once.
  - **2.6–4.0 s:** push. Inside the zone the explorer's horizontal speed gets **−40 px/s**, back toward the start, on the ground and in the air.
- It never pushes faster than the explorer walks (72), so walking right still moves forward.

**Steam vent:** the existing D-055 vent, unchanged. In N4 it sits with a sulfur crust (`Props/sulfur_crust`).

## Level data

Coordinates are level pixels: x goes right from the left edge, y goes up from the ground line. All five levels use:
- width **7200**
- start **(40, 0)**
- checkpoints **(2400, 0)** and **(4800, 0)**
- goal **(7040, 0)**
- Spikes are 32 wide. Boulders: S = small, B = big. Pickups give the bottom-middle spot.

### N1 Mount Pulag — Pine Forest
- **Pits:** (900, 48), (2000, 96) bridged, (3300, 56), (5200, 64), (6200, 96) bridged
- **Crumbling ledges (x, top):** steps (1150, 40), (1220, 64); bridge (2000, 0), (2032, 0), (2064, 0); bridge (6200, 0), (6232, 0), (6264, 0)
- **Spikes:** 1700, 4150, 5550, 6850
- **Boulders:** S420, B1480, S2650, B3950, S4450, B5850, S6650
- **Ledges (x, top, width):** (700, 40, 64), (3580, 40, 64), (3660, 72, 48)
- **Pickups:** sandstone (732, 40), clay (1236, 64), granite (2560, 0), feldspar (3000, 24), sandstone (3684, 72), clay (5232, 40), granite (6500, 48)

### N2 Mount Pinatubo — Ash Gorge
- **Pits:** (800, 48), (2500, 56), (3500, 64), (5100, 56), (6200, 64)
- **Falling rocks (x, offset):** (1100, 0), (1900, 1.0), (3000, 0.5), (3700, 2.0), (5400, 0), (5460, 1.5), (6500, 1.0)
- **Spikes:** 1300, 4440, 6880
- **Boulders:** S500, B1500, S2700, B4000, S4600, B5900, S6700
- **Ledges:** (2200, 40, 64), (4300, 48, 80)
- **Pickups:** conglomerate (1100, 0), shale (2232, 40), pumice (2528, 48), quartz (3000, 0), conglomerate (4340, 48), shale (5430, 0), pumice (6232, 40)

### N3 Mount Matutum — Mossy Forest
- **Pits:** (2000, 200), (3400, 56), (5200, 240), (6100, 200)
- **Moving platforms (x0, x1, top):** (1000, 1200, 40) over ground (the safe first one), (2000, 2152, 0), (5200, 5392, 0), (6100, 6252, 0)
- **Spikes:** 1500, 3000, 4600, 6600
- **Boulders:** S450, B1600, S2700, B3900, S4450, B5700, S6750
- **Ledges:** (1260, 72, 64), (3700, 40, 64), (4200, 48, 48)
- **Pickups:** basalt (1292, 72), magnetite (2100, 40), andesite (3016, 48), basalt (3732, 40), magnetite (4224, 48), andesite (5300, 32), basalt (6900, 0)

### N4 Mount Apo — Mossy Forest, with sulfur at the vent
- **Pits:** (800, 48), (1500, 96) bridged, (3600, 64), (5400, 200), (6300, 64)
- **Crumbling ledges:** bridge (1500, 0), (1532, 0), (1564, 0)
- **Falling rocks:** (2200, 0), (4300, 1.5)
- **Moving platform:** (5400, 5552, 0)
- **Steam vent:** 2900, with ledge (2940, 100, 72) above it
- **Spikes:** 1100, 3900, 6700
- **Boulders:** S500, B1900, S2600, B3300, S4600, B5000, S6500
- **Ledges:** (2940, 100, 72), (4050, 48, 64)
- **Pickups:** pyrite (1560, 48), obsidian (2200, 0), sulfur (2976, 100), quartz (3632, 48), pyrite (4082, 48), sulfur (5476, 32), obsidian (6950, 0)

### N5 Mount Everest — Snow Peak
- **Pits:** (2200, 48), (3500, 56), (4000, 96) bridged, (5500, 64)
- **Crumbling ledges:** bridge (4000, 0), (4032, 0), (4064, 0)
- **Falling rock:** (4300, 0)
- **Wind zones (x, width, offset):** (900, 500, 0), (2000, 400, 1.0), (3300, 600, 2.0), (5300, 500, 0.5), (6200, 500, 1.5)
- **Spikes:** 1500, 2900, 5000, 6500
- **Boulders:** S600, B1700, S2700, B3800, S4600, B6000, S6800
- **Ledges:** (1200, 48, 64), (4700, 40, 64), (4780, 72, 48)
- **Pickups:** limestone (1232, 48), gneiss (2224, 40), marble (2916, 48), garnet (3528, 40), limestone (4804, 72), gneiss (5532, 40), marble (6900, 0)

**Bot check (2026-11-15, scratch sim with the motor's numbers and the rules above):**
- Times to reach the goal: N1 97.6 s, N2 98.7 s, N3 105.1 s, N4 99.6 s, N5 111.6 s.
- All 35 pickups can be reached and left safely.
- The bot waits for rocks, boards platforms at the near end and waits out gusts before jumping a pit.

## Content (draft until a teacher reviews it, Q-04)

Every lesson sentence measures **≤ 298 px** in Andika at UI size 15 and is at most 44 characters.

**N1 Mount Pulag — Weathering** (picture `lesson_pulag`)
- Lesson:
  1. Mount Pulag is our third highest mountain.
  2. On cold nights, frost forms on its peak.
  3. Ice, rain and roots slowly crack the rock.
  4. This breaking of rock is called weathering.
- Review: "Weathering breaks rock into small pieces. Feldspar can weather into soft clay. Pieces of rock and clay help make soil."

**N2 Mount Pinatubo — Erosion** (`lesson_pinatubo`)
- Lesson:
  1. Mount Pinatubo erupted in 1991.
  2. Thick ash covered its slopes.
  3. Heavy rain washed the ash down as lahar.
  4. Carrying rock and soil away is erosion.
- Review: "Erosion carries rock and soil to new places. Rivers round pebbles that end up in conglomerate. Mud that settles can harden into shale."

**N3 Mount Matutum — Mountains and Water** (`lesson_matutum`)
- Lesson:
  1. Matutum is near General Santos City.
  2. Its forest soaks up rain like a sponge.
  3. Tree roots hold the soil in place.
  4. Its rivers bring water to nearby towns.
- Review: "Forests on mountains hold soil and store water. Rivers can carry black magnetite sand. Caring for forests protects our water."

**N4 Mount Apo — Minerals from Mountains** (`lesson_apo`)
- Lesson:
  1. Mount Apo is the highest in the country.
  2. Hot steam comes out of vents near its top.
  3. Yellow sulfur forms around these vents.
  4. Mountains give us many useful minerals.
- Review: "Mountains hold many minerals we use. Sulfur is used to make medicine and fertilizer. Pyrite looks like gold, but it is not gold."

**N5 Mount Everest — The Highest Mountain** (`lesson_everest`)
- Lesson:
  1. Mount Everest is the highest on Earth.
  2. Moving plates still push it up slowly.
  3. Its top is cold, windy and snowy.
  4. Rock at its top was once under the sea.
- Review: "Moving plates lift mountains very slowly. Limestone at Everest's top holds old sea fossils. Heat and squeezing turn rock into gneiss."

**New kinds** (all `isDraft: true`, home landform Mountains; id = picture name):

| id | Name | Rock? | Description |
|---|---|---|---|
| sandstone | Sandstone | rock | A rock made of sand grains stuck together. It wears away in wind and rain. |
| clay | Clay | mineral | Very fine soil from weathered rock. It feels sticky when it is wet. |
| conglomerate | Conglomerate | rock | A rock of round pebbles stuck in sand. Rivers rounded the pebbles long ago. |
| shale | Shale | rock | A rock made from mud that settled in water. It splits into thin layers. |
| basalt | Basalt | rock | A dark rock made from cooled lava. It often has tiny holes from gas. |
| magnetite | Magnetite | mineral | A black mineral that sticks to a magnet. It makes the black sand in rivers. |
| sulfur | Sulfur | mineral | A bright yellow mineral. It forms around hot volcanic vents. |
| pyrite | Pyrite | mineral | Called fool's gold. Its shiny gold cubes are not real gold. |
| limestone | Limestone | rock | A pale rock made from shells under the sea. Heat can turn it into marble. |
| gneiss | Gneiss | rock | A rock with light and dark bands. Deep heat and squeezing make it. |

Sources checked on 2026-11-15:
- **Pulag:** 3rd highest, 2,928 m, with frost from November to February. Wikipedia and Inquirer.
- **Pinatubo:** about half of the 1991 ash was carried away as lahars. USGS FS 114-97.
- **Matutum:** a watershed for South Cotabato, General Santos and Sarangani, and the source of five rivers. FPE and MindaNews.
- **Apo:** solfataras and sulfur deposits. Inquirer and VolcanoDiscovery.
- **Everest:** its summit is Ordovician seafloor limestone with marine fossils, thrust up by the India–Asia collision. Montana State University and IUGS.

## Assets (already made, in the repo)

Run in this order, from the project folder. All scripts are deterministic.
- `python Tools/Art/make_normal_art.py`
  - Level sprites: `pine_* ash_* moss_* snow_*` (top, soil, ledge), the boulders `boulder_{small,big}_{pine,ash,moss,snow}`, `hut`, `bridge`, `trail_sign`, `summit_flags`, `crumble_00..03`, `rock_fall`, `rock_warn_00..01`, `platform`, and the `Props/` sprites: `pine_tree`, `fern`, `sulfur_crust`, `crumble_bit`, `pebbles_00..01`, `gust_00..03`.
  - Items: 10 new PNGs in `Assets/Art/Items/Sprites/`.
  - Lessons: 5 lesson pictures in `Assets/Art/Lesson/Sprites/`.
- `python Tools/Art/make_profile_art.py`: adds `badge_{philippine_peaks,top_of_the_world,rock_scholar}` and their `_locked` versions.
- `python Tools/Audio/make_normal_audio.py`: makes `amb_wind_normal.ogg`, `sfx_gust.wav`, `sfx_crumble.wav` and `sfx_rockfall.wav`.

**Sprite sizes for placement:**
- Goals: `hut` 56×48 (door centre in the middle), `bridge` 72×44, `trail_sign` 32×40, `summit_flags` 56×56.
- Props: `pine_tree` 24×48, `fern` 16×12, `sulfur_crust` 24×6.
- Gusts: `gust_*` 20×6.

## Not in scope

- Mountains Hard.
- The Hills, Plains and Plateaus landforms.
- Teacher review of the content (Q-04).
