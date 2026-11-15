"""Draws the moving background layers for the seven Mountains Hard themes (D-063, D-065).

Run from the Unity project folder:  python Tools/Art/make_hard_backdrops.py
It writes L0.png to L5.png (960x270 each) to Assets/Art/Level/Sprites/Backdrop/<Theme>/ for
LavaSlope (Mount Mayon), StormRidge (Sierra Madre), EagleForest (Kitanglad Range), BonsaiForest (Mount Hamiguitan),
GlacierHorn (Matterhorn), EquatorSummit (Kilimanjaro) and SeaPeak (Mauna Kea),
plus a preview at .scratch/mountains-hard/backdrop-preview.png.

Each set keeps the six layers and the scrolling of the other Mountains backdrops:
L0 sky, L1 clouds, L2 far range, L3 middle range, L4 forest band, L5 near slope.
Most themes use palette colours or even mixes of two. Storm Ridge uses muted storm tones.
The rows behind the explorer (100 to 220) stay
mid-toned and soft, so the explorer, the pickups and the ledges stand out.
Every layer tiles sideways without a seam.
"""
import math  # sin, hypot and pi for the shapes
import os  # Folders and paths
from PIL import Image  # Saves the preview
import make_mainmenu_art as mm  # Palette and image helpers
import make_landform_map_art as lm  # Map colours and the fixed noise
import make_mountains_backdrop as mb  # The Easy backdrop helpers: pixels, ridges, pines and dither
import make_normal_backdrops as nb  # The Normal backdrop helpers: gradient, sun, clouds, ranges, crowns and the preview

W, H = mb.W, mb.H  # 960x270 layers
mix, put, column, dither, jag = mb.mix, mb.put, mb.column, mb.dither, mb.jag  # Shared drawing helpers
PREVIEW = os.path.join(mb.ROOT, ".scratch", "mountains-hard", "backdrop-preview.png")  # Where the preview goes


