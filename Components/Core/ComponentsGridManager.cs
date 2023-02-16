using System;
using System.Collections.Generic;
using TravelMind.Components.Core.Abstracts;
using TravelMind.Plugins.Utils;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Components.Core
{
    public class ComponentsGridManager : MonoBehaviour
    {
        public event Action OnChanged = delegate { };

        [SerializeField] private Transform spawnParent;

        private AComponent[,] grid;

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
        
        public void Initialize(string[,] componentsData)
        {
            if (componentsData == null)
                throw new ArgumentException("Grid example cannot be null");

            var columnsCount = componentsData.GetLength(0);
            var rowsCount = componentsData.GetLength(1);
            
            grid = new AComponent[columnsCount, rowsCount];

            for (var i = 0; i < columnsCount; i++)
            {
                for (var j = 0; j < rowsCount; j++)
                {
                    //Check example grid block
                    var blockId = componentsData[i, j];
                    if(string.IsNullOrEmpty(blockId) || 
                       !ComponentResourcesManager.Instance.ComponentResources.TryGetValue(blockId, out var componentResource))
                        continue;

                    //Check if the current grid block is available
                    Add(new Vector2Int(i, j), componentResource);
                }
            }

            OnChanged();
        }

        public AComponent GetAtCoordinates(Vector2Int coordinates) => !CheckGridCoordinates(coordinates) ? null : grid[coordinates.x, coordinates.y];
        
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

        public Vector2 GetComponentPosition(Vector2Int coordinates)
        {
            if (!CheckGridCoordinates(coordinates))
                return default;

            var gridSize = new Vector2Int(grid.GetLength(0), grid.GetLength(1));
            return spawnParent.TransformPoint(Ship.IndexToLocalPosition(coordinates, gridSize));
        }
        
        public bool Add(Vector2Int coordinates, AComponent componentPrefab)
        {
            if (GetAtCoordinates(coordinates) != null)
                return false;

            var component = Pools.Spawn<AComponent>(componentPrefab.gameObject, Pools.PoolType.ShipComponents,
                GetComponentPosition(coordinates), spawnParent.rotation, spawnParent);

            component.gameObject.SetLayerRecursively(gameObject.layer);
            grid[coordinates.x, coordinates.y] = component;
            
            OnChanged();
            
            return true;
        }
        
        public bool Remove(Vector2Int coordinates)
        {
            var component = GetAtCoordinates(coordinates);
            if (component == null)
                return false;

            Pools.Despawn(component.gameObject, Pools.PoolType.ShipComponents);
            grid[coordinates.x, coordinates.y] = null;
            
            OnChanged();
            
            return true;
        }
    }
}
