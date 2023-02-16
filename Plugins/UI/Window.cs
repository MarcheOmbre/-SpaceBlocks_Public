using UnityEngine;
using UnityEngine.Events;

namespace TravelMind.Plugins.UI
{
    public class Window : MonoBehaviour
    {
        public UnityEvent onOpen = new();
        public UnityEvent onClose = new();

        public bool IsOpen => gameObject.activeSelf;

        public void Open()
        {
            if (IsOpen)
                return;
            
            gameObject.SetActive(true);
            onOpen.Invoke();
        }

        public void Close()
        {
            if (!IsOpen)
                return;
            
            gameObject.SetActive(false);
            onClose.Invoke();
        }
    }
}