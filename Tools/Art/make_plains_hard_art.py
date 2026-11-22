"""Draws the PNGs for Plains Hard 1 to 7 (D-071): seven level themes (Prairie, Savanna, Pampas, Siberia, Nullarbor,
Ganges and Steppe) with their goals, the rolling hay bale and its straw dust, three herds (wildebeest, cattle and horses),
the frost tile, the new props, three new items and seven lesson pictures.

Run from the Unity project folder:  python Tools/Art/make_plains_hard_art.py
It writes to Assets/Art/Level/Sprites/ (themes, goals, hay, herds, frost and props), Assets/Art/Items/Sprites/ (items),
Assets/Art/Lesson/Sprites/ (lesson pictures), plus a preview sheet at .scratch/plains-hard/art-preview.png.
It reuses the helpers of make_mainmenu_art.py, make_landform_map_art.py, make_level_art.py, make_lesson_art.py and make_normal_art.py.
All colours come from Assets/Art/Palettes/landgrand.gpl or mix two of its colours.
The eagles (Props/eagle_00.png and Props/eagle_01.png) already exist for other levels, so this script does not draw or write them.
"""
import math  # math gives the rolling hay, the windmill wheel and the lesson curves
import os  # os joins folder paths
from PIL import Image, ImageDraw  # Image builds the preview sheet, ImageDraw draws its grid
import make_mainmenu_art as mm  # Palette, new_image, fill_rect, paste and the pixel font
import make_landform_map_art as lm  # Map colours, the fixed noise
import make_level_art as la  # put, outline, wave and save for level sprites
import make_lesson_art as ls  # framed, disc, fill_poly, outline and the item and lesson folders
import make_normal_art as na  # mix, the shared tile makers, sky and blob

mix = na.mix  # The colour halfway between two palette colours
PREVIEW = os.path.join(mm.ROOT, ".scratch", "plains-hard", "art-preview.png")  # Where the preview sheet goes

# Shared colours, each a palette colour or a mix of two.
HAY = mm.HELMET  # Golden straw
HAY_SH = mm.HELMET_SH  # Straw in shade
HAY_LT = mm.HEADLAMP  # Sunny straw
HAY_RIM = mix(mm.HELMET_SH, mm.GROUND_SH)  # Dark rim of the bale
WOOD = mm.GROUND  # Plank and log body
WOOD_DK = mm.GROUND_SH  # Wood in shade
WOOD_LT = mm.VEST  # Sunny wood
POST = lm.HANDLE  # Light post wood
ICE = mm.SKY_LIGHT  # Pale frost shine
SNOW = lm.SNOW  # Snow
SNOW_SH = lm.HAZE  # Snow in shade
GREY_GREEN = mix(mm.MASK_SH, mm.LEAF)  # Sparse grey-green scrub
OLIVE = mix(mm.HILLS_SH, mm.HELMET_SH)  # Olive-green steppe grass
OLIVE_LT = mix(mm.PLAINS, mm.HELMET)  # Sunny olive grass
GRANITE = mix(mm.MASK, mm.PLATEAU)  # Pink-grey granite
GRANITE_SH = mix(mm.MASK_SH, mm.PLATEAU)  # Granite in shade
GRANITE_LT = mix(mm.MASK, mm.WHITE)  # Sunny granite
CREAM = mix(mm.HEADLAMP, mm.VEST)  # Cream limestone
WILDE = mix(mm.GROUND_SH, mm.STRAP)  # Dark grey-brown wildebeest
WILDE_SH = mm.STRAP  # Wildebeest in shade
CHESTNUT = mix(lm.BACKPACK, mm.PLATEAU)  # Chestnut horse coat
CHESTNUT_SH = mm.BACKPACK_SH  # Horse in shade and dark mane
CARA_WHITE = mm.WHITE  # Cow coat
CARA_SPOT = mm.GROUND  # Brown cow patch
GRANITE_SPECK = mm.STRAP  # Dark granite speck
LEG_FRAMES = [  # Leg positions for the three walk frames: (x, bottom row) of each leg
    ((5, 15), (9, 15), (19, 15), (23, 15)),  # Frame 0: all feet down
    ((4, 14), (9, 15), (20, 15), (23, 14)),  # Frame 1: two legs lifted
    ((6, 15), (8, 14), (18, 14), (22, 15)),  # Frame 2: the other two legs lifted
]


def ell(img, cx, cy, rx, ry, light, dark=None):
    """Paints a filled ellipse; the lower right part uses the dark colour when one is given."""
    for y in range(img.height):  # Each row
        for x in range(img.width):  # Each pixel of the row
            if ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2 <= 1.0:  # Inside the ellipse
                shaded = dark is not None and (x - cx) / rx + (y - cy) / ry > 0.35  # Lower right is in shade
                la.put(img, x, y, dark if shaded else light)  # Paint the pixel


def line(img, x0, y0, x1, y1, color):
    """Paints a straight line of pixels from one point to another."""
    steps = max(abs(x1 - x0), abs(y1 - y0), 1)  # One pixel per step along the longer side
    for i in range(steps + 1):  # Each step, including both ends
        t = i / float(steps)  # How far along the line
        la.put(img, round(x0 + (x1 - x0) * t), round(y0 + (y1 - y0) * t), color)  # One pixel


def ring(img, cx, cy, r, color):
    """Paints a thin round ring of pixels around a centre."""
    for i in range(96):  # Enough points to close the ring without gaps
        a = 2 * math.pi * i / 96.0  # Angle of this point
        la.put(img, round(cx + r * math.cos(a)), round(cy + r * math.sin(a)), color)  # One pixel of the ring


def big(img, scale):
    """Makes a crisp, larger copy of a picture for the preview."""
    return img.resize((img.width * scale, img.height * scale), Image.NEAREST)  # Nearest-neighbour scaling


# ---------- Themes: top, soil, ledge and the two boulder sizes ----------

def capped(mid, shade, light, speck, cap, rows):
    """Returns a boulder maker: a rock with a cap of grass or snow on its top."""
    return lambda w, h: na.capped_boulder(w, h, mid, shade, light, speck, cap, rows)  # Rock with a cap


def rock(mid, shade, light, speck, band=None):
    """Returns a boulder maker: a plain rock with optional layer lines."""
    return lambda w, h: la.boulder_colored(w, h, mid, shade, light, speck, band)  # Plain rock


