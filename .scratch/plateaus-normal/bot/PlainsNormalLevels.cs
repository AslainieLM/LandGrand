// Draft Plains Normal layouts for the bot check (scratch, D-070). The same numbers go into layouts.md and LevelCatalog.
static class PlainsNormalLevels {
    static Boulder S(float x) => new(x, false);
    static Boulder B(float x) => new(x, true);
    static Ledge L(float x, float top, float w) => new(x, top, w);
    static Crumble C(float x) => new(x, 0);
    static readonly Crumble[] NoC = new Crumble[0]; static readonly Rock[] NoR = new Rock[0]; static readonly Platform[] NoP = new Platform[0];
    static readonly Wind[] NoW = new Wind[0]; static readonly Vent[] NoV = new Vent[0]; static readonly Roller[] NoRo = new Roller[0]; static readonly Ice[] NoI = new Ice[0];
    static readonly float[] Cps = { 2400, 4800 };

    // N1 Candaba Swamp: sinking floats on swamp ponds (practice float over a safe pool).
    public static readonly Lvl N1 = new("N1 Candaba Swamp", 7200, 7040, Cps,
        new[] { new Pit(1700, 104), new Pit(3400, 48), new Pit(6000, 192) },
        new float[] { 2900, 5300 },
        new[] { S(420), B(1400), S(2700), B(3900), S(4500), B(5700), S(6700) },
        new[] { L(1100, 40, 64), L(4200, 40, 64), L(4280, 84, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("peat", 436, 24), new Pickup("peat", 848, 40), new Pickup("clay", 1132, 40), new Pickup("siltstone", 2600, 0),
                new Pickup("mudstone", 3424, 48), new Pickup("peat", 4312, 84, true), new Pickup("clay", 6716, 24) },
        new Sand[0]) {
        Pools = new[] { new Pool(800, 96) },
        Floats = new[] { new Float(824), new Float(1728), new Float(6032), new Float(6112) },
        Introduces = new[] { "float" } };

    // N2 Pinatubo Lahar Plain: soft lahar sand and crumbling bridges over two pits.
    public static readonly Lvl N2 = new("N2 Pinatubo Lahar Plain", 7200, 7040, Cps,
        new[] { new Pit(1300, 96), new Pit(3600, 56), new Pit(5700, 96) },
        new float[] { 2900, 4900, 6600 },
        new[] { S(900), B(1700), S(2600), B(3200), S(4500), B(5300), S(6800) },
        new[] { L(1000, 40, 64), L(3900, 40, 64), L(3980, 84, 64), L(5400, 28, 64) },
        new[] { C(1300), C(1332), C(1364), C(5700), C(5732), C(5764) }, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("pumice", 916, 24), new Pickup("dacite", 1032, 40), new Pickup("andesite", 2200, 0), new Pickup("pumice", 3224, 36),
                new Pickup("dacite", 4012, 84, true), new Pickup("andesite", 5432, 28), new Pickup("pumice", 6816, 24) },
        new[] { new Sand(500, 200), new Sand(2000, 300), new Sand(4100, 350), new Sand(6150, 300) });

    // N3 Agno River Delta: river currents, the first one pushing with the player.
    public static readonly Lvl N3 = new("N3 Agno River Delta", 7200, 7040, Cps,
        new[] { new Pit(1300, 48), new Pit(3200, 64), new Pit(5200, 56) },
        new float[] { 2800, 4500, 6500 },
        new[] { S(400), B(1000), S(2200), B(3500), B(5500), S(6700) },
        new[] { L(1600, 40, 64), L(3600, 40, 64), L(3680, 84, 64), L(6300, 28, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("gold", 416, 24), new Pickup("quartz", 700, 0), new Pickup("gold", 1632, 40), new Pickup("siltstone", 2650, 0),
                new Pickup("gold", 3712, 84, true), new Pickup("quartz", 5000, 0), new Pickup("mudstone", 6716, 24) },
        new Sand[0]) {
        Currents = new[] { new Current(600, 200, 1), new Current(1900, 300, -1), new Current(3900, 400, 1), new Current(5800, 300, -1) },
        Introduces = new[] { "current" } };

    // N4 Agusan Marsh: one float, one sand patch and one current; review kinds only.
    public static readonly Lvl N4 = new("N4 Agusan Marsh", 7200, 7040, Cps,
        new[] { new Pit(1800, 104), new Pit(3800, 56), new Pit(6200, 64) },
        new float[] { 1200, 4300, 6600 },
        new[] { S(500), B(2600), S(3500), B(4500), S(5600), B(6800) },
        new[] { L(900, 40, 64), L(4000, 40, 64), L(4080, 84, 64), L(5800, 28, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("peat", 516, 24), new Pickup("clay", 932, 40), new Pickup("mudstone", 1852, 40), new Pickup("peat", 3150, 0),
                new Pickup("clay", 4112, 84, true), new Pickup("mudstone", 5150, 0), new Pickup("peat", 6000, 0) },
        new[] { new Sand(3000, 300) }) {
        Floats = new[] { new Float(1828) },
        Currents = new[] { new Current(5000, 300, 1) } };

    // N5 Mekong Delta: four market boats (horizontal moving platforms at the ground line); the first crosses a safe pool.
    public static readonly Lvl N5 = new("N5 Mekong Delta", 7200, 7040, Cps,
        new[] { new Pit(2000, 200), new Pit(3600, 200), new Pit(5900, 200) },
        new float[] { 1500, 3100, 5300 },
        new[] { S(450), B(1250), S(2700), B(4300), S(5000), S(6700) },
        new[] { L(1100, 40, 64), L(4000, 40, 64), L(4080, 84, 64), L(6400, 28, 64) },
        NoC, NoR, new[] { new Platform(800, 912, 0), new Platform(2000, 2152, 0), new Platform(3600, 3752, 0), new Platform(5900, 6052, 0) }, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("laterite", 466, 24), new Pickup("clay", 1132, 40), new Pickup("laterite", 2076, 40), new Pickup("siltstone", 2900, 0),
                new Pickup("laterite", 4112, 84, true), new Pickup("clay", 5100, 0), new Pickup("mudstone", 6716, 24) },
        new Sand[0]) {
        Pools = new[] { new Pool(800, 160) } };

    public static readonly Lvl[] All = { N1, N2, N3, N4, N5 };
}
