"""Draws the PNGs for Mountains Hard 1 to 7 (D-065): seven new level themes, the rolling boulder and icy ground,
fourteen new rocks and minerals, seven lesson pictures of real mountains, and the Landform Description Screen art.

Run from the Unity project folder:  python Tools/Art/make_hard_art.py
It writes to Assets/Art/Level/Sprites/ (themes and level objects), Assets/Art/Items/Sprites/ (items),
Assets/Art/Lesson/Sprites/ (lesson pictures) and Assets/Art/Description/Sprites/ (Landform Description Screen).
It reuses the helpers of make_level_art.py, make_lesson_art.py and make_normal_art.py.
All colours come from Assets/Art/Palettes/landgrand.gpl or mix two of its colours.
"""
import math  # math gives circles, angles and waves
import os  # os joins folder paths
import make_mainmenu_art as mm  # Palette, new_image, fill_rect, text and buttons
import make_landform_map_art as lm  # Map colours, noise, ridge and the padlock pattern
import make_level_art as la  # put, outline, wave, boulder_colored, draw_grid and save for level sprites
import make_lesson_art as ls  # framed, disc, fill_poly and the item and lesson folders
import make_normal_art as na  # mix, the shared tile makers and the item blob
import make_menu_camp as camp  # The 34x30 icon button and the 2x icon helper

mix = na.mix  # The colour halfway between two palette colours
DESC_OUT = os.path.join(mm.ROOT, "Assets", "Art", "Description", "Sprites")  # Where the Landform Description Screen PNGs go

# Theme colours (D-065), each a palette colour or a mix of two.
ASH_GREY = mix(mm.MASK_SH, mm.STRAP)  # Dark grey volcanic ash on Mayon
LAVA_DARK = la.LAVA_ROCK  # Very dark cooled lava
GLOW = la.LAVA_ORANGE  # Hot orange glow in lava cracks
WET_GREEN = mix(mm.LEAF, mm.STRAP)  # Rain-dark grass on the Sierra Madre
WET_SOIL = mix(mm.GROUND_SH, mm.STRAP)  # Wet brown soil
CANOPY = mm.LEAF  # Rainforest leaves
CANOPY_DK = mix(mm.LEAF, mm.OUTLINE)  # Deep leaf shade
LITTER = mix(mm.GROUND, mm.HELMET_SH)  # Fallen leaves on the forest floor
LATERITE = mm.PLATEAU_SH  # Orange-red ultramafic soil on Hamiguitan
LATERITE_LT = mm.PLATEAU  # The same soil in sun
SERP = mix(mm.HILLS_SH, mm.MASK_SH)  # Grey-green serpentinite rock
ICE = mix(lm.LENS, mm.WHITE)  # Pale blue glacier ice
ICE_SH = mix(lm.LENS, lm.MOUNTAIN)  # Ice in shade
SCREE = mix(mm.VEST_SH, mm.MASK_SH)  # Grey-brown volcanic scree on Kilimanjaro
SCREE_LT = mix(mm.VEST, mm.MASK)  # Scree in sun
CINDER = mix(mm.SCARF_SH, mm.BACKPACK_SH)  # Red-brown cinder on Mauna Kea
CINDER_LT = mix(mm.SCARF_SH, mm.PLATEAU)  # Cinder in sun
CINDER_DK = mix(mm.BACKPACK_SH, mm.OUTLINE)  # Cinder in deep shade
SEA = lm.LENS  # Sea blue
SEA_DEEP = mix(lm.LENS, lm.MOUNTAIN_SH)  # Deep sea blue


# ---------- Lava Slope: Mount Mayon (H1) ----------

def lava_top():
    """Dark grey ash over cooled lava, with one glowing crack."""
    return na.ground_top(LAVA_DARK, ASH_GREY, mm.MASK_SH, mm.STRAP,
                         [(3, 2, mm.STRAP), (9, 1, mm.MASK), (12, 3, mm.STRAP), (6, 6, GLOW), (7, 6, la.LAVA_SWIRL)])  # Ash bumps and a hot crack


def lava_soil():
    """Black cooled lava with faint red swirls."""
    return na.soil_tile(LAVA_DARK, la.LAVA_ROCK_SH, [(x, 6 + la.wave(x, 1), la.LAVA_SWIRL) for x in range(3, 12)] + [(9, 12, mm.SCARF_SH)])  # Swirl and one red fleck


def ledge_lava():
    """A slab of dark lava rock with an ash top."""
    return na.ledge_tile(la.BASALT, ASH_GREY, la.BASALT_SH, [(4, 4, la.BASALT_LT), (11, 5, la.LAVA_ROCK_SH), (8, 1, mm.MASK_SH)])  # Pores and ash


def lava_boulder(w, h):
    """A dark lava boulder with an ash cap."""
    return na.capped_boulder(w, h, la.BASALT, la.BASALT_SH, la.BASALT_LT, la.LAVA_ROCK_SH, ASH_GREY, 2)  # Lava rock with ash


