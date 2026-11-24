# Plateaus Hard 1–7: layouts and bot check (D-074)

Date: 2026-11-24. Status: scratch design, checked by the physics bot. Not built. Not adviser-approved.

**Bot files.** Copy each `bot-check-<Name>.cs.txt` into an empty folder as `<Name>.cs`, and `bot-check-bot.csproj.txt` as `bot.csproj`, then run `dotnet run -c Release` (the argument `H3` runs one Plateaus Hard level; `new` skips the Plains Hard regression):
- Plateaus Hard: `Sim`, `Program`, `PlateausHardLevels`.
- Needed to compile: `PlainsHardLevels` (also the regression), `HillsHardLevels` (also the regression), `PlainsNormalLevels`, `PlateausNormalLevels`.

The output is in `bot-output.txt`; the working copy is `bot/`. The sim is the Plateaus Normal sim (every object so far) at walk 84 px/s, with only a vent-launch counter added. The driver is the Plateaus Normal driver back on the Hard lint (9 pickups, 3 checkpoints, 240 s), keeping the updraft reach search and updraft fairness lint, plus the new checks below.

## Shared Hard rules

- width 9600, start (40, 0), goal 9440, checkpoints 2400, 4800 and 7200, 240 s
- 9 pickups, exactly one secret on a high ledge (y ≥ 72)
- walk 84 px/s; pits and spikes send the explorer back to the furthest flag reached
- no new rules: every object below is an existing rule with its existing numbers

## How the new looks map to old rules

- **Hot spring** = `SteamVent(x)` (24 wide), unchanged: 2 s cycle on the shared level clock, warning at 1.2 s, puff from 1.55 s, launch 480 px/s (about 128 px up; with the double jump, which the launch makes ready, about 157 px). Each hot spring has its own ledge at `x + 40`, top 112, 64 wide, which a plain jump (76 px), a boulder (35 + 76) or a 40 ledge (40 + 76) cannot reach.
- **Gelada troop / yak herd** = `Bat` on the ground: `new Bat(cx, 6, 48, 0, period, phase)`, as in Plains Hard. Walks 96 px back and forth, pushes 24 px over 0.2 s, 1 s rest, never hurts.
- **Sand gust / snow gust** = `WindZone(x, width, offset)`, unchanged: 4 s cycle, warning at 2 s, push from 2.6 s, always **left** at 40 px/s, on the ground and in the air.
- **Blue ice** = `IceZone`, unchanged (speed up 160, slow down 100 px/s²).
- **Desert sand / loess** = `SandZone`, unchanged (walk at 60%).
- **Sandstone slab** = `MovingPlatform`: horizontal `new MovingPlatform(x0, x1, 0)` (48 wide, 40 px/s) across a pit, or vertical `MovingPlatform.Vertical(x, 24, 120, phase)` (rise 96, inside the 48–96 rule).
- **Cliff updraft** = `Updraft(x, top)`, the Plateaus Easy rule: landing ledge at `x + 40`, `top − 4`.
- **Crumbling ledge / terrace edge** = `CrumbleLedge`, unchanged (0.5 s, back after 3 s).

## Lint (checked by the bot)

- **The Hard lint** (Plains Hard, unchanged): no pit, pool, spikes, crumbling ledge, moving platform or herd loop within 100 px of a checkpoint; no current, ice, mud, wind or vertical slab within 40 px of a checkpoint; no boulder on a checkpoint; no hazard (pits, spikes, herds, crumbles, mud, gusts) before x 200 or after goal − 120; no pit or spikes within 80 px of a herd or current; no pit or spikes inside ice; a herd never walks into a boulder; a vertical slab never stands over a pit and rises 48–96; a horizontal slab spans its pit; one secret at y ≥ 72; 9 pickups; no pickup inside a herd loop.
- **Updraft fairness** (Plateaus Normal, unchanged): 60 px from pits and spikes, 40 px from boulders and checkpoints, head at or below 196, landing ledge at `x + 40`, `top − 4`, no ledge in or over the column, not on sand, ice or a current.
- **New for Plateaus Hard:**
  - no pit or spikes inside a sand stretch
  - a gust zone that overlaps ice has no pit or spikes inside it or within 80 px of either end (a pushed, sliding player always has room)
  - a vent is not within 100 px of a checkpoint, not within 120 px of a pit or spikes (a launch never comes down on one), not within 40 px of a boulder or herd, not on ice, sand or a current, not at the start or goal
  - no herd (its walk plus the 8 px touch margin) inside a gust zone
  - safe first encounters for objects new to Plateaus: the first vent (H1), first gust (H3) and first vertical slab (H4) have no pit or spikes within 120 px; the first gust is not over ice
  - every ledge is reachable (closure from the ground and reached ledges, riding updrafts, vents and slabs); the report reruns the reach search without each lifting object and names the ledges and pickups that need it, so every vent-reached ledge is found by the reach search, not assumed
