# 04: Completion Screen, the last Description Screen and the map flag

**What to build** (spec section 7):

1. **`LandformDescriptionController` and `LandformDescriptionBuilder`:**
   - the "You explored all four landforms!" line, shown when there is no next landform and `IsGameComplete`;
   - `ToMap` opens `CompletionScreen` once (device setting `LandGrand.CompletionSeen`), else the map;
   - a "See Ending" button on book visits once `IsGameComplete`.
   Keep every other landform's screen exactly as before.
2. **`CompletionScreen`:** a new `CompletionScreenController` (copy the screen base class the Description Screen uses: fades, Back, `Leave`) and a new editor builder `CompletionBuilder.Build` with the menu item **LandGrand > Build Completion Scene**; add the scene to Build Settings. Layout: start-up background layers, `completion_plateau` in front, the start-up explorer standing on it, the six text lines, one Back to Map button with the existing button art, menu music. Numbers from `CompletionSummary`. Set `LandGrand.CompletionSeen` on opening.
3. **Map:** `LandformMapBuilder` adds the champion flag (two-frame wave) on the Plateaus region and the "All landforms explored!" ribbon; `LandformMapController` shows them only when `IsGameComplete`.
4. **Tests:** EditMode tests for the route rules (put the route choice in a small plain C# rule, e.g. `CompletionRoute.AfterDescription(isGameComplete, seen)`, so it can be tested) and for the totals text.
5. **Build:** run `LandformDescriptionBuilder.Build`, `LandformMapBuilder.Build`, `CompletionBuilder.Build`; no "Missing", no errors.

**Checklist**
- [ ] Mountains, Hills and Plains Description Screens look and act as before
- [ ] First Plateaus completion: Description, then Completion Screen, then the map with the flag; later visits skip the Completion Screen
- [ ] See Ending shows only on a book visit after everything is complete
- [ ] Text fits at 22 px Micro 5 without overflow (check the built scene in a 16:9 and a 20:9 Game view)
- [ ] EditMode tests pass (report the count)

**Blocked by:** 01

**Status:** open

**Builder to run:** **LandGrand > Build Landform Description Scene**, **Build Landform Map Scene**, **Build Completion Scene**
