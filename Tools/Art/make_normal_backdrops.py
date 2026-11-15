"""Draws the moving background layers for the Mountains level themes that have their own background (D-063).

Run from the Unity project folder:  python Tools/Art/make_normal_backdrops.py
It writes L0.png to L5.png (960x270 each) to Assets/Art/Level/Sprites/Backdrop/<Theme>/ for
PineForest (Mount Pulag), AshGorge (Mount Pinatubo), MossyForest (Mount Matutum and Mount Apo)
SnowPeak (Mount Everest), Volcanic (Easy 2, Volcano Trail) and FoldedRock (Easy 3, Folded Peaks),
plus a preview at .scratch/mountains-normal/backdrop-preview.png. Meadow (Easy 1) keeps make_mountains_backdrop.py.

Each set keeps the six layers and the scrolling of the Easy Mountains backdrop (make_mountains_backdrop.py):
L0 sky, L1 clouds, L2 far range, L3 middle range, L4 forest band, L5 near slope.
Colours are palette colours or even mixes of two. The rows behind the explorer (100 to 220) stay
mid-toned and soft, so the explorer, the pickups and the ledges stand out.
Every layer tiles sideways without a seam.
"""
import math  # sin, hypot and pi for the shapes
import os  # Folders and paths
from PIL import Image  # Reads the ground tiles and saves the preview
import make_mainmenu_art as mm  # Palette and image helpers
import make_landform_map_art as lm  # Map colours and the fixed noise
import make_mountains_backdrop as mb  # The Easy backdrop helpers: pixels, ridges, pines and dither

ROOT = mb.ROOT  # The Unity project folder
BACKDROPS = os.path.join(ROOT, "Assets", "Art", "Level", "Sprites", "Backdrop")  # One folder per theme
SPRITES = os.path.join(ROOT, "Assets", "Art", "Level", "Sprites")  # The ground tiles for the preview
PREVIEW = os.path.join(ROOT, ".scratch", "mountains-normal", "backdrop-preview.png")  # Where the preview goes
W, H = mb.W, mb.H  # 960x270 layers
mix, put, column, dither, jag = mb.mix, mb.put, mb.column, mb.dither, mb.jag  # Shared drawing helpers
GROUND_ROW = 222  # Screen row of the trail's ground line, for the preview


def gradient(stops):
    """A sky that fades through the colour stops (row, colour) with a dotted pixel-art blend."""
    img = mb.new_layer()  # Empty layer
    for y in range(H):  # Each row
        for i in range(len(stops) - 1):  # Find the two stops around this row
            (y0, c0), (y1, c1) = stops[i], stops[i + 1]  # The band this row is in
            if y0 <= y <= y1 or i == len(stops) - 2:  # This band, or the last one
                t = max(0.0, min(1.0, (y - y0) / max(1, y1 - y0)))  # How far through the band
                break  # Found it
        for x in range(W):  # Each pixel
            put(img, x, y, c1 if dither(x, y, t) else c0)  # Dotted blend of the two colours
    return img


def sun(img, sx, sy, r, core, glow, halo):
    """A round sun with a bright rim and a soft dotted halo."""
    for y in range(sy - r - 8, sy + r + 9):  # Rows around the sun
        for x in range(sx - r - 8, sx + r + 9):  # Columns around the sun
            d = math.hypot(x - sx, y - sy)  # Distance from the middle
            if d <= r:  # The sun itself
                put(img, x, y, core)  # Sun pixel
            elif d <= r + 2:  # Its rim
                put(img, x, y, glow)  # Rim pixel
            elif d <= r + 8 and dither(x, y, (r + 8 - d) / 10):  # Fading halo
                put(img, x, y, halo)  # Halo pixel


def flat_cloud(img, cx, cy, w, top, under):
    """A long flat cloud: lumpy top colour, a two-row shaded underside."""
    for x in range(cx - w // 2, cx + w // 2):  # Each column
        t = abs(x - cx) / (w / 2)  # 0 in the middle, 1 at the ends
        hump = int(5 * (1 - t * t)) + (lm.noise(x // 4 % 240, cy) % 3)  # Taller in the middle
        for y in range(cy - hump, cy + 3):  # Top to bottom
            put(img, x, y, under if y >= cy + 1 else top)  # Shaded underside


def ranged(peaks, slope, seed, rough, bump, rock, rock_sh, snow, snow_sh, mist, mist_from):
    """A mountain range from the Easy helper, with this theme's mist colour."""
    old = mb.MIST  # The Easy mist colour
    mb.MIST = mist  # The helper reads the mist colour from its module
    img = mb.range_layer(peaks, slope, seed, rough, bump, rock, rock_sh, snow, snow_sh, mist_from=mist_from)  # Draw the range
    mb.MIST = old  # Put the Easy colour back
    return img


def rolling(base, amp, waves, seed):
    """A gently rolling line: one row per x."""
    return [base + int(amp * math.sin(2 * math.pi * waves * x / W + seed) + 0.4 * amp * math.sin(2 * math.pi * (waves * 2 + 1) * x / W)) + jag(x, seed, 1) for x in range(W)]  # Two looping waves and a little roughness


def fill_below(img, tops, lit, body):
    """Fills each column below its top row, with a one-pixel lit top line."""
    for x in range(W):  # Each column
        put(img, x, tops[x], lit)  # Lit top line
        column(img, x, tops[x] + 1, body)  # Body below


def mist_band(img, top, depth, color):
    """A dotted band of mist that thickens going down from row top over depth rows."""
    for y in range(top, min(H, top + depth)):  # Rows of the band
        for x in range(W):  # Each pixel
            if dither(x, y, (y - top) / depth * 0.75):  # Never fully solid, so shapes show through
                put(img, x, y, color)  # Mist pixel


def crown(img, cx, cy, r, lit, shade):
    """A round tree crown: sunny upper right, shaded lower left."""
    for y in range(cy - r, cy + r + 1):  # Rows of the crown
        for x in range(cx - r, cx + r + 1):  # Columns of the crown
            if (x - cx) ** 2 + (y - cy) ** 2 <= r * r:  # Inside the circle
                put(img, x, y, lit if (x - cx) - (y - cy) > r * 0.3 else shade)  # Light from the upper right


# ---------- Pine Forest: Mount Pulag at dawn, a sea of clouds between grassy ridges, frost on the grass ----------

PF_SKY = [(0, mix(lm.MOUNTAIN, lm.SKY, 0.35)), (70, lm.SKY), (130, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.35)), (170, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.6))]  # Blue high up, warm dawn glow low down
PF_FAR = mix(lm.HAZE_SH, lm.MOUNTAIN, 0.3)  # Far grassy ridges, blue with distance
PF_FAR_SH = mix(lm.MOUNTAIN, lm.HAZE_SH, 0.3)  # Their shaded faces
PF_MID = mix(mm.HILLS_SH, lm.MOUNTAIN_SH, 0.55)  # Nearer ridge, dark blue-green
PF_MID_SH = mix(mm.LEAF, lm.MOUNTAIN_SH, 0.6)  # Its shaded faces
PF_SEA = mm.WHITE  # The sea of clouds
PF_SEA_SH = mix(mm.SKY_LIGHT, lm.HAZE, 0.4)  # Its shaded underside and ripples
PF_PINE = mix(mm.LEAF, lm.MOUNTAIN_SH, 0.35)  # Benguet pines
PF_PINE_SH = mix(mm.LEAF, mm.OUTLINE, 0.35)  # Their shaded side
PF_GRASS = mix(mm.HILLS_SH, lm.HAZE, 0.35)  # Ridge grass
PF_GRASS_LIT = mix(mm.HILLS, lm.HAZE, 0.35)  # Sunny grass edge
PF_FROST = lm.SNOW  # Frost on the grass


def pf_sky():
    """L0: dawn sky with a low warm sun."""
    img = gradient(PF_SKY)  # Blue to warm
    sun(img, 380, 112, 9, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.6))  # Low morning sun
    return img


