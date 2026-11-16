# 04: "Coming soon" difficulties and the Result Screen's Next

**What to build** (spec section 2, decision 9):

1. Difficulty and Level Selection: when a difficulty is open but has no level data in `LevelCatalog` (for now, Hills Normal and Hard after the three Hills Easy levels are cleared), tapping it shows "Coming soon" in the same guide panel the map uses. It does not open an empty level trail.
2. `ResultScreen.cs`: when the next level has no data, hide the Next button. Back to Map and Replay stay.
   - After Hills Easy 3, the player sees Back to Map and Replay only.
   - Next Episode after Mountains Hard 7 still opens the Landform Description Screen.
3. If a rule decides this (for example "has next level"), put it in Rules with tests. Example: `LevelCatalog.Next(e, d, n)` returns null when there is no data.

**Checklist**
- [ ] Mountains: Next after Easy 3 still opens Normal 1, and Next after Normal 5 still opens Hard 1
- [ ] Hills: after Easy 3, Next is hidden; Normal shows "Coming soon" when tapped
- [ ] EditMode tests pass (report the count)

**Blocked by:** 02

**Status:** not started

**Builder to run:** **LandGrand > Build Selection Scenes**, **LandGrand > Build Levels**
