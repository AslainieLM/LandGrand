// Draft Hills Hard layouts for the bot check (scratch). The same numbers go into layouts.md.
// Pickup kinds are placeholders k1..k9 (kinds are chosen later); the secret one is marked.
static class HillsHardLevels {
    static Boulder S(float x) => new(x, false);
    static Boulder B(float x) => new(x, true);
    static Ledge L(float x, float top, float w) => new(x, top, w);
    static Pit P(float x, float w) => new(x, w);
    static Mud M(float x, float w) => new(x, w);
    static Roller LogL(float spawn, float end, float offset) => new(spawn, end, offset, -1); // rolls left, toward the player
    static Roller LogR(float spawn, float end, float offset) => new(spawn, end, offset, +1); // rolls right, from behind
    static Bat Bt(float cx, float period, float phase) => new(cx, 16, 24, 12, period, phase);
    static Crumble[] Bridge(float x) => new[] { new Crumble(x, 0), new Crumble(x + 32, 0), new Crumble(x + 64, 0) };
    static Pickup K(int n, float x, float y, bool secret = false) => new($"k{n}", x, y, secret);
    static readonly float[] CP = { 2400, 4800, 7200 };
    const float Width = 9600, Goal = 9440;
    static Lvl Make(string name, Pit[] pits, float[] spikes, Boulder[] boulders, Ledge[] ledges, Pickup[] pickups,
                    Crumble[] crumbles = null, Platform[] platforms = null, Wind[] winds = null, Roller[] logs = null) =>
        new(name, Width, Goal, CP, pits, spikes, boulders, ledges, crumbles ?? new Crumble[0], new Rock[0], platforms ?? new Platform[0],
            winds ?? new Wind[0], new Vent[0], logs ?? new Roller[0], new Ice[0], pickups);

    // H1 Tarsier forest: slippery mud only. The first mud (500-800) is a practice spot: nothing within 120 px.
    public static readonly Lvl H1 = Make("H1 Tarsier Forest",
        new[] { P(1500, 56), P(3000, 64), P(4300, 64), P(6500, 64), P(8300, 64) },
        new float[] { 1200, 2800, 3900, 5500, 6900, 8600 },
        new[] { S(380), B(1000), S(1900), S(2100), B(3500), S(4500), S(5200), B(5800), S(6800), B(7800), S(9000) },
        new[] { L(2600, 40, 64), L(4000, 48, 64), L(6100, 40, 64), L(6180, 84, 48), L(8900, 48, 64) },
        new[] { K(1, 396, 24), K(2, 650, 0), K(3, 1528, 48), K(4, 2632, 40), K(5, 3032, 48), K(6, 4032, 48), K(7, 6204, 84, true), K(8, 7824, 36), K(9, 8932, 48) })
        with { Muds = new[] { M(500, 300), M(1700, 500), M(3200, 500), M(5000, 400), M(6000, 400), M(7500, 600), M(8800, 400) }, Introduces = new[] { "mud" } };

    // H2 Antipolo slopes: rolling logs only, left and right. The first log (1000 -> 800, rolling left) is a practice spot.
    public static readonly Lvl H2 = Make("H2 Antipolo Slopes",
        new[] { P(1500, 56), P(2900, 64), P(4000, 64), P(5600, 64), P(6700, 64), P(8500, 64) },
        new float[] { 1250, 2700, 3260, 4650, 5900, 7000, 8650 },
        new[] { S(400), B(1650), S(2200), B(3100), S(4900), B(5450), S(6500), B(7300), S(8000), B(9100) },
        new[] { L(2500, 40, 64), L(3700, 40, 64), L(3780, 84, 48) },
        new[] { K(1, 416, 24), K(2, 1528, 48), K(3, 2532, 40), K(4, 2932, 48), K(5, 3804, 84, true), K(6, 4916, 24), K(7, 6732, 48), K(8, 7324, 36), K(9, 8532, 48) },
        logs: new[] { LogL(1000, 800, 0), LogL(2000, 1800, 1.0f), LogL(3600, 3380, 0), LogR(4300, 4520, 0.5f), LogL(5300, 5080, 2.0f), LogR(6100, 6330, 1.0f),
                      LogL(7800, 7580, 1.5f), LogR(8100, 8320, 2.5f), LogL(9000, 8780, 0.5f) })
        with { Introduces = new[] { "log" } };

