// Scratch physics bot for Hills Normal (not project code). Copied from the Hills Easy bot (Mountains Hard bot plus soft sand),
// with four new objects: the sinking float, the gentle bat, the river current and the vertical balloon.
// The constants in Rules are the proposed game rules for these objects.
record struct R(float x, float y, float w, float h) {
    public float xMax => x + w; public float yMax => y + h;
    public bool Overlaps(R o) => o.xMax > x && o.x < xMax && o.yMax > y && o.y < yMax;
}
record Pit(float X, float W);
record Boulder(float X, bool Big);
record Ledge(float X, float Top, float W);
record Crumble(float X, float Top);
record Rock(float X, float Offset);
record Platform(float X0, float X1, float Top);
record Wind(float X, float W, float Offset);
record Vent(float X);
record Roller(float Spawn, float End, float Offset); // middle starts at Spawn, rolls left, gone when the middle passes End
record Ice(float X, float W);                        // icy ground from X to X + W
record Sand(float X, float W);                       // soft sand from X to X + W
record Pickup(string Kind, float X, float Y, bool Secret = false);
// New in Hills Normal:
record Pool(float X, float W);                       // shallow safe water: a dip with a floor at -16 (not a hazard)
record Float(float X);                               // sinking float, left edge X, top at the water line (0) when up
record Bat(float Cx, float Cy, float Rx, float Ry, float Period, float Phase); // ellipse loop; phase in seconds
record Current(float X, float W, int Dir);           // river current strip; Dir +1 pushes right, -1 pushes left
record Balloon(float X, float Low, float High, float Phase); // vertical balloon, left edge X; at phase 0 it is at Low going up
record Lvl(string Name, float Width, float Goal, float[] Checkpoints, Pit[] Pits, float[] Spikes, Boulder[] Boulders, Ledge[] Ledges,
           Crumble[] Crumbles, Rock[] Rocks, Platform[] Platforms, Wind[] Winds, Vent[] Vents, Roller[] Rollers, Ice[] Ices, Pickup[] Pickups, Sand[] Sands) {
    public Pool[] Pools { get; init; } = new Pool[0];
    public Float[] Floats { get; init; } = new Float[0];
    public Bat[] Bats { get; init; } = new Bat[0];
    public Current[] Currents { get; init; } = new Current[0];
    public Balloon[] Balloons { get; init; } = new Balloon[0];
    public string[] Introduces { get; init; } = new string[0]; // object types first met in this level ("float", "bat", "current", "balloon")
}

static class Rules {
    public const float CrumbleBreak = 0.5f, CrumbleReturn = 3f;
    public const float RockCycle = 3f, RockFallStart = 1.2f, RockLand = 1.5f, RockGone = 1.65f;
    public const float PlatformSpeed = 40f;
    public const float WindCycle = 4f, WindWarn = 2f, WindPush = 2.6f, WindSpeed = 40f;
    public const float VentCycle = 2f, VentPuffAt = 1.55f, VentLaunch = 480f;
    public const float RollCycle = 3.5f, RollStart = 0.8f, RollSpeed = 90f, RollSize = 14f, RollSkipNear = 48f;
    public static long RollCycleIndex(Roller r, float t) => (long)Math.Floor((t + r.Offset) / RollCycle);
    public const float IceSpeedUp = 160f, IceSlowDown = 100f;
    public const float SandWalk = 0.6f; // share of the walking speed left on soft sand

    // Sinking float
    public const float FloatWidth = 48f;      // platform width
    public const float FloatWait = 0.6f;      // seconds standing on it before it starts to sink (restarts on every landing)
    public const float FloatSinkSpeed = 8f;   // px/s down while the explorer stands on it after the wait
    public const float FloatSunkDepth = 16f;  // at this depth (2 s of sinking) it is under water and holds nothing
    public const float FloatRiseDelay = 1.5f; // seconds after the explorer leaves before it rises
    public const float FloatRiseSpeed = 16f;  // px/s up (back at the water line 1 s after it starts rising from fully sunk)
    public const float PoolDepth = 16f;       // shallow safe water floor under a practice float
    // Gentle bat
    public const float BatW = 16f, BatH = 12f; // hit box around the bat's middle
    public const float BatPushDistance = 24f;  // how far one touch pushes the explorer
    public const float BatPushSeconds = 0.2f;  // the push is spread over this time (120 px/s), the walk input is ignored meanwhile
    public const float BatRest = 1f;           // seconds after a push when that bat cannot push again
    // River current
    public const float CurrentSpeed = 30f;     // px/s push while grounded on the flat ground line inside the strip
    // Vertical balloon
    public const float BalloonWidth = 48f, BalloonSpeed = 40f, BalloonMinRise = 48f, BalloonMaxRise = 96f;

