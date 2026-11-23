// Plateaus Normal bot driver (scratch, D-073): the Plains Hard driver with the Normal lint, plus the cliff updraft
// (reach searches also ride a column; updraft fairness lint from Plateaus Easy).
// Plains Hard bot driver (scratch, D-071): the Plains Normal driver back on the Hard lint (9 pickups, 3 checkpoints, 240 s),
// plus herd checks (a herd is a Bat on the ground: Cy 6, Ry 0), ice checks and hay-versus-straw checks.
// Earlier note: Plains Normal bot driver (scratch, D-070): the Hills Hard driver with a Normal lint (7 pickups, 2 checkpoints, 180 s).
// Earlier note: Hills Hard bot driver, reach searches, lint and self-checks (scratch, not project code).
// Copied from the Hills Normal Program with: rolling logs in both directions, slippery mud stats, the Hard lint (9 pickups,
// log runs on plain ground, nothing within 100 px of a checkpoint, a secret on a high ledge) and the Mountains Hard re-run.
static class Program {
    public const float Limit = 240f;
    public class Stats { public float[] FloatTime, CurrentTime, BalloonTime, BatTime; public int[] BatPushes; public float MudTime; public int LogJumps, LogDodges; }
    static bool LogAheadComing(Sim s, Lvl l) => Enumerable.Range(0, l.Rollers.Length).Any(i => { float x = s.RollerXNow(i); return l.Rollers[i].Dir < 0 && !float.IsNaN(x) && x - s.px > 0 && x - s.px < 44; });
    static float LogBehindGap(Sim s, Lvl l) { float best = float.MaxValue; for (int i = 0; i < l.Rollers.Length; i++) { float x = s.RollerXNow(i); if (l.Rollers[i].Dir > 0 && !float.IsNaN(x) && s.px - x > -8 && s.px - x < 44 && l.Rollers[i].End + Rules.RollSize / 2 > s.px - Sim.W / 2) best = Math.Min(best, s.px - x); } return best; }
    public static (bool ok, float t, string why, Stats st) Bot(Lvl l, float t0 = 0) {
        var r0 = BotRun(l, t0); return (r0.ok, r0.t - t0, r0.why, r0.st);
    }
    static (bool ok, float t, string why, Stats st) BotRun(Lvl l, float t0) {
        var s = new Sim(l, t0); bool prev = false; float stuckX = -1, stuckT = 0; bool dodging = false;
        var st = new Stats { FloatTime = new float[l.Floats.Length], CurrentTime = new float[l.Currents.Length], BalloonTime = new float[l.Balloons.Length], BatTime = new float[l.Bats.Length] };
        var ridden = new bool[l.Balloons.Length]; var balloonStart = Enumerable.Repeat(-1f, l.Balloons.Length).ToArray();
        while (s.t < Limit + t0) {
            int dir = 1; bool forceJump = false, holdJump = false;
            foreach (var r in l.Rocks) { float dx = r.X - s.px, ph = Rules.RockPhase(r, s.t); if (dx > 10 && dx < 44 && ph > 0.7f && ph < Rules.RockGone + 0.05f && s.grounded) dir = 0; }
            var pitAheadObj = l.Pits.FirstOrDefault(p => p.X - s.px > 0 && p.X - s.px < 14);
            if (s.grounded && !s.OnPlatform && pitAheadObj != null) {
                int pi = Array.FindIndex(l.Platforms, p => Math.Abs(p.X0 - pitAheadObj.X) < 1 && p.Top == 0);
                if (pi >= 0) { float x = Rules.PlatformX(l.Platforms[pi], s.t); dir = (s.px < pitAheadObj.X - 10 || x - l.Platforms[pi].X0 < 4) ? 1 : 0; }
                foreach (var w in l.Winds) if (s.px > w.X - 80 && s.px < w.X + w.W) { float ph = Rules.WindPhase(w, s.t); if (ph > Rules.WindWarn - 1.0f) dir = 0; }
            }
            // Wait out a gust before jumping spikes inside a wind zone too (the Mountains Hard bot did this only for pits).
            if (s.grounded && l.Spikes.Any(k => k - s.px > 0 && k - s.px < 30))
                foreach (var w in l.Winds) if (s.px > w.X - 80 && s.px < w.X + w.W) { float ph = Rules.WindPhase(w, s.t); if (ph > Rules.WindWarn - 1.0f) dir = 0; }
            if (s.OnPlatform) { var p = l.Platforms[s.PlatformIndex]; float x = Rules.PlatformX(p, s.t); dir = x > p.X1 - 24 ? 1 : 0; if (s.px < x + 18) dir = 1; }
            if (s.standBalloon >= 0) {
                int bi = s.standBalloon; var b = l.Balloons[bi]; ridden[bi] = true; if (balloonStart[bi] < 0) balloonStart[bi] = s.t;
                float c = b.X + Rules.BalloonWidth / 2, top = Rules.BalloonTop(b, s.t);
                if (top >= b.High - 3 && Rules.BalloonRising(b, s.t)) { dir = 1; forceJump = true; }
                else { dir = s.px < c - 4 ? 1 : s.px > c + 4 ? -1 : 0; holdJump = true; }
            } else if (s.grounded && Math.Abs(s.py) < 0.5f) {
                for (int bi = 0; bi < l.Balloons.Length; bi++) {
                    var b = l.Balloons[bi]; float gap = b.X - (s.px + Sim.W / 2);
                    if (ridden[bi] || gap < -2 || gap > 30) continue;
                    if (balloonStart[bi] < 0) balloonStart[bi] = s.t;
                    float top = Rules.BalloonTop(b, s.t);
                    if (top <= b.Low + 6 && !Rules.BalloonRising(b, s.t)) { dir = 1; forceJump = true; } else { dir = 0; holdJump = true; }
                }
            }
            // A log rolling right that is close behind: stop, let it come, and hop straight up over it.
            float behind = LogBehindGap(s, l); bool dodgeJump = false;
            if (behind < float.MaxValue && Math.Abs(s.py) < 0.5f && !s.OnPlatform && s.standBalloon < 0) { if (!dodging) st.LogDodges++; dodging = true; dir = 0; if (s.grounded && behind < 22) dodgeJump = true; }
            else if (s.grounded) dodging = false;
            bool platPit = pitAheadObj != null && l.Platforms.Any(p => Math.Abs(p.X0 - pitAheadObj.X) < 1 && p.Top == 0);
            bool pitAhead = s.grounded && dir == 1 && !s.GroundAt(s.px + 7) && !platPit && !s.OnPlatform;
            bool spikeAhead = s.grounded && dir == 1 && s.spikes.Any(k => k.x - s.px < 22 && k.x - s.px > 0);
            bool wall = s.grounded && dir == 1 && s.BlockedAhead(6);
            bool rollerAhead = s.grounded && Math.Abs(s.py) < 0.5f && LogAheadComing(s, l);
            bool want = forceJump || dodgeJump || ((wall || pitAhead || spikeAhead || rollerAhead) && s.grounded && !holdJump);
            bool dbl = !s.grounded && !s.airJumpUsed && s.vy <= 0 && !dodging && (!s.GroundAt(s.px + 10) || s.spikes.Any(k => k.x - s.px < 30 && k.xMax > s.px) || s.BlockedAhead(4));
            if (!s.grounded) dir = dodging ? 0 : 1;
            bool press = (want || dbl) && !prev; prev = press;
            if (press && rollerAhead && s.grounded) st.LogJumps++;
            int sf = s.standFloat, sb = s.standBalloon;
            s.Step(dir, press);
            if (s.OnMud) st.MudTime += 1f / 60f;
            if (sf >= 0) st.FloatTime[sf] += 1f / 60f;
            for (int i = 0; i < l.Currents.Length; i++) if (s.grounded && Math.Abs(s.py) < 0.5f && s.px >= l.Currents[i].X && s.px <= l.Currents[i].X + l.Currents[i].W) st.CurrentTime[i] += 1f / 60f;
            for (int i = 0; i < l.Bats.Length; i++) { var b = l.Bats[i]; if (s.px > b.Cx - b.Rx - Rules.BatW && s.px < b.Cx + b.Rx + Rules.BatW) st.BatTime[i] += 1f / 60f; }
            for (int i = 0; i < l.Balloons.Length; i++) if (balloonStart[i] >= 0 && st.BalloonTime[i] == 0 && sb == i && s.standBalloon != i) st.BalloonTime[i] = s.t - balloonStart[i];
            if (s.OnSpikes()) return (false, s.t, $"spikes at x={s.px:F0}", st);
            if (s.HitByRock()) return (false, s.t, $"rock at x={s.px:F0}", st);
            if (s.HitByRoller()) return (false, s.t, $"log at x={s.px:F0}", st);
            if (s.InPit) return (false, s.t, $"pit at x={s.px:F0}", st);
            if (s.px >= l.Goal - 8) { st.BatPushes = s.batPushes; return (true, s.t, "goal", st); }
            if (Math.Abs(s.px - stuckX) < 0.5f) { stuckT += 1f / 60f; if (stuckT > 12f) return (false, s.t, $"stuck at x={s.px:F0},y={s.py:F0}", st); } else { stuckX = s.px; stuckT = 0; }
        }
        return (false, s.t, "time", st);
    }

