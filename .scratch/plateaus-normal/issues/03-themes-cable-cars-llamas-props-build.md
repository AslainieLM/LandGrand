# 03: Themes, cable cars, llamas, salt crust, props; build every scene

**What to build** (spec sections 4, 5 and 8), in this order:

1. `ThemeArt`: `PlatformArtAbove` (default 0) and `PlatformCable` (default null), with a test that every old theme keeps its defaults.
2. The moving-platform build: placement with `PlatformArtAbove`, the cable line strip, and the landing sound. Compare a Mountains Normal moving-platform scene and Plains Normal 5 against `HEAD`: unchanged.
3. The five theme entries (spec section 8 table), including the SaltFlat herd and ice fields.
4. Living props (spec section 8), with the existing prop helpers; report any prop moved more than 64 px.
5. Run the three asset scripts twice each (outputs identical): `make_plateaus_normal_art.py`, `make_plateaus_normal_backdrops.py`, `make_plateaus_normal_audio.py`.
6. Run the six builders one at a time (no "Missing", no errors).
7. Regression: Plains Hard 2 herds, Plains Hard 4 frost, Plains Normal 5 boats and Plateaus Easy 3 updrafts unchanged against `HEAD`.

**Checklist**
- [x] No per-level, per-landform or per-theme `if` in play scripts (D-061)
- [x] Old platforms, herds and ice unchanged
- [x] Goal offsets checked; any change reported
- [x] EditMode tests pass (report the count)

**Blocked by:** 01, 02

**Status:** done (2026-11-24, 563 tests)

**Builder to run:** the six builders (Codex runs them in batch mode; the user re-runs **LandGrand > Build Levels** only if the Editor shows stale scenes)