def pf_clouds():
    """L1: a few thin high clouds, warm underneath."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((120, 30, 120), (420, 52, 70), (640, 22, 150), (860, 60, 90)):  # Cloud places
        flat_cloud(img, cx, cy, w, mm.WHITE, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.5))  # Dawn-lit underside
    return img


def pf_far():
    """L2: long rounded grassy ridges far away, no snow."""
    peaks = [(60, 112, 0), (250, 100, 0), (430, 118, 0), (620, 104, 0), (820, 114, 0)]  # Low, broad summits
    return ranged(peaks, 0.32, 4, 1, 3.0, PF_FAR, PF_FAR_SH, PF_FAR, PF_FAR_SH, mm.SKY_LIGHT, 150)  # Gentle slopes


def pf_mid():
    """L3: a darker nearer ridge with the sea of clouds filling the valley in front of it."""
    img = ranged([(140, 128, 0), (400, 136, 0), (700, 124, 0)], 0.4, 9, 2, 3.0, PF_MID, PF_MID_SH, PF_MID, PF_MID_SH, PF_MID, None)  # The ridge
    for x in range(W):  # The cloud sea, lumpy on top
        top = 160 + int(3 * math.sin(2 * math.pi * 6 * x / W) + 2 * math.sin(2 * math.pi * 17 * x / W)) + lm.noise(x // 3 % 320, 21) % 2  # Soft billows
        for y in range(top, H):  # Down to the bottom
            put(img, x, y, PF_SEA_SH if (y == top + 3 and x % 7 < 4) or (y > top + 14 and dither(x, y, 0.35)) else PF_SEA)  # Ripples and a soft shaded base
    return img


def pf_forest():
    """L4: a band of dark Benguet pines rising out of the clouds."""
    img = mb.new_layer()  # Empty layer
    floor = rolling(190, 3, 4, 11)  # Forest floor
    for x in range(W):  # Fill below the floor
        column(img, x, floor[x], PF_PINE_SH)  # Dark under the trees
    for i, x in enumerate(range(0, W, 7)):  # A pine every 7 pixels
        mb.pine(img, x + lm.noise(i, 9) % 3, floor[x] + 2, 16 + lm.noise(i, 5) % 12, PF_PINE, PF_PINE_SH)  # Tall narrow pine
    return img


def pf_near():
    """L5: grassy ridge just behind the trail, frosted, with a few pines."""
    img = mb.new_layer()  # Empty layer
    tops = rolling(205, 4, 3, 2)  # The ridge line
    fill_below(img, tops, PF_GRASS_LIT, PF_GRASS)  # Grass
    for x in range(W):  # Frost dots on the grass
        for y in range(tops[x] + 1, H):  # Each grass pixel
            if lm.noise((x * 3 + y * 7) % 480, 17) % 23 == 0:  # A few, scattered evenly
                put(img, x, y, PF_FROST)  # Frost pixel
    for x, h in ((70, 26), (84, 18), (300, 30), (520, 22), (534, 28), (760, 24), (900, 30)):  # Near pines
        mb.pine(img, x, tops[x] + 1, h, PF_PINE, PF_PINE_SH, mb.TRUNK)  # Pine with a trunk
    return img


# ---------- Ash Gorge: Mount Pinatubo, a hazy sky, a broad crater rim, lahar canyon walls and a grey ash plain ----------

AG_SKY = [(0, mix(lm.SKY, lm.HAZE, 0.5)), (90, mix(mm.SKY_LIGHT, mm.MASK, 0.3)), (160, mix(mm.MASK, mm.VEST, 0.35))]  # Pale blue to a dusty warm haze
AG_CLOUD = mix(mm.WHITE, mm.MASK, 0.3)  # Ash-tinted clouds
AG_CLOUD_SH = mix(mm.MASK, mm.VEST, 0.3)  # Their dusty undersides
AG_FAR = mix(mm.MASK_SH, lm.HAZE, 0.5)  # The far volcano, grey-blue with haze
AG_FAR_SH = mix(mm.MASK_SH, lm.HAZE_SH, 0.2)  # Its shaded face
AG_HAZE = mix(mm.MASK, mm.VEST, 0.35)  # Dust haze at the volcano's foot
AG_WALL = [mix(mm.VEST, mm.MASK_SH, 0.35), mix(mm.VEST_SH, mm.MASK_SH, 0.3), mix(mm.MASK, mm.VEST, 0.4), mix(mm.GROUND, mm.MASK_SH, 0.45)]  # Layered lahar deposits, top to bottom
AG_WALL_SH = mix(mm.VEST_SH, lm.MOUNTAIN_SH, 0.45)  # Gully shadows in the walls
AG_SLOPE = mix(mm.MASK_SH, mm.VEST_SH, 0.35)  # Grey-brown slope under the shrubs
AG_SHRUB = mix(mm.LEAF, mm.MASK_SH, 0.45)  # Dusty shrubs and cogon grass
AG_SHRUB_SH = mix(mm.LEAF, mm.STRAP, 0.5)  # Their shade
AG_ASH = mix(mm.MASK_SH, mm.MASK, 0.35)  # The ash plain
AG_ASH_LIT = mix(mm.MASK, mm.WHITE, 0.2)  # Its sunny top line
AG_ROCK = mix(mm.STRAP, mm.MASK_SH, 0.35)  # Dark volcanic rocks


def ag_sky():
    """L0: hazy dusty sky with a pale sun."""
    img = gradient(AG_SKY)  # Pale blue to dusty
    sun(img, 360, 50, 8, mix(mb.SUN, mm.WHITE, 0.4), mix(mb.SUN_GLOW, mm.MASK, 0.3), mix(mm.SKY_LIGHT, mm.MASK, 0.4))  # Sun dimmed by haze
    return img


def ag_clouds():
    """L1: thin ash-tinted clouds."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((100, 40, 140), (380, 70, 80), (620, 30, 120), (860, 56, 100)):  # Cloud places
        flat_cloud(img, cx, cy, w, AG_CLOUD, AG_CLOUD_SH)  # Dusty cloud
    return img


