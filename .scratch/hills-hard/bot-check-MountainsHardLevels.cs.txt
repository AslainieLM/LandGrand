// Draft Mountains Hard layouts for the bot check (scratch). Same numbers go into the Codex spec.
static class MountainsHardLevels {
    static Boulder S(float x) => new(x, false);
    static Boulder B(float x) => new(x, true);
    static Crumble[] Bridge(float x) => new[] { new Crumble(x, 0), new Crumble(x + 32, 0), new Crumble(x + 64, 0) };
    static readonly float[] CP = { 2400, 4800, 7200 };
    const float Width = 9600, Goal = 9440;

    public static readonly Lvl H1 = new("H1 Mayon", Width, Goal, CP,
        new[] { new Pit(1500, 56), new Pit(3000, 64), new Pit(4200, 64), new Pit(6600, 64), new Pit(8200, 64) },
        new float[] { 1200, 2800, 4000, 5400, 7000, 8400 },
        new[] { S(400), B(1650), S(2150), B(3250), S(5800), B(6800), S(7350), B(7950), S(9200) },
        new[] { new Ledge(1000, 40, 64), new Ledge(3700, 48, 64), new Ledge(6300, 40, 64), new Ledge(6380, 72, 48), new Ledge(8700, 48, 64) },
        new Crumble[0],
        new[] { new Rock(2600, 0), new Rock(4500, 1.0f), new Rock(8600, 0.5f) },
        new Platform[0], new Wind[0], new Vent[0],
        new[] { new Roller(900, 700, 0), new Roller(2000, 1800, 1.0f), new Roller(3600, 3380, 0), new Roller(5200, 4980, 2.0f), new Roller(6200, 5980, 0.5f), new Roller(7800, 7580, 1.5f), new Roller(9000, 8780, 0) },
        new Ice[0],
        new[] { new Pickup("scoria", 1032, 40), new Pickup("hornblende", 1528, 48), new Pickup("scoria", 2600, 0), new Pickup("andesite", 3732, 48), new Pickup("hornblende", 4232, 40),
                new Pickup("scoria", 6332, 40), new Pickup("hornblende", 6404, 72), new Pickup("scoria", 8732, 48), new Pickup("hornblende", 9216, 24) });

    public static readonly Lvl H2 = new("H2 Sierra Madre", Width, Goal, CP,
        new[] { new Pit(1300, 64), new Pit(2900, 96), new Pit(4100, 56), new Pit(5600, 64), new Pit(6800, 96), new Pit(8500, 64) },
        new float[] { 1000, 2600, 3650, 4300, 6200, 7400, 8300 },
        new[] { S(450), B(1600), S(3100), B(3950), S(5300), B(6500), S(7000), B(8000), S(8950) },
        new[] { new Ledge(1700, 40, 64), new Ledge(3200, 48, 64), new Ledge(7700, 40, 64), new Ledge(7780, 72, 48) },
        Bridge(2900).Concat(Bridge(6800)).Concat(new[] { new Crumble(5000, 40), new Crumble(5070, 64) }).ToArray(),
        new Rock[0], new Platform[0],
        new[] { new Wind(700, 500, 0), new Wind(1900, 400, 1.0f), new Wind(3300, 250, 2.0f), new Wind(5800, 320, 0.5f), new Wind(7600, 600, 1.5f), new Wind(8800, 400, 0) },
        new Vent[0],
        new[] { new Roller(2200, 2000, 0), new Roller(4600, 4380, 1.0f), new Roller(9300, 9080, 2.0f) },
        new Ice[0],
        new[] { new Pickup("diorite", 1732, 40), new Pickup("gold", 2932, 40), new Pickup("diorite", 3232, 48), new Pickup("granite", 3974, 36), new Pickup("gold", 5086, 64),
                new Pickup("diorite", 6832, 40), new Pickup("gold", 7804, 72), new Pickup("diorite", 8532, 48), new Pickup("gold", 8966, 24) });

