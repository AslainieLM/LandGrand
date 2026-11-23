"""Draws the moving background layers for the five Plateaus Normal themes (D-073).

Run from the Unity project folder:  python Tools/Art/make_plateaus_normal_backdrops.py
It writes L0.png to L5.png (960x270 each) to Assets/Art/Level/Sprites/Backdrop/<Theme>/ for
TableTop (Table Mountain), MesaValley (Monument Valley), CanyonRim (Grand Canyon),
TepuiTop (Mount Roraima) and SaltFlat (Salar de Uyuni),
plus a preview at .scratch/plateaus-normal/backdrop-preview.png.

Each set keeps the six layers and the scrolling of the Plateaus Easy backdrops:
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
import make_normal_backdrops as nb  # Gradient, sun, clouds, crowns, rolling lines and the preview
import make_plateaus_easy_backdrops as pe  # Flat-topped plateau shapes and round ranges

W, H = mb.W, mb.H  # 960x270 layers
mix, put, column, dither, jag = mb.mix, mb.put, mb.column, mb.dither, mb.jag  # Shared drawing helpers
noise = lm.noise  # Fixed noise from 0 to 99, the same on every run
PREVIEW = os.path.join(mb.ROOT, ".scratch", "plateaus-normal", "backdrop-preview.png")  # Where the preview goes
SHIRT = (63, 143, 134)  # Palette "Shirt", a blue-green
SHIRT_SH = (45, 107, 100)  # Palette "Shirt shade"
PANTS = (92, 74, 61)  # Palette "Pants", a grey-brown
BOOTS = (61, 42, 30)  # Palette "Boots", a dark brown
HAMMER = lm.HAMMER  # Palette "Hammer head", a blue-grey


def line(img, x0, y0, x1, y1, color):
    """A thin straight line from one point to another, one pixel wide."""
    steps = max(abs(x1 - x0), abs(y1 - y0), 1)  # One step per pixel along the longer way
    for i in range(steps + 1):  # Each step
        put(img, int(round(x0 + (x1 - x0) * i / steps)), int(round(y0 + (y1 - y0) * i / steps)), color)  # Paint the point


def cone_row(x, cx, top, slope, power=1.0, half=None):
    """The top row of a cone or dome at column x. With half set, power shapes the sides; without it, the sides are straight."""
    off = abs(mb.wrap_offset(x, cx))  # Distance from the middle, the short way round
    if half is None:  # A straight-sided cone
        return top + off * slope  # Rows down per pixel across
    return top + slope * half * (off / half) ** power  # A rounder dome


def blob(img, cx, cy, rx, ry, color, fill=1.0):
    """A soft oval: solid in the middle, dotted toward the edge when fill is below 1."""
    for y in range(cy - ry, cy + ry + 1):  # Rows of the oval
        for x in range(cx - rx, cx + rx + 1):  # Columns of the oval
            d = ((x - cx) / max(1, rx)) ** 2 + ((y - cy) / max(1, ry)) ** 2  # 0 in the middle, 1 at the edge
            if d <= 1 and (fill >= 1 or dither(x, y, fill * (1.3 - d))):  # Inside, and dotted at the edge
                put(img, x, y, color)  # Paint it


def tile_step(size):
    """The next spacing from size up that divides 960, so a repeated row of things tiles."""
    return min(d for d in range(max(1, size), W + 1) if W % d == 0)  # Smallest divisor not below size


# ---------- TableTop: Table Mountain, Cape Town. A long flat grey sandstone top under its white tablecloth, the sea, a far city, fynbos ----------

TT_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.2)), (90, lm.SKY), (150, mm.SKY_LIGHT)]  # Clear windy blue sky
TT_ROCK = mix(mm.MASK_SH, lm.HAZE, 0.35)  # Grey sandstone cliff, softened by distance
TT_ROCK_LT = mix(mm.MASK, lm.HAZE, 0.4)  # Sunny edges of the rock buttresses
TT_ROCK_SH = mix(mm.MASK_SH, lm.MOUNTAIN_SH, 0.35)  # Shaded sides and deep cracks
TT_BAND = mix(mm.VEST, lm.HAZE, 0.5)  # Pale flat sandstone layers
TT_SLOPE = mix(mm.HILLS_SH, lm.HAZE, 0.5)  # Grey-green fynbos on the lower slopes
TT_SLOPE_SH = mix(mm.LEAF, lm.HAZE_SH, 0.5)  # Shaded slopes and ravines
TT_CLOTH = mm.WHITE  # The tablecloth cloud
TT_CLOTH_SH = mix(mm.SKY_LIGHT, lm.HAZE, 0.35)  # Its shaded folds
TT_CABLE = mix(mm.STRAP, lm.HAZE, 0.45)  # The thin cable line
TT_CAR = mix(mm.SCARF, lm.HAZE, 0.3)  # The tiny cable car
TT_WALL = [mix(mm.WHITE, lm.HAZE, 0.4), mix(mm.MASK, lm.HAZE, 0.35), mix(mm.VEST, lm.HAZE, 0.45), mix(lm.SNOW, lm.HAZE, 0.25)]  # Far city walls, pale with haze
TT_ROOF = mix(mm.PLATEAU, lm.HAZE, 0.5)  # Red city roofs
TT_WINDOW = mix(lm.MOUNTAIN, lm.HAZE, 0.35)  # Windows
TT_SEA_FAR = mix(lm.LENS, lm.HAZE, 0.5)  # Sea near the far shore, pale
TT_SEA = mix(lm.LENS, lm.MOUNTAIN, 0.35)  # Deeper blue sea
TT_SURF = mm.WHITE  # Surf and wave crests
TT_SHORE = mix(mm.VEST, lm.HAZE, 0.35)  # Far sandy shore under the city
TT_HULL = mix(mm.STRAP, lm.HAZE, 0.4)  # Far ship hulls
TT_SAIL = mix(mm.WHITE, lm.HAZE, 0.2)  # Small white sails
TT_DUNE = mix(mm.VEST, mm.MASK, 0.5)  # Pale sand dunes on the near shore
TT_DUNE_LIT = mix(mm.VEST, mm.WHITE, 0.4)  # Their sunny top line
TT_BUSH = mix(mm.HILLS_SH, mm.MASK_SH, 0.45)  # Grey-green fynbos shrubs
TT_BUSH_SH = mix(mm.LEAF, mm.STRAP, 0.35)  # Their shade
TT_PROTEA = mix(mm.SCARF, mm.WHITE, 0.45)  # Pink protea flowers
TT_PROTEA_SH = mix(mm.SCARF, mm.SCARF_SH, 0.5)  # Their darker middles
TT_RESTIO = mix(mm.VEST_SH, mm.GROUND, 0.5)  # Brown reed-like restio tufts
TT_HEATH = mix(mm.SCARF, mm.HELMET, 0.4)  # Tiny heath flowers
TT_NEAR = mix(mm.LEAF, mm.STRAP, 0.3)  # Near dark fynbos floor
TT_NEAR_LIT = mix(mm.HILLS_SH, mm.MASK_SH, 0.4)  # Its sunny top line

TT_TABLE = (290, 175, 70)  # The table: middle x, half width of the flat top, top row
TT_FOOT = 140  # Row where the slopes meet the city


def tt_sky():
    """L0: a clear windy blue sky with a bright sun."""
    img = nb.gradient(TT_SKY)  # Blue fading to pale near the sea
    nb.sun(img, 410, 32, 8, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4))  # Bright sun
    return img  # Give the picture back


def tt_clouds():
    """L1: a few small fast clouds blown by the south-east wind."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((90, 26, 70), (560, 40, 60), (760, 20, 90), (910, 48, 40)):  # Cloud places and widths, clear above the table
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # One small cloud
    return img  # Give the picture back


def tt_outline():
    """The skyline of the mountain: the long flat table, Devil's Peak on the left, Lion's Head and Signal Hill on the right.
    Returns, for each x, the top row and which part owns it."""
    tops, owners = [], []  # Top row and owning part for each x
    cx, half, top = TT_TABLE  # The table
    for x in range(W):  # Each column
        off = mb.wrap_offset(x, cx)  # Distance from the table middle
        d = abs(off) - half  # How far past the edge of the flat top
        parts = [  # Each part: its top row here, its name and its middle x
            (top + jag(x, 3, 1) if d <= 0 else top + d * 2.6 + jag(x, 3, 2), "table", cx),  # Flat top, steep sides
            (cone_row(x, 40, 56, 0.95) + jag(x, 4, 2), "devil", 40),  # Devil's Peak, a sharp peak beside the table
            (cone_row(x, 640, 96, 0.75, 1.5, 60) + jag(x, 5, 1), "lion", 640),  # Lion's Head, a round rocky dome
            (cone_row(x, 750, 124, 0.25, 2.0, 90), "signal", 750),  # Signal Hill, a long low ridge
            (TT_FOOT - 8 + jag(x, 6, 2), "foot", x),  # Low ground near the sea
        ]  # All the parts
        y, name, mid = min(parts)  # The highest part owns this column
        tops.append(int(y))  # Whole rows only
        owners.append((name, mb.wrap_offset(x, mid)))  # Remember the owner and the distance from its middle
    return tops, owners  # Give the outline back


