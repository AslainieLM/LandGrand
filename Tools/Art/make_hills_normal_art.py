"""Draw the Hills Normal tiles, objects, props, kinds and lessons."""  # Describes the output
from pathlib import Path  # Gives file paths for each sprite
from PIL import Image, ImageDraw  # Draws pixels and labels the preview
import make_mainmenu_art as mm  # Gives the game palette and rectangles
import make_landform_map_art as lm  # Gives sea colours and repeatable noise
import make_level_art as la  # Gives outlines and single pixels
import make_lesson_art as ls  # Gives discs, polygons and output paths
import make_normal_art as na  # Gives shared tiles and rocks

mix = na.mix  # Mixes two palette colours
LEVEL = Path(la.OUT)  # Holds the level sprites
PREVIEW = Path(mm.ROOT) / ".scratch" / "hills-normal" / "art-preview.png"  # Holds the review sheet
LIME = mix(mm.MASK, mm.HEADLAMP)  # Sunlit island limestone
LIME_SHADE = mix(mm.MASK_SH, mm.VEST)  # Shaded limestone
CAVE = mix(lm.MOUNTAIN_SH, mm.GROUND_SH)  # Damp cave rock
CAVE_LIGHT = mix(lm.HAZE, mm.VEST)  # Lit cave stone
MOSS_ROCK = mix(mm.LEAF, mm.MASK_SH)  # Green-grey gorge walls
TERRACE_MUD = mix(mm.PLATEAU_SH, mm.DARK)  # Dark wet terrace soil
TUFF = mix(mm.PLATEAU, mm.HEADLAMP)  # Pale Cappadocia tuff
TUFF_LIGHT = mix(mm.HEADLAMP, mm.VEST)  # Sunny tuff edge
WATER = lm.LENS  # Sunlit shallow water
WATER_MID = mix(lm.LENS, lm.MOUNTAIN_SH)  # Water away from the light
WATER_DARK = mix(lm.MOUNTAIN_SH, mm.LEAF)  # Green-blue water in shade
FOAM = mix(mm.SKY_LIGHT, mm.WHITE)  # Pale foam
WOOD = mm.VEST  # Light wood and bamboo
WOOD_SHADE = mm.GROUND_SH  # Wood away from the light
THATCH = mix(mm.VEST, mm.HELMET_SH)  # Woven roof
BROWN = mix(mm.GROUND_SH, mm.OUTLINE)  # Warm dark bat fur
DAWN = mix(mm.PLATEAU, mm.SKY_LIGHT)  # Pink dawn at the horizon

THEMES = {  # Each row has floor, grass, light, shade, dark speck and bright stone
    "island": (LIME_SHADE, mm.HILLS_SH, mm.PLAINS, mm.LEAF, mm.MASK_SH, LIME),  # Grass over limestone
    "cave": (CAVE, CAVE_LIGHT, mm.HEADLAMP, mm.MASK_SH, mm.STRAP, mm.MASK),  # Bright damp rim over dark rock
    "gorge": (MOSS_ROCK, mm.LEAF, mm.HILLS, mm.HILLS_SH, mm.STRAP, mm.MASK_SH),  # Moss over grey rock
    "terrace": (TERRACE_MUD, mm.HILLS, mm.PLAINS, mm.LEAF, mm.DARK, mm.PLATEAU),  # Grass over wet clay
    "chimney": (mm.PLATEAU, TUFF, TUFF_LIGHT, mm.VEST, mm.PLATEAU_SH, mm.HEADLAMP),  # Pale tuff over warm soil
}  # Ends the theme colours


def set_tiles(theme):  # Draws each theme's five ground pieces
    base, cap, light, shade, dark, stone = THEMES[theme]  # Reads the palette
    flecks = [(2, 1, light), (5, 3, shade), (9, 2, shade), (13, 1, light)]  # Breaks up the top edge
    yield f"{theme}_top", na.ground_top(base, cap, light, shade, flecks)  # Frames the walking line
    yield f"{theme}_soil", na.soil_tile(base, dark, [(3, 5, shade), (4, 5, shade), (11, 11, dark), (12, 11, stone)])  # Places small chips
    yield f"ledge_{theme}", na.ledge_tile(base, cap, dark, [(4, 4, shade), (10, 5, dark), (13, 3, stone)])  # Gives the ledge a clear lip
    for label, w, h in (("small", 32, 24), ("big", 48, 36)):  # Makes both boulder sizes
        yield f"boulder_{label}_{theme}", na.capped_boulder(w, h, base, dark, stone, shade, cap, 2)  # Puts a light edge above the rock


def water_tile(height, current=False):  # Draws one tile of sea or moving water
    img = mm.new_image(16, height)  # Starts a clear tile
    mm.fill_rect(img, 0, 0, 16, height, WATER_MID if current else WATER_DARK)  # Lays down the deep colour
    mm.fill_rect(img, 0, 0, 16, 1, WATER if current else WATER_MID)  # Draws a thin water line
    for x, y, w in ((2, 2, 5), (10, 4, 4), (4, 6, 5), (10, 10, 4), (3, 13, 5)):  # Offsets the little ripples
        if y < height:  # Keeps the strokes in the tile
            mm.fill_rect(img, x, y, w, 1, WATER if current else WATER_MID)  # Adds one broken streak
    return img  # Gives back the tile


def raft():  # Draws the platform at the water line
    img = mm.new_image(48, 10)  # Fits the top's width and depth
    ls.fill_poly(img, [(0, 2), (4, 0), (43, 0), (47, 2), (44, 8), (3, 8)], WOOD_SHADE)  # Cuts a low bamboo hull
    mm.fill_rect(img, 3, 0, 42, 2, mm.HEADLAMP)  # Lights the two-pixel standing line
    mm.fill_rect(img, 2, 2, 44, 2, WOOD)  # Shows the edge of the bamboo
    mm.fill_rect(img, 4, 6, 40, 1, mm.GROUND)  # Shows another row of bamboo
    for x in (8, 24, 39):  # Spaces the dark rope bindings
        mm.fill_rect(img, x, 1, 2, 7, mm.DARK)  # Wraps each binding
        la.put(img, x, 1, mm.VEST_SH)  # Lights one rope corner
    return la.outline(img)  # Makes the raft readable against water


