
using SalesTrackingSystem.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
         
            if (!(this.DataContext is ViewSalesViewModel vm))
            {
                MessageBox.Show("Unable to print: ViewModel not found.", "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (vm.FilteredSales == null || vm.FilteredSales.Count == 0)
            {
                MessageBox.Show("Nothing to print — the grid is empty.", "Print", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Build FlowDocument
            var fd = new FlowDocument();
            fd.PagePadding = new Thickness(40);
            fd.FontFamily = new FontFamily("Segoe UI");
            fd.FontSize = 12;

            // Title
            var title = new Paragraph(new Run("Sales Analytics Report"))
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(0, 0, 0, 6)
            };
            fd.Blocks.Add(title);

            // Header: selected date & generation time
            var headerText = $"Report Date: {vm.SelectedDate:dd MMM yyyy}    Generated: {DateTime.Now:dd MMM yyyy HH:mm}";
            var header = new Paragraph(new Run(headerText))
            {
                FontSize = 11,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 12)
            };
            fd.Blocks.Add(header);

            // Build table based on the DataGrid columns (SalesDataGrid) if available, else fallback to default columns
            var table = new Table();
            table.CellSpacing = 0;
            fd.Blocks.Add(table);

            // Try to get the columns from DataGrid to reflect headers & binding paths
            string[] columnHeaders;
            string[] propertyPaths;

            try
            {
                // If SalesDataGrid exists in the XAML, we can extract its columns
                var dg = this.FindName("SalesDataGrid") as DataGrid;
                if (dg != null && dg.Columns.Count > 0)
                {
                    columnHeaders = dg.Columns.Select(c => c.Header?.ToString() ?? string.Empty).ToArray();

                    propertyPaths = dg.Columns.Select(c =>
                    {
                        if (c is DataGridTextColumn tcol)
                        {
                            // Try to get binding path
                            var binding = tcol.Binding as System.Windows.Data.Binding;
                            if (binding != null && binding.Path != null)
                                return binding.Path.Path;
                        }
                        // fallback: try header normalized
                        return c.Header?.ToString()?.Split(' ')[0] ?? string.Empty;
                    }).ToArray();
                }
                else
                {
                    // Fallback fixed columns known for Sales
                    columnHeaders = new[] {
                        "Date","JustEat (orders)","Uber (orders)","Deliveroo (orders)","InHouse (orders)",
                        "JustEat (£)","Uber (£)","Deliveroo (£)","InHouse (£)"
                    };
                    propertyPaths = new[] {
                        "Date","OrdersJustEat","OrdersUber","OrdersDeliveroo","OrdersInHouse",
                        "AmountJustEat","AmountUber","AmountDeliveroo","AmountInhouse"
                    };
                }
            }
            catch
            {
                // safe fallback if anything fails
                columnHeaders = new[] {
                    "Date","JustEat (orders)","Uber (orders)","Deliveroo (orders)","InHouse (orders)",
                    "JustEat (£)","Uber (£)","Deliveroo (£)","InHouse (£)"
                };
                propertyPaths = new[] {
                    "Date","OrdersJustEat","OrdersUber","OrdersDeliveroo","OrdersInHouse",
                    "AmountJustEat","AmountUber","AmountDeliveroo","AmountInhouse"
                };
            }

            // Create columns for table
            for (int i = 0; i < columnHeaders.Length; i++)
                table.Columns.Add(new TableColumn());

            // Header row group
            var headerGroup = new TableRowGroup();
            table.RowGroups.Add(headerGroup);
            var headerRow = new TableRow();
            headerGroup.Rows.Add(headerRow);

            foreach (var h in columnHeaders)
            {
                var cell = new TableCell(new Paragraph(new Run(h)) { FontWeight = FontWeights.SemiBold });
                cell.Padding = new Thickness(4, 4, 4, 4);
                cell.BorderBrush = Brushes.LightGray;
                cell.BorderThickness = new Thickness(0, 0, 0, 1);
                headerRow.Cells.Add(cell);
            }

            // Data rows
            var bodyGroup = new TableRowGroup();
            table.RowGroups.Add(bodyGroup);

            // Use reflection to fetch property values
            foreach (var item in vm.FilteredSales)
            {
                var row = new TableRow();
                bodyGroup.Rows.Add(row);

                for (int ci = 0; ci < propertyPaths.Length; ci++)
                {
                    var path = propertyPaths[ci];
                    string text = "";

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            // Support nested or Date formatting fallback
                            var prop = item.GetType().GetProperty(path, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                            if (prop != null)
                            {
                                var val = prop.GetValue(item);
                                if (val is DateTime dt)
                                    text = dt.ToString("dd/MM/yyyy");
                                else if (val is decimal dec)
                                    text = dec.ToString("C2", CultureInfo.GetCultureInfo("en-GB"));
                                else if (val != null)
                                    text = val.ToString();
                            }
                            else
                            {
                                // property not found – fallback to empty or try header mapping
                                text = "";
                            }
                        }
                    }
                    catch
                    {
                        text = "";
                    }

                    var cell = new TableCell(new Paragraph(new Run(text)));
                    cell.Padding = new Thickness(4, 2, 4, 2);
                    row.Cells.Add(cell);
                }
            }

            // Summary block (totals)
            var summaryPara = new Paragraph();
            summaryPara.Margin = new Thickness(0, 12, 0, 0);
            summaryPara.Inlines.Add(new Run("    "));
            summaryPara.Inlines.Add(new Run($"Total Revenue: {vm.TotalRevenue.ToString("C2", CultureInfo.GetCultureInfo("en-GB"))}") { FontWeight = FontWeights.Bold });
            fd.Blocks.Add(summaryPara);

            // Print
            var pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                // adjust page size to printable area
                fd.PageHeight = pd.PrintableAreaHeight;
                fd.PageWidth = pd.PrintableAreaWidth;
                fd.ColumnWidth = pd.PrintableAreaWidth;

                IDocumentPaginatorSource idp = fd;
                try
                {
                    pd.PrintDocument(idp.DocumentPaginator, "Sales Report");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Print failed: " + ex.Message, "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes this window and returns to MainWindow
        }
    }
}
