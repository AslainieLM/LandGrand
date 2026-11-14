"""Draws every Main Menu PNG and the button click sound for LandGrand.

Run from the Unity project folder:  python Tools/Art/make_mainmenu_art.py
It writes PNGs to Assets/Art/MainMenu/Sprites/ and Assets/Audio/sfx_ui_click.wav.
All colours come from Assets/Art/Palettes/landgrand.gpl.
"""
import math  # math is used for the clock circle and the click sound wave
import os  # os is used to make folders and join paths
import struct  # struct turns sound numbers into bytes for the WAV file
import wave  # wave writes the click sound file
from PIL import Image  # Pillow draws and saves the pixel-art images

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))  # The Unity project folder
OUT = os.path.join(ROOT, "Assets", "Art", "MainMenu", "Sprites")  # Where the menu PNGs go
START = os.path.join(ROOT, "Assets", "Art", "StartScreen", "Sprites")  # Start-screen art we reuse in pictures
AUDIO = os.path.join(ROOT, "Assets", "Audio")  # Where the click sound goes

# Palette colours (names match landgrand.gpl).
OUTLINE = (31, 26, 36)  # Darkest colour, for outlines and text
DARK = (59, 42, 34)  # Dark brown, for headings and text shadows
HELMET = (242, 193, 78)  # Yellow, for stars and the slider handle
HELMET_SH = (201, 143, 42)  # Yellow shade
HEADLAMP = (255, 244, 184)  # Cream, for the panel paper
STRAP = (58, 61, 69)  # Dark grey, for touch-button pictures
MASK = (201, 201, 192)  # Light grey
MASK_SH = (139, 139, 133)  # Grey, for the draft note
VEST = (194, 165, 107)  # Tan, for the panel frame
VEST_SH = (143, 118, 70)  # Tan shade
SCARF = (224, 96, 58)  # Orange-red, for the Exit and Yes buttons
SCARF_SH = (168, 63, 36)  # Orange-red shade
BACKPACK_SH = (94, 50, 28)  # Deep brown, for button bottoms
SKY_LIGHT = (191, 232, 247)  # Light sky, for tutorial picture backgrounds
WHITE = (255, 255, 255)  # Cloud white, for button labels
LEAF = (63, 122, 51)  # Dark green
PLATEAU = (201, 123, 74)  # Clay, for brown button tops
PLATEAU_SH = (154, 85, 50)  # Clay shade, for brown button faces
HILLS = (127, 191, 90)  # Green, for the Play button top and slider fill
HILLS_SH = (86, 143, 62)  # Green shade, for the Play button face
PLAINS = (181, 217, 106)  # Light green, for the top line of the slider fill
GROUND = (167, 119, 75)  # Brown ground
GROUND_SH = (122, 82, 51)  # Brown ground shade

