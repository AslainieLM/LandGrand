# Plains Hard 1–7: layouts and bot check

Date: 2026-11-22. Status: scratch design, checked by the physics bot. Not built. Not adviser-approved.

**Bot files.** Copy each `bot-check-<Name>.cs.txt` into an empty folder as `<Name>.cs`, and `bot-check-bot.csproj.txt` as `bot.csproj`, then run `dotnet run -c Release` (the argument `H3` runs one level):
- Plains Hard: `Sim`, `Program`, `PlainsHardLevels`.
- Needed to compile: `HillsHardLevels` (also the regression), `NormalLevels`, `EasyLevels`, `MountainsHardLevels`, `PlainsLevels`, `PlainsNormalLevels`.

The output is in `bot-output.txt`; the working copy is `bot/`. The sim is the Plains Normal sim (all objects so far) at walk 84 px/s. The driver is the Hills Hard driver (240 s, Hard lint) plus the herd, ice and straw checks below.

## Shared Hard rules

- width 9600, start (40, 0), goal 9440, checkpoints 2400, 4800 and 7200, 240 s
- 9 pickups, one secret on a high ledge (y ≥ 72)
- pits send the explorer back to the furthest flag reached

## How the new looks map to old rules

- **Hay bale** = `RollingLog` (spawn, end, direction, offset), unchanged numbers: 90 px/s, 3.5 s cycle, 0.8 s warning, 14 px hit box. Only the frames and sound differ (theme data).
- **Herd animal** = `Bat` on the ground: `new Bat(cx, 6, 48, 0, period, phase)`. Centre y 6 and up-down radius 0, so the 16×12 touch box runs from y 0 to 12 and the animal walks 96 px back and forth (left-right radius 48). Same push (24 px over 0.2 s), same 1 s rest, never hurts. The explorer can jump over it.
- **Frozen ground** = `IceZone`, unchanged numbers. Only the top tile differs (`frost_top`).

## Lint (checked by the bot)

- **The Hard lint:** no pit, pool, spikes, crumbling ledge or bat/herd loop within 100 px of a checkpoint; no current, ice or mud within 40 px of a checkpoint; no boulder on a checkpoint; no hazard before x 200 or after goal − 120; no pit or spikes within 80 px of a current, bat or herd loop; float water gaps of 40 px at most; logs (hay) never cross pits, spikes, boulders, currents or checkpoints (100 px); one secret at y ≥ 72; 9 pickups; no pickup inside a bat or herd loop; no ground pickup on a hay run.
- **New for Plains Hard:**
  - a herd never walks into a boulder or straw pile (8 px margin) and keeps 40 px from a hay run
  - no pit or spikes inside an ice stretch
  - no hay run crosses a straw pile (8 px)
  - no straw pile on a hazard or within 100 px of a checkpoint
  - in an introducing level, the first hay bale rolls toward the player in a safe spot (120 px), and the first herd has no hay within 120 px
- **Every ledge is reachable** (closure from the ground and reached ledges).
- **Robustness:** every level is also run with all timed objects shifted by 0.25–3.25 s (13 runs).

## Coordinates and shorthand

- Coordinates are pixels: x from the level start, y up from the ground line.
- S = small boulder 32×24, B = big boulder 48×36. `L(x, top, width)` = a ledge. Spikes are 32 wide.
- Pickups give their bottom-middle spot.
- Hay: `HayL(spawn, end, offset)` rolls left (direction −1); `HayR(spawn, end, offset)` rolls right (+1).
- Herd: `Herd(cx, period, phase)` = `Bat(cx, 6, 48, 0, period, phase)`. Bat: `Bt(cx, period, phase)` = `Bat(cx, 16, 24, 12, period, phase)`.
- Ice: (x, width). Straw pile: left edge x (48×16, unchanged rule).
- Floats give their left edge (top at the water line). Currents are (x, width, direction).
- A crumbling bridge is three 32 px crumbling ledges at top 0 (x, x + 32, x + 64) over a 96 px pit.

## H1 Great Plains (Prairie) — introduces hay

