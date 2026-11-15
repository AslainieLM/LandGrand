"""Draws the PNGs for the Lesson Screen (ticket 06) and the shared rock and mineral pictures.

Run from the Unity project folder:  python Tools/Art/make_lesson_art.py
It writes PNGs to Assets/Art/Lesson/Sprites/ (lesson picture, Start button, goal icons, item slot)
and Assets/Art/Items/Sprites/ (granite, quartz, feldspar and mica; later levels and the pop-up reuse them).
All colours come from Assets/Art/Palettes/landgrand.gpl.
"""
import math  # math gives the wave shape for the folded rock layers
import os  # os is used to make folders and join paths
import make_mainmenu_art as mm  # Main Menu palette, button and drawing helpers
import make_landform_map_art as lm  # Map colours, the Mountains region drawing, outline patterns and the star

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))  # The Unity project folder
LESSON_OUT = os.path.join(ROOT, "Assets", "Art", "Lesson", "Sprites")  # Where the Lesson Screen PNGs go
ITEMS_OUT = os.path.join(ROOT, "Assets", "Art", "Items", "Sprites")  # Where the shared item PNGs go
SUN_GLOW = (255, 225, 122)  # Warm light yellow from the palette, for the lit top of feldspar


LESSON_NEAR = [(22, 30, 1.0), (60, 12, 0.95), (100, 28, 1.05)]  # Three snowy peaks across the lesson picture: x, top y, slope
LESSON_FAR = [(40, 34, 0.6), (82, 30, 0.6), (116, 38, 0.6)]  # Far-away peaks between them