# 5x7 pixel font. "#" is a lit pixel. Each glyph lists its rows from top to bottom.
FONT = {  # Letter shapes, looked up by character
    "A": [".###.", "#...#", "#...#", "#####", "#...#", "#...#", "#...#"],
    "B": ["####.", "#...#", "#...#", "####.", "#...#", "#...#", "####."],
    "C": [".###.", "#...#", "#....", "#....", "#....", "#...#", ".###."],
    "D": ["####.", "#...#", "#...#", "#...#", "#...#", "#...#", "####."],
    "E": ["#####", "#....", "#....", "####.", "#....", "#....", "#####"],
    "F": ["#####", "#....", "#....", "####.", "#....", "#....", "#...."],
    "G": [".###.", "#...#", "#....", "#.###", "#...#", "#...#", ".####"],
    "H": ["#...#", "#...#", "#...#", "#####", "#...#", "#...#", "#...#"],
    "I": ["###", ".#.", ".#.", ".#.", ".#.", ".#.", "###"],  # Narrow, so words with I keep even spacing
    "J": ["..###", "...#.", "...#.", "...#.", "...#.", "#..#.", ".##.."],
    "K": ["#...#", "#..#.", "#.#..", "##...", "#.#..", "#..#.", "#...#"],
    "L": ["#....", "#....", "#....", "#....", "#....", "#....", "#####"],
    "M": ["#...#", "##.##", "#.#.#", "#.#.#", "#...#", "#...#", "#...#"],
    "N": ["#...#", "#...#", "##..#", "#.#.#", "#..##", "#...#", "#...#"],
    "O": [".###.", "#...#", "#...#", "#...#", "#...#", "#...#", ".###."],
    "P": ["####.", "#...#", "#...#", "####.", "#....", "#....", "#...."],
    "Q": [".###.", "#...#", "#...#", "#...#", "#.#.#", "#..#.", ".##.#"],
    "R": ["####.", "#...#", "#...#", "####.", "#.#..", "#..#.", "#...#"],
    "S": [".####", "#....", "#....", ".###.", "....#", "....#", "####."],
    "T": ["#####", "..#..", "..#..", "..#..", "..#..", "..#..", "..#.."],
    "U": ["#...#", "#...#", "#...#", "#...#", "#...#", "#...#", ".###."],
    "V": ["#...#", "#...#", "#...#", "#...#", "#...#", ".#.#.", "..#.."],
    "W": ["#...#", "#...#", "#...#", "#.#.#", "#.#.#", "##.##", "#...#"],
    "X": ["#...#", "#...#", ".#.#.", "..#..", ".#.#.", "#...#", "#...#"],
    "Y": ["#...#", "#...#", ".#.#.", "..#..", "..#..", "..#..", "..#.."],
    "Z": ["#####", "....#", "...#.", "..#..", ".#...", "#....", "#####"],
    "0": [".###.", "#...#", "#..##", "#.#.#", "##..#", "#...#", ".###."],
    "1": [".#.", "##.", ".#.", ".#.", ".#.", ".#.", "###"],  # Narrow, so numbers with 1 keep even spacing
    "2": [".###.", "#...#", "....#", "...#.", "..#..", ".#...", "#####"],
    "3": ["####.", "....#", "....#", ".###.", "....#", "....#", "####."],
    "4": ["...#.", "..##.", ".#.#.", "#..#.", "#####", "...#.", "...#."],
    "5": ["#####", "#....", "####.", "....#", "....#", "#...#", ".###."],
    "6": [".###.", "#....", "#....", "####.", "#...#", "#...#", ".###."],
    "7": ["#####", "....#", "...#.", "..#..", ".#...", ".#...", ".#..."],
    "8": [".###.", "#...#", "#...#", ".###.", "#...#", "#...#", ".###."],
    "9": [".###.", "#...#", "#...#", ".####", "....#", "....#", ".###."],
    ".": [".", ".", ".", ".", ".", ".", "#"],
    ",": ["..", "..", "..", "..", "..", ".#", "#."],
    "!": ["#", "#", "#", "#", "#", ".", "#"],
    "?": [".###.", "#...#", "....#", "...#.", "..#..", ".....", "..#.."],
    "'": ["#", "#", ".", ".", ".", ".", "."],
    "-": ["...", "...", "...", "###", "...", "...", "..."],
    "/": ["....#", "...#.", "...#.", "..#..", ".#...", ".#...", "#...."],
    ":": [".", "#", ".", ".", ".", "#", "."],
    " ": ["...", "...", "...", "...", "...", "...", "..."],
}


def text_width(text, scale=1):
    """Returns how many pixels wide a line of text is."""
    widths = [len(FONT[c][0]) for c in text]  # Width of each letter in font pixels
    return (sum(widths) + max(0, len(text) - 1)) * scale  # Letters plus one gap between each


