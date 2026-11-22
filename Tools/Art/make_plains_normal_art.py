"""Draws the PNGs for Plains Normal 1 to 5 (D-070): five level themes (Candaba Swamp, Lahar Plain, River Delta,
Agusan Marsh and Mekong Delta) with their goals, the moving market boat, the new props, two new items and five
lesson pictures.

Run from the Unity project folder:  python Tools/Art/make_plains_normal_art.py
It writes to Assets/Art/Level/Sprites/ (themes, goals, boat and props), Assets/Art/Items/Sprites/ (items)
and Assets/Art/Lesson/Sprites/ (lesson pictures), plus a preview sheet at .scratch/plains-normal/art-preview.png.
It reuses the helpers of make_mainmenu_art.py, make_landform_map_art.py, make_level_art.py, make_lesson_art.py and make_normal_art.py.
All colours come from Assets/Art/Palettes/landgrand.gpl or mix two of its colours.
Props/bangka.png and lesson_pinatubo.png already belong to older art, so the new ones are saved as
Props/bangka_plains.png and lesson_pinatubo_lahar.png instead of overwriting them.
"""
import math  # math gives the waves and the river bends
import os  # os joins folder paths
from PIL import Image  # Image builds the preview sheet
import make_mainmenu_art as mm  # Palette, new_image, fill_rect and paste
import make_landform_map_art as lm  # Map colours, the fixed noise and the ridge helper
import make_level_art as la  # put, outline, wave, draw_grid and save for level sprites
import make_lesson_art as ls  # framed, disc, fill_poly, put and the item and lesson folders
import make_normal_art as na  # mix, the shared tile makers, sky and the blob shape

mix = na.mix  # The colour halfway between two palette colours
PREVIEW = os.path.join(mm.ROOT, ".scratch", "plains-normal", "art-preview.png")  # Where the preview sheet goes

# Candaba Swamp (N1): wet marsh grass, dark humus, a wooden boardwalk and mossy rock.
CAN_CAP = mm.HILLS_SH  # Wet green marsh grass
CAN_LIGHT = mm.HILLS  # Sunny marsh grass
CAN_SHADE = mm.LEAF  # Marsh grass in shade
CAN_SOIL = mix(mm.GROUND_SH, mm.OUTLINE)  # Dark wet humus
CAN_SOIL_LT = mix(mm.GROUND_SH, mm.BACKPACK_SH)  # Lighter humus layer
CAN_SHINE = mm.SKY_LIGHT  # Water shine in the marsh
WOOD_LT = mm.VEST  # Light boardwalk plank
WOOD = mm.GROUND  # Plank body
WOOD_DK = mm.GROUND_SH  # Plank in shade
STONE_DK = mm.STRAP  # Dark mossy rock
STONE_SH = mm.OUTLINE  # Rock in deep shade
STONE_LT = lm.MOUNTAIN_SH  # Rock in the light
THATCH = mix(mm.VEST, mm.HELMET_SH)  # Nipa roof
THATCH_SH = mm.VEST_SH  # Nipa roof in shade
BAMBOO = mm.PLAINS  # Pale bamboo
BAMBOO_SH = lm.PLAINS_SH  # Bamboo in shade
WATER = lm.LENS  # Calm water blue
WATER_LT = mm.SKY_LIGHT  # Water shine
WATER_DK = mix(lm.MOUNTAIN_SH, mm.LEAF)  # Deep water in shade
WATER_MARSH = mix(lm.LENS, mm.HILLS_SH)  # Green-blue shallow marsh water

# Lahar Plain (N2): pale grey ash-sand, layered lahar sand, pumice and a concrete dike.
LAHAR_CAP = mm.MASK  # Pale grey ash-sand top
LAHAR_LT = mix(mm.MASK, mm.WHITE)  # Sunny ash
LAHAR_SAND = mix(mm.MASK_SH, mm.VEST_SH)  # Grey layered sand
LAHAR_DK = mm.MASK_SH  # Grey crack and shade
PUMICE = mm.HEADLAMP  # Light pumice bits
CONCRETE = mm.MASK  # Concrete dike
CONCRETE_SH = mm.MASK_SH  # Concrete in shade
CONCRETE_LT = mix(mm.MASK, mm.WHITE)  # Sunny concrete
CRACK = mm.STRAP  # Dark crack in the ash
WARN_RED = mm.SCARF  # Red warning border
WARN_BLACK = mm.STRAP  # Dark mud-wave line

# River Delta (N3): green grass over sandy silt, tan and brown layers, a bamboo walkway and smooth cobbles.
SILT = mix(mm.VEST, mm.GROUND)  # Sandy silt under the grass
GRASS = mm.HILLS  # Green grass
GRASS_LT = mm.PLAINS  # Sunny grass
GRASS_SH = mm.HILLS_SH  # Grass in shade
SAND = mm.VEST  # Tan sand layer
SAND_SH = mm.VEST_SH  # Shaded sand
BROWN = mm.GROUND  # Brown silt
BROWN_DK = mm.GROUND_SH  # Dark brown speck
PEBBLE = mix(mm.MASK, mm.VEST)  # Smooth river stone
PEBBLE_SH = mix(mm.MASK_SH, mm.VEST_SH)  # River stone in shade

# Agusan Marsh (N4): dark swamp grass, black-brown peat, a bamboo raft walk and wet stones.
SWAMP = mm.LEAF  # Dark green swamp grass
SWAMP_SH = mix(mm.LEAF, mm.OUTLINE)  # Deep swamp shade
PEAT = mix(mm.BACKPACK_SH, mm.OUTLINE)  # Black-brown peaty soil
PEAT_FIBRE = mm.GROUND_SH  # Plant fibres in the peat
RAFT = mm.GROUND  # Bamboo raft
RAFT_LT = mm.VEST  # Sunny raft top
ROPE = mm.DARK  # Rope binding
WET_STONE = mix(mm.BACKPACK_SH, mm.OUTLINE)  # Wet dark stone
WET_STONE_LT = mix(mm.MASK_SH, mm.BACKPACK_SH)  # Wet stone highlight

# Mekong Delta (N5): red-brown river clay, rusty laterite blocks, a wooden jetty and fruit boats.
CLAY = mix(mm.PLATEAU_SH, mm.GROUND_SH)  # Red-brown river clay
CLAY_DK = mm.BACKPACK_SH  # Dark clay speck
CLAY_SILT = mm.VEST_SH  # Silt band in the clay
RUST = mm.PLATEAU_SH  # Rusty laterite block
RUST_LT = mm.PLATEAU  # Sunny laterite
RUST_SH = mm.BACKPACK_SH  # Laterite in shade
JETTY = mm.GROUND  # Jetty plank
JETTY_LT = mm.VEST  # Sunny jetty plank
RIVER = mix(mm.GROUND, mm.HILLS_SH)  # Brown-green river water
RIVER_RIPPLE = mm.VEST  # Light ripple on the river

