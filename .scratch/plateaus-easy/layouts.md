# Plateaus Easy 1–3: exact layouts (D-072)

Copy these numbers exactly into `LevelCatalog`. Pixels: x across from the level start, y up from the ground line. Every level starts at (40, 0). `S(x)` small boulder 32×24, `B(x)` big boulder 48×36, `L(x, top, width)` ledge, spikes 32 wide. `U(x, top)` is a cliff updraft: a 32 px column from x to x + 32; the feet rise to `top` and hover there.

**Updraft ledge rule:** each updraft's landing ledge starts 8 px right of the column (`x + 40`) and its top is 4 px below the updraft top. The gap keeps the explorer's head from hitting the ledge while it rises; the 4 px lets it drop onto the ledge when it walks out.

## Plateaus Easy 1: "Bukidnon Plateau" (theme PineapplePlateau)

- Width 4800. Goal at 4740 ("Farm Lookout"). Checkpoint at 2400.
- Pits: 1300 (48 wide), 3400 (48).
- Spikes: 2950.
- Boulders: S(450), B(1050), S(1800), B(2800), S(3700), B(4250).
- Ledges: L(650, 32, 80), L(1500, 28, 64), L(2050, 40, 64), L(3100, 28, 80), L(3900, 28, 48), L(3960, 56, 64), L(4040, 84, 64).
- Pickups:
  - basalt (466, 24), on the first boulder
  - andesite (690, 32)
  - hematite (2082, 40)
  - halloysite (4072, 84), **secret spot**: the top of the three-step climb
  - basalt (4600, 16), near the lookout

## Plateaus Easy 2: "Lake Lanao" (theme LakePlateau)

- Width 5400. Goal at 5240 ("Torogan"). Checkpoint at 2700.
- Pits: 900 (48), 3600 (56).
- Spikes: 1800, 4100.
- Boulders: S(450), B(1500), S(2250), S(4450), B(4850).
- Ledges: L(1150, 28, 80), L(1950, 40, 64), L(3050, 40, 64), L(3130, 84, 72), L(3300, 28, 64), L(4650, 28, 64).
- Pickups:
  - tuff (466, 24)
  - volcanic_breccia (1182, 28)
  - goethite (1982, 40)
  - halloysite (3162, 84), **secret spot**: climb from the ledge at 40 to the one at 84 (review kind)
  - tuff (4466, 24)

## Plateaus Easy 3: "Maria Cristina Falls" (theme FallsEdge)

- Width 5400. Goal at 5240 ("Falls Viewing Deck"). Checkpoint at 2500.
- Pits: 900 (48), 2100 (56), 3800 (56).
- Spikes: 1500, 3000, 3100 (a pair, with goethite between them), 4300.
- Boulders: S(350), B(1700), S(2800), B(4050), S(4700).
- Updrafts: U(600, 68) (learn on safe ground), U(1200, 116) (up to the gibbsite ledge), U(3300, 164) (up to the secret ledge), U(4500, 84) (a fun ride before the goal).
- Ledges: L(640, 64, 64), L(1240, 112, 64), L(1900, 40, 64), L(2300, 28, 80), L(3340, 160, 56), L(4540, 80, 64).
- Pickups:
  - laterite (366, 24), on the first boulder
  - gibbsite (1272, 112), on the ledge reached by the updraft at 1200
  - goethite (3066, 0), between the spike pair (review kind)
  - cristobalite (3368, 160), **secret spot**: only the updraft at 3300 reaches it
  - laterite (4716, 24)

## Bot check (2026-11-23)

The scratch bot is the Plains Easy bot plus the updraft (`bot-check-*.cs.txt`; working copy `bot/`). Reach searches also try riding a column: stand still until the explorer hovers at the top, then walk or jump. Lint adds the updraft fairness rules: no updraft within 60 px of a pit or spikes, 40 px of a boulder or the checkpoint, before x 200 or after goal − 120; the first updraft has no pit or spikes within 80 px; the head at the top stays below 196 px (the screen's safe top under the HUD); no ledge in or over a column. Copy `Sim.cs`, `Program.cs`, `PlateausLevels.cs`, `PlainsLevels.cs`, `EasyLevels.cs` and `bot.csproj` into an empty folder and run `dotnet run -c Release`.

```
check: updraft 600: hovering at feet 67.5 (top 68) after 0.75 s (expect about 0.76 s)
check: jump pressed while hovering: feet 68.0 (unchanged), double jump ready True
check: walking off the top: grounded True, feet 64.0 (ledge top 64), x 641
check: walking straight through: highest feet 38.3, then grounded True at x 718
-- Regression, Hills Easy at walk 84 (expect E1 56.8, E2 62.4, E3 70.1):
E1 56.8  E2 62.4  E3 70.1
-- Regression, Plains Easy at walk 84 (expect E1 56.8, E2 62.4, E3 62.2):
E1 56.8  E2 62.4  E3 62.2
-- Plateaus Easy:
E1 Bukidnon Plateau: bot GOAL 56.8s (goal); bounces 0; updrafts 0; pickups 5/5 reachable lint clean
E2 Lake Lanao: bot GOAL 62.4s (goal); bounces 0; updrafts 0; pickups 5/5 reachable lint clean
E3 Maria Cristina Falls: bot GOAL 62.5s (goal); bounces 0; updrafts 4; pickups 5/5 reachable lint clean
   without updrafts: needs an updraft -> pickups [gibbsite(1272,112) cristobalite(3368,160)], ledges [L(1240,112) L(3340,160) L(4540,80)]
```

The last line shows the updrafts are needed: without them the gibbsite ledge, the secret ledge and the fun ledge cannot be reached. Walking straight through a column lifts the explorer only about 38 px, so the main path never needs one; a learner must stand still in the wind to rise.

## Plateaus map life (`LandformMapBuilder`)

The Plateaus region is bottom right: label (424, 248), padlock (330, 150) in the map's canvas coordinates. Place the life inside the Plateaus area of the map picture the way `PlainsLife()` places the Plains life, and report the spots chosen (the map picture decides where water and grass can go):

- one waterfall (`waterfall_0/1`, 4 frames per second) on a plateau edge
- four grass tufts (`grass_0/1`, 1.2–1.8 frames per second, each a little different)
- two hawks (`hawk_0/1`, 3 frames per second) on a slow `CircleFlight` above the region, radius about 20 px
