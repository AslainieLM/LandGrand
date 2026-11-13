"""Draws the PNGs for the Lesson Screen (ticket 06) and the shared rock and mineral pictures.

Run from the Unity project folder:  python Tools/Art/make_lesson_art.py
It writes PNGs to Assets/Art/Lesson/Sprites/ (lesson picture, Start button, goal icons, item slot)
and Assets/Art/Items/Sprites/ (granite, quartz, feldspar and mica; later levels and the pop-up reuse them).
All colours come from Assets/Art/Palettes/landgrand.gpl.
"""
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
    save(mm.button(120, 26, "START", *green, pressed=False, scale=2), LESSON_OUT, "btn_start.png")  # Start button
    save(mm.button(120, 26, "START", *green, pressed=True, scale=2), LESSON_OUT, "btn_start_pressed.png")  # Start button, pressed
    save(lm.star(mm.HELMET, mm.HELMET_SH, mm.HEADLAMP), LESSON_OUT, "icon_star.png")  # Star icon for the collect goal
    save(clock(), LESSON_OUT, "icon_clock.png")  # Clock icon for the time goal
    save(item_slot(), LESSON_OUT, "item_slot.png")  # Frame behind each item picture
    save(granite(), ITEMS_OUT, "granite.png")  # Granite
    save(quartz(), ITEMS_OUT, "quartz.png")  # Quartz
    save(feldspar(), ITEMS_OUT, "feldspar.png")  # Feldspar
    save(mica(), ITEMS_OUT, "mica.png")  # Mica
    print("Lesson art written to", LESSON_OUT, "and", ITEMS_OUT)  # Tell the person running it where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
