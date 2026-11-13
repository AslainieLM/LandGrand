using UnityEngine; // Gives access to MonoBehaviour
using UnityEngine.UI; // Gives access to Slider

namespace LandGrand.MainMenu
{
    /// <summary>
    /// The Settings panel: one slider for music volume and one for sound-effects volume.
    /// Moving a slider changes the volume at once; closing the panel saves both values.
    /// </summary>
    public class SettingsPanel : MonoBehaviour
    {
        public Slider musicSlider; // Slider that sets the music volume
        public Slider sfxSlider; // Slider that sets the sound-effects volume

        void Awake() // Runs once, the first time the panel opens: connects the sliders
        {
            musicSlider.onValueChanged.AddListener(v => GameSettings.MusicVolume = v); // Moving the music slider changes the music volume
            sfxSlider.onValueChanged.AddListener(v => GameSettings.SfxVolume = v); // Moving the sound slider changes the sound-effects volume
        }

        void OnEnable() // Runs each time the panel opens
        {
            musicSlider.SetValueWithoutNotify(GameSettings.MusicVolume); // Show the saved music volume when the panel opens
            sfxSlider.SetValueWithoutNotify(GameSettings.SfxVolume); // Show the saved sound volume when the panel opens
        }

        void OnDisable() // Runs each time the panel closes
        {
            GameSettings.Save(); // Write both volumes to the device when the panel closes
        }
    }
}
