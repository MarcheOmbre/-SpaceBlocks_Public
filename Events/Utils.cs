using System;
using TravelMind.Plugins.Interactions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TravelMind.Events
{
    public static class Utils
    {
        public static Vector2 GenerateOutOfScreenPosition(float distanceFromBorder, Vector2 finalPosition)
        {
            var vector = (Vector2)InteractionsManager.Instance.InteractionCamera.WorldToScreenPoint(finalPosition);

            var halfScreen = new Vector2(Screen.width / 2f, Screen.height / 2f);
            var direction = (vector - halfScreen).normalized;
            
            vector = halfScreen + direction * halfScreen.magnitude;
            vector = (Vector2)InteractionsManager.Instance.InteractionCamera.ScreenToWorldPoint(vector) + direction * distanceFromBorder;
            
            return vector;
        }
        public static Vector2 GenerateRandomOutOfScreenPosition(Vector2 distanceRangeFromBorder, Vector2 centerVector, float maxAngle)
        {
            if (distanceRangeFromBorder.x > distanceRangeFromBorder.y)
                throw new ArgumentException("x must be less than y");
            
            //Compute screen data
            var screenHalfSize = new Vector2(Screen.width / 2f, Screen.height / 2f);
            var halfAngle = maxAngle / 2f;
            
            //Set vector
            var vector = screenHalfSize + centerVector * screenHalfSize.magnitude;
            
            //Add distance
            vector = (Vector2)InteractionsManager.Instance.InteractionCamera.ScreenToWorldPoint(vector) 
                     + vector.normalized * Random.Range(distanceRangeFromBorder.x, distanceRangeFromBorder.y);

            //Apply angle
            vector = Quaternion.Euler(Vector3.forward * Random.Range(-halfAngle, halfAngle)) * vector;
            
            return vector;
        }
    }
}
