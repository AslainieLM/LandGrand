using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Vector3

namespace LandGrand.StartScreen
{
    /// <summary>Keeps a spawned effect locked to a point on a scrolling layer.</summary>
    public class FollowLayer : MonoBehaviour
    {
        ParallaxLayer layer; // The layer to stay on
        float canvasX; // The spot on that layer, in layer pixels

        public void Init(ParallaxLayer target, float x) { layer = target; canvasX = x; } // Set the layer and the spot

        void LateUpdate() // Runs every frame, after the layers have moved
        {
            if (layer == null) return; // Do nothing until a layer is set
            var p = transform.position; // Current position
            transform.position = new Vector3(layer.CanvasToWorldX(canvasX), p.y, p.z); // Move across to the spot; keep the height
        }
    }
}
