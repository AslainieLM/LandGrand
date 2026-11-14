"""Draws the Main Menu base camp (decision D-054) and its new buttons.

Run from the Unity project folder:  python Tools/Art/make_menu_camp.py
It writes to Assets/Art/MainMenu/Sprites/Camp/ and Assets/Art/MainMenu/Sprites/Buttons/.

The base camp is the explorer's home before each trip, in golden late-afternoon light:
  C0 sky (480x270, still): warm blue high up, golden near the horizon, a low sun on the left.
  C1 clouds (960x270, drifts slowly): warm clouds with peach undersides.
  C2 peaks (480x270, still): one big snowy mountain and two smaller ones, lit from the left.
  C3 forest (480x270, still): a band of pines behind the clearing.
  C4 camp (480x270, still): the grassy clearing, the tent, the map board, a log seat and the fire pit's glow.
  fire_0..3 (16x24): the campfire flames.   embers_0..5 (16x32): sparks rising from the fire.
  explorer_stand_0..1 (48x48): the start-screen explorer standing by the fire, facing it, 1.5 times bigger, breathing.
  flag_0..2 (12x10): the pennant on the tent pole.   glint_0..5 (8x8): a twinkle on the quartz in the rock crate.
  butterfly_0..1 (6x4): two butterflies over the grass (the builder flies them in ovals).
The top band (rows 0 to 60) is kept calm so the player badge and the icon buttons stay easy to read,
and the camp sits between rows 120 and 200, above the big PLAY button.
Colours come from the palette (Assets/Art/Palettes/landgrand.gpl) plus even mixes of two palette colours.
"""
import math  # math gives sin, hypot and pi
import os  # os is used to make folders and join paths
from PIL import Image  # Pillow makes and saves the PNG pictures
import make_mainmenu_art as mm  # Main Menu palette, button and text helpers
import make_landform_map_art as lm  # Map colours and the fixed noise

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))  # The Unity project folder
OUT = os.path.join(ROOT, "Assets", "Art", "MainMenu", "Sprites")  # Main Menu sprite folder
W, H = 480, 270  # One screen
SUN = (255, 242, 168)  # Sun, from the palette
SUN_GLOW = (255, 225, 122)  # Sun glow, from the palette


def mix(a, b, t):
    """An even blend of two palette colours: t = 0 gives a, t = 1 gives b."""
    return tuple(int(round(a[i] + (b[i] - a[i]) * t)) for i in range(3))  # Blend red, green and blue


SKY_TOP = mix(lm.SKY, lm.MOUNTAIN, 0.15)  # Deep warm blue at the top
SKY_LOW = mix(lm.SKY, SUN_GLOW, 0.45)  # Golden green-blue near the horizon
HORIZON = mix(SUN, mm.SKY_LIGHT, 0.35)  # Pale gold just above the peaks
PEAK = mix(lm.MOUNTAIN, mm.PLATEAU, 0.25)  # Mountain rock warmed by the low sun
PEAK_LIT = mix(lm.HAZE, SUN_GLOW, 0.35)  # Sunny left face of the rock
PEAK_SNOW = mix(mm.WHITE, SUN, 0.5)  # Sunny snow, golden
PEAK_SNOW_SH = mix(lm.HAZE, lm.SNOW, 0.4)  # Snow in shade, cool blue
FOREST = mix(mm.LEAF, lm.MOUNTAIN, 0.3)  # Pines in the back, soft
FOREST_LIT = mix(mm.HILLS_SH, SUN_GLOW, 0.2)  # Sunny side of the pines
GRASS = mix(mm.HILLS, SUN_GLOW, 0.15)  # Clearing grass in warm light
GRASS_SH = mm.HILLS_SH  # Grass in shade and tufts
DIRT = mm.GROUND  # Bare earth around the fire
DIRT_SH = mm.GROUND_SH  # Darker earth
WOOD = lm.HANDLE  # Wood of the board and the logs
WOOD_SH = mm.PLATEAU_SH  # Shaded wood
TENT = mm.SCARF  # Tent canvas
TENT_SH = mm.SCARF_SH  # Shaded side of the tent
PAPER = SUN  # The map on the board
PAPER_SH = SUN_GLOW  # Map edges
FIRE_GLOW = mix(DIRT, SUN_GLOW, 0.5)  # Warm light on the ground around the fire


def new(w=W, h=H):
    """An empty, see-through picture."""
    return mm.new_image(w, h)  # Clear picture


