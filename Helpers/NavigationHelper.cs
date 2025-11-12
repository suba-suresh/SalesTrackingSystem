using System.Windows;

namespace SalesTrackingSystem.Helpers
{
    
    public static class NavigationHelper
    {
      
        public static void NavigateTo(Window newWindow, Window currentWindow)
        {
            if (newWindow == null || currentWindow == null) return;

            newWindow.Show();
            currentWindow.Close();
        }

       
        public static void OpenDialog(Window window)
        {
            if (window == null) return;
            window.ShowDialog();
        }
    }
}
