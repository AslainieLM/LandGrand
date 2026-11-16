"""Draws the moving background layers for the three Hills Easy themes (D-063, D-066).

Run from the Unity project folder:  python Tools/Art/make_hills_easy_backdrops.py
It writes L0.png to L5.png (960x270 each) to Assets/Art/Level/Sprites/Backdrop/<Theme>/ for
ChocolateHills (Bohol), RollingPasture (Batanes) and SandDunes (La Paz, Ilocos Norte),
plus a preview at .scratch/hills/backdrop-preview.png.

Each set keeps the six layers and the scrolling of the Mountains backdrops:
L0 sky, L1 clouds, L2 far land, L3 middle hills, L4 near band, L5 near slope.
The rows behind the explorer (100 to 220) stay mid-toned and soft, so the explorer,
the pickups and the ledges stand out. Every layer tiles sideways without a seam.
"""
import math  # sin and pi for the shapes
import os  # Folders and paths
from PIL import Image  # Saves the preview
import make_mainmenu_art as mm  # Palette and image helpers
import make_landform_map_art as lm  # Map colours and the fixed noise
import make_mountains_backdrop as mb  # Pixels, dither and the wrap helper
import make_normal_backdrops as nb  # Gradient, sun, clouds, rolling lines, crowns and the preview

W, H = mb.W, mb.H  # 960x270 layers
mix, put, column, dither = mb.mix, mb.put, mb.column, mb.dither  # Shared drawing helpers
PREVIEW = os.path.join(mb.ROOT, ".scratch", "hills", "backdrop-preview.png")  # Where the preview goes


def dome(img, cx, base, r, h, lit, shade, haze=None, haze_t=0.0):
    """A rounded hill like a Chocolate Hill: r is half its width, h its height. Light comes from the right."""
    for dx in range(-r, r + 1):  # Each column of the hill
        t = dx / r  # -1 at the left foot, 1 at the right foot
        top = base - int(h * (1 - t * t) ** 0.8)  # Rounded top
        for y in range(top, base + 1):  # Down to the foot
            c = lit if dx > -r // 4 else shade  # Shaded left side
            if haze and dither(cx + dx, y, haze_t):  # Far hills fade into the haze
                c = haze  # Haze pixel
            put(img, cx + dx, y, c)  # Paint


def palm(img, x, base, h, leaf, leaf_sh, trunk):
    """A small far coconut palm: a thin trunk and a drooping crown."""
    for y in range(base - h, base + 1):  # Trunk rows
        put(img, x + (base - y) // 8, y, trunk)  # Slight lean
    tx = x + h // 8  # Top of the trunk
    for side in (-1, 1):  # Both sides
        for t in range(6):  # Along a frond
            put(img, tx + side * t, base - h + (t * t) // 6, leaf if side > 0 else leaf_sh)  # Frond pixel


# ---------- Chocolate Hills: Bohol. Rows of brown-green domes over green rice fields, palms, a warm blue sky ----------

CH_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.2)), (90, lm.SKY), (175, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.2))]  # Warm tropical blue
CH_FAR = mix(mm.HILLS_SH, lm.HAZE, 0.6)  # Far hills, blue-green with distance
CH_FAR_SH = mix(mm.HILLS_SH, lm.HAZE_SH, 0.5)  # Their shade
CH_HAZE = mix(mm.SKY_LIGHT, lm.HAZE, 0.5)  # Haze between the rows
CH_DOME = mix(mix(mm.GROUND, mm.VEST, 0.5), lm.HAZE, 0.35)  # Middle domes, chocolate brown softened by haze
CH_DOME_SH = mix(mix(mm.GROUND, mm.GROUND_SH, 0.5), lm.HAZE, 0.3)  # Their shaded side
CH_FIELD = mix(mm.HILLS, lm.HAZE, 0.35)  # Rice fields between the domes
CH_FIELD_SH = mix(mm.HILLS_SH, lm.HAZE, 0.3)  # Field lines
CH_NEAR = mix(mix(mm.GROUND, mm.HILLS_SH, 0.5), lm.HAZE, 0.2)  # Near domes, brown-green
CH_NEAR_SH = mix(mix(mm.GROUND_SH, mm.HILLS_SH, 0.5), lm.HAZE, 0.15)  # Their shade
CH_PALM = mix(mm.LEAF, lm.HAZE, 0.3)  # Far palms
CH_PALM_SH = mix(mm.LEAF, lm.HAZE_SH, 0.2)  # Their shade
CH_TRUNK = mix(lm.HANDLE, lm.HAZE, 0.3)  # Palm trunks


