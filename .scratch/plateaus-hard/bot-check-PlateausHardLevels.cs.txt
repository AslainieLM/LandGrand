// Draft Plateaus Hard layouts for the bot check (scratch, D-074). The same numbers go into layouts.md and LevelCatalog.
// Skeletons reuse the bot-checked Plains Hard and Hills Hard levels; objects change to the Plateaus Hard ones.
// Hot-spring vent = Vent (SteamVent rule); gelada / yak herd = Bat on the ground (herd rule); sand or snow gust = Wind (always left);
// blue ice = Ice; loess or desert sand = Sand; sandstone slab = Platform (horizontal) or Balloon (vertical moving platform).
static class PlateausHardLevels {
    static Boulder S(float x) => new(x, false);
    static Boulder B(float x) => new(x, true);
    static Ledge L(float x, float top, float w) => new(x, top, w);
    static Pit P(float x, float w) => new(x, w);
    static Ice I(float x, float w) => new(x, w);
    static Sand Sd(float x, float w) => new(x, w);
    static Wind G(float x, float w, float offset) => new(x, w, offset); // gust zone, pushes left 40 px/s, 4 s cycle
    static Vent V(float x) => new(x);                                 // hot spring, 24 wide; its ledge is at x + 40, top 112
    static Updraft U(float x, float top) => new(x, top);              // landing ledge at x + 40, top - 4
    static Bat Herd(float cx, float period, float phase) => new(cx, PlainsHardLevels.HerdY, PlainsHardLevels.HerdRx, 0, period, phase);
    static Crumble[] Bridge(float x) => new[] { new Crumble(x, 0), new Crumble(x + 32, 0), new Crumble(x + 64, 0) };
    static Pickup K(string kind, float x, float y, bool secret = false) => new(kind, x, y, secret);
    static readonly float[] CP = { 2400, 4800, 7200 };
    const float Width = 9600, Goal = 9440;
    static Lvl Make(string name, Pit[] pits, float[] spikes, Boulder[] boulders, Ledge[] ledges, Pickup[] pickups,
                    Crumble[] crumbles = null, Platform[] platforms = null, Wind[] winds = null, Vent[] vents = null, Ice[] ice = null, Sand[] sand = null) =>
        new(name, Width, Goal, CP, pits, spikes, boulders, ledges, crumbles ?? new Crumble[0], new Rock[0], platforms ?? new Platform[0],
            winds ?? new Wind[0], vents ?? new Vent[0], new Roller[0], ice ?? new Ice[0], pickups, sand ?? new Sand[0]);

    // H1 Deccan Plateau: hot-spring vents lift the explorer to three high ledges; crumbling bridges and steps (Plains Hard H5 skeleton, no bats).
    // The first vent (1800) is a safe place to learn: nothing within 120 px.
    public static readonly Lvl H1 = Make("H1 Deccan Plateau",
        new[] { P(1500, 56), P(2800, 96), P(4000, 64), P(5600, 96), P(6600, 64), P(8000, 96), P(8900, 56) },
        new float[] { 1200, 3300, 4450, 6250, 7600, 8650 },
        new[] { S(400), B(1000), S(2200), B(3600), S(4300), B(5300), S(6900), B(7400), S(8400) },
        new[] { L(1840, 112, 64), L(3020, 40, 64), L(5040, 112, 64), L(6980, 96, 48), L(8260, 112, 64) },
        new[] { K("basalt", 416, 24), K("zeolite", 1872, 112), K("calcite", 2832, 40), K("basalt", 3052, 40), K("olivine", 4032, 48),
                K("zeolite", 5072, 112), K("basalt", 7004, 96, true), K("olivine", 8032, 40), K("calcite", 8292, 112) },
        crumbles: Bridge(2800).Concat(Bridge(5600)).Concat(Bridge(8000)).Concat(new[] { new Crumble(6830, 40), new Crumble(6900, 64) }).ToArray(),
        vents: new[] { V(1800), V(5000), V(8220) })
        with { Introduces = new[] { "vent" } };

