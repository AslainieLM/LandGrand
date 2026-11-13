"""Draws every Landform Map PNG for LandGrand (ticket 04, requirement LG-17).

Run from the Unity project folder:  python Tools/Art/make_landform_map_art.py
It writes PNGs to Assets/Art/LandformMap/Sprites/.
The map is one 480x270 picture in the user's 2x2 placement: Mountains top left, Hills top right,
Plains bottom left, Plateaus bottom right. The regions meet at wavy, natural borders.
All colours come from Assets/Art/Palettes/landgrand.gpl.
"""
import math  # math gives sin for the wavy borders and hills
import os  # os is used to make folders and join paths
from PIL import Image  # Pillow draws and saves the pixel-art images
import make_mainmenu_art as mm  # Reuses the Main Menu palette, pixel font and drawing helpers

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))  # The Unity project folder
OUT = os.path.join(ROOT, "Assets", "Art", "LandformMap", "Sprites")  # Where the map PNGs go
W, H = 480, 270  # Map size in pixels, the same as the game screen

# Palette colours that the Main Menu script does not name (names match landgrand.gpl).
LENS = (110, 198, 217)  # Goggle lens blue, used for the stream
SKY = (143, 211, 244)  # Sky blue
MOUNTAIN = (107, 127, 163)  # Mountain rock
MOUNTAIN_SH = (76, 92, 128)  # Mountain rock in shade
SNOW = (232, 238, 245)  # Snow on the peaks
HAZE = (169, 195, 220)  # Far-away mountains
HAZE_SH = (146, 174, 203)  # Far-away mountains in shade
HANDLE = (160, 113, 63)  # Light wood, used for tree trunks
PLAINS_SH = (143, 177, 77)  # Plains grass in shade
BACKPACK = (138, 75, 42)  # Brown, used for the eagle

MOUNTAINS, HILLS, PLAINS, PLATEAUS = range(4)  # Region numbers, in the order the landforms unlock
NAMES = ["mountains", "hills", "plains", "plateaus"]  # File names of the regions


def v_border(y):
    """X of the wavy border between the left and right regions, at row y."""
    return 240 + round(8 * math.sin(y / 21) + 4 * math.sin(y / 7.0))  # Middle of the screen plus two waves


def h_border(x):
    """Y of the wavy border between the top and bottom regions, at column x."""
    return 135 + round(7 * math.sin(x / 29) + 3 * math.sin(x / 11.0))  # Middle of the screen plus two waves


def region(x, y):
    """Which region the pixel (x, y) belongs to."""
    top = y < h_border(x)  # Above the across border
    left = x < v_border(y)  # Left of the up-down border
    if top:  # Top half
        return MOUNTAINS if left else HILLS  # Mountains top left, Hills top right
    return PLAINS if left else PLATEAUS  # Plains bottom left, Plateaus bottom right


def noise(x, y):
    """A fixed pseudo-random number from 0 to 99 for a pixel, so the art is the same on every run."""
    return (x * 73856093 ^ y * 19349663) % 100  # Mix the two numbers and keep the last two digits


SKY_LIGHT_STEP = (SKY, mm.SKY_LIGHT)  # The two colours of the checker band between the two sky blues


def sky(y):
    """Sky colour for row y: deeper blue at the top, light blue lower down, with a checker step between."""
    if y < 26:  # Top band
        return SKY  # Deeper blue
    return SKY_LIGHT_STEP[y % 2] if y < 30 else mm.SKY_LIGHT  # A short checker band, then light blue


def ridge(x, peaks):
    """Top of a mountain range at column x, and the peak that makes it. Each peak is (x, y, slope)."""
    best, best_peak = 10 ** 6, None  # Lowest y found so far, and its peak
    for peak in peaks:  # Try each peak
        px, py, slope = peak  # Where the peak is and how steep it is
        top = py + abs(x - px) * slope  # The slope goes down on both sides of the peak
        if top < best:  # This peak is higher here
            best, best_peak = top, peak  # Keep it
    return best, best_peak  # Highest point and its peak