def bumps(img, tops, spots, lit, shade):
    """Small rounded rocks or shrubs sitting on a ground line: spots are (x, width)."""
    for x, w in spots:  # Each bump
        for dx in range(w):  # Each column
            hump = int(5 * math.sin(math.pi * (dx + 0.5) / w)) + 1  # Rounded top
            for dy in range(hump):  # Rows
                put(img, x + dx, tops[(x + dx) % W] + 2 - dy, lit if dx > w // 3 else shade)  # Shaded left side


# ---------- Lava Slope: Mount Mayon up close, an ashy sky, a huge cone with glowing lava, grey ash fields and burnt trees ----------

LS_SKY = [(0, mix(mm.MASK_SH, lm.MOUNTAIN, 0.4)), (90, mix(mm.MASK, lm.HAZE, 0.5)), (170, mix(mm.MASK, mb.SUN_GLOW, 0.15))]  # Grey ashy sky, a little warm low down
LS_CONE = mix(lm.MOUNTAIN_SH, mm.MASK_SH, 0.5)  # The huge near cone, grey-blue
LS_CONE_SH = mix(lm.MOUNTAIN_SH, mm.STRAP, 0.45)  # Its shaded face
LS_LAVA = mix(mm.SCARF, mb.SUN_GLOW, 0.35)  # Glowing lava flow
LS_LAVA_SH = mm.SCARF_SH  # Cooler edge of the flow
LS_ASH = mix(mm.MASK, mm.MASK_SH, 0.5)  # Grey ash
LS_ASH_SH = mix(mm.MASK_SH, mm.STRAP, 0.3)  # Ash in shade
LS_SMOKE = mix(mm.MASK, mm.WHITE, 0.3)  # Ash cloud
LS_SMOKE_SH = mm.MASK_SH  # Its darker underside
LS_DEAD = mix(mm.STRAP, mm.MASK_SH, 0.4)  # Burnt trees


def ls_sky():
    """L0: grey ashy sky with a pale, dim sun."""
    img = nb.gradient(LS_SKY)  # Grey to warm
    nb.sun(img, 700, 60, 7, mix(mb.SUN, mm.MASK, 0.4), mix(mb.SUN_GLOW, mm.MASK, 0.5), mix(mm.MASK, lm.HAZE, 0.5))  # Dim sun behind ash
    return img


def ls_clouds():
    """L1: low dark ash clouds."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((140, 40, 180), (470, 24, 140), (760, 50, 200)):  # Cloud places
        nb.flat_cloud(img, cx, cy, w, LS_SMOKE, LS_SMOKE_SH)  # Ashy cloud
    return img


def ls_far():
    """L2: the huge cone, with two glowing lava flows and a tall ash column."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        d = mb.wrap_offset(x, 300)  # Signed distance from the summit
        top = int(26 + (abs(d) ** 1.2) * 0.32)  # Tall concave cone
        for y in range(max(top, 0), H):  # Down the slope
            c = LS_CONE_SH if d < -(y - 26) * 0.1 else LS_CONE  # Shaded left face
            for (off, bend) in ((6, 0.18), (-10, -0.12)):  # Two lava flows
                if abs(d - off - (y - 30) * bend) < 2 and y < 150:  # Inside a flow
                    c = LS_LAVA if abs(d - off - (y - 30) * bend) < 1 else LS_LAVA_SH  # Bright middle, red edge
            if y >= 160 and dither(x, y, min(1.0, (y - 160) / 24)):  # Low down
                c = LS_ASH  # Ash haze
            put(img, x, y, c)  # Paint
    for i in range(50):  # The ash column, rising and leaning right
        cx, cy, r = 300 + i * 1.5, 22 - i * 0.4, 3 + i // 6  # Drifts right and grows
        for y in range(int(cy - r), int(cy + r) + 1):  # Puff rows
            for x in range(int(cx - r), int(cx + r) + 1):  # Puff columns
                if y >= 0 and math.hypot(x - cx, y - cy) <= r and dither(x, y, 1.0 - i / 60):  # Thins out
                    put(img, x, y, LS_SMOKE_SH if y > cy + r // 3 else LS_SMOKE)  # Darker underside
    return img


def ls_mid():
    """L3: low grey ash ridges."""
    return nb.ranged([(120, 150, 0), (380, 142, 0), (640, 154, 0), (870, 146, 0)], 0.45, 3, 3, 3.0, LS_ASH, LS_ASH_SH, LS_ASH, LS_ASH_SH, LS_ASH, 186)  # Ash ridges


def ls_forest():
    """L4: an ash field with bare burnt trees."""
    img = mb.new_layer()  # Empty layer
    floor = nb.rolling(190, 3, 4, 21)  # Field line
    nb.fill_below(img, floor, LS_ASH, LS_ASH_SH)  # Ash field
    for i, x in enumerate(range(0, W, 23)):  # Burnt trees
        h = 10 + lm.noise(i, 71) % 8  # Height
        column(img, x, floor[x] - h, LS_DEAD, floor[x])  # Trunk
        for k in (3, 6):  # Two bare branches
            put(img, x - 1, floor[x] - h + k, LS_DEAD)  # Left twig
            put(img, x + 1, floor[x] - h + k - 1, LS_DEAD)  # Right twig
    return img


def ls_near():
    """L5: grey ash ground with dark lava boulders."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(206, 3, 3, 23)  # Ground line
    nb.fill_below(img, tops, LS_ASH, LS_ASH_SH)  # Ash
    bumps(img, tops, ((90, 14), (300, 10), (520, 18), (700, 12), (880, 16)), LS_CONE, LS_CONE_SH)  # Lava boulders
    return img


# ---------- Storm Ridge: Sierra Madre under broad storm clouds and misty green ridges ----------

SR_SKY = ((0, (63, 80, 96)), (80, (91, 113, 127)), (172, (154, 171, 167)), (269, (154, 171, 167)))  # Slate sky opens into a misty horizon
SR_CLOUD = (67, 85, 102)  # Main storm bank
SR_CLOUD_BASE = (82, 102, 117)  # Lower, lighter shelf of cloud
SR_FAR = (102, 132, 134)  # Blue-green distant range
SR_FAR_MIST = (138, 160, 156)  # Mist softens the foot of the range
SR_MID = (70, 108, 101)  # Nearer forested range
SR_MID_MIST = (109, 143, 128)  # Soft green mist in the valley
SR_CANOPY = (60, 91, 77)  # Rounded forest silhouette
SR_FOREST = (70, 101, 83)  # Wet forest below the trees
SR_SLOPE = (78, 110, 85)  # Foreground slope behind the explorer
SR_SLOPE_BASE = (87, 117, 89)  # A soft lift near the trail


def sr_contour(base, waves):
    """Draws a looping silhouette from waves with whole-number repeat counts."""
    return [round(base + sum(height * math.cos(math.tau * (repeats * x / W + shift)) for repeats, height, shift in waves))  # Same shape either side of the seam
            for x in range(W)]  # One top row per column


def sr_range(tops, crest, base, depth):
    """Draws a clear ridge edge with a slow fade down into the valley."""
    img = mb.new_layer()  # Starts with sky showing above the ridge
    colors = [mix(crest, base, min(1, y / depth)) + (255,) for y in range(H)]  # One soft ramp for every column
    pixels = img.load()  # Writes directly to the layer
    for x, top in enumerate(tops):  # Follows the full silhouette
        for y in range(top, H):  # Fills the ridge below its edge
            pixels[x, y] = colors[y - top]  # No dither or hard shadows between peaks
    return img  # Gives back the painted range


def sr_sky():
    """L0: an even storm sky with a lighter horizon."""
    img = mb.new_layer()  # Starts with no pixels
    for (start, high), (end, low) in zip(SR_SKY, SR_SKY[1:]):  # Walks the sky bands from top to bottom
        for y in range(start, end + 1):  # Fills each row with one soft tone
            color = mix(high, low, (y - start) / (end - start))  # Blends without a dot pattern
            img.paste(color + (255,), (0, y, W, y + 1))  # Keeps the color even across the view
    return img  # Gives back the sky


def sr_clouds():
    """L1: one broad storm bank, with rain left to the moving foreground."""
    img = mb.new_layer()  # Leaves room for the sky below
    lower = sr_contour(40, ((2, 8, .13), (4, 4, .36), (7, 2, .18)))  # Wide cloud shelf with small turns
    upper = sr_contour(17, ((2, 5, .03), (3, 4, .24), (6, 2, .48)))  # A darker band near the top of the sky
    for x, (top, foot) in enumerate(zip(upper, lower)):  # Draws the smooth cloud edge
        for y in range(foot):  # Paints down to the base of the bank
            shade = min(1, max(0, (y - top) / 15))  # Softens the bottom edge before the sky
            put(img, x, y, mix(SR_CLOUD, SR_CLOUD_BASE, shade))  # Uses only calm storm tones
    return img  # Gives back the cloud bank


def sr_far():
    """L2: the long distant spine of the mountain range."""
    tops = sr_contour(119, ((3, 14, .12), (5, 9, .37), (8, 4, .21)))  # Broad crests and shallow saddles
    return sr_range(tops, SR_FAR, SR_FAR_MIST, 67)  # Lets the ridge fade into the basin


def sr_mid():
    """L3: a nearer chain of softly rounded ridges."""
    tops = sr_contour(153, ((2, 15, .47), (4, 10, .12), (7, 5, .35), (11, 2, .2)))  # A distinct lower outline
    return sr_range(tops, SR_MID, SR_MID_MIST, 59)  # Adds soft depth without streaks across the range


def sr_forest():
    """L4: a quiet strip of low, clustered forest crowns."""
    tops = sr_contour(197, ((3, 6, .19), (6, 3, .47), (12, 2, .28)))  # Canopy rises gently with the land
    img = sr_range(tops, SR_CANOPY, SR_FOREST, 32)  # Fills the forest below the crowns
    for i, x in enumerate(range(0, W, 18)):  # Spreads small clusters across the ridge
        if lm.noise(i, 32) < 36:  # Leaves gaps between the groups
            continue  # Keeps the skyline from looking like a row of teeth
        r = 3 + lm.noise(i, 34) % 4  # Varies the size of each crown
        nb.crown(img, x, tops[x] + 1, r, SR_FOREST, SR_CANOPY)  # Uses close tones for the leaves
    return img  # Gives back the forest band


def sr_near():
    """L5: a low green slope that keeps the playable trail easy to see."""
    tops = sr_contour(213, ((2, 3, .39), (5, 2, .13), (9, 1, .45)))  # Keeps the silhouette below the action
    return sr_range(tops, SR_SLOPE, SR_SLOPE_BASE, 24)  # Removes the repeated bright lines at ground level


# ---------- Eagle Forest: Kitanglad Range, misty high ridges, very tall rainforest trees and an eagle overhead ----------

EF_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.2)), (100, mm.SKY_LIGHT), (170, mix(mm.SKY_LIGHT, mm.WHITE, 0.4))]  # Clear blue to misty white
EF_FAR = mix(mm.LEAF, lm.HAZE, 0.65)  # Far forest ridges
EF_FAR_SH = mix(mm.LEAF, lm.HAZE_SH, 0.5)  # Their shade
EF_MID = mix(mm.LEAF, lm.HAZE, 0.45)  # Nearer forest ridge
EF_MID_SH = mix(mm.LEAF, lm.HAZE_SH, 0.3)  # Its shade
EF_TREE = mix(mm.HILLS_SH, lm.HAZE, 0.3)  # Tall tree crowns
EF_TREE_SH = mix(mm.LEAF, lm.MOUNTAIN_SH, 0.3)  # Their shade
EF_TRUNK = mix(mm.GROUND_SH, lm.HAZE, 0.3)  # Pale tall trunks
EF_FLOOR = mix(mm.HILLS_SH, mm.GROUND_SH, 0.45)  # Forest floor
EF_FLOOR_LIT = mix(mm.HILLS, mm.GROUND, 0.4)  # Its lit edge


def ef_sky():
    """L0: clear high sky."""
    img = nb.gradient(EF_SKY)  # Blue to mist
    nb.sun(img, 220, 44, 7, mb.SUN, mb.SUN_GLOW, mm.SKY_LIGHT)  # Bright sun
    return img


def ef_clouds():
    """L1: a few clouds and a Philippine eagle soaring."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((160, 34, 110), (520, 56, 90), (820, 28, 130)):  # Cloud places
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # White cloud
    for (x, y) in ((400, 40), (650, 70)):  # Two eagles soaring
        for k in range(-7, 8):  # Wide wings
            put(img, x + k, y + abs(k) // 3 - (2 if abs(k) > 4 else 0), mb.BIRD)  # Wing pixel, tips turned up
        put(img, x, y + 1, mb.BIRD)  # Body
    return img


def ef_far():
    """L2: misty high forest ridges of the range."""
    peaks = [(100, 92, 0), (330, 80, 0), (560, 96, 0), (780, 84, 0)]  # Rounded forest summits
    return nb.ranged(peaks, 0.5, 11, 3, 4.0, EF_FAR, EF_FAR_SH, EF_FAR, EF_FAR_SH, mm.WHITE, 140)  # With mist in the valleys


def ef_mid():
    """L3: a nearer forest ridge."""
    return nb.ranged([(200, 124, 0), (520, 130, 0), (820, 120, 0)], 0.42, 12, 3, 3.0, EF_MID, EF_MID_SH, EF_MID, EF_MID_SH, mm.WHITE, 168)  # Forest ridge with mist


def ef_forest():
    """L4: very tall rainforest trees with straight pale trunks."""
    img = mb.new_layer()  # Empty layer
    floor = nb.rolling(196, 2, 3, 41)  # Forest floor
    for x in range(W):  # Under the trees
        column(img, x, floor[x], EF_TREE_SH)  # Dark understory
    for i, x in enumerate(range(10, W, 40)):  # Tall trees
        h = 70 + lm.noise(i, 42) % 30  # Very tall
        column(img, x, floor[x] - h, EF_TRUNK, floor[x])  # Trunk
        column(img, x + 1, floor[x] - h, EF_TRUNK, floor[x])  # Thick trunk
        for (dx, dy, r) in ((0, 0, 9), (-8, 4, 6), (8, 4, 6)):  # Wide crown
            nb.crown(img, x + dx, floor[x] - h + dy, r, EF_TREE, EF_TREE_SH)  # Crown part
    return img


def ef_near():
    """L5: forest floor with ferns."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(206, 3, 3, 43)  # Ground line
    nb.fill_below(img, tops, EF_FLOOR_LIT, EF_FLOOR)  # Floor
    bumps(img, tops, ((40, 16), (230, 12), (420, 20), (610, 14), (800, 18)), EF_TREE, EF_TREE_SH)  # Fern clumps
    return img


# ---------- Bonsai Forest: Mount Hamiguitan, a rounded ridge with a red-soil top, low twisted trees and pitcher plants ----------

BF_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.15)), (110, lm.SKY), (170, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.2))]  # Warm clear sky
BF_FAR = mix(mm.LEAF, lm.HAZE, 0.6)  # Far forest ridge
BF_FAR_SH = mix(mm.LEAF, lm.HAZE_SH, 0.45)  # Its shade
BF_RED = mix(mm.PLATEAU, lm.HAZE, 0.35)  # Red ultramafic soil far away
BF_RED_SH = mix(mm.PLATEAU_SH, lm.HAZE_SH, 0.35)  # Its shade
BF_SHRUB = mix(mm.LEAF, lm.HAZE, 0.3)  # Low twisted trees
BF_SHRUB_SH = mix(mm.LEAF, mm.OUTLINE, 0.3)  # Their shade
BF_SOIL = mix(mm.PLATEAU_SH, mm.GROUND_SH, 0.4)  # Near red soil
BF_SOIL_LIT = mix(mm.PLATEAU, lm.HAZE, 0.25)  # Its lit edge


def bf_sky():
    """L0: warm clear sky."""
    img = nb.gradient(BF_SKY)  # Blue to warm
    nb.sun(img, 600, 50, 8, mb.SUN, mb.SUN_GLOW, mm.SKY_LIGHT)  # Sun
    return img


def bf_clouds():
    """L1: soft white clouds."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((90, 40, 120), (400, 26, 150), (720, 52, 110)):  # Cloud places
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # Cloud
    return img


def bf_far():
    """L2: the rounded mountain, green below and a red-soil top where the pygmy forest grows."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        d = mb.wrap_offset(x, 420)  # Distance from the top
        top = int(70 + d * d / 900.0 + jag(x, 5, 2))  # A rounded top
        top = min(top, 150 + int(4 * math.sin(2 * math.pi * 3 * x / W)))  # Low ridge elsewhere
        for y in range(top, H):  # Down the slope
            red = abs(d) < 150 and y < top + 14  # The red-soil top
            c = (BF_RED_SH if d < 0 else BF_RED) if red else (BF_FAR_SH if d < 0 else BF_FAR)  # Red top or green forest
            if red and lm.noise(x % 480, y) % 7 == 0:  # Tiny trees dotted on the red soil
                c = BF_SHRUB  # Tree dot
            put(img, x, y, c)  # Paint
    return img


def bf_mid():
    """L3: a lower green ridge."""
    return nb.ranged([(150, 140, 0), (450, 134, 0), (760, 144, 0)], 0.4, 13, 2, 3.0, BF_FAR, BF_FAR_SH, BF_FAR, BF_FAR_SH, mm.SKY_LIGHT, 178)  # Green ridge


def bf_forest():
    """L4: a band of low, flat-topped twisted trees on red soil."""
    img = mb.new_layer()  # Empty layer
    floor = nb.rolling(192, 2, 4, 51)  # Ground line
    nb.fill_below(img, floor, BF_SOIL_LIT, BF_SOIL)  # Red soil
    for i, x in enumerate(range(0, W, 10)):  # Low trees
        for dx in range(-4, 5):  # Flat crown
            put(img, x + dx, floor[x] - 6 - (1 if abs(dx) < 2 else 0), BF_SHRUB if dx > 0 else BF_SHRUB_SH)  # Crown pixel
            put(img, x + dx, floor[x] - 5, BF_SHRUB_SH)  # Crown underside
        column(img, x + lm.noise(i, 52) % 2, floor[x] - 4, mb.TRUNK, floor[x])  # Short trunk
    return img


def bf_near():
    """L5: red soil with grey-green rocks and pitcher plants."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(206, 3, 3, 53)  # Ground line
    nb.fill_below(img, tops, BF_SOIL_LIT, BF_SOIL)  # Soil
    bumps(img, tops, ((80, 14), (360, 10), (600, 16), (840, 12)), mix(mm.HILLS_SH, mm.MASK_SH, 0.5), mix(mm.LEAF, mm.STRAP, 0.5))  # Serpentinite rocks
    for x in (200, 470, 720):  # Pitcher plants
        for dy in range(5):  # The cup
            put(img, x, tops[x] - dy, mm.SCARF_SH)  # Red cup
            put(img, x + 1, tops[x] - dy, mix(mm.SCARF_SH, mm.LEAF, 0.5))  # Green-red side
    return img


# ---------- Glacier Horn: the Matterhorn, a sharp lone horn, cirques full of ice, a glacier and alpine flowers ----------

GH_SKY = [(0, mix(lm.MOUNTAIN, lm.SKY, 0.4)), (90, lm.SKY), (170, mm.SKY_LIGHT)]  # Cold blue sky
GH_ROCK = mix(lm.MOUNTAIN, lm.HAZE, 0.25)  # Horn rock
GH_ROCK_SH = mix(lm.MOUNTAIN_SH, lm.HAZE, 0.1)  # Its shade
GH_ICE = mix(lm.SNOW, lm.LENS, 0.2)  # Blue glacier ice
GH_ICE_SH = mix(lm.HAZE, lm.LENS, 0.4)  # Ice in shade
GH_MEADOW = mix(mm.HILLS_SH, lm.HAZE, 0.35)  # Alpine meadow
GH_MEADOW_LIT = mix(mm.HILLS, lm.HAZE, 0.35)  # Its lit edge


def gh_sky():
    """L0: cold clear sky."""
    img = nb.gradient(GH_SKY)  # Blue
    nb.sun(img, 160, 40, 6, mm.WHITE, mb.SUN, lm.SKY)  # Small bright sun
    return img


def gh_clouds():
    """L1: one cloud banner off the horn and a few thin clouds."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((560, 38, 70), (820, 26, 140), (120, 60, 100)):  # Cloud places
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # Cloud
    return img


def gh_far():
    """L2: the lone pyramid horn, with steep faces and snow in the cracks, and low snowy peaks around it."""
    peaks = [(300, 96, 20), (80, 108, 26), (560, 112, 24), (760, 118, 22), (900, 116, 22)]  # The horn at x 300, low neighbours
    img = nb.ranged(peaks, 0.5, 2, 3, 2.0, GH_ROCK, GH_ROCK_SH, mm.WHITE, lm.HAZE, mm.SKY_LIGHT, 170)  # Gentle range
    for x in range(W):  # Redraw the horn steeper on top
        d = mb.wrap_offset(x, 300)  # Distance from the horn tip
        top = int(18 + abs(d) * 1.6)  # Very steep sides
        for y in range(top, min(H, 150)):  # Down the horn
            c = GH_ROCK_SH if d < (y - 18) * 0.08 else GH_ROCK  # Shaded left face, ridge line a little right of the middle
            if lm.noise(x % 480, y) % 11 == 0 or y < top + 2 or y < 40:  # Snow in cracks and on the edges
                c = mm.WHITE if d >= 0 else lm.HAZE  # Snow pixel
            put(img, x, y, c)  # Paint
    return img


def gh_mid():
    """L3: a glacier flowing down a wide ice valley between dark ridges."""
    img = nb.ranged([(160, 132, 10), (800, 128, 10)], 0.42, 6, 3, 3.0, GH_ROCK, GH_ROCK_SH, mm.WHITE, lm.HAZE, mm.SKY_LIGHT, 182)  # Two side ridges
    for x in range(W):  # The glacier between them
        top = 156 + int(4 * math.sin(2 * math.pi * 2 * x / W))  # Ice surface
        for y in range(top, H):  # Down
            if not img.getpixel((x, y))[3]:  # Only where the ridges are not
                put(img, x, y, GH_ICE_SH if (x + 2 * y) % 29 < 2 else GH_ICE)  # Ice with crevasse lines
    return img


def gh_forest():
    """L4: a band of rocky moraine and a few dark larches."""
    img = mb.new_layer()  # Empty layer
    floor = nb.rolling(192, 3, 3, 61)  # Ground line
    nb.fill_below(img, floor, mix(mm.MASK_SH, lm.HAZE, 0.5), mix(mm.MASK_SH, lm.MOUNTAIN, 0.4))  # Moraine
    for i, x in enumerate(range(0, W, 26)):  # Larches
        mb.pine(img, x + lm.noise(i, 62) % 6, floor[x] + 1, 12 + lm.noise(i, 63) % 8, mix(mm.LEAF, lm.HAZE, 0.3), mix(mm.LEAF, lm.MOUNTAIN_SH, 0.4))  # A larch
    return img


def gh_near():
    """L5: alpine meadow with white flowers and grey stones."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(206, 3, 3, 63)  # Ground line
    nb.fill_below(img, tops, GH_MEADOW_LIT, GH_MEADOW)  # Meadow
    for x in range(0, W, 11):  # Flowers
        put(img, x, tops[x] + 3 + lm.noise(x % 480, 64) % 8, mm.WHITE)  # White flower
    bumps(img, tops, ((120, 12), (400, 16), (680, 10), (900, 14)), mix(mm.MASK, lm.HAZE, 0.5), mix(mm.MASK_SH, lm.HAZE, 0.4))  # Stones
    return img


# ---------- Equator Summit: Kilimanjaro, a wide flat-topped mountain with a snow cap, rocky alpine desert and giant groundsels ----------

ES_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.35)), (100, lm.SKY), (170, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4))]  # Deep blue, warm near the horizon
ES_ROCK = mix(mm.VEST_SH, lm.HAZE, 0.45)  # Brown volcanic rock far away
ES_ROCK_SH = mix(mm.VEST_SH, lm.HAZE_SH, 0.3)  # Its shade
ES_SNOW = mm.WHITE  # The snow cap
ES_SAV = mix(mm.HELMET_SH, lm.HAZE, 0.45)  # Savanna far below
ES_SCREE = mix(mm.VEST_SH, mm.MASK_SH, 0.5)  # Near scree
ES_SCREE_LIT = mix(mm.VEST, mm.MASK, 0.5)  # Its lit edge
ES_PLANT = mix(mm.HILLS_SH, lm.HAZE, 0.3)  # Groundsel leaves
ES_STEM = mix(mm.GROUND_SH, mm.VEST_SH, 0.5)  # Groundsel stems