    // H2 Ethiopian Highlands: four gelada troops (the herd rule) on the cliff path, two crumbling bridges (Plains Hard H2 skeleton).
    public static readonly Lvl H2 = Make("H2 Ethiopian Highlands",
        new[] { P(1500, 56), P(3000, 96), P(4300, 64), P(6500, 96), P(8300, 64) },
        new float[] { 1200, 2800, 3900, 5500, 6900, 8600 },
        new[] { S(380), B(1000), S(1900), S(2100), B(3500), S(4500), S(5200), B(5800), S(6800), B(7800), S(9000) },
        new[] { L(2600, 40, 64), L(4000, 48, 64), L(6100, 40, 64), L(6180, 84, 48), L(8900, 48, 64) },
        new[] { K("basalt", 396, 24), K("ignimbrite", 1100, 0), K("basalt", 1528, 48), K("tuff", 2632, 40), K("opal", 3032, 40),
                K("ignimbrite", 4032, 48), K("opal", 6204, 84, true), K("tuff", 7824, 36), K("basalt", 8932, 48) },
        crumbles: Bridge(3000).Concat(Bridge(6500)).ToArray())
        with { Bats = new[] { Herd(650, 5.0f, 0), Herd(2000, 4.6f, 1.0f), Herd(5000, 4.4f, 2.0f), Herd(7600, 4.6f, 0.8f) } };

    // H3 Tassili n'Ajjer: four sand gusts (the first one a safe place to learn) and three soft-sand stretches (Plains Hard H3 skeleton, no hay or herds).
    public static readonly Lvl H3 = Make("H3 Tassili n'Ajjer",
        new[] { P(1350, 56), P(2000, 64), P(3100, 64), P(4550, 56), P(6000, 64), P(8800, 64) },
        new float[] { 1200, 3250, 4400, 5900, 6800 },
        new[] { S(400), S(800), B(1050), S(2200), B(3500), S(4200), B(5350), S(6400), B(8200), S(8350) },
        new[] { L(6900, 40, 64), L(6980, 84, 48) },
        new[] { K("sandstone", 416, 24), K("granite", 1378, 48), K("quartz", 2400, 0), K("sandstone", 3132, 48), K("clay", 3524, 36),
                K("granite", 4900, 0), K("sandstone", 6032, 48), K("sandstone", 7004, 84, true), K("quartz", 8832, 48) },
        winds: new[] { G(500, 400, 0), G(2500, 500, 1.0f), G(5000, 500, 0.5f), G(7600, 500, 1.5f) },
        sand: new[] { Sd(1600, 300), Sd(3700, 350), Sd(8400, 300) })
        with { Introduces = new[] { "wind" } };

    // H4 Mesa Verde: cliff-alcove ledges reached by two updrafts and two vertical slabs; two horizontal slabs cross wide pits.
    // The first vertical slab (3500) is optional and safe; the second (5944) is needed for the 160 px gap. The secret needs updraft 4500.
    public static readonly Lvl H4 = Make("H4 Mesa Verde",
        new[] { P(850, 56), P(1800, 200), P(2700, 64), P(4000, 64), P(6000, 160), P(7700, 200), P(8600, 64) },
        new float[] { 1200, 3100, 5700, 6900, 9100 },
        new[] { S(400), B(1350), S(2500), S(3200), S(4200), B(5200), S(6400), B(7000), S(8300) },
        new[] { L(2240, 112, 64), L(3552, 112, 64), L(4540, 144, 56), L(5992, 112, 176) },
        new[] { K("sandstone", 416, 24), K("shale", 878, 48), K("coal", 1876, 40), K("sandstone", 2272, 112), K("coal", 3584, 112),
                K("shale", 4032, 48), K("sandstone", 4568, 144, true), K("coal", 6080, 112), K("sandstone", 7776, 40) },
        platforms: new[] { new Platform(1800, 1952, 0), new Platform(7700, 7852, 0) })
        with { Updrafts = new[] { U(2200, 116), U(4500, 148) },
               Balloons = new[] { new Balloon(3500, 24, 120, 0), new Balloon(5944, 24, 120, 1.0f) },
               Introduces = new[] { "balloon" } };

