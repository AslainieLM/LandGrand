using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Vector3

namespace LandGrand.StartScreen
{
    /// <summary>
    /// Fills the area below the ground with a solid colour, so taller screens
    /// (for example 4:3 tablets) never show an empty strip under the art.
    /// </summary>
    public class ScreenFill : MonoBehaviour
    {
        public Color color = new Color32(122, 82, 51, 255); // Fill colour (ground shade)
        public float topY = -4.2f; // Top edge of the fill in world units
        public int sortingOrder = -1000; // Draw it behind everything

        void Start() // Runs once before the first frame
        {
            var tex = new Texture2D(1, 1) { filterMode = FilterMode.Point }; // Make a 1x1 picture with no blur
            tex.SetPixel(0, 0, Color.white); // Make that pixel white
            tex.Apply(); // Send the picture to the graphics card
            var sr = gameObject.AddComponent<SpriteRenderer>(); // Draws the fill
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 1f), 1f); // Turn the picture into a sprite, pinned at its top edge
            sr.color = color; // Tint it the fill colour
            sr.sortingOrder = sortingOrder; // Draw order
            transform.position = new Vector3(0f, topY, 0f); // Put its top edge at topY
            transform.localScale = new Vector3(200f, 50f, 1f); // Stretch it wide and tall enough for any screen
        }
    }
}