- **Robustness:** every level is also run with all timed objects shifted by 0.25–3.25 s (13 runs).

## Coordinates and shorthand

- Coordinates are pixels: x from the level start, y up from the ground line.
- S = small boulder 32×24, B = big boulder 48×36. `L(x, top, width)` = a ledge. Spikes are 32 wide.
- Pickups give their bottom-middle spot, with the `ItemCatalog` kind id.
- A crumbling bridge is three 32 px crumbling ledges at top 0 (x, x + 32, x + 64) over a 96 px pit.
- `Herd(cx, period, phase)` = `new Bat(cx, 6f, 48f, 0f, period, phase)`.
- `Vent(x)` = `new SteamVent(x)`; `Gust(x, w, offset)` = `new WindZone(x, w, offset)`; `Ice(x, w)` = `new IceZone(x, w)`; `Sand(x, w)` = `new SandZone(x, w)`.
- `Slab(x0, x1)` = `new MovingPlatform(x0, x1, 0f)`; `VSlab(x, phase)` = `MovingPlatform.Vertical(x, 24f, 120f, phase)`; `U(x, top)` = `new Updraft(x, top)`.

## H1 Deccan Plateau — introduces the hot springs (vents)

Plains Hard H5 skeleton, bats removed, three vents with their ledges added.

- Pits (dry): (1500, 56), (2800, 96), (4000, 64), (5600, 96), (6600, 64), (8000, 96), (8900, 56)
- Spikes: 1200, 3300, 4450, 6250, 7600, 8650
- Boulders: S 400, B 1000, S 2200, B 3600, S 4300, B 5300, S 6900, B 7400, S 8400
- Ledges: L(1840, 112, 64), L(3020, 40, 64), L(5040, 112, 64), L(6980, 96, 48), L(8260, 112, 64)
- Crumbling: bridges at 2800, 5600, 8000; steps (6830, top 40), (6900, top 64)
- Hot springs: Vent(1800) (the safe first one: nothing within 120 px), Vent(5000), Vent(8220)
- Pickups (basalt 3, zeolite 2, calcite 2, olivine 2): basalt (416, 24); zeolite (1872, 112) vent ledge; calcite (2832, 40); basalt (3052, 40); olivine (4032, 48); zeolite (5072, 112) vent ledge; **basalt (7004, 96) secret** on the crumbling steps; olivine (8032, 40); calcite (8292, 112) vent ledge

```csharp
vents: new[] { new SteamVent(1800f), new SteamVent(5000f), new SteamVent(8220f) },
crumbles: bridges 2800, 5600, 8000 + new CrumbleLedge(6830f, 40f), new CrumbleLedge(6900f, 64f)
```

## H2 Ethiopian Highlands — gelada troops

Plains Hard H2 skeleton: four of its six herds kept, pits 3000 and 6500 widened to 96 with crumbling bridges.

