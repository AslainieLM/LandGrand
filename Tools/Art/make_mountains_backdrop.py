"""Draws the six moving background layers of the Mountains levels (decision D-049).

Run from the Unity project folder:  python Tools/Art/make_mountains_backdrop.py
It writes L0.png to L5.png (960x270 each) to Assets/Art/Level/Sprites/Backdrop/Mountains/.

The layers replace the start-screen background in the level, which mixed every landform.
Far to near: L0 sky, L1 clouds, L2 far snowy range, L3 middle rocky range, L4 pine forest, L5 alpine meadow.
Colours come from Assets/Art/Palettes/landgrand.gpl, plus a few even mixes of two palette colours for
the haze between ranges. Far layers are light and pale; nearer layers get a little darker and greener,
but the layers behind the explorer (screen rows 100 to 220) stay mid-toned and soft, so the explorer,
the pickups and the ledges always stand out.
Every layer tiles sideways: its left and right edges join without a seam.
"""
import math  # math gives sin, hypot and pi for the gentle ridge bumps and the sun
import os  # os is used to make folders and join paths
import make_mainmenu_art as mm  # Main Menu palette and image helpers
import make_landform_map_art as lm  # Map colours: sky, mountain, snow and haze

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))  # The Unity project folder
OUT = os.path.join(ROOT, "Assets", "Art", "Level", "Sprites", "Backdrop", "Mountains")  # Where the PNGs go
W, H = 960, 270  # Layer size: two screens wide, one screen tall
SUN = (255, 242, 168)  # Sun, from the palette
SUN_GLOW = (255, 225, 122)  # Sun glow, from the palette


def mix(a, b, t):
    """An even blend of two palette colours: t = 0 gives a, t = 1 gives b."""
    return tuple(int(round(a[i] + (b[i] - a[i]) * t)) for i in range(3))  # Blend each of red, green and blue


SKY_TOP = mix(lm.SKY, lm.MOUNTAIN, 0.3)  # Deeper blue high in the sky: thin, cold mountain air
FAR = mix(lm.HAZE, mm.SKY_LIGHT, 0.25)  # Far range rock, pale with distance
FAR_SH = lm.HAZE  # Far range rock in shade
FAR_SNOW = mm.WHITE  # Sunny snow on the far peaks
FAR_SNOW_SH = mix(mm.SKY_LIGHT, lm.HAZE, 0.5)  # Snow in shade, blue-grey so it still shows against the sky
MID = mix(lm.MOUNTAIN, lm.HAZE, 0.45)  # Middle range rock, softened by haze
MID_SH = mix(lm.MOUNTAIN, lm.HAZE, 0.1)  # Middle range rock in shade
MID_SNOW = lm.SNOW  # Snow on the middle peaks
MID_SNOW_SH = lm.HAZE  # Shaded snow on the middle peaks
MIST = mm.SKY_LIGHT  # Valley mist between the ranges
FOREST = mix(mm.LEAF, lm.HAZE, 0.5)  # Far pine forest, blue-green with distance
FOREST_SH = mix(mm.LEAF, lm.HAZE_SH, 0.3)  # Shaded side of the far pines
MEADOW = mix(mm.HILLS_SH, lm.HAZE, 0.3)  # Alpine meadow on the near slope
MEADOW_LIT = mix(mm.HILLS, lm.HAZE, 0.3)  # Sunny top line of the meadow
PINE = mix(mm.LEAF, lm.HAZE, 0.2)  # Near pines, a little greener
PINE_SH = mix(mm.LEAF, lm.MOUNTAIN_SH, 0.35)  # Shaded side of the near pines
TRUNK = mix(lm.HANDLE, lm.MOUNTAIN_SH, 0.4)  # Pine trunks
ROCK = mix(lm.MOUNTAIN, lm.HAZE, 0.3)  # Rocks in the meadow
ROCK_LIT = lm.HAZE  # Sunny top of the rocks
BIRD = mix(lm.MOUNTAIN_SH, lm.HAZE, 0.3)  # Far-away eagles, soft so they do not look like pickups


