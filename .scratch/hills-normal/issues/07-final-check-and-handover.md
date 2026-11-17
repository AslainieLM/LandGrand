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

**Status:** done

**Verified:** Three generators reproduced the reviewed previews. All six Unity builders passed. EditMode passed 360 of 360. The handover records the five-scene Play Mode smoke check and the manual checks still open.

**Play checklist for the user** (copy it into the handover):
- [ ] After Hills Easy 3, Next opens Hills Normal 1; the Hills Normal trail shows five stones
- [ ] Each Normal level shows its banner, lesson, theme, background and goal (Boat Dock, Cave Mouth, Falls Landing, Ifugao House, Balloon Field)
- [ ] N1: floats shake, sink after a short wait and rise again. Practise over the safe pool. A fall into a water pit returns you to the furthest reached flag, or the start before the first flag.
- [ ] N2: bats loop gently; a touch pushes you back a little and never to a flag; the cave plays drips, not birds; the bat sign is visible
- [ ] N3: currents push the way their ripples point; the one against you is still walkable; jumps are unchanged
- [ ] N4: one float, one bat loop and one current, each fair
- [ ] N5: balloons rise and fall with a burner whoosh; one balloon is needed for the wide gap; only the basket is solid
- [ ] Each level has its pickups, and the secret spot is reachable; the seven new kinds show their draft text
- [ ] Collection: the Hills tab shows 17 kinds, and "Found here" counts them
- [ ] Philippine Hills (after N1–N4) and Balloon Rider (after N5) can be earned; "New badge!" shows
- [ ] Mountains and Hills Easy still play as before
- [ ] Mountains Normal: moving platforms carry you and crumbling ledges drop you as before (the landing code was rewritten)

**Builder to run:** all of the above.

- `CONTEXT.md` adds the five requested glossary terms.
- `.scratch/hills-normal/HANDOVER.md` records every added or changed file and the exact builder commands.
