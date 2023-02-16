using System.Collections.Generic;
using System.Linq;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Blocks.Core
{
    public class BlocksLogic : MonoBehaviour
    {
        public float TotalHealth => totalHealth;
        
        public float CurrentHealth => currentHealth;
        
        public int TotalBlocks => totalBlocks;
        
        public int CurrentBlocks => currentBlocks;

        
        [SerializeField] private BlocksGridManager blocksGridManager;

        private readonly List<ShipBlock> shipBlocks = new();
        
        private float totalHealth;
        private float currentHealth;
        private int totalBlocks;
        private int currentBlocks;

        
        private void OnEnable() => RefreshBlocks();

        private void ClearBlocks()
        {
            foreach (var shipBlock in shipBlocks)
            {
                shipBlock.OnAttacked -= OnAttacked;
                shipBlock.OnDepleted -= OnDepleted;
            }
            
            shipBlocks.Clear();
        }
        
        private void RefreshBlocks()
        {
            ClearBlocks();
            
            totalHealth = 0;
            totalBlocks = 0;

            foreach (var block in blocksGridManager.GetByAvailability(Ship.Availability.Occupied)
                         .Select(vector2Int => blocksGridManager.GetAtCoordinates(vector2Int))
                         .Where(block => block != null))
            {
                block.OnAttacked += OnAttacked;
                block.OnDepleted += OnDepleted;
                
                totalHealth += block.MaxHealth; 
                totalBlocks ++;
                
                shipBlocks.Add(block);
            }

            currentHealth = totalHealth;
            currentBlocks = totalBlocks;
        }

        private void OnAttacked(ShipBlock shipBlock, float damages) => currentHealth -= damages;

        private void OnDepleted(ShipBlock shipBlock) => currentBlocks--;
    }
}
