# Plains Easy 1–3: layouts and bot check

Date: 2026-11-21. Status: scratch design, checked by the physics bot. Not built. Not adviser-approved.

**Bot files.** Copy these into an empty folder as `Sim.cs`, `Program.cs`, `PlainsLevels.cs`, `EasyLevels.cs` and `bot.csproj`, then run `dotnet run -c Release`:
- `bot-check-Sim.cs.txt`
- `bot-check-Program.cs.txt`
- `bot-check-PlainsLevels.cs.txt`
- `bot-check-EasyLevels.cs.txt` (the Hills Easy regression)
- `bot-check-bot.csproj.txt`

The full output is in `bot-output.txt`, and the working copy is `bot/`.

The sim is the Hills Hard sim with walk 84 px/s (the game's real speed), plus the straw bounce pile. Its driver is the Easy bot: it walks right, jumps at walls, pits and spikes, and double-jumps over gaps.

**Shared Easy rules** (Hills Easy, `LevelData.cs` `Difficulty.Easy`):
- start (40, 0), 120 s timer, 1 checkpoint
- 5 pickups, with exactly one secret
- pits and spikes send the explorer a short way back

## Rule constants (the sim constants are the proposed game rules)

| Object | Constant | Value | Meaning |
|---|---|---|---|
| Explorer | `walk` | 84 px/s | Game walking speed (all times below use it) |
| | jump / double jump | 290 / 230 px/s | About 47 px and about 29 px high, with gravity 900 |
| Straw bounce pile (new, `StrawPile` X) | `Width`, `Height` | 48 × 16 px | Solid box x+1 … x+47 across, 0 … 15 up (like a small boulder's box rule) |
| | `LaunchSpeed` | 400 px/s | Landing on the top calls `ExplorerMotor.Launch(400)`: about 89 px of rise, feet to about 104 px. The double jump is ready again, so bounce + double jump reaches about 128 px |
| | `SquashSeconds` | 0.15 s | The squash picture after a bounce |
| | side | — | Walking into the side stops the explorer like a wall; no bounce |

**Why 48 px wide:** a 32 px pile was tried first. A walking jump made from right next to it flew over the top before landing (the jump stays above 15 px for about 46 px of forward travel), so the bot never bounced. At 48 px, the same jump lands on it.

**Lint rules checked by the bot (Easy):**
- No pit or spikes within 100 px of the checkpoint, and no boulder on it.
- No pit or spikes before x 200 or after goal − 120.
- For straw piles:
  - none within 60 px of a pit or spikes
  - none within 40 px of a boulder or the checkpoint
  - none before x 200 or after goal − 120
  - the first pile has no pit or spikes within 80 px
- Exactly 5 pickups, with one secret, and 1 checkpoint.
- Every ledge must be reachable. This is a closure: each new ledge is reached from the ground or from ledges already reached.
- The bot also re-searches with every pile removed, to show which ledges and pickups need a pile.

**Coordinates and shorthand:**
- Coordinates are pixels: x from the level start, y up from the ground line.
- S = small boulder 32×24, B = big boulder 48×36.
- `L(x, top, width)` = a ledge.
- Spikes are 32 wide.
- Pickups give their bottom-middle spot.
- Straw piles give their left edge.

## Plains Easy 1: Central Luzon Plain (RicePlain)

- Width 4800; goal 4740; checkpoint 2400
- Pits: (1500, 48), (3300, 48)
- Spikes: 2900
- Boulders: S 400, B 1000, S 1900, B 2700, S 3600, B 4200
- Ledges: L(600, 32, 80), L(1200, 28, 64), L(2000, 40, 64), L(3000, 28, 80), L(3800, 28, 48), L(3860, 56, 64), L(3940, 84, 64)
- Pickups: clay (416, 24); sandstone (640, 32); shale (2032, 40); **siltstone (3972, 84) secret**; clay (4600, 16)

## Plains Easy 2: Pangasinan Salt Beds (SaltBeds)

- Width 5400; goal 5240; checkpoint 2700
- Pits: (800, 48), (3500, 56)
- Spikes: 1700, 4000
- Boulders: S 400, B 1400, S 2300, S 4400, B 4800
- Ledges: L(1100, 28, 80), L(2000, 40, 64), L(3000, 40, 64), L(3080, 84, 72), L(3250, 28, 64), L(4600, 28, 64)
- Pickups: halite (416, 24); gypsum (1132, 28); quartz (2032, 40); **siltstone (3112, 84) secret**; halite (4416, 24)

## Plains Easy 3: Isabela River Plain (RiverPlain)

- Width 5400; goal 5240; checkpoint 2500
- Pits: (900, 48), (2100, 56), (3800, 56)
- Spikes: 1500, 3000, 3100, 4300
- Boulders: S 350, B 1700, S 2800, B 4050, S 4700
- Ledges: L(1240, 96, 64), L(1900, 40, 64), L(2300, 28, 80), L(3360, 120, 56)
- **Straw piles: 600, 1200, 3300, 4500**
- Pickups: conglomerate (366, 24); mudstone (1272, 96); halite (3066, 0); **limestone (3388, 120) secret**; conglomerate (4716, 24)

## Bot output (2026-11-21)

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

The bot steps in 1/60 s frames, so its measured bounce peak (100.6 px) is a little under the exact 103.9 px.

## Plains map life

All positions are in map-picture pixels: x to the right, y down from the top (the 480×270 map drawn full screen). Convert them the same way `HillsLife()` does.

**The Plains region** (`make_landform_map_art.py` `region()`):
- It is the bottom-left quadrant, about x 0–240 and y 125–269. Its borders are wavy: x ≈ 228–252 and y ≈ 125–145.
- The label plaque covers about x 16–104, y 229–267.
- The trail enters at (100, 196) and runs right toward (240, 150–206).
- Keep every sprite inside `region()` and off the label and the trail.

**Life sprites:**
- **Rice clumps** (`Plains/rice_0/1`, bottom of the sprite on the spot) at (30, 170), (70, 182), (140, 168), (190, 160), (200, 218) and (150, 228). Their frame rates are 1.2, 1.5, 1.7, 1.3, 1.6 and 1.4 frames per second.
- **Carabao** (`Plains/carabao_0/1`, bottom on the spot) at (58, 204), swapping frames every 2 s.
- **Egrets** (`Plains/egret_0/1`, 3 frames per second): two egrets on a slow `CircleFlight` around (120, 152), radius (20, 8). Their laps are 14 s and 17 s.
- Everything is active only while Plains is open. Check in the Editor that nothing overlaps the label or the trail; move a sprite by up to 10 px if it does, and report the move.