def ch_sky():
    """L0: warm blue sky with a high sun."""
    img = nb.gradient(CH_SKY)  # Blue to warm
    nb.sun(img, 760, 48, 8, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.4))  # Tropical sun
    return img


def ch_clouds():
    """L1: puffy white clouds."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((120, 46, 120), (420, 30, 90), (650, 58, 150), (880, 38, 70)):  # Cloud places
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # White cloud
    return img


def ch_far():
    """L2: a far line of low hills in the blue haze."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(132, 6, 4, 11)  # Far hill line
    nb.fill_below(img, tops, CH_FAR, CH_FAR_SH)  # Fill it
    nb.mist_band(img, 150, 30, CH_HAZE)  # Haze at the foot
    return img


def ch_mid():
    """L3: the famous rows of chocolate-brown domes standing on flat green fields."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # Flat fields behind the domes
        column(img, x, 168, CH_FIELD)  # Field colour
    for i, x in enumerate(range(0, W, 40)):  # Back row of small domes
        dome(img, x + lm.noise(i, 21) % 14, 168, 14 + lm.noise(i, 22) % 4, 14 + lm.noise(i, 23) % 5,
             CH_DOME, CH_DOME_SH, CH_HAZE, 0.35)  # One small hazy dome
    for i, x in enumerate(range(20, W, 58)):  # Front row of bigger domes
        dome(img, x + lm.noise(i, 24) % 16, 178, 19 + lm.noise(i, 25) % 5, 20 + lm.noise(i, 26) % 6, CH_DOME, CH_DOME_SH)  # One dome
    return img


def ch_forest():
    """L4: green rice fields with lines, and coconut palms."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(190, 2, 3, 27)  # Field line
    nb.fill_below(img, tops, CH_FIELD, CH_FIELD)  # Fields
    for x in range(W):  # Field lines
        for y in range(tops[x] + 4, H, 6):  # Every sixth row
            put(img, x, y, CH_FIELD_SH)  # Dike line
    for i, x in enumerate(range(0, W, 70)):  # Palms
        palm(img, x + lm.noise(i, 28) % 30, tops[(x + lm.noise(i, 28) % 30) % W], 18 + lm.noise(i, 29) % 6, CH_PALM, CH_PALM_SH, CH_TRUNK)  # One palm
    return img


def ch_near():
    """L5: the feet of two big near domes at the trail."""
    img = mb.new_layer()  # Empty layer
    for cx in (180, 660):  # Two near domes
        dome(img, cx, 226, 70, 34, CH_NEAR, CH_NEAR_SH)  # Big dome, only its top shows above the trail
    return img


# ---------- Rolling Pasture: Batanes. Bright green rolling hills to a sea cliff, a far volcano, a lighthouse ----------

RP_SKY = [(0, mix(lm.SKY, lm.MOUNTAIN, 0.25)), (100, lm.SKY), (175, mm.SKY_LIGHT)]  # Clean, windy blue
RP_SEA = mix(lm.LENS, lm.HAZE, 0.3)  # Far sea
RP_SEA_SH = mix(lm.LENS, lm.MOUNTAIN, 0.3)  # Darker sea lines
RP_VOLC = mix(lm.HAZE, mm.HILLS_SH, 0.35)  # Far Mount Iraya, blue-green with distance
RP_VOLC_SH = mix(lm.HAZE_SH, mm.LEAF, 0.3)  # Its shade
RP_FAR = mix(mm.HILLS, lm.HAZE, 0.5)  # Far pasture hills
RP_FAR_SH = mix(mm.HILLS_SH, lm.HAZE, 0.45)  # Their shade
RP_MID = mix(mm.HILLS, lm.HAZE, 0.3)  # Middle pasture hills
RP_MID_SH = mix(mm.HILLS_SH, lm.HAZE, 0.25)  # Their shade
RP_FENCE = mix(mm.MASK, lm.HAZE, 0.3)  # Pale stone fences
RP_NEAR = mix(mm.HILLS_SH, lm.HAZE, 0.15)  # Near grass
RP_NEAR_LIT = mix(mm.PLAINS, lm.HAZE, 0.2)  # Its lit edge
RP_CLIFF = mix(mm.MASK_SH, lm.HAZE, 0.3)  # Grey cliff rock
RP_TOWER = mm.WHITE  # Lighthouse
RP_LAMP = mm.SCARF  # Its red lamp room


def rp_sky():
    """L0: clean windy blue sky."""
    img = nb.gradient(RP_SKY)  # Deep to light blue
    nb.sun(img, 180, 52, 7, mb.SUN, mb.SUN_GLOW, mm.SKY_LIGHT)  # Sun
    return img


