"""Draw the Hills Hard tiles, mud, log, goals, props and lessons."""  # Describes the output
import math  # Gives angles for the log, blades and web
from pathlib import Path  # Gives file paths for each sprite
from PIL import Image, ImageDraw  # Draws pixels and labels the preview
import make_mainmenu_art as mm  # Gives the game palette and rectangles
import make_landform_map_art as lm  # Gives sky and water colours and repeatable noise
import make_level_art as la  # Gives outlines, boulders and single pixels
import make_lesson_art as ls  # Gives discs, polygons and output paths
import make_normal_art as na  # Gives shared tiles and the lesson sky

mix = na.mix  # Mixes two palette colours
LEVEL = Path(la.OUT)  # Holds the level sprites
PREVIEW = Path(mm.ROOT) / ".scratch" / "hills-hard" / "art-preview.png"  # Holds the review sheet
FOREST_SOIL = mix(mm.GROUND_SH, mm.BACKPACK_SH)  # Dark Bohol forest soil
FOREST_DARK = mix(mm.LEAF, mm.OUTLINE)  # Deep shade under the trees
KARST_LIGHT = mix(mm.MASK, mm.WHITE)  # Sunny Masungi limestone
KARST_SHADE = mix(mm.MASK_SH, mm.STRAP)  # Limestone in the pits
WIND_LIGHT = mix(mm.PLAINS, mm.HEADLAMP)  # Sun on windy grass tips
EMERALD = mix(mm.HILLS, mm.LEAF)  # Deep Irish green grass
TILL = mix(mm.GROUND_SH, mm.MASK_SH)  # Stony glacial clay under a drumlin
TUFF = mix(mm.PLATEAU, mm.VEST)  # Warm Roman tuff
MOSS_GREY = mix(mm.MASK_SH, mm.LEAF)  # Mossy grey river rock
PATH = mix(mm.VEST, mm.MASK)  # Pale stone riverside path
MUD = mix(mm.GROUND_SH, mm.STRAP)  # Wet grey-brown mud
MUD_DARK = mix(mm.BACKPACK_SH, mm.OUTLINE)  # Deep soaked mud
MUD_DIM = mix(mm.MASK_SH, mm.GROUND_SH)  # Soft shine on the mud
GLOSS = mix(mm.SKY_LIGHT, mm.MASK)  # Bright wet shine on the mud
BARK = mm.GROUND_SH  # Log bark
BARK_DARK = mm.BACKPACK_SH  # Bark in shade
WOOD = mm.VEST  # Cut wood and rope
WOOD_SHADE = mm.VEST_SH  # Wood away from the light
POST = lm.HANDLE  # Light post wood
FUR = mix(mm.VEST, mm.MASK_SH)  # Soft grey-tan tarsier fur
FUR_LIGHT = mix(mm.VEST, mm.HEADLAMP)  # Lit fur on the hands
TURBINE = mix(mm.WHITE, mm.MASK)  # White turbine paint
MARBLE = mix(mm.HEADLAMP, mm.MASK)  # Pale old stone
MARBLE_SHADE = mix(mm.MASK, mm.VEST)  # Old stone in shade
BAMBOO = mix(mm.VEST, mm.PLAINS)  # Yellow-green bamboo
BAMBOO_SHADE = mix(mm.VEST_SH, mm.HILLS_SH)  # Bamboo in shade
WATER = lm.LENS  # Sunlit river and sea
WATER_MID = mix(lm.LENS, lm.MOUNTAIN_SH)  # Water away from the light
FOAM = mix(mm.SKY_LIGHT, mm.WHITE)  # Pale foam and mist
WOOL = mix(mm.WHITE, mm.HEADLAMP)  # Sheep wool
THATCH = mix(mm.VEST, mm.HELMET_SH)  # Woven roof
HAZE_GREEN = mix(lm.HAZE, mm.HILLS_SH)  # Far green hills in haze

THEMES = {  # Each row has floor, grass, light, shade, dark speck and bright stone
    "tarsier": (FOREST_SOIL, mm.LEAF, mm.HILLS_SH, FOREST_DARK, mix(mm.BACKPACK_SH, mm.OUTLINE), mm.VEST_SH),  # Leaf litter over forest soil
    "slope": (mm.PLATEAU_SH, mm.HILLS, mm.PLAINS, mm.HILLS_SH, mm.BACKPACK_SH, mm.PLATEAU),  # Grass over red-brown clay
    "karst": (mm.MASK_SH, mm.MASK, KARST_LIGHT, KARST_SHADE, mm.STRAP, mm.HEADLAMP),  # Grey jagged limestone
    "wind": (mm.GROUND, mm.PLAINS, WIND_LIGHT, mm.HILLS, mm.GROUND_SH, mm.VEST),  # Light windy grass on the ridge
    "drumlin": (TILL, EMERALD, mm.HILLS, mm.LEAF, mm.STRAP, mm.MASK),  # Irish green over stony clay
    "city": (TUFF, mm.MASK_SH, mm.MASK, mm.STRAP, mm.PLATEAU_SH, mm.HEADLAMP),  # Cobbles over tuff
    "tower": (MOSS_GREY, PATH, mm.HEADLAMP, mm.VEST_SH, mm.STRAP, mm.HILLS_SH),  # Pale path over mossy rock
}  # Ends the theme colours

TOP_FLECKS = {  # Small marks on each top tile
    "tarsier": [(2, 1, mm.HELMET_SH), (3, 2, mm.PLATEAU), (7, 1, mm.VEST), (10, 2, mm.HELMET_SH), (11, 2, mm.PLATEAU_SH), (14, 1, mm.VEST), (5, 3, FOREST_DARK)],  # Fallen leaves
    "slope": [(3, 2, mm.HILLS_SH), (8, 1, mm.PLAINS), (12, 3, mm.PLATEAU), (13, 3, mm.PLATEAU), (6, 3, mm.HILLS_SH)],  # Grass with clay showing
    "karst": [(2, 1, mm.STRAP), (3, 1, KARST_SHADE), (7, 2, mm.STRAP), (11, 1, mm.STRAP), (12, 2, KARST_SHADE), (5, 3, mm.HILLS_SH), (14, 3, mm.HILLS_SH)],  # Sharp pits and moss
    "wind": [(1, 2, mm.HILLS), (2, 1, mm.HILLS), (6, 2, mm.HILLS), (7, 1, mm.HILLS), (11, 2, mm.HILLS), (12, 1, mm.HILLS), (9, 3, WIND_LIGHT)],  # Blades bent by wind
    "drumlin": [(2, 2, mm.HILLS), (5, 1, mm.PLAINS), (9, 2, mm.LEAF), (10, 3, mm.MASK), (11, 3, mm.MASK), (14, 1, mm.HILLS)],  # Grass with a stone peeking
    "tower": [(1, 2, mm.HILLS_SH), (6, 1, mm.VEST_SH), (12, 2, mm.HILLS_SH), (13, 1, mm.HILLS)],  # Worn path with moss
}  # Ends the top marks

SOIL_EXTRA = {  # Extra pixels in each soil tile
    "tarsier": [(x, 6 + la.wave(x, 1), mm.VEST_SH) for x in range(2, 10)],  # A pale root
    "slope": [(3, 5, mm.PLATEAU), (4, 5, mm.PLATEAU), (11, 11, mm.PLATEAU)],  # Lighter clay lumps
    "karst": [(4, 4, KARST_LIGHT), (10, 9, KARST_LIGHT), (11, 9, mm.MASK)],  # Pale chips
    "wind": [(x, 9, mm.GROUND_SH) for x in range(16)],  # A soil layer line
    "drumlin": [(3, 4, mm.MASK), (4, 4, mm.MASK), (4, 5, mm.MASK_SH), (11, 10, mm.MASK), (12, 10, mm.MASK), (12, 11, mm.MASK_SH), (7, 14, mm.MASK)],  # Stones in glacial clay
    "city": [(3, 5, mm.HEADLAMP), (11, 11, mm.PLATEAU_SH), (12, 11, mm.HEADLAMP)],  # Light tuff grains
    "tower": [(3, 5, mm.HILLS_SH), (4, 5, mm.HILLS_SH), (11, 11, mm.MASK)],  # Moss and pale stone
}  # Ends the soil pixels

