using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>Moves an object left at a fixed speed and removes it once it leaves the screen.</summary>
    public class DriftLeft : MonoBehaviour
    {
        public float speed = 1f;
        public float destroyBelowX = -20f;

        void Update()
        {
            transform.position += Vector3.left * (speed * Time.deltaTime);
            if (transform.position.x < destroyBelowX) Destroy(gameObject);
        }
    }
}
