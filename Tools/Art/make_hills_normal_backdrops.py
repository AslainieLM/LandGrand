"""Draw six parallax layers for each Hills Normal theme."""  # Describes the generated art
from pathlib import Path  # Gives folders for the layers
from PIL import Image, ImageDraw  # Draws crisp pixels and labels
import make_mainmenu_art as mm  # Gives the game palette
import make_landform_map_art as lm  # Gives landscape colours
import make_normal_art as na  # Gives two-colour palette mixes
import make_normal_backdrops as nb  # Gives the sky, clouds and backdrop preview

W, H = nb.W, nb.H  # Uses the same 960 by 270 layers as Hills Easy
PREVIEW = Path(nb.ROOT) / ".scratch" / "hills-normal" / "backdrop-preview.png"  # Holds the review sheet
mix = na.mix  # Shares the palette blend
SEA = mix(lm.LENS, lm.HAZE)  # Deep enough to read as the sea
SEA_SHADE = mix(lm.LENS, lm.MOUNTAIN)  # Shaded water near the trail
SEA_LIGHT = mix(lm.LENS, mm.SKY_LIGHT)  # Calm reflected sky
ISLAND_LIME = mix(mm.MASK, mm.HEADLAMP)  # Sunlit coastal limestone
ISLAND_SHADE = mix(mm.MASK_SH, mm.VEST)  # Limestone under the greenery
ISLAND_GREEN = mix(mm.HILLS_SH, lm.HAZE_SH)  # Lighter far-away trees
ISLAND_NEAR = mix(mm.LEAF, lm.MOUNTAIN_SH)  # Deep coastal greenery
CAVE_DARK = mix(mm.OUTLINE, mm.STRAP)  # Soft shadow in the cave
CAVE_GREY = mix(mm.STRAP, lm.MOUNTAIN_SH)  # Cool walls away from the cave mouth
CAVE_LIT = mix(CAVE_GREY, mm.MASK_SH)  # Dim light on the cave rock
CAVE_EARTH = mix(CAVE_DARK, mm.GROUND_SH)  # Warm brown of wet flowstone
CAVE_GLOW = mix(mm.MASK_SH, lm.HAZE_SH)  # Cool light from the cave mouth
CAVE_RAY = mix(CAVE_GLOW, CAVE_LIT)  # Muted light off the direct path
GORGE_FAR = mix(mm.HILLS_SH, lm.HAZE)  # Hazy gorge foliage
GORGE_MOSS = mix(mm.HILLS_SH, lm.MOUNTAIN_SH)  # Moss in shade
GORGE_FACE = mix(mm.LEAF, lm.MOUNTAIN_SH)  # Dark green cliff
GORGE_STONE = mix(mm.MASK_SH, mm.LEAF)  # Grey-green bare cliff
RIVER = mix(lm.LENS, mm.HILLS_SH)  # Water taking on forest colours
RIVER_EDGE = mix(lm.LENS, mm.PLAINS)  # A few light streaks on the river
TERRACE_FAR = mix(mm.LEAF, lm.HAZE)  # Distant green mountains in the mist
TERRACE_MID = mix(mm.HILLS, lm.HAZE_SH)  # Sunlit distant rice
TERRACE_BODY = mix(mm.LEAF, mm.GROUND_SH)  # Terrace banks below their grass lips
PADDY = mix(lm.LENS, mm.HILLS)  # Water and young rice
PADDY_LIGHT = mix(mm.SKY_LIGHT, mm.HILLS)  # A cool reflection in standing water
TUFF = mix(mm.VEST, mm.HEADLAMP)  # Pale volcanic soil
TUFF_FAR = mix(mm.VEST, lm.HAZE)  # Distant tuff softened by dawn
TUFF_SHADE = mix(mm.PLATEAU, mm.VEST)  # Warm shade to the right of a cone
CAP = mix(mm.VEST_SH, mm.GROUND_SH)  # The darker rock atop the chimneys
DAWN = mix(mm.SCARF, mm.SKY_LIGHT)  # Warm dawn below the blue sky


def empty():  # Makes a transparent layer
    return Image.new("RGBA", (W, H))  # Keeps the unopened parts clear