    public static readonly Lvl H3 = new("H3 Kitanglad", Width, Goal, CP,
        new[] { new Pit(1800, 200), new Pit(3300, 56), new Pit(4000, 240), new Pit(5600, 64), new Pit(6400, 200), new Pit(8000, 240), new Pit(8900, 56) },
        new float[] { 1500, 2200, 3600, 5000, 6000, 7000, 8700 },
        new[] { S(450), B(1300), S(2550), B(3800), S(4500), B(5800), S(6800), B(7300), S(8500) },
        new[] { new Ledge(1260, 72, 64), new Ledge(3700, 40, 64), new Ledge(5400, 48, 48) },
        new[] { new Crumble(2700, 40), new Crumble(2770, 64), new Crumble(7500, 40), new Crumble(7570, 64) },
        new Rock[0],
        new[] { new Platform(1000, 1200, 40), new Platform(1800, 1952, 0), new Platform(4000, 4192, 0), new Platform(6400, 6552, 0), new Platform(8000, 8192, 0) },
        new Wind[0], new Vent[0],
        new[] { new Roller(3000, 2850, 0), new Roller(5300, 5080, 1.5f), new Roller(9300, 9100, 0.5f) },
        new Ice[0],
        new[] { new Pickup("tuff", 1292, 72), new Pickup("hematite", 1900, 40), new Pickup("tuff", 2786, 64), new Pickup("hematite", 3724, 40), new Pickup("basalt", 4100, 40),
                new Pickup("tuff", 5424, 48), new Pickup("hematite", 6500, 40), new Pickup("tuff", 7586, 64), new Pickup("hematite", 8100, 32) });

    public static readonly Lvl H4 = new("H4 Hamiguitan", Width, Goal, CP,
        new[] { new Pit(1100, 48), new Pit(1900, 96), new Pit(3200, 64), new Pit(4300, 96), new Pit(5600, 56), new Pit(6600, 96), new Pit(8300, 64) },
        new float[] { 800, 1500, 4000, 5000, 5800, 7000, 9300 },
        new[] { S(450), B(1300), S(2200), B(3800), S(4650), B(5400), S(7400), B(8600) },
        new[] { new Ledge(1400, 40, 64), new Ledge(4900, 48, 64) },
        Bridge(1900).Concat(Bridge(4300)).Concat(Bridge(6600)).Concat(new[] { new Crumble(3500, 40), new Crumble(3570, 64), new Crumble(8800, 40), new Crumble(8870, 64) }).ToArray(),
        new[] { new Rock(2600, 0), new Rock(5200, 1.5f), new Rock(7600, 0.5f), new Rock(9100, 1.0f) },
        new Platform[0], new Wind[0], new Vent[0],
        new[] { new Roller(3000, 2800, 0.5f), new Roller(6200, 5980, 1.0f), new Roller(8100, 7880, 2.0f) },
        new Ice[0],
        new[] { new Pickup("serpentinite", 1432, 40), new Pickup("chromite", 1932, 40), new Pickup("serpentinite", 2600, 0), new Pickup("chromite", 3586, 64), new Pickup("magnetite", 4332, 40),
                new Pickup("serpentinite", 4932, 48), new Pickup("chromite", 6632, 40), new Pickup("serpentinite", 8332, 48), new Pickup("chromite", 8886, 64) });

    public static readonly Lvl H5 = new("H5 Matterhorn", Width, Goal, CP,
        new[] { new Pit(1300, 56), new Pit(2600, 64), new Pit(3800, 96), new Pit(5000, 56), new Pit(6100, 64), new Pit(7400, 96), new Pit(8800, 64) },
        new float[] { 1600, 2900, 4400, 5900, 9020 },
        new[] { S(450), B(1400), S(2200), B(3500), S(4100), B(5500), S(6300), B(7700), S(8300) },
        new[] { new Ledge(2000, 48, 64), new Ledge(5400, 40, 64), new Ledge(5480, 72, 48), new Ledge(8400, 48, 64) },
        Bridge(3800).Concat(Bridge(7400)).ToArray(),
        new Rock[0], new Platform[0],
        new[] { new Wind(1800, 500, 0), new Wind(4200, 500, 1.0f), new Wind(6500, 500, 2.0f), new Wind(9080, 300, 0.5f) },
        new Vent[0],
        new[] { new Roller(4700, 4480, 0), new Roller(7000, 6780, 1.5f), new Roller(9300, 9100, 0.5f) },
        new[] { new Ice(600, 600), new Ice(1800, 500), new Ice(3000, 700), new Ice(5200, 600), new Ice(6500, 500), new Ice(8000, 700) },
        new[] { new Pickup("schist", 2032, 48), new Pickup("calcite", 2632, 48), new Pickup("schist", 3832, 40), new Pickup("calcite", 4116, 24), new Pickup("gneiss", 5432, 40),
                new Pickup("schist", 5504, 72), new Pickup("calcite", 7432, 40), new Pickup("schist", 8432, 48), new Pickup("calcite", 8832, 48) });

