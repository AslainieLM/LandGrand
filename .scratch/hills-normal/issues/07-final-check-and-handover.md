# 07: Final check and handover

**What to do:**

1. Run every new script, then every builder:
   - `python Tools/Art/make_hills_normal_art.py`
   - `python Tools/Art/make_hills_normal_backdrops.py`
   - `python Tools/Audio/make_hills_normal_audio.py`
   - **LandGrand > Build Levels**
   - **LandGrand > Build Lesson Scene**
   - **LandGrand > Build Selection Scenes**
   - **LandGrand > Build Landform Map Scene**
   - **LandGrand > Build Landform Description Scene**
   - **LandGrand > Build Profile Scene**
2. Run the EditMode tests and report the count (296 before this work).
3. Add to `CONTEXT.md`, in its style: Sinking float, Gentle bat, River current, Hot-air balloon, Level ambience.
4. Write `.scratch/hills-normal/HANDOVER.md`, and a short handover in your final message:
   - what was built, file by file
   - test results
   - every number you changed from the spec or `layouts.md`, and why
   - anything not done or not checked in the Editor
   - the three preview PNGs

   Do **not** edit the Obsidian vault (`D:\LandGrand\LandGrand-Sources`).

**Blocked by:** 01–06

**Status:** not started

**Play checklist for the user** (copy it into the handover):
- [ ] After Hills Easy 3, Next opens Hills Normal 1; the Hills Normal trail shows five stones
- [ ] Each Normal level shows its banner, lesson, theme, background and goal (Boat Dock, Cave Mouth, Falls Landing, Ifugao House, Balloon Field)
- [ ] N1: floats shake, then sink after a short wait; staying too long drops you in the water (short return); they rise back; the first float is safe to practise on
- [ ] N2: bats loop gently; a touch pushes you back a little and never to a flag; the cave plays drips, not birds; the bat sign is visible
- [ ] N3: currents push the way their ripples point; the one against you is still walkable; jumps are unchanged
- [ ] N4: one float, one bat loop and one current, each fair
- [ ] N5: balloons rise and fall with a burner whoosh; one balloon is needed for the wide gap; only the basket is solid
- [ ] Each level has its pickups, and the secret spot is reachable; the seven new kinds show their draft text
- [ ] Collection: the Hills tab shows 17 kinds, and "Found here" counts them
- [ ] Philippine Hills (after N1–N4) and Balloon Rider (after N5) can be earned; "New badge!" shows
- [ ] Mountains and Hills Easy still play as before

**Builder to run:** all of the above.