- Pits (dry): (1500, 56), (2900, 64), (4000, 64), (5600, 64), (6700, 64), (8500, 64)
- Spikes: 1250, 2700, 3260, 4650, 5900, 7000, 8650
- Boulders: S 400, B 1650, S 2200, B 3100, S 4900, B 5450, S 6500, B 7300, S 8000, B 9100
- Ledges: L(2500, 40, 64), L(3700, 40, 64), L(3780, 84, 48)
- Straw piles: 2050, 4550, 6900
- Hay: HayL(1000, 800, 0) (practice), HayL(2000, 1800, 1.0), HayL(3600, 3380, 0), HayR(4300, 4520, 0.5), HayL(5300, 5080, 2.0), HayR(6100, 6330, 1.0), HayL(7800, 7580, 1.5), HayR(8100, 8320, 2.5), HayL(9000, 8780, 0.5)
- Pickups: chalk (416, 24); calcite (1528, 48); chalk (2532, 40); limestone (2932, 48); **chalk (3804, 84) secret**; shale (4916, 24); calcite (6732, 48); limestone (7324, 36); sandstone (8532, 48)

## H2 Serengeti (Savanna) — introduces the herd

- Pits (dry): (1500, 56), (3000, 64), (4300, 64), (6500, 64), (8300, 64)
- Spikes: 1200, 2800, 3900, 5500, 6900, 8600
- Boulders: S 380, B 1000, S 1900, S 2100, B 3500, S 4500, S 5200, B 5800, S 6800, B 7800, S 9000
- Ledges: L(2600, 40, 64), L(4000, 48, 64), L(6100, 40, 64), L(6180, 84, 48), L(8900, 48, 64)
- Herds (wildebeest): Herd(650, 5.0, 0) (practice), Herd(2000, 4.6, 1.0), Herd(3650, 4.8, 0.5), Herd(5000, 4.4, 2.0), Herd(6100, 5.0, 1.5), Herd(7600, 4.6, 0.8)
- Pickups: granite (396, 24); tuff (1100, 0); granite (1528, 48); tuff (2632, 40); sandstone (3032, 48); granite (4032, 48); **tuff (6204, 84) secret**; siltstone (7824, 36); shale (8932, 48)

## H3 Pampas (Pampas)

- Pits (dry): (1350, 56), (2000, 64), (3100, 64), (4550, 56), (6000, 64), (8800, 64)
- Spikes: 1200, 3250, 4400, 5900, 6800
- Boulders: S 400, S 800, B 1050, S 2200, B 3500, S 4200, B 5350, S 6400, B 8200, S 8350
- Ledges: L(6900, 40, 64), L(6980, 84, 48)
- Hay: HayL(1700, 1480, 0), HayR(3900, 4120, 1.0), HayL(5800, 5580, 2.0), HayR(7900, 8130, 0.5), HayL(8700, 8480, 1.5)
- Herds (cattle): Herd(2740, 4.8, 0), Herd(5230, 4.4, 1.0), Herd(7560, 5.0, 2.0)
- Pickups: loess (416, 24); mica (1378, 48); feldspar (2400, 0); loess (3132, 48); quartz (3524, 36); mica (4900, 0); feldspar (6032, 48); **loess (7004, 84) secret**; clay (8832, 48)

## H4 West Siberian Plain (FrozenPlain)

- Pits: (1500, 56) dry, (3000, 104) **water**, (4300, 64) dry, (6500, 104) **water**, (8300, 64) dry
- Spikes: 1200, 2800, 3900, 5500, 6900, 8600
- Boulders: S 380, B 1000, S 1900, S 2100, B 3500, S 4500, S 5200, B 5800, S 6800, B 7800, S 9000
- Ledges: L(2600, 40, 64), L(4000, 48, 64), L(6100, 40, 64), L(6180, 84, 48), L(8900, 48, 64)
- Ice (frozen ground): (500, 300), (1700, 500), (3200, 500), (5000, 400), (6000, 400), (7500, 600), (8800, 400)
- Floats (bog ponds): 3028, 6528
- Ambience override: `amb_cold_plain`
- Pickups: peat (396, 24); clay (650, 0); siltstone (1528, 48); peat (2632, 40); mudstone (3052, 40); sandstone (4032, 48); **peat (6204, 84) secret**; clay (7824, 36); siltstone (8932, 48)

## H5 Nullarbor Plain (LimestonePlain)

