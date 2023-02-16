using System;
using System.Collections.Generic;
using System.Linq;
using TravelMind.Plugins.Utils;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Blocks.Core
{
    public class BlocksGridManager : MonoBehaviour
    {
        public event Action OnChanged = delegate { };
        
        public event Action<Vector2Int> OnBlockAdded = delegate { };
        
        public event Action<Vector2Int> OnBlockRemoved = delegate { };

        public Transform SpawnParent => spawnParent;

        public Vector2Int CenterCoordinates { get; private set; }

        
        [SerializeField] private Transform spawnParent;

        private ShipBlock[,] grid;


        private void OnDisable()
        {
            //Check grid and components pools for null to avoid errors on scene change
            if (grid == null) 
                return;
            
            for(var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] != null)
                        Remove(new Vector2Int(i, j));
                }
            }
            
            grid = null;
        }

        private bool CheckGridCoordinates(Vector2Int coordinates)
        {
            if (grid == null)
                return false;
            
            return coordinates.x >= 0 && coordinates.x < grid.GetLength(0) 
                                      && coordinates.y >= 0 && coordinates.y < grid.GetLength(1);
        }
        
        private void OnBlockDepleted(ShipBlock shipBlock)
        {
            if (shipBlock == null)
                return;
            
            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    if (shipBlock != grid[i, j])
                        continue;

                    Remove(new Vector2Int(i, j));
                    
                    //Check for unattached blocks
                    var occupiedBlocks = GetByAvailability(Ship.Availability.Occupied).ToList();

                    for(var h = occupiedBlocks.Count - 1; h >= 0; h--)
                    {
                        var aroundBlock = GetAtCoordinates(occupiedBlocks[h]);
                        if (aroundBlock == null || Shared.HasPath(occupiedBlocks[h], CenterCoordinates, occupiedBlocks))
                            continue;
                
                        //Kill block around
                        aroundBlock.Kill();

                        //If one is deleted, then it'll continue to check for unattached blocks
                        break;
                    }
                    
                    return;
                }
            }
        }

        public void Initialize(string[,] blocksData)
        {
            if (blocksData == null)
                throw new ArgumentException("Grid example cannot be null");

            var columnsCount = blocksData.GetLength(0);
            var rowsCount = blocksData.GetLength(1);

            if (columnsCount % 2 == 0 || rowsCount % 2 == 0)
                throw new ArgumentException("Grid example must have odd number of columns and rows");

            grid = new ShipBlock[columnsCount, rowsCount];

            for (var i = 0; i < columnsCount; i++)
            {
                for (var j = 0; j < rowsCount; j++)
                {
                    var blockId = blocksData[i, j];
                    if (!string.IsNullOrEmpty(blockId) && BlockResourcesManager.Instance.BlockResources.TryGetValue(blockId, out var blockResource))
                        Add(new Vector2Int(i, j), blockResource);
                }
            }
            
            CenterCoordinates = new Vector2Int(columnsCount / 2, rowsCount / 2);
            
            OnChanged();
        }

        public ShipBlock GetAtCoordinates(Vector2Int coordinates) => !CheckGridCoordinates(coordinates) ? null : grid[coordinates.x, coordinates.y];

        public List<Vector2Int> GetByAvailability(Ship.Availability availability)
        {
            var list = new List<Vector2Int>();

            if (grid == null)
                return list;

            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == null && availability is Ship.Availability.Empty or Ship.Availability.Both
                        || grid[i, j] != null && availability is Ship.Availability.Occupied or Ship.Availability.Both)
                        list.Add(new Vector2Int(i, j));
                }
            }

            return list;
        }
        
        public Vector2 GetBlockPosition(Vector2Int coordinates)
        {
            if (!CheckGridCoordinates(coordinates))
                return default;

            var gridSize = new Vector2Int(grid.GetLength(0), grid.GetLength(1));
            return spawnParent.TransformPoint(Ship.IndexToLocalPosition(coordinates, gridSize));
        }

        public List<Vector2Int> GetNeighborBlocks(Vector2Int coordinates)
        {
            var aroundBlocks = new List<Vector2Int>();

            if (!CheckGridCoordinates(coordinates))
                return aroundBlocks;

            var columnsCount = grid.GetLength(0);
            var rowsCount = grid.GetLength(1);

            if (coordinates.x < 0 || coordinates.x >= columnsCount || coordinates.y < 0 || coordinates.y >= rowsCount)
                throw new ArgumentException("X and Y must be in range of blocks array");

            //Check left
            if (coordinates.x > 0 && grid[coordinates.x - 1, coordinates.y] != null)
                aroundBlocks.Add(new Vector2Int(coordinates.x - 1, coordinates.y));

            //Check right
            if (coordinates.x < columnsCount - 1 && grid[coordinates.x + 1, coordinates.y] != null)
                aroundBlocks.Add(new Vector2Int(coordinates.x + 1, coordinates.y));

            //Check up
            if (coordinates.y < rowsCount - 1 && grid[coordinates.x, coordinates.y + 1] != null)
                aroundBlocks.Add(new Vector2Int(coordinates.x, coordinates.y + 1));

            //Check down
            if (coordinates.y > 0 && grid[coordinates.x, coordinates.y - 1] != null)
                aroundBlocks.Add(new Vector2Int(coordinates.x, coordinates.y - 1));

            return aroundBlocks;
        }

        public bool Add(Vector2Int coordinates, ShipBlock blockPrefab)
        {
            if (!blockPrefab)
                throw new ArgumentException("Block Prefab is null");
            
            if (GetAtCoordinates(coordinates) != null)
                return false;
            
            var block = Pools.Spawn<ShipBlock>(blockPrefab.gameObject, Pools.PoolType.ShipBlocks, 
                GetBlockPosition(coordinates), spawnParent.rotation, spawnParent);
            block.gameObject.SetLayerRecursively(gameObject.layer);
            block.OnDepleted += OnBlockDepleted;
            
            grid[coordinates.x, coordinates.y] = block;

            OnBlockAdded(coordinates);
            OnChanged();

            return true;
        }

        public bool Remove(Vector2Int coordinates)
        {
            var block = GetAtCoordinates(coordinates);
            if (block == null)
                return false;
            
            block.OnDepleted -= OnBlockDepleted;
            Pools.Despawn(block.gameObject, Pools.PoolType.ShipBlocks);

            grid[coordinates.x, coordinates.y] = null;
            
            OnBlockRemoved(coordinates);
            OnChanged();

            return true;
        }
    }
}