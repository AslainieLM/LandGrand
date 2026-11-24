// Draft Plains Hard layouts for the bot check (scratch, D-071). The same numbers go into layouts.md and LevelCatalog.
// Each level starts from a bot-checked Hills Hard skeleton (same pits, spikes, boulders and ledges where possible).
// Hay bales are rolling logs (Roller). A herd is a Bat on the ground: centre y 6, up-down radius 0, so it walks back and forth.
static class PlainsHardLevels {
    static Boulder S(float x) => new(x, false);
    static Boulder B(float x) => new(x, true);
    static Ledge L(float x, float top, float w) => new(x, top, w);
    static Pit P(float x, float w) => new(x, w);
    static Ice I(float x, float w) => new(x, w);
    static Roller HayL(float spawn, float end, float offset) => new(spawn, end, offset, -1); // rolls left, toward the player
    static Roller HayR(float spawn, float end, float offset) => new(spawn, end, offset, +1); // rolls right, from behind
    public const float HerdY = 6f, HerdRx = 48f;
    static Bat Herd(float cx, float period, float phase) => new(cx, HerdY, HerdRx, 0, period, phase); // walks 96 px back and forth on the ground
    static Bat Bt(float cx, float period, float phase) => new(cx, 16, 24, 12, period, phase);
    static Crumble[] Bridge(float x) => new[] { new Crumble(x, 0), new Crumble(x + 32, 0), new Crumble(x + 64, 0) };
    static Pickup K(string kind, float x, float y, bool secret = false) => new(kind, x, y, secret);
    static readonly float[] CP = { 2400, 4800, 7200 };
    const float Width = 9600, Goal = 9440;
    static Lvl Make(string name, Pit[] pits, float[] spikes, Boulder[] boulders, Ledge[] ledges, Pickup[] pickups,
                    Crumble[] crumbles = null, Roller[] hay = null, Ice[] ice = null) =>
        new(name, Width, Goal, CP, pits, spikes, boulders, ledges, crumbles ?? new Crumble[0], new Rock[0], new Platform[0],
            new Wind[0], new Vent[0], hay ?? new Roller[0], ice ?? new Ice[0], pickups);

    // H1 Great Plains: rolling hay bales and straw piles (Hills Hard H2 skeleton). The first bale (1000 -> 800, rolling left) is a practice spot.
    public static readonly Lvl H1 = Make("H1 Great Plains",
        new[] { P(1500, 56), P(2900, 64), P(4000, 64), P(5600, 64), P(6700, 64), P(8500, 64) },
        new float[] { 1250, 2700, 3260, 4650, 5900, 7000, 8650 },
        new[] { S(400), B(1650), S(2200), B(3100), S(4900), B(5450), S(6500), B(7300), S(8000), B(9100) },
        new[] { L(2500, 40, 64), L(3700, 40, 64), L(3780, 84, 48) },
        new[] { K("chalk", 416, 24), K("calcite", 1528, 48), K("chalk", 2532, 40), K("limestone", 2932, 48), K("chalk", 3804, 84, true),
                K("shale", 4916, 24), K("calcite", 6732, 48), K("limestone", 7324, 36), K("sandstone", 8532, 48) },
        hay: new[] { HayL(1000, 800, 0), HayL(2000, 1800, 1.0f), HayL(3600, 3380, 0), HayR(4300, 4520, 0.5f), HayL(5300, 5080, 2.0f), HayR(6100, 6330, 1.0f),
                     HayL(7800, 7580, 1.5f), HayR(8100, 8320, 2.5f), HayL(9000, 8780, 0.5f) })
        with { Straws = new[] { new Straw(2050), new Straw(4550), new Straw(6900) }, Introduces = new[] { "log" } };