- Pits (dry): (1500, 56), (2800, 96), (4000, 64), (5600, 96), (6600, 64), (8000, 96), (8900, 56)
- Spikes: 1200, 3300, 4450, 6250, 7600, 8650
- Boulders: S 400, B 1000, S 2200, B 3600, S 4300, B 5300, S 6900, B 7400, S 8400
- Ledges: L(3020, 40, 64), L(6980, 96, 48), L(9220, 48, 64)
- Crumbling: bridges at 2800, 5600, 8000; steps (6830, top 40), (6900, top 64)
- Bats: Bt(1800, 3.0, 0), Bt(2150, 2.8, 1.0), Bt(3800, 2.6, 0.4), Bt(4600, 2.8, 0.6), Bt(5950, 2.6, 1.2), Bt(9150, 2.6, 0.3)
- Pickups: flint (416, 24); limestone (1528, 48); calcite (2832, 40); flint (3052, 40); chalk (4032, 48); limestone (5632, 40); **flint (7004, 96) secret**; calcite (8032, 40); limestone (9252, 48)

## H6 Indo-Gangetic Plain (GangesPlain)

- Pits (all **water**): (1200, 200), (2000, 112), (3200, 64), (3800, 200), (5000, 192), (6200, 200), (7400, 112), (8000, 200)
- Spikes: 1700, 3500, 5500, 7800, 9000
- Boulders: S 400, B 1500, S 2250, B 3000, S 4600, B 5300, S 6000, B 7000, S 8300, B 9150
- Ledges: L(4100, 40, 64), L(4180, 84, 48)
- Floats: 1232, 1312, 2032, 3832, 3912, 5032, 5112, 6232, 6312, 7432, 8032, 8112
- Currents: (550, 450, +1), (2500, 550, −1), (4100, 600, −1), (5650, 450, +1), (6500, 600, −1), (8300, 600, −1)
- Pickups: laterite (416, 24); clay (1300, 40); siltstone (2056, 32); sandstone (3232, 48); clay (3900, 40); **laterite (4204, 84) secret**; mudstone (5136, 32); siltstone (6300, 40); laterite (8100, 40)

## H7 Mongolian Steppe (Steppe) — the finale

- Pits (dry): (1500, 96), (3000, 64), (3600, 64), (4000, 64), (6100, 96), (7800, 96), (8400, 64)
- Spikes: 1000, 5500, 9250
- Boulders: S 400, B 1150, S 2200, B 4600, S 5200, B 7550, S 8700
- Ledges: L(6540, 96, 48)
- Crumbling: bridges at 1500, 6100, 7800; steps (6400, top 40), (6470, top 64)
- Hay: HayL(2000, 1780, 0.5), HayR(4300, 4520, 1.0), HayL(6900, 6680, 2.0), HayR(8950, 9170, 1.5)
- Ice (frozen ground): (500, 400), (4150, 400), (5000, 400), (8550, 350)
- Herds (horses): Herd(1300, 4.6, 0), Herd(5800, 4.4, 0.8)
- Currents: (2480, 400, −1), (7300, 400, −1)
- Pickups: granite (416, 24); chalk (700, 0); loess (1532, 40); flint (2750, 0); tuff (3032, 48); mica (4032, 48); **loess (6564, 96) secret**; feldspar (7856, 32); granite (8432, 48)

## Bot output (2026-11-22)

| Level | Bot time | Shifted runs | Pickups | Lint |
|---|---|---|---|---|
| H1 Great Plains | 112.6 s | 13/13, 112.6 s | 9/9 | clean (straw piles moved from 2300/4750/7100, which were too near checkpoints) |
| H2 Serengeti | 115.7 s | 13/13, 115.5–117.9 s | 9/9 | clean; each herd pushed the bot once |
| H3 Pampas | 115.0 s | 13/13, 114.1–115.7 s | 9/9 | clean |
| H4 West Siberian Plain | 112.6 s | 13/13, 112.6 s | 9/9 | clean |
| H5 Nullarbor Plain | 115.5 s | 13/13, 115.5–117.2 s | 9/9 | clean |
| H6 Indo-Gangetic Plain | 123.8 s | 13/13, 123.8 s | 9/9 | clean |
| H7 Mongolian Steppe | 118.0 s | 13/13, 118.1–119.2 s | 9/9 | clean |

Regression: the Hills Hard layouts give 112.6–132.0 s, as recorded in D-068. Full output: `bot-output.txt`.
