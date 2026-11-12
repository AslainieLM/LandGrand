using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>Moves an object up and down by whole pixels, for the floating title.</summary>
    public class PixelBob : MonoBehaviour
    {
        public float periodSeconds = 2.4f;
        public int amplitudePixels = 1;
        public float pixelsPerUnit = 32f;

        Vector3 basePosition;

        void Start() => basePosition = transform.localPosition;

        void Update()
        {
            float wave = Mathf.Sin(Time.time * Mathf.PI * 2f / periodSeconds);
            float pixels = Mathf.Round(wave * amplitudePixels);
            transform.localPosition = basePosition + new Vector3(0f, pixels / pixelsPerUnit, 0f);
        }
    }
}