BOULDERS = {  # Each row has rock, shade, light, speck, cap and cap rows
    "tarsier": (mix(mm.MASK_SH, mm.GROUND), mm.GROUND_SH, mix(mm.MASK, mm.VEST), mm.BACKPACK_SH, mm.LEAF, 3),  # Brown forest rock with moss
    "slope": (mm.PLATEAU, mm.PLATEAU_SH, mix(mm.PLATEAU, mm.HEADLAMP), mm.BACKPACK_SH, mm.HILLS, 2),  # Clay lump with grass
    "karst": (mm.MASK, mm.MASK_SH, KARST_LIGHT, mm.STRAP, KARST_LIGHT, 1),  # Pale sharp limestone
    "wind": (mm.VEST, mm.VEST_SH, mm.HEADLAMP, mm.GROUND_SH, mm.PLAINS, 2),  # Tan rock with light grass
    "drumlin": (mm.MASK_SH, mm.STRAP, mm.MASK, FOREST_DARK, EMERALD, 3),  # Grey stone with deep green turf
    "city": (TUFF, mm.PLATEAU_SH, mix(mm.VEST, mm.HEADLAMP), mm.GROUND_SH, mix(mm.VEST, mm.HEADLAMP), 1),  # Warm tuff block
    "tower": (mm.MASK_SH, MOSS_GREY, mm.MASK, mm.STRAP, mm.HILLS_SH, 3),  # Grey river rock with moss
}  # Ends the boulder colours


def cobble_top():  # Draws the Roman cobble path top
    base, cap, light, shade, dark, stone = THEMES["city"]  # Reads the city palette
    img = mm.new_image(16, 8)  # Starts the tile
    mm.fill_rect(img, 0, 0, 16, 8, base)  # Lays the tuff below
    mm.fill_rect(img, 0, 0, 16, 5, shade)  # Fills the dark gaps between cobbles
    for x, y, w in ((0, 0, 4), (5, 0, 4), (10, 0, 5), (2, 3, 5), (8, 3, 5), (14, 3, 2), (0, 3, 1)):  # Places two rows of cobbles
        mm.fill_rect(img, x, y, w, 2, cap)  # Draws one cobble
        la.put(img, x, y, light)  # Lights its top-left corner
    mm.fill_rect(img, 0, 5, 16, 1, dark)  # Shades under the cobbles
    return img  # Gives back the tile


def set_tiles(theme):  # Draws each theme's five ground pieces
    base, cap, light, shade, dark, stone = THEMES[theme]  # Reads the palette
    if theme == "city":  # Uses cobbles on the Rome path
        yield f"{theme}_top", cobble_top()  # Draws the cobble walking line
    else:  # Uses a grass or rock cap elsewhere
        yield f"{theme}_top", na.ground_top(base, cap, light, shade, TOP_FLECKS[theme])  # Frames the walking line
    yield f"{theme}_soil", na.soil_tile(base, dark, SOIL_EXTRA[theme])  # Places small chips
    yield f"ledge_{theme}", na.ledge_tile(base, cap, dark, [(4, 4, shade), (10, 5, dark), (13, 3, stone)])  # Gives the ledge a clear lip
    for label, w, h in (("small", 32, 24), ("big", 48, 36)):  # Makes both boulder sizes
        yield f"boulder_{label}_{theme}", na.capped_boulder(w, h, *BOULDERS[theme])  # Puts a cap above the rock


def mud_top():  # Draws dark glossy mud that replaces a top tile
    img = mm.new_image(16, 8)  # Starts the tile
    mm.fill_rect(img, 0, 0, 16, 8, MUD_DARK)  # Soaks the body in dark mud
    mm.fill_rect(img, 0, 0, 16, 4, MUD)  # Lays a wet surface
    mm.fill_rect(img, 0, 0, 16, 1, MUD_DIM)  # Shows the wet top line
    for x, y, w, c in ((2, 1, 3, GLOSS), (9, 1, 2, GLOSS), (5, 2, 1, MUD_DIM), (12, 2, 2, MUD_DIM), (14, 1, 1, GLOSS)):  # Spreads shiny streaks
        mm.fill_rect(img, x, y, w, 1, c)  # Draws one shine
    for x, y in ((0, 3), (7, 3), (8, 2), (15, 3)):  # Adds soft puddle dips
        la.put(img, x, y, MUD_DARK)  # Darkens one dip
    mm.fill_rect(img, 0, 4, 16, 1, mm.OUTLINE)  # Draws a strong shadow line
    for x, y in ((3, 6), (11, 5), (12, 6)):  # Adds wet specks below
        la.put(img, x, y, MUD)  # Paints one speck
    return img  # Gives back the tile


def mud_splash(frame):  # Draws mud flicked up while sliding
    img = mm.new_image(10, 6)  # Fits under the feet
    drops = (((3, 4), (5, 3), (6, 4), (4, 5), (7, 5)), ((1, 3), (2, 1), (7, 1), (8, 3), (5, 0), (4, 2)))[frame]  # Places this frame's drops
    for i, (x, y) in enumerate(drops):  # Paints each drop
        la.put(img, x, y, MUD_DARK if i % 2 else MUD)  # Gives the drop a mud colour
    la.put(img, drops[1][0], drops[1][1], GLOSS)  # Shines one wet drop
    return img  # Leaves the splash unoutlined


def rolling_log(frame):  # Draws a round log end that turns as it rolls left
    img = mm.new_image(16, 16)  # Matches the rolling boulder size
    for y in range(16):  # Visits each row
        for x in range(16):  # Visits each pixel
            d = math.hypot(x - 7.5, y - 7.5)  # Measures from the middle
            lit = (x - 7.5) * 0.7 + (y - 7.5) < 0  # Marks the top-left half
            if d > 7.2:  # Skips outside the log
                continue  # Leaves it clear
            if d > 5.4:  # Paints the bark ring
                la.put(img, x, y, BARK if lit else BARK_DARK)  # Shades the bark
            elif 2.6 < d < 3.6:  # Paints one growth ring
                la.put(img, x, y, WOOD_SHADE)  # Draws the ring line
            elif d < 1:  # Paints the dark middle
                la.put(img, x, y, BARK)  # Marks the pith
            else:  # Paints the cut wood
                la.put(img, x, y, mm.HEADLAMP if lit and d > 3.6 else WOOD)  # Lights the top-left wood
    a = -frame * math.pi / 4  # Turns left by one eighth each frame
    for t in range(-7, 8):  # Walks across the log
        px, py = math.floor(7.5 + math.cos(a) * t), math.floor(7.5 + math.sin(a) * t)  # Finds the line pixel without gaps
        la.put(img, px, py, mm.OUTLINE if abs(t) <= 5 else mm.HEADLAMP)  # Draws a crack with pale bark notches
    return la.outline(img)  # Gives the log a dark edge


def log_warning(frame):  # Draws leaves and twigs falling before a log comes
    img = mm.new_image(24, 4)  # Matches the rock warning size
    for x, y, c, w in ((2, 0, mm.HILLS_SH, 1), (6, 2, BARK, 3), (11, 1, mm.HELMET_SH, 1), (15, 3, mm.HILLS_SH, 1), (18, 0, BARK, 2), (21, 2, mm.PLATEAU, 1)):  # Lists leaves and twigs
        mm.fill_rect(img, (x + frame * 2) % 23, (y + frame) % 4, w, 1, c)  # Drops each piece a little in frame one
    return img  # Leaves the pieces unoutlined


TARSIER = (  # Letter map of a tarsier clinging to a branch
    [".f.....f..wb", "fff...fff.wb", ".fffffff..wb", "fEEEfEEEf.wb", "fEhpfEhpf.wb", "fEppfEppf.wb",  # Ears and big eyes
     ".fffmffff.wb", "..ffffffkkwb", "..sfffff..wb", "..sfffffkkwb", "..ss.ssk..wb", "...s......wb"],  # Body, hands and tail
)  # Ends the open-eye map
TARSIER_COLOURS = {"f": FUR, "s": mm.VEST_SH, "k": FUR_LIGHT, "E": mm.HELMET, "p": mm.OUTLINE, "h": mm.WHITE, "m": mm.DARK, "w": POST, "b": BARK}  # Colours by letter


def from_map(rows, colours):  # Paints a sprite from a letter map
    img = mm.new_image(len(rows[0]), len(rows))  # Sizes the sprite to the map
    for y, row in enumerate(rows):  # Visits each row
        assert len(row) == img.width, row  # Catches a wrong row length
        for x, ch in enumerate(row):  # Visits each letter
            if ch in colours:  # Skips the clear dots
                la.put(img, x, y, colours[ch])  # Paints the pixel
    return img  # Gives back the sprite