def es_sky():
    """L0: deep blue sky, warm near the horizon."""
    img = nb.gradient(ES_SKY)  # Blue to warm
    nb.sun(img, 760, 46, 8, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.5))  # Sun
    return img


def es_clouds():
    """L1: a ring of flat clouds below the summit."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((120, 110, 160), (380, 118, 140), (640, 112, 180), (880, 120, 120)):  # Clouds low down, around the mountain
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # Cloud
    return img


def es_far():
    """L2: the wide, flat-topped mountain with its snow cap, and the yellow savanna far below."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        d = mb.wrap_offset(x, 300)  # Distance from the middle
        top = 84 if abs(d) < 70 else int(84 + (abs(d) - 70) * 0.45)  # Flat top, long gentle sides
        top = min(top, 168) + jag(x, 7, 1)  # Savanna level elsewhere
        for y in range(top, H):  # Down
            c = ES_ROCK_SH if d < -40 else ES_ROCK  # Shaded left side
            if y < 92 + (lm.noise(x % 480, 3) % 4) and abs(d) < 90:  # The snow cap with a ragged edge
                c = ES_SNOW  # Snow
            if y >= 168:  # The savanna
                c = ES_SAV  # Yellow-brown grass
            put(img, x, y, c)  # Paint
    return img


def es_mid():
    """L3: a lower shoulder of the mountain, softened by haze."""
    return nb.ranged([(100, 140, 0), (600, 136, 0)], 0.3, 14, 2, 3.0, mix(ES_ROCK, lm.HAZE, 0.3), ES_ROCK, mix(ES_ROCK, lm.HAZE, 0.3), ES_ROCK, mm.SKY_LIGHT, 180)  # Low shoulder


