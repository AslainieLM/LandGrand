// Plains Easy bot driver, reach searches and Easy lint (scratch, not project code, D-069).
// Same bot as Hills Easy (the Hills Hard Program without the Hard objects), plus the straw bounce pile:
// the bot jumps onto a pile like a boulder and is launched; reach searches also try a double jump late in a bounce.
static class Program {
    public const float Limit = 120f;
    public static (bool ok, float t, string why, int bounces) Bot(Lvl l, float t0 = 0) {
        var s = new Sim(l, t0); bool prev = false; float stuckX = -1, stuckT = 0;
        while (s.t < Limit + t0) {
            int dir = 1;
            bool pitAhead = s.grounded && !s.GroundAt(s.px + 7);
            bool spikeAhead = s.grounded && s.spikes.Any(k => k.x - s.px < 22 && k.x - s.px > 0);
            bool wall = s.grounded && s.BlockedAhead(6);
            bool want = (wall || pitAhead || spikeAhead) && s.grounded;
            bool dbl = !s.grounded && !s.airJumpUsed && s.vy <= 0 && (!s.GroundAt(s.px + 10) || s.spikes.Any(k => k.x - s.px < 30 && k.xMax > s.px) || s.BlockedAhead(4));
            bool press = (want || dbl) && !prev; prev = press;
            s.Step(dir, press);
            if (s.OnSpikes()) return (false, s.t - t0, $"spikes at x={s.px:F0}", s.Bounces);
            if (s.InPit) return (false, s.t - t0, $"pit at x={s.px:F0}", s.Bounces);
            if (s.px >= l.Goal - 8) return (true, s.t - t0, "goal", s.Bounces);
            if (Math.Abs(s.px - stuckX) < 0.5f) { stuckT += 1f / 60f; if (stuckT > 12f) return (false, s.t - t0, $"stuck at x={s.px:F0},y={s.py:F0}", s.Bounces); } else { stuckX = s.px; stuckT = 0; }
        }
        return (false, s.t - t0, "time", s.Bounces);
    }

