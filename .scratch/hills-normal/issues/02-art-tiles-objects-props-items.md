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
- [x] Every file in spec section 8 exists with the exact name and size (list them)
- [x] The preview has been looked at; list what you changed after looking
- [x] No existing PNG was changed

**Blocked by:** none

**Status:** done

**Verified:** 76 PNG paths and sizes checked, including 75 new assets. Opened the revised 3× preview. Unity EditMode passed 334 of 334 tests.

**Builder to run:** none (ticket 06 uses these)

## Files

- `Tools/Art/make_hills_normal_art.py` draws the new sprites and review sheet.
- `.scratch/hills-normal/art-preview.png` shows every sprite at 3× size with labels.
- Unity generated a matching `.meta` for each new PNG.
- `Assets/Art/Items/Sprites/aragonite.png` (24 x 24)
- `Assets/Art/Items/Sprites/coquina.png` (24 x 24)
- `Assets/Art/Items/Sprites/dripstone.png` (24 x 24)
- `Assets/Art/Items/Sprites/flowstone.png` (24 x 24)
- `Assets/Art/Items/Sprites/ignimbrite.png` (24 x 24)
- `Assets/Art/Items/Sprites/volcanic_breccia.png` (24 x 24)
- `Assets/Art/Items/Sprites/zeolite.png` (24 x 24)
- `Assets/Art/Lesson/Sprites/lesson_banaue.png` (120 x 82)
- `Assets/Art/Lesson/Sprites/lesson_callao_cave.png` (120 x 82)
- `Assets/Art/Lesson/Sprites/lesson_cappadocia.png` (120 x 82)
- `Assets/Art/Lesson/Sprites/lesson_hundred_islands.png` (120 x 82)
- `Assets/Art/Lesson/Sprites/lesson_pagsanjan.png` (120 x 82)
- `Assets/Art/Level/Sprites/balloon.png` (48 x 80)
- `Assets/Art/Level/Sprites/balloon_field.png` (56 x 48)
- `Assets/Art/Level/Sprites/balloon_flame_00.png` (8 x 8)
- `Assets/Art/Level/Sprites/balloon_flame_01.png` (8 x 8)
- `Assets/Art/Level/Sprites/bat_00.png` (16 x 10)
- `Assets/Art/Level/Sprites/bat_01.png` (16 x 10)
- `Assets/Art/Level/Sprites/bat_02.png` (16 x 10)
- `Assets/Art/Level/Sprites/boat_dock.png` (56 x 48)
- `Assets/Art/Level/Sprites/boulder_big_cave.png` (48 x 36)
- `Assets/Art/Level/Sprites/boulder_big_chimney.png` (48 x 36)
- `Assets/Art/Level/Sprites/boulder_big_gorge.png` (48 x 36)
- `Assets/Art/Level/Sprites/boulder_big_island.png` (48 x 36)
- `Assets/Art/Level/Sprites/boulder_big_terrace.png` (48 x 36)
- `Assets/Art/Level/Sprites/boulder_small_cave.png` (32 x 24)
- `Assets/Art/Level/Sprites/boulder_small_chimney.png` (32 x 24)
- `Assets/Art/Level/Sprites/boulder_small_gorge.png` (32 x 24)
- `Assets/Art/Level/Sprites/boulder_small_island.png` (32 x 24)
- `Assets/Art/Level/Sprites/boulder_small_terrace.png` (32 x 24)
- `Assets/Art/Level/Sprites/cave_mouth.png` (56 x 48)
- `Assets/Art/Level/Sprites/cave_soil.png` (16 x 16)
- `Assets/Art/Level/Sprites/cave_top.png` (16 x 8)
- `Assets/Art/Level/Sprites/chimney_soil.png` (16 x 16)
- `Assets/Art/Level/Sprites/chimney_top.png` (16 x 8)
- `Assets/Art/Level/Sprites/current_mark_00.png` (12 x 6)
- `Assets/Art/Level/Sprites/current_mark_01.png` (12 x 6)
- `Assets/Art/Level/Sprites/current_top.png` (16 x 8)
- `Assets/Art/Level/Sprites/falls_landing.png` (56 x 48)
- `Assets/Art/Level/Sprites/float_raft.png` (48 x 10)
- `Assets/Art/Level/Sprites/float_ripple_00.png` (16 x 4)
- `Assets/Art/Level/Sprites/float_ripple_01.png` (16 x 4)
- `Assets/Art/Level/Sprites/gorge_soil.png` (16 x 16)
- `Assets/Art/Level/Sprites/gorge_top.png` (16 x 8)
- `Assets/Art/Level/Sprites/ifugao_house.png` (56 x 56)
- `Assets/Art/Level/Sprites/island_soil.png` (16 x 16)
- `Assets/Art/Level/Sprites/island_top.png` (16 x 8)
- `Assets/Art/Level/Sprites/ledge_cave.png` (16 x 10)
- `Assets/Art/Level/Sprites/ledge_chimney.png` (16 x 10)
- `Assets/Art/Level/Sprites/ledge_gorge.png` (16 x 10)
- `Assets/Art/Level/Sprites/ledge_island.png` (16 x 10)
- `Assets/Art/Level/Sprites/ledge_terrace.png` (16 x 10)
- `Assets/Art/Level/Sprites/Props/bangka.png` (40 x 14)
- `Assets/Art/Level/Sprites/Props/bangka_boatman.png` (40 x 20)
- `Assets/Art/Level/Sprites/Props/bat_sign.png` (20 x 20)
- `Assets/Art/Level/Sprites/Props/bulul.png` (12 x 20)
- `Assets/Art/Level/Sprites/Props/chapel_glow_00.png` (24 x 24)
- `Assets/Art/Level/Sprites/Props/chapel_glow_01.png` (24 x 24)
- `Assets/Art/Level/Sprites/Props/cone_house.png` (32 x 48)
- `Assets/Art/Level/Sprites/Props/dry_shrub.png` (14 x 8)
- `Assets/Art/Level/Sprites/Props/falls_mist_00.png` (24 x 16)
- `Assets/Art/Level/Sprites/Props/falls_mist_01.png` (24 x 16)
- `Assets/Art/Level/Sprites/Props/far_balloon_00.png` (12 x 16)
- `Assets/Art/Level/Sprites/Props/far_balloon_01.png` (12 x 16)
- `Assets/Art/Level/Sprites/Props/farmer_00.png` (16 x 20)
- `Assets/Art/Level/Sprites/Props/farmer_01.png` (16 x 20)
- `Assets/Art/Level/Sprites/Props/fern.png` (16 x 12) (reused existing sprite)
- `Assets/Art/Level/Sprites/Props/mangrove.png` (24 x 20)
- `Assets/Art/Level/Sprites/Props/rice_stalk_00.png` (10 x 12)
- `Assets/Art/Level/Sprites/Props/rice_stalk_01.png` (10 x 12)
- `Assets/Art/Level/Sprites/Props/seabird_00.png` (12 x 8)
- `Assets/Art/Level/Sprites/Props/seabird_01.png` (12 x 8)
- `Assets/Art/Level/Sprites/Props/stalagmite.png` (12 x 20)
- `Assets/Art/Level/Sprites/terrace_soil.png` (16 x 16)
- `Assets/Art/Level/Sprites/terrace_top.png` (16 x 8)
- `Assets/Art/Level/Sprites/water_fill.png` (16 x 16)

Preview fixes: separated the rice seed heads, drew a face with a finger at its lips for the bat sign, and spread the daylight across the cave floor. The existing fern matched the exact path and size and was reused. The tracked PNG diff was empty.