def ripple(frame):  # Draws the warning ripples
    img = mm.new_image(16, 4)  # Fits the surface
    mm.fill_rect(img, frame * 2, 1, 9, 1, FOAM)  # Shifts the bright front line
    mm.fill_rect(img, 6 - frame * 4, 3, 8 + frame * 2, 1, WATER)  # Shifts the smaller line
    return img  # Gives back the ripple


def current_mark(frame):  # Draws two chevrons pointing with the water
    img = mm.new_image(12, 6)  # Fits over the wet tile
    for x in (1 + frame, 6 + frame):  # Gives the marks a small forward shift
        ls.fill_poly(img, [(x, 1), (x + 2, 1), (x + 4, 3), (x + 2, 5), (x, 5), (x + 2, 3)], FOAM if x < 5 else WATER)  # Makes one arrow
    return img  # Gives back the chevrons


def bat(frame):  # Draws a friendly bat with three wing positions
    img = mm.new_image(16, 10)  # Fits the bat art
    tip = (1, 4, 7)[frame]  # Moves the wings up and down
    for mid, side in ((6, -1), (9, 1)):  # Mirrors the wing pattern
        wing = [(mid, 5), (mid + side * 5, tip), (mid + side * 5, tip + 2), (mid + side * 3, 7), (mid + side, 6)]  # Makes a scalloped wing
        ls.fill_poly(img, wing, BROWN)  # Fills the wing
        la.put(img, mid + side * 3, (tip + 5) // 2, mm.PLATEAU_SH)  # Lights the wing joint
    ls.fill_poly(img, [(5, 3), (5, 1), (7, 3), (8, 3), (10, 1), (10, 7), (9, 8), (6, 8), (5, 7)], mm.GROUND_SH)  # Gives the bat a small round face
    la.put(img, 6, 4, mm.HEADLAMP)  # Lights the first eye
    la.put(img, 9, 4, mm.HEADLAMP)  # Lights the second eye
    mm.fill_rect(img, 7, 6, 2, 1, mm.VEST)  # Gives it a short smiling mouth
    return la.outline(img)  # Binds the silhouette together


def balloon():  # Draws the envelope, ropes and the full-width basket top
    img = mm.new_image(48, 80)  # Fits the moving balloon sprite
    ls.fill_poly(img, [(7, 5), (15, 1), (31, 1), (40, 6), (45, 17), (45, 34), (38, 47), (29, 58), (18, 58), (8, 47), (2, 35), (2, 18)], mm.SCARF_SH)  # Cuts the envelope silhouette
    ls.fill_poly(img, [(7, 6), (16, 2), (29, 2), (35, 9), (38, 21), (37, 36), (30, 49), (25, 57), (17, 54), (8, 44), (3, 32), (3, 18)], mm.SCARF)  # Lights the canopy from above-left
    ls.fill_poly(img, [(16, 3), (22, 2), (16, 11), (13, 23), (14, 36), (21, 56), (17, 54), (10, 40), (8, 25), (10, 13)], mm.HELMET)  # Stretches one amber panel
    ls.fill_poly(img, [(26, 2), (32, 3), (35, 17), (35, 31), (28, 50), (25, 57), (24, 56), (30, 36), (32, 22)], mm.PLATEAU)  # Shades the right panel
    ls.fill_poly(img, [(11, 7), (17, 4), (14, 8), (10, 16), (7, 17)], mm.HEADLAMP)  # Adds the warm upper-left shine
    for a, b in ((18, 5), (21, 14), (27, 33), (29, 42)):  # Ties all four ropes to the basket
        for y in range(57, 71):  # Draws each rope row
            la.put(img, round(a + (b - a) * (y - 57) / 13), y, mm.DARK)  # Joins the curve to the basket
    mm.fill_rect(img, 0, 70, 48, 2, mm.HEADLAMP)  # Lights the full basket standing line
    mm.fill_rect(img, 1, 72, 46, 3, WOOD)  # Colours the top weave
    mm.fill_rect(img, 3, 75, 42, 4, WOOD_SHADE)  # Shades the bottom weave
    for x in range(4, 45, 5):  # Spaces the wicker strips
        mm.fill_rect(img, x, 74, 2, 4, mm.GROUND)  # Draws one wicker strip
    outlined = la.outline(img)  # Outlines the soft envelope and stiff basket
    mm.fill_rect(outlined, 0, 70, 48, 1, mm.HEADLAMP)  # Keeps the solid line at row seventy
    return outlined  # Gives back the balloon


def flame(frame):  # Draws a two-frame burner flame
    img = mm.new_image(8, 8)  # Fits between the ropes
    tip = 1 - frame  # Makes one flame slightly taller
    ls.fill_poly(img, [(3, tip), (6, 4), (5, 7), (2, 7), (1, 5)], mm.SCARF)  # Draws the hot outer cone
    ls.fill_poly(img, [(3, tip + 2), (4, 4), (4, 7), (2, 6)], mm.HEADLAMP)  # Adds a bright center
    return img  # Gives back the flame


def bangka(with_boatman=False):  # Draws a Philippine outrigger boat
    img = mm.new_image(40, 20 if with_boatman else 14)  # Gives the boatman head room
    y = img.height - 9  # Finds the gunwale
    ls.fill_poly(img, [(1, y), (9, y + 2), (30, y + 2), (38, y - 1), (33, y + 6), (10, y + 7), (5, y + 5)], mm.PLATEAU_SH)  # Carves a curved hull
    mm.fill_rect(img, 6, y + 2, 28, 2, mm.HEADLAMP)  # Lights the gunwale
    mm.fill_rect(img, 7, y + 8, 26, 1, mm.VEST)  # Puts an outrigger beside the boat
    for x in (10, 28):  # Pairs the supports
        mm.fill_rect(img, x, y + 4, 2, 4, WOOD_SHADE)  # Joins the hull to the outrigger
    if with_boatman:  # Adds a person and paddle
        ls.disc(img, 19, 5, 2, mm.PLATEAU)  # Draws the face
        mm.fill_rect(img, 15, 3, 8, 2, THATCH)  # Gives the boatman a sun hat
        ls.fill_poly(img, [(17, 8), (22, 8), (24, y + 2), (17, y + 2)], mm.SCARF)  # Draws the boatman's shirt
        ImageDraw.Draw(img).line([(24, 8), (32, 17)], fill=WOOD, width=2)  # Places the paddle across the side
    return la.outline(img)  # Gives the boat a dark edge


def dock():  # Draws the boat dock goal
    img = mm.new_image(56, 48)  # Fits the goal marker size
    for x in (6, 18, 37, 49):  # Places roof supports and pier legs
        mm.fill_rect(img, x, 17, 3, 29, WOOD_SHADE)  # Draws the post
        mm.fill_rect(img, x, 17, 1, 29, WOOD)  # Lights the left edge
    ls.fill_poly(img, [(3, 19), (16, 8), (42, 8), (53, 19)], THATCH)  # Covers the dock with a roof
    mm.fill_rect(img, 7, 27, 43, 1, WOOD)  # Joins the posts with a rail
    mm.fill_rect(img, 2, 34, 52, 4, mm.GROUND)  # Draws the boardwalk
    mm.fill_rect(img, 2, 34, 52, 1, mm.HEADLAMP)  # Lights the top of the boards
    mm.paste(img, bangka().crop((0, 3, 40, 14)), 6, 37)  # Parks a bangka below the dock
    return la.outline(img)  # Frames the dock


def cave_mouth():  # Draws the cave goal as a bright opening framed by limestone
    img = mm.new_image(56, 48)  # Fits the goal
    ls.fill_poly(img, [(1, 46), (5, 20), (13, 8), (25, 2), (40, 6), (49, 16), (55, 46)], CAVE)  # Draws the cave wall
    ls.fill_poly(img, [(8, 47), (13, 22), (20, 12), (30, 8), (42, 17), (47, 47)], CAVE_LIGHT)  # Lights the stone rim
    ls.fill_poly(img, [(16, 47), (20, 28), (26, 17), (36, 18), (42, 26), (44, 47)], WATER)  # Lets blue sky show through
    ls.fill_poly(img, [(20, 47), (27, 29), (33, 27), (40, 39), (41, 47)], FOAM)  # Lights the path at the mouth
    ls.fill_poly(img, [(26, 17), (28, 23), (31, 16), (37, 25), (39, 19)], CAVE)  # Adds an uneven dripstone edge
    return la.outline(img)  # Outlines the rock mouth


def falls_landing():  # Draws a stone bank and a boat at the falls
    img = mm.new_image(56, 48)  # Fits the goal
    ls.fill_poly(img, [(3, 39), (5, 17), (13, 8), (26, 6), (43, 9), (52, 22), (54, 39)], MOSS_ROCK)  # Stacks the falls rock
    ls.fill_poly(img, [(17, 9), (39, 9), (35, 22), (40, 36), (14, 36), (21, 22)], WATER)  # Drops water into the pool
    ls.fill_poly(img, [(21, 9), (27, 9), (25, 25), (20, 34), (16, 34)], FOAM)  # Lights the left side of the waterfall
    mm.fill_rect(img, 2, 38, 52, 8, WATER_MID)  # Draws the pool
    mm.paste(img, bangka(), 8, 33)  # Shows the landing boat
    return la.outline(img)  # Outlines the goal


def ifugao_house():  # Draws a raised Ifugao house
    img = mm.new_image(56, 56)  # Fits the taller goal
    for x in (11, 40):  # Draws its two visible legs
        mm.fill_rect(img, x, 37, 4, 18, WOOD_SHADE)  # Holds the house above the soil
        mm.fill_rect(img, x - 2, 44, 8, 2, WOOD)  # Adds a wooden rat guard
    mm.fill_rect(img, 10, 29, 35, 13, mm.GROUND)  # Draws the raised living floor
    mm.fill_rect(img, 22, 31, 11, 11, mm.DARK)  # Leaves an open doorway
    ls.fill_poly(img, [(1, 33), (8, 23), (23, 4), (32, 4), (47, 23), (54, 33)], THATCH)  # Builds a wide steep roof
    ls.fill_poly(img, [(24, 4), (32, 4), (53, 32), (33, 32)], mm.VEST_SH)  # Shades its far roof face
    ImageDraw.Draw(img).line([(3, 31), (24, 5)], fill=mm.HEADLAMP, width=2)  # Lights the near roof edge
    for y in range(43, 55, 3):  # Spaces the ladder steps
        mm.fill_rect(img, 22, y, 9, 1, WOOD)  # Draws a ladder rung
    mm.fill_rect(img, 22, 41, 1, 15, WOOD_SHADE)  # Draws the first ladder rail
    mm.fill_rect(img, 30, 41, 1, 15, WOOD_SHADE)  # Draws the second ladder rail
    return la.outline(img)  # Outlines the raised house


def balloon_field():  # Draws the balloon field goal with a mast and windsock
    img = mm.new_image(56, 48)  # Fits the goal
    mm.fill_rect(img, 11, 10, 3, 37, WOOD_SHADE)  # Sets the windsock mast
    ls.fill_poly(img, [(13, 10), (29, 12), (28, 16), (13, 18)], mm.SCARF)  # Flies the windsock
    mm.fill_rect(img, 20, 12, 3, 5, FOAM)  # Bands the sock
    mm.paste(img, balloon().resize((21, 35), Image.Resampling.NEAREST), 29, 2)  # Parks a balloon in the field
    mm.fill_rect(img, 28, 36, 24, 2, WOOD_SHADE)  # Holds the parked basket
    ls.fill_poly(img, [(27, 45), (52, 45), (55, 47), (24, 47)], TUFF)  # Shows the launch pad
    return la.outline(img)  # Outlines the field marker


def seabird(frame):  # Draws a small white sea bird
    img = mm.new_image(12, 8)  # Leaves room for both wing heights
    tip = 1 if frame == 0 else 6  # Moves the wing tips
    ls.fill_poly(img, [(1, tip), (5, 3), (7, 3), (10, tip), (9, 4), (7, 5), (5, 5), (3, 4)], FOAM)  # Places both wings and the body
    la.put(img, 7, 4, mm.VEST_SH)  # Adds the beak
    return la.outline(img)  # Keeps the bird clear against sky


def mangrove():  # Draws a low mangrove with roots
    img = mm.new_image(24, 20)  # Fits the shore prop
    for x in (5, 10, 15, 20):  # Spreads the roots
        ImageDraw.Draw(img).line([(12, 9), (x, 18)], fill=WOOD_SHADE, width=2)  # Drops one root
    for x, y, r in ((5, 8, 4), (10, 5, 5), (16, 5, 4), (20, 8, 3)):  # Places low leaf clusters
        ls.disc(img, x, y, r, mm.LEAF)  # Fills the canopy
        ls.disc(img, x - 1, y - 1, r - 2, mm.HILLS_SH)  # Lights the top left
    return la.outline(img)  # Defines the tree edge


def stalagmite():  # Draws a pale point of rock growing up from the cave floor
    img = mm.new_image(12, 20)  # Fits the cave prop
    ls.fill_poly(img, [(1, 18), (3, 13), (5, 9), (6, 1), (8, 7), (9, 15), (11, 18)], CAVE_LIGHT)  # Shapes the point
    ls.fill_poly(img, [(6, 3), (8, 7), (9, 15), (11, 18), (7, 18)], CAVE)  # Shades the right face
    return la.outline(img)  # Separates the point from the background


def bat_sign():  # Draws a bat and hush icon on a small sign
    img = mm.new_image(20, 20)  # Fits the sign by the start
    mm.fill_rect(img, 8, 12, 3, 7, WOOD_SHADE)  # Sets up the post
    mm.fill_rect(img, 1, 1, 18, 13, mm.HEADLAMP)  # Leaves a pale face for the icon
    mm.paste(img, bat(1).crop((0, 1, 16, 9)).resize((10, 5), Image.Resampling.NEAREST), 2, 4)  # Puts the bat on the left
    ls.disc(img, 15, 6, 3, mm.PLATEAU)  # Shows a small face beside the bat
    la.put(img, 16, 5, mm.DARK)  # Marks the closed eye
    mm.fill_rect(img, 15, 7, 3, 1, mm.SCARF_SH)  # Draws the quiet mouth
    mm.fill_rect(img, 15, 7, 1, 4, mm.DARK)  # Puts a finger in front of the mouth
    return la.outline(img)  # Frames the sign


def chapel_glow(frame):  # Draws the small chapel glow
    img = mm.new_image(24, 24)  # Fits the cave decoration
    ls.fill_poly(img, [(2, 14), (12, 5), (21, 14), (21, 22), (2, 22)], CAVE)  # Makes a small chapel shape
    ls.fill_poly(img, [(5, 14), (12, 8), (18, 14), (18, 21), (5, 21)], WOOD_SHADE)  # Lights the chapel face
    mm.fill_rect(img, 11, 3, 2, 5, CAVE_LIGHT)  # Sets a tiny cross above the roof
    mm.fill_rect(img, 10, 4, 4, 1, CAVE_LIGHT)  # Draws the crossbar
    mm.fill_rect(img, 10, 15, 4, 7, mm.HELMET if frame else mm.HEADLAMP)  # Flickers the light at the door
    return la.outline(img)  # Defines the chapel shape


def falls_mist(frame):  # Draws spray above the landing
    img = mm.new_image(24, 16)  # Fits the spray
    for x, y, r in ((4, 11, 3), (9, 9, 4), (14, 11, 4), (20, 12, 2)):  # Breaks it into puffs
        ls.disc(img, x, y - frame, r, WATER)  # Puts a blue base under each puff
        ls.disc(img, x - 1, y - frame - 1, r - 2, FOAM)  # Lights the top
    la.put(img, 11 + frame * 3, 3 - frame, FOAM)  # Adds one droplet above the spray
    return img  # Leaves the mist unoutlined


def rice_stalk(frame):  # Draws rice bending by the water
    img = mm.new_image(10, 12)  # Fits between terrace stones
    for x, tip in ((2, 3), (5, 1), (8, 4)):  # Staggers the height of the stalks
        sway = min(9, x + frame)  # Shifts the tips while keeping them in the tile
        ImageDraw.Draw(img).line([(5, 11), (x, 7), (sway, tip)], fill=mm.LEAF)  # Draws the stem
        mm.fill_rect(img, sway, tip, 1, 3, mm.PLAINS)  # Keeps each ripe seed head visible
    return img  # Gives back the stalk


def bulul():  # Draws an Ifugao carved rice figure
    img = mm.new_image(12, 20)  # Fits the wooden figure
    ls.disc(img, 6, 4, 3, WOOD)  # Carves the head
    mm.fill_rect(img, 4, 7, 5, 6, mm.GROUND)  # Carves the straight back
    ls.fill_poly(img, [(4, 8), (2, 12), (2, 17), (8, 17), (10, 14), (8, 11), (7, 15), (4, 14)], WOOD_SHADE)  # Shapes bent arms and knees
    mm.fill_rect(img, 2, 17, 9, 2, mm.DARK)  # Sets the figure on its base
    return la.outline(img)  # Defines the carved silhouette


def farmer(frame):  # Draws a farmer moving a hand over the rice
    img = mm.new_image(16, 20)  # Leaves room for the hat and step
    ls.disc(img, 7, 6, 2, mm.PLATEAU)  # Draws the face
    ls.fill_poly(img, [(2, 5), (5, 2), (9, 2), (13, 5)], THATCH)  # Adds the wide hat
    mm.fill_rect(img, 5, 8, 5, 6, mm.SCARF_SH)  # Draws a red shirt
    ImageDraw.Draw(img).line([(8, 9), (12 + frame, 12), (12 + frame, 14)], fill=mm.PLATEAU, width=2)  # Moves the farmer's hand
    mm.fill_rect(img, 4 + frame, 14, 2, 5, CAVE)  # Places the near foot
    mm.fill_rect(img, 8 - frame, 14, 2, 5, mm.STRAP)  # Places the far foot
    return la.outline(img)  # Defines the small person


def cone_house():  # Draws a house cut into a tuff cone
    img = mm.new_image(32, 48)  # Fits the tall stone prop
    ls.fill_poly(img, [(1, 47), (6, 30), (10, 20), (12, 7), (15, 2), (18, 4), (22, 17), (26, 33), (31, 47)], TUFF)  # Cuts the cone silhouette
    ls.fill_poly(img, [(15, 3), (20, 10), (25, 32), (30, 47), (22, 47), (18, 23)], mm.PLATEAU)  # Shades the far face
    ls.fill_poly(img, [(10, 21), (13, 7), (15, 4), (14, 17), (12, 30), (7, 42), (4, 47)], TUFF_LIGHT)  # Lights the left slope
    ls.fill_poly(img, [(11, 47), (11, 38), (14, 34), (18, 34), (20, 38), (20, 47)], mm.DARK)  # Cuts a doorway
    mm.fill_rect(img, 14, 23, 4, 5, WOOD_SHADE)  # Cuts a small window
    return la.outline(img)  # Defines the house outline


def dry_shrub():  # Draws short dry twigs
    img = mm.new_image(14, 8)  # Fits above the tuff tile
    for x in (1, 4, 6, 9, 12):  # Fans out five twigs
        ImageDraw.Draw(img).line([(6, 7), (x, 3), (x, 1 + x % 3)], fill=WOOD_SHADE)  # Draws one bent twig
        la.put(img, x, 1 + x % 3, THATCH)  # Lights the tip
    return img  # Keeps the slender silhouette


def far_balloon(frame):  # Draws a tiny balloon for the distant sky
    img = mm.new_image(12, 16)  # Leaves space for a one-pixel drift
    small = balloon().resize((10, 15), Image.Resampling.NEAREST)  # Keeps the large balloon's shape
    mm.paste(img, small, 1, frame)  # Shifts the sprite up or down
    return img  # Gives back the distant balloon


def aragonite():  # Draws a spray of pale needle-shaped crystals
    img = mm.new_image(24, 24)  # Fits the item card
    for x, y in ((4, 14), (5, 7), (10, 3), (15, 4), (19, 8), (21, 14)):  # Fans the crystals upward
        ls.fill_poly(img, [(10, 20), (x - 1, y + 3), (x, y), (x + 2, y + 2), (14, 20)], LIME_SHADE)  # Draws each crystal body
        ImageDraw.Draw(img).line([(x, y + 1), (11, 19)], fill=mm.HEADLAMP)  # Lights its near edge
    mm.fill_rect(img, 5, 19, 14, 2, WOOD)  # Gives the crystals a small stone base
    return ls.outline(img)  # Frames the whole specimen


def coquina():  # Draws broken shells packed in a rock
    img = na.blob(24, 24, 11.5, 13, 9.5, 7, lambda x, y, shade: LIME_SHADE if shade else WOOD, 2.2)  # Makes a rough warm lump
    for x, y in ((6, 10), (11, 8), (15, 11), (7, 16), (12, 15), (17, 16)):  # Exposes broken shell pieces
        ls.fill_poly(img, [(x - 2, y + 1), (x, y - 1), (x + 2, y), (x + 1, y + 2)], mm.HEADLAMP)  # Draws a shell chip
        la.put(img, x, y + 1, LIME_SHADE)  # Adds its curved inner edge
    return ls.outline(img)  # Defines the shell rock


def flowstone():  # Draws a pale sheet of banded stone
    img = mm.new_image(24, 24)  # Fits the item card
    ls.fill_poly(img, [(3, 8), (7, 5), (15, 4), (20, 7), (19, 12), (22, 17), (16, 21), (8, 20), (3, 16)], LIME_SHADE)  # Gives the sheet a flowing edge
    for y, colour in ((7, mm.HEADLAMP), (10, LIME), (13, mm.VEST), (16, LIME)):  # Stacks mineral bands
        ImageDraw.Draw(img).line([(4, y + 2), (8, y), (12, y - 1), (17, y), (19, y + 2)], fill=colour, width=2)  # Curves one band
    return ls.outline(img)  # Defines the banded sheet


def dripstone():  # Draws a hanging point grown by drips
    img = mm.new_image(24, 24)  # Fits the item card
    ls.fill_poly(img, [(3, 5), (8, 3), (18, 3), (21, 6), (17, 10), (14, 15), (12, 21), (9, 16), (8, 11)], LIME_SHADE)  # Shapes the tapered point
    ls.fill_poly(img, [(4, 5), (9, 4), (14, 5), (11, 8), (11, 14), (12, 19), (9, 15), (8, 10)], mm.HEADLAMP)  # Lights the long face
    mm.fill_rect(img, 11, 6, 8, 1, WOOD)  # Adds one mineral band
    return ls.outline(img)  # Defines the point


def breccia():  # Draws sharp volcanic pieces in ash
    img = na.blob(24, 24, 11.5, 13, 9.5, 7.5, lambda x, y, shade: CAVE if shade else mm.MASK_SH, 3)  # Makes an ash matrix
    for points, colour in (([(3, 11), (7, 7), (10, 10), (9, 13)], mm.BACKPACK_SH), ([(11, 7), (17, 7), (19, 11), (15, 13)], mm.STRAP), ([(7, 15), (11, 12), (15, 17), (13, 19), (8, 18)], mm.PLATEAU_SH), ([(16, 15), (20, 13), (20, 18)], mm.DARK)):  # Sets sharp clasts in the ash
        ls.fill_poly(img, points, colour)  # Fills a rough fragment
        la.put(img, *points[0], mm.MASK)  # Lights its top-left corner
    return ls.outline(img)  # Defines the clast rock


def ignimbrite():  # Draws warm welded ash with flattened dark lenses
    img = na.blob(24, 24, 11.5, 13, 10, 7, lambda x, y, shade: mm.PLATEAU_SH if shade else TUFF, 2.5)  # Makes a warm ash rock
    for x, y, w in ((5, 9, 5), (11, 8, 6), (8, 12, 9), (4, 15, 5), (13, 16, 6)):  # Places flattened fragments
        ls.fill_poly(img, [(x, y), (x + w - 2, y - 1), (x + w, y), (x + 2, y + 1)], mm.PLATEAU_SH)  # Welds one lens into the rock
    return ls.outline(img)  # Defines the welded rock


def zeolite():  # Draws light crystal groups on a porous base
    img = na.blob(24, 24, 11.5, 15, 10, 5.5, lambda x, y, shade: CAVE if shade else WATER_DARK, 2.8)  # Makes the host rock
    for x, y in ((6, 12), (10, 8), (15, 10), (17, 13)):  # Groups the crystals
        ls.fill_poly(img, [(x - 2, y + 3), (x - 2, y - 1), (x, y - 3), (x + 3, y - 1), (x + 3, y + 2), (x + 1, y + 4)], FOAM)  # Draws one crystal group
        ls.fill_poly(img, [(x, y), (x + 3, y - 1), (x + 3, y + 2), (x + 1, y + 4)], WATER)  # Shades the right face
    for x, y in ((4, 16), (8, 18), (13, 16), (18, 17)):  # Dots the base with pores
        la.put(img, x, y, mm.STRAP)  # Draws one pore
    return ls.outline(img)  # Outlines the specimen


def island_stack(img, x, y, w, h, distant=False):  # Paints a limestone islet with a narrow wave-cut foot
    scale = lambda px, py: (x + px * w // 25, y + py * h // 26)  # Fits a shared shape to this islet
    ls.fill_poly(img, [scale(px, py) for px, py in [(0, 13), (3, 5), (9, 1), (17, 0), (22, 4), (25, 12), (23, 17), (19, 20), (19, 24), (23, 26), (2, 26), (6, 23), (6, 20), (1, 17)]], LIME_SHADE if distant else LIME)  # Leaves a notch at the water
    ls.fill_poly(img, [scale(px, py) for px, py in [(0, 13), (3, 5), (9, 1), (17, 0), (22, 4), (25, 12), (19, 11), (15, 14), (9, 11), (4, 14)]], mm.HILLS_SH if distant else mm.LEAF)  # Keeps greenery above the rock lip
    ls.fill_poly(img, [scale(px, py) for px, py in [(3, 8), (6, 4), (12, 2), (17, 2), (11, 7), (7, 7), (5, 10)]], mm.HILLS if distant else mm.HILLS_SH)  # Lights the left leaves
    ls.fill_poly(img, [scale(px, py) for px, py in [(22, 13), (24, 13), (23, 17), (19, 20), (19, 24), (22, 26), (15, 26), (14, 22), (17, 18), (18, 16)]], MOSS_ROCK if distant else LIME_SHADE)  # Shadows the undercut and far edge


def hundred_islands():  # Paints layered islets in bright sea
    img = ls.framed(lambda x, y: na.sky(x, y) if y < 29 else WATER if y < 54 else WATER_MID)  # Sets a broad blue sea and sky
    for x, y, w, h in ((7, 23, 17, 12), (29, 20, 20, 15), (56, 25, 15, 11), (92, 20, 22, 17)):  # Places far stacks
        island_stack(img, x, y, w, h, True)  # Fades the stacks into the distance
    for x, y, w, h in ((3, 37, 31, 25), (40, 31, 39, 32), (83, 40, 31, 26)):  # Places the important undercut shapes
        island_stack(img, x, y, w, h)  # Makes the near rock clear
    for x, y, w in ((9, 64, 18), (41, 66, 26), (79, 69, 20), (8, 75, 15), (67, 75, 12)):  # Spaces the sea reflections
        mm.fill_rect(img, x, y, w, 1, WATER)  # Lays down one low wave
    mm.paste(img, bangka().resize((26, 9), Image.Resampling.NEAREST), 31, 68)  # Sets a small boat in the sea
    return img  # Keeps the lesson's dark outside frame


def callao_cave():  # Paints the cave with warm daylight shafts
    img = ls.framed(lambda x, y: CAVE if y < 58 else mix(CAVE, mm.OUTLINE))  # Sets the dark cave interior
    ls.fill_poly(img, [(5, 13), (26, 4), (45, 6), (64, 15), (60, 25), (44, 26), (37, 37), (21, 36), (9, 28)], CAVE_LIGHT)  # Opens a sunlit window in the roof
    ls.fill_poly(img, [(15, 14), (29, 8), (50, 10), (57, 16), (50, 20), (39, 19), (32, 27), (22, 25)], FOAM)  # Shows daylight through the hole
    ls.fill_poly(img, [(25, 18), (35, 16), (59, 67), (47, 69)], LIME_SHADE)  # Drops one wide shaft onto the cave floor
    ls.fill_poly(img, [(40, 16), (46, 14), (79, 68), (70, 68)], CAVE_LIGHT)  # Adds a second shaft beside it
    ls.fill_poly(img, [(27, 20), (30, 19), (53, 66), (49, 66)], LIME)  # Lights the first shaft at its core
    for x, h in ((10, 30), (19, 24), (67, 27), (74, 17), (86, 34), (104, 39), (114, 27)):  # Hangs points near the roof
        ls.fill_poly(img, [(x - 4, 1), (x + 4, 1), (x + 2, h // 2), (x, h)], mm.STRAP)  # Draws each dripstone
    ls.fill_poly(img, [(78, 65), (88, 53), (93, 35), (96, 1), (101, 1), (103, 40), (113, 57), (118, 60), (118, 79), (80, 79)], mm.STRAP)  # Draws a column close to the viewer
    ls.fill_poly(img, [(90, 56), (96, 29), (98, 9), (100, 26), (104, 45), (107, 59), (97, 58)], CAVE_LIGHT)  # Traces the lit side of the column
    ls.fill_poly(img, [(33, 71), (48, 65), (62, 66), (84, 73), (72, 77), (43, 76)], LIME_SHADE)  # Spreads light along the cave floor
    mm.fill_rect(img, 47, 71, 22, 2, CAVE_LIGHT)  # Lights the edge of a wet stone
    mm.fill_rect(img, 59, 75, 15, 1, CAVE)  # Breaks up the far edge of the light
    return img  # Keeps the lesson's dark outside frame


def pagsanjan():  # Paints a bangka between high gorge walls
    img = ls.framed(lambda x, y: na.sky(x, y) if y < 34 else WATER_MID)  # Sets the river and its sky gap
    ls.fill_poly(img, [(34, 11), (47, 14), (54, 20), (61, 18), (75, 9), (87, 25), (87, 36), (38, 44)], mm.HILLS_SH)  # Hides the far bank behind moss
    ls.fill_poly(img, [(66, 19), (74, 18), (72, 30), (76, 41), (61, 41)], FOAM)  # Drops a waterfall at the far bend
    ls.fill_poly(img, [(1, 1), (29, 1), (37, 13), (42, 24), (40, 37), (46, 45), (34, 58), (19, 80), (1, 80)], mm.LEAF)  # Closes in the left gorge wall
    ls.fill_poly(img, [(13, 4), (25, 3), (32, 15), (36, 25), (31, 39), (39, 44), (22, 58), (14, 80), (1, 80), (1, 55)], MOSS_ROCK)  # Cuts a lit cliff face
    ls.fill_poly(img, [(91, 1), (118, 1), (118, 80), (104, 80), (88, 65), (88, 54), (78, 44), (86, 31), (84, 19)], mm.LEAF)  # Closes in the right gorge wall
    ls.fill_poly(img, [(102, 1), (114, 1), (109, 19), (113, 31), (102, 42), (111, 60), (118, 69), (118, 79), (103, 79), (93, 65), (94, 53), (84, 44), (92, 32), (89, 19)], MOSS_ROCK)  # Shapes the broken face beside the water
    for x, y, w in ((49, 45, 23), (42, 52, 18), (51, 61, 25), (31, 74, 18), (65, 76, 24)):  # Spaces long river lines
        mm.fill_rect(img, x, y, w, 1, WATER)  # Lights one line
    mm.paste(img, bangka(True), 37, 52)  # Puts the paddler in the main river channel
    return img  # Keeps the lesson's dark outside frame


def banaue():  # Paints broad stepped terraces with green lips and water
    img = ls.framed(na.sky)  # Starts with a light mountain sky
    ls.fill_poly(img, [(1, 37), (17, 22), (31, 15), (44, 22), (56, 15), (66, 22), (84, 9), (97, 15), (118, 29), (118, 78), (1, 78)], mix(lm.HAZE, mm.HILLS_SH))  # Puts misty ridges behind the fields
    for tier in range(6):  # Draws the water, grass lip and soil front of each step
        y = 23 + tier * 8  # Drops each later tier closer to the viewer
        x = max(1, 58 - tier * 12)  # Widens the near tiers
        edge = [(x, y + 4), (x + 20, y + 1), (63, y + 5), (90, y + 3), (118, y - 4)]  # Shapes the rising banks
        ls.fill_poly(img, edge + [(118, 80), (x, 80)], mm.LEAF)  # Sets the terrace bank
        ImageDraw.Draw(img).line(edge, fill=mm.PLAINS, width=2)  # Gives the bank a green crest
        ImageDraw.Draw(img).line([(px, py + 3) for px, py in edge], fill=WATER if tier % 2 == 0 else mm.HILLS, width=3)  # Paints water and rice along the steps
    mm.paste(img, ifugao_house().resize((19, 19), Image.Resampling.NEAREST), 81, 28)  # Places an Ifugao house on a slope
    return img  # Keeps the lesson's dark outside frame


def chimney(img, x, y, w, h, distant=False):  # Draws a tapered tuff cone under a harder cap
    ls.fill_poly(img, [(x, y + h), (x + w // 4, y + h // 2), (x + w // 3, y + 5), (x + w * 2 // 3, y + 5), (x + w * 3 // 4, y + h // 2), (x + w, y + h)], WOOD if distant else TUFF)  # Draws its soft body
    ls.fill_poly(img, [(x + w // 2, y + 6), (x + w * 2 // 3, y + 5), (x + w, y + h), (x + w * 2 // 3, y + h)], mm.PLATEAU if distant else WOOD)  # Shades its right side
    ls.fill_poly(img, [(x + w // 5, y + 6), (x + w // 3, y + 1), (x + w * 2 // 3, y), (x + w * 4 // 5, y + 4), (x + w * 3 // 4, y + 7), (x + w // 4, y + 7)], WOOD_SHADE)  # Sets a darker rock cap on top


def cappadocia():  # Paints fairy chimneys above a tuff valley at dawn
    img = ls.framed(lambda x, y: mm.SKY_LIGHT if y < 23 else DAWN if y < 48 else TUFF)  # Sets the layered dawn sky
    ls.disc(img, 22, 27, 8, mm.HEADLAMP)  # Puts the rising sun at the left
    ls.fill_poly(img, [(1, 43), (17, 38), (28, 39), (54, 34), (81, 42), (99, 31), (118, 38), (118, 80), (1, 80)], WOOD)  # Puts soft hills behind the chimneys
    for x, y, w, h in ((3, 33, 20, 28), (29, 39, 21, 29), (49, 31, 17, 33), (69, 45, 20, 23), (93, 34, 23, 34)):  # Places a row of slender capped points
        chimney(img, x, y, w, h, True)  # Colours them for the middle distance
    ls.fill_poly(img, [(1, 70), (27, 63), (51, 70), (80, 64), (111, 70), (118, 67), (118, 80), (1, 80)], TUFF_LIGHT)  # Lights the near valley floor
    chimney(img, 7, 46, 23, 31)  # Brings one cone close
    chimney(img, 85, 41, 28, 36)  # Brings a taller cone close
    for x, y, size in ((50, 9, (11, 18)), (74, 4, (15, 25)), (102, 15, (6, 10))):  # Hangs three balloons at different distances
        mm.paste(img, balloon().resize(size, Image.Resampling.NEAREST), x, y)  # Draws one small balloon
    return img  # Keeps the lesson's dark outside frame


def preview(groups):  # Lays out every sprite at triple size with its exact name
    width, pad, gap = 1160, 12, 12  # Sets the sheet width and clear space around the sprites
    y = pad  # Starts the first group
    placements, headings = [], []  # Saves the drawing positions
    for title, sprites in groups:  # Gives each kind of sprite its own group
        headings.append((title, y))  # Reserves the heading row
        x, y, row = pad, y + 24, 0  # Starts the next row below the heading
        for name, img in sprites:  # Sizes one preview tile at a time
            w, h = img.width * 3, img.height * 3  # Uses whole pixel scaling
            cell = max(w + 12, len(name) * 7 + 12)  # Keeps the full label in the tile
            if x + cell > width - pad:  # Wraps the row when the tile would run off the sheet
                x, y, row = pad, y + row + gap, 0  # Moves to the next row
            placements.append((name, img, x, y, cell))  # Saves the tile position
            x, row = x + cell + gap, max(row, h + 30)  # Makes room for the label and art
        y += row + 20  # Leaves a gap above the next group
    sheet = Image.new("RGBA", (width, y), mm.OUTLINE + (255,))  # Creates a dark sheet around muted tiles
    draw = ImageDraw.Draw(sheet)  # Gives the sheet a pixel text pen
    for title, top in headings:  # Labels each group
        draw.text((pad, top), title, fill=mm.HEADLAMP)  # Draws a pale heading
    for name, img, x, top, cell in placements:  # Draws the sprite and label in its tile
        big = img.resize((img.width * 3, img.height * 3), Image.Resampling.NEAREST)  # Upscales without soft edges
        mm.fill_rect(sheet, x, top, cell, big.height + 26, mix(lm.HAZE_SH, mm.STRAP))  # Gives light and dark sprites an even backing
        sheet.alpha_composite(big, (x + (cell - big.width) // 2, top + 6))  # Centers the sprite above its name
        draw.text((x + 5, top + big.height + 9), name, fill=FOAM)  # Gives the exact name
    sheet.save(PREVIEW)  # Writes the preview sheet


def main():  # Writes the requested PNGs and the preview
    tiles = [entry for theme in THEMES for entry in set_tiles(theme)]  # Builds all twenty-five tiles and boulders
    objects = [("water_fill", water_tile(16)), ("float_raft", raft()), ("current_top", water_tile(8, True)), ("balloon", balloon())]  # Draws single-frame objects
    for name, painter, frames in (("float_ripple", ripple, 2), ("current_mark", current_mark, 2), ("bat", bat, 3), ("balloon_flame", flame, 2)):  # Lists animated objects
        objects.extend((f"{name}_{i:02d}", painter(i)) for i in range(frames))  # Draws every frame
    goals = [(painter.__name__, painter()) for painter in (cave_mouth, falls_landing, ifugao_house, balloon_field)]  # Draws goals named like their painters
    goals.insert(0, ("boat_dock", dock()))  # Gives the pier its asset name
    props = [(painter.__name__, painter()) for painter in (bangka, mangrove, stalagmite, bat_sign, bulul, cone_house, dry_shrub)]  # Draws still props
    props.append(("bangka_boatman", bangka(True)))  # Draws the boat with a paddler
    for painter in (seabird, chapel_glow, falls_mist, rice_stalk, farmer, far_balloon):  # Lists animated props
        props.extend((f"{painter.__name__}_{i:02d}", painter(i)) for i in range(2))  # Draws both frames for each prop
    items = [(painter.__name__, painter()) for painter in (aragonite, coquina, flowstone, dripstone, ignimbrite, zeolite)]  # Draws the matching item pictures
    items.append(("volcanic_breccia", breccia()))  # Gives the ash rock its item id
    lessons = [(f"lesson_{painter.__name__}", painter()) for painter in (hundred_islands, callao_cave, pagsanjan, banaue, cappadocia)]  # Draws the five lesson pictures
    for folder, sprites in ((LEVEL, tiles + objects + goals), (LEVEL / "Props", props), (Path(ls.ITEMS_OUT), items), (Path(ls.LESSON_OUT), lessons)):  # Puts each set in its asset folder
        for name, img in sprites:  # Writes one named PNG
            img.save(folder / f"{name}.png")  # Saves only Hills Normal output paths
    with Image.open(LEVEL / "Props" / "fern.png") as fern:  # Loads the existing 16 by 12 fern for the preview
        preview([("Terrain", tiles), ("Water and moving objects", objects), ("Goals", goals), ("Props", props + [("fern", fern.convert("RGBA"))]), ("Rocks and minerals", items), ("Lessons", lessons)])  # Shows all requested sprites
    print(PREVIEW)  # Prints the preview path


if __name__ == "__main__":  # Runs only when called as a script
    main()  # Writes the art
