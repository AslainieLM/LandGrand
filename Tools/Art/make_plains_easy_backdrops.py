"""Draws the moving background layers for the three Plains Easy themes (D-069).

Run from the Unity project folder:  python Tools/Art/make_plains_easy_backdrops.py
It writes L0.png to L5.png (960x270 each) to Assets/Art/Level/Sprites/Backdrop/<Theme>/ for
RicePlain (Central Luzon Plain, Nueva Ecija), SaltBeds (Pangasinan salt beds) and RiverPlain
(Isabela river plain, Cagayan Valley), plus a preview at .scratch/plains-easy/backdrop-preview.png.

Each set keeps the six layers and the scrolling of the Hills Easy backdrops:
L0 sky, L1 clouds, L2 far line, L3 flat middle ground, L4 near band, L5 near bank.
Plains are flat, so the horizon sits low and the sky is big. The rows behind the explorer (100 to 220)
stay mid-toned and soft, so the explorer, the pickups and the ledges stand out. Every layer tiles sideways without a seam.
"""
import math  # sin, sqrt and pi for the shapes and the perspective
import os  # Folders and paths
from PIL import Image  # Saves the preview
import make_mainmenu_art as mm  # Palette and image helpers
import make_landform_map_art as lm  # Map colours and the fixed noise
import make_mountains_backdrop as mb  # Pixels, dither, mix and the wrap helper
import make_normal_backdrops as nb  # Gradient, sun, clouds, ranges, crowns, rolling lines and the preview

W, H = mb.W, mb.H  # 960x270 layers
mix, put, column, dither = mb.mix, mb.put, mb.column, mb.dither  # Shared drawing helpers
PREVIEW = os.path.join(mb.ROOT, ".scratch", "plains-easy", "backdrop-preview.png")  # Where the preview goes
CELLS = [8, 10, 12, 15, 16, 20, 24, 30, 32, 40, 48, 60]  # Cell widths that divide 960 evenly, so the pattern tiles


def band_index(y, horizon):
    """The band number of row y. Far rows make thin bands and near rows make tall bands, like fields seen from a road."""
    return int(1.2 * math.sqrt(max(0, y - horizon) + 1))  # Bands grow more slowly going down, so near bands are taller


def is_edge(y, horizon):
    """True on the first row of a new band, where a field edge or a water line goes."""
    return band_index(y, horizon) != band_index(y - 1, horizon)  # The band number changed since the row above


def haze_t(y, horizon):
    """How much haze covers row y: rows near the horizon are paler, and rows far below get none."""
    return max(0.0, min(0.5, (horizon + 40 - y) / 60))  # Strongest at the horizon, zero 40 rows below it


def cell_width(y, horizon):
    """Width of one cell on row y: small far away, big near the viewer. Always a divisor of 960."""
    f = (y - horizon) / (H - horizon)  # 0 at the horizon, 1 at the bottom
    return CELLS[min(len(CELLS) - 1, int(f * len(CELLS)))]  # Pick the cell size for this depth


