using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TravelMind.Plugins.Utils
{
    //Create singleton before a conflict appears
    [DefaultExecutionOrder(-10)]
    public class DontDestroyOnLoad : MonoBehaviour
    {
#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        private static void OnEnterPlayMode()
        {
            CurrentInstances.Clear();
        }
#endif

        private static readonly Dictionary<string, DontDestroyOnLoad> CurrentInstances = new();

        [SerializeField] private string id;

        private void Awake()
        {
            if (CurrentInstances.ContainsKey(id))
            {
                Destroy(gameObject);
                return;
            }

            CurrentInstances.Add(id, this);
        }
    }
}