def tarsier(frame):  # Draws a friendly tarsier that blinks
    rows = list(TARSIER[0])  # Copies the open-eye map
    if frame:  # Closes the eyes in frame one
        rows[3:6] = ["fffffffff.wb", "fpppfpppf.wb", "fffffffff.wb"]  # Draws calm closed eyes
    return la.outline(from_map(rows, TARSIER_COLOURS))  # Outlines the tarsier


def tarsier_lookout():  # Draws a forest viewing deck with a tarsier on a tree
    img = mm.new_image(56, 48)  # Fits the goal size
    mm.fill_rect(img, 10, 10, 5, 38, BARK)  # Raises the tree trunk
    mm.fill_rect(img, 10, 10, 1, 38, POST)  # Lights the trunk's left edge
    for x, y, r in ((6, 6, 6), (14, 4, 6), (20, 9, 4)):  # Places the leaf clumps
        ls.disc(img, x, y, r, mm.LEAF)  # Fills one clump
        ls.disc(img, x - 1, y - 2, r - 3, mm.HILLS_SH)  # Lights its top-left
    for x in (21, 51):  # Sets the deck legs
        mm.fill_rect(img, x, 24, 3, 24, WOOD_SHADE)  # Draws one leg
    ls.fill_poly(img, [(18, 13), (26, 5), (48, 5), (55, 13)], THATCH)  # Covers the deck with a roof
    ImageDraw.Draw(img).line([(19, 12), (26, 6)], fill=mm.HEADLAMP)  # Lights the roof edge
    for x in (21, 36, 51):  # Places the rail posts
        mm.fill_rect(img, x, 13, 2, 10, WOOD_SHADE)  # Draws one post
    mm.fill_rect(img, 20, 16, 34, 1, WOOD)  # Draws the hand rail
    mm.fill_rect(img, 18, 22, 37, 3, mm.GROUND)  # Lays the deck floor
    mm.fill_rect(img, 18, 22, 37, 1, mm.HEADLAMP)  # Lights the deck top
    for y in range(27, 47, 4):  # Spaces the ladder rungs
        mm.fill_rect(img, 31, y, 8, 1, WOOD)  # Draws one rung
    mm.fill_rect(img, 30, 25, 1, 23, WOOD_SHADE)  # Draws the left ladder rail
    mm.fill_rect(img, 39, 25, 1, 23, WOOD_SHADE)  # Draws the right ladder rail
    out = la.outline(img)  # Outlines the deck and tree
    mm.paste(out, tarsier(0), 1, 22)  # Hangs a tarsier on the trunk
    return out  # Gives back the goal


def barangay_hall():  # Draws a small barangay hall with a green safe-place sign
    img = mm.new_image(56, 48)  # Fits the goal size
    mm.fill_rect(img, 6, 21, 44, 27, KARST_LIGHT)  # Builds the pale walls
    mm.fill_rect(img, 34, 21, 16, 27, mm.MASK)  # Shades the right wall
    ls.fill_poly(img, [(2, 22), (12, 12), (44, 12), (54, 22)], mm.SCARF_SH)  # Sets the red roof
    ls.fill_poly(img, [(3, 21), (12, 13), (28, 13), (24, 21)], mm.SCARF)  # Lights the left roof face
    mm.fill_rect(img, 23, 31, 10, 17, mm.DARK)  # Opens the door
    for x in (10, 39):  # Places the two windows
        mm.fill_rect(img, x, 26, 7, 6, mm.SKY_LIGHT)  # Draws one window
        mm.fill_rect(img, x + 3, 26, 1, 6, mm.STRAP)  # Splits the window
    mm.fill_rect(img, 20, 45, 16, 3, mm.MASK_SH)  # Adds the front step
    for x in (21, 34):  # Places the sign legs
        mm.fill_rect(img, x, 9, 1, 4, mm.STRAP)  # Holds the sign on the roof
    mm.fill_rect(img, 17, 0, 22, 10, mm.HILLS_SH)  # Draws the green sign board
    mm.fill_rect(img, 17, 0, 22, 1, mm.HILLS)  # Lights the sign top
    ls.fill_poly(img, [(23, 5), (28, 1), (33, 5)], mm.WHITE)  # Draws the white house roof icon
    mm.fill_rect(img, 24, 5, 9, 4, mm.WHITE)  # Draws the white house body
    mm.fill_rect(img, 27, 6, 3, 3, mm.HILLS_SH)  # Opens the icon door
    return la.outline(img)  # Outlines the hall


