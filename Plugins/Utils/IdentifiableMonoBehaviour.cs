using UnityEngine;

namespace TravelMind.Plugins.Utils
{
    public abstract class IdentifiableMonoBehaviour : MonoBehaviour
    {
        public string Id => id;

        [SerializeField] private string id;

        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(Id))
                Debug.LogWarning("Id is empty", this);
#endif
        }
    }
}