    // H3 Masungi: mud, bats and crumbling ledges (bridges over the wide pits, steps to the secret).
    public static readonly Lvl H3 = Make("H3 Masungi",
        new[] { P(1500, 56), P(2800, 96), P(4000, 64), P(5600, 96), P(6600, 64), P(8000, 96), P(8900, 56) },
        new float[] { 1200, 3300, 4450, 6250, 7600, 8650 },
        new[] { S(400), B(1000), S(2200), B(3600), S(4300), B(5300), S(6900), B(7400), S(8400) },
        new[] { L(3020, 40, 64), L(6980, 96, 48), L(9220, 48, 64) },
        new[] { K(1, 416, 24), K(2, 1528, 48), K(3, 2832, 40), K(4, 3052, 40), K(5, 4032, 48), K(6, 5632, 40), K(7, 7004, 96, true), K(8, 8032, 40), K(9, 9252, 48) },
        crumbles: Bridge(2800).Concat(Bridge(5600)).Concat(Bridge(8000)).Concat(new[] { new Crumble(6830, 40), new Crumble(6900, 64) }).ToArray())
        with { Muds = new[] { M(500, 400), M(1700, 400), M(3450, 450), M(5000, 450), M(6700, 120), M(7300, 250), M(8150, 400) },
               Bats = new[] { Bt(1800, 3.0f, 0), Bt(2150, 2.8f, 1.0f), Bt(3800, 2.6f, 0.4f), Bt(4600, 2.8f, 0.6f), Bt(5950, 2.6f, 1.2f), Bt(9150, 2.6f, 0.3f) } };

    // H4 Pililla: wind gusts, logs and vertical balloons. Balloon 5944 is required (160 px gap); balloon 7500 leads to the secret.
    public static readonly Lvl H4 = Make("H4 Pililla",
        new[] { P(850, 56), P(2700, 64), P(4000, 64), P(5300, 64), P(6000, 160), P(8200, 64) },
        new float[] { 1200, 3100, 5700, 6900, 9100 },
        new[] { S(400), B(1350), S(2200), B(3560), S(4200), B(5550), S(6250), B(7000), S(8500) },
        new[] { L(3700, 40, 64), L(5992, 112, 176), L(7552, 112, 64) },
        new[] { K(1, 416, 24), K(2, 878, 48), K(3, 2050, 0), K(4, 3732, 40), K(5, 4032, 48), K(6, 6080, 112), K(7, 7584, 112, true), K(8, 8232, 48), K(9, 8516, 24) },
        winds: new[] { new Wind(600, 400, 0), new Wind(2500, 400, 1.0f), new Wind(5000, 500, 0.5f), new Wind(7900, 500, 1.5f) },
        logs: new[] { LogL(1700, 1500, 0.5f), LogR(3300, 3530, 1.0f), LogL(4500, 4280, 2.0f), LogR(6400, 6620, 1.5f), LogL(8900, 8680, 0.5f) })
        with { Balloons = new[] { new Balloon(5944, 24, 120, 0), new Balloon(7500, 24, 120, 1.0f) } };

    // H5 Clew Bay: mud, current strips and logs.
    public static readonly Lvl H5 = Make("H5 Clew Bay",
        new[] { P(1350, 56), P(2000, 64), P(3100, 64), P(4550, 56), P(6000, 64), P(8800, 64) },
        new float[] { 1200, 3250, 4400, 5900, 6800 },
        new[] { S(400), S(800), B(1050), S(2200), B(3500), S(4200), B(5350), S(6400), B(8200), S(8350) },
        new[] { L(6900, 40, 64), L(6980, 84, 48) },
        new[] { K(1, 416, 24), K(2, 1378, 48), K(3, 2750, 0), K(4, 3132, 48), K(5, 3524, 36), K(6, 5200, 0), K(7, 6032, 48), K(8, 7004, 84, true), K(9, 8832, 48) },
        logs: new[] { LogL(1700, 1480, 0), LogR(3900, 4120, 1.0f), LogL(5800, 5580, 2.0f), LogR(7900, 8130, 0.5f), LogL(8700, 8480, 1.5f) })
        with { Muds = new[] { M(600, 400), M(3300, 500), M(6200, 500), M(8300, 400) },
               Currents = new[] { new Current(1720, 200, -1), new Current(2480, 520, -1), new Current(4950, 560, -1), new Current(7280, 560, -1), new Current(8950, 350, -1) } };

