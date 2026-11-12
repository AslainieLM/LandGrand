using System.Collections;
using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>
    /// Plays the start-screen music and the wind-and-birds ambience as two seamless loops.
    /// Both fade in when the screen opens and can fade out before the next scene loads.
    /// </summary>
    public class StartScreenMusic : MonoBehaviour
    {
        public AudioClip music;
        public AudioClip ambience;
        [Range(0f, 1f)] public float musicVolume = 0.55f;
        [Range(0f, 1f)] public float ambienceVolume = 0.35f;
        public float fadeInSeconds = 1.5f;

        AudioSource musicSource;
        AudioSource ambienceSource;
        float fade;          // 0..1 multiplier applied to both volumes
        float fadeTarget = 1f;
        float fadeRate;

        void Awake()
        {
            musicSource = CreateSource(music);
            ambienceSource = CreateSource(ambience);
        }

        AudioSource CreateSource(AudioClip clip)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.playOnAwake = false;
            source.volume = 0f;
            return source;
        }

        void Start()
        {
            if (music != null) musicSource.Play();
            if (ambience != null) ambienceSource.Play();
            fade = 0f;
            fadeTarget = 1f;
            fadeRate = fadeInSeconds > 0f ? 1f / fadeInSeconds : float.MaxValue;
        }

        void Update()
        {
            fade = Mathf.MoveTowards(fade, fadeTarget, fadeRate * Time.unscaledDeltaTime);
            musicSource.volume = musicVolume * fade;
            ambienceSource.volume = ambienceVolume * fade;
        }

        /// <summary>Fades both loops to silence over the given time. Wait on the returned routine to know when it is done.</summary>
        public IEnumerator FadeOut(float seconds)
        {
            fadeTarget = 0f;
            fadeRate = seconds > 0f ? 1f / seconds : float.MaxValue;
            while (fade > 0f) yield return null;
            musicSource.Stop();
            ambienceSource.Stop();
        }
    }
}
