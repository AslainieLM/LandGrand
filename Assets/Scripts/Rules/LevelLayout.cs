using System.Collections.Generic; // Gives IReadOnlyList
using System.Linq; // Gives ToArray

namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>How one level looks inside its landform: its ground, props and goal (CONTEXT.md "Level theme").</summary>
    public enum LevelTheme { Meadow, Volcanic, FoldedRock, PineForest, AshGorge, MossyForest, SnowPeak, LavaSlope, StormRidge, EagleForest, BonsaiForest, GlacierHorn, EquatorSummit, SeaPeak, ChocolateHills, RollingPasture, SandDunes, IslandHills, LimestoneCave, RiverGorge, RiceTerraces, FairyChimneys, TarsierForest, RainySlope, KarstRidge, WindHills, DrumlinBay, CityHills, KarstTowers, RicePlain, SaltBeds, RiverPlain, CandabaWetland, LaharPlain, RiverDelta, FloatingVillage, FloatingMarket } // Adds the five Plains Normal themes

    /// <summary>The two boulder pictures: small is 32x24 pixels, big is 48x36.</summary>
    public enum BoulderSize { Small, Big } // Boulder list

    /// <summary>One boulder sitting on the ground: its left edge and its size.</summary>
    public readonly struct Boulder // One boulder
    {
        public readonly float X; // Left edge in level pixels
        public readonly BoulderSize Size; // Small or big

        public Boulder(float x, BoulderSize size) { X = x; Size = size; } // Makes a boulder

        /// <summary>Width of the boulder in pixels.</summary>
        public float Width => Size == BoulderSize.Small ? 32f : 48f; // Same as its picture
    }

    /// <summary>One solid stone slab: its left edge, the height of its top and its width. It is 10 pixels thick.</summary>
    public readonly struct Ledge // One ledge
    {
        public readonly float X; // Left edge in level pixels
        public readonly float Top; // Height of its top above the ground line
        public readonly float Width; // Width in pixels

        public Ledge(float x, float top, float width) { X = x; Top = top; Width = width; } // Makes a ledge
    }

    public readonly struct ShallowPool // Marks a safe water gap with a floor
    { // Opens the pool
        public float X { get; } // The left edge of the pool
        public float Width { get; } // The width in pixels

        public ShallowPool(float x, float width) // Places a safe practice pool
        { // Opens the placement
            X = x; // Saves the left edge
            Width = width; // Saves the pool width
        } // Ends the placement
    } // Ends the pool

    /// <summary>
    /// The scenery part of a level's layout (CONTEXT.md "Level layout"): its width, theme, boulders, ledges and steam vents.
    /// The rest of the layout (pits, spikes, pickups, checkpoint and goal) sits on LevelData. One builder makes every level from both.
    /// </summary>
    public sealed class LevelLayout // One level's layout
    {
        public float Width { get; } // Level width in pixels
        public LevelTheme Theme { get; } // Which ground, props and goal it uses
        public IReadOnlyList<Boulder> Boulders { get; } // Boulders on the ground
        public IReadOnlyList<Ledge> Ledges { get; } // Stone slabs to stand on
        public IReadOnlyList<SteamVent> Vents { get; } // Steam vents that launch the explorer (D-055)

        public IReadOnlyList<CrumbleLedge> CrumbleLedges { get; } // Ledges that break
        public IReadOnlyList<Rockfall> Rockfalls { get; } // Falling rocks
        public IReadOnlyList<MovingPlatform> MovingPlatforms { get; } // Platforms that carry the explorer
        public IReadOnlyList<WindZone> WindZones { get; } // Areas with gusts

        public IReadOnlyList<RollingBoulder> RollingBoulders { get; } // Boulders that roll along the trail
        public IReadOnlyList<IceZone> IceZones { get; } // Slippery stretches of ground
        public IReadOnlyList<SandZone> SandZones { get; } // Soft sand that slows walking (D-066)

        public IReadOnlyList<ShallowPool> Pools { get; } // Safe water with a floor
        public IReadOnlyList<SinkingFloat> Floats { get; } // Rafts on pools or pits
        public IReadOnlyList<Bat> Bats { get; } // Gentle bats that circle the trail
        public IReadOnlyList<CurrentZone> Currents { get; } // Water strips that push the explorer
        public IReadOnlyList<MudZone> MudZones { get; } // Slippery mud stretches on the trail
        public IReadOnlyList<RollingLog> Logs { get; } // Logs that roll along the trail
        public IReadOnlyList<StrawPile> StrawPiles { get; } // Straw piles that launch the explorer
        public string AmbienceOverride { get; } // Optional ambience file name without its extension

        public LevelLayout(float width, LevelTheme theme, Boulder[] boulders, Ledge[] ledges, SteamVent[] vents = null, CrumbleLedge[] crumbleLedges = null, Rockfall[] rockfalls = null, MovingPlatform[] movingPlatforms = null, WindZone[] windZones = null, RollingBoulder[] rollingBoulders = null, IceZone[] iceZones = null, SandZone[] sandZones = null, SinkingFloat[] floats = null, Bat[] bats = null, CurrentZone[] currents = null, string ambienceOverride = null, ShallowPool[] pools = null, MudZone[] mudZones = null, RollingLog[] logs = null, StrawPile[] strawPiles = null) // Makes a layout
        {
            MudZones = (mudZones ?? new MudZone[0]).ToArray(); // Copy the mud stretches
            Logs = (logs ?? new RollingLog[0]).ToArray(); // Copy the logs
            StrawPiles = (strawPiles ?? new StrawPile[0]).ToArray(); // Copy the straw piles
            Pools = (pools ?? new ShallowPool[0]).ToArray(); // Copy the safe pools
            Floats = (floats ?? new SinkingFloat[0]).ToArray(); // Copy the rafts
            Bats = (bats ?? new Bat[0]).ToArray(); // Copy the bat loops
            Currents = (currents ?? new CurrentZone[0]).ToArray(); // Copy the water strips
            AmbienceOverride = ambienceOverride; // Save the optional sound name
            SandZones = (sandZones ?? new SandZone[0]).ToArray(); // Copy the soft sand; most levels have none
            CrumbleLedges = (crumbleLedges ?? new CrumbleLedge[0]).ToArray(); // Copy the breakable ledges
            Rockfalls = (rockfalls ?? new Rockfall[0]).ToArray(); // Copy the rocks
            MovingPlatforms = (movingPlatforms ?? new MovingPlatform[0]).ToArray(); // Copy the platforms
            WindZones = (windZones ?? new WindZone[0]).ToArray(); // Copy the gust areas
            RollingBoulders = (rollingBoulders ?? new RollingBoulder[0]).ToArray(); // Copy the rolling boulders
            IceZones = (iceZones ?? new IceZone[0]).ToArray(); // Copy the ice zones
            Width = width; // Remember the width
            Theme = theme; // Remember the theme
            Boulders = boulders.ToArray(); // Keep our own copy, so nobody can change the layout later
            Ledges = ledges.ToArray(); // Keep our own copy
            Vents = (vents ?? new SteamVent[0]).ToArray(); // Keep our own copy; most levels have no vents
        }

        /// <summary>An empty meadow layout of a given width, for levels that are only used in tests.</summary>
        public static LevelLayout Plain(float width) => new LevelLayout(width, LevelTheme.Meadow, new Boulder[0], new Ledge[0]); // No boulders, ledges or vents
    }
}