def watch_station():
    """A 48x48 volcano watch station: a small white building with a dish and an antenna (the H1 goal)."""
    img = mm.new_image(48, 48)  # Empty picture
    mm.fill_rect(img, 6, 22, 36, 26, mm.MASK)  # Wall
    mm.fill_rect(img, 24, 22, 18, 26, mm.MASK_SH)  # Shaded right side
    mm.fill_rect(img, 4, 18, 40, 4, mm.SCARF)  # Red roof band
    mm.fill_rect(img, 18, 32, 12, 16, mm.OUTLINE)  # Open door in the middle
    mm.fill_rect(img, 9, 27, 7, 6, mm.SKY_LIGHT)  # Window
    mm.fill_rect(img, 34, 27, 5, 6, mm.SKY_LIGHT)  # Second window
    mm.fill_rect(img, 35, 4, 2, 14, mm.STRAP)  # Antenna mast
    for x in range(30, 42):  # Antenna cross bars
        la.put(img, x, 6 + (abs(x - 36) // 3), mm.STRAP)  # A small V of wires
    for y in range(8, 14):  # A bowl-shaped dish facing up
        half = (y - 7) * 0.9  # Wider going up from the bottom of the bowl
        mm.fill_rect(img, int(13 - 5 + half), y, max(1, int(10 - 2 * half)), 1, mm.MASK_SH)  # One bowl row
    mm.fill_rect(img, 8, 8, 11, 1, mm.MASK)  # Lit rim of the dish
    mm.fill_rect(img, 13, 14, 1, 4, mm.STRAP)  # Dish stand
    return la.outline(img)  # Dark edge


def burnt_tree():
    """A 20x32 bare grey tree killed by hot ash (decoration only)."""
    img = mm.new_image(20, 32)  # Empty picture
    mm.fill_rect(img, 9, 8, 2, 24, mm.STRAP)  # Trunk
    for (x0, y0, dx) in ((10, 14, -1), (10, 10, 1), (10, 19, 1)):  # Three bare branches
        for t in range(7):  # Along the branch
            la.put(img, x0 + dx * t, y0 - t // 2, mm.MASK_SH)  # One branch pixel
    return la.outline(img)  # Dark edge


# ---------- Storm Ridge: Sierra Madre (H2) ----------

def storm_top():
    """Rain-dark grass with puddle shine."""
    return na.ground_top(WET_SOIL, WET_GREEN, mm.HILLS_SH, mm.STRAP, [(2, 1, mm.LEAF), (7, 2, lm.LENS), (8, 2, lm.LENS), (13, 1, mm.LEAF), (4, 6, mm.STRAP)])  # Grass and a puddle


def storm_soil():
    """Wet brown soil with small stones."""
    return na.soil_tile(WET_SOIL, mm.STRAP, [(4, 4, mm.MASK_SH), (11, 10, mm.MASK_SH), (7, 13, lm.LENS)])  # Stones and a drop


def ledge_storm():
    """A wet grey rock slab with a green top."""
    return na.ledge_tile(lm.MOUNTAIN_SH, WET_GREEN, mm.STRAP, [(3, 4, lm.MOUNTAIN), (10, 5, lm.HAZE_SH), (6, 1, mm.HILLS_SH)])  # Wet shine


def storm_boulder(w, h):
    """A dark wet boulder with moss."""
    return na.capped_boulder(w, h, lm.MOUNTAIN_SH, mm.STRAP, lm.MOUNTAIN, mm.OUTLINE, WET_GREEN, 3)  # Wet rock


def lookout_tower():
    """A 40x64 wooden lookout tower with a roof and a ladder in the middle (the H2 goal)."""
    img = mm.new_image(40, 64)  # Empty picture
    for x in (6, 32):  # Two legs
        mm.fill_rect(img, x, 22, 3, 42, lm.HANDLE)  # One leg
    for y in range(28, 62, 8):  # Cross braces
        for t in range(26):  # Along the brace
            la.put(img, 7 + t, y + (t // 6 if (y // 8) % 2 else 4 - t // 6), mm.GROUND_SH)  # Zig-zag brace
    mm.fill_rect(img, 17, 22, 6, 42, mm.GROUND)  # Ladder rails
    for y in range(24, 64, 4):  # Ladder rungs
        mm.fill_rect(img, 17, y, 6, 1, mm.GROUND_SH)  # One rung
    mm.fill_rect(img, 3, 16, 34, 6, mm.GROUND)  # Platform deck
    mm.fill_rect(img, 3, 10, 2, 6, lm.HANDLE)  # Left rail post
    mm.fill_rect(img, 35, 10, 2, 6, lm.HANDLE)  # Right rail post
    for y in range(2, 10):  # Roof rows
        half = 4 + (y - 2) * 2  # Wider going down
        mm.fill_rect(img, 20 - half, y, half * 2, 1, mm.SCARF if y % 2 else mm.SCARF_SH)  # One roof row
    return la.outline(img)  # Dark edge


def rain(frame):
    """An 8x16 slanted rain streak (frames 0 and 1 move down)."""
    img = mm.new_image(8, 16)  # Empty picture
    for (x0, y0) in ((1, 0), (5, 7)):  # Two drops
        for t in range(5):  # Along the streak
            la.put(img, x0 + t // 3, (y0 + t + frame * 8) % 16, lm.HAZE, 170)  # A see-through blue-white streak
    return img


def bent_tree():
    """A 24x40 tree bent by storm winds (decoration only)."""
    img = mm.new_image(24, 40)  # Empty picture
    for y in range(16, 40):  # Trunk leaning right as it goes up
        mm.fill_rect(img, 8 + (40 - y) // 5, y, 3, 1, lm.HANDLE)  # One trunk row
    for (cx, cy, r) in ((16, 10, 6), (11, 13, 5), (19, 15, 4)):  # Leaf clumps
        ls.disc(img, cx, cy, r, WET_GREEN)  # One clump
    ls.disc(img, 14, 9, 3, mm.HILLS_SH)  # Lit leaves
    return la.outline(img)  # Dark edge


# ---------- Eagle Forest: Kitanglad Range (H3) ----------

def eagle_top():
    """Forest floor with fallen leaves."""
    return na.ground_top(na.HUMUS, LITTER, mm.HELMET_SH, mm.GROUND_SH, [(2, 1, mm.SCARF_SH), (6, 2, mm.HILLS_SH), (11, 1, mm.PLATEAU), (14, 2, mm.GROUND_SH), (9, 6, mm.BACKPACK_SH)])  # Leaves


def eagle_soil():
    """Dark forest soil with roots."""
    return na.soil_tile(na.HUMUS, mm.OUTLINE, [(x, 8 + la.wave(x, 1, 10), mm.GROUND_SH) for x in range(1, 13)])  # A root


def ledge_eagle():
    """A thick tree branch ledge with moss."""
    return na.ledge_tile(mm.GROUND_SH, na.MOSS, mm.BACKPACK_SH, [(2, 4, lm.HANDLE), (7, 5, lm.HANDLE), (13, 4, lm.HANDLE), (5, 0, na.MOSS_LT)])  # Bark and moss


def eagle_boulder(w, h):
    """A grey boulder with leaf litter on top."""
    return na.capped_boulder(w, h, lm.MOUNTAIN, lm.MOUNTAIN_SH, lm.HAZE, mm.STRAP, LITTER, 2)  # Rock with leaves


def eagle_nest():
    """A 56x56 big tree stump with a stick nest and an eagle chick (the H3 goal)."""
    img = mm.new_image(56, 56)  # Empty picture
    mm.fill_rect(img, 18, 24, 20, 32, mm.GROUND_SH)  # Trunk
    mm.fill_rect(img, 18, 24, 7, 32, lm.HANDLE)  # Lit side of the trunk
    for y in range(26, 56, 6):  # Bark lines
        mm.fill_rect(img, 27, y, 8, 1, mm.BACKPACK_SH)  # One line
    for y in range(14, 26):  # Nest rows
        half = 22 - abs(y - 20) * 1.5  # Wide bowl
        for x in range(int(28 - half), int(28 + half)):  # Pixels of the row
            la.put(img, x, y, mm.GROUND if (x + y) % 3 else mm.GROUND_SH)  # Woven sticks
    ls.disc(img, 28, 10, 5, mm.WHITE)  # Fluffy white chick
    la.put(img, 26, 9, mm.OUTLINE)  # Eye
    la.put(img, 30, 9, mm.OUTLINE)  # Eye
    mm.fill_rect(img, 27, 11, 3, 2, mm.HELMET_SH)  # Beak
    return la.outline(img)  # Dark edge


def tall_tree():
    """A 32x64 tall rainforest tree for the Eagle Forest (decoration only)."""
    img = mm.new_image(32, 64)  # Empty picture
    mm.fill_rect(img, 14, 20, 4, 44, mm.GROUND_SH)  # Trunk
    mm.fill_rect(img, 14, 20, 1, 44, lm.HANDLE)  # Lit edge
    for (cx, cy, r) in ((16, 12, 10), (7, 16, 6), (25, 16, 6)):  # Crown clumps
        ls.disc(img, cx, cy, r, CANOPY_DK)  # Shade
        ls.disc(img, cx - 1, cy - 1, r - 2, CANOPY)  # Leaves
    return la.outline(img)  # Dark edge


def orchid():
    """A 12x12 small purple-pink orchid (decoration only)."""
    img = mm.new_image(12, 12)  # Empty picture
    mm.fill_rect(img, 5, 6, 1, 6, mm.LEAF)  # Stem
    for (x, y) in ((4, 3), (6, 3), (5, 2), (3, 4), (7, 4)):  # Petals
        la.put(img, x, y, mix(mm.SCARF, mm.WHITE))  # Pink petal
    la.put(img, 5, 4, mm.HELMET)  # Yellow middle
    mm.fill_rect(img, 2, 9, 3, 1, mm.HILLS_SH)  # Leaf
    return la.outline(img)  # Dark edge


# ---------- Bonsai Forest: Mount Hamiguitan (H4) ----------

def bonsai_top():
    """Orange-red ultramafic soil with low green shrubs."""
    return na.ground_top(LATERITE, mm.HILLS_SH, mm.HILLS, mm.LEAF, [(3, 1, mm.LEAF), (9, 0, mm.HILLS), (13, 2, LATERITE_LT), (6, 6, mm.BACKPACK_SH)])  # Shrubs and red soil


def bonsai_soil():
    """Orange-red soil with dark iron specks."""
    return na.soil_tile(LATERITE, mm.BACKPACK_SH, [(4, 4, LATERITE_LT), (12, 9, SERP), (7, 13, LATERITE_LT)])  # Soil with a green stone


def ledge_bonsai():
    """A grey-green serpentinite slab."""
    return na.ledge_tile(SERP, mm.HILLS_SH, mix(SERP, mm.OUTLINE), [(3, 4, mm.MASK), (9, 5, mm.HILLS_SH), (13, 3, mm.MASK_SH)])  # Shiny green-grey rock


def bonsai_boulder(w, h):
    """A grey-green serpentinite boulder."""
    return la.boulder_colored(w, h, SERP, mix(SERP, mm.OUTLINE), mm.MASK, mm.STRAP, (mm.HILLS_SH, mm.LEAF))  # Green-grey rock with green bands


def sanctuary_sign():
    """A 40x40 wooden sanctuary sign with a leaf picture (the H4 goal)."""
    img = mm.new_image(40, 40)  # Empty picture
    mm.fill_rect(img, 8, 18, 3, 22, lm.HANDLE)  # Left post
    mm.fill_rect(img, 29, 18, 3, 22, lm.HANDLE)  # Right post
    mm.fill_rect(img, 3, 4, 34, 18, mm.GROUND)  # Board
    mm.fill_rect(img, 3, 19, 34, 3, mm.GROUND_SH)  # Board shade
    ls.fill_poly(img, [(20, 6), (27, 13), (20, 19), (13, 13)], mm.HILLS)  # Leaf
    mm.fill_rect(img, 20, 8, 1, 11, mm.LEAF)  # Leaf vein
    return la.outline(img)  # Dark edge


def bonsai_tree():
    """A 24x24 short, twisted bonsai-like tree of the pygmy forest (decoration only)."""
    img = mm.new_image(24, 24)  # Empty picture
    for y in range(12, 24):  # Twisted trunk
        mm.fill_rect(img, 11 + int(2 * math.sin(y / 2.0)), y, 2, 1, mm.GROUND_SH)  # One trunk row
    for (cx, cy, r) in ((8, 9, 4), (15, 8, 5), (12, 5, 3)):  # Flat leaf clumps
        ls.disc(img, cx, cy, r, mm.LEAF)  # One clump
    ls.disc(img, 14, 7, 2, mm.HILLS_SH)  # Lit leaves
    return la.outline(img)  # Dark edge


def pitcher_plant():
    """A 10x14 pitcher plant (decoration only)."""
    img = mm.new_image(10, 14)  # Empty picture
    for y in range(4, 13):  # The cup
        half = 2 + (y - 4) * 0.25  # Wider going down
        for x in range(int(5 - half), int(5 + half) + 1):  # Pixels of the row
            la.put(img, x, y, mm.SCARF_SH if x > 5 else mix(mm.SCARF_SH, mm.LEAF))  # Red-green cup
    mm.fill_rect(img, 3, 3, 5, 1, mm.HELMET_SH)  # Rim
    mm.fill_rect(img, 4, 1, 4, 2, mm.LEAF)  # Lid
    return la.outline(img)  # Dark edge


# ---------- Glacier Horn: Matterhorn (H5) ----------

def glacier_top():
    """Hard snow with a blue icy shine."""
    return na.ground_top(lm.MOUNTAIN_SH, na.SNOW, mm.WHITE, ICE, [(4, 2, ICE), (10, 3, ICE), (13, 1, mm.WHITE), (7, 6, lm.MOUNTAIN)])  # Snow and ice


def glacier_soil():
    """Grey frozen rock with small ice crystals."""
    return na.soil_tile(mix(lm.MOUNTAIN_SH, mm.MASK_SH), mm.STRAP, [(3, 4, ICE), (4, 4, ICE), (11, 9, ICE), (7, 13, lm.HAZE), (13, 2, lm.HAZE)])  # Rock with ice crystals


def ledge_glacier():
    """A slab of blue ice."""
    return na.ledge_tile(ICE_SH, ICE, lm.MOUNTAIN_SH, [(3, 4, ICE), (9, 3, mm.WHITE), (12, 6, lm.MOUNTAIN)])  # Ice with shine


def glacier_boulder(w, h):
    """A grey rock with a snow cap."""
    return na.capped_boulder(w, h, lm.MOUNTAIN, lm.MOUNTAIN_SH, lm.HAZE, mm.STRAP, na.SNOW, 4)  # Snowy rock


def alpine_hut():
    """A 56x48 stone-and-wood alpine hut with a steep roof and a door in the middle (the H5 goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    for y in range(2, 22):  # Steep roof rows
        half = 4 + (y - 2) * 1.2  # Wider going down
        for x in range(int(28 - half), int(28 + half)):  # Pixels of the row
            la.put(img, x, y, mm.STRAP if x < 28 else mm.OUTLINE)  # Dark roof
    mm.fill_rect(img, 5, 20, 46, 2, na.SNOW)  # Snow on the roof edge
    mm.fill_rect(img, 8, 22, 40, 26, mm.GROUND)  # Wooden wall
    mm.fill_rect(img, 8, 38, 40, 10, lm.MOUNTAIN)  # Stone base
    mm.fill_rect(img, 22, 30, 12, 18, mm.OUTLINE)  # Open door
    mm.fill_rect(img, 12, 27, 6, 6, mm.HELMET)  # Warm lit window
    mm.fill_rect(img, 38, 27, 6, 6, mm.HELMET)  # Second window
    mm.fill_rect(img, 11, 27, 1, 6, mm.SCARF)  # Red shutter
    mm.fill_rect(img, 44, 27, 1, 6, mm.SCARF)  # Red shutter
    return la.outline(img)  # Dark edge


def edelweiss():
    """A 10x10 small white mountain flower (decoration only)."""
    img = mm.new_image(10, 10)  # Empty picture
    mm.fill_rect(img, 5, 5, 1, 5, mm.HILLS_SH)  # Stem
    for (x, y) in ((5, 1), (3, 3), (7, 3), (4, 5), (6, 5)):  # Five petals
        la.put(img, x, y, mm.WHITE)  # Petal
    la.put(img, 5, 3, mm.HELMET)  # Middle
    return la.outline(img)  # Dark edge


# ---------- Equator Summit: Kilimanjaro (H6) ----------

def equator_top():
    """Grey-brown volcanic scree with small stones."""
    return na.ground_top(mix(SCREE, mm.STRAP), SCREE, SCREE_LT, mm.VEST_SH, [(3, 2, mm.STRAP), (8, 1, SCREE_LT), (12, 3, mm.STRAP), (6, 6, mm.MASK_SH)])  # Scree


def equator_soil():
    """Dark volcanic gravel."""
    return na.soil_tile(mix(SCREE, mm.STRAP), mm.STRAP, [(4, 3, SCREE_LT), (12, 8, SCREE_LT), (8, 13, mm.MASK_SH)])  # Gravel


def ledge_equator():
    """A slab of brown trachyte lava rock."""
    return na.ledge_tile(mm.VEST_SH, SCREE_LT, mix(mm.VEST_SH, mm.OUTLINE), [(3, 4, mm.VEST), (10, 5, mm.STRAP), (7, 1, mm.MASK)])  # Brown rock


def equator_boulder(w, h):
    """A brown volcanic boulder."""
    return la.boulder_colored(w, h, mm.VEST_SH, mix(mm.VEST_SH, mm.OUTLINE), mm.VEST, mm.STRAP)  # Brown rock


def summit_board():
    """A 48x40 wooden summit sign board on two posts, with a star on it (the H6 goal)."""
    img = mm.new_image(48, 40)  # Empty picture
    mm.fill_rect(img, 8, 18, 3, 22, lm.HANDLE)  # Left post
    mm.fill_rect(img, 37, 18, 3, 22, lm.HANDLE)  # Right post
    mm.fill_rect(img, 2, 2, 44, 20, mm.HILLS_SH)  # Green board
    mm.fill_rect(img, 2, 2, 44, 2, mm.HILLS)  # Lit top
    mm.fill_rect(img, 4, 6, 40, 1, mm.HELMET)  # Yellow line
    mm.fill_rect(img, 4, 17, 40, 1, mm.HELMET)  # Yellow line
    la.draw_grid(img, ["..#..", ".###.", "#####", ".###.", ".#.#."], 21, 9, {"#": mm.HELMET})  # Star
    return la.outline(img)  # Dark edge


def groundsel():
    """A 20x40 giant groundsel plant of Kilimanjaro: a thick stem with a leaf rosette on top (decoration only)."""
    img = mm.new_image(20, 40)  # Empty picture
    mm.fill_rect(img, 8, 12, 4, 28, mm.GROUND_SH)  # Thick stem of old leaves
    for y in range(14, 40, 3):  # Dry leaf rings
        mm.fill_rect(img, 8, y, 4, 1, mm.VEST_SH)  # One ring
    for k in range(8):  # Leaves of the rosette
        a = math.pi + k * math.pi / 7  # Spread over the top half
        for t in range(8):  # Along the leaf
            la.put(img, int(10 + math.cos(a) * t), int(10 + math.sin(a) * t * 0.7), mm.HILLS if k % 2 else mm.HILLS_SH)  # One leaf pixel
    return la.outline(img)  # Dark edge


# ---------- Sea Peak: Mauna Kea (H7) ----------

def sea_top():
    """Red-brown cinder."""
    return na.ground_top(CINDER_DK, CINDER, CINDER_LT, CINDER_DK, [(3, 2, CINDER_DK), (8, 1, CINDER_LT), (12, 3, CINDER_DK), (6, 6, mm.STRAP)])  # Cinder grains


def sea_soil():
    """Dark red-brown cinder with black grains."""
    return na.soil_tile(CINDER_DK, mm.OUTLINE, [(4, 4, CINDER), (11, 9, CINDER), (7, 13, mm.STRAP)])  # Cinder


def ledge_sea():
    """A slab of dark lava with red cinder on top."""
    return na.ledge_tile(la.BASALT, CINDER, la.BASALT_SH, [(3, 4, la.BASALT_LT), (10, 5, la.LAVA_ROCK_SH), (7, 1, CINDER_LT)])  # Lava and cinder


def sea_boulder(w, h):
    """A dark lava boulder with cinder on top."""
    return na.capped_boulder(w, h, la.BASALT, la.BASALT_SH, la.BASALT_LT, la.LAVA_ROCK_SH, CINDER, 2)  # Lava with cinder


def telescope_dome():
    """A 56x48 white telescope dome with an open slit and a door in the middle (the H7 goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    for y in range(4, 26):  # Dome rows
        half = math.sqrt(max(0, 22 ** 2 - (26 - y) ** 2))  # Half circle
        for x in range(int(28 - half), int(28 + half)):  # Pixels of the row
            la.put(img, x, y, mm.WHITE if x < 32 else mm.MASK)  # Lit left, shaded right
    mm.fill_rect(img, 25, 4, 6, 22, mm.OUTLINE)  # Open slit
    mm.fill_rect(img, 26, 12, 4, 4, lm.MOUNTAIN)  # Telescope seen in the slit
    mm.fill_rect(img, 6, 26, 44, 22, mm.MASK)  # Round building
    mm.fill_rect(img, 30, 26, 20, 22, mm.MASK_SH)  # Shaded side
    mm.fill_rect(img, 22, 32, 12, 16, mm.OUTLINE)  # Open door
    return la.outline(img)  # Dark edge


def silversword():
    """A 16x16 silver, spiky ball plant that grows on Mauna Kea (decoration only)."""
    img = mm.new_image(16, 16)  # Empty picture
    for k in range(12):  # Spiky leaves all round
        a = k * math.pi / 6  # Angle of the leaf
        for t in range(7):  # Along the leaf
            y = int(10 + math.sin(a) * t * 0.8)  # Up or down
            if y < 16:  # Stay above the ground
                la.put(img, int(8 + math.cos(a) * t), y, lm.SNOW if k % 2 else lm.HAZE)  # Silver leaf
    return la.outline(img)  # Dark edge


# ---------- Hard level objects: rolling boulder and icy ground ----------

def rolling_boulder(frame):
    """A 16x16 round boulder; the crack line turns 45 degrees each frame (0 to 3) as it rolls left."""
    img = mm.new_image(16, 16)  # Empty picture
    for y in range(16):  # Each row
        for x in range(16):  # Each pixel
            d = math.hypot(x - 7.5, y - 7.5)  # Distance from the middle
            if d <= 7.2:  # Inside the round boulder
                shade = (x - 7.5) * 0.7 + (y - 7.5) > 3  # Lower right in shade
                la.put(img, x, y, la.BASALT_SH if shade else la.BASALT)  # Rock
                if d > 5 and (x - 7.5) + (y - 7.5) < -5:  # Sunny top left edge
                    la.put(img, x, y, la.BASALT_LT)  # Light rock
    a = -frame * math.pi / 4  # Turns to the left (anticlockwise) as it rolls left
    for t in range(-5, 6):  # Along the crack
        la.put(img, int(round(7.5 + math.cos(a) * t)), int(round(7.5 + math.sin(a) * t)), mm.OUTLINE)  # Crack pixel
    return la.outline(img)  # Dark edge


def rumble_dust(frame):
    """A 16x8 dust puff that warns a boulder is coming (frames 0 and 1 grow)."""
    img = mm.new_image(16, 8)  # Empty picture
    for (cx, cy, r) in ((4, 5, 2 + frame), (10, 4, 2 + frame), (14, 6, 1 + frame)):  # Three puffs
        for y in range(8):  # Each row
            for x in range(16):  # Each pixel
                if (x - cx) ** 2 + (y - cy) ** 2 <= r * r:  # Inside the puff
                    la.put(img, x, y, mm.VEST if (x + y) % 3 else mm.VEST_SH, 200)  # See-through tan dust
    return img


def ice_top():
    """A 16x8 icy ground-top tile: pale blue ice with white glints. It replaces the theme's top tile on icy ground."""
    img = mm.new_image(16, 8)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 8, ICE_SH)  # Ice body
    mm.fill_rect(img, 0, 0, 16, 4, ICE)  # Bright ice cap
    mm.fill_rect(img, 0, 0, 16, 1, mm.WHITE)  # Shiny top line
    for (x, y) in ((3, 2), (4, 1), (11, 2), (12, 2)):  # Glints
        la.put(img, x, y, mm.WHITE)  # One glint
    mm.fill_rect(img, 0, 4, 16, 1, lm.MOUNTAIN)  # Shadow line
    return img


def ice_glint(frame):
    """A 5x5 twinkle that flashes on icy ground (frames 0 and 1)."""
    img = mm.new_image(5, 5)  # Empty picture
    r = 1 + frame  # Bigger in frame 1
    for t in range(-r, r + 1):  # Plus shape
        la.put(img, 2 + t, 2, mm.WHITE)  # Across
        la.put(img, 2, 2 + t, mm.WHITE)  # Down
    return img


# ---------- Items (24x24, Assets/Art/Items/Sprites/) ----------

def scoria():
    """Red-brown scoria full of big gas holes."""
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x, y) < 18:  # Many gas holes
            return mm.OUTLINE  # Hole
        return CINDER_DK if s else CINDER  # Red-brown rock
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10, 7.5, c, 2.2))  # Lumpy rock


def hornblende():
    """Black, shiny hornblende crystals shaped like long prisms."""
    img = mm.new_image(24, 24)  # Empty picture
    for (x0, y0, x1, y1) in ((5, 18, 13, 4), (10, 19, 19, 8)):  # Two prisms
        ls.fill_poly(img, [(x0, y0), (x1, y1), (x1 + 4, y1 + 2), (x0 + 4, y0 + 2)], mm.STRAP)  # One prism
        for t in range(8):  # Shine line along the prism
            la.put(img, x0 + 1 + (x1 - x0) * t // 8, y0 + (y1 - y0) * t // 8, lm.MOUNTAIN)  # Shine pixel
    return ls.outline(img)  # Dark edge


def diorite():
    """Diorite: a salt-and-pepper rock of white and black grains."""
    def c(x, y, s):  # Colour of one pixel
        n = lm.noise(x, y)  # Fixed random number
        if n < 35:  # Black grains
            return mm.OUTLINE if s else mm.STRAP  # Dark grain
        return mm.MASK if s else mm.WHITE  # White grain
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 2.6))  # Speckled rock


def gold():
    """A shiny gold nugget in white quartz."""
    img = na.blob(24, 24, 11.5, 14, 10, 7, lambda x, y, s: mm.MASK if s else mm.WHITE, 2.4)  # White quartz
    for (cx, cy, r) in ((10, 12, 3), (14, 15, 2), (8, 16, 1)):  # Gold blobs
        ls.disc(img, cx, cy, r, mm.HELMET)  # Gold
    la.put(img, 9, 11, mm.HEADLAMP)  # Glint
    la.put(img, 14, 14, mm.HEADLAMP)  # Glint
    return ls.outline(img)  # Dark edge


def tuff():
    """Pale tuff: volcanic ash pressed into soft rock, with small bits of pumice."""
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x, y) < 8:  # A few bits of pumice and rock
            return mm.MASK_SH  # Grey bit
        return mm.VEST_SH if s else mix(mm.VEST, mm.MASK)  # Pale beige ash rock
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 2.8))  # Soft-looking rock


