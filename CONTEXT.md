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
- **Home landform**: the one landform an item kind belongs to: the landform whose lesson first teaches it. An item kind found in other landforms still has only one home landform.
- **Shelf**: one row of the Collection for one landform (Mountains, Hills, Plains, Plateaus). It holds the item kinds whose home landform it is.
- **Item card**: the pop-up opened by tapping a found item kind in the Collection: picture, name, rock or mineral, and description.
