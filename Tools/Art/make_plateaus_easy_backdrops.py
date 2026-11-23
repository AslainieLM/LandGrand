"""Draws the moving background layers for the three Plateaus Easy themes (D-072).

Run from the Unity project folder:  python Tools/Art/make_plateaus_easy_backdrops.py
It writes L0.png to L5.png (960x270 each) to Assets/Art/Level/Sprites/Backdrop/<Theme>/ for
PineapplePlateau (Bukidnon Plateau), LakePlateau (Lake Lanao) and FallsEdge (Maria Cristina Falls),
plus a preview at .scratch/plateaus-easy/backdrop-preview.png.

Each set keeps the six layers and the scrolling of the Plains Easy backdrops:
L0 sky, L1 clouds, L2 far line, L3 middle ground, L4 near band, L5 near bank.
A plateau is high, wide land with a flat top and steep sides, so each set shows flat tops and steep cliff faces.
The rows behind the explorer (100 to 220) stay mid-toned and soft, so the explorer, the pickups and the ledges stand out.
Every colour is a palette colour or an even mix of two. Every layer tiles sideways without a seam.
"""
import math  # sin and pi for the shapes
import os  # Folders and paths
from PIL import Image  # Saves the preview
import make_mainmenu_art as mm  # Palette and image helpers
import make_landform_map_art as lm  # Map colours and the fixed noise
import make_mountains_backdrop as mb  # Pixels, dither, mix and the wrap helper
import make_normal_backdrops as nb  # Gradient, sun, clouds, ranges, crowns, rolling lines and the preview

W, H = mb.W, mb.H  # 960x270 layers
mix, put, column, dither, jag = mb.mix, mb.put, mb.column, mb.dither, mb.jag  # Shared drawing helpers
PREVIEW = os.path.join(mb.ROOT, ".scratch", "plateaus-easy", "backdrop-preview.png")  # Where the preview goes
SHIRT = (63, 143, 134)  # Palette "Shirt", a blue-green: pineapple leaves and deep lake water
SHIRT_SH = (45, 107, 100)  # Palette "Shirt shade"
PANTS = (92, 74, 61)  # Palette "Pants", a grey-brown: wet basalt
BOOTS = (61, 42, 30)  # Palette "Boots", a dark brown


def mesa_tops(mesas, seed):
    """The outline of a row of flat-topped plateaus. mesas: (middle x, half width of the flat top, top row, rows down per pixel on the sides).
    Returns, for each x, the top row, how far outside the flat top the column is, on which side, and the flat top row of its plateau."""
    tops, outs, sides, flats = [], [], [], []  # Top row, distance past the edge, side and flat top row for each x
    for x in range(W):  # Each column
        best, out, side, flat = H, 0, 0, H  # Lowest row number wins
        for cx, half, top, steep in mesas:  # Each plateau
            off = mb.wrap_offset(x, cx)  # Signed distance from its middle, the short way round
            d = abs(off) - half  # How far past the edge of the flat top
            y = top if d <= 0 else top + d * steep + jag(x, seed, 2)  # Flat on top, a steep rough side past the edge
            if y < best:  # Higher than the others
                best, out, side, flat = y, max(0, d), (1 if off > 0 else -1), top  # This plateau owns the column
        tops.append(int(best))  # Whole rows only
        outs.append(out)  # Remember the distance past the edge
        sides.append(side)  # Remember the side: -1 left, 1 right
        flats.append(flat)  # Remember the flat top row
    return tops, outs, sides, flats  # Give the outline back


