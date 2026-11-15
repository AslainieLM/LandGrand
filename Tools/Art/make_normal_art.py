"""Draws the PNGs for Mountains Normal 1 to 5 (D-062): four new level themes, the new level objects,
ten new rocks and minerals, and five lesson pictures of real mountains.

Run from the Unity project folder:  python Tools/Art/make_normal_art.py
It writes to Assets/Art/Level/Sprites/ (themes and level objects), Assets/Art/Items/Sprites/ (items)
and Assets/Art/Lesson/Sprites/ (lesson pictures). It reuses the helpers of make_level_art.py and make_lesson_art.py.
All colours come from Assets/Art/Palettes/landgrand.gpl or mix two of its colours.
"""
import math  # math gives circles and waves
import make_mainmenu_art as mm  # Palette, new_image and fill_rect
import make_landform_map_art as lm  # Map colours, noise and ridge
import make_level_art as la  # put, outline, wave, boulder_colored, draw_grid and save for level sprites
import make_lesson_art as ls  # framed, disc, fill_poly and the item and lesson folders


def mix(a, b):
    """The colour halfway between two palette colours."""
    return tuple((x + y) // 2 for x, y in zip(a, b))  # Average of each channel


# Theme colours (D-062), each a palette colour or a mix of two.
PINE_GREEN = mm.LEAF  # Pine needles and dark grass
PINE_DARK = mix(mm.LEAF, mm.OUTLINE)  # Deep shade under the pines
NEEDLE = mm.GROUND_SH  # Brown fallen pine needles
ASH = mix(mm.MASK, mm.VEST)  # Pale grey-beige volcanic ash
ASH_SH = mix(mm.MASK_SH, mm.VEST_SH)  # Ash in shade
ASH_LT = mix(mm.MASK, mm.WHITE)  # Sunny ash
MOSS = mm.HILLS_SH  # Wet moss
MOSS_LT = mm.HILLS  # Moss in sun
HUMUS = mix(mm.BACKPACK_SH, mm.OUTLINE)  # Dark forest soil
SULFUR = mm.HELMET  # Bright yellow sulfur
SULFUR_SH = mm.HELMET_SH  # Sulfur in shade
SNOW = lm.SNOW  # Snow
SNOW_SH = lm.HAZE  # Snow in shade
ICE_ROCK = lm.MOUNTAIN_SH  # Cold dark rock under the snow


# ---------- Shared tile makers ----------

def ground_top(base, cap, light, shade, specks):
    """A 16x8 ground-top tile: a coloured cap over the soil colour, a sunny top line and a shadow line."""
    img = mm.new_image(16, 8)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 8, base)  # Soil below
    mm.fill_rect(img, 0, 0, 16, 4, cap)  # Cap
    mm.fill_rect(img, 0, 0, 16, 1, light)  # Sunny top line
    mm.fill_rect(img, 0, 4, 16, 1, shade)  # Shadow under the cap
    for (x, y, c) in specks:  # Small details
        la.put(img, x, y, c)  # One detail pixel
    return img


def soil_tile(base, speck, extra=None):
    """A 16x16 soil tile with darker specks and optional extra pixels. It tiles in both directions."""
    img = mm.new_image(16, 16)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 16, base)  # Soil
    for y in range(16):  # Each row
        for x in range(16):  # Each pixel
            if lm.noise(x, y) < 9:  # A few darker specks
                la.put(img, x, y, speck)  # Dark speck
    for (x, y, c) in extra or []:  # Roots, layers or stones
        la.put(img, x, y, c)  # One extra pixel
    return img


def ledge_tile(body, top, under, flecks):
    """A 16x10 ledge tile that Unity repeats across: light top, rock body, dark underside and outline."""
    img = mm.new_image(16, 10)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 10, body)  # Rock
    mm.fill_rect(img, 0, 0, 16, 2, top)  # Light top where the explorer stands
    mm.fill_rect(img, 0, 8, 16, 2, under)  # Dark underside
    for (x, y, c) in flecks:  # Texture
        la.put(img, x, y, c)  # One texture pixel
    mm.fill_rect(img, 0, 9, 16, 1, mm.OUTLINE)  # Dark bottom edge
    return img