def hematite():
    """Red-black hematite with a red streak beside it."""
    img = na.blob(24, 24, 10.5, 12, 8.5, 7, lambda x, y, s: mm.OUTLINE if s else mm.STRAP, 2.2)  # Dark metal lump
    for (x, y) in ((8, 9), (9, 8), (10, 8)):  # Metal shine
        la.put(img, x, y, lm.MOUNTAIN)  # Shine
    mm.fill_rect(img, 5, 20, 15, 2, mm.SCARF_SH)  # Red streak it leaves on a tile
    return ls.outline(img)  # Dark edge


def serpentinite():
    """Green serpentinite with smooth, snake-skin streaks."""
    def c(x, y, s):  # Colour of one pixel
        band = ((x + y + la.wave(x, 2, 10)) // 3) % 3  # Wavy streaks
        return [mm.LEAF, SERP, mm.HILLS_SH][band] if not s else [CANOPY_DK, mix(SERP, mm.OUTLINE), mm.LEAF][band]  # Greens
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 2.4))  # Smooth green rock


def chromite():
    """Black chromite grains in a lump, with a dull metal shine."""
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x, y) < 15:  # Grains catch the light
            return lm.MOUNTAIN_SH  # Grey shine
        return mm.OUTLINE if s else mm.STRAP  # Black
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 9.5, 7.5, c, 2.0))  # Heavy black lump


