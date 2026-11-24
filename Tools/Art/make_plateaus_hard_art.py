"""Draws the PNGs for Plateaus Hard 1 to 7 (D-074): seven level themes (Deccan Traps, Simien Highlands, Sandstone Arches,
Cliff Dwellings, Ice Plateau, Loess Terraces and Roof of the World) with their goals, the hot-spring vent and steam,
sand and snow gusts, the gelada and yak herds, three new tops and a slab, the new props, three items, seven lesson
pictures, the Completion Screen plateau, the champion flag on the map and the empty champion ribbon.

Run from the Unity project folder:  python Tools/Art/make_plateaus_hard_art.py
Add  --root SOME_FOLDER  to write every file under another folder instead (used to check that two runs match).
It never overwrites a PNG: when a file with the same name is already there with other pixels, it stops before writing.
It reuses the helpers of make_mainmenu_art.py, make_landform_map_art.py, make_level_art.py, make_lesson_art.py,
make_normal_art.py and make_plains_hard_art.py.
All colours come from Assets/Art/Palettes/landgrand.gpl or mix two of its colours.
The soft loess tile is saved as loess_soft_top.png, because loess_top.png is the Loess Terraces ground top.
"""
import io  # io turns a picture into PNG bytes so two files can be compared
import math  # math gives circles and waves
import os  # os joins folder paths
import random  # random gives a seeded, repeatable spread of grains and flakes
import sys  # sys reads the --root option and stops on a name clash
from PIL import Image  # Image builds the preview sheet
import make_mainmenu_art as mm  # Palette, new_image, fill_rect, paste and the pixel font
import make_landform_map_art as lm  # Map colours, the fixed noise and the ridge helper
import make_level_art as la  # put, outline, wave, draw_grid and boulder_colored for level sprites
import make_lesson_art as ls  # framed, disc, fill_poly and inside
import make_normal_art as na  # mix, the shared tile makers, sky and the blob shape
import make_plains_hard_art as ph  # ell, line and ring for animals and rims

mix = na.mix  # The colour halfway between two palette colours
SEED = 74  # Seed for every random spread, so each run draws the same pixels

# Palette colours that the helper scripts do not name.
SHIRT = (63, 143, 134)  # Shirt, from the palette: blue-green
SHIRT_SH = (45, 107, 100)  # Shirt shade, from the palette: deep blue-green
PANTS = (92, 74, 61)  # Pants, from the palette: grey-brown
BOOTS = (61, 42, 30)  # Boots, from the palette: very dark brown
HAMMER = (154, 163, 171)  # Hammer head, from the palette: steel grey
SUN_GLOW = (255, 225, 122)  # Sun glow, from the palette: warm yellow

# H1 Deccan Traps: dark basalt steps, red-brown soil and dry grass.
BAS = mm.STRAP  # Dark basalt
BAS_SH = mix(mm.STRAP, mm.OUTLINE)  # Basalt in deep shade
BAS_LT = mix(mm.STRAP, mm.MASK_SH)  # Sunny basalt face
BAS_HI = mm.MASK_SH  # Bright basalt edge
RED_SOIL = mix(mm.PLATEAU_SH, lm.BACKPACK)  # Red-brown soil
RED_SOIL_SH = mm.BACKPACK_SH  # Red soil in shade
RED_SOIL_LT = mix(mm.PLATEAU, mm.PLATEAU_SH)  # Sunny red soil
DRYG = mix(mm.VEST, mm.HELMET_SH)  # Dry golden grass
DRYG_SH = mm.VEST_SH  # Dry grass in shade
DRYG_LT = mix(mm.VEST, mm.HEADLAMP)  # Sunny dry grass tips
WOOD = lm.HANDLE  # Light wood
WOOD_SH = mm.GROUND_SH  # Wood in shade
IRON = mm.STRAP  # Dark iron studs and bands

# H2 Simien Highlands: green highland grass on dark basalt cliffs.
GRASS = mm.HILLS  # Highland grass
GRASS_SH = mm.HILLS_SH  # Grass in shade
GRASS_LT = mm.PLAINS  # Sunny grass
SIM = mix(mm.STRAP, PANTS)  # Dark grey-brown basalt
SIM_SH = mix(mm.STRAP, mm.OUTLINE)  # Basalt in deep shade
SIM_LT = mix(PANTS, mm.MASK_SH)  # Sunny basalt
MUD = mix(mm.GROUND, mm.VEST)  # Mud wall of the hut
MUD_SH = mm.GROUND  # Mud wall in shade
MUD_LINE = mm.GROUND_SH  # Line round the mud wall
THATCH = mix(mm.VEST, mm.HELMET_SH)  # Straw roof
THATCH_SH = mm.VEST_SH  # Straw roof in shade
THATCH_LT = mix(mm.VEST, mm.HEADLAMP)  # Sunny straw

# H3 Sandstone Arches (Tassili n'Ajjer): orange sandstone and sand.
TAS = mix(mm.PLATEAU, mm.HELMET_SH)  # Orange sandstone
TAS_SH = mm.PLATEAU_SH  # Sandstone in shade
TAS_LT = mix(mm.PLATEAU, mm.HELMET)  # Sunny sandstone
TAS_DK = lm.BACKPACK  # Dark holes and specks
SAND = mix(mm.HELMET, mm.VEST)  # Golden sand
SAND_LT = mix(mm.HELMET, mm.HEADLAMP)  # Sunny sand
SAND_SH = mix(mm.VEST, mm.PLATEAU)  # Sand in shade
OCHRE = lm.BACKPACK  # Red ochre paint on the rock
OCHRE_DK = mm.BACKPACK_SH  # Dark ochre paint
PAINT_W = mm.HEADLAMP  # Pale paint on the rock

# H4 Cliff Dwellings (Mesa Verde): buff sandstone and juniper green.
MV = mm.VEST  # Buff sandstone
MV_SH = mm.VEST_SH  # Sandstone in shade
MV_LT = mix(mm.VEST, mm.HEADLAMP)  # Sunny sandstone
MV_DK = mix(mm.VEST_SH, mm.GROUND_SH)  # Dark streaks and specks
MV_SOIL = mix(mm.VEST, mm.GROUND)  # Dusty soil on the mesa top
ALCOVE = mix(mm.VEST_SH, mm.DARK)  # Deep shade under the overhang
JUN = mix(mm.LEAF, SHIRT)  # Blue-green juniper
JUN_SH = mix(mm.LEAF, mm.OUTLINE)  # Juniper in shade
JUN_LT = mix(mm.HILLS_SH, SHIRT)  # Sunny juniper
JUN_BARK = mm.MASK_SH  # Grey twisted juniper bark
JUN_BERRY = mix(lm.LENS, lm.MOUNTAIN)  # Blue juniper berry

# H5 Ice Plateau (Antarctica): packed snow over blue-white ice.
SNW = mm.WHITE  # Packed white snow
SNW_SH = lm.SNOW  # Snow in soft shade
SNW_DK = lm.HAZE  # Snow in shade
ICE = mix(mm.SKY_LIGHT, lm.HAZE)  # Blue-white ice
ICE_LT = mix(lm.SNOW, mm.SKY_LIGHT)  # Pale ice layer
ICE_SH = lm.HAZE_SH  # Ice in shade
ICE_DK = lm.MOUNTAIN  # Dark crack in the ice
BLUE_ICE = mix(lm.LENS, mm.SKY_LIGHT)  # Bright blue wind-polished ice
BLUE_ICE_SH = lm.LENS  # Blue ice in shade
HUT = mm.SCARF  # Orange-red station hut
HUT_SH = mm.SCARF_SH  # Hut in shade
HUT_LT = mix(mm.SCARF, mm.HEADLAMP)  # Sunny hut edge
GLASS = mm.SKY_LIGHT  # Window glass
GLASS_SH = lm.HAZE  # Glass in shade

# H6 Loess Terraces: yellow loess earth cut into steps.
LO = mix(mm.VEST, mm.HELMET)  # Yellow loess
LO_SH = mix(mm.VEST_SH, mm.HELMET_SH)  # Loess in shade
LO_LT = mix(mm.VEST, mm.HEADLAMP)  # Sunny loess
LO_DK = mm.VEST_SH  # Dark loess streaks
CROP = lm.PLAINS_SH  # Green crop line on a terrace
CROP_SH = mm.HILLS_SH  # Crop in shade
PAPER = mm.HEADLAMP  # Window paper behind the lattice
CORN = mm.HELMET  # Yellow corn cobs hanging by the door

# H7 Roof of the World (Tibetan Plateau): brown alpine grass, grey rock and some snow.
TG = mix(mm.GROUND, mm.VEST)  # Brown alpine grass
TG_SH = mm.GROUND_SH  # Grass in shade
TG_LT = mm.VEST  # Sunny grass tips
RK = mm.MASK_SH  # Grey rock
RK_SH = mix(mm.MASK_SH, mm.STRAP)  # Rock in shade
RK_LT = mix(mm.MASK, mm.MASK_SH)  # Sunny rock
TENT = mm.HELMET  # Yellow tent
TENT_SH = mm.HELMET_SH  # Tent in shade
LAKE = mix(SHIRT, lm.LENS)  # Turquoise salt lake
LAKE_SH = SHIRT  # Deeper lake water

# Animals, props and items.
GEL = mm.GROUND  # Brown gelada fur
GEL_SH = mm.GROUND_SH  # Fur in shade
GEL_MANE = mix(mm.GROUND, mm.VEST)  # Long pale cape of hair
GEL_FACE = mm.DARK  # Dark gelada face
GEL_CHEST = mm.SCARF  # Red chest patch
YAK = mix(mm.DARK, mm.OUTLINE)  # Dark shaggy yak hair
YAK_LT = mm.DARK  # Yak hair in light
YAK_HI = PANTS  # Shine on the yak hair
HORN = mm.HEADLAMP  # Pale horn
HORN_SH = mm.MASK  # Horn in shade
LANGUR = mm.MASK  # Grey langur fur
LANGUR_SH = mm.MASK_SH  # Fur in shade
LANGUR_LT = lm.SNOW  # Pale fur round the face
VULTURE = mm.STRAP  # Dark vulture wings
VULTURE_BODY = mix(mm.HELMET_SH, mm.VEST)  # Rusty-orange vulture body
TURKEY = mix(mm.BACKPACK_SH, mm.STRAP)  # Dark bronze turkey feathers
TURKEY_LT = mix(lm.BACKPACK, mm.HELMET_SH)  # Bronze shine
TURKEY_HEAD = mix(lm.LENS, mm.SKY_LIGHT)  # Pale blue head
TURKEY_RED = mm.SCARF  # Red wattle
BHARAL = mix(mm.MASK_SH, lm.MOUNTAIN)  # Blue-grey sheep coat
BHARAL_SH = lm.MOUNTAIN_SH  # Coat in shade
BHARAL_HORN = mm.VEST_SH  # Olive-brown horns
COAL = mix(mm.STRAP, mm.OUTLINE)  # Black coal
COAL_MID = mm.STRAP  # Coal face in light
COAL_SHEEN = lm.MOUNTAIN_SH  # Blue sheen on the coal
METEOR = mm.DARK  # Dark brown-black stone
METEOR_SH = mix(mm.DARK, mm.OUTLINE)  # Stone in shade and thumb-print dents
CRUST = mix(mm.OUTLINE, mm.STRAP)  # Thin black fusion crust
OPAL = lm.SNOW  # Milky white opal
OPAL_SH = mm.MASK  # Opal in shade
OPAL_FLASH = (mm.SCARF, mm.HELMET, mm.HILLS, lm.LENS, SHIRT, mix(mm.SCARF, mm.WHITE))  # Rainbow flashes in the opal
GOLD = mm.HELMET  # Gold flag and ribbon
GOLD_SH = mm.HELMET_SH  # Gold in shade
GOLD_LT = mix(mm.HELMET, mm.HEADLAMP)  # Bright gold edge


# ---------- Shared small helpers ----------

