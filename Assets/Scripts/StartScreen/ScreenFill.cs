using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>
    /// Fills the area below the ground with a solid colour, so taller screens
    /// (for example 4:3 tablets) never show an empty strip under the art.
    /// </summary>
    public class ScreenFill : MonoBehaviour
    {
        public Color color = new Color32(122, 82, 51, 255);
        public float topY = -4.2f;
        public int sortingOrder = -1000;

        void Start()
        {
            var tex = new Texture2D(1, 1) { filterMode = FilterMode.Point };
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            var sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 1f), 1f);
            sr.color = color;
            sr.sortingOrder = sortingOrder;
            transform.position = new Vector3(0f, topY, 0f);
            transform.localScale = new Vector3(200f, 50f, 1f);
        }
    }
}