- Pits (dry): (1500, 56), (3000, 96), (4300, 64), (6500, 96), (8300, 64)
- Spikes: 1200, 2800, 3900, 5500, 6900, 8600
- Boulders: S 380, B 1000, S 1900, S 2100, B 3500, S 4500, S 5200, B 5800, S 6800, B 7800, S 9000
- Ledges (cliff ledges): L(2600, 40, 64), L(4000, 48, 64), L(6100, 40, 64), L(6180, 84, 48), L(8900, 48, 64)
- Crumbling: bridges at 3000, 6500
- Gelada troops: Herd(650, 5.0, 0), Herd(2000, 4.6, 1.0), Herd(5000, 4.4, 2.0), Herd(7600, 4.6, 0.8)
- Pickups (basalt 3, ignimbrite 2, tuff 2, opal 2): basalt (396, 24); ignimbrite (1100, 0); basalt (1528, 48); tuff (2632, 40); opal (3032, 40); ignimbrite (4032, 48); **opal (6204, 84) secret**; tuff (7824, 36); basalt (8932, 48)

## H3 Tassili n'Ajjer — introduces sand gusts

Plains Hard H3 skeleton, hay and herds removed, four gusts and three sand stretches added.

- Pits (dry): (1350, 56), (2000, 64), (3100, 64), (4550, 56), (6000, 64), (8800, 64)
- Spikes: 1200, 3250, 4400, 5900, 6800
- Boulders: S 400, S 800, B 1050, S 2200, B 3500, S 4200, B 5350, S 6400, B 8200, S 8350
- Ledges (sandstone): L(6900, 40, 64), L(6980, 84, 48)
- Sand gusts: Gust(500, 400, 0) (the safe first one), Gust(2500, 500, 1.0), Gust(5000, 500, 0.5), Gust(7600, 500, 1.5)
- Soft sand: Sand(1600, 300), Sand(3700, 350), Sand(8400, 300)
- Pickups (sandstone 4, granite 2, quartz 2, clay 1): sandstone (416, 24); granite (1378, 48); quartz (2400, 0); sandstone (3132, 48); clay (3524, 36); granite (4900, 0); sandstone (6032, 48); **sandstone (7004, 84) secret**; quartz (8832, 48)

## H4 Mesa Verde — cliff alcoves, updrafts and sandstone slabs; introduces the vertical slab

Built from the Hills Hard H4 skeleton (its wide gap with the vertical slab and the long ledge), with horizontal slabs over two 200 px pits and two updrafts.

- Pits (dry): (850, 56), (1800, 200), (2700, 64), (4000, 64), (6000, 160), (7700, 200), (8600, 64)
- Spikes: 1200, 3100, 5700, 6900, 9100
- Boulders: S 400, B 1350, S 2500, S 3200, S 4200, B 5200, S 6400, B 7000, S 8300
- Ledges (alcoves): L(2240, 112, 64), L(3552, 112, 64), L(4540, 144, 56), L(5992, 112, 176)
- Horizontal slabs: Slab(1800, 1952), Slab(7700, 7852) (each spans its 200 px pit; the pit cannot be jumped)
- Vertical slabs: VSlab(3500, 0) (the safe first one, optional, leads to the 3552 alcove), VSlab(5944, 1.0) (needed: it lifts the explorer onto the 176 px ledge over the 160 px gap)
- Updrafts: U(2200, 116) → ledge L(2240, 112, 64); U(4500, 148) → ledge L(4540, 144, 56), the secret alcove
- Pickups (sandstone 4, shale 2, coal 3): sandstone (416, 24); shale (878, 48); coal (1876, 40) above the first slab; sandstone (2272, 112) updraft alcove; coal (3584, 112) vertical-slab alcove; shale (4032, 48); **sandstone (4568, 144) secret**, needs updraft 4500; coal (6080, 112) on the long ledge; sandstone (7776, 40) above the second slab

```csharp
platforms: new[] { new MovingPlatform(1800f, 1952f, 0f), new MovingPlatform(7700f, 7852f, 0f),
                   MovingPlatform.Vertical(3500f, 24f, 120f, 0f), MovingPlatform.Vertical(5944f, 24f, 120f, 1.0f) },
updrafts: new[] { new Updraft(2200f, 116f), new Updraft(4500f, 148f) }
```

## H5 Antarctic Plateau — blue ice and snow gusts

