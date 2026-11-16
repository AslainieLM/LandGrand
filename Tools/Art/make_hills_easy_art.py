"""Draws the PNGs for Hills Easy 1 to 3 (D-066): three level themes (Chocolate Hills, Rolling Pasture, Sand Dunes),
soft sand, the grazing animals, three new rocks and minerals, three lesson pictures and the Hills life sprites
for the Landform Map and the Hills menu screens.

Run from the Unity project folder:  python Tools/Art/make_hills_easy_art.py
It writes to Assets/Art/Level/Sprites/ (themes, soft sand and props), Assets/Art/Items/Sprites/ (items),
Assets/Art/Lesson/Sprites/ (lesson pictures) and Assets/Art/LandformMap/Sprites/Hills/ (life sprites),
plus a preview sheet at .scratch/hills/art-preview.png.
It reuses the helpers of make_level_art.py, make_lesson_art.py and make_normal_art.py.
All colours come from Assets/Art/Palettes/landgrand.gpl or mix two of its colours.
"""
import math  # math gives circles and waves
import os  # os joins folder paths
from PIL import Image  # Image builds the preview sheet
import make_mainmenu_art as mm  # Palette, new_image and fill_rect
import make_landform_map_art as lm  # Map colours and the fixed noise
import make_level_art as la  # put, outline, boulder_colored and save for level sprites
import make_lesson_art as ls  # framed, disc, fill_poly and the item and lesson folders
import make_normal_art as na  # mix, the shared tile makers and the item blob

mix = na.mix  # The colour halfway between two palette colours
MAP_OUT = os.path.join(mm.ROOT, "Assets", "Art", "LandformMap", "Sprites", "Hills")  # Where the Hills life sprites go
PREVIEW = os.path.join(mm.ROOT, ".scratch", "hills", "art-preview.png")  # Where the preview sheet goes

# Theme colours (D-066), each a palette colour or a mix of two.
CHOC = mix(mm.GROUND, mm.HILLS_SH)  # Brown-green grass of the Chocolate Hills
CHOC_DRY = mix(mm.GROUND, mm.VEST_SH)  # Dry brown grass in the dry season
CHOC_LT = mix(mm.VEST, mm.HILLS)  # Sunny grass tips
LIME = mix(mm.WHITE, mm.VEST)  # Pale cream limestone
LIME_SH = mix(mm.MASK, mm.VEST)  # Limestone in shade
LIME_DK = mix(mm.MASK_SH, mm.VEST_SH)  # Dark limestone specks
PASTURE = mm.HILLS  # Bright green Batanes grass
PASTURE_SH = mm.HILLS_SH  # Grass in shade
PASTURE_DK = mm.LEAF  # Deep grass shade
VOLC_SOIL = mix(mm.GROUND_SH, mm.STRAP)  # Dark volcanic soil of Batan Island
STONE = mix(mm.MASK, mm.WHITE)  # Pale coral and limestone house walls
STONE_SH = mm.MASK_SH  # Wall stones in shade
THATCH = mix(mm.VEST, mm.HELMET_SH)  # Cogon grass roof
THATCH_SH = mm.VEST_SH  # Roof in shade
SAND = mix(mm.VEST, mm.HEADLAMP)  # Pale dune sand
SAND_SH = mm.VEST  # Sand in shade
SAND_DK = mm.VEST_SH  # Ripple shadows
SOFT = mix(mm.VEST, mm.VEST_SH)  # Loose soft sand, a little darker so it reads as different ground
SEA = lm.LENS  # Sea blue
SEA_DEEP = mix(lm.LENS, lm.MOUNTAIN_SH)  # Deep sea blue
WOOD = lm.HANDLE  # Light wood
WOOD_SH = mm.GROUND_SH  # Wood in shade


# ---------- Chocolate Hills (E1) ----------

def choc_top():
    """Brown-green grass over pale limestone soil."""
    return na.ground_top(LIME_SH, CHOC, CHOC_LT, CHOC_DRY,
                         [(2, 1, CHOC_DRY), (7, 2, CHOC_LT), (11, 1, CHOC_DRY), (14, 2, mm.HILLS_SH)])  # Grass tufts


