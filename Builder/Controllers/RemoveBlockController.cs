using System.Collections.Generic;
using System.Linq;
using TravelMind.Blocks;
using TravelMind.Blocks.Core;
using TravelMind.Builder.Abstracts;
using TravelMind.Components.Core;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Builder.Controllers
{
    public class RemoveBlockController : ABuilderController
    {
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
            
            var occupiedBlocks = blocksGridManager.GetByAvailability(Ship.Availability.Occupied);

            foreach (var occupiedBlock in occupiedBlocks)
            {
                //The center cannot be removed
                var block = blocksGridManager.GetAtCoordinates(occupiedBlock);
                var component = componentsGridManager.GetAtCoordinates(occupiedBlock);
                
                if(!block.IsDestructible || component != null)
                    continue;
                
                var aroundBlocks = blocksGridManager.GetNeighborBlocks(occupiedBlock);
                
                var simulatedOccupied = occupiedBlocks.ToList();
                simulatedOccupied.Remove(occupiedBlock);

                // Check if the block can be destroyed
                var canDestroy = aroundBlocks.All(x => Blocks.Core.Shared.HasPath(x, blocksGridManager.CenterCoordinates, simulatedOccupied));
                
                if (!canDestroy)
                    continue;
                
                //Spawn preview block
                var builderBlock = Pools.Spawn<BuilderBlock>(availableObjectPrefab.gameObject, Pools.PoolType.Tools,
                    blocksGridManager.GetBlockPosition(occupiedBlock), spawnParent.rotation, spawnParent);

                spawnedPrefabList.Add(builderBlock);

                //If click on the block, then spawn block
                builderBlock.Initialize(occupiedBlock);
            }
        }
        
        public void RemoveBlock(BuilderBlock block)
        {
            if (block == null || !spawnedPrefabList.Contains(block))
                return;
            
            blocksGridManager.Remove(block.Coordinates);
        }
    }
}
