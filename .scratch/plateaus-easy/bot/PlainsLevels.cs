// Draft Plains Easy layouts for the bot check (scratch, D-069). The same numbers go into layouts.md and LevelCatalog.
static class PlainsEasyLevels {
    static Boulder S(float x) => new(x, false);
    static Boulder B(float x) => new(x, true);
    static Ledge L(float x, float top, float w) => new(x, top, w);
    static readonly Crumble[] NoC = new Crumble[0]; static readonly Rock[] NoR = new Rock[0]; static readonly Platform[] NoP = new Platform[0];
    static readonly Wind[] NoW = new Wind[0]; static readonly Vent[] NoV = new Vent[0]; static readonly Roller[] NoRo = new Roller[0]; static readonly Ice[] NoI = new Ice[0];

    public static readonly Lvl E1 = new("E1 Central Luzon Plain", 4800, 4740, new float[] { 2400 },
        new[] { new Pit(1500, 48), new Pit(3300, 48) },
        new float[] { 2900 },
        new[] { S(400), B(1000), S(1900), B(2700), S(3600), B(4200) },
        new[] { L(600, 32, 80), L(1200, 28, 64), L(2000, 40, 64), L(3000, 28, 80), L(3800, 28, 48), L(3860, 56, 64), L(3940, 84, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("clay", 416, 24), new Pickup("sandstone", 640, 32), new Pickup("shale", 2032, 40),
                new Pickup("siltstone", 3972, 84, true), new Pickup("clay", 4600, 16) },
        new Sand[0]);

    public static readonly Lvl E2 = new("E2 Pangasinan Salt Beds", 5400, 5240, new float[] { 2700 },
        new[] { new Pit(800, 48), new Pit(3500, 56) },
        new float[] { 1700, 4000 },
        new[] { S(400), B(1400), S(2300), S(4400), B(4800) },
        new[] { L(1100, 28, 80), L(2000, 40, 64), L(3000, 40, 64), L(3080, 84, 72), L(3250, 28, 64), L(4600, 28, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("halite", 416, 24), new Pickup("gypsum", 1132, 28), new Pickup("quartz", 2032, 40),
                new Pickup("siltstone", 3112, 84, true), new Pickup("halite", 4416, 24) },
        new Sand[0]);

    public static readonly Lvl E3 = new("E3 Isabela River Plain", 5400, 5240, new float[] { 2500 },
        new[] { new Pit(900, 48), new Pit(2100, 56), new Pit(3800, 56) },
        new float[] { 1500, 3000, 3100, 4300 },
        new[] { S(350), B(1700), S(2800), B(4050), S(4700) },
        new[] { L(1240, 96, 64), L(1900, 40, 64), L(2300, 28, 80), L(3360, 120, 56) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { new Pickup("conglomerate", 366, 24), new Pickup("mudstone", 1272, 96), new Pickup("halite", 3066, 0),
                new Pickup("limestone", 3388, 120, true), new Pickup("conglomerate", 4716, 24) },
        new Sand[0]) { Straws = new[] { new Straw(600), new Straw(1200), new Straw(3300), new Straw(4500) }, Introduces = new[] { "straw" } };

    public static readonly Lvl[] All = { E1, E2, E3 };
}