def ag_far():
    """L2: the broad, flat-topped crater rim of Pinatubo, with a notch where the lake hides."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        d = abs(mb.wrap_offset(x, 300))  # Distance from the volcano's middle
        top = 96 + max(0, d - 90) * 0.38  # Flat-topped cone: a wide rim, then long slopes
        if d < 90:  # On the rim
            top += 6 * math.exp(-((d - 20) / 22) ** 2) + jag(x, 3, 2)  # A soft notch over the crater and a rough rim
        top = int(top + 3 * math.sin(2 * math.pi * 9 * x / W))  # Eroded ribs
        for y in range(top, H):  # Down the slope
            c = AG_FAR_SH if mb.wrap_offset(x, 300) < -(y - top) * 0.2 else AG_FAR  # The left face is in shade
            if (x + 3 * y) % 23 == 0 and y > top + 6:  # Gully streaks on the slopes
                c = AG_FAR_SH  # A gully
            if y >= 158 and dither(x, y, min(1.0, (y - 158) / 26)):  # Dust haze at the foot
                c = AG_HAZE  # Haze pixel
            put(img, x, y, c)  # Paint
    return img


def ag_mid():
    """L3: steep canyon walls of layered lahar deposits, cut by gullies."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        top = 136 + int(10 * math.sin(2 * math.pi * 3 * x / W) + 5 * math.sin(2 * math.pi * 11 * x / W)) + (6 if (x // 40) % 3 == 1 else 0) + jag(x, 7, 2)  # Mesa tops with steps
        for y in range(top, H):  # Down the wall
            band = AG_WALL[min(len(AG_WALL) - 1, (y - top + jag(x, 13, 2)) // 9)]  # Layers of deposit, about 9 rows each
            g = (x // 4) % 240  # Gullies come in 4-pixel-wide strips
            gully = lm.noise(g, 41) % 7 == 0 and (y - top) < 8 + lm.noise(g, 42) % 34  # Uneven gullies washed down from the top by rain
            put(img, x, y, AG_WALL_SH if gully else band)  # Paint
    mist_band(img, 178, 20, AG_HAZE)  # Dust at the foot of the walls
    return img


def ag_forest():
    """L4: a grey-brown slope with sparse dusty shrubs."""
    img = mb.new_layer()  # Empty layer
    floor = rolling(190, 3, 4, 5)  # Slope line
    fill_below(img, floor, AG_ASH_LIT, AG_SLOPE)  # The slope
    for i, x in enumerate(range(0, W, 23)):  # A shrub now and then
        if lm.noise(i, 31) % 3 == 0:  # Some gaps
            continue  # No shrub here
        crown(img, x + lm.noise(i, 4) % 8, floor[x] - 2, 2 + lm.noise(i, 6) % 3, AG_SHRUB, AG_SHRUB_SH)  # A small round shrub
    return img


def ag_near():
    """L5: the grey ash plain with dark rocks and cogon tufts."""
    img = mb.new_layer()  # Empty layer
    tops = rolling(206, 3, 2, 8)  # Plain line
    fill_below(img, tops, AG_ASH_LIT, AG_ASH)  # Ash
    for x in range(W):  # Rain ripples in the ash
        for y in range(tops[x] + 3, H, 5):  # Every 5 rows
            if (x + y * 3) % 11 < 4:  # Short ripple marks
                put(img, x, y, AG_SLOPE)  # Ripple pixel
    for x, w in ((90, 12), (330, 8), (510, 16), (740, 10), (890, 7)):  # Rocks
        for dx in range(w):  # Each column of the rock
            hump = int(4 * math.sin(math.pi * (dx + 0.5) / w)) + 1  # Rounded top
            for dy in range(hump):  # Each row
                put(img, x + dx, tops[(x + dx) % W] + 2 - dy, AG_ROCK)  # Dark rock
    for x in (40, 200, 260, 420, 600, 680, 820):  # Cogon grass tufts
        for k, (dx, h) in enumerate(((-2, 3), (-1, 5), (0, 6), (1, 4), (2, 3))):  # Blades
            column(img, x + dx, tops[x + dx] - h, AG_SHRUB if k % 2 else AG_SHRUB_SH, tops[x + dx] + 1)  # One blade
    return img


# ---------- Mossy Forest: Mount Matutum and Mount Apo, a lone cone volcano with a thin steam plume over misty rainforest ----------

MF_SKY = [(0, mix(lm.SKY, lm.HAZE, 0.3)), (100, mm.SKY_LIGHT), (170, mix(mm.SKY_LIGHT, mm.HILLS, 0.15))]  # Soft blue to a pale green-white mist
MF_MIST = mix(mm.SKY_LIGHT, mm.WHITE, 0.4)  # Rainforest mist
MF_CONE = mix(mm.LEAF, lm.HAZE, 0.6)  # The far volcano, blue-green with distance
MF_CONE_SH = mix(mm.LEAF, lm.HAZE_SH, 0.45)  # Its shaded face
MF_CONE_TOP = mix(mm.MASK_SH, lm.HAZE, 0.5)  # Bare grey summit rock
MF_STEAM = mm.WHITE  # Steam plume
MF_STEAM_SH = lm.SNOW  # Its softer edge
MF_HILL = mix(mm.LEAF, lm.HAZE, 0.42)  # Rainforest hills
MF_HILL_SH = mix(mm.LEAF, lm.HAZE_SH, 0.25)  # Their shade
MF_CANOPY = mix(mm.HILLS_SH, lm.HAZE, 0.25)  # Near canopy crowns
MF_CANOPY_SH = mix(mm.LEAF, lm.MOUNTAIN_SH, 0.25)  # Their shaded halves
MF_MOSS = mix(mm.HILLS_SH, mm.LEAF, 0.5)  # Mossy slope
MF_MOSS_LIT = mix(mm.HILLS, lm.HAZE, 0.3)  # Its lit edge
MF_FERN = mix(mm.HILLS, mm.LEAF, 0.35)  # Tree ferns
MF_FERN_SH = mix(mm.LEAF, mm.OUTLINE, 0.3)  # Their shade
MF_TRUNK = mix(mm.GROUND_SH, mm.LEAF, 0.3)  # Fern trunks


def mf_sky():
    """L0: soft misty sky with the sun behind haze."""
    img = gradient(MF_SKY)  # Blue to misty
    sun(img, 400, 44, 8, mb.SUN, mb.SUN_GLOW, mm.SKY_LIGHT)  # Sun
    return img


def mf_clouds():
    """L1: soft clouds and long mist streaks."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((80, 36, 120), (560, 28, 130), (820, 64, 90)):  # Clouds
        flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # White cloud
    for cx, cy, w in ((250, 118, 180), (700, 126, 220)):  # Mist streaks low in the sky
        for x in range(cx - w // 2, cx + w // 2):  # Each column
            put(img, x, cy, MF_MIST)  # One thin line
            if abs(x - cx) < w // 3:  # Thicker in the middle
                put(img, x, cy + 1, MF_MIST)  # Second row
    mb.bird(img, 330, 84)  # A Philippine eagle far away
    return img


def mf_far():
    """L2: one tall, even cone volcano with a thin steam plume drifting right, and a smaller cone far off."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        a = 54 + abs(mb.wrap_offset(x, 300)) * 0.62  # The main cone
        b = 112 + abs(mb.wrap_offset(x, 760)) * 0.55  # A smaller cone
        top = int(min(a, b) + jag(x, 5, 2) + 1.5 * math.sin(2 * math.pi * 14 * x / W))  # Rough skyline
        px, ptop = (300, 54) if a <= b else (760, 112)  # The summit that owns this column
        for y in range(top, H):  # Down the slope
            c = MF_CONE_SH if mb.wrap_offset(x, px) < -(y - ptop) * 0.15 else MF_CONE  # Shaded left face
            if y < ptop + 14 + jag(x, 9, 3):  # Near the summit
                c = MF_CONE_TOP  # Bare rock
            elif (x * 5 + y) % 19 == 0 and c == MF_CONE:  # Now and then on the sunny face
                c = MF_CONE_SH  # Ravine line
            if y >= 150 and dither(x, y, min(1.0, (y - 150) / 26)):  # Low down
                c = MF_MIST  # Mist at the feet
            put(img, x, y, c)  # Paint
    for i in range(36):  # The steam plume, rising and bending right
        cx = 300 + i * 0.9 + 0.02 * i * i  # Drifts right as it rises
        cy = 50 - i * 0.9  # Rises
        r = 2 + i // 9  # Spreads out
        for y in range(int(cy - r), int(cy + r) + 1):  # Puff rows
            for x in range(int(cx - r), int(cx + r) + 1):  # Puff columns
                if math.hypot(x - cx, y - cy) <= r and dither(x, y, 1.0 - i / 44):  # Thins out as it goes
                    put(img, x, y, MF_STEAM if i < 18 else MF_STEAM_SH)  # Steam pixel
    return img


def mf_mid():
    """L3: rounded rainforest hills with a bumpy canopy skyline and mist below."""
    img = mb.new_layer()  # Empty layer
    tops = rolling(146, 8, 3, 3)  # Hill line
    for x in range(W):  # Each column
        column(img, x, tops[x], MF_HILL)  # The hill body
        if (x * 5 + tops[x]) % 11 == 0:  # Scattered darker dots
            column(img, x, tops[x] + 4 + x % 7, MF_HILL_SH, tops[x] + 6 + x % 7)  # Shade between the trees
    for i, x in enumerate(range(0, W, 10)):  # Round crowns along the ridge line
        crown(img, x + lm.noise(i, 2) % 4, tops[x] + 1, 4 + lm.noise(i, 3) % 3, MF_HILL, MF_HILL_SH)  # One crown
    mist_band(img, 170, 24, MF_MIST)  # Mist rising out of the valleys
    return img


def mf_forest():
    """L4: a band of round rainforest crowns, crowded and layered."""
    img = mb.new_layer()  # Empty layer
    floor = rolling(192, 3, 4, 6)  # Forest floor
    for x in range(W):  # Fill below
        column(img, x, floor[x], MF_CANOPY_SH)  # Dark under the crowns
    for i, x in enumerate(range(0, W, 9)):  # A crown every 9 pixels
        r = 6 + lm.noise(i, 3) % 5  # Sizes 6 to 10
        crown(img, x + lm.noise(i, 7) % 4, floor[x] - r + 4 - lm.noise(i, 8) % 5, r, MF_CANOPY, MF_CANOPY_SH)  # Round crown
    mist_band(img, 186, 14, MF_MIST)  # Low mist among the trees
    return img


def tree_fern(img, x, base, h):
    """A tree fern: a thin trunk with a spray of drooping fronds on top."""
    column(img, x, base - h, MF_TRUNK, base + 1)  # Trunk
    for k in range(-3, 4):  # Seven fronds
        for s in range(7):  # Along each frond
            fx = x + k * s // 2  # Out to the side
            fy = base - h - 2 + (s * s) // 8 + abs(k) // 2  # Arches up then droops
            put(img, fx, fy, MF_FERN if k >= 0 else MF_FERN_SH)  # Sunny right fronds
            put(img, fx, fy + 1, MF_FERN_SH)  # Leaf underside


def mf_near():
    """L5: a mossy slope with tree ferns."""
    img = mb.new_layer()  # Empty layer
    tops = rolling(205, 4, 3, 4)  # Slope line
    fill_below(img, tops, MF_MOSS_LIT, MF_MOSS)  # Moss
    for x in range(W):  # Moss texture dots
        for y in range(tops[x] + 2, H, 3):  # Every 3 rows
            if (x * 7 + y * 3) % 13 == 0:  # Scattered
                put(img, x, y, MF_FERN_SH)  # Darker moss dot
    for x, h in ((60, 18), (240, 22), (400, 16), (580, 24), (760, 18), (900, 20)):  # Tree ferns
        tree_fern(img, x, tops[x] + 1, h)  # Draw it
    return img


# ---------- Snow Peak: Mount Everest, a deep high-altitude sky, a jagged Himalayan ridge, glaciers and blowing snow ----------

SP_SKY = [(0, mix(lm.MOUNTAIN_SH, lm.SKY, 0.25)), (60, mix(lm.MOUNTAIN, lm.SKY, 0.45)), (140, lm.SKY), (175, mm.SKY_LIGHT)]  # Very deep blue high up
SP_ROCK = mix(lm.MOUNTAIN, lm.HAZE, 0.2)  # Himalayan rock, softened by distance
SP_ROCK_SH = mix(lm.MOUNTAIN_SH, lm.HAZE, 0.15)  # Its shade
SP_SNOW = mm.WHITE  # Sunny snow
SP_SNOW_SH = mix(lm.HAZE, lm.HAZE_SH, 0.5)  # Blue snow in shade
SP_BAND = mix(mm.VEST, lm.HAZE, 0.4)  # The pale Yellow Band of limestone near the summit
SP_MID = mix(lm.MOUNTAIN, lm.HAZE, 0.55)  # Nearer ridge rock, kept mid-toned behind the explorer
SP_MID_SH = mix(lm.MOUNTAIN, lm.HAZE_SH, 0.3)  # Its shade
SP_ICE = lm.SNOW  # Glacier surface
SP_ICE_SH = lm.HAZE  # Crevasses and shade on the glacier
SP_DRIFT = mix(lm.SNOW, lm.HAZE, 0.3)  # Near snow drifts, a touch blue so the trail stands out
SP_DRIFT_LIT = lm.SNOW  # Their lit edge
SP_STONE = mix(lm.MOUNTAIN_SH, lm.HAZE, 0.25)  # Rocks poking through the snow


def sp_sky():
    """L0: deep blue sky with a small bright sun."""
    img = gradient(SP_SKY)  # Deep to pale blue
    sun(img, 390, 40, 6, mm.WHITE, mb.SUN, lm.SKY)  # Small, white-hot sun
    return img


def sp_clouds():
    """L1: long thin wind streaks instead of fluffy clouds."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((100, 30, 200), (420, 58, 140), (700, 20, 240), (880, 76, 120)):  # Streaks
        for x in range(cx - w // 2, cx + w // 2):  # Each column
            t = abs(x - cx) / (w / 2)  # 0 in the middle
            if dither(x, cy, 1.0 - t * 0.7):  # Fading at the ends
                put(img, x, cy, mm.WHITE)  # Thin streak
            if t < 0.5:  # Middle half
                put(img, x + 8, cy + 2, mm.SKY_LIGHT)  # A softer second line
    return img


def sp_far():
    """L2: the jagged Himalayan ridge, with Everest's dark pyramid, its pale band and a snow plume blowing right."""
    peaks = [(80, 74, 40), (190, 92, 34), (300, 40, 46), (400, 70, 38), (520, 86, 36), (640, 58, 44), (760, 90, 32), (880, 66, 40)]  # Summit place, top row, snow depth; Everest is at x 300
    img = ranged(peaks, 1.05, 5, 5, 5.0, SP_ROCK, SP_ROCK_SH, SP_SNOW, SP_SNOW_SH, mm.SKY_LIGHT, 158)  # Steep and jagged
    for x in range(268, 333):  # The Yellow Band across Everest's face
        y = 66 + abs(x - 300) // 6 + jag(x, 2, 1)  # A band a little below the summit
        for dy in range(3):  # Three rows thick
            if img.getpixel((x % W, y + dy))[3]:  # Only on the mountain
                put(img, x, y + dy, SP_BAND)  # Band pixel
    for i in range(70):  # The summit snow plume, blown right
        cx, cy = 302 + i * 1.8, 40 + i * 0.12  # Streams right, sinking a little
        r = 1 + i // 18  # Widens
        for y in range(int(cy - r), int(cy + r) + 1):  # Rows
            for x in range(int(cx - r), int(cx + r) + 1):  # Columns
                if dither(x, y, 1.0 - i / 80):  # Fades out
                    put(img, x, y, SP_SNOW if i < 35 else mm.SKY_LIGHT)  # Plume pixel
    return img


def sp_mid():
    """L3: nearer icy ridges, mostly snow with dark rock ribs."""
    peaks = [(60, 120, 30), (230, 112, 34), (420, 128, 28), (600, 116, 32), (780, 124, 30)]  # Lower, very snowy
    return ranged(peaks, 0.75, 8, 3, 3.0, SP_MID, SP_MID_SH, SP_ICE, SP_SNOW_SH, mm.SKY_LIGHT, 176)  # Icy


def sp_forest():
    """L4: a glacier field with blue crevasses; no trees this high."""
    img = mb.new_layer()  # Empty layer
    floor = rolling(188, 3, 3, 9)  # Glacier edge
    fill_below(img, floor, SP_ICE, SP_ICE)  # Ice
    for x in range(W):  # Crevasses
        for y in range(floor[x] + 3, H):  # Below the edge
            if (x + 2 * (y - floor[x])) % 41 < 2 and (y - floor[x]) < 20:  # Slanting lines near the top
                put(img, x, y, SP_ICE_SH)  # Blue crack
    for x in range(W):  # Moraine stripe of dark stones
        if lm.noise(x % 480, 12) % 4 == 0:  # Broken line
            put(img, x, floor[x] + 9 + jag(x, 4, 2), SP_STONE)  # Stone
    return img


def sp_near():
    """L5: wind-sculpted snow drifts with rocks poking through and blowing snow."""
    img = mb.new_layer()  # Empty layer
    tops = rolling(206, 4, 4, 7)  # Drift line
    fill_below(img, tops, SP_DRIFT_LIT, SP_DRIFT)  # Snow
    for x in range(W):  # Wind ripples
        for y in range(tops[x] + 3, H, 4):  # Every 4 rows
            if (x - y * 2) % 13 < 3:  # Short marks
                put(img, x, y, SP_ICE_SH)  # Blue ripple
    for x, w in ((120, 10), (470, 14), (700, 8), (860, 12)):  # Rocks
        for dx in range(w):  # Each column
            hump = int(5 * math.sin(math.pi * (dx + 0.5) / w)) + 1  # Pointed top
            for dy in range(hump):  # Rows
                put(img, x + dx, tops[(x + dx) % W] + 2 - dy, SP_SNOW if dy == hump - 1 else SP_STONE)  # Snow-capped rock
    for x in range(0, W, 5):  # Blowing snow above the drifts
        y = tops[x] - 4 - lm.noise(x % 480, 15) % 14  # Scattered heights
        if lm.noise(x % 480, 16) % 3 == 0:  # Some columns only
            put(img, x, y, mm.WHITE)  # Flake
            put(img, x + 1, y, lm.SNOW)  # Short streak
    return img


# ---------- Volcanic (Easy 2, Volcano Trail): a Mayon-style perfect cone with a smoke plume, dark lava ridges and green lowlands ----------

VO_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.2)), (90, lm.SKY), (165, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.25))]  # Clear blue, a little warm at the horizon
VO_CONE = mix(lm.MOUNTAIN, mm.MASK_SH, 0.45)  # The far volcano, blue-grey
VO_CONE_SH = mix(lm.MOUNTAIN_SH, mm.MASK_SH, 0.4)  # Its shaded face
VO_GULLY = mix(lm.MOUNTAIN_SH, mm.STRAP, 0.3)  # Dark gullies down the cone
VO_SMOKE = mix(mm.MASK, mm.WHITE, 0.4)  # Smoke from the crater
VO_SMOKE_SH = mm.MASK  # Its shaded puffs
VO_LAVA = mix(mm.STRAP, lm.HAZE_SH, 0.45)  # Old dark lava ridges, softened by haze
VO_LAVA_SH = mix(mm.STRAP, lm.MOUNTAIN_SH, 0.35)  # Their shade
VO_GLOW = mix(mm.SCARF, mm.SCARF_SH, 0.4)  # A faint red glow in the old lava cracks
VO_GREEN = mix(mm.HILLS_SH, lm.HAZE, 0.3)  # Green lowland around the volcano
VO_GREEN_LIT = mix(mm.HILLS, lm.HAZE, 0.3)  # Its sunny edge
VO_PALM = mix(mm.LEAF, lm.HAZE, 0.15)  # Coconut palms and trees
VO_PALM_SH = mix(mm.LEAF, mm.OUTLINE, 0.3)  # Their shade
VO_HAZE = mix(mm.SKY_LIGHT, mm.MASK, 0.35)  # Ash haze at the foot of the cone