    // H5 Antarctic Plateau: blue ice and snow gusts; three gusts blow over ice, with no crevasse within 80 px (Plains Hard H4 skeleton, no floats).
    public static readonly Lvl H5 = Make("H5 Antarctic Plateau",
        new[] { P(1500, 56), P(3000, 64), P(4300, 64), P(6500, 64), P(8300, 64) },
        new float[] { 1200, 2800, 3900, 5500, 6900, 8600 },
        new[] { S(380), B(1000), S(1900), S(2100), B(3500), S(4500), S(5200), B(5800), S(6800), B(7800), S(9000) },
        new[] { L(2600, 40, 64), L(4000, 48, 64), L(6100, 40, 64), L(6180, 84, 48), L(8900, 48, 64) },
        new[] { K("granite", 396, 24), K("meteorite", 650, 0), K("gneiss", 1528, 48), K("coal", 2632, 40), K("granite", 3032, 48),
                K("gneiss", 4032, 48), K("meteorite", 6204, 84, true), K("coal", 7824, 36), K("meteorite", 8932, 48) },
        winds: new[] { G(600, 400, 0), G(1700, 400, 1.0f), G(5000, 400, 0.5f), G(7500, 500, 1.5f) },
        ice: new[] { I(1700, 500), I(3200, 500), I(5000, 400), I(7500, 600) });

    // H6 Loess Plateau: soft loess stretches, crumbling terrace edges, pits, spikes and boulders (Plains Hard H7 skeleton, no hay, ice, herds or currents).
    public static readonly Lvl H6 = Make("H6 Loess Plateau",
        new[] { P(1500, 96), P(3000, 64), P(3600, 64), P(4000, 64), P(6100, 96), P(7800, 96), P(8400, 64) },
        new float[] { 1000, 2600, 4400, 5500, 7000, 9250 },
        new[] { S(400), B(1150), S(2200), B(4600), S(5200), B(7550), S(8700) },
        new[] { L(6540, 96, 48) },
        new[] { K("loess", 416, 24), K("quartz", 700, 0), K("feldspar", 1532, 40), K("loess", 2750, 0), K("mica", 3032, 48),
                K("quartz", 4032, 48), K("loess", 6564, 96, true), K("feldspar", 7856, 32), K("calcite", 8432, 48) },
        crumbles: Bridge(1500).Concat(Bridge(6100)).Concat(Bridge(7800)).Concat(new[] { new Crumble(6400, 40), new Crumble(6470, 64) }).ToArray(),
        sand: new[] { Sd(1800, 400), Sd(4900, 500), Sd(8800, 350) });

    // H7 Tibetan Plateau, the finale: gusts (one over ice), ice, crumbling bridges, two updrafts, three yak herds and a vertical slab
    // over the 160 px gap (Plains Hard H7 / Hills Hard H7 skeleton).
    public static readonly Lvl H7 = Make("H7 Tibetan Plateau",
        new[] { P(1500, 96), P(3000, 64), P(3600, 64), P(3956, 160), P(6100, 96), P(7800, 96), P(8400, 64) },
        new float[] { 1000, 5500, 9250 },
        new[] { S(400), B(1150), S(2200), B(4600), S(5200), B(7550), S(8700) },
        new[] { L(2040, 112, 64), L(3948, 112, 176), L(6540, 96, 48), L(7040, 144, 56) },
        new[] { K("halite", 416, 24), K("granite", 700, 0), K("chromite", 1532, 40), K("serpentinite", 2072, 112), K("halite", 3032, 48),
                K("granite", 4040, 112), K("halite", 6564, 96, true), K("serpentinite", 7068, 144), K("chromite", 8432, 48) },
        crumbles: Bridge(1500).Concat(Bridge(6100)).Concat(Bridge(7800)).Concat(new[] { new Crumble(6400, 40), new Crumble(6470, 64) }).ToArray(),
        winds: new[] { G(3300, 500, 1.0f), G(4250, 350, 0.5f), G(8100, 400, 1.5f) },
        ice: new[] { I(500, 400), I(4200, 400), I(5000, 400), I(8550, 350) })
        with { Bats = new[] { Herd(1300, 4.6f, 0), Herd(2700, 4.8f, 0.5f), Herd(5800, 4.4f, 0.8f) },
               Updrafts = new[] { U(2000, 116), U(7000, 148) },
               Balloons = new[] { new Balloon(3900, 24, 120, 0) } };

    public static readonly Lvl[] All = { H1, H2, H3, H4, H5, H6, H7 };
}