    // H2 Serengeti: a grazing wildebeest herd only (Hills Hard H1 skeleton without the mud). The first herd (650) is a practice spot.
    public static readonly Lvl H2 = Make("H2 Serengeti",
        new[] { P(1500, 56), P(3000, 64), P(4300, 64), P(6500, 64), P(8300, 64) },
        new float[] { 1200, 2800, 3900, 5500, 6900, 8600 },
        new[] { S(380), B(1000), S(1900), S(2100), B(3500), S(4500), S(5200), B(5800), S(6800), B(7800), S(9000) },
        new[] { L(2600, 40, 64), L(4000, 48, 64), L(6100, 40, 64), L(6180, 84, 48), L(8900, 48, 64) },
        new[] { K("granite", 396, 24), K("tuff", 1100, 0), K("granite", 1528, 48), K("tuff", 2632, 40), K("sandstone", 3032, 48),
                K("granite", 4032, 48), K("tuff", 6204, 84, true), K("siltstone", 7824, 36), K("shale", 8932, 48) })
        with { Bats = new[] { Herd(650, 5.0f, 0), Herd(2000, 4.6f, 1.0f), Herd(3650, 4.8f, 0.5f), Herd(5000, 4.4f, 2.0f), Herd(6100, 5.0f, 1.5f), Herd(7600, 4.6f, 0.8f) },
               Introduces = new[] { "herd" } };

    // H3 Pampas: hay bales and a cattle herd (Hills Hard H5 skeleton; the herd walks where the currents were, no mud).
    public static readonly Lvl H3 = Make("H3 Pampas",
        new[] { P(1350, 56), P(2000, 64), P(3100, 64), P(4550, 56), P(6000, 64), P(8800, 64) },
        new float[] { 1200, 3250, 4400, 5900, 6800 },
        new[] { S(400), S(800), B(1050), S(2200), B(3500), S(4200), B(5350), S(6400), B(8200), S(8350) },
        new[] { L(6900, 40, 64), L(6980, 84, 48) },
        new[] { K("loess", 416, 24), K("mica", 1378, 48), K("feldspar", 2400, 0), K("loess", 3132, 48), K("quartz", 3524, 36),
                K("mica", 4900, 0), K("feldspar", 6032, 48), K("loess", 7004, 84, true), K("clay", 8832, 48) },
        hay: new[] { HayL(1700, 1480, 0), HayR(3900, 4120, 1.0f), HayL(5800, 5580, 2.0f), HayR(7900, 8130, 0.5f), HayL(8700, 8480, 1.5f) })
        with { Bats = new[] { Herd(2740, 4.8f, 0), Herd(5230, 4.4f, 1.0f), Herd(7560, 5.0f, 2.0f) } };

    // H4 West Siberian Plain: frozen ground (the icy rule) and peat-bog floats (Hills Hard H1 skeleton; mud becomes ice, two pits become bog ponds).
    public static readonly Lvl H4 = Make("H4 West Siberian Plain",
        new[] { P(1500, 56), P(3000, 104), P(4300, 64), P(6500, 104), P(8300, 64) },
        new float[] { 1200, 2800, 3900, 5500, 6900, 8600 },
        new[] { S(380), B(1000), S(1900), S(2100), B(3500), S(4500), S(5200), B(5800), S(6800), B(7800), S(9000) },
        new[] { L(2600, 40, 64), L(4000, 48, 64), L(6100, 40, 64), L(6180, 84, 48), L(8900, 48, 64) },
        new[] { K("peat", 396, 24), K("clay", 650, 0), K("siltstone", 1528, 48), K("peat", 2632, 40), K("mudstone", 3052, 40),
                K("sandstone", 4032, 48), K("peat", 6204, 84, true), K("clay", 7824, 36), K("siltstone", 8932, 48) },
        ice: new[] { I(500, 300), I(1700, 500), I(3200, 500), I(5000, 400), I(6000, 400), I(7500, 600), I(8800, 400) })
        with { Floats = new[] { new Float(3028), new Float(6528) } };

