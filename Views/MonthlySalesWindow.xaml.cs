using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using SalesTrackingSystem.ViewModels;

namespace SalesTrackingSystem.Views
{
    public partial class MonthlySalesWindow : Window
    {
        public MonthlySalesWindow(System.Collections.ObjectModel.ObservableCollection<SalesTrackingSystem.Models.SaleRecord> sales)
        {
            InitializeComponent();
            DataContext = new MonthlySalesViewModel(sales);
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            var pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                MonthlyDataGrid.UpdateLayout();

                // Prepare FlowDocument for clean printable format
                FlowDocument doc = new FlowDocument
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12,
                    PageWidth = pd.PrintableAreaWidth
                };

                doc.Blocks.Add(new Paragraph(new Run($"Monthly Sales Report — {DateTime.Now:MMMM yyyy}"))
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    TextAlignment = TextAlignment.Center
                });
                doc.Blocks.Add(new Paragraph(new Run($"Printed on {DateTime.Now:G}\n\n")));

                // Table setup
                Table table = new Table();
                table.CellSpacing = 4;
                table.Columns.Add(new TableColumn());
                table.Columns.Add(new TableColumn());
                table.Columns.Add(new TableColumn());
                table.Columns.Add(new TableColumn());
                table.Columns.Add(new TableColumn());

                TableRowGroup group = new TableRowGroup();
                var header = new TableRow();
                header.Cells.Add(new TableCell(new Paragraph(new Run("Week"))) { FontWeight = FontWeights.Bold });
                header.Cells.Add(new TableCell(new Paragraph(new Run("Start Date"))) { FontWeight = FontWeights.Bold });
                header.Cells.Add(new TableCell(new Paragraph(new Run("End Date"))) { FontWeight = FontWeights.Bold });
                header.Cells.Add(new TableCell(new Paragraph(new Run("Total Orders"))) { FontWeight = FontWeights.Bold });
                header.Cells.Add(new TableCell(new Paragraph(new Run("Total Amount (£)"))) { FontWeight = FontWeights.Bold });
                group.Rows.Add(header);

                foreach (var item in MonthlyDataGrid.Items)
                {
                    if (item is WeeklyBreakdownItem row)
                    {
                        var r = new TableRow();
                        r.Cells.Add(new TableCell(new Paragraph(new Run(row.WeekName))));
                        r.Cells.Add(new TableCell(new Paragraph(new Run(row.StartDate))));
                        r.Cells.Add(new TableCell(new Paragraph(new Run(row.EndDate))));
                        r.Cells.Add(new TableCell(new Paragraph(new Run(row.TotalOrders.ToString()))));
                        r.Cells.Add(new TableCell(new Paragraph(new Run($"{row.TotalAmount:F2}"))));
                        group.Rows.Add(r);
                    }
                }

                table.RowGroups.Add(group);
                doc.Blocks.Add(table);

                IDocumentPaginatorSource paginator = doc;
                pd.PrintDocument(paginator.DocumentPaginator, "Monthly Sales Report");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