def schist():
    """Silvery schist with shiny wavy layers."""
    def c(x, y, s):  # Colour of one pixel
        band = ((y + la.wave(x, 1, 8)) // 2) % 2  # Thin wavy layers
        if lm.noise(x, y) < 6:  # Mica sparkles
            return mm.WHITE  # Sparkle
        return (lm.MOUNTAIN, lm.HAZE)[band] if not s else (lm.MOUNTAIN_SH, lm.MOUNTAIN)[band]  # Silver-grey layers
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10.5, 7, c, 3))  # Flat layered rock


def calcite():
    """A clear-white calcite crystal shaped like a leaning box."""
    img = mm.new_image(24, 24)  # Empty picture
    ls.fill_poly(img, [(6, 8), (15, 5), (19, 15), (10, 18)], mix(mm.WHITE, mm.HEADLAMP))  # Front face
    ls.fill_poly(img, [(15, 5), (19, 4), (22, 13), (19, 15)], mm.MASK)  # Side face
    for t in range(6):  # A shine line
        la.put(img, 8 + t, 9 + t // 2, mm.WHITE)  # Shine pixel
    return ls.outline(img)  # Dark edge


def trachyte():
    """Grey-brown trachyte lava rock with small light crystals."""
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x, y) < 7:  # Small light crystals
            return mm.MASK  # Crystal
        return mix(mm.VEST_SH, mm.STRAP) if s else SCREE  # Grey-brown rock
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 2.6))  # Blocky rock


