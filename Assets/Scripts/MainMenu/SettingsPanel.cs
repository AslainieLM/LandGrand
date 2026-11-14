using System; // Gives access to Action
using UnityEngine; // Gives access to MonoBehaviour
using UnityEngine.UI; // Gives access to Slider and Button

namespace LandGrand.MainMenu // Main Menu scripts live here
{
    /// <summary>
    /// The Settings panel: one slider for music volume and one for sound-effects volume.
    /// Moving a slider changes the volume at once, so the player can hear it. Save keeps the volumes and closes
    /// the panel; Back (the arrow icon, or Android's Back button) closes it and puts the old volumes back.
    /// The name is changed in the Profile (D-052).
    /// </summary>
    public class SettingsPanel : MonoBehaviour // Panel with volume sliders and Save
    {
        public Slider musicSlider; // Slider that sets the music volume
        public Slider sfxSlider; // Slider that sets the sound-effects volume
        public Button saveButton; // Keeps the volumes and closes the panel

        /// <summary>Raised after Save has kept the changes; the Main Menu closes the panel.</summary>
        public event Action Saved; // The Main Menu listens to this

        float oldMusic; // Music volume when the panel opened, put back if the player leaves without saving
        float oldSfx; // Sound volume when the panel opened, put back if the player leaves without saving
        bool saved; // True once Save was tapped since the panel opened

        void Awake() // Runs once, the first time the panel opens: connects the sliders and Save
        {
            musicSlider.onValueChanged.AddListener(v => GameSettings.MusicVolume = v); // Moving the music slider changes the music volume now, so the player hears it
            sfxSlider.onValueChanged.AddListener(v => GameSettings.SfxVolume = v); // Moving the sound slider changes the sound-effects volume now
            saveButton.onClick.AddListener(Save); // Save keeps the volumes
        }

        void OnEnable() // Runs each time the panel opens
        {
            oldMusic = GameSettings.MusicVolume; // Remember the music volume, in case the player leaves without saving
            oldSfx = GameSettings.SfxVolume; // Remember the sound volume too
            saved = false; // Nothing saved yet this time
            musicSlider.SetValueWithoutNotify(oldMusic); // Show the saved music volume when the panel opens
            sfxSlider.SetValueWithoutNotify(oldSfx); // Show the saved sound volume when the panel opens
        }

        void OnDisable() // Runs each time the panel closes
        {
            if (saved) return; // Save already kept everything
            GameSettings.MusicVolume = oldMusic; // Leaving without saving: put the old music volume back
            GameSettings.SfxVolume = oldSfx; // And the old sound volume
        }

        void Save() // What the Save button does
        {
            GameSettings.Save(); // Write both volumes to the device
            saved = true; // Do not put the old values back when the panel closes
            Saved?.Invoke(); // Tell the Main Menu to close the panel
        }
    }
}
