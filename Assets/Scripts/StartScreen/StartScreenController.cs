using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace LandGrand.StartScreen
{
    /// <summary>Waits for a tap anywhere, fades the music out, then opens the Main Menu scene.</summary>
    public class StartScreenController : MonoBehaviour
    {
        public string nextScene = "MainMenu";
        [Tooltip("Ignore taps for this long after the screen opens.")]
        public float ignoreTapsSeconds = 0.5f;
        public StartScreenMusic music;
        public float musicFadeOutSeconds = 0.6f;

        bool leaving;

        void Update()
        {
            if (leaving || Time.timeSinceLevelLoad < ignoreTapsSeconds) return;
            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;

            if (Application.CanStreamedLevelBeLoaded(nextScene))
            {
                leaving = true;
                StartCoroutine(Leave());
            }
            else
            {
                Debug.Log($"Tap received. Add a scene named '{nextScene}' to the build to continue.");
            }
        }

        IEnumerator Leave()
        {
            if (music != null) yield return music.FadeOut(musicFadeOutSeconds);
            SceneManager.LoadScene(nextScene);
        }
    }
}
