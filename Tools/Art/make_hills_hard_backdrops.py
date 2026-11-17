"""Draw six parallax layers for each Hills Hard theme."""  # Describes the generated art
import math  # Gives sin and pi for the hill lines
import random  # Gives fixed scatter for rain and grass
from pathlib import Path  # Gives folders for the layers
from PIL import Image, ImageDraw  # Draws crisp pixels and labels
import make_mainmenu_art as mm  # Gives the game palette
import make_landform_map_art as lm  # Gives landscape colours
import make_normal_backdrops as nb  # Gives the sky, clouds, crowns and preview screen
import make_hills_easy_backdrops as he  # Gives the rounded hill shape
import make_hills_normal_backdrops as hn  # Gives polygons, soft banks, skies and leaves

W, H = nb.W, nb.H  # Uses the same 960 by 270 layers as the other hills
PREVIEW = Path(nb.ROOT) / ".scratch" / "hills-hard" / "backdrop-preview.png"  # Holds the review sheet
M = nb.mix  # Blends two palette colours by a share
put, column, rock, bank, sky, clouds, leaves = nb.put, nb.column, hn.rock, hn.bank, hn.sky, hn.clouds, hn.leaves  # Shares the drawing helpers
BACKING = M(mm.STRAP, lm.MOUNTAIN_SH, 0.5)  # Shows clear pixels in the preview

TF_SKY = [(0, M(mm.MASK_SH, lm.HAZE_SH, 0.5)), (100, M(mm.MASK, lm.HAZE, 0.5)), (185, M(mm.SKY_LIGHT, mm.MASK, 0.5))]  # Soft overcast over the forest
TF_CLOUD = M(mm.MASK, mm.WHITE, 0.4)  # Pale rain clouds
TF_CLOUD_SH = M(mm.MASK_SH, lm.HAZE, 0.5)  # Their grey undersides
TF_RAIN = M(mm.MASK, lm.HAZE, 0.3)  # Faint far rain
TF_FAR = M(mm.LEAF, lm.HAZE, 0.7)  # Far hills in humid haze
TF_FAR_SH = M(mm.LEAF, lm.HAZE_SH, 0.55)  # Their shaded sides
TF_HILL = M(mm.HILLS_SH, lm.HAZE, 0.45)  # Nearer forest hills
TF_HILL_SH = M(mm.LEAF, lm.HAZE_SH, 0.4)  # Their deep folds
TF_MIST = M(mm.MASK, mm.WHITE, 0.3)  # Humid haze between hills
TF_TRUNK = M(mm.GROUND_SH, lm.HAZE_SH, 0.4)  # Tall wet trunks
TF_CROWN = M(mm.HILLS_SH, lm.HAZE, 0.3)  # Lit tree tops
TF_CROWN_SH = M(mm.LEAF, lm.HAZE_SH, 0.3)  # Shaded tree tops
TF_VINE = M(mm.LEAF, lm.HAZE, 0.35)  # Hanging vines
TF_FERN = M(mm.LEAF, lm.HAZE, 0.25)  # Low forest ferns
TF_FERN_TIP = M(mm.HILLS, lm.HAZE, 0.35)  # Their light tips

RS_SKY = [(0, M(mm.STRAP, lm.MOUNTAIN, 0.6)), (90, M(mm.MASK_SH, lm.HAZE_SH, 0.4)), (175, M(mm.MASK, lm.HAZE, 0.5))]  # Calm grey rain sky
RS_CLOUD = M(mm.MASK_SH, lm.HAZE, 0.55)  # Soft rain clouds
RS_CLOUD_SH = M(mm.MASK_SH, lm.MOUNTAIN, 0.4)  # Their darker bottoms
RS_RAIN = M(mm.MASK, lm.HAZE, 0.5)  # Light drizzle streaks
RS_FAR = M(mm.HILLS_SH, lm.HAZE_SH, 0.65)  # Far hills in the rain
RS_FAR_SH = M(mm.LEAF, lm.HAZE_SH, 0.6)  # Their shaded line
RS_WALL = M(mm.MASK, lm.HAZE, 0.45)  # Pale far house walls
RS_ROOF = M(mm.PLATEAU_SH, lm.HAZE_SH, 0.55)  # Far rusty roofs
RS_GRASS = M(mm.HILLS_SH, lm.HAZE, 0.4)  # Wet grassy hills
RS_GRASS_LIT = M(mm.HILLS, lm.HAZE, 0.45)  # Their lit edges
RS_CLAY = M(mm.PLATEAU_SH, mm.HILLS_SH, 0.3)  # Red-brown clay patches
RS_CLAY_SH = M(mm.PLATEAU_SH, mm.HILLS_SH, 0.55)  # Shaded clay
RS_NEAR = M(mm.HILLS_SH, lm.HAZE_SH, 0.4)  # Near grass
RS_NEAR_LIT = M(mm.HILLS, lm.HAZE, 0.5)  # Near grass tops
RS_TREE = M(mm.LEAF, lm.HAZE_SH, 0.35)  # Small wet trees
RS_TREE_LIT = M(mm.HILLS_SH, lm.HAZE, 0.35)  # Their lit side

KR_SKY = [(0, M(mm.MASK_SH, lm.MOUNTAIN, 0.4)), (100, M(mm.MASK, lm.HAZE_SH, 0.5)), (180, M(mm.MASK, mm.SKY_LIGHT, 0.5))]  # Light rain over the ridge
KR_CLOUD = M(mm.MASK, mm.WHITE, 0.3)  # Pale low clouds
KR_CLOUD_SH = M(mm.MASK_SH, lm.HAZE, 0.5)  # Their soft bases
KR_RAIN = M(mm.MASK, lm.HAZE, 0.4)  # Light rain streaks
KR_FAR = M(mm.MASK_SH, lm.HAZE, 0.65)  # Far pinnacles in haze
KR_FAR_SH = M(mm.MASK_SH, lm.HAZE_SH, 0.5)  # Their shaded faces
KR_ROCK = M(mm.MASK_SH, lm.HAZE, 0.45)  # Grey limestone
KR_ROCK_SH = M(mm.MASK_SH, lm.MOUNTAIN, 0.5)  # Limestone in shade
KR_FOREST = M(mm.HILLS_SH, lm.HAZE, 0.45)  # Forest around the rocks
KR_FOREST_SH = M(mm.LEAF, lm.HAZE_SH, 0.4)  # Shaded forest
KR_ROPE = M(mm.VEST_SH, mm.GROUND_SH, 0.5)  # Rope trails between rocks
KR_NEAR = M(mm.MASK_SH, lm.HAZE_SH, 0.35)  # Near rocks kept soft
KR_NEAR_LIT = M(mm.MASK_SH, lm.HAZE, 0.5)  # Their lit edges

