"""Draws the PNGs for Plateaus Normal 1 to 5 (D-073): five level themes (Table Top, Mesa Valley, Canyon Rim,
Tepui Top and Salt Flat) with their goals, the moving cable car and its cable, the slippery salt crust top,
the llama herd, the new props, one new item and five lesson pictures.

Run from the Unity project folder:  python Tools/Art/make_plateaus_normal_art.py
It writes to Assets/Art/Level/Sprites/ (themes, goals, cable car, llamas and props), Assets/Art/Items/Sprites/ (item)
and Assets/Art/Lesson/Sprites/ (lesson pictures), plus a preview sheet at .scratch/plateaus-normal/art-preview.png.
It reuses the helpers of make_mainmenu_art.py, make_landform_map_art.py, make_level_art.py, make_lesson_art.py,
make_normal_art.py and make_plains_hard_art.py.
All colours come from Assets/Art/Palettes/landgrand.gpl or mix two of its colours.
Props/pitcher_plant.png already belongs to the Mountains Hard art (10x14), so the new one is saved as
Props/pitcher_plant_tepui.png instead of overwriting it.
"""
import math  # math gives circles, waves and the wheel spokes
import os  # os joins folder paths
from PIL import Image  # Image builds the preview sheet and flips the llama
import make_mainmenu_art as mm  # Palette, new_image, fill_rect and paste
import make_landform_map_art as lm  # Map colours, the fixed noise and the ridge helper
import make_level_art as la  # put, outline, wave, boulder_colored and save for level sprites
import make_lesson_art as ls  # framed, disc, fill_poly and the item and lesson folders
import make_normal_art as na  # mix, the shared tile makers, sky and the blob shape
import make_plains_hard_art as ph  # ell, line and ring for animals and wheels

mix = na.mix  # The colour halfway between two palette colours
PREVIEW = os.path.join(mm.ROOT, ".scratch", "plateaus-normal", "art-preview.png")  # Where the preview sheet goes

# Palette colours that the helper scripts do not name.
SHIRT = (63, 143, 134)  # Shirt, from the palette: blue-green
PANTS = (92, 74, 61)  # Pants, from the palette: grey-brown
SUN = (255, 242, 168)  # Sun, from the palette: pale yellow
SUN_GLOW = (255, 225, 122)  # Sun glow, from the palette: warm yellow

# Table Top: Table Mountain, grey-buff sandstone and fynbos shrubs.
TS = mix(mm.MASK, mm.VEST)  # Grey-buff sandstone
TS_SH = mix(mm.MASK_SH, mm.VEST_SH)  # Sandstone in shade and its bed lines
TS_LT = mix(mm.MASK, mm.HEADLAMP)  # Sunny sandstone
TS_DK = mix(mm.MASK_SH, mm.STRAP)  # Dark specks in the sandstone
FYN = mix(mm.HILLS_SH, mm.MASK_SH)  # Grey-green fynbos
FYN_SH = mix(mm.LEAF, mm.STRAP)  # Fynbos in shade
FYN_LT = mix(lm.PLAINS_SH, mm.MASK)  # Sunny fynbos tips
PROTEA = mix(mm.SCARF, mm.WHITE)  # Pink protea flower
PROTEA_SH = mix(mm.SCARF, mm.MASK)  # Protea in shade
CAR_RED = mm.SCARF  # Red cable car and station trim
CAR_RED_SH = mm.SCARF_SH  # Red in shade
STEEL = mm.STRAP  # Dark steel of the wheel, arm and cable
STEEL_LT = mm.MASK_SH  # Steel shine
WALL = mm.MASK  # Painted station wall
WALL_SH = mm.MASK_SH  # Station wall in shade
GLASS = mm.SKY_LIGHT  # Window glass
GLASS_SH = lm.HAZE  # Window glass in shade

# Mesa Valley: Monument Valley, red-orange sandstone and sand.
MESA = mm.PLATEAU  # Red-orange sandstone
MESA_SH = mm.PLATEAU_SH  # Sandstone in shade
MESA_LT = mix(mm.PLATEAU, mm.HELMET)  # Sunny sandstone
MESA_DK = mm.BACKPACK_SH  # Dark specks in the sandstone
VARNISH = mix(mm.PLATEAU_SH, mm.STRAP)  # Dark desert varnish streaks on the cliffs
SAND = mix(mm.VEST, mm.PLATEAU)  # Orange-tan sand
SAND_LT = mix(mm.PLATEAU, mm.HEADLAMP)  # Sunny sand
SAGE = mix(lm.PLAINS_SH, mm.MASK_SH)  # Grey-green sagebrush
SAGE_SH = mix(mm.HILLS_SH, mm.MASK_SH)  # Sagebrush in shade
SAGE_LT = mix(lm.PLAINS_SH, mm.MASK)  # Sunny sagebrush
WOOD = lm.HANDLE  # Light wood
WOOD_SH = mm.GROUND_SH  # Wood in shade
WOOD_LT = mix(lm.HANDLE, mm.HEADLAMP)  # Sunny plank top

# Canyon Rim: Grand Canyon, layered red, buff and grey rock bands under a pale limestone rim.
KAIBAB = mix(mm.MASK, mm.VEST)  # Pale grey-buff limestone on the rim
KAIBAB_LT = mix(mm.MASK, mm.HEADLAMP)  # Sunny limestone
BUFF = mm.VEST  # Buff sandstone band
BUFF_SH = mm.VEST_SH  # Buff band in shade
RED = mm.PLATEAU_SH  # Red rock band
RED_LT = mm.PLATEAU  # Sunny red rock
RED_DK = mm.BACKPACK_SH  # Red rock in deep shade
GREY = mm.MASK_SH  # Grey rock band
GREY_SH = mix(mm.MASK_SH, mm.STRAP)  # Grey band in shade
PINYON = mm.LEAF  # Dark green pinyon needles
PINYON_SH = mix(mm.LEAF, mm.OUTLINE)  # Pinyon needles in shade
PINYON_LT = mm.HILLS_SH  # Sunny pinyon needles
TRUNK = lm.BACKPACK  # Pinyon trunk
TRUNK_SH = mm.BACKPACK_SH  # Trunk in shade
RIVER = mix(lm.LENS, mm.HILLS_SH)  # Green-blue Colorado River

# Tepui Top: Mount Roraima, black-grey quartzite with a pink tint, pools, moss and mist.
TEP = mm.STRAP  # Black-grey quartzite
TEP_SH = mix(mm.STRAP, mm.OUTLINE)  # Quartzite in deep shade
TEP_LT = mix(mm.STRAP, mm.MASK_SH)  # Grey weathered rock face
TEP_PINK = mix(mm.MASK_SH, mm.SCARF)  # Pink tint of the quartz
TEP_PINK_DK = mix(mm.STRAP, mm.SCARF_SH)  # Dark rose band in the rock
POOL = mix(lm.MOUNTAIN_SH, mm.OUTLINE)  # Dark clear rock pool
POOL_LT = lm.LENS  # Shine on a pool
MOSS = mm.LEAF  # Wet green moss
MOSS_LT = mm.HILLS_SH  # Sunny moss
MIST = mm.WHITE  # White mist
MIST_EDGE = lm.HAZE  # Pale blue mist edge
TENT = mm.SCARF  # Orange-red tent
TENT_SH = mm.SCARF_SH  # Tent in shade
FLAME = mm.HELMET  # Yellow flame
FLAME_HOT = SUN  # Pale hot heart of the flame
FLAME_RED = mm.SCARF  # Red flame edge
PITCHER = mm.HILLS_SH  # Green pitcher tube
PITCHER_SH = mm.LEAF  # Pitcher tube in shade
PITCHER_RED = mm.SCARF_SH  # Red pitcher rim
PITCHER_RIM = mm.SCARF  # Bright red pitcher lip

# Salt Flat: Salar de Uyuni, white hexagon-crusted salt under a blue sky.
SALT = mm.WHITE  # Pure white salt
SALT_SH = mix(mm.MASK, mm.WHITE)  # Salt in soft shade
SALT_DK = mm.MASK  # Salt in shade
SALT_RIDGE = mix(mm.MASK, mm.MASK_SH)  # Raised hexagon ridges and block joints
SALT_SHEEN = mix(mm.WHITE, mm.SKY_LIGHT)  # Wet shiny salt
BRINE = mix(mm.SKY_LIGHT, lm.LENS)  # Blue brine under the crust
MUD = mix(mm.MASK_SH, mm.VEST_SH)  # Grey-brown mud layer in the salt
THATCH = mix(mm.VEST, mm.HELMET_SH)  # Straw roof
THATCH_SH = mm.VEST_SH  # Straw roof in shade
FLAMINGO = mix(mm.SCARF, mm.WHITE)  # Pink flamingo
FLAMINGO_SH = mix(mm.SCARF, mm.MASK)  # Flamingo in shade
FLAMINGO_LEG = mix(mm.SCARF, mm.PLATEAU)  # Darker pink legs

# Animals, props and the item.
LLAMA = mix(mm.VEST, mm.HEADLAMP)  # Cream llama wool
LLAMA_SH = mm.VEST  # Wool in shade
LLAMA_PATCH = mm.GROUND  # Brown patch on the llama
LLAMA_TASSEL = mm.SCARF  # Red wool tassel on the ear
DASSIE = mix(mm.GROUND, mm.MASK_SH)  # Grey-brown rock hyrax fur
DASSIE_SH = mix(mm.GROUND_SH, mm.STRAP)  # Fur in shade
DASSIE_BELLY = mm.VEST  # Pale belly
RAVEN = mix(mm.STRAP, mm.OUTLINE)  # Black raven
RAVEN_LT = mm.STRAP  # Raven feather shine
ULEX = mm.WHITE  # White ulexite fibre
ULEX_SILK = SALT_SHEEN  # Silky blue-white fibre
ULEX_SH = mm.MASK  # Fibre in shade
ULEX_SH_SILK = mix(mm.MASK, mm.SKY_LIGHT)  # Silky fibre in shade
ULEX_SEEN = lm.HAZE_SH  # A dark line seen through the clear fibres