def mesa_layer(img, mesas, seed, cap, cap_depth, face, face_sh, strata, side_lit, side_sh, bottom=H):
    """Draws plateaus: a flat green cap, a cliff face with wavy flat rock layers and dark gullies, and steep sides.
    The sun is on the right, so the left sides are in shade. Returns the outline."""
    tops, outs, sides, flats = mesa_tops(mesas, seed)  # The plateau outline
    for x in range(W):  # Each column
        gully = lm.noise(x // 3 % (W // 3), seed) % 9 == 0  # Some 3-pixel strips hold a gully worn by rain
        gully_len = 6 + lm.noise(x // 3 % (W // 3), seed + 1) % 14  # How far down the gully runs
        for y in range(tops[x], bottom):  # From the top down
            depth = y - tops[x]  # Rows below the local top
            layer = int((y - flats[x] + 1.5 * math.sin(2 * math.pi * 6 * x / W + seed)) / 5)  # Which rock layer this row is in; the layers are flat but a little wavy
            if depth < cap_depth:  # The green cap on the flat top and on the upper sides
                c = cap[0] if depth == 0 else cap[1]  # A lit top line, then the cap body
            elif outs[x] > 0:  # A steep side
                c = side_sh if sides[x] < 0 else side_lit  # Shaded left side, sunny right side
                if layer % 3 == 1 and sides[x] > 0:  # The rock layers show on the sunny side too
                    c = strata  # Layer colour
            else:  # The front cliff face
                c = (face, strata, face)[layer % 3]  # Bands of rock layers
                if gully and depth < cap_depth + gully_len:  # Inside a gully
                    c = face_sh  # Gully shadow
            put(img, x, y, c)  # Paint it
    return tops, outs  # Give the outline back, for trees on top


def dome_range(peaks, seed, lit, shade, spot, mist, mist_from, foot=160, power=2.0):
    """L2 helper: a range of round-topped mountains, not sharp ones. peaks: (middle x, top row, half width at the foot).
    power 2 gives round domes; a smaller power gives broader cones like old volcanoes.
    Shaded left faces, a few dark spots of forest, and mist thickening from row mist_from down."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        best, owner = H, None  # Highest mountain at this column
        for cx, top, half in peaks:  # Each mountain
            off = mb.wrap_offset(x, cx)  # Signed distance from its middle
            y = top + (foot - top) * abs(off / half) ** power  # A round top that gets steeper going out
            if y < best:  # Higher than the others
                best, owner = y, (cx, top, off)  # This mountain owns the column
        best = int(best) + jag(x, seed, 1)  # A little roughness from the trees on the skyline
        cx, top, off = owner  # The mountain that owns this column
        for y in range(best, H):  # From the skyline down
            c = shade if off < (y - top) * 0.35 - 3 else lit  # Left of a slanting line is the shaded face
            if lm.noise(x // 2 % (W // 2), y // 2 + seed) % 19 == 0:  # A few dark spots of forest
                c = spot  # Spot pixel
            if y >= mist_from and dither(x, y, min(1.0, (y - mist_from) / 22)):  # Mist thickens going down
                c = mist  # Mist pixel
            put(img, x, y, c)  # Paint it
    return img  # Give the picture back


def tree(img, x, base, r, lit, shade, trunk):
    """A broad tropical tree: a trunk and a crown of three round clumps."""
    column(img, x, base - r - 6, trunk, base + 1)  # The trunk
    nb.crown(img, x - r + 1, base - r - 6, r - 1, lit, shade)  # Left clump
    nb.crown(img, x + r - 1, base - r - 6, r - 1, lit, shade)  # Right clump
    nb.crown(img, x, base - 2 * r - 4, r, lit, shade)  # Top clump


def pineapple(img, x, base, size, leaf, leaf_sh, fruit):
    """A pineapple plant: a spiky rosette of leaves, with a small fruit in the middle when it is big enough."""
    for t in range(1, size + 1):  # Along each leaf
        put(img, x - t, base - t // 2, leaf_sh)  # Left leaf, lower and shaded
        put(img, x + t, base - t // 2, leaf)  # Right leaf, sunny
        put(img, x - t // 2, base - t, leaf_sh)  # Upper left leaf
        put(img, x + t // 2, base - t, leaf)  # Upper right leaf
    if size >= 3:  # Only bigger plants show the fruit
        put(img, x, base - size // 2, fruit)  # The fruit
        put(img, x, base - size // 2 - 1, fruit)  # Its top half
        put(img, x, base - size // 2 - 2, leaf)  # The little crown on the fruit


def reed(img, x, base, h, stem, head):
    """A thin reed: a one-pixel stem with a darker seed head at the top."""
    column(img, x, base - h, stem, base + 1)  # The stem
    put(img, x, base - h, head)  # Seed head
    put(img, x, base - h + 1, head)  # Seed head, two pixels long


def banca(img, x, y, hull, rig, sail):
    """A small far boat with outriggers and a tiny sail."""
    for dx in range(-3, 4):  # The hull
        put(img, x + dx, y, hull)  # Hull pixel
    for dx in (-5, -4, 4, 5):  # The outrigger floats
        put(img, x + dx, y + 1, rig)  # Float pixel
    column(img, x, y - 4, rig, y)  # The mast
    put(img, x + 1, y - 3, sail)  # Sail
    put(img, x + 1, y - 2, sail)  # Sail
    put(img, x + 2, y - 2, sail)  # Sail, wider at the bottom


def fern(img, x, base, size, leaf, leaf_sh):
    """A fern: five fronds that spread out from the root and droop at the tips, each with little leaflets."""
    for spread, rise in ((-1.0, 0.35), (-0.5, 0.8), (0.0, 1.0), (0.5, 0.8), (1.0, 0.35)):  # Each frond: how far it leans out and how high it climbs
        for t in range(size):  # Along the frond, from the root out
            f = t / size  # 0 at the root, 1 at the tip
            fx = x + int(round(spread * t))  # Across
            fy = base - int(round(1.4 * rise * size * f * (1.6 - f)))  # Up, then a little down at the tip
            c = leaf_sh if spread < 0 else leaf  # Left fronds are in shade
            put(img, fx, fy, c)  # Frond pixel
            if t % 2 == 1 and t < size - 1:  # A pair of leaflets on every second step
                put(img, fx - 1, fy, c)  # Left leaflet
                put(img, fx + 1, fy, c)  # Right leaflet


# ---------- PineapplePlateau: Bukidnon Plateau, Mindanao. Clear sky, Kitanglad range, a flat tableland with cliffs, pineapple rows ----------

PP_SKY = [(0, lm.SKY), (110, mm.SKY_LIGHT), (150, mix(mm.SKY_LIGHT, mm.WHITE, 0.4))]  # Clear deep blue, pale near the range
PP_FAR = mix(lm.HAZE, mm.HILLS_SH, 0.25)  # Kitanglad range, blue-green with distance
PP_FAR_SH = mix(lm.HAZE_SH, lm.MOUNTAIN, 0.3)  # Its shaded faces
PP_MIST = mix(mm.SKY_LIGHT, mm.WHITE, 0.3)  # Mist at its foot
PP_CAP = mix(mm.HILLS, lm.HAZE, 0.3)  # Green grass on the plateau top
PP_CAP_LIT = mix(mm.PLAINS, lm.HAZE, 0.3)  # Sunny top line of the plateau
PP_FACE = mix(mm.PLATEAU, lm.HAZE, 0.35)  # Red-brown cliff face
PP_FACE_SH = mix(mm.PLATEAU_SH, lm.HAZE, 0.3)  # Cracks in the cliff
PP_STRATA = mix(mm.VEST, lm.HAZE, 0.35)  # Pale rock layers in the cliff
PP_SIDE = mix(mm.PLATEAU, lm.HAZE, 0.2)  # Sunny right side of the plateau
PP_SIDE_SH = mix(mm.PLATEAU_SH, lm.HAZE_SH, 0.4)  # Shaded left side
PP_LOW = mix(mm.HILLS_SH, lm.HAZE, 0.4)  # Green low ground below the cliffs
PP_LOW_LIT = mix(mm.HILLS, lm.HAZE, 0.45)  # Its far sunny edge
PP_LOW_FIELD = mix(SHIRT, lm.HAZE, 0.45)  # Far pineapple fields on the low ground
PP_LOW_SOIL = mix(mm.PLATEAU, lm.HAZE, 0.5)  # Far red soil strips
PP_NTREE = mix(mm.LEAF, mm.HILLS, 0.35)  # Near shade trees
PP_NTREE_SH = mix(mm.LEAF, SHIRT_SH, 0.4)  # Their shade
PP_SOIL_LIT = mix(mm.PLATEAU, mm.VEST, 0.5)  # Sunny far edge of the field
PP_TREE = mix(mm.LEAF, lm.HAZE, 0.3)  # Far trees on the plateau
PP_TREE_SH = mix(mm.LEAF, lm.HAZE_SH, 0.45)  # Their shade
PP_SOIL = mix(mm.PLATEAU, mm.GROUND, 0.5)  # Red-brown soil between the pineapple rows
PP_SOIL_SH = mix(mm.PLATEAU_SH, mm.GROUND_SH, 0.5)  # Soil in the furrows
PP_PINE = mix(SHIRT, mm.HILLS_SH, 0.4)  # Pineapple leaves, grey-green
PP_PINE_SH = mix(SHIRT_SH, mm.LEAF, 0.4)  # Shaded pineapple leaves
PP_FRUIT = mm.HELMET_SH  # Golden pineapple fruit
PP_NEAR = mix(mm.PLATEAU_SH, mm.GROUND_SH, 0.5)  # Near red-brown bank
PP_NEAR_LIT = mix(mm.PLATEAU, mm.VEST, 0.4)  # Sunny top of the bank
PP_TRUNK = mix(lm.HANDLE, mm.GROUND_SH, 0.5)  # Tree trunks


def pp_sky():
    """L0: a clear morning sky with a bright sun."""
    img = nb.gradient(PP_SKY)  # Deep blue fading to pale near the range
    nb.sun(img, 380, 40, 8, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4))  # Morning sun
    return img  # Give the picture back


def pp_clouds():
    """L1: a few small white clouds, and a cloud cap resting on the high peaks."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((120, 28, 80), (430, 50, 60), (700, 22, 90), (900, 44, 50)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # One small cloud
    return img  # Give the picture back


def pp_far():
    """L2: the Kitanglad range, broad old volcanoes with round shoulders, pale blue-green."""
    peaks = [(80, 70, 150), (260, 88, 120), (420, 62, 170), (620, 84, 140), (800, 74, 150)]  # Broad summits: middle x, top row, half width
    return dome_range(peaks, 21, PP_FAR, PP_FAR_SH, PP_FAR_SH, PP_MIST, 118, 160, 1.4)  # Wide volcanic range with mist at its foot


def pp_mid():
    """L3: a wide flat plateau with steep red-brown cliffs, a smaller one beside it, and far fields on the low ground below."""
    img = mb.new_layer()  # Empty layer
    low = 158  # Row where the low ground starts
    for y in range(low, H):  # The low ground in front of the cliff feet, in field strips
        k = (y - low) // 4  # Which strip this row is in
        c = (PP_LOW, PP_LOW_FIELD, PP_LOW, PP_LOW_SOIL)[k % 4]  # Green, pineapple green, green and red soil strips
        for x in range(W):  # Each pixel
            put(img, x, y, PP_LOW_LIT if y == low else c)  # A lit far edge, then the strips
    mesas = [(250, 200, 118, 3.0), (760, 110, 130, 2.6)]  # Middle x, half width, top row, steepness
    tops, outs = mesa_layer(img, mesas, 31, (PP_CAP_LIT, PP_CAP), 3, PP_FACE, PP_FACE_SH, PP_STRATA, PP_SIDE, PP_SIDE_SH, low + 3)  # The two plateaus
    for i, x in enumerate(range(0, W, 5)):  # Green bushes along the cliff feet
        cx = x + lm.noise(i, 32) % 4  # A little varied place
        if tops[cx % W] < low and lm.noise(i, 30) % 5 < 2:  # Only where a plateau stands, in uneven clumps
            nb.crown(img, cx, low + 1, 2 + lm.noise(i, 33) % 2, PP_TREE, PP_TREE_SH)  # One bush
    for i, x in enumerate(range(10, W, 26)):  # Small trees on the plateau tops
        cx = (x + lm.noise(i, 34) % 10) % W  # A little varied place
        if outs[cx] == 0 and lm.noise(i, 35) % 3 > 0:  # Only on a flat top, two in three places
            nb.crown(img, cx, tops[cx] - 2, 2, PP_TREE, PP_TREE_SH)  # One small far tree
    return img  # Give the picture back


PP_ROWS = ((190, 2), (196, 3), (203, 4), (211, 5), (220, 6))  # Pineapple rows: base row and plant size, bigger toward the viewer


def pp_rows():
    """L4: pineapple field rows on red-brown soil, getting bigger toward the viewer, with a few scattered shade trees."""
    img = mb.new_layer()  # Empty layer
    top = 186  # Row where the field starts
    for y in range(top, H):  # The soil
        for x in range(W):  # Each pixel
            put(img, x, y, PP_SOIL_LIT if y == top else PP_SOIL)  # A sunny far edge, then red-brown soil
    for r, (y, size) in enumerate(PP_ROWS):  # Each row of plants
        for x in range(W):  # The dark furrow under the row
            put(img, x, y + 1, PP_SOIL_SH)  # Furrow pixel
        for x in range(W):  # The dense mass of leaves along the row
            for dy in range(size // 2 + 1):  # A few rows tall
                put(img, x, y - dy, PP_PINE_SH)  # Leaf mass pixel
        step = min(d for d in range(size + 2, W) if W % d == 0)  # Space between plants: the next size that divides 960, so the row tiles
        for i, x in enumerate(range(0, W, step)):  # Each plant in the row
            fruit = PP_FRUIT if i % 3 == 0 else PP_PINE  # Only some plants show a fruit
            pineapple(img, x + (r * 5) % step, y, size, PP_PINE, PP_PINE_SH, fruit)  # One plant, rows shifted a little
    for i, x in enumerate((90, 350, 610, 840)):  # A few scattered shade trees in the field
        tree(img, x, 192 + lm.noise(i, 36) % 3, 6 + lm.noise(i, 37) % 2, PP_NTREE, PP_NTREE_SH, PP_TRUNK)  # One tree
    return img  # Give the picture back


def pp_near():
    """L5: a near bank of red-brown soil just behind the trail, with big pineapple plants on it."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(219, 2, 3, 39)  # A gently wavy line near the trail
    nb.fill_below(img, tops, PP_NEAR_LIT, PP_NEAR)  # Soil bank with a sunny top line
    for i, x in enumerate(range(16, W, 48)):  # Big plants along the bank
        cx = (x + lm.noise(i, 38) % 12) % W  # A little varied place
        pineapple(img, cx, tops[cx] + 1, 9, PP_PINE, PP_PINE_SH, mm.HELMET)  # One big plant
    return img  # Give the picture back


# ---------- LakePlateau: Lake Lanao. Mountains ring a big calm lake high on the plateau, far boats, reeds on the shore ----------

LP_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.15)), (100, lm.SKY), (140, mm.SKY_LIGHT)]  # Calm blue sky
LP_FAR = mix(lm.HAZE, mm.LEAF, 0.25)  # Ring of mountains, blue-green
LP_FAR_SH = mix(lm.HAZE_SH, lm.MOUNTAIN, 0.35)  # Their shaded faces
LP_MIST = mix(mm.SKY_LIGHT, lm.HAZE, 0.3)  # Mist on the far shore
LP_LAKE_FAR = mix(lm.LENS, lm.HAZE, 0.5)  # Lake water near the far shore, pale
LP_LAKE = mix(lm.LENS, lm.MOUNTAIN, 0.3)  # Lake water, calm blue
LP_SHINE = mix(mm.SKY_LIGHT, mm.WHITE, 0.5)  # Light on the water
LP_CAP = mix(mm.HILLS, lm.HAZE, 0.35)  # Grass on the flat lakeside plateau
LP_CAP_LIT = mix(mm.PLAINS, lm.HAZE, 0.35)  # Its sunny top line
LP_FACE = mix(mm.HILLS_SH, lm.HAZE, 0.45)  # Green forested bank
LP_FACE_SH = mix(mm.LEAF, lm.HAZE, 0.4)  # Dark gullies in the bank
LP_STRATA = mix(mm.GROUND, lm.HAZE, 0.45)  # Bare brown earth layers
LP_SIDE = mix(mm.HILLS_SH, lm.HAZE, 0.3)  # Sunny side
LP_SIDE_SH = mix(mm.LEAF, lm.HAZE_SH, 0.5)  # Shaded side
LP_HULL = mix(mm.DARK, lm.HAZE, 0.35)  # Far boat hulls
LP_RIG = mix(mm.STRAP, lm.HAZE, 0.45)  # Far boat outriggers and masts
LP_SAIL = mix(mm.SCARF, lm.HAZE, 0.35)  # Small coloured sail
LP_SHORE = mix(mm.HILLS_SH, mm.LEAF, 0.35)  # Grassy near shore
LP_SHORE_LIT = mix(mm.HILLS, mm.PLAINS, 0.3)  # Its sunny top line
LP_REED = lm.PLAINS_SH  # Reed stems
LP_REED_HEAD = mm.VEST_SH  # Reed seed heads
LP_TREE = mix(mm.LEAF, mm.HILLS_SH, 0.4)  # Shore trees
LP_TREE_SH = mix(mm.LEAF, SHIRT_SH, 0.5)  # Their shade
LP_TRUNK = mix(lm.HANDLE, mm.GROUND_SH, 0.5)  # Tree trunks
LP_SHORE_FAR = mix(mm.HILLS_SH, lm.HAZE, 0.4)  # Thin grass line on the far shore
LP_REFLECT = mix(lm.LENS, mm.HILLS_SH, 0.5)  # Still green reflection of the banks in the lake
LP_TOP_TREE = mix(mm.LEAF, lm.HAZE, 0.35)  # Far trees on the tablelands
LP_TOP_TREE_SH = mix(mm.LEAF, lm.HAZE_SH, 0.5)  # Their shade
LP_LAKE_EDGE = mix(lm.LENS, mm.WHITE, 0.4)  # Wet line where the near water meets the grass
LP_NEAR = mix(mm.HILLS_SH, mm.LEAF, 0.6)  # Near grass bank
LP_NEAR_LIT = mix(mm.HILLS, mm.HILLS_SH, 0.5)  # Its sunny top line
LP_NREED = mix(lm.PLAINS_SH, mm.LEAF, 0.4)  # Near reed stems


def lp_sky():
    """L0: a calm blue sky with a soft sun."""
    img = nb.gradient(LP_SKY)  # Blue fading to pale at the far shore
    nb.sun(img, 90, 36, 8, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4))  # Soft sun
    return img  # Give the picture back


def lp_clouds():
    """L1: long calm clouds."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((60, 58, 130), (300, 30, 110), (560, 54, 150), (820, 34, 120)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mix(mm.SKY_LIGHT, lm.HAZE, 0.3))  # One cloud
    return img  # Give the picture back


def lp_far():
    """L2: a ring of round, forested mountains all along the far shore, with mist at the water line."""
    peaks = [(60, 92, 150), (230, 104, 120), (400, 84, 160), (590, 100, 130), (760, 88, 150), (900, 106, 100)]  # Round summits: middle x, top row, half width
    return dome_range(peaks, 17, LP_FAR, LP_FAR_SH, LP_FAR_SH, LP_MIST, 118, 150)  # Ring of mountains, misty at the foot


LP_WATER = 138  # Row of the far shore water line


def lp_mid():
    """L3: the big calm lake on the plateau, flat-topped lakeside tablelands with steep banks, their still reflections and small far boats."""
    img = mb.new_layer()  # Empty layer
    for y in range(LP_WATER, H):  # The lake
        t = min(1.0, (y - LP_WATER) / 34)  # Pale far away, deeper blue nearer
        for x in range(W):  # Each pixel
            c = LP_LAKE if dither(x, y, t) else LP_LAKE_FAR  # Dotted blend from pale to blue
            if (y - LP_WATER) % 6 == 3 and (x + 37 * ((y - LP_WATER) // 6)) % 64 < 3 + (y - LP_WATER) // 6:  # Short flat shine lines, longer nearer
                c = LP_SHINE  # Shine pixel
            put(img, x, y, c)  # Paint it
    for x in range(W):  # The thin far shore line
        put(img, x, LP_WATER - 1, LP_SHORE_FAR)  # Grass line on the far shore
    mesas = [(330, 80, 114, 2.4), (740, 110, 118, 2.2)]  # Lakeside tablelands: middle x, half width, top row, steepness
    tops, outs = mesa_layer(img, mesas, 41, (LP_CAP_LIT, LP_CAP), 3, LP_FACE, LP_FACE_SH, LP_STRATA, LP_SIDE, LP_SIDE_SH, LP_WATER)  # Their steep banks drop to the lake
    for x in range(W):  # Their still reflections in the calm water
        for y in range(LP_WATER, LP_WATER + (LP_WATER - tops[x]) // 2):  # Half as tall as the bank above
            if (x + y) % 2 == 0:  # Every second pixel, so the water shows through
                put(img, x, y, LP_REFLECT)  # Reflection pixel
    for i, x in enumerate(range(0, W, 8)):  # Trees along the flat tops
        if outs[x] == 0 and tops[x] < LP_WATER - 4 and lm.noise(i, 42) % 3 > 0:  # Only on a flat top, two in three places
            nb.crown(img, x + lm.noise(i, 43) % 4, tops[x] - 2, 2, LP_TOP_TREE, LP_TOP_TREE_SH)  # One small far tree
    for x, y in ((120, 152), (210, 146), (470, 160), (560, 149), (900, 156)):  # Small far boats
        banca(img, x, y, LP_HULL, LP_RIG, LP_SAIL)  # One boat
    return img  # Give the picture back


def lp_shore():
    """L4: a grassy near shore with clumps of reeds and a few trees. The lake shows above it."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(204, 3, 2, 43)  # The shore line
    nb.fill_below(img, tops, LP_SHORE_LIT, LP_SHORE)  # Grass with a sunny top line
    for x in range(W):  # A thin wet line where the water meets the grass
        put(img, x, tops[x] - 1, LP_LAKE_EDGE)  # Wet pixel
    for i, x in enumerate(range(0, W, 3)):  # Reeds all along the shore
        if lm.noise(i // 4, 44) % 5 < 2:  # In clumps, with gaps between
            reed(img, x + lm.noise(i, 45) % 2, tops[x % W], 4 + lm.noise(i, 46) % 7, LP_REED, LP_REED_HEAD)  # One reed
    for i, x in enumerate((150, 520, 790)):  # A few shore trees
        tree(img, x, tops[x], 6 + lm.noise(i, 47) % 2, LP_TREE, LP_TREE_SH, LP_TRUNK)  # One tree
    return img  # Give the picture back


def lp_near():
    """L5: the near grassy bank just behind the trail, with tall reeds."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(220, 2, 3, 48)  # A gently wavy line near the trail
    nb.fill_below(img, tops, LP_NEAR_LIT, LP_NEAR)  # Grass bank
    for i, x in enumerate(range(0, W, 2)):  # Tall reeds
        if lm.noise(i // 5, 49) % 4 == 0:  # In a few clumps
            reed(img, x, tops[x], 9 + lm.noise(i, 50) % 8, LP_NREED, mm.VEST_SH)  # One tall reed
    return img  # Give the picture back


# ---------- FallsEdge: Maria Cristina Falls. A twin waterfall over dark basalt at the plateau edge, mist, forest and ferns ----------

FE_SKY = [(0, mix(lm.SKY, lm.HAZE, 0.3)), (100, mix(mm.SKY_LIGHT, lm.HAZE, 0.2)), (160, mix(mm.SKY_LIGHT, mm.WHITE, 0.4))]  # Humid pale sky
FE_FAR = mix(lm.HAZE, mm.HILLS_SH, 0.3)  # Far plateau tables, pale green
FE_FAR_CAP = mix(lm.HAZE, mm.HILLS, 0.35)  # Their flat tops
FE_FAR_SH = mix(lm.HAZE_SH, mm.LEAF, 0.25)  # Their shaded sides
FE_FAR_STRATA = mix(lm.HAZE, mm.MASK_SH, 0.3)  # Pale grey rock layers in the far cliffs
FE_GORGE = mix(lm.HAZE_SH, mm.HILLS_SH, 0.4)  # Green floor of the far gorge
FE_RIVER = mix(lm.LENS, mm.SKY_LIGHT, 0.4)  # The far river in the gorge
FE_MIST = mix(mm.SKY_LIGHT, mm.WHITE, 0.5)  # Mist
FE_BASALT = mix(PANTS, lm.HAZE, 0.35)  # Basalt cliff, grey-brown softened by haze
FE_BASALT_SH = mix(mm.STRAP, lm.HAZE, 0.25)  # Joints between basalt columns
FE_BASALT_LT = mix(mm.MASK_SH, lm.HAZE, 0.3)  # Wet light on the columns
FE_BASALT_WET = mix(PANTS, mm.STRAP, 0.5)  # Dark wet basalt low on the cliff, near the spray
FE_FOREST = mix(mm.LEAF, lm.HAZE, 0.3)  # Forest on the cliff top
FE_FOREST_SH = mix(mm.LEAF, SHIRT_SH, 0.5)  # Its shade
FE_WATER = mm.WHITE  # Falling water
FE_WATER_SH = mix(lm.LENS, mm.WHITE, 0.5)  # Blue streaks in the water
FE_CANOPY = mix(mm.LEAF, mm.HILLS_SH, 0.35)  # Near forest canopy
FE_CANOPY_SH = mix(mm.LEAF, SHIRT_SH, 0.6)  # Its shade
FE_FERN = mix(mm.HILLS, mm.PLAINS, 0.3)  # Fern fronds, bright so they show against the forest
FE_FERN_SH = mix(mm.HILLS_SH, mm.HILLS, 0.3)  # Shaded fronds
FE_NEAR = mix(mm.LEAF, SHIRT_SH, 0.5)  # Near dark forest floor
FE_NEAR_LIT = mix(mm.HILLS_SH, mm.LEAF, 0.4)  # Mossy top line


def fe_sky():
    """L0: a pale, humid sky."""
    img = nb.gradient(FE_SKY)  # Soft blue to misty white
    nb.sun(img, 420, 30, 7, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.3))  # A soft sun
    return img  # Give the picture back


def fe_clouds():
    """L1: soft humid clouds."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((90, 40, 140), (360, 24, 100), (620, 46, 160), (870, 30, 110)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, FE_MIST)  # One cloud
    return img  # Give the picture back


def fe_far():
    """L2: far flat-topped plateau tables, with a deep river gorge cut between them and a thin river at its floor."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # The gorge floor first, with its river
        column(img, x, 160, FE_GORGE)  # Gorge floor
        ry = 166 + int(2 * math.sin(2 * math.pi * 2 * x / W))  # The river line, gently winding
        put(img, x, ry, FE_RIVER)  # River pixel
        put(img, x, ry + 1, FE_RIVER)  # River, two pixels wide
    mesas = [(120, 120, 110, 1.8), (520, 130, 104, 1.8), (840, 70, 116, 1.8)]  # Far tables with gorges between them
    mesa_layer(img, mesas, 51, (FE_FAR_CAP, FE_FAR_CAP), 2, FE_FAR, FE_FAR_SH, FE_FAR_STRATA, FE_FAR, FE_FAR_SH, 160)  # The far tables
    nb.mist_band(img, 136, 30, FE_MIST)  # Mist in the gorges
    return img  # Give the picture back


FE_CLIFF = [(290, 120, 100, 3.2), (760, 130, 146, 2.4)]  # The tall falls cliff, and a lower forested edge: middle x, half width, top row, steepness
FE_FALLS = ((276, 14), (298, 10))  # The twin falls: left x and width of each stream
FE_FOOT = 204  # Row where the falls drop into their spray


def fe_mid():
    """L3: the basalt cliff at the plateau edge, with forest on its flat top and the twin falls pouring down."""
    img = mb.new_layer()  # Empty layer
    tops, outs, sides, flats = mesa_tops(FE_CLIFF, 53)  # The outline
    for x in range(W):  # Each column
        for y in range(tops[x], 214):  # From the top down to the gorge floor
            depth = y - tops[x]  # Rows below the top
            ledge = (y - flats[x]) % 24 == 12  # A thin green ledge every 24 rows down the face
            col = x // 6  # Which basalt column this is
            joint = (y + lm.noise(col % 160, 64) % 20) % 20 == 0  # Each column breaks across at its own rows
            if flats[x] == FE_CLIFF[1][2]:  # The lower edge is covered in forest
                put(img, x, y, FE_FOREST_SH)  # Shaded green under the trees; round crowns go on top below
                continue  # Next pixel
            if depth < 3:  # Forest floor on the flat top
                c = FE_FOREST_SH  # Shaded green under the trees
            elif ledge and lm.noise(x // 3 % 320, y) % 4 > 0:  # Plants on a ledge, with gaps
                c = FE_FOREST_SH  # Ledge plants
            elif x % 6 == 0 or joint:  # The joints between and across the upright basalt columns
                c = FE_BASALT_SH  # Joint pixel
            elif x % 6 == 5:  # The sunny right edge of each column
                c = FE_BASALT_LT  # Light pixel
            else:  # The column face
                c = FE_BASALT_WET if y > 180 and dither(x, y, (y - 180) / 24) else FE_BASALT  # Darker and wet low down
            if outs[x] > 0 and sides[x] < 0 and c == FE_BASALT:  # The shaded left side of the cliff
                c = FE_BASALT_SH  # Darker basalt in shade
            put(img, x, y, c)  # Paint it
    for i, (x, y) in enumerate((x, y) for y in range(156, 212, 9) for x in range(0, W, 8)):  # Rows of round crowns on the forested edge
        cx = (x + (y // 9 % 2) * 4 + lm.noise(i, 66) % 3) % W  # Every second row shifted, so the crowns sit between the ones above
        if flats[cx] == FE_CLIFF[1][2] and tops[cx] + 3 < y:  # Only inside the forested edge
            nb.crown(img, cx, y + lm.noise(i, 67) % 4, 3 + lm.noise(i, 68) % 3, FE_FOREST, FE_FOREST_SH)  # One crown, a little varied in place and size
    for i, x in enumerate(range(0, W, 6)):  # Round tree crowns along the cliff tops
        cx = (x + lm.noise(i, 56) % 4) % W  # A little varied place
        if not any(fx - 3 <= cx < fx + fw + 3 for fx, fw in FE_FALLS):  # No trees where the river goes over the lip
            nb.crown(img, cx, tops[cx] - 1, 3 + lm.noise(i, 57) % 2, FE_FOREST, FE_FOREST_SH)  # One crown
    for fx, fw in FE_FALLS:  # The twin falls
        for x in range(fx, fx + fw):  # Each column of the stream
            for y in range(tops[x] - 2, FE_FOOT):  # From the lip down into the spray
                edge = x in (fx, fx + fw - 1)  # The two outer columns
                streak = lm.noise(x, (y + 2 * x) // 5) % 4 == 0  # Blue streaks that run down
                put(img, x, y, FE_WATER_SH if edge or streak else FE_WATER)  # Water pixel
    return img  # Give the picture back


def spray(img, cx, cy, rx, ry):
    """A soft dotted cloud of spray from the falls: white in the middle, misty at the edge."""
    for y in range(cy - ry, cy + ry + 1):  # Rows of the cloud
        for x in range(cx - rx, cx + rx + 1):  # Columns of the cloud
            d = ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2  # 0 in the middle, 1 at the edge
            if d < 1 and dither(x, y, 1.2 * (1 - d)):  # Thicker in the middle
                put(img, x, y, mm.WHITE if d < 0.3 else FE_MIST)  # Spray pixel


def fe_forest():
    """L4: the green forest canopy of the gorge, with the spray of the falls rising over it."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # The canopy floor
        column(img, x, 206 + int(3 * math.sin(2 * math.pi * 3 * x / W)) + jag(x, 58, 2), FE_CANOPY_SH)  # Fill below a rolling line
    for i, x in enumerate(range(0, W, 9)):  # Round crowns along the top
        base = 206 + int(3 * math.sin(2 * math.pi * 3 * x / W))  # The canopy line here
        nb.crown(img, x + lm.noise(i, 59) % 4, base - 1, 5 + lm.noise(i, 60) % 4, FE_CANOPY, FE_CANOPY_SH)  # One crown
    spray(img, 293, 200, 46, 18)  # The spray at the foot of the falls
    return img  # Give the picture back


def fe_near():
    """L5: the near forest floor just behind the trail, with big ferns."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(220, 2, 3, 61)  # A gently wavy line near the trail
    nb.fill_below(img, tops, FE_NEAR_LIT, FE_NEAR)  # Floor with a mossy top line
    for i, x in enumerate(range(8, W, 24)):  # Ferns along the floor
        cx = (x + lm.noise(i, 62) % 10) % W  # A little varied place
        fern(img, cx, tops[cx] + 1, 11 + lm.noise(i, 63) % 4, FE_FERN, FE_FERN_SH)  # One fern
    return img  # Give the picture back


THEMES = {  # One entry per Plateaus Easy theme (D-072): its six layers and its ground tiles for the preview
    "PineapplePlateau": ((pp_sky, pp_clouds, pp_far, pp_mid, pp_rows, pp_near), "pineapple"),  # Bukidnon pineapple plateau
    "LakePlateau": ((lp_sky, lp_clouds, lp_far, lp_mid, lp_shore, lp_near), "lake"),  # Lake Lanao
    "FallsEdge": ((fe_sky, fe_clouds, fe_far, fe_mid, fe_forest, fe_near), "falls"),  # Maria Cristina Falls
}


def ground_name(ground):
    """The ground tile name for the preview; plain grass when the theme's own tile is not made yet."""
    return ground if os.path.exists(os.path.join(nb.SPRITES, ground + "_top.png")) else "grass"  # Fall back to grass


def main():
    """Draws and saves every set and the preview row."""
    shots = []  # One preview screen per theme
    for theme, (makers, ground) in THEMES.items():  # Each theme
        out = os.path.join(nb.BACKDROPS, theme)  # Its folder
        os.makedirs(out, exist_ok=True)  # Make it if missing
        layers = [make() for make in makers]  # Draw L0 to L5
        for i, img in enumerate(layers):  # Save each one
            img.save(os.path.join(out, "L%d.png" % i))  # L0.png ... L5.png
        shots.append(nb.preview_screen(layers, ground_name(ground)))  # Preview screen
        print("Backdrop written to", out)  # Tell where it went
    sheet = Image.new("RGBA", (3 * 480 + 2 * 8, 270), mm.OUTLINE + (255,))  # One row of three screens with dark gaps
    for i, shot in enumerate(shots):  # Place each screen
        sheet.paste(shot, (i * 488, 0))  # 8-pixel gaps
    os.makedirs(os.path.dirname(PREVIEW), exist_ok=True)  # Make the folder
    sheet.save(PREVIEW)  # Normal size, to keep the file small
    print("Preview written to", PREVIEW)  # Tell where it went


if __name__ == "__main__":
    main()  # Run when started directly
