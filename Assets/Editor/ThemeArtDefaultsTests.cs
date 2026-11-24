using LandGrand.Rules; // Gives the level themes
using NUnit.Framework; // Gives the test checks
using System.Linq; // Gives helpers for the list of themes
using UnityEditor; // Loads the saved sprites
using UnityEngine; // Gives objects and sprite renderers
using LandGrand.Play; // Gives the moving platform view

public class ThemeArtDefaultsTests // Checks the optional roller, bat and ice art in the theme table (D-071)
{ // Opens the tests
    [Test] // Old themes must keep today's art
    public void OldThemesKeepTheDefaultRollerBatAndIceArt() // Hills logs, Hills bats and Mountains ice stay the same
    { // Opens the check
        CollectionAssert.IsEmpty(LevelBuilder.OldThemesWithNewArt()); // No older theme sets a new field
        CollectionAssert.AreEqual(new[] { "log", "log_warn", "sfx_log_roll.wav", "bat", "3", "sfx_bat_flap.wav", "0", "False", "ice_top" }, LevelBuilder.ObjectArtNames(LevelTheme.RainySlope)); // A log trail keeps the log art
        CollectionAssert.AreEqual(new[] { "log", "log_warn", "sfx_log_roll.wav", "bat", "3", "sfx_bat_flap.wav", "0", "False", "ice_top" }, LevelBuilder.ObjectArtNames(LevelTheme.GlacierHorn)); // An ice trail keeps the ice art
    } // Ends the check

    [Test] // New fields must leave older platforms alone
    public void OlderThemesKeepTheirPlatformArtDefaults() // Checks all the themes before Plateaus Normal
    { // Opens the default check
        var older = System.Enum.GetValues(typeof(LevelTheme)).Cast<LevelTheme>().Where(theme => theme < LevelTheme.TableTop); // Lists all older themes
        foreach (var theme in older) // Checks the art settings of each theme
        { // Opens the theme check
            var (above, cable) = LevelBuilder.PlatformArtSettings(theme); // Reads the offset and cable
            Assert.AreEqual(0, above, theme.ToString()); // Leaves the sprite below the solid top
            Assert.IsNull(cable, theme.ToString()); // Leaves the crossing without a cable
        } // Ends the theme check
        Assert.AreEqual((24, "cable_line"), LevelBuilder.PlatformArtSettings(LevelTheme.TableTop)); // Aligns the cable car roof and strip
    } // Ends the default check

    [TestCase("platform", 40f, 0, 35f)] // The 10-pixel slab still sits below the top
    [TestCase("market_boat", 0f, 0, -8f)] // The 16-pixel boat still sits below the top
    [TestCase("cable_car", 0f, 24, 0f)] // The roof is halfway down the 48-pixel picture
    public void MovingPlatformArtKeepsItsStandingRowOnTheSolidTop(string name, float top, int above, float centerY) // Checks the art placement used during play
    { // Opens the placement check
        var go = new GameObject("Platform Art Check"); // Holds the view for this test
        try // Always clean up the test object
        { // Opens the check
            var view = go.AddComponent<MovingPlatformView>(); // Uses the real platform view
            view.art = go.AddComponent<SpriteRenderer>(); // Holds the sprite used by the view
            view.art.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Art/Level/Sprites/{name}.png"); // Reads the authored sprite
            Assert.IsNotNull(view.art.sprite); // The sprite must be present
            view.top = top; // Sets the solid top
            view.artAbove = above; // Sets the theme's standing row
            Assert.AreEqual(centerY, view.ArtCenterY); // Checks where the sprite's middle belongs
        } // Ends the check
        finally { Object.DestroyImmediate(go); } // Removes the test object from the scene
    } // Ends the placement check

    [TestCase(LevelTheme.Prairie, "hay", "hay_warn", "sfx_hay_roll.wav", "bat", "3", "sfx_bat_flap.wav", "0", "False", "ice_top")] // Hay bales only
    [TestCase(LevelTheme.Savanna, "log", "log_warn", "sfx_log_roll.wav", "wildebeest", "3", "sfx_herd_hooves.wav", "2", "True", "ice_top")] // Wildebeest only
    [TestCase(LevelTheme.Pampas, "hay", "hay_warn", "sfx_hay_roll.wav", "cattle", "3", "sfx_herd_hooves.wav", "2", "True", "ice_top")] // Hay and cattle
    [TestCase(LevelTheme.FrozenPlain, "log", "log_warn", "sfx_log_roll.wav", "bat", "3", "sfx_bat_flap.wav", "0", "False", "frost_top")] // Frozen ground only
    [TestCase(LevelTheme.LimestonePlain, "log", "log_warn", "sfx_log_roll.wav", "bat", "3", "sfx_bat_flap.wav", "0", "False", "ice_top")] // Bats stay bats
    [TestCase(LevelTheme.GangesPlain, "log", "log_warn", "sfx_log_roll.wav", "bat", "3", "sfx_bat_flap.wav", "0", "False", "ice_top")] // Default art
    [TestCase(LevelTheme.Steppe, "hay", "hay_warn", "sfx_hay_roll.wav", "horse", "3", "sfx_herd_hooves.wav", "2", "True", "frost_top")] // Hay, horses and frost
    [TestCase(LevelTheme.SaltFlat, "log", "log_warn", "sfx_log_roll.wav", "llama", "3", "sfx_llama.wav", "2", "False", "salt_crust_top")] // Keeps the llama art and salt crust on this theme
    public void ThemesNameTheirObjectArt(LevelTheme theme, params string[] expected) // Follows the spec section 8 table
    { // Opens the check
        CollectionAssert.AreEqual(expected, LevelBuilder.ObjectArtNames(theme)); // Names the right frames, sounds, lift and facing
    } // Ends the check
} // Ends the tests