    // Start spots for the reach searches: ground near the target, boulder tops, reachable ledges, balloon baskets, floats, platforms.
    static List<(float x, float y, int balloon, int plat)> Starts(Lvl l, float x, HashSet<int> ledgesOk) {
        var starts = new List<(float x, float y, int balloon, int plat)>();
        foreach (var dx in new[] { -120f, -90f, -60f, -40f, -20f, 20f, 40f, 60f }) starts.Add((x + dx, 0, -1, -1));
        for (int i = 0; i < l.Ledges.Length; i++) { var e = l.Ledges[i]; if (ledgesOk.Contains(i) && Math.Abs(e.X - x) < 300) starts.Add((e.X + e.W / 2, e.Top, -1, -1)); }
        foreach (var c in l.Crumbles) if (Math.Abs(c.X - x) < 300) starts.Add((c.X + 16, c.Top, -1, -1));
        foreach (var b in l.Boulders) if (Math.Abs(b.X - x) < 300) starts.Add((b.X + (b.Big ? 24 : 16), b.Big ? 35 : 23, -1, -1));
        foreach (var f in l.Floats) if (Math.Abs(f.X - x) < 300) starts.Add((f.X + 24, 0, -1, -1));
        for (int i = 0; i < l.Balloons.Length; i++) if (Math.Abs(l.Balloons[i].X - x) < 300 && l.Balloons[i].Low <= 40) starts.Add((l.Balloons[i].X + 24, 0, i, -1));
        for (int i = 0; i < l.Platforms.Length; i++) if (Math.Abs(l.Platforms[i].X0 - x) < 400) starts.Add((0, l.Platforms[i].Top, -1, i));
        return starts;
    }
    // Can some (start time, start spot, jump frame, double-jump delay, direction) meet the goal test and land safely?
    static bool Search(Lvl l, float x, HashSet<int> ledgesOk, Func<Sim, bool> hit, bool needLandAfter) {
        foreach (var (sx0, sy0, bal, plat) in Starts(l, x, ledgesOk)) foreach (var t0 in new[] { 0f, 0.5f, 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4.5f, 6f })
            foreach (int dir in new[] { 1, -1 }) foreach (bool ride in l.Updrafts.Length > 0 ? new[] { false, true } : new[] { false }) for (int j = 0; j < 40; j += 3) for (int d = -1; d < 40; d += 3) {
                var s = new Sim(l, t0); float sy = bal >= 0 ? Rules.BalloonTop(l.Balloons[bal], t0) : sy0;
                float sx = plat >= 0 ? Rules.PlatformX(l.Platforms[plat], t0) + 24 : sx0;
                s.Place(sx, sy + 0.5f); for (int q = 0; q < 6; q++) s.Step(0, false);
                if (!s.grounded || s.InPit || s.OnSpikes() || s.HitByRoller()) break;
                bool got = false; float hover = 0; int a = 0; // a counts the frames with input; riding frames do not count
                for (int f = 0; f < 400 && a < 160; f++) {
                    if (ride && s.InUpdraft && hover < 0.3f) { if (s.Hovering) hover += 1f / 60f; s.Step(0, false); } // stand in the column until it holds the explorer at the top
                    else { s.Step(dir, a == j || a == j + d); a++; }
                    if (s.InPit || s.OnSpikes() || s.HitByRock() || s.HitByRoller()) break;
                    if (hit(s)) got = true;
                    if (got && (!needLandAfter || s.grounded)) return true;
                }
            }
        return false;
    }
    static HashSet<int> ReachableLedges(Lvl l) {
        var ok = new HashSet<int>(); bool grew = true;
        while (grew) { grew = false;
            for (int i = 0; i < l.Ledges.Length; i++) { if (ok.Contains(i)) continue; var e = l.Ledges[i];
                if (Search(l, e.X + e.W / 2, ok, s => s.grounded && Math.Abs(s.py - e.Top) < 0.5f && s.px > e.X && s.px < e.X + e.W, false)) { ok.Add(i); grew = true; } } }
        return ok;
    }
    static bool Reach(Lvl l, Pickup k, HashSet<int> ledgesOk) { var box = new R(k.X - 12, k.Y, 24, 24); return Search(l, k.X, ledgesOk, s => s.Body.Overlaps(box), true); }