    static float Phase(float t, float cycle, float offset) => ((t + offset) % cycle + cycle) % cycle;
    public static float RockY(Rock r, float t) { float p = Phase(t, RockCycle, r.Offset); if (p < RockFallStart || p >= RockGone) return float.NaN; if (p >= RockLand) return 0; return 270f * (1 - (p - RockFallStart) / (RockLand - RockFallStart)); }
    public static float RockPhase(Rock r, float t) => Phase(t, RockCycle, r.Offset);
    public static float PlatformX(Platform p, float t) { float span = p.X1 - p.X0; if (span <= 0) return p.X0; float d = (t * PlatformSpeed) % (2 * span); return p.X0 + (d < span ? d : 2 * span - d); }
    public static bool Pushing(Wind w, float t) => Phase(t, WindCycle, w.Offset) >= WindPush;
    public static float WindPhase(Wind w, float t) => Phase(t, WindCycle, w.Offset);
    public static bool VentPuff(float t) => Phase(t, VentCycle, 0) >= VentPuffAt;
    public static float RollerX(Roller r, float t) { float p = Phase(t, RollCycle, r.Offset); if (p < RollStart) return float.NaN; float x = r.Spawn - (p - RollStart) * RollSpeed; return x < r.End ? float.NaN : x; }
    public static float RollerPhase(Roller r, float t) => Phase(t, RollCycle, r.Offset);
    public static (float x, float y) BatAt(Bat b, float t) { double a = 2 * Math.PI * Phase(t, b.Period, b.Phase) / b.Period; return (b.Cx + b.Rx * (float)Math.Cos(a), b.Cy + b.Ry * (float)Math.Sin(a)); }
    public static float BalloonTop(Balloon b, float t) { float span = b.High - b.Low; float d = Phase(t * BalloonSpeed, 2 * span, b.Phase * BalloonSpeed); return b.Low + (d < span ? d : 2 * span - d); }
    public static bool BalloonRising(Balloon b, float t) { float span = b.High - b.Low; return Phase(t * BalloonSpeed, 2 * span, b.Phase * BalloonSpeed) < span; }
}

