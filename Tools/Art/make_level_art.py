"""Draws the PNGs for the Mountains Easy 1 level (ticket 07): ground and ledge tiles, boulders,
the cave entrance, the goal flag, the touch buttons and the Continue button.

Run from the Unity project folder:  python Tools/Art/make_level_art.py
It writes PNGs to Assets/Art/Level/Sprites/.
All colours come from Assets/Art/Palettes/landgrand.gpl.
"""
import os  # os is used to make folders and join paths
import math  # math gives sine waves and square roots for the new themes
from PIL import Image  # Image is used to read and turn the explorer pictures
import make_mainmenu_art as mm  # Main Menu palette, button and drawing helpers
import make_landform_map_art as lm  # Map colours and the noise helper
import make_menu_camp as camp  # The Main Menu's 34x30 icon button, reused for the pause menu icons

# Pause and Time's up icons (D-057), drawn at 2x like the Main Menu's Settings, Tutorial and Exit icons.
RESUME = camp.PLAY  # Resume: the same play triangle as the Main Menu's PLAY button
RETRY = ["...###.#.", ".##...##.", "#....###.", "#........", "#.......#", "#.......#", ".#.....#.", "..#####.."]  # Retry: a circle arrow going round
LEVELS = ["##.##.##", "##.##.##", "........", "##.##.##", "##.##.##", "........", "##.##.##", "##.##.##"]  # Level selection: a grid of level squares (D-064)
HOME = ["....#....", "...###...", "..#####..", ".#######.", "#########", ".#######.", ".###.###.", ".###.###.", ".###.###."]  # Main Menu: a house

# Result Screen icons (D-059), drawn at 2x inside the word buttons, like the triangle on the Main Menu's PLAY button.
MAP = ["..###..", ".#####.", "##...##", "##...##", ".#####.", "..###..", "..###..", "...#..."]  # Back to Map: a map pin
NEXT = ["#...#", "##..#", "###.#", "#####", "###.#", "##..#", "#...#"]  # Next: a play triangle with a bar, for "go on"


def icon_word_button(label, rows, colors, pressed):
    """A 26-pixel-tall word button with a white 2x icon before the word (D-059). colors are (top, face, bottom)."""
    icon = camp.big(rows, mm.WHITE)  # White icon at 2x
    gap, pad = 4, 6  # Space between icon and word, and at each end; tight so all three buttons fit the Result panel
    w = pad + icon.width + gap + mm.text_width(label, 2) + pad  # Width that fits icon and word
    w += w % 2  # Keep the width even, so the button sits on whole pixels when centred
    img = mm.button(w, 26, "", *colors, pressed=pressed, scale=2)  # The empty button face
    drop = 1 if pressed else 0  # Everything drops 1 pixel when pressed
    x = (w - (icon.width + gap + mm.text_width(label, 2))) // 2  # Centre icon and word together
    y = (26 - icon.height) // 2 + drop  # Centre the icon down
    mm.paste(img, camp.big(rows, mm.DARK), x, y + 1)  # Icon shadow
    mm.paste(img, icon, x, y)  # Icon
    mm.draw_text(img, x + icon.width + gap, (26 - 14) // 2 + drop, label, mm.WHITE, 2, shadow=mm.DARK)  # The word, as on the other buttons
    return img

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))  # The Unity project folder
OUT = os.path.join(ROOT, "Assets", "Art", "Level", "Sprites")  # Where the PNGs go


def put(img, x, y, color, alpha=255):
    """Paints one pixel, skipping pixels outside the image."""
    if 0 <= x < img.width and 0 <= y < img.height:  # Stay inside the image
        img.putpixel((x, y), color + (alpha,))  # The colour at the given see-through level


def outline(img):
    """Adds a dark outline around the shape: every empty pixel that touches it turns dark."""
    shape = [(x, y) for y in range(img.height) for x in range(img.width) if img.getpixel((x, y))[3] > 0]  # Pixels of the shape
    for (x, y) in shape:  # Look around each shape pixel
        for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):  # The four neighbours
            nx, ny = x + dx, y + dy  # Neighbour position
            if 0 <= nx < img.width and 0 <= ny < img.height and img.getpixel((nx, ny))[3] == 0:  # Empty neighbour inside the image
                put(img, nx, ny, mm.OUTLINE)  # Paint it dark
    return img


def grass_top():
    """A 16x8 tile for the top of the ground: grass with a dark edge over brown soil. Unity repeats it across."""
    img = mm.new_image(16, 8)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 8, mm.GROUND)  # Soil
    mm.fill_rect(img, 0, 0, 16, 3, mm.HILLS)  # Grass
    mm.fill_rect(img, 0, 0, 16, 1, mm.PLAINS)  # Sunny top line of the grass
    for x in (2, 7, 12):  # Grass tufts hanging over the soil
        put(img, x, 3, mm.HILLS_SH)  # One tuft pixel
    mm.fill_rect(img, 0, 4, 16, 1, mm.GROUND_SH)  # Shadow under the grass
    put(img, 5, 6, mm.GROUND_SH)  # Small stone in the soil
    put(img, 11, 7, mm.GROUND_SH)  # Another small stone
    return img


def soil():
    """A 16x16 soil tile that Unity repeats below the grass."""
    img = mm.new_image(16, 16)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 16, mm.GROUND)  # Soil
    for y in range(16):  # Each row
        for x in range(16):  # Each pixel
            if lm.noise(x, y) < 8:  # A few darker specks
                put(img, x, y, mm.GROUND_SH)  # Dark speck
    return img


def ledge():
    """A 16x10 rock ledge tile that Unity repeats across: a light top, rock body and a dark underside."""
    img = mm.new_image(16, 10)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 10, lm.MOUNTAIN)  # Rock
    mm.fill_rect(img, 0, 0, 16, 2, lm.HAZE)  # Light top where the explorer stands
    mm.fill_rect(img, 0, 8, 16, 2, lm.MOUNTAIN_SH)  # Dark underside
    for (x, y, c) in ((4, 4, lm.MOUNTAIN_SH), (11, 5, lm.MOUNTAIN_SH), (8, 6, lm.HAZE)):  # Rock texture pixels
        put(img, x, y, c)  # One texture pixel
    mm.fill_rect(img, 0, 9, 16, 1, mm.OUTLINE)  # Dark bottom edge
    return img


