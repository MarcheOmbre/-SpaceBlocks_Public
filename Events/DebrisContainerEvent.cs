using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TravelMind.Entities.Debris;
using TravelMind.Events.Abstracts;
using TravelMind.Plugins.Interactions;
using TravelMind.Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TravelMind.Events
{
    [CreateAssetMenu(menuName = "TravelMind/Events/DebrisContainer", fileName = "DebrisContainerEvent", order = 0)]
    public class DebrisContainerEvent : AEvent
    {
        [Serializable]
        private struct DebrisContainerInfo
        {
            public DebrisContainer debrisContainer;
            [Min(0)] public float enableForce;
            [Min(1)] public float weight;
        }
        
        [SerializeField] private DebrisContainerInfo[] debrisContainerInfos;
        [SerializeField] [Min(0)] private float maxAngle;
        [SerializeField] private Vector2 distanceRangeFromBorder;
        [SerializeField] private Vector2Int resourcesCountRange;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (distanceRangeFromBorder.y < distanceRangeFromBorder.x)
                distanceRangeFromBorder.y = distanceRangeFromBorder.x;
        }
#endif

        public override IEnumerator Process(Action<bool> onCompleted)
        {
            var quantity = Random.Range(resourcesCountRange.x, resourcesCountRange.y);
            var totalWeight = debrisContainerInfos.Sum(x => x.weight);

            var spawnedDebris = new List<DebrisContainer>();
            
            for (var i = 0; i < quantity; i++)
            {
                var randomWeight = Random.Range(0, totalWeight);
                var currentWeight = 0f;
                
                DebrisContainerInfo debrisContainerInfo = default;
                foreach (var container in debrisContainerInfos)
                {
                    currentWeight += container.weight;
                    
                    if (currentWeight < randomWeight) 
                        continue;
                    
                    debrisContainerInfo = container;
                    break;
                }
                
                //Spawn
                var position = Utils.GenerateRandomOutOfScreenPosition(distanceRangeFromBorder, Vector2.up, maxAngle);
                var debrisContainer = Pools.Spawn<DebrisContainer>(debrisContainerInfo.debrisContainer.gameObject, 
                    Pools.PoolType.EntitiesSpaceDebris, position);

                    //Add force
                var goalPoint = new Vector2
                {
                    x = Random.Range(0, Screen.width),
                    y = Random.Range(0, Screen.height)
                };
            
                var direction = (goalPoint - (Vector2) 
                    InteractionsManager.Instance.InteractionCamera.WorldToScreenPoint(debrisContainer.transform.position)).normalized;
                debrisContainer.Rigidbody2D.AddForce(direction * debrisContainerInfo.enableForce, ForceMode2D.Impulse);

                spawnedDebris.Add(debrisContainer);
            }
            
            yield return new WaitUntil(() => spawnedDebris.All(x => !x.gameObject.activeInHierarchy));
            
            onCompleted?.Invoke(true);
        }
    }
}