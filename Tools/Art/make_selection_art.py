"""Draws the PNGs for the Difficulty Selection and Level Selection screens (ticket 05).

Run from the Unity project folder:  python Tools/Art/make_selection_art.py
It writes PNGs to Assets/Art/Selection/Sprites/.
All colours come from Assets/Art/Palettes/landgrand.gpl.
"""
import math  # math gives sin for the foothills
import os  # os is used to make folders and join paths
from PIL import Image  # Pillow draws and saves the pixel-art images
import make_mainmenu_art as mm  # Main Menu palette, pixel font and drawing helpers
import make_landform_map_art as lm  # Map colours, mountain ridges, padlock and outline patterns

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))  # The Unity project folder
OUT = os.path.join(ROOT, "Assets", "Art", "Selection", "Sprites")  # Where the PNGs go
W, H = 480, 270  # Screen size in pixels

FAR_PEAKS = [(40, 120, 0.6), (150, 110, 0.7), (300, 118, 0.6), (430, 112, 0.65)]  # Far-away mountains across the whole screen
NEAR_PEAKS = [(100, 96, 1.0), (250, 70, 0.9), (400, 100, 1.05)]  # Big snowy mountains across the whole screen


def foot(x):
    """Row where the foothills begin, at column x."""
    return 204 + round(4 * math.sin(x / 17))  # A gentle wave


def mountains_pixel(x, y):
    """Colour of one pixel of the full-screen Mountains backdrop: sky, haze, snowy peaks and foothills."""
    f = foot(x)  # Where the foothills begin here
    if y >= f:  # Foothills at the bottom
        return mm.LEAF if y < f + 3 else mm.HILLS_SH  # Dark grass edge, then grass
    top, peak = lm.ridge(x, NEAR_PEAKS)  # The big mountains
    if y >= top:  # Inside a big mountain
        px, py, _ = peak  # The peak this pixel belongs to
        if y < py + 16 + (lm.noise(x, 0) % 3):  # In the ragged snow cap
            return lm.SNOW if x <= px else lm.HAZE  # Sunny snow on the left, shaded snow on the right
        return lm.MOUNTAIN if x <= px else lm.MOUNTAIN_SH  # Sunny rock on the left, shaded rock on the right
    far_top, far_peak = lm.ridge(x, FAR_PEAKS)  # The far-away mountains
    if y >= far_top:  # Inside a far mountain
        return lm.HAZE if x <= far_peak[0] else lm.HAZE_SH  # Light on the left, shade on the right
    return lm.sky(y)  # Everything else is sky