def es_forest():
    """L4: alpine desert with scattered giant groundsels."""
    img = mb.new_layer()  # Empty layer
    floor = nb.rolling(192, 2, 3, 71)  # Ground line
    nb.fill_below(img, floor, ES_SCREE_LIT, mix(ES_SCREE, lm.HAZE, 0.3))  # Scree
    for i, x in enumerate(range(0, W, 30)):  # Groundsels
        h = 10 + lm.noise(i, 72) % 6  # Height
        column(img, x, floor[x] - h, ES_STEM, floor[x])  # Stem
        nb.crown(img, x, floor[x] - h - 2, 3, ES_PLANT, mix(mm.LEAF, lm.MOUNTAIN_SH, 0.3))  # Leaf rosette
    return img


def es_near():
    """L5: near grey-brown scree with rocks."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(206, 3, 3, 73)  # Ground line
    nb.fill_below(img, tops, ES_SCREE_LIT, ES_SCREE)  # Scree
    bumps(img, tops, ((50, 14), (300, 12), (540, 18), (780, 10)), ES_ROCK, ES_ROCK_SH)  # Rocks
    return img


# ---------- Sea Peak: Mauna Kea, the summit high above a sea of clouds, red cinder cones and white telescope domes ----------

SP_SKY = [(0, mix(lm.MOUNTAIN_SH, lm.SKY, 0.3)), (90, lm.SKY), (160, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.3))]  # Deep blue high up
SP_CLOUD = mm.WHITE  # Sea of clouds
SP_CLOUD_SH = mix(mm.SKY_LIGHT, lm.HAZE, 0.5)  # Its shade
SP_SEA = mix(lm.LENS, lm.HAZE, 0.5)  # The ocean far away
SP_CONE = mix(mm.SCARF_SH, lm.HAZE, 0.45)  # Red cinder cones
SP_CONE_SH = mix(mm.BACKPACK_SH, lm.HAZE_SH, 0.4)  # Their shade
SP_CINDER = mix(mm.SCARF_SH, mm.BACKPACK_SH, 0.5)  # Near cinder
SP_CINDER_LIT = mix(mm.SCARF_SH, mm.PLATEAU, 0.5)  # Its lit edge
SP_DOME = mm.WHITE  # Telescope domes
SP_DOME_SH = mm.MASK  # Their shade


def sp_sky():
    """L0: deep blue sky."""
    img = nb.gradient(SP_SKY)  # Blue
    nb.sun(img, 300, 40, 7, mm.WHITE, mb.SUN, lm.SKY)  # Sun
    return img


def sp_clouds():
    """L1: the ocean far away, and a flat sea of clouds below the summit."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # The far ocean at the horizon
        column(img, x, 118, SP_SEA, 126)  # Thin band of sea
    for x in range(W):  # The sea of clouds
        top = 126 + int(2 * math.sin(2 * math.pi * 8 * x / W))  # Soft billows
        for y in range(top, 150):  # Down
            put(img, x, y, SP_CLOUD_SH if y > top + 12 and dither(x, y, 0.4) else SP_CLOUD)  # Cloud
    return img