def jagged_rock(img, points, light, shade, cut):  # Paints a grey limestone rock with a shaded side
    ls.fill_poly(img, points, light)  # Fills the rock
    ls.fill_poly(img, [(x, y) for x, y in points if x >= cut] + [(cut, max(y for _, y in points))], shade)  # Shades the far side
    for x in range(max(0, min(p[0] for p in points) + 2), min(img.width, max(p[0] for p in points)), 4):  # Spaces the rain grooves
        for y in range(img.height):  # Walks down the rock
            if ls.inside(x + 0.5, y + 0.5, points) and lm.noise(x, y // 3) > 25:  # Stays inside the rock with a few breaks
                la.put(img, x, y, mm.MASK_SH if x < cut else mm.STRAP)  # Cuts one groove pixel


def web_bridge():  # Draws the Masungi rope web strung between two rocks
    img = mm.new_image(56, 48)  # Fits the goal size
    left = [(0, 47), (0, 10), (3, 4), (5, 8), (7, 1), (10, 9), (13, 6), (14, 20), (12, 30), (15, 40), (15, 47)]  # Shapes the left rock
    jagged_rock(img, left, mm.MASK, mm.MASK_SH, 9)  # Paints the left rock
    jagged_rock(img, [(55 - x, y) for x, y in left], mm.MASK, mm.MASK_SH, 50)  # Paints the right rock
    img = la.outline(img)  # Outlines both rocks
    cx, cy = 28, 22  # Sets the web middle
    anchors = [(9, 5), (4, 22), (10, 40), (28, 44), (46, 40), (51, 22), (46, 5)]  # Ties the ropes to the rocks
    anchors.sort(key=lambda p: math.atan2(p[1] - cy, p[0] - cx))  # Orders the ropes around the middle
    draw = ImageDraw.Draw(img)  # Gives a rope pen
    for ax, ay in anchors:  # Draws each spoke
        draw.line([(cx, cy + 1), (ax, ay + 1)], fill=mm.DARK)  # Shades under the spoke
        draw.line([(cx, cy), (ax, ay)], fill=WOOD)  # Draws the spoke rope
    for f in (0.35, 0.7):  # Draws two rings of rope
        ring = [(round(cx + (ax - cx) * f), round(cy + (ay - cy) * f)) for ax, ay in anchors]  # Finds the ring knots
        draw.line([(x, y + 1) for x, y in ring + ring[:1]], fill=mm.DARK)  # Shades under the ring
        draw.line(ring + ring[:1], fill=mm.HEADLAMP if f < 0.5 else WOOD)  # Draws the ring rope
    ls.disc(img, cx, cy, 1, mm.HEADLAMP)  # Lights the centre knot
    return img  # Gives back the goal


def blade(cx, cy, angle, length, width):  # Gives the corners of a tapering turbine blade
    a = math.radians(angle)  # Turns degrees into radians
    dx, dy, nx, ny = math.cos(a), math.sin(a), -math.sin(a), math.cos(a)  # Finds along and across
    return [(cx + nx * width, cy + ny * width), (cx + dx * length + nx, cy + dy * length + ny), (cx + dx * length - nx, cy + dy * length - ny), (cx - nx * width, cy - ny * width)]  # Lists the four corners


def turbine_base():  # Draws a white turbine tower with blades cut off at the top
    img = mm.new_image(56, 64)  # Fits the tall goal
    ls.fill_poly(img, [(23, 60), (33, 60), (30, 11), (26, 11)], TURBINE)  # Raises the tapered tower
    ls.fill_poly(img, [(28, 60), (33, 60), (30, 11), (28, 11)], mm.MASK)  # Shades the right side
    mm.fill_rect(img, 26, 52, 4, 8, mm.MASK_SH)  # Adds the service door
    for angle in (-105, 15, 135):  # Spaces three blades
        ls.fill_poly(img, blade(26, 8, angle, 40, 2.5), TURBINE)  # Draws one long blade
    mm.fill_rect(img, 24, 4, 12, 7, TURBINE)  # Draws the nacelle
    mm.fill_rect(img, 24, 9, 12, 2, mm.MASK)  # Shades the nacelle bottom
    ls.disc(img, 25, 7, 2, mm.WHITE)  # Draws the hub
    mm.fill_rect(img, 14, 60, 28, 4, mm.MASK_SH)  # Pours the concrete pad
    mm.fill_rect(img, 14, 60, 28, 1, mm.MASK)  # Lights the pad top
    return la.outline(img)  # Outlines the turbine


def stone_pier():  # Draws an Irish stone pier with a lamp and a small boat
    img = mm.new_image(56, 48)  # Fits the goal size
    mm.fill_rect(img, 0, 38, 56, 10, WATER_MID)  # Fills the bay water
    for x, y, w in ((2, 41, 8), (14, 44, 6), (4, 46, 9)):  # Spaces the ripples
        mm.fill_rect(img, x, y, w, 1, WATER)  # Draws one ripple
    mm.fill_rect(img, 22, 24, 34, 22, mm.STRAP)  # Fills the gaps in the stones
    for row, y in enumerate(range(24, 46, 4)):  # Stacks the stone rows
        for x in range(22 + (row % 2) * 4 - 4, 56, 8):  # Offsets each row of blocks
            mm.fill_rect(img, max(22, x), y, min(7, x + 7 - 22) if x < 22 else 7, 3, mm.MASK_SH)  # Draws one block
            la.put(img, max(22, x), y, mm.MASK)  # Lights its corner
    mm.fill_rect(img, 22, 24, 34, 2, mm.MASK)  # Lights the walking top
    mm.fill_rect(img, 46, 19, 5, 5, mm.STRAP)  # Sets the mooring post
    mm.fill_rect(img, 45, 18, 7, 2, mm.MASK_SH)  # Caps the mooring post
    mm.fill_rect(img, 30, 6, 2, 18, mm.STRAP)  # Raises the lamp post
    mm.fill_rect(img, 28, 1, 6, 6, mm.HEADLAMP)  # Lights the lamp
    mm.fill_rect(img, 28, 0, 6, 1, mm.STRAP)  # Caps the lamp
    ls.fill_poly(img, [(1, 35), (19, 35), (17, 40), (4, 40)], mm.STRAP)  # Floats a dark currach
    mm.fill_rect(img, 2, 35, 17, 1, mm.MASK_SH)  # Lights the boat rim
    ImageDraw.Draw(img).line([(18, 36), (46, 21)], fill=WOOD)  # Ties the boat to the post
    return la.outline(img)  # Outlines the pier


def temple_ruins():  # Draws broken Roman columns on stone steps
    img = mm.new_image(56, 48)  # Fits the goal size
    mm.fill_rect(img, 1, 43, 54, 5, MARBLE_SHADE)  # Lays the lower step
    mm.fill_rect(img, 4, 40, 48, 3, MARBLE)  # Lays the upper step
    for x, top in ((6, 9), (19, 9), (32, 18), (44, 28)):  # Places columns from tall to broken
        mm.fill_rect(img, x, top, 7, 40 - top, MARBLE)  # Raises the shaft
        mm.fill_rect(img, x + 5, top, 2, 40 - top, MARBLE_SHADE)  # Shades the right side
        mm.fill_rect(img, x + 3, top + 2, 1, 37 - top, mm.VEST)  # Cuts a flute line
        if top < 15:  # Gives whole columns a capital
            mm.fill_rect(img, x - 1, top - 2, 9, 2, mm.HEADLAMP)  # Draws the capital
        else:  # Breaks the top of short columns
            for i, cut in enumerate((0, 2, 1, 3, 0, 2, 1)):  # Varies the break
                img.paste((0, 0, 0, 0), (x + i, top, x + i + 1, top + cut))  # Clears the broken pixels
    ls.fill_poly(img, [(3, 3), (28, 3), (30, 5), (27, 7), (3, 7)], MARBLE)  # Lays a broken beam on the tall columns
    mm.fill_rect(img, 3, 3, 25, 1, mm.HEADLAMP)  # Lights the beam top
    mm.fill_rect(img, 3, 6, 24, 1, MARBLE_SHADE)  # Shades the beam bottom
    return la.outline(img)  # Outlines the ruins


def bamboo_raft(w):  # Draws a bamboo raft with a raised front
    img = mm.new_image(w, 8)  # Fits the raft height
    mm.fill_rect(img, 0, 4, w - 4, 3, BAMBOO)  # Lays the bamboo poles
    mm.fill_rect(img, 0, 6, w - 4, 1, BAMBOO_SHADE)  # Shades the lowest pole
    ls.fill_poly(img, [(w - 5, 4), (w - 1, 1), (w, 2), (w - 3, 7), (w - 5, 7)], BAMBOO)  # Bends the front upward
    for x in range(3, w - 5, 5):  # Spaces the bamboo joints
        mm.fill_rect(img, x, 4, 1, 3, BAMBOO_SHADE)  # Draws one joint
    return img  # Gives back the raft


def raft_dock():  # Draws a bamboo raft moored at a small dock on the Li River
    img = mm.new_image(56, 48)  # Fits the goal size
    mm.fill_rect(img, 0, 38, 56, 10, WATER_MID)  # Fills the river
    for x, y, w in ((24, 44, 10), (40, 46, 9), (5, 45, 6)):  # Spaces the ripples
        mm.fill_rect(img, x, y, w, 1, WATER)  # Draws one ripple
    for x in (3, 17):  # Sets the dock legs
        mm.fill_rect(img, x, 30, 3, 17, WOOD_SHADE)  # Drives one leg into the river
    mm.fill_rect(img, 0, 29, 23, 3, mm.GROUND)  # Lays the dock boards
    mm.fill_rect(img, 0, 29, 23, 1, mm.HEADLAMP)  # Lights the boards
    mm.paste(img, bamboo_raft(32), 23, 32)  # Floats the raft beside the dock
    mm.fill_rect(img, 40, 16, 1, 21, WOOD_SHADE)  # Raises the shade pole
    ls.fill_poly(img, [(30, 19), (36, 13), (45, 13), (51, 19)], mm.SCARF)  # Opens the shade umbrella
    ls.fill_poly(img, [(40, 13), (45, 13), (51, 19), (41, 19)], mm.SCARF_SH)  # Shades its right half
    mm.fill_rect(img, 31, 32, 6, 4, WOOD)  # Sets a bamboo seat
    ImageDraw.Draw(img).line([(19, 30), (25, 36)], fill=WOOD)  # Ties the raft to the dock
    return la.outline(img)  # Outlines the dock and raft


def vine():  # Draws a hanging forest vine
    img = mm.new_image(12, 24)  # Fits the prop
    for y in range(24):  # Walks down the stem
        la.put(img, 5 + round(1.5 * math.sin(y / 3.5)), y, mm.LEAF)  # Draws the wavy stem
    for i, y in enumerate(range(2, 22, 4)):  # Spaces the leaves
        x = 5 + round(1.5 * math.sin(y / 3.5)) + (1 if i % 2 else -4)  # Alternates the side
        mm.fill_rect(img, x, y, 4, 2, mm.HILLS_SH)  # Draws one leaf
        la.put(img, x + (3 if i % 2 else 0), y, mm.HILLS)  # Lights its tip
    return la.outline(img)  # Outlines the vine


def warning_sign():  # Draws a yellow triangle with a falling-rock icon
    img = mm.new_image(16, 20)  # Fits the prop
    mm.fill_rect(img, 7, 12, 2, 8, mm.STRAP)  # Sets the post
    ls.fill_poly(img, [(8, 0), (16, 14), (0, 14)], mm.SCARF_SH)  # Draws the red border
    ls.fill_poly(img, [(8, 3), (13.5, 12.5), (2.5, 12.5)], mm.HELMET)  # Fills the yellow face
    ls.fill_poly(img, [(11, 7), (12.5, 12), (7, 12)], mm.OUTLINE)  # Draws the slope icon
    mm.fill_rect(img, 5, 8, 2, 2, mm.OUTLINE)  # Draws a falling rock
    la.put(img, 7, 6, mm.OUTLINE)  # Draws a small chip
    return la.outline(img)  # Outlines the sign


def cracked_ground():  # Draws an open crack across the ground
    img = mm.new_image(24, 6)  # Fits the decal
    draw = ImageDraw.Draw(img)  # Gives a crack pen
    main = [(0, 2), (4, 3), (8, 1), (12, 3), (16, 2), (20, 4), (23, 3)]  # Zigzags the main crack
    draw.line([(x, y + 1) for x, y in main], fill=mm.DARK)  # Shows the crack's depth
    draw.line(main, fill=mm.OUTLINE)  # Draws the crack opening
    draw.line([(8, 1), (9, 5)], fill=mm.OUTLINE)  # Branches one crack down
    draw.line([(16, 2), (17, 0)], fill=mm.OUTLINE)  # Branches one crack up
    return img  # Leaves the crack unoutlined


def tilted_post():  # Draws a fence post leaning downhill
    img = mm.new_image(10, 20)  # Fits the prop
    ls.fill_poly(img, [(1, 19), (5, 19), (9, 2), (6, 1)], mm.GROUND_SH)  # Leans the post
    ls.fill_poly(img, [(1, 19), (3, 19), (7, 2), (6, 1)], POST)  # Lights the left face
    mm.fill_rect(img, 0, 18, 7, 2, mm.GROUND)  # Heaps soil at the foot
    return la.outline(img)  # Outlines the post


def karst_spike():  # Draws a sharp grey limestone point
    img = mm.new_image(12, 16)  # Fits the prop
    jagged_rock(img, [(0, 16), (2, 9), (3, 10), (5, 2), (6, 0), (7, 4), (9, 7), (10, 6), (12, 16)], mm.MASK, mm.MASK_SH, 7)  # Paints the point
    mm.fill_rect(img, 1, 14, 3, 2, mm.HILLS_SH)  # Tucks moss at the base
    return la.outline(img)  # Outlines the point


def rope_rail():  # Draws two posts with sagging rope
    img = mm.new_image(32, 10)  # Fits the prop
    for x in (1, 29):  # Places the two posts
        mm.fill_rect(img, x, 1, 2, 9, mm.GROUND_SH)  # Draws one post
        la.put(img, x, 1, POST)  # Lights its top
    draw = ImageDraw.Draw(img)  # Gives a rope pen
    for y in (2, 6):  # Hangs two ropes
        draw.line([(3, y), (9, y + 1), (22, y + 1), (28, y)], fill=WOOD)  # Sags the rope
    return la.outline(img)  # Outlines posts and ropes


def far_outline(img, colour):  # Outlines a far sprite with a soft colour
    shape = [(x, y) for y in range(img.height) for x in range(img.width) if img.getpixel((x, y))[3]]  # Finds the sprite pixels
    for x, y in shape:  # Visits each one
        for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):  # Checks its neighbours
            if 0 <= x + dx < img.width and 0 <= y + dy < img.height and not img.getpixel((x + dx, y + dy))[3]:  # Finds an empty neighbour
                la.put(img, x + dx, y + dy, colour)  # Paints the soft edge
    return img  # Gives back the sprite


