using UnityEngine; // Gives access to MonoBehaviour, SpriteRenderer and Random

namespace LandGrand.LandformMap // Landform Map scripts live here
{
    /// <summary>Blowing snow over the Mountains region: small flakes drift down and to the right and wrap inside an area.</summary>
    public class SnowFlurry : MonoBehaviour // Makes and moves the snowflakes
    {
        public Sprite[] flakeSprites; // Small and big snowflake pictures
        public int count = 36; // How many flakes are in the air
        public Rect area = new Rect(8f, 24f, 220f, 84f); // Where the flakes fly, in map pixels (x, y from the top-left, width, height)
        public Vector2 windPixelsPerSecond = new Vector2(16f, 9f); // Wind push: right and down
        public float swayPixels = 2f; // How far each flake sways from side to side
        public int sortingOrder = 20; // Draw order; in front of the map

        Transform[] flakes; // The flake objects
        Vector2[] positions; // Each flake's place in map pixels
        float[] speeds; // Each flake's own speed, so they do not move together
        float[] phases; // Each flake's own sway timing

        void Awake() // Runs once: makes the flakes at random places
        {
            flakes = new Transform[count]; // Room for every flake
            positions = new Vector2[count]; // Room for every place
            speeds = new float[count]; // Room for every speed
            phases = new float[count]; // Room for every sway timing
            for (int i = 0; i < count; i++) // Make each flake
            {
                var go = new GameObject("Flake"); // New object for the flake
                go.transform.SetParent(transform, false); // Keep it under this object
                var sr = go.AddComponent<SpriteRenderer>(); // Draws the flake
                sr.sprite = flakeSprites.Length > 0 ? flakeSprites[i % 4 == 0 && flakeSprites.Length > 1 ? 1 : 0] : null; // One in four flakes is the big one
                sr.sortingOrder = sortingOrder; // In front of the map
                flakes[i] = go.transform; // Remember the flake
                positions[i] = new Vector2(Random.Range(area.xMin, area.xMax), Random.Range(area.yMin, area.yMax)); // Random start place in the area
                speeds[i] = Random.Range(0.6f, 1.4f); // Some flakes are faster than others
                phases[i] = Random.Range(0f, Mathf.PI * 2f); // Random sway timing, anywhere in one full sway
            }
        }

        void Update() // Runs every frame: moves each flake
        {
            for (int i = 0; i < flakes.Length; i++) // Each flake
            {
                var p = positions[i] + windPixelsPerSecond * (speeds[i] * Time.deltaTime); // Push it with the wind
                if (p.x > area.xMax) p.x -= area.width; // Past the right edge: come back on the left
                if (p.y > area.yMax) p.y -= area.height; // Past the bottom edge: come back at the top
                positions[i] = p; // Keep the new place
                float sway = Mathf.Sin(Time.time * 1.7f + phases[i]) * swayPixels; // Small side-to-side sway
                flakes[i].localPosition = MapPixels.ToWorld(p.x + sway, p.y); // Show it on a whole pixel
            }
        }
    }
}
