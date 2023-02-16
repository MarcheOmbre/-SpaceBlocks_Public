using System.Collections.Generic;
using TravelMind.Components.Core.Abstracts;
using TravelMind.Plugins.Utils;
using UnityEngine;

namespace TravelMind.Components.Core
{
    //Load resources before any other script
    [DefaultExecutionOrder(-5)]
    public class ComponentResourcesManager : MonoBehaviourSingleton<ComponentResourcesManager>
    {
        public IReadOnlyDictionary<string, AComponent> ComponentResources => componentResources;
        
        private readonly Dictionary<string, AComponent> componentResources = new();

        private void Awake()
        {
            foreach (var resource in Resources.LoadAll<AComponent>("Components/"))
            {
                if (componentResources.ContainsKey(resource.Id))
                {
                    Debug.LogWarning("Block with id " + resource.Id + " already exists");
                    continue;
                }
                
                componentResources.Add(resource.Id, resource);
            }
        }
    }
}