def lesson_pixel(x, y):
    """Colour of one pixel of the lesson picture: sky, far haze, three snowy peaks and green foothills (map style)."""
    foot = 66 + (1 if (x // 9) % 2 else 0)  # Where the foothills begin, with a small wave
    if y >= foot:  # Foothills at the bottom
        return mm.LEAF if y < foot + 2 else mm.HILLS_SH  # Dark grass edge, then grass
    top, peak = lm.ridge(x, LESSON_NEAR)  # The big mountains
    if y >= top:  # Inside a big mountain
        px, py, _ = peak  # The peak this pixel belongs to
        if y < py + 10 + (lm.noise(x, 0) % 3):  # In the ragged snow cap
            return lm.SNOW if x <= px else lm.HAZE  # Sunny snow on the left, shaded snow on the right
        return lm.MOUNTAIN if x <= px else lm.MOUNTAIN_SH  # Sunny rock on the left, shaded rock on the right
    far_top, far_peak = lm.ridge(x, LESSON_FAR)  # The far-away mountains
    if y >= far_top:  # Inside a far mountain
        return lm.HAZE if x <= far_peak[0] else lm.HAZE_SH  # Light on the left, shade on the right
    return lm.SKY if y < 8 else (lm.SKY, mm.SKY_LIGHT)[(x + y) % 2] if y < 10 else mm.SKY_LIGHT  # Deep sky, a checker step, then light sky


def lesson_picture():
    """A 120x82 framed picture of three snowy mountains above green foothills with pines."""
    w, h = 118, 80  # Size of the picture inside its frame
    img = mm.new_image(w + 2, h + 2)  # One extra pixel on each side for the frame
    mm.fill_rect(img, 0, 0, w + 2, h + 2, mm.OUTLINE)  # Dark frame
    for y in range(h):  # Each row of the picture
        for x in range(w):  # Each pixel of the row
            img.putpixel((x + 1, y + 1), lesson_pixel(x, y) + (255,))  # Paint the pixel inside the frame
    for x in range(8, w, 15):  # Pines along the foothills
        base = 76  # Bottom row of each pine
        for row in range(8):  # Pine gets wider toward the bottom
            mm.fill_rect(img, x + 1 - row // 2, base - 8 + row, 1 + (row // 2) * 2, 1, mm.LEAF)  # One row of the pine
        mm.fill_rect(img, x + 1, base, 1, 2, lm.HANDLE)  # Trunk
    return img


def framed(pixel):
    """Wraps a pixel function into a framed 120x82 lesson picture (same size and frame as the mountains picture)."""
    w, h = 118, 80  # Size of the picture inside its frame
    img = mm.new_image(w + 2, h + 2)  # One extra pixel on each side for the frame
    mm.fill_rect(img, 0, 0, w + 2, h + 2, mm.OUTLINE)  # Dark frame
    for y in range(h):  # Each row of the picture
        for x in range(w):  # Each pixel of the row
            img.putpixel((x + 1, y + 1), pixel(x, y) + (255,))  # Paint the pixel inside the frame
    return img


def disc(img, cx, cy, r, color):
    """Paints a filled round puff."""
    for y in range(cy - r, cy + r + 1):  # Rows of the puff
        for x in range(cx - r, cx + r + 1):  # Pixels of the row
            if (x - cx) ** 2 + (y - cy) ** 2 <= r * r + r:  # Inside the circle
                put(img, x, y, color)  # Paint it


VOLCANO_X, VOLCANO_TOP = 56, 26  # Middle column and top row of the volcano cone


def lava_middle(y):
    """Column of the middle of the lava stream at row y: it starts at the crater and runs down the right side."""
    return VOLCANO_X + 3 + int((y - VOLCANO_TOP) * 0.62) + (2 if (y // 5) % 2 else 0)  # Slants right with a small wiggle


def volcano_pixel(x, y):
    """Colour of one pixel of the volcano picture: sky, far haze, a cone with lava, and green foothills."""
    foot = 66 + (1 if (x // 9) % 2 else 0)  # Where the foothills begin, with a small wave
    if y >= foot:  # Foothills at the bottom
        return mm.LEAF if y < foot + 2 else mm.HILLS_SH  # Dark grass edge, then grass
    half = 5 + (y - VOLCANO_TOP) * 0.95 + (y - VOLCANO_TOP) ** 2 * 0.004  # Half-width of the cone, a little curved
    if y >= VOLCANO_TOP and abs(x - VOLCANO_X) <= half:  # Inside the cone
        if y < VOLCANO_TOP + 2 and abs(x - VOLCANO_X) < 4:  # Glowing crater mouth
            return mm.HELMET  # Hot yellow
        flow = x - lava_middle(y)  # Sideways distance from the lava stream
        if VOLCANO_TOP <= y < VOLCANO_TOP + 46 and abs(flow) <= 2 + (y - VOLCANO_TOP) // 9:  # Inside the lava stream
            if y < VOLCANO_TOP + 20:  # Upper part is still hot
                return mm.HELMET if abs(flow) <= 1 else mm.SCARF  # Bright middle, orange edges
            if y < VOLCANO_TOP + 30:  # Cooling part
                return mm.SCARF_SH if (x + y) % 2 else mm.MASK_SH  # Red and grey mix
            return mm.STRAP if (x * 3 + y) % 5 else mm.MASK_SH  # Cooled dark grey rock with grey specks
        if x == VOLCANO_X - int(half) or x == VOLCANO_X + int(half):  # Sloping edge of the cone
            return mm.OUTLINE  # Dark outline
        return mm.GROUND_SH if x <= VOLCANO_X else mm.BACKPACK_SH  # Sunny brown left, shaded right
    far_top, far_peak = lm.ridge(x, [(10, 40, 0.6), (100, 36, 0.6), (116, 44, 0.6)])  # The far-away mountains
    if y >= far_top:  # Inside a far mountain
        return lm.HAZE if x <= far_peak[0] else lm.HAZE_SH  # Light on the left, shade on the right
    return lm.SKY if y < 8 else (lm.SKY, mm.SKY_LIGHT)[(x + y) % 2] if y < 10 else mm.SKY_LIGHT  # Deep sky, a checker step, then light sky


def lesson_volcano():
    """A 120x82 framed picture of a cone volcano with a steam plume and a lava flow that cools into dark rock."""
    img = framed(volcano_pixel)  # Sky, cone, lava and foothills
    for (cx, cy, r) in ((57, 21, 4), (60, 16, 5), (66, 12, 5), (73, 9, 4), (80, 7, 3)):  # Plume puffs, drifting right as they rise
        disc(img, cx + 1, cy + 1, r, mm.MASK)  # Grey shaded puff behind
        disc(img, cx, cy, r, mm.WHITE)  # White lit puff in front
    for (cx, cy) in ((60, 18), (67, 14), (74, 10)):  # Dark ash patches inside the plume
        put(img, cx + 1, cy + 1, mm.MASK_SH)  # Ash pixel
        put(img, cx + 2, cy + 1, mm.MASK)  # Lighter ash pixel
    for x in range(8, 118, 15):  # Pines along the foothills
        if abs(x - VOLCANO_X) < 12:  # Keep the front of the volcano clear
            continue  # Skip this pine
        base = 76  # Bottom row of each pine
        for row in range(8):  # Pine gets wider toward the bottom
            mm.fill_rect(img, x + 1 - row // 2, base - 8 + row, 1 + (row // 2) * 2, 1, mm.LEAF)  # One row of the pine
        mm.fill_rect(img, x + 1, base, 1, 2, lm.HANDLE)  # Trunk
    return img


FOLD_X, FOLD_TOP = 59, 14  # Middle column and top row of the folded mountain
FOLD_LIGHT = (mm.PLATEAU, mm.HELMET, lm.MOUNTAIN, mm.HILLS)  # Four rock layers, sunny side
FOLD_SHADE = (mm.PLATEAU_SH, mm.HELMET_SH, lm.MOUNTAIN_SH, mm.HILLS_SH)  # The same layers in shade


def folds_pixel(x, y):
    """Colour of one pixel of the folds picture: sky, a snowy mountain cut open to show bent rock layers, and grass."""
    foot = 66 + (1 if (x // 9) % 2 else 0)  # Where the grass begins, with a small wave
    if y >= foot:  # Grass at the bottom
        return mm.LEAF if y < foot + 2 else mm.HILLS_SH  # Dark grass edge, then grass
    top = FOLD_TOP + int(abs(x - FOLD_X) * 0.88) + lm.noise(x, 3) % 2  # Slope of the mountain on both sides
    if y >= top:  # Inside the mountain
        if y == top or (y == top + 1 and lm.noise(x, 5) < 40):  # Edge of the slope
            return mm.OUTLINE  # Dark outline
        if y < FOLD_TOP + 13 + lm.noise(x, 0) % 3:  # In the ragged snow cap
            return lm.SNOW if x <= FOLD_X else lm.HAZE  # Sunny snow on the left, shaded snow on the right
        wave = 8 * math.cos((x - FOLD_X) * 2 * math.pi / 44)  # Layers rise and fall like a wave
        s = y + wave + 80  # Position along the layers
        band = int(s // 7) % 4  # Which of the four layers
        palette = FOLD_LIGHT if x <= FOLD_X else FOLD_SHADE  # Sunny left, shaded right
        if int(s) % 7 == 0:  # The thin line between two layers
            return mm.OUTLINE if band % 2 else mm.BACKPACK_SH  # Dark line
        return palette[band]  # Colour of the layer
    return lm.SKY if y < 8 else (lm.SKY, mm.SKY_LIGHT)[(x + y) % 2] if y < 10 else mm.SKY_LIGHT  # Deep sky, a checker step, then light sky


def arrow(img, x, y, direction):
    """Draws a small orange push arrow; direction is 1 for pointing right and -1 for pointing left."""
    for i in range(14):  # Shaft of the arrow
        put(img, x + i * direction, y, mm.SCARF)  # Orange shaft pixel
        put(img, x + i * direction, y - 1, mm.SCARF)  # Second row, to make it thick
        put(img, x + i * direction, y + 1, mm.SCARF_SH)  # Shaded bottom edge
    for i in range(4):  # Arrow head: wider toward the back
        for dy in range(-i, i + 1):  # Rows of the head column
            put(img, x + (14 - i) * direction, y + dy, mm.SCARF)  # Head pixel


def lesson_folds():
    """A 120x82 framed picture of a snowy mountain cut open: rock layers bend into waves when pushed together."""
    img = framed(folds_pixel)  # Sky, mountain with bent layers and grass
    arrow(img, 6, 74, 1)  # Arrow pushing from the left
    arrow(img, 113, 74, -1)  # Arrow pushing from the right
    return img


def outline(img):
    """Adds a dark outline around the shape: every empty pixel that touches it turns dark."""
    shape = [(x, y) for y in range(img.height) for x in range(img.width) if img.getpixel((x, y))[3] > 0]  # Pixels of the shape
    for (x, y) in shape:  # Look around each shape pixel
        for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):  # The four neighbours
            nx, ny = x + dx, y + dy  # Neighbour position
            if 0 <= nx < img.width and 0 <= ny < img.height and img.getpixel((nx, ny))[3] == 0:  # Empty neighbour inside the image
                img.putpixel((nx, ny), mm.OUTLINE + (255,))  # Paint it dark
    return img


def put(img, x, y, color):
    """Paints one pixel, skipping pixels outside the image."""
    if 0 <= x < img.width and 0 <= y < img.height:  # Stay inside the image
        img.putpixel((x, y), color + (255,))  # Fully visible colour


def granite():
    """A 24x24 speckled grey boulder: light grey with black, white and clay specks."""
    img = mm.new_image(24, 24)  # Empty picture
    cx, cy, rx, ry = 11.5, 13.5, 10.0, 7.5  # Middle and half-sizes of the boulder
    for y in range(24):  # Each row
        for x in range(24):  # Each pixel
            d = ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2  # Distance from the middle, 1 at the edge
            if d > 1.0:  # Outside the boulder
                continue  # Leave it see-through
            shade = (x - cx) + (y - cy) * 1.4 > 6  # Lower right is in shade
            color = mm.MASK_SH if shade else mm.MASK  # Grey rock
            n = lm.noise(x, y)  # Fixed random number for this pixel
            if n < 9:  # Some pixels are black specks
                color = mm.STRAP  # Dark speck
            elif n < 15:  # Some are white specks
                color = mm.WHITE  # White speck
            elif n < 21:  # Some are clay specks (feldspar in the rock)
                color = lm.HANDLE if shade else mm.PLATEAU  # Clay speck
            put(img, x, y, color)  # Paint the pixel
    for (x, y) in ((7, 8), (8, 8), (6, 9)):  # A small shine on the top left
        put(img, x, y, mm.WHITE)  # Shine pixel
    return outline(img)  # Dark edge around the boulder


def quartz():
    """A 24x24 cluster of three clear crystal points on a grey base."""
    img = mm.new_image(24, 24)  # Empty picture
    for (x, top, w) in [(4, 9, 5), (9, 3, 6), (15, 7, 5)]:  # Each crystal: left edge, tip row, width
        mid = x + w // 2  # Column of the tip
        for y in range(top, 19):  # Rows from the tip down to the base
            for xx in range(x, x + w):  # Columns of the crystal
                if y < top + 3 and abs(xx - mid) > (y - top):  # Cut the pointed tip
                    continue  # Leave it see-through
                color = lm.SNOW if xx < mid else lm.HAZE  # Light left face, shaded right face
                if xx == x + 1 and y > top + 3:  # A thin shine line on the left face
                    color = mm.WHITE  # Shine
                put(img, xx, y, color)  # Paint the pixel
    mm.fill_rect(img, 3, 19, 18, 3, mm.MASK_SH)  # Grey rock base
    mm.fill_rect(img, 5, 19, 6, 1, mm.MASK)  # Light top edge of the base
    return outline(img)  # Dark edge around the cluster


def feldspar():
    """A 24x24 blocky clay-pink crystal seen from the front and a little above: lit top, front face, shaded side."""
    img = mm.new_image(24, 24)  # Empty picture
    for y in range(6, 10):  # Top face: four rows, each pushed one pixel further right going up
        shift = 10 - y  # 1 to 4 pixels to the right
        mm.fill_rect(img, 4 + shift, y, 13, 1, SUN_GLOW)  # One row of the lit top
    mm.fill_rect(img, 4, 10, 13, 10, mm.PLATEAU)  # Front face
    for x in range(17, 21):  # Side face: four columns, each pushed one pixel higher going right
        k = x - 16  # 1 to 4 pixels up
        mm.fill_rect(img, x, 10 - k, 1, 10, mm.PLATEAU_SH)  # One column of the shaded side
    mm.fill_rect(img, 6, 12, 1, 6, mm.HEADLAMP)  # Shine line on the front face
    mm.fill_rect(img, 9, 15, 6, 1, mm.PLATEAU_SH)  # Split line across the front, where feldspar breaks
    return outline(img)  # Dark edge around the crystal


def mica():
    """A 24x24 silvery mica book: thin sheets stacked unevenly, with a lifted top sheet and sparkles."""
    img = mm.new_image(24, 24)  # Empty picture
    for i, (x, w) in enumerate([(4, 16), (3, 17), (5, 15), (4, 16), (6, 13)]):  # Each sheet from the bottom up: left edge and width
        y = 18 - i * 2  # Row of this sheet; sheets touch, like pages
        mm.fill_rect(img, x, y, w, 2, lm.HAZE_SH if i % 2 else mm.MASK)  # The sheet, two shades of silver
        mm.fill_rect(img, x + 1, y, w - 2, 1, lm.SNOW)  # Bright top edge of the sheet
    for row in range(3):  # The top sheet peels up on its right side
        mm.fill_rect(img, 12 + row * 2, 9 - row, 6, 1, lm.SNOW if row == 2 else mm.MASK)  # One slanted row of the lifted sheet
    for (x, y) in ((7, 9), (8, 8), (9, 9), (8, 10)):  # A four-point sparkle on the stack
        put(img, x, y, mm.WHITE)  # Sparkle pixel
    return outline(img)  # Dark edge around the stack


def inside(px, py, pts):
    """True when the point is inside the polygon (ray test)."""
    hit = False  # Not inside until a ray crosses an edge
    j = len(pts) - 1  # Previous corner
    for i in range(len(pts)):  # Each corner
        xi, yi = pts[i]  # This corner
        xj, yj = pts[j]  # Previous corner
        if (yi > py) != (yj > py) and px < (xj - xi) * (py - yi) / (yj - yi) + xi:  # The ray crosses this edge
            hit = not hit  # Flip inside or outside
        j = i  # Move on
    return hit  # Answer


def fill_poly(img, pts, color):
    """Paints every pixel whose middle is inside the polygon."""
    for y in range(img.height):  # Each row
        for x in range(img.width):  # Each pixel
            if inside(x + 0.5, y + 0.5, pts):  # Pixel is inside the shape
                put(img, x, y, color)  # Paint it


def andesite():
    """A 24x24 angular dark-grey rock with very fine grain and a few small pale crystal flecks."""
    img = mm.new_image(24, 24)  # Empty picture
    cx, cy = 11.5, 13.5  # Middle of the rock
    for y in range(24):  # Each row
        for x in range(24):  # Each pixel
            d = abs((x - cx) / 10.5) ** 3 + abs((y - cy) / 7.5) ** 3  # Boxy shape: a flatter circle with soft corners
            if d > 1.0:  # Outside the rock
                continue  # Leave it see-through
            shade = (x - cx) * 0.7 + (y - cy) * 1.2 > 4  # Lower right is in shade
            color = mm.MASK_SH if shade else lm.HAMMER  # Mid grey rock, darker in shade
            if lm.noise(x, y) % 3 == 0:  # Fine grain: scattered slightly darker pixels
                color = lm.MOUNTAIN_SH if shade else mm.MASK_SH  # Slightly darker grain pixel
            if lm.noise(x, y) < 10:  # A few very dark grain dots
                color = mm.STRAP  # Dark dot
            put(img, x, y, color)  # Paint the pixel
    for (x, y) in ((9, 9), (14, 11), (6, 14), (16, 15), (11, 17)):  # Small pale crystal flecks
        put(img, x, y, mm.MASK)  # Light fleck
        put(img, x + 1, y, mm.WHITE)  # Bright end of the fleck
    for x in range(5, 17):  # A flat lit top edge, because the rock is angular
        put(img, x, 7, mm.MASK)  # Light edge pixel
    return outline(img)  # Dark edge around the rock


def pumice():
    """A 24x24 light cream lumpy stone full of small round holes."""
    img = mm.new_image(24, 24)  # Empty picture
    cx, cy = 11.5, 12.5  # Middle of the stone
    for y in range(24):  # Each row
        for x in range(24):  # Each pixel
            lump = 1.0 + 0.07 * math.sin(x * 0.9) + 0.07 * math.cos(y * 0.8 + 1)  # Soft lumpy edge
            d = ((x - cx) / (9.5 * lump)) ** 2 + ((y - cy) / (8.5 * lump)) ** 2  # Distance from the middle, 1 at the edge
            if d > 1.0:  # Outside the stone
                continue  # Leave it see-through
            right_low = (x - cx) * 0.8 + (y - cy) * 1.0  # Bigger means more to the lower right
            color = mm.HEADLAMP  # Cream stone
            if right_low > 4:  # Lower right is in shade
                color = mm.MASK  # Light grey shade
            if right_low > 10:  # Far lower right is deeper shade
                color = mm.MASK_SH  # Grey shade
            put(img, x, y, color)  # Paint the pixel
    for (x, y, big) in ((8, 8, 1), (13, 7, 0), (16, 11, 1), (6, 12, 0), (11, 12, 1), (9, 16, 0), (15, 16, 1), (12, 19, 0), (18, 14, 0), (7, 18, 0)):  # Holes: place and size
        put(img, x, y, mm.STRAP)  # Dark dot of the hole
        if big:  # Larger holes get two pixels
            put(img, x + 1, y, mm.STRAP)  # Deeper dark side
            put(img, x, y + 1, mm.MASK_SH)  # Dark bottom of the hole
            put(img, x, y - 1, mm.WHITE)  # Bright rim above the hole, light from above
    for (x, y) in ((9, 5), (10, 5), (8, 6)):  # A soft shine on the top
        put(img, x, y, mm.WHITE)  # Shine pixel
    return outline(img)  # Dark edge around the stone


def obsidian():
    """A 24x24 black glassy rock with sharp curved shell-like breaks and bright blue-white streaks."""
    img = mm.new_image(24, 24)  # Empty picture
    shape = [(3, 16), (5, 8), (10, 3), (16, 4), (21, 9), (20, 16), (15, 20), (8, 20)]  # Sharp corners of the rock
    fill_poly(img, shape, mm.OUTLINE)  # Black glass body
    for y in range(24):  # Each row
        for x in range(24):  # Each pixel
            if img.getpixel((x, y))[3] == 0:  # Not part of the rock
                continue  # Skip it
            r = ((x - 17) ** 2 + (y - 16) ** 2) ** 0.5  # Distance from the point where the glass was hit
            if int(r) % 5 in (3, 4):  # A ring of the shell-like break
                put(img, x, y, lm.MOUNTAIN_SH)  # Blue-grey ring line
            r2 = ((x - 7) ** 2 + (y - 12) ** 2) ** 0.5  # Distance from a second break point
            if int(r2) % 4 in (2, 3) and x < 13:  # A second set of rings on the left
                put(img, x, y, mm.STRAP)  # Grey-black ring line
    for (x, y) in ((11, 4), (12, 4), (13, 4), (14, 5), (15, 5), (16, 6), (17, 7), (18, 8)):  # Lit top right edge
        put(img, x, y, lm.MOUNTAIN)  # Blue edge pixel
    for (x, y) in ((7, 7), (8, 7), (8, 8), (9, 9), (9, 10), (10, 11), (10, 12)):  # Long streak, top left
        put(img, x, y, lm.LENS)  # Bright blue
    for (x, y) in ((7, 7), (8, 8), (9, 10)):  # Brightest streak pixels
        put(img, x, y, mm.WHITE)  # White shine
    for (x, y) in ((15, 8), (16, 8), (16, 9), (17, 10)):  # Short streak, right
        put(img, x, y, mm.SKY_LIGHT)  # Pale blue shine
    return outline(img)  # Dark edge around the glass


def slate():
    """A 24x24 dark blue-grey slab made of four thin smooth sheets, the stacked edge facing us."""
    img = mm.new_image(24, 24)  # Empty picture
    for y in range(6, 9):  # Top face: three rows, each pushed one pixel right going up
        shift = 9 - y  # 1 to 3 pixels to the right
        mm.fill_rect(img, 3 + shift, y, 14, 1, lm.HAZE_SH)  # One row of the lit top
    for k, (x, w) in enumerate([(3, 14), (3, 14), (4, 13), (3, 14)]):  # Each sheet from the top: left edge and width
        y = 9 + k * 3  # Top row of this sheet
        mm.fill_rect(img, x, y, w, 3, lm.MOUNTAIN_SH)  # Sheet face
        mm.fill_rect(img, x, y, w, 1, lm.MOUNTAIN)  # Lit top edge of the sheet
        mm.fill_rect(img, x, y + 2, w, 1, mm.STRAP)  # Dark gap under the sheet
        if k == 2:  # One sheet sticks out less, so the layers show
            put(img, x - 1, y + 1, mm.STRAP)  # Small notch shadow
    for x in range(17, 20):  # Side face: three columns, each pushed one pixel higher going right
        k = x - 16  # 1 to 3 pixels up
        mm.fill_rect(img, x, 9 - k, 1, 12, mm.STRAP)  # One column of the shaded side
        for gap in (11, 14, 17):  # Sheet gaps continue on the side
            put(img, x, gap - k, mm.OUTLINE)  # Dark gap pixel
    for x in range(6, 12):  # A soft smooth shine on the top sheet
        put(img, x, 9, lm.HAZE)  # Light line pixel
    return outline(img)  # Dark edge around the slab


def marble():
    """A 24x24 smooth white-cream stone with soft grey veins."""
    img = mm.new_image(24, 24)  # Empty picture
    cx, cy = 11.5, 12.5  # Middle of the stone
    for y in range(24):  # Each row
        for x in range(24):  # Each pixel
            d = abs((x - cx) / 9.5) ** 2.6 + abs((y - cy) / 8.5) ** 2.6  # Rounded square, like a polished block
            if d > 1.0:  # Outside the stone
                continue  # Leave it see-through
            light = (x - cx) * 0.9 - (y - cy) * 0.9  # Bigger means more toward the upper right
            color = mm.WHITE if light > 4 else mm.HEADLAMP if light > -3 else lm.SNOW  # Bright upper right, cream middle, cool lower left
            if light < -9:  # Lower left edge is in shade
                color = lm.HAZE  # Soft blue-grey shade
            put(img, x, y, color)  # Paint the pixel
    for (x, y) in ((5, 8), (6, 9), (7, 9), (8, 10), (9, 11), (10, 11), (11, 12), (12, 13), (12, 14), (13, 15), (14, 16), (15, 16), (16, 17)):  # Main vein from top left to bottom right
        put(img, x, y, mm.MASK_SH)  # Grey vein
        put(img, x + 1, y + 1, mm.MASK_SH if (x + y) % 3 == 0 else lm.HAZE_SH)  # Darker soft edge of the vein
    for (x, y) in ((13, 6), (14, 7), (15, 7), (16, 8), (17, 9)):  # A thin side vein
        put(img, x, y, mm.MASK)  # Light grey vein
    for (x, y) in ((6, 15), (7, 16), (8, 16), (9, 17)):  # A small vein lower left
        put(img, x, y, mm.MASK)  # Light grey vein
    return outline(img)  # Dark edge around the stone


def garnet():
    """A 24x24 pair of dark red faceted crystals (rhombic dodecahedron look) on a tiny grey rock."""
    img = mm.new_image(24, 24)  # Empty picture
    cx, cy = 11.5, 18.5  # Middle of the rock base
    for y in range(24):  # Each row
        for x in range(24):  # Each pixel
            d = ((x - cx) / 10.0) ** 2 + ((y - cy) / 3.5) ** 2  # Flat low oval for the base
            if d <= 1.0:  # Inside the base
                put(img, x, y, mm.MASK_SH if (x + y * 2) % 7 else mm.MASK)  # Grey rock with light flecks
    for (mx, my, s) in ((9, 11, 1.0), (16, 13, 0.7)):  # Two crystals: middle x, middle y, size
        top = [(mx, my - 6 * s), (mx - 5 * s, my - 3 * s), (mx, my), (mx + 5 * s, my - 3 * s)]  # Top face corners
        left = [(mx - 5 * s, my - 3 * s), (mx - 5 * s, my + 3 * s), (mx, my + 6 * s), (mx, my)]  # Left face corners
        right = [(mx + 5 * s, my - 3 * s), (mx + 5 * s, my + 3 * s), (mx, my + 6 * s), (mx, my)]  # Right face corners
        fill_poly(img, left, mm.BACKPACK_SH)  # Dark left face
        fill_poly(img, right, mm.SCARF_SH)  # Mid right face
        fill_poly(img, top, mm.SCARF)  # Bright top face, light from above
        put(img, round(mx + 1), round(my - 4 * s), mm.WHITE)  # Bright highlight on the top facet
        put(img, round(mx + 2), round(my - 3 * s), mm.HEADLAMP)  # Second highlight pixel
    return outline(img)  # Dark edge around the crystals


def item_slot():
    """A 30x30 frame that holds one item picture: a tan edge around tan paper."""
    img = mm.new_image(30, 30)  # Empty picture
    mm.rounded_box(img, 0, 0, 30, 30)  # Dark outline
    mm.fill_rect(img, 1, 1, 28, 28, mm.VEST_SH)  # Darker tan edge
    mm.fill_rect(img, 2, 2, 26, 26, mm.VEST)  # Tan paper
    return img


CLOCK = [  # o = rim, f = face, h = hands
    "..ooooo..",
    ".offhffo.",
    "offfhfffo",
    "offfhfffo",
    "offfhhhfo",
    "offfffffo",
    ".offfffo.",
    "..ooooo..",
]


def clock():
    """An 11x10 clock icon for the time goal."""
    return lm.pattern(CLOCK, {"o": mm.HELMET_SH, "f": mm.HEADLAMP, "h": mm.OUTLINE})  # Outlined clock


def save(img, folder, name):
    """Saves an image in the given folder."""
    os.makedirs(folder, exist_ok=True)  # Make the folder if it is missing
    img.save(os.path.join(folder, name))  # Write the PNG


def main():
    """Draws everything."""
    green = (mm.HILLS, mm.HILLS_SH, mm.LEAF)  # Same colours as the Play button
    save(lesson_picture(), LESSON_OUT, "lesson_mountains.png")  # Lesson picture for Mountains
    save(lesson_volcano(), LESSON_OUT, "lesson_volcano.png")  # Lesson picture for volcanoes
    save(lesson_folds(), LESSON_OUT, "lesson_folds.png")  # Lesson picture for folded mountains
    save(mm.button(120, 26, "START", *green, pressed=False, scale=2), LESSON_OUT, "btn_start.png")  # Start button
    save(mm.button(120, 26, "START", *green, pressed=True, scale=2), LESSON_OUT, "btn_start_pressed.png")  # Start button, pressed
    save(lm.star(mm.HELMET, mm.HELMET_SH, mm.HEADLAMP), LESSON_OUT, "icon_star.png")  # Star icon for the collect goal
    save(clock(), LESSON_OUT, "icon_clock.png")  # Clock icon for the time goal
    save(item_slot(), LESSON_OUT, "item_slot.png")  # Frame behind each item picture
    save(granite(), ITEMS_OUT, "granite.png")  # Granite
    save(quartz(), ITEMS_OUT, "quartz.png")  # Quartz
    save(feldspar(), ITEMS_OUT, "feldspar.png")  # Feldspar
    save(mica(), ITEMS_OUT, "mica.png")  # Mica
    save(andesite(), ITEMS_OUT, "andesite.png")  # Andesite
    save(pumice(), ITEMS_OUT, "pumice.png")  # Pumice
    save(obsidian(), ITEMS_OUT, "obsidian.png")  # Obsidian
    save(slate(), ITEMS_OUT, "slate.png")  # Slate
    save(marble(), ITEMS_OUT, "marble.png")  # Marble
    save(garnet(), ITEMS_OUT, "garnet.png")  # Garnet
    print("Lesson art written to", LESSON_OUT, "and", ITEMS_OUT)  # Tell the person running it where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
