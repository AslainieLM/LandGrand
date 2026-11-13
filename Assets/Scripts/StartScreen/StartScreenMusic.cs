using System.Collections; // Gives access to IEnumerator for the fade-out
using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and AudioSource

namespace LandGrand.StartScreen
{
    /// <summary>
    /// Plays the start-screen music and the wind-and-birds ambience as two seamless loops.
    /// Both fade in when the screen opens and can fade out before the next scene loads.
    /// </summary>
    public class StartScreenMusic : MonoBehaviour
    {
        public AudioClip music; // The Quest music loop
        public AudioClip ambience; // The wind-and-birds loop
        [Range(0f, 1f)] public float musicVolume = 0.55f; // Base music loudness
        [Range(0f, 1f)] public float ambienceVolume = 0.35f; // Base ambience loudness
        public float fadeInSeconds = 1.5f; // How long the fade-in takes

        AudioSource musicSource; // Plays the music
        AudioSource ambienceSource; // Plays the ambience
        float fade;          // 0..1 multiplier applied to both volumes
        float fadeTarget = 1f; // Fade level to move toward
        float fadeRate; // How fast the fade moves, per second

        void Awake() // Runs once when the object is created
        {
            musicSource = CreateSource(music); // Make the music player
            ambienceSource = CreateSource(ambience); // Make the ambience player
        }

        AudioSource CreateSource(AudioClip clip) // Makes one looping, silent sound player
        {
            var source = gameObject.AddComponent<AudioSource>(); // New sound player
            source.clip = clip; // Sound to play
            source.loop = true; // Play it again and again
            source.playOnAwake = false; // Wait for Start() to begin
            source.volume = 0f; // Start silent
            return source; // Give it back
        }

        void Start() // Runs once before the first frame
        {
            if (music != null) musicSource.Play(); // Start the music
            if (ambience != null) ambienceSource.Play(); // Start the ambience
            fade = 0f; // Begin silent
            fadeTarget = 1f; // Fade up to full
            fadeRate = fadeInSeconds > 0f ? 1f / fadeInSeconds : float.MaxValue; // Speed of the fade-in
        }

        void Update() // Runs every frame
        {
            fade = Mathf.MoveTowards(fade, fadeTarget, fadeRate * Time.unscaledDeltaTime); // Move the fade level toward its target
            float setting = GameSettings.MusicVolume; // The player's music volume from Settings (0 to 1)
            musicSource.volume = musicVolume * fade * setting; // Music loudness: base level, fade, and the player's setting
            ambienceSource.volume = ambienceVolume * fade * setting; // Wind-and-birds loudness: base level, fade, and the player's setting
        }

        /// <summary>Fades both loops to silence over the given time. Wait on the returned routine to know when it is done.</summary>
        public IEnumerator FadeOut(float seconds) // Fades out; the caller can wait for it
        {
            fadeTarget = 0f; // Fade down to silence
            fadeRate = seconds > 0f ? 1f / seconds : float.MaxValue; // Speed of the fade-out
            while (fade > 0f) yield return null; // Wait frame by frame until silent
            musicSource.Stop(); // Stop the music
            ambienceSource.Stop(); // Stop the ambience
        }
    }
}
