"""Draws the moving background layers for the five Plains Normal themes (decision D-070).

Run from the Unity project folder:  python Tools/Art/make_plains_normal_backdrops.py
It writes L0.png to L5.png (960x270 each) to Assets/Art/Level/Sprites/Backdrop/<Theme>/ for
CandabaWetland (Candaba Swamp, Pampanga), LaharPlain (Pinatubo lahar plain), RiverDelta (Agno River delta,
Pangasinan), FloatingVillage (Agusan Marsh, Mindanao) and FloatingMarket (Mekong Delta, Vietnam),
plus a preview at .scratch/plains-normal/backdrop-preview.png.

Each set keeps the six layers and the scrolling of the Plains Easy backdrops (make_plains_easy_backdrops.py):
L0 sky, L1 clouds, L2 far line, L3 flat middle ground, L4 near band, L5 near bank.
Plains are flat, so the horizon sits low and the sky is big. The rows behind the explorer (100 to 220)
stay mid-toned and soft, so the explorer, the pickups and the ledges stand out.
Every layer tiles sideways without a seam. Colours are palette colours or even mixes of two of them.
"""
import math  # sin, sqrt, hypot and pi for the shapes
import os  # Folders and paths
from PIL import Image  # Saves the preview
import make_mainmenu_art as mm  # Palette and image helpers
import make_landform_map_art as lm  # Map colours and the fixed noise
import make_mountains_backdrop as mb  # Pixels, mix and the wrap helper
import make_normal_backdrops as nb  # Gradient, sun, clouds, ranges, crowns, rolling lines and the preview
import make_plains_easy_backdrops as pe  # Band rows, haze and palms from the Plains Easy set

W, H = mb.W, mb.H  # 960x270 layers
mix, put, column = mb.mix, mb.put, mb.column  # Shared drawing helpers
band_index, haze_t = pe.band_index, pe.haze_t  # Band rows and haze from the Plains Easy set
PREVIEW = os.path.join(mb.ROOT, ".scratch", "plains-normal", "backdrop-preview.png")  # Where the preview goes


def flock(img, x, y, color, n):
    """A small V-shaped flock of far ducks. Each duck is a tiny 'v', and the flock spreads out in a V."""
    for dx, dy in ((0, 0), (-5, 3), (5, 3), (-10, 6), (10, 6))[:n]:  # Duck places in a V
        put(img, x + dx - 1, y + dy - 1, color)  # Left wing tip
        put(img, x + dx, y + dy, color)  # Body
        put(img, x + dx + 1, y + dy - 1, color)  # Right wing tip


def arayat(img, cx, top, half, base, rock, rock_sh):
    """Mount Arayat: one isolated, rounded mountain on the flat plain. Its top is round and its feet meet the plain."""
    for x in range(cx - half, cx + half + 1):  # Each column of the mountain
        t = (x - cx) / half  # -1 at the left foot, 0 at the top, 1 at the right foot
        y0 = int(base - (base - top) * math.sqrt(max(0.0, 1 - t * t)) + mb.jag(x, 13, 2))  # Round top edge with a little roughness
        for y in range(y0, base + 1):  # Down to the foot of the mountain
            put(img, x, y, rock_sh if t < -0.2 else rock)  # Shaded left side, sunny right side


def pinatubo(img, cx, top, half_top, slope, rock, rock_sh, rim):
    """Mount Pinatubo from far away: a wide cone with a flat, cut-off top and a broken rim. No smoke."""
    for x in range(cx - 200, cx + 201):  # Each column of the cone
        d = abs(x - cx)  # Distance from the middle
        y0 = top + int(max(0, d - half_top) * slope)  # Flat top in the middle, sides slope down
        if d > half_top - 10 and lm.noise(x // 6 % 160, 3) % 10 < 2:  # Gaps in the rim, in a few places
            y0 += 4  # The rim dips down here
        put(img, x, y0, rim)  # Dusty top edge of the cone
        for y in range(y0 + 1, H):  # Rock from under the rim down
            put(img, x, y, rock_sh if x < cx else rock)  # Shaded left side, sunny right side


def braid(img, base, amp, waves, phase, half, color):
    """One channel of a braided river: a thin band that winds left and right across the plain."""
    for x in range(W):  # Each column
        cy = base + int(amp * math.sin(2 * math.pi * waves * x / W + phase))  # Centre row, wound by a wave that loops across the picture
        for y in range(cy - half, cy + half + 1):  # Each row across the channel
            put(img, x, y, color)  # Water pixel


# ---------- CandabaWetland: Candaba Swamp, Pampanga. Soft morning sky, shallow ponds that mirror it, marsh grass, far ducks ----------

CW_HORIZON = 142  # Row of the flat horizon
CW_SKY = [(0, lm.SKY), (90, mm.SKY_LIGHT), (150, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.3))]  # Soft morning blue, warm near the horizon
CW_FAR = mix(lm.HAZE, mm.SKY_LIGHT, 0.5)  # Faint Sierra Madre, very pale
CW_FAR_SH = mix(lm.HAZE, lm.MOUNTAIN, 0.2)  # Its shade, still faint
CW_ARAYAT = mix(lm.MOUNTAIN, lm.HAZE, 0.45)  # Mount Arayat, a little darker than the range
CW_ARAYAT_SH = mix(lm.MOUNTAIN_SH, lm.HAZE, 0.5)  # Its shaded left side
CW_MIST = mm.SKY_LIGHT  # Mist at the foot of the range
CW_WATER = mix(lm.SKY, mm.SKY_LIGHT, 0.5)  # Shallow pond, a mirror of the sky
CW_SHINE = mm.WHITE  # Sky sparkles on the water
CW_GRASS = mix(mm.PLAINS, mm.HILLS, 0.5)  # Marsh grass, mid green
CW_REED =mix(mm.LEAF, lm.HAZE, 0.25)  # Reeds
CW_REED_LT = mix(mm.HILLS, lm.HAZE, 0.2)  # Sunny reed tips
CW_BIRD = mix(lm.MOUNTAIN, mm.OUTLINE, 0.5)  # Far ducks, dark but soft
CW_NEAR = mix(mm.HILLS_SH, lm.MOUNTAIN_SH, 0.3)  # Near wet bank
CW_NEAR_LIT = mix(mm.PLAINS, lm.HAZE, 0.25)  # Sunny edge of the bank


