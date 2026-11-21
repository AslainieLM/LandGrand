"""Draws the PNGs for Plains Easy 1 to 3 (D-069): three level themes (Rice Plain, Salt Beds, River Plain),
the straw bounce pile, the Plains props, four new rocks and minerals, three lesson pictures and the
Plains life sprites for the Landform Map and the Plains menu screens.

Run from the Unity project folder:  python Tools/Art/make_plains_easy_art.py
It writes to Assets/Art/Level/Sprites/ (themes, straw pile and props), Assets/Art/Items/Sprites/ (items),
Assets/Art/Lesson/Sprites/ (lesson pictures) and Assets/Art/LandformMap/Sprites/Plains/ (life sprites),
plus a preview sheet at .scratch/plains-easy/art-preview.png.
It reuses the helpers of make_mainmenu_art.py, make_landform_map_art.py, make_level_art.py, make_lesson_art.py and make_normal_art.py.
All colours come from Assets/Art/Palettes/landgrand.gpl or mix two of its colours.
"""
import math  # math gives circles, waves and square roots
import os  # os joins folder paths
from PIL import Image  # Image builds the preview sheet
import make_mainmenu_art as mm  # Palette, new_image and fill_rect
import make_landform_map_art as lm  # Map colours, the fixed noise and the ridge helper
import make_level_art as la  # put, outline, wave and save for level sprites
import make_lesson_art as ls  # framed, disc, fill_poly, inside and the item and lesson folders
import make_normal_art as na  # mix, the shared tile makers and the item blob

mix = na.mix  # The colour halfway between two palette colours
MAP_OUT = os.path.join(mm.ROOT, "Assets", "Art", "LandformMap", "Sprites", "Plains")  # Where the Plains life sprites go
PREVIEW = os.path.join(mm.ROOT, ".scratch", "plains-easy", "art-preview.png")  # Where the preview sheet goes

# Theme colours (D-069), each a palette colour or a mix of two.
MUD = mix(mm.GROUND_SH, mm.BACKPACK_SH)  # Wet dark-brown clay of the paddy dikes
MUD_DK = mix(mm.GROUND_SH, mm.OUTLINE)  # Darkest wet clay, for specks
MUD_LT = mix(mm.GROUND, mm.GROUND_SH)  # Silt layer in the clay
RICE_GREEN = mm.HILLS  # Bright green rice
RICE_DK = mm.HILLS_SH  # Rice stalks in shade
RICE_TIP = mm.PLAINS  # Light tips of the rice
WATER = lm.LENS  # Paddy water shine
WATER_LT = mm.SKY_LIGHT  # Pale water shine
BROWN_GREY = mix(mm.GROUND, mm.MASK_SH)  # Brown-grey sandstone of the rice boulders
BROWN_GREY_SH = mix(mm.GROUND_SH, mm.MASK_SH)  # Brown-grey sandstone in shade
BROWN_GREY_LT = mix(mm.MASK, mm.GROUND)  # Sunny brown-grey sandstone
THATCH = mix(mm.VEST, mm.HELMET_SH)  # Nipa and grass roofs
THATCH_SH = mm.VEST_SH  # Roof in shade
WOOD = lm.HANDLE  # Light wood
WOOD_SH = mm.GROUND_SH  # Wood in shade
SACK = mm.VEST  # Woven rice sack
SACK_SH = mm.VEST_SH  # Sack in shade
SALT_SOIL = mix(mm.MASK_SH, mm.VEST_SH)  # Grey sandy mud of the salt beds
SALT_SOIL_DK = mix(mm.MASK_SH, mm.STRAP)  # Dark specks in the grey mud
SALT_WHITE = mm.WHITE  # Salt crust
BRINE = lm.LENS  # Sea and brine blue
BRINE_LT = mm.SKY_LIGHT  # Shallow brine shine
SEA_DEEP = mix(lm.LENS, lm.MOUNTAIN_SH)  # Deep sea blue
BAMBOO = mm.PLAINS  # Pale bamboo poles
BAMBOO_SH = lm.PLAINS_SH  # Bamboo in shade
RIVER_SOIL = mix(mm.GROUND, mm.GROUND_SH)  # Rich brown alluvial soil
RIVER_DK = mix(mm.GROUND_SH, mm.OUTLINE)  # Dark soil specks
RIVER_WATER = mix(mm.GROUND, mm.HILLS_SH)  # Calm brown-green river water
SILT_RIV = mm.VEST_SH  # Silt layer in the river soil
SAND_RIV = mm.VEST  # Sand layer in the river soil
COBBLE_LT = mm.MASK  # Sunny grey river cobble
COBBLE_SH = mm.MASK_SH  # Cobble in shade
COBBLE_DK = mm.STRAP  # Dark cobble speck
CORN = mm.HELMET  # Yellow corn cob
CORN_SH = mm.HELMET_SH  # Corn kernel shade
STRAW = mm.HELMET  # Golden rice straw
STRAW_SH = mm.HELMET_SH  # Straw in shade
STRAW_LT = mm.HEADLAMP  # Sunny straw
CARA = mm.MASK_SH  # Grey water buffalo
CARA_LT = mm.MASK  # Buffalo head, lit
CARA_SH = mix(mm.MASK_SH, mm.STRAP)  # Buffalo legs and belly in shade
HOOF = mm.STRAP  # Hooves and tail tip
HORN = mm.VEST  # Buffalo horns
EGRET = mm.WHITE  # Egret feathers
EGRET_SH = mm.MASK  # Egret shade
LEG = mm.STRAP  # Egret legs
BEAK = mm.HELMET  # Egret beak
STALK = mm.HILLS_SH  # Corn stalk
LEAF_CORN = mm.HILLS  # Corn leaf
LEAF_DK = mm.LEAF  # Corn leaf tip
HALITE_FRONT = mix(mm.WHITE, mm.PLATEAU)  # Pink-white face of a salt cube
HALITE_SIDE = mix(mm.MASK, mm.PLATEAU)  # Pink-grey side of a salt cube
GYP = mix(mm.WHITE, mm.HEADLAMP)  # Cream gypsum
GYP_SH = mix(mm.WHITE, mm.MASK)  # Gypsum in shade
SILT = mix(mm.MASK_SH, mm.GROUND)  # Grey-brown siltstone
SILT_SH = mix(mm.GROUND_SH, mm.MASK_SH)  # Siltstone in shade
SILT_LN = mix(mm.GROUND_SH, mm.OUTLINE)  # Dark siltstone layer line
MUDSTONE = mix(mm.STRAP, mm.GROUND_SH)  # Dark grey-brown mudstone
MUDSTONE_SH = mix(mm.STRAP, mm.OUTLINE)  # Mudstone in shade
SKIN = mix(mm.PLATEAU, mm.HEADLAMP)  # Farmer's face and arms
WOOD_LT = mix(lm.HANDLE, mm.HEADLAMP)  # Sunny top of a plank or log