    static bool Near(float a0, float a1, float b0, float b1, float m) => a0 < b1 + m && a1 > b0 - m;
    static (float a, float b) Run(Roller r) => (Math.Min(r.Spawn, r.End), Math.Max(r.Spawn, r.End));
    // Static checks on a Hard layout (Mountains Hard log rules + Hills Normal object rules + the agreed Hills Hard rules).
    public static List<string> Lint(Lvl l, int pickups = 9) {
        var bad = new List<string>(); var sands = l.Sands ?? new Sand[0];
        foreach (var r in l.Rollers) {
            var (a0, b0) = Run(r); float a = a0 - 8, b = b0 + 8;
            if (Math.Sign(r.End - r.Spawn) != r.Dir) bad.Add($"log {r.Spawn} End is on the wrong side for its direction");
            if (b0 - a0 > Rules.RollSpeed * (Rules.RollCycle - Rules.RollStart)) bad.Add($"log {r.Spawn} run too long");
            if (l.Pits.Any(p => p.X < b && p.X + p.W > a) || l.Pools.Any(p => p.X < b && p.X + p.W > a)) bad.Add($"log {r.Spawn} crosses a pit");
            if (l.Spikes.Any(s => s < b && s + 32 > a)) bad.Add($"log {r.Spawn} crosses spikes");
            if (l.Boulders.Any(o => o.X < b && o.X + (o.Big ? 48 : 32) > a)) bad.Add($"log {r.Spawn} crosses a boulder");
            if (l.Vents.Any(v => v.X < b && v.X + 24 > a)) bad.Add($"log {r.Spawn} crosses a vent");
            if (l.Balloons.Any(v => v.X < b && v.X + 48 > a)) bad.Add($"log {r.Spawn} crosses a balloon");
            if (l.Currents.Any(v => v.X < b && v.X + v.W > a)) bad.Add($"log {r.Spawn} crosses a current");
            if (l.Bats.Any(v => Near(a, b, v.Cx - v.Rx - 8, v.Cx + v.Rx + 8, 40))) bad.Add($"log {r.Spawn} under a bat");
            if (l.Checkpoints.Any(c => Near(a, b, c, c, 100))) bad.Add($"log {r.Spawn} within 100 px of a checkpoint");
            if (a < 200 || b > l.Goal - 120) bad.Add($"log {r.Spawn} at start or goal");
        }
        foreach (var c in l.Checkpoints) {
            if (l.Pits.Any(p => Near(p.X, p.X + p.W, c, c, 100))) bad.Add($"pit near checkpoint {c}");
            if (l.Pools.Any(p => Near(p.X, p.X + p.W, c, c, 100))) bad.Add($"pool near checkpoint {c}");
            if (l.Spikes.Any(s => Near(s, s + 32, c, c, 100))) bad.Add($"spikes near checkpoint {c}");
            if (l.Crumbles.Any(s => Near(s.X, s.X + 32, c, c, 100))) bad.Add($"crumble near checkpoint {c}");
            if (l.Platforms.Any(p => Near(p.X0, p.X1 + 48, c, c, 100))) bad.Add($"moving platform near checkpoint {c}");
            if (l.Boulders.Any(o => o.X < c + 60 && o.X + 48 > c - 60)) bad.Add($"boulder on checkpoint {c}");
            if (l.Rocks.Any(r => Math.Abs(r.X - c) < 120)) bad.Add($"rock near checkpoint {c}");
            if (l.Bats.Any(b => Near(b.Cx - b.Rx - 8, b.Cx + b.Rx + 8, c, c, 100))) bad.Add($"bat near checkpoint {c}");
            if (l.Currents.Any(k => Near(k.X, k.X + k.W, c, c, 40))) bad.Add($"current on checkpoint {c}");
            if (l.Balloons.Any(b => Near(b.X, b.X + 48, c, c, 40))) bad.Add($"balloon on checkpoint {c}");
            if (l.Ices.Any(i => Near(i.X, i.X + i.W, c, c, 40))) bad.Add($"ice on checkpoint {c}");
            if (l.Muds.Any(i => Near(i.X, i.X + i.W, c, c, 40))) bad.Add($"mud on checkpoint {c}");
            if (l.Winds.Any(i => Near(i.X, i.X + i.W, c, c, 40))) bad.Add($"wind on checkpoint {c}");
        }
        bool HazardNear(float a, float b, float m) => l.Pits.Any(p => Near(a, b, p.X, p.X + p.W, m)) || l.Spikes.Any(s => Near(a, b, s, s + 32, m));
        foreach (var i in l.Ices) if (l.Rocks.Any(r => r.X > i.X - 30 && r.X < i.X + i.W + 30)) bad.Add($"rock on ice {i.X}");
        foreach (var m in l.Muds) {
            if (l.Rocks.Any(r => r.X > m.X - 30 && r.X < m.X + m.W + 30)) bad.Add($"rock on mud {m.X}");
            if (HazardNear(m.X, m.X + m.W, 0)) bad.Add($"pit or spikes inside mud {m.X}");
            if (l.Currents.Any(k => Near(m.X, m.X + m.W, k.X, k.X + k.W, 0)) || sands.Any(k => Near(m.X, m.X + m.W, k.X, k.X + k.W, 0))) bad.Add($"mud {m.X} overlaps a current or sand");
        }
        foreach (var b in l.Bats) {
            bool herd = b.Ry == 0 && b.Cy == PlainsHardLevels.HerdY; float a = b.Cx - b.Rx - 8, z = b.Cx + b.Rx + 8;
            if (HazardNear(a, z, 80)) bad.Add($"hazard within 80 px of {(herd ? "herd" : "bat")} {b.Cx}");
            if (!herd && (b.Cy - b.Ry - 6 > 20 || b.Cy + b.Ry - 6 < 6)) bad.Add($"bat {b.Cx} not at body height");
            if (herd && l.Boulders.Any(o => Near(a, z, o.X, o.X + (o.Big ? 48 : 32), 8))) bad.Add($"herd {b.Cx} walks into a boulder");
            if (herd && l.Straws.Any(k => Near(a, z, k.X, k.X + Rules.StrawW, 8))) bad.Add($"herd {b.Cx} walks into a straw pile");
            if (herd && l.Rollers.Any(r => Near(a, z, Run(r).a, Run(r).b, 40))) bad.Add($"herd {b.Cx} on a hay run");
        }
        foreach (var i in l.Ices) if (HazardNear(i.X, i.X + i.W, 0)) bad.Add($"pit or spikes inside ice {i.X}");
        foreach (var r in l.Rollers) if (l.Straws.Any(k => Near(Run(r).a, Run(r).b, k.X, k.X + Rules.StrawW, 8))) bad.Add($"hay {r.Spawn} crosses a straw pile");
        foreach (var k in l.Straws) if (HazardNear(k.X, k.X + Rules.StrawW, 0) || l.Checkpoints.Any(c => Near(k.X, k.X + Rules.StrawW, c, c, 100))) bad.Add($"straw {k.X} on a hazard or checkpoint");
        if (l.Introduces.Contains("herd")) { var h = l.Bats.OrderBy(b => b.Cx).First(); if (l.Rollers.Any(r => Near(Run(r).a, Run(r).b, h.Cx - h.Rx, h.Cx + h.Rx, 120))) bad.Add("first herd has hay near"); }
        foreach (var k in l.Currents) if (HazardNear(k.X, k.X + k.W, 80)) bad.Add($"hazard within 80 px of current {k.X}");
        foreach (var b in l.Balloons) { float rise = b.High - b.Low; if (rise < Rules.BalloonMinRise || rise > Rules.BalloonMaxRise) bad.Add($"balloon {b.X} rise {rise}"); if (l.Pits.Any(p => Near(b.X, b.X + 48, p.X, p.X + p.W, 0))) bad.Add($"balloon {b.X} over a pit"); }
        foreach (var f in l.Floats) {
            var pit = l.Pits.FirstOrDefault(p => f.X >= p.X && f.X + 48 <= p.X + p.W); var pool = l.Pools.FirstOrDefault(p => f.X >= p.X && f.X + 48 <= p.X + p.W);
            if (pit == null && pool == null) { bad.Add($"float {f.X} not inside water"); continue; }
            float wx = pit?.X ?? pool.X, ww = pit?.W ?? pool.W;
            var edges = new List<float> { wx }; foreach (var o in l.Floats.Where(o => o.X >= wx && o.X + 48 <= wx + ww).OrderBy(o => o.X)) { edges.Add(o.X); edges.Add(o.X + 48); } edges.Add(wx + ww);
            for (int i = 0; i + 1 < edges.Count; i += 2) if (edges[i + 1] - edges[i] > 40) bad.Add($"water gap {edges[i + 1] - edges[i]} px at {edges[i]} wider than one jump");
        }
        foreach (var p in l.Platforms) if (!l.Pits.Any(q => Math.Abs(q.X - p.X0) < 1 && p.X1 + 48 >= q.X + q.W) && p.Top == 0) bad.Add($"moving platform {p.X0} does not span its pit");
        var hazX = l.Pits.Select(p => (p.X, p.X + p.W)).Concat(l.Spikes.Select(s => (s, s + 32))).Concat(l.Bats.Select(b => (b.Cx - b.Rx - 8, b.Cx + b.Rx + 8)))
            .Concat(l.Crumbles.Select(c => (c.X, c.X + 32f))).Concat(l.Muds.Select(m => (m.X, m.X + m.W))).Concat(l.Winds.Select(w => (w.X, w.X + w.W)));
        if (hazX.Any(h => h.Item1 < 200) || hazX.Any(h => h.Item2 > l.Goal - 120)) bad.Add("hazard at start or goal");
        void Intro(string kind, float a, float b, bool overPit, float m = 80) { if (overPit) bad.Add($"first {kind} is over a pit"); if (HazardNear(a, b, m)) bad.Add($"first {kind} is not a safe place to learn"); }
        if (l.Introduces.Contains("mud") && l.Muds.Length > 0) { var m = l.Muds.OrderBy(m => m.X).First(); Intro("mud", m.X, m.X + m.W, false, 120); if (l.Rollers.Any(r => Near(Run(r).a, Run(r).b, m.X, m.X + m.W, 120))) bad.Add("first mud has a log near"); }
        if (l.Introduces.Contains("log") && l.Rollers.Length > 0) { var r = l.Rollers.OrderBy(r => Run(r).a).First(); Intro("log", Run(r).a, Run(r).b, false, 120); if (r.Dir > 0) bad.Add("first log rolls with the player (comes from behind)"); }
        if (l.Pickups.Count(p => p.Secret) != 1) bad.Add("needs exactly one secret pickup");
        else if (l.Pickups.First(p => p.Secret).Y < 72) bad.Add("secret pickup is not on a high ledge (y >= 72)");
        if (l.Pickups.Length != pickups) bad.Add($"{l.Pickups.Length} pickups, needs {pickups}");
        foreach (var p in l.Pickups) if (l.Bats.Any(b => Math.Abs(p.X - b.Cx) < b.Rx + 24)) bad.Add($"pickup {p.Kind} {p.X} inside a bat loop");
        foreach (var p in l.Pickups) if (p.Y < 1 && l.Rollers.Any(r => p.X > Run(r).a - 8 && p.X < Run(r).b + 8)) bad.Add($"ground pickup {p.Kind} {p.X} on a log run");
        if (l.Checkpoints.Length != 3) bad.Add("Hard needs 3 checkpoints");
        return bad;
    }

