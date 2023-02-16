using System.Collections.Generic;
using TravelMind.Plugins.Utils;
using UnityEngine;

namespace TravelMind.Blocks.Core
{
    //Load resources before any other script
    [DefaultExecutionOrder(-5)]
    public class BlockResourcesManager : MonoBehaviourSingleton<BlockResourcesManager>
    {
        public IReadOnlyDictionary<string, ShipBlock> BlockResources => blockResources;

        private readonly Dictionary<string, ShipBlock> blockResources = new();
        

        private void Awake()
        {
            foreach (var resource in Resources.LoadAll<ShipBlock>("Blocks/"))
            {
                if (blockResources.ContainsKey(resource.Id))
                {
                    Debug.LogWarning("Block with id " + resource.Id + " already exists");
                    continue;
                }
                
                blockResources.Add(resource.Id, resource);
            }
        }
    }
}