# ---------- Rice Plain (E1) ----------

def rice_top():
    """A 16x8 wet paddy edge: upright rice stalk tips over muddy soil, with pale water glints between some stalks."""
    img = mm.new_image(16, 8)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 8, MUD)  # Wet clay below
    mm.fill_rect(img, 0, 6, 16, 2, MUD_DK)  # Shaded clay at the bottom
    for x in (1, 4, 7, 10, 13):  # Five upright stalks
        for y in range(1, 6):  # Each stalk, from the water line up to the tip
            la.put(img, x, y, RICE_GREEN if y > 2 else RICE_TIP)  # Green stalk with a light yellow-green tip
    for (x, y) in ((2, 4), (8, 2), (11, 5), (14, 3)):  # Pale blue water glints between stalks
        la.put(img, x, y, WATER_LT)  # One glint pixel
    return img  # Done


def rice_soil():
    """A 16x16 wet dark clay tile with lighter silt layers and a little water shine."""
    layers = [(x, 5 + la.wave(x, 1), MUD_LT) for x in range(16)]  # A wavy silt layer across the tile
    layers += [(x, 12 + la.wave(x, 1), MUD_LT) for x in range(16)]  # A second silt layer lower down
    layers += [(3, 9, WATER), (4, 9, WATER_LT), (11, 2, WATER)]  # Small water shine in the wet clay
    return na.soil_tile(MUD, MUD_DK, layers)  # Dark clay with its layers


def ledge_rice():
    """A 16x10 raised earthen paddy dike with a grass top."""
    return na.ledge_tile(mm.GROUND, RICE_GREEN, MUD, [(4, 4, MUD_DK), (10, 5, MUD_LT), (13, 3, mm.VEST_SH),
                                                      (3, 0, RICE_TIP), (9, 0, RICE_TIP)])  # Clay body with grass on top


def rice_boulder(w, h):
    """A brown-grey boulder with a thin cap of green grass."""
    return na.capped_boulder(w, h, BROWN_GREY, BROWN_GREY_SH, BROWN_GREY_LT, mm.STRAP, RICE_DK, 1)  # Rock with grass