    // H6 Rome: logs, horizontal moving platforms over the wide pits, and sinking floats over water pits.
    public static readonly Lvl H6 = Make("H6 Rome",
        new[] { P(1200, 200), P(2000, 112), P(3200, 64), P(3800, 200), P(5000, 192), P(6200, 200), P(7400, 112), P(8000, 220) },
        new float[] { 1700, 3500, 5500, 7800, 9000 },
        new[] { S(400), B(1500), S(2250), B(3000), S(4600), B(5300), S(6000), B(7000), S(8300), B(9150) },
        new[] { L(4100, 40, 64), L(4180, 84, 48) },
        new[] { K(1, 416, 24), K(2, 1300, 40), K(3, 2056, 32), K(4, 3232, 48), K(5, 3900, 40), K(6, 4204, 84, true), K(7, 5136, 32), K(8, 6300, 40), K(9, 8100, 40) },
        platforms: new[] { new Platform(1200, 1352, 0), new Platform(3800, 3952, 0), new Platform(6200, 6352, 0), new Platform(8000, 8172, 0) },
        logs: new[] { LogL(900, 700, 0), LogR(2700, 2920, 1.0f), LogR(3560, 3790, 0.7f), LogL(4500, 4280, 2.0f), LogR(5700, 5920, 0.5f), LogL(6900, 6680, 1.5f), LogR(8500, 8720, 2.5f) })
        with { Floats = new[] { new Float(2032), new Float(5032), new Float(5112), new Float(7432) } };

    // H7 Guilin, the finale: mud, logs, bats, currents, floats, balloons, crumbling ledges and wind.
    public static readonly Lvl H7 = Make("H7 Guilin",
        new[] { P(1500, 96), P(3000, 112), P(3600, 64), P(3956, 160), P(6100, 96), P(7800, 192), P(8400, 64) },
        new float[] { 1000, 5500, 9250 },
        new[] { S(400), B(1150), S(2200), B(4600), S(5200), B(7550), S(8700) },
        new[] { L(3948, 112, 176), L(6540, 96, 48) },
        new[] { K(1, 416, 24), K(2, 700, 0), K(3, 1532, 40), K(4, 2750, 0), K(5, 3056, 32), K(6, 4040, 112), K(7, 6564, 96, true), K(8, 7856, 32), K(9, 8432, 48) },
        crumbles: Bridge(1500).Concat(Bridge(6100)).Concat(new[] { new Crumble(6400, 40), new Crumble(6470, 64) }).ToArray(),
        winds: new[] { new Wind(1100, 500, 0), new Wind(3300, 500, 1.0f), new Wind(8100, 500, 0.5f) },
        logs: new[] { LogL(2000, 1780, 0.5f), LogR(4300, 4520, 1.0f), LogL(6900, 6680, 2.0f), LogR(8950, 9170, 1.5f) })
        with { Muds = new[] { M(500, 400), M(4150, 400), M(5000, 400), M(8550, 350) },
               Bats = new[] { Bt(1300, 2.6f, 0), Bt(5800, 2.4f, 0.8f) },
               Currents = new[] { new Current(2480, 400, -1), new Current(7300, 400, -1) },
               Floats = new[] { new Float(3032), new Float(7832), new Float(7912) },
               Balloons = new[] { new Balloon(3900, 24, 120, 0) } };

    public static readonly Lvl[] All = { H1, H2, H3, H4, H5, H6, H7 };
}