    // Normal lint: the Hard lint with 7 pickups, then the Normal fixes: 2 checkpoints, and a ground-line boat may cross a safe pool.
    public static List<string> NormalLint(Lvl l) {
        var bad = Lint(l, 7).Where(x => x != "Hard needs 3 checkpoints" && !x.StartsWith("moving platform")).ToList();
        foreach (var p in l.Platforms) if (p.Top == 0 && !l.Pits.Concat(l.Pools.Select(q => new Pit(q.X, q.W))).Any(q => Math.Abs(q.X - p.X0) < 1 && p.X1 + 48 >= q.X + q.W)) bad.Add($"moving platform {p.X0} does not span its pit or pool");
        if (l.Checkpoints.Length != 2) bad.Add("Normal needs 2 checkpoints");
        foreach (var u in l.Updrafts) {
            if (l.Pits.Any(p => Near(u.X, u.X + 32, p.X, p.X + p.W, 60))) bad.Add($"updraft {u.X} within 60 px of a pit");
            if (l.Spikes.Any(k => Near(u.X, u.X + 32, k, k + 32, 60))) bad.Add($"updraft {u.X} within 60 px of spikes");
            if (l.Boulders.Any(o => Near(u.X, u.X + 32, o.X, o.X + (o.Big ? 48 : 32), 40))) bad.Add($"updraft {u.X} within 40 px of a boulder");
            if (l.Checkpoints.Any(c => Near(u.X, u.X + 32, c, c, 40))) bad.Add($"updraft {u.X} on a checkpoint");
            if (u.X < 200 || u.X + 32 > l.Goal - 120) bad.Add($"updraft {u.X} at start or goal");
            if (u.Top + 26 > 196) bad.Add($"updraft {u.X} lifts the head above 196");
            if (l.Ledges.Any(e => e.X < u.X + 32 + 6 && e.X + e.W > u.X - 6 && e.Top - 10 < u.Top + 26 && e.Top > 0)) bad.Add($"updraft {u.X} has a ledge in or over its column");
            if (!l.Ledges.Any(e => e.X == u.X + 40 && e.Top == u.Top - 4)) bad.Add($"updraft {u.X} has no landing ledge at x + 40, top - 4");
            if ((l.Sands ?? new Sand[0]).Any(k => Near(u.X, u.X + 32, k.X, k.X + k.W, 0)) || l.Ices.Any(k => Near(u.X, u.X + 32, k.X, k.X + k.W, 0)) || l.Currents.Any(k => Near(u.X, u.X + 32, k.X, k.X + k.W, 0))) bad.Add($"updraft {u.X} on sand, ice or a current");
        }
        if (l.Introduces.Contains("float")) { var f = l.Floats.OrderBy(f => f.X).First(); if (!l.Pools.Any(q => f.X >= q.X && f.X + 48 <= q.X + q.W)) bad.Add("first float is not over a safe pool"); }
        if (l.Introduces.Contains("current")) { var c = l.Currents.OrderBy(c => c.X).First(); if (c.Dir < 0) bad.Add("first current pushes against the player"); }
        return bad;
    }