def sp_far():
    """L2: the gentle summit ridge with white telescope domes on top."""
    img = nb.ranged([(240, 100, 0), (620, 96, 0)], 0.35, 15, 1, 2.0, SP_CONE, SP_CONE_SH, SP_CONE, SP_CONE_SH, SP_CLOUD, 150)  # Broad, gentle shield
    for cx in (226, 252, 606, 634):  # Telescope domes on the ridge
        top = next(y for y in range(H) if img.getpixel((cx, y))[3])  # Ridge top here
        for y in range(top - 6, top + 1):  # Dome rows
            half = int(math.sqrt(max(0, 36 - (y - top) ** 2)))  # Half circle
            for x in range(cx - half, cx + half + 1):  # Pixels
                put(img, x, y, SP_DOME if x < cx + 1 else SP_DOME_SH)  # White dome
    return img


def sp_mid():
    """L3: red cinder cones."""
    peaks = [(120, 140, 0), (300, 130, 0), (520, 144, 0), (760, 134, 0)]  # Round cones
    return nb.ranged(peaks, 0.7, 16, 2, 2.0, SP_CONE, SP_CONE_SH, SP_CONE, SP_CONE_SH, SP_CLOUD, 186)  # Cones


def sp_forest():
    """L4: bare red-brown cinder slope with silversword plants."""
    img = mb.new_layer()  # Empty layer
    floor = nb.rolling(192, 2, 3, 81)  # Ground line
    nb.fill_below(img, floor, SP_CINDER_LIT, mix(SP_CINDER, lm.HAZE, 0.3))  # Cinder
    for i, x in enumerate(range(0, W, 36)):  # Silverswords
        nb.crown(img, x + lm.noise(i, 82) % 8, floor[x] - 3, 3, lm.SNOW, lm.HAZE)  # Silver ball
    return img


