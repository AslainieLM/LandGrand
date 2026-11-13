using LandGrand.Rules; // Gives access to NameRule
using TMPro; // Gives access to TMP_InputField and TMP_Text
using UnityEngine; // Gives access to MonoBehaviour
using UnityEngine.UI; // Gives access to Slider

namespace LandGrand.MainMenu // Main Menu scripts live here
{
    /// <summary>
    /// The Settings panel: one slider for music volume, one for sound-effects volume, and the player's name.
    /// Moving a slider changes the volume at once; closing the panel saves both values.
    /// A good new name is saved when typing ends or the panel closes.
    /// </summary>
    public class SettingsPanel : MonoBehaviour // Panel with volume sliders and the name box
    {
        public Slider musicSlider; // Slider that sets the music volume
        public Slider sfxSlider; // Slider that sets the sound-effects volume
        public TMP_InputField nameInput; // Box that shows and changes the player's name
        public TMP_Text nameMessage; // Shows why a new name was refused

        void Awake() // Runs once, the first time the panel opens: connects the sliders
        {
            musicSlider.onValueChanged.AddListener(v => GameSettings.MusicVolume = v); // Moving the music slider changes the music volume
            sfxSlider.onValueChanged.AddListener(v => GameSettings.SfxVolume = v); // Moving the sound slider changes the sound-effects volume
            nameInput.characterLimit = NameRule.MaxLength; // The keyboard stops at 12 characters
            nameInput.onEndEdit.AddListener(_ => ApplyName(true)); // When typing ends, check and save the name
            nameInput.onValueChanged.AddListener(_ => nameMessage.text = ""); // Typing again clears the old message
        }

        void OnEnable() // Runs each time the panel opens
        {
            musicSlider.SetValueWithoutNotify(GameSettings.MusicVolume); // Show the saved music volume when the panel opens
            sfxSlider.SetValueWithoutNotify(GameSettings.SfxVolume); // Show the saved sound volume when the panel opens
            nameInput.SetTextWithoutNotify(GameProgress.Current.Name); // Show the saved name (empty before the first Play)
            nameMessage.text = ""; // No message yet
        }

        void OnDisable() // Runs each time the panel closes
        {
            ApplyName(false); // Keep a good name that was typed but not confirmed yet
            GameSettings.Save(); // Write both volumes to the device when the panel closes
        }

        void ApplyName(bool showMessage) // Checks the typed name and saves it when it is good
        {
            if (nameInput.text == GameProgress.Current.Name) return; // Nothing changed
            if (!NameRule.TryClean(nameInput.text, out var name, out var problem)) // If the new name is not good
            {
                if (showMessage) nameMessage.text = problem; // Tell the player what to fix
                return; // Keep the old name
            }
            GameProgress.Current.Name = name; // Keep the new name in the player's progress
            GameProgress.Save(); // Write it to the device now
            nameInput.SetTextWithoutNotify(name); // Show the cleaned name
            nameMessage.text = ""; // Clear any old message
        }
    }
}
