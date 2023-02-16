using TravelMind.Plugins.Utils;
using UnityEngine;

namespace TravelMind.Currencies.Loot
{
    public class LootPointsManager : MonoBehaviourSingleton<LootPointsManager>
    {
        public Vector3 PlayerPosition => playerTransform.position;

        [SerializeField] private Transform playerTransform;
    }
}
