using UnityEngine; // Gives access to MonoBehaviour, Sprite and SpriteRenderer

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// Light snow drifting down near the summit of Folded Peaks (D-055, decoration only). The flakes fall inside a stretch of the level
    /// and start again at the top when they reach the ground. They have no effect on play.
    /// </summary>
    public class SummitSnow : MonoBehaviour // Makes and moves the flakes
    {
        public Sprite[] flakeSprites; // Small and big flake pictures
        public float fromX = 4000f; // Left end of the snowy stretch, in level pixels
        public float toX = 5400f; // Right end
        public float topY = 210f; // Where flakes start, above the screen's play area
        public int count = 40; // How many flakes
        public int sortingOrder = 18; // Behind the explorer (20) and in front of the level, so a flake never hides the explorer

        public bool useLevelClock; // Hard snow waits when the game pauses
        float previousTime; // Last time used for the movement step
        Transform[] flakes; // The flake objects
        Vector2[] places; // Their places in level pixels
        float[] speeds; // How fast each one falls
        float[] sway; // Each flake's own sway timing

        void Start() // Makes the flakes at random places
        {
            if (flakeSprites == null || flakeSprites.Length == 0) { enabled = false; return; } // No flake pictures: no snow
            previousTime = LevelClock.Now; // Starts the paused snow at the current level time
            var rng = new System.Random(7); // Fixed seed: the same snow every time
            flakes = new Transform[count]; places = new Vector2[count]; speeds = new float[count]; sway = new float[count]; // Room for each flake
            for (int i = 0; i < count; i++) // Each flake
            {
                var go = new GameObject("Flake"); // A new flake object
                go.transform.SetParent(transform, false); // Keep it under this object
                var sr = go.AddComponent<SpriteRenderer>(); // Draws it
                sr.sprite = flakeSprites.Length > 1 && i % 4 == 0 ? flakeSprites[1] : flakeSprites[0]; // One in four is the big flake
                sr.sortingOrder = sortingOrder; // In front
                flakes[i] = go.transform; // Remember it
                places[i] = new Vector2(fromX + (float)rng.NextDouble() * (toX - fromX), (float)rng.NextDouble() * topY); // Somewhere in the stretch
                speeds[i] = 14f + (float)rng.NextDouble() * 14f; // Slow, gentle falling
                sway[i] = (float)rng.NextDouble() * 6.28f; // Different sway for each
            }
        }

        void Update() // Moves every flake a little
        {
            float t = useLevelClock ? LevelClock.Now : Time.time; // Uses the selected clock for sway
            float dt = useLevelClock ? t - previousTime : Time.deltaTime; // Steps only when that clock moves
            previousTime = t; // Remembers this clock time
            for (int i = 0; i < flakes.Length; i++) // Each flake
            {
                places[i].y -= speeds[i] * dt; // Fall
                places[i].x -= 6f * dt; // The wind blows a little to the left
                if (places[i].y < 0f) places[i].y = topY; // Back to the top at the ground
                if (places[i].x < fromX) places[i].x = toX; // Back to the right end
                float x = places[i].x + Mathf.Sin(t * 1.3f + sway[i]) * 3f; // A small side-to-side sway
                flakes[i].position = LevelGeometry.ToWorld(Mathf.Round(x), Mathf.Round(places[i].y)); // Whole pixels keep the art sharp
            }
        }
    }
}
