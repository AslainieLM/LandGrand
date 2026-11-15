using System.Collections; // Gives timed steps for the animation
using UnityEngine; // Gives sprites, colors, and time
using UnityEngine.UI; // Gives the map and banner images

namespace LandGrand // Holds shared screen effects
{ // Opens the game code
    public sealed class UnlockAnimation : MonoBehaviour // Shows an opening padlock and three sparkles
    { // Opens the effect
        public Sprite[] padlockFrames; // Four frames of the padlock opening
        public Sprite[] sparkleFrames; // Three sparkle frames
        public Image[] sparkles; // Three small sparks near the padlock
        const float OpenSeconds = 0.8f; // Time for the padlock to open
        const float FadeSeconds = 0.7f; // Time for the grey region to fade

        public IEnumerator Play(Image padlock, Image grey = null) // Plays on the map or on the summary banner
        { // Starts the animation
            padlock.rectTransform.sizeDelta = padlockFrames[0].rect.size; // Uses the unlock art's size
            for (float age = 0f; age < OpenSeconds + FadeSeconds; age += Time.unscaledDeltaTime) // Runs at the same speed as the screen
            { // Draws one frame
                int frame = Mathf.Min(padlockFrames.Length - 1, (int)(age * padlockFrames.Length / OpenSeconds)); // Opens the padlock in four steps
                padlock.sprite = padlockFrames[frame]; // Shows this opening frame
                float fade = Mathf.Clamp01((age - OpenSeconds) / FadeSeconds); // Starts to fade after the lock opens
                if (grey != null) grey.color = new Color(1f, 1f, 1f, 1f - fade); // Lets the colour show through
                ShowSparkles(age); // Draws the staggered sparkles
                yield return null; // Waits for the next frame
            } // Ends the timing loop
            padlock.sprite = padlockFrames[padlockFrames.Length - 1]; // Leaves the opened lock on the summary
            foreach (var spark in sparkles) spark.gameObject.SetActive(false); // All three sparkles go away
            if (grey != null) grey.color = Color.clear; // The map keeps an invisible tap area
        } // Ends the effect

        void ShowSparkles(float age) // Draws the three short twinkles
        { // Starts the twinkle check
            for (int i = 0; i < sparkles.Length; i++) // Gives each spark its own start
            { // Draws one spark
                float time = age - 0.55f - i * 0.14f; // Spreads them over the opening and fade
                int frame = time < 0f ? -1 : (int)(time * 8f); // Reads its frame only after it starts
                bool visible = frame >= 0 && frame < sparkleFrames.Length; // Goes away after three frames
                sparkles[i].gameObject.SetActive(visible); // Shows or hides this spark
                if (visible) sparkles[i].sprite = sparkleFrames[frame]; // Shows this twinkle frame
            } // Ends one spark
        } // Ends the twinkle check
    } // Ends the effect
} // Ends the game code
