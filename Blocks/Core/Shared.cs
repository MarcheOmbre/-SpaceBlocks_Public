using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TravelMind.Blocks.Core
{
    public static class Shared
    {
        #region PathFinding

        private static void ExploreConnectedNodes(ISet<Vector2Int> nodes, Vector2Int coordinates, ICollection<Vector2Int> occupiedCoordinates)
        {
            if (nodes.Contains(coordinates))
                return;

            nodes.Add(coordinates);
            
            //Check all directions
            var topNeighbor = new Vector2Int(coordinates.x, coordinates.y + 1);
            var bottomNeighbor = new Vector2Int(coordinates.x, coordinates.y - 1);
            var leftNeighbor = new Vector2Int(coordinates.x - 1, coordinates.y);
            var rightNeighbor = new Vector2Int(coordinates.x + 1, coordinates.y);
            
            var neighborBlocks = new List<Vector2Int>();
            if(occupiedCoordinates.Contains(topNeighbor))
                neighborBlocks.Add(topNeighbor);
            if(occupiedCoordinates.Contains(bottomNeighbor))
                neighborBlocks.Add(bottomNeighbor);
            if(occupiedCoordinates.Contains(leftNeighbor))
                neighborBlocks.Add(leftNeighbor);
            if(occupiedCoordinates.Contains(rightNeighbor))
                neighborBlocks.Add(rightNeighbor);

            //Register new nodes
            foreach (var connected in neighborBlocks.Where(x => !nodes.Contains(x)))
                ExploreConnectedNodes(nodes, connected, occupiedCoordinates);
        }

        public static bool HasPath(Vector2Int startCoordinates, Vector2Int endCoordinates, ICollection<Vector2Int> occupiedCoordinates)
        {
            var nodes = new HashSet<Vector2Int>();
 
            ExploreConnectedNodes(nodes, startCoordinates, occupiedCoordinates);

            return nodes.Contains(endCoordinates);
        }
        #endregion
    }
}
