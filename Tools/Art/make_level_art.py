"""Draws the PNGs for the Mountains Easy 1 level (ticket 07): ground and ledge tiles, boulders,
the cave entrance, the goal flag, the touch buttons and the Continue button.

Run from the Unity project folder:  python Tools/Art/make_level_art.py
It writes PNGs to Assets/Art/Level/Sprites/.
All colours come from Assets/Art/Palettes/landgrand.gpl.
"""
import os  # os is used to make folders and join paths
from PIL import Image  # Image is used to read and turn the explorer pictures
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
    save(mm.button(120, 26, "CONTINUE", *green, pressed=False, scale=2), "btn_continue.png")  # Continue button
    save(mm.button(120, 26, "CONTINUE", *green, pressed=True, scale=2), "btn_continue_pressed.png")  # Continue button, pressed
    save(checkpoint_flag(False), "checkpoint_off.png")  # Checkpoint not reached
    save(checkpoint_flag(True), "checkpoint_on.png")  # Checkpoint reached
    save(spikes(), "spikes.png")  # Spike trap tile
    save(pit_fill(), "pit_fill.png")  # Inside of the pit
    brown = (mm.PLATEAU, mm.PLATEAU_SH, mm.BACKPACK_SH)  # Same colours as the Back buttons
    save(mark_icon(True), "icon_tick.png")  # Goal met
    save(mark_icon(False), "icon_cross.png")  # Goal not met
    for pressed in (False, True):  # Normal and pressed pictures
        tag = "_pressed" if pressed else ""  # File name ending
        save(mm.button(110, 26, "REPLAY", *brown, pressed=pressed, scale=2), "btn_replay%s.png" % tag)  # Replay from the lesson
        save(mm.button(130, 26, "BACK TO MAP", *brown, pressed=pressed, scale=2), "btn_map%s.png" % tag)  # Back to the Landform Map
    for pressed in (False, True):  # Normal and pressed pictures
        tag = "_pressed" if pressed else ""  # File name ending
        save(pause_icon(pressed), "btn_pause%s.png" % tag)  # Pause button
        save(mm.button(120, 26, "RESUME", *green, pressed=pressed, scale=2), "btn_resume%s.png" % tag)  # Resume
        save(mm.button(120, 26, "RETRY", *brown, pressed=pressed, scale=2), "btn_retry%s.png" % tag)  # Retry
        save(mm.button(120, 26, "MAIN MENU", *brown, pressed=pressed, scale=2), "btn_menu%s.png" % tag)  # Back to Main Menu
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
    print("Level art written to", OUT)  # Tell the person running it where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