def theme(stem, base, cap, light, shade, speck, layer, boulder):
    """Makes the five pieces of one theme. base is the soil, cap the top grass, layer a band in the soil."""
    top = na.ground_top(base, cap, light, shade,  # Grass over soil
                        [(2, 2, light), (6, 1, shade), (11, 2, light), (14, 1, cap), (4, 6, speck), (10, 5, speck)])  # Tufts and specks
    layers = [(x, 5 + la.wave(x, 1), layer) for x in range(16)]  # A wavy layer
    layers += [(x, 12 + la.wave(x, 1), layer) for x in range(16)]  # A second layer lower down
    soil = na.soil_tile(base, speck, layers + [(3, 9, speck), (11, 3, speck)])  # Soil with its layers
    ledge = na.ledge_tile(base, cap, speck, [(4, 4, shade), (10, 5, speck), (13, 3, light)])  # Ledge with a grass lip
    return [(f"{stem}_top.png", top), (f"{stem}_soil.png", soil), (f"ledge_{stem}.png", ledge),  # The first three pieces
            (f"boulder_small_{stem}.png", boulder(32, 24)), (f"boulder_big_{stem}.png", boulder(48, 36))]  # Both boulder sizes


def prairie_tiles():
    """Golden-green tall grass over brown loam, with grey-brown boulders under a grass cap."""
    return theme("prairie", mm.GROUND, mix(mm.PLAINS, mm.HELMET), mm.PLAINS, lm.PLAINS_SH, mm.GROUND_SH, mm.VEST,
                 capped(mix(mm.MASK, mm.GROUND), mix(mm.MASK_SH, mm.GROUND_SH), mm.MASK, mm.STRAP, mm.HILLS, 2))


def savanna_tiles():
    """Dry yellow grass over red-brown earth, with granite grey-pink boulders."""
    return theme("savanna", mix(mm.PLATEAU_SH, mm.GROUND_SH), mm.HELMET, mm.HEADLAMP, mm.HELMET_SH, mm.BACKPACK_SH, mm.PLATEAU,
                 rock(GRANITE, GRANITE_SH, GRANITE_LT, GRANITE_SPECK))


def pampas_tiles():
    """Lush green grass over dark rich soil, with grey boulders under a green cap."""
    return theme("pampas", mix(mm.BACKPACK_SH, mm.GROUND_SH), mm.HILLS, mm.PLAINS, mm.HILLS_SH, mm.OUTLINE, mm.BACKPACK_SH,
                 capped(mm.MASK_SH, mm.STRAP, mm.MASK, mm.OUTLINE, mm.HILLS, 2))


def siberia_tiles():
    """Snowy grass over dark frozen peat, with boulders that have snow caps."""
    return theme("siberia", mix(mm.BACKPACK_SH, mm.OUTLINE), SNOW, mm.WHITE, SNOW_SH, mm.OUTLINE, mm.STRAP,
                 capped(lm.MOUNTAIN, lm.MOUNTAIN_SH, lm.HAZE, mm.STRAP, SNOW, 2))


def nullarbor_tiles():
    """Sparse grey-green scrub over pale cream limestone, with cream limestone boulders in layers."""
    return theme("nullarbor", CREAM, GREY_GREEN, mm.WHITE, mm.VEST_SH, mm.VEST, mm.VEST_SH,
                 rock(CREAM, mm.VEST, mm.WHITE, mm.VEST_SH, (mm.VEST_SH, mm.GROUND_SH)))


def ganges_tiles():
    """A green paddy edge over grey-brown alluvial mud, with smooth grey-brown boulders."""
    return theme("ganges", mix(mm.MASK_SH, mm.GROUND), mm.HILLS, mm.PLAINS, mm.HILLS_SH, mm.STRAP, mm.MASK_SH,
                 rock(mix(mm.MASK_SH, mm.GROUND), mix(mm.GROUND_SH, mm.STRAP), mm.MASK, mm.STRAP))


def steppe_tiles():
    """Short olive-green grass over tan earth, with tan boulders under an olive cap."""
    return theme("steppe", mm.VEST, OLIVE, OLIVE_LT, mm.HILLS_SH, mm.VEST_SH, mm.GROUND_SH,
                 capped(mm.VEST, mm.VEST_SH, mm.HEADLAMP, mm.GROUND_SH, OLIVE, 1))


def frost_top():
    """A 16x8 frozen grass top: pale blue-white frost over grey-blue ground."""
    return na.ground_top(lm.HAZE_SH, SNOW, mm.WHITE, SNOW_SH,
                         [(2, 2, ICE), (6, 1, SNOW_SH), (11, 2, ICE), (14, 1, SNOW_SH), (4, 6, ICE), (10, 5, SNOW_SH)])  # Frost specks


# ---------- Goals (56x48, base on the bottom row) ----------

