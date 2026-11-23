using System.Collections.Generic; // Uses the hazard list
using LandGrand.Play; // Uses the explorer for the theme callback
using LandGrand.Rules; // Uses level and hazard data
using UnityEngine; // Uses the parent transform

public static partial class LevelBuilder // Builds Plains Hard decorations (D-071, spec section 8)
{ // Opens the trail props
    const string PlainsHardArt = PixelSceneKit.PlainsHardArtScript; // Short name for the Plains Hard art script

    static void PrairieProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds fences, hay rolls and wind pumps
    { // Starts the prairie props
        RepeatTrailProp(level, parent, "fence_post", PlainsHardArt, 140f, 240f, 8f, 3, clearFullZones: true); // Fence posts every 240 px
        PlaceTrailProps(level, parent, "hay_roll_prop", PlainsHardArt, new[] { 1400f, 3400f, 5800f, 8200f }, 8f, 2, clearFullZones: true); // Four resting hay rolls
        PlaceTrailProps(level, parent, "windpump", PlainsHardArt, new[] { 2200f, 6400f }, 24f, 1, clearFullZones: true); // Two far wind pumps
    } // Ends the prairie props

    static void SavannaProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds acacias and zebras
    { // Starts the savanna props
        RepeatTrailProp(level, parent, "acacia_tree", PlainsHardArt, 300f, 900f, 24f, 1, clearFullZones: true); // Acacia trees every 900 px
        PlacePlainsAnimals(level, parent, "zebra", new[] { 2600f, 7000f }, 8f, PlainsHardArt, clearFullZones: true, order: 2); // Two grazing zebras
    } // Ends the savanna props

    static void PampasProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds pampas grass and fences
    { // Starts the pampas props
        RepeatTrailProp(level, parent, "pampas_grass", PlainsHardArt, 180f, 300f, 10f, 3, clearFullZones: true); // Pampas grass every 300 px
        RepeatTrailProp(level, parent, "fence_post", PlainsHardArt, 400f, 500f, 8f, 3, clearFullZones: true); // Fence posts every 500 px
    } // Ends the pampas props

    static void FrozenPlainProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds birches and snow tufts
    { // Starts the cold props
        RepeatTrailProp(level, parent, "birch_tree", PlainsHardArt, 250f, 700f, 24f, 1, clearFullZones: true); // Birch trees every 700 px
        RepeatTrailProp(level, parent, "snow_tuft", PlainsHardArt, 120f, 350f, 4f, 3, clearFullZones: true); // Snow tufts every 350 px
    } // Ends the cold props

    static void LimestonePlainProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds saltbush only; the name means "no tree"
    { // Starts the limestone props
        RepeatTrailProp(level, parent, "saltbush", PlainsHardArt, 220f, 450f, 6f, 3, clearFullZones: true); // Saltbush every 450 px
    } // Ends the limestone props

    static void GangesPlainProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds wheat, egrets and reeds
    { // Starts the flood plain props
        RepeatTrailProp(level, parent, "wheat_sheaf", PlainsHardArt, 300f, 600f, 8f, 3, clearFullZones: true); // Wheat sheaves every 600 px on dry ground
        PlacePlainsAnimals(level, parent, "egret", new[] { 2800f, 6800f }, 5f, clearFullZones: true); // Reuses the two-frame egrets
        RepeatTrailProp(level, parent, "reed_tuft", PixelSceneKit.PlainsNormalArtScript, 450f, 500f, 7f, 3, clearFullZones: true); // Reuses the Plains Normal reeds every 500 px
    } // Ends the flood plain props

    static void SteppeProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds steppe grass and eagles
    { // Starts the steppe props
        RepeatTrailProp(level, parent, "steppe_grass", PlainsHardArt, 140f, 260f, 6f, 3, clearFullZones: true); // Steppe grass every 260 px
        PlacePlainsAnimals(level, parent, "eagle", new[] { 3000f, 7600f }, 120f, ArtScript, clearFullZones: true, order: 1); // Reuses the eagle frames high in the sky
    } // Ends the steppe props
} // Closes the trail props
