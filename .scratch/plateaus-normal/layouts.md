# Plateaus Normal 1–5: exact layouts (D-073)

Copy these numbers exactly into `LevelCatalog`. Pixels: x across from the level start, y up from the ground line. Every level: width 7200, start (40, 0), goal 7040, checkpoints 2400 and 4800, 180 s, 7 pickups with exactly one secret (y ≥ 72). `S(x)` small boulder 32×24, `B(x)` big boulder 48×36, `L(x, top, width)` ledge, `C(x)` crumbling ledge 32 wide at the ground line, spikes 32 wide. `U(x, top)` cliff updraft (Plateaus Easy rule): its landing ledge is at `x + 40`, `top − 4`. `P(x0, x1)` horizontal moving platform (48 px, 40 px/s, top 0), drawn as a cable car. `Ice(x, w)` salt crust (the ice rule). `Llama(cx, period, phase)` is the herd rule: `new Bat(cx, 6, 48, 0, period, phase)`.

The skeletons reuse the bot-checked Plains Normal levels (N1 = Plains N5, N2 = Plains N2, N3 = Plains N3, N4 = Plains N4, N5 = Plains N3); objects and kinds change.

## N1 "Table Mountain" (theme TableTop, goal "Cable Station")

- Pits: 2000 (200), 3600 (200), 5900 (200), dry. Safe pool: 800 (160), water.
- Spikes: 1500, 3100, 5300.
- Boulders: S(450), B(1250), S(2700), B(4300), S(5000), S(6700).
- Ledges: L(1100, 40, 64), L(4000, 40, 64), L(4080, 84, 64), L(6400, 28, 64).
- Cable cars: P(800, 912) over the safe pool (learn), P(2000, 2152), P(3600, 3752), P(5900, 6052).
- Pickups: sandstone (466, 24), quartzite (1132, 40), granite (2076, 40), shale (2900, 0), quartzite (4112, 84) **secret**, sandstone (5100, 0), granite (6716, 24).

## N2 "Monument Valley" (theme MesaValley, goal "Trading Post")

- Pits: 1300 (96), 3600 (56), 5700 (96), dry.
- Spikes: 2900, 4900, 6600.
- Boulders: S(900), B(1700), S(2600), B(3200), S(4500), B(5300), S(6800).
- Ledges: L(1000, 40, 64), L(2490, 112, 64), L(3900, 40, 64), L(3980, 84, 64), L(4740, 96, 64), L(5400, 28, 64).
- Crumbling bridges: C(1300), C(1332), C(1364), C(5700), C(5732), C(5764).
- Soft sand: (500, 200), (2000, 300), (4100, 350), (6150, 300).
- Updrafts: U(2450, 116), U(4700, 100).
- Pickups: sandstone (916, 24), siltstone (1032, 40), conglomerate (2200, 0), hematite (2522, 112) on the first updraft ledge, sandstone (4012, 84) **secret**, siltstone (5432, 28), conglomerate (6816, 24).

## N3 "Grand Canyon" (theme CanyonRim, goal "Canyon Lookout")

- Pits: 1300 (48), 3200 (64), 5200 (56), water (the river).
- Spikes: 2800, 4500, 6500.
- Boulders: S(400), B(1000), S(2200), B(3500), B(5500), S(6700).
- Ledges: L(1600, 40, 64), L(3600, 40, 64), L(3680, 84, 64), L(6300, 28, 64).
- Currents: (600, 200, right), (1900, 300, left), (3900, 400, right), (5800, 300, left).
- Pickups: limestone (416, 24), schist (700, 0), granite (1632, 40), sandstone (2650, 0), limestone (3712, 84) **secret**, schist (5000, 0), granite (6716, 24).

## N4 "Mount Roraima" (theme TepuiTop, goal "Explorer Camp")

