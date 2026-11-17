# Hills Normal 1–5: layouts and bot check

Date: 2026-11-17. Status: scratch design, checked by the physics bot. Not built. Not adviser-approved.

Bot files (copy into an empty folder as `Sim.cs`, `NormalLevels.cs`, `EasyLevels.cs` and `bot.csproj`, then run `dotnet run -c Release`):
`bot-check-Sim.cs.txt`, `bot-check-NormalLevels.cs.txt`, `bot-check-EasyLevels.cs.txt` (needed to compile; also a regression check), `bot-check-bot.csproj.txt`. Full output: `bot-output.txt`.

Shared Normal rules (from Mountains Normal, `MountainsNormalTests.cs`): width 7200, start x 40, goal x 7040, checkpoints 2400 and 4800, 180 s timer, 7 pickups. Hills adds one secret pickup on a high ledge.

## Rule constants (the sim constants are the proposed game rules)

| Object | Constant | Value | Meaning |
|---|---|---|---|
| Sinking float | `FloatWidth` | 48 px | Platform width; top at the water line (y 0) when up |
| | `FloatWait` | 0.6 s | Standing time before it sinks; restarts on every landing |
| | `FloatSinkSpeed` | 8 px/s | Sinking speed while the explorer stands on it |
| | `FloatSunkDepth` | 16 px | After 2 s of sinking the top is 16 px down; it then holds nothing |
| | `FloatRiseDelay` | 1.5 s | Time after the explorer leaves before it rises |
| | `FloatRiseSpeed` | 16 px/s | Rising speed (proposed; back up 1 s after it starts rising) |
| | `PoolDepth` | 16 px | Shallow safe water under the practice float (floor at -16, not a hazard) |
| Gentle bat | ellipse | per bat | x = Cx + Rx·cos(2π(t+Phase)/Period), y = Cy + Ry·sin(...) |
| | `BatW`, `BatH` | 16 × 12 px | Hit box around the bat's middle |
| | `BatPushDistance` | 24 px | One touch pushes the explorer this far, away from the bat (straight under it: against the walk) |
| | `BatPushSeconds` | 0.2 s | Push is spread over this time (120 px/s); walk input is ignored meanwhile |
| | `BatRest` | 1 s | Time after a push when that bat cannot push again |
| River current | `CurrentSpeed` | 30 px/s | Push while grounded on the flat ground line in the strip (same footing test as sand, `ZoneFooting`) |
| Vertical balloon | `BalloonWidth` | 48 px | Basket top width; only the top is solid |
| | `BalloonSpeed` | 40 px/s | Up and down speed |
| | `BalloonMinRise`, `BalloonMaxRise` | 48, 96 px | Allowed High − Low |
| | phase | s | At phase 0 the balloon is at Low going up |

Floats and balloons are one-way tops, like the game's dynamic slabs (`LevelGeometry.LedgeShapes`): they hold from above only and never block the sides or the head. Standing on one carries the explorer up or down. A fall into a water pit is an ordinary pit hazard (short return).

Lint rules checked by the bot: no pit, pool, spikes or bat loop within 100 px of a checkpoint; no current or balloon within 40 px of a checkpoint; no hazard (pit, spikes, bat loop) before x 200 or within 120 px of the goal; no pit or spikes within 80 px of any bat loop or current strip; every water gap next to a float is at most 40 px; balloon rise 48–96 and no balloon over a pit; the first object of a type in its introducing level is on safe ground (80 px clear, not over a pit; the first current pushes with the player); exactly 7 pickups, one secret; no pickup inside a bat loop. All ledges must be reachable from the ground (the reach check starts only from ledges already reached).

Coordinates: pixels, x from the level start, y up from the ground line. `B(x, Small)` = 32×24 boulder, `B(x, Big)` = 48×36. `L(x, top, width)` = ledge. Pickups give the bottom-middle spot. Spikes are 32 wide.

## N1 "Hundred Islands" (theme IslandHills)