def new_layer():
    """An empty, fully see-through layer."""
    return mm.new_image(W, H)  # 960x270, clear


def put(img, x, y, color):
    """Paints one pixel; x wraps around so shapes that cross an edge come back on the other side."""
    if 0 <= y < H:  # Stay inside the rows
        img.putpixel((x % W, y), color + (255,))  # Wrap across and paint it solid


def column(img, x, top, color, bottom=H):
    """Paints a column of pixels from row top down to row bottom."""
    for y in range(max(0, int(top)), bottom):  # Each row of the column
        put(img, x, y, color)  # Paint it


def dither(x, y, t):
    """True when a pixel takes the second colour of a blend, using a 4x4 ordered pattern for a smooth pixel-art fade."""
    bayer = (0, 8, 2, 10, 12, 4, 14, 6, 3, 11, 1, 9, 15, 7, 13, 5)  # Classic 4x4 order: spreads the dots evenly
    return t * 16 > bayer[(y % 4) * 4 + (x % 4)]  # More dots as t grows


def wrap_offset(x, px):
    """Signed distance across from px to x, taking the short way around the 960-pixel loop."""
    d = (x - px) % W  # Distance going right
    return d - W if d > W / 2 else d  # Going left is shorter for big distances


def jag(x, seed, size):
    """A fixed small roughness for a ridge line: steps of 0 to size pixels, the same on every run."""
    return lm.noise(x // 2 % (W // 2), seed) * (size + 1) // 100  # Changes every 2 pixels, wraps every 960 pixels


def ridge(peaks, slope, seed, rough, bump):
    """Works out a mountain line. Returns, for each x, the top row and the peak it belongs to.
    peaks: (x, top row, snow depth). slope: rows down per pixel across. rough and bump: jaggedness."""
    tops, owners = [], []  # Top row and owning peak for each x
    for x in range(W):  # Each column
        best, owner = H, None  # Highest point so far (smallest row) and its peak
        for p in peaks:  # Each peak
            y = p[1] + slope * abs(wrap_offset(x, p[0]))  # The peak's side at this column
            if y < best:  # Higher than the others
                best, owner = y, p  # This peak owns the column
        wave = math.sin(2 * math.pi * 5 * x / W + seed) + 0.6 * math.sin(2 * math.pi * 13 * x / W + 2 * seed)  # Two looping waves give shoulders and dips on the slopes
        best += jag(x, seed, rough) + bump * wave  # Rough rock on top of the waves
        tops.append(int(best))  # Whole rows only
        owners.append(owner)  # Remember which peak it is
    return tops, owners


def range_layer(peaks, slope, seed, rough, bump, rock, rock_sh, snow, snow_sh, mist_from=None):
    """Draws a mountain range: sunny right faces, shaded left faces, snow caps with a ragged lower edge,
    and an optional band of valley mist from row mist_from down."""
    img = new_layer()  # Empty layer
    tops, owners = ridge(peaks, slope, seed, rough, bump)  # The mountain line
    for x in range(W):  # Each column
        px, ptop, depth = owners[x]  # The peak this column belongs to
        lean = ((px * 13 + seed) % 7 - 3) * 0.12  # Each summit's ridge line leans a little left or right, so the faces are not mirror images
        drip = (x * 7 + seed) % 5  # A ragged snow line, 0 to 4 pixels lower in places
        snow_to = ptop + depth + drip - abs(wrap_offset(x, px)) * slope * 0.25  # Snow reaches less far down away from the summit
        for y in range(tops[x], H):  # From the ridge down
            left = wrap_offset(x, px) < (y - ptop) * lean + jag(y, seed + px, 2) - 1  # Left of the ragged ridge line: the face away from the sun, which is on the right
            if y < snow_to:  # Inside the snow cap
                c = snow_sh if left else snow  # Shaded or sunny snow
            else:  # Bare rock
                c = rock_sh if left else rock  # Shaded or sunny rock
            if left and snow_to <= y < snow_to + 6 and (x + y) % 9 == 0:  # A few snow streaks just below the cap on the shaded side
                c = snow_sh  # Streak of snow in a gully
            if mist_from is not None and y >= mist_from and dither(x, y, min(1.0, (y - mist_from) / 24)):  # Mist thickens going down
                c = MIST  # Mist pixel
            put(img, x, y, c)  # Paint the pixel
    return img


def sky():
    """L0: deep blue high up, fading to pale blue at the horizon, with a soft sun. It does not move."""
    img = new_layer()  # Empty layer
    for y in range(H):  # Each row
        for x in range(W):  # Each pixel
            if y < 60:  # Upper sky: deep blue to sky blue
                c = lm.SKY if dither(x, y, max(0.0, (y - 44) / 16)) else SKY_TOP  # A short fade between rows 44 and 60
            else:  # Lower sky: sky blue to pale blue
                c = mm.SKY_LIGHT if dither(x, y, max(0.0, min(1.0, (y - 128) / 16))) else lm.SKY  # A short fade between rows 128 and 144, hidden behind the peaks
            put(img, x, y, c)  # Paint the pixel
    sx, sy = 372, 46  # Sun place: upper right of the first screen (the sky stays still, so only this part shows)
    for y in range(sy - 16, sy + 17):  # Rows around the sun
        for x in range(sx - 16, sx + 17):  # Columns around the sun
            d = math.hypot(x - sx, y - sy)  # Distance from the sun's middle
            if d <= 8:  # The sun itself
                put(img, x, y, SUN)  # Sun pixel
            elif d <= 10:  # Its bright rim
                put(img, x, y, SUN_GLOW)  # Glow pixel
            elif d <= 16 and dither(x, y, (16 - d) / 12):  # A soft dotted glow fading out
                put(img, x, y, mm.SKY_LIGHT)  # Pale glow pixel
    return img


def cloud(img, cx, cy, w):
    """A long, flat mountain cloud: white top, a pale blue underside."""
    for x in range(cx - w // 2, cx + w // 2):  # Each column of the cloud
        t = abs(x - cx) / (w / 2)  # 0 in the middle, 1 at the ends
        hump = int(5 * (1 - t * t)) + (lm.noise(x // 4 % 240, cy) % 3)  # Taller in the middle, a little lumpy
        for y in range(cy - hump, cy + 3):  # From the lumpy top to the flat bottom
            put(img, x, y, mm.SKY_LIGHT if y >= cy + 1 else mm.WHITE)  # Bottom two rows are the shaded underside


def bird(img, x, y):
    """A tiny far-away eagle: a soft 5-pixel wide 'v'."""
    for dx, dy in ((-2, -1), (-1, 0), (0, 1), (1, 0), (2, -1)):  # The wing pixels
        put(img, x + dx, y + dy, BIRD)  # One wing pixel


def clouds():
    """L1: four long clouds and two eagles. The layer drifts slowly."""
    img = new_layer()  # Empty layer
    for cx, cy, w in ((90, 34, 110), (330, 62, 64), (560, 26, 140), (810, 74, 80)):  # Place and width of each cloud
        cloud(img, cx, cy, w)  # Draw it
    bird(img, 250, 92)  # An eagle low over the range
    bird(img, 262, 98)  # Its partner
    return img


def far_range():
    """L2: tall, pale snowy peaks far away, with mist at their feet."""
    peaks = [(40, 78, 30), (170, 98, 22), (300, 66, 34), (430, 92, 24), (560, 72, 30), (690, 100, 20), (800, 70, 32), (900, 94, 22)]  # Summit place, top row, snow depth
    return range_layer(peaks, 0.85, 3, 3, 4.0, FAR, FAR_SH, FAR_SNOW, FAR_SNOW_SH, mist_from=150)  # Steep, light, misty at the bottom


def mid_range():
    """L3: nearer rocky peaks with small snow caps, a little darker than the far range."""
    peaks = [(80, 122, 14), (230, 136, 10), (360, 114, 16), (520, 132, 10), (650, 118, 14), (800, 138, 10), (900, 126, 12)]  # Summit place, top row, snow depth
    return range_layer(peaks, 0.7, 7, 2, 3.0, MID, MID_SH, MID_SNOW, MID_SNOW_SH, mist_from=176)  # Mist again at the bottom, so the forest stands in front


def pine(img, cx, base, h, lit, shade, trunk=None):
    """A pixel pine tree in three tiers: the sunny right half and the shaded left half."""
    for i in range(h):  # Each row from the tip down
        tier = i % max(4, h // 3)  # Row within its tier
        half = min(1 + i // 3, 1 + tier // 2 + i // 6)  # Wider going down; each tier starts a little narrower
        for dx in range(-half, half + 1):  # Each pixel across this row
            put(img, cx + dx, base - h + i, shade if dx < 0 else lit)  # Left half in shade
    if trunk:  # Near trees show a short trunk
        put(img, cx, base, trunk)  # Trunk pixel
        put(img, cx, base + 1, trunk)  # Trunk pixel


def forest():
    """L4: a band of soft blue-green pines along the lower slopes."""
    img = new_layer()  # Empty layer
    for x in range(W):  # Each column
        floor = 186 + int(3 * math.sin(2 * math.pi * 4 * x / W)) + jag(x, 11, 2)  # The forest floor line, gently rolling
        column(img, x, floor, FOREST_SH)  # Fill below it
    for i, x in enumerate(range(0, W, 6)):  # A tree every 6 pixels
        h = 14 + lm.noise(i, 5) % 12  # Heights from 14 to 25 pixels
        base = 190 + int(3 * math.sin(2 * math.pi * 4 * x / W))  # Stand on the rolling floor
        pine(img, x + lm.noise(i, 9) % 3, base, h, FOREST, FOREST_SH)  # Draw it
    return img


def meadow():
    """L5: a near alpine meadow just behind the trail, with a few pines and grey rocks."""
    img = new_layer()  # Empty layer
    tops = []  # Top row of the meadow at each x
    for x in range(W):  # Each column
        top = 204 + int(5 * math.sin(2 * math.pi * 3 * x / W) + 2 * math.sin(2 * math.pi * 7 * x / W))  # Soft rolling hill line
        tops.append(top)  # Remember it for the trees and rocks
        put(img, x, top, MEADOW_LIT)  # Sunny top line
        column(img, x, top + 1, MEADOW)  # Grass below it
    for x, h in ((40, 24), (52, 18), (210, 28), (390, 20), (402, 26), (600, 22), (760, 30), (774, 20), (900, 24)):  # Near pines, some in pairs
        pine(img, x, tops[x] + 1, h, PINE, PINE_SH, TRUNK)  # Draw it standing in the meadow
    for x, w in ((130, 10), (480, 14), (680, 8), (850, 12)):  # Rocks in the grass
        for dx in range(w):  # Each column of the rock
            hump = int(4 * math.sin(math.pi * (dx + 0.5) / w)) + 1  # Rounded top
            for dy in range(hump):  # Each row of the rock
                put(img, x + dx, tops[x + dx] + 2 - dy, ROCK_LIT if dy == hump - 1 else ROCK)  # Sunny top row
    return img


def save(img, name):
    """Saves one layer as a PNG in the output folder."""
    os.makedirs(OUT, exist_ok=True)  # Make the folder if it is missing
    img.save(os.path.join(OUT, name))  # Write the file


def main():
    """Draws and saves all six layers."""
    save(sky(), "L0.png")  # Sky and sun
    save(clouds(), "L1.png")  # Clouds and eagles
    save(far_range(), "L2.png")  # Far snowy range
    save(mid_range(), "L3.png")  # Middle rocky range
    save(forest(), "L4.png")  # Pine forest
    save(meadow(), "L5.png")  # Alpine meadow
    print("Mountains backdrop written to", OUT)  # Tell the person running it where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