FAR_PEAKS = [(20, 58, 0.6), (90, 52, 0.7), (165, 56, 0.6), (222, 66, 0.6)]  # Far-away mountains
NEAR_PEAKS = [(55, 40, 1.1), (125, 28, 1.0), (195, 46, 1.2)]  # Big snowy mountains


def mountains_pixel(x, y):
    """Colour of one pixel of the Mountains region: sky, far haze, snowy peaks and green foothills."""
    foot = 104 + round(3 * math.sin(x / 13))  # Where the foothills begin
    if y >= foot:  # Foothills at the bottom
        return mm.LEAF if y < foot + 3 else mm.HILLS_SH  # Dark grass edge, then grass
    top, peak = ridge(x, NEAR_PEAKS)  # The big mountains
    if y >= top:  # Inside a big mountain
        px, py, _ = peak  # The peak this pixel belongs to
        snow_line = py + 12 + (noise(x, 0) % 3)  # Ragged bottom edge of the snow cap
        if y < snow_line:  # In the snow cap
            return SNOW if x <= px else HAZE  # Sunny snow on the left, shaded snow on the right
        return MOUNTAIN if x <= px else MOUNTAIN_SH  # Sunny rock on the left, shaded rock on the right
    far_top, far_peak = ridge(x, FAR_PEAKS)  # The far-away mountains
    if y >= far_top:  # Inside a far mountain
        return HAZE if x <= far_peak[0] else HAZE_SH  # Light on the left, shade on the right
    return sky(y)  # Everything else is sky


def hills_pixel(x, y):
    """Colour of one pixel of the Hills region: sky, two rows of rolling hills and a stream."""
    back = 72 + round(12 * math.sin(x / 27))  # Top of the far hills
    front = 94 + round(10 * math.sin(x / 19 + 1))  # Top of the near hills
    stream = 340 + round(9 * math.sin(y / 9))  # Middle of the stream at this row
    if y >= front - 2 and abs(x - stream) <= 1 + (y - front) // 12:  # The stream gets wider as it comes nearer
        return mm.WHITE if noise(x, y) < 8 else LENS  # Blue water with a few white sparkles
    if y >= front:  # Near hills
        return mm.HILLS if y < front + 2 or noise(x, y) > 6 else mm.PLAINS  # Bright grass with a few light flecks
    if y >= back:  # Far hills
        return mm.HILLS_SH  # Darker green
    return sky(y)  # Everything else is sky


