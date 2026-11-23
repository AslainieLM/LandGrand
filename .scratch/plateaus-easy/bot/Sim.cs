// Scratch physics bot for Plateaus Easy (not project code). Plains Easy bot plus the cliff updraft (D-072).
// Earlier note: scratch physics bot for Plains Easy. Hills Hard bot plus the straw bounce pile (D-069).
// Earlier note: scratch physics bot for Hills Hard. Copied from the Hills Normal bot (Mountains Hard bot plus soft sand,
// sinking float, gentle bat, river current, vertical balloon), with two Hills Hard objects: slippery mud (the ice rule in
// MudZone stretches) and the rolling log (the rolling-boulder rule with a direction, +1 right / -1 left).
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
record Roller(float Spawn, float End, float Offset, int Dir = -1); // middle starts at Spawn, rolls toward End (Dir -1 left, +1 right), gone when the middle passes End
record Ice(float X, float W);                        // icy ground from X to X + W
record Sand(float X, float W);                       // soft sand from X to X + W
record Pickup(string Kind, float X, float Y, bool Secret = false);
// New in Hills Normal:
record Updraft(float X, float Top);                  // Plateaus Easy: cliff updraft, 32 wide from X; lifts the feet up to Top, then hovers
record Straw(float X);                               // Plains Easy: straw bounce pile, solid 48x16, left edge X; landing on top launches the explorer
record Mud(float X, float W);                        // Hills Hard: slippery mud from X to X + W (same rule and numbers as ice)
record Pool(float X, float W);                       // shallow safe water: a dip with a floor at -16 (not a hazard)
record Float(float X);                               // sinking float, left edge X, top at the water line (0) when up
record Bat(float Cx, float Cy, float Rx, float Ry, float Period, float Phase); // ellipse loop; phase in seconds
record Current(float X, float W, int Dir);           // river current strip; Dir +1 pushes right, -1 pushes left
record Balloon(float X, float Low, float High, float Phase); // vertical balloon, left edge X; at phase 0 it is at Low going up
record Lvl(string Name, float Width, float Goal, float[] Checkpoints, Pit[] Pits, float[] Spikes, Boulder[] Boulders, Ledge[] Ledges,
           Crumble[] Crumbles, Rock[] Rocks, Platform[] Platforms, Wind[] Winds, Vent[] Vents, Roller[] Rollers, Ice[] Ices, Pickup[] Pickups, Sand[] Sands = null) {
    public Updraft[] Updrafts { get; init; } = new Updraft[0]; // Plateaus Easy: cliff updrafts
    public Straw[] Straws { get; init; } = new Straw[0];    // Plains Easy: straw bounce piles
    public Mud[] Muds { get; init; } = new Mud[0];          // Hills Hard: slippery mud (ice rule)
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
    public const float StrawW = 48f, StrawH = 16f, StrawBounce = 400f; // straw pile size and its launch speed (about 89 px up)
    public const float UpW = 32f, UpRise = 90f; // updraft width and its rising speed
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
    public static float RollerX(Roller r, float t) { float p = Phase(t, RollCycle, r.Offset); if (p < RollStart) return float.NaN; float x = r.Spawn + r.Dir * (p - RollStart) * RollSpeed; return (r.Dir < 0 ? x < r.End : x > r.End) ? float.NaN : x; }
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
        foreach (var k in l.Straws) statics.Add(new R(k.X + 1, 0, Rules.StrawW - 2, Rules.StrawH - 1)); // pile: solid like a boulder, top at 15
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
    public bool OnIce => grounded && Math.Abs(py) < 0.5f && (L.Ices.Any(i => px > i.X && px < i.X + i.W) || OnMud);
    public bool OnMud => grounded && Math.Abs(py) < 0.5f && L.Muds.Any(m => px > m.X && px < m.X + m.W);
    public bool OnSand => grounded && Math.Abs(py) < 0.5f && (L.Sands ?? new Sand[0]).Any(s => px > s.X && px < s.X + s.W);
    public int CurrentDir => OnFlatGround ? L.Currents.Where(c => px >= c.X && px <= c.X + c.W).Select(c => c.Dir).FirstOrDefault() : 0;
    public float SandTime; public int Bounces;
    public bool Pushed => pushLeft > 0;
    public int Updrafts; // times the explorer entered a column
    int lastUp = -1;
    // The column the explorer's middle is in, with the feet between the ground and the column top.
    public int UpIndex() { for (int i = 0; i < L.Updrafts.Length; i++) { var u = L.Updrafts[i]; if (px >= u.X && px <= u.X + Rules.UpW && py >= -0.5f && py <= u.Top + 0.5f) return i; } return -1; }
    public bool InUpdraft => UpIndex() >= 0;
    public bool Hovering => InUpdraft && Math.Abs(py - L.Updrafts[UpIndex()].Top) < 0.5f;
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
        vy = Math.Max(vy - g * dt, -maxFall);
        int ui = UpIndex();
        if (ui >= 0) { // Updraft: no gravity, rise at UpRise up to the top, then hover; jumps do nothing inside; the double jump is ready on leaving
            if (ui != lastUp) Updrafts++;
            var u = L.Updrafts[ui]; grounded = false; jumpedFromGround = true; airJumpUsed = false; standingPlatform = -1; standFloat = -1; standBalloon = -1;
            vy = Math.Clamp((u.Top - py) / dt, 0f, Rules.UpRise);
        }
        lastUp = ui;
        MoveX(vx * dt); MoveY(vy * dt); px = Math.Clamp(px, W / 2, L.Width - W / 2);
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
        if (grounded && Math.Abs(py - (Rules.StrawH - 1)) < 0.5f && L.Straws.Any(k => px + W / 2 > k.X + 1 && px - W / 2 < k.X + Rules.StrawW - 1)) { Launch(Rules.StrawBounce); Bounces++; }
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
        if (landed && UpIndex() >= 0 && dy > 0) landed = false;
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