def fill_rect(img, x, y, w, h, color):
    """Fills a rectangle with one colour, skipping pixels outside the image."""
    for yy in range(y, y + h):  # Each row of the rectangle
        for xx in range(x, x + w):  # Each pixel of the row
            if 0 <= xx < img.width and 0 <= yy < img.height:  # Stay inside the image
                img.putpixel((xx, yy), color + (255,))  # Put the colour, fully visible


def draw_text(img, x, y, text, color, scale=1, shadow=None):
    """Draws one line of text with its top-left corner at (x, y)."""
    if shadow is not None:  # Draw the shadow first, so the text sits on top of it
        draw_text(img, x, y + 1, text, shadow, scale)  # Same text, one pixel lower, in the shadow colour
    for c in text:  # Go through each letter
        glyph = FONT[c]  # Get the letter's pixel rows
        for row, line in enumerate(glyph):  # Each row of the letter
            for col, ch in enumerate(line):  # Each pixel of the row
                if ch == "#":  # Only lit pixels are drawn
                    fill_rect(img, x + col * scale, y + row * scale, scale, scale, color)  # One font pixel becomes a scale x scale square
        x += (len(glyph[0]) + 1) * scale  # Move right past the letter and a 1-pixel gap


def new_image(w, h):
    """Makes an empty, fully see-through image."""
    return Image.new("RGBA", (w, h), (0, 0, 0, 0))


def text_image(lines, color, scale=1, line_gap=2):
    """Makes an image that holds one or more centred lines of text."""
    lines = lines if isinstance(lines, list) else [lines]  # Allow one line or many
    colors = color if isinstance(color, list) else [color] * len(lines)  # One colour per line
    w = max(text_width(t, scale) for t in lines)  # Widest line sets the image width
    line_h = 7 * scale  # Font is 7 pixels tall
    h = len(lines) * line_h + (len(lines) - 1) * line_gap  # Room for all lines
    img = new_image(w, h)  # Empty image of that size
    for i, t in enumerate(lines):  # Draw each line
        x = (w - text_width(t, scale)) // 2  # Centre the line
        draw_text(img, x, i * (line_h + line_gap), t, colors[i], scale)  # Draw this line at its row
    return img


def rounded_box(img, x, y, w, h):
    """Draws a dark box with its four corner pixels cut off, so it looks rounded."""
    fill_rect(img, x + 1, y, w - 2, h, OUTLINE)  # Tall part of the box
    fill_rect(img, x, y + 1, w, h - 2, OUTLINE)  # Wide part of the box


def button(w, h, label, top, face, bottom, pressed, scale):
    """Draws a rounded pixel button with its label baked in."""
    img = new_image(w, h)  # Empty button image
    rounded_box(img, 0, 0, w, h)  # Dark outline
    fill_rect(img, 1, 1, w - 2, h - 2, face)  # Main face colour
    if pressed:  # Pressed: the button looks pushed in
        fill_rect(img, 1, 1, w - 2, 2, bottom)  # Dark band on top, like an inset
    else:  # Normal: the button looks raised
        fill_rect(img, 1, 1, w - 2, 2, top)  # Light band on top
        fill_rect(img, 1, h - 3, w - 2, 2, bottom)  # Dark band on the bottom
    tx = (w - text_width(label, scale)) // 2  # Centre the label across
    ty = (h - 7 * scale) // 2 + (1 if pressed else 0)  # Centre the letters in the face between the light and dark bands; drop 1 pixel when pressed
    draw_text(img, tx, ty, label, WHITE, scale, shadow=DARK)  # White label with a dark shadow
    return img


def panel():
    """Draws a small 24x24 panel that Unity stretches (9-slice) to any size."""
    img = new_image(24, 24)  # Empty 24x24 image
    rounded_box(img, 0, 0, 24, 24)  # Dark outline
    fill_rect(img, 1, 1, 22, 22, VEST)  # Tan frame
    fill_rect(img, 3, 3, 18, 18, VEST_SH)  # Darker inner edge of the frame
    fill_rect(img, 4, 4, 16, 16, HEADLAMP)  # Cream paper in the middle
    return img