def put(img, x, y, c):
    """Paints one pixel if it is inside the picture."""
    if 0 <= x < img.width and 0 <= y < img.height:  # Inside?
        img.putpixel((int(x), int(y)), c + (255,))  # Paint it solid


def dither(x, y, t):
    """True when a pixel takes the second colour of a blend, with a 4x4 ordered pattern."""
    bayer = (0, 8, 2, 10, 12, 4, 14, 6, 3, 11, 1, 9, 15, 7, 13, 5)  # Classic 4x4 order
    return t * 16 > bayer[(y % 4) * 4 + (x % 4)]  # More dots as t grows


def sky():
    """C0: warm blue at the top, gold near the horizon, and a big low sun on the left."""
    img = new()  # Empty picture
    for y in range(H):  # Each row
        for x in range(W):  # Each pixel
            if y < 70:  # Top: deep warm blue fading to sky blue
                c = lm.SKY if dither(x, y, max(0.0, (y - 40) / 30)) else SKY_TOP  # Fade between rows 40 and 70
            elif y < 120:  # Middle: sky blue fading to golden blue
                c = SKY_LOW if dither(x, y, (y - 70) / 50) else lm.SKY  # Fade between rows 70 and 120
            else:  # Low: golden blue fading to pale gold
                c = HORIZON if dither(x, y, min(1.0, (y - 120) / 30)) else SKY_LOW  # Fade between rows 120 and 150
            put(img, x, y, c)  # Paint it
    sx, sy = 96, 112  # The sun, low on the left, partly behind the peaks
    for y in range(sy - 26, sy + 27):  # Rows around the sun
        for x in range(sx - 26, sx + 27):  # Columns around the sun
            d = math.hypot(x - sx, y - sy)  # Distance from its middle
            if d <= 12:  # The sun itself
                put(img, x, y, SUN)  # Sun pixel
            elif d <= 15:  # Its bright rim
                put(img, x, y, SUN_GLOW)  # Glow pixel
            elif d <= 26 and dither(x, y, (26 - d) / 16):  # A soft dotted glow
                put(img, x, y, HORIZON)  # Pale gold pixel
    return img


