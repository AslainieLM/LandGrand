# Spec: Mountains Hard 1–7 and the Landform Description Screen (D-065)

Settled with the user on 2026-11-15 in a grilling interview (Q1–Q14, "Go with your recommendation and choose the best option for the LandGrand Game", then "I agree with your chosen recommendation and choose the best option"). The art and audio are already made and checked. The level layouts are already checked by a physics bot. **Codex builds the code from the tickets in `issues/`.** Read `CODEX-START.md` first.

## Goal

Mountains Hard has seven levels, as the final manuscript requires (three Easy, five Normal and seven Hard per episode, Ch. III P066). Each level is set on a **real mountain** and teaches one topic. Clearing all 15 Mountains levels finishes the episode: the **Landform Description Screen** sums up Mountains, **Hills unlocks** and is saved, and the player goes back to the Landform Map (P069, LG-14, LG-32).

| # | Banner | Topic (lesson title) | Theme | Level objects | New kinds | Review kind |
|---|---|---|---|---|---|---|
| H1 | Mount Mayon | Volcano Safety | Lava Slope | **Rolling boulders** (new), falling rocks | scoria, hornblende | andesite |
| H2 | Sierra Madre | Mountain Ranges | Storm Ridge | Wind gusts, crumbling bridges, rolling boulders | diorite, gold | granite |
| H3 | Kitanglad Range | Mountains Are Homes | Eagle Forest | Moving platforms, crumbling steps, rolling boulders | tuff, hematite | basalt |
| H4 | Mount Hamiguitan | Caring for Mountains | Bonsai Forest | Crumbling bridges, falling rocks, rolling boulders | serpentinite, chromite | magnetite |
| H5 | Matterhorn | Glaciers Shape Mountains | Glacier Horn | **Icy ground** (new), wind gusts, crumbling bridges, rolling boulders | schist, calcite | gneiss |
| H6 | Mount Kilimanjaro | Climbing Changes Weather | Equator Summit | Steam vents, platforms, falling rocks, rolling boulders, icy ground | trachyte, tanzanite | obsidian |
| H7 | Mauna Kea | Mountains Under the Sea | Sea Peak | Everything: platforms, bridges, rocks, gusts, vent, rolling boulders, icy ground | gabbro, olivine | basalt |

## Rules that are already settled (do not change)

- **Time limit:** 4 minutes (D-039, already in `LevelCatalog.TimeLimitFor`). **Stars:** 0–40% = 1, 41–99% = 2, 100% = 3 (D-039). With 9 pickups: 0–3 = 1 star, 4–8 = 2 stars, 9 = 3 stars.
- **Hazards on Hard:** spikes, pits, falling rocks, rolling boulders and a crumbled ledge you fall through all send the explorer back to the **last checkpoint reached** (D-040). The Easy short-return rules (D-048, D-056) do **not** apply.
- **Unlocking depends on clearing, not on stars** (D-039). Hard unlocks after all Normal levels (already in `PlayerProgress`).
- **Ambience:** gusty, near-storm wind for Hard (D-042): `amb_wind_hard.ogg`, same Mountains music.
- **Pause and Time's up** keep Resume, Retry and Levels (D-064).

## New decisions (D-065)

1. **9 pickups and 3 checkpoints per Hard level.** Checkpoints at x 2400, 4800 and 7200. Each level has 2 new kinds (one rock, one mineral; 4 pickups each) and 1 review pickup of an older kind. One pickup per level sits in a hard-to-reach "secret" spot (marked per level below).
2. **Each level is 9600 px wide** with the goal at x 9440. The bot reaches every goal in 131–156 s of the 240 s limit.
3. **Two new level objects:** the rolling boulder and icy ground (exact rules below). Every other object is reused unchanged.
4. **Seven new themes**, each with its own ground, soil, ledge, boulders, goal, living props and six-layer background (D-063).
5. **Fourteen new kinds**, so Mountains has 34. The Collection gets a **third page** (12 + 12 + 10) with no new layout.
6. **Four new badges** (15 in all) and the Achievements grid becomes **5×3**.
7. **"Required episode levels" means all 15 levels** of the landform (answers part of Q-03). Because levels unlock in order, clearing Hard 7 means all 15 are cleared.
8. **Next landform unlock is worked out from saved clears:** a landform is open when it is Mountains, or when every level of the landform before it is cleared. The save file does not change; the unlock is saved the moment the Hard 7 clear is saved (LG-32 "auto-saved").
9. **An open landform with no levels yet** (Hills now) shows in colour and animated on the map, and tapping it says "Coming soon".
10. **One-time unlock animation:** the first time the map shows a newly opened landform, its padlock opens and the grey fades away (about 1.5 s) with a sparkle and `sfx_unlock.wav`. The "seen" mark is a device setting (`PlayerPrefs`, like the volume settings), not part of the save.
11. **Landform Description Screen** (new scene `LandformDescription`): opened by the Result Screen's **Next Episode** button (which only appears once the whole landform is cleared) and by a new **book button** on the difficulty selection screen once the landform is complete.
12. **Completion Screen** (after all four landforms) is **not in scope**.
13. **Manuscript (D-036, Q-13 d):** the rolling boulder, icy ground, the four new badges and 5×3 grid, the unlock animation, the book button and the Hard sizes (9600 px, 9 pickups, 3 checkpoints) are additions the manuscript does not name. They are listed for the adviser and panel.