def rock(img, x, y, w, h, points, colour):  # Places a pixel polygon with coordinates on a ten-point grid
    draw = ImageDraw.Draw(img)  # Gets a flat colour pen
    pts = [(x + px * w // 10, y + py * h // 10) for px, py in points]  # Scales the outline into its box
    for dx in (-W, 0, W):  # Wraps objects that cross the tile edge
        if -w <= x + dx < W:  # Skips parts outside this tile
            draw.polygon([(px + dx, py) for px, py in pts], fill=colour)  # Draws the shape with no smoothing


def profile(knots):  # Makes a hillside with a soft curve from point to point
    tops = []  # Holds one top row for each x
    for (left, low), (right, high) in zip(knots, knots[1:]):  # Joins each pair of profile points
        for x in range(left, right):  # Steps along the join
            share = (x - left) / (right - left)  # Finds how far this column is along
            blend = share * share * (3 - 2 * share)  # Rounds the slope at each end
            tops.append(round(low + (high - low) * blend))  # Snaps the top to a pixel
    tops.append(knots[-1][1])  # Keeps the same top at both edges
    return tops  # Gives the level line to the fill helper


def bank(img, knots, lip, body):  # Places one continuous ground or water bank
    tops = profile(knots)  # Builds a seamless top line
    nb.fill_below(img, tops, lip, body)  # Paints the whole layer below that line
    return tops  # Lets other shapes follow the bank


def clouds(places, light, shade):  # Draws sparse wisps with the existing sky helper
    img = empty()  # Starts the cloud layer
    for x, y, width in places:  # Places each wisp
        nb.flat_cloud(img, x, y, width, light, shade)  # Shapes a flat pixel cloud
    return img  # Gives back the layer


def flecks(img, waves, colour):  # Draws a few pieces of reflected light
    for x, y, w in waves:  # Moves to each small ripple
        mm.fill_rect(img, x, y, w, 1, colour)  # Adds a horizontal flash


def sky(stops):  # Draws a smooth-looking pixel sky from the supplied palette stops
    shades = []  # Adds small colour steps to soften the dither
    for (start, low), (end, high) in zip(stops, stops[1:]):  # Walks from each palette colour to the next
        for step in range(4):  # Adds three blends between the original stops
            shades.append((start + (end - start) * step // 4, nb.mix(low, high, step / 4)))  # Keeps flat patches from looking speckled
    return nb.gradient(shades + [stops[-1]])  # Paints the softened sky


def sea_stack(img, x, y, w, h, far):  # Shapes an undercut limestone island
    base = ISLAND_GREEN if far else ISLAND_LIME  # Gives far rock a haze tint
    shade = mix(ISLAND_GREEN, lm.MOUNTAIN) if far else ISLAND_SHADE  # Darkens the far face
    cap = mix(ISLAND_GREEN, mm.HILLS) if far else mm.HILLS_SH  # Caps the rock with forest
    rock(img, x, y, w, h, [(0, 6), (1, 3), (3, 1), (5, 0), (7, 1), (9, 3), (10, 5), (9, 7), (8, 8), (8, 9), (9, 10), (1, 10), (2, 9), (2, 8)], base)  # Leaves a wave-cut foot
    rock(img, x, y, w, h, [(6, 2), (8, 2), (10, 5), (9, 7), (8, 8), (8, 9), (9, 10), (6, 10), (5, 9), (6, 7)], shade)  # Gives the rock a shaded side
    rock(img, x, y, w, h, [(0, 6), (1, 3), (3, 1), (5, 0), (7, 1), (9, 3), (10, 5), (8, 4), (7, 5), (5, 4), (3, 5), (2, 5)], cap)  # Spreads trees over the top
    rock(img, x, y, w, h, [(1, 4), (3, 1), (5, 1), (6, 2), (4, 3), (2, 3)], mix(cap, mm.HILLS))  # Adds sunlit crowns at the left


def bangka(img, x, y, w, h, paddler=False):  # Paints a small boat in the distance
    rock(img, x, y, w, h, [(0, 4), (3, 6), (8, 5), (10, 3), (9, 7), (7, 9), (2, 9)], mm.VEST_SH)  # Bends both ends of the hull up
    rock(img, x, y, w, h, [(1, 4), (3, 5), (8, 4), (9, 4), (8, 6), (2, 7)], TUFF)  # Gives the gunwale a light strip
    rock(img, x, y, w, h, [(2, 9), (8, 8), (8, 9), (2, 10)], SEA_LIGHT)  # Places an outrigger beside the hull
    if paddler:  # Fits a person in the gorge boat
        rock(img, x, y, w, h, [(4, 2), (5, 1), (6, 1), (6, 3), (5, 4), (6, 6), (4, 6)], mix(mm.SCARF, mm.VEST))  # Adds a small boatman
        rock(img, x, y, w, h, [(5, 3), (6, 3), (8, 9), (7, 9)], mm.VEST)  # Angles the paddle down to the river


def island_sky():  # L0 shows the bright Philippine sea sky
    return sky([(0, lm.SKY), (100, mix(lm.SKY, mm.SKY_LIGHT)), (170, mm.SKY_LIGHT)])  # Gets lighter toward the sea


def island_clouds():  # L1 shows loose white cloud bands
    return clouds([(106, 49, 92), (347, 32, 142), (630, 60, 104), (811, 36, 133)], mix(mm.WHITE, mm.SKY_LIGHT), mm.SKY_LIGHT)  # Spaces the clouds widely


def island_far():  # L2 shows small islands on the far horizon
    img = empty()  # Starts the distant layer
    mm.fill_rect(img, 0, 147, W, H - 147, SEA)  # Draws the far sea plane
    for x, w, h in ((15, 62, 36), (103, 41, 23), (183, 66, 30), (290, 54, 32), (372, 70, 40), (502, 42, 21), (609, 80, 32), (756, 61, 39), (870, 66, 31)):  # Places little irregular groups on the horizon
        sea_stack(img, x, 153 - h, w, h, True)  # Paints one hazy island
    flecks(img, [(75, 158, 36), (263, 155, 64), (465, 159, 46), (717, 155, 63), (899, 166, 33)], SEA_LIGHT)  # Adds faint reflections
    return img  # Gives back the horizon


def island_mid():  # L3 shows nearer limestone islets
    img = empty()  # Starts the limestone layer
    for x, y, w, h in ((38, 117, 104, 64), (189, 132, 68, 48), (310, 94, 129, 89), (536, 143, 80, 49), (688, 106, 116, 74), (839, 130, 104, 57)):  # Stacks the islets at different distances
        sea_stack(img, x, y, w, h, False)  # Paints harder outlines in the middle distance
    return img  # Leaves water and sky showing between the islands


def island_boats():  # L4 shows bangkas along a band of sea
    img = empty()  # Starts the sea layer
    bank(img, [(0, 198), (86, 192), (222, 199), (370, 194), (490, 202), (650, 191), (801, 198), (959, 198)], SEA, SEA_SHADE)  # Gives reflected water a low edge
    for x, y, w, h in ((81, 189, 51, 11), (258, 197, 36, 8), (446, 191, 58, 13), (633, 198, 43, 10), (821, 193, 60, 12)):  # Places boats below the islands
        bangka(img, x, y, w, h)  # Paints a coastal bangka
    flecks(img, [(7, 207, 34), (142, 211, 35), (304, 206, 39), (534, 213, 55), (729, 207, 43), (897, 209, 47)], SEA)  # Adds quiet foreground reflections
    return img  # Gives back the water and boats
def island_near():  # L5 shows mangrove crowns and roots low by the trail
    img = empty()  # Starts the shoreline trim
    for x, y, r in ((34, 213, 12), (51, 211, 16), (73, 216, 12), (328, 212, 14), (352, 207, 16), (373, 213, 12), (676, 217, 16), (700, 213, 14), (922, 215, 10)):  # Groups the crowns with broad gaps
        rock(img, x - r, y, r * 2, 23, [(4, 0), (6, 0), (6, 5), (9, 10), (6, 10), (5, 6), (3, 10), (1, 10), (4, 5)], mm.VEST_SH)  # Fans roots down to the water
        nb.crown(img, x, y, r, GORGE_MOSS, ISLAND_NEAR)  # Places a low rounded crown
    return img  # Gives back sparse silhouettes


def cave_sky():  # L0 gives the cave its dim base and a soft upper glow
    return sky([(0, CAVE_DARK), (80, CAVE_GREY), (154, mix(CAVE_GREY, CAVE_LIT)), (225, CAVE_GREY)])  # Keeps the explorer's band quiet


def cave_mouth_light():  # L1 shows the mouth and rock frame with light from the left
    img = empty()  # Starts the light layer
    for x, y, w, h in ((41, 18, 194, 123), (530, 29, 167, 109)):  # Places two distant openings in the long repeat
        rock(img, x, y, w, h, [(0, 6), (1, 2), (4, 0), (8, 1), (10, 4), (8, 7), (6, 8), (4, 10), (1, 8)], CAVE_LIT)  # Draws the dark rock around the mouth
        rock(img, x, y, w, h, [(1, 5), (3, 2), (5, 1), (8, 2), (9, 4), (7, 6), (5, 6), (3, 8), (1, 7)], CAVE_GLOW)  # Lets in distant sky
        rock(img, x, y, w, h, [(3, 5), (5, 3), (8, 3), (8, 4), (5, 5), (4, 7), (2, 7)], mix(CAVE_GLOW, mm.HEADLAMP))  # Adds a pale opening inside the rim
        rock(img, x, y + 54, w * 2, h, [(2, 1), (3, 0), (8, 10), (6, 10)], CAVE_RAY)  # Spreads a soft shaft into the cavern
        rock(img, x, y + 54, w * 2, h, [(2, 1), (2, 0), (6, 10), (5, 10)], mix(CAVE_RAY, CAVE_GLOW))  # Keeps the lit shaft lighter than its edge
    return img  # Gives back the cave's light


def cave_roof():  # L2 hangs broken rows of stone down from the ceiling
    img = empty()  # Starts the far rock roof
    tops = profile([(0, 55), (121, 33), (219, 57), (306, 93), (426, 48), (554, 27), (655, 51), (789, 71), (884, 52), (959, 55)])  # Cuts out two large chambers
    pen = ImageDraw.Draw(img)  # Gives the roof flat shapes
    pen.polygon([(0, 0), (959, 0)] + list(reversed(list(enumerate(tops)))), fill=CAVE_GREY)  # Covers the top of the chamber
    for x, w, h in ((9, 24, 30), (33, 20, 42), (196, 28, 64), (224, 16, 29), (295, 29, 32), (370, 42, 42), (488, 19, 33), (664, 22, 40), (691, 31, 59), (815, 27, 41), (879, 36, 39)):  # Hangs clustered stalactites
        y = tops[x + w // 2] - 12  # Starts the stone inside the roof
        rock(img, x, y, w, h, [(0, 0), (10, 0), (8, 4), (6, 6), (4, 10), (3, 6), (2, 4)], CAVE_GREY)  # Continues the ceiling form
        rock(img, x, y, w, h, [(0, 1), (3, 2), (4, 4), (4, 9), (3, 7), (2, 4)], CAVE_LIT)  # Picks up light on the left face
    return img  # Gives back the stalactites


def cave_columns():  # L3 shows stone columns where points have joined
    img = empty()  # Starts the column layer
    for x, y, w, h in ((263, 1, 104, 228), (433, 1, 81, 231), (753, 1, 103, 232), (891, 1, 43, 235)):  # Leaves large openings between columns
        rock(img, x, y, w, h, [(0, 0), (10, 0), (8, 2), (7, 4), (8, 7), (10, 10), (0, 10), (2, 8), (3, 5), (2, 2)], CAVE_DARK)  # Gives the base and roof broad joins
        rock(img, x, y, w, h, [(1, 0), (3, 2), (4, 5), (3, 7), (1, 10), (5, 10), (5, 7), (5, 4), (4, 2), (3, 0)], CAVE_GREY)  # Lights a narrow face down the column
    return img  # Gives back the pillars


def cave_shelf():  # L4 shows wet stone rising low against the path
    img = empty()  # Starts the near wall
    bank(img, [(0, 214), (105, 200), (228, 211), (344, 189), (459, 213), (561, 199), (675, 218), (815, 192), (959, 214)], CAVE_LIT, CAVE_EARTH)  # Shapes a low stone shelf
    for x, y, w, h in ((23, 182, 95, 55), (237, 189, 99, 43), (342, 175, 126, 57), (559, 178, 88, 52), (751, 188, 136, 47)):  # Lays flowstone over the shelf in patches
        rock(img, x, y, w, h, [(0, 9), (1, 6), (2, 5), (3, 2), (5, 0), (7, 0), (9, 3), (9, 6), (10, 10), (0, 10)], CAVE_EARTH)  # Gives the flowstone its smooth draped shape
        rock(img, x, y, w, h, [(0, 9), (2, 6), (4, 6), (6, 5), (9, 5), (9, 6), (6, 6), (4, 8), (1, 9)], CAVE_LIT)  # Lights one thin mineral ribbon
    return img  # Gives back the dark wall


def cave_near():  # L5 shows large close stone points at the edges
    img = empty()  # Starts the closest cave layer
    for x, w, h in ((20, 44, 45), (174, 61, 63), (293, 40, 39), (465, 65, 58), (583, 40, 44), (723, 75, 69), (891, 43, 38)):  # Breaks up the near ceiling
        rock(img, x, 0, w, h, [(0, 0), (10, 0), (8, 3), (6, 4), (5, 7), (4, 10), (3, 5), (1, 3)], CAVE_DARK)  # Hangs a dark point
    for x, y, w, h in ((104, 187, 36, 61), (390, 201, 41, 47), (631, 191, 33, 57), (846, 204, 64, 42)):  # Adds a few floor points behind the path
        rock(img, x, y, w, h, [(0, 10), (3, 6), (4, 1), (5, 0), (6, 3), (7, 7), (10, 10)], CAVE_DARK)  # Draws a sharp lower shape
        rock(img, x, y, w, h, [(0, 10), (3, 6), (4, 1), (5, 0), (4, 7), (3, 10)], CAVE_LIT)  # Lights the near edge
    return img  # Gives back the near cave shapes


def gorge_sky():  # L0 shows the sky through the narrow gap
    return sky([(0, mix(lm.SKY, lm.HAZE)), (124, mm.SKY_LIGHT), (214, PADDY_LIGHT)])  # Blends blue sky into green river haze


def gorge_clouds():  # L1 gives the broken sky a few high wisps
    return clouds([(174, 36, 102), (475, 23, 117), (717, 44, 123)], mix(mm.SKY_LIGHT, mm.WHITE), mm.SKY_LIGHT)  # Lets the cliffs frame the wisps


def gorge_far():  # L2 shows distant walls and waterfalls between them
    img = empty()  # Starts the far cliff layer
    bank(img, [(0, 56), (95, 75), (157, 41), (234, 71), (292, 80), (347, 119), (455, 61), (544, 48), (667, 82), (735, 89), (783, 119), (834, 50), (959, 56)], GORGE_FAR, mix(GORGE_FAR, GORGE_MOSS))  # Cuts a narrow sky opening
    for x, y, w, h in ((244, 84, 74, 98), (703, 96, 66, 89)):  # Places two far drops along the repeat
        rock(img, x, y, w, h, [(2, 0), (7, 0), (6, 3), (7, 6), (10, 10), (0, 10), (2, 6), (4, 3)], mix(lm.LENS, lm.HAZE))  # Folds the waterfall down the cliff
        rock(img, x, y, w, h, [(2, 0), (4, 0), (5, 3), (3, 6), (2, 9), (0, 10), (2, 6), (4, 3)], mm.SKY_LIGHT)  # Lights the left edge of the falling water
    return img  # Gives back the far walls


def gorge_middle():  # L3 shows near mossy rock faces in broad sections
    img = empty()  # Starts the middle cliff layer
    cliff = [(0, 10), (0, 4), (1, 1), (3, 0), (4, 1), (7, 1), (8, 3), (8, 6), (10, 10)]  # Shapes a rounded cliff with a steep right side
    for x, y, w, h in ((-45, 37, 240, 170), (346, 73, 214, 143), (541, 42, 126, 165), (827, 53, 130, 155)):  # Leaves the waterfall visible between nearer walls
        rock(img, x, y, w, h, cliff, GORGE_FACE)  # Draws a big mossy face
        rock(img, x, y, w, h, [(1, 5), (2, 3), (4, 2), (6, 3), (7, 5), (6, 7), (7, 9), (4, 10), (2, 9)], GORGE_STONE)  # Lights a weathered face
        rock(img, x, y, w, h, [(0, 4), (1, 1), (3, 0), (4, 1), (7, 1), (8, 3), (6, 2), (4, 2), (2, 3)], GORGE_MOSS)  # Puts moss along the top edge
    return img  # Gives back the cliff shapes


def gorge_river():  # L4 shows the green river and its boatmen
    img = empty()  # Starts the river layer
    bank(img, [(0, 181), (102, 188), (211, 178), (337, 188), (447, 181), (560, 189), (704, 179), (858, 186), (959, 181)], mix(RIVER, GORGE_FAR), RIVER)  # Joins the water under the cliff
    flecks(img, [(15, 196, 45), (86, 209, 34), (195, 184, 38), (246, 188, 68), (353, 208, 45), (499, 199, 58), (610, 211, 31), (696, 189, 53), (780, 193, 43), (866, 208, 63)], RIVER_EDGE)  # Lets the river carry light around the bends
    for x, y, w in ((139, 191, 44), (408, 197, 56), (710, 190, 44), (839, 197, 58)):  # Spaces the boatmen below the walls
        bangka(img, x, y, w, 17, True)  # Sets a small bangka on the river
    return img  # Gives back the river


def leaves(img, x, y, spread, height, shade, tip):  # Draws a bending clump of leaves
    draw = ImageDraw.Draw(img)  # Uses a crisp pixel pen
    for side in (-1, 1):  # Fans leaves out both sides
        for reach, rise in ((spread, height // 2), (spread * 2 // 3, height * 3 // 4), (spread // 3, height)):  # Builds three fronds on each side
            end = x + side * reach  # Finds the far end of the blade
            draw.line([(x, y), (x + side * reach // 2, y - rise), (end, y - rise + 2)], fill=shade, width=2)  # Bends each frond outward
            draw.point((end, y - rise + 2), fill=tip)  # Lights its growing end


def gorge_near():  # L5 shows a few fern groups near the trail
    img = empty()  # Starts the closest greenery
    for x, y, w, h in ((54, 226, 23, 31), (94, 230, 15, 23), (343, 225, 22, 30), (387, 226, 32, 40), (610, 227, 31, 32), (744, 229, 18, 24), (918, 226, 22, 22)):  # Leaves clear gaps in the fronds
        leaves(img, x, y, w, h, GORGE_FACE, GORGE_MOSS)  # Grows one fern out of the bank
    return img  # Gives back the plants


def terrace_sky():  # L0 shows pale mountain air
    return sky([(0, mix(lm.SKY, lm.HAZE_SH)), (93, mm.SKY_LIGHT), (198, mix(mm.SKY_LIGHT, mm.MASK))])  # Softens the valley toward the horizon


def terrace_clouds():  # L1 shows wisps of mist high in the valley
    return clouds([(89, 69, 111), (351, 45, 137), (626, 64, 151), (835, 51, 127)], mix(mm.SKY_LIGHT, mm.MASK), lm.HAZE)  # Spreads thin clouds between ridges


def terrace_mountains():  # L2 shows layered mountain ridges
    img = empty()  # Starts the mountain layer
    bank(img, [(0, 116), (85, 65), (203, 112), (329, 49), (436, 102), (544, 67), (674, 111), (794, 53), (959, 116)], lm.HAZE, lm.HAZE_SH)  # Draws the valley's distant range
    bank(img, [(0, 136), (123, 111), (210, 139), (319, 91), (450, 144), (560, 102), (647, 124), (766, 81), (882, 117), (959, 136)], TERRACE_MID, TERRACE_FAR)  # Brings a greener range closer
    return img  # Gives back the silhouette


def paddy_slope(img, y, scale, colours):  # Draws a hillside of water shelves from high to low
    bank_light, bank_body, water, reflection = colours  # Reads the colour set for this distance
    for tier in range(4):  # Makes four broad terrace levels
        base = y + tier * scale  # Lowers each terrace
        rise = scale + tier * 2  # Opens up the foreground terraces
        knots = [(0, base), (70, base - rise), (175, base - rise * 2), (280, base - rise), (400, base + rise // 2), (528, base - rise), (640, base - rise * 2), (754, base - rise // 2), (869, base + rise // 3), (959, base)]  # Bends each step around the valley
        tops = bank(img, knots, mix(reflection, water), bank_body)  # Draws the soil below a soft water edge
        for x, top in enumerate(tops):  # Follows the curve of the bank
            paddy_end = top + max(3, scale // 3 + 1)  # Gives standing water a broad face
            nb.column(img, x, top + 1, water, paddy_end)  # Fills the paddy between banks
            nb.column(img, x, paddy_end, bank_light, paddy_end + 2)  # Draws a thin grassy dike
        for start, end in ((30, 98), (218, 258), (474, 530), (712, 760), (828, 868)):  # Breaks up reflections across the paddies
            for x in range(start + tier * 9, end + tier * 9):  # Follows a short stretch of water
                nb.put(img, x, tops[x] + 2, reflection)  # Lights only the open water


def terrace_steps():  # L3 shows the rice terraces climbing the hillside
    img = empty()  # Starts the far terraces
    paddy_slope(img, 148, 14, (TERRACE_MID, mix(TERRACE_BODY, TERRACE_FAR), mix(PADDY, TERRACE_MID), PADDY_LIGHT))  # Stacks the distant water steps
    return img  # Gives back the far paddies


def hillside_house(img, x, y, w, h):  # Draws a little raised Ifugao house
    rock(img, x, y, w, h, [(2, 5), (8, 5), (8, 8), (7, 8), (7, 10), (6, 10), (6, 8), (4, 8), (4, 10), (3, 10), (3, 8), (2, 8)], mm.GROUND_SH)  # Builds the floor and posts
    rock(img, x, y, w, h, [(0, 6), (2, 3), (4, 0), (6, 0), (8, 3), (10, 6)], mm.VEST)  # Gives the house a broad thatched roof
    rock(img, x, y, w, h, [(5, 0), (6, 0), (8, 3), (10, 6), (7, 6)], mm.VEST_SH)  # Shades the far roof
    rock(img, x, y, w, h, [(4, 6), (6, 6), (6, 8), (4, 8)], mm.DARK)  # Opens a small doorway


def terrace_village():  # L4 shows lower reflective paddies and houses
    img = empty()  # Starts the village layer
    paddy_slope(img, 191, 19, (mm.HILLS_SH, TERRACE_BODY, PADDY, PADDY_LIGHT))  # Grows broader fields below the village
    for x, y, w, h in ((116, 151, 34, 32), (331, 169, 43, 35), (611, 150, 29, 29), (814, 169, 38, 35)):  # Places houses on open slopes
        hillside_house(img, x, y, w, h)  # Draws one house
    return img  # Gives back the lower terraces


def terrace_rice():  # L5 shows small stands of rice at the foot of the hills
    img = empty()  # Starts the closest rice layer
    for x, y, w, h in ((57, 222, 11, 16), (78, 225, 12, 19), (154, 226, 12, 14), (361, 228, 14, 20), (389, 226, 13, 16), (630, 220, 17, 21), (652, 220, 13, 19), (898, 225, 19, 25)):  # Gathers the rice away from the skyline
        leaves(img, x, y, w, h, mm.LEAF, mm.HILLS)  # Adds one bright stand of rice
    return img  # Gives back the near rice


def chimney_sky():  # L0 shows the warm dawn over Cappadocia
    img = sky([(0, lm.HAZE_SH), (78, mix(DAWN, lm.HAZE)), (133, DAWN), (190, TUFF)])  # Brings warm light up to a muted blue
    nb.sun(img, 152, 113, 25, mm.HEADLAMP, TUFF, mix(TUFF, DAWN))  # Places a pale sunrise above the valley
    return img  # Gives back the painted sky


def little_balloon(img, x, y, w, h, cloth):  # Draws a quiet small balloon in the dawn
    rock(img, x, y, w, h, [(2, 1), (5, 0), (8, 1), (10, 3), (9, 5), (7, 7), (3, 7), (1, 5), (0, 3)], cloth)  # Shapes the envelope
    rock(img, x, y, w, h, [(3, 1), (5, 0), (4, 2), (4, 5), (5, 7), (3, 6), (2, 4), (2, 2)], mix(cloth, TUFF))  # Lights a left fabric panel
    rock(img, x, y, w, h, [(3, 7), (4, 7), (4, 9), (3, 9)], CAP)  # Hangs one line
    rock(img, x, y, w, h, [(6, 7), (7, 7), (7, 9), (6, 9)], CAP)  # Hangs the other line
    rock(img, x, y, w, h, [(3, 9), (7, 9), (6, 10), (4, 10)], CAP)  # Suspends the basket


def chimney_balloons():  # L1 shows several balloons at different distances
    img = empty()  # Starts the sky decoration
    for x, y, w, h, cloth in ((88, 54, 24, 34, mm.PLATEAU), (235, 33, 30, 44, mm.SCARF), (391, 68, 17, 25, mm.VEST_SH), (593, 31, 25, 39, mm.SCARF_SH), (733, 51, 21, 33, mm.PLATEAU), (893, 70, 16, 21, mm.VEST_SH)):  # Spaces the small balloons across the sky
        little_balloon(img, x, y, w, h, mix(cloth, DAWN))  # Softens the distant fabric
    return img  # Gives back the drifting balloons


def cone(img, x, y, w, h, lit, shade, cap=None):  # Shapes a tuff spire in the morning sun
    rock(img, x, y, w, h, [(0, 10), (1, 7), (3, 5), (4, 2), (4, 1), (6, 1), (7, 3), (7, 5), (9, 8), (10, 10)], lit)  # Erodes the body of the cone
    rock(img, x, y, w, h, [(5, 1), (6, 1), (7, 3), (7, 5), (9, 8), (10, 10), (6, 10), (6, 7), (5, 4)], shade)  # Shades the face away from dawn
    if cap is not None:  # Gives the capped cones a hard top
        rock(img, x, y, w, h, [(2, 2), (3, 1), (4, 0), (6, 0), (8, 1), (9, 2), (7, 3), (3, 3)], cap)  # Tops the soft stone with a dark cap


def chimney_far():  # L2 shows pale tuff hills and cones far away
    img = empty()  # Starts the distant valley
    bank(img, [(0, 156), (93, 124), (188, 144), (300, 109), (412, 139), (546, 124), (650, 146), (782, 113), (884, 140), (959, 156)], mix(TUFF_FAR, TUFF), TUFF_FAR)  # Lays rolling tuff across the horizon
    for x, y, w, h in ((25, 122, 64, 53), (117, 120, 63, 58), (199, 133, 54, 49), (298, 112, 68, 67), (414, 137, 54, 57), (527, 104, 73, 89), (675, 127, 69, 60), (804, 116, 87, 68)):  # Breaks up the far hill line
        cone(img, x, y, w, h, TUFF_FAR, mix(TUFF_FAR, mm.VEST_SH))  # Gives each distant cone two broad tones
    return img  # Gives back the hazy cones


def chimney_middle():  # L3 shows groups of capped fairy chimneys
    img = empty()  # Starts the middle-distance land
    bank(img, [(0, 194), (120, 180), (254, 189), (407, 184), (542, 199), (673, 182), (823, 180), (959, 194)], TUFF, TUFF_SHADE)  # Draws the warm valley below the cones
    for x, y, w, h in ((33, 131, 61, 80), (128, 151, 60, 61), (229, 108, 92, 101), (338, 137, 65, 73), (461, 117, 57, 101), (549, 150, 46, 62), (659, 124, 84, 87), (783, 141, 63, 74), (860, 115, 70, 101)):  # Places capped fairy chimneys in loose groups
        cone(img, x, y, w, h, TUFF, TUFF_SHADE, CAP)  # Sets a hard cap over pale tuff
    return img  # Gives back the middle chimneys


def chimney_houses():  # L4 shows doors and windows carved into closer cones
    img = empty()  # Starts the closer cliff layer
    for x, y, w, h in ((76, 163, 83, 71), (345, 156, 115, 84), (635, 169, 58, 60), (841, 155, 105, 85)):  # Leaves views of the spires between the houses
        cone(img, x, y, w, h, mix(TUFF, mm.HEADLAMP), mm.VEST)  # Raises a pale house cone
        rock(img, x, y, w, h, [(3, 10), (3, 8), (4, 7), (5, 7), (6, 8), (6, 10)], CAP)  # Cuts a doorway into the rock
        rock(img, x, y, w, h, [(4, 5), (4, 4), (5, 4), (5, 5)], CAP)  # Cuts a small square window
    return img  # Gives back the cave houses


def chimney_near():  # L5 shows the low dry valley floor
    img = empty()  # Starts the closest tuff
    tops = bank(img, [(0, 215), (134, 207), (256, 219), (374, 212), (502, 219), (619, 209), (746, 218), (858, 209), (959, 215)], TUFF, mix(TUFF, TUFF_SHADE))  # Draws a low rolling bank
    for x, w in ((39, 12), (201, 13), (309, 16), (491, 16), (604, 11), (726, 13), (883, 17)):  # Tucks small shrubs along the bank
        leaves(img, x, tops[x] + 1, w, 7, CAP, mm.VEST)  # Draws a dry twig cluster
    return img  # Gives back the low bank


THEMES = {  # One entry per level theme in L0 to L5 order
    "IslandHills": ((island_sky, island_clouds, island_far, island_mid, island_boats, island_near), "island"),  # Coastal limestone hills
    "LimestoneCave": ((cave_sky, cave_mouth_light, cave_roof, cave_columns, cave_shelf, cave_near), "cave"),  # Dim cave with roof light
    "RiverGorge": ((gorge_sky, gorge_clouds, gorge_far, gorge_middle, gorge_river, gorge_near), "gorge"),  # Tall green cliffs and boats
    "RiceTerraces": ((terrace_sky, terrace_clouds, terrace_mountains, terrace_steps, terrace_village, terrace_rice), "terrace"),  # Winding farm steps
    "FairyChimneys": ((chimney_sky, chimney_balloons, chimney_far, chimney_middle, chimney_houses, chimney_near), "chimney"),  # Warm tuff formations
}  # Ends the theme table


def preview(shots):  # Shows a 480 by 270 view and all six separate layers
    sheet = Image.new("RGBA", (1094, 298 * len(shots)), mm.OUTLINE + (255,))  # Leaves room for labels and gaps
    pen = ImageDraw.Draw(sheet)  # Writes preview labels
    for i, (theme, stacked, layers) in enumerate(shots):  # Places one theme per row
        y = i * 298 + 8  # Finds this theme's row
        pen.text((8, y), theme, fill=mm.HEADLAMP)  # Names the theme beside its layers
        sheet.alpha_composite(stacked, (8, y + 16))  # Shows the game-sized composition
        for j, img in enumerate(layers):  # Shows each full-width layer alone
            x, top = 502 + (j % 2) * 294, y + (j // 2) * 94  # Places six thumbnails in two columns
            mm.fill_rect(sheet, x, top + 13, 288, 81, CAVE_GREY)  # Shows clear pixels against an even backing
            sheet.alpha_composite(img.resize((288, 81), Image.Resampling.NEAREST), (x, top + 13))  # Keeps the whole layer visible
            pen.text((x, top), f"L{j}", fill=mm.HEADLAMP)  # Names this layer
    sheet.save(PREVIEW)  # Writes the review sheet


def main():  # Writes the five background sets
    shots = []  # Keeps the ground and layers for inspection
    for theme, (makers, ground) in THEMES.items():  # Makes one set at a time
        folder = Path(nb.BACKDROPS) / theme  # Finds the named Unity art folder
        folder.mkdir(parents=True, exist_ok=True)  # Creates just the new theme folder
        layers = [make() for make in makers]  # Draws all six layers from far to near
        for i, img in enumerate(layers):  # Saves each layer at its exact size
            img.paste(img.crop((0, 0, 1, H)), (W - 1, 0))  # Matches both edge columns while the broad curves flatten at the seam
            img.save(folder / f"L{i}.png")  # Keeps the builder's layer names
        shots.append((theme, nb.preview_screen(layers, ground), layers))  # Shows the theme over its walking tiles
    preview(shots)  # Saves the stacked views and layer thumbnails
    print(PREVIEW)  # Prints the preview path


if __name__ == "__main__":  # Runs only when started as a script
    main()  # Writes the backdrop art
