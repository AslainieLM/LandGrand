"""Draws the PNGs for Plateaus Easy 1 to 3 (D-072): three level themes (Pineapple Plateau, Lake Plateau, Falls Edge),
the cliff updraft, the Plateaus props, four new minerals, three lesson pictures and the
Plateaus life sprites for the Landform Map and the Plateaus menu screens.

Run from the Unity project folder:  python Tools/Art/make_plateaus_easy_art.py
It writes to Assets/Art/Level/Sprites/ (themes, updraft and props), Assets/Art/Items/Sprites/ (items),
Assets/Art/Lesson/Sprites/ (lesson pictures) and Assets/Art/LandformMap/Sprites/Plateaus/ (life sprites),
plus a preview sheet at .scratch/plateaus-easy/art-preview.png.
It reuses the helpers of make_mainmenu_art.py, make_landform_map_art.py, make_level_art.py, make_lesson_art.py and make_normal_art.py.
All colours come from Assets/Art/Palettes/landgrand.gpl or mix two of its colours.
The waterfall spray prop is named falls_spray, because falls_mist is already the Hills Normal spray (24x16).
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
MAP_OUT = os.path.join(mm.ROOT, "Assets", "Art", "LandformMap", "Sprites", "Plateaus")  # Where the Plateaus life sprites go
PREVIEW = os.path.join(mm.ROOT, ".scratch", "plateaus-easy", "art-preview.png")  # Where the preview sheet goes

# Palette colours that the helper scripts do not name.
SHIRT = (63, 143, 134)  # Shirt, from the palette: blue-green
SHIRT_SH = (45, 107, 100)  # Shirt shade, from the palette: dark blue-green
PANTS = (92, 74, 61)  # Pants, from the palette: grey-brown
SUN_GLOW = (255, 225, 122)  # Sun glow, from the palette: warm yellow

# Theme colours (D-072), each a palette colour or a mix of two.
RED_SOIL = mm.PLATEAU_SH  # Red-brown volcanic farm soil of Bukidnon
RED_SOIL_DK = mm.BACKPACK_SH  # Dark specks in the red soil
RED_SOIL_LT = mm.PLATEAU  # Lighter clay layer in the red soil
GRASS = mm.HILLS  # Green grass
GRASS_SH = mm.HILLS_SH  # Grass in shade
GRASS_LT = mm.PLAINS  # Sunny grass tips
PINE_LEAF = SHIRT  # Blue-green pineapple leaves
PINE_LEAF_SH = SHIRT_SH  # Pineapple leaves in shade
FRUIT = mm.HELMET  # Yellow pineapple fruit
FRUIT_SH = mm.HELMET_SH  # Pineapple fruit eyes
BASALT = mm.STRAP  # Dark basalt rock
BASALT_SH = mix(mm.STRAP, mm.OUTLINE)  # Basalt in shade
BASALT_LT = mix(mm.STRAP, mm.MASK_SH)  # Sunny basalt edge
BASALT_PORE = mix(mm.OUTLINE, mm.DARK)  # Tiny gas holes in the basalt
TUFF = lm.HAMMER  # Pale grey volcanic tuff of Lanao
TUFF_SH = mm.MASK_SH  # Tuff in shade and its darker layers
TUFF_LT = mm.MASK  # Sunny tuff
TUFF_DK = mix(mm.MASK_SH, mm.STRAP)  # Dark specks in the tuff
CLAST_RED = mix(mm.PLATEAU_SH, mm.MASK_SH)  # Reddish rock bits in the breccia
LAKE = lm.LENS  # Lake blue
LAKE_LT = mm.SKY_LIGHT  # Pale lake shine
LAKE_DEEP = mix(lm.LENS, lm.MOUNTAIN_SH)  # Deep lake blue
FOAM = mm.WHITE  # White falling water and foam
MOSS = mm.LEAF  # Wet dark moss and ferns
MOSS_DK = mix(mm.LEAF, mm.OUTLINE)  # Moss in deep shade
WOOD = lm.HANDLE  # Light wood
WOOD_SH = mm.GROUND_SH  # Wood in shade
WOOD_LT = mix(lm.HANDLE, mm.HEADLAMP)  # Sunny top of a plank
THATCH = mix(mm.VEST, mm.HELMET_SH)  # Grass roof
THATCH_SH = mm.VEST_SH  # Grass roof in shade
ROOF_DARK = mm.DARK  # Dark roof ridge
OKIR_RED = mm.SCARF  # Red of the carved beam ends
OKIR_YELLOW = mm.HELMET  # Yellow of the carved beam ends
OKIR_GREEN = SHIRT  # Green of the carved beam ends
HAWK = lm.BACKPACK  # Brown hawk wings
HAWK_SH = mm.BACKPACK_SH  # Dark wing tips and tail
HAWK_BELLY = mm.VEST  # Pale hawk belly and head
REED = mm.HILLS_SH  # Reed stems
REED_DK = mm.LEAF  # Reed stems in shade
REED_HEAD = mm.GROUND_SH  # Brown reed heads
BOAT = WOOD  # Boat hull wood
BOAT_SH = mm.GROUND_SH  # Hull in shade
BAMBOO = mm.VEST  # Outrigger arms and floats
BAMBOO_SH = mm.VEST_SH  # Bamboo in shade
WIND = mm.WHITE  # Bright end of a wind streak
DUST = mm.VEST  # Dust grains lifted by the wind
DUST_SH = mm.VEST_SH  # Darker dust grains
HALLOY = mix(mm.WHITE, mm.HEADLAMP)  # White-cream halloysite clay
HALLOY_SH = mix(mm.MASK, mm.VEST)  # Halloysite in shade
HALLOY_PIT = mm.VEST  # Earthy pits in the clay
GOETH = mix(mm.BACKPACK_SH, mm.OUTLINE)  # Brown-black goethite
GOETH_SH = mix(mm.OUTLINE, mm.DARK)  # Goethite in shade
GOETH_RUST = mm.HELMET_SH  # Rusty yellow-brown streak
GOETH_RUST_DK = lm.BACKPACK  # Darker rust streak
GIBB = mm.MASK  # Pale white-grey gibbsite
GIBB_SH = mix(mm.MASK, mm.MASK_SH)  # Gibbsite in shade
GIBB_SHINE = mix(mm.WHITE, mm.SKY_LIGHT)  # Pearly shine on gibbsite
GLASS = mix(mm.STRAP, mm.OUTLINE)  # Dark volcanic glass around cristobalite
GLASS_SH = mix(mm.OUTLINE, mm.DARK)  # Glass in shade
GLASS_SHINE = lm.MOUNTAIN  # Blue-grey shine on the glass
BALL = mm.WHITE  # White cristobalite balls
BALL_SH = mm.MASK  # Shaded side of a ball


# ---------- Pineapple Plateau: Bukidnon (E1) ----------

def pineapple_top():
    """A 16x8 tile of green grass over red-brown volcanic soil, with blue-green pineapple leaf tips poking up."""
    specks = [(2, 1, PINE_LEAF), (3, 2, PINE_LEAF_SH), (9, 1, PINE_LEAF), (10, 2, PINE_LEAF_SH),  # Pineapple leaf tips
              (6, 2, GRASS_SH), (13, 1, GRASS_SH), (5, 6, RED_SOIL_DK), (12, 6, RED_SOIL_DK)]  # Grass tufts and soil specks
    return na.ground_top(RED_SOIL, GRASS, GRASS_LT, GRASS_SH, specks)  # Grass cap over red soil


def pineapple_soil():
    """A 16x16 red-brown volcanic soil tile with a lighter clay layer and small dark basalt pebbles."""
    layers = [(x, 6 + la.wave(x, 1), RED_SOIL_LT) for x in range(16)]  # A wavy lighter clay layer
    layers += [(3, 12, BASALT), (4, 12, BASALT_SH), (11, 2, BASALT), (13, 13, BASALT_SH)]  # Small basalt pebbles
    return na.soil_tile(RED_SOIL, RED_SOIL_DK, layers)  # Red soil with its layer and pebbles


def ledge_pineapple():
    """A 16x10 raised red earth terrace with a grass top."""
    flecks = [(4, 4, RED_SOIL_DK), (10, 5, RED_SOIL), (13, 3, RED_SOIL_DK), (7, 6, RED_SOIL),  # Soil texture
              (3, 0, GRASS_LT), (11, 0, GRASS_LT)]  # Sunny grass tips
    return na.ledge_tile(RED_SOIL_LT, GRASS, RED_SOIL_DK, flecks)  # Clay body with grass on top


def basalt_boulder(w, h, cap):
    """A dark basalt boulder with small gas holes and a thin cap of grass or moss."""
    return na.capped_boulder(w, h, BASALT, BASALT_SH, BASALT_LT, BASALT_PORE, cap, 1)  # Dark rock with a green cap


def pineapple_plant():
    """A 12x12 pineapple plant: spiky blue-green leaves round a yellow fruit (decoration only)."""
    img = mm.new_image(12, 12)  # Empty picture
    for (tx, ty) in ((0, 6), (1, 3), (10, 2), (11, 6), (3, 8), (8, 8)):  # Leaf tips around the plant
        steps = 8  # Points along each leaf
        for t in range(steps + 1):  # From the base to the tip
            x = round(6 + (tx - 6) * t / steps)  # Column of this leaf pixel
            y = round(11 + (ty - 11) * t / steps)  # Row of this leaf pixel
            la.put(img, x, y, PINE_LEAF_SH if t < 4 else PINE_LEAF)  # Darker near the base
    for y in range(4, 9):  # Rows of the fruit
        for x in range(5, 8):  # Columns of the fruit
            la.put(img, x, y, FRUIT_SH if (x + y) % 2 == 0 else FRUIT)  # Yellow fruit with eyes
    for (x, y) in ((5, 2), (6, 1), (7, 2), (6, 3)):  # Little crown on top of the fruit
        la.put(img, x, y, PINE_LEAF)  # Crown leaf
    return img  # No outline, like the other plants


def farm_lookout():
    """A 56x48 wooden farm lookout tower with a grass roof, a ladder and pineapple plants (the E1 goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    for y in range(14, 48):  # Rows of the two slanted legs
        lean = (47 - y) * 5 // 33  # Legs lean in going up
        mm.fill_rect(img, 13 + lean, y, 2, 1, WOOD_SH)  # Left leg
        mm.fill_rect(img, 40 - lean, y, 2, 1, WOOD_SH)  # Right leg
    for y in range(14, 48):  # Rows of the two inner legs
        mm.fill_rect(img, 22, y, 2, 1, WOOD)  # Inner left leg
        mm.fill_rect(img, 32, y, 2, 1, WOOD)  # Inner right leg
    for (y0, y1) in ((16, 28), (28, 40)):  # Two levels of cross braces
        for t in range(y1 - y0 + 1):  # Down the brace
            x_a = 15 + (t * 22) // (y1 - y0)  # Brace going down to the right
            x_b = 37 - (t * 22) // (y1 - y0)  # Brace going down to the left
            la.put(img, x_a, y0 + t, WOOD_SH)  # One pixel of the first brace
            la.put(img, x_b, y0 + t, WOOD_SH)  # One pixel of the second brace
    mm.fill_rect(img, 13, 28, 30, 1, WOOD)  # Middle cross beam
    mm.fill_rect(img, 10, 12, 38, 3, WOOD)  # Lookout floor
    mm.fill_rect(img, 10, 12, 38, 1, WOOD_LT)  # Sunny top of the floor
    for x in (11, 20, 29, 37, 46):  # Rail posts
        mm.fill_rect(img, x, 6, 1, 6, WOOD_SH)  # One rail post
    mm.fill_rect(img, 11, 7, 36, 1, WOOD)  # Top rail
    for y in range(0, 6):  # Roof rows
        half = 13 + y * 2  # Roof gets wider going down
        for x in range(28 - half, 28 + half + 1):  # Pixels of the roof row
            la.put(img, x, y, THATCH_SH if (x + y) % 4 == 0 else THATCH)  # Grass roof with lines
    for y in range(15, 48):  # Ladder rows
        la.put(img, 44, y, WOOD)  # Left ladder rail
        la.put(img, 47, y, WOOD)  # Right ladder rail
        if y % 4 == 1:  # Every fourth row
            mm.fill_rect(img, 45, y, 2, 1, WOOD_SH)  # A ladder rung
    img = la.outline(img)  # Dark edge round the tower
    mm.paste(img, pineapple_plant(), 0, 36)  # A pineapple plant at the foot, left
    mm.paste(img, pineapple_plant(), 24, 36)  # A pineapple plant between the legs
    return img  # The finished picture


# ---------- Lake Plateau: Lake Lanao (E2) ----------

def lake_top():
    """A 16x8 lake-shore tile: short green grass over grey volcanic tuff, with a few reed blades."""
    specks = [(1, 1, REED_DK), (1, 2, REED_DK), (8, 1, REED_DK), (12, 2, GRASS_SH), (5, 2, GRASS_LT),  # Reed blades and grass
              (4, 6, TUFF_DK), (11, 6, TUFF_DK)]  # Dark specks in the tuff
    return na.ground_top(TUFF, GRASS, GRASS_LT, GRASS_SH, specks)  # Grass cap over grey tuff


def lake_soil():
    """A 16x16 grey volcanic tuff tile with wavy darker layers and small rock bits like breccia."""
    layers = [(x, 4 + la.wave(x, 1), TUFF_SH) for x in range(16)]  # A wavy darker ash layer
    layers += [(x, 11 + la.wave(x + 5, 1), TUFF_SH) for x in range(16)]  # A second ash layer lower down
    layers += [(2, 8, BASALT), (3, 8, BASALT), (3, 7, BASALT_SH), (10, 14, CLAST_RED), (11, 14, CLAST_RED),  # Rock bits
               (12, 7, BASALT), (7, 1, CLAST_RED), (14, 1, TUFF_LT), (6, 13, TUFF_LT)]  # More bits and pale pumice
    return na.soil_tile(TUFF, TUFF_DK, layers)  # Tuff with its layers and bits


def ledge_lake():
    """A 16x10 ledge of cut grey tuff stone with a grass top."""
    flecks = [(3, 4, BASALT), (4, 4, BASALT_SH), (9, 6, CLAST_RED), (12, 3, TUFF_DK), (7, 3, TUFF_LT),  # Breccia bits
              (2, 0, GRASS_LT), (10, 0, GRASS_LT)]  # Sunny grass tips
    flecks += [(x, 5, TUFF_SH) for x in range(16) if x % 5]  # A thin layer line with small gaps
    return na.ledge_tile(TUFF, GRASS, TUFF_SH, flecks)  # Tuff body with grass on top


def tuff_boulder(w, h):
    """A pale grey tuff boulder with dark rock bits and a cap of green moss."""
    return na.capped_boulder(w, h, TUFF, TUFF_SH, TUFF_LT, BASALT, GRASS_SH, 1)  # Grey rock with moss


def torogan():
    """A 56x48 Maranao torogan house on round stilts, with the upswept roof ridge and roof ends
    and coloured carved beam ends (the E2 goal). No writing and no religious signs."""
    img = mm.new_image(56, 48)  # Empty picture
    for x in (9, 18, 27, 36, 45):  # Round tree-trunk stilts
        mm.fill_rect(img, x, 33, 3, 13, WOOD_SH)  # One stilt
        la.put(img, x, 34, WOOD)  # Light on the round stilt, top
        mm.fill_rect(img, x, 35, 1, 10, WOOD)  # Light side of the stilt
        mm.fill_rect(img, x - 1, 46, 5, 2, TUFF_SH)  # Flat stone under the stilt
    mm.fill_rect(img, 6, 31, 44, 3, WOOD_SH)  # Floor beam
    mm.fill_rect(img, 6, 31, 44, 1, WOOD)  # Sunny top of the floor beam
    mm.fill_rect(img, 8, 21, 40, 10, WOOD)  # House wall
    for x in range(8, 48, 5):  # Wall panels
        mm.fill_rect(img, x, 21, 1, 10, WOOD_SH)  # One panel line
    for x in range(11, 46, 5):  # Small slatted windows in a row
        mm.fill_rect(img, x, 24, 3, 4, mm.OUTLINE)  # Dark window
        la.put(img, x + 1, 25, WOOD_SH)  # Slat across the window
        la.put(img, x + 1, 27, WOOD_SH)  # Second slat
    for side in (-1, 1):  # Carved beam ends sweep out on both sides
        for t in range(7):  # Along the flared end, from the beam out to its tip
            x = 27 + side * (22 + t) + (1 if side > 0 else 0)  # Column of this part of the beam end
            top = 31 - (t * t) // 8  # The end curls upward toward its tip
            for y in range(top, 34 - t // 3):  # Rows of this column of the beam end
                col = OKIR_RED  # Red carved wood
                if y == top:  # Top edge of the flare
                    col = OKIR_YELLOW  # Yellow line
                elif y == top + 1 and t < 5:  # Under the yellow line
                    col = OKIR_GREEN  # Green line
                la.put(img, x, y, col)  # Paint it
    for x in range(3, 53):  # Columns of the big roof
        t = abs(x - 27.5) / 24.5  # 0 in the middle, 1 at the roof ends
        top = 8 - round(7 * t ** 4)  # The ridge sweeps up at both ends
        eave = 21 - round(4 * t ** 4)  # The eave line also sweeps up at the ends
        for y in range(top, eave + 1):  # Rows of this roof column
            col = THATCH_SH if (x + 2 * y) % 5 == 0 else THATCH  # Roof with lines
            if y == top:  # The ridge line
                col = ROOF_DARK  # Dark ridge
            la.put(img, x, y, col)  # Paint it
    return la.outline(img)  # Dark edge


def lake_reed(frame):
    """An 8x16 clump of lake reeds with brown heads; frame 1 bends the tips right (decoration only)."""
    img = mm.new_image(8, 16)  # Empty picture
    for (x, h) in ((1, 11), (3, 15), (5, 13), (7, 9)):  # Reeds: column and height
        for t in range(h):  # Up the reed from the ground row 15
            lean = frame if t > h * 2 // 3 else 0  # Top third leans in frame 1
            la.put(img, min(7, x + lean), 15 - t, REED_DK if t < 4 else REED)  # One reed pixel
    for (x, h) in ((3, 15), (5, 13)):  # The two tall reeds carry a head
        for d in (0, 1, 2):  # Head is three pixels long
            la.put(img, min(7, x + frame), 15 - h + 1 + d, REED_HEAD)  # Brown head pixel
    return img  # No outline, like the other plants


def banca(frame):
    """A 24x10 small outrigger boat; frame 1 sits one pixel lower, so it bobs (decoration only)."""
    img = mm.new_image(24, 10)  # Empty picture
    dy = frame  # How far down this frame sits
    for x in range(2, 22):  # Columns of the hull
        t = abs(x - 11.5) / 9.5  # 0 in the middle, 1 at the ends
        top = 3 + dy - (1 if t > 0.85 else 0)  # Bow and stern rise a little
        bottom = 6 + dy - round(2 * t ** 2)  # The hull narrows to points at the ends
        for y in range(top, bottom + 1):  # Rows of this hull column
            la.put(img, x, y, BOAT if y < 5 + dy else BOAT_SH)  # Light upper hull, darker lower hull
    mm.fill_rect(img, 5, 4 + dy, 14, 1, OKIR_RED)  # Painted stripe along the hull
    mm.fill_rect(img, 9, 4 + dy, 2, 1, OKIR_YELLOW)  # Yellow mark in the stripe
    mm.fill_rect(img, 14, 4 + dy, 2, 1, OKIR_GREEN)  # Green mark in the stripe
    for x in (7, 16):  # Two outrigger arms across the hull
        for y in range(2 + dy, 8 + dy):  # From just above the hull down to the float
            la.put(img, x, y, BAMBOO_SH)  # One arm pixel
    mm.fill_rect(img, 3, 8 + dy, 18, 1, BAMBOO)  # Bamboo float in front of the hull, at the water line
    return la.outline(img)  # Dark edge


# ---------- Falls Edge: Maria Cristina Falls (E3) ----------

def falls_top():
    """A 16x8 tile of dark wet moss over dark basalt, with small fern leaves and mist drops."""
    specks = [(2, 1, GRASS_SH), (3, 0, GRASS_SH), (9, 1, GRASS_SH), (10, 0, GRASS_SH), (14, 2, GRASS_SH),  # Fern leaves
              (6, 1, LAKE_LT), (12, 3, LAKE_LT), (4, 6, BASALT_PORE), (11, 6, BASALT_PORE)]  # Mist drops and gas holes
    return na.ground_top(BASALT, MOSS, GRASS_SH, MOSS_DK, specks)  # Moss cap over basalt


def falls_soil():
    """A 16x16 dark basalt tile with column joints, so it looks like columnar basalt."""
    extra = []  # Joint lines and wet shine
    for x in (0, 5, 10):  # Three column joints across the tile
        for y in range(16):  # The joint runs top to bottom
            extra.append((x, y, BASALT_SH))  # Dark joint line
            extra.append((x + 1, y, BASALT_LT if y % 8 < 5 else BASALT))  # Light edge beside the joint
    for (x, y) in ((3, 4), (8, 12), (13, 7)):  # Short cross cracks in each column
        extra += [(x, y, BASALT_SH), (x + 1, y, BASALT_SH)]  # One crack
    extra += [(7, 2, LAKE), (7, 3, LAKE), (14, 10, LAKE)]  # Wet shine from the mist
    return na.soil_tile(BASALT, BASALT_PORE, extra)  # Basalt with its columns


def ledge_falls():
    """A 16x10 ledge of dark basalt with a mossy top and fern bits."""
    flecks = [(4, 4, BASALT_SH), (5, 4, BASALT_LT), (11, 5, BASALT_SH), (12, 5, BASALT_LT), (8, 6, BASALT_PORE),  # Rock texture
              (2, 0, GRASS_SH), (9, 0, GRASS_SH), (14, 1, LAKE_LT)]  # Fern bits and a mist drop
    return na.ledge_tile(BASALT, MOSS, BASALT_SH, flecks)  # Basalt body with moss on top


def falls_deck():
    """A 56x48 wooden viewing deck with a rail, on dark basalt rock, with a fern (the E3 goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    for x in range(56):  # Columns of the basalt rock under the deck
        top = 37 + round(2 * math.sin(x / 5.0)) + (1 if x % 7 == 3 else 0)  # Bumpy rock top
        for y in range(top, 48):  # Rows of rock in this column
            col = BASALT  # Basalt
            if x % 7 == 0:  # A column joint
                col = BASALT_SH  # Dark joint
            elif x % 7 == 1:  # Beside the joint
                col = BASALT_LT  # Light edge
            if y == top:  # Top of the rock
                col = MOSS  # A little moss on top
            la.put(img, x, y, col)  # Paint it
    for x in (7, 27, 47):  # Posts that hold up the deck
        mm.fill_rect(img, x, 29, 2, 10, WOOD_SH)  # One post down into the rock
    mm.fill_rect(img, 3, 26, 50, 3, WOOD)  # Deck planks
    mm.fill_rect(img, 3, 26, 50, 1, WOOD_LT)  # Sunny top of the deck
    for x in range(3, 53, 6):  # Plank ends along the deck edge
        la.put(img, x, 28, WOOD_SH)  # One plank end
    for x in (4, 16, 28, 40, 51):  # Rail posts
        mm.fill_rect(img, x, 13, 2, 13, WOOD_SH)  # One rail post
        la.put(img, x, 13, WOOD)  # Lit post top
    mm.fill_rect(img, 4, 13, 49, 2, WOOD)  # Top rail
    mm.fill_rect(img, 4, 13, 49, 1, WOOD_LT)  # Sunny top of the rail
    mm.fill_rect(img, 4, 19, 49, 1, WOOD_SH)  # Middle rail
    img = la.outline(img)  # Dark edge
    mm.paste(img, fern(0), 42, 27)  # A fern growing on the rock below the deck
    return img  # The finished picture


def fern(frame):
    """A 12x12 fern: three arching fronds with leaflets on both sides; frame 1 lifts the frond tips (decoration only)."""
    img = mm.new_image(12, 12)  # Empty picture
    for (side, reach, rise) in ((-1, 5, 6), (1, 5, 6), (0, 0, 9)):  # Fronds: side, how far out, how high
        for t in range(10):  # Ten steps along the frond, from the base to the tip
            u = t / 9.0  # 0 at the base, 1 at the tip
            x = round(5.5 + side * reach * u)  # Column of this frond pixel
            y = round(11 - rise * math.sin(math.pi * u * (0.75 if side else 0.5)))  # The frond arches up and out
            if u > 0.6:  # Near the tip
                y -= frame  # Frame 1 lifts the tip
            la.put(img, x, y, MOSS)  # Frond stem
            if 2 <= t <= 8 and t % 2 == 0:  # Every other step has a pair of leaflets
                if side:  # A side frond: leaflets above and below the stem
                    la.put(img, x, y - 1, GRASS_SH)  # Leaflet above
                    la.put(img, x, y + 1, GRASS_SH)  # Leaflet below
                else:  # The middle frond: leaflets left and right
                    la.put(img, x - 1, y, GRASS_SH)  # Leaflet on the left
                    la.put(img, x + 1, y, GRASS_SH)  # Leaflet on the right
    la.put(img, 5, 11, MOSS_DK)  # Base of the fern
    la.put(img, 6, 11, MOSS_DK)  # Base of the fern
    return img  # No outline, like the other plants


def falls_spray(frame):
    """A 16x12 soft white puff of waterfall mist; frame 1 rises and spreads a little (decoration only)."""
    img = mm.new_image(16, 12)  # Empty picture
    for (cx, cy, r) in ((4, 8, 3), (9, 6, 4), (13, 8, 2)):  # Puffs: middle x, middle y, radius
        cy -= frame  # Frame 1 rises one pixel
        for y in range(12):  # Each row
            for x in range(16):  # Each pixel
                d = (x - cx) ** 2 + (y - cy) ** 2  # Square distance from the middle of the puff
                if d <= r * r:  # Inside the puff
                    la.put(img, x, y, FOAM if d < (r - 1) ** 2 else LAKE_LT, 170 if d < (r - 1) ** 2 else 120)  # Soft white middle, pale edge
    for (x, y) in ((2 + frame, 2), (12 - frame, 1 + frame)):  # Two tiny drops above the mist
        la.put(img, x, y, FOAM, 200)  # One drop
    return img  # No outline, mist is soft


# ---------- Cliff updraft (new object) ----------

STREAKS = [(5, 0, 2, 7), (12, 9, 1, 5), (20, 4, 2, 8), (27, 13, 1, 6)]  # Wind streaks: column, phase, sway, length


def updraft_pattern(x, sy):
    """The wind picture at column x and pattern row sy (0 to 31). It repeats every 32 rows, so it stacks with no seam.
    Gives (colour, strength) or None for an empty pixel."""
    for (x0, phase, sway, length) in STREAKS:  # Each rising streak
        wx = x0 + round(sway * math.sin(2 * math.pi * (sy + phase) / 32.0))  # The streak sways side to side
        k = (sy + phase) % 16  # Place along the dash, which repeats every 16 rows
        if x == wx and k < length:  # This pixel is on a dash
            return (WIND, 230) if k < 2 else (WIND, 150)  # Bright head at the top, softer tail below
    for (cx, cy) in ((9, 6), (23, 22)):  # Two small swirls
        dy = (sy - cy + 16) % 32 - 16  # Row distance, wrapped so the swirl repeats
        dx = x - cx  # Column distance
        d2 = dx * dx + dy * dy  # Square distance from the swirl middle
        if 5 <= d2 <= 10 and not (dx > 0 and dy > 0):  # A ring with a gap on the lower right
            return (WIND, 140)  # Soft swirl pixel
    return None  # Empty, so most of the column is see-through


def updraft(frame):
    """A 32x32 updraft frame. Each frame moves the wind 8 rows up, so frames 0 to 3 loop."""
    img = mm.new_image(32, 32)  # Empty picture
    for y in range(32):  # Each row
        sy = (y + frame * 8) % 32  # Row of the pattern shown here, so the wind rises frame to frame
        for x in range(32):  # Each pixel
            got = updraft_pattern(x, sy)  # Wind here or not
            if got:  # There is wind here
                la.put(img, x, y, got[0], got[1])  # See-through wind pixel
    return img  # The finished picture


def updraft_base():
    """A 32x8 flat basalt rock with a dark crack in the middle where the wind comes up."""
    img = mm.new_image(32, 8)  # Empty picture
    for x in range(1, 31):  # Columns of the rock
        top = 3 if 3 <= x <= 28 else 4  # Rounded ends
        for y in range(top, 8):  # Rows of the rock
            col = BASALT_LT if y == top else BASALT  # Sunny top edge
            if (x + y) % 9 == 0:  # A few gas holes
                col = BASALT_PORE  # Dark hole
            la.put(img, x, y, col)  # Paint it
    img = la.outline(img)  # Dark edge
    for y in range(2, 8):  # Rows of the crack
        half = max(0, 3 - (y - 2) // 2)  # The crack is wider at the top
        mm.fill_rect(img, 16 - half, y, half * 2, 1, mm.OUTLINE)  # Dark crack
    for (x, y) in ((14, 1), (17, 0), (15, 0), (18, 1)):  # Wisps of wind leaving the crack
        la.put(img, x, y, WIND, 150)  # One soft wisp pixel
    return img  # The finished picture


def updraft_leaf(frame):
    """A 6x6 small green leaf caught in the updraft; frame 1 is turned edge-on, so it spins."""
    img = mm.new_image(6, 6)  # Empty picture
    if frame == 0:  # Leaf seen flat, lying on a slant
        for (x, y) in ((1, 4), (2, 3), (3, 2), (4, 1), (2, 2), (3, 3), (3, 1), (4, 2)):  # Leaf pixels
            la.put(img, x, y, GRASS)  # Green leaf
        la.put(img, 0, 5, WOOD_SH)  # Leaf stem
        la.put(img, 2, 3, GRASS_SH)  # Middle vein
    else:  # Leaf turned edge-on, the other slant
        for (x, y) in ((1, 1), (2, 2), (3, 3), (4, 4)):  # Thin leaf pixels
            la.put(img, x, y, GRASS_SH)  # Leaf edge, darker
        la.put(img, 2, 1, GRASS)  # Bit of the leaf face
        la.put(img, 5, 5, WOOD_SH)  # Leaf stem
    return img  # The finished picture


def updraft_dust(frame):
    """A 6x6 few dust grains lifted by the wind; frame 1 moves them up and apart."""
    img = mm.new_image(6, 6)  # Empty picture
    grains = ((1, 4), (3, 2), (4, 5), (2, 1)) if frame == 0 else ((0, 3), (3, 1), (5, 4), (2, 0))  # Grain places
    for i, (x, y) in enumerate(grains):  # Each grain
        la.put(img, x, y, DUST if i % 2 == 0 else DUST_SH, 220)  # Tan grain
    return img  # The finished picture


# ---------- Props (decoration) ----------

def hawk(frame):
    """A 16x10 brown hawk gliding, seen from below; frame 0 lifts the wing tips, frame 1 lowers them (decoration only)."""
    img = mm.new_image(16, 10)  # Empty picture
    lift = 3 if frame == 0 else -1  # How far the wing tips rise
    for x in range(1, 15):  # Columns of the wings
        dx = abs(x - 7.5)  # Distance from the body
        y0 = 4 - round(lift * (dx - 0.5) / 6.5)  # Wing line, rising or falling to the tip
        thick = 3 if dx < 4 else (2 if dx < 6 else 1)  # Wings are thick at the body and thin at the tip
        for y in range(y0, y0 + thick):  # Rows of the wing at this column
            la.put(img, x, y, HAWK_SH if dx > 5.5 or y == y0 + thick - 1 and dx > 2 else HAWK)  # Brown wing, dark tips and back edge
    mm.fill_rect(img, 7, 2, 2, 6, HAWK_BELLY)  # Pale body and belly
    mm.fill_rect(img, 7, 1, 2, 1, HAWK_BELLY)  # Head
    la.put(img, 7, 0, mm.HELMET)  # Yellow hooked beak
    for x in range(6, 10):  # Spread tail fan
        la.put(img, x, 8, HAWK_SH)  # Tail feather
    mm.fill_rect(img, 7, 9, 2, 1, HAWK_SH)  # Tail tip
    return la.outline(img)  # Dark edge so the bird reads on the sky


# ---------- New minerals (24x24, Assets/Art/Items/Sprites/) ----------

def halloysite():
    """A white-cream earthy lump of halloysite clay with small brown pits."""
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x, y) < 7:  # A few earthy pits
            return HALLOY_PIT  # Brown pit
        return HALLOY_SH if s else HALLOY  # Cream clay, shaded at the lower right
    return ls.outline(na.blob(24, 24, 11.5, 13.0, 10.0, 7.5, c, 2.3))  # Soft earthy lump


def goethite():
    """A brown-black lump of goethite with rusty yellow-brown streaks."""
    def c(x, y, s):  # Colour of one pixel
        k = (x + y // 2) % 7  # Slanting stripes across the lump
        if k == 0:  # A rusty streak
            return GOETH_RUST  # Yellow-brown streak
        if k == 1:  # Beside the streak
            return GOETH_RUST_DK  # Darker rust
        return GOETH_SH if s else GOETH  # Brown-black rock
    img = na.blob(24, 24, 11.5, 13.0, 10.0, 8.0, c, 2.0)  # Round lump
    for (x, y) in ((7, 9), (8, 8), (9, 9)):  # A small dull shine near the top left
        la.put(img, x, y, mm.MASK_SH)  # Grey shine
    return ls.outline(img)  # Dark edge


def gibbsite():
    """A soft pale white-grey lump of gibbsite with a pearly shine."""
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x, y) < 5:  # A few soft grey spots
            return GIBB_SH  # Grey spot
        return GIBB_SH if s else GIBB  # Pale grey, shaded at the lower right
    img = na.blob(24, 24, 11.5, 13.5, 9.5, 7.0, c, 2.0)  # Soft lump
    for (x, y) in ((6, 11), (7, 10), (8, 9), (9, 9), (10, 8), (7, 11), (8, 10)):  # Pearly shine on the top left
        la.put(img, x, y, GIBB_SHINE)  # Shine pixel
    la.put(img, 9, 8, mm.WHITE)  # Brightest point of the shine
    return ls.outline(img)  # Dark edge


def cristobalite():
    """Small white snowflake-like balls of cristobalite sitting in dark volcanic glass."""
    def c(x, y, s):  # Colour of one pixel
        return GLASS_SH if s else GLASS  # Dark glass, shaded at the lower right
    img = na.blob(24, 24, 11.5, 13.0, 10.5, 8.0, c, 2.2)  # Dark glassy rock
    for (x, y) in ((6, 10), (7, 9)):  # Glassy shine on the top left
        la.put(img, x, y, GLASS_SHINE)  # Shine pixel
    for (cx, cy) in ((9, 12), (14, 10), (16, 15), (10, 17), (5, 14), (13, 15)):  # Middle of each white ball
        for (dx, dy) in ((0, 0), (1, 0), (-1, 0), (0, 1), (0, -1)):  # A small plus shape, like a snowflake
            la.put(img, cx + dx, cy + dy, BALL)  # White ball pixel
        la.put(img, cx + 1, cy + 1, BALL_SH)  # Shaded lower right of the ball
    return ls.outline(img)  # Dark edge


# ---------- Lesson pictures (120x82, Assets/Art/Lesson/Sprites/) ----------

VOLCANO = [(88, 6, 0.9)]  # Far volcano behind the Bukidnon plateau: x, top y, slope
PLATEAU_TOP = 30  # Row of the flat top of the Bukidnon plateau
LOWLAND = 48  # Row where the low land starts beside the plateau


def bukidnon_pixel(x, y):
    """Colour of one pixel of the Bukidnon picture: sky, a far volcano, a flat high plateau with steep red sides,
    pineapple rows on top and low green land below."""
    left = 14 - (y - PLATEAU_TOP) // 3  # Left cliff edge, steep
    right = 104 + (y - PLATEAU_TOP) // 3  # Right cliff edge, steep
    on_plateau = y >= PLATEAU_TOP and left <= x <= right  # Inside the plateau shape
    if not on_plateau:  # Sky, volcano or low land
        if y >= LOWLAND:  # Low land around the plateau foot
            return mm.LEAF if lm.noise(x, y) < 25 else GRASS_SH  # Green low land with trees
        top, peak = lm.ridge(x, VOLCANO)  # The far volcano
        if y >= top:  # Inside the volcano
            return lm.HAZE if x <= peak[0] else lm.HAZE_SH  # Lit or shaded haze
        return na.sky(x, y)  # Sky
    d = y - PLATEAU_TOP  # Depth below the flat top
    if d < 9:  # The flat top with pineapple rows
        if d % 3 == 2:  # Red soil between the rows
            return RED_SOIL  # Soil
        return PINE_LEAF_SH if (x + d) % 3 == 0 else PINE_LEAF  # Blue-green pineapple row
    if d == 9:  # Grass lip at the cliff edge
        return GRASS  # Grass
    if x <= left + 1 or x >= right - 1:  # The very edge of each cliff
        return RED_SOIL_DK  # Dark edge
    band = (d + la.wave(x, 1, 16)) % 6  # Rock layers in the cliff
    if band == 0:  # A dark layer line
        return RED_SOIL_DK  # Dark layer
    return RED_SOIL if x < 60 else mix(mm.PLATEAU_SH, mm.BACKPACK_SH)  # Red cliff, darker on the right side


def lesson_bukidnon():
    """A 120x82 framed picture of a wide flat high plateau with steep sides, pineapple rows and a far volcano."""
    img = ls.framed(bukidnon_pixel)  # Sky, volcano, plateau and low land
    mm.paste(img, pineapple_plant(), 30, 24)  # A big pineapple plant on the plateau edge
    mm.paste(img, pineapple_plant(), 78, 24)  # A second pineapple plant
    return img  # The finished picture


LANAO_FAR = [(10, 12, 0.5), (44, 9, 0.45), (76, 11, 0.5), (110, 10, 0.55)]  # Far mountains round the lake
LANAO_NEAR = [(4, 20, 0.7), (30, 24, 0.6), (92, 23, 0.6), (116, 19, 0.7)]  # Nearer green mountains
LAKE_TOP = 30  # Row where the lake starts
SHORE = 64  # Row where the near shore starts


def lanao_pixel(x, y):
    """Colour of one pixel of the Lanao picture: sky, a ring of mountains, a big lake and the near plateau shore."""
    if y >= SHORE:  # Near shore on the plateau
        if y == SHORE:  # Grass lip
            return GRASS_LT  # Sunny grass edge
        if y < SHORE + 6:  # Grass
            return GRASS if lm.noise(x, y) > 15 else GRASS_SH  # Grass with shade
        return TUFF if (y + la.wave(x, 1)) % 4 else TUFF_SH  # Layered grey tuff bank
    if y >= LAKE_TOP:  # The lake
        if y % 3 == 0 and lm.noise(x // 3, y) < 4:  # A few short flat glints
            return LAKE_LT  # Pale glint
        if y < LAKE_TOP + 3:  # Far water near the mountains
            return LAKE_DEEP  # Darker, deeper blue
        return LAKE  # Lake blue
    near_top, near_peak = lm.ridge(x, LANAO_NEAR)  # Nearer mountains
    if y >= near_top:  # Inside a near mountain
        return GRASS_SH if x <= near_peak[0] else mm.LEAF  # Green, lit or shaded
    far_top, far_peak = lm.ridge(x, LANAO_FAR)  # Far mountains
    if y >= far_top:  # Inside a far mountain
        return lm.HAZE if x <= far_peak[0] else lm.HAZE_SH  # Haze, lit or shaded
    return na.sky(x, y)  # Sky


def lesson_lanao():
    """A 120x82 framed picture of a big lake on a high plateau ringed by mountains, with a small outrigger boat."""
    img = ls.framed(lanao_pixel)  # Sky, mountains, lake and shore
    mm.paste(img, banca(0), 70, 42)  # A small boat on the lake
    mm.paste(img, lake_reed(0), 8, 50)  # Reeds at the shore, left
    mm.paste(img, lake_reed(1), 104, 50)  # Reeds at the shore, right
    return img  # The finished picture


FALLS = [(30, 46), (64, 84)]  # The two falls: left and right columns at the top
FALLS_TOP = 16  # Row of the cliff lip where the water pours over
POOL = 64  # Row of the pool at the foot of the falls


def maria_cristina_pixel(x, y):
    """Colour of one pixel of the Maria Cristina picture: twin waterfalls pouring over dark basalt cliffs into a misty pool."""
    if y < 8:  # Sky
        return na.sky(x, y)  # Sky colour
    if y < FALLS_TOP:  # Forest and river on top of the cliff
        for (a, b) in FALLS:  # Each falls
            if a <= x <= b and y >= FALLS_TOP - 3:  # The river right above the lip
                return LAKE  # River water
        return mm.LEAF if lm.noise(x, y) < 40 else GRASS_SH  # Forest on the plateau top
    if y < POOL:  # The cliff face
        spread = (y - FALLS_TOP) // 8  # The falls get a little wider going down
        for (a, b) in FALLS:  # Each falls
            if a - spread <= x <= b + spread:  # Inside the falling water
                k = (x * 3 + y // 2) % 5  # Streaks down the water
                if k == 0:  # A blue streak
                    return LAKE  # Blue water
                return LAKE_LT if k == 1 else FOAM  # Pale and white water
        if y >= POOL - 8 and lm.noise(x, y) < 45:  # Mist at the foot of the cliff
            return LAKE_LT  # Pale mist
        if x % 6 == 0:  # A basalt column joint
            return BASALT_SH  # Dark joint
        if x % 6 == 1:  # Beside the joint
            return BASALT_LT  # Light column edge
        if lm.noise(x, y) < 6:  # A few ferns on the cliff
            return MOSS  # Fern
        return BASALT  # Dark basalt
    if y < POOL + 10:  # The pool
        if lm.noise(x, y) < 30 and (22 <= x <= 54 or 56 <= x <= 92):  # Foam where the water lands
            return FOAM  # White foam
        return LAKE if (x + y) % 5 else LAKE_LT  # Pool water with shine
    return BASALT_SH if lm.noise(x, y) < 30 else BASALT  # Dark rocks at the front


def lesson_maria_cristina():
    """A 120x82 framed picture of a twin waterfall pouring over dark basalt cliffs, with mist and ferns."""
    img = ls.framed(maria_cristina_pixel)  # Sky, cliffs, falls and pool
    mm.paste(img, fern(0), 6, 70)  # A fern on the front rocks, left
    mm.paste(img, fern(1), 102, 70)  # A fern on the front rocks, right
    return img  # The finished picture


# ---------- Plateaus life sprites for the Landform Map and the Plateaus menu screens ----------

def hawk_life(frame):
    """A 7x4 tiny brown hawk for the Plateaus sky; frame 0 has wings up, frame 1 has wings down."""
    img = mm.new_image(7, 4)  # Empty picture
    if frame == 0:  # Wings up
        wing = ((0, 0), (1, 1), (2, 1), (4, 1), (5, 1), (6, 0))  # Wing pixels
    else:  # Wings down
        wing = ((0, 2), (1, 2), (2, 1), (4, 1), (5, 2), (6, 2))  # Wing pixels
    for (x, y) in wing:  # Each wing pixel
        la.put(img, x, y, HAWK_SH if x in (0, 6) else HAWK)  # Brown wing, dark tips
    la.put(img, 3, 1, HAWK_BELLY)  # Pale body
    la.put(img, 3, 2, HAWK)  # Lower body
    la.put(img, 3, 3, HAWK_SH)  # Tail
    return img  # The finished picture


def waterfall_life(frame):
    """A 4x10 tiny waterfall; the white and blue streaks move down from frame 0 to frame 1, so it shines."""
    img = mm.new_image(4, 10)  # Empty picture
    for y in range(10):  # Each row
        for x in range(4):  # Each column
            k = (x + y - frame * 2) % 4  # Streak pattern that moves down each frame
            la.put(img, x, y, (FOAM, LAKE_LT, LAKE, LAKE_LT)[k])  # White, pale and blue water
    mm.fill_rect(img, 0, 0, 4, 1, LAKE)  # The river at the lip
    return img  # The finished picture


def grass_life(frame):
    """A 7x6 Plateaus grass tuft in green; frame 1 bends the tips right so it sways."""
    img = mm.new_image(7, 6)  # Empty picture
    for x in (1, 3, 5):  # Three blades
        for t in range(6):  # Up the blade, from the ground row 5 to the tip row 0
            la.put(img, x + (frame if t >= 4 else 0), 5 - t, GRASS_SH if t < 3 else GRASS)  # One blade pixel
    return img  # The finished picture


# ---------- Saving and the preview sheet ----------

def map_save(img, name):
    """Saves an image under Assets/Art/LandformMap/Sprites/Plateaus/."""
    os.makedirs(MAP_OUT, exist_ok=True)  # Make the folder if it is missing
    img.save(os.path.join(MAP_OUT, name))  # Write the PNG


def put_file(where, name, img):
    """Saves one picture in the folder for its kind: level, item, lesson or map."""
    if where == "item":  # Shared item picture
        ls.save(img, ls.ITEMS_OUT, name)  # Items folder
    elif where == "lesson":  # Lesson picture
        ls.save(img, ls.LESSON_OUT, name)  # Lesson folder
    elif where == "map":  # Landform Map life sprite
        map_save(img, name)  # Plateaus folder
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


def on_sky(img):
    """Puts a picture on a plain sky block, only for the preview, so pale wind shows up."""
    back = Image.new("RGBA", img.size, lm.SKY + (255,))  # Sky block the same size
    back.alpha_composite(img)  # The picture on top
    return back  # The picture on sky


def stacked_updraft(frame):
    """Three updraft frames stacked on the base, only for the preview, to check that the column has no seams."""
    img = mm.new_image(32, 104)  # Room for three tiles and the base
    for i in range(3):  # Three tiles, one above the other
        mm.paste(img, updraft(frame), 0, i * 32)  # One tile
    mm.paste(img, updraft_base(), 0, 96)  # The rock crack at the bottom
    return on_sky(img)  # On sky, so the pale wind shows


def main():
    """Draws everything, saves each picture and writes the preview sheet."""
    groups = [  # Each group: its title, then (kind, file name, picture) for each picture
        ("E1 PINEAPPLE PLATEAU", [
            ("level", "pineapple_top.png", pineapple_top()),  # Ground top
            ("level", "pineapple_soil.png", pineapple_soil()),  # Ground below the grass
            ("level", "ledge_pineapple.png", ledge_pineapple()),  # Red earth terrace ledge
            ("level", "boulder_small_pineapple.png", basalt_boulder(32, 24, GRASS_SH)),  # Small basalt boulder
            ("level", "boulder_big_pineapple.png", basalt_boulder(48, 36, GRASS_SH)),  # Big basalt boulder
            ("level", "farm_lookout.png", farm_lookout()),  # Goal: farm lookout tower
        ]),
        ("E2 LAKE PLATEAU", [
            ("level", "lake_top.png", lake_top()),  # Ground top
            ("level", "lake_soil.png", lake_soil()),  # Ground below the grass
            ("level", "ledge_lake.png", ledge_lake()),  # Tuff stone ledge
            ("level", "boulder_small_lake.png", tuff_boulder(32, 24)),  # Small tuff boulder
            ("level", "boulder_big_lake.png", tuff_boulder(48, 36)),  # Big tuff boulder
            ("level", "torogan.png", torogan()),  # Goal: torogan house
        ]),
        ("E3 FALLS EDGE", [
            ("level", "falls_top.png", falls_top()),  # Ground top
            ("level", "falls_soil.png", falls_soil()),  # Ground below the moss
            ("level", "ledge_falls.png", ledge_falls()),  # Basalt ledge
            ("level", "boulder_small_falls.png", basalt_boulder(32, 24, MOSS)),  # Small mossy basalt boulder
            ("level", "boulder_big_falls.png", basalt_boulder(48, 36, MOSS)),  # Big mossy basalt boulder
            ("level", "falls_deck.png", falls_deck()),  # Goal: viewing deck
        ]),
        ("CLIFF UPDRAFT", [
            ("level", "updraft_00.png", updraft(0)),  # Updraft, frame 0
            ("level", "updraft_01.png", updraft(1)),  # Updraft, frame 1
            ("level", "updraft_02.png", updraft(2)),  # Updraft, frame 2
            ("level", "updraft_03.png", updraft(3)),  # Updraft, frame 3
            ("level", "updraft_base.png", updraft_base()),  # Rock crack at the foot
            ("level", "Props/updraft_leaf_00.png", updraft_leaf(0)),  # Leaf, flat
            ("level", "Props/updraft_leaf_01.png", updraft_leaf(1)),  # Leaf, edge-on
            ("level", "Props/updraft_dust_00.png", updraft_dust(0)),  # Dust, frame 0
            ("level", "Props/updraft_dust_01.png", updraft_dust(1)),  # Dust, frame 1
        ]),
        ("PLATEAUS PROPS", [
            ("level", "Props/pineapple_plant.png", pineapple_plant()),  # Pineapple plant
            ("level", "Props/hawk_00.png", hawk(0)),  # Hawk, wings up
            ("level", "Props/hawk_01.png", hawk(1)),  # Hawk, wings down
            ("level", "Props/lake_reed_00.png", lake_reed(0)),  # Reeds
            ("level", "Props/lake_reed_01.png", lake_reed(1)),  # Reeds swaying
            ("level", "Props/banca_00.png", banca(0)),  # Boat up
            ("level", "Props/banca_01.png", banca(1)),  # Boat down
            ("level", "Props/fern_00.png", fern(0)),  # Fern
            ("level", "Props/fern_01.png", fern(1)),  # Fern swaying
            ("level", "Props/falls_spray_00.png", falls_spray(0)),  # Falls mist, frame 0
            ("level", "Props/falls_spray_01.png", falls_spray(1)),  # Falls mist, frame 1
        ]),
        ("ITEMS", [
            ("item", "halloysite.png", halloysite()),  # Cream clay mineral
            ("item", "goethite.png", goethite()),  # Brown-black with rust streaks
            ("item", "gibbsite.png", gibbsite()),  # Pale pearly lump
            ("item", "cristobalite.png", cristobalite()),  # White balls in dark glass
        ]),
        ("LESSON PICTURES", [
            ("lesson", "lesson_bukidnon.png", lesson_bukidnon()),  # Pineapple plateau
            ("lesson", "lesson_lanao.png", lesson_lanao()),  # Plateau lake
            ("lesson", "lesson_maria_cristina.png", lesson_maria_cristina()),  # Twin waterfall
        ]),
        ("PLATEAUS LIFE", [
            ("map", "hawk_0.png", hawk_life(0)),  # Hawk wings up
            ("map", "hawk_1.png", hawk_life(1)),  # Hawk wings down
            ("map", "waterfall_0.png", waterfall_life(0)),  # Waterfall, frame 0
            ("map", "waterfall_1.png", waterfall_life(1)),  # Waterfall, frame 1
            ("map", "grass_0.png", grass_life(0)),  # Grass, frame 0
            ("map", "grass_1.png", grass_life(1)),  # Grass, frame 1
        ]),
    ]
    preview_groups = []  # (title, pictures) for the preview sheet
    for title, entries in groups:  # Each group
        sprites = []  # Pictures of this group
        for where, name, img in entries:  # Each picture
            put_file(where, name, img)  # Write it to its folder
            sprites.append(on_sky(img) if name.startswith("updraft_0") or "updraft_" in name and "Props" in name else img)  # Pale wind bits go on sky in the preview
        preview_groups.append((title, sprites))  # Add the group to the sheet
    preview_groups.append(("UPDRAFT COLUMN CHECK", [stacked_updraft(f) for f in range(4)]))  # Stacked columns, preview only
    preview(preview_groups)  # The check sheet
    print("Plateaus Easy art written to", la.OUT, ls.ITEMS_OUT, ls.LESSON_OUT, "and", MAP_OUT)  # Tell where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