## Level objects: exact rules

Both new objects go in the plain C# Rules assembly (D-061), with EditMode tests. The play scripts only show them. Both use `LevelClock.Now`, so they pause with the game.

**Rolling boulder** (`RollingBoulder`, `Spawn`, `End`, `Offset`)
- Cycle of **3.5 s**, shifted by `Offset` seconds:
  - **0–0.8 s:** warning. A dust puff (`Props/rumble_dust_00`, then `_01` from 0.4 s) shows at `Spawn`, and `sfx_boulder_rumble.wav` plays once if the explorer is within 300 px.
  - **From 0.8 s:** the boulder's middle starts at `Spawn` and rolls **left at 90 px/s** along the ground (y 0). It disappears with a dust puff (reuse the start-screen `Dust` frames) when its middle passes `End`.
- **Skip rule (fairness):** at the moment a boulder would start rolling (phase 0.8 s), if the explorer's middle is within **48 px** of `Spawn`, that boulder skips this cycle. This stops a boulder appearing on top of the explorer.
- **Hit box:** 14×14, centred on the boulder's middle x, from y 0 to 14. Touching it counts as a trap (checkpoint rule).
- Its run (`Spawn − End`) is at most 243 px (90 × 2.7), always over plain ground: no pit, spikes, boulder, vent or checkpoint between `End − 8` and `Spawn + 8`. The data tests check this.
- Sprites: `roll_boulder_00..03` (the crack turns 45° per frame; change frame every 0.1 s). Sound: `sfx_boulder_roll.wav` loops while a boulder rolls within 260 px of the explorer.
- A single jump clears it: the explorer jumps 46 px high and the boulder is 14 px tall.

**Icy ground** (`IceZone`, `X`, `Width`)
- A stretch of ground from `X` to `X + Width`. The ground there draws `ice_top` instead of the theme's top tile. A few `Props/ice_glint_00/01` twinkles sit on it.
- While the explorer **stands on ice** (grounded at y 0 with its middle inside the zone), its walking speed changes gradually instead of at once:
  - speeding up toward the held direction at **160 px/s²** (0 to 72 takes 0.45 s);
  - slowing down, stopping or turning at **100 px/s²** (72 to 0 takes 0.72 s, a slide of about 26 px).
- In the air, on ledges, on platforms and off the ice, walking works as before (instant). Hitting a wall sets the walking speed to 0.
- The wind push is added on top, as now (`ExternalPushX`).
- Pure rule for tests: `IceSlide.Next(float current, float target, float dt)`.
- Sound: `sfx_ice_slide.wav` once when the explorer starts sliding (it lets go of the arrows or turns while moving on ice).
- No falling rocks on or within 30 px of ice, and no ice on a checkpoint (data tests).

## Level data

Coordinates are level pixels: x goes right from the left edge, y goes up from the ground line. All seven levels use:
- width **9600**
- start **(40, 0)**
- checkpoints **(2400, 0)**, **(4800, 0)** and **(7200, 0)**
- goal **(9440, 0)**
- Spikes are 32 wide. Boulders: S = small, B = big. A "bridge" at x is three crumbling ledges at x, x + 32 and x + 64 with top 0. Pickups give the bottom-middle spot.
- Rolling boulders are (spawn, end, offset). Ice zones are (x, width).