def capped_boulder(w, h, mid, shade, light, speck, cap, cap_rows):
    """A boulder like make_level_art's, with a cap of moss or snow over its top rows."""
    img = la.boulder_colored(w, h, mid, shade, light, speck)  # The rock
    top = {}  # First rock row of each column
    for x in range(w):  # Each column
        for y in range(h):  # Top to bottom
            r, g, b, a = img.getpixel((x, y))  # The pixel here
            if a and (r, g, b) != mm.OUTLINE:  # First rock pixel under the outline
                top[x] = y  # Remember it
                break  # Next column
    for x, y0 in top.items():  # Each column with rock
        for y in range(y0, y0 + cap_rows + (1 if lm.noise(x, 3) < 12 else 0)):  # The cap rows, a little ragged
            r, g, b, a = img.getpixel((x, y))  # The pixel there
            if a and (r, g, b) != mm.OUTLINE:  # Still rock, not the outline
                la.put(img, x, y, cap)  # Cover it with moss or snow
    return img


# ---------- Pine Forest: Mount Pulag (N1) ----------

def pine_top():
    """Dark pine-forest grass with fallen needles and a little frost."""
    return ground_top(mm.GROUND_SH, PINE_GREEN, mm.HILLS, PINE_DARK,
                      [(2, 2, NEEDLE), (3, 2, NEEDLE), (9, 3, NEEDLE), (13, 1, NEEDLE), (6, 0, SNOW), (11, 0, SNOW), (5, 6, PINE_DARK)])  # Needles, frost and a dark root


def pine_soil():
    """Brown forest soil with pale roots."""
    roots = [(x, 5 + la.wave(x, 1), mm.VEST_SH) for x in range(2, 9)] + [(x, 12 + la.wave(x, 1, 8), mm.VEST_SH) for x in range(9, 15)]  # Two thin roots
    return soil_tile(mm.GROUND_SH, mm.BACKPACK_SH, roots)  # Soil with roots


def ledge_pine():
    """A grey stone slab with moss on top."""
    return ledge_tile(lm.MOUNTAIN, mm.HILLS_SH, lm.MOUNTAIN_SH, [(3, 0, mm.HILLS), (9, 1, PINE_GREEN), (5, 4, lm.MOUNTAIN_SH), (12, 5, lm.HAZE)])  # Moss, cracks and a light fleck


def pine_boulder(w, h):
    """A grey boulder with a mossy top."""
    return capped_boulder(w, h, lm.MOUNTAIN, lm.MOUNTAIN_SH, lm.HAZE, mm.STRAP, MOSS, 3)  # Rock with moss


def ranger_hut():
    """A 56x48 wooden ranger hut with a pine-green roof and an open door at the bottom middle (the N1 goal)."""
    img = mm.new_image(56, 48)  # Empty picture
    for y in range(4, 22):  # Roof rows
        half = 6 + (y - 4) * 1.25  # Roof gets wider going down
        for x in range(int(28 - half), int(28 + half)):  # Pixels of the roof row
            la.put(img, x, y, PINE_GREEN if x < 28 else PINE_DARK)  # Sunny left, shaded right
    for y in range(22, 48):  # Wall rows
        for x in range(8, 48):  # Wall pixels
            la.put(img, x, y, mm.GROUND if (y - 22) % 5 else mm.GROUND_SH)  # Log lines every 5 rows
    mm.fill_rect(img, 22, 30, 12, 18, mm.OUTLINE)  # Open door
    mm.fill_rect(img, 12, 28, 6, 6, mm.SKY_LIGHT)  # Small window
    mm.fill_rect(img, 38, 28, 6, 6, mm.SKY_LIGHT)  # Second window
    mm.fill_rect(img, 40, 6, 4, 8, mm.STRAP)  # Chimney
    return la.outline(img)  # Dark edge


def pine_tree():
    """A 24x48 background pine for the Pine Forest (decoration only)."""
    img = mm.new_image(24, 48)  # Empty picture
    for y in range(2, 42):  # Needle rows
        half = 2 + ((y - 2) % 10) * 0.9 + (y - 2) // 10 * 1.6  # Layered tiers, wider going down
        for x in range(int(12 - half), int(12 + half) + 1):  # Pixels of this row
            la.put(img, x, y, PINE_GREEN if x < 12 else PINE_DARK)  # Sunny left, shaded right
    mm.fill_rect(img, 11, 42, 3, 6, lm.HANDLE)  # Trunk
    return la.outline(img)  # Dark edge


# ---------- Ash Gorge: Mount Pinatubo (N2) ----------

def ash_top():
    """Pale ash ground with small grey pebbles."""
    return ground_top(ASH_SH, ASH, ASH_LT, mm.MASK_SH, [(3, 2, mm.MASK_SH), (8, 1, mm.STRAP), (13, 2, mm.MASK_SH), (10, 6, mm.VEST_SH)])  # Pebbles


