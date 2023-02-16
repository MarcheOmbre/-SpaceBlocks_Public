using UnityEngine;

namespace TravelMind.Plugins.Utils
{
    public static class LayersUtil
    {
        public static void SetLayerRecursively(this GameObject obj, int newLayer)
        {
            if (null == obj)
                return;

            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (child == null)
                    continue;

                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }
}