def boulder(w, h):
    """A w x h grey boulder with a sunny top left, a shaded bottom right and a few cracks."""
    img = mm.new_image(w, h)  # Empty picture
    cx, cy, rx, ry = (w - 1) / 2, h * 0.62, w / 2 - 1, h * 0.62 - 1  # An oval cut flat at the bottom
    for y in range(h - 1):  # Each row except the outline row at the bottom
        for x in range(1, w - 1):  # Each pixel inside the side outlines
            if y < cy and ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2 > 1.0:  # Outside the rounded top
                continue  # Leave it see-through
            shade = (x - cx) * 0.8 + (y - cy) > 2  # Lower right is in shade
            color = lm.MOUNTAIN_SH if shade else lm.MOUNTAIN  # Rock colour
            if (x - cx) * 0.8 + (y - cy) < -rx * 0.9:  # Sunny edge on the top left
                color = lm.HAZE  # Light rock
            if lm.noise(x, y) < 4:  # A few dark cracks
                color = mm.STRAP  # Crack
            put(img, x, y, color)  # Paint the pixel
    return outline(img)  # Dark edge around the boulder


def cave():
    """A 72x56 rocky cave entrance with a dark opening at the bottom middle."""
    w, h = 72, 56  # Picture size
    img = mm.new_image(w, h)  # Empty picture
    for y in range(4, h):  # Rows of the rock mound
        half = min(w // 2 - 1, 10 + (y - 4) * 1.2)  # Mound gets wider going down
        for x in range(int(w / 2 - half), int(w / 2 + half)):  # Pixels of the mound in this row
            shade = x > w / 2 + (h - y) * 0.3  # Right side in shade
            color = lm.MOUNTAIN_SH if shade else lm.MOUNTAIN  # Rock colour
            if lm.noise(x, y) < 5:  # Texture specks
                color = mm.STRAP if shade else lm.HAZE  # Dark or light speck
            put(img, x, y, color)  # Paint the pixel
    for y in range(22, h):  # Rows of the dark opening
        half = 12 if y > 30 else 4 + (y - 22)  # Rounded top of the opening
        for x in range(w // 2 - half, w // 2 + half):  # Pixels of the opening in this row
            put(img, x, y, mm.OUTLINE)  # Dark inside the cave
    mm.fill_rect(img, w // 2 - 12, h - 3, 24, 3, mm.STRAP)  # Floor of the opening, a little lighter
    return outline(img)  # Dark edge around the mound


def flag():
    """A 16x28 goal flag: a wooden pole with a red flag at the top."""
    img = mm.new_image(16, 28)  # Empty picture
    mm.fill_rect(img, 2, 1, 2, 27, lm.HANDLE)  # Wooden pole
    mm.fill_rect(img, 4, 2, 10, 7, mm.SCARF)  # Red flag
    mm.fill_rect(img, 4, 7, 10, 2, mm.SCARF_SH)  # Shaded bottom of the flag
    mm.fill_rect(img, 12, 4, 2, 3, mm.SCARF_SH)  # Shading at the tip
    mm.fill_rect(img, 2, 0, 2, 1, mm.HELMET)  # Gold knob on top of the pole
    return outline(img)  # Dark edge around the flag


def checkpoint_flag(reached):
    """A 16x28 checkpoint flag. Not reached: a grey rag hangs low on the pole. Reached: a green flag flies at the top."""
    img = mm.new_image(16, 28)  # Empty picture
    mm.fill_rect(img, 2, 1, 2, 27, lm.HANDLE)  # Wooden pole
    if reached:  # Raised green flag
        mm.fill_rect(img, 4, 2, 10, 7, mm.HILLS)  # Green flag
        mm.fill_rect(img, 4, 7, 10, 2, mm.HILLS_SH)  # Shaded bottom
        mm.fill_rect(img, 12, 4, 2, 3, mm.HILLS_SH)  # Shading at the tip
        mm.fill_rect(img, 2, 0, 2, 1, mm.HELMET)  # Gold knob lit up
    else:  # Grey rag low on the pole
        mm.fill_rect(img, 4, 14, 8, 6, mm.MASK_SH)  # Grey rag
        mm.fill_rect(img, 4, 18, 8, 2, mm.STRAP)  # Darker bottom
    return outline(img)  # Dark edge


def spikes():
    """A 16x10 tile of grey spikes on a dark base. Unity repeats it across the trap."""
    img = mm.new_image(16, 10)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 2, mm.STRAP)  # Dark base
    for x0 in (0, 8):  # Two spikes per tile
        for row in range(8):  # Each row of a spike, from the base up
            half = (8 - row) // 2  # The spike gets thinner going up
            mm.fill_rect(img, x0 + 4 - half, 2 + row, max(1, half * 2), 1, mm.MASK)  # One row of the spike
        put(img, x0 + 3, 4, mm.WHITE)  # A shine pixel
    img = img.transpose(Image.FLIP_TOP_BOTTOM)  # Drawn base-first at the top; flip so the spikes point up
    return outline(img)  # Dark edge


def pit_fill():
    """A 16x16 dark tile for the inside of the pit."""
    img = mm.new_image(16, 16)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 16, mm.OUTLINE)  # Almost black
    for (x, y) in ((3, 4), (10, 9), (6, 13)):  # A few specks of rock
        put(img, x, y, mm.BACKPACK_SH)  # One speck
    return img  # No outline, it is a fill


ARROW = ["#.......", "##......", "###.....", "####....", "#####...", "######..", "#####...", "####....", "###.....", "##......", "#......."]  # Right arrow, 11 rows
JUMP = ["....#....", "...###...", "..#####..", ".#######.", "#########", "...###...", "...###...", "...###...", "...###..."]  # Up arrow


def touch_button(size, pattern, flip, pressed):
    """A size x size see-through dark button with a white symbol; pressed buttons are darker and the symbol drops 1 pixel."""
    img = mm.new_image(size, size)  # Empty button
    alpha = 210 if pressed else 150  # Pressed buttons are less see-through
    for y in range(size):  # Each row
        for x in range(size):  # Each pixel
            corner = min(x, size - 1 - x) + min(y, size - 1 - y)  # Distance into the nearest corner
            if corner < 3:  # Cut the corners so the button looks round
                continue  # Leave it see-through
            edge = x in (0, size - 1) or y in (0, size - 1) or corner == 3  # The rim
            put(img, x, y, mm.OUTLINE if edge else mm.STRAP, 255 if edge else alpha)  # Dark rim, see-through middle
    rows = [r[::-1] for r in pattern] if flip else pattern  # Mirror the symbol for the left arrow
    scale = 2  # Each symbol pixel becomes 2x2, so the symbol is easy to see
    ox = (size - len(rows[0]) * scale) // 2  # Centre the symbol across
    oy = (size - len(rows) * scale) // 2 + (1 if pressed else 0)  # Centre it down; drop 1 pixel when pressed
    for r, line in enumerate(rows):  # Each symbol row
        for c, ch in enumerate(line):  # Each symbol pixel
            if ch == "#":  # Lit pixel
                mm.fill_rect(img, ox + c * scale, oy + r * scale, scale, scale, mm.WHITE)  # White 2x2 block
    return img


def somersault(n=4):
    """Makes the somersault frames from the start-screen explorer: the body is tucked into a ball, then turned a
    quarter turn at a time. Quarter turns keep every pixel sharp, so no blurry rotation. Returns n 32x32 images."""
    src = Image.open(os.path.join(ROOT, "Assets", "Art", "StartScreen", "Sprites", "Explorer", "explorer_08.png")).convert("RGBA")  # Standing explorer
    body = src.crop(src.getbbox())  # Only the drawn pixels
    ball = body.resize((20, 20), Image.NEAREST)  # Squashed into a compact curled-up shape
    frames = []  # The finished pictures
    for i in range(n):  # One picture per quarter turn
        turned = ball.rotate(-90 * i, expand=True)  # Clockwise, so the flip goes forward when the explorer faces right
        img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))  # Same size as the walk frames
        img.paste(turned, (16 - turned.width // 2, 18 - turned.height // 2), turned)  # Ball centre where the body centre was, 13 pixels above the feet
        frames.append(img)  # Keep it
    return frames  # All frames


def gem_icon():
    """A 12x12 blue-white gem for the collectibles counter at the top left of the level."""
    img = mm.new_image(12, 12)  # Empty icon
    rows = ["....oooo....", "...obbbbo...", "..obwwbbbo..", ".obwbbbbbbo.", "obbbbbbbbbbo", "obbbbbbbbbbo", ".obbbbbbbbo.", "..obbbbbbo..", "...obbbbo...", "....obbo....", ".....oo.....", "............"]  # o outline, b blue, w shine
    colors = {"o": mm.OUTLINE, "b": (92, 160, 214), "w": mm.WHITE}  # What each letter paints
    for y, row in enumerate(rows):  # Each row, top to bottom
        for x, ch in enumerate(row):  # Each pixel in the row
            if ch in colors:  # Skip empty pixels
                put(img, x, y, colors[ch])  # Paint it
    return img


def pause_icon(pressed):
    """A 24x24 brown button with two white bars, for the pause button at the top right."""
    img = mm.new_image(24, 24)  # Empty button
    mm.rounded_box(img, 0, 0, 24, 24)  # Dark outline
    mm.fill_rect(img, 1, 1, 22, 22, mm.PLATEAU_SH if pressed else mm.PLATEAU)  # Button face
    mm.fill_rect(img, 1, 20, 22, 3, mm.BACKPACK_SH)  # Shaded bottom band
    off = 1 if pressed else 0  # A pressed button drops its symbol 1 pixel
    mm.fill_rect(img, 8, 6 + off, 3, 10, mm.WHITE)  # Left bar
    mm.fill_rect(img, 13, 6 + off, 3, 10, mm.WHITE)  # Right bar
    return img  # Done


def mark_icon(ok):
    """A 10x10 tick (green) or cross (red) for the goals on the Result Screen."""
    img = mm.new_image(10, 10)  # Empty icon
    if ok:  # A tick
        for (x, y) in ((1, 5), (2, 6), (3, 7), (4, 6), (5, 5), (6, 4), (7, 3), (8, 2)):  # The tick path
            put(img, x, y, mm.LEAF)  # One pixel
            put(img, x, y + 1, mm.HILLS_SH)  # A thicker line
    else:  # A cross
        for i in range(2, 8):  # The two diagonals
            put(img, i, i, mm.SCARF_SH)  # Down-right line
            put(img, i, 9 - i, mm.SCARF_SH)  # Up-right line
            put(img, i + 1, i, mm.SCARF)  # Thicker
            put(img, i + 1, 9 - i, mm.SCARF)  # Thicker
    return img  # Done


PROGRESS_W = 144  # Width of the trail progress bar under the timer (decision D-049)


def progress_track():
    """The 144x6 empty trail bar: a dark frame around a grey groove."""
    img = mm.new_image(PROGRESS_W, 6)  # Empty bar
    mm.fill_rect(img, 0, 0, PROGRESS_W, 6, mm.OUTLINE)  # Dark frame
    mm.fill_rect(img, 1, 1, PROGRESS_W - 2, 4, mm.STRAP)  # Grey groove
    mm.fill_rect(img, 1, 1, PROGRESS_W - 2, 1, mm.OUTLINE)  # Inner shadow along the top of the groove
    return img  # Done


def progress_fill():
    """A 1x4 green column; Unity stretches it sideways to show how far the explorer has walked."""
    img = mm.new_image(1, 4)  # One pixel wide
    put(img, 0, 0, mm.PLAINS)  # Sunny top line
    put(img, 0, 1, mm.HILLS)  # Green
    put(img, 0, 2, mm.HILLS)  # Green
    put(img, 0, 3, mm.HILLS_SH)  # Shaded bottom line
    return img  # Done


def progress_marker():
    """A 9x9 explorer marker for the bar: a round yellow helmet over a cream face, with a dark edge."""
    img = mm.new_image(9, 9)  # Room for the dot and its outline
    for y in range(1, 8):  # Rows of the dot
        for x in range(1, 8):  # Columns of the dot
            if (x - 4) ** 2 + (y - 4) ** 2 <= 10:  # Inside a round dot 7 pixels across
                put(img, x, y, mm.HELMET if y <= 4 else mm.HEADLAMP)  # Helmet on top, face below
    put(img, 5, 2, mm.WHITE)  # Shine on the helmet
    return outline(img)  # Dark edge


def progress_flag(color, shade):
    """An 8x12 small flag for the bar: a wooden pole with a coloured flag, standing on the bar."""
    img = mm.new_image(8, 12)  # Room for the flag and its outline
    mm.fill_rect(img, 1, 1, 1, 10, lm.HANDLE)  # Pole
    mm.fill_rect(img, 2, 1, 4, 4, color)  # Flag
    mm.fill_rect(img, 2, 4, 4, 1, shade)  # Shaded bottom edge of the flag
    return outline(img)  # Dark edge


# ---- Volcano Trail and Folded Peaks art (new level themes) ----
BASALT = (74, 74, 84)  # Dark basalt grey
BASALT_SH = (52, 52, 62)  # Basalt in shade
BASALT_LT = (112, 112, 124)  # Basalt in sun
LAVA_ROCK = (44, 42, 52)  # Very dark rock for the volcanic soil
LAVA_ROCK_SH = (32, 30, 40)  # Even darker specks
LAVA_SWIRL = (58, 43, 50)  # Faint cooled-lava swirl
LAVA_ORANGE = (255, 140, 40)  # Hot orange glow
FOLD_GREY = (128, 131, 140)  # Folded rock grey band
FOLD_GREY_SH = (98, 101, 112)  # Grey band in shade
FOLD_TAN = (190, 164, 118)  # Folded rock tan band
FOLD_TAN_SH = (150, 125, 88)  # Tan band in shade
FOLD_BLUE = (104, 122, 150)  # Folded rock blue-grey band
FOLD_BLUE_SH = (80, 96, 124)  # Blue-grey band in shade
FROST = (232, 238, 245)  # Snow and frost white
STEAM_SH = (200, 208, 220)  # Pale grey shade of the steam


def wave(x, amp=2, period=16):
    """A whole-number wave height for column x. It repeats every `period` pixels so tiles join with no seam."""
    return int(round(amp * math.sin(2 * math.pi * x / period)))  # Up and down by amp


def draw_grid(img, rows, ox, oy, colors):
    """Paints a small picture drawn with letters. A dot or a letter not in colors stays empty."""
    for y, row in enumerate(rows):  # Each row, top to bottom
        for x, ch in enumerate(row):  # Each letter in the row
            if ch in colors:  # Skip empty pixels
                put(img, ox + x, oy + y, colors[ch])  # Paint it


def volcanic_top():
    """A 16x8 tile for volcanic ground: dark basalt with a few warm ember specks. Unity repeats it across."""
    img = mm.new_image(16, 8)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 8, LAVA_ROCK)  # Dark rock below
    mm.fill_rect(img, 0, 0, 16, 4, BASALT)  # Basalt cap
    mm.fill_rect(img, 0, 0, 16, 1, BASALT_LT)  # Sunny top line
    mm.fill_rect(img, 0, 4, 16, 1, BASALT_SH)  # Shadow under the cap
    for x in (1, 6, 11, 14):  # Rough bumps hanging under the cap
        put(img, x, 5, BASALT_SH)  # One bump pixel
    for (x, y) in ((4, 1), (11, 2), (13, 6)):  # Ember specks
        put(img, x, y, mm.SCARF)  # Warm red speck
    put(img, 8, 6, LAVA_ROCK_SH)  # A dark stone
    return img


def volcanic_soil():
    """A 16x16 very dark rock tile with faint cooled-lava swirls. It tiles left-right and top-bottom."""
    img = mm.new_image(16, 16)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 16, LAVA_ROCK)  # Dark rock
    for y in range(16):  # Each row
        for x in range(16):  # Each pixel
            if lm.noise(x, y) < 9:  # A few darker specks
                put(img, x, y, LAVA_ROCK_SH)  # Dark speck
    for y0 in (4, 12):  # Two swirl lines per tile
        for x in range(16):  # Along the tile
            put(img, x, (y0 + wave(x, 2)) % 16, LAVA_SWIRL)  # Faint wavy line that wraps round
    return img