def cloud(img, cx, cy, w):
    """A soft warm cloud: white top, peach underside. x wraps so the 960-wide layer loops."""
    for x in range(cx - w // 2, cx + w // 2):  # Each column of the cloud
        t = abs(x - cx) / (w / 2)  # 0 in the middle, 1 at the ends
        hump = int(6 * (1 - t * t)) + lm.noise(x // 4 % 240, cy) % 3  # Taller in the middle, a little lumpy
        for y in range(cy - hump, cy + 3):  # From the lumpy top to the flat bottom
            put(img, x % img.width, y, mix(mm.WHITE, SUN_GLOW, 0.55) if y >= cy + 1 else mm.WHITE)  # Peach underside, white top


def clouds():
    """C1: five warm clouds on a 960-wide strip that loops as it drifts. Kept between rows 70 and 110,
    below the badge and icons, so they never pass behind the text."""
    img = new(960, H)  # Two screens wide
    for cx, cy, w in ((150, 84, 90), (360, 100, 60), (560, 78, 120), (760, 96, 70), (900, 88, 50)):  # Place and width
        cloud(img, cx, cy, w)  # Draw it
    return img


def peaks():
    """C2: one big snowy mountain in the middle and two smaller ones, lit by the low sun on the left."""
    img = new()  # Empty picture
    shapes = [(300, 58, 0.62, 30), (160, 104, 0.7, 14), (430, 96, 0.68, 16)]  # Summit x, top row, slope, snow depth
    for x in range(W):  # Each column
        best, owner = H, None  # Highest top so far and its mountain
        for p in shapes:  # Each mountain
            y = p[1] + p[2] * abs(x - p[0]) + (lm.noise(x // 2, p[0]) % 3)  # Its side here, a little rough
            if y < best:  # Higher than the others
                best, owner = y, p  # This mountain owns the column
        px, top, slope, depth = owner  # The owner
        for y in range(int(best), 200):  # From the ridge down to behind the forest
            lit = x < px - (y - top) * 0.08  # Left of a leaning ridge line faces the sun
            snow = y < top + depth + (x * 7 % 5) - abs(x - px) * slope * 0.3  # Inside the snow cap, with a ragged lower edge
            c = (PEAK_SNOW if lit else PEAK_SNOW_SH) if snow else (PEAK_LIT if lit else PEAK)  # Snow or rock, sunny or shaded
            put(img, x, y, c)  # Paint it
    return img


def pine(img, cx, base, h, lit, shade):
    """A pixel pine: sunny left half (the sun is on the left), shaded right half, with a short trunk."""
    for i in range(h):  # Each row from the tip down
        tier = i % max(4, h // 3)  # Row within its tier
        half = min(1 + i // 3, 1 + tier // 2 + i // 6)  # Wider going down; each tier starts narrower
        for dx in range(-half, half + 1):  # Each pixel across
            put(img, cx + dx, base - h + i, lit if dx < 0 else shade)  # Left half lit
    put(img, cx, base, WOOD_SH)  # Trunk
    put(img, cx, base + 1, WOOD_SH)  # Trunk


def forest():
    """C3: a band of pines behind the clearing, from row about 150 down to 182."""
    img = new()  # Empty picture
    for x in range(W):  # Each column
        floor = 172 + int(2 * math.sin(x / 30))  # The forest floor, gently rolling
        for y in range(floor, 190):  # Fill below it
            put(img, x, y, FOREST)  # Forest shade
    for i, x in enumerate(range(-4, W + 4, 7)):  # A tree every 7 pixels
        h = 16 + lm.noise(i, 5) % 12  # Heights from 16 to 27
        pine(img, x + lm.noise(i, 9) % 3, 176 + int(2 * math.sin(x / 30)), h, FOREST_LIT, FOREST)  # Draw it
    return img


def tent(img, x0, base):
    """A two-tone A-frame tent, 72 wide and 46 tall, with a dark open flap."""
    w, h = 72, 46  # Size
    for y in range(h):  # Each row from the top
        half = int((y + 1) * (w / 2) / h)  # Wider going down
        for dx in range(-half, half + 1):  # Each pixel across
            c = TENT if dx < 0 else TENT_SH  # Sunny left side, shaded right side
            if abs(dx) == half:  # The outline
                c = mm.OUTLINE  # Dark edge
            put(img, x0 + dx, base - h + y, c)  # Paint it
        door = int(y * 12 / h) if y > h // 3 else -1  # The open flap gets wider going down
        for dx in range(0, door + 1):  # Inside the flap
            put(img, x0 + dx, base - h + y, mm.DARK)  # Dark inside
    for dx in range(-w // 2 - 4, w // 2 + 5):  # Ground line under the tent
        put(img, x0 + dx, base, mm.OUTLINE)  # Shadow line
    put(img, x0, base - h - 1, mm.OUTLINE)  # Tip of the pole
    put(img, x0, base - h - 2, mm.OUTLINE)  # Tip of the pole


def board(img, x0, top):
    """A wooden map board on two legs: a parchment map with four tiny landforms."""
    for y in range(top, top + 46):  # The two legs
        put(img, x0 + 3, y, WOOD_SH)  # Left leg
        put(img, x0 + 38, y, WOOD_SH)  # Right leg
    for y in range(top, top + 28):  # The frame and map
        for x in range(x0, x0 + 42):  # Each pixel across
            edge = x in (x0, x0 + 41) or y in (top, top + 27)  # Outer edge
            frame = x in (x0 + 1, x0 + 40) or y in (top + 1, top + 26)  # Wooden frame
            put(img, x, y, mm.OUTLINE if edge else WOOD if frame else (PAPER_SH if (x + y) % 11 == 0 else PAPER))  # Paper with a few spots
    marks = [(x0 + 9, top + 12, mm.MASK_SH), (x0 + 30, top + 11, mm.HILLS_SH), (x0 + 11, top + 20, mm.PLAINS), (x0 + 30, top + 20, mm.PLATEAU)]  # Mountains, hills, plains, plateaus
    for mx, my, c in marks:  # Each tiny landform
        for dx in range(-4, 5):  # A small mound
            for dy in range(0, 4 - abs(dx) // 2):  # Taller in the middle
                put(img, mx + dx, my - dy, c)  # Paint it
    for i in range(0, 14, 2):  # A dashed red trail between the landforms
        put(img, x0 + 13 + i, top + 14 + (i // 4), TENT)  # One dash


def trail(img):
    """A dirt trail that starts at the bottom of the screen and winds up into a gap in the forest, toward the mountains:
    the adventure starts here. Wide near the viewer, narrow far away, with darker dotted edges and a few footprints."""
    points = [(236, 272, 46), (230, 246, 36), (246, 222, 26), (272, 204, 16), (298, 190, 10), (312, 182, 6)]  # Middle x, row, width
    for (x0, y0, w0), (x1, y1, w1) in zip(points, points[1:]):  # Each stretch of the trail
        for y in range(y1, y0 + 1):  # Each row of this stretch
            t = (y0 - y) / max(1, y0 - y1)  # 0 at the near end, 1 at the far end
            cx, w = x0 + (x1 - x0) * t, w0 + (w1 - w0) * t  # Middle and width on this row
            for x in range(int(cx - w / 2), int(cx + w / 2) + 1):  # Across the trail
                edge = abs(x - cx) > w / 2 - 2  # The outer two pixels
                if edge and not dither(x, y, 0.5):  # Ragged, dotted edge
                    continue  # Leave grass here
                put(img, x, y, DIRT_SH if edge else DIRT)  # Edge or middle
    for x, y in ((232, 252), (238, 244), (233, 236), (241, 228), (248, 220), (256, 213)):  # Footprints going up the trail
        put(img, x, y, DIRT_SH)  # Toe
        put(img, x, y + 1, DIRT_SH)  # Heel


def signpost(img, x0, base):
    """A wooden signpost with two arrow boards pointing up the trail, to the mountains and on to new lands."""
    for y in range(base - 22, base):  # The post
        put(img, x0, y, WOOD_SH)  # Shaded side
        put(img, x0 + 1, y, WOOD)  # Lit side
    for row, (dx0, dx1, tip) in enumerate(((-2, 14, 1), (-12, 4, -1))):  # Upper board points right, lower board points left
        top = base - 21 + row * 7  # Top of this board
        for y in range(top, top + 5):  # Board rows
            for x in range(x0 + dx0, x0 + dx1):  # Board columns
                edge = y in (top, top + 4) or x in (x0 + dx0, x0 + dx1 - 1)  # Outline
                put(img, x, y, mm.OUTLINE if edge else (PAPER_SH if y == top + 1 else WOOD))  # Painted board
        tx = x0 + (dx1 if tip > 0 else dx0 - 1)  # The pointed tip
        for dy, d in ((1, 0), (2, 1), (3, 0)):  # A small triangle
            put(img, tx + tip * d, top + dy, mm.OUTLINE)  # Tip pixel
        for x in range(x0 + dx0 + 2, x0 + dx1 - 2, 2):  # A dashed arrow painted on the board
            put(img, x, top + 2, TENT)  # Red dash


def boulder(img, cx, base, w):
    """A round grey boulder with a lit top and a dark outline."""
    h = w * 2 // 3  # Height
    for y in range(base - h, base):  # Rows
        for x in range(cx - w // 2, cx + w // 2):  # Columns
            dx, dy = (x - cx) / (w / 2), (y - base + h * 0.4) / (h * 0.75)  # Distance from the middle, as a part of the size
            d = dx * dx + dy * dy  # Inside when below 1
            if d <= 1:  # Inside the boulder
                put(img, x, y, mm.OUTLINE if d > 0.8 else (mm.MASK if dy < -0.2 and dx < 0.3 else mm.MASK_SH))  # Outline, lit top, shade


def mushrooms(img):
    """A few red-capped mushrooms near the forest edge."""
    for x, y in ((24, 196), (30, 198), (450, 206), (196, 190)):  # Places
        for dx in (-1, 0, 1):  # Cap
            put(img, x + dx, y - 2, mm.SCARF)  # Red cap
        put(img, x, y - 3, mm.SCARF)  # Cap top
        put(img, x, y - 2, SUN)  # A pale spot on the cap
        put(img, x, y - 1, PAPER_SH)  # Stem
        put(img, x, y, PAPER_SH)  # Stem


def rope(img, cx, base):
    """A coil of rope on the ground: two rings of tan rope."""
    for ring, (w, h) in enumerate(((10, 5), (6, 3))):  # Outer ring, inner ring
        for x in range(cx - w // 2, cx + w // 2 + 1):  # Columns
            for y in range(base - h, base + 1):  # Rows
                d = ((x - cx) / (w / 2)) ** 2 + ((y - base + h / 2) / (h / 2)) ** 2  # Distance from the ring's middle
                if 0.55 <= d <= 1.05:  # On the ring
                    put(img, x, y, mm.VEST if y < base - h / 2 else mm.VEST_SH)  # Lit top, shaded bottom


def lantern_pole(img, x0, base):
    """A wooden pole with a hook at the top for the lantern, by the tent."""
    for y in range(base - 26, base):  # The pole
        put(img, x0, y, WOOD_SH)  # Shaded side
        put(img, x0 + 1, y, WOOD)  # Lit side
    for x in range(x0 + 2, x0 + 7):  # The arm the lantern hangs from
        put(img, x, base - 26, WOOD_SH)  # Arm pixel


def camp():
    """C4: the clearing in front of the forest, with the tent, the map board, a log seat and the fire pit."""
    img = new()  # Empty picture
    for y in range(180, H):  # The ground, from row 180 down
        for x in range(W):  # Each pixel
            c = GRASS if not dither(x, y, max(0.0, min(1.0, (y - 236) / 30))) else GRASS_SH  # Grass, a little darker near the bottom
            if (x * 13 + y * 7) % 29 == 0 and y < 240:  # A few grass tufts
                c = GRASS_SH  # Tuft pixel
            put(img, x, y, c)  # Paint it
    for y in range(180, 184):  # A soft edge where the clearing meets the forest
        for x in range(W):  # Each pixel
            if dither(x, y, (184 - y) / 4):  # Fewer dots going down
                put(img, x, y, GRASS_SH)  # Shade pixel
    trail(img)  # The trail toward the mountains, under everything else on the ground
    fx, fy = 172, 199  # Fire pit middle
    for y in range(fy - 8, fy + 9):  # The bare earth and the warm glow around the fire
        for x in range(fx - 30, fx + 31):  # Each pixel
            d = math.hypot((x - fx) / 2.6, y - fy)  # An oval, wider than tall
            if d <= 4.5:  # Bare earth right under the fire
                put(img, x, y, DIRT_SH if d > 3.5 else DIRT)  # Pit
            elif d <= 8 and dither(x, y, (8 - d) / 4):  # Warm light fading out
                put(img, x, y, FIRE_GLOW)  # Glow pixel
    for dx in (-9, -6, 6, 9):  # Stones around the pit
        put(img, fx + dx, fy + 1, mm.MASK_SH)  # Stone
        put(img, fx + dx, fy, mm.MASK)  # Stone top
    for x in range(370, 414):  # A log seat in front of the tent
        for y in range(204, 210):  # Its thickness
            put(img, x, y, mm.OUTLINE if y in (204, 209) or x in (370, 413) else (WOOD if y < 207 else WOOD_SH))  # Outlined log
    for y in range(205, 209):  # The cut end of the log
        put(img, 371, y, mm.VEST)  # Cut end of the log, light wood
    backpack(img, 418, 210)  # The backpack beside the log
    rope(img, 330, 206)  # A coil of rope by the crate
    lantern_pole(img, 444, 198)  # The pole the lantern hangs from
    signpost(img, 214, 208)  # The signpost at the start of the trail
    boulder(img, 92, 192, 14)  # Boulders at the forest edge
    boulder(img, 104, 194, 8)  # A small one beside it
    boulder(img, 468, 194, 12)  # One on the far right
    mushrooms(img)  # Mushrooms near the forest edge
    crate(img, 342, 197)  # The rock crate by the tent
    flowers(img)  # Flowers in the grass
    tent(img, 398, 196)  # The tent on the right
    board(img, 30, 152)  # The map board on the left
    for x, y in ((250, 205), (262, 211), (330, 222), (90, 214)):  # A few small stones in the grass
        put(img, x, y, mm.MASK)  # Stone top
        put(img, x + 1, y, mm.MASK_SH)  # Stone side
    return img


def flame(frame):
    """A 16x24 campfire flame. Four frames sway left and right; two crossed logs at the bottom."""
    img = new(16, 24)  # Empty picture
    sway = (0, 1, 0, -1)[frame]  # Tip leans this way
    for y in range(4, 20):  # Flame rows
        t = (y - 4) / 15  # 0 at the tip, 1 at the base
        half = int(1 + t * 5.5)  # Wider at the base
        lean = int(round(sway * (1 - t) * 2))  # The tip sways most
        for dx in range(-half, half + 1):  # Each pixel across
            r = abs(dx) / max(1, half)  # 0 in the middle, 1 at the edge
            c = SUN if r < 0.35 and t > 0.3 else SUN_GLOW if r < 0.7 else mm.SCARF  # Bright core, gold, red edge
            if y == 4 + (frame % 2) and dx != 0:  # Thin tip
                continue  # Skip the sides of the tip
            put(img, 8 + dx + lean, y, c)  # Paint it
    for x in range(2, 14):  # Two crossed logs
        put(img, x, 20 + (x % 4 == 0), WOOD_SH)  # Lower log
        put(img, x, 21, WOOD)  # Upper log
    put(img, 4 + frame, 2 if frame % 2 else 1, SUN_GLOW)  # A spark leaving the tip
    return img


def embers(frame):
    """A 16x32 strip of three sparks rising and fading; six frames loop."""
    img = new(16, 32)  # Empty picture
    for i, (x, start) in enumerate(((5, 0), (10, 2), (7, 4))):  # Three sparks, each starting at a different time
        step = (frame + start) % 6  # Where this spark is in its climb
        y = 30 - step * 5  # Higher each frame
        put(img, x + (step % 2), y, SUN if step < 3 else SUN_GLOW)  # Bright when low, dimmer when high
    return img


# The explorer standing by the fire: the start-screen explorer itself, in its relaxed standing frame (explorer_15, the end
# of the pick-up animation: legs together, arms down; explorer_00 is mid-stride and looks wrong when still), facing the fire,
# so the character looks the same on every screen. It is drawn 1.5 times bigger so it reads well in the camp.
LAMP_OLD, LAMP = (255, 244, 184), mm.HELMET  # The start-screen headlamp is almost white; here it glows amber so it is not a white dot
STAND_SIZE = 48  # The 32-pixel explorer drawn at 48 pixels


def standing(frame):
    """The start-screen explorer standing by the fire, 48x48. Frame 1 lowers the head and body by 1 pixel: breathing."""
    src = Image.open(os.path.join(ROOT, "Assets", "Art", "StartScreen", "Sprites", "Explorer", "explorer_15.png")).convert("RGBA")  # The start-screen explorer, standing still
    px = src.load()  # Its pixels
    for y in range(src.height):  # Each row
        for x in range(src.width):  # Each pixel
            if px[x, y][3] and px[x, y][:3] == LAMP_OLD:  # The near-white headlamp
                px[x, y] = LAMP + (255,)  # Amber instead
    img = src.resize((STAND_SIZE, STAND_SIZE), Image.NEAREST)  # 1.5 times bigger, still hard-edged
    if frame == 1:  # Breathing out
        top = img.crop((0, 0, STAND_SIZE, 30))  # Head and body, above the legs
        img.paste(new(STAND_SIZE, 1), (0, 0))  # Clear the top row
        img.paste(top, (0, 1))  # One pixel lower; the legs stay on the ground
    return img


def flag(frame):
    """A 12x10 red pennant on the tent pole, waving in three frames."""
    img = new(12, 10)  # Empty picture
    for x in range(1, 11):  # Along the pennant
        wave = (0, 1, 0)[(x // 3 + frame) % 3]  # Ripple moves along it
        half = max(0, 3 - x // 3)  # Narrower toward the tip
        for y in range(4 - half + wave, 5 + half + wave):  # Its height at this point
            put(img, x, y, TENT if y < 5 + wave else TENT_SH)  # Light top, shaded bottom
    for y in range(0, 10):  # The pole tip it hangs on
        put(img, 0, y, mm.OUTLINE)  # Pole pixel
    return img


def glint(frame):
    """An 8x8 twinkle for the quartz in the rock crate: dark for three frames, then a quick sparkle.
    Even size, so its centre sits on a whole pixel (an odd size lands half a pixel off)."""
    img = new(8, 8)  # Empty picture
    size = (0, 0, 0, 0, 1, 0)[frame]  # Arm length of the sparkle: a short, gentle twinkle in one frame only
    if size == 0:  # Resting frames
        return img  # Nothing to show
    for d in range(-size, size + 1):  # Cross arms
        put(img, 3 + d, 3, SUN)  # Across, pale gold so it does not read as a white dot
        put(img, 3, 3 + d, SUN)  # Up and down
    return img


def butterfly(frame):
    """A 6x4 yellow butterfly, wings open then closed. Even size, so it sits on whole pixels."""
    img = new(6, 4)  # Empty picture
    rows = (["#.#.#", "##.##", ".#.#.", "....."], [".#.#.", ".###.", "..#..", "....."])[frame]  # Open, closed
    for y, row in enumerate(rows):  # Each row
        for x, ch in enumerate(row):  # Each pixel
            if ch == "#":  # Wing or body
                put(img, x, y, mm.OUTLINE if x == 2 else mm.HELMET)  # Dark body, yellow wings
    return img


def crate(img, x0, base):
    """A small wooden crate of collected rocks by the tent: granite, quartz and feldspar on top."""
    for y in range(base - 10, base):  # Crate rows
        for x in range(x0, x0 + 20):  # Crate columns
            edge = x in (x0, x0 + 19) or y in (base - 10, base - 1)  # Outline
            plank = y == base - 5  # A plank line across
            put(img, x, y, mm.OUTLINE if edge else WOOD_SH if plank else WOOD)  # Paint it
    for dx, c, c2 in ((3, mm.MASK, mm.MASK_SH), (9, mm.WHITE, mm.SKY_LIGHT), (14, mm.PLATEAU, mm.PLATEAU_SH)):  # Granite, quartz, feldspar
        for x in range(x0 + dx, x0 + dx + 4):  # Each rock, 4 wide
            put(img, x, base - 12, c)  # Top
            put(img, x, base - 11, c2)  # Shaded bottom
        put(img, x0 + dx + 1, base - 13, c)  # A bump on top


def flowers(img):
    """Small flowers in the grass, away from the PLAY button and the camp."""
    for i, (x, y) in enumerate(((60, 226), (74, 232), (300, 214), (316, 220), (420, 230), (438, 224), (96, 248), (390, 250), (20, 240), (460, 244))):  # Places
        c = (mm.HELMET, mm.SCARF, lm.LENS)[i % 3]  # Yellow, red or blue petals (no white, so nothing reads as a stray dot)
        for dx, dy in ((0, -1), (-1, 0), (1, 0), (0, 1)):  # Four petals
            put(img, x + dx, y + dy, c)  # Petal
        put(img, x, y, mm.HELMET_SH)  # Centre, a darker gold
        put(img, x, y + 2, GRASS_SH)  # Stem


def backpack(img, x0, base):
    """The explorer's backpack resting on the ground beside the log."""
    for y in range(base - 9, base):  # Pack rows
        for x in range(x0, x0 + 8):  # Pack columns
            edge = x in (x0, x0 + 7) or y in (base - 9, base - 1)  # Outline
            put(img, x, y, mm.OUTLINE if edge else (lm.BACKPACK if y < base - 4 else mm.BACKPACK_SH))  # Paint it
    for x in range(x0 + 1, x0 + 7):  # The flap
        put(img, x, base - 6, mm.BACKPACK_SH)  # Flap line


def big(rows, color):
    """An icon pattern drawn with 2x2 blocks, so it reads at a glance on a phone."""
    small = mm.mono(rows, color)  # One pixel per mark
    return small.resize((small.width * 2, small.height * 2), Image.NEAREST)  # Each mark becomes a 2x2 block


def icon_button(rows, face, top, bottom, pressed, w=34, h=30):
    """A 34x30 button with a white icon drawn at 2x and no words: raised normally, pushed in when pressed."""
    img = new(w, h)  # Empty button
    mm.rounded_box(img, 0, 0, w, h)  # Dark outline
    mm.fill_rect(img, 1, 1, w - 2, h - 2, face)  # Face colour
    if pressed:  # Pushed in
        mm.fill_rect(img, 1, 1, w - 2, 2, bottom)  # Dark band on top
    else:  # Raised
        mm.fill_rect(img, 1, 1, w - 2, 2, top)  # Light band on top
        mm.fill_rect(img, 1, h - 3, w - 2, 2, bottom)  # Dark band on the bottom
    icon = big(rows, mm.WHITE)  # White icon at 2x
    x = (w - icon.width) // 2  # Centre across
    y = (h - icon.height) // 2 + (1 if pressed else 0)  # Centre down; drop 1 pixel when pressed
    mm.paste(img, big(rows, mm.DARK), x, y + 1)  # Shadow
    mm.paste(img, icon, x, y)  # Icon
    return img


GEAR = ["....#....", ".#.###.#.", "..#####..", ".##...##.", "###...###", ".##...##.", "..#####..", ".#.###.#.", "....#...."]  # Settings
BOOK = ["##.....##", "####.####", "#..#.#..#", "#.##.##.#", "#..#.#..#", "#.##.##.#", "####.####", "....#...."]  # Tutorial: an open book
DOOR = ["#####....", "#...#....", "#...#..#.", "#.#.#####", "#...#..#.", "#...#....", "#####...."]  # Exit: a door with an arrow going out
PLAY = ["#....", "##...", "###..", "####.", "#####", "####.", "###..", "##...", "#...."]  # The triangle on PLAY


def play_button(pressed):
    """The big 136x36 green PLAY button with a play triangle before the word."""
    img = mm.button(136, 36, "   PLAY", mm.HILLS, mm.HILLS_SH, mm.LEAF, pressed, 2)  # Green button with the word, leaving room on the left
    tri = big(PLAY, mm.WHITE)  # White triangle at 2x
    y = (36 - tri.height) // 2 + (1 if pressed else 0)  # Centre down; drop when pressed
    mm.paste(img, big(PLAY, mm.DARK), 30, y + 1)  # Shadow
    mm.paste(img, tri, 30, y)  # Triangle
    return img


def lantern(frame):
    """A 6x10 lantern hanging from the pole: a dark frame around a warm light that flickers in two frames."""
    img = new(6, 10)  # Empty picture
    for x in range(1, 5):  # The hook ring at the top
        put(img, x, 0 if x in (2, 3) else 1, mm.OUTLINE)  # Ring pixel
    for y in range(2, 9):  # The lantern body
        for x in range(0, 6):  # Each column
            edge = x in (0, 5) or y in (2, 8)  # Its dark frame
            put(img, x, y, mm.OUTLINE if edge else (SUN if frame == 0 or (x + y) % 2 else SUN_GLOW))  # Light inside, flickering
    return img


def save(img, *parts):
    """Saves a picture under the Main Menu sprite folder."""
    path = os.path.join(OUT, *parts)  # Full path
    os.makedirs(os.path.dirname(path), exist_ok=True)  # Make the folder if it is missing
    img.save(path)  # Write the PNG


def main():
    """Draws everything."""
    save(sky(), "Camp", "C0_sky.png")  # Still sky with the sun
    save(clouds(), "Camp", "C1_clouds.png")  # Drifting clouds
    save(peaks(), "Camp", "C2_peaks.png")  # Snowy peaks
    save(forest(), "Camp", "C3_forest.png")  # Pine band
    save(camp(), "Camp", "C4_camp.png")  # The clearing and the camp
    for f in range(4):  # Fire frames
        save(flame(f), "Camp", "fire_%d.png" % f)  # One flame frame
    for f in range(6):  # Spark frames
        save(embers(f), "Camp", "embers_%d.png" % f)  # One spark frame
    for f in range(2):  # Breathing frames
        save(standing(f), "Camp", "explorer_stand_%d.png" % f)  # One explorer frame
    for f in range(3):  # Pennant frames
        save(flag(f), "Camp", "flag_%d.png" % f)  # One flag frame
    for f in range(6):  # Twinkle frames
        save(glint(f), "Camp", "glint_%d.png" % f)  # One twinkle frame
    for f in range(2):  # Wing frames
        save(butterfly(f), "Camp", "butterfly_%d.png" % f)  # One butterfly frame
    for f in range(2):  # Lantern frames
        save(lantern(f), "Camp", "lantern_%d.png" % f)  # One lantern frame
    brown = (mm.PLATEAU, mm.PLATEAU_SH, mm.BACKPACK_SH)  # Top, face and bottom of brown buttons
    red = (mm.SCARF, mm.SCARF_SH, mm.BACKPACK_SH)  # Top, face and bottom of the red Exit button
    for name, rows, (top, face, bottom) in (("settings_icon", GEAR, brown), ("tutorial_icon", BOOK, brown), ("exit_icon", DOOR, red)):  # Each icon button
        save(icon_button(rows, face, top, bottom, False), "Buttons", "btn_%s.png" % name)  # Normal picture
        save(icon_button(rows, face, top, bottom, True), "Buttons", "btn_%s_pressed.png" % name)  # Pressed picture
    save(play_button(False), "Buttons", "btn_play_big.png")  # Big PLAY
    save(play_button(True), "Buttons", "btn_play_big_pressed.png")  # Big PLAY, pressed
    print("Main Menu camp art written to", OUT)  # Tell the person running it where the files went


if __name__ == "__main__":
    main()  # Draw everything when run as a script