def cw_sky():
    """L0: soft morning sky with a low, pale sun on the left."""
    img = nb.gradient(CW_SKY)  # Blue fading to warm near the horizon
    nb.sun(img, 160, 58, 7, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4))  # Soft morning sun
    return img  # Give the picture back


def cw_clouds():
    """L1: thin morning clouds and two small flocks of far ducks in a V."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((120, 40, 130), (420, 22, 90), (700, 52, 150), (900, 30, 80)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # One soft cloud
    flock(img, 230, 96, CW_BIRD, 5)  # A flock of ducks, far away
    flock(img, 560, 104, CW_BIRD, 4)  # A second, smaller flock
    return img  # Give the picture back


def cw_far():
    """L2: a faint, low line of the Sierra Madre, and Mount Arayat alone on the plain."""
    peaks = [(30, 128, 0), (190, 124, 0), (350, 130, 0), (520, 126, 0), (700, 131, 0), (880, 125, 0)]  # Low, wide peaks: place, top row, snow depth
    img = nb.ranged(peaks, 0.2, 9, 1, 1.0, CW_FAR, CW_FAR_SH, CW_FAR, CW_FAR_SH, CW_MIST, 138)  # Faint low range with mist at its foot
    arayat(img, 380, 108, 64, 150, CW_ARAYAT, CW_ARAYAT_SH)  # One round mountain on the plain
    return img  # Give the picture back


def cw_mid():
    """L3: two calm, wide bands of pond and two of marsh grass, with a few soft sky glints on the water."""
    img = mb.new_layer()  # Empty layer
    for y in range(CW_HORIZON, H):  # Each row from the horizon down
        k = (y - CW_HORIZON) // 22  # Which wide band this row is in, 22 rows each
        water = k % 2 == 0  # Every other band is pond
        base = CW_WATER if water else CW_GRASS  # Pond or marsh colour
        for x in range(W):  # Each pixel of the row
            c = mix(CW_SHINE, base, 0.5) if water and y % 8 == 3 and (x // 8 + y) % 15 == 0 else base  # Sparse soft sky glint in a short dash
            put(img, x, y, mix(c, lm.HAZE, haze_t(y, CW_HORIZON)))  # Paint it, a little hazy far away
    return img  # Give the picture back


def cw_reeds():
    """L4: small reed clumps standing along the edges of the bands. The rest stays see-through."""
    img = mb.new_layer()  # Empty layer, see-through
    for j, edge in enumerate((164, 208, 252)):  # Band edges where reeds grow
        for i, x in enumerate((80, 260, 440, 620, 800)):  # Clump places along each edge
            base = edge + lm.noise(i, 101 + j) % 3  # Each clump sits on its edge, a row lower or higher
            for dx, h in ((-2, 9), (0, 12), (2, 8)):  # Three reed stalks with different heights
                column(img, x + dx + 30 * j, base - h, CW_REED, base + 1)  # One stalk, from its tip down to the edge
                put(img, x + dx + 30 * j, base - h, CW_REED_LT)  # Sunny tip
    return img  # Give the picture back


def cw_near():
    """L5: a low, wet bank just behind the trail."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(220, 1, 4, 61)  # A gently wavy line near the trail
    nb.fill_below(img, tops, CW_NEAR_LIT, CW_NEAR)  # Bank with a sunny top edge
    return img  # Give the picture back