class Sim {
    public Lvl L; public float px, py, vx, vy, t, baseVx; public bool grounded, jumpedFromGround, airJumpUsed; float lastGrounded = -1;
    public const float W = 12, H = 26, walk = 84, g = 900, maxFall = 420, jump = 290, dj = 230, coyote = 0.08f;
    List<R> statics = new(); public List<R> spikes = new();
    float[] crumbleTouched; float[] crumbleBroke; int standingPlatform = -1; long[] rollSkipped;
    // new objects
    public float[] floatDepth, floatStand, floatOff; public int standFloat = -1, standBalloon = -1;
    public float[] batLast; public int[] batPushes; float pushLeft; int pushDir;
    public Sim(Lvl l, float t0 = 0) {
        L = l; t = t0; crumbleTouched = Enumerable.Repeat(-1f, l.Crumbles.Length).ToArray(); crumbleBroke = Enumerable.Repeat(-99f, l.Crumbles.Length).ToArray(); rollSkipped = Enumerable.Repeat(long.MinValue, l.Rollers.Length).ToArray();
        floatDepth = new float[l.Floats.Length]; floatStand = new float[l.Floats.Length]; floatOff = Enumerable.Repeat(99f, l.Floats.Length).ToArray();
        batLast = Enumerable.Repeat(-99f, l.Bats.Length).ToArray(); batPushes = new int[l.Bats.Length];
        float from = 0;
        var gaps = l.Pits.Select(p => (p.X, p.W)).Concat(l.Pools.Select(p => (p.X, p.W))).OrderBy(p => p.X).Append((X: l.Width, W: 0f));
        foreach (var p in gaps) { statics.Add(new R(from, -64, p.X - from, 64)); from = p.X + p.W; }
        foreach (var p in l.Pools) statics.Add(new R(p.X, -64, p.W, 64 - Rules.PoolDepth));
        foreach (var b in l.Boulders) { float bw = b.Big ? 48 : 32, bh = b.Big ? 36 : 24; statics.Add(new R(b.X + 1, 0, bw - 2, bh - 1)); }
        foreach (var e in l.Ledges) statics.Add(new R(e.X, e.Top - 10, e.W, 10));
        foreach (var s in l.Spikes) spikes.Add(new R(s + 3, 0, 26, 8));
        px = 40; py = 0;
    }
    public void Place(float x, float y) { px = x; py = y; vx = vy = 0; baseVx = 0; grounded = false; }
    bool CrumbleSolid(int i) => !(t - crumbleBroke[i] < Rules.CrumbleReturn);
    public IEnumerable<(R r, int plat)> Solids() {
        foreach (var s in statics) yield return (s, -1);
        for (int i = 0; i < L.Crumbles.Length; i++) if (CrumbleSolid(i)) yield return (new R(L.Crumbles[i].X, L.Crumbles[i].Top - 10, 32, 10), -1);
        for (int i = 0; i < L.Platforms.Length; i++) yield return (new R(Rules.PlatformX(L.Platforms[i], t), L.Platforms[i].Top - 10, 48, 10), i);
    }
    public bool FloatSolid(int i) => floatDepth[i] < Rules.FloatSunkDepth;
    // Tops that hold the explorer only from above (like the game's dynamic slabs): kind 0 = float, 1 = balloon.
    public IEnumerable<(R r, int kind, int idx)> OneWays() {
        for (int i = 0; i < L.Floats.Length; i++) if (FloatSolid(i)) yield return (new R(L.Floats[i].X, -floatDepth[i] - 10, Rules.FloatWidth, 10), 0, i);
        for (int i = 0; i < L.Balloons.Length; i++) yield return (new R(L.Balloons[i].X, Rules.BalloonTop(L.Balloons[i], t) - 10, Rules.BalloonWidth, 10), 1, i);
    }
    public float Push() => L.Winds.Any(w => px > w.X && px < w.X + w.W && Rules.Pushing(w, t)) ? -Rules.WindSpeed : 0;
    bool OnFlatGround => grounded && Math.Abs(py) < 0.5f && standFloat < 0 && standBalloon < 0 && standingPlatform < 0;
    public bool OnIce => grounded && Math.Abs(py) < 0.5f && L.Ices.Any(i => px > i.X && px < i.X + i.W);
    public bool OnSand => grounded && Math.Abs(py) < 0.5f && L.Sands.Any(s => px > s.X && px < s.X + s.W);
    public int CurrentDir => OnFlatGround ? L.Currents.Where(c => px >= c.X && px <= c.X + c.W).Select(c => c.Dir).FirstOrDefault() : 0;
    public float SandTime;
    public bool Pushed => pushLeft > 0;
    public void Step(int dir, bool press, float dt = 1f / 60f) {
        if (standingPlatform >= 0) { var p = L.Platforms[standingPlatform]; px += Rules.PlatformX(p, t + dt) - Rules.PlatformX(p, t); }
        if (standBalloon >= 0) { var b = L.Balloons[standBalloon]; py += Rules.BalloonTop(b, t + dt) - Rules.BalloonTop(b, t); }
        for (int i = 0; i < L.Floats.Length; i++) {
            float d0 = floatDepth[i];
            if (standFloat == i) { floatOff[i] = 0; floatStand[i] += dt; if (floatStand[i] > Rules.FloatWait) floatDepth[i] = Math.Min(Rules.FloatSunkDepth, d0 + Rules.FloatSinkSpeed * dt); py -= floatDepth[i] - d0; }
            else { floatStand[i] = 0; floatOff[i] += dt; if (floatOff[i] >= Rules.FloatRiseDelay) floatDepth[i] = Math.Max(0, d0 - Rules.FloatRiseSpeed * dt); }
        }
        t += dt;
        for (int i = 0; i < L.Rollers.Length; i++) { var r = L.Rollers[i]; float ph = Rules.RollerPhase(r, t); if (ph >= Rules.RollStart && ph - dt < Rules.RollStart && Math.Abs(px - r.Spawn) < Rules.RollSkipNear) rollSkipped[i] = Rules.RollCycleIndex(r, t); }
        float target = dir * walk * (OnSand ? Rules.SandWalk : 1f); if (OnSand) SandTime += dt;
        if (OnIce) {
            bool speedingUp = target != 0 && (baseVx == 0 || Math.Sign(target) == Math.Sign(baseVx)) && Math.Abs(target) > Math.Abs(baseVx);
            float a = speedingUp ? Rules.IceSpeedUp : Rules.IceSlowDown;
            baseVx = baseVx < target ? Math.Min(target, baseVx + a * dt) : Math.Max(target, baseVx - a * dt);
        } else baseVx = target;
        vx = baseVx + Push() + CurrentDir * Rules.CurrentSpeed;
        if (pushLeft > 0) { vx = pushDir * Rules.BatPushDistance / Rules.BatPushSeconds; pushLeft -= dt; }
        bool canGround = grounded || (t - lastGrounded <= coyote && !jumpedFromGround);
        if (press && canGround) { vy = jump; jumpedFromGround = true; grounded = false; }
        else if (press && !grounded && !airJumpUsed) { vy = dj; airJumpUsed = true; }
        vy = Math.Max(vy - g * dt, -maxFall); MoveX(vx * dt); MoveY(vy * dt); px = Math.Clamp(px, W / 2, L.Width - W / 2);
        for (int i = 0; i < L.Crumbles.Length; i++) {
            var c = L.Crumbles[i]; bool on = grounded && Math.Abs(py - c.Top) < 0.5f && px + W / 2 > c.X && px - W / 2 < c.X + 32 && CrumbleSolid(i);
            if (on && crumbleTouched[i] < 0) crumbleTouched[i] = t;
            if (crumbleTouched[i] >= 0 && t - crumbleTouched[i] >= Rules.CrumbleBreak) { crumbleBroke[i] = t; crumbleTouched[i] = -1; }
        }
        for (int i = 0; i < L.Bats.Length; i++) {
            var (bx, by) = Rules.BatAt(L.Bats[i], t);
            if (t - batLast[i] >= Rules.BatRest && Body.Overlaps(new R(bx - Rules.BatW / 2, by - Rules.BatH / 2, Rules.BatW, Rules.BatH))) {
                batLast[i] = t; batPushes[i]++; pushLeft = Rules.BatPushSeconds;
                pushDir = px < bx ? -1 : px > bx ? 1 : (dir != 0 ? -dir : -1); // away from the bat; straight under it, against the walk
            }
        }
        if (grounded) foreach (var v in L.Vents) if (px > v.X && px < v.X + 24 && Math.Abs(py) < 0.5f && Rules.VentPuff(t)) Launch(Rules.VentLaunch);
    }
    public void Launch(float s) { vy = s; grounded = false; jumpedFromGround = true; airJumpUsed = false; standingPlatform = -1; standFloat = -1; standBalloon = -1; }
    void MoveX(float dx) { if (dx == 0) return; float next = px + dx; var box = new R(next - W / 2, py + 0.01f, W, H - 0.02f);
        foreach (var (s, _) in Solids()) { if (!box.Overlaps(s)) continue; next = dx > 0 ? s.x - W / 2 : s.xMax + W / 2; box = box with { x = next - W / 2 }; baseVx = 0; } px = next; }
    void MoveY(float dy) { float next = py + dy, left = px - W / 2, right = px + W / 2; bool landed = false; int plat = -1, fl = -1, bl = -1;
        if (dy <= 0) {
            foreach (var (s, pi) in Solids()) { if (right <= s.x || left >= s.xMax) continue; if (py >= s.yMax - 0.01f && next <= s.yMax) { next = s.yMax; landed = true; plat = pi; fl = bl = -1; } }
            foreach (var (s, kind, idx) in OneWays()) { if (right <= s.x || left >= s.xMax) continue; if (py >= s.yMax - 0.01f && next <= s.yMax) { next = s.yMax; landed = true; plat = -1; fl = kind == 0 ? idx : -1; bl = kind == 1 ? idx : -1; } }
        }
        else foreach (var (s, _) in Solids()) { if (right <= s.x || left >= s.xMax) continue; float head = py + H; if (head <= s.y + 0.01f && next + H > s.y) { next = s.y - H; vy = 0; } }
        py = next; grounded = landed; standingPlatform = landed ? plat : -1; standFloat = landed ? fl : -1; standBalloon = landed ? bl : -1;
        if (landed) { vy = 0; lastGrounded = t; jumpedFromGround = false; airJumpUsed = false; } }
    public R Body => new(px - W / 2, py, W, H);
    public bool OnSpikes() => spikes.Any(s => Body.Overlaps(s));
    public bool HitByRock() => L.Rocks.Any(r => { float y = Rules.RockY(r, t); return !float.IsNaN(y) && Body.Overlaps(new R(r.X - 8, y, 16, 16)); });
    public float RollerXNow(int i) { var r = L.Rollers[i]; return rollSkipped[i] == Rules.RollCycleIndex(r, t) ? float.NaN : Rules.RollerX(r, t); }
    public bool HitByRoller() => Enumerable.Range(0, L.Rollers.Length).Any(i => { float x = RollerXNow(i); return !float.IsNaN(x) && Body.Overlaps(new R(x - Rules.RollSize / 2, 0, Rules.RollSize, Rules.RollSize)); });
    public bool InPit => py < -40;
    public bool GroundAt(float x) => Solids().Any(p => x > p.r.x && x < p.r.xMax && p.r.yMax <= 0.5f && p.r.yMax > -0.5f)
                                  || OneWays().Any(p => p.kind == 0 && x > p.r.x && x < p.r.xMax && p.r.yMax <= 0.5f && p.r.yMax > -0.5f);
    public bool BlockedAhead(float d) { var b = new R(px - W / 2 + d, py + 0.5f, W, H - 1); return Solids().Any(s => b.Overlaps(s.r)); }
    public bool OnPlatform => standingPlatform >= 0;
    public int PlatformIndex => standingPlatform;
}