    static List<(float x, float y)> Starts(Lvl l, float x, HashSet<int> ledgesOk) {
        var starts = new List<(float x, float y)>();
        foreach (var dx in new[] { -160f, -120f, -90f, -60f, -40f, -20f, 20f, 40f, 60f }) starts.Add((x + dx, 0));
        for (int i = 0; i < l.Ledges.Length; i++) { var e = l.Ledges[i]; if (ledgesOk.Contains(i) && Math.Abs(e.X - x) < 300) starts.Add((e.X + e.W / 2, e.Top)); }
        foreach (var b in l.Boulders) if (Math.Abs(b.X - x) < 300) starts.Add((b.X + (b.Big ? 24 : 16), b.Big ? 35 : 23));
        return starts;
    }
    // Can some (start spot, jump frame, double-jump delay, direction) meet the test and land safely?
    static bool Search(Lvl l, float x, HashSet<int> ledgesOk, Func<Sim, bool> hit, bool needLandAfter) {
        foreach (var (sx, sy) in Starts(l, x, ledgesOk)) foreach (int dir in new[] { 1, -1 }) for (int j = 0; j < 40; j += 3) for (int d = -1; d < 90; d += 3) {
            var s = new Sim(l); s.Place(sx, sy + 0.5f); for (int q = 0; q < 6; q++) s.Step(0, false);
            if (!s.grounded || s.InPit || s.OnSpikes()) break;
            bool got = false;
            for (int f = 0; f < 200; f++) {
                s.Step(dir, f == j || f == j + d);
                if (s.InPit || s.OnSpikes()) break;
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
    // Easy rules: 5 pickups, 1 checkpoint, one secret; nothing within 100 px of the checkpoint; no hazard at start or goal;
    // straw piles: never within 60 px of a pit or spikes, 40 px of a boulder or the checkpoint; the first pile is safe (80 px).
    public static List<string> Lint(Lvl l) {
        var bad = new List<string>();
        foreach (var c in l.Checkpoints) {
            if (l.Pits.Any(p => Near(p.X, p.X + p.W, c, c, 100))) bad.Add($"pit near checkpoint {c}");
            if (l.Spikes.Any(s => Near(s, s + 32, c, c, 100))) bad.Add($"spikes near checkpoint {c}");
            if (l.Boulders.Any(o => o.X < c + 60 && o.X + 48 > c - 60)) bad.Add($"boulder on checkpoint {c}");
            if (l.Straws.Any(k => Near(k.X, k.X + 48, c, c, 40))) bad.Add($"straw pile on checkpoint {c}");
        }
        var hazX = l.Pits.Select(p => (p.X, p.X + p.W)).Concat(l.Spikes.Select(s => (s, s + 32)));
        if (hazX.Any(h => h.Item1 < 200) || hazX.Any(h => h.Item2 > l.Goal - 120)) bad.Add("hazard at start or goal");
        foreach (var k in l.Straws) {
            if (l.Pits.Any(p => Near(k.X, k.X + 48, p.X, p.X + p.W, 60))) bad.Add($"straw {k.X} within 60 px of a pit");
            if (l.Spikes.Any(s => Near(k.X, k.X + 48, s, s + 32, 60))) bad.Add($"straw {k.X} within 60 px of spikes");
            if (l.Boulders.Any(o => Near(k.X, k.X + 48, o.X, o.X + (o.Big ? 48 : 32), 40))) bad.Add($"straw {k.X} within 40 px of a boulder");
            if (k.X < 200 || k.X + 48 > l.Goal - 120) bad.Add($"straw {k.X} at start or goal");
        }
        if (l.Straws.Length > 0) { var f = l.Straws.OrderBy(k => k.X).First(); if (l.Pits.Any(p => Near(f.X, f.X + 48, p.X, p.X + p.W, 80)) || l.Spikes.Any(s => Near(f.X, f.X + 48, s, s + 32, 80))) bad.Add("first straw pile is not a safe place to learn"); }
        if (l.Pickups.Count(p => p.Secret) != 1) bad.Add("needs exactly one secret pickup");
        if (l.Pickups.Length != 5) bad.Add($"{l.Pickups.Length} pickups, needs 5");
        if (l.Checkpoints.Length != 1) bad.Add("Easy needs 1 checkpoint");
        return bad;
    }

    static void SelfCheck() {
        // A bounce: walk-jump onto a pile on flat ground and measure the highest foot height after the launch.
        var e3 = PlainsEasyLevels.E3; var k0 = e3.Straws[0]; var s = new Sim(e3); s.Place(k0.X - 30, 0.5f); for (int q = 0; q < 6; q++) s.Step(0, false);
        float top = 0; bool jumped = false; for (int f = 0; f < 150; f++) { bool p = !jumped && s.BlockedAhead(6); if (p) jumped = true; s.Step(1, p); if (s.Bounces > 0) top = Math.Max(top, s.py); }
        Console.WriteLine($"check: straw {k0.X}: bounces {s.Bounces}, highest feet {top:F1} px (15 + 400^2/1800 = 103.9)");
        // The same pile with a double jump pressed at the top of the bounce.
        s = new Sim(e3); s.Place(k0.X - 30, 0.5f); for (int q = 0; q < 6; q++) s.Step(0, false); jumped = false; bool dj = false; top = 0;
        for (int f = 0; f < 150; f++) { bool p = !jumped && s.BlockedAhead(6); if (p) jumped = true; bool d = !dj && s.Bounces > 0 && s.vy <= 0; if (d) dj = true; s.Step(1, p || d); top = Math.Max(top, s.py); }
        Console.WriteLine($"check: straw {k0.X} plus a double jump at the top: highest feet {top:F1} px (+ 230^2/1800 = 29.4)");
        // Walking into the pile's side never bounces.
        s = new Sim(e3); s.Place(k0.X - 30, 0.5f); for (int f = 0; f < 120; f++) s.Step(1, false);
        Console.WriteLine($"check: walking into the pile's side: bounces {s.Bounces}, x {s.px:F1} (stops at {k0.X + 1 - Sim.W / 2})");
    }
    static void Report(Lvl l) {
        var r = Bot(l); var ok = ReachableLedges(l);
        var missing = l.Pickups.Where(k => !Reach(l, k, ok)).Select(k => $"{k.Kind}({k.X},{k.Y})").ToList();
        var unreachLedges = Enumerable.Range(0, l.Ledges.Length).Where(i => !ok.Contains(i)).Select(i => $"L({l.Ledges[i].X},{l.Ledges[i].Top})").ToList();
        var lint = Lint(l);
        Console.WriteLine($"{l.Name}: bot {(r.ok ? "GOAL" : "FAILED")} {r.t:F1}s ({r.why}); bounces {r.bounces}; pickups {l.Pickups.Length - missing.Count}/{l.Pickups.Length} reachable"
            + (missing.Count > 0 ? " MISSING " + string.Join(" ", missing) : "") + (unreachLedges.Count > 0 ? " UNREACHABLE LEDGES " + string.Join(" ", unreachLedges) : "")
            + (lint.Count > 0 ? " LINT " + string.Join("; ", lint) : " lint clean"));
        if (l.Straws.Length > 0) {
            // Which pickups and ledges need a straw pile? Remove every pile and search again.
            var noStraw = l with { Straws = new Straw[0] }; var ok2 = ReachableLedges(noStraw);
            var needs = l.Pickups.Where(k => !Reach(noStraw, k, ok2)).Select(k => $"{k.Kind}({k.X},{k.Y})").ToList();
            var needL = Enumerable.Range(0, l.Ledges.Length).Where(i => !ok2.Contains(i)).Select(i => $"L({l.Ledges[i].X},{l.Ledges[i].Top})").ToList();
            Console.WriteLine($"   without straw piles: needs a pile -> pickups [{string.Join(" ", needs)}], ledges [{string.Join(" ", needL)}]");
        }
    }
    static void Main(string[] args) {
        SelfCheck();
        Console.WriteLine("-- Regression, Hills Easy at walk 84 (expect E1 56.8, E2 62.4, E3 70.1):");
        foreach (var l in HillsEasyLevels.All) { var r = Bot(l); Console.Write($"{l.Name.Split(' ')[0]} {r.t:F1}{(r.ok ? "" : " FAILED " + r.why)}  "); }
        Console.WriteLine();
        Console.WriteLine("-- Plains Easy:");
        foreach (var l in PlainsEasyLevels.All) Report(l);
    }
}