def choc_soil():
    """Pale limestone soil with small shell bits."""
    return na.soil_tile(LIME_SH, LIME_DK, [(4, 5, mm.WHITE), (5, 5, mm.WHITE), (11, 11, mm.WHITE), (9, 3, LIME_DK)])  # Shell bits


def ledge_choc():
    """A slab of cream limestone with a grassy top."""
    return na.ledge_tile(LIME, CHOC, LIME_DK, [(4, 4, LIME_SH), (10, 5, LIME_DK), (13, 3, mm.WHITE)])  # Pores


def choc_boulder(w, h):
    """A cream limestone boulder with a grass cap."""
    return na.capped_boulder(w, h, LIME, LIME_SH, mm.WHITE, LIME_DK, CHOC, 2)  # Limestone with grass


def viewing_deck():
    """A 56x48 wooden stair up to a small roofed viewing deck (the E1 goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    for i in range(9):  # Nine steps going up to the right
        mm.fill_rect(img, 2 + i * 4, 44 - i * 3, 6, 2, WOOD)  # Step top
        mm.fill_rect(img, 2 + i * 4, 46 - i * 3, 6, 1, WOOD_SH)  # Step front
    mm.fill_rect(img, 36, 18, 18, 3, WOOD)  # Deck floor
    mm.fill_rect(img, 36, 21, 18, 1, WOOD_SH)  # Deck floor shade
    for x in (37, 52):  # Two posts
        mm.fill_rect(img, x, 8, 2, 27, WOOD_SH)  # One post
    mm.fill_rect(img, 37, 14, 17, 1, WOOD)  # Hand rail
    ls.fill_poly(img, [(33, 9), (45, 2), (56, 9)], mm.PLATEAU)  # Roof
    mm.fill_rect(img, 33, 8, 23, 2, mm.PLATEAU_SH)  # Roof edge
    return la.outline(img)  # Dark edge


def palm_tree():
    """A 24x40 coconut palm (decoration only)."""
    img = mm.new_image(24, 40)  # Empty picture
    for y in range(12, 40):  # Curved trunk
        mm.fill_rect(img, 11 + int(3 * math.sin(y / 9.0)), y, 2, 1, WOOD if y % 3 else WOOD_SH)  # One trunk row with rings
    for side in (-1, 1):  # Fronds to the left and to the right
        for (reach, lift) in ((11, 4), (9, 0), (6, -3)):  # Long low frond, middle frond, short high frond
            for t in range(reach):  # Pixels along the frond
                droop = (t * t) // (reach + 2) - lift * (reach - t) // reach  # Curves up, then droops at the tip
                x, y = 12 + side * t, 10 + droop  # Where this frond pixel is
                la.put(img, x, y, mm.LEAF)  # Frond rib
                la.put(img, x, y + 1, mm.HILLS_SH if t % 2 else mm.LEAF)  # Leaflets hanging under the rib
    ls.disc(img, 12, 10, 2, mm.HILLS_SH)  # Leafy crown in the middle
    ls.disc(img, 11, 13, 1, mm.GROUND_SH)  # Coconut
    ls.disc(img, 14, 13, 1, mm.GROUND_SH)  # Coconut
    return la.outline(img)  # Dark edge


def cogon_tuft():
    """A 12x8 tuft of cogon grass (decoration only)."""
    img = mm.new_image(12, 8)  # Empty picture
    for x in range(1, 11):  # Blades
        h = 3 + (lm.noise(x, 5) % 4)  # Blade height
        for y in range(8 - h, 8):  # Down the blade
            la.put(img, x, y, CHOC_LT if y < 8 - h + 2 else CHOC)  # Light tip, darker base
    return img


# ---------- Rolling Pasture: Batanes (E2) ----------

def pasture_top():
    """Short bright grass on dark volcanic soil."""
    return na.ground_top(VOLC_SOIL, PASTURE, mm.PLAINS, PASTURE_SH,
                         [(3, 2, PASTURE_SH), (8, 1, mm.PLAINS), (12, 2, PASTURE_DK), (6, 3, mm.HELMET)])  # Grass and a small flower


def pasture_soil():
    """Dark volcanic soil with roots."""
    return na.soil_tile(VOLC_SOIL, mm.STRAP, [(3, 1, mm.GROUND_SH), (3, 2, mm.GROUND_SH), (4, 3, mm.GROUND_SH), (11, 8, mm.MASK_SH)])  # Roots and a stone


def ledge_pasture():
    """A slab of grey andesite with a grass top."""
    return na.ledge_tile(mm.MASK_SH, PASTURE, mm.STRAP, [(4, 4, mm.MASK), (10, 5, mm.STRAP), (13, 3, mm.MASK)])  # Stone flecks


def pasture_boulder(w, h):
    """A grey andesite boulder with a grass cap."""
    return na.capped_boulder(w, h, mm.MASK_SH, mm.STRAP, mm.MASK, mm.STRAP, PASTURE, 2)  # Grey rock with grass


def stone_house():
    """A 56x48 Ivatan stone house: thick pale stone walls, a thick grass roof and a door in the middle (the E2 goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    mm.fill_rect(img, 6, 22, 44, 26, STONE)  # Walls
    for y in range(24, 48, 4):  # Rows of stones
        for x in range(6 + (y // 4 % 2) * 3, 50, 6):  # Stones in the row
            mm.fill_rect(img, x, y, 1, 3, STONE_SH)  # Joint between stones
        mm.fill_rect(img, 6, y + 3, 44, 1, STONE_SH)  # Row joint
    for y in range(4, 24):  # Thick roof rows
        half = 6 + (y - 4) * 1.25  # Wider going down
        for x in range(int(28 - half), int(28 + half)):  # Pixels of the row
            la.put(img, x, y, THATCH_SH if (x + y) % 4 == 0 else THATCH)  # Thatch with lines
    mm.fill_rect(img, 23, 32, 10, 16, mm.GROUND_SH)  # Wooden door frame
    mm.fill_rect(img, 24, 33, 8, 15, WOOD)  # Door boards
    mm.fill_rect(img, 11, 30, 6, 6, mm.STRAP)  # Small window, left
    mm.fill_rect(img, 39, 30, 6, 6, mm.STRAP)  # Small window, right
    return la.outline(img)  # Dark edge


def goat(frame):
    """A 16x12 white goat; frame 0 grazes with its head down, frame 1 lifts its head (decoration only)."""
    img = mm.new_image(16, 12)  # Empty picture
    mm.fill_rect(img, 4, 4, 9, 4, mm.WHITE)  # Body
    mm.fill_rect(img, 4, 7, 9, 1, mm.MASK)  # Belly shade
    for x in (5, 7, 10, 12):  # Four legs
        mm.fill_rect(img, x, 8, 1, 3, mm.MASK_SH)  # One leg
    mm.fill_rect(img, 13, 3, 1, 2, mm.MASK)  # Tail
    hy = 6 if frame == 0 else 1  # Head low to graze, or up
    mm.fill_rect(img, 1, hy, 3, 3, mm.WHITE)  # Head
    la.put(img, 2, hy - 1, mm.MASK_SH)  # Horn
    la.put(img, 1, hy + 1, mm.OUTLINE)  # Eye
    return la.outline(img)  # Dark edge


def cow(frame):
    """A 24x16 brown cow; frame 0 grazes, frame 1 lifts its head (decoration only)."""
    img = mm.new_image(24, 16)  # Empty picture
    mm.fill_rect(img, 6, 4, 15, 7, mm.GROUND)  # Body
    mm.fill_rect(img, 6, 9, 15, 2, mm.GROUND_SH)  # Belly shade
    mm.fill_rect(img, 12, 5, 4, 3, mm.HEADLAMP)  # Pale patch
    for x in (7, 10, 16, 19):  # Four legs
        mm.fill_rect(img, x, 11, 2, 4, mm.GROUND_SH)  # One leg
    mm.fill_rect(img, 21, 4, 1, 4, mm.GROUND_SH)  # Tail
    hy = 8 if frame == 0 else 2  # Head down to graze, or up
    mm.fill_rect(img, 1, hy, 5, 4, mm.GROUND)  # Head
    mm.fill_rect(img, 1, hy + 3, 2, 1, mm.PLATEAU)  # Nose
    la.put(img, 3, hy + 1, mm.OUTLINE)  # Eye
    la.put(img, 5, hy - 1, mm.MASK)  # Horn
    return la.outline(img)  # Dark edge


def stone_fence():
    """A 32x10 low stone fence of piled pale stones (decoration only)."""
    img = mm.new_image(32, 10)  # Empty picture
    for i, (x, y, r) in enumerate(((3, 7, 2), (9, 7, 2), (15, 7, 2), (21, 7, 2), (27, 7, 2), (6, 4, 2), (12, 4, 2), (18, 4, 2), (24, 4, 2))):  # Stones
        ls.disc(img, x, y, r, STONE if i % 2 else STONE_SH)  # One stone
    return la.outline(img)  # Dark edge


# ---------- Sand Dunes: La Paz, Ilocos Norte (E3) ----------

def dune_top():
    """Firm pale sand with small ripples."""
    return na.ground_top(SAND_SH, SAND, mm.HEADLAMP, SAND_DK,
                         [(2, 2, SAND_SH), (3, 2, SAND_SH), (9, 3, SAND_SH), (10, 3, SAND_SH), (14, 1, mm.STRAP)])  # Ripples and a black grain


def dune_soil():
    """Packed sand with black grains."""
    return na.soil_tile(SAND_SH, SAND_DK, [(5, 4, mm.STRAP), (12, 9, mm.STRAP), (8, 13, mm.STRAP), (2, 11, mm.HEADLAMP)])  # Black and bright grains


def ledge_dune():
    """A slab of tan sandstone with a sandy top."""
    return na.ledge_tile(mm.VEST, SAND, mm.VEST_SH, [(3, 4, mm.PLATEAU_SH), (9, 5, SAND_DK), (13, 4, mm.HEADLAMP)])  # Layers


def dune_boulder(w, h):
    """A tan sandstone boulder with a sand cap."""
    return na.capped_boulder(w, h, mm.VEST, mm.VEST_SH, mm.HEADLAMP, mm.PLATEAU_SH, SAND, 1)  # Sandstone with sand


def dune_marker():
    """A 40x48 wooden dune marker post with a flag and a sign board (the E3 goal)."""
    img = mm.new_image(40, 48)  # Empty picture
    mm.fill_rect(img, 18, 6, 3, 42, WOOD)  # Post
    mm.fill_rect(img, 20, 6, 1, 42, WOOD_SH)  # Post shade
    ls.fill_poly(img, [(21, 6), (34, 10), (21, 14)], mm.SCARF)  # Flag
    mm.fill_rect(img, 6, 22, 28, 10, mm.GROUND)  # Sign board
    mm.fill_rect(img, 6, 30, 28, 2, mm.GROUND_SH)  # Board shade
    for x in range(9, 31):  # Painted dune shape on the board
        la.put(img, x, 26 + int(2 * math.sin(x / 3.0)), SAND)  # One line pixel
    return la.outline(img)  # Dark edge


def soft_top():
    """A 16x8 soft-sand ground-top tile with deep loose ripples. It replaces the theme's top tile on soft sand."""
    return na.ground_top(SAND_SH, SOFT, SAND, SAND_DK,
                         [(1, 2, SAND_DK), (2, 1, SAND), (6, 2, SAND_DK), (7, 1, SAND), (11, 2, SAND_DK), (12, 1, SAND), (5, 3, mm.HEADLAMP)])  # Loose ripples


def sand_puff(frame):
    """A 10x6 puff of sand kicked up by the explorer's feet on soft sand (frames 0 and 1 spread out)."""
    img = mm.new_image(10, 6)  # Empty picture
    for (x, y) in (((3, 4), (5, 3), (6, 4), (4, 5)), ((1, 3), (3, 1), (7, 2), (8, 4), (5, 0)))[frame]:  # Grains of this frame
        la.put(img, x, y, SAND if (x + y) % 2 else SAND_DK)  # One grain
    return img


def beach_grass():
    """A 12x12 tuft of dune grass (decoration only)."""
    img = mm.new_image(12, 12)  # Empty picture
    for i, x in enumerate((2, 4, 6, 8, 10)):  # Five blades
        for t in range(9):  # Up the blade
            la.put(img, x + (t * (i - 2)) // 6, 11 - t, mm.HILLS_SH if t < 5 else mm.PLAINS)  # Bent blade
    return img


# ---------- New rocks and minerals (24x24, Assets/Art/Items/Sprites/) ----------

def coral_stone():
    """Cream coral stone: old coral rock full of tiny round pores."""
    def c(x, y, s):  # Colour of one pixel
        if (x % 4, y % 4) in ((1, 1), (3, 3)):  # A neat grid of coral pores
            return LIME_DK  # Pore
        return LIME_SH if s else LIME  # Cream rock
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 2.4))  # Lumpy rock


def limonite():
    """Yellow-brown limonite: an earthy iron mineral with rusty streaks."""
    def c(x, y, s):  # Colour of one pixel
        if (x + 2 * y) % 7 == 0:  # Rusty streak
            return mm.PLATEAU_SH  # Rust
        if lm.noise(x, y) < 12:  # Earthy specks
            return mm.GROUND_SH  # Dark speck
        return mm.VEST_SH if s else mm.HELMET_SH  # Yellow-brown
    return ls.outline(na.blob(24, 24, 11.5, 14, 10, 7, c, 2.0))  # Rounded lump


def kaolinite():
    """White kaolinite: a soft, powdery clay mineral."""
    img = na.blob(24, 24, 11.5, 14, 10, 7, lambda x, y, s: mm.MASK if s else mm.WHITE, 2.0)  # Soft white lump
    for (x, y) in ((7, 12), (11, 10), (15, 13), (9, 16), (14, 17)):  # Powder dots
        ls.put(img, x, y, mm.MASK_SH)  # One dot
    for x in range(15, 20):  # Powder smudge at the edge
        ls.put(img, x, 20, mm.MASK)  # One smudge pixel
    return ls.outline(img)  # Dark edge


# ---------- Lesson pictures (120x82, Assets/Art/Lesson/Sprites/) ----------

def chocolate_hills_pixel(x, y):
    """The Chocolate Hills: rows of rounded cone hills, brown-green, with blue sky."""
    best = None  # The nearest hill that covers this pixel
    for i, (cx, base, r) in enumerate(((8, 50, 8), (30, 49, 9), (54, 51, 8), (78, 49, 9), (102, 50, 8),
                                      (18, 66, 11), (46, 68, 12), (76, 66, 11), (106, 68, 12))):  # Hill middles, feet and sizes
        top = base - r * 1.1 + ((x - cx) / r) ** 2 * r * 1.1  # A low, rounded dome
        if abs(x - cx) < r and y >= top and y <= base + 2:  # Inside this hill
            best = (i, cx)  # Later rows are nearer, so they win
    if best is not None:  # On a hill
        i, cx = best  # Which hill
        shade = x < cx - 2  # Left side in shade
        if i < 5:  # Far row, paler
            return CHOC_DRY if shade else mix(CHOC_DRY, mm.VEST)  # Far brown hills
        return CHOC if shade else mix(CHOC, CHOC_LT)  # Near brown-green hills
    if y >= 47:  # Flat ground between the hills
        return mm.HILLS_SH if y > 56 else mix(mm.HILLS_SH, lm.HAZE)  # Green fields, hazy far away
    return na.sky(x, y)  # Sky


def batanes_pixel(x, y):
    """Batanes: rolling green hills that end at a sea cliff, with a little lighthouse."""
    top = 38 + int(6 * math.sin(x / 14.0)) + int(3 * math.sin(x / 5.0 + 1))  # Rolling hill line
    cliff = 92  # Where the hills drop to the sea
    if x < cliff and y >= top:  # On the hills
        if y < top + 2:  # Sunny top line
            return mm.PLAINS  # Bright grass
        if x > cliff - 4:  # The cliff face
            return mm.MASK_SH  # Grey rock
        rising = math.cos(x / 14.0) * 6 / 14.0 + math.cos(x / 5.0 + 1) * 3 / 5.0  # How steeply the hill line climbs here
        if rising > 0.15 and y < top + 14:  # Slopes that face away from the sun on the left
            return PASTURE_SH  # Shaded slope
        return PASTURE if y < top + 18 else mix(PASTURE, PASTURE_SH)  # Bright grass, a little darker lower down
    if y >= 52:  # The sea
        return SEA if y < 60 else SEA_DEEP  # Blue sea
    return na.sky(x, y)  # Sky


def la_paz_pixel(x, y):
    """La Paz Sand Dunes: big tan dunes with ripples and the sea far away."""
    far = 40 + int(5 * math.sin(x / 11.0))  # Far dune line
    near = 54 + int(9 * math.sin(x / 19.0 + 2))  # Near dune line
    if y >= near:  # Near dune
        if (y - near + x // 3) % 5 == 0:  # Wind ripples
            return SAND_DK  # Ripple shadow
        return SAND if x % 38 > 12 else SAND_SH  # Lit and shaded faces
    if y >= far:  # Far dunes
        return SAND_SH  # Paler with distance
    if y >= 34:  # Sea strip on the horizon
        return SEA  # Blue sea
    return na.sky(x, y)  # Sky


def batanes_lesson():
    """The Batanes picture with a small white lighthouse standing on the hill top."""
    img = ls.framed(batanes_pixel)  # Hills, cliff and sea
    top = 38 + int(6 * math.sin(70 / 14.0)) + int(3 * math.sin(70 / 5.0 + 1)) + 1  # Hill line under the lighthouse (frame adds 1)
    mm.fill_rect(img, 70, top - 10, 3, 10, mm.WHITE)  # Tower
    mm.fill_rect(img, 70, top - 12, 3, 2, mm.SCARF)  # Red lamp room
    return img


# ---------- Hills life sprites for the Landform Map and the Hills menu screens ----------

def tree(frame):
    """A 9x11 round tree; frame 1 leans its crown one pixel right, so two frames make it sway."""
    img = mm.new_image(9, 11)  # Empty picture
    mm.fill_rect(img, 4, 7, 1, 4, WOOD_SH)  # Trunk
    ls.disc(img, 3 + frame, 4, 3, mm.LEAF)  # Crown
    ls.disc(img, 4 + frame, 3, 1, mm.HILLS_SH)  # Sunny leaves
    return img


def ripple(frame):
    """A 6x2 light shine on the stream; the two frames swap places so the water seems to flow."""
    img = mm.new_image(6, 2)  # Empty picture
    for x in ((0, 1, 4), (2, 3, 5))[frame]:  # Shine pixels of this frame
        la.put(img, x, frame, mm.WHITE)  # One shine pixel
    return img


def small_bird(frame):
    """A 5x3 small dark bird for the Hills sky; frame 0 wings up, frame 1 wings down."""
    img = mm.new_image(5, 3)  # Empty picture
    rows = ((0, 0), (1, 1), (2, 1), (3, 1), (4, 0)) if frame == 0 else ((0, 2), (1, 1), (2, 1), (3, 1), (4, 2))  # Wing pixels
    for (x, y) in rows:  # Each pixel
        la.put(img, x, y, mm.STRAP)  # Dark bird
    return img


def grass_blade(frame):
    """A 7x6 tuft of tall grass; frame 1 bends the tips right, so the grass sways in the wind."""
    img = mm.new_image(7, 6)  # Empty picture
    for x in (1, 3, 5):  # Three blades
        for t in range(6):  # Up the blade, from the ground row 5 to the tip row 0
            la.put(img, x + (frame if t >= 4 else 0), 5 - t, mm.HILLS_SH if t < 3 else mm.PLAINS)  # One blade pixel
    return img


def map_save(img, name):
    """Saves an image under Assets/Art/LandformMap/Sprites/Hills/."""
    os.makedirs(MAP_OUT, exist_ok=True)  # Make the folder if it is missing
    img.save(os.path.join(MAP_OUT, name))  # Write the PNG


def preview(sprites):
    """Lays every new sprite out at 3x size on one sheet, so a person can check them at a glance."""
    scale, pad, width = 3, 6, 900  # Size up, gap and sheet width
    x = y = pad  # Where the next sprite goes
    row_h = 0  # Tallest sprite in the row
    places = []  # Where each sprite lands
    for img in sprites:  # Work out the places first
        w, h = img.width * scale, img.height * scale  # Scaled size
        if x + w > width:  # No room left in this row
            x, y, row_h = pad, y + row_h + pad, 0  # Start a new row
        places.append((x, y))  # Remember the place
        x, row_h = x + w + pad, max(row_h, h)  # Move along
    sheet = Image.new("RGBA", (width, y + row_h + pad), mm.SKY_LIGHT + (255,))  # Light sky behind, like the game
    for img, (px, py) in zip(sprites, places):  # Paste each one
        big = img.resize((img.width * scale, img.height * scale), Image.NEAREST)  # Crisp pixels
        sheet.alpha_composite(big, (px, py))  # Place it
    os.makedirs(os.path.dirname(PREVIEW), exist_ok=True)  # Make the folder
    sheet.save(PREVIEW)  # Write the sheet
    print("Preview written to", PREVIEW)  # Tell where


def main():
    shown = []  # Every sprite drawn, for the preview sheet
    themes = {  # Sprite name: drawing function, for the three Easy themes and soft sand
        "choc_top": choc_top, "choc_soil": choc_soil, "ledge_choc": ledge_choc, "viewing_deck": viewing_deck,
        "pasture_top": pasture_top, "pasture_soil": pasture_soil, "ledge_pasture": ledge_pasture, "stone_house": stone_house,
        "dune_top": dune_top, "dune_soil": dune_soil, "ledge_dune": ledge_dune, "dune_marker": dune_marker,
        "soft_top": soft_top,
    }
    for name, draw in themes.items():  # Each sprite
        img = draw()  # Draw it
        la.save(img, name + ".png")  # Write it
        shown.append(img)  # Keep it for the preview
    for ending, draw in (("_choc", choc_boulder), ("_pasture", pasture_boulder), ("_dune", dune_boulder)):  # Each theme's boulders
        for (w, h, size) in ((32, 24, "small"), (48, 36, "big")):  # Small and big
            img = draw(w, h)  # Draw it
            la.save(img, "boulder_%s%s.png" % (size, ending))  # Write it
            shown.append(img)  # Keep it
    for i in range(2):  # Two-frame objects
        for img, name in ((sand_puff(i), "Props/sand_puff_%02d.png"), (goat(i), "Props/goat_%02d.png"), (cow(i), "Props/cow_%02d.png")):  # Each one
            la.save(img, name % i)  # Write it
            shown.append(img)  # Keep it
    props = {"palm_tree": palm_tree, "cogon_tuft": cogon_tuft, "stone_fence": stone_fence, "beach_grass": beach_grass}  # Living props, decoration only
    for name, draw in props.items():  # Each prop
        img = draw()  # Draw it
        la.save(img, "Props/%s.png" % name)  # Write it
        shown.append(img)  # Keep it
    items = {"coral_stone": coral_stone, "limonite": limonite, "kaolinite": kaolinite}  # The three new kinds (E3 uses feldspar, D-066)
    for name, draw in items.items():  # Each new kind
        img = draw()  # Draw it
        ls.save(img, ls.ITEMS_OUT, name + ".png")  # Write it to the shared item folder
        shown.append(img)  # Keep it
    for img, name in ((ls.framed(chocolate_hills_pixel), "lesson_chocolate_hills"), (batanes_lesson(), "lesson_batanes"),
                      (ls.framed(la_paz_pixel), "lesson_la_paz")):  # The three lesson pictures
        ls.save(img, ls.LESSON_OUT, name + ".png")  # Write it
        shown.append(img)  # Keep it
    for i in range(2):  # Hills life frames
        for img, name in ((tree(i), "tree_%d.png"), (ripple(i), "ripple_%d.png"), (small_bird(i), "bird_%d.png"), (grass_blade(i), "grass_%d.png")):  # Each one
            map_save(img, name % i)  # Write it
            shown.append(img)  # Keep it
    preview(shown)  # The check sheet
    print("Hills Easy art written to", la.OUT, ls.ITEMS_OUT, ls.LESSON_OUT, "and", MAP_OUT)  # Tell where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