def ledge_volcanic():
    """A 16x10 dark grey rock slab tile that Unity repeats across."""
    img = mm.new_image(16, 10)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 10, BASALT)  # Rock
    mm.fill_rect(img, 0, 0, 16, 2, BASALT_LT)  # Light top where the explorer stands
    mm.fill_rect(img, 0, 8, 16, 2, BASALT_SH)  # Dark underside
    for (x, y, c) in ((4, 4, BASALT_SH), (11, 5, BASALT_SH), (8, 6, BASALT_LT), (14, 3, LAVA_ROCK)):  # Rock texture pixels
        put(img, x, y, c)  # One texture pixel
    mm.fill_rect(img, 0, 9, 16, 1, mm.OUTLINE)  # Dark bottom edge
    return img


def boulder_colored(w, h, mid, shade, light, speck, band=None):
    """Same shape as boulder(w, h) with your own colours. band is an optional (colour, shade) pair for layered rock."""
    img = mm.new_image(w, h)  # Empty picture
    cx, cy, rx, ry = (w - 1) / 2, h * 0.62, w / 2 - 1, h * 0.62 - 1  # An oval cut flat at the bottom
    for y in range(h - 1):  # Each row except the outline row at the bottom
        for x in range(1, w - 1):  # Each pixel inside the side outlines
            if y < cy and ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2 > 1.0:  # Outside the rounded top
                continue  # Leave it see-through
            shaded = (x - cx) * 0.8 + (y - cy) > 2  # Lower right is in shade
            color = shade if shaded else mid  # Rock colour
            if band and y % 6 == 3:  # A thin layer line across the rock
                color = band[1] if shaded else band[0]  # Layer colour
            if (x - cx) * 0.8 + (y - cy) < -rx * 0.9:  # Sunny edge on the top left
                color = light  # Light rock
            if lm.noise(x, y) < 4:  # A few dark cracks or pores
                color = speck  # Dark speck
            put(img, x, y, color)  # Paint the pixel
    return outline(img)  # Dark edge around the boulder


