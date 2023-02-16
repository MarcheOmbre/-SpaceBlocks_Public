using UnityEngine;

namespace TravelMind.Shared
{
    public static class Utility2D
    {
        public static bool IsInCameraView(Camera camera, Renderer renderer)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }
    }
}