def hay_barn():
    """A red barn with a dark roof, a hay bale at each side and a grass step (the Prairie goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    mm.fill_rect(img, 10, 22, 36, 22, mm.SCARF)  # Red barn walls
    for x in range(14, 46, 6):  # Board lines
        mm.fill_rect(img, x, 22, 1, 22, mm.SCARF_SH)  # One board line
    ls.fill_poly(img, [(4, 24), (28, 6), (52, 24)], mm.STRAP)  # Dark roof
    mm.fill_rect(img, 6, 23, 44, 1, mm.MASK)  # White roof trim
    mm.fill_rect(img, 20, 30, 16, 14, mm.OUTLINE)  # Big door
    line(img, 20, 30, 35, 43, mm.MASK)  # Door cross brace
    line(img, 35, 30, 20, 43, mm.MASK)  # Door cross brace
    ell(img, 6, 42, 4, 3, HAY, HAY_SH)  # Hay bale on the left
    ell(img, 50, 42, 4, 3, HAY, HAY_SH)  # Hay bale on the right
    mm.fill_rect(img, 0, 44, 56, 1, mm.HILLS)  # Grass top of the step
    mm.fill_rect(img, 0, 45, 56, 3, mm.GROUND_SH)  # Earth under the grass
    return la.outline(img)  # Dark edge


def kopje():
    """A pile of rounded granite boulders with a small lookout sign on a post (the Serengeti goal)."""
    img = mm.new_image(56, 48)  # Empty picture

    def granite(x, y, shaded):  # Colour of one granite pixel
        if lm.noise(x, y) < 6:  # A few dark specks
            return GRANITE_SPECK  # Speck
        return GRANITE_SH if shaded else GRANITE  # Pink-grey stone
    mm.paste(img, na.blob(36, 30, 18, 15, 17, 14, granite), 8, 14)  # Big rounded boulder
    mm.paste(img, na.blob(18, 16, 9, 8, 8, 7, granite), 0, 30)  # Small boulder on the left
    mm.paste(img, na.blob(18, 14, 9, 7, 8, 6, granite), 34, 32)  # Small boulder on the right
    mm.fill_rect(img, 46, 20, 2, 26, POST)  # Sign post
    mm.fill_rect(img, 40, 10, 14, 10, mm.HEADLAMP)  # Sign board
    mm.fill_rect(img, 40, 10, 14, 1, mm.HELMET)  # Sign top line
    ell(img, 47, 15, 4, 2, mm.WHITE)  # Eye on the sign
    la.put(img, 47, 15, mm.OUTLINE)  # Pupil
    mm.fill_rect(img, 0, 46, 56, 2, mm.GROUND_SH)  # Ground under the rocks
    return la.outline(img)  # Dark edge


def estancia_gate():
    """A wooden ranch gate with an arch and a sign board, between two posts and two rails (the Pampas goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    ls.fill_poly(img, [(4, 16), (8, 10), (20, 6), (36, 6), (48, 10), (52, 16), (48, 16), (44, 11), (36, 9), (20, 9), (12, 11), (8, 16)], WOOD)  # Arch beam
    mm.fill_rect(img, 6, 14, 5, 32, POST)  # Left post
    mm.fill_rect(img, 45, 14, 5, 32, POST)  # Right post
    mm.fill_rect(img, 20, 10, 16, 6, mm.HEADLAMP)  # Sign board in the arch
    mm.fill_rect(img, 22, 12, 12, 2, mm.SCARF)  # Red brand on the sign
    mm.fill_rect(img, 11, 26, 34, 2, WOOD)  # Top rail
    mm.fill_rect(img, 11, 36, 34, 2, WOOD)  # Bottom rail
    mm.fill_rect(img, 0, 44, 56, 1, mm.PLAINS)  # Grass top
    mm.fill_rect(img, 0, 45, 56, 3, mm.GROUND_SH)  # Earth under the grass
    return la.outline(img)  # Dark edge


def winter_hut():
    """A log hut with a dark roof and a snowy cap, a door and windows, on snow (the Siberia goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    mm.fill_rect(img, 10, 24, 36, 20, WOOD)  # Log walls
    for y in range(26, 44, 3):  # Log lines
        mm.fill_rect(img, 10, y, 36, 1, WOOD_DK)  # One log line
    ls.fill_poly(img, [(6, 25), (28, 8), (50, 25)], lm.MOUNTAIN_SH)  # Dark roof boards
    ls.fill_poly(img, [(8, 25), (28, 9), (48, 25), (48, 21), (28, 5), (8, 21)], SNOW)  # Snowy cap on the roof
    mm.fill_rect(img, 23, 32, 10, 12, mm.OUTLINE)  # Door
    mm.fill_rect(img, 14, 28, 5, 4, mm.SKY_LIGHT)  # Left window
    mm.fill_rect(img, 37, 28, 5, 4, mm.SKY_LIGHT)  # Right window
    mm.fill_rect(img, 0, 44, 56, 2, SNOW)  # Snow on the ground
    mm.fill_rect(img, 0, 46, 56, 2, SNOW_SH)  # Shaded snow at the bottom
    return la.outline(img)  # Dark edge


def nullarbor_cave():
    """A dark cave mouth in a low cream limestone outcrop (the Nullarbor goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    ls.fill_poly(img, [(0, 47), (4, 30), (16, 20), (40, 20), (52, 30), (55, 47)], mm.HEADLAMP)  # Outcrop in the light
    ls.fill_poly(img, [(30, 20), (40, 20), (52, 30), (55, 47), (30, 47)], mm.VEST)  # Outcrop in shade
    line(img, 8, 36, 12, 44, mm.VEST_SH)  # Crack in the limestone
    line(img, 44, 30, 47, 38, mm.VEST_SH)  # Second crack
    ell(img, 28, 34, 8, 6, mm.OUTLINE)  # Round top of the cave mouth
    mm.fill_rect(img, 20, 34, 16, 13, mm.OUTLINE)  # Cave mouth down to the ground
    ell(img, 28, 44, 5, 3, mm.STRAP)  # Lighter back of the cave
    return la.outline(img)  # Dark edge


def stilt_house():
    """A wooden house on stilts over the water, with a pitched roof and a door (the Ganges goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    mm.fill_rect(img, 0, 40, 56, 8, lm.LENS)  # Water
    for x in (4, 20, 40):  # Ripples on the water
        mm.fill_rect(img, x, 43, 8, 1, mm.SKY_LIGHT)  # One ripple
    for x in (10, 22, 34, 44):  # Four stilts going into the water
        mm.fill_rect(img, x, 31, 3, 11, WOOD_DK)  # One stilt
    mm.fill_rect(img, 6, 28, 44, 3, WOOD_LT)  # Floor on the stilts
    mm.fill_rect(img, 10, 16, 36, 12, WOOD)  # House walls
    for y in (19, 23):  # Plank lines
        mm.fill_rect(img, 10, y, 36, 1, WOOD_DK)  # One plank line
    ls.fill_poly(img, [(4, 18), (28, 4), (52, 18)], mm.PLATEAU)  # Roof in the light
    ls.fill_poly(img, [(28, 4), (52, 18), (28, 18)], mm.PLATEAU_SH)  # Roof in shade
    mm.fill_rect(img, 22, 20, 8, 8, mm.OUTLINE)  # Door
    mm.fill_rect(img, 13, 19, 5, 4, mm.SKY_LIGHT)  # Window
    return la.outline(img)  # Dark edge


def ger():
    """A round white felt tent with a dark roof opening, a red and green band and a wooden door (the Steppe goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    ell(img, 28, 36, 22, 12, mm.WHITE, mm.MASK)  # Round felt wall and roof
    ell(img, 28, 25, 5, 2, mm.STRAP)  # Dark roof opening
    mm.fill_rect(img, 8, 33, 40, 1, mm.SCARF)  # Red band
    mm.fill_rect(img, 8, 35, 40, 1, mm.HILLS)  # Green band
    for x in range(10, 46, 5):  # Dots on the band
        la.put(img, x, 31, mm.SCARF)  # One red dot
    ell(img, 28, 40, 6, 3, WOOD)  # Wooden door arch
    mm.fill_rect(img, 24, 38, 8, 9, mm.OUTLINE)  # Door opening
    mm.fill_rect(img, 0, 45, 56, 1, mm.HILLS)  # Grass top
    mm.fill_rect(img, 0, 46, 56, 2, mm.GROUND_SH)  # Earth at the bottom
    return la.outline(img)  # Dark edge


# ---------- Hay (16x16 rolling bale, 24x4 straw dust) ----------

def hay_bale(size, frame, radius):
    """A round hay bale whose spiral turns left by one eighth each frame, with a dark edge."""
    img = mm.new_image(size, size)  # Empty picture
    c = (size - 1) / 2.0  # Middle of the bale
    a = -frame * math.pi / 4  # Turns left by one eighth each frame
    for y in range(size):  # Each row
        for x in range(size):  # Each pixel of the row
            dx, dy = x - c, y - c  # Offset from the middle
            d = math.hypot(dx, dy)  # Distance from the middle
            if d > radius:  # Outside the bale
                continue  # Leave it see-through
            if d > radius - 1.5:  # The dark rim
                col = HAY_RIM  # Rim colour
            else:  # The straw inside
                theta = math.atan2(dy, dx) + a  # Angle, turned by the frame
                col = HAY if int(math.floor(theta * 2 / math.pi + d * 0.9)) % 2 else HAY_SH  # Spiral stripe
                if lm.noise(x, y) < 10:  # A few bright straws
                    col = HAY_LT  # Bright straw
            la.put(img, x, y, col)  # Paint the pixel
    return la.outline(img)  # Dark edge


def hay_warning(frame):
    """A 24x4 small cloud of straw dust and bits, drifting a little in frame one."""
    img = mm.new_image(24, 4)  # Matches the log warning size
    for x, y, c, w in ((2, 0, HAY, 1), (6, 2, HAY_SH, 3), (11, 1, HAY_LT, 1), (15, 3, HAY_SH, 1), (18, 0, WOOD, 2), (21, 2, HAY, 1)):  # Straw bits
        mm.fill_rect(img, (x + frame * 2) % 23, (y + frame) % 4, w, 1, c)  # Drops each bit a little in frame one
    return img  # Leaves the bits unoutlined


# ---------- Herds (32x16, facing left, feet on the bottom row) ----------

def legs(img, frame, color, hoof):
    """Paints the four legs of one walk frame, each ending in a hoof on the bottom row."""
    for x, bottom in LEG_FRAMES[frame]:  # Each leg
        mm.fill_rect(img, x, 11, 2, bottom - 10, color)  # The leg from the body down
        mm.fill_rect(img, x, bottom, 2, 1, hoof)  # The hoof on its bottom row


def wildebeest(frame):
    """A dark grey-brown wildebeest, head down and grazing, with a beard and curved horns."""
    img = mm.new_image(32, 16)  # Empty picture
    ell(img, 16, 7, 11, 4, WILDE, WILDE_SH)  # Body
    legs(img, frame, WILDE_SH, mm.OUTLINE)  # Legs and hooves
    mm.fill_rect(img, 8, 5, 4, 5, WILDE)  # Neck
    ell(img, 6, 9, 4, 4, WILDE, WILDE_SH)  # Head low, grazing
    mm.fill_rect(img, 2, 12, 2, 2, mm.STRAP)  # Beard
    line(img, 6, 5, 2, 3, mm.VEST)  # Curved horn
    line(img, 7, 4, 10, 2, mm.VEST_SH)  # Horn behind the head
    la.put(img, 4, 7, mm.OUTLINE)  # Eye
    line(img, 27, 6, 30, 11, mm.STRAP)  # Tail
    return la.outline(img)  # Dark edge


def cattle(frame):
    """A brown and white cow, head up, with brown patches and short horns."""
    img = mm.new_image(32, 16)  # Empty picture
    ell(img, 16, 7, 11, 4, CARA_WHITE, mm.MASK)  # Body
    ell(img, 12, 6, 3, 2, CARA_SPOT, WOOD_DK)  # Brown patch on the back
    ell(img, 22, 5, 2, 2, CARA_SPOT)  # Second patch
    legs(img, frame, mm.MASK_SH, mm.STRAP)  # Legs and hooves
    ell(img, 5, 6, 3, 3, CARA_WHITE, mm.MASK)  # Head
    ell(img, 3, 4, 1, 1, CARA_SPOT)  # Brown patch on the head
    la.put(img, 5, 2, mm.HEADLAMP)  # Horn
    mm.fill_rect(img, 2, 8, 2, 2, mm.PLATEAU)  # Pink muzzle
    la.put(img, 5, 5, mm.OUTLINE)  # Eye
    line(img, 27, 6, 29, 12, WOOD_DK)  # Tail
    return la.outline(img)  # Dark edge


def horse(frame):
    """A chestnut Mongolian horse with a dark mane, a short neck and short legs."""
    img = mm.new_image(32, 16)  # Empty picture
    ell(img, 16, 7, 11, 4, CHESTNUT, CHESTNUT_SH)  # Body
    legs(img, frame, CHESTNUT_SH, mm.STRAP)  # Short legs and hooves
    mm.fill_rect(img, 5, 3, 4, 6, CHESTNUT)  # Neck
    ell(img, 4, 4, 3, 2, CHESTNUT, CHESTNUT_SH)  # Head
    line(img, 8, 2, 5, 4, CHESTNUT_SH)  # Dark mane
    mm.fill_rect(img, 1, 5, 2, 1, CHESTNUT_SH)  # Muzzle
    la.put(img, 3, 3, mm.OUTLINE)  # Eye
    line(img, 26, 6, 30, 12, CHESTNUT_SH)  # Dark tail
    return la.outline(img)  # Dark edge


# ---------- Frost, props and items ----------

def fence_post():
    """A 6x20 wooden fence post with a lit top (decoration only)."""
    img = mm.new_image(6, 20)  # Empty picture
    mm.fill_rect(img, 1, 0, 4, 20, POST)  # The post
    mm.fill_rect(img, 1, 0, 1, 20, WOOD_LT)  # Lit left side
    mm.fill_rect(img, 4, 0, 1, 20, WOOD_DK)  # Shaded right side
    return la.outline(img)  # Dark edge


def hay_roll_prop():
    """A 16x16 hay roll lying on the grass (decoration only)."""
    return hay_bale(16, 0, 7.3)  # The same round bale as the rolling hay


def windpump():
    """A 28x48 farm windmill on a lattice tower, with a wheel of spokes and a tail vane (decoration only)."""
    img = mm.new_image(28, 48)  # Empty picture
    line(img, 2, 47, 12, 20, WOOD_DK)  # Left leg of the tower
    line(img, 25, 47, 15, 20, WOOD_DK)  # Right leg of the tower
    line(img, 13, 47, 14, 20, WOOD_DK)  # Middle leg of the tower
    line(img, 4, 36, 22, 36, WOOD_DK)  # Brace
    line(img, 6, 44, 21, 44, WOOD_DK)  # Lower brace
    mm.fill_rect(img, 11, 17, 6, 4, mm.MASK)  # Top box under the wheel
    ring(img, 14, 13, 10, mm.MASK)  # Wheel ring
    for k in range(8):  # Eight spokes
        a = k * math.pi / 4.0  # Angle of this spoke
        line(img, 14, 13, round(14 + 10 * math.cos(a)), round(13 + 10 * math.sin(a)), mm.MASK)  # One spoke
    la.put(img, 14, 13, mm.HEADLAMP)  # Hub
    line(img, 14, 13, 26, 4, mm.SCARF)  # Tail vane
    return la.outline(img)  # Dark edge


def acacia_tree():
    """A 48x48 savanna acacia with a flat top and a leaning trunk (decoration only)."""
    img = mm.new_image(48, 48)  # Empty picture
    mm.fill_rect(img, 22, 26, 4, 22, mm.GROUND_SH)  # Trunk
    mm.fill_rect(img, 23, 26, 2, 22, WOOD)  # Lit trunk
    line(img, 24, 30, 12, 22, WOOD_DK)  # Left branch
    line(img, 25, 30, 36, 22, WOOD_DK)  # Right branch
    ls.fill_poly(img, [(0, 20), (6, 11), (42, 11), (48, 20), (40, 24), (8, 24)], mm.HILLS)  # Flat canopy
    ls.fill_poly(img, [(8, 24), (40, 24), (48, 20), (46, 22), (38, 27), (10, 27)], mm.HILLS_SH)  # Shade under the canopy
    mm.fill_rect(img, 10, 13, 6, 1, mm.PLAINS)  # Sunny leaf
    mm.fill_rect(img, 30, 14, 8, 1, mm.PLAINS)  # Sunny leaf
    return la.outline(img)  # Dark edge


def zebra(frame):
    """A 24x16 zebra with black stripes, a dark mane and striped legs; the legs swap in frame one (decoration only)."""
    img = mm.new_image(24, 16)  # Empty picture
    ell(img, 14, 7, 9, 4, mm.WHITE, mm.MASK)  # Body
    for x in range(6, 22, 3):  # Body stripes
        mm.fill_rect(img, x, 4, 1, 7, mm.STRAP)  # One stripe
    mm.fill_rect(img, 4, 2, 3, 4, mm.STRAP)  # Dark mane
    ell(img, 4, 6, 3, 3, mm.WHITE, mm.MASK)  # Head
    mm.fill_rect(img, 2, 4, 1, 4, mm.STRAP)  # Face stripe
    la.put(img, 3, 5, mm.OUTLINE)  # Eye
    for i, x in enumerate((6, 9, 17, 20)):  # Four legs
        bottom = 15 if (i + frame) % 2 == 0 else 14  # Legs swap between frames
        mm.fill_rect(img, x, 11, 2, bottom - 10, mm.STRAP if i % 2 else mm.WHITE)  # Striped leg
        mm.fill_rect(img, x, bottom, 2, 1, mm.OUTLINE)  # Hoof
    line(img, 23, 6, 22, 11, mm.STRAP)  # Tail
    return la.outline(img)  # Dark edge


def pampas_grass():
    """A 24x40 tall grass with feathery cream plumes on thin stems (decoration only)."""
    img = mm.new_image(24, 40)  # Empty picture
    for tx, ty in ((4, 6), (8, 2), (14, 1), (19, 5), (21, 12)):  # Five stems and their plumes
        line(img, 12, 39, tx, ty + 3, mm.HILLS_SH)  # Stem
        ell(img, tx, ty, 2, 3, mm.HEADLAMP, mm.VEST)  # Feathery plume
    for x in range(8, 18, 2):  # Leaves at the bottom
        line(img, 12, 39, x, 30 + (x % 3), mm.PLAINS)  # One leaf
    return img  # No outline, like the other living props


def birch_tree():
    """A 24x48 birch with a white trunk with dark marks and a round green crown (decoration only)."""
    img = mm.new_image(24, 48)  # Empty picture
    mm.fill_rect(img, 10, 18, 4, 30, mm.WHITE)  # White trunk
    for y in range(18, 48, 5):  # Dark marks on the bark
        la.put(img, 11, y, mm.STRAP)  # One mark
        la.put(img, 12, y + 1, mm.STRAP)  # Mark continues
    line(img, 11, 24, 5, 16, mm.WHITE)  # Left branch
    line(img, 12, 28, 19, 19, mm.WHITE)  # Right branch
    ell(img, 12, 11, 10, 9, mm.HILLS, mm.HILLS_SH)  # Round crown
    la.put(img, 7, 6, mm.PLAINS)  # Sunny leaf
    la.put(img, 16, 4, mm.PLAINS)  # Sunny leaf
    return img  # No outline, like the other living props


def snow_tuft():
    """A 12x8 tuft of dry grass with snow on its foot (decoration only)."""
    img = mm.new_image(12, 8)  # Empty picture
    line(img, 3, 7, 2, 2, mm.VEST_SH)  # Blade
    line(img, 6, 7, 7, 1, mm.VEST_SH)  # Tall blade
    line(img, 9, 7, 10, 3, mm.VEST_SH)  # Blade
    ell(img, 6, 7, 6, 3, SNOW, SNOW_SH)  # Snow mound
    return img  # No outline


def saltbush():
    """A 16x8 low grey-green shrub with small pale leaves (decoration only)."""
    img = mm.new_image(16, 8)  # Empty picture
    ell(img, 8, 5, 7, 4, GREY_GREEN, mix(GREY_GREEN, mm.OUTLINE))  # Round shrub
    for x, y in ((4, 3), (8, 2), (11, 4), (6, 5)):  # Pale leaves
        la.put(img, x, y, mix(GREY_GREEN, mm.MASK))  # One pale leaf
    return img  # No outline


def wheat_sheaf():
    """A 10x14 tied bundle of golden wheat with grain heads (decoration only)."""
    img = mm.new_image(10, 14)  # Empty picture
    for i in range(5):  # Five stalks
        line(img, 5, 13, 2 + i * 2, 1 + (i % 2), mm.HELMET_SH)  # One stalk
        ell(img, 2 + i * 2, 1 + (i % 2), 1, 1, mm.HELMET)  # Grain head
    mm.fill_rect(img, 2, 9, 6, 1, WOOD)  # Tie
    mm.fill_rect(img, 2, 10, 6, 1, WOOD_DK)  # Shaded tie
    return img  # No outline


def steppe_grass():
    """A 12x10 clump of short olive-green steppe grass (decoration only)."""
    img = mm.new_image(12, 10)  # Empty picture
    for x, tip in ((1, 3), (3, 1), (6, 2), (8, 4), (10, 5)):  # Five blades: column and tip row
        line(img, x, 9, x + 1, tip + 2, OLIVE)  # One blade
        la.put(img, x + 1, tip + 2, OLIVE_LT)  # Sunny tip
    return img  # No outline


def chalk():
    """A 24x24 soft white chalk lump with a faint grey speck."""
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x, y) < 8:  # A faint speck
            return mm.VEST_SH  # Speck
        return mix(mm.MASK, mm.WHITE) if s else mm.WHITE  # Soft white
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 2.6))  # Soft lump