WH_SKY = [(0, M(lm.SKY, lm.MOUNTAIN, 0.35)), (95, lm.SKY), (170, M(mm.SKY_LIGHT, lm.HAZE, 0.4))]  # Clear windy sky
WH_CLOUD = M(mm.WHITE, mm.SKY_LIGHT, 0.3)  # Thin fast clouds
WH_CLOUD_SH = M(mm.SKY_LIGHT, lm.HAZE, 0.5)  # Their light shade
WH_SHORE = M(lm.HAZE_SH, lm.MOUNTAIN, 0.3)  # Far hills across the lake
WH_SHORE_SH = M(lm.HAZE_SH, lm.MOUNTAIN, 0.5)  # Their shaded sides
WH_LAKE = M(lm.LENS, lm.HAZE, 0.55)  # Laguna lake far below
WH_LAKE_LIGHT = M(mm.SKY_LIGHT, lm.HAZE, 0.3)  # Wind streaks on the lake
WH_RIDGE = M(mm.HILLS_SH, lm.HAZE, 0.5)  # The long grassy ridge
WH_RIDGE_LIT = M(mm.PLAINS, lm.HAZE, 0.5)  # Its sunny crest
WH_TURBINE = M(mm.WHITE, lm.HAZE, 0.15)  # White turbine towers
WH_TURBINE_SH = M(mm.MASK, lm.HAZE_SH, 0.4)  # Their shaded sides
WH_SLOPE = M(mm.HILLS_SH, lm.HAZE, 0.3)  # Nearer grassy slope
WH_SLOPE_LIT = M(mm.PLAINS, lm.HAZE, 0.45)  # Grass bent by wind
WH_GRASS = M(mm.HILLS_SH, lm.HAZE, 0.3)  # Near tall grass
WH_GRASS_TIP = M(mm.PLAINS, lm.HAZE, 0.3)  # Its pale tips

DB_SKY = [(0, M(mm.MASK_SH, lm.HAZE, 0.5)), (110, M(mm.MASK, lm.HAZE, 0.45)), (160, M(mm.SKY_LIGHT, mm.MASK, 0.5))]  # Soft grey Irish sky
DB_CLOUD = M(mm.MASK, mm.WHITE, 0.45)  # Soft clouds
DB_CLOUD_SH = M(mm.MASK_SH, lm.HAZE, 0.6)  # Their grey bottoms
DB_CONE = M(mm.MASK_SH, lm.HAZE_SH, 0.55)  # Croagh Patrick in haze
DB_CONE_SH = M(lm.MOUNTAIN, lm.HAZE_SH, 0.5)  # Its shaded side
DB_SCREE = M(mm.MASK, lm.HAZE, 0.5)  # Pale scree near the top
DB_SHORE = M(mm.LEAF, lm.HAZE, 0.65)  # Far green shore
DB_SEA = M(lm.MOUNTAIN, lm.HAZE, 0.6)  # Grey bay water
DB_SEA_LIGHT = M(mm.MASK, lm.HAZE, 0.5)  # Light on the water
DB_ISLE_FAR = M(mm.HILLS_SH, lm.HAZE, 0.6)  # Far drumlins
DB_ISLE_FAR_SH = M(mm.LEAF, lm.HAZE, 0.55)  # Their shaded sides
DB_ISLE = M(mm.HILLS_SH, lm.HAZE, 0.4)  # Nearer drumlins
DB_ISLE_SH = M(mm.LEAF, lm.HAZE_SH, 0.4)  # Their shaded sides
DB_CLAY = M(mm.VEST_SH, lm.HAZE, 0.4)  # Bare clay cliffs on the drumlins
DB_GRASS = M(mm.HILLS_SH, lm.HAZE_SH, 0.35)  # Near shore grass
DB_GRASS_LIT = M(mm.HILLS, lm.HAZE, 0.4)  # Its top edge
DB_STONE = M(mm.MASK_SH, lm.HAZE_SH, 0.4)  # Shore stones

CH_SKY = [(0, M(lm.SKY, lm.HAZE, 0.5)), (95, M(mm.SKY_LIGHT, mm.VEST, 0.3)), (175, M(mm.HEADLAMP, mm.PLATEAU, 0.3))]  # Warm Roman sky
CH_CLOUD = M(mm.WHITE, mm.HEADLAMP, 0.4)  # Warm clouds
CH_CLOUD_SH = M(mm.VEST, mm.SKY_LIGHT, 0.5)  # Their golden bottoms
CH_FAR = M(mm.VEST, mm.SKY_LIGHT, 0.45)  # Far city in warm haze
CH_FAR_SH = M(mm.VEST_SH, mm.SKY_LIGHT, 0.45)  # Far shaded domes
CH_HILL = M(mm.HILLS_SH, mm.VEST, 0.45)  # Low green hills
CH_HILL_LIT = M(mm.PLAINS, mm.VEST, 0.5)  # Their sunny tops
CH_WALL = M(mm.HEADLAMP, mm.VEST, 0.5)  # Ochre walls
CH_WALL_SH = M(mm.VEST, mm.VEST_SH, 0.5)  # Shaded walls
CH_ROOF = M(mm.PLATEAU, mm.SCARF_SH, 0.3)  # Terracotta roofs
CH_ROOF_SH = M(mm.PLATEAU_SH, mm.SCARF_SH, 0.4)  # Shaded roofs
CH_DOME = M(mm.MASK_SH, lm.HAZE_SH, 0.4)  # Grey lead domes
CH_PINE = M(mm.LEAF, lm.MOUNTAIN_SH, 0.3)  # Umbrella pine needles
CH_PINE_LIT = M(mm.HILLS_SH, mm.LEAF, 0.4)  # Their sunny tops
CH_TRUNK = M(mm.GROUND_SH, mm.VEST_SH, 0.4)  # Pine trunks
CH_RUIN = M(mm.VEST, mm.MASK, 0.45)  # Old pale stone
CH_RUIN_SH = M(mm.VEST_SH, mm.MASK_SH, 0.5)  # Old stone in shade
CH_BRICK = M(mm.PLATEAU_SH, mm.VEST_SH, 0.5)  # Old Roman brick
CH_GRASS = M(mm.HILLS_SH, mm.VEST_SH, 0.4)  # Dry near grass