    public static readonly Lvl H6 = new("H6 Kilimanjaro", Width, Goal, CP,
        new[] { new Pit(1100, 56), new Pit(2000, 200), new Pit(3100, 64), new Pit(4300, 96), new Pit(5300, 64), new Pit(6700, 240), new Pit(7600, 64), new Pit(9000, 56) },
        new float[] { 800, 2600, 3300, 4650, 6550, 7400 },
        new[] { S(450), B(1300), S(2200), B(3450), S(4550), B(5450), S(6200), B(7000), S(8500) },
        new[] { new Ledge(1540, 100, 72), new Ledge(3740, 100, 72), new Ledge(8600, 48, 64) },
        Bridge(4300),
        new[] { new Rock(2700, 0), new Rock(5000, 1.0f), new Rock(7900, 0.5f) },
        new[] { new Platform(2000, 2152, 0), new Platform(6700, 6892, 0) },
        new Wind[0],
        new[] { new Vent(1500), new Vent(3700) },
        new[] { new Roller(4250, 4060, 1.0f), new Roller(5800, 5580, 0), new Roller(9300, 9080, 2.0f) },
        new[] { new Ice(6000, 500), new Ice(8200, 700) },
        new[] { new Pickup("trachyte", 1576, 100), new Pickup("tanzanite", 2100, 40), new Pickup("trachyte", 2700, 0), new Pickup("tanzanite", 3776, 100), new Pickup("obsidian", 4332, 40),
                new Pickup("trachyte", 5332, 48), new Pickup("tanzanite", 6800, 32), new Pickup("trachyte", 8632, 48), new Pickup("tanzanite", 9028, 48) });

    public static readonly Lvl H7 = new("H7 Mauna Kea", Width, Goal, CP,
        new[] { new Pit(1000, 56), new Pit(1900, 96), new Pit(3000, 200), new Pit(4200, 64), new Pit(5500, 96), new Pit(6400, 200), new Pit(8000, 64), new Pit(8900, 56) },
        new float[] { 700, 2750, 4000, 5000, 7000, 8300, 9200 },
        new[] { S(400), B(1250), S(2850), B(3500), S(4500), B(6200), S(7700), S(8750) },
        new[] { new Ledge(2240, 100, 72), new Ledge(6700, 48, 64), new Ledge(9000, 40, 64) },
        Bridge(1900).Concat(Bridge(5500)).ToArray(),
        new[] { new Rock(2600, 0), new Rock(5100, 1.5f) },
        new[] { new Platform(3000, 3152, 0), new Platform(6400, 6552, 0) },
        new[] { new Wind(7400, 500, 0) },
        new[] { new Vent(2200) },
        new[] { new Roller(1600, 1400, 0), new Roller(3900, 3680, 1.0f), new Roller(6000, 5780, 2.0f), new Roller(8600, 8380, 0.5f) },
        new[] { new Ice(4400, 300), new Ice(7500, 400) },
        new[] { new Pickup("gabbro", 1932, 40), new Pickup("olivine", 2276, 100), new Pickup("gabbro", 3100, 40), new Pickup("olivine", 4516, 24), new Pickup("basalt", 5532, 40),
                new Pickup("gabbro", 6732, 48), new Pickup("olivine", 7716, 24), new Pickup("gabbro", 8032, 48), new Pickup("olivine", 9032, 40) });

    public static readonly Lvl[] All = { H1, H2, H3, H4, H5, H6, H7 };
}