def loess():
    """A 24x24 pale yellow-tan powdery lump with a few darker specks."""
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x, y) < 10:  # A darker speck
            return mm.VEST_SH  # Speck
        return mix(mm.VEST, mm.HELMET_SH) if s else mix(mm.HEADLAMP, mm.VEST)  # Powdery tan
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 2.6))  # Powdery lump


def flint():
    """A 24x24 dark grey glassy nodule with a white crust round its edge."""
    def c(x, y, s):  # Colour of one pixel
        d = ((x - 11.5) / 10.0) ** 2 + ((y - 11.5) / 8.0) ** 2  # How far out from the middle
        if d > 0.6:  # The white crust near the edge
            return mm.MASK if s else mm.WHITE  # Crust
        if lm.noise(x, y) < 12:  # A glassy shine
            return mm.SKY_LIGHT  # Shine
        return mm.OUTLINE if s else mm.STRAP  # Dark grey glassy stone
    return ls.outline(na.blob(24, 24, 11.5, 11.5, 10.0, 8.0, c, 2.0))  # Nodule


# ---------- Lesson pictures (120x82) ----------

def gaucho(img, x, y):
    """Paints a tiny gaucho with a hat, a red poncho and dark legs, standing on a horse (drawn on the lesson picture)."""
    mm.fill_rect(img, x - 1, y + 1, 8, 1, mm.STRAP)  # Hat brim
    mm.fill_rect(img, x + 1, y - 1, 5, 2, mm.STRAP)  # Hat crown
    ell(img, x + 3, y + 4, 2, 2, mix(mm.PLATEAU, mm.HEADLAMP))  # Face
    ls.fill_poly(img, [(x + 1, y + 6), (x + 6, y + 6), (x + 7, y + 11), (x, y + 11)], mm.SCARF)  # Poncho
    mm.fill_rect(img, x + 1, y + 11, 2, 3, mm.STRAP)  # Left leg
    mm.fill_rect(img, x + 5, y + 11, 2, 3, mm.STRAP)  # Right leg