# ---------- LaharPlain: Pinatubo lahar plain. Clear pale sky, a cut-off volcano far away, grey-beige sand, braided river ----------

LH_HORIZON = 150  # Row of the flat horizon
LH_SKY = [(0, mix(lm.SKY, mm.WHITE, 0.2)), (100, lm.SKY), (150, mm.SKY_LIGHT)]  # Clear, pale blue sky
LH_FAR = mix(mm.MASK_SH, lm.HAZE, 0.5)  # Pinatubo, grey-blue with haze
LH_FAR_SH = mix(mm.MASK_SH, lm.MOUNTAIN_SH, 0.4)  # Its shaded left side
LH_RIM = mix(mm.MASK, mm.WHITE, 0.3)  # Pale, dusty top edge of the rim
LH_SAND = mix(mm.MASK, mm.VEST, 0.4)  # Grey-beige volcanic sand
LH_SAND_DK = mix(mm.MASK_SH, mm.VEST_SH, 0.4)  # Darker sand ripples
LH_WATER = mix(lm.SKY, lm.HAZE, 0.4)  # Shallow braided channel, pale blue
LH_BUSH = mix(mm.LEAF, mm.MASK_SH, 0.45)  # Dusty green bush
LH_BUSH_LT = mix(mm.HILLS, mm.MASK, 0.3)  # Sunny bush top
LH_NEAR = mix(mm.MASK_SH, mm.VEST_SH, 0.3)  # Near sand bank
LH_NEAR_LIT = mix(mm.MASK, mm.WHITE, 0.2)  # Sunny edge of the bank


def lh_sky():
    """L0: clear pale sky with a small sun."""
    img = nb.gradient(LH_SKY)  # Pale blue fading to paler at the horizon
    nb.sun(img, 790, 44, 7, mb.SUN, mb.SUN_GLOW, mix(lm.SKY, mb.SUN_GLOW, 0.3))  # Small clear sun
    return img  # Give the picture back


def lh_clouds():
    """L1: two thin white clouds, no more, so the sky stays clear."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((150, 36, 110), (720, 24, 90)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # One soft cloud
    return img  # Give the picture back


def lh_far():
    """L2: faint low ridges far to each side, and Mount Pinatubo in the middle with its flat, cut-off top."""
    img = nb.ranged([(60, 132, 0), (300, 134, 0), (700, 132, 0), (920, 130, 0)], 0.25, 4, 2, 1.5, LH_FAR, LH_FAR_SH, LH_FAR, LH_FAR_SH, mm.SKY_LIGHT, 142)  # Low, pale side ridges
    pinatubo(img, 380, 110, 58, 0.36, LH_FAR, LH_FAR_SH, LH_RIM)  # The volcano with its broken rim, no smoke
    return img  # Give the picture back


def lh_mid():
    """L3: a flat grey-beige plain of volcanic sand with faint ripples, and four braided channels of pale water."""
    img = mb.new_layer()  # Empty layer
    for y in range(LH_HORIZON, H):  # Each row from the horizon down
        for x in range(W):  # Each pixel of the row
            c = LH_SAND_DK if (x // 12 + 2 * y) % 5 == 0 and y % 2 == 0 else LH_SAND  # Faint ripple lines in the sand
            put(img, x, y, mix(c, lm.HAZE, haze_t(y, LH_HORIZON)))  # Paint it, a little hazy far away
    for base, amp, waves, phase, half in ((160, 3, 1, 2.1, 1), (176, 4, 2, 0.0, 2), (196, 3, 3, 1.3, 2), (214, 5, 2, 0.6, 3)):  # Four channels: place, wiggle, waves, phase, half width
        braid(img, base, amp, waves, phase, half, LH_WATER)  # Draw one braided channel
    return img  # Give the picture back


def lh_bushes():
    """L4: green bushes coming back on the plain. The rest stays see-through."""
    img = mb.new_layer()  # Empty layer, see-through
    for i, x in enumerate((40, 150, 260, 400, 520, 640, 780, 900)):  # Bush places, spread over the plain
        cy = 200 + lm.noise(i, 111) % 8  # Each bush sits a little higher or lower
        nb.crown(img, x, cy, 4 + lm.noise(i, 112) % 3, LH_BUSH_LT, LH_BUSH)  # One round bush, sunny top right
    return img  # Give the picture back


def lh_near():
    """L5: a low sand bank just behind the trail."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(219, 2, 2, 71)  # A gently wavy line near the trail
    nb.fill_below(img, tops, LH_NEAR_LIT, LH_NEAR)  # Bank with a sunny top edge
    return img  # Give the picture back


# ---------- RiverDelta: Agno River delta, Pangasinan. Bright sky, far sea, green delta cut by channels and sandbars ----------