def turbine_far(frame):  # Draws a far turbine with turning blades
    img = mm.new_image(24, 48)  # Fits the prop
    ls.fill_poly(img, [(11, 47), (13, 47), (12.5, 12), (11.5, 12)], TURBINE)  # Raises the thin tower
    draw = ImageDraw.Draw(img)  # Gives a blade pen
    for k in range(3):  # Draws three blades
        a = math.radians(-90 + frame * 60 + k * 120)  # Turns the blades by a sixth each frame
        draw.line([(12, 11), (12 + 10 * math.cos(a), 11 + 10 * math.sin(a))], fill=TURBINE)  # Draws one blade
    la.put(img, 12, 11, mm.WHITE)  # Draws the hub
    return far_outline(img, mm.MASK_SH)  # Gives a soft grey edge


def wind_grass(frame):  # Draws grass bent by the wind
    img = mm.new_image(12, 10)  # Fits the prop
    draw = ImageDraw.Draw(img)  # Gives a blade pen
    for x, tip in ((1, 3), (3, 1), (6, 2), (8, 4)):  # Staggers the blades
        bend = 2 + frame * 2  # Bends further in frame one
        draw.line([(x, 9), (x + 1, 5), (min(11, x + bend), tip + frame)], fill=mm.HILLS)  # Draws one blade
        la.put(img, min(11, x + bend), tip + frame, WIND_LIGHT)  # Lights the tip
    return img  # Keeps the thin blades unoutlined


def stone_wall():  # Draws an Irish dry stone wall
    img = mm.new_image(32, 10)  # Fits the prop
    mm.fill_rect(img, 0, 1, 32, 9, mm.STRAP)  # Fills the gaps between stones
    for x, y, w, h in ((0, 5, 6, 4), (7, 6, 5, 3), (13, 5, 7, 4), (21, 6, 5, 3), (27, 5, 5, 4), (1, 1, 5, 3), (7, 2, 6, 3), (14, 1, 4, 3), (19, 2, 6, 3), (26, 1, 5, 3)):  # Stacks the stones
        mm.fill_rect(img, x, y, w, h, mm.MASK_SH)  # Draws one stone
        mm.fill_rect(img, x, y, w - 1, 1, mm.MASK)  # Lights its top
    return la.outline(img)  # Outlines the wall


SHEEP = (  # Letter maps for a grazing sheep
    ["................", ".....ww.....dd..", "...wwwwww..dddd.", "..wwwwwwwwwdedd.", ".wwwwwwwwwwwddd.", ".wwwwwwwwwwww...",  # Head up
     ".swwwwwwwwwws...", "..sswwwwwwss....", "...ssssssss.....", "...dd.....dd....", "...dd.....dd....", "...dd.....dd...."],  # Legs
    ["................", ".....ww.........", "...wwwwww.......", "..wwwwwwwww.....", ".wwwwwwwwwwww...", ".wwwwwwwwwwwdd..",  # Head down
     ".swwwwwwwwwwddd.", "..sswwwwwwssdedd", "...sssssssss.ddd", "...dd.....dd....", "...dd.....dd....", "...dd.....dd...."],  # Legs
)  # Ends the sheep maps


def sheep(frame):  # Draws a sheep looking up then grazing
    return la.outline(from_map(SHEEP[frame], {"w": WOOL, "s": mm.MASK, "d": mm.STRAP, "e": mm.HEADLAMP}))  # Outlines the sheep


def cypress():  # Draws a tall narrow Italian cypress
    img = mm.new_image(12, 32)  # Fits the prop
    for y in range(1, 29):  # Walks down the tree
        half = min(4.5, 0.5 + (y - 1) * 0.35) if y < 20 else 4.5 - (y - 20) * 0.3  # Widens then narrows the flame shape
        for x in range(round(5.5 - half), round(5.5 + half) + 1):  # Fills the row
            la.put(img, x, y, mm.HILLS_SH if x < 5 else mm.LEAF if x < 8 else FOREST_DARK)  # Lights the left side
    mm.fill_rect(img, 5, 28, 2, 4, mm.GROUND_SH)  # Adds the trunk
    return la.outline(img)  # Outlines the tree


def column_broken():  # Draws one broken Roman column
    img = mm.new_image(12, 24)  # Fits the prop
    mm.fill_rect(img, 0, 21, 12, 3, MARBLE_SHADE)  # Lays the base
    for i, top in enumerate((6, 4, 5, 3, 4, 7, 8, 6)):  # Breaks the top unevenly
        x = 2 + i  # Finds the column pixel
        colour = mm.HEADLAMP if i < 2 else MARBLE_SHADE if i > 5 else MARBLE  # Lights the left side
        mm.fill_rect(img, x, top, 1, 21 - top, colour)  # Raises one strip of shaft
    for x in (5, 7):  # Cuts two flutes
        mm.fill_rect(img, x, 9, 1, 11, mm.VEST)  # Draws one flute
    return la.outline(img)  # Outlines the column


def bamboo_raft_far():  # Draws a far raft with a boatman poling
    img = mm.new_image(32, 8)  # Fits the prop
    mm.paste(img, bamboo_raft(29).crop((0, 1, 29, 8)), 1, 1)  # Lays the raft low
    mm.fill_rect(img, 12, 1, 5, 1, THATCH)  # Draws the boatman's hat
    mm.fill_rect(img, 13, 2, 3, 3, mm.STRAP)  # Draws the boatman
    ImageDraw.Draw(img).line([(16, 2), (20, 7)], fill=WOOD_SHADE)  # Draws the pole
    return la.outline(img)  # Outlines the raft


CORMORANT = (  # Letter maps for a cormorant on a perch
    ["......bb....", ".....bbeyy..", ".....bgg....", "....bbb.....", "...bbbb.....", "...bsbbb....",  # Perched
     "..bbsbbb....", "..bbbsbb....", "...bbbb.....", "..bbbb......", "wwwwwwwwwwww", "dddddddddddd"],  # Tail and perch
    ["......bb....", ".....bbeyy..", ".....bgg....", "bb..bbb..bb.", ".bbbbbbbbb..", "..bsbbbsb...",  # Wings spread to dry
     "..bbsbbb....", "..bbbsbb....", "...bbbb.....", "..bbbb......", "wwwwwwwwwwww", "dddddddddddd"],  # Tail and perch
)  # Ends the cormorant maps


