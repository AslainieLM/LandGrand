// Draft Hills Easy layouts for the bot check (scratch, D-066). The same numbers go into the spec and LevelCatalog.
static class HillsEasyLevels {
    static Boulder S(float x) => new(x, false);
    static Boulder B(float x) => new(x, true);
    static Ledge L(float x, float top, float w) => new(x, top, w);
    static readonly Crumble[] NoC = new Crumble[0]; static readonly Rock[] NoR = new Rock[0]; static readonly Platform[] NoP = new Platform[0];
    static readonly Wind[] NoW = new Wind[0]; static readonly Vent[] NoV = new Vent[0]; static readonly Roller[] NoRo = new Roller[0]; static readonly Ice[] NoI = new Ice[0];

    public static readonly Lvl E1 = new("E1 Chocolate Hills", 4800, 4740, new float[] { 2400 },
        new[] { new Pit(1880, 48), new Pit(3150, 48) },
        new float[] { 3450 },
        new[] { S(300), B(900), S(1400), B(2150), S(2700), B(3300), S(3900), B(4300) },
        new[] { L(520, 40, 80), L(1100, 28, 80), L(1600, 40, 64), L(2950, 28, 80), L(3560, 28, 48), L(3620, 56, 96), L(4000, 28, 80) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("limestone", 316, 24), new Pickup("calcite", 560, 40), new Pickup("clay", 1140, 28),
                new Pickup("coral_stone", 3668, 56), new Pickup("limestone", 4600, 16) },
        new Sand[0]);

    public static readonly Lvl E2 = new("E2 Batanes", 5400, 5240, new float[] { 2700 },
        new[] { new Pit(700, 48), new Pit(3380, 56) },
        new float[] { 1800, 3800 },
        new[] { S(360), B(1550), S(2350), S(4300), B(4700) },
        new[] { L(1300, 28, 80), L(2050, 40, 64), L(3000, 40, 64), L(3080, 84, 72), L(3300, 28, 64), L(3452, 28, 64), L(4900, 28, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("andesite", 376, 24), new Pickup("limonite", 1332, 28), new Pickup("kaolinite", 2082, 40),
                new Pickup("coral_stone", 3112, 84), new Pickup("andesite", 4316, 24) },
        new Sand[0]);

    public static readonly Lvl E3 = new("E3 La Paz", 5400, 5240, new float[] { 2500 },
        new[] { new Pit(800, 48), new Pit(2800, 64), new Pit(3900, 56) },
        new float[] { 1300, 3200, 3300 },
        new[] { B(1050), S(1800), S(4400), B(4700), S(5000) },
        new[] { L(300, 24, 64), L(380, 48, 64), L(460, 72, 80), L(1500, 40, 64), L(2000, 28, 80), L(3500, 28, 56), L(3580, 56, 56), L(3480, 84, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("quartz", 500, 72), new Pickup("magnetite", 1532, 40), new Pickup("limonite", 3266, 0),
                new Pickup("feldspar", 3512, 84), new Pickup("quartz", 4416, 24) },
        new[] { new Sand(560, 160), new Sand(1900, 400), new Sand(2640, 150), new Sand(4050, 330) });

    public static readonly Lvl[] All = { E1, E2, E3 };
}