def rp_clouds():
    """L1: long clouds stretched by the sea wind."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((90, 40, 170), (380, 26, 120), (600, 54, 200), (860, 34, 110)):  # Cloud places
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mm.SKY_LIGHT)  # Long cloud
    return img


def rp_far():
    """L2: the sea with a far volcano island (Mount Iraya) rising out of it."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # The sea
        column(img, x, 140, RP_SEA)  # Sea
        for y in range(144, 200, 7):  # Rows of small waves
            if (x + y * 5) % 29 < 5:  # Short broken wave lines
                put(img, x, y, RP_SEA_SH)  # Wave pixel
    for x in range(W):  # The volcano
        d = abs(mb.wrap_offset(x, 520))  # Distance from the summit
        top = 88 + int(d * 0.42 + (d * d) / 900)  # A broad cone
        for y in range(top, 141):  # Down to the sea
            put(img, x, y, RP_VOLC_SH if mb.wrap_offset(x, 520) < 0 else RP_VOLC)  # Shaded left side
    return img


def rp_mid():
    """L3: rolling pasture hills with a white lighthouse on one top and a cliff at the sea."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(150, 12, 3, 31)  # Rolling hill line
    for x in range(W):  # Each column
        rising = sum(tops[(x + k) % W] for k in range(1, 21)) < sum(tops[(x - k) % W] for k in range(1, 21)) - 40  # The hill climbs to the right here, averaged over 20 pixels on each side so it never stripes
        put(img, x, tops[x], RP_FAR)  # Top line
        column(img, x, tops[x] + 1, RP_FAR_SH if rising else RP_FAR)  # Slopes facing away from the sun are darker
    lx = min(range(200, 420), key=lambda x: tops[x])  # The highest top in this stretch
    for y in range(tops[lx] - 14, tops[lx]):  # Lighthouse tower
        put(img, lx, y, RP_TOWER)  # Tower pixel
        put(img, lx + 1, y, RP_TOWER)  # Tower pixel
    for dx in (0, 1):  # Lamp room
        put(img, lx + dx, tops[lx] - 15, RP_LAMP)  # Lamp pixel
        put(img, lx + dx, tops[lx] - 16, RP_LAMP)  # Lamp pixel
    return img


def rp_forest():
    """L4: nearer pasture with lines of pale stone fences."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(180, 8, 2, 33)  # Hill line
    nb.fill_below(img, tops, RP_MID, RP_MID)  # Grass
    for x in range(W):  # Each column
        for y in range(tops[x] + 3, min(H, tops[x] + 9)):  # A soft dotted shadow under the hill line
            if dither(x, y, 0.35):  # Only some dots, so it stays soft
                put(img, x, y, RP_MID_SH)  # Shadow dot
    for start in (60, 430, 760):  # Three fences
        for x in range(start, start + 110):  # Along the fence
            y = tops[x % W] + 10 + (x - start) // 12  # Fence runs down the slope
            put(img, x, y, RP_FENCE)  # Fence stone
            if x % 3:  # Stones on top, with gaps
                put(img, x, y - 1, RP_FENCE)  # Upper stone
    return img


