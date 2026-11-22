"""Draws the moving background layers for the seven Plains Hard themes.

Run from the Unity project folder:  python Tools/Art/make_plains_hard_backdrops.py
It writes L0.png to L5.png (960x270 each) to Assets/Art/Level/Sprites/Backdrop/<Theme>/ for
Prairie (Great Plains, USA), Savanna (Serengeti, Tanzania), Pampas (Argentina), FrozenPlain (West Siberian Plain),
LimestonePlain (Nullarbor, Australia), GangesPlain (Indo-Gangetic Plain) and Steppe (Mongolian Steppe),
plus a preview at .scratch/plains-hard/backdrop-preview.png.

Each set keeps the six layers and the scrolling of the Plains Normal and Plains Easy backdrops:
L0 sky, L1 clouds, L2 far line, L3 flat middle ground, L4 near band, L5 near bank.
Far layers are low contrast and near layers are stronger. The rows behind the explorer (100 to 220)
stay mid-toned and soft. Every layer tiles sideways without a seam. Only fixed numbers are used, so every run gives the same bytes.
A theme folder is written only if it is new, or was made by an earlier run of this script (listed in .scratch/plains-hard/made-folders.txt).
"""
import math  # sin, sqrt and pi for the shapes and the waves
import os  # Folders and paths
from PIL import Image  # Saves the preview
import make_mainmenu_art as mm  # Palette and image helpers, including the pixel font for labels
import make_landform_map_art as lm  # Map colours and the fixed noise
import make_mountains_backdrop as mb  # Pixels, dither, mix and the wrap helper
import make_normal_backdrops as nb  # Gradient, sun, clouds, rolling lines, crowns and the preview
import make_plains_easy_backdrops as pe  # Band rows, haze and palms from the Plains Easy set

W, H = mb.W, mb.H  # 960x270 layers
mix, put, column, dither = mb.mix, mb.put, mb.column, mb.dither  # Shared drawing helpers
band_index, is_edge, haze_t, palm = pe.band_index, pe.is_edge, pe.haze_t, pe.palm  # Band rows, haze and palms
PREVIEW = os.path.join(mb.ROOT, ".scratch", "plains-hard", "backdrop-preview.png")  # Where the preview goes
MADE = os.path.join(mb.ROOT, ".scratch", "plains-hard", "made-folders.txt")  # Theme folders this script has written
LABEL_H = 16  # Height of the label above each preview screen


def read_made():
    """The theme folder names an earlier run wrote, or an empty set on the first run."""
    if not os.path.exists(MADE):  # No record yet
        return set()  # Nothing was made by this script
    with open(MADE, encoding="utf-8") as f:  # Read the list
        return set(line.strip() for line in f if line.strip())  # One name per line


def write_made(names):
    """Saves the sorted list of theme folder names, so the next run knows they are its own."""
    os.makedirs(os.path.dirname(MADE), exist_ok=True)  # Make the folder if missing
    with open(MADE, "w", encoding="utf-8", newline="\n") as f:  # Write the list
        f.write("".join(name + "\n" for name in sorted(names)))  # One name per line


def paint_px(img, horizon, pick):
    """Paints every pixel from row horizon down. pick(x, y) gives its colour, and far rows are hazed toward the sky colour."""
    for y in range(horizon, H):  # Each row from the horizon down
        t = haze_t(y, horizon)  # How much haze covers this row
        for x in range(W):  # Each pixel of the row
            put(img, x, y, mix(pick(x, y), lm.HAZE, t))  # Paint it, a little hazy far away


def fence_line(img, top, step, h, post, wire):
    """A fence: posts every step pixels, h rows tall from row top, joined by one wire row across the whole picture."""
    for x in range(0, W, step):  # Posts, step divides 960 so the fence tiles
        column(img, x, top, post, top + h)  # One post
    for x in range(W):  # The wire, across the whole picture
        put(img, x, top + 2, wire)  # One wire pixel


def bluff(img, cx, top, half, base, rock, rock_sh):
    """A far chalk bluff: a flat top, sloping sides and a pale face that goes down to the horizon."""
    for x in range(cx - half - 40, cx + half + 41):  # Columns across the top and the slopes
        over = max(0, abs(x - cx) - half)  # Columns past the flat top
        y0 = min(base, top + int(over * 0.6))  # The side drops toward the base
        column(img, x, y0, rock_sh if x < cx else rock, base)  # Pale face, shaded on the left


def windpump(img, x, base, h, tower, blade):
    """A far windpump: a tower that narrows at the top, a round wheel of blades and a small tail vane."""
    top = base - h  # Top of the tower
    for y in range(top + 4, base + 1):  # Legs, wide at the ground and narrow at the top
        spread = (y - top - 4) // 3 + 1  # Half the gap between the legs on this row
        put(img, x - spread, y, tower)  # Left leg
        put(img, x + spread, y, tower)  # Right leg
    nb.crown(img, x, top + 2, 2, blade, blade)  # The wheel, a small round hub
    for dx in range(-4, 5):  # Blades across the wheel
        put(img, x + dx, top + 2, blade)  # One blade pixel
    put(img, x + 3, top, tower)  # Tail vane
    put(img, x + 4, top - 1, tower)  # Tail vane tip


def plume(img, x, base, h, stem, cream, cream_sh):
    """A pampas grass plume: a thin stalk with a soft, feathery head on top."""
    top = base - h  # Top of the stalk
    for y in range(top + 3, base):  # The stalk, under the head
        put(img, x, y, stem)  # Stalk pixel
    for dy in range(-3, 4):  # Head rows, from the top down
        for dx in range(-3, 4):  # Columns across the head
            if dx * dx + dy * dy <= 10 and dither(x + dx, top + 3 + dy, 0.85):  # Round, feathery edge
                put(img, x + dx, top + 3 + dy, cream if dx <= 0 else cream_sh)  # Sunny left, shaded right