def ash_soil():
    """Layered ash and mud bands left by lahars."""
    img = mm.new_image(16, 16)  # Empty tile
    bands = [ASH_SH, ASH_SH, mm.VEST_SH, ASH, ASH, mm.MASK_SH, mm.VEST_SH, mm.VEST_SH]  # Eight 2-row layers
    for y in range(16):  # Each row
        for x in range(16):  # Each pixel
            c = bands[((y + la.wave(x, 1)) % 16) // 2]  # The layer, bent a little
            la.put(img, x, y, mm.STRAP if lm.noise(x, y) < 5 else c)  # Some dark pebbles
    return img


def ledge_ash():
    """A pale hardened-ash slab."""
    return ledge_tile(ASH, ASH_LT, ASH_SH, [(4, 4, mm.MASK_SH), (11, 5, mm.VEST_SH), (8, 3, ASH_LT)])  # Texture


def ash_boulder(w, h):
    """A pale ash-grey boulder with dark pebbles."""
    return la.boulder_colored(w, h, ASH, ASH_SH, ASH_LT, mm.STRAP, (mm.VEST, mm.VEST_SH))  # Banded ash rock


def stone_bridge():
    """A 72x44 stone footbridge with an arch under it; the explorer walks past in front (the N2 goal)."""
    img = mm.new_image(72, 44)  # Empty picture
    for y in range(8, 44):  # Each row
        for x in range(72):  # Each pixel
            if y > 20 and (x - 36) ** 2 / 400 + (y - 44) ** 2 / 500 < 1:  # The open arch underneath
                continue  # Leave it see-through
            shift = 4 if (y // 6) % 2 else 0  # Every other row of bricks is moved half a brick
            stone = ((x + shift) // 8 + y // 6) % 2  # Brick pattern
            la.put(img, x, y, ASH if stone else ASH_SH)  # Two stone shades
            if y % 6 == 0 or (x + shift) % 8 == 0:  # Mortar lines
                la.put(img, x, y, mm.MASK_SH)  # Mortar
    mm.fill_rect(img, 0, 6, 72, 3, mm.MASK)  # Flat top of the bridge
    for x in range(2, 72, 10):  # Railing posts
        mm.fill_rect(img, x, 0, 2, 6, lm.HANDLE)  # One post
    mm.fill_rect(img, 0, 1, 72, 1, lm.HANDLE)  # Railing
    return la.outline(img)  # Dark edge


def rock_fall():
    """A 16x16 falling rock (N2 rockfall)."""
    return la.boulder_colored(16, 16, mm.MASK_SH, mm.STRAP, mm.MASK, mm.OUTLINE)  # Grey rock


def rock_warning(frame):
    """A 24x4 dark shadow that warns where a rock will land; frame 1 is darker."""
    img = mm.new_image(24, 4)  # Empty picture
    for y in range(4):  # Each row
        for x in range(24):  # Each pixel
            if ((x - 11.5) / 12) ** 2 + ((y - 1.5) / 2) ** 2 <= 1:  # Inside the flat oval
                la.put(img, x, y, mm.OUTLINE, 90 if frame == 0 else 160)  # See-through dark shadow
    return img


def pebbles(frame):
    """A 16x8 trickle of small stones that falls before a rock comes down (frames 0 and 1)."""
    img = mm.new_image(16, 8)  # Empty picture
    for (x, y) in ((3, 1), (8, 3), (12, 0), (6, 6), (10, 5)):  # Stone places
        la.put(img, x, (y + frame * 2) % 8, mm.MASK_SH)  # One stone, a little lower in frame 1
    return img


# ---------- Mossy Forest: Mount Matutum (N3) and Mount Apo (N4) ----------

def moss_top():
    """Thick wet moss over dark soil, with small ferns."""
    return ground_top(HUMUS, MOSS, MOSS_LT, PINE_DARK, [(2, 1, MOSS_LT), (6, 2, mm.LEAF), (10, 1, mm.PLAINS), (14, 2, mm.LEAF), (8, 6, mm.BACKPACK_SH)])  # Moss bumps


def moss_soil():
    """Dark forest soil with tiny water drops."""
    return soil_tile(HUMUS, mm.OUTLINE, [(4, 3, lm.LENS), (11, 9, lm.LENS), (7, 13, mm.BACKPACK_SH)])  # Soil and drops


def ledge_moss():
    """A mossy fallen-log ledge."""
    return ledge_tile(mm.BACKPACK_SH, MOSS, mm.OUTLINE, [(2, 4, lm.BACKPACK), (7, 5, lm.BACKPACK), (12, 4, lm.BACKPACK), (5, 1, MOSS_LT), (11, 0, MOSS_LT)])  # Bark lines and moss


def moss_boulder(w, h):
    """A dark boulder thick with moss."""
    return capped_boulder(w, h, mm.STRAP, mm.OUTLINE, lm.MOUNTAIN, mm.OUTLINE, MOSS, 6)  # Dark rock, thick moss


def trail_sign():
    """A 32x40 wooden trail sign with an arrow pointing on (the N3 and N4 goal)."""
    img = mm.new_image(32, 40)  # Empty picture
    mm.fill_rect(img, 14, 10, 4, 30, lm.HANDLE)  # Post
    mm.fill_rect(img, 2, 6, 26, 12, mm.GROUND)  # Board
    mm.fill_rect(img, 2, 15, 26, 3, mm.GROUND_SH)  # Board shade
    la.draw_grid(img, ["#....", "###..", "#####", "###..", "#...."], 20, 9, {"#": mm.HELMET})  # Arrow tip
    mm.fill_rect(img, 6, 11, 14, 2, mm.HELMET)  # Arrow shaft
    mm.fill_rect(img, 2, 6, 26, 2, MOSS)  # Moss on the board
    return la.outline(img)  # Dark edge


def fern():
    """A 16x12 fern for the Mossy Forest (decoration only)."""
    img = mm.new_image(16, 12)  # Empty picture
    for k, (dx, lift) in enumerate(((-1, 0.55), (1, 0.55), (-1, 1.2), (1, 1.2), (0, 0))):  # Five fronds: two low, two high, one straight up
        for t in range(8):  # Along the frond
            x = 8 + dx * t  # Out to the side
            y = 11 - int(t * lift) if dx else 11 - t  # Up as it goes out
            la.put(img, x, y, mm.HILLS if k % 2 else mm.LEAF)  # The stem
            la.put(img, x, y - 1, mm.HILLS_SH)  # Leaflets on top of the stem
    return la.outline(img)  # Dark edge


def sulfur_crust():
    """A 24x6 yellow sulfur crust that sits around Mount Apo's vents (decoration only)."""
    img = mm.new_image(24, 6)  # Empty picture
    for x in range(24):  # Each column
        top = 2 + (1 if lm.noise(x, 1) < 16 else 0)  # Lumpy top
        for y in range(top, 6):  # Rows of the crust
            la.put(img, x, y, SULFUR if y == top else SULFUR_SH)  # Bright top, darker below
    return img


def platform():
    """A 48x10 stone block that slides (N3 moving platforms): cracked like a fault block."""
    piece = ledge_tile(lm.MOUNTAIN, lm.HAZE, lm.MOUNTAIN_SH, [(x, 3 + (x // 6) % 3, mm.STRAP) for x in range(4, 14)])  # One 16-pixel piece
    wide = mm.new_image(48, 10)  # The whole block
    for i in range(3):  # Three pieces side by side
        wide.paste(piece, (i * 16, 0))  # Copy the piece
    for y in range(10):  # Dark ends, so it reads as one block
        la.put(wide, 0, y, mm.OUTLINE)  # Left end
        la.put(wide, 47, y, mm.OUTLINE)  # Right end
    return wide


# ---------- Crumbling ledge (N1) ----------

def crumble(frame):
    """A 32x10 stone ledge that cracks more in each frame: 0 whole, 1 cracked, 2 breaking, 3 almost gone."""
    img = mm.new_image(32, 10)  # Empty picture
    tile = ledge_pine()  # The same stone as the Pine Forest ledges
    img.paste(tile, (0, 0))  # Left half
    img.paste(tile, (16, 0))  # Right half
    cracks = [[], [(10, 2), (10, 3), (11, 4), (22, 3), (21, 4)],
              [(10, 2), (10, 3), (11, 4), (11, 5), (11, 6), (22, 3), (21, 4), (21, 5), (21, 6), (16, 2), (16, 3)]]  # Crack pixels per frame
    for (x, y) in cracks[min(frame, 2)]:  # The cracks of this frame
        la.put(img, x, y, mm.OUTLINE)  # Dark crack
    if frame == 3:  # Almost gone: only scattered pieces remain
        for y in range(10):  # Each row
            for x in range(32):  # Each pixel
                if lm.noise(x, y) % 3:  # Two thirds of the pixels fall away
                    img.putpixel((x, y), (0, 0, 0, 0))  # See-through
    return img


def crumble_bit():
    """A 4x3 falling stone chip."""
    img = mm.new_image(4, 3)  # Empty picture
    mm.fill_rect(img, 0, 0, 4, 3, lm.MOUNTAIN)  # Stone
    la.put(img, 3, 2, lm.MOUNTAIN_SH)  # Shade
    return img


# ---------- Snow Peak: Mount Everest (N5) ----------

def snow_top():
    """Deep snow with blue shade."""
    return ground_top(ICE_ROCK, SNOW, mm.WHITE, SNOW_SH, [(4, 2, SNOW_SH), (11, 3, SNOW_SH), (8, 6, lm.MOUNTAIN)])  # Snow ripples


def snow_soil():
    """Cold dark rock with ice streaks."""
    return soil_tile(ICE_ROCK, mm.STRAP, [(x, 7 + la.wave(x, 1), lm.HAZE_SH) for x in range(16)])  # Rock with an ice streak


def ledge_snow():
    """A rock slab with a snow top."""
    return ledge_tile(lm.MOUNTAIN, SNOW, lm.MOUNTAIN_SH, [(3, 2, SNOW_SH), (10, 2, SNOW_SH), (7, 5, lm.HAZE_SH)])  # Snow edge and ice


def snow_boulder(w, h):
    """A dark rock with a snow cap."""
    return capped_boulder(w, h, lm.MOUNTAIN, lm.MOUNTAIN_SH, lm.HAZE, mm.STRAP, SNOW, 5)  # Rock with snow


def summit_flags():
    """A 56x56 summit marker: a snowy top with a pole and lines of coloured prayer flags (the N5 goal)."""
    img = mm.new_image(56, 56)  # Empty picture
    for y in range(30, 56):  # Snow mound
        half = 6 + (y - 30) * 0.9  # Wider going down
        for x in range(int(28 - half), int(28 + half)):  # Pixels of the row
            la.put(img, x, y, SNOW if x < 28 else SNOW_SH)  # Sunny left, shaded right
    mm.fill_rect(img, 27, 2, 2, 30, lm.HANDLE)  # Pole
    colours = [lm.LENS, mm.WHITE, mm.SCARF, mm.LEAF, mm.HELMET]  # Blue, white, red, green and yellow flags
    for side in (-1, 1):  # A line of flags on each side of the pole
        for i in range(5):  # Five flags
            x = 28 + side * (4 + i * 5) - (4 if side < 0 else 0)  # Left edge of the flag
            y = 4 + i * 4  # The line droops going out
            mm.fill_rect(img, x, y, 4, 4, colours[i])  # One flag
    return la.outline(img)  # Dark edge


def gust(frame):
    """A 20x6 white wind streak for the gusts (frames 0 to 3 slide and fade)."""
    img = mm.new_image(20, 6)  # Empty picture
    for (y, x0, length) in ((1, 0, 10), (3, 6, 12), (5, 2, 8)):  # Three streaks
        for x in range(length):  # Along the streak
            la.put(img, (x0 + x + frame * 2) % 20, y, mm.WHITE, 200 - frame * 40)  # Moves right and fades
    return img


# ---------- Items (24x24, Assets/Art/Items/Sprites/) ----------

def blob(w, h, cx, cy, rx, ry, colour_at, power=2.0):
    """Paints a rounded lump; colour_at(x, y, shaded) gives each pixel's colour."""
    img = mm.new_image(w, h)  # Empty picture
    for y in range(h):  # Each row
        for x in range(w):  # Each pixel
            if abs((x - cx) / rx) ** power + abs((y - cy) / ry) ** power > 1.0:  # Outside the lump
                continue  # Leave it see-through
            shaded = (x - cx) * 0.8 + (y - cy) * 1.3 > 4  # Lower right is in shade
            ls.put(img, x, y, colour_at(x, y, shaded))  # Paint the pixel
    return img


def sandstone():
    """Tan sandstone with fine sand grains and gentle layers."""
    def c(x, y, s):  # Colour of one pixel
        if y % 4 == 0:  # A layer line
            return mm.VEST_SH if s else mm.PLATEAU_SH  # Darker layer line
        if lm.noise(x, y) < 6:  # A few bright grains
            return mm.HELMET_SH  # Grain
        return mm.VEST_SH if s else mm.VEST  # Tan sand
    return ls.outline(blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 3))  # Boxy tan rock


def clay():
    """A smooth reddish-brown lump of clay with a fingerprint dent."""
    img = blob(24, 24, 11.5, 14, 9.5, 7, lambda x, y, s: mm.PLATEAU_SH if s else mm.PLATEAU)  # Rounded smooth lump
    for (x, y) in ((9, 11), (10, 11), (11, 12), (12, 12)):  # A dent from a finger
        ls.put(img, x, y, mm.PLATEAU_SH)  # Dent pixel
    for (x, y) in ((7, 10), (8, 10)):  # Wet shine
        ls.put(img, x, y, mm.SCARF)  # Shine pixel
    return ls.outline(img)  # Dark edge


def conglomerate():
    """Round pebbles of many colours stuck in sandy rock."""
    img = blob(24, 24, 11.5, 13.5, 10.5, 8, lambda x, y, s: mm.VEST_SH if s else mm.VEST)  # Sandy cement
    for (px, py, r, col) in ((7, 11, 2, mm.MASK), (13, 10, 2, mm.PLATEAU), (10, 16, 2, mm.STRAP), (16, 15, 2, mm.MASK_SH), (5, 15, 1, mm.WHITE), (17, 11, 1, mm.STRAP)):  # Pebbles
        ls.disc(img, px, py, r, col)  # One pebble
    return ls.outline(img)  # Dark edge


def shale():
    """Dull grey-brown shale in thin flaky layers."""
    img = mm.new_image(24, 24)  # Empty picture
    for k in range(6):  # Six thin flakes
        y = 7 + k * 2  # Top row of this flake
        mm.fill_rect(img, 3 + (k % 2), y, 17 - (k % 3), 1, mm.MASK_SH if k % 2 else mm.VEST_SH)  # One flake
        mm.fill_rect(img, 3 + (k % 2), y + 1, 17 - (k % 3), 1, mm.STRAP)  # Thin dark gap under it
    return ls.outline(img)  # Dark edge


def basalt():
    """Dark grey basalt with small gas holes."""
    def c(x, y, s):  # Colour of one pixel
        if lm.noise(x, y) < 7:  # A gas hole
            return mm.OUTLINE  # Black hole
        return mm.STRAP if s else lm.MOUNTAIN_SH  # Dark rock
    return ls.outline(blob(24, 24, 11.5, 13.5, 10, 7.5, c, 2.6))  # Blocky dark rock


def magnetite():
    """A black shiny magnetite crystal (an eight-sided diamond) on a pile of black sand."""
    img = mm.new_image(24, 24)  # Empty picture
    for x in range(2, 22):  # Black sand pile at the bottom
        for y in range(19 - int(3 * math.sin(math.pi * (x - 2) / 20)), 22):  # The pile's rows
            ls.put(img, x, y, mm.STRAP if lm.noise(x, y) % 2 else mm.OUTLINE)  # Sand grains
    ls.fill_poly(img, [(12, 3), (19, 11), (12, 19), (5, 11)], mm.STRAP)  # The crystal
    ls.fill_poly(img, [(12, 3), (12, 19), (5, 11)], lm.MOUNTAIN_SH)  # Its lit face
    for (x, y) in ((10, 8), (9, 9), (11, 7)):  # Metal shine
        ls.put(img, x, y, lm.HAZE)  # Shine pixel
    return ls.outline(img)  # Dark edge


def sulfur():
    """Bright yellow sulfur crystals."""
    img = mm.new_image(24, 24)  # Empty picture
    for (x0, top, w) in ((4, 9, 6), (9, 4, 7), (15, 8, 5)):  # Three crystals
        ls.fill_poly(img, [(x0, 20), (x0, top + 3), (x0 + w / 2, top), (x0 + w, top + 3), (x0 + w, 20)], SULFUR)  # One crystal
        mm.fill_rect(img, x0 + w // 2 + 1, top + 2, w - w // 2 - 1, 20 - top - 2, SULFUR_SH)  # Its shaded side
    mm.fill_rect(img, 3, 20, 18, 2, mm.VEST_SH)  # Rock base
    return ls.outline(img)  # Dark edge


def pyrite():
    """Shiny gold pyrite cubes ("fool's gold")."""
    img = mm.new_image(24, 24)  # Empty picture
    for (x, y, s) in ((4, 11, 9), (12, 5, 8), (12, 14, 7)):  # Three cubes
        mm.fill_rect(img, x, y, s, s, mm.HELMET_SH)  # Front face
        mm.fill_rect(img, x, y, s, 2, mm.HEADLAMP)  # Lit top
        mm.fill_rect(img, x + s - 2, y + 2, 2, s - 2, mm.VEST_SH)  # Shaded side
        ls.put(img, x + 2, y + 3, mm.WHITE)  # Glint
    return ls.outline(img)  # Dark edge


def limestone():
    """Pale cream limestone with a small shell fossil."""
    img = blob(24, 24, 11.5, 13.5, 10.5, 7.5, lambda x, y, s: mm.MASK if s else mix(mm.WHITE, mm.VEST), 2.6)  # Pale rock
    for t in range(16):  # A small spiral shell
        a = t * 0.75  # Angle along the spiral
        ls.put(img, int(round(11 + math.cos(a) * t * 0.3)), int(round(13 + math.sin(a) * t * 0.3)), mm.VEST_SH)  # One shell pixel
    return ls.outline(img)  # Dark edge


def gneiss():
    """Gneiss with wavy black and white bands."""
    def c(x, y, s):  # Colour of one pixel
        band = ((y + la.wave(x, 2, 12)) // 2) % 3  # Which band, wavy
        return [mm.OUTLINE, mm.MASK_SH, mm.MASK][band] if s else [mm.STRAP, mm.MASK, mm.WHITE][band]  # Dark, grey and light bands
    return ls.outline(blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 2.4))  # Banded rock


# ---------- Lesson pictures (120x82, Assets/Art/Lesson/Sprites/) ----------

def sky(x, y):
    """The lesson sky: deep at the top, a checker step, then light (same as the Easy pictures)."""
    if y < 8:  # Top rows
        return lm.SKY  # Deep sky
    if y < 10:  # Two rows of checker
        return (lm.SKY, mm.SKY_LIGHT)[(x + y) % 2]  # Checker step
    return mm.SKY_LIGHT  # Light sky


def pulag_pixel(x, y):
    """Mount Pulag: rolling grassy summits above a sea of clouds, pines on the lower slopes."""
    top = 22 + int(6 * math.sin(x / 14.0)) + int(3 * math.sin(x / 5.0))  # Rolling summit line
    cloud = 50 + int(3 * math.sin(x / 6.0))  # Top of the sea of clouds
    if y >= 70:  # Foreground grass
        return mm.HILLS_SH  # Grass
    if y >= cloud and y < cloud + 12:  # The sea of clouds in front of the lower slopes
        return mm.WHITE if (x // 5 + y) % 4 else lm.SNOW  # Soft white cloud
    if y >= top:  # Inside the mountain
        if y < top + 10:  # The grassy top
            return mm.PLAINS if x % 9 < 6 else mm.HILLS  # Dwarf-bamboo grass
        return mm.LEAF if (x + y) % 7 else PINE_DARK  # Pine forest below
    return sky(x, y)  # Sky


def pinatubo_pixel(x, y):
    """Mount Pinatubo: a crater lake in a wide grey rim, with ash gullies down the slopes."""
    d = abs(x - 59)  # Distance from the middle
    rim = 26 + int(max(0, d - 22) ** 1.3 * 0.22)  # Flat-topped mountain whose slopes fall away past the crater
    if d < 22:  # Inside the crater, the rim walls dip down to the lake
        rim = 26 + (22 - d) // 3 if d > 16 else 30  # Inner walls, then the lake level
    if y >= rim:  # Inside the mountain
        if d <= 16 and y < 33:  # The crater lake between the walls
            return mm.SKY_LIGHT if y == 30 else lm.LENS  # Light edge, then blue-green water
        if (x + int(y * 0.3)) % 11 == 0 and y > 40:  # Gullies cut by rain
            return mm.MASK_SH  # Gully
        return ASH if x < 59 else ASH_SH  # Ash slopes
    return sky(x, y)  # Sky


def matutum_pixel(x, y):
    """Mount Matutum: a tall green cone covered in forest, with rivers at its foot."""
    half = (y - 12) * 0.95  # The cone widens going down
    if y >= 70:  # Farmland at the foot
        return lm.LENS if abs((x % 40) - 20) < 2 else mm.HILLS_SH  # Rivers through the farmland
    if y >= 12 and abs(x - 59) <= half:  # Inside the cone
        if y < 18:  # Misty mossy top
            return mix(MOSS, mm.MASK)  # Grey-green
        return mm.LEAF if x < 59 or (x + y) % 5 == 0 else PINE_DARK  # Forest, sunny left
    return sky(x, y)  # Sky


def apo_pixel(x, y):
    """Mount Apo: the highest peak in the Philippines, mossy green, with a yellow sulfur patch on its side."""
    top, peak = lm.ridge(x, [(50, 10, 1.0), (82, 22, 0.9)])  # Main peak and a shoulder
    if y >= top:  # Inside the mountain
        if 30 <= y < 38 and 66 <= x < 76:  # Sulfur on the side
            return SULFUR if (x + y) % 3 else SULFUR_SH  # Yellow patch
        if y < top + 6:  # Bare rock near the top
            return lm.MOUNTAIN  # Rock
        return MOSS if x < peak[0] else PINE_DARK  # Mossy forest
    return sky(x, y)  # Sky


def everest_pixel(x, y):
    """Mount Everest: a snowy rock pyramid with a pale band of limestone near the summit."""
    top, peak = lm.ridge(x, [(64, 8, 1.25), (28, 30, 0.9), (100, 26, 0.9)])  # Everest and two neighbours
    if y >= top:  # Inside a mountain
        if peak[0] == 64 and 14 <= y < 18:  # The pale limestone band near the top
            return mix(mm.VEST, mm.WHITE)  # Cream band
        if y < top + 3 or y > top + 18 or lm.noise(x, y) < 20:  # Snow on the ridges, the lower fields and in patches
            return SNOW if x < peak[0] else SNOW_SH  # Sunny or shaded snow
        return lm.MOUNTAIN if x < peak[0] else lm.MOUNTAIN_SH  # Bare dark rock
    return sky(x, y)  # Sky


def lesson_with_puffs(pixel, puffs):
    """A framed lesson picture with white puffs drawn on top (steam, cloud or blowing snow)."""
    img = ls.framed(pixel)  # The picture
    for (cx, cy, r) in puffs:  # Each puff
        ls.disc(img, cx + 1, cy + 1, r, mm.MASK)  # Shade
        ls.disc(img, cx, cy, r, mm.WHITE)  # Puff
    return img


def main():
    themes = {  # Sprite name: drawing function, for the four new level themes and the new level objects
        "pine_top": pine_top, "pine_soil": pine_soil, "ledge_pine": ledge_pine, "hut": ranger_hut,
        "ash_top": ash_top, "ash_soil": ash_soil, "ledge_ash": ledge_ash, "bridge": stone_bridge,
        "moss_top": moss_top, "moss_soil": moss_soil, "ledge_moss": ledge_moss, "trail_sign": trail_sign,
        "snow_top": snow_top, "snow_soil": snow_soil, "ledge_snow": ledge_snow, "summit_flags": summit_flags,
        "rock_fall": rock_fall, "platform": platform,
    }
    for name, draw in themes.items():  # Each sprite
        la.save(draw(), name + ".png")  # Write it
    for ending, draw in (("_pine", pine_boulder), ("_ash", ash_boulder), ("_moss", moss_boulder), ("_snow", snow_boulder)):  # Each theme's boulders
        la.save(draw(32, 24), "boulder_small%s.png" % ending)  # Small boulder
        la.save(draw(48, 36), "boulder_big%s.png" % ending)  # Big boulder
    for i in range(4):  # Four-frame objects
        la.save(crumble(i), "crumble_%02d.png" % i)  # Crumbling ledge
        la.save(gust(i), "Props/gust_%02d.png" % i)  # Wind streak
    for i in range(2):  # Two-frame objects
        la.save(rock_warning(i), "rock_warn_%02d.png" % i)  # Landing shadow
        la.save(pebbles(i), "Props/pebbles_%02d.png" % i)  # Trickle of stones before a rockfall
    la.save(crumble_bit(), "Props/crumble_bit.png")  # Falling chip
    la.save(pine_tree(), "Props/pine_tree.png")  # Background pine
    la.save(fern(), "Props/fern.png")  # Fern
    la.save(sulfur_crust(), "Props/sulfur_crust.png")  # Sulfur around Apo's vents
    items = {"sandstone": sandstone, "clay": clay, "conglomerate": conglomerate, "shale": shale, "basalt": basalt,
             "magnetite": magnetite, "sulfur": sulfur, "pyrite": pyrite, "limestone": limestone, "gneiss": gneiss}  # The ten new kinds
    for name, draw in items.items():  # Each new kind
        ls.save(draw(), ls.ITEMS_OUT, name + ".png")  # Write it to the shared item folder
    ls.save(ls.framed(pulag_pixel), ls.LESSON_OUT, "lesson_pulag.png")  # Mount Pulag
    ls.save(ls.framed(pinatubo_pixel), ls.LESSON_OUT, "lesson_pinatubo.png")  # Mount Pinatubo
    ls.save(lesson_with_puffs(matutum_pixel, [(66, 14, 3), (72, 13, 3)]), ls.LESSON_OUT, "lesson_matutum.png")  # Mount Matutum with a cloud at its top
    ls.save(lesson_with_puffs(apo_pixel, [(74, 26, 3), (77, 21, 3), (81, 17, 2)]), ls.LESSON_OUT, "lesson_apo.png")  # Mount Apo with steam from the sulfur vents
    ls.save(lesson_with_puffs(everest_pixel, [(70, 9, 2), (75, 8, 3), (81, 8, 3), (87, 9, 2)]), ls.LESSON_OUT, "lesson_everest.png")  # Mount Everest with its snow plume
    print("Normal art written to", la.OUT, ls.ITEMS_OUT, "and", ls.LESSON_OUT)  # Tell the person running it where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