RD_HORIZON = 132  # Row where the delta starts, under the far sea
RD_SEA_TOP = 124  # Row of the far sea line
RD_SKY = [(0, mix(lm.SKY, mm.WHITE, 0.1)), (90, lm.SKY), (125, mm.SKY_LIGHT)]  # Bright, clear sky
RD_SEA = mix(lm.MOUNTAIN, lm.LENS, 0.45)  # Far sea, blue-teal
RD_SEA_LT = mix(lm.LENS, mm.WHITE, 0.4)  # Sun glints on the sea
RD_GREEN = mix(mm.HILLS, mm.PLAINS, 0.3)  # Bright green delta
RD_GREEN_DK = mix(mm.HILLS_SH, mm.LEAF, 0.2)  # Dark dots in the grass
RD_WATER = mix(lm.LENS, mm.WHITE, 0.25)  # Shallow river channels
RD_WATER_DK = mix(lm.LENS, lm.MOUNTAIN, 0.3)  # Darker river banks
RD_TRUNK_ROW = 188  # Row of the wide river that comes in from the far left
RD_BRANCH = ((200, 330), (400, 420), (560, 600), (720, 860))  # Each channel: where it leaves the river (x at the bottom) and where it ends at the sea line (x at the top)
RD_SAND = mix(mm.VEST, mm.HEADLAMP, 0.4)  # Sandbars in the channels
RD_POST = mix(lm.HANDLE, lm.HAZE, 0.2)  # Bamboo stakes of the fish pens
RD_PALM = mix(mm.LEAF, lm.HAZE, 0.15)  # Palm crown
RD_PALM_SH = mix(mm.LEAF, mm.OUTLINE, 0.3)  # Shaded fronds
RD_TRUNK = mix(lm.HANDLE, lm.HAZE, 0.25)  # Palm trunk
RD_NEAR = mix(lm.PLAINS_SH, mm.HILLS_SH, 0.4)  # Near green bank
RD_NEAR_LIT = mix(mm.PLAINS, mm.HEADLAMP, 0.25)  # Sunny edge of the bank


def rd_sky():
    """L0: bright sky with a strong sun."""
    img = nb.gradient(RD_SKY)  # Bright blue, pale at the horizon
    nb.sun(img, 240, 40, 10, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.5))  # Bright sun
    return img  # Give the picture back


