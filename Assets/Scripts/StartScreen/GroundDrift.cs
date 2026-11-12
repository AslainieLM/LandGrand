using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>Moves an effect left with the ground, so it stays where it was kicked up while the world scrolls.</summary>
    public class GroundDrift : MonoBehaviour
    {
        public ParallaxLayer ground;

        void Update()
        {
            if (ground == null) return;
            float speed = ground.speedPixelsPerSecond / ground.pixelsPerUnit * ParallaxLayer.WalkSpeedScale;
            transform.position += Vector3.left * (speed * Time.deltaTime);
        }
    }
}