def sp_near():
    """L5: near red cinder with dark lava stones."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(206, 3, 3, 83)  # Ground line
    nb.fill_below(img, tops, SP_CINDER_LIT, SP_CINDER)  # Cinder
    bumps(img, tops, ((90, 12), (320, 16), (560, 10), (820, 14)), mix(mm.STRAP, lm.HAZE, 0.3), mm.STRAP)  # Lava stones
    return img


THEMES = {  # One entry per Hard theme (D-061): its six layers and its ground tiles for the preview
    "LavaSlope": ((ls_sky, ls_clouds, ls_far, ls_mid, ls_forest, ls_near), "lava"),
    "StormRidge": ((sr_sky, sr_clouds, sr_far, sr_mid, sr_forest, sr_near), "storm"),
    "EagleForest": ((ef_sky, ef_clouds, ef_far, ef_mid, ef_forest, ef_near), "eagle"),
    "BonsaiForest": ((bf_sky, bf_clouds, bf_far, bf_mid, bf_forest, bf_near), "bonsai"),
    "GlacierHorn": ((gh_sky, gh_clouds, gh_far, gh_mid, gh_forest, gh_near), "glacier"),
    "EquatorSummit": ((es_sky, es_clouds, es_far, es_mid, es_forest, es_near), "equator"),
    "SeaPeak": ((sp_sky, sp_clouds, sp_far, sp_mid, sp_forest, sp_near), "sea"),
}


def main():
    """Draws and saves every set and the preview grid."""
    shots = []  # One preview screen per theme
    for theme, (makers, ground) in THEMES.items():  # Each theme
        out = os.path.join(nb.BACKDROPS, theme)  # Its folder
        os.makedirs(out, exist_ok=True)  # Make it if missing
        layers = [make() for make in makers]  # Draw L0 to L5
        for i, img in enumerate(layers):  # Save each
            img.save(os.path.join(out, "L%d.png" % i))  # L0.png ... L5.png
        shots.append(nb.preview_screen(layers, ground))  # Preview screen
        print("Backdrop written to", out)  # Tell where
    rows = (len(shots) + 1) // 2  # Two screens per row
    sheet = Image.new("RGBA", (968, rows * 278 - 8), mm.OUTLINE + (255,))  # Grid with dark gaps
    for i, shot in enumerate(shots):  # Place each screen
        sheet.paste(shot, ((i % 2) * 488, (i // 2) * 278))  # 8-pixel gaps
    os.makedirs(os.path.dirname(PREVIEW), exist_ok=True)  # Make the folder
    sheet.save(PREVIEW)  # Normal size, to keep the file small
    print("Preview written to", PREVIEW)  # Tell where


if __name__ == "__main__":
    main()  # Run when started directly