def bar(fill, edge):
    """Draws a 16x8 slider bar that Unity stretches (9-slice) to any width."""
    img = new_image(16, 8)  # Empty bar image
    rounded_box(img, 0, 0, 16, 8)  # Dark outline
    fill_rect(img, 1, 1, 14, 6, fill)  # Bar colour
    fill_rect(img, 1, 1, 14, 1, edge)  # Thin top line for a little depth
    return img


def handle():
    """Draws the 10x16 slider knob the player drags."""
    img = new_image(10, 16)  # Empty knob image
    rounded_box(img, 0, 0, 10, 16)  # Dark outline
    fill_rect(img, 1, 1, 8, 14, HELMET)  # Yellow body
    fill_rect(img, 1, 13, 8, 2, HELMET_SH)  # Shade at the bottom
    fill_rect(img, 2, 2, 2, 5, HEADLAMP)  # Shine at the top-left
    return img


def load(path):
    """Opens one of the start-screen PNGs."""
    return Image.open(os.path.join(START, path)).convert("RGBA")


def paste(base, sprite, x, y):
    """Pastes a sprite onto a picture, keeping its see-through pixels."""
    base.alpha_composite(sprite, (x, y))  # Blend the sprite onto the picture at (x, y)


def outlined(rows, fill, scale=1):
    """Turns a '#' pattern into a sprite with a 1-pixel dark outline around it."""
    h, w = len(rows), len(rows[0])  # Pattern size
    img = new_image(w * scale + 2, h * scale + 2)  # One extra pixel on each side for the outline
    lit = set()  # Pixels that belong to the shape
    for r, line in enumerate(rows):  # Each pattern row
        for c, ch in enumerate(line):  # Each pattern pixel
            if ch == "#":  # Lit pattern pixel
                for dy in range(scale):  # Make it scale x scale big
                    for dx in range(scale):  # Each pixel across inside the big square
                        lit.add((c * scale + dx + 1, r * scale + dy + 1))  # Remember this pixel as part of the shape
    for (x, y) in lit:  # First pass: outline around every lit pixel
        for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):  # The four neighbours
            img.putpixel((x + dx, y + dy), OUTLINE + (255,))  # Paint the neighbour dark
    for (x, y) in lit:  # Second pass: fill the shape over the outline
        img.putpixel((x, y), fill + (255,))  # Paint the shape colour
    return img


def mono(rows, color):
    """Turns a '#' pattern into a one-colour sprite with no outline."""
    img = new_image(len(rows[0]), len(rows))  # Image the size of the pattern
    for r, line in enumerate(rows):  # Each row
        for c, ch in enumerate(line):  # Each pixel
            if ch == "#":  # Lit pattern pixel
                img.putpixel((c, r), color + (255,))  # Paint the lit pixel
    return img


STAR = ["....#....", "...###...", "#########", ".#######.", "..#####..", ".###.###.", ".##...##.", "#.......#"]  # Star pattern
ARROW_RIGHT = ["#...", "##..", "###.", "####", "###.", "##..", "#..."]  # Right arrow pattern
ARROW_UP = ["...#...", "..###..", ".#####.", "#######", "..###..", "..###.."]  # Up arrow pattern
BACK_ARROW = ["...#.....", "..##.....", ".########", "#########", ".########", "..##.....", "...#....."]  # Arrow with a tail, for Back (same as the map's Back icon)
PENCIL = ["....##", "...###", "..###.", ".###..", "###...", "##....", "#....."]  # Pencil, tip at the bottom left, for Edit name
PAGE_ARROW = ["#....", "##...", "###..", "####.", "#####", "####.", "###..", "##...", "#...."]  # Solid triangle, for Next (flipped for Prev)


