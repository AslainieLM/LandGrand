# 02: Rules for the rolling boulder and icy ground

**What to build:** The behaviour of the two new level objects as plain C# rules with tests. The numbers are in the spec; put them as named constants on the types.

1. **`RollingBoulder`** (cycle 3.5 s, warning 0.8 s, speed 90 px/s, size 14, skip distance 48):
   - `Phase(float t)`: seconds into the cycle, with `Offset`.
   - `IsWarning(float t)`: the phase is below 0.8.
   - `CycleIndex(float t)`: which cycle `t` is in (for the skip rule).
   - `XAt(float t)`: the middle x while rolling, or `float.NaN` during the warning or after it passes `End`.
   - A hit box for time `t`: the 14×14 box centred on x, from y 0, or none.
   - **Skip rule:** a small state class `RollingBoulderRun` (one per boulder) with `Update(float t, float explorerX)`. When the phase crosses 0.8 and `|explorerX − Spawn| < 48`, that cycle is skipped. `CurrentX(float t)` returns `NaN` for a skipped cycle. The views and the hazard check both use this.
   - `MaxRun` = 90 × (3.5 − 0.8) = 243 px.
2. **`IceZone`**: `Contains(float x)` for the explorer's middle.
3. **`IceSlide.Next(float current, float target, float dt)`**: moves `current` toward `target`. Speeding up toward a held direction (same sign as `current`, or `current` is 0, and `|target| > |current|`) uses 160 px/s². Anything else (slowing, stopping, turning) uses 100 px/s². It never passes `target`.

**Blocked by:** 01

**Status:** done

- [x] Rolling boulder tests: warning then rolling; x falls by 90 per second; gone after `End`; the offset shifts the cycle; the hit box is 14×14 at y 0; a cycle is skipped when the explorer is within 48 px of `Spawn` at 0.8 s, and only that cycle
- [x] Ice tests: 0 to 72 takes about 0.45 s; 72 to 0 takes about 0.72 s; turning uses the slow rate; it never overshoots; `IceZone.Contains` at the edges
- [x] Wind push still adds on top (no change to `WindZone`)
- [x] No Unity types; basic-English comments

**Verified:** Unity EditMode 212 passed. Boulder timing, hit box, per-cycle skip, and ice acceleration and edges are covered. Wind rules are unchanged.

**Builder to run:** none (rules only).

Spec: `../spec.md` ("Level objects: exact rules").