# Boat, props and items.
FRUIT_YEL = mm.HELMET  # Yellow mango
FRUIT_YEL_SH = mm.HELMET_SH  # Mango in shade
FRUIT_GRN = mm.HILLS  # Green fruit
FRUIT_GRN_SH = mm.HILLS_SH  # Green fruit in shade
BOAT = mm.GROUND  # Wooden boat hull
BOAT_DK = mm.GROUND_SH  # Hull plank line
BOAT_DECK = mm.VEST  # Deck line the explorer stands on
PINK = mix(mm.SCARF, mm.WHITE)  # Lily pink
PINK_SH = mix(mm.SCARF, mm.MASK)  # Lily pink in shade
LILAC = mix(lm.MOUNTAIN, mm.WHITE)  # Pale lilac
LILAC_SH = lm.MOUNTAIN  # Lilac in shade
DUCK_HEAD = mm.LEAF  # Green duck head
DUCK_BODY = mm.GROUND  # Brown duck body
DUCK_WING = mm.BACKPACK_SH  # Dark wing
DUCK_BELLY = mm.VEST  # Pale belly and neck ring
CROC = mm.LEAF  # Crocodile back
CROC_LT = mm.HILLS_SH  # Crocodile scales
BEAK = mm.HELMET  # Yellow beak
AWNING_A = mm.SCARF  # Awning red stripe
AWNING_B = mm.HEADLAMP  # Awning cream stripe
BASKET = mm.VEST  # Woven basket
PEAT_BROWN = lm.BACKPACK  # Peat body
PEAT_DK = mm.BACKPACK_SH  # Peat in shade
LAT = mm.SCARF  # Laterite body
LAT_SH = mm.SCARF_SH  # Laterite in shade
LAT_IRON = mm.BACKPACK_SH  # Dark iron spot
EGRET_ROWS = ["...ww..", "..wwwy.", "...ww..", "..wwww.", ".wwwwww", "wwwwwww", ".wwwww.", "..www..", "..l.l..", "..l.l.."]  # Egret picture, 7 by 10
EGRET_COLORS = {"w": mm.WHITE, "y": BEAK, "l": mm.STRAP}  # What each egret letter paints

MOUTH_X, MOUTH_Y = 60, 50  # Middle column and top row of the Agno river mouth
PEAK_X, PEAK_Y = 58, 24  # Middle column and flat top row of the Pinatubo mountain


# ---------- Candaba Swamp (N1) ----------

def candaba_top():
    """A 16x8 tile of wet green marsh grass over dark humus, with a few water shine spots."""  # Describes the tile
    return na.ground_top(CAN_SOIL, CAN_CAP, CAN_LIGHT, CAN_SHADE,
                         [(2, 2, mm.PLAINS), (6, 1, mm.PLAINS), (11, 2, CAN_LIGHT), (14, 1, mm.PLAINS),
                          (4, 6, CAN_SHINE), (10, 5, CAN_SHINE)])  # Grass tips and water shine


def candaba_soil():
    """A 16x16 dark wet humus tile with pale wavy layers, plant bits and a little water shine."""  # Describes the tile
    layers = [(x, 5 + la.wave(x, 1), CAN_SOIL_LT) for x in range(16)]  # A wavy humus layer
    layers += [(x, 12 + la.wave(x, 1), CAN_SOIL_LT) for x in range(16)]  # A second layer lower down
    layers += [(3, 9, mm.LEAF), (4, 8, mm.LEAF), (10, 3, mm.HILLS_SH), (13, 11, mm.LEAF), (7, 14, CAN_SHINE)]  # Plant bits and water shine
    return na.soil_tile(CAN_SOIL, mm.OUTLINE, layers)  # Dark wet soil with its layers


def ledge_candaba():
    """A 16x10 low wooden boardwalk with plank joints."""  # Describes the tile
    joints = [(x, y, WOOD_DK) for x in (0, 8) for y in range(2, 8)]  # Plank joints
    return na.ledge_tile(WOOD, WOOD_LT, WOOD_DK, joints + [(4, 5, mm.MASK_SH), (12, 4, mm.MASK_SH)])  # Planks with nails


def candaba_boulder(w, h, cap_rows):
    """A dark mossy rock with a green moss cap on its top."""  # Describes the boulder
    return na.capped_boulder(w, h, STONE_DK, STONE_SH, STONE_LT, mm.OUTLINE, mm.HILLS_SH, cap_rows)  # Rock with moss