def chalk_pillar(x, y, cx, half_w, top, base):
    """Colour of one pixel of a tall chalk pillar with a rounded top and layer lines, or None outside it."""
    if y < top or y > base:  # Above the top or below the base
        return None  # Not part of the pillar
    w = half_w  # Half-width on this row
    if y < top + 4:  # The rounded top
        dy = (top + 4 - y) / 4.0  # How far up into the round top
        w = half_w * math.sqrt(max(0.0, 1 - dy * dy))  # Narrower toward the top
    if abs(x - cx) > w:  # Outside the rounded column
        return None  # Not part of the pillar
    if y % 6 == 0:  # A horizontal layer line
        return mm.VEST  # Layer colour
    return mm.HEADLAMP if x < cx else mm.VEST_SH  # Lit left side, shaded right side


def great_plains_pixel(x, y):
    """Colour of one pixel of the Great Plains picture: sky, wide green grass and two cream-white chalk pillars on the grass."""
    if y < 22:  # Sky
        return na.sky(x, y)  # Sky colour
    for cx, hw, top in ((90, 6, 26), (106, 5, 34)):  # Two chalk pillars: middle x, half width, top row
        p = chalk_pillar(x, y, cx, hw, top, 50)  # This pixel of the pillar
        if p is not None:  # Part of the pillar
            return p  # Paint the pillar
    if y < 50:  # Grass behind the cattle
        return mm.PLAINS if (y // 6) % 2 else mix(mm.PLAINS, mm.HELMET)  # Golden-green grass bands
    return mm.HILLS if (y // 6) % 2 else mm.PLAINS  # Grass in the front


def lesson_great_plains():
    """A 120x82 framed picture of wide grassland with cattle and white chalk bluffs."""
    img = ls.framed(great_plains_pixel)  # Sky, grass and bluffs
    for x, y, f in ((26, 60, 0), (46, 64, 1), (92, 58, 0)):  # Three cattle
        mm.paste(img, cattle(f), x, y)  # One cow
    return img


def serengeti_pixel(x, y):
    """Colour of one pixel of the Serengeti picture: sky, dry yellow savanna and bare red-brown patches."""
    if y < 22:  # Sky
        return na.sky(x, y)  # Sky colour
    if y < 52:  # Dry grass on the plain
        return mix(mm.HELMET, mm.PLAINS) if (x // 7 + y // 3) % 2 else mm.HELMET  # Dry yellow grass
    if (x * 5 + y) % 37 < 3:  # Bare patches
        return mm.PLATEAU_SH  # Red-brown earth
    return mm.HELMET_SH if (y // 5) % 2 else mm.HELMET  # Grass in the front


def lesson_serengeti():
    """A 120x82 framed picture of a savanna with a kopje, an acacia and a herd of wildebeest."""
    img = ls.framed(serengeti_pixel)  # Sky and savanna
    mm.paste(img, kopje(), 2, 30)  # Granite kopje on the left
    mm.paste(img, acacia_tree(), 62, 22)  # Flat acacia tree
    for x, y, f in ((68, 60, 0), (86, 62, 1), (102, 58, 2)):  # A herd of three wildebeest
        mm.paste(img, wildebeest(f), x, y)  # One wildebeest
    return img


def pampas_pixel(x, y):
    """Colour of one pixel of the Pampas picture: sky and rolling green grassland in bands."""
    if y < 22:  # Sky
        return na.sky(x, y)  # Sky colour
    if y < 50:  # Green grass behind
        return mm.HILLS if (y // 6) % 2 else mm.PLAINS  # Green bands
    return mm.HILLS_SH if (y // 6) % 2 else mm.HILLS  # Grass in the front


def lesson_pampas():
    """A 120x82 framed picture of green grassland with a gaucho on a horse and cattle."""
    img = ls.framed(pampas_pixel)  # Sky and grass
    mm.paste(img, horse(0), 30, 60)  # The horse
    gaucho(img, 44, 52)  # The gaucho on its back
    mm.paste(img, cattle(1), 72, 62)  # A cow
    mm.paste(img, cattle(0), 92, 64)  # A second cow
    return img


def siberia_pixel(x, y):
    """Colour of one pixel of the West Siberian picture: sky, a flat snowy plain and a dark bog."""
    if y < 24:  # Sky
        return na.sky(x, y)  # Sky colour
    if ((x - 30) / 24.0) ** 2 + ((y - 72) / 7.0) ** 2 <= 1.0:  # Dark bog pool in the front
        return mix(mm.BACKPACK_SH, mm.OUTLINE) if (x + y) % 5 else mix(mm.GROUND_SH, mm.OUTLINE)  # Peat
    if lm.noise(x, y) < 5:  # Sparse frost flecks
        return lm.HAZE  # Pale blue fleck
    return mix(SNOW, mm.SKY_LIGHT) if y < 40 else SNOW  # Solid pale snow, bluer far away


def lesson_siberia():
    """A 120x82 framed picture of a flat snowy plain with birch trees and a dark bog."""
    img = ls.framed(siberia_pixel)  # Sky, snow and bog
    mm.paste(img, birch_tree(), 16, 28)  # Birch on the left
    mm.paste(img, birch_tree(), 86, 30)  # Birch on the right
    mm.paste(img, snow_tuft(), 52, 60)  # Snowy tuft
    mm.paste(img, snow_tuft(), 66, 62)  # Second tuft
    return img


def nullarbor_pixel(x, y):
    """Colour of one pixel of the Nullarbor picture: sky, a flat treeless cream plain, a cliff edge with a cave and the sea."""
    if y < 24:  # Sky
        return na.sky(x, y)  # Sky colour
    if x >= 92 and y >= 46:  # The cliff edge on the right
        if ((x - 102) / 7.0) ** 2 + ((y - 60) / 7.0) ** 2 <= 1.0:  # The cave mouth
            return mm.OUTLINE  # Dark cave
        return mm.VEST_SH if (x + y) % 7 == 0 else mm.VEST  # Cliff face
    if y >= 70:  # Sand at the bottom
        return mm.VEST if x < 92 else lm.LENS  # Sand, then the sea beyond the cliff
    if (x * 7 + y * 3) % 23 == 0:  # Sparse grey-green scrub
        return GREY_GREEN  # Scrub bush
    return CREAM if (y // 6) % 2 else mix(CREAM, mm.WHITE)  # Flat cream plain in bands


def lesson_nullarbor():
    """A 120x82 framed picture of a flat treeless plain with a cliff edge, a cave and a few scrub bushes."""
    img = ls.framed(nullarbor_pixel)  # Plain, cliff and sea
    mm.paste(img, saltbush(), 14, 60)  # A saltbush
    mm.paste(img, saltbush(), 40, 66)  # A second saltbush
    mm.paste(img, saltbush(), 64, 56)  # A third saltbush
    return img


def ganges_pixel(x, y):
    """Colour of one pixel of the Ganges picture: sky, a wide river, rice and wheat fields and a village."""
    if y < 22:  # Sky
        return na.sky(x, y)  # Sky colour
    river = 54 + round(5 * math.sin(x / 14.0))  # Middle of the river on this column
    if abs(y - river) <= 4:  # The river
        return mm.SKY_LIGHT if lm.noise(x, y) < 10 else lm.LENS  # Water with shine
    if y < 50:  # Rice and wheat fields
        return mm.HILLS if (y // 6) % 2 else mm.HELMET  # Green rice and gold wheat in solid bands
    if 92 <= x <= 112 and 34 <= y <= 44:  # Village houses
        return mm.PLATEAU if y < 38 else mm.HEADLAMP  # Roof, then wall
    return mm.HILLS_SH if (y // 5) % 2 else mm.HILLS  # Grass on the near bank


def lesson_ganges():
    """A 120x82 framed picture of a wide river with rice and wheat fields and a village."""
    img = ls.framed(ganges_pixel)  # Sky, river, fields and village
    for x, y in ((6, 60), (20, 62), (34, 58)):  # Three wheat sheaves on the near bank
        mm.paste(img, wheat_sheaf(), x, y)  # One sheaf
    return img


def steppe_pixel(x, y):
    """Colour of one pixel of the Steppe picture: sky and rolling olive grassland in patches."""
    if y < 26:  # Sky
        return na.sky(x, y)  # Sky colour
    return OLIVE_LT if (y // 6) % 2 else OLIVE  # Olive grass in solid bands


def lesson_steppe():
    """A 120x82 framed picture of grassland with a round ger tent and horses."""
    img = ls.framed(steppe_pixel)  # Sky and grass
    mm.paste(img, ger(), 6, 30)  # The round tent
    mm.paste(img, horse(1), 74, 58)  # A horse
    mm.paste(img, horse(2), 90, 60)  # A second horse
    return img


# ---------- Saving and the preview sheet ----------

def put_file(kind, name, img):
    """Saves one picture in the folder for its kind (item, lesson or level sprite) and prints its size."""
    if kind == "item":  # Shared item picture
        ls.save(img, ls.ITEMS_OUT, name)  # Items folder
    elif kind == "lesson":  # Lesson picture
        ls.save(img, ls.LESSON_OUT, name)  # Lesson folder
    else:  # Level sprite or prop, Props/ names included
        la.save(img, name)  # Level sprites folder
    print(name, f"{img.width}x{img.height}")  # Tell which file was written and its size


def label_text(name):
    """Turns a file name into the upper-case text the pixel font can draw."""
    base = name.split("/")[-1].replace(".png", "")  # Name without the folder and extension
    return base.replace("_", " ").upper()  # Upper case words with spaces


def preview(groups):
    """Lays the new sprites out at 3x size on a grey grid, one labelled group after another."""
    scale, pad, width = 3, 8, 960  # Size up, gap and sheet width
    ops, cells = [], []  # Pictures to paste and grey grid cells to draw
    y = pad  # Where the next row goes
    for title, sprites in groups:  # Each group
        head = big(mm.text_image(title, mm.DARK, 1), 2)  # Group title
        ops.append((head, pad, y))  # Title at the top of the group
        y += head.height + pad  # Move below the title
        x, row_h = pad, 0  # Start a row
        for name, img in sprites:  # Each sprite in the group
            label = mm.text_image(label_text(name), mm.DARK, 1)  # Small name label
            cell_w = max(img.width * scale, label.width) + pad  # Grid cell width
            cell_h = img.height * scale + label.height + pad + 4  # Grid cell height
            if x + cell_w > width - pad:  # No room left in this row
                x, y, row_h = pad, y + row_h + pad, 0  # Start a new row
            cells.append((x, y, cell_w, cell_h))  # Grey cell outline
            ops.append((label, x + pad // 2, y + pad // 2))  # Label at the top of the cell
            ops.append((big(img, scale), x + pad // 2, y + pad // 2 + label.height + 4))  # Sprite under the label
            x, row_h = x + cell_w + pad, max(row_h, cell_h)  # Move along
        y += row_h + 2 * pad  # Space before the next group
    sheet = Image.new("RGBA", (width, y + pad), (226, 226, 226, 255))  # Light grey background
    draw = ImageDraw.Draw(sheet)  # Pen for the grid
    for x, y0, w, h in cells:  # Each grid cell
        draw.rectangle([x, y0, x + w - 1, y0 + h - 1], outline=(150, 150, 150, 255))  # Grey grid line
    for img, px, py in ops:  # Paste each label and sprite
        sheet.alpha_composite(img, (px, py))  # Place it
    os.makedirs(os.path.dirname(PREVIEW), exist_ok=True)  # Make the folder
    sheet.save(PREVIEW)  # Write the sheet
    print("Preview written to", PREVIEW)  # Tell where


def theme_group(title, pieces):
    """Groups one theme's five pieces, as (kind, name, picture) entries."""
    return (title, [("level", name, img) for name, img in pieces])  # Level sprites


def main():
    """Draws everything, saves each picture and writes the preview sheet."""
    groups = [  # Each group: its title, then (kind, file name, picture) for each picture
        theme_group("PRAIRIE", prairie_tiles()),  # Golden-green tall grass over loam
        theme_group("SAVANNA", savanna_tiles()),  # Dry yellow grass over red-brown earth
        theme_group("PAMPAS", pampas_tiles()),  # Lush green grass over dark soil
        theme_group("SIBERIA", siberia_tiles()),  # Snowy grass over frozen peat
        theme_group("NULLARBOR", nullarbor_tiles()),  # Grey-green scrub over cream limestone
        theme_group("GANGES", ganges_tiles()),  # Green paddy edge over alluvial mud
        theme_group("STEPPE", steppe_tiles()),  # Olive grass over tan earth
        ("FROZEN GROUND", [("level", "frost_top.png", frost_top())]),  # Frost tile
        ("GOALS", [
            ("level", "hay_barn.png", hay_barn()),  # Prairie goal
            ("level", "kopje.png", kopje()),  # Serengeti goal
            ("level", "estancia_gate.png", estancia_gate()),  # Pampas goal
            ("level", "winter_hut.png", winter_hut()),  # Siberia goal
            ("level", "nullarbor_cave.png", nullarbor_cave()),  # Nullarbor goal
            ("level", "stilt_house.png", stilt_house()),  # Ganges goal
            ("level", "ger.png", ger()),  # Steppe goal
        ]),
        ("HAY", [("level", f"hay_{f:02d}.png", hay_bale(16, f, 7.3)) for f in range(4)] + [  # Four rolling frames
            ("level", "hay_warn_00.png", hay_warning(0)),  # Straw dust, frame 0
            ("level", "hay_warn_01.png", hay_warning(1)),  # Straw dust, frame 1
        ]),
        ("HERDS", [("level", f"wildebeest_{f:02d}.png", wildebeest(f)) for f in range(3)]  # Wildebeest walk
                  + [("level", f"cattle_{f:02d}.png", cattle(f)) for f in range(3)]  # Cattle walk
                  + [("level", f"horse_{f:02d}.png", horse(f)) for f in range(3)]),  # Horse walk
        ("PLAINS HARD PROPS", [
            ("level", "Props/fence_post.png", fence_post()),  # Wooden fence post
            ("level", "Props/hay_roll_prop.png", hay_roll_prop()),  # Hay roll on the grass
            ("level", "Props/windpump.png", windpump()),  # Farm windmill
            ("level", "Props/acacia_tree.png", acacia_tree()),  # Flat-top acacia
            ("level", "Props/zebra_00.png", zebra(0)),  # Zebra, frame 0
            ("level", "Props/zebra_01.png", zebra(1)),  # Zebra, frame 1
            ("level", "Props/pampas_grass.png", pampas_grass()),  # Tall feathery grass
            ("level", "Props/birch_tree.png", birch_tree()),  # White-trunk birch
            ("level", "Props/snow_tuft.png", snow_tuft()),  # Snowy grass tuft
            ("level", "Props/saltbush.png", saltbush()),  # Low grey-green shrub
            ("level", "Props/wheat_sheaf.png", wheat_sheaf()),  # Tied wheat
            ("level", "Props/steppe_grass.png", steppe_grass()),  # Short steppe grass
        ]),
        ("ITEMS", [
            ("item", "chalk.png", chalk()),  # Soft white lump
            ("item", "loess.png", loess()),  # Pale powdery lump
            ("item", "flint.png", flint()),  # Dark glassy nodule with a white crust
        ]),
        ("LESSON PICTURES", [
            ("lesson", "lesson_great_plains.png", lesson_great_plains()),  # Grassland with cattle and chalk
            ("lesson", "lesson_serengeti.png", lesson_serengeti()),  # Savanna with wildebeest
            ("lesson", "lesson_pampas.png", lesson_pampas()),  # Gaucho on a horse
            ("lesson", "lesson_siberia.png", lesson_siberia()),  # Snowy plain with birch and bog
            ("lesson", "lesson_nullarbor.png", lesson_nullarbor()),  # Treeless plain with a cliff and cave
            ("lesson", "lesson_ganges.png", lesson_ganges()),  # River with fields and a village
            ("lesson", "lesson_steppe.png", lesson_steppe()),  # Grassland with a ger and horses
        ]),
    ]
    preview_groups = []  # (title, (name, picture) pairs) for the preview sheet
    for title, entries in groups:  # Each group
        sprites = []  # Pictures of this group
        for kind, name, img in entries:  # Each picture
            put_file(kind, name, img)  # Write it to its folder
            sprites.append((name, img))  # Keep it for the preview
        preview_groups.append((title, sprites))  # Add the group to the sheet
    preview(preview_groups)  # The check sheet
    print("Plains Hard art written to", la.OUT, ls.ITEMS_OUT, "and", ls.LESSON_OUT)  # Tell where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