def palm(img, x, base, h, leaf, leaf_sh, trunk):
    """A bold coconut palm: a 2-pixel trunk that leans a little, and a full crown of drooping fronds."""
    for y in range(base - h, base + 1):  # Trunk rows, from the top down to the ground
        put(img, x + (base - y) // 8, y, trunk)  # Left trunk pixel with a slight lean
        put(img, x + (base - y) // 8 + 1, y, trunk)  # Right trunk pixel, so the trunk is 2 pixels wide
    tx = x + h // 8  # Top of the trunk
    for side in (-1, 1):  # Left and right fronds
        for t in range(9):  # Along each frond, a little longer than before
            for dy in (0, 1):  # Each frond is two pixels thick
                put(img, tx + side * t, base - h + (t * t) // 6 + dy - 1, leaf if side > 0 else leaf_sh)  # Frond pixel that droops
    for dx in range(-2, 3):  # The full crown core, around the top of the trunk
        for dy in (-2, -1):  # Two rows above the trunk top
            put(img, tx + dx, base - h + dy, leaf)  # Crown pixel that fills the middle


def egret(img, x, y, color):
    """A tiny white egret: a soft 5-pixel wide 'v' standing over the fields."""
    for dx, dy in ((-2, -1), (-1, 0), (0, 1), (1, 0), (2, -1)):  # The wing pixels
        put(img, x + dx, y + dy, color)  # One wing pixel


def nipa(img, x, base):
    """A far nipa hut: a thatched roof on a small wall, standing at the edge of the fields."""
    for dy in range(4):  # Roof rows, from the tip down
        half = 1 + dy  # The roof gets wider going down
        for dx in range(-half, half + 1):  # Each pixel across the roof
            put(img, x + dx, base - 6 + dy, RP_THATCH)  # Thatch pixel
    for dy in range(4):  # Wall rows
        for dx in range(-2, 3):  # Wall width
            put(img, x + dx, base - 2 + dy, RP_WALL)  # Wall pixel


# ---------- RicePlain: Central Luzon Plain, Nueva Ecija. Bright morning sky, flat paddies in bands to a pale Sierra Madre ----------

RP_HORIZON = 142  # Row of the flat horizon
RP_SKY = [(0, lm.SKY), (100, mm.SKY_LIGHT), (150, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.1))]  # Bright morning blue, pale near the horizon
RP_FAR = mix(lm.HAZE, mm.SKY_LIGHT, 0.4)  # Sierra Madre, very pale blue
RP_FAR_SH = mix(lm.HAZE_SH, lm.MOUNTAIN, 0.2)  # Its shade, still pale
RP_MIST = mm.SKY_LIGHT  # Haze at the foot of the range
RP_PAD_A = mix(mm.PLAINS, mm.HILLS, 0.5)  # Light paddy green, a mid tone for the play area
RP_PAD_B = mix(mm.HILLS, mm.HILLS_SH, 0.5)  # Darker paddy green in every second band
RP_WATER = mix(lm.HAZE, mm.WHITE, 0.5)  # Thin silver water line on each paddy edge
RP_THATCH = mm.VEST  # Nipa roof, tan thatch
RP_WALL = mm.GROUND_SH  # Nipa hut wall
RP_LEAF = mix(mm.LEAF, lm.HAZE, 0.2)  # Coconut palm crown
RP_LEAF_SH = mix(mm.LEAF, lm.HAZE_SH, 0.35)  # Shaded fronds
RP_TRUNK = mix(lm.HANDLE, lm.HAZE, 0.25)  # Palm trunk
RP_NEAR = mix(mm.HILLS_SH, mm.LEAF, 0.4)  # Near field bank
RP_NEAR_LIT = mix(mm.HILLS, lm.HAZE, 0.2)  # Sunny top of the bank


def rp_sky():
    """L0: bright morning sky with a soft sun."""
    img = nb.gradient(RP_SKY)  # Blue fading to pale at the horizon
    nb.sun(img, 210, 44, 8, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4))  # Morning sun
    return img  # Give the picture back


def rp_clouds():
    """L1: soft, flat white clouds."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((80, 30, 120), (330, 56, 150), (600, 24, 100), (860, 50, 130)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # One soft cloud
    return img  # Give the picture back


def rp_far():
    """L2: a thin, low, pale line of the Sierra Madre on the horizon."""
    peaks = [(40, 120, 0), (200, 128, 0), (360, 118, 0), (520, 130, 0), (680, 122, 0), (840, 128, 0)]  # Low, wide peaks: place, top row, snow depth
    return nb.ranged(peaks, 0.3, 5, 1, 1.5, RP_FAR, RP_FAR_SH, RP_FAR, RP_FAR_SH, RP_MIST, 126)  # Thin low range with mist at its foot


def nipa_and_egrets(img):
    """Adds the far nipa hut and three white egrets on top of the paddies."""
    nipa(img, 300, 146)  # A far nipa hut at the edge of the paddies
    for x, y in ((330, 152), (340, 158), (352, 154)):  # Three egret places
        egret(img, x, y, mm.WHITE)  # One white egret


def rp_mid():
    """L3: flat rice paddies in bands to the horizon, with thin silver water lines, a nipa hut and egrets."""
    img = mb.new_layer()  # Empty layer
    for y in range(RP_HORIZON, H):  # Each row from the horizon down
        k = band_index(y, RP_HORIZON)  # Which paddy band this row is in
        c = RP_WATER if is_edge(y, RP_HORIZON) else (RP_PAD_A if k % 2 == 0 else RP_PAD_B)  # Silver line on the edge, else a paddy colour
        c = mix(c, lm.HAZE, haze_t(y, RP_HORIZON))  # Far rows fade a little into the haze
        for x in range(W):  # Paint the whole row
            put(img, x, y, c)  # One pixel
    nipa_and_egrets(img)  # The hut and the egrets go on top
    return img  # Give the picture back


def rp_palms():
    """L4: a row of coconut palms on the paddy edges. Only the palms are drawn, so the rest stays see-through."""
    img = mb.new_layer()  # Empty layer, see-through
    for i, x in enumerate((60, 170, 300, 520, 720, 880)):  # Six palm places, fewer so the bolder crowns fit
        palm(img, x, 200 + lm.noise(i, 81) % 4, 22 + lm.noise(i, 82) % 8, RP_LEAF, RP_LEAF_SH, RP_TRUNK)  # One palm with a varied height
    return img  # Give the picture back


def rp_near():
    """L5: the near field bank, low and flat, just behind the trail."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(218, 2, 3, 37)  # A gently wavy line near the trail
    nb.fill_below(img, tops, RP_NEAR_LIT, RP_NEAR)  # Bank with a sunny top line
    return img  # Give the picture back


# ---------- SaltBeds: Pangasinan salt beds by the sea. Pale hot sky, a flat blue sea, square salt ponds and white heaps ----------

SB_HORIZON = 140  # Row where the ponds start
SB_CALM_ROW = 180  # Row where the lower third starts, with fewer lines
SB_SEA_TOP = 128  # Row of the flat sea line
SB_SKY = [(0, mix(lm.SKY, mm.WHITE, 0.35)), (100, mm.SKY_LIGHT), (150, mix(mm.SKY_LIGHT, mm.HEADLAMP, 0.5))]  # Pale, hot sky
SB_SEA = mix(lm.LENS, lm.HAZE, 0.25)  # Flat blue sea
SB_SEA_LT = mix(lm.LENS, mm.WHITE, 0.4)  # Shine on the sea
SB_ISLAND = mix(mm.HILLS_SH, lm.HAZE, 0.5)  # Low far island, pale green-grey
SB_POND_A = mix(lm.SKY, mm.WHITE, 0.25)  # Pale blue salt pond
SB_POND_B = mix(lm.SKY, lm.HAZE, 0.3)  # Second pond colour, a little deeper
SB_SHINE = mm.WHITE  # White glint on a pond
SB_DYKE = mix(mm.VEST, lm.HAZE, 0.55)  # Sandy dyke between the ponds, softened toward the sky so the grid is less loud
SB_HEAP = mm.WHITE  # Top of a salt heap
SB_HEAP_SH = mm.MASK  # Shaded side of a salt heap
SB_POST = mix(lm.HANDLE, lm.HAZE, 0.4)  # Bamboo post, softened toward the sky so it is less bright
SB_LEAF = mix(mm.LEAF, lm.HAZE, 0.2)  # Palm crown
SB_LEAF_SH = mix(mm.LEAF, lm.HAZE_SH, 0.35)  # Shaded fronds
SB_TRUNK = mix(lm.HANDLE, lm.HAZE, 0.25)  # Palm trunk
SB_NEAR = mix(mm.VEST, mm.VEST_SH, 0.3)  # Near sand bank
SB_NEAR_LIT = mix(mm.HEADLAMP, mm.VEST, 0.3)  # Sunny top of the bank


def sb_sky():
    """L0: pale, hot sky with a strong sun."""
    img = nb.gradient(SB_SKY)  # Pale blue to warm white
    nb.sun(img, 700, 46, 10, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.5))  # Hot sun
    return img  # Give the picture back


