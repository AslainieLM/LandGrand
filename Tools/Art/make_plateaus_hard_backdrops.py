"""Draws the moving background layers for the seven Plateaus Hard themes (D-074).

Run from the Unity project folder:  python Tools/Art/make_plateaus_hard_backdrops.py
It writes L0.png to L5.png (960x270 each) to Assets/Art/Level/Sprites/Backdrop/<Theme>/ for
DeccanTraps (Deccan Plateau, India), SimienHighlands (Ethiopian Highlands), SandstoneArches (Tassili n'Ajjer, Algeria),
CliffDwellings (Mesa Verde, USA), IcePlateau (Antarctic Plateau), LoessTerraces (Loess Plateau, China)
and RoofOfWorld (Tibetan Plateau), plus a preview at .scratch/plateaus-hard/backdrop-preview.png.

Each set keeps the six layers and the scrolling of the Plateaus Normal backdrops:
L0 sky, L1 clouds, L2 far line, L3 middle ground, L4 near band, L5 near bank.
A plateau is high, wide land with a flat top and steep sides, so each set shows flat tops and steep faces.
Hard levels have a strong high wind: clouds are torn into long streaks, thin gusts cross the sky and grass leans over.
The rows behind the explorer (100 to 220) stay mid-toned and soft, so the explorer, the pickups and the ledges stand out.
Every colour is a palette colour or a mix of two. Every layer tiles sideways without a seam.
Only fixed numbers are used, so every run gives the same bytes.
A theme folder is written only if it is new, or was made by an earlier run of this script (listed in .scratch/plateaus-hard/made-folders.txt).
"""
import math  # sin and pi for the shapes
import os  # Folders and paths
from PIL import Image  # Saves the preview
import make_mainmenu_art as mm  # Palette, image helpers and the pixel font
import make_landform_map_art as lm  # Map colours and the fixed noise
import make_mountains_backdrop as mb  # Pixels, dither, mix and the wrap helper
import make_normal_backdrops as nb  # Gradient, sun, clouds, crowns, rolling lines and the preview
import make_plateaus_easy_backdrops as pe  # Flat-topped plateau shapes
import make_plateaus_normal_backdrops as pn  # Lines, ovals, cones, pinyon pines and sagebrush

W, H = mb.W, mb.H  # 960x270 layers
mix, put, column, dither, jag = mb.mix, mb.put, mb.column, mb.dither, mb.jag  # Shared drawing helpers
noise = lm.noise  # Fixed noise from 0 to 99, the same on every run
PREVIEW = os.path.join(mb.ROOT, ".scratch", "plateaus-hard", "backdrop-preview.png")  # Where the preview goes
MADE = os.path.join(mb.ROOT, ".scratch", "plateaus-hard", "made-folders.txt")  # Theme folders this script has written
LABEL_H = 16  # Height of the label above each preview screen
SHIRT = pn.SHIRT  # Palette "Shirt", a blue-green
SHIRT_SH = pn.SHIRT_SH  # Palette "Shirt shade"
PANTS = pn.PANTS  # Palette "Pants", a grey-brown
BOOTS = pn.BOOTS  # Palette "Boots", a dark brown


# ---------- Wind helpers, shared by every theme ----------

def streak_cloud(img, cx, cy, w, top, under, seed):
    """A long thin cloud torn by the high wind: a thick head on the left that thins into ragged wisps trailing to the right."""
    x0 = cx - w // 2  # Left end, the head of the cloud
    for x in range(x0, x0 + w):  # Each column
        t = (x - x0) / w  # 0 at the head, 1 at the tail
        thick = max(0, int(8 * min(1.0, t * 10) * (1 - t) ** 1.3 + 1.5 * math.sin(t * 19 + seed)))  # A rounded head with soft lumps, thin at the tail
        for y in range(cy - thick, cy + 2):  # Top to bottom
            if t < 0.45 or dither(x, y, 1.5 - 1.3 * t):  # A solid head and a dotted, torn tail
                put(img, x, y, under if y >= cy + 1 else top)  # Shaded underside
    for k in range(3):  # Thin wisps blown off the tail
        wy = cy - 3 + k * 2  # Row of this wisp
        start = x0 + w * (5 + k) // 10  # Where it leaves the cloud
        length = w // 2 + noise(k, seed) % 20  # How far it trails
        for x in range(start, start + length):  # Along the wisp
            if dither(x, wy, 0.7 - 0.6 * (x - start) / length):  # Fading out toward its end
                put(img, x, wy, top)  # Wisp pixel


def gusts(img, color, seed, rows):
    """Thin broken lines of wind across the picture, so the strong high wind shows. Each gust tapers at both ends."""
    for i, y in enumerate(rows):  # Each row that carries gusts
        shift = noise(i, seed) * 7  # Each row starts its gusts in a different place
        for x in range(W):  # Each pixel
            s = (x + shift) // 80 % 12  # Which 80-pixel stretch this is; 12 stretches make 960, so it tiles
            pos = (x + shift) % 80  # Place inside the stretch
            length = 24 + noise(s, seed + i) % 40  # Gust length in this stretch
            if noise(s, seed + 50 + i) % 3 == 0 and pos < length:  # Only some stretches have a gust
                t = pos / length  # 0 at the start, 1 at the end
                if dither(x, y, 0.95 - 0.8 * abs(2 * t - 1)):  # Solid in the middle, dotted at the ends
                    put(img, x, y, color)  # Gust pixel


def blown_tuft(img, x, base, h, color, tip=None):
    """A grass tuft bent by the wind: three blades that lean more and more to the right toward the tip."""
    for k, lean in enumerate((0.3, 0.6, 1.0)):  # Three blades, each leaning a little more
        bh = max(2, h - k)  # Each one a little shorter
        for t in range(bh):  # Up the blade
            c = tip if tip is not None and t >= bh - 2 else color  # A paler seed head at the tip, when given
            put(img, x + k - 1 + int(lean * t * t / bh), base - t, c)  # Blade pixel, bending right


def wind_sky(stops, sun_at, sun_colors, gust_color, seed):
    """L0 helper: a sky gradient, the sun and a few faint high gusts."""
    img = nb.gradient(stops)  # The sky
    sx, sy, r = sun_at  # Where the sun is and how big
    nb.sun(img, sx, sy, r, *sun_colors)  # The sun
    gusts(img, gust_color, seed, (14, 58, 88))  # Faint high gusts
    return img  # Give the picture back


def wind_clouds(clouds, top, under, gust_color, seed, rows=(24, 44, 70, 96)):
    """L1 helper: long streaky clouds and gust lines, all torn by the same wind."""
    img = mb.new_layer()  # Empty layer
    for i, (cx, cy, w) in enumerate(clouds):  # Each cloud
        streak_cloud(img, cx, cy, w, top, under, seed + i)  # One streaky cloud
    gusts(img, gust_color, seed + 20, rows)  # Gust lines between them
    return img  # Give the picture back


def tops_ground(base, amp, waves, seed):
    """A gently rolling ground line that tiles, from the Normal helper."""
    return nb.rolling(base, amp, waves, seed)  # One row per x


# ---------- DeccanTraps: the Deccan Plateau in the dry season. Wide flat hills of stair-step basalt, a far hill fort, dry grass ----------

DT_SKY = [(0, mix(lm.SKY, lm.HAZE, 0.3)), (90, mix(mm.SKY_LIGHT, lm.HAZE, 0.35)), (150, mix(mm.SKY_LIGHT, mm.VEST, 0.4))]  # Dry, dusty blue
DT_CLOUD = mix(mm.WHITE, mm.HEADLAMP, 0.3)  # Dusty white clouds
DT_CLOUD_SH = mix(mm.SKY_LIGHT, mm.VEST, 0.35)  # Their dusty undersides
DT_GUST = mix(mm.SKY_LIGHT, mm.VEST, 0.25)  # Dusty gust lines
DT_FAR = mix(PANTS, lm.HAZE, 0.62)  # Far trap hills, softened by haze
DT_FAR_BAND = mix(mm.STRAP, lm.HAZE, 0.55)  # Their dark basalt steps
DT_FAR_SH = mix(PANTS, lm.HAZE_SH, 0.5)  # Their shaded sides
DT_FORT = mix(mm.STRAP, lm.HAZE, 0.45)  # The far hill fort walls
DT_FAR_PLAIN = mix(mm.VEST, lm.HAZE, 0.55)  # The far dry plain
DT_BASALT = mix(PANTS, lm.HAZE_SH, 0.3)  # Dark basalt cliffs of the near traps
DT_BASALT_LT = mix(PANTS, mm.MASK, 0.4)  # Sunny cliff edges
DT_BASALT_SH = mix(PANTS, mm.STRAP, 0.45)  # Shaded cliffs and column joints
DT_LEDGE = mix(mm.VEST, lm.PLAINS_SH, 0.3)  # Dry grass on the ledges
DT_LEDGE_SH = mix(mm.VEST_SH, lm.HAZE_SH, 0.35)  # Shaded ledges
DT_TOP_LIT = mix(mm.VEST, mm.HEADLAMP, 0.35)  # Sunny line on the flat tops
DT_PLAIN = mix(mm.VEST, mm.VEST_SH, 0.35)  # Dry grass plain below the hills
DT_PLAIN_SH = mix(mm.VEST_SH, lm.PLAINS_SH, 0.35)  # Darker grass streaks
DT_TREE = mix(mm.LEAF, mm.VEST, 0.35)  # Dull olive trees of the dry season
DT_TREE_SH = mix(mm.LEAF, PANTS, 0.45)  # Their shade
DT_TRUNK = mix(PANTS, mm.GROUND_SH, 0.5)  # Trunks
DT_GRASS = mix(mm.VEST, mm.HELMET_SH, 0.3)  # Near golden dry grass
DT_GRASS_LIT = mix(mm.VEST, mm.HEADLAMP, 0.45)  # Its sunny top line
DT_GRASS_SH = mix(mm.VEST_SH, mm.HELMET_SH, 0.35)  # Its darker blades
DT_NEAR = mix(mm.GROUND_SH, PANTS, 0.5)  # Near dry dark earth
DT_NEAR_LIT = mix(mm.VEST_SH, mm.GROUND, 0.5)  # Its sunny top line
DT_BOULDER = mix(PANTS, mm.STRAP, 0.3)  # Dark basalt boulders
DT_BOULDER_LT = mix(PANTS, mm.MASK_SH, 0.5)  # Their sunny tops


def step_drop(d, rise, run):
    """How far below the flat top a stair-step side is, d pixels past the edge:
    a short steep cliff, then a gently sloping ledge, again and again."""
    k, r = divmod(d, run)  # Which step, and how far along it
    cliff = rise * 0.45  # Rows the cliff part drops
    if r < 2:  # The short steep cliff
        return k * rise + cliff * (r + 1) / 2  # Drops fast
    return k * rise + cliff + (rise - cliff) * (r - 2) / (run - 2)  # The ledge, dropping slowly


def trap_tops(hills, seed):
    """The outline of a row of trap hills. hills: (middle x, half width of the flat top, top row, rows per step, pixels per step).
    Returns, for each x, the top row and (signed distance from the middle, the hill)."""
    tops, owners = [], []  # Top row and owner for each x
    for x in range(W):  # Each column
        best, owner = H, None  # Lowest row number wins
        for hill in hills:  # Each hill
            cx, half, top, rise, run = hill  # Its shape
            off = mb.wrap_offset(x, cx)  # Signed distance from its middle
            d = abs(off) - half  # How far past the edge of the flat top
            y = top + jag(x, seed, 1) + (0 if d <= 0 else step_drop(d, rise, run))  # Flat top, or the stair-step side, a little rough
            if y < best:  # Higher than the others
                best, owner = y, (off, hill)  # This hill owns the column
        tops.append(int(best))  # Whole rows only
        owners.append(owner)  # Remember the owner
    return tops, owners  # Give the outline back


