using System.Collections.Generic;
using TravelMind.Blocks;
using TravelMind.Blocks.Core;
using TravelMind.Builder.Abstracts;
using TravelMind.Components.Core;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Builder.Controllers
{
    public class ComponentsBuilderController : ABuilderController
    {
        public BlocksGridManager BlocksGridManager => blocksGridManager;
        
        public ComponentsGridManager ComponentsGridManager => componentsGridManager;

        protected override IReadOnlyList<BuilderBlock> SpawnedPrefabList => spawnedPrefabList;
        
        
        [SerializeField] private BlocksGridManager blocksGridManager;
        [SerializeField] private ComponentsGridManager componentsGridManager;
        [SerializeField] private BuilderBlock availableObjectPrefab;
        [SerializeField] private Transform spawnParent;

        private readonly List<BuilderBlock> spawnedPrefabList = new();


        protected override void OnEnable()
        {
            base.OnEnable();
            
            RefreshBuildMode(true);
            blocksGridManager.OnChanged += OnBlockGridManagerChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            blocksGridManager.OnChanged -= OnBlockGridManagerChanged;
            RefreshBuildMode(false);
        }

        private void OnBlockGridManagerChanged() => RefreshBuildMode(true);

        private void RefreshBuildMode(bool value)
        {
            foreach (var prefab in spawnedPrefabList) 
                Pools.Despawn(prefab.gameObject, Pools.PoolType.Tools);

            spawnedPrefabList.Clear();

            if (!value)
                return;
            
            var availableBlocks = blocksGridManager.GetByAvailability(Ship.Availability.Occupied);

            foreach (var coordinates in availableBlocks)
            {
                //Spawn preview block
                var builderBlock = Pools.Spawn<BuilderBlock>(availableObjectPrefab.gameObject, Pools.PoolType.Tools,
                    blocksGridManager.GetBlockPosition(coordinates), spawnParent.rotation, spawnParent);

                spawnedPrefabList.Add(builderBlock);

                //If click on the block, then spawn block
                builderBlock.Initialize(coordinates);
            }
        }

        public void AddComponent(BuilderBlock block, string id)
        {
            //Check block
            if (block == null || !spawnedPrefabList.Contains(block))
                return;
            
            //Check id
            if (!ComponentResourcesManager.Instance.ComponentResources.TryGetValue(id, out var componentPrefab))
                return;

            componentsGridManager.Add(block.Coordinates, componentPrefab);
        }
        
        public void RemoveComponent(BuilderBlock block)
        {
            if (block == null || !spawnedPrefabList.Contains(block))
                return;
            
            componentsGridManager.Remove(block.Coordinates);
        }
    }
}