def stone_course(img, x0, x1, y0, y1, faces, joint, h=4, w=8):
    """Paints a wall of stone blocks in courses; the block colour comes from the fixed noise."""  # Describes the helper
    for y in range(y0, y1):  # Rows of the wall
        row = (y - y0) // h  # Course of blocks
        for x in range(x0, x1):  # Columns of the wall
            sx = (x - x0 + (row % 2) * (w // 2)) // w  # Block number along the course
            col = faces[lm.noise(sx, row) % len(faces)]  # Colour of this block
            if (y - y0) % h == h - 1 or (x - x0 + (row % 2) * (w // 2)) % w == 0:  # Joint between blocks
                col = joint  # Dark joint
            la.put(img, x, y, col)  # Paint it


def arch_open(x, y, cx, top, half):
    """True when (x, y) is inside a round-topped doorway with its middle at cx, its top row at top and half width half."""  # Describes the helper
    if y >= top + half:  # Below the round top
        return abs(x + 0.5 - cx) <= half  # Inside the straight sides
    return (x + 0.5 - cx) ** 2 + (y + 0.5 - top - half) ** 2 <= half * half  # Inside the round top


# ---------- H1 Deccan Traps (Deccan Plateau, India) ----------

def deccan_top():
    """A 16x8 tile of dry golden grass over red-brown soil, with basalt pebbles."""  # Describes the tile
    specks = [(2, 1, DRYG_SH), (3, 2, DRYG_LT), (7, 2, DRYG_SH), (11, 1, DRYG_SH), (12, 2, DRYG_LT), (14, 2, DRYG_SH),  # Grass blades
              (5, 6, BAS), (6, 6, BAS_LT), (12, 6, RED_SOIL_SH), (9, 7, BAS)]  # Pebbles and specks in the soil
    return na.ground_top(RED_SOIL, DRYG, DRYG_LT, RED_SOIL_SH, specks)  # Dry grass over red soil


def deccan_soil():
    """A 16x16 tile of two dark basalt flows, each with a sunny top step and a thin red soil band under it."""  # Describes the tile
    layers = []  # Extra pixels on the basalt
    for y0 in (0, 8):  # Two lava flows in the tile
        layers += [(x, y0, BAS_LT) for x in range(16)]  # Sunny top edge of the flow, like a step
        layers += [(x, y0 + 6, RED_SOIL) for x in range(16)]  # Red soil band between two flows
        layers += [(x, y0 + 7, RED_SOIL_SH) for x in range(16) if x % 5]  # Shade under the red band
        layers += [(x, y, BAS_SH) for x in (3, 10) for y in range(y0 + 1, y0 + 6)]  # Upright joints in the flow
    return na.soil_tile(BAS, BAS_SH, layers)  # Basalt with its steps


def ledge_deccan():
    """A 16x10 basalt ledge with a dry grass top and a red soil band."""  # Describes the tile
    flecks = [(x, 5, RED_SOIL) for x in range(16) if x % 6]  # Red band with small gaps
    flecks += [(4, 3, BAS_LT), (11, 6, BAS_SH), (2, 0, DRYG_LT), (9, 1, DRYG_SH), (13, 0, DRYG_LT)]  # Rock and grass bits
    return na.ledge_tile(BAS, DRYG, BAS_SH, flecks)  # Basalt body with dry grass on top


def deccan_boulder(w, h):
    """A dark basalt boulder with a thin dry grass cap."""  # Describes the boulder
    return na.capped_boulder(w, h, BAS_LT, BAS, BAS_HI, BAS_SH, DRYG, 1)  # Basalt with dry grass on top


def hill_fort_gate():
    """A 56x48 stone gate of basalt blocks: two square towers with battlements and a round-topped gateway with a wooden door (the H1 goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    faces = (BAS_LT, mix(mm.STRAP, HAMMER), BAS_LT, BAS)  # Block colours of the basalt
    stone_course(img, 1, 55, 43, 47, (BAS,), BAS_SH, 4, 54)  # Plinth under the gate
    mm.fill_rect(img, 1, 43, 54, 1, BAS_HI)  # Sunny top of the plinth
    stone_course(img, 16, 40, 16, 43, faces, BAS_SH)  # Wall between the towers
    for x0 in (2, 40):  # Left and right towers
        stone_course(img, x0, x0 + 14, 8, 43, faces, BAS_SH)  # One tower
        mm.fill_rect(img, x0 + 10, 8, 4, 35, BAS)  # Shaded right side of the tower
        for k in range(4):  # Battlements on the tower
            mm.fill_rect(img, x0 + k * 4, 4, 2, 4, BAS_LT)  # One merlon
        mm.fill_rect(img, x0, 8, 14, 1, BAS_HI)  # Sunny top of the tower wall
        mm.fill_rect(img, x0 + 6, 20, 2, 5, mm.OUTLINE)  # Arrow slit
    for k in range(6):  # Battlements on the middle wall
        mm.fill_rect(img, 17 + k * 4, 12, 2, 4, BAS_LT)  # One merlon
    mm.fill_rect(img, 16, 16, 24, 1, BAS_HI)  # Sunny top of the middle wall
    for y in range(22, 43):  # Rows of the gateway
        for x in range(19, 37):  # Columns of the gateway
            if arch_open(x, y, 28, 22, 8):  # Inside the round-topped opening
                la.put(img, x, y, WOOD if x < 28 else WOOD_SH)  # Two wooden door leaves
                if (x - 20) % 4 == 0:  # Plank lines
                    la.put(img, x, y, WOOD_SH)  # Dark plank line
    mm.fill_rect(img, 28, 22, 1, 21, mm.OUTLINE)  # Gap between the two door leaves
    for y in (27, 34, 40):  # Iron bands across the doors
        for x in range(20, 37):  # Along each band
            if arch_open(x, y, 28, 22, 8):  # Only on the door
                la.put(img, x, y, IRON)  # Iron band
    for (x, y) in ((23, 30), (33, 30), (23, 37), (33, 37)):  # Iron studs
        la.put(img, x, y, BAS_HI)  # One stud
    for y in range(20, 30):  # Rows of the arch stones
        for x in range(17, 39):  # Columns of the arch stones
            inner = arch_open(x, y, 28, 22, 8)  # Inside the opening
            outer = arch_open(x, y, 28, 20, 10)  # Inside the ring of arch stones
            if outer and not inner:  # On the ring
                la.put(img, x, y, BAS_HI if (x + y) % 3 else BAS_SH)  # Light arch stones with joints
    return la.outline(img)  # Dark edge


# ---------- H2 Simien Highlands (Ethiopian Highlands) ----------

def simien_top():
    """A 16x8 tile of green highland grass over dark basalt, with small yellow daisies."""  # Describes the tile
    specks = [(3, 1, mm.HELMET), (10, 2, mm.HELMET), (6, 2, GRASS_SH), (13, 1, GRASS_SH), (1, 2, GRASS_LT),  # Daisies and grass
              (5, 6, SIM_LT), (12, 6, SIM_SH)]  # Specks in the basalt
    return na.ground_top(SIM, GRASS, GRASS_LT, GRASS_SH, specks)  # Grass over basalt


def simien_soil():
    """A 16x16 dark basalt tile of upright columns with a few green moss specks."""  # Describes the tile
    extra = []  # Extra pixels on the basalt
    for x in (0, 5, 11):  # Joints between columns
        for y in range(16):  # Down the whole tile, so it stacks
            extra.append((x, y, SIM_SH))  # Dark joint
            extra.append(((x + 1) % 16, y, SIM_LT))  # Lit edge of the next column
    extra += [(x, 9, SIM_SH) for x in range(1, 5)]  # A cross break in one column
    extra += [(x, 3, SIM_SH) for x in range(7, 11)]  # A cross break in another column
    extra += [(8, 12, GRASS_SH), (3, 6, GRASS_SH), (14, 14, GRASS_SH)]  # Moss specks
    return na.soil_tile(SIM, SIM_SH, extra)  # Basalt columns


def ledge_simien():
    """A 16x10 basalt ledge with a green grass top."""  # Describes the tile
    flecks = [(5, 3, SIM_SH), (5, 4, SIM_SH), (5, 5, SIM_SH), (6, 3, SIM_LT), (12, 4, SIM_SH), (12, 5, SIM_SH),  # Column joints
              (2, 0, GRASS_LT), (8, 1, mm.HELMET), (14, 0, GRASS_LT)]  # Grass and a daisy on top
    return na.ledge_tile(SIM, GRASS, SIM_SH, flecks)  # Basalt body with grass on top


def simien_boulder(w, h):
    """A dark basalt boulder with a cap of green grass."""  # Describes the boulder
    return na.capped_boulder(w, h, SIM_LT, SIM, mm.MASK_SH, SIM_SH, GRASS, 1)  # Basalt with grass on top


HUT_ROOF = [(26, 3), (47, 28), (5, 28)]  # Cone of the straw roof of the round hut


def highland_village():
    """A 56x48 round tukul hut with a mud wall and a cone straw roof, with a stick fence on both sides (the H2 goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    mm.fill_rect(img, 1, 44, 54, 3, GRASS_SH)  # Grass ground under the hut
    mm.fill_rect(img, 1, 44, 54, 1, GRASS)  # Sunny top of the ground
    for x in range(2, 55, 5):  # Tufts on the ground
        la.put(img, x, 45, mm.LEAF)  # One tuft
    for y in range(26, 44):  # Rows of the round wall
        for x in range(10, 42):  # Columns of the wall
            col = MUD if x < 34 else MUD_SH  # Lit left, shaded right, so it looks round
            if x in (10, 11) or x == 41:  # Curved ends of the wall
                col = MUD_SH  # Shade at the round edge
            if y in (31, 38):  # Two lines of the woven wall
                col = MUD_LINE  # Dark line
            la.put(img, x, y, col)  # Paint it
    mm.fill_rect(img, 22, 32, 8, 12, mm.OUTLINE)  # Doorway
    mm.fill_rect(img, 21, 31, 10, 1, WOOD_SH)  # Wooden lintel
    mm.fill_rect(img, 21, 32, 1, 12, WOOD_SH)  # Left door post
    mm.fill_rect(img, 30, 32, 1, 12, WOOD_SH)  # Right door post
    ls.fill_poly(img, HUT_ROOF, THATCH)  # Straw roof
    for y in range(4, 29):  # Rows of the roof
        for x in range(4, 48):  # Columns of the roof
            if ls.inside(x + 0.5, y + 0.5, HUT_ROOF):  # On the roof
                if x >= 26 + (y - 3) * 0.5:  # Right half
                    la.put(img, x, y, THATCH_SH if (x + y) % 4 else THATCH)  # Shaded straw
                elif (x + 2 * y) % 5 == 0:  # Straw lines on the lit half
                    la.put(img, x, y, THATCH_LT)  # Light straw line
    mm.fill_rect(img, 5, 27, 43, 2, THATCH_SH)  # Ragged lower edge of the roof
    for x in range(5, 48, 3):  # Straw ends hanging down
        la.put(img, x, 29, THATCH_SH)  # One straw end
    mm.fill_rect(img, 25, 0, 3, 4, mm.PLATEAU_SH)  # Clay pot on the roof top
    la.put(img, 25, 0, mm.PLATEAU)  # Sunny rim of the pot
    for (x0, x1) in ((1, 9), (43, 55)):  # Fence on each side of the hut
        for x in range(x0, x1, 3):  # Fence posts
            mm.fill_rect(img, x, 35, 1, 9, WOOD)  # One post
            la.put(img, x, 34, WOOD_SH)  # Sharp post top
        mm.fill_rect(img, x0, 37, x1 - x0, 1, WOOD_SH)  # Upper rail
        mm.fill_rect(img, x0, 41, x1 - x0, 1, WOOD_SH)  # Lower rail
    return la.outline(img)  # Dark edge


# ---------- H3 Sandstone Arches (Tassili n'Ajjer, Algeria) ----------

def tassili_top():
    """A 16x8 tile of golden sand over orange sandstone, with ripples and pebbles."""  # Describes the tile
    specks = [(1, 2, SAND_SH), (2, 1, SAND_LT), (6, 2, SAND_SH), (7, 1, SAND_LT), (11, 2, SAND_SH), (12, 1, SAND_LT),  # Sand ripples
              (4, 6, TAS_DK), (13, 6, TAS_LT), (9, 7, TAS_SH)]  # Specks in the sandstone
    return na.ground_top(TAS, SAND, SAND_LT, TAS_SH, specks)  # Sand over sandstone


def tassili_soil():
    """A 16x16 orange sandstone tile with slanting beds and small wind-cut holes."""  # Describes the tile
    layers = [(x, 4, TAS_LT) for x in range(16)]  # A sunny bed
    layers += [(x, 11, TAS_SH) for x in range(16)]  # A shaded bed
    layers += [(1 + t, 9 - t // 2, TAS_SH) for t in range(6)]  # Slanting cross-bed
    layers += [(9 + t, 15 - t // 2, TAS_SH) for t in range(6)]  # Another cross-bed
    for (cx, cy) in ((11, 7), (4, 13)):  # Two small wind-cut holes
        layers += [(cx, cy, TAS_DK), (cx + 1, cy, TAS_DK), (cx, cy + 1, TAS_SH), (cx + 1, cy + 1, TAS_SH), (cx - 1, cy, TAS_SH)]  # One hole
    return na.soil_tile(TAS, TAS_DK, layers)  # Sandstone with its beds


def ledge_tassili():
    """A 16x10 orange sandstone ledge with a sandy top."""  # Describes the tile
    flecks = [(x, 5, TAS_SH) for x in range(16) if x % 7]  # Bed line
    flecks += [(4, 3, TAS_LT), (11, 6, TAS_DK), (12, 6, TAS_SH), (9, 0, SAND_LT), (2, 1, SAND_SH)]  # Rock and sand bits
    return na.ledge_tile(TAS, SAND, TAS_SH, flecks)  # Sandstone body with sand on top


def tassili_boulder(w, h):
    """A rounded orange sandstone boulder with sunny beds."""  # Describes the boulder
    return la.boulder_colored(w, h, TAS, TAS_SH, TAS_LT, TAS_DK, (TAS_LT, TAS_SH))  # Layered orange rock


COW = ["......#.#",  # Horns
       "#.....###",  # Tail tip and head
       ".#######.",  # Back
       ".##ww###.",  # Body with a pale patch
       ".#.#..#.#"]  # Legs
STICK = [".#.",  # Head
         "###",  # Arms
         ".#.",  # Body
         ".#.",  # Hips
         "#.#"]  # Legs


def rock_art_shelter():
    """A 56x48 sandstone overhang on two rock legs; under it a shaded back wall with simple painted cattle and stick people (the H3 goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    back = TAS_SH  # The deep-shaded back wall under the overhang
    mm.fill_rect(img, 4, 12, 48, 32, back)  # Back wall
    for y in range(16, 44, 5):  # Faint beds on the back wall
        mm.fill_rect(img, 4, y, 48, 1, mix(back, TAS_DK))  # One bed
    for x in range(56):  # Each column of the overhang roof
        bottom = 19 - int(round(6 * math.sin(math.pi * x / 55.0)))  # Curved underside: low at the legs, high in the middle
        bump = 1 + (lm.noise(x // 5, 1) % 3 if 2 < x < 53 else 2)  # Rounded, lumpy top edge
        for y in range(bump, bottom + 1):  # Rows of the roof in this column
            col = TAS if (y - 1) % 5 else TAS_LT  # Sandstone with sunny beds
            if y >= bottom - 1:  # The underside of the lip
                col = TAS_SH  # Shaded underside
            la.put(img, x, y, col)  # Paint it
        for y in (bottom + 1, bottom + 2):  # The deep shadow just under the lip
            la.put(img, x, y, TAS_SH if y == bottom + 2 else TAS_DK)  # Dark, then softer shadow
    for (x0, x1) in ((0, 7), (48, 56)):  # Left and right rock legs
        for y in range(14, 44):  # Rows of the leg
            for x in range(x0, x1):  # Columns of the leg
                col = TAS if x < x1 - 2 else TAS_SH  # Shaded right side
                if (y - 14) % 6 == 0:  # Beds across the leg
                    col = TAS_LT  # Sunny bed
                la.put(img, x, y, col)  # Paint it
    mm.fill_rect(img, 0, 43, 56, 4, SAND)  # Sandy floor
    mm.fill_rect(img, 0, 43, 56, 1, SAND_LT)  # Sunny top of the sand
    for x in range(3, 56, 6):  # Ripples in the sand
        la.put(img, x, 45, SAND_SH)  # One ripple
    paint = {"#": OCHRE_DK, "w": PAINT_W}  # Colours of the painted cattle
    for (x, y) in ((11, 24), (27, 29), (36, 22)):  # Three painted cattle
        la.draw_grid(img, COW, x, y, paint)  # One cow
    for (x, y, col) in ((10, 33, OCHRE_DK), (15, 34, PAINT_W), (22, 23, OCHRE_DK), (42, 31, PAINT_W)):  # Stick people
        la.draw_grid(img, STICK, x, y, {"#": col})  # One person
    return la.outline(img)  # Dark edge


# ---------- H4 Cliff Dwellings (Mesa Verde, USA) ----------

def mesaverde_top():
    """A 16x8 tile of dusty soil with juniper-green tufts over buff sandstone."""  # Describes the tile
    specks = [(2, 1, JUN), (3, 2, JUN_SH), (9, 2, JUN), (10, 1, JUN_LT), (14, 2, JUN_SH), (6, 2, MV_SH),  # Tufts and soil
              (5, 6, MV_DK), (12, 6, MV_LT)]  # Specks in the sandstone
    return na.ground_top(MV, MV_SOIL, MV_LT, MV_SH, specks)  # Dusty soil over sandstone


def mesaverde_soil():
    """A 16x16 buff sandstone tile with flat beds and a dark streak running down."""  # Describes the tile
    layers = [(x, 2, MV_LT) for x in range(16)]  # A sunny bed
    layers += [(x, 8, MV_SH) for x in range(16)]  # A shaded bed
    layers += [(x, 13, MV_SH) for x in range(16) if x % 4]  # A lower bed with gaps
    layers += [(12, y, MV_DK) for y in range(16) if y % 5 != 2]  # Dark streak down the face
    layers += [(4 + t, 6 - t // 2, MV_SH) for t in range(4)]  # Short cross-bed
    return na.soil_tile(MV, MV_DK, layers)  # Sandstone with its beds


def ledge_mesaverde():
    """A 16x10 buff sandstone ledge with a dusty top and a juniper tuft."""  # Describes the tile
    flecks = [(x, 5, MV_SH) for x in range(16) if x % 6]  # Bed line
    flecks += [(3, 0, JUN), (4, 1, JUN_SH), (11, 3, MV_LT), (8, 6, MV_DK)]  # Tuft and rock bits
    return na.ledge_tile(MV, MV_SOIL, MV_SH, flecks)  # Sandstone body with soil on top


def mesaverde_boulder(w, h):
    """A rounded buff sandstone boulder with faint beds."""  # Describes the boulder
    return la.boulder_colored(w, h, MV, MV_SH, MV_LT, MV_DK, (MV_LT, MV_SH))  # Layered buff rock


def house_block(img, x0, x1, top, bottom, door):
    """Paints one small stone house of buff blocks with a dark doorway or window."""  # Describes the helper
    stone_course(img, x0, x1, top, bottom, (MV_LT, MV, MV_LT), MV_SH, 3, 5)  # Stone courses
    mm.fill_rect(img, x1 - 1, top, 1, bottom - top, MV_SH)  # Shaded right edge
    mm.fill_rect(img, x0, top, x1 - x0, 1, MV_LT)  # Sunny top
    dx, dy, kind = door  # Where the opening is and its shape
    if kind == "T":  # A T-shaped doorway
        mm.fill_rect(img, dx, dy, 5, 2, mm.OUTLINE)  # Wide top of the T
        mm.fill_rect(img, dx + 1, dy + 2, 3, 4, mm.OUTLINE)  # Narrow foot of the T
    else:  # A small square window
        mm.fill_rect(img, dx, dy, 2, 2, mm.OUTLINE)  # Dark window


def cliff_palace_ladder():
    """A 56x48 cliff alcove under a sandstone overhang, with small stone houses, a square tower and a wooden ladder (the H4 goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    for y in range(0, 44):  # Rows of the cliff and the alcove
        for x in range(56):  # Columns
            inside = ((x - 27.5) / 27.0) ** 2 + ((y - 44) / 32.0) ** 2 <= 1.0  # Inside the half-oval alcove
            if inside and y >= 14:  # The alcove
                la.put(img, x, y, ALCOVE)  # Deep shade
            elif y >= 2 + (lm.noise(x // 5, 3) % 2):  # The cliff rock around the alcove
                col = MV if (y + la.wave(x, 1, 28)) % 6 else MV_LT  # Buff rock with sunny beds
                if lm.noise(x, y) < 5:  # A few dark specks
                    col = MV_DK  # Speck
                la.put(img, x, y, col)  # Paint it
    for x in range(56):  # The lip of the overhang
        for y in range(10, 44):  # From the top of the alcove
            if ((x - 27.5) / 27.0) ** 2 + ((y - 44) / 32.0) ** 2 <= 1.0 and y >= 14:  # First alcove row in this column
                la.put(img, x, y, MV_SH)  # Shaded underside of the overhang
                la.put(img, x, y + 1, MV_DK)  # Deeper shade under it
                break  # Next column
    for x in (3, 10, 20, 31, 45):  # Junipers on the mesa top
        ls.disc(img, x, 2, 2, JUN)  # One juniper crown
        la.put(img, x - 1, 1, JUN_LT)  # Sunny needles
        la.put(img, x + 1, 3, JUN_SH)  # Needles in shade
    house_block(img, 6, 17, 32, 44, (9, 36, "T"))  # Left house with a T door
    house_block(img, 17, 27, 28, 44, (21, 31, "w"))  # Taller house with a window
    house_block(img, 33, 41, 16, 44, (36, 20, "w"))  # Square tower
    la.put(img, 36, 28, mm.OUTLINE)  # Second tower window
    la.put(img, 37, 28, mm.OUTLINE)  # Second tower window, right half
    house_block(img, 41, 51, 31, 44, (44, 35, "T"))  # Right house with a T door
    for x in range(18, 27, 3):  # Roof beams poking out of the taller house
        la.put(img, x, 29, WOOD_SH)  # One beam end
    mm.fill_rect(img, 0, 44, 56, 3, MV_SH)  # Rock floor of the alcove
    mm.fill_rect(img, 0, 44, 56, 1, MV)  # Lit edge of the floor
    img = la.outline(img)  # Dark edge
    for y in range(24, 47):  # Rows of the ladder, drawn after the outline so it stays clean
        left = 27 + (1 if y < 35 else 0)  # Left pole leans a little to the right going up
        la.put(img, left, y, WOOD)  # Left pole
        la.put(img, left + 4, y, WOOD)  # Right pole
        if (46 - y) % 4 == 2:  # A rung every few rows
            for x in range(left + 1, left + 4):  # Across the rung
                la.put(img, x, y, WOOD)  # One rung pixel
    return img  # The finished picture


# ---------- H5 Ice Plateau (Antarctic Plateau) ----------

def iceplateau_top():
    """A 16x8 tile of packed white snow with low wind ridges over blue-white ice."""  # Describes the tile
    specks = [(2, 2, SNW_DK), (3, 2, SNW_DK), (4, 1, SNW_SH), (9, 2, SNW_DK), (10, 1, SNW_SH), (13, 2, SNW_DK),  # Wind ridges
              (5, 6, ICE_LT), (12, 6, ICE_SH)]  # Specks in the ice
    return na.ground_top(ICE, SNW_SH, SNW, SNW_DK, specks)  # Snow over ice


def iceplateau_soil():
    """A 16x16 blue-white ice tile with pale year layers, air bubbles and a thin crack."""  # Describes the tile
    layers = [(x, 3, ICE_LT) for x in range(16)]  # A pale year layer
    layers += [(x, 8 + la.wave(x, 1), ICE_SH) for x in range(16)]  # A shaded wavy layer
    layers += [(x, 13, ICE_LT) for x in range(16) if x % 5]  # A lower pale layer
    layers += [(5, 5, mm.WHITE), (11, 11, mm.WHITE), (2, 10, mm.WHITE), (13, 6, mm.WHITE)]  # Air bubbles
    layers += [(9, 0, ICE_DK), (9, 1, ICE_DK), (10, 2, ICE_DK), (10, 3, ICE_DK), (9, 4, ICE_SH)]  # A thin crack
    return na.soil_tile(ICE, ICE_SH, layers)  # Ice with its layers


def ledge_iceplateau():
    """A 16x10 ice ledge with a snow top."""  # Describes the tile
    flecks = [(x, 5, ICE_LT) for x in range(16) if x % 6]  # Pale layer
    flecks += [(4, 3, mm.WHITE), (11, 6, ICE_SH), (12, 3, ICE_DK), (8, 0, SNW), (2, 1, SNW_DK)]  # Ice and snow bits
    return na.ledge_tile(ICE, SNW_SH, ICE_SH, flecks)  # Ice body with snow on top


def iceplateau_boulder(w, h):
    """A blue-white ice block with a snow cap."""  # Describes the boulder
    return na.capped_boulder(w, h, ICE, ICE_SH, ICE_LT, ICE_DK, SNW, 2)  # Ice with snow on top


def research_station():
    """A 56x48 small orange-red research hut on steel legs above the snow, with a steps, windows and a weather mast (the H5 goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    for y in range(40, 47):  # Rows of the snow mound
        for x in range(1, 55):  # Columns of the mound
            if y >= 43 or abs(x - 27) < 24 + (y - 40) * 2:  # Rounded mound shoulders
                la.put(img, x, y, SNW_SH if x < 40 else SNW_DK)  # Lit and shaded snow
    mm.fill_rect(img, 1, 43, 54, 1, SNW)  # Bright snow line under the legs
    for x in (8, 20, 32, 42):  # Four steel legs
        mm.fill_rect(img, x, 31, 2, 12, mm.STRAP)  # One leg
        la.put(img, x, 31, mm.MASK_SH)  # Light leg top
    for (xa, xb) in ((9, 21), (33, 43)):  # Cross braces between the legs
        ph.line(img, xa, 32, xb, 40, mm.STRAP)  # One brace
        ph.line(img, xa, 40, xb, 32, mm.STRAP)  # The other brace
    mm.fill_rect(img, 5, 15, 42, 16, HUT)  # Hut body
    mm.fill_rect(img, 40, 15, 7, 16, HUT_SH)  # Shaded right end
    mm.fill_rect(img, 5, 29, 42, 2, HUT_SH)  # Shaded floor edge
    mm.fill_rect(img, 5, 15, 42, 1, HUT_LT)  # Sunny top edge
    for x in range(9, 40, 6):  # Panel seams on the wall
        mm.fill_rect(img, x, 16, 1, 13, HUT_SH)  # One seam
    mm.fill_rect(img, 4, 12, 44, 3, SNW_SH)  # Snow on the flat roof
    mm.fill_rect(img, 4, 12, 44, 1, SNW)  # Bright snow top
    for x0 in (11, 23):  # Two windows
        mm.fill_rect(img, x0, 19, 6, 5, GLASS)  # Glass
        mm.fill_rect(img, x0 + 4, 19, 2, 5, GLASS_SH)  # Glass in shade
        la.put(img, x0 + 1, 20, mm.WHITE)  # Glint
    mm.fill_rect(img, 33, 18, 6, 11, HUT_SH)  # Door
    la.put(img, 37, 24, mm.HELMET)  # Door handle
    for k in range(4):  # Steps down from the door to the snow
        mm.fill_rect(img, 35 + k * 2, 31 + k * 3, 4, 1, mm.STRAP)  # One step
    mm.fill_rect(img, 50, 4, 2, 39, mm.STRAP)  # Weather mast
    mm.fill_rect(img, 50, 4, 1, 39, mm.MASK_SH)  # Light edge of the mast
    for y in (14, 26):  # Cross bars on the mast
        mm.fill_rect(img, 48, y, 6, 1, mm.STRAP)  # One cross bar
    mm.fill_rect(img, 47, 4, 8, 1, mm.STRAP)  # Arm of the wind cups
    for x in (47, 54):  # Two wind cups
        mm.fill_rect(img, x - 1, 3, 2, 2, mm.MASK)  # One cup
    mm.fill_rect(img, 50, 0, 1, 4, mm.STRAP)  # Thin antenna at the top
    img = la.outline(img)  # Dark edge
    la.put(img, 50, 0, mm.SCARF)  # Small red light on the antenna tip
    return img  # The finished picture


def blue_ice_top():
    """A 16x8 slippery blue ice top with a few snow specks (used like ice_top)."""  # Describes the tile
    img = mm.new_image(16, 8)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 8, BLUE_ICE_SH)  # Ice body
    mm.fill_rect(img, 0, 0, 16, 4, BLUE_ICE)  # Bright blue ice cap
    mm.fill_rect(img, 0, 0, 16, 1, mm.SKY_LIGHT)  # Shiny top line
    for (x, y) in ((1, 2), (2, 2), (8, 1), (9, 3), (13, 2)):  # Polished streaks
        la.put(img, x, y, mm.SKY_LIGHT)  # One streak pixel
    for (x, y) in ((5, 1), (11, 2), (15, 1)):  # Snow specks on the ice
        la.put(img, x, y, mm.WHITE)  # One speck
    mm.fill_rect(img, 0, 4, 16, 1, lm.MOUNTAIN)  # Shadow line, like ice_top
    for (x, y) in ((3, 6), (10, 5), (14, 7)):  # Dark specks deep in the ice
        la.put(img, x, y, lm.MOUNTAIN_SH)  # One speck
    return img  # The finished tile


# ---------- H6 Loess Terraces (Loess Plateau, China) ----------

def loess_top():
    """A 16x8 tile of sunny dusty loess with small green crop tufts over yellow loess."""  # Describes the tile
    specks = [(2, 1, CROP), (3, 2, CROP_SH), (8, 2, CROP), (9, 1, CROP), (13, 2, CROP_SH), (6, 2, LO_SH),  # Crops and dust
              (4, 6, LO_DK), (11, 6, LO_LT), (14, 7, LO_SH)]  # Specks in the loess
    return na.ground_top(LO, LO_LT, mm.HEADLAMP, LO_SH, specks)  # Dusty top over loess


def loess_soil():
    """A 16x16 yellow loess tile cut into two terrace steps, each with a green crop line and upright dust streaks."""  # Describes the tile
    layers = []  # Extra pixels on the loess
    for y0 in (0, 8):  # Two terrace steps in the tile
        layers += [(x, y0, CROP) for x in range(16) if x % 3]  # Green crop line on the step
        layers += [(x, y0, CROP_SH) for x in range(16) if x % 3 == 0]  # Crop in shade
        layers += [(x, y0 + 1, LO_LT) for x in range(16)]  # Sunny flat step
        layers += [(x, y, LO_SH) for x in (2, 7, 13) for y in range(y0 + 2, y0 + 7)]  # Upright dust streaks on the face
        layers += [(x, y0 + 7, LO_SH) for x in range(16)]  # Shade at the foot of the face
    return na.soil_tile(LO, LO_DK, layers)  # Loess with its terraces


def ledge_loess():
    """A 16x10 loess ledge with a green crop top."""  # Describes the tile
    flecks = [(x, y, LO_SH) for x in (3, 10) for y in range(3, 7)]  # Upright streaks
    flecks += [(1, 0, CROP_SH), (6, 1, CROP_SH), (12, 0, LO_LT), (14, 1, CROP_SH)]  # Crop bits on top
    return na.ledge_tile(LO, CROP, LO_SH, flecks)  # Loess body with crops on top


def loess_boulder(w, h):
    """A rounded yellow loess lump with faint layers."""  # Describes the boulder
    return la.boulder_colored(w, h, LO, LO_SH, LO_LT, LO_DK, (LO_LT, LO_SH))  # Layered loess


def loess_soft_top():
    """A 16x8 soft loess top of loose yellow dust with deep footprints (used like soft_top)."""  # Describes the tile
    return na.ground_top(LO_SH, LO_LT, mm.HEADLAMP, LO_DK,  # Loose dust over shaded loess
                         [(1, 2, LO_SH), (2, 1, LO), (6, 2, LO_SH), (7, 1, LO), (11, 2, LO_SH), (12, 1, LO),  # Loose ripples
                          (4, 3, mm.HEADLAMP), (9, 6, LO_DK)])  # Pale dust and a dark speck


def cave_house():
    """A 56x48 loess cliff with two round-topped cave homes cut into it; each has a wooden front with a lattice window and a door (the H6 goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    for y in range(4, 44):  # Rows of the loess wall
        for x in range(56):  # Columns of the wall
            if y < 6 and lm.noise(x // 3, 5) % 3 == 0:  # A ragged top edge
                continue  # Leave it see-through
            col = LO if x % 9 not in (4,) else LO_SH  # Loess with upright streaks
            if lm.noise(x, y) < 6:  # A few dark specks
                col = LO_DK  # Speck
            la.put(img, x, y, col)  # Paint it
    for x in range(56):  # The top of the wall
        la.put(img, x, 6, CROP if x % 4 else CROP_SH)  # Green crops along the top
    for cx in (15, 41):  # Two cave homes
        for y in range(12, 44):  # Rows of the arch
            for x in range(cx - 11, cx + 11):  # Columns of the arch
                if arch_open(x, y, cx, 13, 10) and not arch_open(x, y, cx, 15, 8):  # Brick ring of the arch
                    la.put(img, x, y, LO_DK if (x + y) % 3 else LO_SH)  # Dark brick
                elif arch_open(x, y, cx, 15, 8):  # The wooden front inside the arch
                    la.put(img, x, y, WOOD)  # Wood frame
        for y in range(16, 31):  # Rows of the lattice window
            for x in range(cx - 6, cx + 6):  # Columns of the window
                if arch_open(x, y, cx, 17, 6):  # Inside the round window
                    grid = (x - cx) % 3 == 0 or (y - 17) % 3 == 0  # Bars of the lattice
                    diag = (x + y) % 6 == 0  # A slanting bar for a simple pattern
                    la.put(img, x, y, WOOD_SH if grid or diag else PAPER)  # Lattice bars over pale paper
        mm.fill_rect(img, cx - 7, 31, 14, 1, WOOD_SH)  # Beam under the window
        mm.fill_rect(img, cx - 3, 33, 6, 11, WOOD_SH)  # Door
        mm.fill_rect(img, cx - 3, 33, 1, 11, mm.OUTLINE)  # Gap at the left of the door
        la.put(img, cx + 1, 38, mm.HELMET)  # Door ring
        for k in range(3):  # Corn cobs hanging beside the door
            mm.fill_rect(img, cx + 5, 32 + k * 3, 2, 2, CORN)  # One cob
    mm.fill_rect(img, 0, 44, 56, 3, LO_SH)  # Packed earth yard
    mm.fill_rect(img, 0, 44, 56, 1, LO_LT)  # Sunny edge of the yard
    return la.outline(img)  # Dark edge


# ---------- H7 Roof of the World (Tibetan Plateau) ----------

def tibet_top():
    """A 16x8 tile of brown alpine grass over grey rock, with a small snow patch."""  # Describes the tile
    specks = [(2, 1, TG_SH), (6, 2, TG_SH), (9, 1, TG_LT), (13, 2, TG_SH), (11, 1, lm.SNOW), (12, 1, lm.SNOW),  # Grass and snow
              (4, 6, RK_SH), (13, 6, RK_LT)]  # Specks in the rock
    return na.ground_top(RK, TG, TG_LT, TG_SH, specks)  # Brown grass over grey rock


def tibet_soil():
    """A 16x16 grey rock tile with frost cracks and a few snow specks."""  # Describes the tile
    extra = [(x, 5 + la.wave(x, 1), RK_SH) for x in range(16)]  # A wavy shaded layer
    extra += [(x, 12, RK_LT) for x in range(16) if x % 4]  # A sunny layer
    extra += [(3, y, RK_SH) for y in range(8, 12)] + [(4, 7, RK_SH), (11, 1, RK_SH), (11, 2, RK_SH), (12, 3, RK_SH)]  # Frost cracks
    extra += [(8, 9, lm.SNOW), (14, 14, lm.SNOW), (1, 2, lm.SNOW)]  # Snow in the cracks
    return na.soil_tile(RK, RK_SH, extra)  # Grey rock


def ledge_tibet():
    """A 16x10 grey rock ledge with a brown grass top and a snow patch."""  # Describes the tile
    flecks = [(x, 5, RK_SH) for x in range(16) if x % 5]  # A layer line
    flecks += [(4, 3, RK_LT), (12, 6, RK_SH), (9, 0, lm.SNOW), (10, 0, lm.SNOW), (2, 1, TG_SH)]  # Rock, snow and grass bits
    return na.ledge_tile(RK, TG, RK_SH, flecks)  # Rock body with grass on top


def tibet_boulder(w, h):
    """A grey rock boulder with a snow cap."""  # Describes the boulder
    return na.capped_boulder(w, h, RK, RK_SH, RK_LT, mm.STRAP, lm.SNOW, 1)  # Grey rock with snow on top


def summit_camp():
    """A 56x48 stone hut and a yellow tent on grey rock, with a plain wooden post (the H7 goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    mm.fill_rect(img, 1, 43, 54, 4, RK)  # Grey rock ground
    mm.fill_rect(img, 1, 43, 54, 1, RK_LT)  # Sunny top of the rock
    for x in range(3, 55, 7):  # Brown grass tufts on the rock
        la.put(img, x, 43, TG)  # One tuft
    stone_course(img, 3, 27, 22, 43, (RK_LT, RK, mm.MASK), RK_SH, 4, 6)  # Stone hut walls
    mm.fill_rect(img, 23, 22, 4, 21, RK_SH)  # Shaded right end of the hut
    mm.fill_rect(img, 2, 19, 26, 3, WOOD_SH)  # Flat roof of beams and earth
    mm.fill_rect(img, 2, 19, 26, 1, TG)  # Grass growing on the roof
    for x in range(4, 27, 5):  # Beam ends under the roof
        la.put(img, x, 22, WOOD)  # One beam end
    mm.fill_rect(img, 11, 33, 6, 10, mm.OUTLINE)  # Doorway
    mm.fill_rect(img, 10, 32, 8, 1, WOOD)  # Wooden lintel
    mm.fill_rect(img, 5, 26, 3, 3, mm.OUTLINE)  # Small window
    ls.fill_poly(img, [(41, 25), (53, 43), (29, 43)], TENT)  # Tent body
    ls.fill_poly(img, [(41, 25), (53, 43), (44, 43)], TENT_SH)  # Shaded right side of the tent
    ls.fill_poly(img, [(41, 33), (44, 43), (38, 43)], mm.OUTLINE)  # Open tent door
    mm.fill_rect(img, 40, 23, 2, 3, mm.STRAP)  # Tent pole poking out
    mm.fill_rect(img, 30, 34, 2, 9, WOOD)  # Plain wooden post beside the tent, for tying the tent rope
    la.put(img, 31, 34, WOOD_SH)  # Shaded top of the post
    return la.outline(img)  # Dark edge


# ---------- Hot-spring vent and steam ----------

def vent_hotspring():
    """A 24x8 hot-spring pool set into the ground: a grey rock rim with pale mineral crust and blue-green water."""  # Describes the vent
    img = mm.new_image(24, 8)  # Empty picture
    mm.fill_rect(img, 0, 0, 24, 8, RK)  # Rim rock
    mm.fill_rect(img, 0, 0, 24, 1, RK_LT)  # Sunny top line like the ground
    mm.fill_rect(img, 0, 6, 24, 2, RK_SH)  # Dark bottom
    for (x, y) in ((2, 3), (4, 2), (19, 3), (21, 2), (20, 5), (1, 5)):  # Joints between the rim stones
        la.put(img, x, y, RK_SH)  # Dark pixel
    mm.fill_rect(img, 5, 0, 14, 6, mm.HEADLAMP)  # Pale mineral crust round the pool
    mm.fill_rect(img, 6, 0, 12, 5, SHIRT_SH)  # Shaded water near the rim
    mm.fill_rect(img, 7, 1, 10, 4, SHIRT)  # Blue-green water
    mm.fill_rect(img, 8, 2, 8, 2, LAKE)  # Bright warm water in the middle
    for x in (9, 13):  # Small bubbles
        la.put(img, x, 2, mm.SKY_LIGHT)  # One bubble
    la.put(img, 5, 0, RK_LT)  # Round the top left of the crust
    la.put(img, 18, 0, RK_LT)  # Round the top right of the crust
    return img  # The finished picture


def hotspring_warn(frame):
    """A 24x16 thin soft white-blue wisp rising from the pool; frame 1 is a little taller. It warns before the puff."""  # Describes the wisp
    img = mm.new_image(24, 16)  # Empty picture
    top = 8 if frame == 0 else 3  # How high the wisp climbs
    for y in range(15, top - 1, -1):  # From the pool up
        t = (15 - y) / float(15 - top)  # 0 at the pool, 1 at the tip
        x = 12 + int(round(1.5 * math.sin((15 - y) * 0.7 + frame * 1.5)))  # Wavy path
        alpha = int(200 - 130 * t)  # Softer than the vent wisp, fades going up
        la.put(img, x, y, mm.WHITE, alpha)  # Thin white line
        if t < 0.5:  # Thicker near the pool
            la.put(img, x + 1, y, mm.SKY_LIGHT, alpha)  # Pale blue pixel beside it
    return img  # The finished picture


def hotspring_steam(frame):
    """A 24x64 soft white-blue steam column; it grows with the frame and thins at the top. Bottom centre sits on the pool."""  # Describes the steam
    img = mm.new_image(24, 64)  # Empty picture
    height = (24, 40, 52, 60)[frame]  # How tall the puff is in this frame, like the vent steam
    widest = (5, 7, 9, 10)[frame]  # How wide the puff gets
    for y in range(63, 63 - height, -1):  # From the pool up
        t = (63 - y) / float(height)  # 0 at the pool, 1 at the top
        half = 2.5 + (widest - 2.5) * math.sin(min(1.0, t * 1.3) * math.pi / 2)  # Wider going up
        half += 1.6 * math.sin(y * 0.45 + frame * 1.3) * min(1.0, t * 3)  # Billowing sides
        for x in range(int(12 - half), int(12 + half)):  # Pixels of the column in this row
            if t > 0.65 and lm.noise(x + frame * 5, y) > 100 - (t - 0.65) * 240:  # Ragged, thin top
                continue  # Leave a hole
            alpha = 205 if t < 0.65 else int(205 - (t - 0.65) * 380)  # Softer than the vent steam, fades near the top
            left = x < 12 - half + 3  # Left edge is in shade, light is from the right
            la.put(img, x, y, ICE_LT if left else mm.WHITE, max(40, alpha))  # Pale blue or white
        if t > 0.1 and int(12 + half) - int(12 - half) > 3:  # Pale blue edge on the right so the puff reads on the sky
            la.put(img, int(12 + half) - 1, y, mm.SKY_LIGHT, 200 if t < 0.65 else 110)  # Edge pixel
    return img  # The finished picture


# ---------- Sand and snow gusts (20x6, like gust_00 to gust_03) ----------

GUST_STREAKS = ((1, 0, 10), (3, 6, 12), (5, 2, 8))  # The three streaks of the plain gust: row, start column and length


def gust_sand(frame):
    """A 20x6 sandy wind streak with flying grains; frames 0 to 3 slide right and fade, like the plain gust."""  # Describes the gust
    img = mm.new_image(20, 6)  # Empty picture
    rng = random.Random(SEED)  # The same grains on every frame and every run
    for (y, x0, length) in GUST_STREAKS:  # Three streaks
        for x in range(length):  # Along the streak
            col = SAND_LT if x % 3 else SAND  # Light sand with darker bits
            la.put(img, (x0 + x + frame * 2) % 20, y, col, 200 - frame * 40)  # Moves right and fades
    for _ in range(6):  # Six grains of sand
        gx, gy = rng.randrange(20), rng.choice((0, 2, 4))  # Where the grain starts, between the streaks
        la.put(img, (gx + frame * 3) % 20, gy, SAND_SH, 230 - frame * 40)  # Grains fly a little faster
    return img  # The finished picture


def gust_snow(frame):
    """A 20x6 white snowy wind streak with flakes; frames 0 to 3 slide right and fade, like the plain gust."""  # Describes the gust
    img = mm.new_image(20, 6)  # Empty picture
    rng = random.Random(SEED + 1)  # The same flakes on every frame and every run
    for (y, x0, length) in GUST_STREAKS:  # Three streaks
        for x in range(length):  # Along the streak
            col = mm.WHITE if x % 4 else lm.SNOW  # White streak with faint bits
            la.put(img, (x0 + x + frame * 2) % 20, y, col, 200 - frame * 40)  # Moves right and fades
    for _ in range(5):  # Five snowflakes
        gx, gy = rng.randrange(20), rng.choice((0, 2, 4))  # Where the flake starts, between the streaks
        la.put(img, (gx + frame * 3) % 20, gy, mm.WHITE, 240 - frame * 40)  # Flakes fly a little faster
        la.put(img, (gx + frame * 3 + 1) % 20, gy, mm.SKY_LIGHT, 160 - frame * 30)  # Pale tail on the flake
    return img  # The finished picture


# ---------- Gelada and yak herds (32x16, facing right, feet on the bottom row) ----------

GELADA_LEGS = [  # Leg positions for the three walk frames, facing right: (x, bottom row) of each leg
    ((7, 15), (11, 15), (20, 15), (24, 15)),  # Frame 0: all hands and feet down
    ((6, 14), (11, 15), (21, 15), (24, 14)),  # Frame 1: two legs lifted
    ((8, 15), (10, 14), (19, 14), (23, 15)),  # Frame 2: the other two legs lifted
]


def gelada(frame):
    """A 32x16 brown gelada walking right on all fours, with a long pale cape of hair, a dark face and a red chest patch."""  # Describes the animal
    img = mm.new_image(32, 16)  # Empty picture, facing right
    for x, bottom in GELADA_LEGS[frame]:  # Each leg
        mm.fill_rect(img, x, 10, 2, bottom - 9, GEL_SH)  # The leg from the body down
        mm.fill_rect(img, x, bottom, 2, 1, mm.DARK)  # Dark hand or foot on its bottom row
    for (x, y) in ((5, 8), (4, 9), (3, 10), (2, 11), (2, 12), (1, 13)):  # Long tail hanging down behind
        la.put(img, x, y, GEL_SH)  # One tail pixel
    mm.fill_rect(img, 0, 13, 2, 2, GEL_MANE)  # Tuft on the tail tip
    ph.ell(img, 14, 8, 9, 3, GEL, GEL_SH)  # Body
    ph.ell(img, 20, 7, 5, 4, GEL_MANE)  # Long cape of hair over the shoulders
    for x in range(16, 26, 2):  # Ragged hair hanging down
        la.put(img, x, 11, GEL_MANE)  # One hanging lock
    mm.fill_rect(img, 22, 9, 3, 2, GEL_CHEST)  # Red chest patch
    la.put(img, 23, 11, GEL_CHEST)  # Point of the red patch
    ph.ell(img, 26, 5, 3, 2.5, GEL_MANE)  # Hairy head
    mm.fill_rect(img, 27, 4, 3, 4, GEL_FACE)  # Dark face
    mm.fill_rect(img, 29, 6, 2, 2, GEL_FACE)  # Short muzzle
    la.put(img, 28, 5, mm.HEADLAMP)  # Pale eyelid
    la.put(img, 25, 3, GEL_SH)  # Small ear
    return la.outline(img)  # Dark edge


YAK_LEGS = [  # Leg positions for the three walk frames, facing right: (x, bottom row) of each leg
    ((6, 15), (10, 15), (19, 15), (23, 15)),  # Frame 0: all hooves down
    ((5, 14), (10, 15), (20, 15), (23, 14)),  # Frame 1: two legs lifted
    ((7, 15), (9, 14), (18, 14), (22, 15)),  # Frame 2: the other two legs lifted
]


def yak(frame):
    """A 32x16 dark shaggy yak walking right, with a shoulder hump, a long hair skirt, a plume tail and pale curved horns."""  # Describes the animal
    img = mm.new_image(32, 16)  # Empty picture, facing right
    for x, bottom in YAK_LEGS[frame]:  # Each leg
        mm.fill_rect(img, x, 11, 2, bottom - 10, YAK)  # Short sturdy leg
        mm.fill_rect(img, x, bottom, 2, 1, mm.MASK_SH)  # Grey hoof on its bottom row
    ph.ell(img, 14, 7, 10, 4.5, YAK_LT, YAK)  # Big body
    ph.ell(img, 19, 4, 4, 3, YAK_LT)  # Shoulder hump
    for x in range(5, 24):  # Long hair skirt hanging below the belly
        la.put(img, x, 11 + (x % 3 == 0), YAK)  # Ragged hair ends
        la.put(img, x, 10, YAK)  # Hair over the belly line
    for (x, y) in ((8, 4), (11, 3), (14, 4), (17, 3), (20, 2), (10, 7), (15, 7)):  # Shine on the shaggy hair
        la.put(img, x, y, YAK_HI)  # One shine pixel
    for (x, y) in ((4, 5), (3, 6), (3, 7), (2, 8), (2, 9), (3, 10)):  # Plume tail hanging behind
        la.put(img, x, y, YAK_LT)  # One tail pixel
    ph.ell(img, 26, 8, 3, 2.5, YAK_LT)  # Head held low
    mm.fill_rect(img, 28, 9, 2, 2, YAK)  # Muzzle
    la.put(img, 27, 7, mm.HEADLAMP)  # Eye shine
    for (x, y) in ((25, 5), (24, 4), (24, 3), (25, 2)):  # Back horn, curving up
        la.put(img, x, y, HORN_SH)  # One horn pixel
    for (x, y) in ((28, 6), (29, 5), (29, 4), (28, 3)):  # Front horn, curving up
        la.put(img, x, y, HORN)  # One horn pixel
    return la.outline(img)  # Dark edge


# ---------- Sandstone slab (48x10, like platform.png) ----------

def sandstone_slab():
    """A 48x10 carved buff sandstone slab that moves: three cut blocks with a carved zigzag band."""  # Describes the slab
    piece = na.ledge_tile(MV, MV_LT, MV_SH, [(x, 5 + (x % 4 if x % 4 < 2 else 3 - x % 4), MV_DK) for x in range(1, 15)])  # One block with a carved zigzag
    wide = mm.new_image(48, 10)  # The whole slab
    for i in range(3):  # Three blocks side by side
        wide.paste(piece, (i * 16, 0))  # Copy the block
    for x in (16, 32):  # Joints between the blocks
        for y in range(2, 9):  # Down the joint
            la.put(wide, x, y, MV_SH)  # Dark joint
    for (x, y) in ((4, 3), (21, 7), (40, 3)):  # A few chisel marks
        la.put(wide, x, y, MV_DK)  # One mark
    for y in range(10):  # Dark ends, so it reads as one slab
        la.put(wide, 0, y, mm.OUTLINE)  # Left end
        la.put(wide, 47, y, mm.OUTLINE)  # Right end
    return wide  # The finished slab


# ---------- Props (decoration) ----------

def dry_teak():
    """A 16x24 dry-season teak tree with a pale trunk, spreading branches and a few big brown-gold leaves."""  # Describes the prop
    img = mm.new_image(16, 24)  # Empty picture
    mm.fill_rect(img, 7, 12, 2, 12, WOOD)  # Trunk
    la.put(img, 8, 16, WOOD_SH)  # Bark mark
    mm.fill_rect(img, 5, 23, 6, 1, WOOD_SH)  # Roots
    for (x0, y0, x1, y1) in ((8, 13, 2, 5), (8, 12, 13, 4), (8, 14, 14, 9), (7, 15, 1, 10), (8, 11, 8, 2)):  # Bare branches
        ph.line(img, x0, y0, x1, y1, WOOD_SH)  # One branch
    for (cx, cy, r, col) in ((3, 5, 1, mm.HELMET_SH), (12, 4, 2, DRYG), (8, 2, 1, mm.VEST_SH), (14, 9, 1, mm.PLATEAU_SH), (2, 10, 1, DRYG)):  # Leaf clumps
        ls.disc(img, cx, cy, r, col)  # One clump of big leaves
    for (x, y) in ((5, 7), (11, 7), (10, 3), (4, 3)):  # Single leaves
        la.put(img, x, y, mm.HELMET_SH)  # One leaf
    return la.outline(img)  # Dark edge so the thin tree reads


def langur(frame):
    """A 12x12 grey langur monkey sitting, facing right, with a black face and a long tail; frame 1 swings the tail."""  # Describes the prop
    img = mm.new_image(12, 12)  # Empty picture
    if frame == 0:  # Tail curled up over the back
        tail = ((2, 8), (1, 7), (1, 6), (1, 5), (1, 4), (2, 3), (3, 3))  # Tail pixels
    else:  # Tail hanging down
        tail = ((2, 9), (1, 10), (1, 11), (2, 11), (0, 11))  # Tail pixels
    for (x, y) in tail:  # Each tail pixel
        la.put(img, x, y, LANGUR_SH)  # Grey tail
    ph.ell(img, 5, 7.5, 3, 3, LANGUR, LANGUR_SH)  # Sitting body
    mm.fill_rect(img, 5, 10, 4, 1, LANGUR_SH)  # Folded legs
    ph.ell(img, 7.5, 3.5, 2, 2, LANGUR)  # Grey head
    mm.fill_rect(img, 7, 2, 3, 1, LANGUR_LT)  # Pale brow over the face
    mm.fill_rect(img, 8, 3, 2, 2, mm.OUTLINE)  # Black face
    la.put(img, 10, 4, mm.OUTLINE)  # Black muzzle
    mm.fill_rect(img, 7, 8, 2, 1, mm.OUTLINE)  # Black hands on the knees
    return la.outline(img)  # Dark edge so the grey fur reads on grey rock


def giant_lobelia():
    """A 10x24 giant lobelia: a rosette of long leaves at the foot and a tall spike of small blue flowers."""  # Describes the prop
    img = mm.new_image(10, 24)  # Empty picture
    for y in range(1, 19):  # Rows of the flower spike
        half = 1 if y < 4 else 2  # Thin tip, thicker below
        for x in range(5 - half, 5 + half):  # Columns of the spike
            col = mix(lm.LENS, lm.MOUNTAIN) if (x + y) % 3 else GRASS_SH  # Small blue flowers between green bracts
            la.put(img, x, y, col)  # Paint it
    la.put(img, 4, 0, GRASS_SH)  # Pointed top of the spike
    for (tx, ty) in ((0, 18), (1, 16), (9, 18), (8, 16), (2, 20), (7, 20), (0, 21), (9, 21)):  # Tips of the long leaves
        ph.line(img, 5, 23, tx, ty, GRASS)  # One leaf from the middle
    for (x, y) in ((3, 21), (6, 21), (5, 22)):  # Shade in the middle of the rosette
        la.put(img, x, y, GRASS_SH)  # One shaded pixel
    return img  # No outline, like the other plants


def lammergeier(frame):
    """A 16x8 lammergeier (bearded vulture) flying right with long dark wings; frame 0 wings up, frame 1 wings down."""  # Describes the prop
    img = mm.new_image(16, 8)  # Empty picture
    mm.fill_rect(img, 4, 4, 7, 2, VULTURE_BODY)  # Rusty body
    mm.fill_rect(img, 11, 3, 2, 2, VULTURE_BODY)  # Head
    la.put(img, 13, 4, mm.STRAP)  # Hooked beak
    la.put(img, 12, 5, mm.OUTLINE)  # Black beard
    for (x, y) in ((0, 4), (1, 4), (2, 4), (3, 4), (1, 5), (2, 5), (3, 5), (0, 6), (1, 6)):  # Long wedge tail
        la.put(img, x, y, VULTURE)  # Tail pixel
    if frame == 0:  # Wings up: a long narrow wing swept up and back
        rows = ((3, 6, 10), (2, 5, 9), (1, 4, 8), (0, 3, 6))  # Each wing row: row, first column, last column
    else:  # Wings down: the wing held below the body
        rows = ((6, 5, 10), (7, 4, 8))  # Each wing row: row, first column, last column
    for (y, x0, x1) in rows:  # Each row of the wing
        for x in range(x0, x1 + 1):  # Along the row
            la.put(img, x, y, VULTURE if x > x0 else mm.OUTLINE)  # Dark wing with a black tip feather
    la.put(img, 7, 4, VULTURE)  # Wing root on the back
    return img  # Dark bird needs no outline


def saharan_cypress():
    """A 12x24 Saharan cypress: a narrow dark green cone of a tree on a short trunk."""  # Describes the prop
    img = mm.new_image(12, 24)  # Empty picture
    mm.fill_rect(img, 5, 19, 2, 5, WOOD_SH)  # Short trunk
    for y in range(1, 20):  # Rows of the crown
        half = 1 + (y * 4) // 19  # Narrow at the top, wider at the foot
        for x in range(6 - half, 6 + half):  # Columns of the crown
            col = mm.LEAF if x < 6 else JUN_SH  # Lit left, shaded right
            if (x + y * 2) % 7 == 0:  # Lumpy needles
                col = mm.HILLS_SH if x < 6 else mm.LEAF  # Lighter lump
            la.put(img, x, y, col)  # Paint it
    la.put(img, 5, 0, mm.LEAF)  # Pointed tip
    return la.outline(img)  # Dark edge so the dark tree reads on rock


def sand_ripple():
    """A 16x4 low sand ridge with wind ripples (decoration on the ground)."""  # Describes the prop
    img = mm.new_image(16, 4)  # Empty picture
    for x in range(16):  # Each column
        top = 3 - int(round(2.5 * math.sin(math.pi * x / 15.0)))  # Low rounded hump
        for y in range(top, 4):  # Rows of the hump in this column
            col = SAND_LT if y == top else SAND  # Sunny crest
            if x > 10 and y > top:  # Lee side in shade
                col = SAND_SH  # Shaded sand
            la.put(img, x, y, col)  # Paint it
    for x in (3, 7, 11):  # Ripple marks
        la.put(img, x, 3, SAND_SH)  # One ripple
    return img  # Soft sand needs no outline


def juniper_mesa():
    """A 16x16 twisted Utah juniper with grey bark and blue-green scaly leaves with a few blue berries."""  # Describes the prop
    img = mm.new_image(16, 16)  # Empty picture
    for (y, x) in ((15, 7), (14, 7), (13, 8), (12, 8), (11, 7), (10, 6), (9, 6)):  # Twisted trunk
        mm.fill_rect(img, x, y, 2, 1, JUN_BARK)  # Trunk pixel pair
        la.put(img, x + 1, y, mm.STRAP)  # Shaded side of the trunk
    mm.fill_rect(img, 5, 15, 6, 1, JUN_BARK)  # Roots
    ph.line(img, 8, 12, 12, 9, JUN_BARK)  # A twisted side branch
    for (cx, cy, r, col) in ((5, 6, 3, JUN_SH), (11, 7, 3, JUN_SH), (7, 4, 4, JUN), (12, 5, 2, JUN), (4, 4, 2, JUN)):  # Leafy lumps
        ls.disc(img, cx, cy, r, col)  # One lump
    for (x, y) in ((6, 1), (8, 1), (4, 3), (10, 3), (13, 4)):  # Sunny leaf tips
        la.put(img, x, y, JUN_LT)  # One light tip
    for (x, y) in ((9, 6), (5, 7), (12, 8)):  # Blue berries
        la.put(img, x, y, JUN_BERRY)  # One berry
    return la.outline(img)  # Dark edge so the tree reads on rock


def cliff_ladder():
    """A 16x48 wooden ladder leaning on a small sandstone ledge at the top right; its two poles stick up above the ledge."""  # Describes the prop
    img = mm.new_image(16, 48)  # Empty picture
    mm.fill_rect(img, 10, 7, 6, 4, MV)  # Small sandstone ledge
    mm.fill_rect(img, 10, 7, 6, 1, MV_LT)  # Sunny top of the ledge
    mm.fill_rect(img, 10, 10, 6, 1, MV_SH)  # Shaded under the ledge
    mm.fill_rect(img, 9, 6, 7, 1, mm.OUTLINE)  # Dark edge over the ledge
    mm.fill_rect(img, 9, 7, 1, 5, mm.OUTLINE)  # Dark edge at the ledge front
    mm.fill_rect(img, 10, 11, 6, 1, mm.OUTLINE)  # Dark edge under the ledge
    for y in range(1, 48):  # Rows of the ladder
        t = (47 - y) / 46.0  # 0 at the foot, 1 at the top
        left = round(1 + 7 * t)  # Left pole leans to the right going up
        for px in (left, left + 5):  # The two poles
            la.put(img, px, y, WOOD)  # Lit pole
            la.put(img, px + 1, y, WOOD_SH)  # Shaded side of the pole
        if (47 - y) % 5 == 2 and y > 3:  # A rung every few rows
            for x in range(left + 2, left + 5):  # Across the rung
                la.put(img, x, y, WOOD_SH)  # One rung pixel
    return img  # Two-tone poles read without an outline, so the rung gaps stay clear


def wild_turkey(frame):
    """A 12x12 wild turkey facing right with dark bronze feathers and a blue-red head; frame 1 pecks the ground."""  # Describes the prop
    img = mm.new_image(12, 12)  # Empty picture
    for (x, y) in ((5, 9), (5, 10), (5, 11), (7, 9), (7, 10), (7, 11)):  # Two legs
        la.put(img, x, y, mix(mm.SCARF, mm.MASK))  # Pink-grey leg
    for (x, y) in ((0, 7), (1, 7), (0, 8), (1, 8), (2, 8), (1, 9)):  # Tail held low behind
        la.put(img, x, y, TURKEY)  # One tail pixel
    for (x, y) in ((0, 9), (2, 9)):  # Pale tips of the tail feathers
        la.put(img, x, y, mm.VEST)  # One tip pixel
    ph.ell(img, 5, 6, 4, 2.6, TURKEY, TURKEY)  # Plump body
    mm.fill_rect(img, 3, 5, 4, 1, TURKEY_LT)  # Bronze shine along the wing
    la.put(img, 7, 5, mm.WHITE)  # Pale bar on the wing
    if frame == 0:  # Head up
        neck = ((8, 4), (9, 3), (9, 2))  # Neck pixels
        head, wattle, beak = (10, 1), (10, 3), (11, 2)  # Head, red wattle and beak
    else:  # Head down to peck
        neck = ((8, 6), (9, 7), (10, 8))  # Neck pixels
        head, wattle, beak = (10, 9), (9, 9), (11, 10)  # Head, red wattle and beak
    for (x, y) in neck:  # The neck
        la.put(img, x, y, TURKEY_HEAD)  # Pale blue neck
    la.put(img, head[0], head[1], TURKEY_HEAD)  # Head
    la.put(img, wattle[0], wattle[1], TURKEY_RED)  # Red wattle
    la.put(img, beak[0], beak[1], mm.VEST)  # Pale beak
    return la.outline(img)  # Dark edge


def snow_drift():
    """A 16x6 smooth snow drift with a sharp wind-cut crest and blue shade on the right."""  # Describes the prop
    img = mm.new_image(16, 6)  # Empty picture
    for x in range(16):  # Each column
        top = 5 - int(round(4 * (x / 11.0 if x <= 11 else (15 - x) / 4.0)))  # Long gentle slope, steep lee side
        for y in range(max(1, top), 6):  # Rows of the drift in this column
            col = SNW if x <= 11 else SNW_DK  # Sunny slope, shaded lee side
            if y > 3 and x <= 11:  # Soft shade near the foot
                col = SNW_SH  # Pale shade
            la.put(img, x, y, col)  # Paint it
    return la.outline(img)  # Dark edge so white snow reads on white ground


def snow_petrel(frame):
    """A 12x8 pure white snow petrel flying right with a black eye and beak; frame 0 wings up, frame 1 wings down."""  # Describes the prop
    img = mm.new_image(12, 8)  # Empty picture
    mm.fill_rect(img, 3, 4, 6, 2, mm.WHITE)  # Body
    mm.fill_rect(img, 3, 5, 6, 1, lm.SNOW)  # Belly in soft shade
    mm.fill_rect(img, 9, 3, 2, 2, mm.WHITE)  # Round head
    la.put(img, 10, 3, mm.OUTLINE)  # Black eye
    la.put(img, 11, 4, mm.OUTLINE)  # Black beak
    for (x, y) in ((1, 4), (2, 4), (1, 5), (2, 5)):  # Short tail
        la.put(img, x, y, lm.SNOW)  # Tail pixel
    if frame == 0:  # Wings up
        wing = ((4, 3), (5, 3), (6, 3), (5, 2), (6, 2), (4, 1), (5, 1), (3, 0), (4, 0))  # Wing pixels
    else:  # Wings down
        wing = ((4, 6), (5, 6), (6, 6), (4, 7), (5, 7), (3, 7))  # Wing pixels
    for (x, y) in wing:  # Each wing pixel
        la.put(img, x, y, lm.SNOW)  # White wing
    return la.outline(img)  # Dark edge so the white bird reads on snow and sky


def millet(frame):
    """A 10x14 clump of millet: thin green stalks with drooping golden seed heads; frame 1 sways the heads right."""  # Describes the prop
    img = mm.new_image(10, 14)  # Empty picture
    for (x0, top) in ((2, 4), (5, 2), (8, 5)):  # Three stalks: foot column and top row
        for y in range(top, 14):  # Up the stalk
            sway = frame if y < top + 4 else 0  # The top sways in frame 1
            la.put(img, x0 + sway, y, CROP if y > 9 else CROP_SH)  # Green stalk
        for k in range(4):  # The drooping seed head
            la.put(img, x0 + frame - 1 + (k > 1), top - 1 + k, mm.HELMET_SH if k % 2 else DRYG)  # One seed pixel
    for (x, y) in ((1, 10), (6, 9), (8, 11)):  # Leaves on the stalks
        la.put(img, x, y, CROP)  # One leaf
    return img  # No outline, like the other plants


def terrace_step():
    """A 24x10 small loess terrace: two flat steps with green crop lines and dusty faces."""  # Describes the prop
    img = mm.new_image(24, 10)  # Empty picture
    for (x0, y0) in ((0, 5), (8, 1)):  # Lower wide step and upper step
        mm.fill_rect(img, x0, y0, 24 - x0, 10 - y0, LO)  # Loess body of the step
        mm.fill_rect(img, x0, y0, 24 - x0, 1, CROP)  # Green crop line on the step
        mm.fill_rect(img, x0, y0 + 1, 24 - x0, 1, LO_LT)  # Sunny flat top
        for x in range(x0 + 2, 24, 5):  # Upright dust streaks on the face
            mm.fill_rect(img, x, y0 + 2, 1, 2, LO_SH)  # One streak
    mm.fill_rect(img, 0, 9, 24, 1, LO_SH)  # Shaded foot
    return la.outline(img)  # Dark edge


def alpine_cushion():
    """A 12x6 cushion plant: a tight round green mound dotted with tiny white and pink flowers."""  # Describes the prop
    img = mm.new_image(12, 6)  # Empty picture
    ph.ell(img, 5.5, 5.5, 5.5, 4.5, mm.HILLS_SH, mm.LEAF)  # Round mound, shaded lower right
    for (x, y, col) in ((3, 2, mm.WHITE), (6, 1, mix(mm.SCARF, mm.WHITE)), (8, 3, mm.WHITE), (2, 4, mix(mm.SCARF, mm.WHITE)), (5, 3, mm.WHITE)):  # Tiny flowers
        la.put(img, x, y, col)  # One flower
    return img  # No outline, like the other plants


def bharal(frame):
    """A 16x12 bharal (blue sheep) facing right, blue-grey with a white belly, black flank stripe and curved horns; frame 1 grazes."""  # Describes the prop
    img = mm.new_image(16, 12)  # Empty picture
    for x in (3, 5, 10, 12):  # Four legs
        mm.fill_rect(img, x, 7, 1, 5, mm.WHITE if x in (3, 10) else BHARAL_SH)  # Pale front edge, darker back legs
        la.put(img, x, 11, mm.OUTLINE)  # Hoof
    ph.ell(img, 7.5, 5, 5.5, 2.5, BHARAL, BHARAL_SH)  # Body
    mm.fill_rect(img, 4, 7, 8, 1, mm.WHITE)  # White belly
    mm.fill_rect(img, 3, 6, 10, 1, mm.OUTLINE)  # Black stripe along the flank
    la.put(img, 2, 4, BHARAL_SH)  # Short tail
    hy = 2 if frame == 0 else 7  # Head up, or head down to graze
    for (x, y) in ((12, 4), (13, hy + 1 if frame == 0 else 6)):  # Neck
        la.put(img, x, y, BHARAL)  # One neck pixel
    mm.fill_rect(img, 13, hy, 2, 2, BHARAL)  # Head
    la.put(img, 15, hy + 1, BHARAL_SH)  # Muzzle
    la.put(img, 14, hy, mm.OUTLINE)  # Eye
    for (x, y) in ((13, hy - 1), (12, hy - 1), (11, hy), (11, hy + 1)):  # Horn curving back and down
        la.put(img, x, y, BHARAL_HORN)  # One horn pixel
    return img  # Clean shape without an outline, like the dassie


# ---------- Items (24x24, Assets/Art/Items/Sprites/) ----------

def coal():
    """A 24x24 pair of black shiny coal lumps with flat broken faces and blue sheen."""  # Describes the item
    def c(x, y, s):  # Colour of one pixel
        if s:  # Lower right, in shade
            return COAL  # Black
        if (x + 2 * y) % 9 == 0:  # Edges of the broken faces
            return COAL_SHEEN  # Blue sheen line
        return COAL_MID if (x // 4 + y // 4) % 2 else COAL  # Flat faces, some lit
    img = na.blob(24, 24, 13.5, 13.5, 9.0, 7.5, c, 2.6)  # The big lump
    small = na.blob(24, 24, 5.5, 17.5, 4.5, 3.5, c, 2.4)  # A small lump in front, left
    mm.paste(img, small, 0, 0)  # Put the small lump on
    for (x, y) in ((10, 8), (11, 8), (15, 9), (12, 11), (4, 15), (18, 12)):  # Bright glassy shine
        la.put(img, x, y, mm.WHITE if x % 2 else HAMMER)  # One shine pixel
    return ls.outline(img)  # Dark edge


def meteorite():
    """A 24x24 dark brown-black meteorite with thumb-print dents, a thin black fusion crust and a few metal flecks."""  # Describes the item
    def c(x, y, s):  # Colour of one pixel
        if ((x - 12) / 9.5) ** 2 + ((y - 12.5) / 8.0) ** 2 > 0.78:  # The thin outer crust
            return CRUST  # Black fusion crust
        if ((x - 9) ** 2 + (y - 10) ** 2) < 5 or ((x - 15) ** 2 + (y - 14) ** 2) < 4:  # Thumb-print dents
            return METEOR_SH  # Dark dent
        return METEOR_SH if s else METEOR  # Brown-black stone
    img = na.blob(24, 24, 12.0, 12.5, 10.0, 8.5, c, 2.2)  # Rounded lump
    for (x, y) in ((12, 8), (17, 11), (7, 14), (13, 17)):  # Metal flecks
        la.put(img, x, y, HAMMER)  # One fleck
    for (x, y) in ((6, 8), (8, 6), (10, 5)):  # Shine on the crust, top left
        la.put(img, x, y, mm.MASK_SH)  # One shine pixel
    return ls.outline(img)  # Dark edge


def opal():
    """A 24x24 milky white opal with small rainbow flashes inside."""  # Describes the item
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x // 2, y // 2) < 38 and 4 < x < 19 and 6 < y < 19:  # A small patch of colour
            return OPAL_FLASH[(x // 3 + y // 2) % len(OPAL_FLASH)]  # Rainbow colour picked by place
        return OPAL_SH if s else OPAL  # Milky white, grey in shade
    img = na.blob(24, 24, 11.5, 12.5, 9.5, 7.5, c, 2.0)  # Smooth oval stone
    for (x, y) in ((7, 8), (8, 7), (9, 7)):  # Bright shine top left
        la.put(img, x, y, mm.WHITE)  # One shine pixel
    return ls.outline(img)  # Dark edge


# ---------- Lesson pictures (120x82, Assets/Art/Lesson/Sprites/) ----------

DECCAN_HILLS = [(28, 30, 10, 6, 4, True), (96, 26, 12, 6, 4, True), (62, 36, 14, 7, 5, False)]  # Hills: middle x, top row, half top width, step height, step widening, far
FAR_LIT, FAR_SHADE, FAR_BENCH = mix(PANTS, lm.HAZE), mix(mm.DARK, lm.HAZE_SH), mix(mm.VEST, lm.HAZE)  # Dark basalt and dry grass seen through the haze on far hills


def deccan_pixel(x, y):
    """Colour of one pixel of the Deccan picture: sky, flat-topped hills built of lava flows in steps, red soil and dry grass."""  # Describes the picture
    if y >= 68:  # Dry grass in front
        if y == 68:  # Sunny edge of the grass
            return DRYG_LT  # Light line
        return DRYG_SH if lm.noise(x, y) < 25 else DRYG  # Dry grass with tufts
    for (cx, top, half, step, widen, far) in reversed(DECCAN_HILLS):  # Each hill, nearest first
        lift = lm.noise(x // 3 + cx, 7) % 2  # Step edges rise and fall a little along the hill
        if y < top + lift:  # Above this hill
            continue  # Try the next one
        k = (y - top - lift) // step  # Which lava layer this row is in
        r = (y - top - lift) % step  # Row inside the layer
        ragged = lm.noise(k + cx, y // 2) % 3 - 1  # Uneven, crumbly side of the layer
        if abs(x + 0.5 - cx) <= half + k * widen + ragged:  # Inside the hill
            if far:  # A far hill in the haze
                if r == 0 and lm.noise(x, k) < 70:  # Patchy grass on the soft step edge
                    return FAR_BENCH  # Hazy dry grass
                return FAR_LIT if x < cx + (lm.noise(k, cx) % 5) else FAR_SHADE  # Hazy dark basalt, lit left and shaded right
            if r == 0:  # The flat grassy bench on top of the layer
                return DRYG if lm.noise(x, k) < 75 else DRYG_SH  # Patchy dry grass
            face = BAS_LT if x < cx else BAS  # Lit left face, shaded right face
            if x % 4 == 0 and r > 1 and lm.noise(x, k) < 60:  # A few upright joints in the lava
                face = BAS_SH  # Dark joint
            return face  # Dark basalt face
    if y >= 60:  # Red soil fields between the hills
        return RED_SOIL if (x // 9 + y // 3) % 2 else RED_SOIL_LT  # Strips of ploughed red soil
    return na.sky(x, y)  # Sky


def lesson_deccan():
    """A 120x82 framed picture of flat-topped hills built of dark lava flows stacked like steps, over red soil."""  # Describes the lesson
    img = ls.framed(deccan_pixel)  # Sky, stepped hills, soil and grass
    mm.paste(img, dry_teak(), 6, 54)  # A dry teak tree in front, left
    return img  # The finished picture


ETH_TOP = 30  # Row of the flat green plateau top


def ethiopia_pixel(x, y):
    """Colour of one pixel of the Ethiopia picture: sky, a far peak, a high green plateau ending in a sheer cliff and hazy lowlands."""  # Describes the picture
    edge = 82 + (y - ETH_TOP) * 0.15 + (2 if (y // 6) % 2 else 0)  # The cliff edge, a little stepped
    if y >= ETH_TOP and x <= edge:  # The plateau
        if y < ETH_TOP + 2:  # Sunny top edge
            return GRASS_LT  # Light grass line
        if x >= edge - 3:  # The cliff face at the edge
            return SIM_LT if (x + y) % 4 else SIM  # Dark basalt columns
        if lm.noise(x, y) < 3:  # Small yellow daisies
            return mm.HELMET  # Flower
        return GRASS if y < 60 or lm.noise(x, y) > 30 else GRASS_SH  # Green highland grass
    if y >= ETH_TOP and x > edge:  # Beyond the cliff
        if x <= edge + 6 + (y - ETH_TOP) * 0.6 and y < 70:  # The cliff wall falling away
            return SIM if x % 3 else SIM_SH  # Dark basalt in shade
        if y >= 56:  # The far lowlands
            return mix(GRASS_SH, lm.HAZE)  # Hazy green land
        return lm.HAZE_SH if y >= 50 + (x % 7 == 0) else na.sky(x, y)  # Haze, then sky
    top, peak = lm.ridge(x, [(34, 14, 0.8), (60, 22, 0.6)])  # A far high peak behind the plateau
    if y >= top:  # Inside the far peak
        return lm.HAZE if x <= peak[0] else lm.HAZE_SH  # Lit or shaded haze
    return na.sky(x, y)  # Sky


def lesson_ethiopia():
    """A 120x82 framed picture of a high green plateau ending in a sheer cliff, with geladas on the grass."""  # Describes the lesson
    img = ls.framed(ethiopia_pixel)  # Sky, plateau, cliff and lowlands
    mm.paste(img, gelada(0), 10, 50)  # A gelada on the grass, left
    mm.paste(img, gelada(2), 42, 60)  # A gelada on the grass, nearer
    mm.paste(img, giant_lobelia(), 70, 34)  # A giant lobelia near the edge
    return img  # The finished picture


def tassili_pixel(x, y):
    """Colour of one pixel of the Tassili picture: sky, a sandstone arch and two mushroom-shaped pillars on golden sand."""  # Describes the picture
    if y >= 62:  # The sand
        if (y + la.wave(x, 1, 20)) % 5 == 0:  # Ripples in the sand
            return SAND_SH  # Darker ripple
        return SAND if y < 72 else SAND_LT  # Golden sand, sunnier in front
    arch = 14 <= x <= 62 and 20 <= y  # The block of rock that holds the arch
    hole = ((x - 38) / 14.0) ** 2 + ((y - 62) / 30.0) ** 2 <= 1.0  # The opening under the arch
    if arch and not hole and (y >= 22 or 18 <= x <= 58):  # The arch rock, rounded at the top corners
        col = TAS if x < 50 else TAS_SH  # Lit left, shaded right
        if (y - 20) % 7 == 0:  # Beds across the rock
            col = TAS_LT if x < 50 else TAS  # Sunny bed
        return col  # The arch
    for (cx, top, half) in ((82, 30, 5), (102, 40, 4)):  # Two pillars: middle, top row, half width of the stem
        cap = top <= y < top + 6 and abs(x + 0.5 - cx) <= half + 3  # Wide cap on top of the pillar
        stem = y >= top + 6 and abs(x + 0.5 - cx) <= half - (1 if top + 8 <= y <= top + 14 else 0)  # Stem, thin under the cap
        if cap or stem:  # Inside the pillar
            col = TAS_LT if x < cx else TAS_SH  # Lit left, shaded right
            if cap and y == top:  # Top of the cap
                col = TAS_LT  # Sunny top
            return col  # The pillar
    return na.sky(x, y)  # Sky


def lesson_tassili():
    """A 120x82 framed picture of a natural sandstone arch and mushroom-shaped rock pillars standing on sand."""  # Describes the lesson
    img = ls.framed(tassili_pixel)  # Sky, arch, pillars and sand
    mm.paste(img, saharan_cypress(), 104, 58)  # A Saharan cypress at the right
    return img  # The finished picture


MV_ALCOVE = (60, 52, 46, 28)  # Alcove: middle x, floor row, half width, height


def mesa_verde_pixel(x, y):
    """Colour of one pixel of the Mesa Verde picture: sky, junipers on the mesa top, a tall cliff with a deep alcove of stone houses."""  # Describes the picture
    if y < 14:  # Above the mesa top
        if y >= 10 and lm.noise(x // 3, y // 2) < 45:  # Juniper crowns on the rim
            return JUN if y < 12 else JUN_SH  # Green juniper
        return na.sky(x, y)  # Sky
    cx, floor, half, high = MV_ALCOVE  # Where the alcove is
    if floor - high <= y < floor and ((x - cx) / half) ** 2 + ((y - floor) / high) ** 2 <= 1.0:  # Inside the alcove
        for (x0, x1, top) in ((22, 34, 42), (34, 44, 38), (52, 58, 30), (60, 74, 40), (74, 84, 44), (84, 94, 41)):  # Stone houses and a tower
            if x0 <= x < x1 and y >= top:  # Inside this house
                if (y - top) % 3 == 2:  # Lines between stone courses
                    return MV_SH  # Joint
                if x1 - x0 > 8 and y == top + 3 and (x - x0) in (3, 4):  # A small dark window
                    return mm.OUTLINE  # Window
                if x0 == 52 and y in (34, 40) and x in (54, 55):  # Windows of the tower
                    return mm.OUTLINE  # Window
                return MV_LT if x < x1 - 1 else MV  # Buff stone, shaded right edge
        return ALCOVE  # Deep shade
    if y >= 70:  # Rubble slope with junipers at the foot of the cliff
        return JUN_SH if lm.noise(x, y) < 22 else MV_SH  # Junipers on the slope
    col = MV if (y + la.wave(x, 1, 30)) % 7 else MV_LT  # Buff cliff with sunny beds
    if x % 13 == 5 and y > 16:  # Dark streaks down the cliff
        col = MV_DK  # Streak
    return col  # The cliff


def lesson_mesa_verde():
    """A 120x82 framed picture of stone cliff houses built in a deep alcove under a sandstone overhang."""  # Describes the lesson
    return ls.framed(mesa_verde_pixel)  # Sky, cliff, alcove and houses


ANT_HORIZON = 30  # Row where the ice sheet meets the sky
METEORITES = [(46, 62, 2), (70, 66, 3), (58, 72, 2), (84, 63, 2), (36, 68, 2)]  # Dark stones on the blue ice: x, y, width


def antarctic_pixel(x, y):
    """Colour of one pixel of the Antarctic picture: sky, a dead-flat white ice sheet, a patch of blue ice with dark meteorites."""  # Describes the picture
    if y < ANT_HORIZON:  # Above the horizon
        if y >= ANT_HORIZON - 3 and abs(x - 96) < 6 - (ANT_HORIZON - y) * 2:  # A small far rock peak poking through the ice
            return lm.MOUNTAIN_SH  # Dark rock
        return na.sky(x, y)  # Sky
    if y == ANT_HORIZON:  # The thin far edge of the ice
        return SNW_DK  # Pale blue line
    for (mx, my, w) in METEORITES:  # Each meteorite
        if my <= y <= my + 1 and mx <= x < mx + w:  # Inside the small dark stone
            return METEOR if y == my else mm.OUTLINE  # Dark stone with a black lower edge
    if ((x - 60) / 44.0) ** 2 + ((y - 67) / 10.0) ** 2 <= 1.0:  # The patch of wind-polished blue ice
        if (x + 3 * y) % 17 == 0:  # Polished streaks
            return mm.SKY_LIGHT  # Pale streak
        return BLUE_ICE  # Blue ice
    if (y - ANT_HORIZON) % 6 == 3 and (x + y) % 9 < 3:  # Low wind ridges on the snow
        return SNW_DK  # Faint blue ridge
    return SNW if y > 44 else SNW_SH  # White snow, a little shaded far away


def lesson_antarctic():
    """A 120x82 framed picture of the flat white Antarctic ice sheet, with a patch of blue ice where dark meteorites lie."""  # Describes the lesson
    return ls.framed(antarctic_pixel)  # Sky, ice sheet, blue ice and meteorites


LOESS_CLIFF = 62  # Row where the loess cliff with the cave homes starts


def loess_pixel(x, y):
    """Colour of one pixel of the Loess picture: sky, a yellow hill cut into curved terraces, and a loess cliff with cave homes."""  # Describes the picture
    if y >= LOESS_CLIFF:  # The loess cliff in front
        if y >= 78:  # Yard in front of the homes
            return LO_SH  # Packed earth
        for cx in (18, 44, 70, 96):  # Middles of the cave home doors
            if abs(x + 0.5 - cx) <= 5 and (y >= 68 or (x + 0.5 - cx) ** 2 + (y - 68) ** 2 <= 25):  # Inside the round-topped opening
                if y >= 72 and abs(x + 0.5 - cx) <= 2:  # The door
                    return WOOD_SH  # Wooden door
                return WOOD_SH if (x + y) % 3 == 0 else PAPER  # Lattice over pale paper
        return LO_SH if x % 7 == 3 else LO  # Loess cliff with upright streaks
    top = 16 + int(6 * math.sin(x / 19.0))  # The rounded top of the hill
    if y < top:  # Above the hill
        return na.sky(x, y)  # Sky
    k = (y + int(round(3 * math.sin(x / 11.0)))) % 6  # Row inside a curved terrace step
    if k == 0:  # The green crop line on each step
        return CROP  # Green crops
    if k == 1:  # The sunny flat step
        return LO_LT  # Light loess
    return LO if x % 5 else LO_SH  # Loess face with streaks


def lesson_loess():
    """A 120x82 framed picture of a yellow loess hill cut into curved terraces, with cave homes dug into the cliff below."""  # Describes the lesson
    return ls.framed(loess_pixel)  # Sky, terraces and cave homes


TIBET_PEAKS = [(16, 18, 0.9), (48, 12, 1.0), (84, 20, 0.8), (110, 14, 0.9)]  # Snowy peaks: x, top row, slope
TIBET_LAND = 36  # Row where the flat plateau starts


def tibet_pixel(x, y):
    """Colour of one pixel of the Tibet picture: sky, snowy peaks, a turquoise salt lake with a white shore and brown alpine grass."""  # Describes the picture
    if y < TIBET_LAND:  # Above the plateau
        top, peak = lm.ridge(x, TIBET_PEAKS)  # The snowy peaks
        if y >= top:  # Inside a peak
            if y < top + 5 + (x % 3):  # Snow on the top
                return lm.SNOW if x <= peak[0] else lm.HAZE  # Snow, shaded on the right
            return lm.MOUNTAIN if x <= peak[0] else lm.MOUNTAIN_SH  # Rock
        return na.sky(x, y)  # Sky
    d = ((x - 60) / 44.0) ** 2 + ((y - 46) / 7.0) ** 2  # Distance from the middle of the lake
    if d <= 1.0:  # The lake
        if d > 0.82:  # The white salt shore
            return mm.WHITE  # Salt
        return LAKE if (x + y) % 11 else mm.SKY_LIGHT  # Turquoise water with glints
    if y < TIBET_LAND + 3:  # Far grass in the haze
        return mix(mm.VEST, lm.HAZE)  # Hazy brown grass
    if lm.noise(x, y) < 6:  # Grey rocks in the grass
        return RK  # Rock
    return TG if y < 62 or lm.noise(x, y) > 35 else TG_SH  # Brown alpine grass


def lesson_tibet():
    """A 120x82 framed picture of a very high flat plateau with snowy peaks, a turquoise salt lake and a yak."""  # Describes the lesson
    img = ls.framed(tibet_pixel)  # Sky, peaks, lake and grass
    mm.paste(img, yak(0), 76, 60)  # A yak on the grass
    return img  # The finished picture


# ---------- Completion Screen plateau, map flag and ribbon ----------

COMP_W, COMP_H = 960, 90  # Same width as the start-up background layers, one third of their 270-pixel height
COMP_TOP = 4  # Row of the flat top where the explorer's feet go
COMP_BANDS = (mm.PLATEAU, mm.PLATEAU_SH)  # The two clay band colours, like the start-up plateaus


def completion_plateau():
    """A 960x90 plateau top for the Completion Screen: grass tufts, a flat green top on row 4 and a layered clay cliff below, in the start-up style."""  # Describes the picture
    img = mm.new_image(COMP_W, COMP_H)  # Empty picture
    for y in range(COMP_TOP, COMP_H):  # Each row of the plateau
        for x in range(COMP_W):  # Each column
            if y == COMP_TOP:  # Sunny top line of the grass
                col = mm.PLAINS  # Light green
            elif y < COMP_TOP + 3:  # The grass cap
                col = mm.HILLS  # Green
            elif y == COMP_TOP + 3:  # Shade under the grass
                col = mm.HILLS_SH if (x // 3) % 4 else mm.HILLS  # Grass edge with small hanging tufts
            else:  # The layered clay cliff
                band = (y - COMP_TOP - 4 + la.wave(x, 1, 120)) // 6  # Which band this row is in
                col = COMP_BANDS[band % 2]  # Light or dark clay band
                if (y - COMP_TOP - 4 + la.wave(x, 1, 120)) % 6 == 0 and band % 2 == 0:  # Sunny top line of a light band
                    col = mix(mm.PLATEAU, mm.HELMET)  # Light clay
                if y > COMP_H - 14 and (x + y) % 2 == 0:  # Checker shade near the bottom, like the start-up art
                    col = mm.BACKPACK_SH if col == mm.PLATEAU_SH else mm.PLATEAU_SH  # Darker clay
                if lm.noise(x, y) < 2:  # A few small stones
                    col = mm.BACKPACK_SH  # Dark speck
            la.put(img, x, y, col)  # Paint it
    for x in range(2, COMP_W, 7):  # Grass tufts on the top
        h = 1 + lm.noise(x, 7) % 3  # Height of this tuft
        for t in range(h):  # Up the tuft
            la.put(img, x, COMP_TOP - 1 - t, mm.HILLS if t == 0 else mm.PLAINS)  # One tuft pixel
    for x in range(20, COMP_W, 37):  # Cracks down the cliff
        for y in range(COMP_TOP + 6, COMP_TOP + 6 + 4 + lm.noise(x, 9) % 8):  # Down the crack
            la.put(img, x + (y // 5) % 2, y, mm.BACKPACK_SH)  # One crack pixel
    return img  # The finished picture


def champion_flag(frame):
    """A 7x10 small golden flag on a pole for the Landform Map; frame 0 and frame 1 wave the cloth."""  # Describes the flag
    img = mm.new_image(7, 10)  # Empty picture, the same height as the map waterfall
    mm.fill_rect(img, 0, 1, 1, 9, mm.STRAP)  # Pole
    la.put(img, 0, 0, GOLD_LT)  # Gold knob on the pole top
    for x in range(1, 7):  # Columns of the cloth
        dip = (0, 0, 1, 1, 0, 0)[x - 1] if frame == 0 else (0, 1, 1, 0, 0, 1)[x - 1]  # Wave of the cloth in this frame
        for y in range(1 + dip, 5 + dip):  # Rows of the cloth in this column
            col = GOLD if y < 4 + dip else GOLD_SH  # Gold, shaded at the lower edge
            if y == 1 + dip and x < 4:  # Light along the top near the pole
                col = GOLD_LT  # Bright gold
            la.put(img, x, y, col)  # Paint it
    return img  # The finished picture


RIBBON_W, RIBBON_H, RIBBON_NOTCH = 188, 24, 8  # Room for "All landforms explored!" in Micro 5 at 22 px (about 158 x 12), plus the notched ends


def champion_ribbon():
    """A 188x24 empty gold ribbon banner with notched ends and a thin light line inside, for one line of text."""  # Describes the ribbon
    w, h = RIBBON_W, RIBBON_H  # Banner size
    img = mm.new_image(w, h)  # Empty banner
    face = set()  # Pixels of the gold banner
    for y in range(1, h - 1):  # Each row inside the border
        for x in range(1, w - 1):  # Each column inside the border
            e = min(x, w - 1 - x)  # Distance from the nearest end
            if e < RIBBON_NOTCH and abs(y - (h - 1) / 2) < RIBBON_NOTCH - e:  # Inside the sideways V notch
                continue  # Leave it empty
            face.add((x, y))  # Keep the pixel
    for (x, y) in face:  # Outline pass
        for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):  # Four neighbours
            if (x + dx, y + dy) not in face:  # Edge of the banner
                la.put(img, x + dx, y + dy, mm.OUTLINE)  # Dark outline pixel
    for (x, y) in face:  # Fill pass
        col = GOLD  # Gold face
        if y <= 2:  # Light top band
            col = GOLD_LT  # Bright gold
        elif y >= h - 4:  # Dark bottom band
            col = GOLD_SH  # Darker gold
        la.put(img, x, y, col)  # Paint it
    for x in range(RIBBON_NOTCH + 4, w - RIBBON_NOTCH - 4):  # A thin light stitch line near the top and bottom
        if x % 2 == 0:  # Dashed, like stitches
            la.put(img, x, 4, mm.HEADLAMP)  # Upper stitch
            la.put(img, x, h - 6, GOLD_SH)  # Lower stitch
    return img  # The finished banner


# ---------- Saving (never overwrites) and the preview sheet ----------

def png_bytes(img):
    """The PNG file bytes of a picture, so a new file can be compared with one already on disk."""  # Describes the helper
    buf = io.BytesIO()  # A file in memory
    img.save(buf, "PNG")  # Write the PNG into it
    return buf.getvalue()  # The bytes


def folder_for(root, where):
    """The folder for each kind of picture under the given project root."""  # Describes the helper
    art = os.path.join(root, "Assets", "Art")  # The art folder
    return {  # One folder per kind
        "level": os.path.join(art, "Level", "Sprites"),  # Level sprites and props
        "item": os.path.join(art, "Items", "Sprites"),  # Shared items
        "lesson": os.path.join(art, "Lesson", "Sprites"),  # Lesson pictures
        "map": os.path.join(art, "LandformMap", "Sprites", "Plateaus"),  # Plateaus life sprites on the map
        "label": os.path.join(art, "LandformMap", "Sprites", "Labels"),  # Map labels and banners
        "completion": os.path.join(art, "Completion", "Sprites"),  # Completion Screen art
        "preview": os.path.join(root, ".scratch", "plateaus-hard"),  # The preview sheet
    }[where]  # The folder for this kind


def png_names(root):
    """Every PNG name under Assets/Art, with the folders where it is found, to spot a name clash anywhere."""  # Describes the helper
    found = {}  # Name: list of full paths
    for folder, _dirs, files in os.walk(os.path.join(root, "Assets", "Art")):  # Each folder under the art folder
        for f in files:  # Each file in it
            if f.lower().endswith(".png"):  # Only pictures
                found.setdefault(f.lower(), []).append(os.path.normcase(os.path.join(folder, f)))  # Remember where it is
    return found  # The name list


def write_all(root, files):
    """Writes every (path, bytes) pair. It first checks them all and stops, writing nothing, when a name is taken by other pixels."""  # Describes the helper
    names = png_names(root)  # PNG names already in the art folders
    clashes, todo = [], []  # Name clashes, and the files still to write
    for path, data in files:  # Each planned file
        same_name = [p for p in names.get(os.path.basename(path).lower(), []) if p != os.path.normcase(path)]  # The same name in another folder
        if same_name:  # The name is used somewhere else
            clashes.append("%s (also at %s)" % (path, same_name[0]))  # Note the clash
        elif os.path.exists(path):  # A file is already at this path
            with open(path, "rb") as f:  # Open it
                if f.read() != data:  # Its pixels are different
                    clashes.append("%s (already there with other pixels)" % path)  # Note the clash, never overwrite
        else:  # A new file
            todo.append((path, data))  # Write it later
    if clashes:  # Some names are taken
        print("Stopped, nothing written. Pick new names for:")  # Tell the person running it
        for c in clashes:  # Each clash
            print("  " + c)  # One clash
        sys.exit(1)  # Stop with an error
    for path, data in todo:  # Each new file
        os.makedirs(os.path.dirname(path), exist_ok=True)  # Make the folder if it is missing
        with open(path, "wb") as f:  # Open the new file
            f.write(data)  # Write the PNG bytes
    print("Wrote %d new files, %d were already there and the same." % (len(todo), len(files) - len(todo)))  # Tell what happened


def big(img, scale):
    """Makes a crisp, larger copy of a picture for the preview."""  # Describes the helper
    return img.resize((img.width * scale, img.height * scale), Image.NEAREST)  # Nearest-neighbour scaling


def on_sky(img):
    """Puts a picture on a plain sky block, only for the preview, so pale things show up."""  # Describes the helper
    back = Image.new("RGBA", img.size, lm.SKY + (255,))  # Sky block the same size
    back.alpha_composite(img)  # The picture on top
    return back  # The picture on sky


def tiled(tile, across, down):
    """Repeats a tile across and down, only for the preview, to check that it joins with no seam."""  # Describes the check
    img = mm.new_image(tile.width * across, tile.height * down)  # Room for the copies
    for j in range(down):  # Each row of copies
        for i in range(across):  # Each copy in the row
            mm.paste(img, tile, i * tile.width, j * tile.height)  # One copy
    return img  # The tiled picture


def ground_check(top, soil):
    """A strip of ground: a row of top tiles over two rows of soil tiles, only for the preview."""  # Describes the check
    img = mm.new_image(64, 40)  # Four tiles wide
    mm.paste(img, tiled(top, 4, 1), 0, 0)  # The top row
    mm.paste(img, tiled(soil, 4, 2), 0, 8)  # Two rows of soil
    return img  # The strip


def explorer_check():
    """A slice of the completion plateau with the start-up explorer standing on its top row, only for the preview."""  # Describes the check
    img = mm.new_image(160, COMP_H + 32)  # Room above the plateau for the explorer
    mm.paste(img, completion_plateau().crop((0, 0, 160, COMP_H)), 0, 32)  # The left part of the plateau
    img = on_sky(img)  # On sky
    path = os.path.join(mm.START, "Explorer", "explorer_00.png")  # The explorer from the start-up screen
    if os.path.exists(path):  # Only when the project art is there
        mm.paste(img, mm.load(os.path.join("Explorer", "explorer_00.png")), 60, 32 + COMP_TOP - 31)  # Feet (row 30 of the explorer) just above the flat top
    return img  # The check picture


def preview_sheet(groups):
    """Lays the new sprites out at 3x size on one neutral grey sheet, one labelled group after another."""  # Describes the sheet
    scale, pad = 3, 6  # Size up and gap
    width = max(1200, max(img.width for _t, s in groups for img in s) * scale + 2 * pad)  # Wide enough for the widest sprite
    ops = []  # Each (picture, x, y) to paste
    y = pad  # Where the next row goes
    for title, sprites in groups:  # Each group
        label = mm.text_image(title, mm.DARK, 1)  # Group name in the pixel font
        ops.append((big(label, scale), pad, y))  # Label at the top of the group
        y += label.height * scale + pad  # Move below the label
        x, row_h = pad, 0  # Start a row
        for img in sprites:  # Each sprite in the group
            w, h = img.width * scale, img.height * scale  # Scaled size
            if x + w > width - pad:  # No room left in this row
                x, y, row_h = pad, y + row_h + pad, 0  # Start a new row
            ops.append((big(img, scale), x, y))  # Place the sprite
            x, row_h = x + w + pad, max(row_h, h)  # Move along
        y += row_h + 2 * pad  # Space before the next group
    sheet = Image.new("RGBA", (width, y + pad), mm.MASK + (255,))  # Neutral grey background
    for img, px, py in ops:  # Paste each one
        sheet.alpha_composite(img, (px, py))  # Place it
    return sheet  # The finished sheet


PALE = ("hotspring_steam", "gust_snow", "snow_petrel", "lammergeier", "champion_flag")  # Names shown on sky in the preview


def main():
    """Draws everything, checks every name, saves the new pictures and writes the preview sheet."""  # Describes the main function
    root = mm.ROOT  # The Unity project folder by default
    if "--root" in sys.argv:  # Another folder was asked for
        root = os.path.abspath(sys.argv[sys.argv.index("--root") + 1])  # Write everything under it instead
    groups = [("H%d %s" % (i + 1, title), [  # Each theme group: its title, then (kind, file name, picture)
        ("level", "%s_top.png" % p, top()),  # Ground top
        ("level", "%s_soil.png" % p, soil()),  # Ground below the top
        ("level", "ledge_%s.png" % p, ledge()),  # Ledge
        ("level", "boulder_small_%s.png" % p, boulder(32, 24)),  # Small boulder
        ("level", "boulder_big_%s.png" % p, boulder(48, 36)),  # Big boulder
        ("level", "%s.png" % goal.__name__, goal()),  # Goal
    ]) for i, (title, p, top, soil, ledge, boulder, goal) in enumerate([  # The seven themes
        ("DECCAN TRAPS", "deccan", deccan_top, deccan_soil, ledge_deccan, deccan_boulder, hill_fort_gate),  # H1
        ("SIMIEN HIGHLANDS", "simien", simien_top, simien_soil, ledge_simien, simien_boulder, highland_village),  # H2
        ("SANDSTONE ARCHES", "tassili", tassili_top, tassili_soil, ledge_tassili, tassili_boulder, rock_art_shelter),  # H3
        ("CLIFF DWELLINGS", "mesaverde", mesaverde_top, mesaverde_soil, ledge_mesaverde, mesaverde_boulder, cliff_palace_ladder),  # H4
        ("ICE PLATEAU", "iceplateau", iceplateau_top, iceplateau_soil, ledge_iceplateau, iceplateau_boulder, research_station),  # H5
        ("LOESS TERRACES", "loess", loess_top, loess_soil, ledge_loess, loess_boulder, cave_house),  # H6
        ("ROOF OF THE WORLD", "tibet", tibet_top, tibet_soil, ledge_tibet, tibet_boulder, summit_camp),  # H7
    ])]
    groups += [  # The other groups
        ("OBJECTS", [  # New level objects
            ("level", "vent_hotspring.png", vent_hotspring()),  # Hot-spring pool vent
            ("level", "blue_ice_top.png", blue_ice_top()),  # Slippery blue ice top
            ("level", "loess_soft_top.png", loess_soft_top()),  # Soft loess top (new name, loess_top.png is the H6 ground top)
            ("level", "sandstone_slab.png", sandstone_slab()),  # Moving sandstone slab
        ] + [("level", "Props/hotspring_steam_warn_%02d.png" % i, hotspring_warn(i)) for i in range(2)]  # Warning wisps
          + [("level", "Props/hotspring_steam_%02d.png" % i, hotspring_steam(i)) for i in range(4)]  # Steam puffs
          + [("level", "Props/gust_sand_%02d.png" % i, gust_sand(i)) for i in range(4)]  # Sand gust streaks
          + [("level", "Props/gust_snow_%02d.png" % i, gust_snow(i)) for i in range(4)]),  # Snow gust streaks
        ("HERDS", [("level", "gelada_%02d.png" % i, gelada(i)) for i in range(3)]  # Gelada walk frames
          + [("level", "yak_%02d.png" % i, yak(i)) for i in range(3)]),  # Yak walk frames
        ("PLATEAUS HARD PROPS", [  # Decoration props
            ("level", "Props/dry_teak.png", dry_teak()),  # H1 dry teak tree
            ("level", "Props/langur_00.png", langur(0)),  # H1 langur sitting
            ("level", "Props/langur_01.png", langur(1)),  # H1 langur swinging its tail
            ("level", "Props/giant_lobelia.png", giant_lobelia()),  # H2 giant lobelia
            ("level", "Props/lammergeier_00.png", lammergeier(0)),  # H2 vulture, wings up
            ("level", "Props/lammergeier_01.png", lammergeier(1)),  # H2 vulture, wings down
            ("level", "Props/saharan_cypress.png", saharan_cypress()),  # H3 Saharan cypress
            ("level", "Props/sand_ripple.png", sand_ripple()),  # H3 sand ripple
            ("level", "Props/juniper_mesa.png", juniper_mesa()),  # H4 juniper
            ("level", "Props/cliff_ladder.png", cliff_ladder()),  # H4 ladder on a ledge
            ("level", "Props/wild_turkey_00.png", wild_turkey(0)),  # H4 turkey, head up
            ("level", "Props/wild_turkey_01.png", wild_turkey(1)),  # H4 turkey pecking
            ("level", "Props/snow_drift.png", snow_drift()),  # H5 snow drift
            ("level", "Props/snow_petrel_00.png", snow_petrel(0)),  # H5 petrel, wings up
            ("level", "Props/snow_petrel_01.png", snow_petrel(1)),  # H5 petrel, wings down
            ("level", "Props/millet_00.png", millet(0)),  # H6 millet
            ("level", "Props/millet_01.png", millet(1)),  # H6 millet swaying
            ("level", "Props/terrace_step.png", terrace_step()),  # H6 terrace step
            ("level", "Props/alpine_cushion.png", alpine_cushion()),  # H7 cushion plant
            ("level", "Props/bharal_00.png", bharal(0)),  # H7 blue sheep, head up
            ("level", "Props/bharal_01.png", bharal(1)),  # H7 blue sheep grazing
        ]),
        ("ITEMS", [  # New items
            ("item", "coal.png", coal()),  # Coal
            ("item", "meteorite.png", meteorite()),  # Meteorite
            ("item", "opal.png", opal()),  # Opal
        ]),
        ("LESSON PICTURES", [  # New lesson pictures
            ("lesson", "lesson_deccan.png", lesson_deccan()),  # Stepped lava hills
            ("lesson", "lesson_ethiopia.png", lesson_ethiopia()),  # Green plateau and geladas
            ("lesson", "lesson_tassili.png", lesson_tassili()),  # Sandstone arch and pillars
            ("lesson", "lesson_mesa_verde.png", lesson_mesa_verde()),  # Cliff houses in an alcove
            ("lesson", "lesson_antarctic.png", lesson_antarctic()),  # Ice sheet and meteorites
            ("lesson", "lesson_loess.png", lesson_loess()),  # Terraces and cave homes
            ("lesson", "lesson_tibet.png", lesson_tibet()),  # High plateau, peaks and salt lake
        ]),
        ("MAP FLAG AND RIBBON", [  # Landform Map art
            ("map", "champion_flag_0.png", champion_flag(0)),  # Golden flag, frame 0
            ("map", "champion_flag_1.png", champion_flag(1)),  # Golden flag, frame 1
            ("label", "champion_ribbon.png", champion_ribbon()),  # Empty gold ribbon
        ]),
        ("COMPLETION PLATEAU", [  # Completion Screen art
            ("completion", "completion_plateau.png", completion_plateau()),  # Foreground plateau top
        ]),
    ]
    files, preview_groups = [], []  # Files to write, and the preview groups
    for title, entries in groups:  # Each group
        sprites = []  # Pictures of this group
        for where, name, img in entries:  # Each picture
            files.append((os.path.join(folder_for(root, where), *name.split("/")), png_bytes(img)))  # Plan the file
            sprites.append(on_sky(img) if any(p in name for p in PALE) else img)  # Pale things and dark birds go on sky
        preview_groups.append((title, sprites))  # Add the group to the sheet
    preview_groups.append(("TILE CHECK", [ground_check(t(), s()) for t, s in (  # Seams of each ground, preview only
        (deccan_top, deccan_soil), (simien_top, simien_soil), (tassili_top, tassili_soil), (mesaverde_top, mesaverde_soil),  # H1 to H4
        (iceplateau_top, iceplateau_soil), (loess_top, loess_soil), (tibet_top, tibet_soil))]  # H5 to H7
        + [tiled(blue_ice_top(), 4, 1), tiled(loess_soft_top(), 4, 1), on_sky(tiled(champion_flag(0), 1, 1)), explorer_check()]))  # Special tops, flag and the explorer on the plateau
    files.append((os.path.join(folder_for(root, "preview"), "art-preview.png"), png_bytes(preview_sheet(preview_groups))))  # The preview sheet
    write_all(root, files)  # Check every name, then write
    print("Plateaus Hard art written under", root)  # Tell where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
