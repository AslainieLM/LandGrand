"""Draws the PNGs for the Mountains Easy 1 level (ticket 07): ground and ledge tiles, boulders,
the cave entrance, the goal flag, the touch buttons and the Continue button.

Run from the Unity project folder:  python Tools/Art/make_level_art.py
It writes PNGs to Assets/Art/Level/Sprites/.
All colours come from Assets/Art/Palettes/landgrand.gpl.
"""
import os  # os is used to make folders and join paths
import make_mainmenu_art as mm  # Main Menu palette, button and drawing helpers
import make_landform_map_art as lm  # Map colours and the noise helper

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


def save(img, name):
    """Saves an image under Assets/Art/Level/Sprites/."""
    os.makedirs(OUT, exist_ok=True)  # Make the folder if it is missing
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
    green = (mm.HILLS, mm.HILLS_SH, mm.LEAF)  # Same colours as the Play button
    save(mm.button(120, 26, "CONTINUE", *green, pressed=False, scale=2), "btn_continue.png")  # Continue button
    save(mm.button(120, 26, "CONTINUE", *green, pressed=True, scale=2), "btn_continue_pressed.png")  # Continue button, pressed
    save(mm.text_image("LEVEL CLEARED!", mm.DARK, 2), "text_cleared.png")  # Result heading
    print("Level art written to", OUT)  # Tell the person running it where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