- Pits: 1800 (104), 3800 (56), 6200 (64), water (summit pools).
- Spikes: 1200, 4300, 6600.
- Boulders: S(500), B(2600), S(3500), B(4500), S(5600), B(6800).
- Ledges: L(900, 40, 64), L(3040, 112, 64), L(4000, 40, 64), L(4080, 84, 64), L(5040, 144, 56), L(5800, 28, 64).
- Floats: 1828, 6208.
- Updrafts: U(3000, 116), U(5000, 148).
- Pickups: quartzite (516, 24), quartz (932, 40), gneiss (1852, 40), sandstone (3072, 112), quartzite (4112, 84), quartz (5068, 144) **secret** on the high updraft ledge, gneiss (6000, 0).

## N5 "Salar de Uyuni" (theme SaltFlat, goal "Salt Hotel")

- Pits: 1300 (48), 3200 (64), 5200 (56), dry.
- Spikes: 2800, 4500, 6500.
- Boulders: S(400), B(1000), S(2200), B(3500), B(5500), S(6700) (drawn as salt blocks).
- Ledges: L(1600, 40, 64), L(3600, 40, 64), L(3680, 84, 64), L(6300, 28, 64).
- Salt crust: Ice(600, 300), Ice(3900, 400), Ice(5900, 400).
- Llama herds: Llama(1900, 4.6, 0), Llama(3000, 4.8, 1.0), Llama(5000, 4.4, 2.0).
- Pickups: halite (416, 24), gypsum (700, 0), ignimbrite (1632, 40), andesite (2650, 0), halite (3712, 84) **secret**, ulexite (4100, 0), ulexite (6716, 24).

## Bot check (2026-11-24)

The scratch bot is the Plains Hard bot plus the Plateaus Easy updraft (`bot-check-*.cs.txt`; working copy `bot/`), with the Normal lint plus the updraft fairness rules (60 px from pits and spikes, 40 px from boulders and checkpoints, the head at or below 196, a landing ledge at `x + 40`, `top − 4`, no updraft on sand, ice or a current). Copy the `bot-check-*` files (renamed to `.cs` / `.csproj`) into an empty folder and run `dotnet run -c Release`.

```
N1 83.8  N2 93.0  N3 85.8  N4 85.2  N5 98.4  
-- Plateaus Normal:
N1 Table Mountain: bot GOAL 98.4s (goal); pickups 7/7 reachable lint clean
   start-time shifts 0.25..3.25 s (every timed object out of phase): 13/13 reach the goal, 95.2-98.2 s
   without moving platforms: FAILED (pit at x=2085)
   N2 Monument Valley without updrafts: needs an updraft -> pickups [hematite(2522,112)]
N2 Monument Valley: bot GOAL 93.0s (goal); pickups 7/7 reachable lint clean
   start-time shifts 0.25..3.25 s (every timed object out of phase): 13/13 reach the goal, 93.0-93.0 s
   crumbling ledges: 6
N3 Grand Canyon: bot GOAL 85.8s (goal); pickups 7/7 reachable lint clean
   start-time shifts 0.25..3.25 s (every timed object out of phase): 13/13 reach the goal, 85.8-85.8 s
   currents (s in strip): 600R:1.75, 1900L:5.37, 3900R:3.52, 5800L:5.57
   N4 Mount Roraima without updrafts: needs an updraft -> pickups [sandstone(3072,112) quartz(5068,144)]
N4 Mount Roraima: bot GOAL 83.8s (goal); pickups 7/7 reachable lint clean
   start-time shifts 0.25..3.25 s (every timed object out of phase): 13/13 reach the goal, 83.8-83.8 s
   floats (s standing): 1828:0.28, 6208:0.00
N5 Salar de Uyuni: bot GOAL 85.8s (goal); pickups 7/7 reachable lint clean
   start-time shifts 0.25..3.25 s (every timed object out of phase): 13/13 reach the goal, 85.3-86.4 s
   bats (s in loop span / pushes): 1900:2.05/1, 3000:2.12/3, 5000:2.05/1
```

Every level reaches the goal with all 7 pickups reachable and a clean lint, also when every timed object starts out of phase. Without the cable cars N1 fails (the pits are 200 px). Without the updrafts, the N2 hematite ledge and the N4 sandstone and secret quartz ledges cannot be reached.
