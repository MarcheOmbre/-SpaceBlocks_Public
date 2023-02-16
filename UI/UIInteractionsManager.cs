using TravelMind.Plugins.UI;
using TravelMind.Plugins.Utils;
using UnityEngine;

namespace TravelMind.UI
{
    public class UIInteractionsManager : MonoBehaviourSingleton<UIInteractionsManager>
    {
        [SerializeField] private StationWindow stationWindow;
        
        private Window currentWindow;

        private void CloseWindow(Object window)
        {
            if (currentWindow == null || currentWindow != window)
                return;
            
            currentWindow.Close();
            currentWindow = null;
        }
        
        
        public void OpenBuildActionsWindow()
        {
            if (currentWindow != null)
                return;

            currentWindow = stationWindow;
            stationWindow.Open();
        }

        public void CloseBuildActionsWindow()
        {
            if(currentWindow != stationWindow)
                return;

            CloseWindow(stationWindow);
        }
    }
}
