using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TravelMind.Builder.Data;
using TravelMind.Enemies;
using TravelMind.Events.Abstracts;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Events
{
    [CreateAssetMenu(menuName = "TravelMind/Events/Enemies", fileName = "EnemiesEvent", order = 0)]
    public class EnemiesEvent : AEvent
    {
        [Serializable]
        private struct EnemyInfo
        {
            //public Enemy loadableEnemyPrefab;
            public BuilderDataContainer builderDataContainer;
            public Vector2 finalPosition;
            public float angle;
        }
        
        
        [SerializeField] private SimpleLoadableEnemyShip loadableEnemyPrefab;
        [SerializeField] private EnemyInfo[] enemies;
        [SerializeField][Min(0)] private float distanceFromBorder;
        [SerializeField] private Ease ease;
        [SerializeField][Min(0)] private float duration;

        public override IEnumerator Process(Action<bool> onCompleted)
        {
            var spawnedEnemies = new List<SimpleLoadableEnemyShip>();
            
            foreach (var waveEnemy in enemies)
            {
                var position = Utils.GenerateOutOfScreenPosition(distanceFromBorder, waveEnemy.finalPosition);
                var rotation = Quaternion.Euler(0, 0, waveEnemy.angle);

                var enemy = Pools.Spawn<SimpleLoadableEnemyShip>(loadableEnemyPrefab.gameObject,
                    Pools.PoolType.EntitiesShips, position, rotation);
                
                enemy.Initialize(waveEnemy.builderDataContainer);
                enemy.transform.DOMove(waveEnemy.finalPosition, duration).SetEase(ease);
                
                spawnedEnemies.Add(enemy);
            }
            
            yield return new WaitUntil(() => spawnedEnemies.All(x => !x.gameObject.activeInHierarchy));
            
            onCompleted?.Invoke(true);
        }
    }
}
