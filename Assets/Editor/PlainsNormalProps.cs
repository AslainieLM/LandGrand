using System.Collections.Generic; // Uses the hazard list
using LandGrand.Play; // Uses the explorer for the theme callback
using LandGrand.Rules; // Uses level and hazard data
using UnityEngine; // Uses the parent transform

public static partial class LevelBuilder // Builds Plains Normal decorations
{ // Opens the trail props
    static void CandabaProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds wetland plants and birds
    { // Starts the swamp props
        RepeatTrailProp(level, parent, "reed_tuft", PixelSceneKit.PlainsNormalArtScript, 120f, 260f, 7f, 3, clearFullZones: true); // Adds reeds along the swamp
        PlaceTrailAnimals(level, parent, "duck", new[] { 1200f, 3000f, 5500f }, 5f, PixelSceneKit.PlainsNormalArtScript, clearFullZones: true); // Places the three ducks
        PlaceTrailAnimals(level, parent, "egret", new[] { 2200f, 4600f }, 5f, clearFullZones: true); // Reuses the two-frame egrets
    } // Ends the swamp props

    static void LaharProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds warning signs and reeds
    { // Starts the lahar props
        PlaceTrailProps(level, parent, "lahar_sign", PixelSceneKit.PlainsNormalArtScript, new[] { 450f, 3000f, 5900f }, 10f, 3, clearFullZones: true); // Places three signs on dry ground
        RepeatTrailProp(level, parent, "reed_tuft", PixelSceneKit.PlainsNormalArtScript, 200f, 400f, 7f, 7, clearFullZones: true); // Fits reed tufts around the sand
    } // Ends the lahar props

    static void DeltaProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds fish-pen stakes, boats and an egret
    { // Starts the delta props
        PlaceTrailProps(level, parent, "fish_stakes", PixelSceneKit.PlainsNormalArtScript, new[] { 900f, 2500f, 4300f, 6100f }, 8f, 2, clearFullZones: true); // Places four groups of stakes
        PlaceTrailProps(level, parent, "bangka_plains", PixelSceneKit.PlainsNormalArtScript, new[] { 1800f, 5000f }, 6f, 3, clearFullZones: true); // Places two boats on the bank
        PlaceTrailAnimals(level, parent, "egret", new[] { 3300f }, 5f, clearFullZones: true); // Reuses the Plains egret
    } // Ends the delta props

    static void MarshProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds marsh plants and crocodiles
    { // Starts the marsh props
        RepeatTrailProp(level, parent, "water_hyacinth", PixelSceneKit.PlainsNormalArtScript, 160f, 300f, 4f, 7, clearFullZones: true); // Fits the plants around trail objects
        PlaceTrailProps(level, parent, "crocodile", PixelSceneKit.PlainsNormalArtScript, new[] { 2300f, 5400f }, 4f, 2, clearFullZones: true); // Places two resting crocodiles
        RepeatTrailProp(level, parent, "reed_tuft", PixelSceneKit.PlainsNormalArtScript, 400f, 500f, 7f, 3, clearFullZones: true); // Leaves room for the sand and water
    } // Ends the marsh props

    static void MarketProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds crates and palms to the market banks
    { // Starts the market props
        PlaceTrailProps(level, parent, "fruit_crate", PixelSceneKit.PlainsNormalArtScript, new[] { 600f, 2900f, 4700f, 6500f }, 5f, 4, clearFullZones: true); // Places four fruit crates
        RepeatTrailProp(level, parent, "palm_tree", HillsArtScript, 350f, 700f, 20f, 3, clearFullZones: true); // Uses the existing palms
    } // Ends the market props
} // Closes the trail props
