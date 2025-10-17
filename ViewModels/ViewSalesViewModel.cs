using SalesTrackingSystem.Data;
using SalesTrackingSystem.Models;
using SalesTrackingSystem.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Data.Entity; // for Include and DbFunctions

namespace SalesTrackingSystem.ViewModels
{
    public class ViewSalesViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        public ObservableCollection<SaleRecord> FilteredSales { get; set; } = new ObservableCollection<SaleRecord>();

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set { _selectedDate = value; OnPropertyChanged(); }
        }

        private int _totalOrders;
        public int TotalOrders
        {
            get => _totalOrders;
            set { _totalOrders = value; OnPropertyChanged(); }
        }

        private decimal _totalRevenue;
        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set { _totalRevenue = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand DailyViewCommand { get; }
        public ICommand MonthlyViewCommand { get; }
        public ICommand YearlyViewCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand PrintCommand { get; }

        public ViewSalesViewModel()
        {
            _context = new AppDbContext();
            DailyViewCommand = new RelayCommand(o => LoadDailyData());
            MonthlyViewCommand = new RelayCommand(o => LoadMonthlyData());
            YearlyViewCommand = new RelayCommand(o => LoadYearlyData());
            RefreshCommand = new RelayCommand(o => LoadCurrentView());
            PrintCommand = new RelayCommand(o => PrintGrid());

            // default
            LoadDailyData();
        }

        private void LoadCurrentView()
        {
            // Re-run the current view (default daily)
            LoadDailyData();
        }

        private void LoadDailyData()
        {
            // Use DbFunctions.TruncateTime for date compare in EF6
            var records = _context.Sales
                .Where(s => DbFunctions.TruncateTime(s.Date) == DbFunctions.TruncateTime(SelectedDate))
                .OrderByDescending(s => s.Date)
                .ToList();

            ApplyLoadedRecords(records);
        }

        private void LoadMonthlyData()
        {
            var y = SelectedDate.Year;
            var m = SelectedDate.Month;

            var records = _context.Sales
                .Where(s => s.Date.Year == y && s.Date.Month == m)
                .OrderByDescending(s => s.Date)
                .ToList();

            ApplyLoadedRecords(records);
        }

        private void LoadYearlyData()
        {
            var y = SelectedDate.Year;

            var records = _context.Sales
                .Where(s => s.Date.Year == y)
                .OrderByDescending(s => s.Date)
                .ToList();

            ApplyLoadedRecords(records);
        }

        private void ApplyLoadedRecords(System.Collections.Generic.List<SaleRecord> records)
        {
            FilteredSales.Clear();
            foreach (var rec in records)
                FilteredSales.Add(rec);

            TotalOrders = records.Sum(r => (int)(r.OrdersJustEat + r.OrdersUber + r.OrdersDeliveroo + r.OrdersInHouse));
            TotalRevenue = records.Sum(r => (r.AmountJustEat + r.AmountUber + r.AmountDeliveroo + r.AmountInhouse));
        }

        private void PrintGrid()
        {
            MessageBox.Show("Printing current grid data...", "Print", MessageBoxButton.OK, MessageBoxImage.Information);
            // Actual printing handled in View (ViewSalesWindow.Print_Click)
        }
    }
}