    static void SelfCheck() {
        // Mud uses the ice numbers: from full walk (84) to a stop is 0.84 s and about 35 px of slide; 0 to 84 takes 0.525 s.
        var mudLvl = HillsHardLevels.H1; var m0 = mudLvl.Muds[0]; var s = new Sim(mudLvl); s.Place(m0.X + 20, 0.5f);
        for (int f = 0; f < 120; f++) s.Step(1, false); float x1 = s.px; int fr = 0; while (s.baseVx > 0 && fr < 300) { s.Step(0, false); fr++; }
        Console.WriteLine($"check: mud {m0.X}: let go at full walk, slides {s.px - x1:F1} px in {fr / 60f:F2} s (ice rule: 84^2/200 = 35.3 px, 0.84 s)");
        s = new Sim(mudLvl); s.Place(m0.X + 20, 0.5f); for (int f = 0; f < 6; f++) s.Step(0, false); fr = 0; while (s.baseVx < Sim.walk && fr < 300) { s.Step(1, false); fr++; }
        Console.WriteLine($"check: mud {m0.X}: 0 to full walk in {fr / 60f:F2} s (84/160 = 0.53 s)");
        // Right log: it catches a player who stands still; a straight-up hop clears it; walking on is never caught; skip rule.
        var h2 = HillsHardLevels.H2; int ri = Array.FindIndex(h2.Rollers, r => r.Dir > 0); var rr = h2.Rollers[ri];
        float tStart = ((Rules.RollStart - rr.Offset) % Rules.RollCycle + Rules.RollCycle) % Rules.RollCycle + Rules.RollCycle; // a roll start after t = 3.5
        s = new Sim(h2, tStart - 0.5f); s.Place(rr.Spawn + 100, 0.5f); float hit = -1; for (int f = 0; f < 240 && hit < 0; f++) { s.Step(0, false); if (s.HitByRoller()) hit = s.t - tStart; }
        Console.WriteLine($"check: right log {rr.Spawn}->{rr.End}: idle 100 px past spawn is hit {hit:F2} s after the roll starts ((100-13)/90 = 0.97 s)");
        s = new Sim(h2, tStart - 0.5f); s.Place(rr.Spawn + 100, 0.5f); bool hurt = false, jumped = false;
        for (int f = 0; f < 200; f++) { float x = s.RollerXNow(ri); bool p = !jumped && s.grounded && !float.IsNaN(x) && s.px - x < 22; if (p) jumped = true; s.Step(0, p); if (s.HitByRoller()) hurt = true; }
        Console.WriteLine($"check: same spot, hop straight up when the log is 22 px behind: {(hurt ? "HIT" : "cleared")}");
        s = new Sim(h2, tStart - 0.5f); s.Place(rr.Spawn + 100, 0.5f); hurt = false;
        for (int f = 0; f < 200; f++) { s.Step(1, false); if (s.HitByRoller()) hurt = true; }
        Console.WriteLine($"check: same spot, walking right at 84 px/s: {(hurt ? "HIT" : "never caught (log 90 px/s gains 6 px/s; its run ends first)")}");
        s = new Sim(h2, tStart - 0.2f); s.Place(rr.Spawn + 20, 0.5f); bool seen = false; for (int f = 0; f < 120; f++) { s.Step(0, false); if (!float.IsNaN(s.RollerXNow(ri))) seen = true; }
        Console.WriteLine($"check: standing 20 px from a right log's spawn when it would start: {(seen ? "ROLLS" : "skipped this cycle")} (skip rule 48 px)");
    }
    static void Report(Lvl l) {
        var r = Bot(l); var ok = ReachableLedges(l);
        var missing = l.Pickups.Where(k => !Reach(l, k, ok)).Select(k => $"{k.Kind}({k.X},{k.Y})").ToList();
        var unreachLedges = Enumerable.Range(0, l.Ledges.Length).Where(i => !ok.Contains(i)).Select(i => $"L({l.Ledges[i].X},{l.Ledges[i].Top})").ToList();
        var lint = NormalLint(l);
        if (l.Updrafts.Length > 0) { var nu = l with { Updrafts = new Updraft[0] }; var okU = ReachableLedges(nu); var need = l.Pickups.Where(k => !Reach(nu, k, okU)).Select(k => $"{k.Kind}({k.X},{k.Y})").ToList(); Console.WriteLine($"   {l.Name} without updrafts: needs an updraft -> pickups [{string.Join(" ", need)}]"); }
        Console.WriteLine($"{l.Name}: bot {(r.ok ? "GOAL" : "FAILED")} {r.t:F1}s ({r.why}); pickups {l.Pickups.Length - missing.Count}/{l.Pickups.Length} reachable"
            + (missing.Count > 0 ? " MISSING " + string.Join(" ", missing) : "") + (unreachLedges.Count > 0 ? " UNREACHABLE LEDGES " + string.Join(" ", unreachLedges) : "")
            + (lint.Count > 0 ? " LINT " + string.Join("; ", lint) : " lint clean"));
        var shifts = Enumerable.Range(1, 13).Select(k => (k * 0.25f, Bot(l, k * 0.25f))).ToList(); var fails = shifts.Where(x => !x.Item2.ok).ToList();
        Console.WriteLine($"   start-time shifts 0.25..3.25 s (every timed object out of phase): {13 - fails.Count}/13 reach the goal, {shifts.Where(x => x.Item2.ok).Min(x => x.Item2.t):F1}-{shifts.Where(x => x.Item2.ok).Max(x => x.Item2.t):F1} s"
            + (fails.Count > 0 ? " FAIL " + string.Join(", ", fails.Select(x => $"{x.Item1}:{x.Item2.why}")) : ""));
        var st = r.st;
        if (l.Muds.Length > 0) Console.WriteLine($"   mud: {l.Muds.Length} stretches, {l.Muds.Sum(m => m.W)} px; bot on mud {st.MudTime:F1} s");
        if (l.Rollers.Length > 0) Console.WriteLine($"   logs: {l.Rollers.Count(x => x.Dir < 0)} left, {l.Rollers.Count(x => x.Dir > 0)} right; bot jumped {st.LogJumps} oncoming, stopped and hopped {st.LogDodges} from behind");
        if (l.Crumbles.Length > 0) Console.WriteLine($"   crumbling ledges: {l.Crumbles.Length}");
        if (l.Floats.Length > 0) Console.WriteLine("   floats (s standing): " + string.Join(", ", l.Floats.Select((f, i) => $"{f.X}:{st.FloatTime[i]:F2}")));
        if (l.Bats.Length > 0) Console.WriteLine("   bats (s in loop span / pushes): " + string.Join(", ", l.Bats.Select((b, i) => $"{b.Cx}:{st.BatTime[i]:F2}/{(st.BatPushes != null ? st.BatPushes[i] : -1)}")));
        if (l.Currents.Length > 0) Console.WriteLine("   currents (s in strip): " + string.Join(", ", l.Currents.Select((c, i) => $"{c.X}{(c.Dir > 0 ? "R" : "L")}:{st.CurrentTime[i]:F2}")));
        if (l.Balloons.Length > 0) Console.WriteLine("   balloons (s from arriving to jumping off): " + string.Join(", ", l.Balloons.Select((b, i) => $"{b.X}:{st.BalloonTime[i]:F2}")));
        if (l.Platforms.Length > 0) { var np = Bot(l with { Platforms = new Platform[0] }); Console.WriteLine($"   without moving platforms: {(np.ok ? "GOAL" : "FAILED")} ({np.why})"); }
        if (l.Balloons.Length > 0) { var nb = Bot(l with { Balloons = new Balloon[0] }); Console.WriteLine($"   without balloons: {(nb.ok ? "GOAL" : "FAILED")} ({nb.why})"); }
    }
    static void Main(string[] args) {
        Console.WriteLine("-- Regression, Plains Normal at walk 84 (expect 83.8-98.4 s):");
        foreach (var l in PlainsNormalLevels.All) { var r = Bot(l); Console.Write($"{l.Name.Split(' ')[0]} {r.t:F1}{(r.ok ? "" : " FAILED " + r.why)}  "); }
        Console.WriteLine();
        string only = args.FirstOrDefault(a => a.StartsWith("N"));
        Console.WriteLine("-- Plateaus Normal:");
        foreach (var l in PlateausNormalLevels.All) if (only == null || l.Name.StartsWith(only)) Report(l);
    }
}