def bird_hide():
    """A 56x48 raised bamboo bird-watching hut on stilts, with a ladder and a viewing slot (the Candaba goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    for y in range(6, 20):  # Roof rows
        half = 5 + (y - 6) * 1.2  # Roof gets wider going down
        for x in range(int(28 - half), int(28 + half)):  # Pixels of the roof row
            la.put(img, x, y, THATCH_SH if (x + y) % 4 == 0 else THATCH)  # Nipa roof with lines
    for y in range(19, 34):  # Bamboo wall rows
        for x in range(12, 44):  # Wall pixels
            la.put(img, x, y, BAMBOO if x % 4 else BAMBOO_SH)  # Bamboo poles with dark gaps
    mm.fill_rect(img, 18, 23, 20, 4, mm.OUTLINE)  # Viewing slot in the wall
    mm.fill_rect(img, 16, 27, 24, 1, WOOD)  # Shelf under the viewing slot
    mm.fill_rect(img, 8, 34, 40, 2, WOOD_DK)  # Raised floor
    for x in (12, 22, 34, 40):  # Four stilts under the floor
        mm.fill_rect(img, x, 36, 2, 9, WOOD_DK)  # One stilt
    mm.fill_rect(img, 46, 36, 2, 10, lm.HANDLE)  # Left rail of the ladder
    mm.fill_rect(img, 51, 36, 2, 10, lm.HANDLE)  # Right rail of the ladder
    for y in (38, 41, 44):  # Ladder rungs
        mm.fill_rect(img, 46, y, 7, 1, WOOD)  # One rung
    return la.outline(img)  # Dark edge


# ---------- Lahar Plain (N2) ----------

def lahar_top():
    """A 16x8 tile of pale grey ash-sand with a few cracks and small pebbles."""  # Describes the tile
    return na.ground_top(LAHAR_SAND, LAHAR_CAP, LAHAR_LT, LAHAR_DK,
                         [(5, 2, CRACK), (6, 3, LAHAR_DK), (10, 1, CRACK), (11, 2, CRACK), (8, 6, LAHAR_DK), (13, 6, LAHAR_DK)])  # Cracks and pebbles


def lahar_soil():
    """A 16x16 grey layered lahar sand tile with small pumice bits."""  # Describes the tile
    layers = [(x, 4 + la.wave(x, 1), LAHAR_CAP) for x in range(16)]  # A pale layer
    layers += [(x, 9 + la.wave(x, 1), mm.VEST_SH) for x in range(16)]  # A darker layer
    layers += [(x, 13 + la.wave(x, 1), LAHAR_CAP) for x in range(16)]  # A pale layer lower down
    layers += [(3, 7, PUMICE), (10, 12, PUMICE), (12, 3, PUMICE), (6, 15, PUMICE)]  # Pumice bits
    return na.soil_tile(LAHAR_SAND, LAHAR_DK, layers)  # Grey sand with its layers


def ledge_lahar():
    """A 16x10 concrete dike block with two joints."""  # Describes the tile
    joints = [(8, y, CONCRETE_SH) for y in range(2, 8)]  # Joint between two blocks
    return na.ledge_tile(CONCRETE, CONCRETE_LT, CONCRETE_SH, joints + [(4, 5, CONCRETE_SH), (12, 3, CONCRETE_SH)])  # Concrete with joints


def lahar_boulder(w, h):
    """A light grey pumice-like boulder with dark pits."""  # Describes the boulder
    return la.boulder_colored(w, h, mm.MASK, mm.MASK_SH, mm.WHITE, mm.VEST_SH)  # Pale rock with pits


def lahar_marker():
    """A 40x48 tall striped warning post with a red-edged sign that shows a mud wave, no words (the Lahar goal)."""  # Describes the goal
    img = mm.new_image(40, 48)  # Empty picture
    for y in range(18, 44):  # Post rows
        for x in range(18, 22):  # Post columns
            la.put(img, x, y, mm.STRAP if ((y - 18) // 4) % 2 else mm.HELMET)  # Black and yellow stripes
    mm.fill_rect(img, 12, 43, 16, 3, CONCRETE)  # Concrete base
    mm.fill_rect(img, 12, 46, 16, 2, CONCRETE_SH)  # Shaded base
    ls.fill_poly(img, [(20, 1), (37, 28), (3, 28)], WARN_RED)  # Red warning triangle
    ls.fill_poly(img, [(20, 6), (33, 26), (7, 26)], mm.WHITE)  # White inside of the sign
    for x in range(13, 28):  # Columns of the mud wave
        la.put(img, x, 15 + la.wave(x, 1, 8), WARN_BLACK)  # Upper wave line
        la.put(img, x, 20 + la.wave(x, 1, 8), WARN_BLACK)  # Lower wave line
    return la.outline(img)  # Dark edge


# ---------- River Delta (N3) ----------

def delta_top():
    """A 16x8 tile of green grass over sandy silt, with tan sand specks."""  # Describes the tile
    return na.ground_top(SILT, GRASS, GRASS_LT, GRASS_SH,
                         [(2, 2, GRASS_SH), (7, 3, GRASS_SH), (11, 1, GRASS_LT), (5, 6, SAND_SH), (12, 6, SAND)])  # Grass tufts and sand


def delta_soil():
    """A 16x16 brown and tan layered sand and silt tile."""  # Describes the tile
    layers = [(x, 4 + la.wave(x, 1), SAND) for x in range(16)]  # A tan sand layer
    layers += [(x, 11 + la.wave(x, 1), SAND_SH) for x in range(16)]  # A darker sand layer lower down
    layers += [(3, 14, BROWN_DK), (9, 8, BROWN_DK)]  # Dark brown specks
    return na.soil_tile(BROWN, BROWN_DK, layers)  # Brown silt with its layers


def ledge_delta():
    """A 16x10 bamboo walkway with node rings along the length."""  # Describes the tile
    nodes = [(3, 2, BROWN_DK), (3, 6, BROWN_DK), (11, 2, BROWN_DK), (11, 6, BROWN_DK)]  # Bamboo node rings
    return na.ledge_tile(lm.PLAINS_SH, mm.PLAINS, BROWN_DK, nodes)  # Bamboo with nodes


def delta_boulder(w, h):
    """A smooth tan river cobble with a few dark specks."""  # Describes the boulder
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x, y) < 6:  # A few dark specks
            return mm.STRAP  # Dark speck
        return PEBBLE_SH if s else PEBBLE  # Smooth stone, shaded at the lower right
    return ls.outline(na.blob(w, h, (w - 1) / 2.0, h / 2.0, w / 2.0 - 1, h / 2.0 - 1, c, 2.0))  # Round cobble


def fish_pen_hut():
    """A 56x48 nipa hut on stilts over water, with fish-pen stakes and a net (the Delta goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    mm.fill_rect(img, 0, 38, 56, 10, WATER_DK)  # Water under the hut
    mm.fill_rect(img, 0, 38, 56, 1, WATER)  # Water surface line
    for y in range(6, 21):  # Roof rows
        half = 4 + (y - 6) * 1.3  # Roof gets wider going down
        for x in range(int(22 - half), int(22 + half)):  # Pixels of the roof row
            la.put(img, x, y, THATCH_SH if (x + y) % 4 == 0 else THATCH)  # Nipa roof with lines
    for y in range(20, 34):  # Bamboo wall rows
        for x in range(8, 38):  # Wall pixels
            la.put(img, x, y, BAMBOO if x % 4 else BAMBOO_SH)  # Bamboo poles with dark gaps
    mm.fill_rect(img, 16, 25, 8, 9, mm.OUTLINE)  # Doorway
    mm.fill_rect(img, 6, 34, 36, 2, WOOD)  # Floor on the stilts
    for x in (10, 22, 34):  # Stilts going into the water
        mm.fill_rect(img, x, 36, 2, 8, WOOD_DK)  # One stilt
    for x in range(42, 54, 4):  # Fish-pen stakes
        mm.fill_rect(img, x, 26, 2, 18, BAMBOO)  # One stake
    for y in range(28, 42):  # Net rows
        for x in range(43, 53):  # Net columns
            if (x + y) % 3 == 0:  # Every third pixel is net
                la.put(img, x, y, mm.MASK)  # Net mesh
    return la.outline(img)  # Dark edge


# ---------- Agusan Marsh (N4) ----------

def agusan_top():
    """A 16x8 tile of dark swamp grass over black-brown peat."""  # Describes the tile
    return na.ground_top(PEAT, SWAMP, mm.HILLS_SH, SWAMP_SH,
                         [(2, 1, mm.HILLS_SH), (7, 2, mm.HILLS), (12, 1, mm.HILLS_SH), (10, 6, PEAT_FIBRE), (4, 6, SWAMP)])  # Swamp tufts


def agusan_soil():
    """A 16x16 black-brown peaty soil tile with plant fibres and a water drop."""  # Describes the tile
    fibres = [(x, 5 + la.wave(x, 1, 16), PEAT_FIBRE) for x in range(2, 9)]  # Fibres in one band
    fibres += [(x, 11 + la.wave(x, 1, 16), PEAT_FIBRE) for x in range(8, 15)]  # Fibres in a second band
    fibres += [(6, 14, mm.LEAF)]  # A green bit
    return na.soil_tile(PEAT, mm.OUTLINE, fibres)  # Peat with fibres


def ledge_agusan():
    """A 16x10 floating bamboo raft walk with rope lashings."""  # Describes the tile
    lashings = [(4, 2, ROPE), (4, 3, ROPE), (11, 2, ROPE), (11, 3, ROPE)]  # Rope lashings
    return na.ledge_tile(RAFT, RAFT_LT, mm.GROUND_SH, lashings)  # Bamboo raft top


def agusan_boulder(w, h):
    """A dark wet stone with a pale wet highlight."""  # Describes the boulder
    return la.boulder_colored(w, h, WET_STONE, mm.OUTLINE, WET_STONE_LT, mm.OUTLINE)  # Dark wet rock


def floating_house():
    """A 56x48 Manobo-style wooden house on a bamboo raft, with water at the bottom (the Agusan goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    mm.fill_rect(img, 0, 40, 56, 8, WATER_DK)  # Water at the bottom
    mm.fill_rect(img, 0, 40, 56, 1, WATER)  # Water surface line
    mm.fill_rect(img, 4, 36, 48, 4, RAFT)  # Bamboo raft
    mm.fill_rect(img, 4, 36, 48, 1, RAFT_LT)  # Sunny raft top
    for x in (12, 30, 46):  # Rope lashings
        mm.fill_rect(img, x, 36, 2, 4, ROPE)  # One lashing
    ls.fill_poly(img, [(28, 3), (50, 22), (6, 22)], THATCH)  # Steep thatch roof
    ls.fill_poly(img, [(28, 3), (50, 22), (28, 22)], THATCH_SH)  # Shaded right half of the roof
    mm.fill_rect(img, 12, 22, 32, 14, lm.HANDLE)  # Wooden walls
    for y in range(26, 36, 4):  # Plank lines
        mm.fill_rect(img, 12, y, 32, 1, WOOD_DK)  # One plank line
    mm.fill_rect(img, 24, 26, 8, 10, mm.OUTLINE)  # Doorway
    mm.fill_rect(img, 15, 25, 5, 4, mm.SKY_LIGHT)  # Window on the left
    mm.fill_rect(img, 38, 25, 5, 4, mm.SKY_LIGHT)  # Window on the right
    return la.outline(img)  # Dark edge


# ---------- Mekong Delta (N5) ----------

def mekong_top():
    """A 16x8 tile of green grass over red-brown river clay."""  # Describes the tile
    return na.ground_top(CLAY, mm.HILLS, mm.PLAINS, mm.HILLS_SH,
                         [(3, 2, mm.HILLS_SH), (9, 1, mm.PLAINS), (12, 3, mm.HILLS_SH), (5, 6, CLAY_DK), (11, 6, CLAY_SILT)])  # Grass and clay specks


def mekong_soil():
    """A 16x16 red-brown clay tile with wavy silt bands."""  # Describes the tile
    layers = [(x, 5 + la.wave(x, 1), CLAY_SILT) for x in range(16)]  # A silt band
    layers += [(x, 12 + la.wave(x, 1), CLAY_SILT) for x in range(16)]  # A second silt band
    return na.soil_tile(CLAY, CLAY_DK, layers)  # Clay with its bands


def ledge_mekong():
    """A 16x10 wooden jetty plank with joints and nails."""  # Describes the tile
    joints = [(x, y, mm.GROUND_SH) for x in (5, 11) for y in range(2, 8)]  # Plank joints
    return na.ledge_tile(JETTY, JETTY_LT, mm.GROUND_SH, joints + [(2, 4, mm.MASK), (14, 4, mm.MASK)])  # Planks with nails


def mekong_boulder(w, h):
    """A rusty red laterite block with small holes."""  # Describes the boulder
    return la.boulder_colored(w, h, RUST, RUST_SH, RUST_LT, mm.OUTLINE)  # Rusty rock with holes


def market_pier():
    """A 56x48 wooden market pier over the river: a striped awning, fruit baskets and posts (the Mekong goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    mm.fill_rect(img, 0, 36, 56, 12, RIVER)  # River under the pier
    for x in range(2, 56, 9):  # Ripples on the river
        la.put(img, x, 40, RIVER_RIPPLE)  # One ripple
        la.put(img, x + 1, 40, RIVER_RIPPLE)  # Ripple continues
    for x in (4, 26, 48):  # Posts standing in the river
        mm.fill_rect(img, x, 34, 3, 12, WOOD_DK)  # One post
    for x in range(6, 50):  # Columns of the awning
        for y in range(6, 15):  # Rows of the awning
            la.put(img, x, y, AWNING_A if (x // 4) % 2 == 0 else AWNING_B)  # Red and cream stripes
        if x % 4 in (1, 2):  # Scalloped edge at the bottom of the awning
            la.put(img, x, 15, AWNING_A if (x // 4) % 2 == 0 else AWNING_B)  # Edge pixel
    mm.fill_rect(img, 0, 30, 56, 4, JETTY_LT)  # Pier deck
    for x in range(0, 56, 7):  # Deck plank joints
        mm.fill_rect(img, x, 30, 1, 4, WOOD_DK)  # One joint
    mm.fill_rect(img, 6, 15, 2, 15, WOOD_DK)  # Left pole under the awning
    mm.fill_rect(img, 47, 15, 2, 15, WOOD_DK)  # Right pole under the awning
    for x0 in (12, 34):  # Two fruit baskets on the deck
        ls.fill_poly(img, [(x0, 24), (x0 + 9, 24), (x0 + 8, 29), (x0 + 1, 29)], BASKET)  # Woven basket
        mm.fill_rect(img, x0, 24, 10, 1, WOOD_DK)  # Basket rim
        ls.disc(img, x0 + 2, 23, 2, FRUIT_YEL)  # Yellow fruit on the left
        ls.disc(img, x0 + 6, 22, 2, FRUIT_GRN)  # Green fruit on the right
    return la.outline(img)  # Dark edge


# ---------- Moving market boat (48x16) ----------

def market_boat():
    """A 48x16 wooden river boat seen from the side. Row 0 is the flat deck the explorer stands on.
    A small pile of green and yellow fruit sits at the right end, on rows 0 to 2."""  # Describes the boat
    img = mm.new_image(48, 16)  # Empty picture
    ls.fill_poly(img, [(0, 1), (47, 1), (41, 12), (7, 12)], BOAT)  # The hull under the deck
    mm.fill_rect(img, 0, 0, 48, 1, BOAT_DECK)  # The deck line on row 0
    mm.fill_rect(img, 3, 5, 42, 1, BOAT_DK)  # Plank line in the hull
    mm.fill_rect(img, 5, 9, 38, 1, BOAT_DK)  # Lower plank line
    mm.fill_rect(img, 36, 1, 7, 2, FRUIT_GRN)  # Green fruit row
    mm.fill_rect(img, 38, 0, 6, 1, FRUIT_YEL)  # Yellow fruit on the deck line
    mm.fill_rect(img, 39, 2, 4, 1, FRUIT_GRN_SH)  # Shaded fruit at the bottom of the pile
    return la.outline(img)  # Dark edge so it reads on water


# ---------- Props (decoration) ----------

def duck(frame):
    """A 16x10 wild duck swimming on the water; frame 0 has its head up, frame 1 has its head down (decoration only)."""  # Describes the prop
    img = mm.new_image(16, 10)  # Empty picture
    mm.fill_rect(img, 0, 7, 16, 3, WATER)  # Water under the duck
    mm.fill_rect(img, 0, 7, 16, 1, WATER_LT)  # Water shine on top
    ls.fill_poly(img, [(2, 6), (5, 4), (11, 4), (13, 6), (11, 8), (4, 8)], DUCK_BODY)  # Round brown body on the water
    ls.fill_poly(img, [(4, 5), (9, 4), (10, 6), (5, 7)], DUCK_WING)  # Dark wing on the back
    mm.fill_rect(img, 3, 7, 8, 1, DUCK_BELLY)  # Pale belly line at the water
    la.put(img, 1, 5, DUCK_WING)  # Tail tip
    if frame == 0:  # Head up
        mm.fill_rect(img, 11, 1, 3, 4, DUCK_HEAD)  # Green neck and head
        mm.fill_rect(img, 12, 0, 2, 2, DUCK_HEAD)  # Round top of the head
        la.put(img, 13, 3, DUCK_BELLY)  # White neck ring
        la.put(img, 14, 2, BEAK)  # Yellow beak
        la.put(img, 13, 1, mm.OUTLINE)  # Eye
    else:  # Head down
        mm.fill_rect(img, 10, 4, 3, 3, DUCK_HEAD)  # Green head, low on the water
        mm.fill_rect(img, 13, 5, 2, 1, BEAK)  # Yellow beak
        la.put(img, 11, 5, mm.OUTLINE)  # Eye
    return la.outline(img)  # Dark edge so the duck reads on the water


def water_lily():
    """A 12x6 pair of green lily pads with one pink flower on top (decoration only)."""  # Describes the prop
    img = mm.new_image(12, 6)  # Empty picture
    mm.fill_rect(img, 0, 5, 12, 1, WATER)  # Water under the pads
    ls.disc(img, 3, 4, 2, mm.LEAF)  # Left lily pad
    ls.disc(img, 8, 4, 2, mm.HILLS_SH)  # Right lily pad
    for (x, y) in ((6, 0), (5, 1), (7, 1), (6, 2)):  # Four petals of the flower
        la.put(img, x, y, PINK)  # One petal
    la.put(img, 6, 1, mm.HELMET)  # Yellow middle of the flower
    la.put(img, 7, 2, PINK_SH)  # Petal in shade
    return img  # No outline, like the other living props


def lahar_sign():
    """A 16x20 small concrete post with a yellow warning triangle showing a mud wave, no words (decoration only)."""  # Describes the prop
    img = mm.new_image(16, 20)  # Empty picture
    mm.fill_rect(img, 7, 10, 2, 10, mm.MASK_SH)  # Grey concrete post
    ls.fill_poly(img, [(8, 0), (15, 11), (1, 11)], mm.HELMET)  # Yellow warning triangle
    for x in range(4, 13):  # Columns of the mud wave
        la.put(img, x, 6 + la.wave(x, 1, 8), mm.STRAP)  # Upper wave line
        la.put(img, x, 9 + la.wave(x, 1, 8), mm.STRAP)  # Lower wave line
    return la.outline(img)  # Dark edge round the sign and post


def reed_tuft():
    """A 12x14 clump of tall pale reeds with fluffy heads (decoration only)."""  # Describes the prop
    img = mm.new_image(12, 14)  # Empty picture
    for (x, h, lean) in ((1, 12, -1), (3, 14, 0), (5, 11, 1), (7, 13, 0), (9, 10, 1), (10, 8, 0)):  # Reeds: column, height, lean
        for t in range(h):  # Up the reed from its base
            la.put(img, x + (lean if t > h // 2 else 0), 13 - t, mm.VEST if t > h - 4 else lm.PLAINS_SH)  # Pale tip, green stem
        la.put(img, x + lean, 13 - h, mm.HEADLAMP)  # Fluffy head at the top
    return img  # No outline, like the other living props


def fish_stakes():
    """A 24x16 row of bamboo fish-pen stakes standing in water, tied with ropes (decoration only)."""  # Describes the prop
    img = mm.new_image(24, 16)  # Empty picture
    mm.fill_rect(img, 0, 9, 24, 7, WATER_DK)  # Deep water at the bottom
    mm.fill_rect(img, 0, 9, 24, 1, WATER)  # Water surface line
    for i, x in enumerate(range(1, 24, 3)):  # Eight stakes, three pixels apart
        top = 2 + (i * 5) % 4  # Each stake has a different height
        mm.fill_rect(img, x, top, 2, 15 - top, BAMBOO)  # One bamboo stake
        mm.fill_rect(img, x + 1, top, 1, 15 - top, BAMBOO_SH)  # Shaded side of the stake
    mm.fill_rect(img, 0, 6, 24, 1, mm.GROUND)  # Rope across the stakes
    mm.fill_rect(img, 0, 12, 24, 1, mm.GROUND_SH)  # Lower rope, in the water
    return img  # No outline, the water shows the edge


def bangka_plains():
    """A 32x12 small outrigger boat pulled up on the bank, with a float beam on top (decoration only)."""  # Describes the prop
    img = mm.new_image(32, 12)  # Empty picture
    ls.fill_poly(img, [(1, 5), (6, 9), (26, 9), (31, 4), (28, 10), (5, 11)], mm.PLATEAU_SH)  # Curved wooden hull
    mm.fill_rect(img, 5, 6, 20, 1, mm.HEADLAMP)  # Light top edge of the hull
    mm.fill_rect(img, 4, 0, 24, 1, GROUND_LOG)  # Outrigger float, a log on top
    mm.fill_rect(img, 3, 2, 26, 1, lm.HANDLE)  # Beam that joins the float to the hull
    for x in (8, 21):  # Two struts
        mm.fill_rect(img, x, 3, 1, 3, WOOD_DK)  # One strut
    return la.outline(img)  # Dark edge


GROUND_LOG = mm.GROUND  # Brown log for the outrigger float


def water_hyacinth():
    """A 14x8 floating patch of round green leaves with one lilac flower spike (decoration only)."""  # Describes the prop
    img = mm.new_image(14, 8)  # Empty picture
    mm.fill_rect(img, 0, 7, 14, 1, WATER)  # Water under the leaves
    ls.disc(img, 4, 5, 2, mm.LEAF)  # Left leaf
    ls.disc(img, 10, 5, 2, mm.HILLS_SH)  # Right leaf
    la.put(img, 7, 4, mm.HILLS)  # Small leaf stalk
    la.put(img, 7, 3, mm.HILLS_SH)  # Stalk going up
    for (x, y) in ((6, 0), (5, 1), (7, 1), (6, 2), (8, 2)):  # Lilac petals of the flower spike
        la.put(img, x, y, LILAC)  # One petal
    la.put(img, 6, 1, mm.HELMET)  # Yellow middle of the flower
    la.put(img, 8, 1, LILAC_SH)  # Petal in shade
    return img  # No outline, like the other living props


def crocodile():
    """A 24x8 calm crocodile resting in the water, with only its back, its eyes and its long snout above the water (decoration only)."""  # Describes the prop
    img = mm.new_image(24, 8)  # Empty picture
    mm.fill_rect(img, 1, 3, 19, 3, CROC)  # Long back of the crocodile
    mm.fill_rect(img, 18, 4, 5, 2, CROC)  # Snout reaching to the right
    for x in range(2, 17, 3):  # Back scales
        la.put(img, x, 4, CROC_LT)  # One scale
    mm.fill_rect(img, 15, 1, 3, 2, CROC)  # Eye bump on the head
    la.put(img, 16, 1, mm.HEADLAMP)  # Eye white, a friendly look
    la.put(img, 17, 1, mm.OUTLINE)  # Eye pupil
    mm.fill_rect(img, 19, 5, 3, 1, mm.PLAINS)  # Light smile line under the snout
    img = la.outline(img)  # Dark edge before the water is painted
    mm.fill_rect(img, 0, 6, 24, 2, WATER)  # Water hides the lower body
    mm.fill_rect(img, 0, 7, 24, 1, WATER_DK)  # Darker water at the bottom
    return img


def fruit_crate():
    """A 14x10 wooden crate with yellow mangoes and green fruit peeking out of the top (decoration only)."""  # Describes the prop
    img = mm.new_image(14, 10)  # Empty picture
    ls.disc(img, 3, 2, 2, FRUIT_YEL)  # Mango at the left
    ls.disc(img, 7, 1, 2, FRUIT_GRN)  # Green fruit in the middle
    ls.disc(img, 11, 2, 2, FRUIT_YEL_SH)  # Shaded mango at the right
    mm.fill_rect(img, 0, 4, 14, 6, mm.GROUND)  # Crate body
    mm.fill_rect(img, 0, 4, 14, 1, mm.VEST)  # Light top edge of the crate
    mm.fill_rect(img, 0, 6, 14, 1, mm.GROUND_SH)  # Slat line
    return la.outline(img)  # Dark edge


# ---------- Items (24x24, Assets/Art/Items/Sprites/) ----------

def peat():
    """A 24x24 soft dark-brown fibrous lump with tiny root and leaf bits."""  # Describes the item
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x, y) < 10:  # Some pale fibres
            return mm.GROUND  # Fibre
        return PEAT_DK if s else PEAT_BROWN  # Dark brown peat
    img = na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 2.6)  # Soft lump
    for (x, y) in ((5, 17), (6, 18), (7, 19), (8, 19), (9, 20)):  # A root going down
        ls.put(img, x, y, mm.VEST_SH)  # Root pixel
    for (x, y) in ((15, 8), (16, 7), (16, 6)):  # A small leaf bit
        ls.put(img, x, y, mm.LEAF)  # Leaf pixel
    return ls.outline(img)  # Dark edge


def laterite():
    """A 24x24 rusty red-orange block with small holes and darker iron spots."""  # Describes the item
    def c(x, y, s):  # Colour of one pixel
        n = lm.noise(x, y)  # Fixed random number for this pixel
        if n < 6:  # A small hole
            return mm.OUTLINE  # Black hole
        if n > 92:  # A dark iron spot
            return LAT_IRON  # Dark spot
        return LAT_SH if s else LAT  # Rusty red-orange rock
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 3.0))  # Blocky rusty block


# ---------- Lesson pictures (120x82, Assets/Art/Lesson/Sprites/) ----------

def egret_sprite():
    """A 7x10 white egret standing on thin legs, with a dark edge (decoration for the Candaba picture)."""  # Describes the sprite
    img = mm.new_image(7, 10)  # Empty picture
    la.draw_grid(img, EGRET_ROWS, 0, 0, EGRET_COLORS)  # Paint the bird
    return la.outline(img)  # Dark edge so the white bird reads on the water


def flying_duck(img, x, y):
    """Paints one wild duck in flight as a small dark W shape, centred on (x, y)."""  # Describes the helper
    for (dx, dy) in ((-2, -1), (-1, 0), (0, -1), (1, 0), (2, -1)):  # The five pixels of the wings
        la.put(img, x + dx, y + dy, mm.STRAP)  # One dark wing pixel


def candaba_pixel(x, y):
    """Colour of one pixel of the Candaba picture: sky, dark reeds on the horizon, shallow marsh water with shine and a grassy bank."""  # Describes the picture
    if y < 32:  # Sky above the marsh
        return na.sky(x, y)  # Sky colour
    if y < 36:  # Far reed line on the horizon
        return mm.HILLS_SH if (x // 3 + y) % 2 else mm.LEAF  # Dark green reeds
    if y >= 66:  # Grassy bank in the front
        return mm.HILLS if y < 68 else mm.HILLS_SH  # Grass edge, then shade
    if y % 5 == 1 and lm.noise(x, y) < 30:  # A few shine lines on the water
        return WATER_LT  # Pale shine
    if 40 <= y < 46 and (x // 6) % 3 == 0 and lm.noise(x, y) < 50:  # Small reed clumps in the water
        return mm.HILLS  # Reed tip
    return WATER_MARSH  # Shallow marsh water


def lesson_candaba():
    """A 120x82 framed picture of a wide shallow wetland with wild ducks flying in a V and egrets wading."""  # Describes the lesson
    img = ls.framed(candaba_pixel)  # Sky, reeds and marsh water
    for (x, y) in ((60, 16), (52, 20), (68, 20), (44, 24), (76, 24)):  # Duck places in a V
        flying_duck(img, x, y)  # One duck in flight
    for (x, y) in ((22, 48), (88, 56)):  # Two egret places in the water
        mm.paste(img, egret_sprite(), x, y)  # One egret wading
    return img


def pinatubo_pixel(x, y):
    """Colour of one pixel of the Pinatubo picture: sky, a wide low mountain with a flat cut-off top and a small crater lake, and a calm grey-beige lahar plain with a river and a few green bushes."""  # Describes the picture
    if y < PEAK_Y:  # Sky above the mountain top
        return na.sky(x, y)  # Sky colour
    if y < 42:  # The mountain and the sky beside it
        half = 26 + (y - PEAK_Y) * 1.4  # Half-width of the mountain on this row, broad and low
        if abs(x - PEAK_X) <= half:  # Inside the mountain
            if y < PEAK_Y + 2:  # The broad crater rim on the flat top
                return mm.MASK  # Pale rim
            if y < PEAK_Y + 6 and abs(x - PEAK_X) <= 10:  # The small turquoise crater lake
                return lm.LENS if y > PEAK_Y + 2 else WATER_LT  # Turquoise lake with a pale edge
            return mm.MASK_SH if x <= PEAK_X else mm.STRAP  # Grey slope, dark in shade
        return na.sky(x, y)  # Sky beside the mountain
    river_mid = 62 + round(6 * math.sin((y - 42) / 6.0))  # Middle of the river on this row, winding
    half_river = 2 + (y - 42) // 8  # The river gets wider toward the front
    if abs(x - river_mid) <= half_river:  # Inside the river
        return WATER  # Calm river, no speckle
    for (bx, by) in ((16, 48), (46, 54), (100, 50)):  # Three small green bushes
        if (x - bx) ** 2 + (y - by) ** 2 <= 5:  # Inside a bush
            return mm.LEAF if (x + y) % 2 else mm.HILLS_SH  # Green bush
    if lm.noise(x, y) < 2:  # Very few light speckles
        return mm.VEST_SH  # Speck
    if (y // 4) % 2 == 0:  # Calm bands of grey-beige, one lighter
        return mix(mm.MASK, mm.VEST)  # Lighter grey-beige band
    return mix(mm.MASK_SH, mm.VEST)  # Darker grey-beige band

def lesson_pinatubo_lahar():
    """A 120x82 framed picture of a far volcano with a summit lake, and a wide grey lahar plain with a river in front. No people or damage."""  # Describes the lesson
    return ls.framed(pinatubo_pixel)  # Sky, volcano, plain and river


def agno_pixel(x, y):
    """Colour of one pixel of the Agno picture: sky, green land, a river that splits into four channels across a green and tan fan-shaped delta, and a flat blue sea with soft dashes."""  # Describes the picture
    if y < 22:  # Sky
        return na.sky(x, y)  # Sky colour
    if y < MOUTH_Y:  # Land and river above the delta
        river = 14 + (y - 22) * 1.7 + 4 * math.sin((y - 22) / 6.0)  # Middle of the river on this row, flowing down to the right
        if abs(x - river) <= 2 + (y - 22) // 10:  # Inside the river
            return WATER_LT if lm.noise(x, y) < 8 else WATER  # Blue water with a little shine
        return mm.HILLS if y < 36 else mm.PLAINS  # Green land, lighter lower down
    rel = y - MOUTH_Y  # Rows below the river mouth
    fan = 4 + 26 * min(1.0, rel / 18.0) ** 0.6  # Half-width of the delta on this row, spreading out
    if y >= 70 or abs(x - MOUTH_X) > fan:  # The flat sea, outside the fan and at the bottom
        if y % 6 == 2 and (x + (y // 6) * 7) % 16 < 5:  # A soft wave dash
            return WATER_LT  # Pale dash
        return WATER  # Flat blue sea
    for k in (-1.5, -0.5, 0.5, 1.5):  # Four river channels that fan out
        if abs(x - (MOUTH_X + k * rel * 0.9)) < 1.0:  # On a channel line
            return WATER  # Channel water
    if abs(x - MOUTH_X) > fan - 3:  # The sandy edge of the delta
        return mm.VEST  # Tan sand
    if rel % 9 in (4, 5) and (x // 5) % 3 == 0:  # Small sandbars
        return mm.VEST  # Pale sandbar
    return mm.HILLS if (rel // 6) % 2 == 0 else mm.PLAINS  # Green land in bands

def lesson_agno():
    """A 120x82 framed picture of a river winding to the sea and spreading into a green fan-shaped delta with sandbars."""  # Describes the lesson
    return ls.framed(agno_pixel)  # Sky, land, river, delta and sea


def small_house():
    """A 24x18 small floating house on a bamboo raft, for the Agusan picture (decoration only)."""  # Describes the sprite
    img = mm.new_image(24, 18)  # Empty picture
    ls.fill_poly(img, [(12, 0), (23, 8), (1, 8)], THATCH)  # Roof
    mm.fill_rect(img, 4, 8, 16, 6, lm.HANDLE)  # Wooden walls
    mm.fill_rect(img, 10, 10, 4, 4, mm.OUTLINE)  # Doorway
    mm.fill_rect(img, 0, 14, 24, 3, RAFT)  # Bamboo raft
    mm.fill_rect(img, 0, 14, 24, 1, RAFT_LT)  # Sunny raft top
    return la.outline(img)  # Dark edge


def agusan_pixel(x, y):
    """Colour of one pixel of the Agusan picture: sky, a dark swamp bank and calm dark marsh water with shine."""  # Describes the picture
    if y < 30:  # Sky
        return na.sky(x, y)  # Sky colour
    if y < 36:  # Dark swamp bank
        return SWAMP if (x + y) % 4 else SWAMP_SH  # Swamp grass
    if y % 4 == 2 and lm.noise(x, y) < 30:  # Shine lines on the calm water
        return WATER_LT  # Pale shine
    return WATER_DK  # Dark calm water


def lesson_agusan():
    """A 120x82 framed picture of a calm marsh with floating houses on rafts and swamp trees."""  # Describes the lesson
    img = ls.framed(agusan_pixel)  # Sky, bank and water
    for cx in (14, 40, 70, 98):  # Four swamp trees along the bank
        ls.disc(img, cx, 20, 8, mm.LEAF)  # Leafy crown
        ls.disc(img, cx + 3, 22, 5, SWAMP_SH)  # Shaded side of the crown
        mm.fill_rect(img, cx - 1, 26, 3, 12, mm.GROUND_SH)  # Trunk going into the water
    mm.paste(img, small_house(), 22, 48)  # Floating house on a raft
    mm.paste(img, small_house(), 74, 56)  # Second floating house
    return img


def mekong_pixel(x, y):
    """Colour of one pixel of the Mekong picture: sky, a green riverbank and a brown river with light ripples."""  # Describes the picture
    if y < 26:  # Sky
        return na.sky(x, y)  # Sky colour
    if y < 34:  # Green riverbank with bushes
        return mm.HILLS_SH if x % 9 else mm.LEAF  # Bush shape
    if y < 40:  # Bank edge, a little lower
        return mm.GROUND if (x + y) % 6 else mm.GROUND_SH  # Brown bank
    if y % 5 == 0 and lm.noise(x, y) < 40:  # Light ripples on the river
        return RIVER_RIPPLE  # Ripple
    return RIVER  # Brown-green river


def full_boat():
    """A 48x20 fruit boat: the market boat with a row of mangoes and green fruit on its deck, for the Mekong picture (decoration only)."""  # Describes the sprite
    img = mm.new_image(48, 20)  # Empty picture
    mm.paste(img, market_boat(), 0, 4)  # The boat, lower down so the fruit fits above it
    for x in range(2, 46, 4):  # Fruit along the deck
        ls.disc(img, x + 2, 3, 2, FRUIT_YEL if (x // 4) % 2 else FRUIT_GRN)  # One piece of fruit, yellow or green
    return img


def lesson_mekong():
    """A 120x82 framed picture of a brown river with several boats full of fruit side by side (a floating market)."""  # Describes the lesson
    img = ls.framed(mekong_pixel)  # Sky, bank and river
    for (x, y) in ((2, 44), (44, 52), (86, 56)):  # Three boats, staggered a little
        mm.paste(img, full_boat(), x, y)  # One boat full of fruit
    return img


# ---------- Saving and the preview sheet ----------

def put_file(where, name, img):
    """Saves one picture in the folder for its kind: item, lesson or level sprite."""  # Describes the helper
    if where == "item":  # Shared item picture
        ls.save(img, ls.ITEMS_OUT, name)  # Items folder
    elif where == "lesson":  # Lesson picture
        ls.save(img, ls.LESSON_OUT, name)  # Lesson folder
    else:  # Level sprite or prop
        la.save(img, name)  # Level sprites folder


def big(img, scale):
    """Makes a crisp, larger copy of a picture for the preview."""  # Describes the helper
    return img.resize((img.width * scale, img.height * scale), Image.NEAREST)  # Nearest-neighbour scaling


def preview(groups):
    """Lays the new sprites out at 3x size on one neutral sheet, one labelled group after another."""  # Describes the sheet
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
    """Draws everything, saves each picture and writes the preview sheet."""  # Describes the main function
    groups = [  # Each group: its title, then (kind, file name, picture) for each picture
        ("N1 CANDABA SWAMP", [
            ("level", "candaba_top.png", candaba_top()),  # Ground top
            ("level", "candaba_soil.png", candaba_soil()),  # Ground below the grass
            ("level", "ledge_candaba.png", ledge_candaba()),  # Boardwalk ledge
            ("level", "boulder_small_candaba.png", candaba_boulder(32, 24, 2)),  # Small boulder
            ("level", "boulder_big_candaba.png", candaba_boulder(48, 36, 3)),  # Big boulder
            ("level", "bird_hide.png", bird_hide()),  # Goal: bird-watching hut
        ]),
        ("N2 LAHAR PLAIN", [
            ("level", "lahar_top.png", lahar_top()),  # Ground top
            ("level", "lahar_soil.png", lahar_soil()),  # Ground below the ash
            ("level", "ledge_lahar.png", ledge_lahar()),  # Concrete dike ledge
            ("level", "boulder_small_lahar.png", lahar_boulder(32, 24)),  # Small boulder
            ("level", "boulder_big_lahar.png", lahar_boulder(48, 36)),  # Big boulder
            ("level", "lahar_marker.png", lahar_marker()),  # Goal: warning post
        ]),
        ("N3 RIVER DELTA", [
            ("level", "delta_top.png", delta_top()),  # Ground top
            ("level", "delta_soil.png", delta_soil()),  # Ground below the grass
            ("level", "ledge_delta.png", ledge_delta()),  # Bamboo walkway ledge
            ("level", "boulder_small_delta.png", delta_boulder(32, 24)),  # Small cobble
            ("level", "boulder_big_delta.png", delta_boulder(48, 36)),  # Big cobble
            ("level", "fish_pen_hut.png", fish_pen_hut()),  # Goal: hut on stilts
        ]),
        ("N4 AGUSAN MARSH", [
            ("level", "agusan_top.png", agusan_top()),  # Ground top
            ("level", "agusan_soil.png", agusan_soil()),  # Ground below the swamp grass
            ("level", "ledge_agusan.png", ledge_agusan()),  # Raft walk ledge
            ("level", "boulder_small_agusan.png", agusan_boulder(32, 24)),  # Small wet stone
            ("level", "boulder_big_agusan.png", agusan_boulder(48, 36)),  # Big wet stone
            ("level", "floating_house.png", floating_house()),  # Goal: house on a raft
        ]),
        ("N5 MEKONG DELTA", [
            ("level", "mekong_top.png", mekong_top()),  # Ground top
            ("level", "mekong_soil.png", mekong_soil()),  # Ground below the grass
            ("level", "ledge_mekong.png", ledge_mekong()),  # Jetty plank ledge
            ("level", "boulder_small_mekong.png", mekong_boulder(32, 24)),  # Small laterite block
            ("level", "boulder_big_mekong.png", mekong_boulder(48, 36)),  # Big laterite block
            ("level", "market_pier.png", market_pier()),  # Goal: market pier
        ]),
        ("MOVING MARKET BOAT", [
            ("level", "market_boat.png", market_boat()),  # The moving fruit boat
        ]),
        ("PLAINS NORMAL PROPS", [
            ("level", "Props/duck_00.png", duck(0)),  # Duck, head up
            ("level", "Props/duck_01.png", duck(1)),  # Duck, head down
            ("level", "Props/water_lily.png", water_lily()),  # Lily pads with a pink flower
            ("level", "Props/lahar_sign.png", lahar_sign()),  # Mud-wave warning post
            ("level", "Props/reed_tuft.png", reed_tuft()),  # Pale reeds
            ("level", "Props/fish_stakes.png", fish_stakes()),  # Fish-pen stakes in water
            ("level", "Props/bangka_plains.png", bangka_plains()),  # Outrigger boat on the bank (new name, Props/bangka.png is taken)
            ("level", "Props/water_hyacinth.png", water_hyacinth()),  # Floating leaves with a lilac flower
            ("level", "Props/crocodile.png", crocodile()),  # Resting crocodile
            ("level", "Props/fruit_crate.png", fruit_crate()),  # Crate of fruit
        ]),
        ("ITEMS", [
            ("item", "peat.png", peat()),  # Soft dark-brown peat
            ("item", "laterite.png", laterite()),  # Rusty red-orange laterite
        ]),
        ("LESSON PICTURES", [
            ("lesson", "lesson_candaba.png", lesson_candaba()),  # Wetland with ducks and egrets
            ("lesson", "lesson_pinatubo_lahar.png", lesson_pinatubo_lahar()),  # Volcano and lahar plain (new name, lesson_pinatubo.png is taken)
            ("lesson", "lesson_agno.png", lesson_agno()),  # River to the sea, delta
            ("lesson", "lesson_agusan.png", lesson_agusan()),  # Marsh with floating houses
            ("lesson", "lesson_mekong.png", lesson_mekong()),  # Brown river with fruit boats
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
    print("Plains Normal art written to", la.OUT, ls.ITEMS_OUT, "and", ls.LESSON_OUT)  # Tell where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