    // H5 Nullarbor Plain: crumbling ledges and bats (Hills Hard H3 skeleton without the mud).
    public static readonly Lvl H5 = Make("H5 Nullarbor Plain",
        new[] { P(1500, 56), P(2800, 96), P(4000, 64), P(5600, 96), P(6600, 64), P(8000, 96), P(8900, 56) },
        new float[] { 1200, 3300, 4450, 6250, 7600, 8650 },
        new[] { S(400), B(1000), S(2200), B(3600), S(4300), B(5300), S(6900), B(7400), S(8400) },
        new[] { L(3020, 40, 64), L(6980, 96, 48), L(9220, 48, 64) },
        new[] { K("flint", 416, 24), K("limestone", 1528, 48), K("calcite", 2832, 40), K("flint", 3052, 40), K("chalk", 4032, 48),
                K("limestone", 5632, 40), K("flint", 7004, 96, true), K("calcite", 8032, 40), K("limestone", 9252, 48) },
        crumbles: Bridge(2800).Concat(Bridge(5600)).Concat(Bridge(8000)).Concat(new[] { new Crumble(6830, 40), new Crumble(6900, 64) }).ToArray())
        with { Bats = new[] { Bt(1800, 3.0f, 0), Bt(2150, 2.8f, 1.0f), Bt(3800, 2.6f, 0.4f), Bt(4600, 2.8f, 0.6f), Bt(5950, 2.6f, 1.2f), Bt(9150, 2.6f, 0.3f) } };

    // H6 Indo-Gangetic Plain: floats over flood ponds and river currents (Hills Hard H6 skeleton without logs or moving platforms).
    public static readonly Lvl H6 = Make("H6 Indo-Gangetic Plain",
        new[] { P(1200, 200), P(2000, 112), P(3200, 64), P(3800, 200), P(5000, 192), P(6200, 200), P(7400, 112), P(8000, 200) },
        new float[] { 1700, 3500, 5500, 7800, 9000 },
        new[] { S(400), B(1500), S(2250), B(3000), S(4600), B(5300), S(6000), B(7000), S(8300), B(9150) },
        new[] { L(4100, 40, 64), L(4180, 84, 48) },
        new[] { K("laterite", 416, 24), K("clay", 1300, 40), K("siltstone", 2056, 32), K("sandstone", 3232, 48), K("clay", 3900, 40),
                K("laterite", 4204, 84, true), K("mudstone", 5136, 32), K("siltstone", 6300, 40), K("laterite", 8100, 40) })
        with { Floats = new[] { new Float(1232), new Float(1312), new Float(2032), new Float(3832), new Float(3912), new Float(5032), new Float(5112),
                                new Float(6232), new Float(6312), new Float(7432), new Float(8032), new Float(8112) },
               Currents = new[] { new Current(550, 450, 1), new Current(2500, 550, -1), new Current(4100, 600, -1), new Current(5650, 450, 1),
                                  new Current(6500, 600, -1), new Current(8300, 600, -1) } };

    // H7 Mongolian Steppe, the finale: hay, a horse herd, frozen ground, crumbling bridges and currents (Hills Hard H7 skeleton,
    // with the float, balloon and wind crossings changed to plain pits or a crumbling bridge).
    public static readonly Lvl H7 = Make("H7 Mongolian Steppe",
        new[] { P(1500, 96), P(3000, 64), P(3600, 64), P(4000, 64), P(6100, 96), P(7800, 96), P(8400, 64) },
        new float[] { 1000, 5500, 9250 },
        new[] { S(400), B(1150), S(2200), B(4600), S(5200), B(7550), S(8700) },
        new[] { L(6540, 96, 48) },
        new[] { K("granite", 416, 24), K("chalk", 700, 0), K("loess", 1532, 40), K("flint", 2750, 0), K("tuff", 3032, 48),
                K("mica", 4032, 48), K("loess", 6564, 96, true), K("feldspar", 7856, 32), K("granite", 8432, 48) },
        crumbles: Bridge(1500).Concat(Bridge(6100)).Concat(Bridge(7800)).Concat(new[] { new Crumble(6400, 40), new Crumble(6470, 64) }).ToArray(),
        hay: new[] { HayL(2000, 1780, 0.5f), HayR(4300, 4520, 1.0f), HayL(6900, 6680, 2.0f), HayR(8950, 9170, 1.5f) },
        ice: new[] { I(500, 400), I(4150, 400), I(5000, 400), I(8550, 350) })
        with { Bats = new[] { Herd(1300, 4.6f, 0), Herd(5800, 4.4f, 0.8f) },
               Currents = new[] { new Current(2480, 400, -1), new Current(7300, 400, -1) } };

    public static readonly Lvl[] All = { H1, H2, H3, H4, H5, H6, H7 };
}
