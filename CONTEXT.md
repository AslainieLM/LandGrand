# LandGrand glossary

Domain words used in the game, its code and its planning notes. The project vault (`D:\LandGrand\LandGrand-Sources\LandGrand`) holds requirements and decisions; this file only fixes the words.

## Player and progress

- **Explorer**: the player, as the game names them ("Explorer: Ana"). One explorer per device.
- **Profile**: the screen, opened from the Main Menu, that shows the explorer's name, totals and Collection. It is local only; it is not an online account.
- **Player badge**: the small card in the top-left corner of the Main Menu with the explorer's portrait, name, stars and item kinds found. Tapping it opens the Profile.
- **Base camp**: the scene behind the Main Menu and the Profile, where the explorer rests by the campfire before each trip. It is not a playable place.

## Rocks and minerals

- **Item kind**: one rock or mineral the game teaches, such as Granite or Quartz. It has a picture, a name, a short description, and is either a rock or a mineral.
- **Pickup**: one item placed in a level. Several pickups can be the same item kind.
- **Found**: an item kind is found once the explorer has collected any pickup of it in any level. Being found is never undone by replaying.
- **Collection**: every item kind, shown in the Profile, with found kinds in colour and the others still hidden. It is a review of what the explorer has found, not a separate inventory.
- **Shared kind**: an item kind that belongs to more than one landform's shelf, such as limestone on both Mountains and Hills. It is found once and counted once, but shows on every shelf it belongs to.
- **Shelf**: one row of the Collection for one landform (Mountains, Hills, Plains, Plateaus). It holds every item kind that belongs to that landform, shared kinds included.
- **Item card**: the pop-up opened by tapping a found item kind in the Collection: picture, name, rock or mineral, and description.
- **Review pickup**: a pickup of an item kind that an earlier level already taught, placed to link the new lesson back to an old one. It gives only a chime, never the first-find pop-up, unless the explorer skipped finding that kind earlier.

- **Achievement**: a reward for reaching a goal in the game, such as clearing a first level or finding every Mountains rock and mineral. It is shown as a badge. An achievement is earned from progress the game already keeps (stars, cleared levels, found item kinds); it is never earned on its own and never lost.
- **Badge**: the round medal that shows one achievement: bronze, silver or gold. A locked badge is dark with a padlock but still shows its name and how to earn it.
- **Badge page**: the 5×3 grid of one landform's badges in the Profile's Achievements tab. Each landform has its own page; the general badges sit on the Mountains page.

## Levels

- **Lesson topic**: the one idea a level teaches about its landform, such as "volcanic mountains". Each level of a landform has a different lesson topic.
- **Level theme**: how one level looks inside its landform: its ground, props and goal. All levels of a landform share the landform's backdrop; the level theme is what tells them apart.
- **Level layout**: where the ground, gaps, ledges, boulders, hazards, pickups, checkpoint and goal of one level are. It is level data, not part of the builder.

- **Crumbling ledge**: a slab that cracks after the explorer stands on it, disappears briefly, then returns. Several slabs can form a bridge over a pit.
- **Falling rock**: a repeating trap with a shadow and pebble warning before a rock falls onto the trail.
- **Moving platform**: a slab that travels back and forth and carries an explorer standing on it.
- **Market boat**: a moving platform drawn as a river boat that carries the explorer across water.
- **Wind gust**: a brief leftward push within a marked stretch of trail, preceded by visible wind streaks.
- **Normal checkpoints**: the two safe flags in each Normal level. Hazards return the explorer to the furthest flag reached, or the start before either flag.
- **Rolling boulder**: a trap that warns before rolling left along the ground. It skips a roll if the explorer is too close to its starting point.
- **Rolling log** gives a dust warning and rolls left or right. It skips a cycle if the explorer is too close at the start.
- **Icy ground**: a stretch of trail where the explorer takes time to speed up and slides before stopping.
- **Slippery mud** uses the same slide and stopping rule as icy ground. A slide in mud adds a squelch and a splash.
- **Hard checkpoints**: the three safe flags in each Hard level. Hazards return the explorer to the furthest flag reached, or to the start before the first flag.
- **Soft sand**: a darker stretch of dune where the explorer walks more slowly, with a crunch and sand puffs. Jumps are not changed.
- **Sinking float**: a bamboo raft that sinks after the explorer stands on it, then rises after the explorer leaves. It supports the explorer from above until it goes under.
- **Gentle bat**: a bat that flies in a small loop and gives the explorer a short sideways push on contact.
- **River current**: a shallow strip of flowing water that pushes a grounded explorer in the direction of the ripples.
- **Straw bounce pile**: a low pile of rice straw. Landing on its top bounces the explorer high. Walking into its side does nothing.
- **Hot-air balloon**: a moving platform that rises and falls. The explorer can stand on the basket's rim.
- **Coming soon difficulty**: a difficulty that is open but has no levels yet. Tapping it says "Coming soon" instead of opening an empty trail.

## Landforms

- **Landform Description Screen**: the screen that reviews a completed landform, shows the explorer's trip totals and names the next landform.
- **Episode complete**: the explorer has cleared all 15 levels of one landform across Easy, Normal and Hard.
- **Hills complete** means the explorer has cleared all 15 Hills levels. Next Episode after Hills Hard 7 opens the Hills Landform Description Screen. Plains unlocks and shows "Coming soon" on the map.
- **Unlock animation**: the opening padlock, fading grey and sparkles that the map shows the first time a landform opens.
- **Level ambience**: the background sound of a trail, such as wind, a stream or cave drips. A trail can use its own ambience.
- **Landform audio table**: the list that names each landform's level music and its ambience for Easy, Normal and Hard (wind for Mountains, stream and rain for Hills).