KT_SKY = [(0, M(lm.HAZE, lm.SKY, 0.4)), (100, M(mm.SKY_LIGHT, mm.MASK, 0.3)), (175, M(mm.SKY_LIGHT, mm.WHITE, 0.4))]  # Misty pale sky
KT_CLOUD = M(mm.SKY_LIGHT, mm.WHITE, 0.5)  # Thin mist clouds
KT_CLOUD_SH = M(mm.SKY_LIGHT, lm.HAZE, 0.5)  # Their soft bottoms
KT_FAR = M(lm.HAZE, mm.SKY_LIGHT, 0.35)  # Farthest towers, almost sky
KT_FAR_SH = M(lm.HAZE_SH, mm.SKY_LIGHT, 0.3)  # Their shaded sides
KT_MID = M(mm.LEAF, lm.HAZE, 0.6)  # Middle towers in mist
KT_MID_SH = M(mm.LEAF, lm.HAZE_SH, 0.45)  # Their shaded sides
KT_NEAR = M(mm.LEAF, lm.HAZE, 0.4)  # Nearer towers
KT_NEAR_SH = M(mm.LEAF, lm.MOUNTAIN_SH, 0.35)  # Their shaded sides
KT_MIST = M(mm.SKY_LIGHT, mm.WHITE, 0.5)  # Mist between the layers
KT_RIVER = M(lm.LENS, lm.HAZE, 0.55)  # The calm Li River
KT_RIVER_LIGHT = M(mm.SKY_LIGHT, mm.WHITE, 0.3)  # Light on the river
KT_RAFT = M(mm.VEST_SH, lm.HAZE, 0.3)  # Bamboo raft far away
KT_BAMBOO = M(mm.HILLS_SH, lm.HAZE, 0.35)  # Bamboo leaves
KT_BAMBOO_SH = M(mm.LEAF, lm.HAZE_SH, 0.35)  # Shaded bamboo
KT_CANE = M(mm.PLAINS, lm.HAZE, 0.45)  # Bamboo canes
KT_BANK = M(mm.HILLS_SH, lm.HAZE_SH, 0.45)  # River bank grass


def empty():  # Makes a transparent layer
    return Image.new("RGBA", (W, H))  # Keeps the unopened parts clear


def pens(img):  # Gives a pen and each wrap shift for shapes that cross the edge
    draw = ImageDraw.Draw(img)  # Gets a flat colour pen
    return [(draw, dx) for dx in (-W, 0, W)]  # Repeats the shape on both sides


def line(img, points, colour, width=1):  # Draws a wrapped crisp line
    for draw, dx in pens(img):  # Draws once per wrap shift
        draw.line([(x + dx, y) for x, y in points], fill=colour, width=width)  # Keeps the line one colour


def oval(img, box, colour):  # Draws a wrapped filled ellipse
    x0, y0, x1, y1 = box  # Reads the bounds
    for draw, dx in pens(img):  # Draws once per wrap shift
        draw.ellipse((x0 + dx, y0, x1 + dx, y1), fill=colour)  # Fills the round shape


