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
- [ ] 30 PNGs exist (5 themes × 6 layers), each 960×270, each tiling without a seam
- [ ] The preview has been looked at; fixes listed
- [ ] No existing backdrop was changed

**Blocked by:** none

**Status:** not started

**Builder to run:** none (ticket 06)
