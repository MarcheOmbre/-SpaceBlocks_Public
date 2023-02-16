using System;
using System.Collections;
using System.Collections.Generic;
using TravelMind.Events.Abstracts;
using UnityEngine;

namespace TravelMind.Managers
{
    public class EventsManager : MonoBehaviour
    {
        [Serializable]
        private struct EventInfo
        {
            public AEvent eventAsset;
            public float distance;
            public bool waitForCompletion;
        }
        
        
        public event Action OnEventSucceed = delegate { };
        public event Action OnEventFailed = delegate { };
        public event Action OnFinished = delegate { };


        [SerializeField] private TravelManager travelManager;
        [SerializeField] private EventInfo[] events;

        private void Start()
        {
            StartCoroutine(EventsProcess());
        }

        private IEnumerator EventsProcess()
        {
            foreach (var eventInfo in events)
            {
                //Wait for distance
                yield return new WaitUntil(() => eventInfo.distance <= travelManager.DistanceTravelled);
                
                if (eventInfo.waitForCompletion)
                    yield return StartCoroutine(eventInfo.eventAsset.Process(null));
                else
                    StartCoroutine(eventInfo.eventAsset.Process(null));
            }
            
            OnFinished();
        }

        private void EventSucceed() => OnEventSucceed();

        private void EventFailed() => OnEventFailed();
    }
}