### H1 Mount Mayon — Lava Slope
- **Pits:** (1500, 56), (3000, 64), (4200, 64), (6600, 64), (8200, 64)
- **Spikes:** 1200, 2800, 4000, 5400, 7000, 8400
- **Boulders:** S400, B1650, S2150, B3250, S5800, B6800, S7350, B7950, S9200
- **Ledges (x, top, width):** (1000, 40, 64), (3700, 48, 64), (6300, 40, 64), (6380, 72, 48), (8700, 48, 64)
- **Falling rocks (x, offset):** (2600, 0), (4500, 1.0), (8600, 0.5)
- **Rolling boulders:** (900, 700, 0), (2000, 1800, 1.0), (3600, 3380, 0), (5200, 4980, 2.0), (6200, 5980, 0.5), (7800, 7580, 1.5), (9000, 8780, 0)
- **Pickups:** scoria (1032, 40), hornblende (1528, 48), scoria (2600, 0), andesite (3732, 48), hornblende (4232, 40), scoria (6332, 40), hornblende (6404, 72) *secret*, scoria (8732, 48), hornblende (9216, 24)
- The first boulder (900) rolls on an empty stretch, so the player learns it safely.

### H2 Sierra Madre — Storm Ridge
- **Pits:** (1300, 64), (2900, 96) bridged, (4100, 56), (5600, 64), (6800, 96) bridged, (8500, 64)
- **Crumbling ledges:** bridge 2900, bridge 6800, steps (5000, 40), (5070, 64)
- **Spikes:** 1000, 2600, 3650, 4300, 6200, 7400, 8300
- **Boulders:** S450, B1600, S3100, B3950, S5300, B6500, S7000, B8000, S8950
- **Ledges:** (1700, 40, 64), (3200, 48, 64), (7700, 40, 64), (7780, 72, 48)
- **Wind zones (x, width, offset):** (700, 500, 0), (1900, 400, 1.0), (3300, 250, 2.0), (5800, 320, 0.5), (7600, 600, 1.5), (8800, 400, 0)
- **Rolling boulders:** (2200, 2000, 0), (4600, 4380, 1.0), (9300, 9080, 2.0)
- **Pickups:** diorite (1732, 40), gold (2932, 40), diorite (3232, 48), granite (3974, 36), gold (5086, 64), diorite (6832, 40), gold (7804, 72) *secret*, diorite (8532, 48), gold (8966, 24)

### H3 Kitanglad Range — Eagle Forest
- **Pits:** (1800, 200), (3300, 56), (4000, 240), (5600, 64), (6400, 200), (8000, 240), (8900, 56)
- **Moving platforms (x0, x1, top):** (1000, 1200, 40) over ground (the safe first one), (1800, 1952, 0), (4000, 4192, 0), (6400, 6552, 0), (8000, 8192, 0)
- **Crumbling ledges:** steps (2700, 40), (2770, 64); steps (7500, 40), (7570, 64)
- **Spikes:** 1500, 2200, 3600, 5000, 6000, 7000, 8700
- **Boulders:** S450, B1300, S2550, B3800, S4500, B5800, S6800, B7300, S8500
- **Ledges:** (1260, 72, 64), (3700, 40, 64), (5400, 48, 48)
- **Rolling boulders:** (3000, 2850, 0), (5300, 5080, 1.5), (9300, 9100, 0.5)
- **Pickups:** tuff (1292, 72) *secret*, hematite (1900, 40), tuff (2786, 64), hematite (3724, 40), basalt (4100, 40), tuff (5424, 48), hematite (6500, 40), tuff (7586, 64), hematite (8100, 32)

### H4 Mount Hamiguitan — Bonsai Forest
- **Pits:** (1100, 48), (1900, 96) bridged, (3200, 64), (4300, 96) bridged, (5600, 56), (6600, 96) bridged, (8300, 64)
- **Crumbling ledges:** bridges 1900, 4300 and 6600; steps (3500, 40), (3570, 64); steps (8800, 40), (8870, 64)
- **Spikes:** 800, 1500, 4000, 5000, 5800, 7000, 9300
- **Boulders:** S450, B1300, S2200, B3800, S4650, B5400, S7400, B8600
- **Ledges:** (1400, 40, 64), (4900, 48, 64)
- **Falling rocks:** (2600, 0), (5200, 1.5), (7600, 0.5), (9100, 1.0)
- **Rolling boulders:** (3000, 2800, 0.5), (6200, 5980, 1.0), (8100, 7880, 2.0)
- **Pickups:** serpentinite (1432, 40), chromite (1932, 40), serpentinite (2600, 0), chromite (3586, 64), magnetite (4332, 40), serpentinite (4932, 48), chromite (6632, 40), serpentinite (8332, 48), chromite (8886, 64) *secret*

