using System;
using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>Plays a list of sprites as a frame animation. A sequence entry of -1 hides the sprite for that frame.</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteFlipbook : MonoBehaviour
    {
        public Sprite[] frames = Array.Empty<Sprite>();
        [Tooltip("Frame indices to play. Leave empty to play every frame in order. -1 hides the sprite.")]
        public int[] sequence = Array.Empty<int>();
        public float framesPerSecond = 10f;
        public bool loop = true;
        public bool destroyWhenDone;

        /// <summary>Raised with the sequence step each time the shown frame changes.</summary>
        public event Action<int> FrameChanged;
        /// <summary>Raised once when a non-looping sequence reaches its end.</summary>
        public event Action Finished;

        /// <summary>The frame index currently shown, or -1 when hidden.</summary>
        public int CurrentFrame { get; private set; } = -1;

        SpriteRenderer spriteRenderer;
        float elapsed;
        int lastStep = -1;
        bool finished;

        int Length => sequence.Length > 0 ? sequence.Length : frames.Length;

        void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();

        void OnEnable() => Restart();

        /// <summary>Switches to another sequence and plays it from the start.</summary>
        public void Play(int[] newSequence, bool shouldLoop)
        {
            sequence = newSequence ?? Array.Empty<int>();
            loop = shouldLoop;
            Restart();
            Update();
        }

        void Restart()
        {
            elapsed = 0f;
            lastStep = -1;
            finished = false;
        }

        void Update()
        {
            int length = Length;
            if (length == 0 || framesPerSecond <= 0f || finished) return;

            elapsed += Time.deltaTime;
            int step = Mathf.FloorToInt(elapsed * framesPerSecond);
            if (step >= length)
            {
                if (!loop)
                {
                    finished = true;
                    if (destroyWhenDone) { Destroy(gameObject); return; }
                    Finished?.Invoke();
                    return;
                }
                elapsed %= length / framesPerSecond;
                step %= length;
            }
            if (step == lastStep) return;
            lastStep = step;

            int frame = sequence.Length > 0 ? sequence[step] : step;
            CurrentFrame = frame;
            spriteRenderer.enabled = frame >= 0;
            if (frame >= 0 && frame < frames.Length) spriteRenderer.sprite = frames[frame];
            FrameChanged?.Invoke(step);
        }
    }
}