def volcanic_boulder(w, h):
    """A dark porous volcanic boulder."""
    return boulder_colored(w, h, BASALT, BASALT_SH, BASALT_LT, mm.OUTLINE)  # Dark rock, black pores


def arch():
    """A 64x56 lava-rock arch the explorer walks under: two dark pillars, a curved top and a glowing crack.
    The gap in the middle is 24 pixels wide and 36 pixels tall."""
    w, h = 64, 56  # Picture size
    img = mm.new_image(w, h)  # Empty picture
    for y in range(1, h):  # Each row of the arch
        if y < 29:  # Curved top part
            half = 32 * math.sqrt(max(0.0, 1 - ((y - 29) / 28.0) ** 2))  # Outer curve width in this row
        else:  # Straight pillars
            half = 32  # Full width
        if y >= 20:  # Straight gap sides
            gap = 13.0  # Gap is 26 wide before the outline takes 1 pixel each side
        elif y >= 14:  # Curved ceiling of the gap
            gap = 13 * math.sqrt(max(0.0, 1 - ((y - 20) / 6.0) ** 2))  # Inner curve width in this row
        else:  # Solid rock above the gap
            gap = -1  # No gap here
        for x in range(int(32 - half + 0.5), int(32 + half - 0.5)):  # Pixels of the arch in this row
            if abs(x + 0.5 - 32) < gap:  # Inside the open gap
                continue  # Leave it see-through
            shade = (32 - x) * 0.6 + (y - 28) * 0.4 > 6  # Left and lower parts are in shade
            color = BASALT_SH if shade else BASALT  # Rock colour
            if (x - 32) * 0.6 - (y - 28) * 0.4 > 22:  # Sunny upper right
                color = BASALT_LT  # Light rock
            if lm.noise(x, y) < 6:  # Texture specks
                color = LAVA_ROCK  # Dark speck
            put(img, x, y, color)  # Paint the pixel
    for (x, y) in ((38, 4), (38, 5), (39, 6), (39, 7), (40, 8), (40, 9), (41, 10), (41, 11), (40, 12)):  # Glowing crack in the top
        put(img, x, y, LAVA_ORANGE)  # Orange glow
        put(img, x + 1, y, mm.SCARF_SH)  # Dim edge on the right
    for (x, y) in ((6, 38), (6, 39), (7, 40), (7, 41), (8, 42), (8, 43), (7, 44)):  # Small glowing crack low in the left pillar
        put(img, x, y, LAVA_ORANGE)  # Orange glow
    put(img, 39, 7, mm.HELMET)  # Hot yellow spot
    put(img, 7, 41, mm.HELMET)  # Hot yellow spot
    return outline(img)  # Dark edge around the arch


