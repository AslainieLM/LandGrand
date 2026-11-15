"""Draws the Profile screen art for LandGrand: section tabs, landform tabs, badge medals and the NEW BADGE ribbon.

Run from the Unity project folder:  python Tools/Art/make_profile_art.py
It writes PNGs to Assets/Art/MainMenu/Sprites/Profile/.
All colours come from Assets/Art/Palettes/landgrand.gpl (plus even mixes of two palette colours).
"""
import os  # os is used to make folders and join paths
import sys  # sys lets us find the other art scripts
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))  # Look for the other scripts next to this one
import make_mainmenu_art as mm  # Main Menu palette, button and text helpers
import make_landform_map_art as lm  # Blue-grey colours for the silver medal

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))  # The Unity project folder
OUT = os.path.join(ROOT, "Assets", "Art", "MainMenu", "Sprites", "Profile")  # Where the Profile PNGs go


def mix(a, b):
    """An even mix of two palette colours."""
    return tuple((p + q) // 2 for p, q in zip(a, b))  # Average each colour channel


GREY_FACE = mm.MASK  # Locked tab face, dull grey
GREY_TEXT = mix(mm.MASK_SH, mm.STRAP)  # Locked tab text, darker grey
GREY_EDGE = mm.MASK_SH  # Locked tab bottom band
LOCK_ROWS = ["..###..", ".#...#.", ".#...#.", "#######", "#######", "###.###", "#######", "#######"]  # 7x8 padlock


def put(img, x, y, c):
    """Puts one solid pixel, skipping places outside the image."""
    if 0 <= x < img.width and 0 <= y < img.height:  # Stay inside the image
        img.putpixel((x, y), c + (255,))  # Paint the pixel fully visible


def save(img, name):
    """Saves a picture in the Profile sprite folder."""
    os.makedirs(OUT, exist_ok=True)  # Make the folder if it is missing
    img.save(os.path.join(OUT, name))  # Write the PNG


def tab(w, h, label, state, scale=2, lock=False):
    """A folder tab: state is on (cream, open at the bottom), off (brown, recessed) or locked (grey, recessed)."""
    img = mm.new_image(w, h)  # Empty tab
    on = state == "on"  # Is this the active tab
    top = 0 if on else 2  # Inactive tabs sit 2 pixels lower, so they look pushed back
    bh = h - top  # Height of the tab body
    mm.fill_rect(img, 1, top, w - 2, bh, mm.OUTLINE)  # Outline, tall part
    mm.fill_rect(img, 0, top + 1, w, bh - 1, mm.OUTLINE)  # Outline, wide part (corners stay cut)
    if on:  # Active: cream face with a tan border, no bottom line
        mm.fill_rect(img, 1, top + 1, w - 2, bh - 1, mm.VEST)  # Tan border
        mm.fill_rect(img, 2, top + 2, w - 4, bh - 2, mm.HEADLAMP)  # Cream face, runs to the last row
        put(img, 1, top + 1, mm.OUTLINE)  # Round the inner corner, left
        put(img, w - 2, top + 1, mm.OUTLINE)  # Round the inner corner, right
        text_col, shadow = mm.PLATEAU_SH, None  # Clay text, no shadow
    elif state == "off":  # Inactive: brown face, closed bottom
        mm.fill_rect(img, 1, top + 1, w - 2, bh - 2, mm.PLATEAU_SH)  # Brown face
        mm.fill_rect(img, 2, top + 1, w - 4, 1, mm.PLATEAU)  # Thin light top edge
        mm.fill_rect(img, 1, h - 3, w - 2, 2, mm.BACKPACK_SH)  # Dark band at the bottom
        text_col, shadow = mm.WHITE, mm.DARK  # White text with a dark shadow
    else:  # Locked: grey face
        mm.fill_rect(img, 1, top + 1, w - 2, bh - 2, GREY_FACE)  # Grey face
        mm.fill_rect(img, 1, h - 3, w - 2, 2, GREY_EDGE)  # Grey band at the bottom
        text_col, shadow = GREY_TEXT, None  # Dull dark-grey text
    tw = mm.text_width(label, scale)  # Width of the word
    gap = 3  # Space between padlock and word
    group = tw + (7 + gap if lock else 0)  # Lock plus word together
    x = (w - group) // 2  # Centre the group
    face_h = bh - (3 if on else 4)  # Usable face height
    ty = top + 1 + (face_h - 7 * scale) // 2 + (1 if on else 1)  # Centre the word in the face
    if lock:  # Draw the padlock first
        mm.paste(img, mm.mono(LOCK_ROWS, GREY_TEXT), x, top + 1 + (face_h - 8) // 2 + 1)  # Padlock left of the word
        x += 7 + gap  # Word starts after the lock
    mm.draw_text(img, x, ty, label, text_col, scale, shadow)  # The word
    return img


def parts():
    """Paints the section and landform tabs."""
    for name, word in (("collection", "COLLECTION"), ("achievements", "ACHIEVEMENTS")):  # The two sections
        w = mm.text_width(word, 2) + 24  # Text plus padding
        w += w % 2  # Keep the width even
        save(tab(w, 22, word, "on"), "tab_%s_on.png" % name)  # Active tab
        save(tab(w, 22, word, "off"), "tab_%s_off.png" % name)  # Inactive tab
    for name, word in (("mountains", "MOUNTAINS"), ("hills", "HILLS"), ("plains", "PLAINS"), ("plateaus", "PLATEAUS")):  # The four landforms
        save(tab(108, 20, word, "on"), "land_%s_on.png" % name)  # Active
        save(tab(108, 20, word, "off"), "land_%s_off.png" % name)  # Inactive
        fits = mm.text_width(word, 2) + 7 + 3 <= 104  # Does word plus lock fit at scale 2
        save(tab(108, 20, word, "locked", 2 if fits else 1, lock=True), "land_%s_locked.png" % name)  # Locked, smaller word if needed


# Symbols are 14 wide at most. "#" is the main colour, "o" is the shade colour.
SYMBOLS = {  # One pattern per badge
    "first_steps": [  # A boot
        "..####........", "..####........", "..####........", "..####........", "..####........",
        "..#####.......", "..######......", "..#######.....", "..########....", ".###########..",
        ".#############", ".#############", "..oooooooooooo"],
    "rock_hound": [  # A boulder
        "....#######...", "..###########.", ".#############", ".####o########", "##############",
        "##############", "###o####ooo###", "##############", "#oo###########", ".#############",
        "..oooooooooo.."],
    "crystal_finder": [  # A gem
        "...########...", "..##########..", ".############.", "##############", "oooooo########",
        ".oooooo######.", "..oooooo####..", "...oooooo##...", "....oooooo#...", ".....oooo#....",
        "......oo#....."],
    "star_explorer": [  # A big star
        "......#......", "......#......", ".....###.....", ".....###.....", "#############",
        ".###########.", "..#########..", "...#######...", "...#######...", "..####.####..",
        "..###...###..", ".###.....###.", ".##.......##."],
    "volcano_visitor": [  # A volcano with smoke
        ".....###......", "....#####.##..", ".....###.####.", "......#...##..", "....#oooo#....",
        "...oo######...", "..ooo#######..", ".oooo########.", "oooo##########", "oo############",
        "##############"],
    "summit_climber": [  # A peak with a flag
        "......#####...", "......####....", "......###.....", "......#.......", ".....###......",
        "....o####.....", "...ooo#####...", "..oooo######..", ".ooooo#######.", "oooooo########",
        "oooooo########", "oooooo########"],
    "philippine_peaks": [  # Two Philippine peaks side by side (D-062)
        "....#.........", "...###........", "..#####...#...", "..#####..###..", ".#######.####.",
        ".#######o#####", "#######oo#####", "######ooo#####", "#####oooo#####", "####ooooo#####",
        "##############"],
    "top_of_the_world": [  # The highest peak with a flag on top (D-062)
        "......#####...", "......#####...", "......#......", "......#......", ".....###.....",
        ".....###.....", "....o####....", "...oo#####...", "..ooo######..", ".oooo#######.",
        "ooooo########", "ooooo########"],
    "rock_scholar": [  # A magnifying glass over a rock (D-062)
        "...#####.....", "..#ooooo#....", ".#ooooooo#...", ".#ooooooo#...", ".#ooooooo#...",
        "..#ooooo#....", "...#####.....", "........##...", ".........##..", "..........##.",
        "...........##"],
    "brave_hiker": [  # A boulder rolling with speed lines (D-065)
        "........####..", "......########", ".....#########", "##...####o####", ".....###o#####",
        "####.##o######", ".....#########", "##...#########", "......########", "........####..",
        "oooooooooooooo"],
    "world_peaks": [  # A round globe with a peak on top (D-065)
        "......#.......", ".....###......", "....##o##.....", "...########...", "..##oo####o#..",
        ".###ooo###oo#.", ".####oo####o#.", ".######o#####.", "..##########..", "...########...",
        ".....####....."],
    "hard_star": [  # A star over a steep peak (D-065)
        "......#......", ".....###.....", "...#######...", "....#####....", "....##.##....",
        "......o......", ".....ooo.....", "....ooooo....", "...ooooooo...", "..ooooooooo..",
        ".ooooooooooo.", "ooooooooooooo"],
    "mountain_master": [  # A crown over a mountain (D-065)
        "#..#..#..#..#.", "##.##.##.##.##", "##############", "##############", "..............",
        "......o.......", ".....ooo......", "....ooooo.....", "...ooooooo....", "..ooooooooo...",
        ".ooooooooooo..", "ooooooooooooo."],
    "perfect_trail": None,  # Three stars in an arc, built by code below
    "mountain_collector": [  # A backpack
        "....######....", "...########...", "..##########..", ".############.", ".############.",
        ".oooooooooooo.", ".############.", "##oooooooooo##", "##o########o##", "##oooooooooo##",
        ".############."],
}

STAR5 = ["..#..", ".###.", "#####", ".###.", ".#.#."]  # A small star, 5x5
STAR7 = ["...#...", "..###..", "#######", ".#####.", "..###..", ".##.##.", "##...##"]  # A bigger star, 7x7


def trail_rows():
    """Three stars in an arc: a big one on top, two small ones lower left and right."""
    canvas = [["."] * 15 for _ in range(12)]  # Empty 15x12 pattern
    for rows, ox, oy in ((STAR5, 0, 6), (STAR7, 4, 0), (STAR5, 10, 6)):  # Where each star goes
        for r, line in enumerate(rows):  # Each star row
            for c, ch in enumerate(line):  # Each star pixel
                if ch == "#":  # Lit pixel
                    canvas[oy + r][ox + c] = "#"  # Copy it into the arc
    return ["".join(row) for row in canvas]  # Rows as text


SYMBOLS["perfect_trail"] = trail_rows()  # Fill in the stars badge

TIERS = {  # Colours per tier: fill, shade, ring, ribbon, symbol, symbol shade, symbol shadow
    "bronze": dict(fill=mm.PLATEAU, shade=mm.PLATEAU_SH, ring=mix(mm.PLATEAU, mm.HEADLAMP), ribbon=mm.HILLS_SH, ribbon_sh=mm.LEAF,
                   sym=mm.WHITE, sym_sh=mm.MASK, sym_shadow=mm.BACKPACK_SH),  # Warm brown-orange, green ribbon
    "silver": dict(fill=lm.HAZE, shade=lm.MOUNTAIN, ring=lm.SNOW, ribbon=lm.MOUNTAIN, ribbon_sh=mm.STRAP,
                   sym=mm.STRAP, sym_sh=lm.MOUNTAIN, sym_shadow=mm.WHITE),  # Light grey-blue, blue ribbon
    "gold": dict(fill=mm.HELMET, shade=mm.HELMET_SH, ring=mm.HEADLAMP, ribbon=mm.SCARF, ribbon_sh=mm.SCARF_SH,
                 sym=mm.BACKPACK_SH, sym_sh=mm.DARK, sym_shadow=mm.HEADLAMP),  # Yellow, red ribbon
}
BADGES = [  # id, tier
    ("first_steps", "bronze"), ("rock_hound", "bronze"), ("crystal_finder", "bronze"),
    ("star_explorer", "silver"), ("volcano_visitor", "silver"), ("summit_climber", "silver"),
    ("perfect_trail", "gold"), ("mountain_collector", "gold"),
    ("philippine_peaks", "silver"), ("top_of_the_world", "gold"), ("rock_scholar", "silver"),  # Mountains Normal badges (D-062)
    ("brave_hiker", "silver"), ("world_peaks", "silver"), ("hard_star", "gold"), ("mountain_master", "gold"),  # Mountains Hard badges (D-065)
]
CX, CY, R = 16.0, 13.5, 13.0  # Medal centre and outer radius


def tail_pixels(x0, cx):
    """Pixels of one ribbon tail (7 wide, rows 20-31) with a V notch at the bottom."""
    pix = set()  # Pixels of this tail
    for y in range(20, 32):  # Tail rows
        for x in range(x0, x0 + 7):  # Tail columns
            k = y - 29  # How deep into the notch this row is
            if k >= 0 and abs(x - cx) <= k:  # Cut the V notch
                continue  # Skip this pixel
            pix.add((x, y))  # Keep the pixel
    return pix


def symbol_image(rows, main, shade, shadow):
    """Draws a symbol pattern, with an optional 1 px shadow under it."""
    img = mm.new_image(32, 32)  # Full badge canvas
    w, h = max(len(r) for r in rows), len(rows)  # Pattern size
    ox, oy = 16 - w // 2, 13 - h // 2  # Centre it on the medal
    for r, line in enumerate(rows):  # Each row
        for c, ch in enumerate(line):  # Each pixel
            if ch in "#o" and shadow is not None:  # Mark shadow below
                put(img, ox + c, oy + r + 1, shadow)  # Shadow pixel
    for r, line in enumerate(rows):  # Each row again, the symbol itself
        for c, ch in enumerate(line):  # Each pixel
            if ch == "#":  # Main colour
                put(img, ox + c, oy + r, main)  # Main pixel
            elif ch == "o":  # Shade colour
                put(img, ox + c, oy + r, shade)  # Shade pixel
    return img


def medal(bid, tier, locked):
    """A 32x32 round medal with two ribbon tails; locked ones are flat dark grey with a padlock."""
    t = TIERS[tier]  # Colours of the tier
    img = mm.new_image(32, 32)  # Empty badge
    tails = tail_pixels(8, 11) | tail_pixels(17, 20)  # Both ribbon tails
    ribbon = GREY_TEXT if locked else t["ribbon"]  # Tail colour
    ribbon_sh = mm.STRAP if locked else t["ribbon_sh"]  # Tail shade colour
    for (x, y) in tails:  # Outline around the tails
        for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):  # Four neighbours
            if (x + dx, y + dy) not in tails:  # Edge of the tail
                put(img, x + dx, y + dy, mm.OUTLINE)  # Dark outline pixel
    for (x, y) in tails:  # Tail fill
        right_half = (x - (8 if x < 16 else 17)) >= 4  # Light from the right
        put(img, x, y, ribbon if right_half or y < 24 else ribbon_sh)  # Light on the right, shade on the left
    fill = mix(mm.STRAP, mm.MASK_SH) if locked else t["fill"]  # Disc colour
    shade = mm.STRAP if locked else t["shade"]  # Lower-left rim colour
    ring = fill if locked else t["ring"]  # Inner ring colour
    for y in range(32):  # Each row
        for x in range(32):  # Each column
            dx, dy = x + 0.5 - CX, y + 0.5 - CY  # Offset from the centre
            d = (dx * dx + dy * dy) ** 0.5  # Distance from the centre
            if d <= R:  # Outer outline
                put(img, x, y, mm.OUTLINE)  # Outline pixel
            if d <= R - 1.0:  # Body of the disc
                col = fill  # Base colour
                if 10.2 <= d <= 11.3 and not locked:  # Lighter inner ring
                    col = ring  # Ring pixel
                elif d > 10.2 and (dx - dy) < -6:  # Shaded lower-left rim
                    col = shade  # Rim shade
                put(img, x, y, col)  # Paint it
    if locked:  # Grey symbol, a little lighter than the disc, plus a padlock
        sym = symbol_image(SYMBOLS[bid], mm.MASK_SH, mm.MASK_SH, None)  # Flat lighter-grey symbol
        mm.paste(img, sym, 0, 0)  # Symbol on the medal
        lock = mm.outlined(LOCK_ROWS, mm.MASK)  # Light padlock with a dark outline
        mm.paste(img, lock, 22, 21)  # Lower right corner
    else:  # Coloured symbol
        sym = symbol_image(SYMBOLS[bid], t["sym"], t["sym_sh"], t["sym_shadow"])  # Symbol with its shadow
        mm.paste(img, sym, 0, 0)  # Symbol on the medal
    return img


def new_ribbon():
    """The 96x20 red NEW BADGE! banner with notched ends."""
    w, h = 96, 20  # Banner size
    img = mm.new_image(w, h)  # Empty banner
    face = set()  # Pixels of the red banner
    for y in range(1, h - 1):  # Each row inside the border
        for x in range(1, w - 1):  # Each column inside the border
            e = min(x, w - 1 - x)  # Distance from the nearest end
            if e < 6 and abs(y - (h - 1) / 2) < 6 - e:  # Inside the sideways V notch
                continue  # Leave it empty
            face.add((x, y))  # Keep the pixel
    for (x, y) in face:  # Outline pass
        for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):  # Four neighbours
            if (x + dx, y + dy) not in face:  # Edge of the banner
                put(img, x + dx, y + dy, mm.OUTLINE)  # Dark outline pixel
    for (x, y) in face:  # Fill pass
        col = mm.SCARF  # Red face
        if y <= 2:  # Light top band
            col = mix(mm.SCARF, mm.HEADLAMP)  # Lighter red
        elif y >= h - 4:  # Dark bottom band
            col = mm.SCARF_SH  # Darker red
        put(img, x, y, col)  # Paint it
    text = "NEW BADGE!"  # Banner words
    scale = 1  # Scale 2 is 106 px wide, too wide for 96
    tx = (w - mm.text_width(text, scale)) // 2  # Centre the words
    mm.draw_text(img, tx, (h - 7 * scale) // 2, text, mm.WHITE, scale, mm.DARK)  # White words with a dark shadow
    return img


def main():
    """Draws everything."""
    parts()  # Tabs
    for bid, tier in BADGES:  # Each badge
        save(medal(bid, tier, False), "badge_%s.png" % bid)  # Coloured medal
        save(medal(bid, tier, True), "badge_%s_locked.png" % bid)  # Locked medal
    save(new_ribbon(), "badge_new_ribbon.png")  # NEW BADGE banner
    print("Profile art written to", OUT)  # Tell the user where


if __name__ == "__main__":
    main()  # Run when called from the command line
