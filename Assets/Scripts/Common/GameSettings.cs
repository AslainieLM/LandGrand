using UnityEngine; // Gives access to PlayerPrefs and Mathf

namespace LandGrand
{
    /// <summary>
    /// Player preferences that every scene shares: music volume and sound-effects volume.
    /// Values are saved on this device with PlayerPrefs, so they stay after the game closes.
    /// </summary>
    public static class GameSettings
    {
        const string MusicKey = "LandGrand.MusicVolume"; // Save name for the music volume
        const string SfxKey = "LandGrand.SfxVolume"; // Save name for the sound-effects volume

        static float? music; // Music volume kept in memory, so it is read from the device only once
        static float? sfx; // Sound-effects volume kept in memory, so it is read from the device only once

        /// <summary>Music volume from 0 (silent) to 1 (full). Starts at full the first time.</summary>
        public static float MusicVolume // Read or change the music volume
        {
            get => music ??= PlayerPrefs.GetFloat(MusicKey, 1f); // Use the value in memory, or read the saved one (1 if none)
            set { music = Mathf.Clamp01(value); PlayerPrefs.SetFloat(MusicKey, music.Value); } // Keep it between 0 and 1 and remember it; Save() writes it to the device
        }

        /// <summary>Sound-effects volume from 0 (silent) to 1 (full). Starts at full the first time.</summary>
        public static float SfxVolume // Read or change the sound-effects volume
        {
            get => sfx ??= PlayerPrefs.GetFloat(SfxKey, 1f); // Use the value in memory, or read the saved one (1 if none)
            set { sfx = Mathf.Clamp01(value); PlayerPrefs.SetFloat(SfxKey, sfx.Value); } // Keep it between 0 and 1 and remember it; Save() writes it to the device
        }

        /// <summary>Writes the settings to the device now. Call it when the player closes the Settings panel.</summary>
        public static void Save() => PlayerPrefs.Save(); // Write every changed setting to the device's storage
    }
}
