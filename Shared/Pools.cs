using System;
using HellTap.PoolKit;
using UnityEngine;

namespace TravelMind.Shared
{
    public static class Pools
    {
        public enum PoolType
        {
            Tools,
            ShipBlocks,
            ShipComponents,
            EntitiesSpaceDebris,
            EntitiesProjectiles,
            EntitiesShips,
            Currency
        }

        public static GameObject Spawn(GameObject gameObject, PoolType poolType, Vector2 position = default, 
            Quaternion rotation = default, Transform parent = null)
        {
            var pool = GetPool(poolType);
            return pool.SpawnGO(gameObject, (Vector3)position, rotation, parent);
        }
        
        public static T Spawn<T>(GameObject gameObject, PoolType poolType, Vector2 position = default, 
            Quaternion rotation = default, Transform parent = null) where T : MonoBehaviour
        {
            //Position
            var position3D = (Vector3)position;
            if (poolType == PoolType.ShipComponents)
                position3D.z = -1;
            
            if(!Spawn(gameObject, poolType, position, rotation, parent).TryGetComponent<T>(out var component))
                throw new Exception($"Spawned object {gameObject.name} does not have component {typeof(T).Name}");

            return component;
        }
        
        public static void Despawn(GameObject gameObject, PoolType poolType)
        {
            var pool = GetPool(poolType);
            if (!pool)
                return;
            
            pool.Despawn(gameObject);
        }

        #region Pools

        private static Pool GetPool(PoolType poolType)
        {
            return poolType switch
            {
                PoolType.Tools => Tool,
                PoolType.ShipBlocks => ShipBlocks,
                PoolType.ShipComponents => ShipComponents,
                PoolType.EntitiesSpaceDebris => EntitiesSpaceDebris,
                PoolType.EntitiesProjectiles => EntitiesProjectiles,
                PoolType.EntitiesShips => EntitiesShips,
                PoolType.Currency => Currency,
                _ => throw new ArgumentOutOfRangeException(nameof(poolType), poolType, null)
            };
        }

        private static Pool Tool
        {
            get
            {
                if(!_tool)
                    _tool = PoolKit.FindPool("Tools");
                
                return _tool;
            }
        }
        
        private static Pool ShipBlocks
        {
            get
            {
                if(!_shipBlocks)
                    _shipBlocks = PoolKit.FindPool("Ship_Blocks");
                
                return _shipBlocks;
            }
        }
        
        private static Pool ShipComponents
        {
            get
            {
                if(!_shipComponents)
                    _shipComponents = PoolKit.FindPool("Ship_Components");
                
                return _shipComponents;
            }
        }
        
        private static Pool EntitiesSpaceDebris
        {
            get
            {
                if(!_entitiesSpaceDebris)
                    _entitiesSpaceDebris = PoolKit.FindPool("Entities_SpaceDebris");
                
                return _entitiesSpaceDebris;
            }
        }
        
        private static Pool EntitiesProjectiles
        {
            get
            {
                if(!_entitiesProjectiles)
                    _entitiesProjectiles = PoolKit.FindPool("Entities_Projectiles");
                
                return _entitiesProjectiles;
            }
        }
        
        private static Pool EntitiesShips
        {
            get
            {
                if(!_entitiesShips)
                    _entitiesShips = PoolKit.FindPool("Entities_Ships");
                
                return _entitiesShips;
            }
        }
        
        private static Pool Currency
        {
            get
            {
                if(!_currency)
                    _currency = PoolKit.FindPool("Currency");
                
                return _currency;
            }
        }
        
        private static Pool _tool;
        private static Pool _shipBlocks;
        private static Pool _shipComponents;
        private static Pool _entitiesSpaceDebris;
        private static Pool _entitiesProjectiles;
        private static Pool _entitiesShips;
        private static Pool _currency;

        #endregion
    }
}