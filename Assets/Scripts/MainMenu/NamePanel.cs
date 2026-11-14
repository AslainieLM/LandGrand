using System; // Gives access to Action
using LandGrand.Rules; // Gives access to NameRule
using TMPro; // Gives access to TMP_InputField and TMP_Text
using UnityEngine; // Gives access to MonoBehaviour
using UnityEngine.UI; // Gives access to Button

namespace LandGrand.MainMenu // Main Menu scripts live here
{
    /// <summary>
    /// The name panel that Play shows the first time, and that the pencil in the Profile opens to change the name (D-052).
    /// The player types a name with the phone's keyboard.
    /// A good name (1 to 12 letters, numbers or spaces) is saved at once; a bad one shows a short message.
    /// </summary>
    public class NamePanel : MonoBehaviour // Panel that asks for the name
    {
        public TMP_InputField input; // Where the player types the name
        public TMP_Text message; // Shows why a name was refused
        public Button okButton; // Confirms the name

        /// <summary>Called after a good name was saved.</summary>
        public event Action Accepted; // The Main Menu opens the Landform Map; the Profile shows the new name

        void Awake() // Runs once, the first time the panel opens: connects the controls
        {
            okButton.onClick.AddListener(Submit); // OK checks and saves the name
            input.onSubmit.AddListener(_ => Submit()); // The keyboard's Done key does the same
            input.onValueChanged.AddListener(_ => message.text = ""); // Typing again clears the old message
            input.characterLimit = NameRule.MaxLength; // The keyboard stops at 12 characters
        }

        void OnEnable() // Runs each time the panel opens
        {
            input.SetTextWithoutNotify(GameProgress.Current.Name); // Start with the saved name, so a change from the Profile begins from it (empty the first time)
            message.text = ""; // No message yet
            input.ActivateInputField(); // Open the keyboard at once
        }

        void Submit() // Checks the typed name and saves it
        {
            if (!NameRule.TryClean(input.text, out var name, out var problem)) // If the name is not good
            {
                message.text = problem; // Tell the player what to fix
                return; // Stay on this panel
            }
            GameProgress.Current.Name = name; // Keep the name in the player's progress
            GameProgress.Save(); // Write it to the device now
            Accepted?.Invoke(); // Tell the menu to carry on
        }
    }
}
