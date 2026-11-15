# 02: Rules for the four level objects

**What to build:** Plain C# rules, with tests, for the crumbling ledge, the falling rock, the moving platform and the wind gust, exactly as in the spec ("Level objects: exact rules"). They follow the style of `SteamVent.cs`: constants plus pure functions of time and position. The play scripts in ticket 04 only ask these rules.

| Type | Constants | Functions (suggested) |
|---|---|---|
| `CrumbleLedge` | `Width = 32`, `BreakSeconds = 0.5`, `ReturnSeconds = 3.0` | `StateAt(touchedAt, now)`, giving `Whole`, `Cracking` (with 0–1 progress), `Gone` or `Back`; `IsSolid(...)` |
| `Rockfall` | `CycleSeconds = 3`, `FallStart = 1.2`, `Land = 1.5`, `Gone = 1.65`, `Size = 16` | `PhaseAt(seconds)` (`Warning`/`Falling`/`Landed`/`Clear`), `RockBottomAt(seconds)` (270 down to 0, or none), `Hits(bodyLeft, bodyBottom, bodyWidth, bodyHeight, seconds)` |
| `MovingPlatform` | `Width = 48`, `Speed = 40` | `LeftAt(seconds)`, back and forth between X0 and X1, starting at X0 at time 0 |
| `WindZone` | `CycleSeconds = 4`, `WarnAt = 2.0`, `PushAt = 2.6`, `PushSpeed = 40` | `PhaseAt(seconds)` (`Calm`/`Warning`/`Push`), `PushAt(feetX, seconds)` (0 or −40) |

All times use the level clock that the steam vents already use (`Time.timeSinceLevelLoad` on the play side). Each `Offset` shifts the cycle: `phase = (t + Offset) mod Cycle`.

**`LevelRun` hooks:**
- A rock hit sends the explorer back exactly like a trap on Normal: to the last checkpoint, with items and time kept.
- A fall through a broken crumbling ledge is a normal pit fall.
- On Easy nothing changes (no Easy level has these objects).

**Blocked by:** 01

**Status:** done

- [x] Tests for each type at the cycle edges: 0, just before and after each threshold, and wrap-around with an offset
- [x] Moving platform test: position at 0, at X1, and coming back
- [x] Wind test: the push is never more than 40 and is 0 outside the zone
- [x] Rock hit test: no hit during the warning, a hit while falling through the body, no hit after `Gone`
- [x] No Unity types; basic-English comments on every line

**Builder to run:** none (rules only).

Spec: `../spec.md` ("Level objects: exact rules").

Verified: Unity EditMode 192 passed, 0 failed.
