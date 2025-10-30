// File: Views/ExpenseView.xaml.cs
using SalesTrackingSystem.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SalesTrackingSystem.Views
{
    public partial class ExpenseView : Window
    {
        public ExpenseView()
        {
            InitializeComponent();
            DataContext = new ExpenseViewModel();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ExpenseViewModel vm))
            {
                MessageBox.Show("Unable to print: ViewModel not found.", "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (vm.Expenses == null || vm.Expenses.Count == 0)
            {
                MessageBox.Show("No expenses to print.", "Print", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Build FlowDocument for printing
            var fd = new FlowDocument
            {
                PagePadding = new Thickness(36),
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 12,
                ColumnWidth = 9999 // keep single column
            };

            // Title
            var title = new Paragraph(new Run("Expense Report"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 8)
            };
            fd.Blocks.Add(title);

            // Header with date/time and total count
            var headerTxt = $"Report Date: {DateTime.Now:dd MMM yyyy}    Records: {vm.Expenses.Count}";
            fd.Blocks.Add(new Paragraph(new Run(headerTxt)) { Foreground = Brushes.Gray, Margin = new Thickness(0, 0, 0, 12) });

            // Table definition (columns: Date, Category, PaymentType, Amount)
            var table = new Table { CellSpacing = 0 };
            table.Columns.Add(new TableColumn { Width = new GridLength(120) }); // Date
            table.Columns.Add(new TableColumn { Width = new GridLength(300) }); // Category
            table.Columns.Add(new TableColumn { Width = new GridLength(120) }); // PaymentType
            table.Columns.Add(new TableColumn { Width = new GridLength(120) }); // Amount

            // Header row group
            var headerGroup = new TableRowGroup();
            table.RowGroups.Add(headerGroup);
            var headerRow = new TableRow();
            headerGroup.Rows.Add(headerRow);

            string[] headers = { "Date", "Category", "Payment Type", "Amount (£)" };
            foreach (var h in headers)
            {
                var cell = new TableCell(new Paragraph(new Run(h)) { FontWeight = FontWeights.SemiBold });
                cell.Padding = new Thickness(4);
                cell.BorderBrush = Brushes.LightGray;
                cell.BorderThickness = new Thickness(0, 0, 0, 1);
                headerRow.Cells.Add(cell);
            }

            // Body rows
            var bodyGroup = new TableRowGroup();
            table.RowGroups.Add(bodyGroup);

            foreach (var ex in vm.Expenses)
            {
                var row = new TableRow();
                bodyGroup.Rows.Add(row);

                // Date
                var dateText = ex.Date.ToString("dd/MM/yyyy");
                var dateCell = new TableCell(new Paragraph(new Run(dateText))) { Padding = new Thickness(4) };
                row.Cells.Add(dateCell);

                // Category
                var catText = ex.Category ?? "";
                var catCell = new TableCell(new Paragraph(new Run(catText))) { Padding = new Thickness(4) };
                row.Cells.Add(catCell);

                // PaymentType
                var payText = ex.PaymentType ?? "";
                var payCell = new TableCell(new Paragraph(new Run(payText))) { Padding = new Thickness(4) };
                row.Cells.Add(payCell);

                // Amount
                var amtText = ex.Amount.ToString("C2", CultureInfo.GetCultureInfo("en-GB"));
                var amtCell = new TableCell(new Paragraph(new Run(amtText)) { TextAlignment = TextAlignment.Right }) { Padding = new Thickness(4) };
                row.Cells.Add(amtCell);
            }

            fd.Blocks.Add(table);

            // Totals summary
            var totalsPara = new Paragraph { Margin = new Thickness(0, 12, 0, 0) };
            totalsPara.Inlines.Add(new Run($"Total Cash: {vm.TotalCash.ToString("C2", CultureInfo.GetCultureInfo("en-GB"))}") { FontWeight = FontWeights.Bold });
            totalsPara.Inlines.Add(new Run("    "));
            totalsPara.Inlines.Add(new Run($"Total Card: {vm.TotalCard.ToString("C2", CultureInfo.GetCultureInfo("en-GB"))}") { FontWeight = FontWeights.Bold });
            totalsPara.Inlines.Add(new Run("    "));
            totalsPara.Inlines.Add(new Run($"Grand Total: {vm.TotalAmount.ToString("C2", CultureInfo.GetCultureInfo("en-GB"))}") { FontWeight = FontWeights.Bold });
            fd.Blocks.Add(totalsPara);

            // PrintDialog and actual print
            var pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                try
                {
                    fd.PageHeight = pd.PrintableAreaHeight;
                    fd.PageWidth = pd.PrintableAreaWidth;
                    fd.ColumnWidth = pd.PrintableAreaWidth;
                    IDocumentPaginatorSource idp = fd;
                    pd.PrintDocument(idp.DocumentPaginator, "Expense Report");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Print failed: " + ex.Message, "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