def dt_sky():
    """L0: a dry-season sky, hazy with dust, and a hot sun."""
    return wind_sky(DT_SKY, (150, 30, 8), (mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4)), DT_GUST, 201)  # Sky, sun and gusts


def dt_clouds():
    """L1: a few dusty clouds torn into streaks by the wind."""
    return wind_clouds(((260, 30, 150), (600, 50, 110), (860, 22, 130)), DT_CLOUD, DT_CLOUD_SH, DT_GUST, 202)  # Streaky clouds


def dt_far():
    """L2: far flat trap hills with faint stair-step basalt bands, a hill fort on one top, and the dry plain."""
    img = mb.new_layer()  # Empty layer
    plain = 150  # Row of the far plain
    hills = [(90, 70, 118, 7, 11), (330, 60, 124, 7, 10), (560, 50, 106, 7, 11), (800, 90, 120, 7, 12)]  # Far hills
    tops, owners = trap_tops(hills, 203)  # Their outline
    for x in range(W):  # Each column
        off, (cx, half, top, rise, run) = owners[x]  # The hill here
        for y in range(tops[x], plain):  # From the top down to the plain
            band = (y - top) % rise < 3  # The dark basalt steps
            c = DT_FAR_BAND if band else DT_FAR  # Steps and slopes
            if off < 0 and abs(off) > half:  # The shaded left side
                c = DT_FAR_SH if not band else DT_FAR_BAND  # Shade pixel
            put(img, x, y, c)  # Paint it
    for x in range(W):  # The far dry plain
        column(img, x, plain, DT_FAR_PLAIN)  # Plain pixel
    dt_fort(img, 560, 106)  # The hill fort on the tallest top
    return img  # Give the picture back


def dt_fort(img, cx, top):
    """A far hill fort along a flat top: a wall with battlements, round bastions and a gate tower."""
    for x in range(cx - 36, cx + 37):  # The long wall
        column(img, x, top - 3, DT_FORT, top)  # Wall pixel
        if x % 3 == 0:  # Battlements along the top
            put(img, x, top - 4, DT_FORT)  # Merlon pixel
    for bx in range(cx - 36, cx + 37, 18):  # Round bastions along the wall
        for dx in range(-3, 4):  # Across the bastion
            column(img, bx + dx, top - 5 - (1 if abs(dx) < 2 else 0), DT_FORT, top)  # A rounded tower
    for x in range(cx + 4, cx + 10):  # The gate tower
        column(img, x, top - 9, DT_FORT, top)  # Tower pixel
    put(img, cx + 6, top - 10, DT_FORT)  # A small dome on the tower
    put(img, cx + 7, top - 10, DT_FORT)  # The other half of the dome