static class Program {
    public const float Limit = 240f;
    static float lastSand;
    static float SandOf(Lvl l) => lastSand;
    public class Stats { public float[] FloatTime, CurrentTime, BalloonTime, BatTime; public int[] BatPushes; }
    public static (bool ok, float t, string why, Stats st) Bot(Lvl l) {
        var s = new Sim(l); bool prev = false; float stuckX = -1, stuckT = 0;
        var st = new Stats { FloatTime = new float[l.Floats.Length], CurrentTime = new float[l.Currents.Length], BalloonTime = new float[l.Balloons.Length], BatTime = new float[l.Bats.Length] };
        var ridden = new bool[l.Balloons.Length]; var balloonStart = Enumerable.Repeat(-1f, l.Balloons.Length).ToArray();
        while (s.t < Limit) {
            int dir = 1; bool forceJump = false, holdJump = false;
            foreach (var r in l.Rocks) { float dx = r.X - s.px, ph = Rules.RockPhase(r, s.t); if (dx > 10 && dx < 44 && ph > 0.7f && ph < Rules.RockGone + 0.05f && s.grounded) dir = 0; }
            var pitAheadObj = l.Pits.FirstOrDefault(p => p.X - s.px > 0 && p.X - s.px < 14);
            if (s.grounded && !s.OnPlatform && pitAheadObj != null) {
                int pi = Array.FindIndex(l.Platforms, p => Math.Abs(p.X0 - pitAheadObj.X) < 1 && p.Top == 0);
                if (pi >= 0) { float x = Rules.PlatformX(l.Platforms[pi], s.t); dir = (s.px < pitAheadObj.X - 10 || x - l.Platforms[pi].X0 < 4) ? 1 : 0; }
                foreach (var w in l.Winds) if (s.px > w.X - 80 && s.px < w.X + w.W) { float ph = Rules.WindPhase(w, s.t); if (ph > Rules.WindWarn - 1.0f) dir = 0; }
            }
            if (s.OnPlatform) { var p = l.Platforms[s.PlatformIndex]; float x = Rules.PlatformX(p, s.t); dir = x > p.X1 - 24 ? 1 : 0; if (s.px < x + 18) dir = 1; }
            // Balloons: wait near the basket until it is low, hop on, ride to the top, then jump off to the right.
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
            bool platPit = pitAheadObj != null && l.Platforms.Any(p => Math.Abs(p.X0 - pitAheadObj.X) < 1 && p.Top == 0);
            bool pitAhead = s.grounded && dir == 1 && !s.GroundAt(s.px + 7) && !platPit && !s.OnPlatform;
            bool spikeAhead = s.grounded && dir == 1 && s.spikes.Any(k => k.x - s.px < 22 && k.x - s.px > 0);
            bool wall = s.grounded && dir == 1 && s.BlockedAhead(6);
            bool rollerAhead = s.grounded && Math.Abs(s.py) < 0.5f && Enumerable.Range(0, l.Rollers.Length).Any(i => { float x = s.RollerXNow(i); return !float.IsNaN(x) && x - s.px > 0 && x - s.px < 44; });
            bool want = forceJump || ((wall || pitAhead || spikeAhead || rollerAhead) && s.grounded && !holdJump);
            bool dbl = !s.grounded && !s.airJumpUsed && s.vy <= 0 && (!s.GroundAt(s.px + 10) || s.spikes.Any(k => k.x - s.px < 30 && k.xMax > s.px) || s.BlockedAhead(4));
            if (!s.grounded) dir = 1;
            bool press = (want || dbl) && !prev; prev = press;
            int sf = s.standFloat, sb = s.standBalloon;
            s.Step(dir, press);
            if (sf >= 0) st.FloatTime[sf] += 1f / 60f;
            for (int i = 0; i < l.Currents.Length; i++) if (s.grounded && Math.Abs(s.py) < 0.5f && s.px >= l.Currents[i].X && s.px <= l.Currents[i].X + l.Currents[i].W) st.CurrentTime[i] += 1f / 60f;
            for (int i = 0; i < l.Bats.Length; i++) { var b = l.Bats[i]; if (s.px > b.Cx - b.Rx - Rules.BatW && s.px < b.Cx + b.Rx + Rules.BatW) st.BatTime[i] += 1f / 60f; }
            for (int i = 0; i < l.Balloons.Length; i++) if (balloonStart[i] >= 0 && st.BalloonTime[i] == 0 && sb == i && s.standBalloon != i) st.BalloonTime[i] = s.t - balloonStart[i];
            if (s.OnSpikes()) return (false, s.t, $"spikes at x={s.px:F0}", st);
            if (s.HitByRock()) return (false, s.t, $"rock at x={s.px:F0}", st);
            if (s.HitByRoller()) return (false, s.t, $"roller at x={s.px:F0}", st);
            if (s.InPit) return (false, s.t, $"pit at x={s.px:F0}", st);
            if (s.px >= l.Goal - 8) { lastSand = s.SandTime; st.BatPushes = s.batPushes; return (true, s.t, "goal", st); }
            if (Math.Abs(s.px - stuckX) < 0.5f) { stuckT += 1f / 60f; if (stuckT > 12f) return (false, s.t, $"stuck at x={s.px:F0},y={s.py:F0}", st); } else { stuckX = s.px; stuckT = 0; }
        }
        return (false, s.t, "time", st);
    }