def icon_button(rows, pressed):
    """A 24x20 brown button with a white arrow and no words, matching the map's Back icon."""
    w, h = 24, 20  # Button size in pixels
    img = new_image(w, h)  # Empty button image
    rounded_box(img, 0, 0, w, h)  # Dark outline
    fill_rect(img, 1, 1, w - 2, h - 2, PLATEAU_SH)  # Main face colour
    if pressed:  # Pressed: the button looks pushed in
        fill_rect(img, 1, 1, w - 2, 2, BACKPACK_SH)  # Dark band on top, like an inset
    else:  # Normal: the button looks raised
        fill_rect(img, 1, 1, w - 2, 2, PLATEAU)  # Light band on top
        fill_rect(img, 1, h - 3, w - 2, 2, BACKPACK_SH)  # Dark band on the bottom
    arrow = mono(rows, WHITE)  # White arrow picture
    x = (w - arrow.width) // 2  # Centre the arrow across
    y = (h - arrow.height) // 2 + (1 if pressed else 0)  # Centre it up and down; drop 1 pixel when pressed
    paste(img, mono(rows, DARK), x, y + 1)  # Dark shadow one pixel lower
    paste(img, arrow, x, y)  # The arrow on top
    return img


def portrait(pressed, size=32):
    """A round explorer portrait for the Profile button (D-052): the start-screen explorer's head at 2x inside a brown ring."""
    src = Image.open(os.path.join(ROOT, "Assets", "Art", "StartScreen", "Sprites", "Explorer", "explorer_00.png")).convert("RGBA")  # The explorer, standing
    head = src.crop((11, 2, 25, 16)).resize((28, 28), Image.NEAREST)  # Helmet, face and scarf, doubled so the face reads at a glance
    img = new_image(size, size)  # Empty portrait
    c = (size - 1) / 2  # Middle of the circle
    ring = BACKPACK_SH if pressed else PLATEAU_SH  # Darker ring while pressed
    for y in range(size):  # Each row
        for x in range(size):  # Each pixel in the row
            d = ((x - c) ** 2 + (y - c) ** 2) ** 0.5  # Distance from the middle
            if d <= c - 3:  # Inside: light sky behind the head
                img.putpixel((x, y), SKY_LIGHT + (255,))  # Sky colour
            elif d <= c - 1:  # Two-pixel brown ring
                img.putpixel((x, y), ring + (255,))  # Ring colour
            elif d <= c + 0.5:  # One-pixel dark outline
                img.putpixel((x, y), OUTLINE + (255,))  # Outline colour
    off = (size - 28) // 2 + (1 if pressed else 0)  # Centre the head; drop 1 pixel when pressed
    for y in range(28):  # Each head row
        for x in range(28):  # Each head pixel
            px = head.getpixel((x, y))  # Head colour here
            tx, ty = x + (size - 28) // 2, y + off + 3  # Where it goes; 3 pixels lower so the shoulders meet the ring
            if px[3] == 0 or ty >= size:  # See-through or below the picture
                continue  # Skip it
            if ((tx - c) ** 2 + (ty - c) ** 2) ** 0.5 <= c - 3:  # Only inside the sky circle
                img.putpixel((tx, ty), px)  # Paint the head pixel
    return img


def save_portrait():
    """Saves the explorer portrait button for the Main Menu and the Profile (D-052)."""
    save(portrait(False), "Buttons", "btn_profile.png")  # Normal picture
    save(portrait(True), "Buttons", "btn_profile_pressed.png")  # Pressed picture


def save_icon_buttons():
    """Saves the Back, Prev, Next and Edit (pencil) icon buttons used on the Tutorial and Settings panels."""
    flipped = [r[::-1] for r in PAGE_ARROW]  # Left-pointing triangle for Prev
    for name, rows in [("back_icon", BACK_ARROW), ("prev_icon", flipped), ("next_icon", PAGE_ARROW), ("edit_icon", PENCIL)]:  # Name and arrow of each icon button
        save(icon_button(rows, False), "Buttons", "btn_%s.png" % name)  # Normal picture
        save(icon_button(rows, True), "Buttons", "btn_%s_pressed.png" % name)  # Pressed picture