Plains Hard H4 skeleton, floats removed (both water pits become 64 px dry crevasses), four of its seven ice stretches kept, four gusts added. Three gusts blow over ice; none has a pit or spikes inside it or within 80 px.

- Pits (crevasses, dry): (1500, 56), (3000, 64), (4300, 64), (6500, 64), (8300, 64)
- Spikes: 1200, 2800, 3900, 5500, 6900, 8600
- Boulders: S 380, B 1000, S 1900, S 2100, B 3500, S 4500, S 5200, B 5800, S 6800, B 7800, S 9000
- Ledges: L(2600, 40, 64), L(4000, 48, 64), L(6100, 40, 64), L(6180, 84, 48), L(8900, 48, 64)
- Blue ice: Ice(1700, 500), Ice(3200, 500), Ice(5000, 400), Ice(7500, 600)
- Snow gusts: Gust(600, 400, 0) on plain snow, Gust(1700, 400, 1.0) on ice, Gust(5000, 400, 0.5) on ice, Gust(7500, 500, 1.5) on ice
- No herds.
- Pickups (meteorite 3, granite 2, gneiss 2, coal 2): granite (396, 24); meteorite (650, 0); gneiss (1528, 48); coal (2632, 40); granite (3032, 48); gneiss (4032, 48); **meteorite (6204, 84) secret**; coal (7824, 36); meteorite (8932, 48)

## H6 Loess Plateau — soft loess and crumbling terrace edges

Plains Hard H7 skeleton, hay, ice, herds and currents removed; three loess stretches and three more spike strips added.

- Pits (dry): (1500, 96), (3000, 64), (3600, 64), (4000, 64), (6100, 96), (7800, 96), (8400, 64)
- Spikes: 1000, 2600, 4400, 5500, 7000, 9250
- Boulders: S 400, B 1150, S 2200, B 4600, S 5200, B 7550, S 8700
- Ledges: L(6540, 96, 48)
- Crumbling terrace edges: bridges at 1500, 6100, 7800; steps (6400, top 40), (6470, top 64)
- Loess: Sand(1800, 400), Sand(4900, 500), Sand(8800, 350)
- Pickups (loess 3, quartz 2, feldspar 2, mica 1, calcite 1): loess (416, 24); quartz (700, 0); feldspar (1532, 40); loess (2750, 0); mica (3032, 48); quartz (4032, 48); **loess (6564, 96) secret**; feldspar (7856, 32); calcite (8432, 48)

## H7 Tibetan Plateau — the finale

Plains Hard H7 / Hills Hard H7 skeleton: the 160 px gap with the vertical slab, three gusts (one over ice), four ice stretches, crumbling bridges, two updrafts, three yak herds.

- Pits (dry): (1500, 96), (3000, 64), (3600, 64), (3956, 160), (6100, 96), (7800, 96), (8400, 64)
- Spikes: 1000, 5500, 9250
- Boulders: S 400, B 1150, S 2200, B 4600, S 5200, B 7550, S 8700
- Ledges: L(2040, 112, 64), L(3948, 112, 176), L(6540, 96, 48), L(7040, 144, 56)
- Crumbling: bridges at 1500, 6100, 7800; steps (6400, top 40), (6470, top 64)
- Ice: Ice(500, 400), Ice(4200, 400), Ice(5000, 400), Ice(8550, 350)
- Gusts: Gust(3300, 500, 1.0) over the 3600 pit, Gust(4250, 350, 0.5) on ice, Gust(8100, 400, 1.5) over the 8400 pit
- Yak herds: Herd(1300, 4.6, 0), Herd(2700, 4.8, 0.5), Herd(5800, 4.4, 0.8)
- Updrafts: U(2000, 116) → L(2040, 112, 64); U(7000, 148) → L(7040, 144, 56)
- Vertical slab: VSlab(3900, 0) (needed: lifts the explorer onto L(3948, 112, 176) over the 160 px gap)
- Pickups (halite 3, granite 2, chromite 2, serpentinite 2): halite (416, 24); granite (700, 0); chromite (1532, 40); serpentinite (2072, 112) updraft ledge; halite (3032, 48); granite (4040, 112) on the long ledge; **halite (6564, 96) secret**; serpentinite (7068, 144) updraft ledge; chromite (8432, 48)