def vent():
    """A 24x8 steam vent set into the ground: a dark rock rim, a black hole and an orange glow inside.
    The top row lines up with the top of the ground."""
    img = mm.new_image(24, 8)  # Empty picture
    mm.fill_rect(img, 0, 0, 24, 8, BASALT)  # Rim rock
    mm.fill_rect(img, 0, 0, 24, 1, BASALT_LT)  # Sunny top line like the ground
    mm.fill_rect(img, 0, 6, 24, 2, BASALT_SH)  # Dark bottom
    for (x, y) in ((2, 3), (4, 2), (19, 3), (21, 2), (20, 5)):  # Rim texture
        put(img, x, y, BASALT_SH)  # Dark pixel
    mm.fill_rect(img, 6, 0, 12, 6, mm.OUTLINE)  # Black hole
    mm.fill_rect(img, 7, 4, 10, 2, mm.SCARF_SH)  # Dim glow at the bottom of the hole
    mm.fill_rect(img, 8, 5, 8, 1, LAVA_ORANGE)  # Bright orange glow
    mm.fill_rect(img, 10, 5, 4, 1, mm.HELMET)  # Hot yellow middle
    put(img, 6, 0, BASALT_SH)  # Round the top left of the hole
    put(img, 17, 0, BASALT_SH)  # Round the top right of the hole
    return img


def steam_warn(frame):
    """A 24x16 thin white wisp rising from the vent. Frame 1 is a little taller. It warns before the puff."""
    img = mm.new_image(24, 16)  # Empty picture
    top = 8 if frame == 0 else 3  # How high the wisp climbs
    for y in range(15, top - 1, -1):  # From the vent up
        t = (15 - y) / float(15 - top)  # 0 at the vent, 1 at the tip
        x = 12 + int(round(1.5 * math.sin((15 - y) * 0.7 + frame * 1.5)))  # Wavy path
        alpha = int(235 - 150 * t)  # Fades out going up
        put(img, x, y, mm.WHITE, alpha)  # Thin white line
        if t < 0.5:  # Thicker near the vent
            put(img, x + 1, y, STEAM_SH, alpha)  # Pale shade pixel beside it
    return img


def steam_puff(frame):
    """A 24x64 steam column. It grows with the frame, billows and thins out at the top. Bottom centre sits on the vent."""
    img = mm.new_image(24, 64)  # Empty picture
    height = (24, 40, 52, 60)[frame]  # How tall the puff is in this frame
    widest = (5, 7, 9, 10)[frame]  # How wide the puff gets
    for y in range(63, 63 - height, -1):  # From the vent up
        t = (63 - y) / float(height)  # 0 at the vent, 1 at the top
        half = 2.5 + (widest - 2.5) * math.sin(min(1.0, t * 1.3) * math.pi / 2)  # Wider going up
        half += 1.6 * math.sin(y * 0.45 + frame * 1.3) * min(1.0, t * 3)  # Billowing sides
        for x in range(int(12 - half), int(12 + half)):  # Pixels of the column in this row
            if t > 0.7 and lm.noise(x + frame * 5, y) > 100 - (t - 0.7) * 260:  # Ragged, thin top
                continue  # Leave a hole
            alpha = 240 if t < 0.7 else int(240 - (t - 0.7) * 400)  # Fades near the top
            left = x < 12 - half + 3  # Left edge is in shade, light is from the right
            put(img, x, y, STEAM_SH if left else mm.WHITE, max(40, alpha))  # Pale grey or white
        if t > 0.1 and int(12 + half) - int(12 - half) > 3:  # Pale edge on the right so the puff reads on the sky
            put(img, int(12 + half) - 1, y, STEAM_SH, 230 if t < 0.7 else 120)  # Edge pixel
    return img


