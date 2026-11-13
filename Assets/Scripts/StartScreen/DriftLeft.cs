using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Vector3

namespace LandGrand.StartScreen
{
    /// <summary>Moves an object left at a fixed speed and removes it once it leaves the screen.</summary>
    public class DriftLeft : MonoBehaviour
    {
        public float speed = 1f; // How fast it moves left, in world units per second
        public float destroyBelowX = -20f; // Remove the object once it is left of this x

        void Update() // Runs every frame
        {
            transform.position += Vector3.left * (speed * Time.deltaTime); // Move left a little this frame
            if (transform.position.x < destroyBelowX) Destroy(gameObject); // Remove it once it is off the screen
        }
    }
}