def cormorant(frame):  # Draws a cormorant resting then drying its wings
    return la.outline(from_map(CORMORANT[frame], {"b": mm.STRAP, "s": mix(mm.STRAP, mm.LEAF), "e": WATER, "y": mm.HELMET_SH, "g": mm.HELMET, "w": POST, "d": mm.GROUND_SH}))  # Outlines the bird


def reframe(img):  # Redraws the dark lesson frame after painting
    w, h = img.size  # Reads the picture size
    for rect in ((0, 0, w, 1), (0, h - 1, w, 1), (0, 0, 1, h), (w - 1, 0, 1, h)):  # Lists the four edges
        mm.fill_rect(img, *rect, mm.OUTLINE)  # Paints one edge
    return img  # Gives back the lesson


def lesson_tarsier_forest():  # Paints a tarsier on a trunk in a shady forest
    img = ls.framed(lambda x, y: mm.LEAF if y < 20 else FOREST_DARK if y < 66 else FOREST_SOIL)  # Sets canopy, shade and floor
    for x, w, c in ((8, 6, mm.GROUND_SH), (32, 4, mix(mm.GROUND_SH, FOREST_DARK)), (88, 5, mix(mm.GROUND_SH, FOREST_DARK)), (104, 8, mm.GROUND_SH)):  # Places the back trunks
        mm.fill_rect(img, x, 10, w, 58, c)  # Draws one trunk
    ls.fill_poly(img, [(40, 1), (52, 1), (78, 66), (62, 66)], mix(FOREST_DARK, mm.HILLS_SH))  # Drops a soft light shaft
    mm.fill_rect(img, 56, 4, 12, 64, mm.GROUND_SH)  # Raises the near trunk
    mm.fill_rect(img, 56, 4, 3, 64, POST)  # Lights its left edge
    for x, y, r in ((10, 6, 10), (30, 3, 9), (50, 6, 8), (74, 3, 10), (96, 7, 9), (114, 4, 8)):  # Places canopy clumps
        ls.disc(img, x, y, r, mm.LEAF)  # Fills one clump
        ls.disc(img, x - 2, y - 2, r - 4, mm.HILLS_SH)  # Lights its top-left
    for x, y in ((6, 70), (20, 74), (34, 68), (49, 76), (77, 71), (93, 75), (109, 69)):  # Scatters fallen leaves
        mm.fill_rect(img, x, y, 3, 1, mm.HELMET_SH if x % 2 else mm.PLATEAU)  # Draws one leaf
    for x in (18, 96):  # Places two ferns
        for dx in (-5, -2, 2, 5):  # Fans the fronds
            ImageDraw.Draw(img).line([(x, 72), (x + dx, 64)], fill=mm.HILLS_SH)  # Draws one frond
    mm.paste(img, tarsier(0).resize((36, 36), Image.Resampling.NEAREST), 26, 22)  # Hangs a big tarsier on the trunk
    return reframe(img)  # Keeps the frame


def lesson_antipolo_slope():  # Paints a calm hillside showing landslide warning signs
    img = ls.framed(na.sky)  # Starts with sky
    ls.fill_poly(img, [(1, 48), (40, 38), (80, 22), (118, 12), (118, 80), (1, 80)], mm.HILLS)  # Raises the grassy slope
    ls.fill_poly(img, [(1, 48), (40, 38), (80, 22), (118, 12), (118, 15), (80, 26), (40, 41), (1, 51)], mm.PLAINS)  # Lights the slope crest
    ls.fill_poly(img, [(60, 80), (74, 56), (92, 48), (106, 52), (118, 46), (118, 80)], mm.PLATEAU_SH)  # Shows bare red clay
    ls.fill_poly(img, [(66, 80), (77, 60), (92, 53), (100, 56), (90, 80)], mm.PLATEAU)  # Lights the clay face
    draw = ImageDraw.Draw(img)  # Gives a crack pen
    for pts in ([(28, 52), (34, 50), (38, 54), (45, 51), (50, 53)], [(70, 38), (76, 36), (80, 39), (88, 34)], [(18, 64), (24, 66), (30, 63)]):  # Lists the cracks
        draw.line(pts, fill=mm.PLATEAU_SH, width=3)  # Opens bare soil along the crack
        draw.line(pts, fill=mm.OUTLINE)  # Draws the dark split
    for i, x in enumerate((46, 58, 70, 82)):  # Places a fence leaning more down the hill
        y = 46 - (x - 40) * 0.4  # Follows the slope
        post = tilted_post() if i > 1 else mm.new_image(10, 20)  # Picks a leaning post lower down
        if i <= 1:  # Draws straight posts higher up
            mm.fill_rect(post, 4, 2, 3, 18, POST)  # Draws one upright post
            post = la.outline(post)  # Outlines it
        mm.paste(img, post, x, int(y) - 14)  # Sets the post on the slope
    mm.paste(img, warning_sign(), 10, 30)  # Stands the warning sign
    for x, y in ((96, 4), (108, 2)):  # Places trees on the ridge
        ls.disc(img, x, y + 6, 5, mm.LEAF)  # Draws one tree
    ls.fill_poly(img, [(4, 70), (12, 63), (20, 70)], mm.SCARF_SH)  # Puts a house roof at the safe foot
    mm.fill_rect(img, 6, 70, 13, 9, KARST_LIGHT)  # Draws the house wall
    mm.fill_rect(img, 11, 73, 3, 6, mm.DARK)  # Opens the door
    return reframe(img)  # Keeps the frame


