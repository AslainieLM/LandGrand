# Hills Hard 1–7: layouts and bot check

Date: 2026-11-18. Status: scratch design, checked by the physics bot. Not built. Not adviser-approved. Pickup kinds are placeholders `k1`..`k9` per level (kinds are chosen later).

Bot files (copy into an empty folder as `Sim.cs`, `Program.cs`, `HardLevels.cs`, `MountainsHardLevels.cs`, `NormalLevels.cs`, `EasyLevels.cs` and `bot.csproj`, then run `dotnet run -c Release`):
`bot-check-Sim.cs.txt`, `bot-check-Program.cs.txt`, `bot-check-HardLevels.cs.txt` (Hills Hard), `bot-check-MountainsHardLevels.cs.txt` (Mountains Hard, the `LevelData.cs` numbers, for the band), `bot-check-NormalLevels.cs.txt` and `bot-check-EasyLevels.cs.txt` (regression), `bot-check-bot.csproj.txt`. Full output: `bot-output.txt`. The working copy is `bot/`. Arguments: `quick` skips the self-checks, Mountains Hard and regression; `H3` runs one level; `mh` stops after Mountains Hard.

The sim is the Hills Normal bot (Mountains Hard sim + sand, float, bat, current, balloon) with walk 84 px/s (the game's real speed), plus slippery mud and the rolling log with a direction.

Shared Hard rules (Mountains Hard, `LevelData.cs` `Difficulty.Hard`): width 9600, start (40, 0), goal (9440, 0), checkpoints 2400, 4800, 7200, 240 s timer, 9 pickups with one secret on a high ledge (y ≥ 72). Pits and spikes send the explorer back to the furthest flag reached.

## Rule constants (the sim constants are the proposed game rules)

| Object | Constant | Value | Meaning |
|---|---|---|---|
| Explorer | `walk` | 84 px/s | Game walking speed (all times below use it) |
| Slippery mud (new, `MudZone` X, Width) | rule | `IceSlide.Next` | Exactly the icy-ground rule, applied while standing on mud (grounded at y 0, middle inside the zone); only the drawing differs |
| | `IceSlide.SpeedUp` | 160 px/s² | Speeding up toward the held direction (0 to 84 in 0.53 s) |
| | `IceSlide.SlowDown` | 100 px/s² | Slowing, stopping or turning (84 to 0 in 0.84 s, a slide of about 35 px) |
| | walls, air | — | Hitting a wall sets walking speed 0; in the air, on ledges, platforms, floats and baskets walking is instant |
| Rolling log (new, `RollingLog` Spawn, End, Direction, Offset) | `CycleSeconds` | 3.5 s | Same as `RollingBoulder`, shifted by `Offset` |
| | `WarningSeconds` | 0.8 s | Dust warning at `Spawn` before each roll |
| | `Speed` | 90 px/s | Rolls along y 0 in `Direction` (+1 right, −1 left) |
| | `Size` | 14 × 14 px | Hit box centred on the middle x, y 0–14; a touch is a trap |
| | `SkipDistance` | 48 px | If the explorer's middle is within 48 px of `Spawn` when the roll would start, that cycle is skipped |
| | `MaxRun` | 243 px | \|End − Spawn\| ≤ 90 × 2.7; End is right of Spawn for +1, left for −1; gone when the middle passes End |
| Reused unchanged | crumbling ledge | 0.5 s break, 3 s back | `CrumbleLedge` |
| | wind gust | 4 s cycle, warn at 2 s, push 40 px/s left from 2.6 s | `WindZone` |
| | moving platform | 48 px wide, 40 px/s | `MovingPlatform` (horizontal, over the pit it starts at) |
| | vertical balloon | 48 px, 40 px/s, rise 48–96 | `MovingPlatform.Vertical` |
| | sinking float | 48 px, wait 0.6 s, sink 8 px/s, sunk at 16, rise after 1.5 s at 16 px/s | `SinkingFloat` |
| | bat | 16 × 12 box, push 24 px in 0.2 s, rest 1 s | `Bat` (all bats here: Cy 16, Rx 24, Ry 12) |
| | river current | 30 px/s on flat ground in the strip | `CurrentZone` |

A right-rolling log moves at 90 px/s, only 6 px/s faster than walking, so a player who keeps walking is never caught (its run ends first). It catches a player who stands still, turns back or is slowed (mud start, waiting for a gust, a platform or a basket). A hop straight up clears it (self-check). Right logs are placed where players stop: H6 log 3560→3790 rolls into the waiting spot at the platform pit 3800.

Lint rules checked by the bot (Hard): every log run is at most 243 px, End on the right side for its direction, over plain ground (no pit, pool, spikes, boulder, vent, balloon, current between the run ±8; no bat loop within 40 px); no pit, pool, spikes, crumbling ledge, moving platform, bat loop or log run within 100 px of a checkpoint; no current, balloon, ice, mud or wind within 40 px of a checkpoint; no boulder on a checkpoint; no hazard (pit, spikes, bat loop, crumble, mud, wind, log run) before x 200 or after goal − 120; no pit or spikes inside mud; mud does not overlap a current or sand; no pit or spikes within 80 px of a bat loop or current; float water gaps ≤ 40 px; balloon rise 48–96, not over a pit; a horizontal platform spans its pit; the first mud of H1 has no pit, spikes or log within 120 px; the first log of H2 rolls toward the player with no pit or spikes within 120 px; exactly 9 pickups, one secret at y ≥ 72; no pickup inside a bat loop; no ground pickup on a log run; 3 checkpoints. Every ledge must be reachable from the ground (closure from ledges already reached). Robustness: the bot is also started with every timed object shifted by 0.25–3.25 s (13 runs per level).

Coordinates: pixels, x from the level start, y up from the ground line. S = small boulder 32×24, B = big 48×36. `L(x, top, width)` = ledge. Spikes are 32 wide. A bridge at x = crumbling ledges at x, x + 32, x + 64 with top 0 over a 96 px pit. Pickups give the bottom-middle spot. Logs are (spawn, end, direction, offset). Bats are (centre x, period s, phase s), all with centre y 16 and radius 24 × 12. Currents are (x, width, direction). Balloons are (x, low top, high top, phase). Winds are (x, width, offset). Moving platforms are (x0, x1, top).

All levels: width 9600, start (40, 0), goal (9440, 0), checkpoints 2400, 4800, 7200.

## H1 "Tarsier Forest" (theme TarsierForest, goal Tarsier Lookout)

- Objects: slippery mud only (introduced here).
- Pits: (1500, 56), (3000, 64), (4300, 64), (6500, 64), (8300, 64). Water pits / pools: none.
- Spikes: 1200, 2800, 3900, 5500, 6900, 8600.
- Boulders: S380, B1000, S1900, S2100, B3500, S4500, S5200, B5800, S6800, B7800, S9000.
- Ledges: L(2600, 40, 64), L(4000, 48, 64), L(6100, 40, 64), L(6180, 84, 48), L(8900, 48, 64).
- Mud (x, width): (500, 300) **practice** (nothing within 120 px), (1700, 500), (3200, 500), (5000, 400), (6000, 400), (7500, 600), (8800, 400).
- Logs, bats, currents, floats, balloons, platforms, wind, crumbles: none.
- Pickups: k1 (396, 24) on S380; k2 (650, 0) on the practice mud; k3 (1528, 48) over pit 1500; k4 (2632, 40) ledge; k5 (3032, 48) over pit 3000; k6 (4032, 48) ledge; **k7 (6204, 84) secret**: climb from L(6100, 40) to L(6180, 84) from the mud; k8 (7824, 36) on B7800; k9 (8932, 48) ledge.

## H2 "Antipolo Slopes" (theme RainySlope, goal Barangay Hall)

- Objects: rolling logs only (introduced here), 6 left and 3 right.
- Pits: (1500, 56), (2900, 64), (4000, 64), (5600, 64), (6700, 64), (8500, 64).
- Spikes: 1250, 2700, 3260, 4650, 5900, 7000, 8650.
- Boulders: S400, B1650, S2200, B3100, S4900, B5450, S6500, B7300, S8000, B9100.
- Ledges: L(2500, 40, 64), L(3700, 40, 64), L(3780, 84, 48).
- Logs (spawn, end, dir, offset): (1000, 800, −1, 0) **practice** (rolls toward the player; no pit or spikes within 120 px); (2000, 1800, −1, 1.0); (3600, 3380, −1, 0); (4300, 4520, +1, 0.5); (5300, 5080, −1, 2.0); (6100, 6330, +1, 1.0); (7800, 7580, −1, 1.5); (8100, 8320, +1, 2.5); (9000, 8780, −1, 0.5).
- Pickups: k1 (416, 24) on S400; k2 (1528, 48) over pit 1500; k3 (2532, 40) ledge; k4 (2932, 48) over pit 2900; **k5 (3804, 84) secret**: L(3700, 40) then L(3780, 84), above log 3600→3380; k6 (4916, 24) on S4900; k7 (6732, 48) over pit 6700; k8 (7324, 36) on B7300; k9 (8532, 48) over pit 8500.

## H3 "Masungi" (theme KarstRidge, goal Spider-Web Bridge)

- Objects: mud, bats, crumbling ledges.
- Pits: (1500, 56), (2800, 96) bridge, (4000, 64), (5600, 96) bridge, (6600, 64), (8000, 96) bridge, (8900, 56).
- Spikes: 1200, 3300, 4450, 6250, 7600, 8650.
- Boulders: S400, B1000, S2200, B3600, S4300, B5300, S6900, B7400, S8400.
- Ledges: L(3020, 40, 64), L(6980, 96, 48) (secret), L(9220, 48, 64).
- Crumbling ledges (x, top): bridges at 2800, 5600, 8000 (each 3 × top 0); steps (6830, 40), (6900, 64) up to the secret ledge. 11 in all.
- Mud: (500, 400), (1700, 400), (3450, 450), (5000, 450), (6700, 120), (7300, 250), (8150, 400).
- Bats (centre x, period, phase): (1800, 3.0, 0), (2150, 2.8, 1.0), (3800, 2.6, 0.4), (4600, 2.8, 0.6), (5950, 2.6, 1.2), (9150, 2.6, 0.3). Bats 1800, 2150 and 3800 fly over mud.
- Pickups: k1 (416, 24); k2 (1528, 48) over pit 1500; k3 (2832, 40) over bridge 2800; k4 (3052, 40) ledge; k5 (4032, 48) over pit 4000; k6 (5632, 40) over bridge 5600; **k7 (7004, 96) secret**: crumbling steps 40 → 64 → ledge 96; k8 (8032, 40) over bridge 8000; k9 (9252, 48) ledge (outside the 9150 bat loop).

## H4 "Pililla" (theme WindHills, goal Turbine Base)

- Objects: wind gusts, logs, vertical balloons.
- Pits: (850, 56), (2700, 64), (4000, 64), (5300, 64), (6000, 160) the wide gap, (8200, 64).
- Spikes: 1200, 3100, 5700, 6900, 9100.
- Boulders: S400, B1350, S2200, B3560, S4200, B5550, S6250, B7000, S8500.
- Ledges: L(3700, 40, 64), L(5992, 112, 176) the bridge over pit 6000 (reached only from balloon 5944), L(7552, 112, 64) the secret ledge.
- Wind (x, width, offset): (600, 400, 0) over pit 850; (2500, 400, 1.0) over pit 2700; (5000, 500, 0.5) over pit 5300; (7900, 500, 1.5) over pit 8200.
- Logs: (1700, 1500, −1, 0.5); (3300, 3530, +1, 1.0); (4500, 4280, −1, 2.0); (6400, 6620, +1, 1.5); (8900, 8680, −1, 0.5).
- Balloons (x, low, high, phase): (5944, 24, 120, 0) **required** (without balloons the bot falls at the 160 px gap); (7500, 24, 120, 1.0) to the secret.
- Pickups: k1 (416, 24); k2 (878, 48) over pit 850 in the wind; k3 (2050, 0); k4 (3732, 40) ledge; k5 (4032, 48) over pit 4000; k6 (6080, 112) on the balloon bridge; **k7 (7584, 112) secret**: ride balloon 7500; k8 (8232, 48) over pit 8200 in the wind; k9 (8516, 24) on S8500.

## H5 "Clew Bay" (theme DrumlinBay, goal Stone Pier)

- Objects: mud, current strips (all push left, against travel), logs.
- Pits: (1350, 56), (2000, 64), (3100, 64), (4550, 56), (6000, 64), (8800, 64).
- Spikes: 1200, 3250, 4400, 5900, 6800.
- Boulders: S400, S800, B1050, S2200, B3500, S4200, B5350 (in current 4950), S6400, B8200, S8350.
- Ledges: L(6900, 40, 64), L(6980, 84, 48).
- Mud: (600, 400), (3300, 500), (6200, 500), (8300, 400).
- Currents (x, width, dir): (1720, 200, −1), (2480, 520, −1), (4950, 560, −1), (7280, 560, −1), (8950, 350, −1). Walking speed in them is 54 px/s.
- Logs: (1700, 1480, −1, 0); (3900, 4120, +1, 1.0); (5800, 5580, −1, 2.0); (7900, 8130, +1, 0.5); (8700, 8480, −1, 1.5) on mud 8300.
- Pickups: k1 (416, 24); k2 (1378, 48) over pit 1350; k3 (2750, 0) in current 2480; k4 (3132, 48) over pit 3100; k5 (3524, 36) on B3500 (in mud); k6 (5200, 0) in current 4950; k7 (6032, 48) over pit 6000; **k8 (7004, 84) secret**: L(6900, 40) then L(6980, 84); k9 (8832, 48) over pit 8800.

## H6 "Rome" (theme CityHills, goal Temple Ruins)

- Objects: logs, horizontal moving platforms, sinking floats.
- Pits: (1200, 200) platform, (3200, 64), (3800, 200) platform, (6200, 200) platform, (8000, 220) platform.
- Water pits (floats): (2000, 112), (5000, 192), (7400, 112).
- Spikes: 1700, 3500, 5500, 7800, 9000.
- Boulders: S400, B1500, S2250, B3000, S4600, B5300, S6000, B7000, S8300, B9150.
- Ledges: L(4100, 40, 64), L(4180, 84, 48).
- Moving platforms (x0, x1, top): (1200, 1352, 0), (3800, 3952, 0), (6200, 6352, 0), (8000, 8172, 0). Without them the bot falls at pit 1200.
- Floats (left edge, top 0): 2032 (gaps 32/32), 5032 and 5112 (gaps 32/32/32), 7432 (gaps 32/32).
- Logs: (900, 700, −1, 0); (2700, 2920, +1, 1.0); (3560, 3790, +1, 0.7) rolls into the waiting spot at platform pit 3800; (4500, 4280, −1, 2.0); (5700, 5920, +1, 0.5); (6900, 6680, −1, 1.5); (8500, 8720, +1, 2.5).
- Pickups: k1 (416, 24); k2 (1300, 40) over platform pit 1200; k3 (2056, 32) over float 2032; k4 (3232, 48) over pit 3200; k5 (3900, 40) over platform pit 3800; **k6 (4204, 84) secret**: L(4100, 40) then L(4180, 84); k7 (5136, 32) over float 5112; k8 (6300, 40) over platform pit 6200; k9 (8100, 40) over platform pit 8000.

## H7 "Guilin" (theme KarstTowers, goal Raft Dock), the finale

- Objects: every Hills object: mud, logs, bats, currents, floats, balloon, crumbling ledges, wind.
- Pits: (1500, 96) bridge, (3600, 64) in wind, (3956, 160) the wide gap, (6100, 96) bridge, (8400, 64) in wind.
- Water pits (floats): (3000, 112), (7800, 192).
- Spikes: 1000, 5500, 9250.
- Boulders: S400, B1150, S2200, B4600, S5200, B7550 (in current 7300), S8700.
- Ledges: L(3948, 112, 176) the bridge over pit 3956 (from balloon 3900 only), L(6540, 96, 48) the secret ledge.
- Crumbling ledges: bridges at 1500 and 6100; steps (6400, 40), (6470, 64) up to the secret ledge. 8 in all.
- Mud: (500, 400), (4150, 400) with log 4300→4520 on it, (5000, 400), (8550, 350).
- Logs: (2000, 1780, −1, 0.5); (4300, 4520, +1, 1.0); (6900, 6680, −1, 2.0); (8950, 9170, +1, 1.5).
- Bats: (1300, 2.6, 0) inside wind 1100; (5800, 2.4, 0.8).
- Currents: (2480, 400, −1), (7300, 400, −1).
- Floats: 3032 (gaps 32/32); 7832 and 7912 (gaps 32/32/32).
- Balloon: (3900, 24, 120, 0) **required** (without it the bot falls at the 160 px gap).
- Wind: (1100, 500, 0) over bat 1300 and bridge 1500; (3300, 500, 1.0) over pit 3600; (8100, 500, 0.5) over pit 8400.
- Pickups: k1 (416, 24); k2 (700, 0) on mud; k3 (1532, 40) over bridge 1500; k4 (2750, 0) in current 2480; k5 (3056, 32) over float 3032; k6 (4040, 112) on the balloon bridge; **k7 (6564, 96) secret**: crumbling steps 40 → 64 → ledge 96; k8 (7856, 32) over float 7832; k9 (8432, 48) over pit 8400.

## Bot results (2026-11-18, walk 84)

Mountains Hard band, re-run with this sim at walk 84 on the `LevelData.cs` numbers (the spec's 131–156 s was measured at 72): H1 113.5, H2 118.6, H3 134.1, H4 113.4, H5 118.0, H6 131.0, H7 122.4 s. **Band: 113.4–134.1 s.**

| Level | Bot to goal | Shifted starts (13) | Pickups reachable | Lint | New objects (bot) |
|---|---|---|---|---|---|
| H1 Tarsier Forest | 112.6 s | 13/13, 112.6 | 9/9 | clean | mud 7 stretches 3100 px, 31.8 s on mud |
| H2 Antipolo Slopes | 112.6 s | 13/13, 112.6 | 9/9 | clean | logs 6 L / 3 R, jumped 8 oncoming |
| H3 Masungi | 115.5 s | 13/13, 115.5–117.2 | 9/9 | clean | mud 26.8 s; 6 bats, 7 pushes; 11 crumbles |
| H4 Pililla | 125.8 s | 13/13, 123.3–130.6 | 9/9 | clean | balloons 5944 4.1 s, 7500 5.0 s; 4 log jumps; gust waits |
| H5 Clew Bay | 126.4 s | 13/13, 126.4 | 9/9 | clean | currents 3.7/9.6/8.8/10.4/6.5 s; mud 16.8 s; 6 log jumps |
| H6 Rome | 130.5 s | 13/13, 129.3–137.6 | 9/9 | clean | 4 platforms; floats 0.02–0.30 s; 4 log jumps, 2 hops over right logs |
| H7 Guilin | 132.0 s | 13/13, 131.0–134.7 | 9/9 | clean | balloon 5.6 s; bats 1/2 pushes; currents 7.4/5.8 s; mud 16.7 s |

- All under 240 s with at least 102 s spare (worst shifted start 137.6 s).
- H1 and H2 are 0.8 s under the band floor. A 9400 px walk at 84 px/s takes 111.9 s, and mud and logs never make the bot stop, so only waiting objects (wind, platforms, balloons) can add time; H1 and H2 may not use them. Mountains Hard H1 and H4 (113.5, 113.4 s) sit at the same floor. No number changed for this.
- Self-checks: mud slide from full walk 34.6 px in 0.85 s (35.3 px, 0.84 s expected); 0 to full walk 0.53 s; a right log hits a player idle 100 px past its spawn 0.97 s after it starts; a straight-up hop at 22 px clears it; walking on at 84 px/s is never caught; standing 20 px from a right log's spawn skips that cycle.
- Regression with the extended sim: E1 56.8, E2 62.4, E3 70.1, N1 83.8, N2 85.9, N3 84.2, N4 86.2, N5 91.1 s (unchanged).
- Mountains Hard bot note: with the old bot logic at walk 84, Mountains Hard H5 failed at spikes 4400 inside wind 4200 (a gust began mid-jump). The bot now waits out a gust before spikes in a wind zone, as it already did before pits; then H5 passes (118.0 s). It is a bot limit, not a level fault; the other Mountains times did not change.

## Notes for the build

- `MudZone` can reuse `IceSlide` and the ice footing check as is; only the art (`mud_top`, drips) differs.
- `RollingLog` is `RollingBoulder` plus `Direction`: `x = Spawn + Direction × elapsed × Speed`, gone when it passes `End` in that direction. The data tests mirror the boulder tests for both directions.
- Right logs only threaten a player who stops, turns or is slowed. Most right logs here are low-threat for a player who keeps walking; H6 3560→3790 is the one placed at a natural stop.
- The bot rides every balloon it meets, including the optional secret balloon in H4, so its H4 time includes that detour.
- The Hills Hard lint flags two Mountains Hard layouts (H4, H5) for a hazard near the start or goal and H5 for a boulder run within 100 px of checkpoint 4800. These are stricter Hills rules, not faults in Mountains Hard.