def rp_near():
    """L5: near grass with a few grey rocks."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(206, 3, 3, 35)  # Ground line
    nb.fill_below(img, tops, RP_NEAR_LIT, RP_NEAR)  # Grass
    for x, w in ((140, 10), (500, 14), (790, 8)):  # Rocks
        for dx in range(w):  # Each column of the rock
            for dy in range(int(4 * math.sin(math.pi * (dx + 0.5) / w)) + 1):  # Rounded top
                put(img, x + dx, tops[(x + dx) % W] + 2 - dy, RP_CLIFF)  # Rock pixel
    return img


# ---------- Sand Dunes: La Paz, Ilocos Norte. Big tan dunes, a hot pale sky and a thin strip of sea ----------

SD_SKY = [(0, mix(lm.SKY, lm.HAZE, 0.3)), (110, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.15)), (175, mix(mm.SKY_LIGHT, mm.HEADLAMP, 0.4))]  # Hot, pale sky
SD_SEA = mix(lm.LENS, lm.HAZE, 0.25)  # Sea strip
SD_SEA_LT = mix(lm.LENS, mm.WHITE, 0.4)  # Wave shine
SD_FAR = mix(mm.VEST, lm.HAZE, 0.45)  # Far dunes, pale with distance
SD_FAR_SH = mix(mm.VEST_SH, lm.HAZE, 0.45)  # Their shade
SD_MID = mix(mm.VEST, mm.HEADLAMP, 0.35)  # Middle dunes in sun
SD_MID_SH = mix(mm.VEST, mm.VEST_SH, 0.45)  # Their steep shaded faces
SD_RIPPLE = mix(mm.VEST_SH, mm.VEST, 0.4)  # Wind ripples
SD_SCRUB = mix(mm.HILLS_SH, mm.VEST_SH, 0.4)  # Dune scrub
SD_SCRUB_SH = mix(mm.LEAF, mm.VEST_SH, 0.4)  # Its shade
SD_NEAR = mix(mm.VEST, mm.HEADLAMP, 0.2)  # Near sand bank
SD_NEAR_LIT = mix(mm.HEADLAMP, mm.VEST, 0.3)  # Its sunny edge


def sd_sky():
    """L0: hot pale sky with a bright sun."""
    img = nb.gradient(SD_SKY)  # Pale blue to warm white
    nb.sun(img, 620, 44, 9, mb.SUN, mb.SUN_GLOW, mix(mm.SKY_LIGHT, mb.SUN_GLOW, 0.5))  # Strong sun
    return img


def sd_clouds():
    """L1: a few thin, high clouds."""
    img = mb.new_layer()  # Empty layer
    for cx, cy, w in ((200, 34, 90), (520, 22, 60), (800, 44, 110)):  # Cloud places
        nb.flat_cloud(img, cx, cy, w, mm.WHITE, mix(mm.SKY_LIGHT, mm.WHITE, 0.5))  # Thin cloud
    return img


def sd_far():
    """L2: a thin strip of blue sea behind low far dunes."""
    img = mb.new_layer()  # Empty layer
    for x in range(W):  # The sea strip
        column(img, x, 128, SD_SEA, 150)  # Sea
        if x % 23 < 4:  # Wave shine
            put(img, x, 132 + (x // 23) % 3 * 5, SD_SEA_LT)  # Shine pixel
    tops = nb.rolling(146, 5, 5, 41)  # Far dune line
    nb.fill_below(img, tops, SD_FAR, SD_FAR)  # Far dunes
    return img


def dune_line(base, amp, waves, seed):
    """A dune line: a gentle windward slope on the left and a steeper face on the right, like real dunes."""
    line = []  # One top row per x
    for x in range(W):  # Each column
        p = (x * waves / W) % 1.0  # Place inside one dune, 0 to 1
        h = p / 0.75 if p < 0.75 else (1 - p) / 0.25  # Long rise, short steep drop
        line.append(base - int(amp * h) + mb.jag(x, seed, 1))  # Row of the top
    return line


def sd_mid():
    """L3: big tan dunes with steep shaded faces."""
    img = mb.new_layer()  # Empty layer
    tops = dune_line(166, 30, 4, 43)  # Big dunes
    for x in range(W):  # Each column
        p = (x * 4 / W) % 1.0  # Place inside the dune
        face = SD_MID_SH if p >= 0.75 else SD_MID  # The steep face is in shade
        column(img, x, tops[x], face)  # Fill
    return img


def sd_forest():
    """L4: nearer dunes with wind ripples and low scrub."""
    img = mb.new_layer()  # Empty layer
    tops = dune_line(192, 14, 6, 45)  # Nearer dunes
    for x in range(W):  # Each column
        p = (x * 6 / W) % 1.0  # Place inside the dune
        column(img, x, tops[x], SD_MID_SH if p >= 0.75 else SD_MID)  # Fill
        for y in range(tops[x] + 3, H, 5):  # Ripple rows
            if (x + y) % 7 < 4:  # Broken ripple lines
                put(img, x, y, SD_RIPPLE)  # Ripple pixel
    for i, x in enumerate(range(0, W, 64)):  # Scrub bushes
        bx = x + lm.noise(i, 46) % 30  # Bush place
        nb.crown(img, bx, tops[bx % W] - 1, 3, SD_SCRUB, SD_SCRUB_SH)  # One low bush
    return img


def sd_near():
    """L5: a near pale sand bank."""
    img = mb.new_layer()  # Empty layer
    tops = nb.rolling(208, 3, 2, 47)  # Ground line
    nb.fill_below(img, tops, SD_NEAR_LIT, SD_NEAR)  # Sand
    return img


THEMES = {  # One entry per Hills Easy theme (D-061): its six layers and its ground tiles for the preview
    "ChocolateHills": ((ch_sky, ch_clouds, ch_far, ch_mid, ch_forest, ch_near), "choc"),
    "RollingPasture": ((rp_sky, rp_clouds, rp_far, rp_mid, rp_forest, rp_near), "pasture"),
    "SandDunes": ((sd_sky, sd_clouds, sd_far, sd_mid, sd_forest, sd_near), "dune"),
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