    // Start spots for the reach searches: ground near the target, boulder tops, reachable ledges, balloon baskets, floats.
    static List<(float x, float y, int balloon)> Starts(Lvl l, float x, HashSet<int> ledgesOk) {
        var starts = new List<(float x, float y, int balloon)>();
        foreach (var dx in new[] { -120f, -90f, -60f, -40f, -20f, 20f, 40f, 60f }) starts.Add((x + dx, 0, -1));
        for (int i = 0; i < l.Ledges.Length; i++) { var e = l.Ledges[i]; if (ledgesOk.Contains(i) && Math.Abs(e.X - x) < 300) starts.Add((e.X + e.W / 2, e.Top, -1)); }
        foreach (var c in l.Crumbles) if (Math.Abs(c.X - x) < 300) starts.Add((c.X + 16, c.Top, -1));
        foreach (var b in l.Boulders) if (Math.Abs(b.X - x) < 300) starts.Add((b.X + (b.Big ? 24 : 16), b.Big ? 35 : 23, -1));
        foreach (var f in l.Floats) if (Math.Abs(f.X - x) < 300) starts.Add((f.X + 24, 0, -1));
        for (int i = 0; i < l.Balloons.Length; i++) if (Math.Abs(l.Balloons[i].X - x) < 300 && l.Balloons[i].Low <= 40) starts.Add((l.Balloons[i].X + 24, 0, i));
        return starts;
    }
    // Can some (start time, start spot, jump frame, double-jump delay, direction) meet the goal test and land safely?
    static bool Search(Lvl l, float x, HashSet<int> ledgesOk, Func<Sim, bool> hit, bool needLandAfter) {
        foreach (var (sx0, sy0, bal) in Starts(l, x, ledgesOk)) foreach (var t0 in new[] { 0f, 0.5f, 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4.5f, 6f })
            foreach (int dir in new[] { 1, -1 }) for (int j = 0; j < 40; j += 3) for (int d = -1; d < 40; d += 3) {
                var s = new Sim(l, t0); float sy = bal >= 0 ? Rules.BalloonTop(l.Balloons[bal], t0) : sy0;
                s.Place(sx0, sy + 0.5f); for (int q = 0; q < 6; q++) s.Step(0, false);
                if (!s.grounded || s.InPit || s.OnSpikes() || s.HitByRoller()) break;
                bool got = false;
                for (int f = 0; f < 160; f++) {
                    s.Step(dir, f == j || f == j + d);
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
    // Static checks on the layout.
    static List<string> Lint(Lvl l) {
        var bad = new List<string>();
        foreach (var c in l.Checkpoints) {
            if (l.Pits.Any(p => p.X < c + 100 && p.X + p.W > c - 100)) bad.Add($"pit near checkpoint {c}");
            if (l.Pools.Any(p => p.X < c + 100 && p.X + p.W > c - 100)) bad.Add($"pool near checkpoint {c}");
            if (l.Spikes.Any(s => s < c + 100 && s + 32 > c - 100)) bad.Add($"spikes near checkpoint {c}");
            if (l.Boulders.Any(o => o.X < c + 60 && o.X + 48 > c - 60)) bad.Add($"boulder on checkpoint {c}");
            if (l.Bats.Any(b => Near(b.Cx - b.Rx - 8, b.Cx + b.Rx + 8, c, c, 100))) bad.Add($"bat near checkpoint {c}");
            if (l.Currents.Any(k => Near(k.X, k.X + k.W, c, c, 40))) bad.Add($"current on checkpoint {c}");
            if (l.Balloons.Any(b => Near(b.X, b.X + 48, c, c, 40))) bad.Add($"balloon on checkpoint {c}");
        }
        bool HazardNear(float a, float b, float m) => l.Pits.Any(p => Near(a, b, p.X, p.X + p.W, m)) || l.Spikes.Any(s => Near(a, b, s, s + 32, m));
        foreach (var b in l.Bats) { if (HazardNear(b.Cx - b.Rx - 8, b.Cx + b.Rx + 8, 80)) bad.Add($"hazard within 80 px of bat {b.Cx}"); if (b.Cy - b.Ry - 6 > 20 || b.Cy + b.Ry - 6 < 6) bad.Add($"bat {b.Cx} not at body height"); }
        foreach (var k in l.Currents) if (HazardNear(k.X, k.X + k.W, 80)) bad.Add($"hazard within 80 px of current {k.X}");
        foreach (var b in l.Balloons) { float rise = b.High - b.Low; if (rise < Rules.BalloonMinRise || rise > Rules.BalloonMaxRise) bad.Add($"balloon {b.X} rise {rise}"); if (l.Pits.Any(p => Near(b.X, b.X + 48, p.X, p.X + p.W, 0))) bad.Add($"balloon {b.X} over a pit"); }
        foreach (var f in l.Floats) {
            var pit = l.Pits.FirstOrDefault(p => f.X >= p.X && f.X + 48 <= p.X + p.W); var pool = l.Pools.FirstOrDefault(p => f.X >= p.X && f.X + 48 <= p.X + p.W);
            if (pit == null && pool == null) { bad.Add($"float {f.X} not inside water"); continue; }
            float wx = pit?.X ?? pool.X, ww = pit?.W ?? pool.W;
            var edges = new List<float> { wx }; foreach (var o in l.Floats.Where(o => o.X >= wx && o.X + 48 <= wx + ww).OrderBy(o => o.X)) { edges.Add(o.X); edges.Add(o.X + 48); } edges.Add(wx + ww);
            for (int i = 0; i + 1 < edges.Count; i += 2) if (edges[i + 1] - edges[i] > 40) bad.Add($"water gap {edges[i + 1] - edges[i]} px at {edges[i]} wider than one jump");
        }
        var hazX = l.Pits.Select(p => (p.X, p.X + p.W)).Concat(l.Spikes.Select(s => (s, s + 32))).Concat(l.Bats.Select(b => (b.Cx - b.Rx - 8, b.Cx + b.Rx + 8)));
        if (hazX.Any(h => h.Item1 < 200) || hazX.Any(h => h.Item2 > l.Goal - 120)) bad.Add("hazard at start or goal");
        void Intro(string kind, float a, float b, bool overPit) { if (overPit) bad.Add($"first {kind} is over a pit"); if (HazardNear(a, b, 80)) bad.Add($"first {kind} is not a safe place to learn"); }
        if (l.Introduces.Contains("float") && l.Floats.Length > 0) { var f = l.Floats.OrderBy(f => f.X).First(); Intro("float", f.X, f.X + 48, l.Pits.Any(p => Near(f.X, f.X + 48, p.X, p.X + p.W, 0))); }
        if (l.Introduces.Contains("bat") && l.Bats.Length > 0) { var b = l.Bats.OrderBy(b => b.Cx).First(); Intro("bat", b.Cx - b.Rx - 8 - 40, b.Cx + b.Rx + 8 + 40, false); }
        if (l.Introduces.Contains("current") && l.Currents.Length > 0) { var k = l.Currents.OrderBy(k => k.X).First(); Intro("current", k.X, k.X + k.W, false); if (k.Dir < 0) bad.Add("first current pushes against the player"); }
        if (l.Introduces.Contains("balloon") && l.Balloons.Length > 0) { var b = l.Balloons.OrderBy(b => b.X).First(); Intro("balloon", b.X, b.X + 48, false); }
        if (l.Pickups.Count(p => p.Secret) != 1) bad.Add("needs exactly one secret pickup");
        if (l.Pickups.Length != 7) bad.Add($"{l.Pickups.Length} pickups, Normal needs 7");
        foreach (var p in l.Pickups) if (l.Bats.Any(b => Math.Abs(p.X - b.Cx) < b.Rx + 24)) bad.Add($"pickup {p.Kind} {p.X} inside a bat loop");
        return bad;
    }

    static void SelfCheck() {
        var n1 = HillsNormalLevels.N1; var s = new Sim(n1); s.Place(1852, 0.5f); float fell = -1;
        for (int f = 0; f < 600 && fell < 0; f++) { s.Step(0, false); if (s.InPit) fell = s.t; }
        Console.WriteLine($"check: idle on float 1828 falls into the pit at {fell:F2}s (rule: 0.6 wait + 2.0 sink, then a short fall)");
        s = new Sim(n1); s.Place(848, 0.5f); for (int f = 0; f < 300; f++) s.Step(0, false); Console.WriteLine($"check: idle 5 s on practice float 824 ends at y={s.py:F1} (pool floor -16, safe)");
        var n3 = HillsNormalLevels.N3; s = new Sim(n3); s.Place(3100, 0.5f); for (int f = 0; f < 60; f++) s.Step(1, false); Console.WriteLine($"check: 1 s walking right in the left current moves {s.px - 3100:F1} px (expect 42)");
        s = new Sim(n3); s.Place(3100, 0.5f); for (int f = 0; f < 60; f++) s.Step(0, false); Console.WriteLine($"check: 1 s idle in the left current moves {s.px - 3100:F1} px (expect -30)");
        var n2 = HillsNormalLevels.N2; s = new Sim(n2); s.Place(1340, 0.5f); float x0 = -1;
        for (int f = 0; f < 240; f++) { s.Step(0, false); if (s.batPushes[0] == 1 && x0 < 0) x0 = s.px; }
        Console.WriteLine($"check: idle next to bat 1350 for 4 s: {s.batPushes[0]} pushes, ended at x={s.px:F1} (started 1340; push goes away from the bat, then 1 s rest)");
    }
    static void Main() {
        SelfCheck();
        foreach (var l in HillsEasyLevels.All) {
            var r = Bot(l); var ok = ReachableLedges(l);
            var missing = l.Pickups.Where(k => !Reach(l, k, ok)).Select(k => $"{k.Kind}({k.X},{k.Y})").ToList();
            Console.WriteLine($"{l.Name}: bot {(r.ok ? "GOAL" : "FAILED")} {r.t:F1}s ({r.why}); pickups {l.Pickups.Length - missing.Count}/{l.Pickups.Length}" + (missing.Count > 0 ? " MISSING " + string.Join(" ", missing) : "") + $"; time on sand {SandOf(l):F1}s");
        }
        foreach (var l in HillsNormalLevels.All) {
            var r = Bot(l); var ok = ReachableLedges(l);
            var missing = l.Pickups.Where(k => !Reach(l, k, ok)).Select(k => $"{k.Kind}({k.X},{k.Y})").ToList();
            var unreachLedges = Enumerable.Range(0, l.Ledges.Length).Where(i => !ok.Contains(i)).Select(i => $"L({l.Ledges[i].X},{l.Ledges[i].Top})").ToList();
            var lint = Lint(l);
            Console.WriteLine($"{l.Name}: bot {(r.ok ? "GOAL" : "FAILED")} {r.t:F1}s ({r.why}); pickups {l.Pickups.Length - missing.Count}/{l.Pickups.Length} reachable"
                + (missing.Count > 0 ? " MISSING " + string.Join(" ", missing) : "") + (unreachLedges.Count > 0 ? " UNREACHABLE LEDGES " + string.Join(" ", unreachLedges) : "")
                + (lint.Count > 0 ? " LINT " + string.Join("; ", lint) : " lint clean"));
            var st = r.st;
            if (l.Floats.Length > 0) Console.WriteLine("   floats (s standing): " + string.Join(", ", l.Floats.Select((f, i) => $"{f.X}:{st.FloatTime[i]:F2}")));
            if (l.Bats.Length > 0) Console.WriteLine("   bats (s in loop span / pushes): " + string.Join(", ", l.Bats.Select((b, i) => $"{b.Cx}:{st.BatTime[i]:F2}/{(st.BatPushes != null ? st.BatPushes[i] : -1)}")));
            if (l.Currents.Length > 0) Console.WriteLine("   currents (s in strip): " + string.Join(", ", l.Currents.Select((c, i) => $"{c.X}{(c.Dir > 0 ? "R" : "L")}:{st.CurrentTime[i]:F2}")));
            if (l.Balloons.Length > 0) Console.WriteLine("   balloons (s from arriving to jumping off): " + string.Join(", ", l.Balloons.Select((b, i) => $"{b.X}:{st.BalloonTime[i]:F2}")));
            if (l.Balloons.Length > 0) { var nb = Bot(l with { Balloons = new Balloon[0] }); Console.WriteLine($"   without balloons: {(nb.ok ? "GOAL" : "FAILED")} ({nb.why})"); }
        }
    }
}
