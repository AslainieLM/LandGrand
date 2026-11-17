# 05: Hills life on the map and the menu screens

**What to build** (spec section 9, "Map life" and "Menu life"):

1. `LandformMapBuilder.cs:89-95`:
   - Remove the "Hills Cloud".
   - Build the Hills life set (six swaying trees, three stream ripples and two birds) at the given places, from `Assets/Art/LandformMap/Sprites/Hills/`.
   - Use the existing `SpriteFlipbook` and `CircleFlight` components.
   - It is active only while Hills is open, like `regions[0].life`.
2. Menu screens (`SelectionBuilder.cs`, `LandformBackdrop.cs`):
   - Make the life objects per landform: Mountains keeps snow and eagle; Hills gets two birds and six grass tufts; the cloud shows for every landform.
   - `LandformBackdrop` turns on only the set for `GameSession.Episode`.
   - The set belongs to the landform, not to the screen.
3. Plains on the map: no code change is expected. Locked Plains says "Finish Hills first", and its one-time unlock uses `LandGrand.UnlockSeen.Plains`. Check it, and report.

**Checklist**
- [ ] Mountains map and menu screens look exactly as before
- [ ] Hills (open) shows trees swaying, the stream shining and two birds, and no cloud
- [ ] The Hills difficulty, level and lesson screens show birds and grass and no snow or eagle

**Blocked by:** none

**Status:** built by Claude (Haiku), not yet run in Unity

**Builder to run:** **LandGrand > Build Landform Map Scene**, **LandGrand > Build Selection Scenes**, **LandGrand > Build Lesson Scene**
