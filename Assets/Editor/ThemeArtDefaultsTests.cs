using LandGrand.Rules; // Gives the level themes
using NUnit.Framework; // Gives the test checks

public class ThemeArtDefaultsTests // Checks the optional roller, bat and ice art in the theme table (D-071)
{ // Opens the tests
    [Test] // Old themes must keep today's art
    public void OldThemesKeepTheDefaultRollerBatAndIceArt() // Hills logs, Hills bats and Mountains ice stay the same
    { // Opens the check
        CollectionAssert.IsEmpty(LevelBuilder.OldThemesWithNewArt()); // No older theme sets a new field
        CollectionAssert.AreEqual(new[] { "log", "log_warn", "sfx_log_roll.wav", "bat", "3", "sfx_bat_flap.wav", "0", "False", "ice_top" }, LevelBuilder.ObjectArtNames(LevelTheme.RainySlope)); // A log trail keeps the log art
        CollectionAssert.AreEqual(new[] { "log", "log_warn", "sfx_log_roll.wav", "bat", "3", "sfx_bat_flap.wav", "0", "False", "ice_top" }, LevelBuilder.ObjectArtNames(LevelTheme.GlacierHorn)); // An ice trail keeps the ice art
    } // Ends the check

    [TestCase(LevelTheme.Prairie, "hay", "hay_warn", "sfx_hay_roll.wav", "bat", "3", "sfx_bat_flap.wav", "0", "False", "ice_top")] // Hay bales only
    [TestCase(LevelTheme.Savanna, "log", "log_warn", "sfx_log_roll.wav", "wildebeest", "3", "sfx_herd_hooves.wav", "2", "True", "ice_top")] // Wildebeest only
    [TestCase(LevelTheme.Pampas, "hay", "hay_warn", "sfx_hay_roll.wav", "cattle", "3", "sfx_herd_hooves.wav", "2", "True", "ice_top")] // Hay and cattle
    [TestCase(LevelTheme.FrozenPlain, "log", "log_warn", "sfx_log_roll.wav", "bat", "3", "sfx_bat_flap.wav", "0", "False", "frost_top")] // Frozen ground only
    [TestCase(LevelTheme.LimestonePlain, "log", "log_warn", "sfx_log_roll.wav", "bat", "3", "sfx_bat_flap.wav", "0", "False", "ice_top")] // Bats stay bats
    [TestCase(LevelTheme.GangesPlain, "log", "log_warn", "sfx_log_roll.wav", "bat", "3", "sfx_bat_flap.wav", "0", "False", "ice_top")] // Default art
    [TestCase(LevelTheme.Steppe, "hay", "hay_warn", "sfx_hay_roll.wav", "horse", "3", "sfx_herd_hooves.wav", "2", "True", "frost_top")] // Hay, horses and frost
    public void PlainsHardThemesNameTheirObjectArt(LevelTheme theme, params string[] expected) // Follows the spec section 8 table
    { // Opens the check
        CollectionAssert.AreEqual(expected, LevelBuilder.ObjectArtNames(theme)); // Names the right frames, sounds, lift and facing
    } // Ends the check
} // Ends the tests
