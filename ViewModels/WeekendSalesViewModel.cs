using SalesTrackingSystem.Commands;
using SalesTrackingSystem.Data;
using SalesTrackingSystem.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Media;

namespace SalesTrackingSystem.ViewModels
{
    public class WeekendSalesViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        private DateTime _selectedWeekDate = DateTime.Today;
        public DateTime SelectedWeekDate
        {
            get => _selectedWeekDate;
            set
            {
                SetProperty(ref _selectedWeekDate, value);
                // ✅ Don't auto-reload here - let the button do it
                // LoadWeekendData(); // Remove this line
            }
        }

        // Dashboard Properties
        private string _weekendDateRange;
        public string WeekendDateRange
        {
            get => _weekendDateRange;
            set => SetProperty(ref _weekendDateRange, value);
        }

        private decimal _totalWeekendRevenue;
        public decimal TotalWeekendRevenue
        {
            get => _totalWeekendRevenue;
            set => SetProperty(ref _totalWeekendRevenue, value);
        }

        private int _totalWeekendOrders;
        public int TotalWeekendOrders
        {
            get => _totalWeekendOrders;
            set => SetProperty(ref _totalWeekendOrders, value);
        }

        private string _weekendSummary;
        public string WeekendSummary
        {
            get => _weekendSummary;
            set => SetProperty(ref _weekendSummary, value);
        }

        private string _ordersBreakdown;
        public string OrdersBreakdown
        {
            get => _ordersBreakdown;
            set => SetProperty(ref _ordersBreakdown, value);
        }

        private string _bestDay;
        public string BestDay
        {
            get => _bestDay;
            set => SetProperty(ref _bestDay, value);
        }

        private decimal _bestDayRevenue;
        public decimal BestDayRevenue
        {
            get => _bestDayRevenue;
            set => SetProperty(ref _bestDayRevenue, value);
        }

        private string _bestDayDetails;
        public string BestDayDetails
        {
            get => _bestDayDetails;
            set => SetProperty(ref _bestDayDetails, value);
        }

        private decimal _saturdayRevenue;
        public decimal SaturdayRevenue
        {
            get => _saturdayRevenue;
            set => SetProperty(ref _saturdayRevenue, value);
        }

        private decimal _sundayRevenue;
        public decimal SundayRevenue
        {
            get => _sundayRevenue;
            set => SetProperty(ref _sundayRevenue, value);
        }

        private string _revenueComparison;
        public string RevenueComparison
        {
            get => _revenueComparison;
            set => SetProperty(ref _revenueComparison, value);
        }

        private decimal _justEatTotal;
        public decimal JustEatTotal
        {
            get => _justEatTotal;
            set => SetProperty(ref _justEatTotal, value);
        }

        private decimal _uberTotal;
        public decimal UberTotal
        {
            get => _uberTotal;
            set => SetProperty(ref _uberTotal, value);
        }

        private decimal _deliverooTotal;
        public decimal DeliverooTotal
        {
            get => _deliverooTotal;
            set => SetProperty(ref _deliverooTotal, value);
        }

        private decimal _inHouseTotal;
        public decimal InHouseTotal
        {
            get => _inHouseTotal;
            set => SetProperty(ref _inHouseTotal, value);
        }

        public ICommand LoadWeekendDataCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand RefreshCommand { get; } // ✅ Add separate refresh command


        public WeekendSalesViewModel()
        {
            _context = new AppDbContext();

            LoadWeekendDataCommand = new RelayCommand(o => LoadWeekendData());
            RefreshCommand = new RelayCommand(o => RefreshToToday()); // ✅ Added
            PrintCommand = new RelayCommand(o => PrintDashboard());

            // Load current week's weekend by default
            LoadWeekendData();
        }