def rice_granary():
    """A 56x48 raised wooden rice storage house on stilts, with a grass roof and rice sacks (the E1 goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    for y in range(4, 22):  # Roof rows
        half = 6 + (y - 4) * 1.2  # Roof gets wider going down
        for x in range(int(28 - half), int(28 + half)):  # Pixels of the roof row
            la.put(img, x, y, THATCH_SH if (x + y) % 4 == 0 else THATCH)  # Grass roof with lines
    mm.fill_rect(img, 10, 21, 36, 16, WOOD)  # Wooden house walls
    for y in range(24, 37, 4):  # Plank lines
        mm.fill_rect(img, 10, y, 36, 1, WOOD_SH)  # One plank line
    mm.fill_rect(img, 8, 36, 40, 2, WOOD_SH)  # Raised floor
    for x in (11, 21, 33, 43):  # Four stilts under the floor
        mm.fill_rect(img, x, 38, 2, 9, WOOD_SH)  # One stilt
    mm.fill_rect(img, 23, 27, 10, 9, mm.OUTLINE)  # Dark doorway
    mm.fill_rect(img, 14, 24, 4, 4, mm.SKY_LIGHT)  # Left window
    mm.fill_rect(img, 38, 24, 4, 4, mm.SKY_LIGHT)  # Right window
    for (cx, cy, r) in ((5, 43, 4), (6, 38, 3), (50, 43, 4), (49, 38, 3)):  # Rice sacks: middle x, middle y, radius
        ls.disc(img, cx, cy, r, SACK)  # One sack
        ls.disc(img, cx + 1, cy + 1, r - 2, SACK_SH)  # Shaded side of the sack
    return la.outline(img)  # Dark edge


# ---------- Salt Beds: Pangasinan (E2) ----------

def salt_top():
    """A 16x8 tile of white salt crust with pale blue brine shine."""
    return na.ground_top(SALT_SOIL, SALT_WHITE, SALT_WHITE, mm.MASK,
                         [(2, 2, BRINE_LT), (3, 2, BRINE_LT), (9, 3, BRINE_LT), (13, 1, BRINE_LT),
                          (5, 1, mm.MASK), (11, 2, mm.MASK)])  # Salt crust with brine shine and grey lumps


def salt_soil():
    """A 16x16 grey sandy mud tile with white salt flecks."""
    flecks = [(4, 5, mm.WHITE), (5, 5, mm.WHITE), (11, 11, mm.WHITE), (9, 3, mm.WHITE), (13, 7, mm.WHITE)]  # White salt flecks
    return na.soil_tile(SALT_SOIL, SALT_SOIL_DK, flecks)  # Grey mud with salt


def ledge_salt():
    """A 16x10 narrow wooden plank walk with plank joints that act as posts."""
    joints = [(x, y, mm.GROUND_SH) for x in (0, 8) for y in range(2, 8)]  # Post joints between the planks
    return na.ledge_tile(WOOD, WOOD_LT, mm.GROUND_SH, joints + [(4, 6, mm.GROUND), (12, 4, mm.GROUND)])  # Plank walk


def salt_boulder(w, h):
    """A pale grey beach rock with a white salt crust on top."""
    return na.capped_boulder(w, h, mm.MASK, mm.MASK_SH, mm.WHITE, mm.STRAP, SALT_WHITE, 1)  # Pale rock with salt


def small_hut():
    """A 22x16 small nipa hut with a bamboo wall, for the Pangasinan picture (decoration only)."""
    img = mm.new_image(22, 16)  # Empty picture
    for y in range(0, 7):  # Roof rows
        half = 2 + y * 1.6  # Roof gets wider going down
        for x in range(int(11 - half), int(11 + half)):  # Pixels of the roof row
            la.put(img, x, y, THATCH_SH if (x + y) % 4 == 0 else THATCH)  # Nipa roof with lines
    mm.fill_rect(img, 4, 7, 14, 9, BAMBOO)  # Bamboo wall
    for x in range(4, 18, 3):  # Bamboo poles
        mm.fill_rect(img, x, 7, 1, 9, BAMBOO_SH)  # One pole in shade
    mm.fill_rect(img, 9, 10, 4, 6, mm.OUTLINE)  # Doorway
    return la.outline(img)  # Dark edge


def salt_hut():
    """A 56x48 bamboo and nipa salt-maker's hut with a white salt heap and a rake leaning on it (the E2 goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    for y in range(6, 22):  # Nipa roof rows
        half = 4 + (y - 6) * 1.3  # Roof gets wider going down
        for x in range(int(24 - half), int(24 + half)):  # Pixels of the roof row
            la.put(img, x, y, THATCH_SH if (x + y) % 4 == 0 else THATCH)  # Nipa with lines
    for y in range(20, 44):  # Bamboo wall rows
        for x in range(6, 42):  # Wall pixels
            la.put(img, x, y, BAMBOO if x % 4 else BAMBOO_SH)  # Bamboo poles with dark gaps
    for y in (26, 34):  # Bamboo ties
        mm.fill_rect(img, 6, y, 36, 1, BAMBOO_SH)  # One tie across the wall
    mm.fill_rect(img, 4, 44, 40, 2, WOOD_SH)  # Hut floor on the ground
    mm.fill_rect(img, 18, 30, 9, 14, mm.OUTLINE)  # Doorway
    ls.disc(img, 48, 40, 6, SALT_WHITE)  # White salt heap
    ls.disc(img, 51, 42, 4, mm.MASK)  # Grey shade on the salt heap
    for t in range(33):  # Rake handle, from the ground up to the top
        la.put(img, 55 - round(t * 11 / 32.0), 46 - t, WOOD)  # One handle pixel leaning on the salt
    mm.fill_rect(img, 50, 45, 6, 1, mm.STRAP)  # Rake head on the ground
    return la.outline(img)  # Dark edge


# ---------- River Plain: Isabela (E3) ----------

def river_top():
    """A 16x8 tile of short green grass over rich brown soil."""
    return na.ground_top(RIVER_SOIL, mm.HILLS, mm.PLAINS, mm.HILLS_SH,
                         [(2, 1, mm.HILLS_SH), (7, 2, mm.PLAINS), (11, 1, mm.HILLS_SH), (14, 2, mm.HILLS_SH),
                          (5, 6, RIVER_DK), (10, 6, RIVER_DK)])  # Short grass tufts and dark specks


def river_soil():
    """A 16x16 brown alluvial soil tile with wavy sand and silt layers and small pebbles."""
    layers = [(x, 4 + la.wave(x, 1), SAND_RIV) for x in range(16)]  # A wavy sand layer
    layers += [(x, 10 + la.wave(x, 1), SILT_RIV) for x in range(16)]  # A wavy silt layer
    layers += [(3, 13, COBBLE_SH), (9, 7, COBBLE_SH), (12, 14, COBBLE_SH)]  # Small pebbles
    return na.soil_tile(RIVER_SOIL, RIVER_DK, layers)  # Alluvial soil with its layers