def wisp(frame):
    """An 8x24 small faint wisp of background steam. It is smaller and fainter than the launch steam."""
    img = mm.new_image(8, 24)  # Empty picture
    for y in range(23, 3, -1):  # From the bottom up
        t = (23 - y) / 19.0  # 0 at the bottom, 1 at the tip
        x = 4 + int(round(1.6 * math.sin((23 - y) * 0.5 + frame * math.pi / 2)))  # Wavy path that moves each frame
        alpha = int(110 * (1 - t))  # Very faint, fades going up
        put(img, x, y, mm.WHITE, alpha)  # Faint white pixel
        put(img, x + 1, y, mm.WHITE, alpha // 2)  # Fainter neighbour
    return img


def lava_crack(frame):
    """A 16x6 jagged glowing crack on a see-through background. Frames glow dim, medium, bright, medium."""
    img = mm.new_image(16, 6)  # Empty picture
    level = (0, 1, 2, 1)[frame]  # How hot the crack is
    path = [3, 3, 2, 2, 3, 4, 4, 3, 2, 1, 1, 2, 3, 3, 4, 4]  # Row of the crack in each column
    core = (mm.SCARF_SH, mm.SCARF, mm.HELMET)[level]  # Middle colour of the crack
    glow = ((90, 36, 30), mm.SCARF_SH, LAVA_ORANGE)[level]  # Colour just above and below the crack
    for x, y in enumerate(path):  # Each column of the crack
        for dy in (-1, 1):  # Pixels above and below
            if (x + dy) % 3 != 0 or level == 2:  # Break the glow up a little so it looks rough
                put(img, x, y + dy, glow, 200)  # Soft glow pixel
    for x, y in enumerate(path):  # Draw the core on top
        put(img, x, y, core)  # Bright middle
    put(img, 9, 0, core)  # A short branch going up
    put(img, 3, 4, core)  # A short branch going down
    return img


def fold_top():
    """A 16x8 tile for folded-rock ground: grey stone with thin patches of snow. Unity repeats it across."""
    img = mm.new_image(16, 8)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 8, FOLD_GREY_SH)  # Rock below
    mm.fill_rect(img, 0, 0, 16, 4, FOLD_GREY)  # Stone cap
    mm.fill_rect(img, 0, 4, 16, 1, FOLD_BLUE_SH)  # Shadow under the cap
    for x in (3, 9, 14):  # Rough bumps hanging under the cap
        put(img, x, 5, FOLD_BLUE_SH)  # One bump pixel
    for (x0, x1) in ((1, 5), (9, 12)):  # Patches of snow
        mm.fill_rect(img, x0, 0, x1 - x0 + 1, 1, FROST)  # Thin snow line
        mm.fill_rect(img, x0 + 1, 1, max(1, x1 - x0 - 1), 1, STEAM_SH)  # Frost under the snow
    put(img, 6, 6, FOLD_TAN_SH)  # A tan fleck in the rock
    put(img, 12, 7, FOLD_BLUE_SH)  # A dark fleck
    return img


def folded_band(v):
    """Which rock layer (light colour, shade colour, is it the first row) a row position v (0 to 15) belongs to."""
    if v < 6:  # Grey band
        return FOLD_GREY, FOLD_GREY_SH, v == 0  # Grey layer
    if v < 10:  # Tan band
        return FOLD_TAN, FOLD_TAN_SH, v == 6  # Tan layer
    return FOLD_BLUE, FOLD_BLUE_SH, v == 10  # Blue-grey layer


def fold_soil():
    """A 16x16 grey rock tile with grey, tan and blue-grey bands that wave up and down. It tiles in both directions."""
    img = mm.new_image(16, 16)  # Empty tile
    for y in range(16):  # Each row
        for x in range(16):  # Each pixel
            v = (y - wave(x, 2)) % 16  # Position inside the layers, bent by the wave
            light, shade, first = folded_band(v)  # The layer here
            put(img, x, y, shade if first else light)  # Darker line where a layer begins
            if lm.noise(x, y) < 5:  # A few specks
                put(img, x, y, shade)  # Dark speck
    return img


def ledge_fold():
    """A 16x10 layered grey slab tile that Unity repeats across, with thin bands."""
    img = mm.new_image(16, 10)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 10, FOLD_GREY)  # Rock
    mm.fill_rect(img, 0, 0, 16, 2, (176, 180, 190))  # Light top where the explorer stands
    mm.fill_rect(img, 0, 4, 16, 1, FOLD_TAN)  # Tan band
    mm.fill_rect(img, 0, 6, 16, 2, FOLD_BLUE)  # Blue-grey band
    mm.fill_rect(img, 0, 8, 16, 2, FOLD_GREY_SH)  # Dark underside
    for x in (3, 10):  # Snow flecks on the top
        put(img, x, 0, FROST)  # One fleck
    mm.fill_rect(img, 0, 9, 16, 1, mm.OUTLINE)  # Dark bottom edge
    return img


def fold_boulder(w, h):
    """A grey layered boulder with thin tan layer lines."""
    return boulder_colored(w, h, FOLD_GREY, FOLD_GREY_SH, (186, 190, 198), FOLD_BLUE_SH, (FOLD_TAN, FOLD_TAN_SH))  # Grey rock with tan layers


def cairn():
    """A 40x44 summit stone pile: flat stones stacked up, a little snow on top."""
    img = mm.new_image(40, 44)  # Empty picture
    stones = [(1, 35, 38, 8, FOLD_GREY, FOLD_GREY_SH), (5, 27, 30, 8, FOLD_TAN, FOLD_TAN_SH), (9, 19, 23, 8, FOLD_BLUE, FOLD_BLUE_SH),
              (11, 12, 18, 7, FOLD_GREY, FOLD_GREY_SH), (14, 5, 12, 7, FOLD_TAN, FOLD_TAN_SH)]  # x, y, width, height, colour, shade of each stone
    for (x0, y0, sw, sh, c, cs) in stones:  # Each stone, bottom to top
        for y in range(y0, y0 + sh):  # Each row of the stone
            for x in range(x0, x0 + sw):  # Each pixel of the row
                if (x in (x0, x0 + sw - 1)) and (y in (y0, y0 + sh - 1)):  # Cut the corners to round the stone
                    continue  # Leave it see-through
                color = cs if y >= y0 + sh - 2 else c  # Shaded underside
                if y == y0 and x > x0 + sw // 2:  # Sunny top right edge
                    color = (210, 212, 220)  # Light edge
                put(img, x, y, color)  # Paint the pixel
    for (x0, y0, sw, _sh, _c, _cs) in stones[2:]:  # Snow on the upper stones
        mm.fill_rect(img, x0 + 2, y0, sw - 5, 1, FROST)  # Thin snow on the top edge
    mm.fill_rect(img, 16, 3, 8, 2, FROST)  # Snow cap on the top stone
    mm.fill_rect(img, 18, 2, 4, 1, FROST)  # Round the snow cap
    return outline(img)  # Dark edge around the pile


