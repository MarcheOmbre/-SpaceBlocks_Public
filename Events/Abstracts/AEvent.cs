using System;
using System.Collections;
using UnityEngine;

namespace TravelMind.Events.Abstracts
{
    public abstract class AEvent : ScriptableObject
    {
        public abstract IEnumerator Process(Action<bool> onCompleted);
    }
}