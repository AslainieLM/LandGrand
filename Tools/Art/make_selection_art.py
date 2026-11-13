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


def save(img, *parts):
    """Saves an image under Assets/Art/Selection/Sprites/."""
    path = os.path.join(OUT, *parts)  # Full file path
    os.makedirs(os.path.dirname(path), exist_ok=True)  # Make the folder if it is missing
    img.save(path)  # Write the PNG


def main():
    """Draws everything."""
    save(backdrop(), "bg_mountains.png")  # Mountains behind both screens
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