def picture_base():
    """Makes a 200x80 tutorial picture with sky, a frame and grassy ground."""
    img = new_image(200, 80)  # Empty picture
    fill_rect(img, 0, 0, 200, 80, OUTLINE)  # Dark frame
    fill_rect(img, 1, 1, 198, 78, SKY_LIGHT)  # Light sky inside the frame
    fill_rect(img, 1, 62, 198, 17, GROUND)  # Brown ground at the bottom
    fill_rect(img, 1, 62, 198, 2, HILLS)  # Green grass edge on top of the ground
    fill_rect(img, 1, 64, 198, 1, HILLS_SH)  # Grass shade under the edge
    return img


def touch_button(sprite, color):
    """Draws a 20x20 on-screen touch button with a picture in the middle."""
    img = new_image(20, 20)  # Empty button
    rounded_box(img, 0, 0, 20, 20)  # Dark outline
    fill_rect(img, 1, 1, 18, 18, color)  # Button colour
    paste(img, sprite, (20 - sprite.width) // 2, (20 - sprite.height) // 2)  # Centre the picture
    return img


def tutorial_pictures():
    """Draws the four tutorial page pictures."""
    walk = load("Explorer/explorer_00.png")  # Explorer standing in the walk cycle
    walk_b = load("Explorer/explorer_04.png")  # Explorer mid-step
    hold_up = load("Explorer/explorer_13.png")  # Explorer holding a mineral up
    minerals = [load("Minerals/mineral_%02d.png" % i) for i in range(4)]  # Quartz, gold, jasper, grey rock
    feet_y = 62 - 30  # Puts the explorer's feet on the grass line
    pics = []  # The finished pictures

    # Page 1: move and jump.
    img = picture_base()  # Sky and ground
    paste(img, walk, 84, feet_y)  # Explorer in the middle
    for x, y in [(116, 34), (122, 26), (130, 22), (138, 24), (144, 32)]:  # Dots that show a jump arc
        fill_rect(img, x, y, 2, 2, WHITE)  # A 2x2 white dot
    right = mono(ARROW_RIGHT, WHITE)  # Right arrow picture
    left = right.transpose(Image.FLIP_LEFT_RIGHT)  # Left arrow is the right arrow flipped
    paste(img, touch_button(left, STRAP), 6, 56)  # Left touch button, bottom-left
    paste(img, touch_button(right, STRAP), 30, 56)  # Right touch button next to it
    paste(img, touch_button(mono(ARROW_UP, WHITE), HELMET_SH), 174, 56)  # Jump button, bottom-right
    pics.append(img)  # Page 1 is done

    # Page 2: collect rocks and minerals.
    img = picture_base()  # Sky and ground
    paste(img, walk_b, 30, feet_y)  # Explorer walking
    paste(img, minerals[0], 70, 51)  # Quartz on the ground
    paste(img, minerals[2], 94, 51)  # Jasper on the ground
    fill_rect(img, 116, 8, 76, 40, OUTLINE)  # Pop-up box outline
    fill_rect(img, 117, 9, 74, 38, HEADLAMP)  # Pop-up box paper
    paste(img, minerals[0], 121, 13)  # Quartz picture inside the pop-up
    draw_text(img, 137, 15, "QUARTZ", DARK)  # Mineral name inside the pop-up
    for i in range(3):  # Grey lines stand for the short description
        fill_rect(img, 121, 29 + i * 5, 66 - i * 14, 2, MASK_SH)  # Each line a bit shorter than the one above
    pics.append(img)  # Page 2 is done

    # Page 3: earn stars.
    img = picture_base()  # Sky and ground
    paste(img, hold_up, 30, feet_y)  # Explorer holding up a find
    star = outlined(STAR, HELMET, 2)  # One big yellow star
    for i in range(3):  # Three stars in a row
        paste(img, star, 80 + i * 26, 18 - (6 if i == 1 else 0))  # The middle star sits a bit higher
    pics.append(img)  # Page 3 is done

    # Page 4: watch out for pits, traps and the timer.
    img = picture_base()  # Sky and ground
    paste(img, walk_b, 20, feet_y)  # Explorer walking toward danger
    fill_rect(img, 64, 62, 26, 17, OUTLINE)  # Dark pit in the ground
    fill_rect(img, 63, 62, 1, 17, GROUND_SH)  # Left pit wall
    fill_rect(img, 90, 62, 1, 17, GROUND_SH)  # Right pit wall
    for i in range(4):  # A row of grey spikes
        x = 110 + i * 8  # Left edge of this spike
        for row in range(7):  # The spike gets wider toward the bottom
            half = row // 2  # Half the width at this row
            fill_rect(img, x + 3 - half, 55 + row, 1 + half * 2, 1, MASK if row < 5 else MASK_SH)  # One row of the spike; the bottom rows are darker
    cx, cy, r = 176, 20, 11  # Clock centre and size
    for yy in range(cy - r, cy + r + 1):  # Each row around the clock
        for xx in range(cx - r, cx + r + 1):  # Each pixel around the clock
            d = (xx - cx) ** 2 + (yy - cy) ** 2  # Squared distance from the centre
            if d <= r * r:  # Inside the clock
                fill_rect(img, xx, yy, 1, 1, WHITE if d < (r - 1) ** 2 else OUTLINE)  # White face, dark rim
    fill_rect(img, cx, cy - 7, 1, 7, OUTLINE)  # Minute hand pointing up
    fill_rect(img, cx, cy, 5, 1, SCARF_SH)  # Hour hand pointing right
    pics.append(img)  # Page 4 is done
    return pics


def save(img, *parts):
    """Saves an image under Assets/Art/MainMenu/Sprites/."""
    path = os.path.join(OUT, *parts)  # Full file path
    os.makedirs(os.path.dirname(path), exist_ok=True)  # Make the folder if it is missing
    img.save(path)  # Write the PNG


def click_sound():
    """Writes a short, soft 'tick' sound for button presses."""
    rate = 44100  # Samples per second
    length = int(rate * 0.07)  # 70 milliseconds long
    frames = bytearray()  # The sound data
    for i in range(length):  # Make each sample
        t = i / rate  # Time in seconds
        freq = 1046 if t < 0.025 else 1568  # Two quick notes: C6 then G6
        env = math.exp(-t * 60) * min(1.0, i / 40)  # Fade out fast; tiny fade in stops a pop
        s = math.sin(2 * math.pi * freq * t) * 0.6 + math.sin(4 * math.pi * freq * t) * 0.15  # Note plus a soft overtone
        frames += struct.pack("<h", int(max(-1, min(1, s * env)) * 32767 * 0.8))  # 16-bit sample
    with wave.open(os.path.join(AUDIO, "sfx_ui_click.wav"), "wb") as w:  # Open the WAV file for writing
        w.setnchannels(1)  # Mono
        w.setsampwidth(2)  # 16-bit
        w.setframerate(rate)  # 44.1 kHz
        w.writeframes(bytes(frames))  # Write the samples


def main():
    """Draws everything."""
    brown = (PLATEAU, PLATEAU_SH, BACKPACK_SH)  # Colours for normal buttons
    green = (HILLS, HILLS_SH, LEAF)  # Colours for the Play and No buttons
    red = (SCARF, SCARF_SH, BACKPACK_SH)  # Colours for the Exit and Yes buttons

    # Big main-menu buttons, 120x26, with 14-pixel-tall labels.
    for name, label, colors in [("play", "PLAY", green), ("settings", "SETTINGS", brown),  # Name, label and colours of each big button
                                ("tutorial", "TUTORIAL", brown), ("exit", "EXIT", red)]:  # (continued)
        save(button(120, 26, label, *colors, pressed=False, scale=2), "Buttons", "btn_%s.png" % name)  # Normal picture
        save(button(120, 26, label, *colors, pressed=True, scale=2), "Buttons", "btn_%s_pressed.png" % name)  # Pressed picture

    # Small panel buttons, 56x20, with 7-pixel-tall labels.
    for name, label, colors in [("back", "BACK", brown), ("prev", "PREV", brown), ("next", "NEXT", brown),  # Name, label and colours of each small button
                                ("yes", "YES", red), ("no", "NO", green), ("ok", "OK", green), ("save", "SAVE", green)]:  # (continued; OK confirms the typed name, SAVE keeps the settings)
        save(button(56, 20, label, *colors, pressed=False, scale=1), "Buttons", "btn_%s.png" % name)  # Normal picture
        save(button(56, 20, label, *colors, pressed=True, scale=1), "Buttons", "btn_%s_pressed.png" % name)  # Pressed picture

    save_icon_buttons()  # Back, Prev, Next and pencil icon buttons
    save_portrait()  # Round explorer portrait that opens the Profile (D-052)

    # Panel and slider parts.
    save(panel(), "Panels", "panel.png")  # Panel background
    save(bar(GROUND_SH, BACKPACK_SH), "Panels", "slider_track.png")  # Empty slider bar
    save(bar(HILLS, PLAINS), "Panels", "slider_fill.png")  # Filled part of the slider bar
    save(handle(), "Panels", "slider_handle.png")  # Slider knob

    # Headings and labels.
    save(text_image("SETTINGS", DARK, 2), "Text", "heading_settings.png")  # Settings heading
    save(text_image("TUTORIAL", DARK, 2), "Text", "heading_tutorial.png")  # Tutorial heading
    save(text_image("EXIT GAME?", DARK, 2), "Text", "heading_exit.png")  # Exit question
    save(text_image("LANDFORM MAP", DARK, 2), "Text", "heading_map.png")  # Coming Soon heading
    save(text_image(["COMING SOON!", "THE MAP IS NOT READY YET."], [PLATEAU_SH, OUTLINE]), "Text", "text_coming_soon.png")  # Coming Soon message
    save(text_image("MUSIC", OUTLINE), "Text", "label_music.png")  # Music slider label
    save(text_image("SOUND EFFECTS", OUTLINE), "Text", "label_sfx.png")  # Sound slider label
    save(text_image("DRAFT: GAME RULES ARE NOT FINAL YET", MASK_SH), "Text", "text_draft.png")  # Draft note on tutorial pages

    # Tutorial page text. The first line of each page is its title.
    pages = [  # Words for each tutorial page
        ["1/4  MOVE AND JUMP", "TAP THE ARROWS TO WALK LEFT OR RIGHT.", "TAP THE JUMP BUTTON TO JUMP.", "TAP AGAIN IN THE AIR TO DOUBLE JUMP."],
        ["2/4  COLLECT", "WALK INTO A ROCK OR MINERAL TO PICK IT UP.", "READ ITS NAME AND WHAT IT IS."],
        ["3/4  EARN STARS", "REACH THE END TO FINISH THE LEVEL.", "SEE WHAT YOU FOUND AND YOUR STARS."],
        ["4/4  WATCH OUT", "JUMP OVER PITS AND TRAPS.", "FINISH BEFORE THE TIMER RUNS OUT."],
    ]  # End of the page list
    for i, lines in enumerate(pages):  # One text image per page
        save(text_image(lines, [PLATEAU_SH] + [OUTLINE] * (len(lines) - 1)), "Tutorial", "page_%d_text.png" % (i + 1))  # Title line in clay, body lines dark
    for i, pic in enumerate(tutorial_pictures()):  # One picture per page
        save(pic, "Tutorial", "page_%d_picture.png" % (i + 1))  # Save the picture

    click_sound()  # The button sound
    print("Main Menu art written to", OUT)  # Tell the person running it where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