def lesson_masungi():  # Paints the rope web among grey limestone points
    img = ls.framed(lambda x, y: na.sky(x, y) if y < 30 else HAZE_GREEN if y < 44 else mm.LEAF)  # Sets sky, far hills and forest
    for x, y, h in ((2, 34, 30), (16, 26, 38), (84, 30, 34), (100, 22, 44), (110, 36, 30)):  # Places the jagged points
        jagged_rock(img, [(x, y + h), (x + 2, y + h // 2), (x + 4, y + h // 3), (x + 6, y + 4), (x + 8, y), (x + 10, y + 6), (x + 12, y + h // 3), (x + 14, y + h // 2), (x + 16, y + h)], mm.MASK, mm.MASK_SH, x + 9)  # Paints one tapering point
    mm.paste(img, web_bridge(), 32, 18)  # Hangs the web between two rocks
    for x, y, r in ((6, 74, 7), (24, 77, 6), (90, 75, 8), (110, 78, 6)):  # Places the front trees
        ls.disc(img, x, y, r, mm.HILLS_SH)  # Draws one tree
    return reframe(img)  # Keeps the frame


def lesson_pililla():  # Paints a windy ridge of turbines above Laguna de Bay
    img = ls.framed(lambda x, y: na.sky(x, y) if y < 34 else WATER if y < 44 else mm.HILLS)  # Sets sky, lake and grass
    ls.fill_poly(img, [(1, 34), (30, 30), (60, 33), (118, 29), (118, 34)], HAZE_GREEN)  # Shows the far shore
    ls.fill_poly(img, [(1, 60), (30, 48), (60, 44), (90, 46), (118, 40), (118, 80), (1, 80)], mm.HILLS)  # Raises the ridge
    ls.fill_poly(img, [(1, 60), (30, 48), (60, 44), (90, 46), (118, 40), (118, 43), (90, 49), (60, 47), (30, 51), (1, 63)], mm.PLAINS)  # Lights the ridge top
    for x, y, size, f in ((6, 26, (16, 32), 0), (36, 18, (16, 32), 1), (60, 8, (24, 48), 0), (92, 2, (24, 48), 1)):  # Lines turbines along the crest
        mm.paste(img, turbine_far(f).resize(size, Image.Resampling.NEAREST), x, y)  # Stands one turbine
    for x in (6, 30, 54, 80, 104):  # Spaces the near grass
        mm.paste(img, wind_grass(1), x, 68)  # Bends one tuft
    return reframe(img)  # Keeps the frame


def dome(img, x, y, w, h, near):  # Paints one round drumlin island
    for px in range(w):  # Walks across the island
        top = round(h * math.sqrt(max(0.0, 1 - ((px - w / 2 + 0.5) / (w / 2)) ** 2)))  # Rounds the top
        for py in range(top):  # Fills the column
            colour = mm.HILLS if px < w / 3 and py > top - 3 else EMERALD if near else HAZE_GREEN  # Lights the top-left
            la.put(img, x + px, y - py, colour)  # Paints one pixel
    if near:  # Shows a clay cliff on the near islands
        ls.fill_poly(img, [(x, y), (x + 2, y - h // 2), (x + w // 4, y - h // 3), (x + w // 4, y)], TILL)  # Cuts the bare side


def lesson_clew_bay():  # Paints many small drumlin islands in a bay
    img = ls.framed(lambda x, y: na.sky(x, y) if y < 28 else WATER_MID if y < 44 else WATER)  # Sets sky and bay
    ls.fill_poly(img, [(76, 28), (92, 8), (98, 7), (114, 28)], mm.MASK_SH)  # Raises the far cone mountain
    ls.fill_poly(img, [(76, 28), (92, 8), (95, 8), (88, 28)], mm.MASK)  # Lights its left face
    for x, y, w, h in ((4, 30, 8, 2), (18, 31, 7, 2), (30, 30, 9, 3), (46, 31, 7, 2), (58, 30, 10, 3), (72, 31, 8, 2), (104, 31, 9, 2)):  # Places far islands
        dome(img, x, y, w, h, False)  # Paints one far island
    for x, y, w, h in ((8, 40, 14, 4), (30, 42, 12, 4), (52, 39, 16, 5), (78, 41, 13, 4), (98, 40, 15, 5)):  # Places middle islands
        dome(img, x, y, w, h, False)  # Paints one middle island
    for x, y, w, h in ((2, 58, 24, 8), (36, 62, 20, 7), (66, 56, 26, 9), (98, 64, 18, 7), (20, 76, 22, 7), (60, 78, 24, 8), (96, 79, 20, 6)):  # Places near islands
        dome(img, x, y, w, h, True)  # Paints one near island
    return reframe(img)  # Keeps the frame


def lesson_rome_hills():  # Paints buildings, cypresses and ruins on gentle hills
    img = ls.framed(lambda x, y: na.sky(x, y) if y < 40 else mm.HILLS_SH)  # Sets sky and hills
    for pts, colour in (([(1, 44), (30, 32), (58, 40), (86, 30), (118, 40), (118, 80), (1, 80)], HAZE_GREEN), ([(1, 60), (24, 48), (52, 54), (80, 46), (118, 56), (118, 80), (1, 80)], mm.HILLS_SH)):  # Layers two hill rows
        ls.fill_poly(img, pts, colour)  # Paints one hill row
    for x, y, w, h in ((22, 36, 8, 6), (31, 34, 10, 8), (80, 32, 9, 7), (90, 34, 8, 6), (70, 46, 12, 8), (8, 50, 9, 7)):  # Places houses on the hills
        mm.fill_rect(img, x, y, w, h, mix(mm.HELMET_SH, mm.PLATEAU))  # Draws the ochre wall
        mm.fill_rect(img, x, y - 2, w, 2, mm.SCARF_SH)  # Adds the tile roof
        mm.fill_rect(img, x + 2, y + 2, 2, 2, mm.DARK)  # Opens a window
    for x, y in ((44, 36), (60, 40), (100, 38), (4, 50)):  # Places the cypress trees
        mm.paste(img, cypress(), x, y - 20)  # Stands one cypress
    for x in (36, 50, 64):  # Places ruin columns in front
        mm.paste(img, column_broken(), x, 54)  # Stands one column
    mm.fill_rect(img, 33, 77, 46, 3, MARBLE_SHADE)  # Lays the ruin step
    return reframe(img)  # Keeps the frame


def tower(img, x, base, w, h, colour, light):  # Paints one tall round-topped karst tower
    top = base - h  # Finds the top row
    ls.fill_poly(img, [(x, base), (x + 1, top + h // 3), (x + w // 5, top + 3), (x + w // 2, top), (x + w * 4 // 5, top + 3), (x + w - 1, top + h // 3), (x + w, base)], colour)  # Shapes the tower
    ls.fill_poly(img, [(x + 1, base), (x + 2, top + h // 3), (x + w // 5 + 1, top + 4), (x + w // 3, top + 3), (x + w // 4, base)], light)  # Lights its left side


def lesson_guilin():  # Paints karst towers above the Li River with a raft
    img = ls.framed(lambda x, y: na.sky(x, y) if y < 50 else WATER_MID)  # Sets sky and river
    for x, w, h in ((0, 16, 30), (20, 14, 36), (44, 18, 26), (70, 14, 34), (96, 20, 30)):  # Places far towers
        tower(img, x, 52, w, h, lm.HAZE_SH, lm.HAZE)  # Paints one misty tower
    for x, w, h in ((8, 18, 40), (54, 16, 44), (86, 20, 38)):  # Places middle towers
        tower(img, x, 54, w, h, HAZE_GREEN, mm.HILLS)  # Paints one green tower
    for x, w, h in ((-2, 22, 50), (100, 20, 54)):  # Places near towers at the sides
        tower(img, x, 58, w, h, mm.LEAF, mm.HILLS_SH)  # Paints one near tower
    mm.fill_rect(img, 1, 54, 118, 4, mm.HILLS_SH)  # Draws the river bank
    for x, y, w in ((20, 62, 22), (60, 66, 26), (12, 72, 18), (80, 75, 20)):  # Spaces river lines
        mm.fill_rect(img, x, y, w, 1, WATER)  # Draws one line
    mm.paste(img, bamboo_raft_far(), 44, 64)  # Floats a raft on the river
    return reframe(img)  # Keeps the frame


# ---------- Items (24x24, Assets/Art/Items/Sprites/) ----------

OLIVE_DARK = mix(mm.LEAF, mm.OUTLINE)  # Dark green rock between the grains
OLIVE_GRAIN = mm.LEAF  # Deep green olivine grain
OLIVE_LIGHT = mix(mm.LEAF, mm.HILLS)  # Lit olivine grain
CHLORITE = mix(mm.LEAF, mm.STRAP)  # Soft dark green flake
CHLORITE_LIGHT = mix(mm.LEAF, mm.MASK_SH)  # Pale grey-green flake rim
CHLORITE_DARK = mix(CHLORITE, mm.OUTLINE)  # Shadow under a flake
CHLORITE_MID = mix(CHLORITE, mm.LEAF)  # Brighter green flake
DACITE = mm.MASK  # Pale grey volcanic rock
DACITE_SHADE = mix(mm.MASK, mm.MASK_SH)  # Dacite away from the light
PINK = mix(mm.WHITE, mm.PLATEAU)  # Pale pinkish-white quartzite
PINK_SHADE = mix(PINK, mm.PLATEAU_SH)  # Quartzite in shade
PINK_LIGHT = mix(mm.WHITE, PINK)  # Sugary sparkle
CREAM = mix(mm.HEADLAMP, mix(mm.HEADLAMP, mm.VEST))  # Warm cream travertine
CREAM_BAND = mix(mm.HEADLAMP, mm.VEST)  # Slightly darker band in travertine
CREAM_SHADE = mix(mm.VEST, mm.HEADLAMP)  # Travertine in shade
HOLE = mm.VEST_SH  # Small hole in travertine
LAVA_DARK = mix(mm.STRAP, mm.OUTLINE)  # Dark host rock around leucite


def peridotite():  # Draws a dark green rock packed with green grains
    def c(x, y, s):  # Gives the colour of one pixel
        n = lm.noise(x, y)  # Picks a fixed random number
        if n < 12:  # Black pyroxene speck
            return mm.OUTLINE  # Paints a black grain
        if n < 30:  # Lit grain face
            return OLIVE_GRAIN if s else OLIVE_LIGHT  # Paints a bright grain
        if n < 70:  # Plain grain
            return OLIVE_DARK if s else OLIVE_GRAIN  # Paints a green grain
        return mix(OLIVE_DARK, mm.OUTLINE) if s else OLIVE_DARK  # Paints the dark rock between grains
    return ls.outline(na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 2.6))  # Outlines the grainy lump


def chlorite():  # Draws soft dark green flakes stacked in scales
    img = mm.new_image(24, 24)  # Fits the item card
    for row, (y, xs) in enumerate(((8, (8, 13)), (11, (6, 11, 16)), (14, (5, 10, 15, 19)), (17, (7, 12, 17)))):  # Places rows of flakes from back to front
        for i, x in enumerate(xs):  # Places one flake in the row
            body = CHLORITE if (row + i) % 2 else CHLORITE_MID  # Swaps the shade so each flake shows
            for dy in range(-2, 3):  # Runs down the flake
                half = 3 - abs(dy) // 2  # Makes the flake a flat oval
                mm.fill_rect(img, x - half, y + dy, half * 2 + 1, 1, body)  # Fills one line of the flake
            ls.put(img, x + 3, y, CHLORITE_DARK)  # Darkens the flake's right tip
            mm.fill_rect(img, x - 2, y - 2, 5, 1, CHLORITE_LIGHT)  # Lights the flake rim
            mm.fill_rect(img, x - 2, y + 2, 5, 1, CHLORITE_DARK)  # Shades under the flake
    return ls.outline(img)  # Outlines the flaky pile


def dacite():  # Draws pale grey volcanic rock with small light crystals
    def c(x, y, s):  # Gives the colour of one pixel
        n = lm.noise(x, y)  # Picks a fixed random number
        if n > 96:  # Rare black mica speck
            return mm.STRAP  # Paints a dark crystal
        return DACITE_SHADE if s else DACITE  # Paints the pale rock
    img = na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, c, 2.4)  # Makes a rounded pale lump
    for x, y in ((5, 11), (10, 9), (15, 10), (8, 15), (13, 15), (17, 14)):  # Places small clear crystals
        mm.fill_rect(img, x, y, 2, 2, mm.WHITE)  # Draws one light crystal
        ls.put(img, x + 1, y + 1, mm.MASK_SH)  # Shades its lower corner
    return ls.outline(img)  # Outlines the lump


def quartzite():  # Draws hard pale pink sugary rock with sharp faces
    img = mm.new_image(24, 24)  # Fits the item card
    ls.fill_poly(img, [(3, 15), (6, 8), (12, 5), (19, 7), (21, 13), (18, 19), (9, 20)], PINK)  # Cuts an angular block
    ls.fill_poly(img, [(19, 7), (21, 13), (18, 19), (9, 20), (14, 14)], PINK_SHADE)  # Shades the right face
    ImageDraw.Draw(img).line([(6, 9), (12, 6)], fill=PINK_LIGHT)  # Lights the top edge
    for y in range(24):  # Checks every row
        for x in range(24):  # Checks every pixel
            if img.getpixel((x, y))[3] and lm.noise(x, y) < 18:  # Picks sugary grains inside the rock
                ls.put(img, x, y, PINK_LIGHT if img.getpixel((x, y))[:3] == PINK else PINK)  # Makes the grain sparkle
    return ls.outline(img)  # Outlines the hard block


def travertine():  # Draws a cut cream block with bands and small holes
    img = mm.new_image(24, 24)  # Fits the item card
    mm.fill_rect(img, 2, 6, 20, 14, CREAM)  # Cuts a building block
    mm.fill_rect(img, 2, 6, 20, 1, mm.HEADLAMP)  # Lights the top edge
    mm.fill_rect(img, 2, 18, 20, 2, CREAM_SHADE)  # Shades the bottom face
    for y in (8, 11, 14):  # Spaces the bands
        for x in range(2, 22):  # Runs across the block
            ls.put(img, x, y + (x // 6) % 2, CREAM_BAND)  # Draws one gently wavy band
    for x, y in ((5, 10), (12, 7), (17, 10), (8, 13), (15, 13), (11, 16), (19, 16)):  # Places small holes
        mm.fill_rect(img, x, y, 2, 1, HOLE)  # Draws one flat hole
    return ls.outline(img)  # Outlines the block


def leucite():  # Draws round white crystals set in dark rock
    img = na.blob(24, 24, 11.5, 13.5, 10.5, 7.5, lambda x, y, s: mm.OUTLINE if s else LAVA_DARK, 2.2)  # Makes the dark host rock
    for cx, cy, r in ((7, 12, 3), (15, 11, 3), (11, 17, 2), (17, 17, 2)):  # Places the round crystals
        ls.disc(img, cx, cy, r, mm.MASK)  # Draws the crystal body
        ls.disc(img, cx - 1, cy - 1, r - 1, mm.WHITE)  # Lights its upper left
    return ls.outline(img)  # Outlines the specimen


def preview(groups):  # Lays out every sprite at triple size with its exact name
    width, pad, gap = 1160, 12, 12  # Sets the sheet width and clear space
    y = pad  # Starts the first group
    placements, headings = [], []  # Saves the drawing positions
    for title, sprites in groups:  # Gives each kind of sprite its own group
        headings.append((title, y))  # Reserves the heading row
        x, y, row = pad, y + 24, 0  # Starts the next row below the heading
        for name, img in sprites:  # Sizes one preview tile at a time
            w, h = img.width * 3, img.height * 3  # Uses whole pixel scaling
            cell = max(w + 12, len(name) * 7 + 12)  # Keeps the full label in the tile
            if x + cell > width - pad:  # Wraps when the tile would run off the sheet
                x, y, row = pad, y + row + gap, 0  # Moves to the next row
            placements.append((name, img, x, y, cell))  # Saves the tile position
            x, row = x + cell + gap, max(row, h + 30)  # Makes room for the label and art
        y += row + 20  # Leaves a gap above the next group
    sheet = Image.new("RGBA", (width, y), mm.OUTLINE + (255,))  # Creates a dark sheet
    draw = ImageDraw.Draw(sheet)  # Gives the sheet a text pen
    for title, top in headings:  # Labels each group
        draw.text((pad, top), title, fill=mm.HEADLAMP)  # Draws a pale heading
    for name, img, x, top, cell in placements:  # Draws the sprite and label in its tile
        big = img.resize((img.width * 3, img.height * 3), Image.Resampling.NEAREST)  # Upscales without soft edges
        mm.fill_rect(sheet, x, top, cell, big.height + 26, mix(lm.HAZE_SH, mm.STRAP))  # Gives an even backing
        sheet.alpha_composite(big, (x + (cell - big.width) // 2, top + 6))  # Centers the sprite above its name
        draw.text((x + 5, top + big.height + 9), name, fill=FOAM)  # Gives the exact name
    PREVIEW.parent.mkdir(parents=True, exist_ok=True)  # Makes the preview folder
    sheet.save(PREVIEW)  # Writes the preview sheet


def main():  # Writes the requested PNGs and the preview
    tiles = [entry for theme in THEMES for entry in set_tiles(theme)]  # Builds all thirty-five tiles and boulders
    objects = [("mud_top", mud_top())]  # Draws the mud tile
    for name, painter, frames in (("mud_splash", mud_splash, 2), ("log", rolling_log, 4), ("log_warn", log_warning, 2)):  # Lists animated objects
        objects.extend((f"{name}_{i:02d}", painter(i)) for i in range(frames))  # Draws every frame
    goals = [(painter.__name__, painter()) for painter in (tarsier_lookout, barangay_hall, web_bridge, turbine_base, stone_pier, temple_ruins, raft_dock)]  # Draws the seven goals
    props = [(painter.__name__, painter()) for painter in (vine, warning_sign, cracked_ground, tilted_post, karst_spike, rope_rail, stone_wall, cypress, column_broken, bamboo_raft_far)]  # Draws still props
    for painter in (tarsier, turbine_far, wind_grass, sheep, cormorant):  # Lists animated props
        props.extend((f"{painter.__name__}_{i:02d}", painter(i)) for i in range(2))  # Draws both frames
    lessons = [(painter.__name__, painter()) for painter in (lesson_tarsier_forest, lesson_antipolo_slope, lesson_masungi, lesson_pililla, lesson_clew_bay, lesson_rome_hills, lesson_guilin)]  # Draws the seven lessons
    items = [(painter.__name__, painter()) for painter in (peridotite, chlorite, dacite, quartzite, travertine, leucite)]  # Draws the six item pictures
    for folder, sprites in ((LEVEL, tiles + objects + goals), (LEVEL / "Props", props), (Path(ls.ITEMS_OUT), items), (Path(ls.LESSON_OUT), lessons)):  # Puts each set in its folder
        for name, img in sprites:  # Writes one named PNG
            img.save(folder / f"{name}.png")  # Saves only Hills Hard output paths
    with Image.open(LEVEL / "ice_top.png") as ice, Image.open(LEVEL / "soft_top.png") as soft, Image.open(LEVEL / "roll_boulder_00.png") as roll:  # Loads old sprites to compare
        compare = [("ice_top (old)", ice.convert("RGBA")), ("soft_top (old)", soft.convert("RGBA")), ("roll_boulder_00 (old)", roll.convert("RGBA"))]  # Keeps them for the sheet
        preview([("Terrain", tiles), ("Mud and log", objects + compare), ("Goals", goals), ("Props", props), ("Items", items), ("Lessons", lessons)])  # Shows all sprites
    print(PREVIEW)  # Prints the preview path


if __name__ == "__main__":  # Runs only when called as a script
    main()  # Writes the art