# ---------- Table Top: Table Mountain (N1) ----------

def tabletop_top():
    """A 16x8 tile of grey-green fynbos over grey-buff sandstone, with pink protea flowers."""  # Describes the tile
    specks = [(3, 1, PROTEA), (4, 1, PROTEA_SH), (11, 2, PROTEA), (7, 2, FYN_SH), (14, 1, FYN_SH), (1, 2, FYN_SH),  # Flowers and shrub shade
              (5, 6, TS_SH), (12, 6, TS_DK)]  # Specks in the sandstone
    return na.ground_top(TS, FYN, FYN_LT, FYN_SH, specks)  # Fynbos cap over sandstone


def tabletop_soil():
    """A 16x16 grey-buff sandstone tile with flat bed lines and short slanting cross-beds."""  # Describes the tile
    layers = [(x, 5, TS_SH) for x in range(16)]  # A flat bed line
    layers += [(x, 4, TS_LT) for x in range(16) if x % 7]  # Sunny edge above the bed line
    layers += [(x, 12, TS_SH) for x in range(16)]  # A second flat bed line
    layers += [(2 + t, 10 - t // 2, TS_SH) for t in range(5)]  # Short slanting cross-bed
    layers += [(10 + t, 3 - t // 2, TS_SH) for t in range(4)]  # Another cross-bed
    return na.soil_tile(TS, TS_DK, layers)  # Sandstone with its beds


def ledge_tabletop():
    """A 16x10 sandstone ledge with a fynbos top and a flat bed line."""  # Describes the tile
    flecks = [(x, 5, TS_SH) for x in range(16) if x % 6]  # Bed line with small gaps
    flecks += [(3, 0, PROTEA), (10, 1, FYN_SH), (13, 0, FYN_LT), (7, 3, TS_LT), (12, 6, TS_DK)]  # Flower, shrubs and rock bits
    return na.ledge_tile(TS, FYN, TS_SH, flecks)  # Sandstone body with fynbos on top


def tabletop_boulder(w, h):
    """A grey-buff sandstone boulder with a thin fynbos cap."""  # Describes the boulder
    return na.capped_boulder(w, h, TS, TS_SH, TS_LT, TS_DK, FYN, 1)  # Sandstone with shrubs on top


def cable_station():
    """A 56x48 small stone cable-car station hut with a flat red roof and a big cable wheel on its frame (the N1 goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    for y in range(42, 47):  # Rows of the stone plinth
        for x in range(3, 53):  # Columns of the plinth
            joint = y == 44 or (x + (y // 3) * 4) % 8 == 0  # Joints between the cut stones
            la.put(img, x, y, TS_SH if joint else TS)  # Sandstone block or joint
    mm.fill_rect(img, 3, 42, 50, 1, TS_LT)  # Sunny top of the plinth
    mm.fill_rect(img, 8, 20, 30, 22, WALL)  # Station wall
    mm.fill_rect(img, 34, 20, 4, 22, WALL_SH)  # Shaded right end of the wall
    mm.fill_rect(img, 14, 30, 7, 12, mm.OUTLINE)  # Doorway
    mm.fill_rect(img, 14, 30, 7, 1, CAR_RED_SH)  # Red lintel over the door
    for x0 in (24, 30):  # Two windows
        mm.fill_rect(img, x0, 25, 4, 5, GLASS)  # Glass
        mm.fill_rect(img, x0 + 2, 25, 2, 5, GLASS_SH)  # Glass in shade
        mm.fill_rect(img, x0, 27, 4, 1, WALL_SH)  # Window bar
    mm.fill_rect(img, 10, 23, 2, 2, CAR_RED)  # Small red light by the door
    mm.fill_rect(img, 6, 16, 34, 4, CAR_RED)  # Flat red roof
    mm.fill_rect(img, 6, 19, 34, 1, CAR_RED_SH)  # Roof edge in shade
    mm.fill_rect(img, 6, 16, 34, 1, mix(mm.SCARF, mm.HEADLAMP))  # Sunny roof top
    mm.fill_rect(img, 47, 12, 3, 30, STEEL)  # Steel post of the wheel frame
    mm.fill_rect(img, 47, 12, 1, 30, STEEL_LT)  # Light edge of the post
    ph.line(img, 40, 18, 47, 12, STEEL)  # Brace from the roof to the wheel
    ph.line(img, 41, 18, 47, 13, STEEL)  # Brace, second pixel
    for y in range(20, 42, 5):  # Cross bars between the post and the wall
        mm.fill_rect(img, 38, y, 9, 1, STEEL)  # One cross bar
    cx, cy = 44, 10  # Middle of the cable wheel
    ph.ring(img, cx, cy, 8, STEEL)  # Outer rim of the wheel
    ph.ring(img, cx, cy, 7, STEEL_LT)  # Light inner rim
    for k in range(6):  # Six spokes
        a = k * math.pi / 3 + 0.3  # Angle of this spoke
        ph.line(img, cx, cy, round(cx + 6 * math.cos(a)), round(cy + 6 * math.sin(a)), STEEL)  # One spoke
    ls.disc(img, cx, cy, 1, mm.HELMET)  # Yellow hub
    img = la.outline(img)  # Dark edge
    ph.line(img, 44, 2, 55, 0, STEEL)  # The cable leaving the top of the wheel
    return img  # The finished picture


# ---------- Mesa Valley: Monument Valley (N2) ----------

def mesa_top():
    """A 16x8 tile of orange sand over red-orange sandstone, with sagebrush tufts."""  # Describes the tile
    specks = [(2, 2, SAGE), (3, 1, SAGE_LT), (4, 2, SAGE_SH), (11, 1, SAGE), (12, 2, SAGE_SH),  # Sagebrush tufts
              (8, 2, MESA_SH), (6, 6, MESA_DK), (13, 6, MESA_LT)]  # Sand ripple and sandstone specks
    return na.ground_top(MESA, SAND, SAND_LT, MESA_SH, specks)  # Sand cap over sandstone


def mesa_soil():
    """A 16x16 red-orange sandstone tile with flat beds and a dark varnish streak."""  # Describes the tile
    layers = [(x, 3, MESA_LT) for x in range(16)]  # A sunny bed
    layers += [(x, 9, MESA_SH) for x in range(16)]  # A shaded bed
    layers += [(x, 10, MESA_SH) for x in range(16) if x % 5]  # Thicker shade with gaps
    layers += [(x, 14, MESA_LT) for x in range(16) if x % 4]  # A lower sunny bed with gaps
    layers += [(6, y, VARNISH) for y in range(0, 8)]  # Dark varnish streak running down
    layers += [(7, y, VARNISH) for y in range(0, 4)]  # Wider top of the streak
    return na.soil_tile(MESA, MESA_DK, layers)  # Sandstone with its beds


def ledge_mesa():
    """A 16x10 red-orange sandstone ledge with a sandy top."""  # Describes the tile
    flecks = [(x, 5, MESA_SH) for x in range(16) if x % 7]  # Bed line
    flecks += [(4, 3, MESA_LT), (11, 6, MESA_DK), (2, 1, MESA_SH), (9, 0, SAND_LT), (13, 7, VARNISH)]  # Rock and sand bits
    return na.ledge_tile(MESA, SAND, MESA_SH, flecks)  # Sandstone body with sand on top


def mesa_boulder(w, h):
    """A rounded red-orange sandstone boulder with sunny beds."""  # Describes the boulder
    return la.boulder_colored(w, h, MESA, MESA_SH, MESA_LT, MESA_DK, (MESA_LT, MESA_SH))  # Layered red rock


def trading_post():
    """A 56x48 simple wooden trading post with a porch, a hitching rail and a barrel; no writing and no symbols (the N2 goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    mm.fill_rect(img, 8, 10, 40, 34, WOOD)  # Front wall and tall flat front
    for y in range(12, 44, 3):  # Plank lines across the wall
        mm.fill_rect(img, 8, y, 40, 1, WOOD_SH)  # One plank line
    mm.fill_rect(img, 6, 8, 44, 2, WOOD_SH)  # Cap board on top of the front
    mm.fill_rect(img, 6, 8, 44, 1, WOOD_LT)  # Sunny top of the cap board
    for x in (8, 47):  # Corner posts
        mm.fill_rect(img, x, 10, 1, 34, WOOD_SH)  # One corner post
    mm.fill_rect(img, 24, 30, 8, 14, mm.OUTLINE)  # Doorway
    mm.fill_rect(img, 25, 31, 2, 13, WOOD_SH)  # Open door leaf
    for x0 in (12, 37):  # Two windows
        mm.fill_rect(img, x0, 30, 7, 6, GLASS)  # Glass
        mm.fill_rect(img, x0 + 4, 30, 3, 6, GLASS_SH)  # Glass in shade
        mm.fill_rect(img, x0 + 3, 30, 1, 6, WOOD_SH)  # Upright bar
        mm.fill_rect(img, x0, 32, 7, 1, WOOD_SH)  # Cross bar
    mm.fill_rect(img, 2, 22, 52, 3, WOOD_SH)  # Porch roof
    mm.fill_rect(img, 2, 22, 52, 1, WOOD_LT)  # Sunny edge of the porch roof
    for x in (3, 52):  # Porch posts
        mm.fill_rect(img, x, 25, 2, 19, WOOD_SH)  # One porch post
    mm.fill_rect(img, 1, 44, 54, 3, WOOD)  # Porch floor
    mm.fill_rect(img, 1, 44, 54, 1, WOOD_LT)  # Sunny top of the floor
    for x in range(1, 55, 6):  # Plank ends along the floor edge
        la.put(img, x, 46, WOOD_SH)  # One plank end
    ph.ell(img, 44, 40, 3, 4, WOOD, WOOD_SH)  # Wooden barrel by the door
    mm.fill_rect(img, 41, 38, 7, 1, STEEL)  # Barrel hoop
    mm.fill_rect(img, 41, 42, 7, 1, STEEL)  # Lower barrel hoop
    mm.fill_rect(img, 10, 39, 10, 1, WOOD_SH)  # Hitching rail bar
    for x in (10, 19):  # Hitching rail legs
        mm.fill_rect(img, x, 39, 1, 5, WOOD_SH)  # One leg
    return la.outline(img)  # Dark edge


# ---------- Canyon Rim: Grand Canyon (N3) ----------

def canyon_top():
    """A 16x8 tile of pale limestone rim over buff rock, with small sagebrush tufts."""  # Describes the tile
    specks = [(3, 1, SAGE), (4, 2, SAGE_SH), (10, 2, SAGE), (11, 1, SAGE_LT), (7, 2, BUFF_SH), (14, 2, BUFF_SH),  # Sage and rock bits
              (5, 6, RED), (12, 6, BUFF_SH)]  # Specks in the rock below
    return na.ground_top(BUFF, KAIBAB, KAIBAB_LT, BUFF_SH, specks)  # Limestone cap over buff rock


CANYON_BANDS = [BUFF, BUFF, BUFF, BUFF_SH, RED, RED, RED_LT, RED, RED_DK, GREY, GREY, GREY_SH, RED, RED_LT, RED, RED_DK]  # Band of each row


def canyon_soil():
    """A 16x16 tile of layered red, buff and grey rock bands, so stacked tiles look like a canyon wall."""  # Describes the tile
    img = mm.new_image(16, 16)  # Empty tile
    for y in range(16):  # Each row
        for x in range(16):  # Each pixel
            col = CANYON_BANDS[y]  # Colour of this band
            if lm.noise(x, y) < 7:  # A few specks
                col = RED_DK if col != RED_DK else GREY_SH  # Dark speck
            la.put(img, x, y, col)  # Paint it
    for (x, y) in ((4, 1), (5, 1), (12, 6), (13, 6), (2, 10), (9, 13)):  # Short cracks across the bands
        la.put(img, x, y, GREY_SH)  # One crack pixel
    return img  # The finished tile


def ledge_canyon():
    """A 16x10 banded rock ledge with a pale limestone top."""  # Describes the tile
    flecks = [(x, 4, BUFF) for x in range(16)]  # A buff band
    flecks += [(x, 6, GREY) for x in range(16) if x % 5]  # A thin grey band
    flecks += [(3, 3, RED_LT), (11, 2, RED_LT), (7, 7, RED_DK), (13, 0, KAIBAB_LT)]  # Rock bits
    return na.ledge_tile(RED, KAIBAB, RED_DK, flecks)  # Red body with limestone on top


def canyon_boulder(w, h):
    """A rounded red rock boulder with buff bands."""  # Describes the boulder
    return la.boulder_colored(w, h, RED_LT, RED, mix(mm.PLATEAU, mm.HEADLAMP), RED_DK, (BUFF, BUFF_SH))  # Banded red rock


def canyon_lookout():
    """A 56x48 stone lookout wall of red, buff and grey stones with a flat top and a steel rail (the N3 goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    stones = (BUFF, RED_LT, GREY, KAIBAB, RED)  # Stone colours, picked by the fixed noise
    for y in range(24, 48):  # Rows of the stone wall
        lean = (47 - y) // 6  # The wall leans in a little going up
        for x in range(4 + lean, 52 - lean):  # Columns of this row
            row = (y - 24) // 4  # Course of stones
            sx = (x + (row % 2) * 4) // 8  # Stone number along the course
            col = stones[lm.noise(sx, row) % 5]  # Colour of this stone
            if (y - 24) % 4 == 3 or (x + (row % 2) * 4) % 8 == 0:  # Mortar line
                col = GREY_SH  # Dark mortar
            la.put(img, x, y, col)  # Paint it
    mm.fill_rect(img, 2, 21, 52, 3, KAIBAB)  # Flat stone top slab
    mm.fill_rect(img, 2, 21, 52, 1, KAIBAB_LT)  # Sunny top of the slab
    mm.fill_rect(img, 2, 23, 52, 1, BUFF_SH)  # Slab edge in shade
    for x in (4, 15, 27, 39, 50):  # Rail posts
        mm.fill_rect(img, x, 9, 2, 12, STEEL)  # One rail post
        la.put(img, x, 9, STEEL_LT)  # Light post top
    mm.fill_rect(img, 4, 9, 48, 2, STEEL)  # Top rail
    mm.fill_rect(img, 4, 9, 48, 1, STEEL_LT)  # Light on the top rail
    mm.fill_rect(img, 4, 15, 48, 1, STEEL)  # Middle rail
    img = la.outline(img)  # Dark edge
    mm.paste(img, sagebrush(), 43, 38)  # A sagebrush at the foot, right
    return img  # The finished picture


# ---------- Tepui Top: Mount Roraima (N4) ----------

def tepui_top():
    """A 16x8 tile of grey weathered quartzite over black rock, with pink specks, moss and a small dark pool."""  # Describes the tile
    specks = [(2, 1, TEP_PINK), (3, 2, TEP_PINK_DK), (6, 1, MOSS), (7, 1, MOSS_LT), (14, 2, MOSS),  # Pink quartz and moss
              (10, 1, POOL), (11, 1, POOL), (12, 1, POOL_LT), (10, 2, POOL), (11, 2, POOL),  # A small dark pool
              (4, 6, TEP_PINK_DK), (13, 6, TEP_LT)]  # Specks in the rock below
    return na.ground_top(TEP, TEP_LT, mm.MASK_SH, TEP_SH, specks)  # Grey cap over black rock


def tepui_soil():
    """A 16x16 black quartzite tile with deep vertical cracks and a rose band."""  # Describes the tile
    extra = [(x, 7 + la.wave(x, 1), TEP_PINK_DK) for x in range(16)]  # A dark rose band
    extra += [(x, 8 + la.wave(x, 1), TEP_PINK_DK) for x in range(16) if x % 3]  # Thicker band with gaps
    for x in (4, 12):  # Two deep vertical cracks
        for y in range(16):  # Down the whole tile, so it stacks
            if (y + x) % 7 != 3:  # Small gaps in the crack
                extra.append((x, y, TEP_SH))  # Dark crack
                extra.append((x + 1, y, TEP_LT))  # Lit edge beside the crack
    extra += [(8, 2, TEP_PINK), (2, 12, TEP_PINK), (14, 14, TEP_PINK)]  # Pink quartz specks
    return na.soil_tile(TEP, TEP_SH, extra)  # Black rock with its cracks


def ledge_tepui():
    """A 16x10 black quartzite ledge with a grey top, moss and pink specks."""  # Describes the tile
    flecks = [(5, 4, TEP_SH), (5, 5, TEP_SH), (6, 4, TEP_LT), (11, 3, TEP_PINK), (12, 6, TEP_PINK_DK),  # Rock texture
              (2, 0, MOSS), (3, 0, MOSS_LT), (9, 1, MOSS), (14, 0, TEP_PINK)]  # Moss and pink bits on top
    return na.ledge_tile(TEP, TEP_LT, TEP_SH, flecks)  # Black body with a grey top


def tepui_boulder(w, h):
    """A black quartzite boulder with pink specks and a cap of green moss."""  # Describes the boulder
    return na.capped_boulder(w, h, TEP_LT, TEP, mm.MASK_SH, TEP_PINK, MOSS, 1)  # Grey-black rock with moss


def explorer_camp():
    """A 56x48 explorer camp: an orange tent and a small campfire ringed with stones (the N4 goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    mm.fill_rect(img, 1, 44, 54, 3, TEP_LT)  # Flat rock under the camp
    mm.fill_rect(img, 1, 44, 54, 1, mm.MASK_SH)  # Sunny top of the rock
    for x in range(1, 55, 5):  # Cracks in the rock
        la.put(img, x, 46, TEP)  # One crack
    ls.fill_poly(img, [(21, 12), (39, 44), (3, 44)], TENT)  # Tent body
    ls.fill_poly(img, [(21, 12), (39, 44), (25, 44)], TENT_SH)  # Shaded right side of the tent
    ls.fill_poly(img, [(21, 26), (27, 44), (15, 44)], mm.OUTLINE)  # Open tent door
    ph.line(img, 21, 12, 15, 44, mix(mm.SCARF, mm.HEADLAMP))  # Light seam down the left of the door
    mm.fill_rect(img, 20, 9, 2, 4, STEEL)  # Tent pole poking out of the top
    ph.ell(img, 46, 42, 6, 2, mm.MASK_SH, TEP_LT)  # Stones round the fire
    for (x, y) in ((41, 41), (45, 40), (49, 41), (51, 42)):  # Gaps between the stones
        la.put(img, x, y, TEP)  # Dark gap
    ph.line(img, 41, 41, 50, 38, WOOD_SH)  # A log in the fire
    ph.line(img, 41, 38, 51, 41, WOOD)  # A crossed log
    ls.fill_poly(img, [(46, 26), (51, 39), (41, 39)], FLAME_RED)  # Red outer flame
    ls.fill_poly(img, [(46, 30), (49, 39), (43, 39)], FLAME)  # Yellow flame
    ls.fill_poly(img, [(46, 34), (47, 39), (45, 39)], FLAME_HOT)  # Pale hot heart
    img = la.outline(img)  # Dark edge
    ph.line(img, 21, 11, 33, 4, mm.MASK_SH)  # Guy rope from the pole top
    for (x, y) in ((44, 22), (48, 18), (45, 14)):  # Sparks rising from the fire
        la.put(img, x, y, FLAME)  # One spark
    return img  # The finished picture


# ---------- Salt Flat: Salar de Uyuni (N5) ----------

def hex_ridges(img, color):
    """Paints a low honeycomb ridge line on rows 1 to 3 of a 16-wide top tile; it repeats every 8 columns."""  # Describes the helper
    for x in range(16):  # Each column
        k = x % 8  # Place inside one honeycomb cell
        if 2 <= k <= 5:  # Top edge of a cell
            la.put(img, x, 1, color)  # Ridge pixel
        elif k in (1, 6):  # Slanting side of a cell
            la.put(img, x, 2, color)  # Ridge pixel
        else:  # Bottom edge between two cells
            la.put(img, x, 3, color)  # Ridge pixel


def saltflat_top():
    """A 16x8 tile of white salt crust with low honeycomb ridges over shaded salt."""  # Describes the tile
    img = na.ground_top(SALT_DK, SALT_SH, SALT, SALT_RIDGE, [(4, 6, BRINE), (11, 6, SALT_RIDGE), (8, 7, SALT_SH)])  # Salt cap and specks
    hex_ridges(img, SALT_RIDGE)  # Honeycomb ridges on the crust
    return img  # The finished tile


def saltflat_soil():
    """A 16x16 tile of layered salt with thin mud and blue brine lines."""  # Describes the tile
    layers = [(x, 3 + la.wave(x, 1), SALT_DK) for x in range(16)]  # A shaded salt layer
    layers += [(x, 7, MUD) for x in range(16) if x % 6]  # A thin mud line
    layers += [(x, 8, SALT) for x in range(16)]  # A bright salt layer under the mud
    layers += [(x, 12 + la.wave(x, 1), BRINE) for x in range(16) if x % 4 != 1]  # A thin brine line
    layers += [(x, 14, SALT_RIDGE) for x in range(16) if x % 5]  # A grey layer near the bottom
    return na.soil_tile(SALT_SH, SALT_RIDGE, layers)  # Salt with its layers


def ledge_saltflat():
    """A 16x10 ledge of cut salt blocks with joints."""  # Describes the tile
    flecks = [(x, y, SALT_RIDGE) for x in (0, 8) for y in range(2, 8)]  # Joints between the blocks
    flecks += [(x, 5, SALT_DK) for x in range(16) if x % 8]  # A layer line in the blocks
    flecks += [(4, 3, SALT), (12, 6, MUD)]  # Bright and muddy bits
    return na.ledge_tile(SALT_SH, SALT, SALT_RIDGE, flecks)  # Salt blocks with a white top


def salt_block(img, x0, y0, w, h):
    """Paints one cut salt block: a white top, a pale face with layer lines and a shaded right side."""  # Describes the helper
    for y in range(y0, y0 + h):  # Each row of the block
        for x in range(x0, x0 + w):  # Each column of the block
            col = SALT_SH  # Pale face
            if y < y0 + 2:  # The top of the block
                col = SALT  # White top
            elif x >= x0 + w - 3:  # The right side
                col = SALT_DK  # Shaded side
            if y > y0 + 2 and (y - y0) % 4 == 0 and x < x0 + w - 3:  # Layer lines on the face
                col = SALT_RIDGE if (x + y) % 5 else SALT_DK  # Grey layer line
            if lm.noise(x, y) < 3:  # A few grey specks
                col = SALT_RIDGE  # Speck
            la.put(img, x, y, col)  # Paint it


def salt_boulder_small():
    """A 32x24 single white-grey salt block."""  # Describes the boulder
    img = mm.new_image(32, 24)  # Empty picture
    salt_block(img, 1, 6, 30, 17)  # One block
    return la.outline(img)  # Dark edge


def salt_boulder_big():
    """A 48x36 pile of three white-grey salt blocks: two on the ground and one on top."""  # Describes the boulder
    img = mm.new_image(48, 36)  # Empty picture
    salt_block(img, 1, 16, 23, 19)  # Left bottom block
    salt_block(img, 25, 19, 22, 16)  # Right bottom block
    mm.fill_rect(img, 24, 19, 1, 16, mm.MASK_SH)  # Seam between the two bottom blocks
    salt_block(img, 7, 4, 26, 12)  # Top block resting on the left block
    mm.fill_rect(img, 7, 16, 18, 1, mm.MASK_SH)  # Seam under the top block
    return la.outline(img)  # Dark edge


def salt_hotel():
    """A 56x48 small house built of white salt blocks, with a straw roof, a door and two windows (the N5 goal)."""  # Describes the goal
    img = mm.new_image(56, 48)  # Empty picture
    for y in range(20, 47):  # Rows of the salt-block walls
        for x in range(6, 50):  # Columns of the walls
            row = (y - 20) // 4  # Course of blocks
            joint = (y - 20) % 4 == 3 or (x - 6 + (row % 2) * 5) % 10 == 0  # Joint between the blocks
            col = SALT_RIDGE if joint else (SALT_DK if x >= 46 else SALT_SH)  # Joint, shaded end or pale block
            if not joint and (y - 20) % 4 == 0:  # Top row of each block
                col = SALT if x < 46 else SALT_SH  # Sunny top edge of the block
            la.put(img, x, y, col)  # Paint it
    ls.fill_poly(img, [(10, 6), (46, 6), (54, 21), (2, 21)], THATCH)  # Straw roof
    for x in range(2, 55, 3):  # Straw lines down the roof
        for y in range(7, 21):  # Rows of the roof
            if ls.inside(x + 0.5, y + 0.5, [(10, 6), (46, 6), (54, 21), (2, 21)]) and (x + y // 2) % 3 == 0:  # On the roof
                la.put(img, x, y, THATCH_SH)  # Darker straw line
    mm.fill_rect(img, 2, 20, 53, 1, THATCH_SH)  # Shaded roof edge
    mm.fill_rect(img, 10, 6, 36, 1, mix(mm.VEST, mm.HEADLAMP))  # Sunny roof ridge
    mm.fill_rect(img, 24, 34, 8, 13, mm.OUTLINE)  # Doorway
    mm.fill_rect(img, 25, 33, 6, 1, mm.OUTLINE)  # Round top of the doorway
    for x0 in (12, 38):  # Two small windows
        mm.fill_rect(img, x0, 27, 6, 5, mm.OUTLINE)  # Dark window
        mm.fill_rect(img, x0 + 1, 28, 2, 2, BRINE)  # Sky seen in the glass
    return la.outline(img)  # Dark edge


def salt_crust_top():
    """A 16x8 slippery salt crust top: shiny wet white salt with a blue sheen and glints (used like ice_top)."""  # Describes the tile
    img = mm.new_image(16, 8)  # Empty tile
    mm.fill_rect(img, 0, 0, 16, 8, SALT_SH)  # Salt body
    mm.fill_rect(img, 0, 0, 16, 4, SALT_SHEEN)  # Shiny wet crust
    mm.fill_rect(img, 0, 0, 16, 1, mm.WHITE)  # Bright top line
    hex_ridges(img, mm.SKY_LIGHT)  # Faint honeycomb under the wet sheen
    for (x, y) in ((2, 2), (3, 2), (9, 1), (10, 3), (14, 2)):  # Blue streaks of wet shine
        la.put(img, x, y, BRINE)  # One shine pixel
    for (x, y) in ((5, 1), (12, 2)):  # Glints
        la.put(img, x, y, mm.WHITE)  # One glint
    mm.fill_rect(img, 0, 4, 16, 1, lm.HAZE)  # Cool blue shadow line, like ice
    for (x, y) in ((3, 6), (11, 5), (7, 7)):  # Specks in the salt
        la.put(img, x, y, SALT_DK)  # One speck
    return img  # The finished tile


# ---------- Moving cable car (48x48) and its cable (16x3) ----------

def cable_car():
    """A 48x48 red cable car. The cabin is the bottom 24 rows and its flat roof is row 24, where the explorer stands.
    A thin hanger arm rises from the roof near the left end to the top row, where it grips the cable."""  # Describes the car
    img = mm.new_image(48, 48)  # Empty picture
    cab = mm.new_image(48, 24)  # The cabin alone, roof on its row 0
    mm.fill_rect(cab, 1, 0, 46, 22, CAR_RED)  # Cabin body
    mm.fill_rect(cab, 1, 0, 46, 2, WALL)  # Flat light roof the explorer stands on
    mm.fill_rect(cab, 1, 0, 46, 1, mm.WHITE)  # Bright roof top line
    mm.fill_rect(cab, 1, 2, 46, 1, STEEL)  # Roof rim
    mm.fill_rect(cab, 41, 3, 6, 19, CAR_RED_SH)  # Shaded right end of the cabin
    for x0 in (4, 15, 26):  # Three windows
        mm.fill_rect(cab, x0, 5, 9, 8, GLASS)  # Glass
        mm.fill_rect(cab, x0 + 6, 5, 3, 8, GLASS_SH)  # Glass in shade
        la.put(cab, x0 + 1, 6, mm.WHITE)  # Glint on the glass
    mm.fill_rect(cab, 37, 5, 3, 8, GLASS_SH)  # Small end window
    mm.fill_rect(cab, 1, 15, 46, 1, mix(mm.SCARF, mm.HEADLAMP))  # Light stripe along the cabin
    mm.fill_rect(cab, 1, 20, 46, 2, CAR_RED_SH)  # Floor edge in shade
    mm.fill_rect(cab, 3, 22, 42, 1, STEEL)  # Floor frame under the cabin
    cab = la.outline(cab)  # Dark edge round the cabin, nothing above the roof
    mm.paste(img, cab, 0, 24)  # The cabin in the bottom 24 rows
    mm.fill_rect(img, 7, 3, 2, 21, STEEL)  # Hanger arm from the roof up
    mm.fill_rect(img, 7, 3, 1, 21, STEEL_LT)  # Light edge of the arm
    mm.fill_rect(img, 5, 22, 6, 2, STEEL)  # Foot of the arm on the roof
    mm.fill_rect(img, 4, 2, 8, 2, STEEL)  # Grip under the cable
    mm.fill_rect(img, 4, 0, 2, 2, STEEL)  # Left jaw of the grip
    mm.fill_rect(img, 10, 0, 2, 2, STEEL)  # Right jaw of the grip
    la.put(img, 7, 2, mm.HELMET)  # Yellow bolt on the grip
    return img  # The finished picture


def cable_line():
    """A 16x3 steel cable that tiles across: a twisted light line, the steel, and a dark lower edge."""  # Describes the cable
    img = mm.new_image(16, 3)  # Empty picture
    for x in range(16):  # Each column
        la.put(img, x, 0, STEEL_LT if x % 4 in (0, 1) else STEEL)  # Twisted strands catch the light
        la.put(img, x, 1, STEEL if x % 4 != 2 else STEEL_LT)  # Middle of the cable
        la.put(img, x, 2, mm.OUTLINE)  # Dark lower edge
    return img  # The finished picture


# ---------- Llama herd (32x16, facing right, feet on the bottom row) ----------

LLAMA_LEGS = [  # Leg positions for the three walk frames, drawn facing left: (x, bottom row) of each leg
    ((9, 15), (13, 15), (23, 15), (27, 15)),  # Frame 0: all feet down
    ((8, 14), (13, 15), (24, 15), (27, 14)),  # Frame 1: two legs lifted
    ((10, 15), (12, 14), (22, 14), (26, 15)),  # Frame 2: the other two legs lifted
]


def llama(frame):
    """A 32x16 cream llama with a brown patch, a long upright neck, tall ears and a red ear tassel, walking right."""  # Describes the animal
    img = mm.new_image(32, 16)  # Empty picture, drawn facing left first
    for x, bottom in LLAMA_LEGS[frame]:  # Each leg
        mm.fill_rect(img, x, 9, 2, bottom - 8, LLAMA_SH)  # The long leg from the body down
        mm.fill_rect(img, x, bottom, 2, 1, mm.STRAP)  # The foot on its bottom row
    ph.ell(img, 18, 7, 10, 3, LLAMA, LLAMA_SH)  # Woolly body
    ph.ell(img, 22, 6, 3, 2, LLAMA_PATCH)  # Brown patch on the back
    mm.fill_rect(img, 6, 2, 4, 7, LLAMA)  # Long upright neck
    mm.fill_rect(img, 9, 3, 1, 6, LLAMA_SH)  # Back of the neck in shade
    ph.ell(img, 6, 2, 3, 1.5, LLAMA)  # Head
    mm.fill_rect(img, 2, 2, 2, 2, LLAMA_PATCH)  # Brown muzzle
    la.put(img, 6, 0, LLAMA)  # Front ear
    la.put(img, 8, 0, LLAMA_SH)  # Back ear
    la.put(img, 7, 0, LLAMA_TASSEL)  # Red wool tassel between the ears
    la.put(img, 5, 2, mm.OUTLINE)  # Eye
    mm.fill_rect(img, 28, 4, 2, 3, LLAMA_SH)  # Short fluffy tail
    img = la.outline(img)  # Dark edge
    return img.transpose(Image.FLIP_LEFT_RIGHT)  # Turn it to face right


# ---------- Props (decoration) ----------

def fynbos(frame):
    """A 12x12 fynbos shrub of fine grey-green leaves with a pink protea flower; frame 1 sways the tips right."""  # Describes the prop
    img = mm.new_image(12, 12)  # Empty picture
    for (tx, ty) in ((1, 4), (3, 2), (6, 1), (9, 3), (11, 6), (4, 6), (8, 6)):  # Tips of the stems
        steps = 10  # Points along each stem
        for t in range(steps + 1):  # From the base to the tip
            x = round(6 + (tx - 6) * t / steps) + (frame if t > 6 else 0)  # Column of this stem pixel, tip sways
            y = round(11 + (ty - 11) * t / steps)  # Row of this stem pixel
            la.put(img, x, y, FYN_SH if t < 4 else (FYN_LT if t % 3 == 0 else FYN))  # Darker base, light needles
    for (x, y) in ((5, 1), (6, 1), (7, 1), (5, 2), (6, 2), (7, 2)):  # The protea cup
        la.put(img, x + frame, y, PROTEA if y == 1 else PROTEA_SH)  # Pink petals
    la.put(img, 6 + frame, 0, mm.HEADLAMP)  # Pale tip in the middle of the flower
    return img  # No outline, like the other plants


def dassie(frame):
    """A 12x8 grey-brown rock hyrax (dassie) sitting on its belly; frame 1 lifts its head to sniff."""  # Describes the prop
    img = mm.new_image(12, 8)  # Empty picture
    lift = frame  # How far the head rises
    ph.ell(img, 5, 4.5, 4, 2.5, DASSIE, DASSIE_SH)  # Round body
    mm.fill_rect(img, 3, 6, 4, 1, DASSIE_BELLY)  # Pale belly
    ph.ell(img, 9, 3.5 - lift, 2, 1.6, DASSIE, DASSIE_SH)  # Head
    la.put(img, 8, 2 - lift, DASSIE_SH)  # Small round ear
    la.put(img, 10, 3 - lift, mm.OUTLINE)  # Eye
    la.put(img, 11, 4 - lift, mm.OUTLINE)  # Nose
    mm.fill_rect(img, 2, 7, 2, 1, DASSIE_SH)  # Back foot
    mm.fill_rect(img, 7, 7, 2, 1, DASSIE_SH)  # Front foot
    return img  # Dark eye and feet show the shape


def sagebrush():
    """A 12x10 round grey-green sagebrush with a short brown stem."""  # Describes the prop
    img = mm.new_image(12, 10)  # Empty picture
    mm.fill_rect(img, 5, 7, 2, 3, WOOD_SH)  # Short woody stem
    for (cx, cy, r, col) in ((3, 6, 2, SAGE_SH), (8, 6, 2, SAGE_SH), (6, 4, 3, SAGE), (3, 4, 2, SAGE), (9, 4, 2, SAGE)):  # Leafy clumps
        ls.disc(img, cx, cy, r, col)  # One clump
    for (x, y) in ((4, 2), (6, 1), (8, 2), (2, 3), (10, 3)):  # Sunny leaf tips
        la.put(img, x, y, SAGE_LT)  # One light tip
    for (x, y) in ((5, 6), (8, 7), (3, 7)):  # Dark gaps in the bush
        la.put(img, x, y, FYN_SH)  # One dark gap
    return img  # No outline, like the other plants


def pinyon_pine():
    """A 16x24 short pinyon pine with a round lumpy crown and a twisted trunk."""  # Describes the prop
    img = mm.new_image(16, 24)  # Empty picture
    for (y, x) in ((23, 7), (22, 7), (21, 6), (20, 6), (19, 7), (18, 8), (17, 8), (16, 7), (15, 7), (14, 6)):  # Twisted trunk
        mm.fill_rect(img, x, y, 2, 1, TRUNK)  # Trunk pixel pair
        la.put(img, x + 1, y, TRUNK_SH)  # Shaded side of the trunk
    mm.fill_rect(img, 5, 23, 6, 1, TRUNK_SH)  # Roots spreading at the foot
    ph.line(img, 8, 16, 12, 13, TRUNK)  # A side branch
    for (cx, cy, r, col) in ((5, 10, 4, PINYON_SH), (11, 11, 3, PINYON_SH), (8, 6, 5, PINYON), (4, 8, 3, PINYON),
                             (12, 7, 3, PINYON), (8, 13, 3, PINYON_SH)):  # Lumps of needles
        ls.disc(img, cx, cy, r, col)  # One lump
    for (x, y) in ((6, 2), (7, 2), (5, 3), (9, 3), (3, 6), (11, 5), (4, 7), (8, 4)):  # Sunny needles on the top left
        la.put(img, x, y, PINYON_LT)  # One light needle
    return la.outline(img)  # Dark edge so the dark tree reads on rock


def raven(frame):
    """A 12x8 black raven flying right; frame 0 has its wings up, frame 1 has its wings down."""  # Describes the prop
    img = mm.new_image(12, 8)  # Empty picture
    mm.fill_rect(img, 3, 4, 6, 2, RAVEN)  # Body
    mm.fill_rect(img, 9, 3, 2, 2, RAVEN)  # Head
    la.put(img, 11, 4, RAVEN_LT)  # Thick beak
    la.put(img, 10, 3, RAVEN_LT)  # Shine on the head
    for (x, y) in ((0, 3), (1, 4), (2, 4), (0, 5), (1, 5), (2, 5), (1, 6)):  # Wedge-shaped tail
        la.put(img, x, y, RAVEN)  # Tail pixel
    if frame == 0:  # Wings up
        wing = ((4, 3), (5, 3), (6, 3), (5, 2), (6, 2), (4, 1), (5, 1), (3, 0), (4, 0))  # Wing pixels
    else:  # Wings down
        wing = ((4, 6), (5, 6), (6, 6), (4, 7), (5, 7), (3, 7))  # Wing pixels
    for (x, y) in wing:  # Each wing pixel
        la.put(img, x, y, RAVEN)  # Black wing
    la.put(img, 5, 4, RAVEN_LT)  # Feather shine on the back
    return img  # Dark bird needs no outline


def pitcher_plant_tepui():
    """A 10x12 clump of three tall tube-shaped pitcher plants, green with red lips, like those on Roraima."""  # Describes the prop
    img = mm.new_image(10, 12)  # Empty picture
    for (x0, top, w) in ((0, 4, 3), (3, 1, 3), (7, 5, 3)):  # Tubes: left column, top row and width
        for y in range(top, 12):  # Rows of the tube
            for x in range(x0, x0 + w):  # Columns of the tube
                col = PITCHER_SH if x == x0 + w - 1 else PITCHER  # Shaded right side
                if y <= top + 1:  # The lip at the top
                    col = PITCHER_RIM if y == top else PITCHER_RED  # Red lip and rim
                la.put(img, x, y, col)  # Paint it
        la.put(img, x0 + 1, top + 1, mm.OUTLINE)  # Dark mouth of the tube
    for x in range(10):  # Moss at the foot
        la.put(img, x, 11, MOSS if x % 3 else MOSS_LT)  # One moss pixel
    return img  # No outline, like the other plants


def tepui_mist(frame):
    """A 16x12 soft band of white mist with long thin wisps; frame 1 drifts right and thins (decoration only)."""  # Describes the prop
    img = mm.new_image(16, 12)  # Empty picture
    for (cx, cy, rx, ry) in ((6, 7, 6, 2.5), (11, 4, 4, 1.6), (4, 10, 4, 1.2)):  # Wisps: middle x, middle y, half width, half height
        cx += frame  # Frame 1 drifts right
        for y in range(12):  # Each row
            for x in range(16):  # Each pixel
                d = ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2  # Distance from the middle, 1 at the edge
                if d <= 1.0:  # Inside the wisp
                    inner = d < 0.45  # The thick middle
                    la.put(img, x, y, MIST if inner else MIST_EDGE, (200 if inner else 140) - frame * 20)  # Soft white, see-through
    return img  # No outline, mist is soft


def flamingo(frame):
    """A 12x16 pink flamingo facing right on long thin legs; frame 0 stands on one leg, frame 1 has its head down to feed."""  # Describes the prop
    img = mm.new_image(12, 16)  # Empty picture
    ph.ell(img, 5, 8, 3.5, 2, FLAMINGO, FLAMINGO_SH)  # Body
    mm.fill_rect(img, 1, 7, 2, 1, FLAMINGO_SH)  # Tail feathers
    la.put(img, 3, 9, mm.OUTLINE)  # Dark wing tip
    la.put(img, 4, 9, mm.OUTLINE)  # Dark wing tip
    if frame == 0:  # Head up on an S-curved neck
        neck = ((8, 7), (9, 6), (9, 5), (8, 4), (8, 3), (9, 2))  # Neck pixels
        head = ((9, 1), (10, 1), (10, 2))  # Head pixels
        beak = ((11, 2), (11, 3))  # Beak: pale then dark tip
    else:  # Head down to the water
        neck = ((8, 8), (9, 9), (10, 10), (10, 11), (10, 12))  # Neck pixels
        head = ((10, 13), (11, 13))  # Head pixels
        beak = ((11, 14), (10, 14))  # Beak: pale then dark tip
    for (x, y) in neck + head:  # Neck and head
        la.put(img, x, y, FLAMINGO)  # Pink feather
    la.put(img, beak[0][0], beak[0][1], mm.HEADLAMP)  # Pale beak
    la.put(img, beak[1][0], beak[1][1], mm.OUTLINE)  # Dark beak tip
    la.put(img, head[0][0], head[0][1], mm.OUTLINE if frame == 0 else FLAMINGO_SH)  # Eye
    for y in range(10, 16):  # The standing leg
        la.put(img, 5, y, FLAMINGO_LEG)  # One leg pixel
    if frame == 0:  # Second leg tucked up
        for (x, y) in ((6, 10), (7, 11), (6, 12)):  # Bent leg
            la.put(img, x, y, FLAMINGO_LEG)  # One leg pixel
    else:  # Both legs down
        for y in range(10, 16):  # The second leg
            la.put(img, 7, y, FLAMINGO_LEG)  # One leg pixel
    return img  # Thin legs stay clean without an outline


def salt_cone():
    """A 16x8 small cone-shaped heap of raked salt, white with a shaded right side."""  # Describes the prop
    img = mm.new_image(16, 8)  # Empty picture
    for y in range(1, 8):  # Rows of the heap
        half = y  # The heap gets wider going down
        for x in range(16):  # Each column
            dx = x - 7.5  # Distance from the middle
            if abs(dx) <= half:  # Inside the heap
                la.put(img, x, y, SALT if dx < 0 else (SALT_DK if dx > half - 2 else SALT_SH))  # Lit left, shaded right
    for (x, y) in ((6, 4), (5, 6), (9, 5)):  # A few grey grains
        la.put(img, x, y, SALT_RIDGE)  # One grain
    return la.outline(img)  # Dark edge so white salt reads on white ground


# ---------- Item (24x24, Assets/Art/Items/Sprites/) ----------

def ulexite():
    """A 24x24 block of white fibrous ulexite with silky see-through stripes; a dark line shows through its top."""  # Describes the item
    def c(x, y, s):  # Colour of one pixel
        silk = x % 3 == 1  # Every third column is a silky fibre
        if s:  # Lower right, in shade
            return ULEX_SH_SILK if silk else ULEX_SH  # Shaded fibres
        return ULEX_SILK if silk else ULEX  # White and silky fibres
    img = na.blob(24, 24, 11.5, 12.5, 9.5, 9.0, c, 3.2)  # Squarish fibrous block
    for x in range(5, 16):  # Columns of the line seen through the top
        la.put(img, x, 7 + la.wave(x, 1, 10), ULEX_SEEN)  # The line, a little wavy as if through the fibres
    for (x, y) in ((6, 5), (6, 6), (6, 10), (9, 9)):  # Bright shine on the fibre ends
        la.put(img, x, y, mm.WHITE)  # Shine pixel
    return ls.outline(img)  # Dark edge


# ---------- Lesson pictures (120x82, Assets/Art/Lesson/Sprites/) ----------

TABLE_TOP, TABLE_L, TABLE_R = 26, 12, 104  # Flat top row and the top corners of Table Mountain
CLOTH_L, CLOTH_R = 26, 90  # Left and right ends of the cloud cloth on the top
CLIFF_FOOT = 40  # Row where the steep cliff ends and the green slope starts
CITY, SEA = 56, 64  # Row of the city foot and row of the sea


def table_pixel(x, y):
    """Colour of one pixel of the Table Mountain picture: sky, a long dead-flat mountain top with steep cliffs,
    a thin white cloud cloth lying on the top, a small city and the sea."""  # Describes the picture
    if y >= SEA:  # The sea
        if y % 4 == 1 and (x + y * 3) % 13 < 4:  # Soft wave dashes
            return mm.SKY_LIGHT  # Pale dash
        return lm.LENS if y < SEA + 8 else mix(lm.LENS, lm.MOUNTAIN_SH)  # Blue sea, deeper in front
    if y >= CITY - 1 - (lm.noise(x // 3, 1) % 7) and y < CITY + 6:  # A city block of this height
        if y >= CITY + 4:  # Shore road and beach
            return mm.VEST if y == CITY + 5 else mm.MASK_SH  # Road and sand
        if (x // 3) % 4 == 3:  # A gap between buildings
            return mm.HILLS_SH  # Trees between the buildings
        if x % 3 == 1 and y % 2 == 0 and y < CITY + 3:  # Windows in neat rows
            return mm.STRAP  # Dark window
        return (mm.WHITE, mm.MASK, mm.HEADLAMP)[(x // 3) % 3]  # White, grey and cream walls
    if TABLE_TOP - 3 <= y < TABLE_TOP:  # Rows of the cloud lying on the top
        inset = 2 if y == TABLE_TOP - 3 else 0  # The top row is a little shorter, so the ends are round
        if CLOTH_L + inset <= x <= CLOTH_R - inset:  # Inside the cloud band
            return mm.WHITE  # White cloud
    if TABLE_TOP <= y <= TABLE_TOP + 2 and CLOTH_L <= x <= CLOTH_R:  # The cloud spilling over the front edge
        drip = 30 <= x <= 40 or 58 <= x <= 70 or 80 <= x <= 86  # A few smooth places where it hangs lower
        if y < TABLE_TOP + 1 or (drip and y < TABLE_TOP + 2):  # The thin white spill
            return mm.WHITE  # White cloud
        if y == TABLE_TOP + 1 or drip:  # The soft lower edge of the spill
            return lm.SNOW  # Pale cloud edge
    spread = (y - TABLE_TOP) * 0.25 + max(0, y - CLIFF_FOOT) * 1.6  # Steep cliffs, then a wider green slope
    if y >= TABLE_TOP and TABLE_L - spread <= x <= TABLE_R + spread:  # Inside Table Mountain
        if y >= CLIFF_FOOT:  # Green lower slopes
            return FYN if lm.noise(x, y) > 25 else FYN_SH  # Fynbos on the slopes
        shaded = x > 84  # The right end of the cliff is in shade
        if y == TABLE_TOP:  # The dead-flat top edge
            return TS_LT  # Sunny top line
        if (y - TABLE_TOP) % 4 == 0 and x % 9 != 4:  # Flat bed lines across the cliff
            return TS_DK if shaded else TS_SH  # Bed line
        if x % 11 == 0 and y > TABLE_TOP + 4:  # A few thin upright cracks
            return TS_SH if not shaded else TS_DK  # Crack
        return TS_SH if shaded else TS  # Sandstone cliff, shaded on the right
    return na.sky(x, y)  # Sky


def lesson_table_mountain():
    """A 120x82 framed picture of a flat-topped mountain over a city by the sea, with a white cloud 'tablecloth'."""  # Describes the lesson
    return ls.framed(table_pixel)  # Sky, mountain, cloud, city and sea


GROUND_Y = 54  # Row where the desert floor starts
BUTTES = [(0, 26, 34, 46, True), (36, 48, 20, 44, False), (50, 52, 27, 44, False),
          (66, 68, 28, 44, False), (70, 82, 22, 44, False), (96, 117, 32, 47, True)]  # Buttes: left, right, top, cliff foot, far


def monument_pixel(x, y):
    """Colour of one pixel of the Monument Valley picture: sky, red mesas and buttes with slopes of rubble, and sand."""  # Describes the picture
    if y >= GROUND_Y:  # The sandy desert floor
        if (x * 2 + y * 5) % 23 == 0:  # A few sagebrush dots
            return SAGE_SH  # Dark sage
        if (y + la.wave(x, 1, 20)) % 6 == 0:  # Ripples in the sand
            return MESA  # Darker ripple
        return SAND if y < GROUND_Y + 10 else SAND_LT  # Orange sand, sunnier in front
    for (x0, x1, top, foot, far) in reversed(BUTTES):  # Each butte, nearest first
        if top <= y < foot and x0 <= x <= x1:  # The sheer cliff
            if far:  # A far mesa in the haze
                return mix(mm.PLATEAU, lm.HAZE) if x < (x0 + x1) // 2 + 4 else mix(mm.PLATEAU_SH, lm.HAZE)  # Hazy red
            if y < top + 8 and lm.noise(x, 1) < 30:  # Dark varnish streaks below the top
                return VARNISH  # Dark streak
            return MESA if x < x1 - 2 else MESA_SH  # Red cliff, shaded on the right
        spread = (y - foot) * 1.3  # The rubble slope widens going down
        if foot <= y < GROUND_Y and x0 - spread <= x <= x1 + spread:  # The rubble slope
            return mix(mm.PLATEAU, mm.VEST) if x < (x0 + x1) // 2 else MESA_SH  # Lit or shaded slope
    return na.sky(x, y)  # Sky


def lesson_monument_valley():
    """A 120x82 framed picture of red mesas and buttes standing on a flat sandy desert."""  # Describes the lesson
    img = ls.framed(monument_pixel)  # Sky, buttes and sand
    mm.paste(img, sagebrush(), 8, 64)  # A sagebrush in front, left
    mm.paste(img, sagebrush(), 96, 68)  # A sagebrush in front, right
    return img  # The finished picture


GC_BANDS = [KAIBAB, BUFF, BUFF, RED_LT, RED, RED, GREY, RED_LT, RED, RED_DK]  # Rock bands of the canyon walls, top to bottom
GC_FAR = [mix(mm.MASK, lm.HAZE), mix(mm.VEST, lm.HAZE), mix(mm.VEST, lm.HAZE), mix(mm.PLATEAU, lm.HAZE),
          mix(mm.PLATEAU_SH, lm.HAZE), mix(mm.PLATEAU_SH, lm.HAZE), mix(mm.MASK_SH, lm.HAZE), mix(mm.PLATEAU, lm.HAZE),
          mix(mm.PLATEAU_SH, lm.HAZE), mix(mm.BACKPACK_SH, lm.HAZE)]  # The same bands in the haze, for the far wall


def canyon_pixel(x, y):
    """Colour of one pixel of the Grand Canyon picture: sky, a far banded wall, near stepped walls and a river at the bottom."""  # Describes the picture
    if y < 16:  # Sky
        return na.sky(x, y)  # Sky colour
    band = min(9, max(0, (y - 16 + la.wave(x, 1, 24)) // 5))  # Which rock band this row is in
    step_l = 4 * ((y // 7) % 2)  # Small steps in the near left wall
    left = 6 + (y - 24) * 0.95 + step_l  # Near left wall edge, going down to the river
    right = 112 - (y - 24) * 0.85 - 4 * (((y + 3) // 7) % 2)  # Near right wall edge
    if y >= 24 and (x < left or x > right):  # A near wall
        if y == 24 or (y < 27 and lm.noise(x, y) < 20):  # The rim at the top of the near wall
            return KAIBAB_LT  # Pale rim
        col = GC_BANDS[min(9, (y - 24) // 6)]  # Near band, thicker
        if lm.noise(x, y) < 5:  # A few dark specks
            return RED_DK  # Speck
        if x > right and col in (RED, RED_LT):  # The right wall is in shade
            return RED_DK if col == RED else RED  # Shaded red
        return col  # Lit near band
    if y >= 66:  # The river at the bottom of the canyon
        return mm.SKY_LIGHT if y == 70 and x % 5 == 0 else RIVER  # Green-blue river with a few glints
    if y < 18:  # The flat far rim
        return mix(mm.MASK, lm.HAZE)  # Hazy pale rim
    if y >= 52 and abs(x - 60) < 14 - (y - 52) // 2:  # A temple butte in the middle of the canyon
        return mix(mm.PLATEAU_SH, lm.HAZE_SH)  # Hazy red butte
    return GC_FAR[band]  # The far wall band


def lesson_grand_canyon():
    """A 120x82 framed picture of a deep canyon with layered rock walls and a river at the bottom."""  # Describes the lesson
    img = ls.framed(canyon_pixel)  # Sky, walls and river
    mm.paste(img, pinyon_pine(), 2, 3)  # A pinyon pine on the near rim
    return img  # The finished picture


TEPUI_TOP, TEPUI_L, TEPUI_R = 18, 16, 102  # Flat top row and the top corners of Roraima
JUNGLE = 56  # Row where the jungle starts
FALLS_X = (38, 74)  # Columns of the two waterfalls


def roraima_pixel(x, y):
    """Colour of one pixel of the Roraima picture: sky, a sheer-walled table-top mountain with waterfalls, mist and jungle."""  # Describes the picture
    jungle_top = JUNGLE + round(2 * math.sin(x / 3.0)) + (lm.noise(x // 4, 2) % 3)  # Bumpy tree crowns
    if y >= jungle_top:  # The jungle
        n = lm.noise(x, y)  # Fixed random number for this pixel
        if n < 30:  # Shade inside the crowns
            return mix(mm.LEAF, mm.OUTLINE)  # Deep shade
        return mm.LEAF if n < 70 else mm.HILLS_SH  # Dark and sunny leaves
    for fx in FALLS_X:  # Spray where each waterfall meets the jungle
        if (x - fx) ** 2 + 3 * (y - JUNGLE + 1) ** 2 < 30 and y <= JUNGLE + 1:  # Inside the spray cloud
            return mm.WHITE if (x + y) % 3 else lm.SNOW  # White spray
    if 40 <= y <= 44 and (x < 30 or x > 86) and lm.noise(x, y) < 70:  # A band of mist on the walls
        return mm.WHITE if y in (41, 42) else lm.SNOW  # White mist
    left = TEPUI_L - (y - TEPUI_TOP) * 0.15  # Left wall, almost upright
    right = TEPUI_R + (y - TEPUI_TOP) * 0.15  # Right wall, almost upright
    if y >= TEPUI_TOP and left <= x <= right:  # Inside the tepui
        for fx in FALLS_X:  # Each waterfall
            if fx <= x <= fx + 1 and y > TEPUI_TOP + 1:  # The falling water
                return mm.WHITE if (y + x) % 4 else mm.SKY_LIGHT  # White water with blue streaks
        if y < TEPUI_TOP + 2:  # The top edge with plants
            return MOSS if lm.noise(x, y) < 50 else TEP  # Green and black top
        if x % 5 == 0:  # Upright cracks in the wall
            return TEP_SH  # Dark crack
        if lm.noise(x, y) < 10:  # Pink patches of rock
            return TEP_PINK_DK  # Rose rock
        return TEP_LT if x < 60 else TEP  # Grey lit wall, black in shade
    if y < 14 and (x - 20) ** 2 // 6 + (y - 8) ** 2 < 18:  # A white cloud in the sky
        return mm.WHITE  # Cloud
    return na.sky(x, y)  # Sky


def lesson_roraima():
    """A 120x82 framed picture of a sheer-walled table-top mountain above jungle, with waterfalls and mist."""  # Describes the lesson
    return ls.framed(roraima_pixel)  # Sky, tepui, falls and jungle


HORIZON = 38  # Row where the salt flat meets the sky
VOLCANOES = [(18, 26, 0.7), (40, 31, 0.9), (92, 24, 0.6), (112, 30, 0.8)]  # Far volcanoes: x, top y, slope
DRY = 58  # Row where the dry hexagon crust starts in front


def uyuni_above(x, y):
    """Colour above the horizon: sky and far volcanoes with snowy tops."""  # Describes the helper
    top, peak = lm.ridge(x, VOLCANOES)  # The far volcanoes
    if y >= top:  # Inside a volcano
        if y < top + 3 and peak[1] < 28:  # Snow on the tallest tops
            return lm.SNOW  # Snow
        return lm.HAZE if x <= peak[0] else lm.HAZE_SH  # Lit or shaded haze
    return na.sky(x, y)  # Sky


def uyuni_pixel(x, y):
    """Colour of one pixel of the Uyuni picture: sky and volcanoes, the wet salt flat mirroring them,
    and a dry white hexagon crust in front."""  # Describes the picture
    if y < HORIZON:  # Above the horizon
        return uyuni_above(x, y)  # Sky and volcanoes
    if y == HORIZON:  # The thin line where the flat meets the sky
        return mm.WHITE  # Bright line
    if y < DRY:  # The wet mirror
        col = uyuni_above(x, 2 * HORIZON - y)  # The same scene, upside down
        if y % 5 == 0 and (x + y) % 11 < 3:  # Tiny ripples on the water film
            return mm.WHITE  # Ripple
        return col  # The mirror picture
    depth = y - DRY  # Rows into the dry crust
    cell = 3 + depth // 4  # Hexagon cells get bigger toward the front
    v = (depth % cell) / float(cell)  # Place down inside one row of cells
    row = depth // cell  # Which row of cells
    u = ((x + row * cell) % (cell * 3)) / float(cell * 3)  # Place across one cell
    edge = abs(abs(u - 0.5) - (0.22 + 0.2 * abs(2 * v - 1)))  # Distance from the slanted cell side
    if depth % cell == 0 and abs(u - 0.5) < 0.24:  # Top edge of a cell
        return SALT_RIDGE  # Ridge
    if edge * cell * 3 < 0.8:  # Side of a cell
        return SALT_RIDGE  # Ridge
    return mm.WHITE if v < 0.5 else SALT_SH  # White crust, softly shaded


def lesson_uyuni():
    """A 120x82 framed picture of a white salt flat that mirrors the sky, with far volcanoes and a hexagon crust in front."""  # Describes the lesson
    img = ls.framed(uyuni_pixel)  # Sky, volcanoes, mirror and crust
    mm.paste(img, flamingo(0), 30, 40)  # A flamingo standing in the shallow water
    return img  # The finished picture


# ---------- Saving and the preview sheet ----------

def put_file(where, name, img):
    """Saves one picture in the folder for its kind: item, lesson or level sprite."""  # Describes the helper
    if where == "item":  # Shared item picture
        ls.save(img, ls.ITEMS_OUT, name)  # Items folder
    elif where == "lesson":  # Lesson picture
        ls.save(img, ls.LESSON_OUT, name)  # Lesson folder
    else:  # Level sprite or prop
        la.save(img, name)  # Level sprites folder


def big(img, scale):
    """Makes a crisp, larger copy of a picture for the preview."""  # Describes the helper
    return img.resize((img.width * scale, img.height * scale), Image.NEAREST)  # Nearest-neighbour scaling


def preview(groups):
    """Lays the new sprites out at 3x size on one neutral sheet, one labelled group after another."""  # Describes the sheet
    scale, pad, width = 3, 6, 900  # Size up, gap and sheet width
    ops = []  # Each (picture, x, y) to paste
    y = pad  # Where the next row goes
    for title, sprites in groups:  # Each group
        label = mm.text_image(title, mm.DARK, 1)  # Group name in the pixel font
        ops.append((big(label, scale), pad, y))  # Label at the top of the group
        y += label.height * scale + pad  # Move below the label
        x, row_h = pad, 0  # Start a row
        for img in sprites:  # Each sprite in the group
            w, h = img.width * scale, img.height * scale  # Scaled size
            if x + w > width - pad:  # No room left in this row
                x, y, row_h = pad, y + row_h + pad, 0  # Start a new row
            ops.append((big(img, scale), x, y))  # Place the sprite
            x, row_h = x + w + pad, max(row_h, h)  # Move along
        y += row_h + 2 * pad  # Space before the next group
    sheet = Image.new("RGBA", (width, y + pad), mm.MASK + (255,))  # Neutral grey background
    for img, px, py in ops:  # Paste each one
        sheet.alpha_composite(img, (px, py))  # Place it
    os.makedirs(os.path.dirname(PREVIEW), exist_ok=True)  # Make the folder
    sheet.save(PREVIEW)  # Write the sheet
    print("Preview written to", PREVIEW)  # Tell where


def on_sky(img):
    """Puts a picture on a plain sky block, only for the preview, so pale things show up."""  # Describes the helper
    back = Image.new("RGBA", img.size, lm.SKY + (255,))  # Sky block the same size
    back.alpha_composite(img)  # The picture on top
    return back  # The picture on sky


def cable_check():
    """The cable car hanging from four cable tiles, on sky, only for the preview, to check the grip and the seams."""  # Describes the check
    img = mm.new_image(64, 48)  # Room for four cable tiles and the car
    for i in range(4):  # Four tiles side by side
        mm.paste(img, cable_line(), i * 16, 0)  # One cable tile on the top rows
    mm.paste(img, cable_car(), 8, 0)  # The car, its grip on the cable
    return on_sky(img)  # On sky


def tiled(tile, across, down):
    """Repeats a tile across and down, only for the preview, to check that it joins with no seam."""  # Describes the check
    img = mm.new_image(tile.width * across, tile.height * down)  # Room for the copies
    for j in range(down):  # Each row of copies
        for i in range(across):  # Each copy in the row
            mm.paste(img, tile, i * tile.width, j * tile.height)  # One copy
    return img  # The tiled picture


def ground_check(top, soil):
    """A strip of ground: a row of top tiles over two rows of soil tiles, only for the preview."""  # Describes the check
    img = mm.new_image(64, 40)  # Four tiles wide
    mm.paste(img, tiled(top, 4, 1), 0, 0)  # The top row
    mm.paste(img, tiled(soil, 4, 2), 0, 8)  # Two rows of soil
    return img  # The strip


def main():
    """Draws everything, saves each picture and writes the preview sheet."""  # Describes the main function
    groups = [  # Each group: its title, then (kind, file name, picture) for each picture
        ("N1 TABLE TOP", [
            ("level", "tabletop_top.png", tabletop_top()),  # Ground top
            ("level", "tabletop_soil.png", tabletop_soil()),  # Ground below the fynbos
            ("level", "ledge_tabletop.png", ledge_tabletop()),  # Sandstone ledge
            ("level", "boulder_small_tabletop.png", tabletop_boulder(32, 24)),  # Small sandstone boulder
            ("level", "boulder_big_tabletop.png", tabletop_boulder(48, 36)),  # Big sandstone boulder
            ("level", "cable_station.png", cable_station()),  # Goal: cable-car station
        ]),
        ("N2 MESA VALLEY", [
            ("level", "mesa_top.png", mesa_top()),  # Ground top
            ("level", "mesa_soil.png", mesa_soil()),  # Ground below the sand
            ("level", "ledge_mesa.png", ledge_mesa()),  # Sandstone ledge
            ("level", "boulder_small_mesa.png", mesa_boulder(32, 24)),  # Small red boulder
            ("level", "boulder_big_mesa.png", mesa_boulder(48, 36)),  # Big red boulder
            ("level", "trading_post.png", trading_post()),  # Goal: trading post
        ]),
        ("N3 CANYON RIM", [
            ("level", "canyon_top.png", canyon_top()),  # Ground top
            ("level", "canyon_soil.png", canyon_soil()),  # Banded rock below the rim
            ("level", "ledge_canyon.png", ledge_canyon()),  # Banded ledge
            ("level", "boulder_small_canyon.png", canyon_boulder(32, 24)),  # Small banded boulder
            ("level", "boulder_big_canyon.png", canyon_boulder(48, 36)),  # Big banded boulder
            ("level", "canyon_lookout.png", canyon_lookout()),  # Goal: stone lookout
        ]),
        ("N4 TEPUI TOP", [
            ("level", "tepui_top.png", tepui_top()),  # Ground top
            ("level", "tepui_soil.png", tepui_soil()),  # Black rock below
            ("level", "ledge_tepui.png", ledge_tepui()),  # Quartzite ledge
            ("level", "boulder_small_tepui.png", tepui_boulder(32, 24)),  # Small mossy boulder
            ("level", "boulder_big_tepui.png", tepui_boulder(48, 36)),  # Big mossy boulder
            ("level", "explorer_camp.png", explorer_camp()),  # Goal: tent and campfire
        ]),
        ("N5 SALT FLAT", [
            ("level", "saltflat_top.png", saltflat_top()),  # Ground top
            ("level", "saltflat_soil.png", saltflat_soil()),  # Layered salt below
            ("level", "ledge_saltflat.png", ledge_saltflat()),  # Salt block ledge
            ("level", "boulder_small_saltflat.png", salt_boulder_small()),  # One salt block
            ("level", "boulder_big_saltflat.png", salt_boulder_big()),  # Pile of salt blocks
            ("level", "salt_hotel.png", salt_hotel()),  # Goal: salt-block house
            ("level", "salt_crust_top.png", salt_crust_top()),  # Slippery salt crust top
        ]),
        ("CABLE CAR", [
            ("level", "cable_car.png", cable_car()),  # The moving cable car
            ("level", "cable_line.png", cable_line()),  # The cable, tiled across
        ]),
        ("LLAMA HERD", [
            ("level", "llama_00.png", llama(0)),  # Llama, frame 0
            ("level", "llama_01.png", llama(1)),  # Llama, frame 1
            ("level", "llama_02.png", llama(2)),  # Llama, frame 2
        ]),
        ("PLATEAUS NORMAL PROPS", [
            ("level", "Props/fynbos_00.png", fynbos(0)),  # Fynbos shrub
            ("level", "Props/fynbos_01.png", fynbos(1)),  # Fynbos swaying
            ("level", "Props/dassie_00.png", dassie(0)),  # Rock hyrax resting
            ("level", "Props/dassie_01.png", dassie(1)),  # Rock hyrax sniffing
            ("level", "Props/sagebrush.png", sagebrush()),  # Sagebrush
            ("level", "Props/pinyon_pine.png", pinyon_pine()),  # Pinyon pine
            ("level", "Props/raven_00.png", raven(0)),  # Raven, wings up
            ("level", "Props/raven_01.png", raven(1)),  # Raven, wings down
            ("level", "Props/pitcher_plant_tepui.png", pitcher_plant_tepui()),  # Tepui pitcher plants (new name, Props/pitcher_plant.png is taken)
            ("level", "Props/tepui_mist_00.png", tepui_mist(0)),  # Mist, frame 0
            ("level", "Props/tepui_mist_01.png", tepui_mist(1)),  # Mist, frame 1
            ("level", "Props/flamingo_00.png", flamingo(0)),  # Flamingo on one leg
            ("level", "Props/flamingo_01.png", flamingo(1)),  # Flamingo feeding
            ("level", "Props/salt_cone.png", salt_cone()),  # Salt heap
        ]),
        ("ITEMS", [
            ("item", "ulexite.png", ulexite()),  # White fibrous ulexite
        ]),
        ("LESSON PICTURES", [
            ("lesson", "lesson_table_mountain.png", lesson_table_mountain()),  # Table Mountain and its cloth
            ("lesson", "lesson_monument_valley.png", lesson_monument_valley()),  # Red mesas and buttes
            ("lesson", "lesson_grand_canyon.png", lesson_grand_canyon()),  # Layered canyon and river
            ("lesson", "lesson_roraima.png", lesson_roraima()),  # Tepui above the jungle
            ("lesson", "lesson_uyuni.png", lesson_uyuni()),  # Salt flat mirror
        ]),
    ]
    preview_groups = []  # (title, pictures) for the preview sheet
    for title, entries in groups:  # Each group
        sprites = []  # Pictures of this group
        for where, name, img in entries:  # Each picture
            put_file(where, name, img)  # Write it to its folder
            sprites.append(on_sky(img) if "mist" in name or "raven" in name else img)  # Pale mist and dark birds go on sky
        preview_groups.append((title, sprites))  # Add the group to the sheet
    preview_groups.append(("TILE AND CABLE CHECK", [ground_check(tabletop_top(), tabletop_soil()), ground_check(mesa_top(), mesa_soil()),
                                                    ground_check(canyon_top(), canyon_soil()), ground_check(tepui_top(), tepui_soil()),
                                                    ground_check(saltflat_top(), saltflat_soil()), tiled(salt_crust_top(), 4, 1),
                                                    cable_check()]))  # Seams and the hanging car, preview only
    preview(preview_groups)  # The check sheet
    print("Plateaus Normal art written to", la.OUT, ls.ITEMS_OUT, "and", ls.LESSON_OUT)  # Tell where the files went


if __name__ == "__main__":
    main()  # Run when the file is started directly