        // ✅ Add this method
        private void RefreshToToday()
        {
            SelectedWeekDate = DateTime.Today;
            LoadWeekendData();
            MessageBox.Show($"Refreshed to current week", "Refreshed", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadWeekendData()
        {
            try
            {
                // Find the Saturday and Sunday of the selected week
                var (saturday, sunday) = GetWeekendDates(SelectedWeekDate);

                // Convert dates to database-compatible format
                var saturdayDate = saturday.Date;
                var sundayDate = sunday.Date;

                // Query database - fetch to memory first to avoid LINQ-to-Entities issues
                var allSales = _context.Sales.ToList();

                var saturdayRecord = allSales.FirstOrDefault(s => s.Date.Date == saturdayDate);
                var sundayRecord = allSales.FirstOrDefault(s => s.Date.Date == sundayDate);

                // Calculate totals
                decimal satRevenue = saturdayRecord?.TotalAmount ?? 0;
                decimal sunRevenue = sundayRecord?.TotalAmount ?? 0;
               
                // Set properties
                WeekendDateRange = $"{saturday:dddd, dd MMM} - {sunday:dddd, dd MMM yyyy}";
                TotalWeekendRevenue = satRevenue + sunRevenue;
                SaturdayRevenue = satRevenue;
                SundayRevenue = sunRevenue;

                WeekendSummary = $"Sat: £{satRevenue:N2} + Sun: £{sunRevenue:N2}";
                

                // Determine best day
                if (satRevenue > sunRevenue)
                {
                    BestDay = "Saturday";
                    BestDayRevenue = satRevenue;
                    BestDayDetails = $" {(satRevenue - sunRevenue > 0 ? "£" + (satRevenue - sunRevenue).ToString("N2") + " more than Sunday" : "Same as Sunday")}";
                }
                else if (sunRevenue > satRevenue)
                {
                    BestDay = "Sunday";
                    BestDayRevenue = sunRevenue;
                    BestDayDetails = $" £{(sunRevenue - satRevenue):N2} more than Saturday";
                }
                else
                {
                    BestDay = "Tie";
                    BestDayRevenue = satRevenue;
                    BestDayDetails = "Both days performed equally";
                }

                // Revenue comparison
                if (satRevenue > sunRevenue)
                    RevenueComparison = $"Saturday leads by £{(satRevenue - sunRevenue):N2}";
                else if (sunRevenue > satRevenue)
                    RevenueComparison = $"Sunday leads by £{(sunRevenue - satRevenue):N2}";
                else
                    RevenueComparison = "Both days are equal";

                // Channel breakdown
                JustEatTotal = (saturdayRecord?.AmountJustEat ?? 0) + (sundayRecord?.AmountJustEat ?? 0);
                UberTotal = (saturdayRecord?.AmountUber ?? 0) + (sundayRecord?.AmountUber ?? 0);
                DeliverooTotal = (saturdayRecord?.AmountDeliveroo ?? 0) + (sundayRecord?.AmountDeliveroo ?? 0);
                InHouseTotal = (saturdayRecord?.AmountInhouse ?? 0) + (sundayRecord?.AmountInhouse ?? 0);

                if (saturdayRecord == null && sundayRecord == null)
                {
                    MessageBox.Show($"No sales data found for weekend:\n{saturday:dd/MM/yyyy} and {sunday:dd/MM/yyyy}",
                        "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading weekend data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private (DateTime saturday, DateTime sunday) GetWeekendDates(DateTime date)
        {
            // Find the Saturday of the current week
            int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)date.DayOfWeek + 7) % 7;
            DateTime saturday = date.AddDays(daysUntilSaturday);

            // If today is Sunday, go back to the previous Saturday
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                saturday = date.AddDays(-1);
            }

            DateTime sunday = saturday.AddDays(1);

            return (saturday, sunday);
        }

        private void PrintDashboard()
        {
            try
            {
                // Create a printable document
                FlowDocument document = new FlowDocument();
                document.PagePadding = new Thickness(50);
                document.ColumnWidth = double.PositiveInfinity;

                // Title
                Paragraph title = new Paragraph(new Run("Weekend Sales Dashboard"));
                title.FontSize = 24;
                title.FontWeight = FontWeights.Bold;
                title.TextAlignment = TextAlignment.Center;
                document.Blocks.Add(title);

                // Date Range
                Paragraph dateRange = new Paragraph(new Run(WeekendDateRange));
                dateRange.FontSize = 16;
                dateRange.TextAlignment = TextAlignment.Center;
                dateRange.Margin = new Thickness(0, 10, 0, 20);
                document.Blocks.Add(dateRange);

                // Summary
                Paragraph summary = new Paragraph();
                summary.Inlines.Add(new Bold(new Run("Total Weekend Revenue: ")));
                summary.Inlines.Add(new Run($"£{TotalWeekendRevenue:N2}\n"));
                summary.Inlines.Add(new Bold(new Run("Best Day: ")));
                summary.Inlines.Add(new Run($"{BestDay} (£{BestDayRevenue:N2})\n\n"));
                summary.FontSize = 14;
                document.Blocks.Add(summary);

                // Day Comparison
                Paragraph comparison = new Paragraph();
                comparison.Inlines.Add(new Bold(new Run("Day-by-Day Breakdown:\n")));
                comparison.Inlines.Add(new Run($"Saturday: £{SaturdayRevenue:N2}\n"));
                comparison.Inlines.Add(new Run($"Sunday: £{SundayRevenue:N2}\n"));
                comparison.Inlines.Add(new Run($"{RevenueComparison}\n\n"));
                comparison.FontSize = 14;
                document.Blocks.Add(comparison);

                // Channel Breakdown
                Paragraph channels = new Paragraph();
                channels.Inlines.Add(new Bold(new Run("Revenue by Channel:\n")));
                channels.Inlines.Add(new Run($"JustEat: £{JustEatTotal:N2}\n"));
                channels.Inlines.Add(new Run($"Uber: £{UberTotal:N2}\n"));
                channels.Inlines.Add(new Run($"Deliveroo: £{DeliverooTotal:N2}\n"));
                channels.Inlines.Add(new Run($"InHouse: £{InHouseTotal:N2}\n"));
                channels.FontSize = 14;
                document.Blocks.Add(channels);

                // Print Dialog
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    IDocumentPaginatorSource idpSource = document;
                    printDialog.PrintDocument(idpSource.DocumentPaginator, "Weekend Sales Report");
                    MessageBox.Show("Print job sent successfully!", "Print", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Print failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}