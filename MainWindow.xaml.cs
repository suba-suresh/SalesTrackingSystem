using SalesTrackingSystem.ViewModels;
using System.Windows;

namespace SalesTrackingSystem.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new SalesViewModel();
        }
    }
}
