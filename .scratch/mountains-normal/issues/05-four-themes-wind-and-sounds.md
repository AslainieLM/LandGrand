# 05: Four new themes, Normal wind and the new sounds

**What to build:**

1. **Themes** in `Assets/Editor/LevelThemes.cs` (one entry each, D-061):

| Theme | GroundTop / Soil / Ledge | BoulderEnding | Goal (name, sprite, offset x, y) | Props |
|---|---|---|---|---|
| PineForest | `pine_top` / `pine_soil` / `ledge_pine` | `_pine` | Ranger Hut, `hut`, (20, 24) | `Props/pine_tree` (24×48, order 3) about every 260 px, never over a pit or in front of a ledge, plus the existing `SummitSnow` flakes, very light, over the whole level (frost) |
| AshGorge | `ash_top` / `ash_soil` / `ledge_ash` | `_ash` | Stone Bridge, `bridge`, (36, 22) | none (the rockfalls give the motion) |
| MossyForest | `moss_top` / `moss_soil` / `ledge_moss` | `_moss` | Trail Sign, `trail_sign`, (16, 20) | `Props/fern` (order 7) about every 180 px on the ground, not over pits; `Props/sulfur_crust` (order 7) at each steam vent, either side |
| SnowPeak | `snow_top` / `snow_soil` / `ledge_snow` | `_snow` | Summit Flags, `summit_flags`, (28, 28) | `SummitSnow` over the whole level, not only near the end |

   Check that each goal picture sits on the ground line, with its walk-in point at the goal. Adjust the offset if it looks wrong, and report the numbers you used.
2. **Ambience by difficulty.** `PixelSceneKit.LevelMusic` takes the difficulty:
   - Easy keeps `amb_breeze_easy.ogg`.
   - Normal uses `amb_wind_normal.ogg`.
   - Hard keeps the Easy file for now; note it as to do.
3. **Sounds.** Wire `sfx_crumble.wav`, `sfx_rockfall.wav` and `sfx_gust.wav` in the builder, as `sfx_steam.wav` is. If one is missing, log a clear error naming `python Tools/Audio/make_normal_audio.py`.

**Blocked by:** 03, 04

**Status:** to do

- [ ] The four themes build; the Easy themes are unchanged
- [ ] A missing theme entry still gives the clear error from `ArtFor`
- [ ] Normal scenes play the stronger wind; Easy scenes keep the breeze
- [ ] Basic-English comments

**Builder to run:** **LandGrand > Build Mountains Levels**.

Spec: `../spec.md` ("Assets").