def backdrop():
    """The full-screen Mountains picture behind the cards and the trail."""
    img = Image.new("RGBA", (W, H))  # Full-screen picture
    for y in range(H):  # Each row
        for x in range(W):  # Each pixel in the row
            img.putpixel((x, y), mountains_pixel(x, y) + (255,))  # Its colour
    for x in range(10, W, 21):  # Pines along the foothills
        base = foot(x) + 8  # A little below the foothill edge
        for row in range(9):  # Pine gets wider toward the bottom
            mm.fill_rect(img, x - row // 2, base - 9 + row, 1 + (row // 2) * 2, 1, mm.LEAF)  # One row of the pine
        mm.fill_rect(img, x, base, 1, 2, lm.HANDLE)  # Trunk
    return img


def panel(frame, edge, paper):
    """A 24x24 card panel that Unity stretches (9-slice), in the given frame and paper colours."""
    img = mm.new_image(24, 24)  # Empty 24x24 image
    mm.rounded_box(img, 0, 0, 24, 24)  # Dark outline
    mm.fill_rect(img, 1, 1, 22, 22, frame)  # Frame
    mm.fill_rect(img, 3, 3, 18, 18, edge)  # Darker inner edge of the frame
    mm.fill_rect(img, 4, 4, 16, 16, paper)  # Paper in the middle
    return img


def stone(face, shade):
    """A 30x22 rounded trail stone: a face colour with a darker bottom and a light top edge."""
    img = mm.new_image(30, 22)  # Empty stone picture
    mm.fill_rect(img, 3, 0, 24, 22, mm.OUTLINE)  # Tall part of the outline
    mm.fill_rect(img, 1, 2, 28, 18, mm.OUTLINE)  # Wide part of the outline
    mm.fill_rect(img, 0, 4, 30, 14, mm.OUTLINE)  # Widest middle part of the outline
    mm.fill_rect(img, 4, 1, 22, 20, face)  # Tall part of the face
    mm.fill_rect(img, 2, 3, 26, 16, face)  # Wide part of the face
    mm.fill_rect(img, 1, 5, 28, 12, face)  # Widest middle part of the face
    mm.fill_rect(img, 2, 15, 26, 3, shade)  # Darker bottom
    mm.fill_rect(img, 4, 18, 22, 2, shade)  # Darker bottom, lower row
    mm.fill_rect(img, 6, 2, 10, 1, mm.WHITE if face == mm.MASK else face)  # Small shine on top of an open stone
    return img


def dot():
    """One 2x3 dash of the trail between stones: cream on top of a dark shadow."""
    img = mm.new_image(2, 3)  # Empty dash picture
    mm.fill_rect(img, 0, 1, 2, 2, mm.DARK)  # Shadow
    mm.fill_rect(img, 0, 0, 2, 2, mm.HEADLAMP)  # Cream dash
    return img


def mix(a, b, t):
    """An even blend of two palette colours: t = 0 gives a, t = 1 gives b."""
    return tuple(int(round(a[i] + (b[i] - a[i]) * t)) for i in range(3))  # Blend red, green and blue


def picture(pixel):
    """A full-screen picture from a function that gives the colour of each pixel."""
    img = Image.new("RGBA", (W, H))  # Full-screen picture
    for y in range(H):  # Each row
        for x in range(W):  # Each pixel in the row
            img.putpixel((x, y), pixel(x, y) + (255,))  # Its colour
    return img


def tree(img, x, base, r, lit, shade):
    """A small round tree: a crown lit on the left like the hills, and a short trunk."""
    for dy in range(-r, r + 1):  # Rows of the crown
        for dx in range(-r, r + 1):  # Columns of the crown
            if dx * dx + dy * dy <= r * r and 0 <= x + dx < W:  # Inside the circle and the picture
                img.putpixel((x + dx, base - r - 2 + dy), (lit if dx <= 0 else shade) + (255,))  # Sunny left half
    mm.fill_rect(img, x, base - 2, 1, 2, lm.HANDLE)  # Trunk


HILL_GREEN = mix(mm.HILLS, mm.GROUND, 0.25)  # Grassy hills, a touch brown like the Chocolate Hills in the dry season
HILL_GREEN_SH = mix(mm.HILLS_SH, mm.GROUND_SH, 0.25)  # Their shaded side
HILL_FAR = mix(mm.HILLS, lm.HAZE, 0.55)  # Far hills, pale with distance
HILL_FAR_SH = mix(mm.HILLS_SH, lm.HAZE, 0.5)  # Far hills in shade
TREE_SH = mix(mm.LEAF, mm.OUTLINE, 0.3)  # Shaded half of the small trees
HILLS_NEAR = [(20, 166, 22), (84, 150, 28), (150, 170, 20), (204, 156, 26), (272, 168, 22), (330, 148, 30), (400, 164, 22), (458, 154, 26)]  # Rows of rounded cone hills: middle x, top row, half width
HILLS_FAR = [(50, 144, 26), (120, 138, 24), (176, 148, 22), (246, 136, 28), (300, 150, 20), (370, 140, 26), (440, 146, 24)]  # A paler row behind


def dome(x, hills):
    """Top row of a row of rounded hills at column x, and the hill that makes it."""
    best, owner = H, None  # Highest top so far and its hill
    for hx, top, half in hills:  # Each hill
        d = abs(x - hx) / half  # 0 at the middle, 1 at the foot
        if d < 1.4:  # Near enough to this hill
            y = top + half * (1 - math.sqrt(max(0.0, 1 - min(1.0, d) ** 2))) + max(0.0, d - 1) * half  # A round, dome-shaped top
            if y < best:  # Higher than the others here
                best, owner = y, (hx, top, half)  # This hill owns the column
    return int(best), owner


def hills_pixel(x, y):
    """Colour of one pixel of the Hills backdrop: many rounded cone hills, like the Chocolate Hills of Bohol."""
    f = foot(x)  # Where the foreground grass begins
    if y >= f:  # Foreground
        return mm.LEAF if y < f + 3 else mm.HILLS_SH  # Grass edge, then grass
    top, hill = dome(x, HILLS_NEAR)  # The near row of hills
    if hill and y >= top:  # Inside a near hill
        return HILL_GREEN if x <= hill[0] else HILL_GREEN_SH  # Sunny left, shaded right
    far_top, far_hill = dome(x, HILLS_FAR)  # The far row
    if far_hill and y >= far_top:  # Inside a far hill
        return HILL_FAR if x <= far_hill[0] else HILL_FAR_SH  # Light on the left, shade on the right
    if y >= 170:  # Low ground between the hills
        return mix(mm.HILLS, lm.HAZE, 0.4)  # Pale green valley
    return lm.sky(y)  # Sky


def hills_backdrop():
    """The full-screen Hills picture behind the cards, the trail and the lesson."""
    img = picture(hills_pixel)  # Hills and sky
    for x in range(14, W, 37):  # Trees along the foreground
        tree(img, x, foot(x) + 8, 4, mm.LEAF, TREE_SH)  # A round tree
    return img


PLAINS_FIELDS = [mm.PLAINS, lm.PLAINS_SH, mix(mm.PLAINS, mm.HELMET, 0.35), mm.HILLS]  # Rice paddies: young green, deeper green, ripening gold, fresh green
PLAINS_DIKE = mix(mm.GROUND, mm.PLAINS, 0.35)  # Low earth walls between the paddies
PLAINS_FAR = mix(lm.MOUNTAIN, lm.HAZE, 0.55)  # Low blue mountains on the far horizon
HORIZON = 150  # Row where the flat land meets the sky


def plains_pixel(x, y):
    """Colour of one pixel of the Plains backdrop: wide flat rice fields to a far, low horizon."""
    f = foot(x)  # Where the foreground grass begins
    if y >= f:  # Foreground
        return mm.LEAF if y < f + 3 else mm.HILLS_SH  # Grass edge, then grass
    if y >= HORIZON:  # The flat fields
        depth = (y - HORIZON) / (f - HORIZON)  # 0 at the horizon, 1 near the viewer
        row = int((depth ** 0.6) * 9)  # Field rows get taller closer to the viewer
        edges = [HORIZON + int(((k / 9) ** (1 / 0.6)) * (f - HORIZON)) for k in range(10)]  # Rows where the dikes run across
        if y in edges:  # A dike running across
            return PLAINS_DIKE  # Earth wall
        across = (x + row * 53) // int(70 + depth * 90)  # Which paddy across; paddies get wider closer to the viewer
        if (x + row * 53) % int(70 + depth * 90) == 0:  # A short dike between two paddies
            return PLAINS_DIKE  # Earth wall
        return PLAINS_FIELDS[(row * 3 + across) % len(PLAINS_FIELDS)]  # The paddy colour
    hill = HORIZON - 10 - int(8 * math.sin(x / 41) + 5 * math.sin(x / 13 + 1))  # Low far mountains
    if y >= hill:  # Inside them
        return PLAINS_FAR  # Hazy blue
    return lm.sky(y)  # Sky


def hut(img, x, base):
    """A small nipa hut on stilts: a thatched roof over a tan room."""
    mm.fill_rect(img, x - 6, base - 9, 13, 5, mm.VEST)  # The room
    mm.fill_rect(img, x - 5, base - 4, 1, 4, lm.HANDLE)  # Left stilt
    mm.fill_rect(img, x + 5, base - 4, 1, 4, lm.HANDLE)  # Right stilt
    for row in range(6):  # The thatched roof, wider at the bottom
        mm.fill_rect(img, x - row - 2, base - 15 + row, 5 + row * 2, 1, mm.GROUND_SH if row % 2 else mm.GROUND)  # One roof row


def plains_backdrop():
    """The full-screen Plains picture behind the cards, the trail and the lesson."""
    img = picture(plains_pixel)  # Fields and sky
    hut(img, 360, 178)  # A nipa hut out in the fields
    for x, r in ((90, 5), (104, 4), (330, 4), (420, 5)):  # Trees standing in the fields
        tree(img, x, 168 + r, r, mm.LEAF, TREE_SH)  # A round tree
    for x in range(20, W, 47):  # Trees along the foreground
        tree(img, x, foot(x) + 8, 4, mm.LEAF, TREE_SH)  # A round tree
    return img


MESA_TOP = 104  # Row of the flat top of the big plateau
MESA_LAYERS = [mm.PLATEAU, mix(mm.PLATEAU, mm.VEST, 0.4), mm.PLATEAU_SH, mix(mm.PLATEAU, mm.GROUND, 0.5)]  # Rock layers in the cliff
MESA_GRASS = mix(mm.HILLS, mm.PLAINS, 0.4)  # Grass on the flat top
MESA_FAR = mix(mm.PLATEAU, lm.HAZE, 0.6)  # A far plateau, pale with distance
MESA_FAR_SH = mix(mm.PLATEAU_SH, lm.HAZE, 0.55)  # Its shaded cliff


def plateaus_pixel(x, y):
    """Colour of one pixel of the Plateaus backdrop: a wide tableland with a flat grassy top and layered cliffs."""
    f = foot(x)  # Where the foreground grass begins
    if y >= f:  # Foreground
        return mm.LEAF if y < f + 3 else mm.HILLS_SH  # Grass edge, then grass
    down = max(0, y - MESA_TOP)  # Rows below the flat top
    left, right = 70 - down * 0.35, 390 + down * 0.35  # The cliffs spread out going down
    if left <= x <= right and y >= MESA_TOP + (lm.noise(x, 3) % 2):  # Inside the big plateau
        if y < MESA_TOP + 4:  # The flat top
            return MESA_GRASS  # Grass on top
        layer = (y - MESA_TOP - 4 + (lm.noise(x // 6, 5) % 2)) // 8  # Which rock layer, about 8 rows each
        c = MESA_LAYERS[layer % len(MESA_LAYERS)]  # Its colour
        if x > right - 18 - down * 0.2:  # The right end is in shade
            c = mix(c, mm.PLATEAU_SH, 0.6)  # Darker
        if (x * 7 + y) % 29 == 0:  # A few cracks
            c = mm.PLATEAU_SH  # Crack pixel
        return c
    far_left, far_right = 330 - max(0, y - 128) * 0.4, 470 + max(0, y - 128) * 0.4  # A smaller plateau behind, on the right
    if y >= 128 and far_left <= x <= far_right:  # Inside it
        if y < 131:  # Its flat top
            return mix(MESA_GRASS, lm.HAZE, 0.5)  # Pale grass
        c = MESA_FAR if ((y - 131) // 8) % 2 == 0 else mix(MESA_FAR, MESA_FAR_SH, 0.5)  # Pale rock layers
        return mix(c, MESA_FAR_SH, 0.6) if x > 452 else c  # Shaded right end
    if y >= 176:  # Low ground around the plateaus
        return mix(mm.HILLS, lm.HAZE, 0.4)  # Pale green lowland
    return lm.sky(y)  # Sky


def plateaus_backdrop():
    """The full-screen Plateaus picture behind the cards, the trail and the lesson."""
    img = picture(plateaus_pixel)  # Plateau and sky
    for x in range(90, 380, 23):  # Small trees along the flat top
        tree(img, x, MESA_TOP + 1, 2, mm.LEAF, TREE_SH)  # A tiny far tree
    for x in range(10, W, 41):  # Trees along the foreground
        tree(img, x, foot(x) + 8, 4, mm.LEAF, TREE_SH)  # A round tree
    return img


def save(img, *parts):
    """Saves an image under Assets/Art/Selection/Sprites/."""
    path = os.path.join(OUT, *parts)  # Full file path
    os.makedirs(os.path.dirname(path), exist_ok=True)  # Make the folder if it is missing
    img.save(path)  # Write the PNG


def main():
    """Draws everything."""
    save(backdrop(), "bg_mountains.png")  # Mountains behind the difficulty, level and lesson screens
    save(hills_backdrop(), "bg_hills.png")  # Hills behind the same screens when Hills is picked (D-063)
    save(plains_backdrop(), "bg_plains.png")  # Plains behind the same screens when Plains is picked
    save(plateaus_backdrop(), "bg_plateaus.png")  # Plateaus behind the same screens when Plateaus is picked
    save(panel(mm.VEST, mm.VEST_SH, mm.HEADLAMP), "Cards", "card_open.png")  # Open card: tan frame, cream paper
    save(panel(lm.HAMMER, mm.MASK_SH, mm.MASK), "Cards", "card_locked.png")  # Locked card: grey frame, grey paper
    for d in ["easy", "normal", "hard"]:  # Each difficulty
        save(mm.text_image(d.upper(), mm.DARK, 2), "Cards", "heading_%s.png" % d)  # Heading on an open card
        save(mm.text_image(d.upper(), mm.STRAP, 2), "Cards", "heading_%s_locked.png" % d)  # Heading on a locked card
    for e in lm.NAMES:  # Each landform
        save(mm.text_image(e.upper(), mm.DARK, 2), "Titles", "title_%s.png" % e)  # Difficulty screen title
        for d in ["easy", "normal", "hard"]:  # Each difficulty
            save(mm.text_image("%s: %s" % (e.upper(), d.upper()), mm.DARK, 2), "Titles", "title_%s_%s.png" % (e, d))  # Level trail title
    save(stone(mm.MASK, mm.MASK_SH), "Trail", "stone_open.png")  # Stone that can be played
    save(stone(mm.MASK_SH, mm.STRAP), "Trail", "stone_locked.png")  # Stone that is still locked
    save(lm.star(mm.HELMET, mm.HELMET_SH, mm.HEADLAMP), "Trail", "star_full.png")  # Star that was earned: yellow
    save(lm.star(mm.MASK_SH, mm.STRAP, mm.MASK), "Trail", "star_empty.png")  # Star not earned yet: grey
    save(lm.pattern(lm.PADLOCK, {"s": mm.MASK, "b": mm.HELMET_SH, "h": mm.HELMET, "k": mm.OUTLINE}), "Trail", "padlock_small.png")  # Small padlock on a locked stone
    save(dot(), "Trail", "dot.png")  # One dash of the trail
    print("Selection art written to", OUT)  # Tell the person running it where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