def rd_clouds():
    """L1: bright white clouds with pale undersides."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((90, 30, 120), (400, 56, 140), (700, 24, 110), (900, 60, 90)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # One cloud
    return img  # Give the picture back


def rd_far():
    """L2: a thin strip of far sea under the sky, with a few sunny glints."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        column(img, x, RD_SEA_TOP, RD_SEA, RD_HORIZON)  # Sea from its top line down to the delta
        if x % 24 < 4:  # Short glints every 24 pixels (960 divides by 24)
            put(img, x, RD_SEA_TOP + 3 + (x // 24) % 2, RD_SEA_LT)  # One glint pixel, on one of two rows
    return img  # Give the picture back


def rd_branch_x(k, y):
    """Centre column of channel k on row y: it leaves the wide river at the bottom and fans out to the sea line at the top."""
    x0, x1 = RD_BRANCH[k]  # Where the channel leaves the river, and where it ends at the sea line
    f = (y - RD_SEA_TOP - 8) / (RD_TRUNK_ROW - RD_SEA_TOP - 8)  # 0 at the sea line, 1 at the river
    return int(x1 + (x0 - x1) * max(0.0, min(1.0, f)))  # Move from the far end toward the river


def rd_mid():
    """L3: a flat green delta with dark land patches, a wide river from the far left that splits into four channels fanning toward the sea, pale sand bars between the channels, and fish-pen stakes in two channels."""
    img = mb.new_layer()  # Empty layer
    for y in range(RD_HORIZON, H):  # Each row from the far delta down
        for x in range(W):  # Each pixel of the row
            c = RD_GREEN_DK if (x + 5 * y) % 16 == 0 else RD_GREEN  # Sparse dark dots give a grassy look
            put(img, x, y, mix(c, lm.HAZE, haze_t(y, RD_HORIZON)))  # Paint it, a little hazy far away
    for cx, cy, rx, ry in ((120, 150, 30, 4), (300, 166, 44, 5), (640, 148, 36, 4), (880, 172, 48, 6), (30, 204, 30, 5), (520, 160, 26, 4)):  # Dark land patches: centre, half width, half height
        for dy in range(-ry, ry + 1):  # Each row of the patch
            for dx in range(-rx, rx + 1):  # Each pixel across the patch
                if (dx / rx) ** 2 + (dy / ry) ** 2 <= 1:  # Inside the oval
                    put(img, cx + dx, cy + dy, RD_GREEN_DK)  # Dark land pixel
    for x in range(W):  # The wide river, from the far left across the whole picture
        cy = RD_TRUNK_ROW + int(4 * math.sin(2 * math.pi * x / W))  # Centre row, wavy but looping across the picture
        for y in range(cy - 7, cy + 8):  # Rows across the river
            put(img, x, y, RD_WATER_DK if abs(y - cy) > 4 else RD_WATER)  # Darker banks, lighter middle
    for k in range(4):  # Four channels, each one leaving the river and fanning toward the sea line
        for y in range(RD_SEA_TOP + 8, RD_TRUNK_ROW + 1):  # Each row from the sea line down to the river
            f = (y - RD_SEA_TOP - 8) / (RD_TRUNK_ROW - RD_SEA_TOP - 8)  # 0 at the sea line, 1 at the river
            cx = rd_branch_x(k, y)  # Centre column of this channel on this row
            half = 1 + int(2 * f)  # Wider near the river
            for x in range(cx - half, cx + half + 1):  # Each pixel across the channel
                put(img, x, y, RD_WATER)  # Shallow water
    for y_bar in (150, 166):  # Two rows of sand bars
        for k in range(3):  # One pale bar between each pair of channels
            left = rd_branch_x(k, y_bar)  # Left channel centre
            right = rd_branch_x(k + 1, y_bar)  # Right channel centre
            mid = (left + right) // 2  # Middle of the land between them
            half_bar = (right - left) // 2 - 4  # The bar stops a little short of each channel
            for x in range(mid - half_bar, mid + half_bar + 1):  # Pixels across the bar
                put(img, x, y_bar, RD_SAND)  # Sand pixel
                put(img, x, y_bar + 1, RD_SAND)  # Second sand row
    for k in (1, 2):  # Fish pens in the two middle channels
        for y in range(150, RD_TRUNK_ROW - 2, 6):  # A stake pair every 6 rows
            cx = rd_branch_x(k, y)  # Centre of the channel on this row
            half = 1 + int(2 * (y - RD_SEA_TOP - 8) / (RD_TRUNK_ROW - RD_SEA_TOP - 8))  # Same half width as the channel
            put(img, cx - half, y, RD_POST)  # Left stake
            put(img, cx + half, y, RD_POST)  # Right stake
    return img  # Give the picture back


def rd_posts():
    """L4: coconut palms on the sand and two fish pens of bamboo stakes in the water. The rest stays see-through."""
    img = mb.new_layer()  # Empty layer, see-through
    for i, x in enumerate((40, 230, 560, 860)):  # Coconut palm places
        pe.palm(img, x, 200 + lm.noise(i, 141) % 4, 22 + lm.noise(i, 142) % 6, RD_PALM, RD_PALM_SH, RD_TRUNK)  # One palm with a varied height
    return img  # Give the picture back


def rd_near():
    """L5: a low green bank just behind the trail."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(219, 2, 3, 83)  # A gently wavy line near the trail
    nb.fill_below(img, tops, RD_NEAR_LIT, RD_NEAR)  # Bank with a sunny top edge
    return img  # Give the picture back


# ---------- FloatingVillage: Agusan Marsh, Mindanao. Humid hazy sky, dark swamp forest, calm dark water, houses on rafts ----------

FV_HORIZON = 136  # Row of the flat horizon, where the water starts
FV_SKY = [(0, mix(lm.SKY, lm.HAZE, 0.5)), (90, mix(mm.SKY_LIGHT, lm.HAZE, 0.4)), (150, mix(mm.SKY_LIGHT, mm.WHITE, 0.5))]  # Humid, hazy blue-grey sky, pale at the horizon
FV_CLOUD = mix(mm.WHITE, lm.HAZE, 0.3)  # Humid clouds
FV_CLOUD_SH = mix(lm.HAZE, lm.HAZE_SH, 0.5)  # Their shaded undersides
FV_FOREST = mix(mm.LEAF, lm.HAZE, 0.3)  # Dark green swamp forest on the far horizon
FV_FOREST_LT = mix(mm.LEAF, mm.HILLS, 0.4)  # Lit top edge of the trees
FV_FOREST_SH = mix(mm.LEAF, mm.OUTLINE, 0.35)  # Shade between the crowns
FV_WATER = mix(mm.LEAF, lm.MOUNTAIN_SH, 0.5)  # Calm dark water
FV_WATER_LT = mix(mm.LEAF, lm.HAZE, 0.3)  # Water with sky stripes
FV_WALL = mix(mm.VEST, mm.MASK, 0.25)  # Wooden house wall, sunny side
FV_WALL_SH = mm.VEST_SH  # Shaded wall
FV_ROOF = mix(mm.GROUND, mm.OUTLINE, 0.35)  # Brown thatch roof
FV_RAFT = mix(lm.HANDLE, mm.OUTLINE, 0.35)  # Raft of logs under each house
FV_REED = mix(mm.LEAF, mm.HILLS, 0.3)  # Reed stalks
FV_CATTAIL = mix(mm.GROUND_SH, mm.OUTLINE, 0.3)  # Brown cattail heads
FV_HYACINTH = mix(mm.HILLS, mm.LEAF, 0.4)  # Water hyacinth leaves, fresh green
FV_HYACINTH_SH = mix(mm.LEAF, mm.OUTLINE, 0.3)  # Shade under the hyacinth leaves
FV_FLOWER = lm.LENS  # Hyacinth flowers, blue


def fv_sky():
    """L0: humid, hazy sky with no sun."""
    return nb.gradient(FV_SKY)  # Blue-grey fading to pale at the horizon


def fv_clouds():
    """L1: low, wide humid clouds and a band of mist over the forest."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((140, 112, 200), (520, 100, 240), (860, 116, 180)):  # Low cloud places and widths
        nb.flat_cloud(img, cx, cy, w, FV_CLOUD, FV_CLOUD_SH)  # One humid cloud
    nb.mist_band(img, 116, 22, mm.WHITE)  # A dotted band of mist over the forest line
    return img  # Give the picture back


def fv_far():
    """L2: a dark green swamp forest along the far horizon, with taller trees poking up."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        top = 120 + int(3 * math.sin(2 * math.pi * 6 * x / W)) + lm.noise(x // 5 % 192, 17) % 6 - 2  # Bumpy forest top that loops across the picture
        column(img, x, top, FV_FOREST, H)  # Forest from its top edge down
        put(img, x, top, FV_FOREST_LT)  # Lit top edge
    for i, x in enumerate(range(10, W, 30)):  # Taller trees, one every 30 pixels
        nb.crown(img, x, 114 + lm.noise(i, 19) % 4, 3 + lm.noise(i, 20) % 2, FV_FOREST_LT, FV_FOREST_SH)  # One tree crown poking up
    return img  # Give the picture back


def fv_house(img, x, base):
    """A house on a floating raft: a raft of logs, a wooden wall with a door, and a sloping thatch roof."""
    for dx in range(-14, 15):  # The raft, 29 pixels wide
        put(img, x + dx, base, FV_RAFT)  # Top row of the raft
        put(img, x + dx, base + 1, FV_RAFT)  # Lower row of the raft
    for dy in range(-9, 0):  # Wall rows above the raft
        for dx in range(-9, 10):  # Wall width, 19 pixels
            put(img, x + dx, base + dy, FV_WALL_SH if dx < -6 else FV_WALL)  # Shaded left edge, sunny wall
    for i in range(5):  # Roof rows, narrower each row going up
        for dx in range(-(11 - 2 * i), 12 - 2 * i):  # Pixels across this roof row
            put(img, x + dx, base - 10 - i, FV_ROOF)  # Thatch pixel
    for dy in (-3, -2):  # Door, two pixels tall
        put(img, x, base + dy, mm.OUTLINE)  # Door pixel


def fv_mid():
    """L3: calm dark water with a few sky stripes, and five houses on rafts in the middle distance."""
    img = mb.new_layer()  # Empty layer
    for y in range(FV_HORIZON, H):  # Each row from the forest down
        for x in range(W):  # Each pixel of the row
            c = FV_WATER_LT if (x // 8 + 2 * y) % 5 == 0 and y % 4 == 1 else FV_WATER  # Few sky stripes on the calm water
            put(img, x, y, mix(c, lm.HAZE, haze_t(y, FV_HORIZON)))  # Paint it, a little hazy far away
    for x, base in ((90, 178), (300, 170), (520, 182), (700, 172), (900, 180)):  # House places: x and raft row
        fv_house(img, x, base)  # Draw one house on its raft
    return img  # Give the picture back


def fv_reeds():
    """L4: reed stalks and brown cattail heads at the water's edge. The rest stays see-through."""
    img = mb.new_layer()  # Empty layer, see-through
    for i, x in enumerate((30, 170, 350, 470, 610, 800, 940)):  # Reed clump places
        base = 198 + lm.noise(i, 121) % 6  # Each clump stands in the water
        for dx, h in ((-3, 14), (-1, 18), (1, 12), (3, 16)):  # Four stalks with different heights
            column(img, x + dx, base - h, FV_REED, base + 2)  # One stalk, from its tip down into the water
        for dy in range(3):  # Cattail head on the tallest stalk
            put(img, x - 1, base - 18 + dy, FV_CATTAIL)  # Brown head pixel
    return img  # Give the picture back


def fv_near():
    """L5: dark water at the front, with clumps of water hyacinth floating on it."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(216, 1, 2, 91)  # A gently wavy water line, just above the trail
    nb.fill_below(img, tops, FV_WATER_LT, FV_WATER)  # Dark water with a lit top edge
    for i, x in enumerate(range(16, W, 64)):  # Hyacinth clumps, one every 64 pixels (960 divides by 64)
        cx = x + lm.noise(i, 131) % 16 - 8  # Each clump moves a little from its place
        cy = 216 + lm.noise(i, 132) % 4  # Clumps float at the water's edge, in front of the trail line
        for dx, dy, r in ((0, 0, 4), (-6, 3, 3), (6, 2, 3)):  # Three round leaves in a clump
            nb.crown(img, cx + dx, cy + dy, r, FV_HYACINTH, FV_HYACINTH_SH)  # One leaf
        put(img, cx + 2, cy - 4, FV_FLOWER)  # A blue flower
        put(img, cx - 3, cy - 3, FV_FLOWER)  # A second blue flower
    return img  # Give the picture back


# ---------- FloatingMarket: Mekong Delta, Vietnam. Warm golden sky, wide brown river, boats with fruit and cone hats, a wooden pier ----------

FM_HORIZON = 146  # Row of the flat horizon, where the river starts
FM_SKY = [(0, mix(lm.SKY, mb.SUN_GLOW, 0.35)), (90, mix(mm.HEADLAMP, mb.SUN_GLOW, 0.3)), (150, mb.SUN_GLOW)]  # Golden sky, warmest at the horizon
FM_CLOUD_SH = mix(mm.HEADLAMP, mb.SUN_GLOW, 0.5)  # Warm underside of the clouds
FM_BANK = mix(lm.PLAINS_SH, mm.HILLS_SH, 0.3)  # Far bank, flat green
FM_CROWN = mix(mm.LEAF, lm.HAZE, 0.2)  # Far palm crown
FM_PALM = mix(mm.LEAF, lm.HAZE, 0.15)  # Palm crown
FM_PALM_SH = mix(mm.LEAF, mm.OUTLINE, 0.3)  # Shaded fronds
FM_TRUNK = mix(lm.HANDLE, lm.HAZE, 0.25)  # Palm trunk
FM_RIVER = mix(mm.GROUND, mm.GROUND_SH, 0.35)  # Wide brown river
FM_RIVER_DK = mix(mm.GROUND_SH, lm.HANDLE, 0.4)  # Dark current lines
FM_RIVER_LT = mix(mm.GROUND, mb.SUN_GLOW, 0.35)  # Sunny glints on the water
FM_HULL = mix(lm.HANDLE, mm.OUTLINE, 0.25)  # Dark wood hull
FM_HULL_LT = lm.HANDLE  # Sunny rim of a hull
FM_HAT = mm.VEST  # Cone hat
FM_HAT_SH = mm.VEST_SH  # Tip of the cone hat
FM_PIER = mix(lm.HANDLE, mm.HEADLAMP, 0.25)  # Light wood deck
FM_PIER_SH = mix(lm.HANDLE, mm.OUTLINE, 0.35)  # Plank joints, posts and shadows


def fm_sky():
    """L0: warm golden sky with a big low sun."""
    img = nb.gradient(FM_SKY)  # Golden blue fading to warm gold
    nb.sun(img, 300, 92, 14, mb.SUN, mb.SUN_GLOW, mix(mb.SUN_GLOW, mm.HEADLAMP, 0.5))  # Big low sun
    return img  # Give the picture back


def fm_clouds():
    """L1: a few clouds with warm undersides."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((120, 40, 140), (460, 58, 110), (760, 30, 160)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.HEADLAMP, FM_CLOUD_SH)  # One cloud with a warm underside
    return img  # Give the picture back


def fm_far():
    """L2: a flat green far bank with a row of small palms."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        column(img, x, 138 + int(2 * math.sin(2 * math.pi * 4 * x / W)), FM_BANK, FM_HORIZON)  # Flat far bank, a thin strip
    for x in range(20, W, 48):  # Far palms, one every 48 pixels (960 divides by 48)
        column(img, x, 130, FM_TRUNK, 140)  # Thin trunk
        nb.crown(img, x, 128, 3, FM_CROWN, FM_PALM_SH)  # Small palm crown
    return img  # Give the picture back


def fm_fruit_boat(img, x, base, hull, fruit):
    """A small market boat: a low wooden hull, a heap of coloured fruit, and a seller in a cone hat."""
    for dy in range(4):  # Hull rows, from the water up to the rim
        half = 8 + 2 * dy  # The hull is wider at the top
        for dx in range(-half, half + 1):  # Each pixel across the row
            put(img, x + dx, base - dy, FM_HULL_LT if dy == 3 else hull)  # Sunny rim on top
    for i, dx in enumerate((-10, -5, 0, 5, 10)):  # Five heaps of fruit along the boat
        nb.crown(img, x + dx, base - 7 - (i % 2), 3, fruit[i % 3], fruit[i % 3])  # One bigger fruit heap, plain colour
    for i in range(4):  # Cone hat rows, from the widest at the bottom to the tip
        for dx in range(-(3 - i), 4 - i):  # Pixels across this hat row
            put(img, x + dx, base - 11 - i, FM_HAT_SH if i == 3 else FM_HAT)  # Tan hat, darker at the tip


def fm_boats():
    """L4: a row of small market boats in the middle distance, and one palm on the bank."""
    img = mb.new_layer()  # Empty layer, see-through
    hulls = (FM_HULL, mix(mm.SCARF, lm.HANDLE, 0.35), mix(mm.LEAF, lm.HANDLE, 0.4))  # Three hull colours
    fruit = ((mm.HELMET, mm.SCARF, mm.LEAF), (mm.PLATEAU, mm.HELMET, mm.SCARF), (mm.LEAF, mm.PLATEAU, mm.HELMET))  # Fruit colours for each boat
    for i, (x, base) in enumerate(((60, 190), (170, 180), (300, 194), (420, 184), (560, 192), (680, 180), (800, 190), (910, 184))):  # Eight boat places and water rows
        fm_fruit_boat(img, x, base, hulls[i % 3], fruit[i % 3])  # One boat
    pe.palm(img, 360, 200, 24, FM_PALM, FM_PALM_SH, FM_TRUNK)  # One palm on the middle bank
    return img  # Give the picture back


def fm_mid():
    """L3: a wide brown river with wavy dark current lines and sunny glints."""
    img = mb.new_layer()  # Empty layer
    for y in range(FM_HORIZON, H):  # Each row from the far bank down
        for x in range(W):  # Each pixel of the row
            yy = y + int(1.5 * math.sin(2 * math.pi * 2 * x / W))  # Wavy current lines, looping across the picture
            c = FM_RIVER_DK if yy % 6 == 0 else FM_RIVER  # Dark current lines
            if y % 5 == 2 and (x + 7 * (y // 5)) % 24 < 12:  # Soft ripple dashes, 12 pixels long, one row in five
                c = mix(c, FM_RIVER_LT, 0.5)  # Soft, not bright
            put(img, x, y, mix(c, mm.HEADLAMP, haze_t(y, FM_HORIZON)))  # Paint it, with warm haze far away
    return img  # Give the picture back


def fm_pier():
    """L5: a low wooden pier across the front, with a deck, joints and posts going down into the river."""
    img = mb.new_layer()  # Empty layer
    deck = 222  # Top row of the deck, level with the trail
    for x in range(W):  # Each column
        column(img, x, deck + 5, FM_RIVER_DK)  # River water under the pier
        for y in range(deck, deck + 4):  # The four deck rows
            put(img, x, y, FM_PIER_SH if x % 12 == 0 else FM_PIER)  # Plank joints every 12 pixels
        put(img, x, deck + 4, FM_PIER_SH)  # Shadow under the deck
    for x in range(30, W, 60):  # Posts, one every 60 pixels (960 divides by 60)
        column(img, x, deck + 5, FM_PIER_SH)  # Post going down into the water
        column(img, x + 1, deck + 5, FM_PIER_SH)  # Two pixels wide
    return img  # Give the picture back


THEMES = {  # One entry per Plains Normal theme (D-070): its six layers and its ground tiles for the preview
    "CandabaWetland": ((cw_sky, cw_clouds, cw_far, cw_mid, cw_reeds, cw_near), "rice"),  # Ponds and marsh
    "LaharPlain": ((lh_sky, lh_clouds, lh_far, lh_mid, lh_bushes, lh_near), "ash"),  # Grey-beige volcanic sand
    "RiverDelta": ((rd_sky, rd_clouds, rd_far, rd_mid, rd_posts, rd_near), "pasture"),  # Green delta and channels
    "FloatingVillage": ((fv_sky, fv_clouds, fv_far, fv_mid, fv_reeds, fv_near), "moss"),  # Swamp and houses on rafts
    "FloatingMarket": ((fm_sky, fm_clouds, fm_far, fm_mid, fm_boats, fm_pier), "river"),  # Brown river and market boats
}


def main():
    """Draws and saves every set and the preview grid."""
    shots = []  # One preview screen per theme
    for theme, (makers, ground) in THEMES.items():  # Each theme
        out = os.path.join(nb.BACKDROPS, theme)  # Its folder
        os.makedirs(out, exist_ok=True)  # Make it if missing
        layers = [make() for make in makers]  # Draw L0 to L5
        for i, img in enumerate(layers):  # Save each one
            img.save(os.path.join(out, "L%d.png" % i))  # L0.png ... L5.png
        shots.append(nb.preview_screen(layers, ground))  # Preview screen
        print("Backdrop written to", out)  # Tell where it went
    sheet = Image.new("RGBA", (3 * 480 + 2 * 8, 2 * 270 + 8), mm.OUTLINE + (255,))  # Grid of three screens per row, with dark gaps
    for i, shot in enumerate(shots):  # Place each screen
        sheet.paste(shot, ((i % 3) * 488, (i // 3) * 278))  # 8-pixel gaps
    os.makedirs(os.path.dirname(PREVIEW), exist_ok=True)  # Make the folder
    sheet.save(PREVIEW)  # Normal size, to keep the file small
    print("Preview written to", PREVIEW)  # Tell where it went


if __name__ == "__main__":
    main()  # Run when started directly
