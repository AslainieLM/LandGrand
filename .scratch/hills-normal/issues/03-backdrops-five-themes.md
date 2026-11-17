# 03: Backgrounds for the five Hills Normal themes

**What to build** (spec section 8, `make_hills_normal_backdrops.py`):

1. Write `Tools/Art/make_hills_normal_backdrops.py` in the style of `Tools/Art/make_hills_easy_backdrops.py`:
   - A `THEMES` table.
   - Helpers from `make_normal_backdrops as nb` and `make_normal_art as na`.
   - D-033 comments on every line.
2. For each theme, write `Assets/Art/Level/Sprites/Backdrop/<Theme>/L0.png`–`L5.png`, each 960×270:
   - L0 is the sky (furthest); L5 is the nearest band.
   - Each layer tiles left to right without a seam.
   - **Themes:** IslandHills, LimestoneCave, RiverGorge, RiceTerraces and FairyChimneys, as described in spec section 8.
3. **LimestoneCave:**
   - The far layers show the light of the cave mouth.
   - The nearer layers show stalactites, columns and flowstone in dark rock.
   - Keep the values low, so the explorer and the `cave_top` ground stand out.
4. Write the preview `.scratch/hills-normal/backdrop-preview.png`: each theme stacked into one 480×270 frame, plus each layer alone. Look at it, and fix stripes, seams and muddy colours.

**Checklist**
- [x] 30 PNGs exist (5 themes × 6 layers), each 960×270, each tiling without a seam
- [x] The preview has been looked at; fixes listed
- [x] No existing backdrop was changed

**Blocked by:** none

**Status:** done

**Verified:** 30 RGBA layers at 960×270 with matching edge columns. Opened the stacked and per-layer preview. Unity EditMode passed 334 of 334 tests.

**Builder to run:** none (ticket 06)

## Files

- `Tools/Art/make_hills_normal_backdrops.py` draws the backdrops.
- `.scratch/hills-normal/backdrop-preview.png` shows each 480×270 composition and its six layers.
- Unity generated matching `.meta` files for the 30 sprites and five new folders.
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L0.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L1.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L2.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L3.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L4.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/FairyChimneys/L5.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L0.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L1.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L2.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L3.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L4.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/IslandHills/L5.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L0.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L1.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L2.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L3.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L4.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/LimestoneCave/L5.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L0.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L1.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L2.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L3.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L4.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/RiceTerraces/L5.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L0.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L1.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L2.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L3.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L4.png` (960 x 270)
- `Assets/Art/Level/Sprites/Backdrop/RiverGorge/L5.png` (960 x 270)

Preview fixes: softened the sky dither, broke up the water reflections on the rice terraces, and joined the waterfall to the far cliff. No tracked backdrop PNGs changed.
