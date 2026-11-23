// Draft Plateaus Normal layouts for the bot check (scratch, D-073). The same numbers go into layouts.md and LevelCatalog.
// Skeletons reuse the bot-checked Plains Normal levels; objects change to the Plateaus ones (cable cars, updrafts, salt crust, llamas).
static class PlateausNormalLevels {
    static Boulder S(float x) => new(x, false);
    static Boulder B(float x) => new(x, true);
    static Ledge L(float x, float top, float w) => new(x, top, w);
    static Crumble C(float x) => new(x, 0);
    static Pickup K(string kind, float x, float y, bool secret = false) => new(kind, x, y, secret);
    static Bat Llama(float cx, float period, float phase) => new(cx, 6f, 48f, 0, period, phase); // the herd rule: walks 96 px on the ground
    static readonly Crumble[] NoC = new Crumble[0]; static readonly Rock[] NoR = new Rock[0]; static readonly Platform[] NoP = new Platform[0];
    static readonly Wind[] NoW = new Wind[0]; static readonly Vent[] NoV = new Vent[0]; static readonly Roller[] NoRo = new Roller[0]; static readonly Ice[] NoI = new Ice[0];
    static readonly float[] Cps = { 2400, 4800 };

    // N1 Table Mountain: four cable cars (horizontal moving platforms at the ground line); the first crosses a safe pool.
    public static readonly Lvl N1 = new("N1 Table Mountain", 7200, 7040, Cps,
        new[] { new Pit(2000, 200), new Pit(3600, 200), new Pit(5900, 200) },
        new float[] { 1500, 3100, 5300 },
        new[] { S(450), B(1250), S(2700), B(4300), S(5000), S(6700) },
        new[] { L(1100, 40, 64), L(4000, 40, 64), L(4080, 84, 64), L(6400, 28, 64) },
        NoC, NoR, new[] { new Platform(800, 912, 0), new Platform(2000, 2152, 0), new Platform(3600, 3752, 0), new Platform(5900, 6052, 0) }, NoW, NoV, NoRo, NoI,
        new[] { K("sandstone", 466, 24), K("quartzite", 1132, 40), K("granite", 2076, 40), K("shale", 2900, 0),
                K("quartzite", 4112, 84, true), K("sandstone", 5100, 0), K("granite", 6716, 24) },
        new Sand[0]) { Pools = new[] { new Pool(800, 160) } };

    // N2 Monument Valley: desert sand, crumbling bridges over two pits, and two updrafts.
    public static readonly Lvl N2 = new("N2 Monument Valley", 7200, 7040, Cps,
        new[] { new Pit(1300, 96), new Pit(3600, 56), new Pit(5700, 96) },
        new float[] { 2900, 4900, 6600 },
        new[] { S(900), B(1700), S(2600), B(3200), S(4500), B(5300), S(6800) },
        new[] { L(1000, 40, 64), L(2490, 112, 64), L(3900, 40, 64), L(3980, 84, 64), L(4740, 96, 64), L(5400, 28, 64) },
        new[] { C(1300), C(1332), C(1364), C(5700), C(5732), C(5764) }, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { K("sandstone", 916, 24), K("siltstone", 1032, 40), K("conglomerate", 2200, 0), K("hematite", 2522, 112),
                K("sandstone", 4012, 84, true), K("siltstone", 5432, 28), K("conglomerate", 6816, 24) },
        new[] { new Sand(500, 200), new Sand(2000, 300), new Sand(4100, 350), new Sand(6150, 300) })
        { Updrafts = new[] { new Updraft(2450, 116), new Updraft(4700, 100) } };

    // N3 Grand Canyon: river currents in the canyon floor, the first one pushing with the player.
    public static readonly Lvl N3 = new("N3 Grand Canyon", 7200, 7040, Cps,
        new[] { new Pit(1300, 48), new Pit(3200, 64), new Pit(5200, 56) },
        new float[] { 2800, 4500, 6500 },
        new[] { S(400), B(1000), S(2200), B(3500), B(5500), S(6700) },
        new[] { L(1600, 40, 64), L(3600, 40, 64), L(3680, 84, 64), L(6300, 28, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { K("limestone", 416, 24), K("schist", 700, 0), K("granite", 1632, 40), K("sandstone", 2650, 0),
                K("limestone", 3712, 84, true), K("schist", 5000, 0), K("granite", 6716, 24) },
        new Sand[0]) {
        Currents = new[] { new Current(600, 200, 1), new Current(1900, 300, -1), new Current(3900, 400, 1), new Current(5800, 300, -1) },
        Introduces = new[] { "current" } };

    // N4 Mount Roraima: summit pools with floats, and two updrafts; the secret sits on the high updraft ledge.
    public static readonly Lvl N4 = new("N4 Mount Roraima", 7200, 7040, Cps,
        new[] { new Pit(1800, 104), new Pit(3800, 56), new Pit(6200, 64) },
        new float[] { 1200, 4300, 6600 },
        new[] { S(500), B(2600), S(3500), B(4500), S(5600), B(6800) },
        new[] { L(900, 40, 64), L(3040, 112, 64), L(4000, 40, 64), L(4080, 84, 64), L(5040, 144, 56), L(5800, 28, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { K("quartzite", 516, 24), K("quartz", 932, 40), K("gneiss", 1852, 40), K("sandstone", 3072, 112),
                K("quartzite", 4112, 84), K("quartz", 5068, 144, true), K("gneiss", 6000, 0) },
        new Sand[0]) {
        Floats = new[] { new Float(1828), new Float(6208) },
        Updrafts = new[] { new Updraft(3000, 116), new Updraft(5000, 148) } };

    // N5 Salar de Uyuni: slippery salt crust (the ice rule) and three llama herds (the herd rule).
    public static readonly Lvl N5 = new("N5 Salar de Uyuni", 7200, 7040, Cps,
        new[] { new Pit(1300, 48), new Pit(3200, 64), new Pit(5200, 56) },
        new float[] { 2800, 4500, 6500 },
        new[] { S(400), B(1000), S(2200), B(3500), B(5500), S(6700) },
        new[] { L(1600, 40, 64), L(3600, 40, 64), L(3680, 84, 64), L(6300, 28, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, new[] { new Ice(600, 300), new Ice(3900, 400), new Ice(5900, 400) },
        new[] { K("halite", 416, 24), K("gypsum", 700, 0), K("ignimbrite", 1632, 40), K("andesite", 2650, 0),
                K("halite", 3712, 84, true), K("ulexite", 4100, 0), K("ulexite", 6716, 24) },
        new Sand[0]) {
        Bats = new[] { Llama(1900, 4.6f, 0), Llama(3000, 4.8f, 1.0f), Llama(5000, 4.4f, 2.0f) },
        Introduces = new[] { "herd" } };

    public static readonly Lvl[] All = { N1, N2, N3, N4, N5 };
}
