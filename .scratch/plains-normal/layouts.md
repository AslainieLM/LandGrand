# Plains Normal 1–5: layouts and bot check

Date: 2026-11-22. Status: scratch design, checked by the physics bot. Not built. Not adviser-approved.

**Bot files.** Copy each `bot-check-<Name>.cs.txt` into an empty folder as `<Name>.cs`, and `bot-check-bot.csproj.txt` as `bot.csproj`, then run `dotnet run -c Release` (the argument `N3` runs one level):
- Plains Normal: `Sim`, `Program`, `PlainsNormalLevels`.
- Needed to compile: `NormalLevels` (Hills Normal, also the regression), `EasyLevels`, `HillsHardLevels`, `MountainsHardLevels`, `PlainsLevels`.

The output is in `bot-output.txt`; the working copy is `bot/`.

The sim is the Plains Easy sim (all objects so far) at walk 84 px/s. The driver is the Hills Hard driver with a 180 s limit and a Normal lint.

## Shared Normal rules

- width 7200, start (40, 0), goal 7040, checkpoints 2400 and 4800, 180 s
- 7 pickups, one secret on a high ledge (y ≥ 72)
- pits send the explorer back to the furthest flag reached

## Rule numbers

All objects reuse existing rules, unchanged; see spec section 5.

## Lint (checked by the bot)

- **The Hard lint, with 7 pickups:**
  - no pit, pool, spikes, crumbling ledge, moving platform or bat loop within 100 px of a checkpoint
  - no current, ice, mud or wind within 40 px of a checkpoint; no boulder on a checkpoint
  - no hazard before x 200 or after goal − 120
  - no pit or spikes within 80 px of a current
  - float water gaps of 40 px at most; a pit or pool next to a float may not have a gap wider than one jump
  - one secret at y ≥ 72
- **The Normal additions:**
  - exactly 2 checkpoints
  - a ground-line boat must span its pit **or safe pool**: X0 = its left edge, and X1 + 48 ≥ its right edge
  - the first float of an introducing level sits over a safe pool
  - the first current of an introducing level pushes with the player
- **Every ledge is reachable.** This is a closure: each new ledge is reached from the ground or from ledges already reached.
- **Robustness:** every level is also run with all timed objects shifted by 0.25–3.25 s (13 runs).

## Coordinates and shorthand

- Coordinates are pixels: x from the level start, y up from the ground line.
- S = small boulder 32×24, B = big boulder 48×36. `L(x, top, width)` = a ledge. Spikes are 32 wide.
- Pickups give their bottom-middle spot.
- Floats give their left edge (top at the water line). A pool is (x, width), 16 px deep and safe.
- A crumbling bridge is three 32 px crumbling ledges at top 0 (x, x + 32, x + 64) over a 96 px pit.
- Currents are (x, width, direction +1 right / −1 left).
- Boats are moving platforms (x0, x1, top 0): they ride from x0 to x1 and back.

## N1 Candaba Swamp (CandabaWetland)

- Pits (water): (1700, 104), (3400, 48), (6000, 192)
- Spikes: 2900, 5300
- Pool (safe): (800, 96)
- Floats: 824 (practice, over the pool), 1728, 6032, 6112
- Boulders: S 420, B 1400, S 2700, B 3900, S 4500, B 5700, S 6700
- Ledges: L(1100, 40, 64), L(4200, 40, 64), L(4280, 84, 64)
- Pickups: peat (436, 24); peat (848, 40); clay (1132, 40); siltstone (2600, 0); mudstone (3424, 48); **peat (4312, 84) secret**; clay (6716, 24)

## N2 Pinatubo Lahar Plain (LaharPlain)

- Pits (dry): (1300, 96), (3600, 56), (5700, 96)
- Crumbling bridges: 1300, 1332, 1364 and 5700, 5732, 5764 (top 0)
- Spikes: 2900, 4900, 6600
- Soft sand: (500, 200) to learn on, (2000, 300), (4100, 350), (6150, 300)
- Boulders: S 900, B 1700, S 2600, B 3200, S 4500, B 5300, S 6800
- Ledges: L(1000, 40, 64), L(3900, 40, 64), L(3980, 84, 64), L(5400, 28, 64)
- Pickups: pumice (916, 24); dacite (1032, 40); andesite (2200, 0); pumice (3224, 36); **dacite (4012, 84) secret**; andesite (5432, 28); pumice (6816, 24)