### H5 Matterhorn — Glacier Horn
- **Pits:** (1300, 56), (2600, 64), (3800, 96) bridged, (5000, 56), (6100, 64), (7400, 96) bridged, (8800, 64)
- **Crumbling ledges:** bridges 3800 and 7400
- **Ice zones:** (600, 600) on its own first, so the player learns to slide safely; (1800, 500), (3000, 700), (5200, 600), (6500, 500), (8000, 700)
- **Spikes:** 1600, 2900, 4400, 5900, 9020
- **Boulders:** S450, B1400, S2200, B3500, S4100, B5500, S6300, B7700, S8300
- **Ledges:** (2000, 48, 64), (5400, 40, 64), (5480, 72, 48), (8400, 48, 64)
- **Wind zones:** (1800, 500, 0), (4200, 500, 1.0), (6500, 500, 2.0), (9080, 300, 0.5)
- **Rolling boulders:** (4700, 4480, 0), (7000, 6780, 1.5), (9300, 9100, 0.5)
- **Pickups:** schist (2032, 48), calcite (2632, 48), schist (3832, 40), calcite (4116, 24), gneiss (5432, 40), schist (5504, 72) *secret*, calcite (7432, 40), schist (8432, 48), calcite (8832, 48)

### H6 Mount Kilimanjaro — Equator Summit
- **Pits:** (1100, 56), (2000, 200), (3100, 64), (4300, 96) bridged, (5300, 64), (6700, 240), (7600, 64), (9000, 56)
- **Moving platforms:** (2000, 2152, 0), (6700, 6892, 0)
- **Crumbling ledges:** bridge 4300
- **Steam vents:** 1500 with ledge (1540, 100, 72) above it; 3700 with ledge (3740, 100, 72) above it
- **Ice zones:** (6000, 500), (8200, 700) (the snowy top of the climb)
- **Spikes:** 800, 2600, 3300, 4650, 6550, 7400
- **Boulders:** S450, B1300, S2200, B3450, S4550, B5450, S6200, B7000, S8500
- **Ledges:** (1540, 100, 72), (3740, 100, 72), (8600, 48, 64)
- **Falling rocks:** (2700, 0), (5000, 1.0), (7900, 0.5)
- **Rolling boulders:** (4250, 4060, 1.0), (5800, 5580, 0), (9300, 9080, 2.0)
- **Pickups:** trachyte (1576, 100), tanzanite (2100, 40), trachyte (2700, 0), tanzanite (3776, 100) *secret*, obsidian (4332, 40), trachyte (5332, 48), tanzanite (6800, 32), trachyte (8632, 48), tanzanite (9028, 48)

### H7 Mauna Kea — Sea Peak
- **Pits:** (1000, 56), (1900, 96) bridged, (3000, 200), (4200, 64), (5500, 96) bridged, (6400, 200), (8000, 64), (8900, 56)
- **Crumbling ledges:** bridges 1900 and 5500
- **Moving platforms:** (3000, 3152, 0), (6400, 6552, 0)
- **Steam vent:** 2200 with ledge (2240, 100, 72) above it
- **Ice zones:** (4400, 300), (7500, 400)
- **Wind zone:** (7400, 500, 0)
- **Spikes:** 700, 2750, 4000, 5000, 7000, 8300, 9200
- **Boulders:** S400, B1250, S2850, B3500, S4500, B6200, S7700, S8750
- **Ledges:** (2240, 100, 72), (6700, 48, 64), (9000, 40, 64)
- **Falling rocks:** (2600, 0), (5100, 1.5)
- **Rolling boulders:** (1600, 1400, 0), (3900, 3680, 1.0), (6000, 5780, 2.0), (8600, 8380, 0.5)
- **Pickups:** gabbro (1932, 40), olivine (2276, 100) *secret*, gabbro (3100, 40), olivine (4516, 24), basalt (5532, 40), gabbro (6732, 48), olivine (7716, 24), gabbro (8032, 48), olivine (9032, 40)

