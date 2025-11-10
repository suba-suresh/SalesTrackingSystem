using System.Windows;
using SalesTrackingSystem.Views;  
using System.Collections.ObjectModel;
using SalesTrackingSystem.Models;
namespace SalesTrackingSystem.Views
{
    public partial class HomeWindow : Window
    {
        
        private ObservableCollection<SaleRecord> _sales = new ObservableCollection<SaleRecord>();

        public HomeWindow()
        {
            InitializeComponent();
           
        }

        private void OpenSales_Click(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();  // Sales page
            window.ShowDialog();
        }

        private void OpenSalary_Click(object sender, RoutedEventArgs e)
        {
            var window = new SalaryView(); //Salary page
            window.ShowDialog();
        }

        private void OpenExpenses_Click(object sender, RoutedEventArgs e)
        {
            var window = new ExpenseView(); //Expense Page
            window.ShowDialog();
        }

        private void OpenWeeklySales_Click(object sender, RoutedEventArgs e)
        {
            // Pass the required sales collection to the WeeklySalesWindow constructor
            var window = new WeeklySalesWindow(_sales); 
            window.ShowDialog();
        }

        private void OpenMonthlySales_Click(object sender, RoutedEventArgs e)
        {
            var window = new MonthlySalesWindow(_sales); //Monthlysales Page
            window.ShowDialog();
        }

       
    }
}
