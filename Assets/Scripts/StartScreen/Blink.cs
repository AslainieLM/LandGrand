using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>Turns a sprite on and off, for the "Tap to Start" prompt.</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Blink : MonoBehaviour
    {
        public float onSeconds = 0.6f;
        public float offSeconds = 0.4f;

        SpriteRenderer spriteRenderer;

        void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();

        void Update()
        {
            float t = Mathf.Repeat(Time.time, onSeconds + offSeconds);
            spriteRenderer.enabled = t < onSeconds;
        }
    }
}
