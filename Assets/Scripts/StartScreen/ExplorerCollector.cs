using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>
    /// Switches the explorer between walking and picking up a mineral:
    /// stop, crouch, grab it from the ground, hold it up, put it in the backpack, walk on.
    /// The world stops scrolling while the explorer is collecting.
    /// </summary>
    [RequireComponent(typeof(SpriteFlipbook))]
    public class ExplorerCollector : MonoBehaviour
    {
        public int[] walkSequence = { 0, 1, 2, 3, 4, 5, 6, 7 };
        [Tooltip("Collect frames at 10 fps. Repeated entries hold a pose longer.")]
        public int[] collectSequence = { 8, 9, 10, 11, 11, 12, 13, 13, 13, 14, 15 };
        public int grabFrame = 11;
        public int showFrame = 13;
        public int stashFrame = 14;
        [Tooltip("Glove position for each frame index, in pixels from the sprite centre (x right, y up).")]
        public Vector2[] handOffsetsPixels = new Vector2[16];
        public float pixelsPerUnit = 32f;
        [Tooltip("Seconds for the world to start scrolling again after collecting.")]
        public float resumeSeconds = 0.3f;

        public bool IsCollecting { get; private set; }

        SpriteFlipbook flipbook;
        ExplorerFootsteps footsteps;
        MineralPickup carried;
        bool grabbed;
        bool shown;

        void Awake()
        {
            flipbook = GetComponent<SpriteFlipbook>();
            footsteps = GetComponent<ExplorerFootsteps>();
        }

        void OnEnable()
        {
            flipbook.FrameChanged += OnStep;
            flipbook.Finished += OnCollectFinished;
        }

        void OnDisable()
        {
            flipbook.FrameChanged -= OnStep;
            flipbook.Finished -= OnCollectFinished;
        }

        void Start()
        {
            ParallaxLayer.WalkSpeedScale = 1f;
            flipbook.Play(walkSequence, true);
        }

        void Update()
        {
            if (!IsCollecting && ParallaxLayer.WalkSpeedScale < 1f)
            {
                float rate = resumeSeconds > 0f ? Time.deltaTime / resumeSeconds : 1f;
                ParallaxLayer.WalkSpeedScale = Mathf.MoveTowards(ParallaxLayer.WalkSpeedScale, 1f, rate);
            }
        }

        /// <summary>Starts the pick-up animation for this mineral. Returns false if already busy.</summary>
        public bool TryCollect(MineralPickup mineral)
        {
            if (IsCollecting || mineral == null) return false;
            IsCollecting = true;
            carried = mineral;
            grabbed = false;
            shown = false;
            ParallaxLayer.WalkSpeedScale = 0f;
            if (footsteps != null) footsteps.enabled = false;
            flipbook.Play(collectSequence, false);
            return true;
        }

        /// <summary>World position of the glove in the current frame.</summary>
        public Vector3 HandPosition
        {
            get
            {
                int f = flipbook.CurrentFrame;
                Vector2 px = f >= 0 && f < handOffsetsPixels.Length ? handOffsetsPixels[f] : Vector2.zero;
                return transform.position + new Vector3(px.x, px.y, 0f) / pixelsPerUnit;
            }
        }

        void OnStep(int step)
        {
            if (!IsCollecting || carried == null) return;
            int f = flipbook.CurrentFrame;
            if (f >= grabFrame && f < stashFrame)
            {
                if (!grabbed) { grabbed = true; carried.OnGrabbed(); }
                carried.HoldAt(HandPosition);
                if (f >= showFrame && !shown) { shown = true; carried.OnShown(); }
            }
            else if (f >= stashFrame && grabbed)
            {
                carried.OnStashed();
                carried = null;
            }
        }

        void OnCollectFinished()
        {
            IsCollecting = false;
            if (carried != null) { carried.OnStashed(); carried = null; }
            if (footsteps != null) footsteps.enabled = true;
            flipbook.Play(walkSequence, true);
        }
    }
}
