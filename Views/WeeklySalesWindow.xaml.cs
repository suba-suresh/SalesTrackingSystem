using SalesTrackingSystem.Models;
using SalesTrackingSystem.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace SalesTrackingSystem.Views
{
    public partial class WeeklySalesWindow : Window
    {
        public WeeklySalesWindow(ObservableCollection<SaleRecord> sales)
        {
            InitializeComponent();
            DataContext = new WeeklySalesViewModel(sales);
        }
    }
}
