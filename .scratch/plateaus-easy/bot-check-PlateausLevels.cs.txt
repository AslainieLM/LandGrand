// Draft Plateaus Easy layouts for the bot check (scratch, D-072). The same numbers go into layouts.md and LevelCatalog.
// Updraft(x, top): a 32 px column from x; the feet rise to top and hover there. Its ledge sits 4 px lower, 8 px to the right.
static class PlateausEasyLevels {
    static Boulder S(float x) => new(x, false);
    static Boulder B(float x) => new(x, true);
    static Ledge L(float x, float top, float w) => new(x, top, w);
    static readonly Crumble[] NoC = new Crumble[0]; static readonly Rock[] NoR = new Rock[0]; static readonly Platform[] NoP = new Platform[0];
    static readonly Wind[] NoW = new Wind[0]; static readonly Vent[] NoV = new Vent[0]; static readonly Roller[] NoRo = new Roller[0]; static readonly Ice[] NoI = new Ice[0];

    public static readonly Lvl E1 = new("E1 Bukidnon Plateau", 4800, 4740, new float[] { 2400 },
        new[] { new Pit(1300, 48), new Pit(3400, 48) },
        new float[] { 2950 },
        new[] { S(450), B(1050), S(1800), B(2800), S(3700), B(4250) },
        new[] { L(650, 32, 80), L(1500, 28, 64), L(2050, 40, 64), L(3100, 28, 80), L(3900, 28, 48), L(3960, 56, 64), L(4040, 84, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("basalt", 466, 24), new Pickup("andesite", 690, 32), new Pickup("hematite", 2082, 40),
                new Pickup("halloysite", 4072, 84, true), new Pickup("basalt", 4600, 16) },
        new Sand[0]);

    public static readonly Lvl E2 = new("E2 Lake Lanao", 5400, 5240, new float[] { 2700 },
        new[] { new Pit(900, 48), new Pit(3600, 56) },
        new float[] { 1800, 4100 },
        new[] { S(450), B(1500), S(2250), S(4450), B(4850) },
        new[] { L(1150, 28, 80), L(1950, 40, 64), L(3050, 40, 64), L(3130, 84, 72), L(3300, 28, 64), L(4650, 28, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("tuff", 466, 24), new Pickup("volcanic_breccia", 1182, 28), new Pickup("goethite", 1982, 40),
                new Pickup("halloysite", 3162, 84, true), new Pickup("tuff", 4466, 24) },
        new Sand[0]);

    public static readonly Lvl E3 = new("E3 Maria Cristina Falls", 5400, 5240, new float[] { 2500 },
        new[] { new Pit(900, 48), new Pit(2100, 56), new Pit(3800, 56) },
        new float[] { 1500, 3000, 3100, 4300 },
        new[] { S(350), B(1700), S(2800), B(4050), S(4700) },
        new[] { L(640, 64, 64), L(1240, 112, 64), L(1900, 40, 64), L(2300, 28, 80), L(3340, 160, 56), L(4540, 80, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("laterite", 366, 24), new Pickup("gibbsite", 1272, 112), new Pickup("goethite", 3066, 0),
                new Pickup("cristobalite", 3368, 160, true), new Pickup("laterite", 4716, 24) },
        new Sand[0]) { Updrafts = new[] { new Updraft(600, 68), new Updraft(1200, 116), new Updraft(3300, 164), new Updraft(4500, 84) }, Introduces = new[] { "updraft" } };

    public static readonly Lvl[] All = { E1, E2, E3 };
}