def vo_sky():
    """L0: clear blue sky, warm at the horizon."""
    img = gradient(VO_SKY)  # Blue to warm
    sun(img, 400, 48, 8, mb.SUN, mb.SUN_GLOW, mm.SKY_LIGHT)  # Sun
    return img


def vo_clouds():
    """L1: a few white clouds."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((90, 30, 120), (560, 24, 140), (830, 60, 90)):  # Cloud places
        flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # White cloud
    return img


def vo_far():
    """L2: one tall, perfect cone with a smoke plume, like Mayon, and a low far ridge."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Each column
        d = mb.wrap_offset(x, 300)  # Signed distance from the summit
        cone = 46 + (abs(d) ** 1.25) * 0.24  # Concave sides: steep at the top, spreading at the foot
        ridge = 150 + 6 * math.sin(2 * math.pi * 4 * x / W) + jag(x, 3, 2)  # Low ridge far off
        top = int(min(cone, ridge))  # The skyline
        for y in range(top, H):  # Down the slope
            c = VO_CONE_SH if d < -(y - 46) * 0.12 else VO_CONE  # Shaded left face
            g = (x // 3) % 320  # Gullies come in 3-pixel-wide strips
            if cone < ridge and y > 54 and lm.noise(g, 61) % 5 == 0 and y < 60 + lm.noise(g, 62) % 70:  # Uneven gullies running down from the summit
                c = VO_GULLY  # Gully pixel
            if y >= 156 and dither(x, y, min(1.0, (y - 156) / 24)):  # Low down
                c = VO_HAZE  # Ash haze
            put(img, x, y, c)  # Paint
    for i in range(44):  # The smoke plume, leaning right with the wind
        cx = 300 + i * 1.2 + 0.03 * i * i  # Drifts right
        cy = 42 - i * 0.8  # Rises
        r = 2 + i // 7  # Billows wider
        for y in range(int(cy - r), int(cy + r) + 1):  # Puff rows
            for x in range(int(cx - r), int(cx + r) + 1):  # Puff columns
                if math.hypot(x - cx, y - cy) <= r and dither(x, y, 1.0 - i / 52):  # Thins out
                    put(img, x, y, VO_SMOKE_SH if y > cy + r // 2 else VO_SMOKE)  # Shaded underside
    return img


def vo_mid():
    """L3: dark old lava ridges with faint glowing cracks."""
    img = ranged([(100, 146, 0), (330, 138, 0), (560, 150, 0), (800, 142, 0)], 0.5, 6, 3, 3.0, VO_LAVA, VO_LAVA_SH, VO_LAVA, VO_LAVA_SH, VO_HAZE, 182)  # Low rough ridges
    for x in range(W):  # Glowing cracks
        if lm.noise(x % 480, 44) % 37 == 0:  # Rare
            y = 168 + lm.noise(x % 480, 45) % 10  # Low on the ridge
            for k in range(3):  # A short crack
                if img.getpixel(((x + k) % W, y + k))[3]:  # Only on rock
                    put(img, x + k, y + k, VO_GLOW)  # Glow pixel
    return img


def palm(img, x, base, h):
    """A coconut palm: a thin leaning trunk and a spray of fronds."""
    for i in range(h):  # Trunk rows from the ground up
        put(img, x + i // 6, base - i, mb.TRUNK)  # Leans a little right
    tx, ty = x + h // 6, base - h  # Top of the trunk
    for k in (-3, -2, -1, 1, 2, 3):  # Six fronds
        for s in range(6):  # Along each frond
            put(img, tx + k * s // 2, ty + (s * s) // 9 + (1 if abs(k) == 1 else 0), VO_PALM if k > 0 else VO_PALM_SH)  # Droops at the ends


def vo_forest():
    """L4: green lowland with round trees in front of the lava."""
    img = mb.new_layer()  # Empty layer
    floor = rolling(190, 3, 4, 12)  # Ground line
    fill_below(img, floor, VO_GREEN_LIT, VO_GREEN)  # Green land
    for i, x in enumerate(range(0, W, 12)):  # Round trees
        if lm.noise(i, 46) % 3 == 0:  # Some gaps
            continue  # No tree here
        crown(img, x + lm.noise(i, 47) % 5, floor[x] - 3, 4 + lm.noise(i, 48) % 3, VO_PALM, VO_PALM_SH)  # A tree
    return img


def vo_near():
    """L5: grass with dark lava boulders and coconut palms."""
    img = mb.new_layer()  # Empty layer
    tops = rolling(206, 3, 3, 13)  # Ground line
    fill_below(img, tops, VO_GREEN_LIT, VO_GREEN)  # Grass
    for x, w in ((60, 14), (250, 10), (440, 18), (650, 12), (850, 16)):  # Lava boulders
        for dx in range(w):  # Each column
            hump = int(5 * math.sin(math.pi * (dx + 0.5) / w)) + 1  # Rounded top
            for dy in range(hump):  # Rows
                put(img, x + dx, tops[(x + dx) % W] + 2 - dy, VO_LAVA if dx > w // 3 else VO_LAVA_SH)  # Shaded left side
    for x, h in ((150, 26), (340, 30), (560, 24), (760, 28), (930, 22)):  # Coconut palms
        palm(img, x, tops[x] + 1, h)  # Draw it
    return img


# ---------- Folded Rock (Easy 3, Folded Peaks): snowy peaks behind ridges whose rock layers bend in folds ----------

FR_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.3)), (70, lm.SKY), (150, mm.SKY_LIGHT)]  # Cold clear blue
FR_FAR = mix(lm.HAZE, mm.SKY_LIGHT, 0.2)  # Far snowy range rock
FR_FAR_SH = lm.HAZE  # Its shade
FR_BANDS = [mix(mm.MASK_SH, lm.HAZE, 0.6), mix(mm.VEST, lm.HAZE, 0.6), mix(lm.MOUNTAIN, lm.HAZE, 0.6), mix(mm.VEST_SH, lm.HAZE, 0.6)]  # Rock layers in the folds: grey, tan, blue-grey, rust
FR_EDGE = mix(lm.MOUNTAIN, lm.HAZE, 0.35)  # Thin darker line between layers
FR_SNOW = lm.SNOW  # Snow on the ridge tops
FR_MEADOW = mix(mm.HILLS_SH, lm.HAZE, 0.35)  # Alpine grass
FR_MEADOW_LIT = mix(mm.HILLS, lm.HAZE, 0.35)  # Its sunny edge
FR_SCREE = mix(mm.MASK_SH, lm.HAZE, 0.3)  # Loose rock at the foot


def fr_sky():
    """L0: cold clear sky."""
    img = gradient(FR_SKY)  # Deep to pale blue
    sun(img, 390, 46, 8, mb.SUN, mb.SUN_GLOW, mm.SKY_LIGHT)  # Sun
    return img


def fr_clouds():
    """L1: white clouds and a far eagle."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((100, 34, 110), (420, 64, 70), (620, 26, 140), (860, 70, 80)):  # Cloud places
        flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # White cloud
    mb.bird(img, 260, 92)  # An eagle
    return img


def fr_far():
    """L2: a long snowy range far away."""
    peaks = [(50, 80, 32), (180, 96, 24), (310, 70, 36), (450, 92, 26), (590, 76, 32), (720, 98, 22), (850, 74, 34)]  # Summits with deep snow
    return ranged(peaks, 0.9, 4, 3, 4.0, FR_FAR, FR_FAR_SH, mm.WHITE, mix(mm.SKY_LIGHT, lm.HAZE, 0.5), mm.SKY_LIGHT, 152)  # Steep, light, misty below


def fr_mid():
    """L3: nearer ridges cut open so their rock layers show, bent up and down in big folds."""
    img = mb.new_layer()  # Empty layer
    tops = rolling(128, 10, 3, 5)  # Ridge line
    for x in range(W):  # Each column
        fold = 14 * math.sin(2 * math.pi * 4 * x / W)  # The folds: layers rise and sink along the ridge
        for y in range(tops[x], H):  # Down the face
            depth = y - fold  # Height inside the bent layers
            k = int(depth // 10)  # Which layer, 10 rows thick
            c = FR_EDGE if int(depth) % 10 == 0 else FR_BANDS[k % len(FR_BANDS)]  # A dark line between layers
            if y < tops[x] + 3:  # The ridge top
                c = FR_SNOW  # A thin snow line
            if y >= 178 and dither(x, y, min(1.0, (y - 178) / 22)):  # Low down
                c = mm.SKY_LIGHT  # Mist
            put(img, x, y, c)  # Paint
    return img


def fr_forest():
    """L4: grey scree slopes with a few dark pines."""
    img = mb.new_layer()  # Empty layer
    floor = rolling(190, 3, 4, 14)  # Slope line
    fill_below(img, floor, FR_MEADOW_LIT, FR_SCREE)  # Scree
    for i, x in enumerate(range(0, W, 14)):  # Pines in small groups
        if lm.noise(i, 51) % 3 == 0:  # Some gaps
            continue  # No tree here
        mb.pine(img, x + lm.noise(i, 52) % 5, floor[x] + 2, 12 + lm.noise(i, 53) % 8, mb.FOREST, mb.FOREST_SH)  # Small pine
    return img


def fr_near():
    """L5: an alpine meadow with tilted, layered rocks poking out."""
    img = mb.new_layer()  # Empty layer
    tops = rolling(205, 4, 3, 15)  # Meadow line
    fill_below(img, tops, FR_MEADOW_LIT, FR_MEADOW)  # Grass
    for x, w in ((80, 16), (300, 12), (520, 18), (740, 14), (900, 10)):  # Tilted rocks with layers
        for dx in range(w):  # Each column
            h = 3 + dx // 2  # Taller to the right: the layers are tilted
            for dy in range(h):  # Rows
                put(img, x + dx, tops[(x + dx) % W] + 2 - dy, FR_BANDS[(dy + dx // 3) % len(FR_BANDS)])  # Slanted stripes
    for x, h in ((40, 22), (200, 26), (640, 20), (820, 24)):  # A few pines
        mb.pine(img, x, tops[x] + 1, h, mb.PINE, mb.PINE_SH, mb.TRUNK)  # Pine with a trunk
    return img


THEMES = {  # One entry per Normal theme (D-061): its six layers and its ground tiles for the preview
    "PineForest": ((pf_sky, pf_clouds, pf_far, pf_mid, pf_forest, pf_near), "pine"),
    "AshGorge": ((ag_sky, ag_clouds, ag_far, ag_mid, ag_forest, ag_near), "ash"),
    "MossyForest": ((mf_sky, mf_clouds, mf_far, mf_mid, mf_forest, mf_near), "moss"),
    "SnowPeak": ((sp_sky, sp_clouds, sp_far, sp_mid, sp_forest, sp_near), "snow"),
    "Volcanic": ((vo_sky, vo_clouds, vo_far, vo_mid, vo_forest, vo_near), "volcanic"),
    "FoldedRock": ((fr_sky, fr_clouds, fr_far, fr_mid, fr_forest, fr_near), "fold"),
}


def preview_screen(layers, ground):
    """One 480x270 screen: the six layers at the level start, with the theme's ground tiles at the trail line."""
    screen = Image.new("RGBA", (480, 270))  # Empty screen
    for img in layers:  # Far to near
        screen.alpha_composite(img.crop((0, 0, 480, 270)))  # First screen of each layer
    top = Image.open(os.path.join(SPRITES, ground + "_top.png")).convert("RGBA")  # Grass, ash or snow top
    soil = Image.open(os.path.join(SPRITES, ground + "_soil.png")).convert("RGBA")  # Soil under it
    for x in range(0, 480, top.width):  # Along the trail
        screen.alpha_composite(top, (x, GROUND_ROW))  # Top row of tiles
        for y in range(GROUND_ROW + top.height, 270, soil.height):  # Soil below
            screen.alpha_composite(soil, (x, y))  # Soil tile
    return screen


def main():
    """Draws and saves every set and the preview grid."""
    shots = []  # One preview screen per theme
    for theme, (makers, ground) in THEMES.items():  # Each theme
        out = os.path.join(BACKDROPS, theme)  # Its folder
        os.makedirs(out, exist_ok=True)  # Make it if missing
        layers = [make() for make in makers]  # Draw L0 to L5
        for i, img in enumerate(layers):  # Save each
            img.save(os.path.join(out, "L%d.png" % i))  # L0.png ... L5.png
        shots.append(preview_screen(layers, ground))  # Preview screen
        print("Backdrop written to", out)  # Tell where
    rows = (len(shots) + 1) // 2  # Two screens per row
    sheet = Image.new("RGBA", (968, rows * 278 - 8), mm.OUTLINE + (255,))  # Grid with dark gaps
    for i, shot in enumerate(shots):  # Place each screen
        sheet.paste(shot, ((i % 2) * 488, (i // 2) * 278))  # 8-pixel gaps
    os.makedirs(os.path.dirname(PREVIEW), exist_ok=True)  # Make the folder
    sheet.resize((sheet.width * 2, sheet.height * 2), Image.NEAREST).save(PREVIEW)  # Twice size, crisp pixels
    print("Preview written to", PREVIEW)  # Tell where


if __name__ == "__main__":
    main()  # Run when started directly
