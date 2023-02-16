using UnityEngine;

namespace TravelMind.Plugins.Utils
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (!_instance)
                    _instance = FindObjectOfType<T>();
                
                return _instance;
            }
        }
        
        private static T _instance;
    }
}