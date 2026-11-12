using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>Keeps a spawned effect locked to a point on a scrolling layer.</summary>
    public class FollowLayer : MonoBehaviour
    {
        ParallaxLayer layer;
        float canvasX;

        public void Init(ParallaxLayer target, float x) { layer = target; canvasX = x; }

        void LateUpdate()
        {
            if (layer == null) return;
            var p = transform.position;
            transform.position = new Vector3(layer.CanvasToWorldX(canvasX), p.y, p.z);
        }
    }
}