def ledge_river():
    """A 16x10 wooden log platform with bark lines and rings."""
    return na.ledge_tile(WOOD, WOOD_LT, mm.GROUND_SH,
                         [(3, 4, mm.GROUND_SH), (9, 6, mm.GROUND_SH), (12, 3, mm.VEST_SH), (6, 2, mm.GROUND)])  # Bark and rings


def river_cobble(w, h):
    """A round grey river cobble, smooth and rounded."""
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x, y) < 6:  # A few dark specks
            return COBBLE_DK  # Dark speck
        return COBBLE_SH if s else COBBLE_LT  # Grey stone, shaded at the lower right
    return ls.outline(na.blob(w, h, (w - 1) / 2.0, h / 2.0, w / 2.0 - 1, h / 2.0 - 1, c, 2.0))  # Round cobble


def farm_shed():
    """A 56x48 open farm shed with yellow corn cobs drying under the roof and a wooden cart wheel (the E3 goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    mm.fill_rect(img, 2, 8, 52, 9, mm.PLATEAU)  # Clay roof
    mm.fill_rect(img, 2, 16, 52, 2, mm.PLATEAU_SH)  # Shade under the roof edge
    mm.fill_rect(img, 8, 20, 24, 25, WOOD_SH)  # Back wall, open at the front
    for x in range(8, 32, 4):  # Wall boards
        mm.fill_rect(img, x, 20, 1, 25, WOOD)  # One board line
    mm.fill_rect(img, 6, 18, 2, 27, WOOD)  # Left post
    mm.fill_rect(img, 32, 18, 2, 27, WOOD)  # Right post
    mm.fill_rect(img, 8, 18, 24, 1, WOOD_SH)  # Beam under the roof
    for (x, n) in ((12, 9), (18, 8), (24, 9)):  # Three corn cobs hanging: column and length
        la.put(img, x, 19, mm.GROUND_SH)  # String from the beam
        mm.fill_rect(img, x - 1, 20, 3, n, CORN)  # Yellow cob
        for yy in range(21, 20 + n, 2):  # Kernel lines down the cob
            mm.fill_rect(img, x - 1, yy, 3, 1, CORN_SH)  # One kernel line
    cx, cy, r = 45, 35, 8  # Cart wheel: middle x, middle y, radius
    for yy in range(cy - r, cy + r + 1):  # Each row of the wheel
        for xx in range(cx - r, cx + r + 1):  # Each pixel of the row
            d = math.sqrt((xx - cx) ** 2 + (yy - cy) ** 2)  # Distance from the middle
            if r - 2 <= d <= r:  # The rim of the wheel
                la.put(img, xx, yy, WOOD)  # Rim pixel
    for t in range(-r + 2, r - 1):  # Spokes from the hub to the rim
        la.put(img, cx + t, cy, WOOD_SH)  # Spoke across
        la.put(img, cx, cy + t, WOOD_SH)  # Spoke down
        la.put(img, cx + t, cy + t, WOOD_SH)  # Spoke diagonal
        la.put(img, cx + t, cy - t, WOOD_SH)  # Spoke other diagonal
    ls.disc(img, cx, cy, 2, WOOD_SH)  # Hub of the wheel
    return la.outline(img)  # Dark edge


# ---------- Straw bounce pile (new object) ----------

def straw_pile(squash):
    """A 48x16 rounded golden rice-straw pile with a dark outline; its flat bottom is on row 15.
    With squash true it is flatter and a little wider, about 10 pixels tall."""
    img = mm.new_image(48, 16)  # Empty picture
    base = 14  # The last straw row; the dark outline is put under it on row 15
    top_h = 9 if squash else 14  # How tall the pile is at its middle
    power = 0.35 if squash else 0.5  # Squash makes the sides fuller and flatter
    for x in range(1, 47):  # Columns of the pile, one pixel in from each side
        t = (x - 23.5) / 24.0  # -1 at the left edge, 1 at the right edge
        h = 2 + int((top_h - 2) * (1 - t * t) ** power)  # Height of this column
        for y in range(base - h + 1, base + 1):  # Rows of this column
            color = STRAW_SH if (x * 2 + y) % 5 == 0 else STRAW  # Straw strands
            if y == base - h + 1 and h > 4:  # The top row of the column
                color = STRAW_LT  # Sunny top
            la.put(img, x, y, color)  # Paint the straw
    return la.outline(img)  # Dark outline all round, so it reads on every ground


def straw_bits(frame):
    """A 12x6 few loose straw bits flying up; frame 1 has them a little higher."""
    img = mm.new_image(12, 6)  # Empty picture
    rise = 3 + frame  # How high the fan reaches
    for dx in (-2, -1, 0, 1, 2):  # Five bits in a small upward fan
        bx = 6 + dx  # Base column of this bit
        tx = bx + dx * frame  # Tip column; frame 1 spreads the fan wider
        ty = 5 - (rise - abs(dx))  # Tip row; the middle bits go highest
        for t in (0, 0.5, 1):  # Three pixels along the bit, from base to tip
            x = int(round(bx + (tx - bx) * t))  # Column of this pixel
            y = int(round(5 + (ty - 5) * t))  # Row of this pixel
            la.put(img, x, y, STRAW_SH if t in (0, 1) else STRAW)  # Darker gold ends, golden middle
    return img  # Done


# ---------- Props (decoration) ----------

def carabao(frame):
    """A 24x16 grey water buffalo with curved horns; frame 0 grazes with its head down, frame 1 lifts its head (decoration only)."""
    img = mm.new_image(24, 16)  # Empty picture
    mm.fill_rect(img, 6, 4, 14, 7, CARA)  # Body
    mm.fill_rect(img, 6, 9, 14, 2, CARA_SH)  # Belly shade
    for x in (7, 10, 16, 18):  # Four legs
        mm.fill_rect(img, x, 11, 2, 3, CARA_SH)  # One leg
        mm.fill_rect(img, x, 14, 2, 1, HOOF)  # Its hoof
    mm.fill_rect(img, 20, 4, 1, 5, HOOF)  # Tail
    hy = 8 if frame == 0 else 2  # Head low to graze, or up
    mm.fill_rect(img, 1, hy, 6, 4, CARA_LT)  # Head
    mm.fill_rect(img, 1, hy + 3, 2, 1, mm.MASK)  # Nose
    la.put(img, 5, hy - 1, HORN)  # Horn base
    la.put(img, 6, hy - 2, HORN)  # Horn curving up and back
    la.put(img, 2, hy + 1, mm.OUTLINE)  # Eye
    return la.outline(img)  # Dark edge


def egret(frame):
    """A 12x10 white egret standing on thin legs; frame 0 holds its neck up, frame 1 lowers it (decoration only)."""
    img = mm.new_image(12, 10)  # Empty picture
    ls.disc(img, 5, 5, 2, EGRET)  # Round body
    for y in (7, 8, 9):  # Rows of the legs
        la.put(img, 4, y, LEG)  # Left leg
        la.put(img, 6, y, LEG)  # Right leg
    la.put(img, 3, 4, EGRET_SH)  # Wing shade on the back
    neck, head = (((7, 3), (7, 2), (8, 1)), (9, 1)) if frame == 0 else (((7, 4), (8, 5)), (9, 6))  # Neck and head, up or down
    for (x, y) in neck:  # Each neck pixel
        la.put(img, x, y, EGRET)  # White neck
    la.put(img, head[0], head[1], EGRET)  # Head
    la.put(img, head[0] + 1, head[1], BEAK)  # Yellow beak
    return la.outline(img)  # Dark edge so the white bird reads on the sky


def rice_tuft():
    """A 12x10 clump of green rice stalks with grain heads at the tops (decoration only)."""
    img = mm.new_image(12, 10)  # Empty picture
    for (x, h, lean) in ((2, 7, -1), (4, 9, 0), (6, 8, 1), (8, 7, 1), (10, 5, 1)):  # Stalks: column, height, lean
        for t in range(h):  # Up the stalk from its base
            la.put(img, x + (lean if t > h // 2 else 0), 9 - t, RICE_DK if t < 3 else RICE_GREEN)  # One stalk pixel
        la.put(img, x + lean, 10 - h, mm.HELMET_SH)  # Grain head at the top
    return img


def salt_mound():
    """A 16x8 small white salt heap, lit on the left and grey on the right (decoration only)."""
    img = mm.new_image(16, 8)  # Empty picture
    for x in range(1, 15):  # Columns of the heap
        t = (x - 7.5) / 7.5  # -1 at the left edge, 1 at the right edge
        h = 1 + int(5 * (1 - t * t) ** 0.5)  # Height of this column
        for y in range(6 - h + 1, 7):  # Rows of this column, down to the base row 6
            la.put(img, x, y, SALT_WHITE if x < 9 else mm.MASK)  # Lit white on the left, grey on the right
    return la.outline(img)  # Dark edge


def corn_stalk(frame):
    """A 10x28 corn plant with long leaves and a yellow cob; frame 1 leans its top a little to the right (decoration only)."""
    img = mm.new_image(10, 28)  # Empty picture
    sway = frame  # How far the top half leans right in this frame
    for y in range(4, 28):  # Up the stalk from the ground
        la.put(img, 4 + (sway if y < 16 else 0), y, STALK)  # Stalk, leaning in the top half
    for (y, side) in ((24, -1), (19, 1), (14, -1), (9, 1)):  # Leaves: row, and the side they grow to
        for t in range(1, 5):  # Out from the stalk
            x = 4 + side * t + (sway if y < 16 else 0)  # Column of this leaf pixel
            la.put(img, x, y - t // 2, LEAF_CORN if t < 4 else LEAF_DK)  # Leaf pixel, tip darker
    for y in range(12, 16):  # The corn cob, two columns wide
        la.put(img, 6 + sway, y, CORN)  # Yellow kernel column
        la.put(img, 7 + sway, y, CORN_SH)  # Shaded kernel column
    return img  # No outline, like the other living props


# ---------- New rocks and minerals (24x24, Assets/Art/Items/Sprites/) ----------

def siltstone():
    """Flat grey-brown siltstone with fine, even layers."""
    def c(x, y, s):  # Colour of one pixel
        if y % 4 == 0:  # A fine layer line
            return SILT_LN  # Dark layer line
        return SILT_SH if s else SILT  # Grey-brown rock
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10.5, 6.5, c, 3.0))  # Flat, layered slab


def halite():
    """Clear-white salt cubes stacked up, with a faint pink-white tint."""
    img = mm.new_image(24, 24)  # Empty picture
    for (x, y, s) in ((2, 11, 11), (12, 4, 9)):  # Two cubes: left edge, top row, size
        mm.fill_rect(img, x, y, s, 3, mm.WHITE)  # Lit top face
        mm.fill_rect(img, x, y + 3, s - 3, s - 3, HALITE_FRONT)  # Front face
        mm.fill_rect(img, x + s - 3, y + 3, 3, s - 3, HALITE_SIDE)  # Shaded side
    return ls.outline(img)  # Dark edge


def gypsum():
    """Soft white-cream bladed crystals that are a little see-through."""
    img = mm.new_image(24, 24)  # Empty picture
    for (x0, top, w) in ((3, 8, 6), (9, 3, 7), (15, 7, 6)):  # Three blades: left edge, tip row, width
        pts = [(x0, 20), (x0 + w / 2.0, top), (x0 + w, 20)]  # Triangle of one blade
        fill_poly_alpha(img, pts, GYP, 215)  # Soft see-through blade
        fill_poly_alpha(img, [(x0 + w / 2.0, top), (x0 + w, 20), (x0 + w / 2.0, 20)], GYP_SH, 215)  # Shaded half
    return ls.outline(img)  # Dark edge


def fill_poly_alpha(img, pts, color, alpha):
    """Paints every pixel whose middle is inside a polygon, see-through at the given strength."""
    for y in range(img.height):  # Each row
        for x in range(img.width):  # Each pixel
            if ls.inside(x + 0.5, y + 0.5, pts):  # Pixel middle is inside the shape
                la.put(img, x, y, color, alpha)  # See-through colour


def mudstone():
    """Smooth, blocky dark grey-brown rock with no layers."""
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x, y) < 5:  # A few tiny pits
            return MUDSTONE_SH  # Darker pit
        return MUDSTONE_SH if s else MUDSTONE  # Smooth rock, shaded at the lower right
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 4.0))  # Blocky smooth rock


# ---------- Lesson pictures (120x82, Assets/Art/Lesson/Sprites/) ----------

CL_FAR = [(14, 22, 0.8), (46, 20, 0.6), (80, 21, 0.7), (104, 23, 0.9)]  # Tiny far mountains: x, top y, slope
HORIZON = 34  # Row of the far horizon, where the paddies start


def central_luzon_pixel(x, y):
    """Colour of one pixel of the Central Luzon picture: sky, tiny far mountains and flat rice paddies with dikes."""
    if y < HORIZON:  # Above the horizon: sky and far mountains
        far_top, far_peak = lm.ridge(x, CL_FAR)  # The tiny far mountains
        if y >= far_top:  # Inside a far mountain
            return lm.HAZE if x <= far_peak[0] else lm.HAZE_SH  # Light or shaded haze
        return na.sky(x, y)  # Sky
    d = y - HORIZON  # Depth below the horizon
    if d % 4 == 3:  # A thin pale water line between two bands
        return WATER_LT  # Pale water line
    if d % 4 == 1 and x % 16 == 7:  # A few tidy glints, one per band section
        return WATER_LT  # Pale glint
    return RICE_GREEN if d % 4 == 1 else RICE_DK  # Neat green bands of rice


def lesson_central_luzon():
    """A 120x82 framed picture of flat rice paddies under a far horizon, with a carabao and a farmer in a hat."""
    img = ls.framed(central_luzon_pixel)  # Sky, far mountains and paddies
    mm.paste(img, carabao(0), 58, 56)  # A grazing carabao in the paddy
    mm.fill_rect(img, 28, 52, 12, 1, mm.HELMET_SH)  # Hat brim
    mm.fill_rect(img, 31, 50, 6, 2, mm.HELMET_SH)  # Hat crown
    mm.fill_rect(img, 31, 53, 6, 4, SKIN)  # Farmer's face
    mm.fill_rect(img, 30, 57, 8, 8, mm.VEST)  # Farmer's shirt
    mm.fill_rect(img, 28, 58, 2, 6, SKIN)  # Left arm
    mm.fill_rect(img, 38, 58, 2, 6, SKIN)  # Right arm
    mm.fill_rect(img, 31, 65, 2, 6, mm.STRAP)  # Left leg
    mm.fill_rect(img, 35, 65, 2, 6, mm.STRAP)  # Right leg
    return img


def pangasinan_pixel(x, y):
    """Colour of one pixel of the Pangasinan picture: sky, sea, a sand beach and square shallow salt ponds."""
    if y < 26:  # Sky
        return na.sky(x, y)  # Sky colour
    if y < 40:  # Sea
        if lm.noise(x, y) < 12:  # A few light waves
            return WATER_LT  # Light wave
        return BRINE if y < 36 else SEA_DEEP  # Sea blue, deeper near the beach
    if y < 46:  # Sandy beach strip
        return mm.VEST_SH if (x + y) % 7 == 0 else mm.VEST  # Sand with a few darker grains
    if x % 16 == 0 or y % 8 == 6:  # Dike lines between the ponds
        return mm.MASK  # Grey-white dike
    if x % 16 in (1, 14) or y % 8 in (1, 5):  # Edges of each pond
        return mm.WHITE  # Salt crust at the pond edge
    return BRINE_LT if (x + y) % 6 else BRINE  # Shallow brine with light shine


def lesson_pangasinan():
    """A 120x82 framed picture of square salt ponds by the sea, with white salt heaps and a small hut."""
    img = ls.framed(pangasinan_pixel)  # Sky, sea, beach and salt ponds
    mm.paste(img, salt_mound(), 14, 37)  # A white salt heap on the beach
    mm.paste(img, salt_mound(), 98, 37)  # A second salt heap
    mm.paste(img, small_hut(), 74, 30)  # A small nipa hut on the beach
    return img


def isabela_pixel(x, y):
    """Colour of one pixel of the Isabela picture: sky, corn fields, a wide calm river and a cut bank of river soil."""
    if y < 24:  # Sky above the flat plain
        return na.sky(x, y)  # Sky colour
    if y >= 66:  # The cut bank in the front: layers of river soil
        band = ((y - 66) + la.wave(x, 1, 16)) // 3 % 4  # Which soil layer, bent a little
        return (RIVER_SOIL, SILT_RIV, SAND_RIV, RIVER_DK)[band]  # The layer colour
    if y >= 63:  # Grass lip on top of the cut bank
        return mm.HILLS if y == 63 else mm.HILLS_SH  # Grass edge, then shade
    yc = 46 + round(6 * math.sin(x / 17.0))  # Middle row of the river at this column
    half = 3 + x // 40  # Half-width of the river, a little wider on the right
    if abs(y - yc) <= half:  # Inside the river
        if lm.noise(x, y) < 4:  # A few glints on the calm water
            return mm.HEADLAMP  # Light glint
        return RIVER_WATER  # Calm brown-green water
    if x % 5 == 2 and y % 6 == 2:  # Corn tassel on a plant
        return CORN  # Yellow tassel
    if x % 5 == 2 and y % 6 == 3:  # Corn leaf below the tassel
        return mm.LEAF  # Dark green leaf
    return mm.HILLS_SH if y % 6 in (0, 1) else mm.HILLS  # Field rows between the corn plants


def lesson_isabela():
    """A 120x82 framed picture of a wide calm river across a flat plain, with corn fields and a cut bank of river soil."""
    return ls.framed(isabela_pixel)  # Sky, fields, river and the layered bank


# ---------- Plains life sprites for the Landform Map and the Plains menu screens ----------

def rice_life(frame):
    """A 7x6 clump of green rice; frame 1 bends the tips right, so the rice sways in the wind."""
    img = mm.new_image(7, 6)  # Empty picture
    for x in (1, 3, 5):  # Three stalks
        for t in range(6):  # Up the stalk, from the ground row 5 to the tip row 0
            la.put(img, x + (frame if t >= 4 else 0), 5 - t, RICE_DK if t < 3 else RICE_GREEN)  # One stalk pixel
    la.put(img, 3 + frame, 0, mm.HELMET_SH)  # A grain head on the middle stalk
    return img


def egret_life(frame):
    """A 5x3 tiny white egret for the Plains sky; frame 0 has wings up, frame 1 has wings down."""
    img = mm.new_image(5, 3)  # Empty picture
    rows = ((0, 0), (1, 1), (2, 1), (3, 1), (4, 0)) if frame == 0 else ((0, 2), (1, 1), (2, 1), (3, 1), (4, 2))  # Wing pixels
    for (x, y) in rows:  # Each pixel
        la.put(img, x, y, EGRET)  # White bird
    la.put(img, 2, 1, EGRET_SH)  # Body shade
    return img


def carabao_life(frame):
    """A 9x6 tiny carabao for the Plains; frame 0 grazes with its head down, frame 1 lifts its head."""
    img = mm.new_image(9, 6)  # Empty picture
    mm.fill_rect(img, 1, 2, 6, 2, CARA)  # Body
    mm.fill_rect(img, 2, 4, 1, 2, CARA_SH)  # Front leg
    mm.fill_rect(img, 5, 4, 1, 2, CARA_SH)  # Back leg
    hy = 3 if frame == 0 else 1  # Head low or up
    mm.fill_rect(img, 0, hy, 2, 2, CARA_LT)  # Head
    la.put(img, 0, hy - 1, HORN)  # Horn
    la.put(img, 7, 2, HOOF)  # Tail tip
    return img


def grass_life(frame):
    """A 7x6 Plains grass tuft in golden green; frame 1 bends the tips right so it sways."""
    img = mm.new_image(7, 6)  # Empty picture
    for x in (1, 3, 5):  # Three blades
        for t in range(6):  # Up the blade, from the ground row 5 to the tip row 0
            la.put(img, x + (frame if t >= 4 else 0), 5 - t, lm.PLAINS_SH if t < 3 else mm.PLAINS)  # One blade pixel
    return img


# ---------- Saving and the preview sheet ----------

def map_save(img, name):
    """Saves an image under Assets/Art/LandformMap/Sprites/Plains/."""
    os.makedirs(MAP_OUT, exist_ok=True)  # Make the folder if it is missing
    img.save(os.path.join(MAP_OUT, name))  # Write the PNG


def put_file(where, name, img):
    """Saves one picture in the folder for its kind: level, item, lesson or map."""
    if where == "item":  # Shared item picture
        ls.save(img, ls.ITEMS_OUT, name)  # Items folder
    elif where == "lesson":  # Lesson picture
        ls.save(img, ls.LESSON_OUT, name)  # Lesson folder
    elif where == "map":  # Landform Map life sprite
        map_save(img, name)  # Plains folder
    else:  # Level sprite or prop
        la.save(img, name)  # Level sprites folder


def big(img, scale):
    """Makes a crisp, larger copy of a picture for the preview."""
    return img.resize((img.width * scale, img.height * scale), Image.NEAREST)  # Nearest-neighbour scaling


def preview(groups):
    """Lays the new sprites out at 3x size on one neutral sheet, one labelled group after another."""
    scale, pad, width = 3, 6, 900  # Size up, gap and sheet width
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
    os.makedirs(os.path.dirname(PREVIEW), exist_ok=True)  # Make the folder
    sheet.save(PREVIEW)  # Write the sheet
    print("Preview written to", PREVIEW)  # Tell where


def main():
    """Draws everything, saves each picture and writes the preview sheet."""
    groups = [  # Each group: its title, then (kind, file name, picture) for each picture
        ("E1 RICE PLAIN", [
            ("level", "rice_top.png", rice_top()),  # Ground top
            ("level", "rice_soil.png", rice_soil()),  # Ground below the grass
            ("level", "ledge_rice.png", ledge_rice()),  # Paddy dike ledge
            ("level", "boulder_small_rice.png", rice_boulder(32, 24)),  # Small boulder
            ("level", "boulder_big_rice.png", rice_boulder(48, 36)),  # Big boulder
            ("level", "rice_granary.png", rice_granary()),  # Goal: rice storage house
        ]),
        ("E2 SALT BEDS", [
            ("level", "salt_top.png", salt_top()),  # Ground top
            ("level", "salt_soil.png", salt_soil()),  # Ground below the crust
            ("level", "ledge_salt.png", ledge_salt()),  # Plank walk ledge
            ("level", "boulder_small_salt.png", salt_boulder(32, 24)),  # Small boulder
            ("level", "boulder_big_salt.png", salt_boulder(48, 36)),  # Big boulder
            ("level", "salt_hut.png", salt_hut()),  # Goal: salt-maker's hut
        ]),
        ("E3 RIVER PLAIN", [
            ("level", "river_top.png", river_top()),  # Ground top
            ("level", "river_soil.png", river_soil()),  # Ground below the grass
            ("level", "ledge_river.png", ledge_river()),  # Log platform ledge
            ("level", "boulder_small_river.png", river_cobble(32, 24)),  # Small cobble
            ("level", "boulder_big_river.png", river_cobble(48, 36)),  # Big cobble
            ("level", "farm_shed.png", farm_shed()),  # Goal: open farm shed
        ]),
        ("STRAW BOUNCE PILE", [
            ("level", "straw_pile.png", straw_pile(False)),  # Straw pile
            ("level", "straw_pile_squash.png", straw_pile(True)),  # Squashed straw pile
            ("level", "Props/straw_bits_00.png", straw_bits(0)),  # Loose straw, frame 0
            ("level", "Props/straw_bits_01.png", straw_bits(1)),  # Loose straw, frame 1
        ]),
        ("PLAINS PROPS", [
            ("level", "Props/carabao_00.png", carabao(0)),  # Carabao grazing
            ("level", "Props/carabao_01.png", carabao(1)),  # Carabao head up
            ("level", "Props/egret_00.png", egret(0)),  # Egret neck up
            ("level", "Props/egret_01.png", egret(1)),  # Egret neck down
            ("level", "Props/rice_tuft.png", rice_tuft()),  # Rice clump
            ("level", "Props/salt_mound.png", salt_mound()),  # Salt heap
            ("level", "Props/corn_stalk_00.png", corn_stalk(0)),  # Corn plant
            ("level", "Props/corn_stalk_01.png", corn_stalk(1)),  # Corn plant swaying
        ]),
        ("ITEMS", [
            ("item", "siltstone.png", siltstone()),  # Layered grey-brown rock
            ("item", "halite.png", halite()),  # Salt cubes
            ("item", "gypsum.png", gypsum()),  # Bladed cream crystals
            ("item", "mudstone.png", mudstone()),  # Smooth dark rock
        ]),
        ("LESSON PICTURES", [
            ("lesson", "lesson_central_luzon.png", lesson_central_luzon()),  # Rice paddies
            ("lesson", "lesson_pangasinan.png", lesson_pangasinan()),  # Salt ponds
            ("lesson", "lesson_isabela.png", lesson_isabela()),  # River plain
        ]),
        ("PLAINS LIFE", [
            ("map", "rice_0.png", rice_life(0)),  # Rice, frame 0
            ("map", "rice_1.png", rice_life(1)),  # Rice, frame 1
            ("map", "egret_0.png", egret_life(0)),  # Egret wings up
            ("map", "egret_1.png", egret_life(1)),  # Egret wings down
            ("map", "carabao_0.png", carabao_life(0)),  # Carabao head down
            ("map", "carabao_1.png", carabao_life(1)),  # Carabao head up
            ("map", "grass_0.png", grass_life(0)),  # Grass, frame 0
            ("map", "grass_1.png", grass_life(1)),  # Grass, frame 1
        ]),
    ]
    preview_groups = []  # (title, pictures) for the preview sheet
    for title, entries in groups:  # Each group
        sprites = []  # Pictures of this group
        for where, name, img in entries:  # Each picture
            put_file(where, name, img)  # Write it to its folder
            sprites.append(img)  # Keep it for the preview
        preview_groups.append((title, sprites))  # Add the group to the sheet
    preview(preview_groups)  # The check sheet
    print("Plains Easy art written to", la.OUT, ls.ITEMS_OUT, ls.LESSON_OUT, "and", MAP_OUT)  # Tell where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