def snow_flake(size):
    """A 2x2 or 4x4 white snowflake dot."""
    img = mm.new_image(size, size)  # Empty picture
    if size == 2:  # Tiny dot
        mm.fill_rect(img, 0, 0, 2, 2, mm.WHITE)  # White block
    else:  # Slightly bigger round flake
        draw_grid(img, [".##.", "####", "####", ".##."], 0, 0, {"#": mm.WHITE})  # Round white flake
        put(img, 3, 2, STEAM_SH)  # Pale shade on the lower right
        put(img, 2, 3, STEAM_SH)  # Pale shade on the lower right
    return img


EAGLE_COLORS = {"o": mm.OUTLINE, "b": lm.BACKPACK, "g": (214, 160, 60), "d": mm.BACKPACK_SH, "w": (240, 228, 196), "y": mm.HELMET}  # Eagle letter colours


def eagle(frame):
    """A 32x16 golden-brown eagle facing right. Frame 0 sits with folded wings; frames 1 to 3 flap in flight."""
    img = mm.new_image(32, 16)  # Empty picture
    if frame == 0:  # Perched on its feet at the bottom centre
        rows = ["....ooooo...",
                "...owwwwwo..",
                "..owwwowwoyy",
                "..owwwwwwyyo",
                "..ooggggooo.",
                ".obbgggggbo.",
                "obbbgggggbbo",
                "obbggggggbbo",
                "obbbggggbbbo",
                "obbbbggbbbbo",
                "odbbbbbbbbdo",
                ".odbbbbbbdo.",
                "..oddbbddo..",
                "...ooooooo..",
                "...oy..yo...",
                "...yy..yy..."]  # 12 wide, 16 tall: head, chest, folded wings, tail and feet
        draw_grid(img, rows, 10, 0, EAGLE_COLORS)  # Centre it
        return img
    tip = {1: 1, 2: 7, 3: 14}[frame]  # Where the wing tips are
    for side in (-1, 1):  # Left and right wing
        for d in range(1, 15):  # Out from the body
            t = d / 14.0  # 0 at the shoulder, 1 at the tip
            yc = 8 + (tip - 8) * (t ** 1.2)  # Wing centre line
            thick = int(round(4 - 2.5 * t))  # Wing is thinner at the tip
            x = 16 + side * d  # Column of the wing
            for k in range(thick):  # Pixels of the wing across its thickness
                y = int(round(yc)) + k - thick // 2  # Row of this pixel
                color = (214, 160, 60) if k < thick - 1 else lm.BACKPACK  # Gold with a darker back edge
                if d > 10:  # Dark feather tips
                    color = mm.BACKPACK_SH  # Dark tip
                put(img, x, y, color)  # Paint the pixel
    draw_grid(img, ["..bbbb..", ".bggggb.", "bbggggbw", ".bggggww", "..bbbbwy"], 12, 6, EAGLE_COLORS)  # Body, tail on the left and head on the right
    put(img, 21, 8, mm.OUTLINE)  # Eye
    return outline(img)  # Dark edge around the whole bird


def banner_plate():
    """A 208x32 level-start title plate: a cream panel with a brown wood border and small rivets. The middle is plain."""
    w, h = 208, 32  # Picture size
    img = mm.new_image(w, h)  # Empty picture
    mm.fill_rect(img, 0, 0, w, h, mm.OUTLINE)  # Dark outline
    mm.fill_rect(img, 1, 1, w - 2, h - 2, mm.PLATEAU)  # Wood border
    mm.fill_rect(img, 1, h - 4, w - 2, 3, mm.PLATEAU_SH)  # Shaded lower border
    mm.fill_rect(img, 1, 1, 1, h - 2, mm.PLATEAU_SH)  # Shaded left border
    mm.fill_rect(img, 1, 1, w - 2, 1, mm.HELMET_SH)  # Sunny top line
    for x in range(4, w - 4, 7):  # Faint wood grain on the top and bottom border
        put(img, x, 2, mm.PLATEAU_SH)  # Grain mark on top
        put(img, x + 3, h - 3, mm.BACKPACK_SH)  # Grain mark at the bottom
    mm.fill_rect(img, 4, 4, w - 8, h - 8, mm.VEST_SH)  # Inner dark edge of the paper
    mm.fill_rect(img, 5, 5, w - 10, h - 10, mm.HEADLAMP)  # Cream paper
    mm.fill_rect(img, 5, h - 6, w - 10, 1, mm.VEST)  # Soft shadow along the paper bottom
    for (x, y) in ((2, 2), (w - 4, 2), (2, h - 4), (w - 4, h - 4)):  # Four rivets in the corners
        mm.fill_rect(img, x, y, 2, 2, mm.MASK)  # Metal rivet
        put(img, x, y + 1, mm.MASK_SH)  # Shade on the rivet
    for (x, y) in ((0, 0), (w - 1, 0), (0, h - 1), (w - 1, h - 1)):  # Cut the outer corners
        img.putpixel((x, y), (0, 0, 0, 0))  # Make the corner see-through
    return img


def save(img, name):
    """Saves an image under Assets/Art/Level/Sprites/."""
    os.makedirs(os.path.dirname(os.path.join(OUT, name)), exist_ok=True)  # Make the folder if it is missing
    img.save(os.path.join(OUT, name))  # Write the PNG


