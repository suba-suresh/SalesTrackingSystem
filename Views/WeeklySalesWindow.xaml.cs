using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using SalesTrackingSystem.Models;
using SalesTrackingSystem.ViewModels;

namespace SalesTrackingSystem.Views
{
    public partial class WeeklySalesWindow : Window
    {
        public WeeklySalesWindow(ObservableCollection<SaleRecord> sales)
        {
            InitializeComponent();
            DataContext = new WeeklySalesViewModel();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as WeeklySalesViewModel;
            var result = MessageBox.Show("Print visible weekly grid?", "Print", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            var pd = new PrintDialog();
            if (pd.ShowDialog() != true) return;

            // Create a FlowDocument with header + table text from DataGrid
            var fd = new FlowDocument();
            fd.PagePadding = new Thickness(40);
            fd.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
            fd.FontSize = 12;

            var title = new Paragraph(new Run($"Weekly Sales Report — week of {vm?.WeekStart.ToString("dd MMM yyyy") ?? DateTime.Today.ToString("dd MMM yyyy")}"));
            title.FontSize = 16;
            title.FontWeight = FontWeights.Bold;
            fd.Blocks.Add(title);

            // Build a simple table
            var table = new Table();
            table.CellSpacing = 0;
            table.Columns.Add(new TableColumn { Width = new GridLength(140) });
            table.Columns.Add(new TableColumn { Width = new GridLength(140) });
            table.Columns.Add(new TableColumn { Width = new GridLength(120) });
            table.Columns.Add(new TableColumn { Width = new GridLength(120) });

            // header row
            var headerRow = new TableRow();
            var headerGroup = new TableRowGroup();
            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Day"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Total Orders"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Total Amount"))) { FontWeight = FontWeights.Bold });

            // data rows
            if (vm?.WeeklySummary != null)
            {
                var bodyGroup = new TableRowGroup();
                foreach (var row in vm.WeeklySummary)
                {
                    var tr = new TableRow();
                    tr.Cells.Add(new TableCell(new Paragraph(new Run(row.Day))));
                    tr.Cells.Add(new TableCell(new Paragraph(new Run(row.Date))));
                    tr.Cells.Add(new TableCell(new Paragraph(new Run(row.TotalOrders.ToString()))));
                    tr.Cells.Add(new TableCell(new Paragraph(new Run(string.Format("£{0:F2}", row.TotalAmount)))));
                    bodyGroup.Rows.Add(tr);
                }
                table.RowGroups.Add(bodyGroup);
            }

            fd.Blocks.Add(table);

            // print
            IDocumentPaginatorSource doc = fd;
            pd.PrintDocument(doc.DocumentPaginator, "Weekly Sales Report");
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
