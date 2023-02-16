using System.Collections.Generic;
using TravelMind.Blocks;
using TravelMind.Blocks.Core;
using TravelMind.Builder.Abstracts;
using TravelMind.Shared;
using UnityEngine;
using UnityEngine.Serialization;

namespace TravelMind.Builder.Controllers
{
    public class AddBlockController : ABuilderController
    {
        protected override IReadOnlyList<BuilderBlock> SpawnedPrefabList => spawnedPrefabList;
        
        [FormerlySerializedAs("blocksBuilderManager")] [SerializeField] private BlocksGridManager blocksGridManager;
        [SerializeField] private BuilderBlock availableObjectPrefab;
        [SerializeField] private ShipBlock newBlockPrefab;
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
            
            foreach (var availableBlock in blocksGridManager.GetByAvailability(Ship.Availability.Empty))
            {
                if (blocksGridManager.GetNeighborBlocks(availableBlock).Count <= 0)
                    continue;

                //Spawn preview block
                var builderBlock = Pools.Spawn<BuilderBlock>(availableObjectPrefab.gameObject, Pools.PoolType.Tools,
                    blocksGridManager.GetBlockPosition(availableBlock), spawnParent.rotation, spawnParent);

                spawnedPrefabList.Add(builderBlock);

                //If click on the block, then spawn block
                builderBlock.Initialize(availableBlock);
            }
        }

        public void AddBlock(BuilderBlock blockPrefab)
        {
            if (blockPrefab == null || !spawnedPrefabList.Contains(blockPrefab))
                return;

            blocksGridManager.Add(blockPrefab.Coordinates, newBlockPrefab);
        }
    }
}