- Width 7200. Goal at 7040 (Boat Dock). Checkpoints 2400, 4800.
- Pits (water): 1800 (104), 3500 (48), 6100 (192). Spikes: 2900, 5300.
- Shallow pool (safe, practice): 800 (96).
- Floats (x, top 0): 824 over the pool (practice), 1828 over pit 1800 (gaps 28), 6132 and 6212 over pit 6100 (gaps 32, 32, 32).
- Boulders: B(420, Small), B(1500, Big), B(2700, Small), B(3900, Big), B(4500, Small), B(5900, Big), B(6700, Small).
- Ledges: L(1200, 40, 64), L(4200, 40, 64), L(4280, 84, 64).
- Pickups:
  - aragonite (436, 24), on the first boulder
  - coquina (848, 40), over the practice float
  - coral stone (1232, 40), on a ledge
  - aragonite (2600, 0), on the ground
  - coral stone (3524, 48), over the small water pit
  - coquina (4312, 84), **secret spot**: climb from the ledge at 40 to the one at 84
  - aragonite (6716, 24), on the last boulder

## N2 "Callao Cave" (theme LimestoneCave)

- Width 7200. Goal at 7040 (Cave Mouth). Checkpoints 2400, 4800.
- Pits: 900 (48), 3300 (64), 5600 (56). Spikes: 1700, 6300.
- Bats (center x, y, radius x×y, period, phase): (1350, 16, 24×12, 3.0 s, 0) practice; (3800, 16, 24×12, 2.6 s, 0.5); (6000, 16, 24×12, 3.2 s, 1.2).
- Boulders: B(500, Small), B(2000, Big), B(2700, Small), B(4200, Big), B(4500, Small), B(5200, Big), B(6700, Small).
- Ledges: L(1088, 40, 64), L(5000, 40, 64), L(5080, 84, 64).
- Pickups:
  - flowstone (516, 24)
  - dripstone (1120, 40), on a ledge
  - calcite (2600, 0)
  - flowstone (3332, 48), over pit 3300
  - dripstone (5112, 84), **secret spot**: the high ledge
  - calcite (6550, 0)
  - flowstone (6716, 24)

## N3 "Pagsanjan Gorge" (theme RiverGorge)

- Width 7200. Goal at 7040 (Falls Landing). Checkpoints 2400, 4800.
- Pits: 1700 (56), 3700 (64), 5800 (64). Spikes: 2000, 4250, 6400.
- Currents (x, width, direction): (1000, 240, right) practice; (3000, 320, **left**, against travel: 42 px/s forward); (5300, 300, right, ends 200 px before pit 5800).
- Boulders: B(500, Small), B(1450, Big), B(2650, Small), B(3500, Big), B(4550, Small), B(6050, Big), B(6750, Small).
- Ledges: L(1900, 40, 64), L(4000, 40, 64), L(4080, 84, 64).
- Pickups:
  - volcanic breccia (516, 24)
  - tuff (1120, 0), in the practice current
  - andesite (1932, 40), on a ledge
  - volcanic breccia (3160, 0), in the left-pushing current
  - tuff (4112, 84), **secret spot**: the high ledge
  - andesite (5832, 48), over pit 5800
  - volcanic breccia (6766, 24)

## N4 "Banaue Rice Terraces" (theme RiceTerraces)

- Width 7200. Goal at 7040 (Ifugao House). Checkpoints 2400, 4800.
- Pits: 1200 (56), 3300 (112, the terrace pond), 5900 (64). Spikes: 1800, 4300, 6500.
- Float: 3332 over the pond pit 3300 (gaps 32). Not over a safe pool: the float is not new here (met in N1).
- Bat: (2900, 16, 24×12, 3.0 s, 0).
- Current: (5200, 280, left), the irrigation channel.
- Boulders: B(500, Small), B(1500, Big), B(2650, Small), B(3800, Big), B(4550, Small), B(6200, Big), B(6750, Small).
- Ledges (terrace steps): L(700, 32, 96), L(2000, 40, 64), L(4000, 40, 64), L(4080, 84, 64).
- Pickups (review kinds only):
  - clay (516, 24)
  - kaolinite (748, 32), on the first terrace step
  - clay (2032, 40)
  - kaolinite (3356, 32), over the pond float
  - clay (4112, 84), **secret spot**: the high terrace
  - kaolinite (5360, 0), in the channel
  - clay (6766, 24)