def tt_far():
    """L2: Table Mountain with its grey cliffs and pale rock layers, the white tablecloth spilling over the top,
    a thin cable line with a tiny car, and the far city along its foot."""
    img = mb.new_layer()  # Empty layer
    tops, owners = tt_outline()  # The skyline
    cx, half, top = TT_TABLE  # The table
    for x in range(W):  # Each column
        name, off = owners[x]  # Which part owns the column
        cliff_to = {"table": 108, "devil": 84, "lion": 104}.get(name, 0) + jag(x, 7, 3)  # Bare rock above this row, fynbos below
        crack = noise(x, 8) % 9 == 0 and noise(x - 1, 8) % 9 != 0  # Some single columns hold a deep crack between buttresses
        crack_to = tops[x] + 8 + noise(x, 29) % 34  # How far down the crack runs
        lit = noise(x + 1, 8) % 9 == 0  # The column just left of a crack catches the sun
        for y in range(tops[x], H):  # From the skyline down
            if y < cliff_to:  # Bare grey rock
                c = TT_BAND if (y - top) % 11 == 5 and noise(x // 13 % 74, y) % 4 > 0 else TT_ROCK  # Flat pale rock layers, broken in places
                if crack and y < crack_to:  # A deep crack
                    c = TT_ROCK_SH  # Crack pixel
                elif lit and y < crack_to:  # The sunny edge beside it
                    c = TT_ROCK_LT  # Light pixel
                if name == "table" and abs(off) > half:  # The steep ends of the table
                    c = TT_ROCK_SH if off < 0 else TT_ROCK_LT  # Shaded left end, sunny right end
                if name in ("devil", "lion") and off < (y - tops[x]) * 0.2 - 2:  # The shaded left face of a peak
                    c = TT_ROCK_SH  # Shaded rock
            else:  # The fynbos slopes below the cliffs
                ravine = noise(x // 4 % 240, 9) % 7 == 0 and y < cliff_to + 6 + noise(x // 4 % 240, 30) % 20  # A few dark ravines down the slope
                c = TT_SLOPE_SH if ravine or (off < 0 and name in ("devil", "lion", "signal")) else TT_SLOPE  # Shade and ravines
                if noise((x * 5 + y * 3) % 960, 31) % 13 == 0:  # Scattered dots of bush
                    c = TT_SLOPE_SH if c == TT_SLOPE else TT_SLOPE  # Bush pixel
            put(img, x, y, c)  # Paint it
    for x in range(cx - half + 20, cx + half - 30):  # The tablecloth lying on the flat top
        lump = 4 + noise(x // 5 % 192, 10) % 3 + int(2 * math.sin(x / 11))  # Its soft lumpy top
        for y in range(top - lump, top + 2):  # From its top down to the rock
            put(img, x, y, TT_CLOTH)  # Cloud pixel
        drop = 10 + int(9 * (1 + math.sin(x / 17)) + 5 * math.sin(x / 7)) + noise(x // 4 % 240, 11) % 5  # How far this part spills down the face, in soft tongues
        for y in range(top + 2, top + 2 + drop):  # The cloud spilling down
            t = (y - top - 2) / drop  # 0 at the lip, 1 at the end
            if dither(x, y, 1.0 - t):  # Thinner going down, so it melts away
                put(img, x, y, TT_CLOTH if t < 0.4 else TT_CLOTH_SH)  # Cloud pixel
    lx, ly = cx + half - 18, top - 3  # The top cable station, at the right end of the table
    for x in range(lx - 2, lx + 3):  # A small grey station
        column(img, x, ly, TT_CABLE, top + 1)  # Station pixel
    bx, by = cx + half - 70, 128  # The lower cable station on the slope
    line(img, bx, by, lx, ly + 1, TT_CABLE)  # The cable line
    for x in range(bx - 2, bx + 3):  # The lower station
        column(img, x, by - 2, TT_CABLE, by + 1)  # Station pixel
    mx, my = bx + (lx - bx) * 3 // 5, by + (ly - by) * 3 // 5  # A car three fifths of the way up
    for x in range(mx - 1, mx + 2):  # The car body
        column(img, x, my + 2, TT_CAR, my + 4)  # Car pixel
    put(img, mx, my + 1, TT_CABLE)  # The hanger from the line
    tt_city(img)  # The far city along the foot
    return img  # Give the picture back


def tt_city(img):
    """The far city along the foot of the mountain: small pale blocks, taller in the middle, with red roofs."""
    x = 230  # Start of the city
    i = 0  # Building count
    while x < 860:  # Along the foot
        w = 3 + noise(i, 12) % 5  # Building width
        tall = 14 if 450 < x < 560 else 6  # Tall towers in the city middle
        h = 3 + noise(i, 13) % tall  # Building height
        c = TT_WALL[noise(i, 14) % len(TT_WALL)]  # Wall colour
        for bx in range(x, x + w):  # Each column of the building
            column(img, bx, TT_FOOT - h, c, TT_FOOT + 6)  # Wall
            if h < 6 and noise(i, 15) % 2 == 0:  # Low houses get red roofs
                put(img, bx, TT_FOOT - h, TT_ROOF)  # Roof pixel
        for wy in range(TT_FOOT - h + 2, TT_FOOT + 4, 3):  # Rows of windows
            for wx in range(x + 1, x + w - 1, 2):  # Windows across
                put(img, wx, wy, TT_WINDOW)  # Window pixel
        x += w + noise(i, 16) % 3  # Next building, sometimes with a gap
        i += 1  # Count it
    for x in range(W):  # The pale sandy shore under the city
        column(img, x, TT_FOOT + 6, TT_SHORE)  # Shore pixel


def tt_mid():
    """L3: the blue bay between the viewer and the mountain, with wave lines, surf on the far shore and small ships."""
    img = mb.new_layer()  # Empty layer
    sea = 150  # Row of the far shore water line
    for y in range(sea, H):  # The sea
        t = min(1.0, (y - sea) / 30)  # Pale far away, deeper nearer
        for x in range(W):  # Each pixel
            c = TT_SEA if dither(x, y, t) else TT_SEA_FAR  # Dotted blend
            k = (y - sea) // 5  # Which band of waves
            if (y - sea) % 5 == 2 and (x + noise(k, 32) * 7) % 48 < 3 + k and noise(x // 48 % 20, k) % 3 > 0:  # Short scattered wave crests, longer nearer
                c = TT_SURF  # Crest pixel
            put(img, x, y, c)  # Paint it
    for x in range(W):  # Surf along the far shore
        if noise(x // 2 % 480, 17) % 3 > 0:  # Broken line of foam
            put(img, x, sea, TT_SURF)  # Foam pixel
    for x, y, big in ((120, 162, 0), (250, 170, 1), (430, 158, 0), (690, 166, 1), (880, 172, 0)):  # Small far ships and yachts
        if big:  # A small ship
            for dx in range(-5, 6):  # Hull
                put(img, x + dx, y, TT_HULL)  # Hull pixel
            for dx in range(-2, 3):  # Cabin
                put(img, x + dx, y - 1, TT_SAIL)  # Cabin pixel
        else:  # A yacht
            for dx in range(-2, 3):  # Hull
                put(img, x + dx, y, TT_HULL)  # Hull pixel
            for dy in range(1, 6):  # Triangle sail
                for dx in range(0, (6 - dy) // 2 + 1):  # Wider at the bottom
                    put(img, x + dx, y - dy, TT_SAIL)  # Sail pixel
    return img  # Give the picture back


def fynbos_bush(img, x, base, r, lit, shade):
    """A low rounded fynbos shrub: a flat dome, sunny on the right."""
    for y in range(base - r, base + 1):  # Rows of the shrub
        for dx in range(-r - 2, r + 3):  # Columns, wider than tall
            if (dx / (r + 2)) ** 2 + ((y - base) / r) ** 2 <= 1:  # Inside the flat dome
                put(img, x + dx, y, lit if dx > 0 and y < base - 1 else shade)  # Light from the right


def restio(img, x, base, h, color):
    """A restio tuft: a few thin upright reeds that fan out a little."""
    for lean in (-1, 0, 1):  # Three stems
        for t in range(h - abs(lean) * 2):  # Up each stem
            put(img, x + (lean * t) // 4, base - t, color)  # Stem pixel


def protea(img, x, base, h, leaf, leaf_sh, flower, middle):
    """A protea shrub: a short leafy stem with a big round pink flower head."""
    fynbos_bush(img, x, base, h // 2, leaf, leaf_sh)  # Leafy shrub
    fy = base - h // 2 - 2  # Flower height
    blob(img, x, fy, 2, 2, flower)  # The pink flower head
    put(img, x, fy, middle)  # Its darker middle
    put(img, x - 1, fy - 2, flower)  # Petal tips
    put(img, x + 1, fy - 2, flower)  # Petal tips


def tt_band():
    """L4: pale sand dunes on the near shore, dotted with low fynbos shrubs and restio tufts."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(198, 3, 3, 18)  # The dune line
    nb.fill_below(img, tops, TT_DUNE_LIT, TT_DUNE)  # Sand with a sunny top line
    for i, x in enumerate(range(0, W, 6)):  # Shrubs along the dunes
        cx = (x + noise(i, 19) % 4) % W  # A little varied place
        k = noise(i, 20) % 6  # What grows here
        if k < 3:  # A low shrub
            fynbos_bush(img, cx, tops[cx] + 3, 2 + noise(i, 21) % 3, TT_BUSH, TT_BUSH_SH)  # One shrub
        elif k == 3:  # A restio tuft
            restio(img, cx, tops[cx] + 2, 6 + noise(i, 22) % 4, TT_RESTIO)  # One tuft
    return img  # Give the picture back


def tt_near():
    """L5: the near fynbos bank just behind the trail, with proteas, restios and tiny heath flowers."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(219, 2, 3, 23)  # A gently wavy line near the trail
    nb.fill_below(img, tops, TT_NEAR_LIT, TT_NEAR)  # Fynbos floor
    for i, x in enumerate(range(10, W, 40)):  # Proteas along the bank
        cx = (x + noise(i, 24) % 10) % W  # A little varied place
        protea(img, cx, tops[cx] + 2, 10 + noise(i, 25) % 4, TT_BUSH, TT_BUSH_SH, TT_PROTEA, TT_PROTEA_SH)  # One protea
    for i, x in enumerate(range(28, W, 40)):  # Restio tufts between them
        cx = (x + noise(i, 26) % 8) % W  # A little varied place
        restio(img, cx, tops[cx] + 1, 10 + noise(i, 27) % 5, TT_RESTIO)  # One tuft
    for x in range(W):  # Tiny heath flowers on the top line
        if noise(x % 480, 28) % 11 == 0:  # A few, scattered
            put(img, x, tops[x] - 1, TT_HEATH)  # Flower pixel
    return img  # Give the picture back


# ---------- MesaValley: Monument Valley. Red-orange mesas and tall thin buttes on a flat sandy plain, a wide blue sky, sagebrush ----------

MV_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.35)), (80, lm.SKY), (140, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.3))]  # Deep wide blue, warm near the ground
MV_CLOUD_SH = mix(mm.SKY_LIGHT, lm.HAZE, 0.4)  # Cloud undersides
MV_FAR = mix(mm.PLATEAU, lm.HAZE, 0.55)  # Far mesas, red softened to lilac by distance
MV_FAR_SH = mix(mm.PLATEAU_SH, lm.HAZE_SH, 0.5)  # Their shaded sides
MV_FAR_CAP = mix(mm.PLATEAU, lm.HAZE, 0.45)  # Their sunny top lines
MV_FAR_PLAIN = mix(mm.VEST, lm.HAZE, 0.5)  # Far plain
MV_ROCK = mix(mm.PLATEAU, mm.SCARF, 0.35)  # Red-orange butte rock
MV_ROCK_LT = mix(mm.PLATEAU, mm.HELMET, 0.3)  # Sunny right edges
MV_ROCK_SH = mix(mm.PLATEAU_SH, lm.MOUNTAIN, 0.3)  # Shaded left faces, a little purple
MV_STREAK = mix(mm.PLATEAU_SH, mm.BACKPACK_SH, 0.3)  # Dark streaks of desert varnish
MV_CAPROCK = mix(mm.PLATEAU_SH, mm.VEST_SH, 0.5)  # The harder brown cap on top
MV_TALUS = mix(mm.PLATEAU, mm.VEST, 0.45)  # Sloping rubble at the foot of each butte
MV_TALUS_SH = mix(mm.PLATEAU_SH, mm.VEST_SH, 0.4)  # Its shaded side
MV_TALUS_BAND = mix(mm.PLATEAU_SH, mm.PLATEAU, 0.5)  # Red bands in the rubble slope
MV_PLAIN = mix(mm.VEST, mm.PLATEAU, 0.35)  # The flat sandy plain
MV_PLAIN_LIT = mix(mm.VEST, mm.HEADLAMP, 0.3)  # Its sunny far edge
MV_PLAIN_SH = mix(mm.PLATEAU, mm.GROUND, 0.5)  # Darker sand streaks
MV_SAND = mix(mm.PLATEAU, mm.VEST, 0.6)  # Near red sand
MV_SAND_LIT = mix(mm.VEST, mm.HEADLAMP, 0.4)  # Its sunny top line
MV_SAND_SH = mix(mm.PLATEAU, mm.PLATEAU_SH, 0.5)  # Ripples in the sand
MV_SAGE = mix(mm.MASK_SH, mm.HILLS_SH, 0.5)  # Silver-green sagebrush
MV_SAGE_LT = mix(mm.MASK, lm.PLAINS_SH, 0.5)  # Sunny sagebrush tips
MV_SAGE_SH = mix(mm.MASK_SH, mm.LEAF, 0.5)  # Shaded sagebrush
MV_TWIG = mix(mm.GROUND_SH, mm.MASK_SH, 0.5)  # Grey twigs
MV_NEAR = mix(mm.PLATEAU_SH, mm.GROUND_SH, 0.5)  # Near red earth bank
MV_NEAR_LIT = mix(mm.PLATEAU, mm.VEST, 0.5)  # Its sunny top line

MV_BUTTES = [  # Middle x, half width, top row, row where the rubble slope starts, half width at the foot of the rubble
    (120, 20, 74, 128, 52),  # West Mitten
    (146, 3, 96, 128, 0),  # Its thin thumb
    (300, 19, 78, 130, 50),  # East Mitten
    (276, 3, 100, 130, 0),  # Its thumb, on the other side
    (430, 12, 92, 134, 36),  # Merrick Butte, smaller and farther back
    (660, 92, 100, 136, 124),  # A long flat mesa
    (842, 3, 84, 140, 16),  # A tall thin spire
    (856, 2, 96, 140, 0),  # A smaller spire beside it
    (870, 4, 90, 140, 16),  # A third spire
]  # The buttes
MV_FOOT = 166  # Row where the rubble slopes meet the plain


def mv_sky():
    """L0: a wide deep blue desert sky with a strong sun."""
    img = nb.gradient(MV_SKY)  # Deep blue fading to warm pale near the ground
    nb.sun(img, 400, 30, 8, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.5))  # Strong sun
    return img  # Give the picture back


def mv_clouds():
    """L1: a few round fair-weather clouds floating in the wide sky."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((200, 34, 70), (520, 20, 90), (760, 46, 60)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, MV_CLOUD_SH)  # The flat base
        for k in range(3):  # Round puffs on top
            blob(img, cx - w // 4 + k * w // 4, cy - 5 - (k % 2) * 3, w // 8, 5 + (k % 2) * 2, mm.WHITE)  # One puff
    return img  # Give the picture back


def mv_far():
    """L2: long low far mesas on the horizon, red turned lilac by the dry distance."""
    img = mb.new_layer()  # Empty layer
    plain = 146  # Row of the far plain
    for y in range(plain, H):  # The far plain
        for x in range(W):  # Each pixel
            put(img, x, y, MV_FAR_PLAIN)  # Plain pixel
    mesas = [(60, 120, 122, 1.6), (380, 60, 128, 1.8), (560, 150, 118, 1.5), (870, 40, 130, 2.0)]  # Middle x, half width, top row, steepness
    pe.mesa_layer(img, mesas, 71, (MV_FAR_CAP, MV_FAR), 2, MV_FAR, MV_FAR_SH, MV_FAR, MV_FAR, MV_FAR_SH, plain)  # Plain far mesas, no green cap
    return img  # Give the picture back


def butte_tops(buttes):
    """The outline of a row of buttes: a sheer rock tower on top of a sloping rubble cone.
    Returns, for each x, the top row, the butte that owns it, and whether the column is rock tower or rubble."""
    tops, owners, towers = [], [], []  # Top row, owner and kind for each x
    for x in range(W):  # Each column
        best, owner, tower = H, None, False  # Lowest row number wins
        for b in buttes:  # Each butte
            cx, half, top, neck, foot_half = b  # Its shape
            off = abs(mb.wrap_offset(x, cx))  # Distance from its middle
            wide = half + (neck - top) // 14  # The tower gets one pixel wider every 14 rows down
            if off <= wide:  # The sheer tower
                y, t = top + max(0, off - half) * 14 + (1 if off == half else 0), True  # Flat top, a rounded corner, slightly sloping walls
            elif foot_half > wide and off <= foot_half:  # The rubble slope around it
                y, t = neck + (MV_FOOT - neck) * ((off - wide) / (foot_half - wide)) ** 0.8, False  # Steep near the tower, flatter outward
            else:  # Too far away
                continue  # Next butte
            if y < best:  # Higher than the others
                best, owner, tower = y, b, t  # This butte owns the column
        tops.append(int(best))  # Whole rows only
        owners.append(owner)  # Remember the owner
        towers.append(tower)  # Remember the kind
    return tops, owners, towers  # Give the outline back


def mv_mid():
    """L3: the Mittens, Merrick Butte, a long mesa and thin spires: sheer red-orange towers on rubble slopes, on the flat plain."""
    img = mb.new_layer()  # Empty layer
    for y in range(MV_FOOT, H):  # The flat plain in front
        for x in range(W):  # Each pixel
            c = MV_PLAIN_LIT if y == MV_FOOT else MV_PLAIN  # A sunny far edge, then sand
            if (y - MV_FOOT) % 7 == 4 and noise(x // 16 % 60, y) % 3 == 0:  # Long faint streaks of darker sand
                c = MV_PLAIN_SH  # Streak pixel
            put(img, x, y, c)  # Paint it
    tops, owners, towers = butte_tops(MV_BUTTES)  # The outline
    for x in range(W):  # Each column
        if owners[x] is None:  # Open sky and plain here
            continue  # Next column
        cx, half, top, neck, foot_half = owners[x]  # The butte that owns it
        off = mb.wrap_offset(x, cx)  # Signed distance from its middle
        streak = noise(x, 72) % 5 == 0  # Some columns carry a dark varnish streak
        streak_to = top + 6 + noise(x, 73) % 30  # How far the streak runs down
        shade_to = -half + min(10, max(1, half * 2 // 5))  # Left of this the rock is in shade
        for y in range(tops[x], MV_FOOT + 1):  # From the top down to the plain
            if towers[x] and y < neck + 2:  # The sheer rock tower
                if y < top + 3:  # The harder cap on top
                    c = MV_CAPROCK  # Cap pixel
                elif off < shade_to:  # The shaded left part of the tower
                    c = MV_ROCK_SH  # Shade pixel
                elif off == half or off == half - 1:  # The sunny right edge
                    c = MV_ROCK_LT  # Light pixel
                else:  # The tower face
                    c = MV_STREAK if streak and y < streak_to else MV_ROCK  # Rock with dark streaks
            else:  # The rubble slope
                c = MV_TALUS_SH if off < shade_to else MV_TALUS  # Shaded left side, in line with the shade on the tower
                if (y - neck) % 6 == 3 and noise(x // 3 % 320, y) % 4 > 0:  # Bands of red rock in the slope
                    c = MV_TALUS_BAND  # Band pixel
            put(img, x, y, c)  # Paint it
    return img  # Give the picture back


def sagebrush(img, x, base, r, lit, mid, shade, twig):
    """A sagebrush: a round, ragged silver-green bush on grey twigs."""
    for y in range(base - 2 * r, base - 1):  # Rows of the bush
        for dx in range(-r - 1, r + 2):  # Columns of the bush
            d = (dx / (r + 1)) ** 2 + ((y - base + r + 1) / r) ** 2  # 0 in the middle, 1 at the edge
            if d <= 1 and noise((x + dx) % 960, y) % 5 > 0:  # Inside, with ragged holes
                put(img, x + dx, y, lit if dx > 0 and y < base - r else shade if dx < 0 else mid)  # Light from the upper right
    for dx in (-1, 0, 1):  # Twigs under the bush
        put(img, x + dx, base - 1, twig)  # Twig pixel
        put(img, x + dx * 2, base, twig)  # Twig spreading at the root


def mv_band():
    """L4: the open red sand plain with gentle ripples and low sagebrush dotted across it."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(196, 2, 2, 74)  # A gentle rise
    nb.fill_below(img, tops, MV_SAND_LIT, MV_SAND)  # Red sand with a sunny top line
    for x in range(W):  # Ripples in the sand
        for y in range(tops[x] + 3 + (H - tops[x] - 3) % 5, H, 5):  # Every five rows, on the same rows in every column
            if (x + 3 * y) % 24 < 9:  # Short slanted ripples
                put(img, x, y, MV_SAND_SH)  # Ripple pixel
    for i, x in enumerate(range(0, W, 12)):  # Small bushes
        cx = (x + noise(i, 75) % 8) % W  # A little varied place
        if noise(i, 76) % 3 > 0:  # Two in three places
            sagebrush(img, cx, tops[cx] + 3, 2 + noise(i, 77) % 2, MV_SAGE_LT, MV_SAGE, MV_SAGE_SH, MV_TWIG)  # One small bush
    return img  # Give the picture back


def mv_near():
    """L5: the near red earth bank just behind the trail, with big sagebrush."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(219, 2, 3, 78)  # A gently wavy line near the trail
    nb.fill_below(img, tops, MV_NEAR_LIT, MV_NEAR)  # Red earth bank
    for i, x in enumerate(range(12, W, 30)):  # Sagebrush along the bank
        cx = (x + noise(i, 79) % 12) % W  # A little varied place
        sagebrush(img, cx, tops[cx] + 2, 4 + noise(i, 80) % 3, MV_SAGE_LT, MV_SAGE, MV_SAGE_SH, MV_TWIG)  # One big bush
    return img  # Give the picture back


# ---------- CanyonRim: the Grand Canyon. A vast canyon of flat red, buff and grey rock layers, far rims, a thin river, pinyon pines ----------

CR_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.25)), (80, lm.SKY), (130, mix(mm.SKY_LIGHT, mm.WHITE, 0.3))]  # Clear blue, pale at the far rim
CR_FAR_RIM = mix(mm.LEAF, lm.HAZE, 0.6)  # Forest on the far North Rim, blue-green with distance
CR_FAR_BANDS = [mix(mm.VEST, lm.HAZE, 0.6), mix(mm.MASK, lm.HAZE, 0.55), mix(mm.PLATEAU, lm.HAZE, 0.6), mix(mm.PLATEAU_SH, lm.HAZE, 0.6), mix(mm.PLATEAU, lm.HAZE, 0.5), mix(mm.MASK_SH, lm.HAZE, 0.6)]  # Far rock layers, top to bottom
CR_FAR_SH = mix(lm.MOUNTAIN, lm.HAZE, 0.5)  # Shaded far walls, blue
CR_HAZE = mix(mm.SKY_LIGHT, lm.HAZE, 0.5)  # Haze filling the deep canyon
CR_BANDS = [  # Near rock layers, top to bottom: (sunny colour, shaded colour, thickness in rows)
    (mix(mm.VEST, mm.MASK, 0.5), mix(mm.VEST_SH, lm.MOUNTAIN, 0.4), 6),  # Pale buff limestone at the rim
    (mix(mm.VEST, mm.PLATEAU, 0.35), mix(mm.VEST_SH, mm.PLATEAU_SH, 0.5), 5),  # Tan layer
    (mix(mm.VEST, mm.HEADLAMP, 0.3), mix(mm.VEST_SH, lm.HAZE_SH, 0.4), 7),  # Pale cream sandstone
    (mix(mm.PLATEAU, mm.SCARF_SH, 0.35), mix(mm.PLATEAU_SH, lm.MOUNTAIN_SH, 0.35), 9),  # Red slope shale
    (mix(mm.PLATEAU, mm.PLATEAU_SH, 0.4), mix(mm.PLATEAU_SH, lm.MOUNTAIN_SH, 0.5), 10),  # Red ledges
    (mix(mm.SCARF_SH, mm.PLATEAU, 0.5), mix(mm.SCARF_SH, lm.MOUNTAIN_SH, 0.5), 12),  # Deep red cliff
    (mix(mm.MASK_SH, mm.HILLS_SH, 0.35), mix(mm.MASK_SH, lm.MOUNTAIN_SH, 0.5), 9),  # Grey-green shale platform
    (mix(mm.STRAP, PANTS, 0.5), mix(mm.STRAP, mm.OUTLINE, 0.4), 40),  # Dark old rock of the inner gorge
]  # The rock layers
CR_GORGE = mix(PANTS, mm.MASK_SH, 0.45)  # Grey-brown old rock of the inner gorge
CR_GORGE_SH = mix(PANTS, mm.STRAP, 0.5)  # Its shade and cracks
CR_RIVER = mix(lm.LENS, SHIRT, 0.4)  # The green-blue river far below
CR_GLINT = mix(lm.LENS, mm.WHITE, 0.6)  # Sun glinting on it
CR_ROCK = mix(mm.VEST, mm.MASK, 0.5)  # Pale limestone of our rim
CR_ROCK_LIT = mix(mm.MASK, mm.WHITE, 0.4)  # Its sunny top line
CR_ROCK_SH = mix(mm.VEST_SH, mm.MASK_SH, 0.5)  # Cracks and shade in the limestone
CR_PINE = mix(mm.LEAF, SHIRT_SH, 0.35)  # Pinyon pine needles, dark blue-green
CR_PINE_LT = mix(mm.LEAF, mm.HILLS_SH, 0.5)  # Sunny needles
CR_PINE_SH = mix(mm.LEAF, mm.OUTLINE, 0.35)  # Shaded needles
CR_TRUNK = mix(PANTS, mm.GROUND_SH, 0.5)  # Twisted grey-brown trunks
CR_NEAR = mix(mm.VEST_SH, mm.GROUND_SH, 0.5)  # Near rocky soil
CR_NEAR_LIT = mix(mm.VEST, mm.MASK_SH, 0.5)  # Its sunny top line
CR_TOP = 112  # Row of the highest rock layer in the temples
CR_FLOOR = 186  # Row of the river at the bottom of the inner gorge


def cr_sky():
    """L0: a clear high desert sky."""
    img = nb.gradient(CR_SKY)  # Blue fading to pale over the far rim
    nb.sun(img, 90, 28, 8, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4))  # Bright sun
    return img  # Give the picture back


def cr_clouds():
    """L1: a few long thin clouds over the canyon."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((260, 30, 110), (540, 52, 80), (820, 24, 130)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mix(mm.SKY_LIGHT, lm.HAZE, 0.3))  # One cloud
    return img  # Give the picture back


def cr_far():
    """L2: the far North Rim, a dead-flat forested skyline, with long walls of pale banded rock and promontories below it."""
    img = mb.new_layer()  # Empty layer
    rim = 98  # Row of the far rim
    for x in range(W):  # The flat far rim, forest on top
        top = rim - (1 if noise(x // 2 % 480, 81) % 3 == 0 else 0)  # Tiny bumps of trees
        for y in range(top, rim + 2):  # The forest line
            put(img, x, y, CR_FAR_RIM)  # Forest pixel
    mesas = [(80, 70, rim + 2, 1.2), (300, 110, rim + 2, 1.0), (560, 60, rim + 2, 1.3), (800, 90, rim + 2, 1.1)]  # Promontories that reach out from the rim
    tops, outs, sides, flats = pe.mesa_tops(mesas, 82)  # Their outline
    for x in range(W):  # Each column
        for y in range(rim + 2, 160):  # From the rim down
            if y < tops[x]:  # A gap between promontories: the far canyon wall behind
                c = CR_FAR_SH if (y - rim) % 7 else CR_FAR_BANDS[0]  # Blue shaded wall with faint layers
            else:  # A promontory
                band = CR_FAR_BANDS[min(len(CR_FAR_BANDS) - 1, (y - rim - 2) // 7)]  # Flat rock layers, the same height everywhere
                c = CR_FAR_SH if outs[x] > 0 and sides[x] < 0 else band  # Shaded left sides of the promontories
            put(img, x, y, c)  # Paint it
    nb.mist_band(img, 140, 20, CR_HAZE)  # Haze filling the deep canyon
    for x in range(W):  # The far canyon floor below the haze
        column(img, x, 160, CR_HAZE)  # Haze pixel
    return img  # Give the picture back


def band_at(y, top):
    """The near rock layer at row y, for layers that start at row top; the layers are flat, so every temple shows the same ones."""
    d = y - top  # Rows below the top layer
    for lit, shade, thick in CR_BANDS:  # Each layer, top down
        if d < thick:  # Inside this layer
            return lit, shade  # Its colours
        d -= thick  # Go down past it
    return CR_BANDS[-1][0], CR_BANDS[-1][1]  # The deepest layer


CR_TEMPLES = ((150, 1.0, 0), (430, 0.8, 12), (690, 1.2, 0), (900, 0.6, 18))  # Rock temples: middle x, size and how much lower they stand
CR_STEPS = ((90, 140), (62, 128), (40, 118), (22, CR_TOP))  # The steps of a temple: half width and top row, widest first


def temple_half(size, drop, y):
    """How far a temple reaches out to each side at row y: the widest step whose top is above that row. -1 above the temple."""
    return max([int(half * size + (y - top - drop) * 0.7) for half, top in CR_STEPS if top + drop <= y] + [-1])  # Widest step at this row; each wall slopes out a little going down


def cr_mid():
    """L3: stepped rock temples rising from the canyon, banded red, buff and grey, with the inner gorge and a thin river."""
    img = mb.new_layer()  # Empty layer
    platform = 158  # Row of the grey-green shale platform between the temples
    for x in range(W):  # Each column
        for y in range(116, H):  # From the highest temple top down
            owner = None  # The temple at this pixel, if any
            for cx, size, drop in CR_TEMPLES if y < platform + 4 else ():  # Each temple, down to the platform it stands on
                off = mb.wrap_offset(x, cx)  # Signed distance from its middle
                half = temple_half(size, drop, y)  # How wide it is at this row
                if abs(off) <= half:  # Inside it
                    owner = (off, half, size, drop)  # Remember it
                    break  # Temples do not overlap
            if owner is None:  # No temple here
                if y >= platform:  # The flat shale platform between temples
                    put(img, x, y, CR_BANDS[6][0] if y > platform else CR_BANDS[0][0])  # Grey-green, with a pale top line
                continue  # Next pixel
            off, half, size, drop = owner  # The temple
            lit, shade = band_at(y, CR_TOP)  # Which rock layer
            c = shade if off < -half + max(4, half // 3) else lit  # The left part of every step is in shade
            if temple_half(size, drop, y - 1) < abs(off):  # The top pixel of a step
                c = CR_BANDS[0][0]  # Pale top line
            elif noise(x, 92) % 11 == 0 and temple_half(size, drop, y - 1 - noise(x, 93) % 8) < abs(off) + 0:  # Short dark gullies under each step edge
                c = shade  # Gully pixel
            put(img, x, y, c)  # Paint it
    for x in range(W):  # The inner gorge: a narrow winding cut in the platform, with the river at its bottom
        mid = CR_FLOOR + int(2 * math.sin(2 * math.pi * 4 * x / W))  # The middle of the cut, winding
        g = int(4 + 4 * math.sin(2 * math.pi * 5 * x / W + 1) + 2 * math.sin(2 * math.pi * 11 * x / W))  # How open the cut is here
        for y in range(platform + 1, H):  # The platform rows
            if y > 172 and (y - 172) % 5 == 0 and noise(x // 7 % 137, y) % 3 == 0:  # Faint ledges on the lower platform
                put(img, x, y, CR_BANDS[6][1])  # Ledge pixel
        for y in range(mid - g, mid + 2):  # The cut itself
            put(img, x, y, CR_GORGE_SH if y < mid - g + 2 else CR_GORGE)  # Old dark rock, darker at the lip
        if g > 3:  # Only where the cut is open enough to see the water
            put(img, x, mid, CR_GLINT if noise(x // 3 % 320, 84) % 6 == 0 else CR_RIVER)  # River pixel, with sun glints
    return img  # Give the picture back


def pinyon(img, x, base, h, lit, mid, shade, trunk):
    """A pinyon pine: a short twisted trunk with a wide, lumpy, rounded crown."""
    column(img, x, base - h // 2, trunk, base + 1)  # The trunk
    put(img, x - 1, base - h // 3, trunk)  # A twist in the trunk
    r = max(2, h // 3)  # Size of each lump of the crown
    for dx, dy in ((-r, 0), (r, 0), (0, -r // 2), (-r // 2, -r), (r // 2, -r)):  # Lumps of the crown
        cx, cy = x + dx, base - h // 2 - r + dy  # Middle of this lump
        for y in range(cy - r, cy + r + 1):  # Rows of the lump
            for xx in range(cx - r, cx + r + 1):  # Columns of the lump
                if (xx - cx) ** 2 + (y - cy) ** 2 <= r * r and noise(xx % 960, y) % 6 > 0:  # Inside, a little ragged
                    put(img, xx, y, lit if xx - x > r // 2 and y < cy else shade if xx < x - r // 2 else mid)  # Light from the upper right


def cr_band():
    """L4: our own rim of pale limestone, broken into blocks, with small pinyon pines along it."""
    img = mb.new_layer()  # Empty layer
    tops = [200 + int(3 * math.sin(2 * math.pi * 4 * x / W)) + (noise(x // 6 % 160, 85) % 4) for x in range(W)]  # A blocky rim edge
    for x in range(W):  # Each column
        put(img, x, tops[x], CR_ROCK_LIT)  # Sunny top line
        for y in range(tops[x] + 1, H):  # The limestone
            crack = x % 40 == 0 or (y - tops[x]) % 7 == 0 and noise(x // 8 % 120, y) % 3 == 0  # Joints between the blocks
            put(img, x, y, CR_ROCK_SH if crack else CR_ROCK)  # Rock pixel
    for i, x in enumerate(range(20, W, 64)):  # Small pines along the rim
        cx = (x + noise(i, 86) % 20) % W  # A little varied place
        pinyon(img, cx, tops[cx], 10 + noise(i, 87) % 4, CR_PINE_LT, CR_PINE, CR_PINE_SH, CR_TRUNK)  # One pine
    return img  # Give the picture back


def cr_near():
    """L5: the near rocky soil just behind the trail, with bigger pinyon pines and loose stones."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(219, 2, 3, 88)  # A gently wavy line near the trail
    nb.fill_below(img, tops, CR_NEAR_LIT, CR_NEAR)  # Rocky soil
    for i, x in enumerate(range(50, W, 120)):  # Pines along the bank
        cx = (x + noise(i, 89) % 30) % W  # A little varied place
        pinyon(img, cx, tops[cx] + 1, 18 + noise(i, 90) % 5, CR_PINE_LT, CR_PINE, CR_PINE_SH, CR_TRUNK)  # One bigger pine
    for i, x in enumerate(range(0, W, 16)):  # Loose pale stones on the top line
        if noise(i, 91) % 3 == 0:  # A few
            for dx in range(3):  # A small flat stone
                put(img, x + dx, tops[x] - 1, CR_ROCK)  # Stone pixel
    return img  # Give the picture back


# ---------- TepuiTop: Mount Roraima. Sheer-walled table-top mountains above green jungle and clouds, thin falls, black summit rock with pools ----------

TP_SKY = [(0, mix(lm.SKY, lm.HAZE, 0.35)), (90, mix(mm.SKY_LIGHT, lm.HAZE, 0.3)), (160, mix(mm.SKY_LIGHT, mm.WHITE, 0.5))]  # Soft humid blue
TP_CLOUD = mm.WHITE  # Clouds
TP_CLOUD_SH = mix(mm.SKY_LIGHT, lm.HAZE, 0.5)  # Their grey undersides
TP_FAR = mix(lm.MOUNTAIN, lm.HAZE, 0.6)  # Far tepui walls, blue with distance
TP_FAR_SH = mix(lm.MOUNTAIN, lm.HAZE_SH, 0.35)  # Their shaded walls and streaks
TP_FAR_CAP = mix(mm.LEAF, lm.HAZE, 0.6)  # Their green-grey tops
TP_WALL = mix(mm.MASK_SH, mm.PLATEAU_SH, 0.3)  # The near tepui wall, pink-grey sandstone
TP_WALL_LT = mix(mm.MASK, mm.PLATEAU, 0.3)  # Sunny ribs on the wall
TP_WALL_SH = mix(mm.MASK_SH, lm.MOUNTAIN_SH, 0.45)  # Shaded wall and dark wet streaks
TP_STREAK = mix(mm.STRAP, mm.MASK_SH, 0.5)  # Black streaks of algae down the wall
TP_TOP = mix(mm.STRAP, mm.LEAF, 0.35)  # Dark green-black plants on the flat top
TP_TOP_LIT = mix(mm.MASK_SH, mm.HILLS_SH, 0.5)  # Its sunny top line
TP_FALL = mm.WHITE  # Thin falling water
TP_FALL_SH = mix(lm.LENS, mm.WHITE, 0.5)  # Blue in the falls
TP_JUNGLE = mix(mm.LEAF, mm.HILLS_SH, 0.4)  # Green jungle canopy
TP_JUNGLE_SH = mix(mm.LEAF, SHIRT_SH, 0.5)  # Its shade
TP_MIST = mix(mm.SKY_LIGHT, mm.WHITE, 0.5)  # Low cloud and mist
TP_ROCK = mix(mm.STRAP, mm.MASK_SH, 0.35)  # Black summit rock
TP_ROCK_LT = mix(mm.MASK_SH, HAMMER, 0.5)  # Its sunny edges
TP_ROCK_SH = mix(mm.STRAP, mm.OUTLINE, 0.4)  # Deep cracks
TP_POOL = mix(lm.SKY, mm.SKY_LIGHT, 0.5)  # Pools that hold the sky
TP_POOL_SH = mix(lm.LENS, lm.MOUNTAIN, 0.4)  # The dark rim of each pool
TP_SAND = mix(mm.MASK, mm.HEADLAMP, 0.3)  # Pale quartz sand in the hollows
TP_PLANT = mix(mm.HILLS_SH, mm.SCARF_SH, 0.35)  # Red-green pitcher plants
TP_FLOWER = mm.HELMET  # Tiny yellow flowers
TP_NEAR = mix(mm.STRAP, mm.OUTLINE, 0.2)  # Near black rock
TP_NEAR_LIT = mix(mm.STRAP, HAMMER, 0.4)  # Its sunny top line

TP_TEPUI = (300, 190, 66, 168)  # The near tepui: middle x, half width, top row, row where the jungle hides its foot
TP_FALLS = (150, 196, 262, 330, 404, 455)  # Places where thin falls pour off its lip


def tp_sky():
    """L0: a soft humid sky, the sun pale through thin cloud."""
    img = nb.gradient(TP_SKY)  # Soft blue to white
    nb.sun(img, 450, 26, 7, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.3))  # Pale sun
    return img  # Give the picture back


def tp_clouds():
    """L1: heavy rolls of cloud drifting over the tops."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((120, 40, 150), (420, 18, 110), (650, 52, 170), (880, 30, 120)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, TP_CLOUD, TP_CLOUD_SH)  # The long base
        for k in range(4):  # Round heaps on top
            blob(img, cx - w // 3 + k * w // 5, cy - 4 - (k % 2) * 2, w // 10, 4 + (k % 2) * 2, TP_CLOUD)  # One heap
    return img  # Give the picture back


def tp_far():
    """L2: far tepuis, blue sheer-walled blocks with flat tops, standing in a sea of cloud."""
    img = mb.new_layer()  # Empty layer
    mesas = [(80, 70, 104, 6.0), (560, 110, 92, 6.0), (800, 50, 112, 6.0)]  # Far tepuis: middle x, half width, top row, very steep sides
    tops, outs, sides, flats = pe.mesa_tops(mesas, 94)  # Their outline
    for x in range(W):  # Each column
        streak = noise(x, 95) % 6 == 0  # Some columns hold a dark streak
        for y in range(tops[x], 150):  # From the top down into the cloud
            c = TP_FAR_CAP if y < flats[x] + 2 and outs[x] == 0 else TP_FAR  # A thin green top, then the wall
            if (outs[x] > 0 and sides[x] < 0) or (streak and y > flats[x] + 2):  # Shaded left side and streaks
                c = TP_FAR_SH  # Shade pixel
            put(img, x, y, c)  # Paint it
    for x in range(W):  # The sea of cloud below them
        top = 138 + int(3 * math.sin(2 * math.pi * 5 * x / W) + 2 * math.sin(2 * math.pi * 13 * x / W))  # Soft billows
        for y in range(top, H):  # Down to the bottom
            put(img, x, y, TP_CLOUD_SH if y == top + 4 and x % 9 < 5 else TP_MIST)  # Cloud with faint folds
    return img  # Give the picture back


def tp_mid():
    """L3: the near tepui: a huge pink-grey sandstone wall, dead flat on top, black streaks, thin falls from its lip,
    a collar of cloud and green jungle at its foot."""
    img = mb.new_layer()  # Empty layer
    cx, half, top, foot = TP_TEPUI  # The tepui
    tops, outs, sides, flats = pe.mesa_tops([(cx, half, top, 5.0), (cx - half - 30, 12, top + 40, 4.0)], 96)  # The block, and a small broken tower beside it
    for x in range(W):  # Each column
        rib = noise(x // 11 % 87, 97) % 3  # The wall is cut into wide upright ribs, each its own shade
        join = x % 11 == 0 and noise(x // 11 % 87, 97) % 3 != noise((x // 11 - 1) % 87, 97) % 3  # A dark join only where two different ribs meet
        streak = noise(x, 98) % 7 == 0  # Some columns carry a long black streak
        streak_to = top + 20 + noise(x, 99) % 70  # How far the streak runs down
        for y in range(tops[x], foot + 6):  # From the top down to the jungle
            if y < tops[x] + 3 and outs[x] == 0:  # The plants on the flat top
                c = TP_TOP_LIT if y == tops[x] else TP_TOP  # A sunny line, then dark plants
            elif outs[x] > 0 and sides[x] < 0:  # The shaded left end
                c = TP_WALL_SH  # Shade pixel
            elif streak and y < streak_to:  # A black streak down the wall
                c = TP_STREAK  # Streak pixel
            else:  # The wall
                c = TP_WALL_SH if join else (TP_WALL, TP_WALL, TP_WALL_LT)[rib]  # Ribs with dark joins
                if (y - top) % 13 == 7 and noise(x // 5 % 192, y) % 3 == 0:  # Faint flat layers in the sandstone
                    c = TP_WALL_SH  # Layer pixel
            put(img, x, y, c)  # Paint it
    for fx in TP_FALLS:  # The thin falls
        drop = 50 + noise(fx, 100) % 50  # How far each falls before it turns to mist
        for y in range(top + 1, top + 1 + drop):  # Down the wall
            t = (y - top) / drop  # 0 at the lip, 1 at the end
            if dither(fx, y, 1.3 - t):  # Breaking into spray lower down
                put(img, fx, y, TP_FALL if (y // 3) % 3 else TP_FALL_SH)  # Water pixel
                put(img, fx + 1, y, TP_FALL_SH)  # A second, bluer column of water
            if t > 0.6 and dither(fx + 1, y, t - 0.5):  # The spray spreads a little
                put(img, fx + (1 if y % 2 else -1), y, TP_MIST)  # Spray pixel
    for x in range(W):  # A collar of cloud lying against the lower wall
        c_top = 132 + int(4 * math.sin(2 * math.pi * 3 * x / W + 2) + 2 * math.sin(2 * math.pi * 11 * x / W))  # Its soft top
        for y in range(c_top, c_top + 12):  # A band of cloud
            if dither(x, y, 1.2 - (y - c_top) / 14 if y > c_top + 5 else 1.0):  # Solid on top, thinner below
                put(img, x, y, TP_MIST if y > c_top else mm.WHITE)  # Cloud pixel
    for x in range(W):  # The jungle below the cloud
        column(img, x, foot + int(2 * math.sin(2 * math.pi * 6 * x / W)) + jag(x, 101, 2), TP_JUNGLE_SH)  # Dark under the trees
    for i, x in enumerate(range(0, W, 7)):  # Round crowns of the jungle
        base = foot + int(2 * math.sin(2 * math.pi * 6 * x / W))  # The canopy line here
        nb.crown(img, x + noise(i, 102) % 3, base + noise(i, 103) % 4, 4 + noise(i, 104) % 3, TP_JUNGLE, TP_JUNGLE_SH)  # One crown
    for i, x in enumerate(range(0, W, 23)):  # Wisps of mist in the jungle
        if noise(i, 105) % 3 == 0:  # A few
            blob(img, x, foot + 12 + noise(i, 106) % 10, 14, 3, TP_MIST, 0.6)  # One wisp
    return img  # Give the picture back


def black_rock(img, x, base, w, h, seed):
    """A strange black summit rock worn by rain: a wide rounded cap on a narrower stem, like a mushroom, sunny on its upper right."""
    ry = max(2, h // 3)  # Height of the cap
    cy = base - h + ry  # Middle row of the cap
    stem = max(2, w * 3 // 5)  # Half width of the stem
    for y in range(cy, base + 1):  # The stem, from the cap down to the ground
        for dx in range(-stem, stem + 1):  # Each pixel across
            put(img, x + dx, y, TP_ROCK_SH if dx < -stem // 3 or (y - cy) % 4 == 3 else TP_ROCK)  # Shaded left side and worn layers
    for y in range(cy - ry, cy + ry + 1):  # The cap
        for dx in range(-w, w + 1):  # Each pixel across
            d = (dx / w) ** 2 + ((y - cy) / ry) ** 2  # 0 in the middle, 1 at the edge
            if d <= 1 and noise((x + dx) % 960, y + seed) % 9 > 0:  # Inside, with a few worn holes
                lit = dx - (y - cy) > w * 0.3  # Upper right faces the sun
                put(img, x + dx, y, TP_ROCK_LT if lit else TP_ROCK_SH if y > cy and dx < 0 else TP_ROCK)  # Rock pixel
    for y in range(cy + 1, cy + ry + 1):  # The dark shadow under the cap
        for dx in range(-w + 2, w - 1):  # Across, a little inside the rim
            if (dx / w) ** 2 + ((y - cy) / ry) ** 2 > 0.6:  # Only near the underside
                put(img, x + dx, y, TP_ROCK_SH)  # Shadow pixel


def pool(img, x, y, w):
    """A small flat pool between the rocks, holding the pale sky, with a dark rim."""
    for dx in range(-w, w + 1):  # Each column of the pool
        put(img, x + dx, y, TP_POOL_SH if abs(dx) == w else TP_POOL)  # Water with dark ends
        if abs(dx) < w - 1:  # A second, shorter row
            put(img, x + dx, y + 1, TP_POOL_SH)  # The near rim


def tp_band():
    """L4: the black rocky summit of Roraima, strange worn rock blocks with pale sand and pools between them."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(200, 2, 3, 108)  # The summit floor
    nb.fill_below(img, tops, TP_ROCK_LT, TP_ROCK)  # Black rock floor
    for x in range(W):  # Pale sand in the hollows
        if noise(x // 6 % 160, 109) % 3 == 0:  # In patches
            put(img, x, tops[x] + 1, TP_SAND)  # Sand pixel
            put(img, x, tops[x] + 2, TP_SAND)  # Sand pixel
    for i, x in enumerate(range(0, W, 48)):  # Rock blocks along the floor
        cx = (x + noise(i, 110) % 14) % W  # A little varied place
        black_rock(img, cx, tops[cx] + 2, 8 + noise(i, 111) % 7, 14 + noise(i, 112) % 10, 113 + i)  # One worn block
    for i, x in enumerate(range(20, W, 80)):  # Pools between the rocks
        cx = (x + noise(i, 114) % 20) % W  # A little varied place
        pool(img, cx, tops[cx] + 4, 4 + noise(i, 115) % 4)  # One pool
    return img  # Give the picture back


def pitcher(img, x, base, h):
    """A small pitcher plant: a few upright red-green tubes."""
    for dx in (-1, 0, 1):  # Three tubes
        column(img, x + dx * 2, base - h + abs(dx), TP_PLANT, base + 1)  # One tube
        put(img, x + dx * 2, base - h + abs(dx) - 1, TP_ROCK_LT)  # Its pale lip


def tp_near():
    """L5: the near black rock just behind the trail, with small pools, pitcher plants and tiny yellow flowers."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(219, 2, 3, 116)  # A gently wavy line near the trail
    nb.fill_below(img, tops, TP_NEAR_LIT, TP_NEAR)  # Black rock
    for i, x in enumerate(range(30, W, 90)):  # Small pools on the rock
        cx = (x + noise(i, 117) % 20) % W  # A little varied place
        pool(img, cx, tops[cx] + 1, 6 + noise(i, 118) % 4)  # One pool
    for i, x in enumerate(range(0, W, 45)):  # Pitcher plants
        cx = (x + noise(i, 119) % 12) % W  # A little varied place
        pitcher(img, cx, tops[cx], 4 + noise(i, 120) % 3)  # One plant
    for x in range(W):  # Tiny flowers
        if noise(x % 480, 121) % 29 == 0:  # A few
            put(img, x, tops[x] - 1, TP_FLOWER)  # Flower pixel
    return img  # Give the picture back


# ---------- SaltFlat: Salar de Uyuni. A white salt flat with hexagon crust that mirrors the sky, far volcano cones, flamingos ----------

SF_STOPS = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.4)), (70, lm.SKY), (138, mix(mm.SKY_LIGHT, mm.WHITE, 0.4))]  # High dry deep blue, pale at the horizon
SF_HORIZON = 140  # Row of the horizon, where the sky meets its mirror
SF_CONE = mix(mm.PLATEAU_SH, lm.HAZE, 0.55)  # Far volcano cones, red-brown softened to lilac
SF_CONE_SH = mix(lm.MOUNTAIN, lm.HAZE, 0.4)  # Their shaded faces
SF_CONE_BAND = mix(mm.VEST, lm.HAZE, 0.5)  # Pale ochre bands of old ash
SF_SNOW = lm.SNOW  # A little snow on the tallest summit
SF_REF = mix(lm.HAZE, mm.SKY_LIGHT, 0.4)  # Their soft mirror image in the wet salt
SF_REF_SH = mix(lm.HAZE_SH, mm.SKY_LIGHT, 0.3)  # The shaded side of the mirror image
SF_SALT = mix(mm.MASK, lm.SNOW, 0.45)  # White salt crust
SF_SALT_LIT = mm.WHITE  # Sunny raised crust ridges
SF_EDGE = mix(mm.MASK, lm.HAZE_SH, 0.4)  # Hexagon crust lines, grey-blue
SF_WET = mix(lm.SKY, mm.SKY_LIGHT, 0.5)  # Thin water on the salt, holding the sky
SF_FLAMINGO = mix(mm.SCARF, mm.WHITE, 0.5)  # Pink flamingos
SF_FLAMINGO_SH = mix(mm.SCARF, mm.SCARF_SH, 0.3)  # Their dark pink wings
SF_LEG = mix(mm.SCARF_SH, mm.MASK_SH, 0.5)  # Their thin legs
SF_PILE = mix(mm.WHITE, mm.MASK, 0.3)  # Small heaps of raked salt
SF_PILE_SH = mix(mm.MASK, lm.HAZE_SH, 0.5)  # Their shaded side


def sf_sky_color(x, y):
    """The sky colour at a pixel; below the horizon the same sky shows again, upside down, in the mirror of wet salt."""
    r = y if y < SF_HORIZON else 2 * SF_HORIZON - y  # Mirror the rows below the horizon
    r = max(0, r)  # Stay inside the sky
    for i in range(len(SF_STOPS) - 1):  # Find the two stops around this row
        (y0, c0), (y1, c1) = SF_STOPS[i], SF_STOPS[i + 1]  # The band this row is in
        if y0 <= r <= y1 or i == len(SF_STOPS) - 2:  # This band, or the last one
            t = max(0.0, min(1.0, (r - y0) / max(1, y1 - y0)))  # How far through the band
            return c1 if dither(x, y, t) else c0  # Dotted blend
    return SF_STOPS[-1][1]  # Past the last stop


def sf_sky():
    """L0: a deep blue high-altitude sky, and the same sky again in the mirror of the flat below the horizon."""
    img = mb.new_layer()  # Empty layer
    for y in range(H):  # Each row
        for x in range(W):  # Each pixel
            put(img, x, y, sf_sky_color(x, y))  # Sky, or its mirror
    nb.sun(img, 380, 30, 8, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4))  # Strong high sun
    return img  # Give the picture back


SF_CLOUDS = ((140, 44, 90), (460, 26, 70), (720, 52, 110))  # Cloud places and widths


def sf_clouds():
    """L1: small white clouds, and their upside-down images in the mirror below the horizon."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in SF_CLOUDS:  # Each cloud
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # The cloud
        ry = 2 * SF_HORIZON - cy  # Where its mirror image sits
        for x in range(cx - w // 2, cx + w // 2):  # Its mirror image, soft and dotted
            t = abs(x - cx) / (w / 2)  # 0 in the middle, 1 at the ends
            hump = int(5 * (1 - t * t)) + noise(x // 4 % 240, cy) % 3  # The same lumps, upside down
            for y in range(ry - 2, ry + hump + 1):  # Top to bottom of the image
                if dither(x, y, 0.7):  # Dotted, so it looks like a reflection
                    put(img, x, y, mm.SKY_LIGHT if y <= ry else mm.WHITE)  # Mirror pixel
    return img  # Give the picture back


def sf_far():
    """L2: far volcano cones on the flat horizon, with soft mirror images of them in the wet salt."""
    img = mb.new_layer()  # Empty layer
    cones = [(260, 92, 0.42, 1.0), (40, 118, 0.6, 1.0), (600, 112, 0.5, 1.0), (760, 124, 0.7, 1.0)]  # Middle x, top row, slope, roundness
    for x in range(W):  # Each column
        best, owner = SF_HORIZON, None  # The highest cone here
        for cx, top, slope, _ in cones:  # Each cone
            y = cone_row(x, cx, top, slope) + jag(x, 122, 1)  # Its straight side here
            if y < best:  # Higher than the others
                best, owner = int(y), (cx, top)  # This cone owns the column
        if owner is None:  # Flat horizon here
            continue  # Next column
        cx, top = owner  # The cone
        off = mb.wrap_offset(x, cx)  # Signed distance from its middle
        for y in range(best, SF_HORIZON):  # From the slope down to the horizon
            shaded = off < (y - top) * 0.15 - 2  # Left of a slanting line is the shaded face
            c = SF_CONE_SH if shaded else SF_CONE  # Rock colour
            if (y - top) % 11 == 6 and not shaded and noise(x // 4 % 240, y) % 4 > 0:  # Thin broken ochre bands on the sunny face
                c = SF_CONE_BAND  # Band pixel
            if top < 100 and y < top + 5 + noise(x, 123) % 3:  # A little snow on the tallest summit
                c = SF_SNOW  # Snow pixel
            put(img, x, y, c)  # Paint it
            ry = 2 * SF_HORIZON - y - 1  # The mirror row
            if dither(x, ry, 0.85):  # Slightly dotted, like a reflection
                put(img, x, ry, SF_REF_SH if shaded else SF_REF)  # Mirror pixel
    for x in range(W):  # A thin dark line where the far shore of salt meets the horizon
        if noise(x // 3 % 320, 124) % 4 > 0:  # Broken
            put(img, x, SF_HORIZON, SF_EDGE)  # Shore pixel
    return img  # Give the picture back


SF_HEX_BANDS = ((150, 166, 16, 3), (166, 186, 24, 5), (186, 214, 40, 8), (214, H, 60, 12))  # Crust bands, far to near: first row, end row, cell width, cell height


def hex_cell(x, y, y0, w, h):
    """Which hexagon cell of a crust band a pixel is in. The cells are w wide and squashed to h rows, as if seen low across the flat.
    w divides 960, so the cells tile."""
    u, v = (x % W) / w, (y - y0) / h  # Pixel in cell units
    best, cell = 9.0, None  # Distance to the nearest cell middle, and that cell
    for j in range(int(math.floor(v)) - 1, int(math.floor(v)) + 3):  # Nearby rows of cells
        shift = 0.5 if j % 2 else 0.0  # Every second row is shifted half a cell
        for i in range(int(u) - 2, int(u) + 3):  # Nearby cells in the row
            d = math.hypot(u - (i + shift), (v - j) * 0.87)  # Distance in regular hexagon space
            if d < best:  # Nearest so far
                best, cell = d, (i % (W // w), j)  # Remember it; the cell number wraps so the pattern tiles
    return cell  # Give the cell back


def hex_edge(x, y, y0, w, h):
    """True on the thin line between two hexagon cells: the pixel to the right or the one below is in another cell."""
    here = hex_cell(x, y, y0, w, h)  # This pixel's cell
    return here != hex_cell(x + 1, y, y0, w, h) or here != hex_cell(x, y + 1, y0, w, h)  # A neighbour in another cell


def sf_wet(x, y):
    """True where thin water lies on the salt. Long flat sheets, nearly everywhere far away and fewer toward the viewer."""
    f = math.sin(2 * math.pi * 2 * x / W + 1.3) + 0.7 * math.sin(2 * math.pi * 5 * x / W + y / 3) + 0.4 * math.sin(2 * math.pi * 11 * x / W + y / 2)  # A smooth wavy field, stretched sideways
    return f > -1.6 + (y - SF_HORIZON - 10) / 14  # Most of it far away, little of it near


def sf_mid():
    """L3: the white salt crust in hexagon cells, getting bigger toward the viewer, with long sheets of thin water that hold the sky,
    and a few far flamingos in the wet."""
    img = mb.new_layer()  # Empty layer
    for y in range(SF_HORIZON + 10, H):  # The salt, from a little below the horizon
        band = next((b for b in SF_HEX_BANDS if b[0] <= y < b[1]), SF_HEX_BANDS[-1])  # Which crust band
        y0, _, w, h = band  # Its first row and cell size
        for x in range(W):  # Each pixel
            if sf_wet(x, y):  # Water on the salt: the sky shows in it
                c = sf_sky_color(x, y)  # Mirror of the sky
            elif sf_wet(x, y - 1):  # The wet edge of the dry crust
                c = SF_SALT_LIT  # Bright edge pixel
            elif hex_edge(x, y, y0, w, h):  # A crust line
                c = SF_EDGE  # Line pixel
            else:  # The white crust
                c = SF_SALT  # Crust pixel
            put(img, x, y, c)  # Paint it
    for x, y in ((120, 158), (128, 159), (136, 157), (520, 162), (527, 161), (820, 156)):  # Far flamingos standing in the water
        flamingo(img, x, y)  # One bird
    return img  # Give the picture back


def flamingo(img, x, y):
    """A tiny far flamingo standing on one thin leg, with its faint mirror image below."""
    for dy in range(4):  # The thin leg
        put(img, x, y - dy, SF_LEG)  # Leg pixel
        if dy and dither(x, y + dy, 0.6):  # Its mirror image
            put(img, x, y + dy, SF_LEG)  # Mirror pixel
    for dx in range(-1, 2):  # The pink body
        put(img, x + dx, y - 4, SF_FLAMINGO)  # Body pixel
        put(img, x + dx, y - 5, SF_FLAMINGO_SH if dx < 1 else SF_FLAMINGO)  # Wing pixel
    for dy in range(6, 9):  # The long neck
        put(img, x + 1, y - dy, SF_FLAMINGO)  # Neck pixel
    put(img, x + 2, y - 8, SF_LEG)  # The dark bill
    for dx in range(-1, 2):  # The mirror image of the body
        put(img, x + dx, y + 4, SF_FLAMINGO)  # Mirror pixel


def salt_pile(img, x, base, h):
    """A small cone of raked salt, sunny on the right."""
    for dy in range(h):  # Each row of the cone, from the bottom up
        r = (h - dy) * 3 // 4  # Narrower going up
        for dx in range(-r, r + 1):  # Each pixel across
            put(img, x + dx, base - dy, SF_PILE_SH if dx < 0 else SF_PILE)  # Shaded left side


def sf_band():
    """L4: a row of small raked salt heaps standing on the crust, far apart."""
    img = mb.new_layer()  # Empty layer
    for i, x in enumerate(range(0, W, 32)):  # Heaps in a long row
        if noise(i, 125) % 4 > 0:  # With gaps
            salt_pile(img, x + noise(i, 126) % 6, 198 + noise(i, 127) % 2, 5 + noise(i, 128) % 2)  # One heap
    return img  # Give the picture back


def sf_near():
    """L5: the near crust just behind the trail: raised white hexagon ridges."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(220, 1, 2, 129)  # A nearly flat line
    for x in range(W):  # Each column
        for y in range(tops[x], H):  # The crust
            if hex_edge(x, y, 220, 96, 16):  # A raised ridge between cells
                c = SF_SALT_LIT  # Ridge pixel
            elif hex_edge(x, y - 1, 220, 96, 16):  # The shade just below a ridge
                c = SF_EDGE  # Shade pixel
            else:  # The crust
                c = SF_SALT  # Crust pixel
            put(img, x, y, SF_SALT_LIT if y == tops[x] else c)  # A bright top line
    return img  # Give the picture back


THEMES = {  # One entry per Plateaus Normal theme (D-073): its six layers, and ground tiles for the preview (its own, else a near match)
    "TableTop": ((tt_sky, tt_clouds, tt_far, tt_mid, tt_band, tt_near), ("tabletop", "slope")),  # Table Mountain
    "MesaValley": ((mv_sky, mv_clouds, mv_far, mv_mid, mv_band, mv_near), ("mesa", "dune")),  # Monument Valley
    "CanyonRim": ((cr_sky, cr_clouds, cr_far, cr_mid, cr_band, cr_near), ("canyon", "gorge")),  # Grand Canyon
    "TepuiTop": ((tp_sky, tp_clouds, tp_far, tp_mid, tp_band, tp_near), ("tepui", "lava")),  # Mount Roraima
    "SaltFlat": ((sf_sky, sf_clouds, sf_far, sf_mid, sf_band, sf_near), ("salar", "salt")),  # Salar de Uyuni
}  # All five themes


def ground_name(grounds):
    """The ground tile name for the preview: the theme's own tile, else a near match made earlier, else plain grass."""
    for ground in grounds:  # Own tile first, then the near match
        if os.path.exists(os.path.join(nb.SPRITES, ground + "_top.png")):  # Made already
            return ground  # Use it
    return "grass"  # Fall back to grass


def main():
    """Draws and saves every set and the preview grid."""
    shots = []  # One preview screen per theme
    for theme, (makers, grounds) in THEMES.items():  # Each theme
        out = os.path.join(nb.BACKDROPS, theme)  # Its folder
        os.makedirs(out, exist_ok=True)  # Make it if missing
        layers = [make() for make in makers]  # Draw L0 to L5
        for i, img in enumerate(layers):  # Save each one
            img.save(os.path.join(out, "L%d.png" % i))  # L0.png ... L5.png
        shots.append(nb.preview_screen(layers, ground_name(grounds)))  # Preview screen
        print("Backdrop written to", out)  # Tell where it went
    rows = (len(shots) + 1) // 2  # Two screens per row
    sheet = Image.new("RGBA", (968, rows * 278 - 8), mm.OUTLINE + (255,))  # Grid with dark gaps
    for i, shot in enumerate(shots):  # Place each screen
        sheet.paste(shot, ((i % 2) * 488, (i // 2) * 278))  # 8-pixel gaps
    os.makedirs(os.path.dirname(PREVIEW), exist_ok=True)  # Make the folder
    sheet.save(PREVIEW)  # Normal size, to keep the file small
    print("Preview written to", PREVIEW)  # Tell where it went


if __name__ == "__main__":
    main()  # Run when started directly
