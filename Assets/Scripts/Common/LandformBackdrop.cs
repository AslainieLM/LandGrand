using LandGrand.Rules; // Gives access to Episode
using UnityEngine; // Gives access to MonoBehaviour, Sprite and SpriteRenderer

namespace LandGrand // Game code lives here
{
    /// <summary>
    /// Shows the picture of the landform the player picked on the map behind the difficulty, level and lesson screens (D-063).
    /// Each landform has its own set of life (snow and eagle, birds and grass, and so on); only the picked landform's set is shown.
    /// </summary>
    public class LandformBackdrop : MonoBehaviour // Swaps the full-screen background for the chosen landform
    {
        public SpriteRenderer picture; // The full-screen background picture
        public Sprite[] pictures; // One picture per landform, in Episode order: Mountains, Hills, Plains, Plateaus
        public GameObject[] lifeSets; // One set of animations per landform, in the same Episode order

        void Awake() // Runs when the screen opens, before it is drawn
        {
            int i = (int)GameSession.Episode; // The landform picked on the map
            if (pictures != null && i >= 0 && i < pictures.Length && pictures[i] != null) picture.sprite = pictures[i]; // Its picture; keep Mountains if one is missing
            if (lifeSets == null) return; // No life sets to switch
            for (int k = 0; k < lifeSets.Length; k++) // Each landform's set
            {
                if (lifeSets[k] != null) lifeSets[k].SetActive(k == i); // Show only the picked landform's set
            }
        }
    }
}
