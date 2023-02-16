using System;
using System.Collections.Generic;
using TravelMind.Plugins.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TravelMind.Plugins.Interactions
{
    /// <summary>
    /// System
    /// </summary>
    public class InteractionsManager : MonoBehaviourSingleton<InteractionsManager>
    {
        private struct FingerInteractionData
        {
            public Vector2 StartPosition;
            public float StartTime;
            
            public Vector2 EndPosition;
            public float EndTime;
            
            public bool IsHold;
        }

        public struct TapData
        {
            public float Time;
            public float NormalizedTime;
            public Vector2 Position;
        }

        public event Action<Vector3> OnPointerDown = delegate { };
        public event Action<Vector3> OnPointerUp = delegate { };
        public event Action<Vector3> OnPointerStartHold = delegate { };
        public event Action<TapData> OnTap = delegate { };

        public Camera InteractionCamera => interactionCamera;
        
        
        [SerializeField] private Camera interactionCamera;
        [SerializeField] [Min(0)] private float maxTimeTap = 0.2f;
        [SerializeField] [Min(0)] private float maxTapInchesDistance = 1f;

        private readonly Dictionary<int, FingerInteractionData> interactingFingersDictionary = new();

        private void Update()
        {
            #if UNITY_EDITOR
            var touches = new[] {new Touch
            {
                fingerId = 0,
                position = Input.mousePosition,
                phase = Input.GetMouseButtonDown(0) ? TouchPhase.Began : Input.GetMouseButtonUp(0) ? TouchPhase.Ended : TouchPhase.Moved
            }};
            #else
            var touches = Input.touches;
            #endif
            
            foreach (var touch in touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        
                        if (IsPointerOveringUi(touch.position))
                            continue;

                        //TODO : Check why
                        //Remove interaction if called twice
                        if (interactingFingersDictionary.ContainsKey(touch.fingerId))
                            interactingFingersDictionary.Remove(touch.fingerId);
                        
                        var fingerBeganData = new FingerInteractionData
                        {
                            StartPosition = touch.position,
                            StartTime = Time.time
                        };
                        
                        interactingFingersDictionary.Add(touch.fingerId, fingerBeganData);
                        OnPointerDown(touch.position);
                        
                        break;
                    
                    case TouchPhase.Stationary:
                        case TouchPhase.Moved:
                        
                        if(!interactingFingersDictionary.TryGetValue(touch.fingerId, out var fingerStationaryData))
                            continue;

                        if (!fingerStationaryData.IsHold && Time.time - fingerStationaryData.StartTime > maxTimeTap 
                            && (touch.position - fingerStationaryData.StartPosition).magnitude / Screen.dpi <= maxTapInchesDistance)
                        {
                            OnPointerStartHold(touch.position);
                            fingerStationaryData.IsHold = true;
                            interactingFingersDictionary[touch.fingerId] = fingerStationaryData;
                        }

                        break;
                    
                    case TouchPhase.Ended or TouchPhase.Canceled:
                        
                        if(!interactingFingersDictionary.TryGetValue(touch.fingerId, out var FingerEndedData))
                            continue;
                        
                        FingerEndedData.EndPosition = touch.position;
                        FingerEndedData.EndTime = Time.time;
                        
                        FinishInteraction(FingerEndedData);
                        interactingFingersDictionary.Remove(touch.fingerId);
                        
                        break;
                }
            }
        }

        private void OnDisable()
        {
            interactingFingersDictionary.Clear();
        }

        private void FinishInteraction(FingerInteractionData fingerInteractionData)
        {
            OnPointerUp(fingerInteractionData.EndPosition);

            var time = Time.time - fingerInteractionData.StartTime;
            var vector = fingerInteractionData.EndPosition - fingerInteractionData.StartPosition;
            var distance = vector.magnitude;

            if (distance / Screen.dpi <= maxTapInchesDistance && time <= maxTimeTap)
            {
                OnTap
                (
                    new TapData
                    {
                        Time = time,
                        NormalizedTime = Mathf.Clamp01(time / maxTimeTap),
                        Position = fingerInteractionData.EndPosition
                    }
                );
            }
        }

        private static bool IsPointerOveringUi(Vector3 pointerPosition)
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current) 
                {position = pointerPosition};
            
            var results = new List<RaycastResult>();
            
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            
            return results.Count > 0;
        }
    }
}