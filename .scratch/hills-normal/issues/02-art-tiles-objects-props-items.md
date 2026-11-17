# 02: Art — tiles, objects, goals, props, items and lesson pictures

**What to build** (spec section 8, `make_hills_normal_art.py`):

1. Write `Tools/Art/make_hills_normal_art.py` in the style of `Tools/Art/make_hills_easy_art.py`:
   - The same imports and helpers (`mm`, `lm`, `la`, `ls`, `na`).
   - Named palette mixes at the top.
   - D-033 comments on every line.
2. Draw every sprite in the spec section 8 tables at its **exact** size and path:
   - The tiles and boulders for `_island`, `_cave`, `_gorge`, `_terrace` and `_chimney`.
   - `water_fill`, `float_raft`, `float_ripple_00/01`, `current_top`, `current_mark_00/01`, `bat_00/01/02`, `balloon` and `balloon_flame_00/01`.
   - The five goals and every `Props/` sprite.
   - The seven items and the five lesson pictures.
3. **Look:**
   - Match Hills Easy: chunky outlines, 2–3 shades per colour, light from the top left.
   - Each theme's ground must read clearly against its background. The cave floor must stay readable in the dark cave.
   - **Bats:** friendly, not scary.
   - **Balloon:** only the bottom 10 px basket top looks standable.
   - **Lesson pictures:** recognisable from the real place. Hundred Islands: islets with undercut, mushroom-shaped bases. Callao: the cave with its light shafts. Pagsanjan: a gorge with a bangka. Banaue: terrace steps. Cappadocia: fairy chimneys with balloons.
4. Write the preview `.scratch/hills-normal/art-preview.png`: every sprite at 3× scale, with labels. Look at it and fix what is wrong before you report.

**Checklist**
- [ ] Every file in spec section 8 exists with the exact name and size (list them)
- [ ] The preview has been looked at; list what you changed after looking
- [ ] No existing PNG was changed

**Blocked by:** none

**Status:** not started

**Builder to run:** none (ticket 06 uses these)