## N5 "Cappadocia" (theme FairyChimneys)

- Width 7200. Goal at 7040 (Balloon Field). Checkpoints 2400, 4800.
- Pits: 1500 (64), 3400 (64), 4200 (160, the wide gap), 6100 (64). Spikes: 1900, 3700, 6500.
- Balloons (x, low top, high top, phase): (900, 24, 80, 0) practice on safe ground; (2900, 24, 96, 0.5); (4144, 24, 120, 0) **required**: the only way onto the high ledge over the 160 px gap; (5500, 24, 104, 1.0) to the secret ledge.
- Boulders: B(500, Small), B(1200, Big), B(2600, Small), B(3100, Big), B(5000, Small), B(5900, Big), B(6750, Small).
- Ledges: L(4192, 112, 176) the cliff bridge over pit 4200 (reached only from balloon 4144; feet top out near 76 px with a double jump, and 160 px is beyond the ~83 px double-jump range), L(5552, 112, 64) the secret ledge.
- Pickups:
  - ignimbrite (516, 24)
  - zeolite (924, 104), over the practice balloon
  - tuff (1532, 48), over pit 1500
  - ignimbrite (2924, 124), over balloon 2900 at its top
  - zeolite (4280, 112), on the cliff bridge
  - tuff (5584, 112), **secret spot**: the high ledge from balloon 5500
  - ignimbrite (6766, 24)

## Bot results (2026-11-17)

| Level | Bot to goal | Pickups reachable | Lint | New objects (bot) |
|---|---|---|---|---|
| N1 Hundred Islands | 97.6 s | 7/7 | clean | floats, s standing: 824 0.38, 1828 0.08, 6132 0.15, 6212 0.15 |
| N2 Callao Cave | 99.9 s | 7/7 | clean | bats, s in loop span / pushes: 1350 1.68/1, 3800 1.85/2, 6000 1.68/1 |
| N3 Pagsanjan Gorge | 98.5 s | 7/7 | clean | currents, s in strip: 1000R 2.37, 3000L 7.62, 5300R 2.93 |
| N4 Banaue Rice Terraces | 101.0 s | 7/7 | clean | float 3332 0.15; bat 2900 1.68/1; current 5200L 6.67 |
| N5 Cappadocia | 106.1 s | 7/7 | clean | balloons, s arriving to jumping off: 900 3.87, 2900 1.98, 4144 3.70, 5500 2.13 |

- Target band: Mountains Normal bot times 97.6–111.6 s. All levels are well under 180 s.
- N5 without balloons: the bot fails at the 160 px gap (pit at x 4272), so the balloon is required.
- Every ledge is reachable from the ground (ledge closure check), so the secret ledges are real routes.
- Hills Easy regression with the extended sim: E1 66.0 s, E2 72.7 s, E3 81.1 s, 5/5 pickups each (unchanged).
- Mechanics self-checks: standing still on a float over a pit drops the explorer into the pit at 2.85 s (0.6 s wait + 2 s sink + short fall); on the practice float it ends safely on the pool floor at -16; walking against the left current moves 43 px in 1 s (42 expected, frame rounding); standing in it drifts 29 px left; one bat touch pushes 26 px (24 plus frame rounding) and the explorer is then out of the loop.

## Notes for the build

- Added (small change): the practice float sits over a **shallow safe pool** (`Pool`, floor at -16, not a hazard), one of the two practice options in the brief. It is not one of the 3 pits. The other float pits count as the level's pits.
- `FloatRiseSpeed` 16 px/s is my proposal; the brief only fixes the 1.5 s delay.
- A float that has started to sink sits below the bank, and ground solids block the sides, so the explorer must hop off onto the bank (the motor has no step-up). The bot hops. Every float gap needs a jump anyway.
- The bot crosses floats fast (under 0.4 s), so it never sees them sink; the 2.6 s hold time is the slow-player margin.
- In N4 the float, bat and current are not first-of-type, so they are not on practice spots (the float is over the pond pit).
- Kinds: volcanic_breccia, aragonite, coquina, flowstone, dripstone, ignimbrite, zeolite are new ids; see `kinds-research.md` for source caveats (for example, ignimbrite at Pagsanjan is unproven).
