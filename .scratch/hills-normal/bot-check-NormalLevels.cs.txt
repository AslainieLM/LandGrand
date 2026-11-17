// Draft Hills Normal layouts for the bot check (scratch). The same numbers go into layouts.md.
static class HillsNormalLevels {
    static Boulder S(float x) => new(x, false);
    static Boulder B(float x) => new(x, true);
    static Ledge L(float x, float top, float w) => new(x, top, w);
    static Pickup P(string k, float x, float y, bool secret = false) => new(k, x, y, secret);
    static readonly float[] CP = { 2400, 4800 };
    static readonly Crumble[] NoC = new Crumble[0]; static readonly Rock[] NoR = new Rock[0]; static readonly Platform[] NoP = new Platform[0];
    static readonly Wind[] NoW = new Wind[0]; static readonly Vent[] NoV = new Vent[0]; static readonly Roller[] NoRo = new Roller[0]; static readonly Ice[] NoI = new Ice[0];
    static readonly Sand[] NoS = new Sand[0];

    public static readonly Lvl N1 = new("N1 Hundred Islands", 7200, 7040, CP,
        new[] { new Pit(1800, 104), new Pit(3500, 48), new Pit(6100, 192) },
        new float[] { 2900, 5300 },
        new[] { S(420), B(1500), S(2700), B(3900), S(4500), B(5900), S(6700) },
        new[] { L(1200, 40, 64), L(4200, 40, 64), L(4280, 84, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { P("aragonite", 436, 24), P("coquina", 848, 40), P("coral_stone", 1232, 40), P("aragonite", 2600, 0),
                P("coral_stone", 3524, 48), P("coquina", 4312, 84, true), P("aragonite", 6716, 24) },
        NoS) {
        Pools = new[] { new Pool(800, 96) },
        Floats = new[] { new Float(824), new Float(1828), new Float(6132), new Float(6212) },
        Introduces = new[] { "float" } };

    public static readonly Lvl N2 = new("N2 Callao Cave", 7200, 7040, CP,
        new[] { new Pit(900, 48), new Pit(3300, 64), new Pit(5600, 56) },
        new float[] { 1700, 6300 },
        new[] { S(500), B(2000), S(2700), B(4200), S(4500), B(5200), S(6700) },
        new[] { L(1088, 40, 64), L(5000, 40, 64), L(5080, 84, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { P("flowstone", 516, 24), P("dripstone", 1120, 40), P("calcite", 2600, 0), P("flowstone", 3332, 48),
                P("dripstone", 5112, 84, true), P("calcite", 6550, 0), P("flowstone", 6716, 24) },
        NoS) {
        Bats = new[] { new Bat(1350, 16, 24, 12, 3.0f, 0f), new Bat(3800, 16, 24, 12, 2.6f, 0.5f), new Bat(6000, 16, 24, 12, 3.2f, 1.2f) },
        Introduces = new[] { "bat" } };

    public static readonly Lvl N3 = new("N3 Pagsanjan Gorge", 7200, 7040, CP,
        new[] { new Pit(1700, 56), new Pit(3700, 64), new Pit(5800, 64) },
        new float[] { 2000, 4250, 6400 },
        new[] { S(500), B(1450), S(2650), B(3500), S(4550), B(6050), S(6750) },
        new[] { L(1900, 40, 64), L(4000, 40, 64), L(4080, 84, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { P("volcanic_breccia", 516, 24), P("tuff", 1120, 0), P("andesite", 1932, 40), P("volcanic_breccia", 3160, 0),
                P("tuff", 4112, 84, true), P("andesite", 5832, 48), P("volcanic_breccia", 6766, 24) },
        NoS) {
        Currents = new[] { new Current(1000, 240, 1), new Current(3000, 320, -1), new Current(5300, 300, 1) },
        Introduces = new[] { "current" } };

    public static readonly Lvl N4 = new("N4 Banaue Rice Terraces", 7200, 7040, CP,
        new[] { new Pit(1200, 56), new Pit(3300, 112), new Pit(5900, 64) },
        new float[] { 1800, 4300, 6500 },
        new[] { S(500), B(1500), S(2650), B(3800), S(4550), B(6200), S(6750) },
        new[] { L(700, 32, 96), L(2000, 40, 64), L(4000, 40, 64), L(4080, 84, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { P("clay", 516, 24), P("kaolinite", 748, 32), P("clay", 2032, 40), P("kaolinite", 3356, 32),
                P("clay", 4112, 84, true), P("kaolinite", 5360, 0), P("clay", 6766, 24) },
        NoS) {
        Floats = new[] { new Float(3332) },
        Bats = new[] { new Bat(2900, 16, 24, 12, 3.0f, 0f) },
        Currents = new[] { new Current(5200, 280, -1) } };

    public static readonly Lvl N5 = new("N5 Cappadocia", 7200, 7040, CP,
        new[] { new Pit(1500, 64), new Pit(3400, 64), new Pit(4200, 160), new Pit(6100, 64) },
        new float[] { 1900, 3700, 6500 },
        new[] { S(500), B(1200), S(2600), B(3100), S(5000), B(5900), S(6750) },
        new[] { L(4192, 112, 176), L(5552, 112, 64) },
        NoC, NoR, NoP, NoW, NoV, NoRo, NoI,
        new[] { P("ignimbrite", 516, 24), P("zeolite", 924, 104), P("tuff", 1532, 48), P("ignimbrite", 2924, 124),
                P("zeolite", 4280, 112), P("tuff", 5584, 112, true), P("ignimbrite", 6766, 24) },
        NoS) {
        Balloons = new[] { new Balloon(900, 24, 80, 0f), new Balloon(2900, 24, 96, 0.5f), new Balloon(4144, 24, 120, 0f), new Balloon(5500, 24, 104, 1f) },
        Introduces = new[] { "balloon" } };

    public static readonly Lvl[] All = { N1, N2, N3, N4, N5 };
}