def sb_clouds():
    """L1: a few thin, high white clouds."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((200, 30, 160), (520, 50, 120), (850, 28, 140)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mix(mm.SKY_LIGHT, mm.WHITE, 0.5))  # One thin cloud
    return img  # Give the picture back


def sb_far():
    """L2: a flat blue sea line with a low far island in the middle."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        column(img, x, SB_SEA_TOP, SB_SEA)  # Sea, from its top line down
        if x % 24 < 4:  # Short shine marks in the sea
            put(img, x, 133 + (x // 24) % 3 * 3, SB_SEA_LT)  # One shine pixel
    for x in range(320, 440):  # The low far island
        d = abs(x - 380)  # Distance from the island's middle
        top = SB_SEA_TOP - 1 - int(7 * (1 - (d / 60) ** 2)) if d < 60 else SB_SEA_TOP  # Low round top
        column(img, x, top, SB_ISLAND, SB_SEA_TOP)  # Island from its top down to the sea line
    return img  # Give the picture back


def sb_mid():
    """L3: square shallow salt ponds in a grid that gets bigger toward the viewer, with sandy dykes."""
    img = mb.new_layer()  # Empty layer
    for y in range(SB_HORIZON, H):  # Each row from the ponds' top line down
        k = band_index(y, SB_HORIZON)  # Which row of ponds this row is in
        calm = y >= SB_CALM_ROW  # True in the lower third, where the grid is kept calmer
        edge = is_edge(y, SB_HORIZON) and (not calm or k % 2 == 0)  # Dyke rows; in the calm part only every second row
        cell = cell_width(y, SB_HORIZON)  # Pond width on this row
        f = (y - SB_HORIZON) / (H - SB_HORIZON)  # 0 at the horizon, 1 at the bottom
        t = min(0.6, haze_t(y, SB_HORIZON) + 0.25 * (1 - f))  # Paler toward the horizon so the eye reads depth
        for x in range(W):  # Each pixel of the row
            if edge or (x % cell == 0 and not calm):  # A dyke row, or a dyke column (none in the calm part)
                c = SB_DYKE  # Sandy dyke
            elif x % cell == 2 and (y + k) % 4 == 1 and not calm:  # A small white glint on the water, none in the calm part
                c = SB_SHINE  # Glint pixel
            else:  # Pond water
                c = SB_POND_A if ((x // cell) + k) % 2 == 0 else SB_POND_B  # Checker of two pond colours
            put(img, x, y, mix(c, lm.HAZE, t))  # Paint it, a little hazy far away
    return img  # Give the picture back


def sb_items():
    """L4: salt heaps on the dykes, bamboo posts and a few coconut palms. The rest stays see-through."""
    img = mb.new_layer()  # Empty layer
    for x, w in ((120, 16), (330, 20), (600, 14), (860, 18)):  # Salt heaps: left x and width, a little bigger
        base = 196 + lm.noise(x, 91) % 4  # The heap sits on a dyke row
        for dx in range(w):  # Each column of the heap
            hump = int(6 * math.sin(math.pi * (dx + 0.5) / w)) + 2  # Taller, rounded top
            for dy in range(hump):  # Each row of the heap
                put(img, x + dx, base - dy, SB_HEAP if dy == hump - 1 else SB_HEAP_SH)  # Bright top, shaded sides
    for x in range(20, W, 80):  # Bamboo posts every 80 pixels (80 divides 960)
        post_base = 200 + lm.noise(x, 92) % 3  # The post stands on the dyke
        column(img, x, post_base - 16 - lm.noise(x, 93) % 4, SB_POST, post_base + 1)  # One thin post
    for i, x in enumerate((150, 470, 790)):  # Three palms
        palm(img, x, 200, 22 + lm.noise(i, 94) % 6, SB_LEAF, SB_LEAF_SH, SB_TRUNK)  # One palm
    return img  # Give the picture back


def sb_near():
    """L5: a low pale sand bank just behind the trail."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(218, 2, 2, 49)  # A gently wavy line near the trail
    nb.fill_below(img, tops, SB_NEAR_LIT, SB_NEAR)  # Sand with a sunny top line
    return img  # Give the picture back


# ---------- RiverPlain: Isabela river plain, Cagayan Valley. Warm late sky, corn rows, a calm river and a line of trees ----------

RV_HORIZON = 150  # Row of the flat horizon
RV_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.1)), (80, mix(lm.SKY, mm.HEADLAMP, 0.25)), (130, mix(mm.HEADLAMP, mb.SUN_GLOW, 0.45)), (170, mix(mb.SUN_GLOW, mm.HEADLAMP, 0.3))]  # Warm late-afternoon sky
RV_MIST = mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.25)  # Warm haze at the foot of the hills
RV_FAR = mix(lm.HAZE, mm.SKY_LIGHT, 0.3)  # Far hills, very pale
RV_FAR_SH = mix(lm.HAZE_SH, lm.MOUNTAIN, 0.2)  # Their shade
RV_CORN = mix(mm.PLAINS, mb.SUN_GLOW, 0.25)  # Yellow-green corn rows
RV_CORN_SH = mix(lm.PLAINS_SH, mb.SUN_GLOW, 0.15)  # Corn rows in shade
RV_CORN_LT = mix(mm.PLAINS, mm.WHITE, 0.25)  # Sunny corn leaves
RV_FURROW = mix(mm.HILLS_SH, lm.HANDLE, 0.3)  # Dark line between field strips
RV_RIVER = mix(lm.LENS, lm.HANDLE, 0.35)  # Calm brown-green river
RV_RIVER_SH = mix(mm.LEAF, lm.HANDLE, 0.5)  # Shaded lower half of the river
RV_RIVER_LT = mix(lm.LENS, mm.WHITE, 0.3)  # Sunny top edge of the river
RV_TREE = mix(mm.LEAF, lm.HAZE, 0.15)  # Trees on the bank
RV_TREE_SH = mix(mm.LEAF, lm.HAZE_SH, 0.3)  # Their shade
RV_NEAR = mix(mm.HILLS_SH, lm.HANDLE, 0.25)  # Near brown-green bank
RV_NEAR_LIT = mix(mm.PLAINS, mb.SUN_GLOW, 0.2)  # Sunny edge of the bank


def rv_river_row(x):
    """Centre row of the river at column x. It winds up and down and joins up across the 960-pixel loop."""
    return 178 + int(7 * math.sin(2 * math.pi * 2 * x / W) + 3 * math.sin(2 * math.pi * 5 * x / W + 1.0))  # Two looping waves


def rv_river_half(x):
    """Half the width of the river at column x: about six rows, a little wider in places."""
    return 6 + int(2 * math.sin(2 * math.pi * 3 * x / W + 0.5))  # Changes slowly and loops across the width


def rv_sky():
    """L0: warm late-afternoon sky with a low sun."""
    img = nb.gradient(RV_SKY)  # Blue at the top, warm gold low down
    nb.sun(img, 150, 100, 9, mb.SUN, mb.SUN_GLOW, mix(mm.HEADLAMP, mb.SUN_GLOW, 0.5))  # Low afternoon sun
    return img  # Give the picture back


def rv_clouds():
    """L1: soft clouds with warm undersides."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((100, 30, 140), (400, 46, 100), (640, 22, 160), (880, 56, 110)):  # Cloud places and widths
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mix(mm.HEADLAMP, mb.SUN_GLOW, 0.4))  # One cloud with a warm underside
    return img  # Give the picture back


def rv_far():
    """L2: very pale hills, the Cordillera on the left and the Sierra Madre on the right."""
    peaks = [(60, 100, 0), (210, 108, 0), (380, 124, 0), (560, 118, 0), (720, 110, 0), (880, 104, 0)]  # Left side higher, middle low
    return nb.ranged(peaks, 0.4, 13, 2, 3.0, RV_FAR, RV_FAR_SH, RV_FAR, RV_FAR_SH, RV_MIST, 130)  # Pale ranges with warm mist


def rv_mid():
    """L3: flat strips of corn to the horizon, with a calm river winding across them."""
    img = mb.new_layer()  # Empty layer
    for y in range(RV_HORIZON, H):  # Each row from the horizon down
        k = band_index(y, RV_HORIZON)  # Which field strip this row is in
        c = RV_FURROW if is_edge(y, RV_HORIZON) else (RV_CORN if k % 2 == 0 else RV_CORN_SH)  # Dark line on each strip edge
        t = haze_t(y, RV_HORIZON)  # How much haze to add
        for x in range(W):  # Each pixel of the row
            cc = RV_CORN_LT if c == RV_CORN and (x + y) % 6 == 0 else c  # A few sunny leaves in the corn rows
            put(img, x, y, mix(cc, lm.HAZE, t))  # Paint it, a little hazy far away
    for x in range(W):  # The river, column by column
        cy = rv_river_row(x)  # Its centre row here
        hw = rv_river_half(x)  # Its half width here
        for y in range(cy - hw, cy + hw + 1):  # Each row across the river
            put(img, x, y, RV_RIVER_SH if y > cy else RV_RIVER)  # Shaded lower half, lighter upper half
        put(img, x, cy - hw, RV_RIVER_LT)  # Sunny top edge
    return img  # Give the picture back


def rv_trees():
    """L4: a line of round trees along the riverbank. The rest stays see-through."""
    img = mb.new_layer()  # Empty layer
    for i, x in enumerate(range(20, W, 40)):  # One tree every 40 pixels (40 divides 960)
        cx = x + lm.noise(i, 71) % 12  # Tree place, a little varied
        cy = rv_river_row(cx) - rv_river_half(cx) - 5 - lm.noise(i, 72) % 3  # Crown sits just above the bank
        nb.crown(img, cx, cy, 5 + lm.noise(i, 73) % 3, RV_TREE, RV_TREE_SH)  # One round crown
    return img  # Give the picture back


def rv_near():
    """L5: a low brown-green bank just behind the trail."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(218, 2, 3, 53)  # A gently wavy line near the trail
    nb.fill_below(img, tops, RV_NEAR_LIT, RV_NEAR)  # Bank with a sunny top line
    return img  # Give the picture back


THEMES = {  # One entry per Plains Easy theme (D-069): its six layers and its ground tiles for the preview
    "RicePlain": ((rp_sky, rp_clouds, rp_far, rp_mid, rp_palms, rp_near), "pasture"),  # Rice paddies
    "SaltBeds": ((sb_sky, sb_clouds, sb_far, sb_mid, sb_items, sb_near), "dune"),  # Salt beds by the sea
    "RiverPlain": ((rv_sky, rv_clouds, rv_far, rv_mid, rv_trees, rv_near), "pasture"),  # River plain with corn
}


def main():
    """Draws and saves every set and the preview row."""
    shots = []  # One preview screen per theme
    for theme, (makers, ground) in THEMES.items():  # Each theme
        out = os.path.join(nb.BACKDROPS, theme)  # Its folder
        os.makedirs(out, exist_ok=True)  # Make it if missing
        layers = [make() for make in makers]  # Draw L0 to L5
        for i, img in enumerate(layers):  # Save each one
            img.save(os.path.join(out, "L%d.png" % i))  # L0.png ... L5.png
        shots.append(nb.preview_screen(layers, ground))  # Preview screen
        print("Backdrop written to", out)  # Tell where it went
    sheet = Image.new("RGBA", (3 * 480 + 2 * 8, 270), mm.OUTLINE + (255,))  # One row of three screens with dark gaps
    for i, shot in enumerate(shots):  # Place each screen
        sheet.paste(shot, (i * 488, 0))  # 8-pixel gaps
    os.makedirs(os.path.dirname(PREVIEW), exist_ok=True)  # Make the folder
    sheet.save(PREVIEW)  # Normal size, to keep the file small
    print("Preview written to", PREVIEW)  # Tell where it went


if __name__ == "__main__":
    main()  # Run when started directly