## Kind ids

`basalt`, `ignimbrite`, `tuff`, `sandstone`, `granite`, `quartz`, `shale`, `gneiss`, `halite` are on the Plateaus shelf already. `zeolite`, `calcite`, `olivine`, `clay`, `loess`, `feldspar`, `mica`, `chromite`, `serpentinite` exist but need `Episode.Plateaus` added (as D-072/D-073 did). `opal`, `coal` and `meteorite` are new ids (see `kinds-research.md`). Not decided here.

## Bot output (2026-11-24)

| Level | Bot time | Shifted runs | Pickups | Lint | Needs a lifting or moving object |
|---|---|---|---|---|---|
| H1 Deccan Plateau | 112.6 s | 13/13, 112.5–112.7 s | 9/9 | clean | vents: zeolite (1872,112), zeolite (5072,112), calcite (8292,112) |
| H2 Ethiopian Highlands | 114.7 s | 13/13, 114.4–116.8 s | 9/9 | clean | none; each troop pushed the bot once |
| H3 Tassili n'Ajjer | 124.4 s | 13/13, 124.1–125.0 s | 9/9 | clean | none; pushed 9.3 s, 18.7 s on sand |
| H4 Mesa Verde | 128.3 s | 13/13, 126.6–133.9 s | 9/9 | clean | updrafts: sandstone (2272,112), **secret sandstone (4568,144)**; vertical slabs: coal (3584,112), coal (6080,112); horizontal slabs: coal (1876,40), sandstone (7776,40) |
| H5 Antarctic Plateau | 117.6 s | 13/13, 115.5–117.4 s | 9/9 | clean | none; pushed 10.8 s (3 gusts over ice) |
| H6 Loess Plateau | 121.6 s | 13/13, 121.6 s | 9/9 | clean | none; 23.3 s on loess |
| H7 Tibetan Plateau | 123.2 s | 13/13, 119.9–122.9 s | 9/9 | clean | updrafts: serpentinite (2072,112), serpentinite (7068,144); vertical slab: granite (4040,112) |

- Every ledge is reachable. The ledges reached only with a vent, updraft or slab are found by the reach search with that object present and lost without it (lines "without ..." in `bot-output.txt`).
- Without the horizontal slabs H4 fails at the 200 px pit 1800; without the vertical slabs H4 fails at the 160 px gap 6000 and H7 at the gap 3956.
- H1 sits at the 112 s floor (a 9400 px walk at 84 px/s is 111.9 s; vents and crumbles never make the bot wait). The time order is not the difficulty order: H4 is slowest because the bot waits for four slabs; H7 has the most object types.
- Design changes found by the bot: H5 crevasses 3000 and 6500 were 96 px first; the bot fell at 3000 (spikes 2800 then a 96 px dry pit), so both are 64 px. H1 vent 3 moved from 8200 to 8220 (it was 104 px from pit 8000; the vent lint needs 120).
- **Regression:** Hills Hard 112.6–132.0 s and all seven Plains Hard reports are unchanged from `../plains-hard/bot-output.txt`; the only difference is the new "ice: N stretches" stat line under Plains Hard H4 and H7.
- **Bot vs game code:** no number differs (SteamVent 2 s / 1.55 s / 480; WindZone 4 s / 2.6 s / 40 left, on ground and in air, added after the ice speed; IceSlide 160/100; SoftSand 0.6; MovingPlatform 40 px/s, rise 48–96; Updraft 32 wide, 90 px/s; Bat 16×12, 24 px over 0.2 s, 1 s rest; CrumbleLedge 0.5 s + 3 s). Small differences: the game counts zone and vent edges as inside (`>=`, `<=`), the bot as outside (one frame at most); the game launches from a vent whenever grounded with the feet over it, the bot only on the ground line (vents here are all on flat ground, so no effect); the game buffers jump presses, the bot does not (the game is kinder).