def rain(img, seed, count, top, bottom, colour, length=5):  # Draws calm slanted drizzle
    rng = random.Random(seed)  # Gives the same drops every run
    for _ in range(count):  # Places each drop
        x, y = rng.randrange(W), rng.randrange(top, bottom)  # Picks a spot
        for k in range(length):  # Draws the streak downwards
            put(img, x - k // 3, y + k, colour)  # Leans the streak a little left


def wavy(base, amp, waves, seed):  # Gives a seamless rolling top line
    tops = nb.rolling(base, amp, waves, round(seed * 10))  # Uses the shared looping wave with a whole-number seed
    return [top + round(amp * 0.3 * math.sin(2 * math.pi * x / W + seed)) for x, top in enumerate(tops)]  # Shifts each layer's hills by its own phase


def haze(img, top, depth, colour, peak=0.45, solid_only=True):  # Draws mist that fades in and out with no hard edge
    for y in range(top, min(H, top + depth)):  # Rows of the mist
        share = peak * math.sin(math.pi * (y - top) / depth)  # Thickest in the middle row
        for x in range(W):  # Each pixel in the row
            if solid_only and img.getpixel((x, y))[3] == 0:  # Leaves the open sky clear
                continue  # Skips this pixel
            if nb.dither(x, y, share):  # Keeps the mist dotted so shapes show through
                put(img, x, y, colour)  # Paints one mist pixel


def land(img, tops, lit, body):  # Fills under a top line
    nb.fill_below(img, tops, lit, body)  # Paints the land with a lit lip
    return tops  # Lets shapes sit on the land


def spire(img, x, base, w, h, lit, shade):  # Draws a sharp limestone pinnacle
    rock(img, x, base - h, w, h, [(0, 10), (2, 6), (3, 3), (4, 1), (5, 0), (6, 2), (7, 5), (8, 7), (10, 10)], lit)  # Shapes the rock
    rock(img, x, base - h, w, h, [(5, 0), (6, 2), (7, 5), (8, 7), (10, 10), (6, 10), (6, 5)], shade)  # Shades the right face


def tower(img, x, base, w, h, lit, shade):  # Draws a tall round-topped karst tower
    rock(img, x, base - h, w, h, [(0, 10), (1, 7), (1, 4), (2, 2), (3, 1), (5, 0), (7, 1), (8, 2), (9, 4), (9, 7), (10, 10)], lit)  # Shapes the tall tower
    rock(img, x, base - h, w, h, [(6, 1), (7, 1), (8, 2), (9, 4), (9, 7), (10, 10), (6, 10), (7, 6), (6, 3)], shade)  # Shades the right side


def tall_tree(img, x, base, h, r):  # Draws a tall rainforest tree with vines
    top = base - h  # Finds the crown height
    rock(img, x - 2, top, 5, h, [(2, 0), (8, 0), (10, 10), (0, 10)], TF_TRUNK)  # Raises a slim trunk
    for cx, cy, s in ((0, 0, r), (-r, r // 2, r * 2 // 3), (r, r // 3, r * 2 // 3)):  # Groups three crowns
        nb.crown(img, x + cx, top + cy, s, TF_CROWN, TF_CROWN_SH)  # Paints one crown
    for vx, length in ((-r, h // 3), (-r // 3, h // 2), (r // 2, h // 4), (r + 1, h // 3)):  # Hangs vines from the crown
        for k in range(length):  # Steps down the vine
            put(img, x + vx + (k // 6) % 2, top + r // 2 + k, TF_VINE)  # Lets the vine sway a little


def tf_sky():  # L0 shows a soft overcast sky
    return sky(TF_SKY)  # Paints the grey-green gradient


def tf_clouds():  # L1 shows rain clouds and faint far rain
    img = clouds([(110, 40, 180), (420, 30, 150), (700, 50, 210)], TF_CLOUD, TF_CLOUD_SH)  # Spreads wide grey clouds
    rain(img, 11, 160, 50, 150, TF_RAIN, 4)  # Adds a light far shower
    return img  # Gives back the clouds


def tf_far():  # L2 shows far forest hills in haze
    img = empty()  # Starts the far layer
    land(img, wavy(120, 14, 3, 0.4), TF_FAR, TF_FAR)  # Draws the far hill line
    land(img, wavy(146, 10, 4, 1.7), TF_FAR_SH, TF_FAR_SH)  # Adds a nearer hazy fold
    return img  # Gives back the far hills


def tf_mid():  # L3 shows forest hills with treetop bumps and haze
    img = empty()  # Starts the middle layer
    tops = land(img, wavy(152, 12, 2, 2.3), TF_HILL, TF_HILL_SH)  # Draws the forest hill body
    for x in range(0, W, 11):  # Lines crowns along the hill
        nb.crown(img, x, tops[x] + 2, 7 + x % 3, TF_HILL, TF_HILL_SH)  # Adds a treetop bump
    haze(img, 164, 40, M(TF_MIST, TF_HILL, 0.5), 0.22)  # Adds humid haze at the foot
    return img  # Gives back the middle hills


def tf_trees():  # L4 shows tall trees with vines
    img = empty()  # Starts the tree layer
    land(img, wavy(198, 4, 3, 0.9), TF_CROWN_SH, M(TF_CROWN_SH, TF_TRUNK, 0.4))  # Lays the dark undergrowth
    for x, h, r in ((40, 128, 18), (205, 150, 22), (330, 118, 16), (512, 140, 20), (690, 132, 19), (850, 146, 21)):  # Spaces tall trees widely
        tall_tree(img, x, 205, h, r)  # Paints one tree
    return img  # Gives back the trees


def tf_near():  # L5 shows low ferns
    img = empty()  # Starts the near layer
    for x, y, w, h in ((30, 228, 22, 26), (150, 230, 16, 18), (300, 228, 24, 28), (470, 229, 18, 20), (640, 228, 22, 26), (820, 230, 18, 22)):  # Spaces the ferns
        leaves(img, x, y, w, h, TF_FERN, TF_FERN_TIP)  # Grows one fern
    return img  # Gives back the near plants


def house(img, x, base, w, h, wall, roof):  # Draws a small far house
    rock(img, x, base - h, w, h, [(0, 4), (10, 4), (10, 10), (0, 10)], wall)  # Builds the walls
    rock(img, x, base - h, w, h, [(0, 4), (2, 0), (8, 0), (10, 4)], roof)  # Puts on the roof


def rs_sky():  # L0 shows a calm grey rain sky
    return sky(RS_SKY)  # Paints the grey gradient


def rs_clouds():  # L1 shows low clouds and drizzle
    img = clouds([(90, 34, 200), (360, 48, 170), (610, 30, 220), (860, 52, 150)], RS_CLOUD, RS_CLOUD_SH)  # Lays wide low clouds
    rain(img, 21, 320, 40, 200, RS_RAIN, 6)  # Adds gentle drizzle
    return img  # Gives back the clouds


def rs_far():  # L2 shows far hills with a town
    img = empty()  # Starts the far layer
    tops = land(img, wavy(138, 10, 2, 0.6), RS_FAR, RS_FAR_SH)  # Draws the far hill line
    rng = random.Random(5)  # Gives fixed house sizes
    for start, end in ((40, 210), (330, 460), (600, 790)):  # Groups houses on three hills
        for x in range(start, end, 9):  # Places houses close together
            w, h = rng.choice((6, 7, 8)), rng.choice((6, 8, 10))  # Varies the house sizes
            house(img, x, tops[x] + 4, w, h, RS_WALL, RS_ROOF)  # Draws one house
    rock(img, 120, tops[120] - 22, 8, 24, [(2, 10), (2, 3), (5, 0), (8, 3), (8, 10)], RS_WALL)  # Raises a church tower
    return img  # Gives back the far town


def clay(img, tops, spots, lit, shade):  # Draws red-brown bare clay patches on a hill
    for x, w, depth in spots:  # Visits each patch
        y = max(tops[x], tops[x + w // 2], tops[x + w - 1]) + 4  # Starts below the hill line so the patch sits inside the grass
        oval(img, (x, y, x + w, y + depth // 2), shade)  # Paints the damp clay edge
        oval(img, (x + 2, y, x + w - 2, y + depth // 2 - 2), lit)  # Paints the bare clay face


def rs_mid():  # L3 shows rolling grassy hills with clay
    img = empty()  # Starts the middle layer
    tops = land(img, wavy(162, 13, 2, 2.1), RS_GRASS_LIT, RS_GRASS)  # Draws the rolling hills
    clay(img, tops, ((80, 40, 16), (300, 30, 12), (540, 46, 18), (780, 34, 14)), RS_CLAY, RS_CLAY_SH)  # Opens clay patches
    for x in (180, 430, 690, 900):  # Places a few trees on the slopes
        nb.crown(img, x, tops[x] - 3, 5, RS_TREE_LIT, RS_TREE)  # Paints a small wet tree
    return img  # Gives back the middle hills


def rs_near():  # L4 shows nearer slopes with drizzle
    img = empty()  # Starts the near band
    tops = land(img, wavy(196, 7, 3, 0.2), RS_NEAR_LIT, RS_NEAR)  # Draws the near slope
    clay(img, tops, ((140, 56, 14), (470, 40, 12), (760, 60, 16)), RS_CLAY, RS_CLAY_SH)  # Opens clay patches
    return img  # Gives back the near slope


def tufts(img, spots, base, colour, tip, lean=0):  # Draws small grass tufts
    for x, h in spots:  # Visits each tuft
        for k, dx in enumerate((-3, -1, 0, 2, 4)):  # Draws five blades
            tall = h - abs(dx)  # Makes the middle blades taller
            line(img, [(x + dx, base), (x + dx + lean, base - tall)], colour)  # Draws the blade
            put(img, x + dx + lean, base - tall, tip)  # Lights its tip


def rs_grass():  # L5 shows low wet grass tufts
    img = empty()  # Starts the closest layer
    tufts(img, ((35, 9), (200, 7), (318, 10), (505, 8), (650, 9), (880, 8)), 229, RS_NEAR, RS_NEAR_LIT)  # Spaces the tufts
    return img  # Gives back the grass


def rope(img, a, b, sag, colour):  # Draws a sagging rope trail with posts
    (x0, y0), (x1, y1) = a, b  # Reads both ends
    for x in range(x0, x1 + 1):  # Steps along the rope
        t = (x - x0) / (x1 - x0)  # Finds how far along
        put(img, x, round(y0 + (y1 - y0) * t + sag * 4 * t * (1 - t)), colour)  # Hangs the rope in a curve
    for x, y in (a, b):  # Visits both ends
        column(img, x, y - 3, colour, y + 1)  # Plants a small post


def kr_sky():  # L0 shows a light-rain sky
    return sky(KR_SKY)  # Paints the soft grey gradient


def kr_clouds():  # L1 shows low clouds and light rain
    img = clouds([(140, 44, 190), (470, 32, 160), (760, 56, 200)], KR_CLOUD, KR_CLOUD_SH)  # Lays soft clouds
    rain(img, 31, 200, 40, 190, KR_RAIN, 5)  # Adds light rain
    return img  # Gives back the clouds


def kr_far():  # L2 shows a far jagged limestone ridge
    img = empty()  # Starts the far layer
    land(img, wavy(150, 6, 3, 1.1), KR_FAR_SH, KR_FAR_SH)  # Lays the ridge base
    rng = random.Random(7)  # Gives fixed spire sizes
    for x in range(0, W, 16):  # Fills the ridge with spires
        spire(img, x - 6, 156, rng.randrange(14, 24), rng.randrange(18, 46), KR_FAR, KR_FAR_SH)  # Raises one far spire
    return img  # Gives back the far ridge


def kr_mid():  # L3 shows grey pinnacles, forest and rope trails
    img = empty()  # Starts the middle layer
    peaks = ((20, 34, 70), (88, 26, 52), (160, 40, 86), (250, 30, 60), (330, 36, 78), (430, 28, 56), (520, 42, 90), (610, 30, 64), (700, 38, 80), (790, 28, 58), (870, 36, 74))  # Lists spire spots
    for x, w, h in peaks:  # Raises each pinnacle
        spire(img, x - w // 2, 192, w * 2, h, KR_ROCK, KR_ROCK_SH)  # Paints a broad grey rock
        spire(img, x + w // 3, 192, w, h * 7 // 10, KR_ROCK, KR_ROCK_SH)  # Adds a smaller point beside it
    tops = land(img, wavy(176, 6, 4, 0.5), KR_FOREST, KR_FOREST_SH)  # Wraps forest around the rock feet
    for x in range(0, W, 9):  # Lines crowns along the forest
        nb.crown(img, x, tops[x] + 1, 5, KR_FOREST, KR_FOREST_SH)  # Adds a treetop bump
    for i in range(0, len(peaks) - 1, 2):  # Joins some pinnacles with ropes
        (x0, w0, h0), (x1, w1, h1) = peaks[i], peaks[i + 1]  # Reads two neighbours
        rope(img, (x0 + w0 // 2, 192 - h0 // 2), (x1 + w1 // 2, 192 - h1 // 2), 8, KR_ROPE)  # Strings a rope trail
    return img  # Gives back the middle ridge


def kr_forest():  # L4 shows near forest with rock tips
    img = empty()  # Starts the forest layer
    for x, w, h in ((60, 30, 40), (300, 26, 34), (590, 34, 44), (820, 28, 36)):  # Places rock tips above the trees
        spire(img, x, 204, w, h, KR_ROCK, KR_ROCK_SH)  # Paints a closer pinnacle
    tops = land(img, wavy(200, 4, 3, 2.6), KR_FOREST_SH, KR_FOREST_SH)  # Lays the forest floor
    for x in range(0, W, 13):  # Lines crowns along the forest
        nb.crown(img, x, tops[x], 8 + x % 4, KR_FOREST, KR_FOREST_SH)  # Adds a tree crown
    return img  # Gives back the forest band


def kr_near():  # L5 shows small grey rocks and ferns
    img = empty()  # Starts the closest layer
    for x, w, h in ((110, 16, 18), (410, 20, 22), (720, 14, 16)):  # Places a few small rocks
        spire(img, x, 232, w, h, KR_NEAR_LIT, KR_NEAR)  # Paints one soft rock
    for x, w, h in ((40, 16, 18), (250, 20, 22), (560, 16, 18), (900, 18, 20)):  # Places a few ferns
        leaves(img, x, 229, w, h, KR_FOREST_SH, KR_FOREST)  # Grows one fern
    return img  # Gives back the near rocks


def turbine(img, x, base, h, lit, shade):  # Draws a still wind turbine
    rock(img, x - 2, base - h, 5, h, [(3, 0), (7, 0), (10, 10), (0, 10)], lit)  # Raises the thin tower
    rock(img, x - 2, base - h, 5, h, [(5, 0), (7, 0), (10, 10), (5, 10)], shade)  # Shades the right side
    hub = (x, base - h)  # Finds the hub
    blade = h * 6 // 10  # Sizes the blades to the tower
    for bx, by in ((0, -blade), (blade * 87 // 100, blade // 2), (-blade * 87 // 100, blade // 2)):  # Spaces three blades evenly
        line(img, [hub, (x + bx, base - h + by)], lit)  # Draws one blade
    put(img, x, base - h, shade)  # Marks the hub


def wh_sky():  # L0 shows a clear windy sky
    return sky(WH_SKY)  # Paints the blue gradient


def wh_clouds():  # L1 shows long fast clouds
    img = empty()  # Starts the cloud layer
    for x, y, w in ((60, 30, 150), (300, 52, 110), (420, 22, 190), (650, 44, 130), (820, 28, 170), (180, 70, 90), (560, 78, 100)):  # Stretches thin clouds
        for dy, share in ((0, 1.0), (2, 0.7), (4, 0.4)):  # Thins each cloud into streaks
            line(img, [(x, y + dy), (x + int(w * share), y + dy)], WH_CLOUD if dy < 4 else WH_CLOUD_SH, 2)  # Draws one wind streak
    return img  # Gives back the clouds


def wh_far():  # L2 shows Laguna lake and the far shore
    img = empty()  # Starts the far layer
    land(img, wavy(128, 8, 2, 1.3), WH_SHORE, WH_SHORE_SH)  # Draws hills across the lake
    haze(img, 118, 30, WH_CLOUD_SH, 0.35)  # Softens the far shore
    mm.fill_rect(img, 0, 146, W, H - 146, WH_LAKE)  # Lays the lake
    hn.flecks(img, [(30, 152, 50), (210, 156, 70), (390, 150, 40), (520, 158, 80), (720, 153, 60), (880, 157, 50)], WH_LAKE_LIGHT)  # Adds wind streaks on the water
    return img  # Gives back the lake view


def wh_ridge():  # L3 shows the wide ridge and its turbines
    img = empty()  # Starts the ridge layer
    tops = land(img, wavy(170, 6, 1, 0.3), WH_RIDGE_LIT, WH_RIDGE)  # Draws the long ridge
    for x in range(40, W, 120):  # Spaces turbines along the crest
        turbine(img, x, tops[x] + 2, 46, WH_TURBINE, WH_TURBINE_SH)  # Plants one turbine
    return img  # Gives back the ridge


def wh_slope():  # L4 shows a nearer slope with wind-bent grass
    img = empty()  # Starts the slope layer
    tops = land(img, wavy(200, 6, 2, 1.9), WH_SLOPE_LIT, WH_SLOPE)  # Draws the slope
    rng = random.Random(41)  # Gives fixed grass streaks
    for _ in range(70):  # Draws streaks of bent grass
        x = rng.randrange(W)  # Picks a spot
        y = tops[x] + rng.randrange(4, 22)  # Keeps it on the slope
        line(img, [(x, y), (x + rng.randrange(6, 14), y)], WH_SLOPE_LIT)  # Draws a pale wind streak
    return img  # Gives back the slope


def wh_grass():  # L5 shows tall grass leaning in the wind
    img = empty()  # Starts the closest layer
    tufts(img, ((25, 12), (140, 10), (290, 13), (440, 11), (600, 12), (760, 10), (900, 13)), 230, WH_GRASS, WH_GRASS_TIP, 4)  # Leans the tufts right
    return img  # Gives back the grass


def drumlin(img, cx, base, r, h, lit, shade, cliff=None):  # Draws a rounded drumlin island
    he.dome(img, cx, base, r, h, lit, shade)  # Shapes the smooth hump
    if cliff:  # Shows the bare clay face on the sea side
        rock(img, cx - r, base - h // 2, r // 2, h // 2 + 1, [(0, 10), (3, 2), (10, 0), (10, 10)], cliff)  # Cuts a small clay cliff


def db_sky():  # L0 shows a soft grey sky
    return sky(DB_SKY)  # Paints the grey gradient


def db_clouds():  # L1 shows soft grey clouds
    return clouds([(120, 38, 200), (430, 56, 160), (700, 34, 220)], DB_CLOUD, DB_CLOUD_SH)  # Lays wide soft clouds


def db_far():  # L2 shows Croagh Patrick, the far shore and the bay
    img = empty()  # Starts the far layer
    land(img, wavy(138, 4, 3, 0.8), DB_SHORE, DB_SHORE)  # Draws the low far shore
    rock(img, 230, 80, 180, 62, [(0, 10), (3, 6), (5, 1), (6, 0), (7, 1), (10, 10)], DB_CONE)  # Raises the cone of Croagh Patrick
    rock(img, 230, 80, 180, 62, [(6, 0), (7, 1), (10, 10), (7, 10), (6, 4)], DB_CONE_SH)  # Shades its far side
    for sx in (304, 312, 320):  # Runs three scree streaks down the lit face
        line(img, [(sx + 10, 82), (sx - 6, 104)], DB_SCREE)  # Draws one pale scree line
    mm.fill_rect(img, 0, 142, W, H - 142, DB_SEA)  # Lays the bay water
    for cx, r, h in ((60, 18, 6), (150, 12, 5), (470, 20, 7), (560, 14, 5), (650, 16, 6), (780, 22, 7), (900, 14, 5)):  # Spreads far drumlins
        drumlin(img, cx, 146, r, h, DB_ISLE_FAR, DB_ISLE_FAR_SH)  # Paints one far island
    return img  # Gives back the far bay


def db_isles():  # L3 shows many small drumlin islands
    img = empty()  # Starts the island layer
    rng = random.Random(53)  # Gives fixed island sizes
    for x in range(10, W, 46):  # Spreads islands across the bay
        cx, r = x + rng.randrange(-8, 9), rng.randrange(14, 26)  # Varies place and width
        drumlin(img, cx, 166 + rng.randrange(0, 8), r, r // 2, DB_ISLE, DB_ISLE_SH, DB_CLAY if r > 20 else None)  # Paints one island
    hn.flecks(img, [(40, 176, 30), (230, 180, 40), (420, 175, 34), (610, 181, 44), (820, 177, 36)], DB_SEA_LIGHT)  # Adds light on the water
    return img  # Gives back the islands


def db_near():  # L4 shows a few larger drumlins close by
    img = empty()  # Starts the near water layer
    bank(img, [(0, 194), (240, 192), (480, 195), (720, 192), (959, 194)], DB_SEA_LIGHT, DB_SEA)  # Lays nearer water
    for cx, r, h in ((110, 44, 22), (420, 52, 26), (760, 40, 20)):  # Places three larger islands
        drumlin(img, cx, 198, r, h, DB_ISLE, DB_ISLE_SH, DB_CLAY)  # Paints one near island
    return img  # Gives back the near islands


def db_shore():  # L5 shows a low grassy shore with stones
    img = empty()  # Starts the closest layer
    tops = bank(img, [(0, 216), (200, 212), (420, 218), (640, 211), (840, 217), (959, 216)], DB_GRASS_LIT, DB_GRASS)  # Draws the low shore
    for x, w in ((70, 10), (260, 8), (520, 12), (700, 9), (900, 10)):  # Places shore stones
        oval(img, (x, tops[x] - 3, x + w, tops[x] + 3), DB_STONE)  # Paints one rounded stone
    return img  # Gives back the shore


def umbrella_pine(img, x, base, h, w):  # Draws an Italian stone pine
    rock(img, x - 1, base - h, 3, h, [(3, 0), (7, 0), (10, 10), (0, 10)], CH_TRUNK)  # Raises the bare trunk
    oval(img, (x - w // 2, base - h - 7, x + w // 2, base - h + 3), CH_PINE)  # Spreads the flat canopy
    oval(img, (x - w // 2 + 3, base - h - 7, x + w // 2 - 2, base - h - 2), CH_PINE_LIT)  # Lights the canopy top


def cypress(img, x, base, h):  # Draws a slim cypress
    rock(img, x - 3, base - h, 7, h, [(5, 0), (8, 3), (10, 8), (7, 10), (3, 10), (0, 8), (2, 3)], CH_PINE)  # Shapes the narrow tree
    rock(img, x - 3, base - h, 7, h, [(5, 0), (6, 3), (5, 8), (3, 10), (0, 8), (2, 3)], CH_PINE_LIT)  # Lights its left side


def roman_dome(img, cx, base, r, colour, shade):  # Draws a church dome with a drum and lantern
    rock(img, cx - r, base - r // 2, 2 * r, r // 2, [(1, 0), (9, 0), (9, 10), (1, 10)], shade)  # Builds the drum
    oval(img, (cx - r, base - r // 2 - r, cx + r, base - r // 2 + r), colour)  # Rounds the dome
    rock(img, cx - r, base - r // 2, 2 * r, r + 1, [(0, 0), (10, 0), (10, 10), (0, 10)], shade)  # Covers the dome's lower half with the drum
    rock(img, cx - 2, base - r // 2 - r - 6, 5, 7, [(2, 0), (8, 0), (8, 10), (2, 10)], colour)  # Adds the lantern


def ch_sky():  # L0 shows a warm Roman sky
    img = sky(CH_SKY)  # Paints the warm gradient
    nb.sun(img, 620, 120, 16, mm.HEADLAMP, M(mm.HEADLAMP, mm.VEST, 0.5), M(mm.HEADLAMP, mm.PLATEAU, 0.3))  # Places a low warm sun
    return img  # Gives back the sky


def ch_clouds():  # L1 shows a few warm clouds
    return clouds([(150, 40, 150), (500, 28, 130), (800, 50, 170)], CH_CLOUD, CH_CLOUD_SH)  # Spaces the clouds


def ch_far():  # L2 shows the far city with domes
    img = empty()  # Starts the far layer
    tops = land(img, wavy(150, 5, 2, 1.0), CH_FAR, CH_FAR)  # Draws the far city hills
    rng = random.Random(61)  # Gives fixed building heights
    for x in range(0, W, 7):  # Lines far buildings
        h = rng.randrange(6, 15)  # Picks a height
        rock(img, x, tops[x] - h + 3, 6, h, [(0, 0), (10, 0), (10, 10), (0, 10)], CH_FAR if x % 14 else CH_FAR_SH)  # Draws one block in two tones
    for cx, r in ((140, 18), (420, 10), (610, 12), (840, 9)):  # Places far domes
        roman_dome(img, cx, tops[cx], r, CH_FAR, CH_FAR_SH)  # Paints one dome
    return img  # Gives back the far city


def ch_mid():  # L3 shows terracotta roofs, pines and cypresses on low hills
    img = empty()  # Starts the middle layer
    tops = land(img, wavy(172, 12, 2, 2.4), CH_HILL_LIT, CH_HILL)  # Draws the seven low hills
    rng = random.Random(67)  # Gives fixed house sizes
    for start, end in ((20, 150), (260, 400), (520, 640), (760, 900)):  # Groups houses on the hills
        for x in range(start, end, 11):  # Places houses close together
            w, h = rng.choice((10, 12, 14)), rng.choice((10, 13, 16))  # Varies the house sizes
            house(img, x, tops[x] + 3, w, h, CH_WALL if x % 22 else CH_WALL_SH, CH_ROOF if x % 33 else CH_ROOF_SH)  # Draws one house
    roman_dome(img, 330, tops[330] + 2, 14, CH_DOME, CH_WALL_SH)  # Adds a near grey dome
    for x in (180, 230, 470, 700, 940):  # Places umbrella pines
        umbrella_pine(img, x, tops[x] + 2, 18, 26)  # Paints one pine
    for x in (160, 420, 500, 730, 915):  # Places cypresses
        cypress(img, x, tops[x] + 2, 24)  # Paints one cypress
    return img  # Gives back the hills


def arcade(img, x, base, arches, w, h):  # Draws a broken row of Roman arches
    rock(img, x, base - h, arches * w, h, [(0, 2), (10, 4), (10, 10), (0, 10)], CH_RUIN)  # Builds the broken wall
    for i in range(arches):  # Cuts each arch opening
        ax = x + i * w + w // 4  # Finds the opening
        rock(img, ax, base - h * 6 // 10, w // 2, h * 6 // 10, [(0, 3), (2, 1), (5, 0), (8, 1), (10, 3), (10, 10), (0, 10)], CH_RUIN_SH)  # Shades the arch inside
    rock(img, x, base - h, arches * w, 3, [(0, 0), (10, 0), (10, 10), (0, 10)], CH_BRICK)  # Shows old brick on top


def ch_ruins():  # L4 shows ancient ruins and pines
    img = empty()  # Starts the ruin layer
    land(img, wavy(204, 3, 3, 0.7), CH_GRASS, CH_GRASS)  # Lays the dry ground
    arcade(img, 60, 206, 4, 22, 48)  # Builds one ruined arcade
    arcade(img, 560, 206, 3, 24, 40)  # Builds another arcade
    for x, h in ((300, 50), (330, 36), (800, 46)):  # Stands lone old columns
        rock(img, x, 206 - h, 8, h, [(0, 0), (10, 0), (9, 10), (1, 10)], CH_RUIN)  # Raises one column
        rock(img, x, 206 - h, 8, h, [(6, 0), (10, 0), (9, 10), (6, 10)], CH_RUIN_SH)  # Shades its right side
    for x in (230, 450, 700, 900):  # Places pines among the ruins
        umbrella_pine(img, x, 206, 40, 40)  # Paints one pine
    return img  # Gives back the ruins


def ch_near():  # L5 shows broken column bases and dry grass
    img = empty()  # Starts the closest layer
    for x, w, h in ((90, 14, 14), (380, 18, 10), (640, 12, 16), (870, 16, 12)):  # Places broken bases
        rock(img, x, 232 - h, w, h, [(0, 1), (4, 0), (10, 2), (10, 10), (0, 10)], CH_RUIN)  # Paints one broken drum
        rock(img, x, 232 - h, w, h, [(7, 1), (10, 2), (10, 10), (7, 10)], CH_RUIN_SH)  # Shades its right side
    tufts(img, ((40, 8), (230, 7), (520, 9), (760, 8)), 229, CH_GRASS, CH_HILL_LIT)  # Adds dry grass
    return img  # Gives back the near ruins


def bamboo(img, x, base, h):  # Draws a bamboo clump
    for dx, k in ((-4, 9), (0, 10), (4, 8)):  # Grows three canes
        top = base - h * k // 10  # Finds the cane top
        line(img, [(x + dx, base), (x + dx + k // 4, top)], KT_CANE, 2)  # Draws the cane
        for y in range(top, base - 4, 7):  # Places leaf sprays up the cane
            line(img, [(x + dx, y), (x + dx - 4, y + 2)], KT_BAMBOO_SH)  # Adds a left leaf
            line(img, [(x + dx + 1, y + 2), (x + dx + 5, y + 3)], KT_BAMBOO)  # Adds a right leaf


def raft(img, x, y):  # Draws a far bamboo raft with a boatman
    rock(img, x, y, 22, 3, [(0, 0), (10, 0), (9, 10), (1, 10)], KT_RAFT)  # Lays the raft
    rock(img, x + 9, y - 6, 4, 6, [(2, 0), (8, 0), (10, 10), (0, 10)], KT_NEAR_SH)  # Stands the boatman
    line(img, [(x + 14, y - 8), (x + 20, y + 4)], KT_RAFT)  # Angles his pole


def kt_sky():  # L0 shows a misty pale sky
    return sky(KT_SKY)  # Paints the soft gradient


def kt_far():  # L1 shows the farthest towers fading into mist
    img = empty()  # Starts the far towers
    land(img, wavy(146, 2, 3, 0.9), KT_FAR, KT_FAR)  # Joins the tower feet with a low far bank
    rng = random.Random(71)  # Gives fixed tower sizes
    for x in range(0, W, 40):  # Spreads the towers
        tower(img, x + rng.randrange(-10, 11), 150, rng.randrange(20, 44), rng.randrange(36, 96), KT_FAR, KT_FAR_SH)  # Paints one pale tower at an uneven spot
    haze(img, 100, 60, M(KT_MIST, KT_FAR, 0.4), 0.35)  # Softens the tower feet
    return img  # Gives back the far towers


def kt_mid():  # L2 shows middle towers in misty layers
    img = empty()  # Starts the middle towers
    land(img, wavy(170, 3, 2, 2.2), KT_MID, KT_MID)  # Joins the tower feet with a low green bank
    rng = random.Random(73)  # Gives fixed tower sizes
    for x in range(10, W, 64):  # Spreads the towers
        tower(img, x + rng.randrange(-14, 15), 176, rng.randrange(28, 52), rng.randrange(50, 116), KT_MID, KT_MID_SH)  # Paints one green tower at an uneven spot
    haze(img, 120, 56, M(KT_MIST, KT_MID, 0.3), 0.4)  # Lays mist across their feet
    return img  # Gives back the middle towers


def kt_near():  # L3 shows nearer towers above the river
    img = empty()  # Starts the near towers
    for x, w, h in ((30, 60, 120), (210, 46, 90), (380, 70, 140), (600, 52, 100), (760, 64, 128)):  # Places the large towers
        tower(img, x, 186, w, h, KT_NEAR, KT_NEAR_SH)  # Paints one tower
    return img  # Gives back the near towers


def kt_river():  # L4 shows the Li River, a raft and bamboo
    img = empty()  # Starts the river layer
    bank(img, [(0, 184), (240, 186), (480, 183), (720, 186), (959, 184)], KT_RIVER_LIGHT, KT_RIVER)  # Lays the river
    hn.flecks(img, [(20, 192, 40), (180, 197, 30), (330, 191, 50), (540, 196, 36), (700, 192, 44), (880, 197, 30)], KT_RIVER_LIGHT)  # Adds light on the water
    raft(img, 280, 192)  # Floats one raft
    raft(img, 720, 194)  # Floats another raft
    land(img, wavy(204, 2, 3, 1.2), KT_BANK, KT_BANK)  # Draws the near bank
    return img  # Gives back the river


def kt_bamboo():  # L5 shows bamboo clumps by the trail
    img = empty()  # Starts the closest layer
    for x, h in ((50, 40), (330, 34), (600, 44), (850, 36)):  # Spaces the clumps
        bamboo(img, x, 230, h)  # Grows one clump
    return img  # Gives back the bamboo


THEMES = {  # One entry per level theme in L0 to L5 order, with preview ground tiles
    "TarsierForest": ((tf_sky, tf_clouds, tf_far, tf_mid, tf_trees, tf_near), "moss"),  # Corella forest hills
    "RainySlope": ((rs_sky, rs_clouds, rs_far, rs_mid, rs_near, rs_grass), "storm"),  # Antipolo rainy hills
    "KarstRidge": ((kr_sky, kr_clouds, kr_far, kr_mid, kr_forest, kr_near), "gorge"),  # Masungi limestone ridge
    "WindHills": ((wh_sky, wh_clouds, wh_far, wh_ridge, wh_slope, wh_grass), "pasture"),  # Pililla wind farm ridge
    "DrumlinBay": ((db_sky, db_clouds, db_far, db_isles, db_near, db_shore), "island"),  # Clew Bay drumlins
    "CityHills": ((ch_sky, ch_clouds, ch_far, ch_mid, ch_ruins, ch_near), "terrace"),  # Rome on its seven hills
    "KarstTowers": ((kt_sky, kt_far, kt_mid, kt_near, kt_river, kt_bamboo), "gorge"),  # Guilin karst towers
}  # Ends the theme table


def preview(shots):  # Shows a 480 by 270 view and all six separate layers
    sheet = Image.new("RGBA", (1094, 298 * len(shots)), mm.OUTLINE + (255,))  # Leaves room for labels and gaps
    pen = ImageDraw.Draw(sheet)  # Writes preview labels
    for i, (theme, stacked, layers) in enumerate(shots):  # Places one theme per row
        y = i * 298 + 8  # Finds this theme's row
        pen.text((8, y), theme, fill=mm.HEADLAMP)  # Names the theme
        sheet.alpha_composite(stacked, (8, y + 16))  # Shows the game-sized view
        for j, img in enumerate(layers):  # Shows each full-width layer alone
            x, top = 502 + (j % 2) * 294, y + (j // 2) * 94  # Places six thumbnails in two columns
            mm.fill_rect(sheet, x, top + 13, 288, 81, BACKING)  # Shows clear pixels against an even backing
            sheet.alpha_composite(img.resize((288, 81), Image.Resampling.NEAREST), (x, top + 13))  # Keeps the whole layer visible
            pen.text((x, top), f"L{j}", fill=mm.HEADLAMP)  # Names this layer
    PREVIEW.parent.mkdir(parents=True, exist_ok=True)  # Makes the review folder
    sheet.save(PREVIEW)  # Writes the review sheet


def main():  # Writes the seven background sets
    shots = []  # Keeps the layers for inspection
    for theme, (makers, ground) in THEMES.items():  # Makes one set at a time
        folder = Path(nb.BACKDROPS) / theme  # Finds the named Unity art folder
        folder.mkdir(parents=True, exist_ok=True)  # Creates the theme folder
        layers = [make() for make in makers]  # Draws all six layers from far to near
        for i, img in enumerate(layers):  # Saves each layer at its exact size
            img.paste(img.crop((0, 0, 1, H)), (W - 1, 0))  # Matches both edge columns
            img.save(folder / f"L{i}.png")  # Keeps the builder's layer names
        shots.append((theme, nb.preview_screen(layers, ground), layers))  # Shows the theme over walking tiles
    preview(shots)  # Saves the stacked views and layer thumbnails
    print(PREVIEW)  # Prints the preview path


if __name__ == "__main__":  # Runs only when started as a script
    main()  # Writes the backdrop art