## N3 Agno River Delta (RiverDelta)

- Pits (water): (1300, 48), (3200, 64), (5200, 56)
- Spikes: 2800, 4500, 6500
- Currents: (600, 200, +1) to learn on, (1900, 300, −1), (3900, 400, +1), (5800, 300, −1)
- Boulders: S 400, B 1000, S 2200, B 3500, B 5500, S 6700
- Ledges: L(1600, 40, 64), L(3600, 40, 64), L(3680, 84, 64), L(6300, 28, 64)
- Pickups: gold (416, 24); quartz (700, 0); gold (1632, 40); siltstone (2650, 0); **gold (3712, 84) secret**; quartz (5000, 0); mudstone (6716, 24)

## N4 Agusan Marsh (FloatingVillage)

- Pits (water): (1800, 104), (3800, 56), (6200, 64)
- Spikes: 1200, 4300, 6600
- Float: 1828
- Soft sand: (3000, 300)
- Current: (5000, 300, +1)
- Boulders: S 500, B 2600, S 3500, B 4500, S 5600, B 6800
- Ledges: L(900, 40, 64), L(4000, 40, 64), L(4080, 84, 64), L(5800, 28, 64)
- Pickups: peat (516, 24); clay (932, 40); mudstone (1852, 40); peat (3150, 0); **clay (4112, 84) secret**; mudstone (5150, 0); peat (6000, 0)

## N5 Mekong Delta (FloatingMarket)

- Pits (water): (2000, 200), (3600, 200), (5900, 200)
- Pool (safe): (800, 160)
- Boats: (800, 912, 0) practice over the pool, (2000, 2152, 0), (3600, 3752, 0), (5900, 6052, 0)
- Spikes: 1500, 3100, 5300
- Boulders: S 450, B 1250, S 2700, B 4300, S 5000, S 6700
- Ledges: L(1100, 40, 64), L(4000, 40, 64), L(4080, 84, 64), L(6400, 28, 64)
- Pickups: laterite (466, 24); clay (1132, 40); laterite (2076, 40), over the first river pit, reached from the boat; siltstone (2900, 0); **laterite (4112, 84) secret**; clay (5100, 0); mudstone (6716, 24)

## Bot output (2026-11-22)

```
-- Regression, Hills Normal at walk 84 (expect N1 83.8, N2 85.9, N3 84.2, N4 86.2, N5 91.1):
N1 83.8  N2 85.9  N3 84.2  N4 86.2  N5 91.1
-- Plains Normal:
N1 Candaba Swamp: bot GOAL 83.8s (goal); pickups 7/7 reachable lint clean
   start-time shifts 0.25..3.25 s (every timed object out of phase): 13/13 reach the goal, 83.8-83.8 s
   floats (s standing): 824:0.25, 1728:0.30, 6032:0.02, 6112:0.32
N2 Pinatubo Lahar Plain: bot GOAL 93.0s (goal); pickups 7/7 reachable lint clean
   start-time shifts 0.25..3.25 s (every timed object out of phase): 13/13 reach the goal, 93.0-93.0 s
   crumbling ledges: 6
N3 Agno River Delta: bot GOAL 85.8s (goal); pickups 7/7 reachable lint clean
   start-time shifts 0.25..3.25 s (every timed object out of phase): 13/13 reach the goal, 85.8-85.8 s
   currents (s in strip): 600R:1.75, 1900L:5.37, 3900R:3.52, 5800L:5.57
N4 Agusan Marsh: bot GOAL 85.2s (goal); pickups 7/7 reachable lint clean
   start-time shifts 0.25..3.25 s (every timed object out of phase): 13/13 reach the goal, 85.2-85.2 s
   floats (s standing): 1828:0.28
   currents (s in strip): 5000R:2.63
N5 Mekong Delta: bot GOAL 98.4s (goal); pickups 7/7 reachable lint clean
   start-time shifts 0.25..3.25 s (every timed object out of phase): 13/13 reach the goal, 95.2-98.2 s
   without moving platforms: FAILED (pit at x=2085)
```

The last line shows that the N5 boats are needed: without them, the 200 px river pits cannot be crossed.
