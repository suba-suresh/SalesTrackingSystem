using System.Windows;
using SalesTrackingSystem.ViewModels;
using System.Collections.ObjectModel;
using SalesTrackingSystem.Models;

namespace SalesTrackingSystem.Views
{
    public partial class WeekendSalesView : Window
    {
        private WeekendSalesViewModel _viewModel;

        public WeekendSalesView(ObservableCollection<SaleRecord> sales)
        {
            InitializeComponent();
            _viewModel = new WeekendSalesViewModel(); // Removed WeekendDataGrid argument
            DataContext = _viewModel;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}