**Bot check (2026-11-15, scratch sim with the motor's numbers and the rules above; files `bot-check-*.txt` in this folder):**
- Times to reach the goal: H1 131.0 s, H2 138.3 s, H3 155.6 s, H4 132.4 s, H5 137.0 s, H6 146.0 s, H7 143.4 s (limit 240 s).
- All 63 pickups can be reached and left safely.
- Layout checks pass: every rolling-boulder run is on plain ground and away from checkpoints; no pit, spikes, rock or ice near a checkpoint; no rock near ice.
- The bot jumps rolling boulders when they are 44 px away, waits for rocks, boards platforms at the near end and waits out gusts before jumping a pit.
- To rerun: copy the three `bot-check-*.txt` files into an empty folder as `Sim.cs`, `Levels.cs` and `bot.csproj`, then `dotnet run -c Release`.

## Content (draft until a teacher reviews it, Q-04)

Every lesson sentence measures **≤ 298 px** in Andika at size 15 (checked with Pillow on `Assets/Fonts/Andika-Regular.ttf`) and is at most 44 characters.

**H1 Mount Mayon — Volcano Safety** (picture `lesson_mayon`)
- Lesson:
  1. Mayon in Albay is our most active volcano.
  2. It has an almost perfect cone shape.
  3. Experts watch it and set alert levels.
  4. Stay out of the danger zone near it.
- Review: "Active volcanoes can erupt again. Experts at PHIVOLCS watch them and warn people. Follow the alert levels and stay safe."

**H2 Sierra Madre — Mountain Ranges** (`lesson_sierra_madre`)
- Lesson:
  1. A range is a long row of mountains.
  2. The Sierra Madre is our longest range.
  3. It runs along the east side of Luzon.
  4. It helps weaken some strong typhoons.
- Review: "Mountains in a row make a mountain range. The Sierra Madre helps slow typhoon winds and rain. Its forests need our care too."

**H3 Kitanglad Range — Mountains Are Homes** (`lesson_kitanglad`)
- Lesson:
  1. Kitanglad is a mountain range in Bukidnon.
  2. Dulang-dulang is our second highest peak.
  3. The Philippine eagle lives in its forest.
  4. Mountains are homes for many animals.
- Review: "Mountain forests are homes for plants and animals. The Philippine eagle nests in tall trees. Saving forests saves their homes."

**H4 Mount Hamiguitan — Caring for Mountains** (`lesson_hamiguitan`)
- Lesson:
  1. Mount Hamiguitan is in Davao Oriental.
  2. Tiny old trees grow on its red soil.
  3. It is a World Heritage Site we protect.
  4. Roots hold soil and help stop landslides.
- Review: "Protected mountains keep forests safe. Tree roots hold soil so rain does not wash it away. We can help by not cutting trees."

**H5 Matterhorn — Glaciers Shape Mountains** (`lesson_matterhorn`)
- Lesson:
  1. The Matterhorn is in the Alps of Europe.
  2. A glacier is a huge river of slow ice.
  3. Glaciers carved its four steep sides.
  4. Ice shapes mountains over many years.
- Review: "Glaciers are rivers of ice that move very slowly. As they move, they scrape and carve rock. Glaciers gave the Matterhorn its sharp shape."

**H6 Mount Kilimanjaro — Climbing Changes Weather** (`lesson_kilimanjaro`)
- Lesson:
  1. Kilimanjaro is the highest in Africa.
  2. It is near the equator, but has snow.
  3. The air gets colder as you climb higher.
  4. Plants change from forest to bare rock.
- Review: "The higher you climb, the colder it gets. Kilimanjaro has farms, forest and snow on one mountain. Its snow and ice are shrinking."

**H7 Mauna Kea — Mountains Under the Sea** (`lesson_mauna_kea`)
- Lesson:
  1. Mauna Kea is a volcano in Hawaii.
  2. Most of it is hidden under the sea.
  3. From its base, it is taller than Everest.
  4. Many mountains stand on the sea floor.
- Review: "Some mountains start at the bottom of the sea. From its base, Mauna Kea is over 10,000 m tall. Its top is a good place to watch stars."

**New kinds** (all `isDraft: true`, home landform Mountains; id = picture name):

| id | Name | Rock? | Description |
|---|---|---|---|
| scoria | Scoria | rock | A red-brown lava rock full of holes. Gas bubbles made the holes. |
| hornblende | Hornblende | mineral | A black mineral shaped like long sticks. It is found in lava rock like andesite. |
| diorite | Diorite | rock | A rock with black and white spots. Magma cooled slowly underground to make it. |
| gold | Gold | mineral | A shiny yellow metal found in mountain rocks. It never rusts. |
| tuff | Tuff | rock | A light rock made of volcanic ash. The ash was pressed together over time. |
| hematite | Hematite | mineral | A heavy mineral with iron in it. It leaves a red streak when rubbed. |
| serpentinite | Serpentinite | rock | A smooth green rock that looks like snake skin. Few plants grow on its soil. |
| chromite | Chromite | mineral | A heavy black mineral. It gives us chromium for shiny metal parts. |
| schist | Schist | rock | A shiny rock that splits into thin layers. Heat and squeezing made it. |
| calcite | Calcite | mineral | A clear or white mineral. It is the main mineral in limestone and marble. |
| trachyte | Trachyte | rock | A grey lava rock with tiny crystals. Much of Kilimanjaro's top is made of it. |
| tanzanite | Tanzanite | mineral | A blue-violet gem. It is found only near Mount Kilimanjaro in Tanzania. |
| gabbro | Gabbro | rock | A dark rock with big grains. It forms deep under the sea floor. |
| olivine | Olivine | mineral | A green mineral in lava. It makes the green sand of a Hawaii beach. |

Sources checked on 2026-11-15:
- **Mayon:** the most active Philippine volcano; a 6-km Permanent Danger Zone; PHIVOLCS alert levels. Smithsonian GVP, Rappler, NCCA, Al Jazeera.
- **Sierra Madre:** the longest Philippine range (about 540 km). It helps weaken typhoons but cannot fully protect Luzon; DOST found it cuts rain in western Luzon. Inquirer, ABS-CBN, Philstar (NDRRMC), Wikipedia.
- **Kitanglad / Dulang-dulang:** Dulang-dulang (about 2,938–2,941 m) is the second highest Philippine peak; the park is a Philippine eagle habitat. ASEAN Heritage Parks, Key Biodiversity Areas, Wikipedia.
- **Hamiguitan:** UNESCO World Heritage Site (2014), a "bonsai" pygmy forest of 100-year-old trees on ultramafic soil; serpentine rocks with nickel and iron; a chromite and nickel mineral reservation proposed nearby. UNESCO via SunStar and Inquirer, Manila Bulletin, BusinessWorld.
- **Matterhorn:** a glacial horn with four faces carved by four cirque glaciers. Wikipedia (pyramidal peak), Alpenwild.
- **Kilimanjaro:** Africa's highest; snow and ice near the equator, shrinking; vegetation zones from forest to arctic summit; Kibo's top is mostly trachyte and phonolite. NASA Earth Observatory, Geological Society (Geoscientist 2017). Tanzanite comes only from the Merelani Hills near Kilimanjaro (GIA).
- **Mauna Kea:** about 4,205 m above sea level and about 10,200 m from its base on the sea floor (Guinness). Olivine makes the green sand of Papakōlea beach (USGS); that beach is on Mauna Loa, so the olivine text says "a Hawaii beach".

## Landform Description Screen

A new scene `LandformDescription` (added to the build list after `Lesson`), built by a new menu item **LandGrand > Build Landform Description Scene**, with a controller `LandformDescriptionController` (a `PixelScreen`). It shows the landform in `GameSession.Episode`.

**Layout (480×270):**
- **Background:** the landform's menu picture (`LandformBackdrop`, the same one the difficulty screen uses) with gently falling snow for Mountains (reuse `SummitSnow` or the snow flakes).
- **Panel:** `Description/scroll_panel.png` (300×196), centred at (240, 128).
- **Title** at the top of the panel: "Mountains" in Micro 5, dark.
- **Summary:** five short draft sentences in Andika 15, one per line, left aligned inside the panel (each fits 298 px):
  1. Mountains are high, steep landforms.
  2. Some form from volcanoes; others fold up.
  3. Weathering and erosion wear them down.
  4. They give us water and homes for animals.
  5. Let us all take care of our mountains.
  Mark them draft like the lessons (Q-04).
- **Your trip** row: a star icon with "n/45" (best stars on all 15 levels), the gem icon with "n/34" (Mountains kinds found), and a small medal with "n/15" (badges earned).
- **Unlock banner** at the bottom of the panel: a small Hills picture with the padlock opening (`Description/unlock_padlock_00..03`) and sparkles (`sparkle_00..02`), and the text "Hills is now open!". If the next landform has no levels yet, add a second small line "New levels coming soon". For Plateaus (no next landform), the banner is hidden.
- **Button:** `Description/btn_to_map.png` ("TO THE MAP") under the panel, centred. It goes to the Landform Map.
- **When opened from the book button,** a Back icon shows at the top left (the usual `BackIcon`) and goes back to the difficulty selection; To the Map still goes to the map.
- **Sound:** `sfx_fanfare.wav` once when the screen opens.

**Rules side (no Unity types), with tests:**
- `LandformSummary.For(Episode)` gives the title, the five sentences and the picture name. Mountains has the text above; the other landforms return a short placeholder for now ("Coming soon").
- `PlayerProgress.IsEpisodeComplete(Episode)`: every Easy, Normal and Hard level of that landform is cleared.
- `PlayerProgress.IsEpisodeUnlocked(Episode)`: Mountains, or the episode before it is complete.
- `PlayerProgress.StarsFor(Episode)` (best stars summed) and the maximum (levels × 3).
- `LevelCatalog.HasLevels(Episode)`: whether any level of that landform exists.
- `Episode` order stays Mountains, Hills, Plains, Plateaus (D-039).

**Flow:**
- Result Screen: when `NextAfter` gives `NextEpisode`, the Next Episode button opens `LandformDescription` with `GameSession.Episode` set to the **finished** landform. The Description Screen's To the Map sets `GameSession.Episode` to the next landform and opens the map.
- Landform Map: a region is open when `IsEpisodeUnlocked`. Tapping an open region that `HasLevels` opens the difficulty screen as now; tapping an open region with no levels shows the guide text "Coming soon".
- **Unlock animation** on the map: for each open landform other than Mountains whose device flag `LandGrand.UnlockSeen.<Episode>` is not set, play once: padlock frames 00→03 over 0.8 s on the region's padlock spot, the grey `locked` image fades out over 0.7 s, three sparkles, `sfx_unlock.wav`; then set the flag. Show the guide "Hills is now open!" for 2 s. Tapping is blocked during the 1.5 s.
- Difficulty selection: once `IsEpisodeComplete(landform)`, a book icon button (`Description/btn_book_icon.png`, 34×30) shows at the top right, mirroring the Back icon, and opens `LandformDescription` (with a session flag that it came from the book).
- The Collection's landform tabs follow `IsEpisodeUnlocked`, so the Hills tab opens too and shows its existing "Coming soon" text.

## Profile changes

- **Collection:** Mountains now has 34 kinds, so 3 pages (12, 12, 10) with the existing arrows and "1/3". No new layout.
- **Four new badges** in `AchievementCatalog.All`, after Rock Scholar, worked out from saved progress only:
  - `brave_hiker`, "Brave Hiker", silver: "Clear any Mountains Hard level."
  - `world_peaks`, "World Peaks", silver: "Clear the Matterhorn, Kilimanjaro and Mauna Kea (Hard 5 to 7)."
  - `hard_star`, "Hard Star", gold: "Get 3 stars on all 7 Mountains Hard levels."
  - `mountain_master`, "Mountain Master", gold: "Clear all 15 Mountains levels."
  - Mountain Collector ("find all Mountains kinds") now needs 34 kinds; its rule already counts every kind, so do not change it.
- **Achievements grid 5×3:** five medals per row, three rows (15 cells), names under the medals. Starting point: x = 56 + column × 92, rows at y 104, 162 and 220, name width 88. Check that the names do not overlap; shrink the spacing, not the font. "Badges n/15".

## Assets (already made, in the repo)

Run in this order, from the project folder. All scripts are deterministic.
- `python Tools/Art/make_hard_art.py`
  - Level sprites: `lava_* storm_* eagle_* bonsai_* glacier_* equator_* sea_*` (`_top`, `_soil`, `ledge_*`), the boulders `boulder_{small,big}_{lava,storm,eagle,bonsai,glacier,equator,sea}`, the goals `watch_station`, `lookout_tower`, `eagle_nest`, `sanctuary_sign`, `alpine_hut`, `summit_board`, `telescope_dome`, the objects `roll_boulder_00..03` and `ice_top`.
  - `Props/`: `rumble_dust_00..01`, `ice_glint_00..01`, `rain_00..01`, `burnt_tree`, `bent_tree`, `tall_tree`, `orchid`, `bonsai_tree`, `pitcher_plant`, `edelweiss`, `groundsel`, `silversword`.
  - Items: 14 new PNGs in `Assets/Art/Items/Sprites/`.
  - Lessons: 7 lesson pictures in `Assets/Art/Lesson/Sprites/`.
  - `Assets/Art/Description/Sprites/`: `scroll_panel`, `btn_to_map(_pressed)`, `btn_book_icon(_pressed)`, `unlock_padlock_00..03`, `sparkle_00..02`.
- `python Tools/Art/make_hard_backdrops.py`: six layers each in `Assets/Art/Level/Sprites/Backdrop/{LavaSlope,StormRidge,EagleForest,BonsaiForest,GlacierHorn,EquatorSummit,SeaPeak}/` and the preview `.scratch/mountains-hard/backdrop-preview.png`.
- `python Tools/Art/make_profile_art.py`: adds `badge_{brave_hiker,world_peaks,hard_star,mountain_master}` and their `_locked` versions.
- `python Tools/Audio/make_hard_audio.py`: `amb_wind_hard.ogg`, `sfx_boulder_rumble.wav`, `sfx_boulder_roll.wav`, `sfx_ice_slide.wav`, `sfx_fanfare.wav`, `sfx_unlock.wav`.

**Theme table (for `LevelThemes.cs`):**

| Theme | Top | Soil | Ledge | Boulders | Goal name | Goal sprite (size) | Goal offset | Backdrop | Props |
|---|---|---|---|---|---|---|---|---|---|
| LavaSlope | lava_top | lava_soil | ledge_lava | _lava | Watch Station | watch_station (48×48) | (24, 24) | Backdrop/LavaSlope/ | burnt_tree |
| StormRidge | storm_top | storm_soil | ledge_storm | _storm | Lookout Tower | lookout_tower (40×64) | (20, 32) | Backdrop/StormRidge/ | bent_tree, rain_00/01 falling in front |
| EagleForest | eagle_top | eagle_soil | ledge_eagle | _eagle | Eagle Nest | eagle_nest (56×56) | (28, 28) | Backdrop/EagleForest/ | tall_tree, orchid, the Easy `eagle` flying overhead |
| BonsaiForest | bonsai_top | bonsai_soil | ledge_bonsai | _bonsai | Sanctuary Sign | sanctuary_sign (40×40) | (20, 20) | Backdrop/BonsaiForest/ | bonsai_tree, pitcher_plant |
| GlacierHorn | glacier_top | glacier_soil | ledge_glacier | _glacier | Alpine Hut | alpine_hut (56×48) | (28, 24) | Backdrop/GlacierHorn/ | edelweiss, the Normal snow flakes |
| EquatorSummit | equator_top | equator_soil | ledge_equator | _equator | Summit Board | summit_board (48×40) | (24, 20) | Backdrop/EquatorSummit/ | groundsel |
| SeaPeak | sea_top | sea_soil | ledge_sea | _sea | Telescope Dome | telescope_dome (56×48) | (28, 24) | Backdrop/SeaPeak/ | silversword |

All seven name `BackdropScript = "Tools/Art/make_hard_backdrops.py"`. Props are decoration only and never sit on a hazard, pickup or checkpoint (follow the existing `SnowProps` style).

**Other sprite sizes:** `roll_boulder_*` 16×16 (drawn centred on the boulder's middle, bottom at y 0), `rumble_dust_*` 16×8, `ice_top` 16×8 (same as the other top tiles), `ice_glint_*` 5×5, `rain_*` 8×16, `burnt_tree` 20×32, `bent_tree` 24×40, `tall_tree` 32×64, `orchid` 12×12, `bonsai_tree` 24×24, `pitcher_plant` 10×14, `edelweiss` 10×10, `groundsel` 20×40, `silversword` 16×16.

**Sorting orders:** rolling boulder 19 (like the falling rock), rumble dust 7, ice top like the ground top, ice glints 5, rain 22 (in front of the explorer, see-through).

## Not in scope

- The Completion Screen (after all four landforms).
- Hills, Plains and Plateaus levels, lessons and kinds.
- Teacher review of the content (Q-04).
- Changing Easy or Normal levels.