def dt_mid():
    """L3: big trap hills with clear stair-step layers of dark basalt cliffs and dry grassy ledges, trees on the plain."""
    img = mb.new_layer()  # Empty layer
    foot = 178  # Row where the hills meet the plain
    hills = [(230, 120, 106, 13, 22), (700, 100, 120, 12, 20)]  # Middle x, half width, top row, rows per step, pixels per step
    tops, owners = trap_tops(hills, 204)  # Their outline
    for x in range(W):  # Each column
        off, (cx, half, top, rise, run) = owners[x]  # The hill here
        side = abs(off) > half  # On a stair-step side
        shade = side and off < 0  # The left side faces away from the sun
        joint = x % 6 == 0 and noise(x, 205) % 2 == 0  # Upright joints between basalt columns
        for y in range(tops[x], foot):  # From the top down to the plain
            s = (y - top) % rise  # Row inside its step
            if y == tops[x] and not side:  # The flat top
                c = DT_TOP_LIT  # Sunny top line
            elif s < int(rise * 0.45):  # A basalt cliff band
                c = DT_BASALT_SH if shade or joint else DT_BASALT  # Shade and column joints
                if side and not shade and (abs(off) - half) % run == 0:  # The sunny edge of a step on the right side
                    c = DT_BASALT_LT  # Light pixel
            else:  # A grassy ledge
                c = DT_LEDGE_SH if shade else DT_LEDGE  # Dry grass
                if s == int(rise * 0.45) and noise(x // 2 % 480, y) % 13 == 0:  # A small tree on the ledge now and then
                    put(img, x, y - 1, DT_TREE_SH)  # Tree pixel
                    put(img, x + 1, y - 1, DT_TREE)  # Tree pixel
            put(img, x, y, c)  # Paint it
    for y in range(foot, H):  # The dry plain in front
        for x in range(W):  # Each pixel
            c = DT_PLAIN_SH if (y - foot) % 6 == 3 and noise(x // 12 % 80, y) % 3 == 0 else DT_PLAIN  # Faint streaks in the grass
            put(img, x, y, c)  # Paint it
    for i, x in enumerate(range(0, W, 24)):  # Scattered trees on the plain
        if noise(i, 206) % 3 > 0:  # Two in three places
            nb.crown(img, x + noise(i, 207) % 10, foot + 4 + noise(i, 208) % 10, 2 + noise(i, 209) % 2, DT_TREE, DT_TREE_SH)  # One far tree
    return img  # Give the picture back


def dry_tree(img, x, base, h, lit, shade, trunk):
    """A dry-season tree: a thin trunk and a flat, wide, thin crown pushed to the right by the wind."""
    column(img, x, base - h, trunk, base + 1)  # The trunk
    put(img, x + 1, base - h + 2, trunk)  # A branch
    pn.blob(img, x + 2, base - h - 1, h // 2 + 2, max(2, h // 4), shade, 0.9)  # The shaded crown
    pn.blob(img, x + 3, base - h - 2, h // 2, max(1, h // 4 - 1), lit, 0.9)  # Its sunny top


def dt_band():
    """L4: a dry golden grass field with small flat-topped trees and wind-bent tufts."""
    img = mb.new_layer()  # Empty layer
    tops = tops_ground(198, 3, 3, 210)  # The field line
    nb.fill_below(img, tops, DT_GRASS_LIT, DT_GRASS)  # Dry grass
    for i, x in enumerate(range(0, W, 8)):  # Tufts along the field
        cx = (x + noise(i, 211) % 5) % W  # A little varied place
        blown_tuft(img, cx, tops[cx] + 2, 4 + noise(i, 212) % 3, DT_GRASS_SH, DT_GRASS_LIT)  # One tuft
    for i, x in enumerate(range(30, W, 120)):  # Small trees
        cx = (x + noise(i, 213) % 30) % W  # A little varied place
        dry_tree(img, cx, tops[cx] + 2, 9 + noise(i, 214) % 4, DT_TREE, DT_TREE_SH, DT_TRUNK)  # One tree
    return img  # Give the picture back


def boulder(img, x, base, w, h, body, lit):
    """A rounded boulder, with a sunny top on the right."""
    for y in range(base - h, base + 1):  # Rows of the boulder
        for dx in range(-w, w + 1):  # Columns of the boulder
            if (dx / w) ** 2 + ((y - base) / h) ** 2 <= 1:  # Inside the half oval
                put(img, x + dx, y, lit if y < base - h // 2 and dx > -w // 3 else body)  # Light from above right


def dt_near():
    """L5: the near dry earth bank with tall wind-bent grass and dark basalt boulders."""
    img = mb.new_layer()  # Empty layer
    tops = tops_ground(219, 2, 3, 215)  # A gently wavy line near the trail
    nb.fill_below(img, tops, DT_NEAR_LIT, DT_NEAR)  # Dry earth
    for i, x in enumerate(range(6, W, 12)):  # Tall grass along the bank
        cx = (x + noise(i, 216) % 6) % W  # A little varied place
        blown_tuft(img, cx, tops[cx] + 1, 7 + noise(i, 217) % 4, DT_GRASS, DT_GRASS_LIT)  # One tall tuft
    for i, x in enumerate(range(40, W, 160)):  # Basalt boulders
        cx = (x + noise(i, 218) % 40) % W  # A little varied place
        boulder(img, cx, tops[cx] + 2, 6 + noise(i, 219) % 3, 4 + noise(i, 220) % 2, DT_BOULDER, DT_BOULDER_LT)  # One boulder
    return img  # Give the picture back


# ---------- SimienHighlands: the Ethiopian Highlands. A huge sheer escarpment with a green flat top, deep valleys, a far jagged peak ----------

SH_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.2)), (100, lm.SKY), (160, mix(mm.SKY_LIGHT, mm.WHITE, 0.3))]  # Bright high blue
SH_GUST = mix(mm.SKY_LIGHT, mm.WHITE, 0.5)  # Pale gust lines
SH_CLOUD_SH = mix(mm.SKY_LIGHT, lm.HAZE, 0.4)  # Cloud undersides
SH_FAR = mix(lm.MOUNTAIN, lm.HAZE, 0.5)  # The far jagged peak, blue with distance
SH_FAR_SH = mix(lm.MOUNTAIN, lm.HAZE_SH, 0.2)  # Its shaded faces
SH_FAR_TOP = mix(mm.LEAF, lm.HAZE, 0.62)  # Far green plateau edges
SH_VALLEY = mix(mm.SKY_LIGHT, lm.HAZE, 0.5)  # Haze filling the deep valleys
SH_CLIFF = mix(PANTS, lm.HAZE, 0.35)  # The escarpment's basalt face
SH_CLIFF_LT = mix(mm.MASK_SH, lm.HAZE, 0.3)  # Sunny ribs on the face
SH_CLIFF_SH = mix(PANTS, lm.MOUNTAIN_SH, 0.4)  # Shaded ribs and joins
SH_TOP = mix(mm.HILLS_SH, lm.HAZE, 0.25)  # The green flat top
SH_TOP_LIT = mix(mm.HILLS, lm.HAZE, 0.25)  # Its sunny top line
SH_SLOPE = mix(mm.LEAF, lm.HAZE, 0.42)  # Green slopes below the cliff
SH_SLOPE_SH = mix(mm.LEAF, lm.HAZE_SH, 0.25)  # Shaded gullies in the slopes
SH_MIST = mix(mm.SKY_LIGHT, lm.HAZE, 0.35)  # Mist in the deep valley
SH_GRASS = mix(mm.HILLS_SH, mm.LEAF, 0.3)  # Near highland grass
SH_GRASS_LIT = mix(mm.HILLS, mm.HILLS_SH, 0.5)  # Its sunny top line
SH_TUFT = mix(mm.HILLS, lm.PLAINS_SH, 0.5)  # Pale wind-bent grass
SH_LOBELIA = mix(mm.LEAF, SHIRT_SH, 0.4)  # Giant lobelia rosettes
SH_LOBELIA_LT = mix(mm.HILLS_SH, SHIRT, 0.4)  # Their sunny leaves
SH_SPIKE = mix(mm.LEAF, PANTS, 0.4)  # Their tall flower spikes
SH_NEAR = mix(mm.LEAF, mm.STRAP, 0.25)  # Near dark green bank
SH_NEAR_LIT = mix(mm.HILLS_SH, mm.LEAF, 0.4)  # Its sunny top line
SH_STONE = mix(mm.MASK_SH, PANTS, 0.4)  # Grey stones
SH_STONE_LT = mix(mm.MASK, mm.MASK_SH, 0.5)  # Their sunny tops

SH_SCARP = (300, 250, 78)  # The escarpment: middle x, half width of the flat top, top row
SH_SPIRES = ((640, 4, 128), (690, 6, 112), (740, 3, 134), (830, 5, 120), (905, 3, 138))  # Rock needles in the valley: middle x, half width, top row


def sh_sky():
    """L0: a bright high sky."""
    return wind_sky(SH_SKY, (520, 26, 7), (mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4)), SH_GUST, 221)  # Sky, sun and gusts


def sh_clouds():
    """L1: white clouds torn into streaks by the highland wind."""
    return wind_clouds(((120, 40, 130), (430, 20, 160), (760, 48, 120)), mm.WHITE, SH_CLOUD_SH, SH_GUST, 222)  # Streaky clouds


def sh_far():
    """L2: a far jagged peak and far flat plateau edges, with valley haze below."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        peak = pn.cone_row(x, 760, 64, 0.85) + jag(x, 223, 7)  # The jagged peak, very rough
        flat = 112 + (0 if abs(mb.wrap_offset(x, 260)) < 200 else 6 * (abs(mb.wrap_offset(x, 260)) - 200)) + jag(x, 224, 1)  # A far flat plateau with steep ends
        top = int(min(peak, flat))  # The higher of the two
        off = mb.wrap_offset(x, 760)  # Distance from the peak
        for y in range(top, H):  # From the top down
            if peak <= flat:  # The jagged peak
                c = SH_FAR_SH if off < (y - 64) * 0.2 + jag(y, 225, 3) - 2 else SH_FAR  # A ragged shaded left face
            else:  # The far plateau
                c = SH_FAR_TOP if y < top + 2 else SH_FAR  # A thin green edge, then the cliff
            put(img, x, y, c)  # Paint it
    nb.mist_band(img, 126, 24, SH_VALLEY)  # Haze filling the far valleys
    for x in range(W):  # Solid haze below
        column(img, x, 150, SH_VALLEY)  # Haze pixel
    return img  # Give the picture back


def sh_mid():
    """L3: the huge sheer escarpment: a green flat top, a basalt face cut into ribs, green slopes below,
    rock needles in the deep valley and mist filling it."""
    img = mb.new_layer()  # Empty layer
    cx, half, top = SH_SCARP  # The escarpment
    tops, outs, sides, flats = pe.mesa_tops([(cx, half, top, 7.0)], 226)  # Its outline, with very steep ends
    slope_from = 146  # Row where the sheer face gives way to green slopes
    ribs, k = [], 0  # Which rib each column is in, and a rib count
    for x in range(W):  # Ribs of uneven width, so the face does not look built
        if x == 0 or noise(x, 227) % 8 == 0:  # A new rib starts here
            k += 1  # Count it
        ribs.append(k)  # Remember the rib
    for x in range(W):  # Each column
        if outs[x] == 0:  # On the flat top
            tops[x] += (2 if noise(x // 4 % 240, 232) % 9 == 0 else 0) + jag(x, 233, 1)  # Small notches in the edge
        rib = noise(ribs[x], 228) % 4  # Each rib its own shade
        join = x == 0 or ribs[x] != ribs[x - 1]  # A dark join where a new rib starts
        slope_top = slope_from + jag(x, 229, 4)  # A ragged line where the slopes start
        for y in range(tops[x], H):  # From the top down
            if y < tops[x] + 4 and outs[x] == 0:  # The green flat top
                c = SH_TOP_LIT if y == tops[x] else SH_TOP  # A sunny line, then grass
            elif y >= slope_top:  # The green slopes below the cliff
                gully = noise(x // 3 % 320, 229) % 7 == 0  # A few dark gullies
                c = SH_SLOPE_SH if gully else SH_SLOPE  # Grass or gully
            elif outs[x] > 0 and sides[x] < 0:  # The shaded left end
                c = SH_CLIFF_SH  # Shade pixel
            else:  # The sheer face
                c = SH_CLIFF_SH if join and y > tops[x] + noise(x, 234) % 30 else (SH_CLIFF, SH_CLIFF, SH_CLIFF_LT, SH_CLIFF)[rib]  # Ribs with dark joins that start at different heights
                if (y - top) % 9 == 5 and noise(x // 4 % 240, y) % 3 == 0:  # Faint flat lava layers
                    c = SH_CLIFF_SH  # Layer pixel
                if (y - top) % 9 == 0 and noise(x // 6 % 160, y) % 5 == 0:  # Green tufts on small ledges
                    c = SH_SLOPE  # Tuft pixel
            put(img, x, y, c)  # Paint it
    for sx, sh, st in SH_SPIRES:  # Rock needles rising from the valley
        for y in range(st, 186):  # From the tip down
            hw = sh + (y - st) // 9  # A little wider going down
            for dx in range(-hw, hw + 1):  # Across the needle
                c = SH_TOP if y < st + 2 else SH_CLIFF_SH if dx < -hw // 3 else SH_CLIFF  # Green tip, shaded left side
                put(img, sx + dx, y, c)  # Needle pixel
    nb.mist_band(img, 166, 24, SH_MIST)  # Mist filling the deep valley
    for x in range(W):  # Solid mist below
        column(img, x, 190, SH_MIST)  # Mist pixel
    return img  # Give the picture back


def lobelia(img, x, base, h):
    """A giant lobelia: a short stem, a round rosette of leaves and a tall flower spike."""
    column(img, x, base - h // 3, SH_SPIKE, base + 1)  # The stem
    nb.crown(img, x, base - h // 3, 3, SH_LOBELIA_LT, SH_LOBELIA)  # The rosette
    for t in range(h // 3 + 2, h):  # The flower spike
        put(img, x + (t - h // 3) // 6, base - t, SH_SPIKE)  # Spike pixel, bent a little by the wind
        if t % 2 == 0:  # Small flower bumps
            put(img, x + (t - h // 3) // 6 + 1, base - t, SH_LOBELIA)  # Bump pixel


def sh_band():
    """L4: green highland grass with giant lobelias and wind-bent tufts."""
    img = mb.new_layer()  # Empty layer
    tops = tops_ground(198, 3, 2, 230)  # The grass line
    nb.fill_below(img, tops, SH_GRASS_LIT, SH_GRASS)  # Green grass
    for i, x in enumerate(range(0, W, 7)):  # Tufts
        cx = (x + noise(i, 231) % 4) % W  # A little varied place
        blown_tuft(img, cx, tops[cx] + 2, 3 + noise(i, 232) % 3, SH_TUFT)  # One tuft
    for i, x in enumerate(range(50, W, 96)):  # Lobelias
        cx = (x + noise(i, 233) % 30) % W  # A little varied place
        lobelia(img, cx, tops[cx] + 2, 12 + noise(i, 234) % 6)  # One lobelia
    return img  # Give the picture back


def sh_near():
    """L5: the near green bank with long wind-bent tussocks and grey stones."""
    img = mb.new_layer()  # Empty layer
    tops = tops_ground(219, 2, 3, 235)  # A gently wavy line near the trail
    nb.fill_below(img, tops, SH_NEAR_LIT, SH_NEAR)  # Green bank
    for i, x in enumerate(range(4, W, 10)):  # Tussocks
        cx = (x + noise(i, 236) % 5) % W  # A little varied place
        blown_tuft(img, cx, tops[cx] + 1, 6 + noise(i, 237) % 4, SH_TUFT, SH_GRASS_LIT)  # One tussock
    for i, x in enumerate(range(70, W, 140)):  # Stones
        cx = (x + noise(i, 238) % 40) % W  # A little varied place
        boulder(img, cx, tops[cx] + 2, 4 + noise(i, 239) % 3, 3, SH_STONE, SH_STONE_LT)  # One stone
    return img  # Give the picture back


# ---------- SandstoneArches: Tassili n'Ajjer. Orange sandstone pillars, a natural arch and sand dunes under a hot pale sky ----------

SA_SKY = [(0, mix(lm.SKY, lm.HAZE, 0.45)), (90, mix(mm.SKY_LIGHT, mm.HEADLAMP, 0.4)), (160, mix(mm.HEADLAMP, mm.VEST, 0.4))]  # Hot pale sky
SA_CLOUD = mix(mm.WHITE, mm.HEADLAMP, 0.4)  # Thin warm clouds
SA_CLOUD_SH = mix(mm.HEADLAMP, mm.VEST, 0.3)  # Their undersides
SA_DUST = mix(mm.VEST, mm.HEADLAMP, 0.45)  # Blowing dust lines
SA_FAR = mix(mm.PLATEAU, lm.HAZE, 0.6)  # Far rock towers, orange softened to lilac
SA_FAR_SH = mix(mm.PLATEAU_SH, lm.HAZE_SH, 0.5)  # Their shaded sides
SA_FAR_SAND = mix(mm.VEST, lm.HAZE, 0.45)  # The far sand
SA_ROCK = mix(mm.PLATEAU, mm.SCARF, 0.25)  # Orange sandstone
SA_ROCK_LT = mix(mm.PLATEAU, mm.HELMET, 0.35)  # Its sunny right edges
SA_ROCK_SH = mix(mm.PLATEAU_SH, lm.MOUNTAIN, 0.35)  # Shaded faces
SA_BED = mix(mm.PLATEAU_SH, mm.PLATEAU, 0.5)  # Bedding lines in the sandstone
SA_VARNISH = mix(mm.PLATEAU_SH, mm.BACKPACK_SH, 0.4)  # Dark streaks of desert varnish
SA_DUNE = mix(mm.VEST, mm.PLATEAU, 0.4)  # Orange sand dunes
SA_DUNE_LIT = mix(mm.VEST, mm.HEADLAMP, 0.4)  # Their sunny faces
SA_DUNE_SH = mix(mm.PLATEAU, mm.PLATEAU_SH, 0.45)  # Their shaded slip faces
SA_SAND = mix(mm.PLATEAU, mm.VEST, 0.55)  # Near sand
SA_SAND_LIT = mix(mm.VEST, mm.HEADLAMP, 0.5)  # Its sunny top line
SA_RIPPLE = mix(mm.PLATEAU, mm.PLATEAU_SH, 0.35)  # Sand ripples
SA_SLAB = mix(mm.PLATEAU_SH, mm.STRAP, 0.25)  # Low dark rock slabs
SA_SLAB_LT = mix(mm.PLATEAU, mm.PLATEAU_SH, 0.5)  # Their sunny tops
SA_GRASS = mix(mm.VEST_SH, mm.HELMET_SH, 0.4)  # Dry desert grass
SA_NEAR = mix(mm.PLATEAU_SH, mm.GROUND_SH, 0.4)  # Near dark orange rock and sand
SA_NEAR_LIT = mix(mm.PLATEAU, mm.VEST, 0.5)  # Its sunny top line

SA_FOOT = 182  # Row where the pillars stand in the sand
SA_PILLARS = ((40, 12, 96), (96, 6, 118), (180, 16, 88), (250, 8, 112), (330, 5, 124), (700, 14, 100), (770, 7, 116), (860, 18, 92))  # Middle x, half width, top row
SA_ARCH = (520, 66, 92, 520, 176, 42, 64)  # Arch: middle x, half width, top row, hole middle x, hole bottom row, hole half width, hole height


def pillar_half(half, top, cx, y):
    """How wide a worn sandstone pillar is at row y: a little wider going down, with soft wobbles from softer layers."""
    return half + (y - top) // 16 + int(1.6 * math.sin(y / 5 + cx))  # Wider at the foot, wavy sides


def sa_sky():
    """L0: a hot pale desert sky with a white-hot sun."""
    return wind_sky(SA_SKY, (600, 34, 9), (mm.WHITE, mb.SUN, mix(mm.HEADLAMP, mb.SUN_GLOW, 0.4)), SA_DUST, 241)  # Sky, sun and dust lines


def sa_clouds():
    """L1: thin warm streaks of cloud and lines of blowing dust."""
    return wind_clouds(((200, 34, 120), (520, 56, 90), (820, 26, 140)), SA_CLOUD, SA_CLOUD_SH, SA_DUST, 242)  # Streaky clouds


def sa_far():
    """L2: a far line of many rock towers, softened by the hot haze, standing on far sand."""
    img = mb.new_layer()  # Empty layer
    base = 152  # Row of the far sand
    for x in range(W):  # Each column
        best, off = base, 0  # The highest tower here
        for i in range(24):  # 24 towers, one every 40 pixels
            cx = i * 40 + noise(i, 243) % 20  # Its middle
            half = 3 + noise(i, 244) % 9  # Its half width
            top = 116 + noise(i, 245) % 26  # Its top row
            d = abs(mb.wrap_offset(x, cx)) - half  # Distance past its side
            y = top + (0 if d <= 0 else d * 5) + (1 if d == 0 else 0)  # Flat top with steep sides
            if y < best:  # Higher than the others
                best, off = y, mb.wrap_offset(x, cx) + half  # This tower owns the column
        for y in range(int(best), base):  # From the top down to the sand
            put(img, x, y, SA_FAR_SH if off < 4 else SA_FAR)  # Shaded left part
        column(img, x, base, SA_FAR_SAND)  # The far sand
    return img  # Give the picture back


def sa_mid():
    """L3: worn orange sandstone pillars and a natural arch, with sand dunes swept between the rock towers."""
    img = mb.new_layer()  # Empty layer
    for cx, half, top in SA_PILLARS:  # Each pillar
        streak_seed = 246 + cx  # Its own dark streaks
        for y in range(top, SA_FOOT + 1):  # From the top down
            hw = pillar_half(half, top, cx, y)  # Its width at this row
            for dx in range(-hw, hw + 1):  # Across the pillar
                if y < top + 2 and abs(dx) >= hw - (1 - (y - top)):  # Round off the top corners
                    continue  # Leave the corner as sky
                c = SA_ROCK  # The rock face
                if dx < -hw + max(2, hw * 2 // 5):  # The shaded left part
                    c = SA_ROCK_SH  # Shade pixel
                elif dx >= hw - 1:  # The sunny right edge
                    c = SA_ROCK_LT  # Light pixel
                elif noise((cx + dx) % W, streak_seed) % 6 == 0 and y < top + 10 + noise((cx + dx) % W, streak_seed + 1) % 30:  # Dark varnish streaks from the top
                    c = SA_VARNISH  # Streak pixel
                if (y - top) % 7 == 3 and c != SA_ROCK_SH:  # Flat bedding lines in the sandstone
                    c = SA_BED  # Bedding pixel
                put(img, cx + dx, y, c)  # Paint it
    sa_arch(img)  # The natural arch
    for x in range(W):  # Sand dunes swept between the towers
        dune = 176 - int(10 * abs(math.sin(math.pi * 5 * x / W + 0.4)) ** 0.7) + jag(x, 247, 1)  # Sharp crests, five across the width
        nxt = 176 - int(10 * abs(math.sin(math.pi * 5 * (x + 1) / W + 0.4)) ** 0.7)  # The dune line one pixel to the right
        lit = nxt >= dune  # A face going down to the right faces the sun
        for y in range(dune, H):  # From the dune line down
            c = (SA_DUNE_LIT if lit else SA_DUNE_SH) if y < dune + 4 else SA_DUNE  # Lit and slip faces near the crest, then sand
            put(img, x, y, c)  # Paint it
    return img  # Give the picture back


def sa_arch(img):
    """The natural arch: a wide block of sandstone with a tall rounded hole worn through it, the far land showing inside."""
    cx, half, top, hx, hb, hr, hh = SA_ARCH  # Its shape
    for y in range(top, SA_FOOT + 1):  # From the top down
        hw = pillar_half(half, top, cx, y)  # Width at this row
        for dx in range(-hw, hw + 1):  # Across the block
            x = cx + dx  # The column
            if y < top + 3 and abs(dx) > hw - 3 * (3 - (y - top)):  # A rounded top
                continue  # Leave the corner as sky
            e = ((x - hx) / hr) ** 2 + ((y - hb) / hh) ** 2  # 0 at the bottom middle of the hole, 1 at its edge
            if e < 1 and y <= hb:  # Inside the hole
                continue  # The far land shows through
            c = SA_ROCK  # The rock
            if dx < -hw + max(3, hw // 4):  # Shaded left end
                c = SA_ROCK_SH  # Shade pixel
            elif dx >= hw - 1:  # Sunny right end
                c = SA_ROCK_LT  # Light pixel
            elif e < 1.35 and (x > hx or y < hb - hh * 0.75):  # The inner face of the right leg and the underside of the span
                c = SA_ROCK_SH  # They face away from the sun
            elif e < 1.35:  # The inner face of the left leg catches the sun
                c = SA_ROCK_LT  # Light pixel
            if (y - top) % 7 == 3 and c == SA_ROCK:  # Bedding lines
                c = SA_BED  # Bedding pixel
            put(img, x, y, c)  # Paint it


def sa_band():
    """L4: a near sand ridge with ripples, low dark rock slabs, dry grass bent by the wind and sand blowing off the top."""
    img = mb.new_layer()  # Empty layer
    tops = tops_ground(198, 3, 2, 248)  # The sand line
    nb.fill_below(img, tops, SA_SAND_LIT, SA_SAND)  # Sand
    for x in range(W):  # Ripples in the sand
        for y in range(tops[x] + 3 + (H - tops[x] - 3) % 5, H, 5):  # Every five rows
            if (x + 2 * y) % 20 < 7:  # Short slanted ripples
                put(img, x, y, SA_RIPPLE)  # Ripple pixel
        if noise(x // 30 % 32, 249) % 3 == 0 and dither(x, tops[x] - 1, 0.5 - (x % 30) / 60):  # Sand blowing off the top, to the right
            put(img, x, tops[x] - 1, SA_SAND_LIT)  # Blown sand pixel
    for i, x in enumerate(range(20, W, 80)):  # Low rock slabs
        cx = (x + noise(i, 250) % 30) % W  # A little varied place
        boulder(img, cx, tops[cx] + 2, 8 + noise(i, 251) % 6, 3, SA_SLAB, SA_SLAB_LT)  # One flat slab
    for i, x in enumerate(range(0, W, 26)):  # Dry grass
        cx = (x + noise(i, 252) % 10) % W  # A little varied place
        if noise(i, 253) % 2 == 0:  # Half the places
            blown_tuft(img, cx, tops[cx] + 2, 4 + noise(i, 254) % 3, SA_GRASS, SA_SAND_LIT)  # One tuft
    return img  # Give the picture back


def sa_near():
    """L5: the near dark orange bank of rock and sand, with fallen stones and dry wind-bent grass."""
    img = mb.new_layer()  # Empty layer
    tops = tops_ground(219, 2, 3, 255)  # A gently wavy line near the trail
    nb.fill_below(img, tops, SA_NEAR_LIT, SA_NEAR)  # Rock and sand
    for i, x in enumerate(range(10, W, 60)):  # Fallen stones
        cx = (x + noise(i, 256) % 20) % W  # A little varied place
        boulder(img, cx, tops[cx] + 2, 4 + noise(i, 257) % 4, 3 + noise(i, 258) % 2, SA_SLAB, SA_SLAB_LT)  # One stone
    for i, x in enumerate(range(30, W, 34)):  # Dry grass
        cx = (x + noise(i, 259) % 10) % W  # A little varied place
        blown_tuft(img, cx, tops[cx] + 1, 6 + noise(i, 260) % 3, SA_GRASS, SA_SAND_LIT)  # One tuft
    return img  # Give the picture back


# ---------- CliffDwellings: Mesa Verde. A flat green-topped mesa, tan sandstone cliffs, a big alcove with tiny stone houses and towers ----------

CD_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.4)), (100, lm.SKY), (155, mix(lm.SKY, mm.SKY_LIGHT, 0.6))]  # Deep clear blue
CD_GUST = mix(mm.SKY_LIGHT, lm.SKY, 0.3)  # Pale blue gust lines
CD_CLOUD_SH = mix(mm.SKY_LIGHT, lm.HAZE, 0.45)  # Cloud undersides
CD_FAR = mix(mm.VEST, lm.HAZE, 0.4)  # Far mesa cliffs, pale with distance
CD_FAR_SH = mix(mm.VEST_SH, lm.HAZE_SH, 0.55)  # Their shaded sides
CD_FAR_BAND = mix(mm.VEST_SH, lm.HAZE, 0.55)  # Their darker layers
CD_FAR_CAP = mix(mm.LEAF, lm.HAZE, 0.45)  # Their green tops
CD_FAR_CAP_LIT = mix(mm.HILLS_SH, lm.HAZE, 0.4)  # Sunny edge of the green tops
CD_CAP = mix(mm.LEAF, lm.HAZE, 0.35)  # Pinyon and juniper forest on the mesa top
CD_CAP_LIT = mix(mm.HILLS_SH, lm.HAZE, 0.35)  # Its sunny edge
CD_CLIFF = mix(mm.VEST, mm.MASK, 0.35)  # Tan sandstone cliff
CD_CLIFF_LT = mix(mm.VEST, mm.HEADLAMP, 0.4)  # Its sunny ledges
CD_CLIFF_SH = mix(mm.VEST_SH, lm.MOUNTAIN, 0.3)  # Shaded rock
CD_BAND = mix(mm.VEST, mm.PLATEAU, 0.35)  # Reddish layers in the cliff
CD_VARNISH = mix(mm.VEST_SH, PANTS, 0.5)  # Dark streaks down the cliff
CD_ALCOVE = mix(mm.VEST_SH, mm.STRAP, 0.5)  # The deep shade inside the alcove
CD_ALCOVE_RIM = mix(mm.VEST_SH, lm.MOUNTAIN_SH, 0.35)  # The softer shade near its mouth
CD_HOUSE = mix(mm.VEST, mm.HEADLAMP, 0.25)  # Stone house walls
CD_HOUSE_SH = mix(mm.VEST_SH, mm.VEST, 0.5)  # Their shaded sides
CD_DOOR = mix(mm.STRAP, mm.VEST_SH, 0.3)  # Doors and windows
CD_SLOPE = mix(mm.HILLS_SH, mm.VEST_SH, 0.5)  # The slope of rubble and shrubs below the cliff
CD_SLOPE_TREE = mix(mm.LEAF, mm.VEST_SH, 0.35)  # Dark tree dots on the slope
CD_SOIL = mix(mm.VEST_SH, mm.PLATEAU, 0.3)  # Red-tan soil
CD_SOIL_LIT = mix(mm.VEST, mm.PLATEAU, 0.35)  # Its sunny top line
CD_GRASS = mix(mm.HILLS_SH, mm.VEST, 0.45)  # Pale wind-bent grass
CD_JUNIPER = mix(mm.LEAF, lm.HAZE_SH, 0.35)  # Juniper, blue-green
CD_JUNIPER_LT = mix(mm.HILLS_SH, lm.HAZE_SH, 0.4)  # Its sunny side
CD_JUNIPER_SH = mix(mm.LEAF, mm.OUTLINE, 0.3)  # Its shade
CD_PINE = mix(mm.LEAF, SHIRT_SH, 0.35)  # Pinyon needles
CD_PINE_LT = mix(mm.LEAF, mm.HILLS_SH, 0.5)  # Sunny needles
CD_PINE_SH = mix(mm.LEAF, mm.OUTLINE, 0.35)  # Shaded needles
CD_TRUNK = mix(PANTS, mm.GROUND_SH, 0.5)  # Twisted trunks
CD_SAGE = mix(mm.MASK_SH, mm.HILLS_SH, 0.5)  # Sagebrush
CD_SAGE_LT = mix(mm.MASK, lm.PLAINS_SH, 0.5)  # Sunny sagebrush
CD_SAGE_SH = mix(mm.MASK_SH, mm.LEAF, 0.5)  # Shaded sagebrush
CD_NEAR = mix(mm.PLATEAU_SH, mm.GROUND_SH, 0.5)  # Near red soil bank
CD_NEAR_LIT = mix(mm.VEST_SH, mm.PLATEAU, 0.5)  # Its sunny top line

CD_MESA = (470, 300, 86)  # The mesa: middle x, half width of the flat top, top row
CD_ALCOVES = ((420, 140, 66, 24), (720, 126, 26, 11))  # Alcoves: middle x, floor row, half width, height


def cd_sky():
    """L0: a deep clear blue sky."""
    return wind_sky(CD_SKY, (760, 30, 8), (mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4)), CD_GUST, 261)  # Sky, sun and gusts


def cd_clouds():
    """L1: white clouds torn into long streaks by the wind."""
    return wind_clouds(((180, 26, 150), (470, 48, 110), (820, 36, 130)), mm.WHITE, CD_CLOUD_SH, CD_GUST, 262)  # Streaky clouds


def cd_far():
    """L2: far flat mesas with green tops and pale banded cliffs."""
    img = mb.new_layer()  # Empty layer
    mesas = [(60, 90, 120, 2.0), (330, 70, 126, 2.4), (620, 110, 116, 2.0), (860, 50, 128, 2.5)]  # Middle x, half width, top row, steepness
    pe.mesa_layer(img, mesas, 263, (CD_FAR_CAP_LIT, CD_FAR_CAP), 4, CD_FAR, CD_FAR_SH, CD_FAR_BAND, CD_FAR, CD_FAR_SH)  # Green-topped far mesas
    return img  # Give the picture back


def cd_mid():
    """L3: the big mesa: a green forest top, tan sandstone cliffs with reddish layers, alcoves under overhangs
    holding tiny stone houses and square towers, and a slope of shrubs below."""
    img = mb.new_layer()  # Empty layer
    cx, half, top = CD_MESA  # The mesa
    tops, outs, sides, flats = pe.mesa_tops([(cx, half, top, 3.5)], 264)  # Its outline
    slope_from = 152  # Row where the cliff meets the rubble slope
    for x in range(W):  # Each column
        streak = noise(x, 265) % 14 == 0  # A few columns carry a dark streak
        streak_to = top + 6 + noise(x, 266) % 18  # How far the streak runs
        slope_top = slope_from + jag(x, 267, 3)  # A ragged top to the slope
        for y in range(tops[x], H):  # From the top down
            if y < tops[x] + 4 and outs[x] == 0:  # The forest on the flat top
                c = CD_CAP_LIT if y == tops[x] else CD_CAP  # Sunny edge, then trees
            elif y >= slope_top:  # The slope below the cliff
                c = CD_SLOPE_TREE if noise((x * 3 + y * 5) % 960, 268) % 7 == 0 else CD_SLOPE  # Shrubs dotted on rubble
            elif outs[x] > 0 and sides[x] < 0:  # The shaded left end
                c = CD_CLIFF_SH  # Shade pixel
            else:  # The cliff
                layer = (y - top) // 6 % 4  # Flat layers
                c = CD_BAND if layer == 2 else CD_CLIFF  # Reddish and tan layers
                if streak and y < streak_to:  # A dark streak
                    c = CD_VARNISH  # Streak pixel
            put(img, x, y, c)  # Paint it
        if outs[x] == 0 and noise(x // 2 % 480, 269) % 3 == 0:  # Tree bumps along the top
            put(img, x, tops[x] - 1, CD_CAP)  # Bump pixel
    for ax, floor, rx, ry in CD_ALCOVES:  # Each alcove
        cd_alcove(img, ax, floor, rx, ry)  # Draw it
    return img  # Give the picture back


def cd_alcove(img, ax, floor, rx, ry):
    """An alcove worn under an overhang: a dark rounded hollow with a flat floor, tiny stone houses and square towers inside."""
    for y in range(floor - ry, floor + 1):  # Rows of the hollow
        for x in range(ax - rx, ax + rx + 1):  # Columns of the hollow
            e = ((x - ax) / rx) ** 2 + ((y - floor) / ry) ** 2  # 0 at the floor middle, 1 at the edge
            if e <= 1:  # Inside the hollow
                put(img, x, y, CD_ALCOVE_RIM if e > 0.7 else CD_ALCOVE)  # Softer shade near the mouth
    for x in range(ax - rx - 3, ax + rx + 4):  # The sunny lip of the overhang above
        put(img, x, floor - ry - 1, CD_CLIFF_LT)  # Lip pixel
    for x in range(ax - rx, ax + rx + 1):  # The floor ledge
        put(img, x, floor + 1, CD_CLIFF_LT)  # Ledge pixel
        put(img, x, floor + 2, CD_CLIFF_SH)  # Its shaded edge
    x, i = ax - rx + 6, 0  # First house place, and a count
    while x < ax + rx - 6:  # Houses along the floor
        room = int(ry * math.sqrt(max(0.0, 1 - ((x - ax) / rx) ** 2))) - 2  # Height free under the roof here
        tower = noise(i, 270 + ax) % 4 == 0  # Some are square towers
        w = 3 if tower else 3 + noise(i, 271 + ax) % 5  # Width
        h = min(room, 8 + noise(i, 272 + ax) % 4 if tower else 3 + noise(i, 273 + ax) % 4)  # Height, kept under the roof
        if h >= 2:  # Room for a building here
            for bx in range(x, x + w):  # Each column of it
                column(img, bx, floor - h + 1, CD_HOUSE_SH if bx == x else CD_HOUSE, floor + 1)  # Wall, shaded on the left
            put(img, x + w // 2, floor, CD_DOOR)  # A door
            if h >= 4:  # Tall enough for a window
                put(img, x + w // 2, floor - h + 2, CD_DOOR)  # A small window
        x += w + 1 + noise(i, 274 + ax) % 2  # Next building
        i += 1  # Count it


def juniper(img, x, base, h, lit, mid, shade, trunk):
    """A juniper: a short shaggy trunk and a ragged upright crown, combed to the right by the wind."""
    column(img, x, base - h // 3, trunk, base + 1)  # The trunk
    put(img, x + 1, base - h // 4, trunk)  # A forked branch
    ry = (h - h // 4) // 2  # Half height of the crown
    cy = base - h // 4 - ry  # Middle row of the crown
    rx = max(2, h // 3)  # Half width of the crown
    for y in range(cy - ry, cy + ry + 1):  # Rows of the crown
        lean = (cy + ry - y) // 4  # The top leans right in the wind
        for dx in range(-rx, rx + 1):  # Columns of the crown
            if (dx / rx) ** 2 + ((y - cy) / ry) ** 2 <= 1 and noise((x + dx) % W, y) % 5 > 0:  # Inside, ragged
                put(img, x + dx + lean, y, lit if dx > rx // 3 and y < cy else shade if dx < -rx // 3 else mid)  # Light from the upper right


def cd_band():
    """L4: red-tan soil with pinyon pines, junipers and wind-bent grass."""
    img = mb.new_layer()  # Empty layer
    tops = tops_ground(198, 2, 3, 275)  # The soil line
    nb.fill_below(img, tops, CD_SOIL_LIT, CD_SOIL)  # Soil
    for i, x in enumerate(range(0, W, 9)):  # Grass
        cx = (x + noise(i, 276) % 5) % W  # A little varied place
        blown_tuft(img, cx, tops[cx] + 2, 3 + noise(i, 277) % 3, CD_GRASS)  # One tuft
    for i, x in enumerate(range(10, W, 48)):  # Trees
        cx = (x + noise(i, 278) % 16) % W  # A little varied place
        if i % 2:  # Every second one a pinyon
            pn.pinyon(img, cx, tops[cx] + 1, 10 + noise(i, 279) % 4, CD_PINE_LT, CD_PINE, CD_PINE_SH, CD_TRUNK)  # One pinyon
        else:  # The others junipers
            juniper(img, cx, tops[cx] + 1, 12 + noise(i, 280) % 5, CD_JUNIPER_LT, CD_JUNIPER, CD_JUNIPER_SH, CD_TRUNK)  # One juniper
    return img  # Give the picture back


def cd_near():
    """L5: the near red soil bank with bigger junipers, sagebrush and wind-bent grass."""
    img = mb.new_layer()  # Empty layer
    tops = tops_ground(219, 2, 3, 281)  # A gently wavy line near the trail
    nb.fill_below(img, tops, CD_NEAR_LIT, CD_NEAR)  # Soil bank
    for i, x in enumerate(range(60, W, 160)):  # Big junipers
        cx = (x + noise(i, 282) % 40) % W  # A little varied place
        juniper(img, cx, tops[cx] + 1, 20 + noise(i, 283) % 5, CD_JUNIPER_LT, CD_JUNIPER, CD_JUNIPER_SH, CD_TRUNK)  # One big juniper
    for i, x in enumerate(range(20, W, 40)):  # Sagebrush
        cx = (x + noise(i, 284) % 12) % W  # A little varied place
        pn.sagebrush(img, cx, tops[cx] + 2, 3 + noise(i, 285) % 2, CD_SAGE_LT, CD_SAGE, CD_SAGE_SH, CD_TRUNK)  # One bush
    for i, x in enumerate(range(0, W, 14)):  # Grass
        cx = (x + noise(i, 286) % 6) % W  # A little varied place
        blown_tuft(img, cx, tops[cx] + 1, 5 + noise(i, 287) % 3, CD_GRASS)  # One tuft
    return img  # Give the picture back


# ---------- IcePlateau: the Antarctic Plateau. Endless flat white ice, a low cold sun, faint blue ice, wind-blown snow ----------

IP_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.25)), (80, mix(mm.SKY_LIGHT, lm.HAZE, 0.4)), (138, mix(lm.SNOW, mb.SUN_GLOW, 0.3))]  # Pale cold sky, warm low near the sun
IP_HORIZON = 140  # Row of the flat ice horizon
IP_GUST = mix(lm.SNOW, mm.SKY_LIGHT, 0.4)  # Pale gust lines
IP_CLOUD = mix(mm.WHITE, mm.SKY_LIGHT, 0.3)  # Thin icy clouds
IP_CLOUD_SH = mix(mm.SKY_LIGHT, lm.HAZE, 0.5)  # Their undersides
IP_ROCK = mix(mm.STRAP, lm.HAZE, 0.6)  # The very low far nunataks
IP_ROCK_SH = mix(mm.STRAP, lm.HAZE_SH, 0.45)  # Their shaded sides
IP_FAR_ICE = mix(lm.SNOW, lm.HAZE, 0.35)  # The far ice, a little blue with distance
IP_ICE = mix(lm.SNOW, mm.SKY_LIGHT, 0.25)  # The ice sheet
IP_BLUE = mix(lm.LENS, lm.SNOW, 0.6)  # Faint blue ice patches
IP_BLUE_DEEP = mix(lm.LENS, lm.HAZE, 0.5)  # Their deeper middles
IP_SASTRUGI = mix(lm.HAZE, lm.SNOW, 0.45)  # Wind-cut snow ridges, shaded side
IP_SNOW_BLOW = mm.WHITE  # Blowing snow
IP_DRIFT = mix(lm.SNOW, mm.WHITE, 0.5)  # Near snow drifts
IP_DRIFT_SH = mix(lm.HAZE, lm.SNOW, 0.4)  # Their wind-cut shaded faces
IP_NEAR = mix(lm.HAZE, lm.SNOW, 0.5)  # Near snow bank, in the long shadow of low sun
IP_NEAR_LIT = mix(lm.SNOW, mb.SUN_GLOW, 0.25)  # Its top line, warm from the low sun
IP_NEAR_GROOVE = mix(lm.HAZE_SH, lm.HAZE, 0.5)  # Grooves cut by the wind


def ip_sky():
    """L0: a pale cold sky with the sun hanging low over the ice."""
    img = wind_sky(IP_SKY, (680, 120, 7), (mm.WHITE, mb.SUN, mix(lm.SNOW, mb.SUN_GLOW, 0.5)), IP_GUST, 301)  # Sky, low sun and gusts
    for y in range(100, 140):  # A faint pillar of light above and below the low sun
        if dither(680, y, 0.5) and abs(y - 120) > 10:  # Dotted, outside the sun itself
            put(img, 680, y, mix(lm.SNOW, mb.SUN_GLOW, 0.5))  # Pillar pixel
    return img  # Give the picture back


def ip_clouds():
    """L1: a few thin icy streaks, and many lines of blowing snow."""
    return wind_clouds(((240, 30, 170), (600, 18, 130)), IP_CLOUD, IP_CLOUD_SH, IP_GUST, 302, (40, 56, 72, 90, 108))  # Thin clouds and gusts


def ip_far():
    """L2: the dead flat ice horizon, with only a very low line of far nunataks breaking it."""
    img = mb.new_layer()  # Empty layer
    rocks = ((90, 5, 18), (130, 3, 10), (420, 6, 24), (470, 3, 12), (760, 4, 16), (880, 2, 10))  # Middle x, height, half width
    for x in range(W):  # Each column
        best, off = IP_HORIZON, 0  # The highest rock here
        for cx, h, half in rocks:  # Each nunatak
            d = abs(mb.wrap_offset(x, cx)) / half  # 0 in the middle, 1 at the ends
            y = IP_HORIZON - h * max(0.0, 1 - d * d) + (jag(x, 303, 1) if d < 1 else 0)  # A low rough bump
            if y < best:  # Higher than the others
                best, off = y, mb.wrap_offset(x, cx)  # This rock owns the column
        for y in range(int(best), IP_HORIZON):  # From the top down to the ice
            put(img, x, y, IP_ROCK_SH if off < 0 else IP_ROCK)  # Shaded left side
        put(img, x, IP_HORIZON, lm.SNOW)  # A bright line at the horizon
        column(img, x, IP_HORIZON + 1, IP_FAR_ICE)  # The far ice
    return img  # Give the picture back


def ip_mid():
    """L3: the endless flat ice sheet: faint blue ice patches, small wind-cut ridges and long streaks of blowing snow."""
    img = mb.new_layer()  # Empty layer
    start = 148  # First row of this layer
    for y in range(start, H):  # The ice
        for x in range(W):  # Each pixel
            c = IP_ICE if y > start or dither(x, y, 0.5) else IP_FAR_ICE  # A soft top edge
            s = (x + y * 13) // 8 % 120  # Which short stretch, for the wind-cut ridges
            if y % 4 == 2 and noise(s, y) % 6 == 0:  # A small ridge here
                c = IP_SASTRUGI  # Ridge shade pixel
            put(img, x, y, c)  # Paint it
    for cx, cy, rx, ry in ((80, 160, 50, 2), (300, 172, 70, 3), (560, 156, 40, 2), (700, 184, 90, 4), (900, 166, 50, 2)):  # Blue ice patches, flat with distance
        pn.blob(img, cx, cy, rx, ry, IP_BLUE, 0.85)  # The patch
        pn.blob(img, cx + rx // 5, cy, rx // 2, max(1, ry - 1), IP_BLUE_DEEP, 0.6)  # Its deeper middle
    gusts(img, IP_SNOW_BLOW, 304, (152, 158, 165, 174, 186))  # Long streaks of blowing snow over the ice
    return img  # Give the picture back


def ip_band():
    """L4: near snow drifts cut by the wind into sharp ridges, with snow blowing off the crests."""
    img = mb.new_layer()  # Empty layer
    tops = []  # The drift line
    for x in range(W):  # Each column
        p = x % 120 / 120  # Place inside one 120-pixel drift; 120 divides 960
        rise = p / 0.85 if p < 0.85 else (1 - p) / 0.15  # A long gentle back and a short steep face
        tops.append(201 - int(4 * rise) + jag(x, 305, 1))  # The drift line
    for x in range(W):  # Each column
        steep = x % 120 >= 102  # The short steep face, away from the sun
        put(img, x, tops[x], mm.WHITE)  # A bright top line
        for y in range(tops[x] + 1, H):  # The drift
            put(img, x, y, IP_DRIFT_SH if steep and y < tops[x] + 3 and dither(x, y, 0.7) else IP_DRIFT)  # Soft shade on the steep face
        q = x % 120 - 102  # Place past the crest
        if 0 <= q < 18 and dither(x, tops[x] - 2 - q // 6, 0.7 - q / 26):  # Snow blowing off the crest, thinning out
            put(img, x, tops[x] - 2 - q // 6, IP_SNOW_BLOW)  # Plume pixel
    return img  # Give the picture back


def ip_near():
    """L5: the near snow bank in the long shadow of the low sun, with grooves cut by the wind and a few blue ice chunks."""
    img = mb.new_layer()  # Empty layer
    tops = tops_ground(219, 2, 3, 306)  # A gently wavy line near the trail
    nb.fill_below(img, tops, IP_NEAR_LIT, IP_NEAR)  # Snow bank
    for x in range(W):  # Grooves cut by the wind
        for y in range(tops[x] + 3, H, 7):  # Every seven rows
            if (x + y * 9) % 80 < 22 and noise((x + y * 9) // 80 % 12, y) % 2 == 0:  # Long flat grooves, with gaps
                put(img, x, y, IP_NEAR_GROOVE)  # Groove pixel
    for i, x in enumerate(range(40, W, 120)):  # Blue ice chunks
        cx = (x + noise(i, 307) % 40) % W  # A little varied place
        boulder(img, cx, tops[cx] + 1, 4 + noise(i, 308) % 3, 3, IP_BLUE_DEEP, IP_BLUE)  # One chunk
    return img  # Give the picture back


# ---------- LoessTerraces: the Loess Plateau. Hazy yellow sky, rounded yellow hills cut by gullies, farm terraces, cave homes ----------

LT_SKY = [(0, mix(lm.HAZE, mm.VEST, 0.3)), (80, mix(mm.HEADLAMP, mm.VEST, 0.45)), (150, mix(mm.HEADLAMP, mm.VEST, 0.65))]  # Hazy yellow sky
LT_GUST = mix(mm.HEADLAMP, mm.VEST, 0.25)  # Pale dust lines
LT_CLOUD = mix(mm.HEADLAMP, mm.WHITE, 0.5)  # Dusty clouds
LT_CLOUD_SH = mix(mm.VEST, mm.HEADLAMP, 0.55)  # Their undersides
LT_FAR = mix(mm.VEST, mm.MASK, 0.5)  # Far loess hills, pale with dust
LT_FAR_SH = mix(mm.VEST_SH, mm.MASK_SH, 0.5)  # Their shaded sides
LT_FAR_LINE = mix(mm.VEST_SH, mm.MASK, 0.5)  # Their faint terrace lines
LT_EDGE = mix(mm.VEST_SH, mm.GROUND, 0.4)  # Terrace edges, bare loess risers
LT_EDGE_SH = mix(mm.VEST_SH, mm.GROUND_SH, 0.5)  # Shaded risers
LT_GREEN = mix(mm.HILLS_SH, mm.VEST, 0.4)  # Green crops
LT_GREEN_SH = mix(mm.LEAF, mm.VEST_SH, 0.5)  # Green crops in shade
LT_YELLOW = mix(mm.HELMET, mm.VEST, 0.5)  # Ripe yellow crops
LT_YELLOW_SH = mix(mm.HELMET_SH, mm.VEST_SH, 0.5)  # Yellow crops in shade
LT_LOESS = mix(mm.VEST, mm.HEADLAMP, 0.25)  # Bare yellow loess
LT_LOESS_SH = mix(mm.VEST_SH, mm.VEST, 0.5)  # Bare loess in shade
LT_GULLY = mix(mm.GROUND, mm.VEST, 0.5)  # The sunny wall of a gully
LT_GULLY_SH = mix(mm.GROUND_SH, mm.VEST_SH, 0.5)  # The shaded wall of a gully
LT_WALL = mix(mm.VEST, mm.HEADLAMP, 0.45)  # The fresh-cut face where the cave homes are dug
LT_YARD = mix(mm.VEST, mm.GROUND, 0.4)  # The flat yard in front
LT_FRAME = mix(mm.MASK, mm.VEST, 0.5)  # Pale arch frames
LT_CAVE = mix(mm.STRAP, mm.GROUND_SH, 0.5)  # Dark doors
LT_PAPER = mix(mm.HEADLAMP, mm.WHITE, 0.3)  # Paper in the window lattice
LT_TREE = mix(mm.LEAF, mm.VEST, 0.3)  # Small trees
LT_TREE_SH = mix(mm.LEAF, mm.VEST_SH, 0.4)  # Their shade
LT_TRUNK = mix(PANTS, mm.GROUND_SH, 0.5)  # Trunks
LT_NEAR = mix(mm.VEST_SH, mm.GROUND, 0.5)  # Near loess bank
LT_NEAR_LIT = mix(mm.VEST, mm.HEADLAMP, 0.3)  # Its sunny top line
LT_CRACK = mix(mm.VEST_SH, mm.GROUND_SH, 0.6)  # Upright cracks in the loess
LT_MILLET = mix(mm.HELMET_SH, mm.VEST, 0.4)  # Millet stalks
LT_MILLET_HEAD = mix(mm.HELMET, mm.HEADLAMP, 0.3)  # Their heads

LT_HILLS = ((120, 108, 150), (390, 98, 170), (650, 112, 140), (860, 104, 120))  # Middle x, top row, half width
LT_FOOT = 200  # Row where the hills meet
LT_GULLIES = ((60, 3), (190, 4), (330, 3), (470, 4), (730, 3), (805, 4), (925, 3))  # Middle x, extra half width at the bottom
LT_CAVES = (596, 668, 156, 170)  # Cut face: left x, right x, top row, yard row


def loess_tops(hills, foot, power, seed):
    """The outline of rounded loess hills. Returns, for each x, the top row and (signed distance, top row, half width) of its hill."""
    tops, owners = [], []  # Top row and owner for each x
    for x in range(W):  # Each column
        best, owner = foot, (0, foot, 1)  # Lowest row number wins
        for cx, top, half in hills:  # Each hill
            off = mb.wrap_offset(x, cx)  # Signed distance from its middle
            y = top + (foot - top) * min(1.0, abs(off / half)) ** power  # A round top that gets steeper going out
            if y < best:  # Higher than the others
                best, owner = y, (off, top, half)  # This hill owns the column
        tops.append(int(best) + jag(x, seed, 1))  # Whole rows, a little rough
        owners.append(owner)  # Remember the owner
    return tops, owners  # Give the outline back


def lt_sky():
    """L0: a hazy yellow sky full of fine dust, the sun dim through it."""
    return wind_sky(LT_SKY, (300, 34, 8), (mm.HEADLAMP, mix(mm.HEADLAMP, mm.HELMET, 0.4), mix(mm.HEADLAMP, mm.VEST, 0.35)), LT_GUST, 321)  # Sky, dim sun and dust lines


def lt_clouds():
    """L1: dusty clouds torn into streaks, and lines of blowing dust."""
    return wind_clouds(((150, 40, 140), (480, 24, 110), (780, 50, 150)), LT_CLOUD, LT_CLOUD_SH, LT_GUST, 322)  # Streaky clouds


def lt_far():
    """L2: far rounded loess hills fading into the dust, with faint terrace lines."""
    img = mb.new_layer()  # Empty layer
    hills = ((60, 118, 110), (260, 124, 90), (470, 114, 120), (700, 126, 100), (880, 120, 80))  # Middle x, top row, half width
    tops, owners = loess_tops(hills, 156, 2.2, 323)  # Their outline
    for x in range(W):  # Each column
        off, top, _ = owners[x]  # The hill here
        for y in range(tops[x], H):  # From the top down
            shade = off < (y - top) * 0.3 - 3  # The shaded left face
            c = LT_FAR_SH if shade else LT_FAR  # Shade or sun
            if (y - top) % 4 == 2 and y < 156 and noise(x // 6 % 160, y) % 4 > 0:  # Faint terrace lines
                c = LT_FAR_LINE  # Line pixel
            put(img, x, y, c)  # Paint it
    return img  # Give the picture back


def lt_field(x, y, tier, shade):
    """The colour of a terrace field: green crops, ripe yellow crops or bare loess, changing every few fields."""
    k = noise((x + tier * 13) // 48 % 20, tier + 330) % 5  # Which crop this field holds; 20 fields of 48 pixels make 960, so they tile
    if k < 2:  # Green crops
        c = LT_GREEN_SH if shade else LT_GREEN  # Green
    elif k < 4:  # Yellow crops
        c = LT_YELLOW_SH if shade else LT_YELLOW  # Yellow
    else:  # Bare loess
        c = LT_LOESS_SH if shade else LT_LOESS  # Loess
    if k < 4 and noise((x * 3 + y) % 960, tier) % 9 == 0:  # A few darker plants
        c = LT_EDGE_SH if shade else LT_EDGE  # A darker row
    return c  # Give the colour back


def lt_mid():
    """L3: rounded loess hills wrapped in many farm terraces, cut by deep gullies, with cave homes dug into one hillside."""
    img = mb.new_layer()  # Empty layer
    tops, owners = loess_tops(LT_HILLS, LT_FOOT, 2.4, 324)  # The hill outline
    for x in range(W):  # Each column
        off, top, half = owners[x]  # The hill here
        for y in range(tops[x], H):  # From the top down
            shade = dither(x, y, (-off / half - 0.3) / 0.25)  # The left flank turns into shade, softly
            tier = (y - 90) // 5  # Which terrace this row is in; the terraces are flat, so the same on every hill
            if (y - 90) % 5 == 0:  # The bare riser at the edge of a terrace
                c = LT_EDGE_SH if shade else LT_EDGE  # Riser pixel
            else:  # The field on the terrace
                c = lt_field(x, y, tier, shade)  # Field pixel
            put(img, x, y, c)  # Paint it
    for gx, gh in LT_GULLIES:  # Deep gullies cut into the hills
        g_top = tops[gx % W] + 6  # Where the gully starts, a little below the hill top
        for y in range(g_top, LT_FOOT + 6):  # Down the gully
            mid = gx + int(3 * math.sin((y - g_top) / 6 + gx))  # The gully winds a little
            w = 1 + gh * (y - g_top) // (LT_FOOT + 6 - g_top)  # Narrow at the top, wider lower down
            for dx in range(-w, w + 1):  # Across the gully
                put(img, mid + dx, y, LT_GULLY_SH if dx < 0 else LT_GULLY if dx < w else LT_LOESS)  # Shaded left wall, sunny floor, a bright right lip
    lt_caves(img)  # The cave homes
    return img  # Give the picture back


def lt_caves(img):
    """Cave homes dug into a hillside: a flat cut face with arched doorways, window lattices and a yard."""
    x0, x1, top, yard = LT_CAVES  # The cut face
    for x in range(x0, x1 + 1):  # Each column of the face
        column(img, x, top, LT_WALL, yard)  # Face pixel
        put(img, x, top - 1, LT_EDGE)  # The edge of the field above
        for y in range(yard, yard + 3):  # The flat yard
            put(img, x, y, LT_YARD)  # Yard pixel
    for ax in range(x0 + 8, x1 - 4, 16):  # Arched doorways
        for y in range(top + 3, yard):  # Rows of the arch
            for dx in range(-5, 6):  # Columns of the arch
                rise = top + 3 + (5 - int(math.sqrt(max(0, 25 - dx * dx))))  # Round top of the arch
                if y < rise:  # Above the arch
                    continue  # Leave the face
                edge = abs(dx) == 5 or y == rise  # The pale frame
                inner = y < top + 8  # The window lattice in the arch top
                c = LT_FRAME if edge else (LT_PAPER if (dx + y) % 2 else LT_FRAME) if inner else LT_CAVE  # Frame, lattice or dark door
                if not edge and not inner and abs(dx) > 2:  # Beside the door, the lower wall of the arch
                    c = LT_PAPER if y < top + 11 and abs(dx) == 3 else LT_FRAME  # Small side windows
                put(img, ax + dx, y, c)  # Paint it


def lt_band():
    """L4: a near terrace: a bare riser, then a strip of crops in rows, with small trees and wind-bent grass."""
    img = mb.new_layer()  # Empty layer
    tops = tops_ground(196, 1, 2, 331)  # The terrace edge
    for x in range(W):  # Each column
        for y in range(tops[x], H):  # From the edge down
            d = y - tops[x]  # Rows below the edge
            if d < 3:  # The riser
                c = LT_NEAR_LIT if d == 0 else LT_EDGE  # Sunny lip, then bare loess
            else:  # The crops
                k = noise(x // 40 % 24, 332) % 3  # Which crop in this field
                c = (LT_GREEN, LT_YELLOW, LT_GREEN)[k]  # Green or yellow
                if (y - tops[x]) % 3 == 0:  # Rows of plants
                    c = (LT_GREEN_SH, LT_YELLOW_SH, LT_GREEN_SH)[k]  # Darker row
            put(img, x, y, c)  # Paint it
    for i, x in enumerate(range(20, W, 80)):  # Small trees
        cx = (x + noise(i, 333) % 30) % W  # A little varied place
        column(img, cx, tops[cx] - 8, LT_TRUNK, tops[cx] + 1)  # Trunk
        nb.crown(img, cx + 1, tops[cx] - 10, 3 + noise(i, 334) % 2, LT_TREE, LT_TREE_SH)  # Crown
    for i, x in enumerate(range(0, W, 11)):  # Grass on the lip
        cx = (x + noise(i, 335) % 5) % W  # A little varied place
        blown_tuft(img, cx, tops[cx], 3 + noise(i, 336) % 2, LT_MILLET)  # One tuft
    return img  # Give the picture back


def lt_near():
    """L5: the near loess bank with upright cracks and millet bent by the wind."""
    img = mb.new_layer()  # Empty layer
    tops = tops_ground(219, 2, 3, 337)  # A gently wavy line near the trail
    nb.fill_below(img, tops, LT_NEAR_LIT, LT_NEAR)  # Loess bank
    for x in range(0, W, 24):  # Upright cracks, as loess splits
        cx = x + noise(x // 24, 338) % 10  # A little varied place
        for y in range(tops[cx % W] + 3, tops[cx % W] + 3 + 6 + noise(x // 24, 339) % 10):  # Down the crack
            put(img, cx, y, LT_CRACK)  # Crack pixel
    for i, x in enumerate(range(4, W, 12)):  # Millet
        cx = (x + noise(i, 340) % 6) % W  # A little varied place
        blown_tuft(img, cx, tops[cx] + 1, 7 + noise(i, 341) % 3, LT_MILLET, LT_MILLET_HEAD)  # One clump
    return img  # Give the picture back


# ---------- RoofOfWorld: the Tibetan Plateau. Deep blue sky, very high flat grassland, a turquoise lake, a far row of snowy peaks ----------

RW_SKY = [(0, mix(lm.MOUNTAIN_SH, lm.SKY, 0.3)), (60, mix(lm.MOUNTAIN, lm.SKY, 0.45)), (130, lm.SKY), (160, mix(lm.SKY, mm.SKY_LIGHT, 0.6))]  # Deep blue thin air
RW_GUST = mix(mm.SKY_LIGHT, lm.SKY, 0.4)  # Pale gust lines
RW_CLOUD_SH = mix(mm.SKY_LIGHT, lm.HAZE, 0.45)  # Cloud undersides
RW_ROCK = mix(lm.MOUNTAIN, lm.HAZE, 0.5)  # Far peaks, blue with distance
RW_ROCK_SH = mix(lm.MOUNTAIN, lm.HAZE_SH, 0.2)  # Their shaded faces
RW_SNOW = mm.WHITE  # Sunny snow
RW_SNOW_SH = mix(mm.SKY_LIGHT, lm.HAZE, 0.5)  # Shaded snow
RW_FAR_MIST = mix(lm.HAZE, mm.SKY_LIGHT, 0.5)  # Haze at the foot of the peaks
RW_HILL = mix(mm.VEST_SH, lm.HAZE, 0.45)  # Low brown hills across the lake
RW_HILL_SH = mix(mm.VEST_SH, lm.HAZE_SH, 0.3)  # Their shade
RW_STEPPE = mix(lm.PLAINS_SH, mm.VEST, 0.5)  # The high dry grassland
RW_STEPPE_SH = mix(lm.PLAINS_SH, mm.VEST_SH, 0.5)  # Darker grass streaks
RW_SHORE = mix(mm.MASK, mm.VEST, 0.5)  # The pale stony shore
RW_LAKE = mix(lm.LENS, SHIRT, 0.35)  # Turquoise lake
RW_LAKE_DEEP = mix(SHIRT, lm.LENS, 0.3)  # Deeper turquoise
RW_LAKE_LT = mix(lm.LENS, mm.WHITE, 0.45)  # Shallow pale edge
RW_WAVE = mm.WHITE  # Wind-driven whitecaps
RW_GRASS = mix(lm.PLAINS_SH, mm.VEST_SH, 0.35)  # Near grassland
RW_GRASS_LIT = mix(mm.PLAINS, mm.VEST, 0.4)  # Its sunny top line
RW_TUFT = mix(lm.PLAINS_SH, mm.HEADLAMP, 0.35)  # Pale wind-bent grass
RW_YAK = mix(mm.STRAP, BOOTS, 0.5)  # Dark yaks
RW_HORN = mix(mm.MASK, mm.HEADLAMP, 0.5)  # Their pale horns
RW_NEAR = mix(mm.VEST_SH, mm.LEAF, 0.4)  # Near tussock bank
RW_NEAR_LIT = mix(lm.PLAINS_SH, mm.VEST, 0.4)  # Its sunny top line
RW_STONE = mix(mm.MASK_SH, lm.HAZE_SH, 0.4)  # Grey stones
RW_STONE_LT = mix(mm.MASK, lm.HAZE, 0.4)  # Their sunny tops
RW_FLOWER = lm.SNOW  # Tiny white flowers

RW_PEAKS = [(20, 104, 12), (95, 112, 8), (170, 98, 14), (250, 110, 8), (330, 100, 12), (410, 114, 6), (490, 96, 14),  # First half of the row
            (570, 108, 8), (650, 100, 12), (730, 112, 8), (810, 98, 14), (890, 108, 10)]  # A long row of snowy peaks: middle x, top row, snow depth


def rw_sky():
    """L0: a deep blue sky of very thin high air."""
    return wind_sky(RW_SKY, (200, 26, 7), (mm.WHITE, mb.SUN, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.3)), RW_GUST, 341)  # Sky, sun and gusts


def rw_clouds():
    """L1: bright clouds torn into long streaks by the high wind."""
    return wind_clouds(((140, 40, 160), (520, 28, 120), (820, 52, 140)), mm.WHITE, RW_CLOUD_SH, RW_GUST, 342)  # Streaky clouds


def rw_far():
    """L2: a long row of snowy peaks far away, with haze at their foot."""
    return nb.ranged(RW_PEAKS, 0.75, 343, 3, 2, RW_ROCK, RW_ROCK_SH, RW_SNOW, RW_SNOW_SH, RW_FAR_MIST, 126)  # The far range


def rw_lake(x):
    """The top and bottom rows of the lake at column x, or None where the grassland reaches the far shore."""
    f = math.sin(2 * math.pi * x / W + 0.5) + 0.3 * math.sin(2 * math.pi * 3 * x / W)  # A smooth field across the width
    half = min(7.0, (f + 0.45) * 12)  # How thick the lake is here
    if half < 1:  # Too thin: dry land
        return None  # No lake here
    mid = 160 + int(1.5 * math.sin(2 * math.pi * 2 * x / W))  # The middle row of the lake
    return mid - int(half * 0.6), mid + int(half)  # Thin far side, wider near side


def rw_mid():
    """L3: the very high flat grassland: low brown hills across a long turquoise lake ruffled by the wind, and dry grass."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        hill = 146 - int(5 * max(0.0, math.sin(2 * math.pi * 4 * x / W + 1)) + 2 * math.sin(2 * math.pi * 9 * x / W)) + jag(x, 344, 1)  # Low rounded hills
        for y in range(hill, H):  # From the hill top down
            c = RW_HILL_SH if y < 150 and math.cos(2 * math.pi * 4 * x / W + 1) > 0.3 else RW_HILL  # Shaded left slopes
            if y >= 151:  # The grassland
                c = RW_STEPPE_SH if (y - 151) % 6 == 4 and noise(x // 10 % 96, y) % 3 == 0 else RW_STEPPE  # Faint streaks
            put(img, x, y, c)  # Paint it
        lake = rw_lake(x)  # The lake here
        if lake:  # Water in this column
            lt, lb = lake  # Its top and bottom rows
            put(img, x, lt - 1, RW_SHORE)  # The far shore
            for y in range(lt, lb + 1):  # The water
                c = RW_LAKE_LT if y == lt or y == lb else RW_LAKE_DEEP if y > lt + 2 and y < lb - 1 else RW_LAKE  # Pale edges, deeper middle
                k = (x + y * 11) // 6 % 160  # Which short stretch, for whitecaps
                if lt < y < lb and noise(k, y) % 9 == 0:  # A whitecap whipped up by the wind
                    c = RW_WAVE  # Wave pixel
                put(img, x, y, c)  # Paint it
            put(img, x, lb + 1, RW_SHORE)  # The near shore
    return img  # Give the picture back


def yak(img, x, base):
    """A small far yak grazing: a dark shaggy body, its head down on the left, short legs and pale horns."""
    for dx in range(-3, 4):  # The body
        column(img, x + dx, base - 5 + (1 if abs(dx) == 3 else 0), RW_YAK, base - 1)  # Body pixel
    put(img, x - 1, base - 6, RW_YAK)  # The hump
    put(img, x, base - 6, RW_YAK)  # The hump
    for dx in (-2, 2):  # Short legs
        put(img, x + dx, base - 1, RW_YAK)  # Leg pixel
        put(img, x + dx, base, RW_YAK)  # Leg pixel
    for dx in range(-3, 4, 2):  # A shaggy fringe under the body
        put(img, x + dx, base - 1, RW_YAK)  # Fringe pixel
    put(img, x - 4, base - 3, RW_YAK)  # The neck
    put(img, x - 5, base - 2, RW_YAK)  # The head, down to graze
    put(img, x - 5, base - 1, RW_YAK)  # The muzzle
    put(img, x - 4, base - 4, RW_HORN)  # A pale horn


def rw_band():
    """L4: the near high grassland with tussocks bent by the wind and a few grazing yaks."""
    img = mb.new_layer()  # Empty layer
    tops = tops_ground(198, 2, 2, 345)  # The grass line
    nb.fill_below(img, tops, RW_GRASS_LIT, RW_GRASS)  # Grass
    for i, x in enumerate(range(0, W, 8)):  # Tussocks
        cx = (x + noise(i, 346) % 4) % W  # A little varied place
        blown_tuft(img, cx, tops[cx] + 2, 3 + noise(i, 347) % 3, RW_TUFT)  # One tussock
    for x in (150, 162, 520, 790):  # A few grazing yaks
        yak(img, x, tops[x] + 3)  # One yak
    return img  # Give the picture back


def rw_near():
    """L5: the near tussock bank with long grass bent by the wind, grey stones and tiny white flowers."""
    img = mb.new_layer()  # Empty layer
    tops = tops_ground(219, 2, 3, 348)  # A gently wavy line near the trail
    nb.fill_below(img, tops, RW_NEAR_LIT, RW_NEAR)  # Tussock bank
    for i, x in enumerate(range(4, W, 10)):  # Long grass
        cx = (x + noise(i, 349) % 5) % W  # A little varied place
        blown_tuft(img, cx, tops[cx] + 1, 6 + noise(i, 350) % 4, RW_TUFT, RW_GRASS_LIT)  # One tuft
    for i, x in enumerate(range(50, W, 130)):  # Stones
        cx = (x + noise(i, 351) % 40) % W  # A little varied place
        boulder(img, cx, tops[cx] + 2, 4 + noise(i, 352) % 3, 3, RW_STONE, RW_STONE_LT)  # One stone
    for x in range(W):  # Tiny flowers
        if noise(x % 480, 353) % 23 == 0:  # A few
            put(img, x, tops[x] - 1, RW_FLOWER)  # Flower pixel
    return img  # Give the picture back


THEMES = {  # One entry per Plateaus Hard theme (D-074): its six layers, ground tiles for the preview (its own, else a near match) and a label
    "DeccanTraps": ((dt_sky, dt_clouds, dt_far, dt_mid, dt_band, dt_near), ("deccan", "savanna"), "DECCAN"),  # Deccan Plateau
    "SimienHighlands": ((sh_sky, sh_clouds, sh_far, sh_mid, sh_band, sh_near), ("simien", "pasture"), "SIMIEN"),  # Ethiopian Highlands
    "SandstoneArches": ((sa_sky, sa_clouds, sa_far, sa_mid, sa_band, sa_near), ("tassili", "dune"), "TASSILI"),  # Tassili n'Ajjer
    "CliffDwellings": ((cd_sky, cd_clouds, cd_far, cd_mid, cd_band, cd_near), ("mesaverde", "mesa"), "MESA VERDE"),  # Mesa Verde
    "IcePlateau": ((ip_sky, ip_clouds, ip_far, ip_mid, ip_band, ip_near), ("antarctic", "glacier", "snow"), "ANTARCTIC"),  # Antarctic Plateau
    "LoessTerraces": ((lt_sky, lt_clouds, lt_far, lt_mid, lt_band, lt_near), ("loess", "terrace"), "LOESS"),  # Loess Plateau
    "RoofOfWorld": ((rw_sky, rw_clouds, rw_far, rw_mid, rw_band, rw_near), ("tibet", "steppe"), "TIBET"),  # Tibetan Plateau
}  # All seven themes


def ground_name(grounds):
    """The ground tile name for the preview: the theme's own tile, else a near match made earlier, else plain grass."""
    for ground in grounds:  # Own tile first, then the near matches
        if all(os.path.exists(os.path.join(nb.SPRITES, ground + part)) for part in ("_top.png", "_soil.png")):  # Both tiles made already
            return ground  # Use it
    return "grass"  # Fall back to grass


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


def main():
    """Draws and saves every set and the preview row."""
    made = read_made()  # Folders an earlier run of this script wrote
    for theme in THEMES:  # Check every target before writing anything
        out = os.path.join(nb.BACKDROPS, theme)  # Its folder
        if os.path.isdir(out) and theme not in made:  # Not made by this script: do not touch it
            raise SystemExit("Stopped: %s already exists and was not made by this script." % out)  # Report and stop
    shots = []  # One (label, preview screen) pair per theme
    for theme, (makers, grounds, label) in THEMES.items():  # Each theme
        out = os.path.join(nb.BACKDROPS, theme)  # Its folder
        os.makedirs(out, exist_ok=True)  # Make it if missing
        write_made(made | set(THEMES))  # Remember the folders this script makes, before the first file
        layers = [make() for make in makers]  # Draw L0 to L5
        for i, img in enumerate(layers):  # Save each one
            img.save(os.path.join(out, "L%d.png" % i))  # L0.png ... L5.png
        shots.append((label, nb.preview_screen(layers, ground_name(grounds))))  # Preview screen with its label
        print("Backdrop written to", out)  # Tell where it went
    sheet = Image.new("RGBA", (len(shots) * 488 - 8, LABEL_H + 270), mm.OUTLINE + (255,))  # One row with dark gaps
    for i, (label, shot) in enumerate(shots):  # Place each screen and its label
        x = i * 488  # Column
        text = mm.text_image(label, mm.WHITE, scale=2)  # Label in the pixel font
        sheet.paste(text, (x + (480 - text.width) // 2, 1), text)  # Centred label above the screen
        sheet.paste(shot, (x, LABEL_H))  # The composed scene
    os.makedirs(os.path.dirname(PREVIEW), exist_ok=True)  # Make the folder
    sheet.save(PREVIEW)  # Normal size, to keep the file small
    print("Preview written to", PREVIEW)  # Tell where it went


if __name__ == "__main__":
    main()  # Run when started directly