def ranch(img, x, base, wall, roof, tree, tree_sh):
    """A far ranch: a white house with a flat roof, and a small group of trees beside it."""
    for dy in range(4):  # Wall rows
        for dx in range(-4, 5):  # Wall width
            put(img, x + dx, base - 4 + dy, wall)  # Wall pixel
    for dy in range(2):  # Roof rows, a little wider than the wall
        for dx in range(-5, 6):  # Roof width
            put(img, x + dx, base - 6 + dy, roof)  # Roof pixel
    nb.crown(img, x - 9, base - 5, 3, tree, tree_sh)  # Tree on the left
    nb.crown(img, x + 10, base - 6, 4, tree, tree_sh)  # Bigger tree on the right
    nb.crown(img, x + 15, base - 4, 2, tree, tree_sh)  # Small tree at the end


def kopje(img, cx, base, rx, h, rock, rock_sh, top_lit):
    """A far granite kopje: a round pile of rock with two boulders on its top."""
    for x in range(cx - rx, cx + rx + 1):  # Each column of the pile
        t = (x - cx) / rx  # -1 at the left foot, 0 at the middle, 1 at the right foot
        top = base - int(h * math.sqrt(max(0.0, 1 - t * t)))  # Round top
        column(img, x, top, rock_sh if t < -0.2 else rock, base)  # Shaded left, sunny right
    for dx in (-rx // 2, rx // 4):  # Two boulders on the top
        t = dx / rx  # Position across the pile
        top = base - int(h * math.sqrt(max(0.0, 1 - t * t)))  # Top of the pile here
        nb.crown(img, cx + dx, top, 2, top_lit, rock_sh)  # One boulder


def animal(img, x, y, color):
    """A far grazing animal: a small dark body on four short legs, with a head, standing on row y."""
    for dx in range(-3, 3):  # Body, 6 pixels long
        put(img, x + dx, y - 2, color)  # Back row
        put(img, x + dx, y - 1, color)  # Lower body row
    put(img, x + 3, y - 3, color)  # Head
    for dx in (-2, 2):  # Two legs
        put(img, x + dx, y, color)  # Leg pixel


def acacia(img, x, base, h, half, lit, sh, trunk):
    """A flat-topped acacia: a thin trunk and a wide crown with a level top, like the savanna trees."""
    top = base - h  # Top of the trunk, where the crown starts
    column(img, x, top, trunk, base)  # Trunk, first column
    column(img, x + 1, top + 2, trunk, base)  # Trunk, second column
    for dy in range(4):  # Crown rows, from the level top down
        w = half - (1 if dy == 3 else 0)  # The underside is a little narrower
        for dx in range(-w, w + 1):  # Pixels across the row
            put(img, x + dx, top - 4 + dy, lit if dy < 2 else sh)  # Sunny top, shaded underside


def horse(img, x, y, color):
    """A tiny horse silhouette: a body, a neck and head, a tail and four legs, standing on row y."""
    for dx in range(-3, 4):  # Body, 7 pixels long
        put(img, x + dx, y - 3, color)  # Back row
        put(img, x + dx, y - 2, color)  # Lower body row
    for dx in (-2, 2):  # Two legs, pairs one behind the other
        put(img, x + dx, y - 1, color)  # Upper leg
        put(img, x + dx, y, color)  # Hoof
    put(img, x + 3, y - 4, color)  # Neck
    put(img, x + 4, y - 5, color)  # Head
    put(img, x + 4, y - 4, color)  # Head base
    put(img, x - 4, y - 3, color)  # Tail


def ger(img, x, base, wall, wall_sh, roof, door):
    """A far white ger: a round white tent with a pointed roof and a small dark door."""
    for dy in range(3):  # Roof rows, narrower going up
        for dx in range(-(1 + dy), 2 + dy):  # Pixels across this roof row
            put(img, x + dx, base - 6 + dy, roof)  # Roof pixel
    for dy in range(3):  # Wall rows
        for dx in range(-3, 4):  # Wall width
            put(img, x + dx, base - 3 + dy, wall_sh if dx < -1 else wall)  # Shaded left, sunny right
    put(img, x, base - 2, door)  # Door, two pixels tall
    put(img, x, base - 1, door)  # Door bottom


def birch(img, x, base, h, bark, speck, crown, crown_sh):
    """A birch: a pale trunk with a few dark marks and a round, light crown on top."""
    column(img, x, base - h, bark, base)  # Pale trunk
    for y in range(base - h + 5, base, 4):  # Dark marks on the bark
        put(img, x, y, speck)  # One mark
    nb.crown(img, x, base - h, 3, crown, crown_sh)  # Small, sparse winter crown


def river(img, cy0, amp, half, waves, phase, dark, light):
    """A wide river that winds across the picture: darker edges and a lighter middle, looping across the width."""
    for x in range(W):  # Each column
        cy = cy0 + int(amp * math.sin(2 * math.pi * waves * x / W + phase))  # Centre row, wound by a wave
        for y in range(cy - half, cy + half + 1):  # Each row across the river
            put(img, x, y, light if abs(y - cy) <= half // 2 else dark)  # Light middle, dark edges


def pool(img, cx, cy, rx, ry, dark, light):
    """A frozen bog pool: a flat oval of water with a pale highlight on its upper left."""
    for dy in range(-ry, ry + 1):  # Rows of the oval
        for dx in range(-rx, rx + 1):  # Pixels across the oval
            if (dx / rx) ** 2 + (dy / ry) ** 2 <= 1:  # Inside the oval
                put(img, cx + dx, cy + dy, light if dx < -rx // 3 and dy < 0 else dark)  # Highlight or water


def village_house(img, x, base, wall, wall_sh, roof):
    """A far village house: a small wall with a shallow pitched roof."""
    for dy in range(3):  # Wall rows
        for dx in range(-3, 4):  # Wall width
            put(img, x + dx, base - 3 + dy, wall_sh if dx < -1 else wall)  # Shaded left, sunny right
    for dy in range(2):  # Roof rows, the upper one narrower
        for dx in range(-4 + dy, 5 - dy):  # Roof width
            put(img, x + dx, base - 5 + dy, roof)  # Roof pixel


# ---------- Prairie: Great Plains, USA. Huge pale sky, flat horizon, golden wheat and grass bands, a far windpump and fence ----------

PR_HORIZON = 150  # Row of the flat horizon, low so the sky is big
PR_SKY = [(0, lm.SKY), (100, mm.SKY_LIGHT), (150, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.12))]  # Huge blue sky, pale near the horizon
PR_CHALK = mix(mm.WHITE, lm.HAZE, 0.3)  # Far chalk bluffs, pale white
PR_CHALK_SH = mix(lm.HAZE, mm.SKY_LIGHT, 0.2)  # Shaded face of the chalk
PR_WHEAT = mix(mm.HELMET, mm.VEST, 0.35)  # Golden wheat band
PR_GRASS = mix(mm.PLAINS, mm.VEST, 0.35)  # Dry grass band
PR_GRASS_DK = mix(mm.HILLS_SH, mm.VEST, 0.3)  # Darker grass band
PR_EDGE = mix(mm.VEST_SH, mm.PLAINS, 0.3)  # Line between two bands
PR_STEEL = mix(mm.WHITE, lm.HAZE, 0.25)  # Windpump blades and vane
PR_TOWER = mix(lm.HANDLE, lm.HAZE, 0.3)  # Windpump tower
PR_POST = mix(lm.HANDLE, lm.HAZE, 0.2)  # Fence posts
PR_WIRE = mix(mm.VEST_SH, lm.HAZE, 0.3)  # Fence wire
PR_NEAR = mix(mm.HILLS_SH, mm.VEST, 0.35)  # Near grass bank
PR_NEAR_LIT = mix(mm.PLAINS, mm.HEADLAMP, 0.2)  # Sunny top of the bank


def pr_sky():
    """L0: huge pale-blue sky with a soft sun."""
    img = nb.gradient(PR_SKY)  # Blue fading to pale at the horizon
    nb.sun(img, 220, 52, 8, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4))  # Soft sun
    return img  # Give the picture back


def pr_clouds():
    """L1: a few big, flat white clouds spread across the wide sky."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((110, 36, 200), (470, 22, 150), (760, 50, 220), (930, 20, 110)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # One soft cloud
    return img  # Give the picture back


def pr_far():
    """L2: two pale chalk bluffs far away, and a windpump on the plain."""
    img = mb.new_layer()  # Empty layer
    bluff(img, 150, 126, 56, PR_HORIZON, PR_CHALK, PR_CHALK_SH)  # Left chalk bluff
    bluff(img, 760, 124, 70, PR_HORIZON, PR_CHALK, PR_CHALK_SH)  # Right chalk bluff
    windpump(img, 600, PR_HORIZON, 22, PR_TOWER, PR_STEEL)  # Far windpump
    return img  # Give the picture back


def pr_mid():
    """L3: golden wheat and grass in bands to the horizon, with a fence line along the far plain."""
    img = mb.new_layer()  # Empty layer

    def pick(x, y):  # Colour of each pixel
        if is_edge(y, PR_HORIZON):  # First row of a new band
            return PR_EDGE  # Field edge line
        return (PR_WHEAT, PR_GRASS, PR_GRASS_DK)[band_index(y, PR_HORIZON) % 3]  # Wheat, grass or dark grass

    paint_px(img, PR_HORIZON, pick)  # Paint the bands
    fence_line(img, 146, 32, 7, PR_POST, PR_WIRE)  # Far fence line, posts every 32 pixels
    return img  # Give the picture back


def pr_items():
    """L4: a near fence with posts and wire. The rest stays see-through."""
    img = mb.new_layer()  # Empty layer, see-through
    fence_line(img, 190, 60, 16, PR_POST, PR_WIRE)  # Near fence, posts every 60 pixels
    return img  # Give the picture back


def pr_near():
    """L5: a low grass bank just behind the trail."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(218, 2, 2, 201)  # A gently wavy line near the trail
    nb.fill_below(img, tops, PR_NEAR_LIT, PR_NEAR)  # Bank with a sunny top edge
    return img  # Give the picture back


# ---------- Savanna: Serengeti, Tanzania. Warm sky, flat golden grassland, flat-top acacias, far granite kopjes and herds ----------

SV_HORIZON = 146  # Row of the flat horizon
SV_SKY = [(0, mix(lm.SKY, mm.HEADLAMP, 0.3)), (90, mix(mm.SKY_LIGHT, mm.HEADLAMP, 0.5)), (146, mix(mm.HEADLAMP, mb.SUN_GLOW, 0.5))]  # Warm sky
SV_GRANITE = mix(mm.VEST, lm.HAZE, 0.45)  # Far granite, softened by distance
SV_GRANITE_SH = mix(mm.VEST_SH, lm.HAZE, 0.5)  # Granite in shade
SV_GRANITE_LIT = mix(mm.HEADLAMP, mm.VEST, 0.4)  # Sunny boulder tops
SV_HERD = mix(mm.GROUND_SH, lm.HAZE, 0.4)  # Far herd silhouettes, soft brown
SV_GOLD = mix(mm.HELMET, mm.VEST, 0.25)  # Golden grass band
SV_GOLD_DK = mix(mm.HELMET, mm.VEST_SH, 0.35)  # Darker golden band
SV_TREE_LIT = mix(mm.LEAF, mm.PLAINS, 0.3)  # Sunny acacia crown
SV_TREE_SH = mix(mm.LEAF, mm.OUTLINE, 0.35)  # Shaded acacia crown
SV_TRUNK = mix(lm.HANDLE, mm.OUTLINE, 0.2)  # Acacia trunk
SV_NEAR = mix(mm.VEST, mm.HELMET_SH, 0.35)  # Near dry grass bank
SV_NEAR_LIT = mix(mm.HELMET, mm.HEADLAMP, 0.3)  # Sunny top of the bank


def sv_sky():
    """L0: warm sky with a strong sun."""
    img = nb.gradient(SV_SKY)  # Warm blue fading to gold at the horizon
    nb.sun(img, 680, 58, 10, mb.SUN, mb.SUN_GLOW, mix(mm.HEADLAMP, mb.SUN_GLOW, 0.5))  # Strong sun
    return img  # Give the picture back


def sv_clouds():
    """L1: a few low clouds with warm undersides."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((140, 34, 150), (560, 20, 120), (860, 46, 170)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mix(mm.HEADLAMP, mb.SUN_GLOW, 0.5))  # Cloud with a warm underside
    return img  # Give the picture back


def sv_far():
    """L2: three granite kopjes far away, and two small herds of animals on the horizon."""
    img = mb.new_layer()  # Empty layer
    kopje(img, 120, SV_HORIZON, 46, 22, SV_GRANITE, SV_GRANITE_SH, SV_GRANITE_LIT)  # Left kopje
    kopje(img, 430, SV_HORIZON, 60, 26, SV_GRANITE, SV_GRANITE_SH, SV_GRANITE_LIT)  # Middle kopje
    kopje(img, 800, SV_HORIZON, 40, 18, SV_GRANITE, SV_GRANITE_SH, SV_GRANITE_LIT)  # Right kopje
    for i, x in enumerate((290, 302, 316, 330, 345)):  # First herd, five animals
        animal(img, x, 144 + i % 2, SV_HERD)  # One animal, on two rows
    for i, x in enumerate((600, 614, 628, 642)):  # Second herd, four animals
        animal(img, x, 143 + i % 2, SV_HERD)  # One animal
    return img  # Give the picture back


def sv_mid():
    """L3: flat golden grassland in wide bands to the horizon."""
    img = mb.new_layer()  # Empty layer

    def pick(x, y):  # Colour of each pixel
        if is_edge(y, SV_HORIZON):  # First row of a new band
            return SV_GOLD_DK  # Darker line between bands
        return SV_GOLD if band_index(y, SV_HORIZON) % 2 == 0 else SV_GOLD_DK  # Two golden tones

    paint_px(img, SV_HORIZON, pick)  # Paint the grassland
    return img  # Give the picture back


def sv_items():
    """L4: flat-topped acacia trees across the grassland. The rest stays see-through."""
    img = mb.new_layer()  # Empty layer, see-through
    for i, x in enumerate(range(60, W, 120)):  # One acacia every 120 pixels (120 divides 960)
        base = 206 + lm.noise(i, 411) % 4  # Each tree stands a little higher or lower
        acacia(img, x, base, 14 + lm.noise(i, 412) % 4, 9 + lm.noise(i, 413) % 2, SV_TREE_LIT, SV_TREE_SH, SV_TRUNK)  # One tree
    return img  # Give the picture back


def sv_near():
    """L5: a low dry-grass bank just behind the trail."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(219, 2, 3, 211)  # A gently wavy line near the trail
    nb.fill_below(img, tops, SV_NEAR_LIT, SV_NEAR)  # Bank with a sunny top edge
    return img  # Give the picture back


# ---------- Pampas: Argentina. Soft blue sky, endless green grassland, far pampas plumes and a ranch with trees ----------

PA_HORIZON = 146  # Row of the flat horizon
PA_SKY = [(0, lm.SKY), (100, mm.SKY_LIGHT), (146, mix(mm.SKY_LIGHT, mm.WHITE, 0.4))]  # Soft blue, pale at the horizon
PA_CLOUD_SH = mix(mm.SKY_LIGHT, mm.WHITE, 0.5)  # Cloud underside
PA_FAR = mix(lm.HAZE, mm.SKY_LIGHT, 0.3)  # Very low far hills
PA_HOUSE = mix(mm.WHITE, lm.HAZE, 0.3)  # Ranch wall
PA_ROOF = mix(mm.PLATEAU, lm.HAZE, 0.4)  # Ranch roof
PA_TREE = mix(mm.LEAF, lm.HAZE, 0.3)  # Ranch tree
PA_TREE_SH = mix(mm.LEAF, lm.HAZE_SH, 0.35)  # Shaded side of the tree
PA_STEM = mix(mm.VEST, lm.HAZE, 0.3)  # Pampas grass stalk
PA_PLUME = mix(mm.WHITE, mm.HEADLAMP, 0.5)  # Cream plume, sunny side
PA_PLUME_SH = mix(mm.VEST, mm.HEADLAMP, 0.5)  # Cream plume, shaded side
PA_G1 = mix(mm.HILLS, mm.PLAINS, 0.4)  # Light green grass band
PA_G2 = mix(mm.HILLS, mm.HILLS_SH, 0.5)  # Darker green grass band
PA_G_EDGE = mix(mm.HILLS_SH, mm.HILLS, 0.5)  # Line between two bands
PA_NEAR = mix(mm.HILLS_SH, mm.LEAF, 0.35)  # Near grass bank
PA_NEAR_LIT = mix(mm.HILLS, mm.PLAINS, 0.3)  # Sunny top of the bank


def pa_sky():
    """L0: soft blue sky, no sun."""
    return nb.gradient(PA_SKY)  # Blue fading to pale at the horizon


def pa_clouds():
    """L1: soft white clouds with pale blue undersides."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((90, 30, 130), (360, 50, 110), (620, 24, 150), (880, 54, 120)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, PA_CLOUD_SH)  # One soft cloud
    return img  # Give the picture back


def pa_far():
    """L2: a very low far line of hills, a ranch with trees, and pampas grass plumes on the horizon."""
    img = mb.new_layer()  # Empty layer
    nb.fill_below(img, nb.rolling(138, 2, 2, 301), PA_FAR, PA_FAR)  # Low hills, one flat tone
    ranch(img, 230, PA_HORIZON, PA_HOUSE, PA_ROOF, PA_TREE, PA_TREE_SH)  # Ranch with trees
    for x, h in ((40, 14), (95, 12), (640, 16), (700, 12), (905, 15)):  # Far plumes: place and height
        plume(img, x, PA_HORIZON, h, PA_STEM, PA_PLUME, PA_PLUME_SH)  # One plume
    return img  # Give the picture back


def pa_mid():
    """L3: endless green grassland in bands to the horizon."""
    img = mb.new_layer()  # Empty layer

    def pick(x, y):  # Colour of each pixel
        if is_edge(y, PA_HORIZON):  # First row of a new band
            return PA_G_EDGE  # Line between bands
        return PA_G1 if band_index(y, PA_HORIZON) % 2 == 0 else PA_G2  # Light or dark green

    paint_px(img, PA_HORIZON, pick)  # Paint the grassland
    return img  # Give the picture back


def pa_items():
    """L4: three pampas grass plumes in the grass. The rest stays see-through."""
    img = mb.new_layer()  # Empty layer, see-through
    for x, h in ((150, 22), (430, 26), (770, 20)):  # Plume places and heights
        plume(img, x, 200, h, PA_STEM, PA_PLUME, PA_PLUME_SH)  # One plume
    return img  # Give the picture back


def pa_near():
    """L5: a low grass bank just behind the trail."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(219, 2, 3, 311)  # A gently wavy line near the trail
    nb.fill_below(img, tops, PA_NEAR_LIT, PA_NEAR)  # Bank with a sunny top edge
    return img  # Give the picture back


# ---------- FrozenPlain: West Siberian Plain. Pale cold sky, flat snow, birch forest line, frozen bog pools and a far river ----------

FP_HORIZON = 150  # Row of the flat horizon
FP_SKY = [(0, mix(lm.SKY, mm.WHITE, 0.3)), (110, mm.SKY_LIGHT), (150, mix(mm.WHITE, lm.SKY, 0.3))]  # Pale, cold winter sky
FP_CLOUD = mix(mm.WHITE, lm.HAZE, 0.25)  # Grey-white cloud
FP_CLOUD_SH = mix(lm.HAZE, lm.HAZE_SH, 0.4)  # Cloud underside
FP_CROWN = mix(mm.HELMET, mm.GROUND_SH, 0.35)  # Winter birch crown, yellow-brown
FP_CROWN_SH = mix(mm.GROUND_SH, lm.HAZE, 0.4)  # Shaded winter birch crown
FP_PEAT = (59, 52, 46)  # Frozen peat soil, #3b342e
FP_PEAT_LT = (74, 64, 56)  # Lighter peat, #4a4038
FP_FROST = (232, 238, 245)  # Frost flecks on the peat
FP_BARK = mix(mm.WHITE, lm.HAZE, 0.2)  # Birch bark
FP_SPECK = mix(lm.MOUNTAIN, mm.OUTLINE, 0.3)  # Dark marks on the bark
FP_RIVER = mix(lm.LENS, lm.HAZE, 0.4)  # Far river, pale blue
FP_RIVER_LT = mix(lm.LENS, mm.WHITE, 0.4)  # Light middle of the river
FP_SNOW_A = mix(lm.SNOW, mm.SKY_LIGHT, 0.3)  # Snow band
FP_SNOW_B = mix(lm.SNOW, lm.HAZE, 0.25)  # Snow band in shade
FP_EDGE = mix(lm.HAZE, mm.WHITE, 0.3)  # Line between snow bands
FP_POOL = mix(lm.LENS, lm.MOUNTAIN, 0.35)  # Frozen bog pool
FP_POOL_LT = mix(lm.LENS, mm.WHITE, 0.2)  # Pool highlight
FP_NEAR = mix(lm.HAZE, lm.MOUNTAIN, 0.2)  # Near snow in shade, a little blue
FP_NEAR_LIT = mm.WHITE  # Sunny top of the near snow


def fp_sky():
    """L0: pale, cold winter sky with a weak sun."""
    img = nb.gradient(FP_SKY)  # Pale blue fading to a whiter horizon
    nb.sun(img, 800, 110, 6, mm.WHITE, mb.SUN, mix(mm.WHITE, lm.SKY, 0.4))  # Weak winter sun
    return img  # Give the picture back


def fp_clouds():
    """L1: thin grey-white clouds across the sky."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((150, 34, 170), (520, 24, 130), (860, 46, 150)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, FP_CLOUD, FP_CLOUD_SH)  # One cloud
    return img  # Give the picture back


def fp_far():
    """L2: a birch forest line along the far horizon: thin white trunks with dark marks, and sparse winter crowns."""
    img = mb.new_layer()  # Empty layer
    for i, x in enumerate(range(6, W, 12)):  # One birch every 12 pixels (12 divides 960)
        cy = 134 + lm.noise(i, 421) % 4  # Each crown sits a little higher or lower
        column(img, x, cy + 2, FP_BARK, FP_HORIZON)  # Thin pale trunk under the crown
        for y in range(cy + 4, FP_HORIZON, 4):  # Dark marks on the bark
            put(img, x, y, FP_SPECK)  # One mark
        if i % 3 != 2:  # Sparse crowns: two birches in three have one, the third is bare
            nb.crown(img, x, cy, 2, FP_CROWN, FP_CROWN_SH)  # Small winter crown
    return img  # Give the picture back


def fp_mid():
    """L3: flat snow in bands, a far river winding across it, and frozen bog pools."""
    img = mb.new_layer()  # Empty layer

    def pick(x, y):  # Colour of each pixel
        if is_edge(y, FP_HORIZON):  # First row of a new band
            return FP_EDGE  # Soft edge line
        return FP_SNOW_A if band_index(y, FP_HORIZON) % 2 == 0 else FP_SNOW_B  # Two snow tones

    paint_px(img, FP_HORIZON, pick)  # Paint the snow
    river(img, 163, 2, 2, 2, 0.8, FP_RIVER, FP_RIVER_LT)  # Far river, winding across the plain
    for cx, cy, rx, ry in ((120, 176, 20, 3), (330, 190, 28, 4), (560, 172, 18, 3), (760, 186, 26, 4), (900, 200, 22, 3)):  # Pool places
        pool(img, cx, cy, rx, ry, FP_POOL, FP_POOL_LT)  # One frozen pool
    return img  # Give the picture back


def fp_items():
    """L4: three birches near the front. The rest stays see-through."""
    img = mb.new_layer()  # Empty layer, see-through
    for x, h in ((90, 28), (420, 22), (780, 26)):  # Birch places and heights
        birch(img, x, 206, h, FP_BARK, FP_SPECK, FP_CROWN, FP_CROWN_SH)  # One birch
    return img  # Give the picture back


def fp_near():
    """L5: a low snow bank just behind the trail."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(219, 2, 2, 401)  # A gently wavy line near the trail
    nb.fill_below(img, tops, FP_NEAR_LIT, FP_NEAR)  # Snow with a sunny top edge
    return img  # Give the picture back


def fp_screen(layers):
    """FrozenPlain's preview screen: the six layers, the snow top tiles on the trail line, and a dark frozen peat band under them with frost flecks."""
    screen = Image.new("RGBA", (480, 270))  # Empty screen
    for img in layers:  # Far to near
        screen.alpha_composite(img.crop((0, 0, 480, 270)))  # First screen of each layer
    top = Image.open(os.path.join(nb.SPRITES, "snow_top.png")).convert("RGBA")  # Snow top tiles, as the other themes use
    for x in range(0, 480, top.width):  # Along the trail
        screen.alpha_composite(top, (x, nb.GROUND_ROW))  # One top tile
    for y in range(nb.GROUND_ROW + top.height, 270):  # Peat rows under the top
        for x in range(480):  # Each pixel of the row
            c = FP_PEAT_LT if lm.noise(x // 2, y) % 4 == 0 else FP_PEAT  # Soft two-tone peat
            if lm.noise(x, y) % 29 == 0:  # Sparse frost fleck
                c = FP_FROST  # Frost pixel
            screen.putpixel((x, y), c + (255,))  # Paint the pixel, solid
    return screen


# ---------- LimestonePlain: Nullarbor, Australia. Hot pale sky, flat treeless plain, saltbush, pale limestone cliff above the sea ----------

LP_HORIZON = 146  # Row of the flat horizon
LP_SKY = [(0, mix(lm.SKY, mm.WHITE, 0.45)), (100, mix(mm.SKY_LIGHT, mm.WHITE, 0.4)), (146, mix(mm.HEADLAMP, mm.WHITE, 0.3))]  # Hot, pale sky
LP_SEA = mix(lm.LENS, lm.HAZE, 0.5)  # Far sea, pale blue
LP_SEA_LT = mix(lm.LENS, mm.WHITE, 0.4)  # Sun glints on the sea
LP_CLIFF = mix(mm.VEST, mm.WHITE, 0.45)  # Pale limestone cliff face
LP_CLIFF_SH = mix(mm.VEST, lm.HAZE, 0.3)  # Layers in the cliff face
LP_PLAIN = mix(mm.VEST, mm.MASK, 0.5)  # Flat pale limestone plain
LP_PLAIN_DK = mix(mm.VEST, mm.MASK_SH, 0.35)  # Small dark stones on the plain
LP_BUSH = mix(mm.LEAF, mm.MASK_SH, 0.55)  # Saltbush, grey-green in shade
LP_BUSH_LT = mix(mm.HILLS_SH, mm.MASK, 0.45)  # Saltbush, grey-green in sun
LP_NEAR = mix(mm.VEST_SH, mm.MASK_SH, 0.3)  # Near limestone bank
LP_NEAR_LIT = mix(mm.MASK, mm.WHITE, 0.25)  # Sunny top of the bank


def lp_sky():
    """L0: hot, pale sky with a hard sun."""
    img = nb.gradient(LP_SKY)  # Pale blue fading to hot white
    nb.sun(img, 200, 96, 9, mb.SUN, mb.SUN_GLOW, mix(mm.HEADLAMP, mb.SUN_GLOW, 0.5))  # Hot sun
    return img  # Give the picture back


def lp_clouds():
    """L1: two thin white clouds, so the sky stays pale and hot."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((250, 30, 130), (700, 46, 150)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # One thin cloud
    return img  # Give the picture back


def lp_far():
    """L2: a far sea strip, with the pale limestone cliff edge standing above it."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        column(img, x, 132, LP_SEA, 137)  # Sea, a thin strip
        if x % 24 < 4:  # Short glints every 24 pixels (24 divides 960)
            put(img, x, 134 + (x // 24) % 2, LP_SEA_LT)  # Glint pixel
        column(img, x, 137, LP_CLIFF, LP_HORIZON)  # Cliff face, flat top
        for y in range(138, LP_HORIZON):  # Layers down the cliff
            if (x // 8 + y) % 5 == 0:  # Soft shaded layer lines
                put(img, x, y, LP_CLIFF_SH)  # Layer pixel
    return img  # Give the picture back


def lp_mid():
    """L3: a perfectly flat, treeless plain of pale limestone with a few small dark stones."""
    img = mb.new_layer()  # Empty layer

    def pick(x, y):  # Colour of each pixel
        return LP_PLAIN_DK if (x * 7 + y * 3) % 41 == 0 else LP_PLAIN  # Sparse stones on flat ground

    paint_px(img, LP_HORIZON, pick)  # Paint the plain
    return img  # Give the picture back


def lp_items():
    """L4: low grey-green saltbush scattered across the plain. The rest stays see-through."""
    img = mb.new_layer()  # Empty layer, see-through
    for i, x in enumerate(range(30, W, 60)):  # Saltbush every 60 pixels (60 divides 960)
        cy = 196 + lm.noise(i, 501) % 5  # Each bush sits a little higher or lower
        nb.crown(img, x, cy, 3 + lm.noise(i, 502) % 2, LP_BUSH_LT, LP_BUSH)  # One low bush
    return img  # Give the picture back


def lp_near():
    """L5: a low pale bank just behind the trail."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(219, 1, 2, 511)  # A gently wavy line near the trail
    nb.fill_below(img, tops, LP_NEAR_LIT, LP_NEAR)  # Bank with a sunny top edge
    return img  # Give the picture back


# ---------- GangesPlain: Indo-Gangetic Plain, India. Hazy monsoon sky, a wide river, flooded rice and wheat, palms and a far village ----------

GP_HORIZON = 140  # Row of the flat horizon
GP_SKY = [(0, mix(lm.SKY, lm.HAZE, 0.6)), (100, mix(mm.SKY_LIGHT, lm.HAZE, 0.5)), (140, mix(mm.SKY_LIGHT, mm.WHITE, 0.6))]  # Hazy monsoon sky
GP_CLOUD = mix(lm.HAZE, lm.MOUNTAIN, 0.35)  # Heavy monsoon cloud
GP_CLOUD_SH = mix(lm.MOUNTAIN, lm.HAZE_SH, 0.5)  # Its dark underside
GP_TREE = mix(mm.LEAF, lm.HAZE, 0.45)  # Far tree line
GP_TREE_SH = mix(mm.LEAF, lm.HAZE_SH, 0.5)  # Shaded far trees
GP_HOUSE = mix(mm.VEST, lm.HAZE, 0.5)  # Village wall, sunny side
GP_HOUSE_SH = mix(mm.VEST_SH, lm.HAZE, 0.5)  # Village wall, shaded side
GP_ROOF = mix(mm.GROUND_SH, lm.HAZE, 0.45)  # Village roof
GP_WHEAT = mix(mm.HELMET, mm.VEST, 0.4)  # Wheat field
GP_RICE = mix(mm.PLAINS, mm.HILLS, 0.2)  # Green rice paddy
GP_WATER = mix(lm.LENS, mm.PLAINS, 0.3)  # Flooded paddy water
GP_SHEEN = mix(GP_WATER, mm.WHITE, 0.4)  # Sky sheen on the flooded paddy
GP_EDGE = mix(mm.GROUND, lm.HAZE, 0.3)  # Field bund line
GP_RIVER = mix(mm.GROUND, lm.LENS, 0.4)  # Wide muddy river
GP_RIVER_LT = mix(lm.LENS, mm.WHITE, 0.25)  # Light middle of the river
GP_LEAF = mix(mm.LEAF, lm.HAZE, 0.15)  # Palm crown
GP_LEAF_SH = mix(mm.LEAF, mm.OUTLINE, 0.3)  # Shaded fronds
GP_TRUNK = mix(lm.HANDLE, lm.HAZE, 0.2)  # Palm trunk
GP_NEAR = mix(mm.HILLS_SH, mm.VEST, 0.3)  # Near field bank
GP_NEAR_LIT = mix(mm.PLAINS, mm.HEADLAMP, 0.2)  # Sunny top of the bank


def gp_sky():
    """L0: hazy monsoon sky with a dim sun behind the haze."""
    img = nb.gradient(GP_SKY)  # Hazy blue fading to pale white
    nb.sun(img, 700, 80, 7, mix(mb.SUN, lm.HAZE, 0.4), mix(mb.SUN_GLOW, lm.HAZE, 0.4), mix(lm.HAZE, mm.WHITE, 0.5))  # Dim sun
    return img  # Give the picture back


def gp_clouds():
    """L1: heavy, soft monsoon clouds and a band of mist."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((110, 34, 200), (470, 20, 240), (800, 44, 180)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, GP_CLOUD, GP_CLOUD_SH)  # One heavy cloud
    nb.mist_band(img, 112, 20, mm.WHITE)  # A dotted band of haze over the far land
    return img  # Give the picture back


def gp_far():
    """L2: a far line of trees, with a small village of houses in front of it."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        top = 132 + lm.noise(x // 4 % 240, 521) % 4 - 2  # Bumpy top of the tree line
        column(img, x, top, GP_TREE, GP_HORIZON)  # Tree line from its top down
    for i, x in enumerate((60, 380, 520, 880)):  # Taller trees, poking up
        nb.crown(img, x, 129 + lm.noise(i, 522) % 3, 3, GP_TREE, GP_TREE_SH)  # One tree crown
    for x, base in ((180, 138), (206, 139), (640, 138), (664, 137)):  # House places
        village_house(img, x, base, GP_HOUSE, GP_HOUSE_SH, GP_ROOF)  # One house
    return img  # Give the picture back


def gp_mid():
    """L3: flooded paddies, wheat fields and a wide river winding across the plain."""
    img = mb.new_layer()  # Empty layer

    def pick(x, y):  # Colour of each pixel
        if is_edge(y, GP_HORIZON):  # First row of a new field
            return GP_EDGE  # Field bund line
        k = band_index(y, GP_HORIZON) % 3  # Which kind of field this row is
        if k == 0:  # Flooded paddy, with sky sheen dots
            return GP_SHEEN if (x + 3 * y) % 11 == 0 else GP_WATER
        return GP_WHEAT if k == 1 else GP_RICE  # Wheat or green rice

    paint_px(img, GP_HORIZON, pick)  # Paint the fields
    river(img, 176, 4, 8, 1, 0.0, GP_RIVER, GP_RIVER_LT)  # Wide river across the plain
    return img  # Give the picture back


def gp_items():
    """L4: palm trees standing among the fields. The rest stays see-through."""
    img = mb.new_layer()  # Empty layer, see-through
    for i, x in enumerate((60, 250, 470, 690, 880)):  # Palm places
        palm(img, x, 204 + lm.noise(i, 531) % 4, 22 + lm.noise(i, 532) % 6, GP_LEAF, GP_LEAF_SH, GP_TRUNK)  # One palm
    return img  # Give the picture back


def gp_near():
    """L5: a low green bank just behind the trail."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(218, 2, 3, 531)  # A gently wavy line near the trail
    nb.fill_below(img, tops, GP_NEAR_LIT, GP_NEAR)  # Bank with a sunny top edge
    return img  # Give the picture back


# ---------- Steppe: Mongolian Steppe. Big blue sky, rolling grassland, far white gers, horses and low far hills ----------

ST_HORIZON = 150  # Row of the flat horizon
ST_SKY = [(0, mix(lm.MOUNTAIN, lm.SKY, 0.3)), (80, lm.SKY), (150, mm.SKY_LIGHT)]  # Big, deep blue sky
ST_CLOUD_SH = mix(mm.SKY_LIGHT, mm.WHITE, 0.5)  # Cloud underside
ST_FAR = mix(lm.HAZE, mm.SKY_LIGHT, 0.2)  # Low far hills, pale
ST_FAR_LIT = mix(lm.HAZE, mm.HILLS, 0.25)  # Sunny top of the far hills
ST_GER = mm.WHITE  # Ger felt, sunny side
ST_GER_SH = mix(mm.WHITE, lm.HAZE, 0.4)  # Ger felt, shaded side
ST_ROOF = mix(mm.SKY_LIGHT, mm.WHITE, 0.2)  # Ger roof
ST_DOOR = mix(mm.OUTLINE, lm.HAZE, 0.4)  # Ger door
ST_GRASS_A = mix(mm.PLAINS, mm.HILLS, 0.4)  # Rolling grass, light
ST_GRASS_B = mix(mm.HILLS, mm.HILLS_SH, 0.4)  # Rolling grass, dark
ST_HORSE = mix(mm.GROUND_SH, mm.OUTLINE, 0.4)  # Horse silhouette
ST_NEAR = mix(mm.HILLS_SH, mm.GROUND, 0.25)  # Near grass bank
ST_NEAR_LIT = mix(mm.PLAINS, mm.HEADLAMP, 0.2)  # Sunny top of the bank


def st_sky():
    """L0: big, deep blue sky with a sun."""
    img = nb.gradient(ST_SKY)  # Deep blue fading to pale at the horizon
    nb.sun(img, 400, 60, 8, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4))  # Sun
    return img  # Give the picture back


def st_clouds():
    """L1: a few white clouds across the big sky."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((140, 28, 130), (560, 46, 150), (860, 24, 110)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, ST_CLOUD_SH)  # One cloud
    return img  # Give the picture back


def st_far():
    """L2: low far hills and three white gers on the horizon."""
    img = mb.new_layer()  # Empty layer
    nb.fill_below(img, nb.rolling(138, 3, 3, 601), ST_FAR_LIT, ST_FAR)  # Low rolling hills
    for x, base in ((220, 146), (262, 148), (620, 146)):  # Ger places
        ger(img, x, base, ST_GER, ST_GER_SH, ST_ROOF, ST_DOOR)  # One far ger
    return img  # Give the picture back


def st_mid():
    """L3: rolling grassland in bands that rise and fall across the picture."""
    img = mb.new_layer()  # Empty layer

    def pick(x, y):  # Colour of each pixel
        wave = int(3 * math.sin(2 * math.pi * 2 * x / W))  # The band lines roll up and down across the picture
        return ST_GRASS_A if band_index(y + wave, ST_HORIZON) % 2 == 0 else ST_GRASS_B  # Light or dark grass

    paint_px(img, ST_HORIZON, pick)  # Paint the grassland
    return img  # Give the picture back


def st_items():
    """L4: a small herd of horses on the grass. The rest stays see-through."""
    img = mb.new_layer()  # Empty layer, see-through
    for x, y in ((120, 190), (138, 192), (380, 196), (396, 194), (650, 190), (668, 192), (860, 198)):  # Horse places
        horse(img, x, y, ST_HORSE)  # One horse
    return img  # Give the picture back


def st_near():
    """L5: a low grass bank just behind the trail."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(220, 2, 3, 611)  # A gently wavy line near the trail
    nb.fill_below(img, tops, ST_NEAR_LIT, ST_NEAR)  # Bank with a sunny top edge
    return img  # Give the picture back


THEMES = {  # One entry per Plains Hard theme: its six layers, its ground tiles for the preview and its label
    "Prairie": ((pr_sky, pr_clouds, pr_far, pr_mid, pr_items, pr_near), "pasture", "PRAIRIE"),  # Wheat and grass bands
    "Savanna": ((sv_sky, sv_clouds, sv_far, sv_mid, sv_items, sv_near), "dune", "SAVANNA"),  # Golden grassland with acacias
    "Pampas": ((pa_sky, pa_clouds, pa_far, pa_mid, pa_items, pa_near), "pasture", "PAMPAS"),  # Green grassland with plumes
    "FrozenPlain": ((fp_sky, fp_clouds, fp_far, fp_mid, fp_items, fp_near), "snow", "FROZEN PLAIN"),  # Snow, birches and pools
    "LimestonePlain": ((lp_sky, lp_clouds, lp_far, lp_mid, lp_items, lp_near), "salt", "LIMESTONE PLAIN"),  # Pale flat plain
    "GangesPlain": ((gp_sky, gp_clouds, gp_far, gp_mid, gp_items, gp_near), "rice", "GANGES PLAIN"),  # Paddies and river
    "Steppe": ((st_sky, st_clouds, st_far, st_mid, st_items, st_near), "pasture", "STEPPE"),  # Rolling grass and gers
}


def main():
    """Draws and saves every set and the preview grid."""
    made = read_made()  # Folders an earlier run of this script wrote
    for theme in THEMES:  # Check every target before writing anything
        out = os.path.join(nb.BACKDROPS, theme)  # Its folder
        if os.path.isdir(out) and theme not in made:  # Not made by this script: do not touch it
            raise SystemExit("Stopped: %s already exists and was not made by this script." % out)  # Report and stop
    shots = []  # One (label, preview screen) pair per theme
    for theme, (makers, ground, label) in THEMES.items():  # Each theme
        out = os.path.join(nb.BACKDROPS, theme)  # Its folder
        os.makedirs(out, exist_ok=True)  # Make it if missing
        layers = [make() for make in makers]  # Draw L0 to L5
        for i, img in enumerate(layers):  # Save each one
            img.save(os.path.join(out, "L%d.png" % i))  # L0.png ... L5.png
        shot = fp_screen(layers) if theme == "FrozenPlain" else nb.preview_screen(layers, ground)  # FrozenPlain has its own peat band
        shots.append((label, shot))  # Preview screen with its label
        print("Backdrop written to", out)  # Tell where it went
    write_made(made | set(THEMES))  # Remember the folders this script made
    rows = (len(shots) + 2) // 3  # Three screens per row
    sheet = Image.new("RGBA", (3 * 480 + 2 * 8, rows * (LABEL_H + 270) + (rows - 1) * 8), mm.OUTLINE + (255,))  # Grid with dark gaps
    for i, (label, shot) in enumerate(shots):  # Place each screen and its label
        x = (i % 3) * 488  # Column
        y = (i // 3) * (LABEL_H + 270 + 8)  # Row
        text = mm.text_image(label, mm.WHITE, scale=2)  # Label in the pixel font
        sheet.paste(text, (x + (480 - text.width) // 2, y + 1), text)  # Centred label above the screen
        sheet.paste(shot, (x, y + LABEL_H))  # The composed scene
    os.makedirs(os.path.dirname(PREVIEW), exist_ok=True)  # Make the folder
    sheet.save(PREVIEW)  # Normal size, to keep the file small
    print("Preview written to", PREVIEW)  # Tell where it went


if __name__ == "__main__":
    main()  # Run when started directly