def main():
    """Draws everything."""
    save(grass_top(), "grass_top.png")  # Top of the ground
    save(soil(), "soil.png")  # Ground below the grass
    save(ledge(), "ledge.png")  # Rock ledge
    save(boulder(32, 24), "boulder_small.png")  # Small boulder
    save(boulder(48, 36), "boulder_big.png")  # Big boulder
    save(cave(), "cave.png")  # Cave entrance (the goal)
    save(flag(), "flag.png")  # Goal flag
    for pressed in (False, True):  # Normal and pressed pictures
        tag = "_pressed" if pressed else ""  # File name ending
        save(touch_button(40, ARROW, True, pressed), "btn_left%s.png" % tag)  # Left arrow
        save(touch_button(40, ARROW, False, pressed), "btn_right%s.png" % tag)  # Right arrow
        save(touch_button(44, JUMP, False, pressed), "btn_jump%s.png" % tag)  # Jump
    for i, frame in enumerate(somersault()):  # The somersault frames for the double jump
        save(frame, "Flip/flip_%02d.png" % i)  # flip_00 to flip_03
    green = (mm.HILLS, mm.HILLS_SH, mm.LEAF)  # Same colours as the Play button
    save(checkpoint_flag(False), "checkpoint_off.png")  # Checkpoint not reached
    save(checkpoint_flag(True), "checkpoint_on.png")  # Checkpoint reached
    save(spikes(), "spikes.png")  # Spike trap tile
    save(pit_fill(), "pit_fill.png")  # Inside of the pit
    brown = (mm.PLATEAU, mm.PLATEAU_SH, mm.BACKPACK_SH)  # Same colours as the Back buttons
    save(mark_icon(True), "icon_tick.png")  # Goal met
    save(mark_icon(False), "icon_cross.png")  # Goal not met
    for pressed in (False, True):  # Normal and pressed pictures
        tag = "_pressed" if pressed else ""  # File name ending
        save(icon_word_button("BACK TO MAP", MAP, brown, pressed), "btn_map%s.png" % tag)  # Back to the Landform Map, with a map icon (D-059)
        save(icon_word_button("REPLAY", RETRY, brown, pressed), "btn_replay%s.png" % tag)  # Replay from the lesson, with the circle arrow
        save(icon_word_button("NEXT LEVEL", NEXT, green, pressed), "btn_next_level%s.png" % tag)  # Next level, green like Continue was
        save(icon_word_button("NEXT EPISODE", NEXT, green, pressed), "btn_next_episode%s.png" % tag)  # Next landform, after the whole landform
    for pressed in (False, True):  # Normal and pressed pictures
        tag = "_pressed" if pressed else ""  # File name ending
        save(pause_icon(pressed), "btn_pause%s.png" % tag)  # Pause button
        save(camp.icon_button(RESUME, green[1], green[0], green[2], pressed), "btn_resume_icon%s.png" % tag)  # Resume: green play icon (D-057)
        save(camp.icon_button(RETRY, brown[1], brown[0], brown[2], pressed), "btn_retry_icon%s.png" % tag)  # Retry: brown circle arrow icon
        save(camp.icon_button(HOME, brown[1], brown[0], brown[2], pressed), "btn_home_icon%s.png" % tag)  # Main Menu: brown house icon
        save(camp.icon_button(LEVELS, brown[1], brown[0], brown[2], pressed), "btn_levels_icon%s.png" % tag)  # Level selection: brown grid icon (D-064)
    save(gem_icon(), "icon_gem.png")  # Counter icon
    save(progress_track(), "progress_track.png")  # Empty trail bar
    save(progress_fill(), "progress_fill.png")  # Walked part of the bar
    save(progress_marker(), "progress_marker.png")  # Where the explorer is
    save(progress_flag(mm.MASK_SH, mm.STRAP), "progress_checkpoint_off.png")  # Checkpoint not reached: grey, like the level flag
    save(progress_flag(mm.HILLS, mm.HILLS_SH), "progress_checkpoint_on.png")  # Checkpoint reached: green, like the level flag
    save(progress_flag(mm.SCARF, mm.SCARF_SH), "progress_goal.png")  # The goal: red, like the goal flag
    save(mm.button(80, 26, "OK", *green, pressed=False, scale=2), "btn_ok.png")  # OK button of the first-find pop-up
    save(mm.button(80, 26, "OK", *green, pressed=True, scale=2), "btn_ok_pressed.png")  # OK button, pressed
    save(mm.text_image("LEVEL CLEARED!", mm.DARK, 2), "text_cleared.png")  # Result heading
    save(volcanic_top(), "volcanic_top.png")  # Volcano Trail ground top
    save(volcanic_soil(), "volcanic_soil.png")  # Volcano Trail soil
    save(ledge_volcanic(), "ledge_volcanic.png")  # Volcano Trail ledge
    save(volcanic_boulder(32, 24), "boulder_small_volcanic.png")  # Small volcanic boulder
    save(volcanic_boulder(48, 36), "boulder_big_volcanic.png")  # Big volcanic boulder
    save(arch(), "arch.png")  # Lava-rock arch (the goal)
    save(vent(), "vent.png")  # Steam vent in the ground
    for i in range(2):  # Two warning frames
        save(steam_warn(i), "Props/steam_warn_%02d.png" % i)  # Thin wisp before a puff
    for i in range(4):  # Four frames each
        save(steam_puff(i), "Props/steam_%02d.png" % i)  # Big steam puff
        save(wisp(i), "Props/wisp_%02d.png" % i)  # Faint background wisp
        save(lava_crack(i), "Props/lava_crack_%02d.png" % i)  # Glowing lava crack
        save(eagle(i), "Props/eagle_%02d.png" % i)  # Eagle: perched, then flying
    save(fold_top(), "fold_top.png")  # Folded Peaks ground top
    save(fold_soil(), "fold_soil.png")  # Folded Peaks soil
    save(ledge_fold(), "ledge_fold.png")  # Folded Peaks ledge
    save(fold_boulder(32, 24), "boulder_small_fold.png")  # Small layered boulder
    save(fold_boulder(48, 36), "boulder_big_fold.png")  # Big layered boulder
    save(cairn(), "cairn.png")  # Summit stone pile
    save(snow_flake(2), "Props/snow_00.png")  # Small snowflake
    save(snow_flake(4), "Props/snow_01.png")  # Bigger snowflake
    save(banner_plate(), "banner_plate.png")  # Level-start title plate
    print("Level art written to", OUT)  # Tell the person running it where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
