using SalesTrackingSystem.ViewModels;
using System.Windows;

namespace SalesTrackingSystem.Views
{
    public partial class SalaryView : Window
    {
        public SalaryView()
        {
            InitializeComponent();
            DataContext = new SalaryViewModel();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

      
    }
}