def plains_pixel(x, y):
    """Colour of one pixel of the Plains region: flat grass in rows, with tufts and a few flowers."""
    n = noise(x, y)  # Fixed random number for this pixel
    if n == 0:  # Very rarely
        return mm.HELMET  # A yellow flower
    if (y + ((x // 18) % 2) * 3) % 8 == 0:  # Every eighth row, shifted in blocks
        return PLAINS_SH  # A row of darker grass, like a field
    if n < 4:  # Now and then
        return mm.HILLS  # A green grass tuft
    return mm.PLAINS  # Light green grass


MESAS = [(286, 172, 46, 30), (370, 158, 64, 34), (400, 210, 42, 20), (272, 222, 28, 14)]  # Flat-topped plateaus: left x, top y, width, height


def plateaus_pixel(x, y):
    """Colour of one pixel of the Plateaus region: sandy ground with flat-topped red-brown mesas."""
    for left, top, width, height in MESAS:  # Check each mesa
        if top <= y < top + height:  # On a row of this mesa
            slant = (y - top) // 3  # The sides lean out by one pixel every three rows
            l, r = left - slant, left + width + slant  # Left and right edge on this row
            if l <= x < r:  # Inside the mesa
                if y < top + 2:  # The flat top
                    return mm.GROUND  # Brown top
                if x >= r - (width + 2 * slant) // 3:  # The right third is in shade
                    return mm.PLATEAU_SH  # Shaded face
                return mm.PLATEAU if (y - top) % 6 else mm.PLATEAU_SH  # Sunny face with rock layers
    return mm.VEST_SH if noise(x, y) < 6 else mm.VEST  # Sand with a few darker grains


PIXEL = [mountains_pixel, hills_pixel, plains_pixel, plateaus_pixel]  # Drawing function of each region


def trees(img):
    """Adds pine trees to the mountain foothills and round trees to the hills."""
    for x in range(14, 226, 23):  # Pines along the Mountains foothills
        base = 104 + round(3 * math.sin(x / 13)) + 6  # A little below the foothill edge
        for row in range(7):  # Pine gets wider toward the bottom
            mm.fill_rect(img, x - row // 2, base - 7 + row, 1 + (row // 2) * 2, 1, mm.LEAF)  # One row of the pine
        mm.fill_rect(img, x, base, 1, 2, HANDLE)  # Trunk
    for x, y in [(268, 104), (300, 110), (384, 101), (420, 112), (456, 104)]:  # Round trees on the near hills
        mm.fill_rect(img, x, y + 3, 1, 4, HANDLE)  # Trunk
        for dy in range(-3, 4):  # Rows of the round top
            half = 3 - abs(dy) // 2  # Narrower at the top and bottom
            mm.fill_rect(img, x - half, y + dy, half * 2 + 1, 1, mm.LEAF)  # One row of leaves


TRAIL = [(120, 100), (240, 104), (370, 96), (240, 150), (100, 196), (240, 206), (380, 198)]  # Trail in episode order: Mountains, Hills, Plains, Plateaus


def trail(img):
    """Draws the dotted trail that links the regions in the order they unlock."""
    step = 0  # Counts pixels along the trail
    for (x0, y0), (x1, y1) in zip(TRAIL, TRAIL[1:]):  # Each straight piece of the trail
        length = max(abs(x1 - x0), abs(y1 - y0))  # Number of steps for this piece
        for i in range(length):  # Walk along the piece
            x = round(x0 + (x1 - x0) * i / length)  # Column at this step
            y = round(y0 + (y1 - y0) * i / length)  # Row at this step
            if step % 7 < 3:  # Three pixels on, four off: a dashed line
                mm.fill_rect(img, x, y + 1, 2, 2, mm.DARK)  # Shadow under the dash
                mm.fill_rect(img, x, y, 2, 2, mm.HEADLAMP)  # Cream dash
            step += 1  # Next pixel along the trail


def map_image():
    """Draws the whole connected map."""
    img = Image.new("RGBA", (W, H))  # Full-screen picture
    for y in range(H):  # Each row
        for x in range(W):  # Each pixel in the row
            img.putpixel((x, y), PIXEL[region(x, y)](x, y) + (255,))  # Colour from the pixel's own region
    trees(img)  # Trees on top of the ground
    trail(img)  # The trail on top of everything
    return img


HAMMER = (154, 163, 171)  # Hammer-head grey, a step between the two mask greys
GREYS = [(70, mm.STRAP), (140, mm.MASK_SH), (185, HAMMER), (215, mm.MASK), (999, SNOW)]  # Brightness limit and the grey used below it


def grey(color):
    """Turns a colour into one of the palette greys, keeping dark outlines dark."""
    if color[:3] == mm.OUTLINE:  # Outlines stay as they are
        return mm.OUTLINE  # Darkest colour
    light = 0.3 * color[0] + 0.59 * color[1] + 0.11 * color[2]  # How bright the colour looks
    return next(g for limit, g in GREYS if light < limit)  # The first grey whose limit is above it


def locked_overlay(base, which):
    """A grey copy of one region, see-through everywhere else. Unity also uses it as the region's tap area."""
    img = mm.new_image(W, H)  # Empty, see-through picture
    for y in range(H):  # Each row
        for x in range(W):  # Each pixel in the row
            if region(x, y) == which:  # Only this region's pixels
                img.putpixel((x, y), grey(base.getpixel((x, y))) + (255,))  # Its grey colour
    return img


def pattern(rows, colors, scale=1):
    """Turns a letter pattern into a sprite with a dark outline. Each letter picks a colour; '.' is see-through."""
    h, w = len(rows), len(rows[0])  # Pattern size
    img = mm.new_image(w * scale + 2, h * scale + 2)  # One extra pixel on each side for the outline
    lit = {}  # Shape pixels and their colours
    for r, line in enumerate(rows):  # Each pattern row
        for c, ch in enumerate(line):  # Each pattern pixel
            if ch != ".":  # Part of the shape
                for dy in range(scale):  # Make it scale x scale big
                    for dx in range(scale):  # Each pixel across inside the big square
                        lit[(c * scale + dx + 1, r * scale + dy + 1)] = colors[ch]  # Remember the pixel and its colour
    for (x, y) in lit:  # First pass: outline around every shape pixel
        for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):  # The four neighbours
            img.putpixel((x + dx, y + dy), mm.OUTLINE + (255,))  # Paint the neighbour dark
    for (x, y), color in lit.items():  # Second pass: fill the shape over the outline
        img.putpixel((x, y), color + (255,))  # Paint the shape colour
    return img


PADLOCK = [  # s = shackle, b = body, h = body shine, k = keyhole
    "..ssss..",
    ".s....s.",
    ".s....s.",
    ".s....s.",
    "bbbbbbbb",
    "bhbbbbbb",
    "bbbkkbbb",
    "bbbkkbbb",
    "bbbbkbbb",
    "bbbbbbbb",
]
EAGLE_UP = [  # Wings raised
    "e.......e",
    ".e.....e.",
    "..ee.ee..",
    "....w....",
]
EAGLE_DOWN = [  # Wings lowered
    ".........",
    "..ee.ee..",
    ".e..w..e.",
    "e.......e",
]


def cloud():
    """A small white cloud with a light-blue underside."""
    img = mm.new_image(28, 10)  # Empty cloud picture
    for cx, cy, r in [(8, 6, 4), (14, 4, 4), (20, 6, 4)]:  # Three puffs side by side
        for y in range(10):  # Each row
            for x in range(28):  # Each pixel in the row
                if (x - cx) ** 2 + (y - cy) ** 2 <= r * r:  # Inside this puff
                    img.putpixel((x, y), (mm.SKY_LIGHT if y >= 8 else mm.WHITE) + (255,))  # Light-blue bottom, white top
    return img


def save(img, *parts):
    """Saves an image under Assets/Art/LandformMap/Sprites/."""
    path = os.path.join(OUT, *parts)  # Full file path
    os.makedirs(os.path.dirname(path), exist_ok=True)  # Make the folder if it is missing
    img.save(path)  # Write the PNG


def main():
    """Draws everything."""
    base = map_image()  # The coloured map
    save(base, "map.png")  # Whole map
    for which, name in enumerate(NAMES):  # Each region
        save(locked_overlay(base, which), "Locked", "locked_%s.png" % name)  # Its grey, locked look
        save(mm.text_image(name.upper(), mm.DARK), "Labels", "label_%s.png" % name)  # Its name for the label
    save(pattern(PADLOCK, {"s": mm.MASK, "b": mm.HELMET_SH, "h": mm.HELMET, "k": mm.OUTLINE}, 2), "padlock.png")  # Padlock on locked regions
    save(mm.outlined(mm.STAR, mm.HELMET), "star.png")  # Small star beside the star count
    save(cloud(), "Mountains", "cloud.png")  # Drifting cloud
    save(pattern(EAGLE_UP, {"e": BACKPACK, "w": mm.HEADLAMP}), "Mountains", "eagle_0.png")  # Eagle, wings up
    save(pattern(EAGLE_DOWN, {"e": BACKPACK, "w": mm.HEADLAMP}), "Mountains", "eagle_1.png")  # Eagle, wings down
    flake = mm.new_image(1, 1)  # One-pixel snowflake
    flake.putpixel((0, 0), mm.WHITE + (255,))  # White pixel
    save(flake, "Mountains", "snow_0.png")  # Small snowflake
    big = mm.new_image(2, 2)  # Two-by-two snowflake
    mm.fill_rect(big, 0, 0, 2, 2, mm.WHITE)  # White square
    save(big, "Mountains", "snow_1.png")  # Big snowflake
    print("Landform Map art written to", OUT)  # Tell the person running it where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
