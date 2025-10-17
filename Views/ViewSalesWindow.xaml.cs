// File: Views/ViewSalesWindow.xaml.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SalesTrackingSystem.Views
{
    public partial class ViewSalesWindow : Window
    {
        public ViewSalesWindow()
        {
            InitializeComponent();
            this.DataContext = new SalesTrackingSystem.ViewModels.ViewSalesViewModel();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Do you want to print the current grid?", "Print", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            var pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                // Simple approach: print the DataGrid visual.
                // For longer grids, consider generating a FlowDocument or PDF.
                // Temporarily remove focus to get clean layout
                SalesDataGrid.UpdateLayout();

                // Scale the visual to fit printable area if necessary:
                var printableArea = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);
                SalesDataGrid.Measure(printableArea);
                SalesDataGrid.Arrange(new Rect(printableArea));

                pd.PrintVisual(SalesDataGrid, "Sales Report");
            }
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes this window and returns to MainWindow
        }

    }
}