def tanzanite():
    """A blue-violet tanzanite gem."""
    img = mm.new_image(24, 24)  # Empty picture
    ls.fill_poly(img, [(6, 9), (18, 9), (12, 21)], lm.MOUNTAIN)  # Lower gem, blue
    ls.fill_poly(img, [(8, 5), (16, 5), (18, 9), (6, 9)], lm.HAZE)  # Lit top, pale blue
    ls.fill_poly(img, [(12, 9), (18, 9), (12, 21)], lm.MOUNTAIN_SH)  # Shaded right side, deep blue
    for (x, y) in ((10, 12), (11, 14), (9, 10)):  # A violet glow inside
        la.put(img, x, y, mix(lm.MOUNTAIN, mm.SCARF_SH))  # Violet pixel
    la.put(img, 10, 6, mm.WHITE)  # Glint
    return ls.outline(img)  # Dark edge


def gabbro():
    """Dark gabbro with big black and grey-green grains."""
    def c(x, y, s):  # Colour of one pixel
        n = lm.noise(x // 2, y // 2)  # Big grains, 2 pixels wide
        if n < 40:  # Light grains
            return SERP if not s else mix(SERP, mm.OUTLINE)  # Grey-green grain
        return mm.STRAP if not s else mm.OUTLINE  # Black grain
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 2.6))  # Coarse dark rock


