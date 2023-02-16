using UnityEngine;

namespace TravelMind.Currencies
{
    [CreateAssetMenu(menuName = "TravelMind/Currency", fileName = "Currency", order = 0)]
    public class Currency : ScriptableObject
    {
        public string Id => id;
        
        public string Name => nameKey;


        [SerializeField] private string id;
        [SerializeField] private string nameKey;
    }
}