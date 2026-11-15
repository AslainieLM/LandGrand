using LandGrand.Rules; // Gives access to Episode
using UnityEngine; // Gives access to MonoBehaviour, Sprite and SpriteRenderer

namespace LandGrand // Game code lives here
{
    /// <summary>
    /// Shows the picture of the landform the player picked on the map behind the difficulty, level and lesson screens (D-063).
    /// The snow only falls over the Mountains picture.
    /// </summary>
    public class LandformBackdrop : MonoBehaviour // Swaps the full-screen background for the chosen landform
    {
        public SpriteRenderer picture; // The full-screen background picture
        public Sprite[] pictures; // One picture per landform, in Episode order: Mountains, Hills, Plains, Plateaus
        public GameObject snow; // Blowing snow, shown only for Mountains

        void Awake() // Runs when the screen opens, before it is drawn
        {
            int i = (int)GameSession.Episode; // The landform picked on the map
            if (pictures != null && i >= 0 && i < pictures.Length && pictures[i] != null) picture.sprite = pictures[i]; // Its picture; keep Mountains if one is missing
            if (snow != null) snow.SetActive(GameSession.Episode == Episode.Mountains); // Snow only on the mountains
        }
    }
}