def olivine():
    """Olive-green olivine grains, like the green sand of Hawaii."""
    img = mm.new_image(24, 24)  # Empty picture
    for (cx, cy, r) in ((9, 11, 4), (15, 12, 3), (11, 17, 3), (17, 17, 2), (6, 17, 2)):  # Green grains
        ls.disc(img, cx, cy, r, mix(mm.HILLS, mm.HELMET_SH))  # Olive green grain
        la.put(img, cx - 1, cy - 1, mm.PLAINS)  # Shine
    return ls.outline(img)  # Dark edge


# ---------- Lesson pictures (120x82, Assets/Art/Lesson/Sprites/) ----------

def mayon_pixel(x, y):
    """Mount Mayon: a near-perfect cone with lava glowing down one side and rice fields below."""
    half = (y - 10) * 1.05  # The cone widens going down
    if y >= 70:  # Rice fields at the foot
        return mm.HILLS if (x // 6 + y // 3) % 2 else mm.PLAINS  # Green paddies
    if y >= 10 and abs(x - 59) <= half:  # Inside the cone
        if abs(x - 63 - (y - 10) * 0.15) < 1.5 and y < 40:  # A thin lava stream from the top
            return GLOW  # Glowing lava
        if y < 14:  # Grey ash at the top
            return mm.MASK_SH  # Ash
        return lm.MOUNTAIN if x < 59 else lm.MOUNTAIN_SH  # Grey-blue slopes
    return na.sky(x, y)  # Sky


def sierra_madre_pixel(x, y):
    """Sierra Madre: long green ridges along a coast, with storm clouds coming from the sea on the right."""
    if x > 96:  # The sea on the right
        return SEA if y > 40 else na.sky(x, y)  # Sea below the horizon
    top = 34 + int(5 * math.sin(x / 11.0)) + int(2 * math.sin(x / 4.0))  # A long wavy ridge line
    if y >= top:  # Inside the near range
        return mm.LEAF if (x + y) % 6 else CANOPY_DK  # Forest
    if y >= top - 8:  # A far ridge behind
        return mm.HILLS_SH  # Far green ridge
    return na.sky(x, y)  # Sky


def kitanglad_pixel(x, y):
    """Kitanglad Range: tall forest ridges with a Philippine eagle gliding above."""
    top, peak = lm.ridge(x, [(40, 18, 0.8), (88, 24, 0.7)])  # Two forest peaks
    if y >= top:  # Inside a mountain
        if y < top + 4:  # Treetops on the ridge
            return mm.HILLS_SH  # Lit treetops
        return CANOPY if x < peak[0] else CANOPY_DK  # Forest
    return na.sky(x, y)  # Sky


def hamiguitan_pixel(x, y):
    """Mount Hamiguitan: a rounded mountain with a red-soil top where tiny bonsai trees grow."""
    d = abs(x - 59)  # Distance from the middle
    top = 22 + int(d * d / 90.0)  # A rounded top
    if y >= top:  # Inside the mountain
        if y < top + 9 and d < 30:  # The pygmy forest on red soil near the top
            return LATERITE_LT if (x + y) % 5 else mm.HILLS_SH  # Red soil with small green dots of trees
        return CANOPY if x < 59 else CANOPY_DK  # Tall forest lower down
    return na.sky(x, y)  # Sky


def matterhorn_pixel(x, y):
    """The Matterhorn: a sharp horn peak carved by glaciers, with a glacier at its foot."""
    top, peak = lm.ridge(x, [(62, 6, 2.0), (20, 44, 0.8), (108, 46, 0.8)])  # A steep horn and two low neighbours
    if y >= 66:  # The glacier at the foot
        return ICE if (x + y) % 7 else ICE_SH  # Ice
    if y >= top:  # Inside a mountain
        if lm.noise(x, y) < 25 or y < top + 2:  # Snow patches and snowy edges
            return na.SNOW if x < peak[0] else na.SNOW_SH  # Snow
        return lm.MOUNTAIN if x < peak[0] else lm.MOUNTAIN_SH  # Bare rock
    return na.sky(x, y)  # Sky


def kilimanjaro_pixel(x, y):
    """Kilimanjaro: a wide flat-topped mountain with a snow cap above green slopes and a yellow savanna."""
    d = abs(x - 59)  # Distance from the middle
    top = 18 if d < 16 else 18 + (d - 16) * 0.6  # Flat top, gentle sides
    if y >= 66:  # The savanna
        return mm.HELMET_SH if (x + y) % 4 else mm.VEST_SH  # Yellow grass
    if y >= top:  # Inside the mountain
        if y < 24:  # Snow and ice cap
            return na.SNOW  # Snow
        if y < 40:  # Bare rocky top
            return SCREE if x < 59 else mix(SCREE, mm.STRAP)  # Rock
        return mm.LEAF if (x + y) % 5 else CANOPY_DK  # Forest on the lower slopes
    return na.sky(x, y)  # Sky


def mauna_kea_pixel(x, y):
    """Mauna Kea: only its top shows above the sea; most of the mountain hides under the water."""
    d = abs(x - 59)  # Distance from the middle
    top = 14 + int(d * 0.55)  # A wide gentle shield
    sea = 34  # The sea surface
    if y >= top:  # Inside the mountain
        if y >= sea:  # The part under the sea
            return mix(lm.MOUNTAIN_SH, SEA_DEEP) if x < 59 else mix(mm.STRAP, SEA_DEEP)  # Dark rock seen through water
        return CINDER_LT if x < 59 else CINDER  # Red-brown top above water
    if y >= sea:  # Open water
        return SEA_DEEP if y > 60 else SEA  # Sea, deeper at the bottom
    return na.sky(x, y)  # Sky


def sierra_madre_lesson():
    """Sierra Madre with dark storm clouds rolling in from the sea and rain under them."""
    img = ls.framed(sierra_madre_pixel)  # The ridges and the sea
    for (cx, cy, r) in ((84, 12, 7), (96, 9, 8), (108, 13, 7), (74, 16, 5), (114, 18, 5)):  # Cloud puffs
        ls.disc(img, cx + 1, cy + 1, r, mm.STRAP)  # Dark underside
        ls.disc(img, cx, cy, r, mm.MASK_SH)  # Grey cloud
    for k in range(10):  # Rain streaks under the clouds
        for t in range(6):  # Along a streak
            la.put(img, 76 + k * 4 + t // 3, 22 + (k * 5) % 9 + t, lm.HAZE)  # One rain pixel
    return img


def mayon_lesson():
    """Mount Mayon with an ash plume."""
    return na.lesson_with_puffs(mayon_pixel, [(60, 7, 3), (64, 5, 3), (69, 4, 2)])  # Ash plume above the top


def kitanglad_lesson():
    """Kitanglad with a gliding eagle."""
    img = ls.framed(kitanglad_pixel)  # The mountains
    la.draw_grid(img, ["#.......#", ".##...##.", "..#####..", "....#...."], 60, 8, {"#": lm.BACKPACK})  # Eagle shape
    return img


def mauna_kea_lesson():
    """Mauna Kea with a white telescope dome on the top and a dotted line marking the sea floor."""
    img = ls.framed(mauna_kea_pixel)  # The mountain and sea
    ls.disc(img, 60, 15, 2, mm.WHITE)  # Telescope dome
    for x in range(2, 119, 3):  # The sea floor line at the bottom
        la.put(img, x, 80, mm.VEST_SH)  # One dot
    return img


# ---------- Landform Description Screen (Assets/Art/Description/Sprites/) ----------

BOOK = ["#######.", "#.....##", "#.##.#.#", "#.....##", "#.##.#.#", "#.....##", "#######."]  # An open-book icon for the About button


def scroll_panel(w=300, h=196):
    """A cream paper scroll panel with rolled ends at the top and bottom."""
    img = mm.new_image(w, h)  # Empty picture
    mm.fill_rect(img, 4, 8, w - 8, h - 16, mm.HEADLAMP)  # Paper
    mm.fill_rect(img, 4, 8, 2, h - 16, mm.VEST)  # Left edge shade
    mm.fill_rect(img, w - 6, 8, 2, h - 16, mm.VEST)  # Right edge shade
    for y0 in (0, h - 10):  # Top and bottom rolls
        mm.fill_rect(img, 0, y0, w, 10, mm.VEST)  # Roll
        mm.fill_rect(img, 0, y0, w, 3, mix(mm.VEST, mm.HEADLAMP))  # Lit top of the roll
        mm.fill_rect(img, 0, y0 + 8, w, 2, mm.VEST_SH)  # Shade under the roll
    return la.outline(img)  # Dark edge


def unlock_padlock(frame):
    """The map padlock opening: frame 0 shut, 1 shackle up, 2 shackle swung open, 3 breaking up as it fades."""
    body = list(lm.PADLOCK)[4:]  # The padlock body rows
    shackles = [list(lm.PADLOCK)[:4],  # Shut
                ["..ssss..", ".s....s.", ".s....s.", ".s......"],  # Lifted, right side free
                ["ssss....", "s..s....", "s.......", "s......."]]  # Swung open to the left
    rows = ["........"] + shackles[min(frame, 2)] + body  # One empty row on top so every frame is the same size
    if frame == 1:  # The lifted shackle sits one row higher
        rows = shackles[1] + ["........"] + body  # Lifted up
    img = lm.pattern(rows, {"s": mm.MASK, "b": mm.HELMET_SH, "h": mm.HELMET, "k": mm.OUTLINE}, 2)  # Draw at 2x
    if frame == 3:  # Breaking up as it fades
        for y in range(img.height):  # Each row
            for x in range(img.width):  # Each pixel
                if lm.noise(x, y) % 2:  # Half the pixels vanish
                    img.putpixel((x, y), (0, 0, 0, 0))  # See-through
    return img


def sparkle(frame):
    """A 9x9 golden sparkle for the unlock (frames 0 to 2 grow)."""
    img = mm.new_image(9, 9)  # Empty picture
    r = 1 + int(frame * 1.5)  # Longer rays each frame
    for t in range(-r, r + 1):  # Plus shape
        la.put(img, 4 + t, 4, mm.HELMET)  # Across
        la.put(img, 4, 4 + t, mm.HELMET)  # Down
    la.put(img, 4, 4, mm.WHITE)  # Bright middle
    return img


def main():
    themes = {  # Sprite name: drawing function, for the seven new themes and the new level objects
        "lava_top": lava_top, "lava_soil": lava_soil, "ledge_lava": ledge_lava, "watch_station": watch_station,
        "storm_top": storm_top, "storm_soil": storm_soil, "ledge_storm": ledge_storm, "lookout_tower": lookout_tower,
        "eagle_top": eagle_top, "eagle_soil": eagle_soil, "ledge_eagle": ledge_eagle, "eagle_nest": eagle_nest,
        "bonsai_top": bonsai_top, "bonsai_soil": bonsai_soil, "ledge_bonsai": ledge_bonsai, "sanctuary_sign": sanctuary_sign,
        "glacier_top": glacier_top, "glacier_soil": glacier_soil, "ledge_glacier": ledge_glacier, "alpine_hut": alpine_hut,
        "equator_top": equator_top, "equator_soil": equator_soil, "ledge_equator": ledge_equator, "summit_board": summit_board,
        "sea_top": sea_top, "sea_soil": sea_soil, "ledge_sea": ledge_sea, "telescope_dome": telescope_dome,
        "ice_top": ice_top,
    }
    for name, draw in themes.items():  # Each sprite
        la.save(draw(), name + ".png")  # Write it
    boulders = (("_lava", lava_boulder), ("_storm", storm_boulder), ("_eagle", eagle_boulder), ("_bonsai", bonsai_boulder),
                ("_glacier", glacier_boulder), ("_equator", equator_boulder), ("_sea", sea_boulder))  # Each theme's boulders
    for ending, draw in boulders:  # Each theme
        la.save(draw(32, 24), "boulder_small%s.png" % ending)  # Small boulder
        la.save(draw(48, 36), "boulder_big%s.png" % ending)  # Big boulder
    for i in range(4):  # Four-frame objects
        la.save(rolling_boulder(i), "roll_boulder_%02d.png" % i)  # Rolling boulder
    for i in range(2):  # Two-frame objects
        la.save(rumble_dust(i), "Props/rumble_dust_%02d.png" % i)  # Dust warning before a boulder
        la.save(ice_glint(i), "Props/ice_glint_%02d.png" % i)  # Twinkle on ice
        la.save(rain(i), "Props/rain_%02d.png" % i)  # Rain streak
    props = {"burnt_tree": burnt_tree, "bent_tree": bent_tree, "tall_tree": tall_tree, "orchid": orchid,
             "bonsai_tree": bonsai_tree, "pitcher_plant": pitcher_plant, "edelweiss": edelweiss,
             "groundsel": groundsel, "silversword": silversword}  # Living props, decoration only
    for name, draw in props.items():  # Each prop
        la.save(draw(), "Props/%s.png" % name)  # Write it
    items = {"scoria": scoria, "hornblende": hornblende, "diorite": diorite, "gold": gold, "tuff": tuff,
             "hematite": hematite, "serpentinite": serpentinite, "chromite": chromite, "schist": schist,
             "calcite": calcite, "trachyte": trachyte, "tanzanite": tanzanite, "gabbro": gabbro, "olivine": olivine}  # The fourteen new kinds
    for name, draw in items.items():  # Each new kind
        ls.save(draw(), ls.ITEMS_OUT, name + ".png")  # Write it to the shared item folder
    ls.save(mayon_lesson(), ls.LESSON_OUT, "lesson_mayon.png")  # Mount Mayon
    ls.save(sierra_madre_lesson(), ls.LESSON_OUT, "lesson_sierra_madre.png")  # Sierra Madre
    ls.save(kitanglad_lesson(), ls.LESSON_OUT, "lesson_kitanglad.png")  # Kitanglad Range
    ls.save(ls.framed(hamiguitan_pixel), ls.LESSON_OUT, "lesson_hamiguitan.png")  # Mount Hamiguitan
    ls.save(ls.framed(matterhorn_pixel), ls.LESSON_OUT, "lesson_matterhorn.png")  # The Matterhorn
    ls.save(ls.framed(kilimanjaro_pixel), ls.LESSON_OUT, "lesson_kilimanjaro.png")  # Kilimanjaro
    ls.save(mauna_kea_lesson(), ls.LESSON_OUT, "lesson_mauna_kea.png")  # Mauna Kea
    brown = (mm.PLATEAU, mm.PLATEAU_SH, mm.BACKPACK_SH)  # Same colours as the Back buttons
    green = (mm.HILLS, mm.HILLS_SH, mm.LEAF)  # Same colours as the Play button
    ls.save(scroll_panel(), DESC_OUT, "scroll_panel.png")  # Summary panel
    for pressed in (False, True):  # Normal and pressed pictures
        tag = "_pressed" if pressed else ""  # File name ending
        ls.save(la.icon_word_button("TO THE MAP", la.MAP, green, pressed), DESC_OUT, "btn_to_map%s.png" % tag)  # To the Map button
        ls.save(camp.icon_button(BOOK, brown[1], brown[0], brown[2], pressed), DESC_OUT, "btn_book_icon%s.png" % tag)  # About Mountains book button
    for i in range(4):  # Padlock opening frames for the map unlock
        ls.save(unlock_padlock(i), DESC_OUT, "unlock_padlock_%02d.png" % i)  # One frame
    for i in range(3):  # Sparkle frames
        ls.save(sparkle(i), DESC_OUT, "sparkle_%02d.png" % i)  # One frame
    print("Hard art written to", la.OUT, ls.ITEMS_OUT, ls.LESSON_OUT, "and", DESC_OUT)  # Tell the person running it where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
