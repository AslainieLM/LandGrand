using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Vector3

namespace LandGrand.StartScreen
{
    /// <summary>Moves an effect left with the ground, so it stays where it was kicked up while the world scrolls.</summary>
    public class GroundDrift : MonoBehaviour
    {
        public ParallaxLayer ground; // The ground layer to move with

        void Update() // Runs every frame
        {
            if (ground == null) return; // Do nothing without a ground layer
            float speed = ground.speedPixelsPerSecond / ground.pixelsPerUnit * ParallaxLayer.WalkSpeedScale; // Ground speed now, slower or stopped when the explorer stops
            transform.position += Vector3.left * (speed * Time.deltaTime); // Move left with the ground
        }
    }
}
