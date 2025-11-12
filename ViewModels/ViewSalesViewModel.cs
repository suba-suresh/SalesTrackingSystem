using SalesTrackingSystem.Data;
using SalesTrackingSystem.Models;
using SalesTrackingSystem.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Data.Entity;

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

     

        private decimal _totalRevenue;
        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set { _totalRevenue = value; OnPropertyChanged(); }
        }

        private decimal _averageDailySales;
        public decimal AverageDailySales
        {
            get => _averageDailySales;
            set { _averageDailySales = value; OnPropertyChanged(); }
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
            RefreshCommand = new RelayCommand(o => RefreshToToday());
            PrintCommand = new RelayCommand(o => PrintGrid());

            LoadDailyData(); // Default on startup
        }

        private void RefreshToToday()
        {
            SelectedDate = DateTime.Today;
            OnPropertyChanged(nameof(SelectedDate));
            LoadDailyData();
            MessageBox.Show($"Refreshed to current date: {SelectedDate:dd-MM-yyyy}",
                "Refreshed", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadDailyData()
        {
            try
            {
                var targetDate = SelectedDate.Date;
                var records = _context.Sales
                    .Where(s => DbFunctions.TruncateTime(s.Date) == targetDate)
                    .OrderByDescending(s => s.Date)
                    .ToList();

                ApplyLoadedRecords(records);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading daily data: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMonthlyData()
        {
            try
            {
                int year = SelectedDate.Year;
                int month = SelectedDate.Month;
                var records = _context.Sales
                    .Where(s => s.Date.Year == year && s.Date.Month == month)
                    .OrderByDescending(s => s.Date)
                    .ToList();

                ApplyLoadedRecords(records);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading monthly data: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadYearlyData()
        {
            try
            {
                int year = SelectedDate.Year;
                var records = _context.Sales
                    .Where(s => s.Date.Year == year)
                    .OrderByDescending(s => s.Date)
                    .ToList();

                ApplyLoadedRecords(records);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading yearly data: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyLoadedRecords(System.Collections.Generic.List<SaleRecord> records)
        {
            FilteredSales.Clear();
            foreach (var rec in records)
                FilteredSales.Add(rec);

           

            TotalRevenue = records.Any()
                ? records.Sum(r => (r.AmountJustEat + r.AmountUber + r.AmountDeliveroo + r.AmountInhouse))
                : 0;

            AverageDailySales = records.Any()
                ? records.Average(r => (r.AmountJustEat + r.AmountUber + r.AmountDeliveroo + r.AmountInhouse))
                : 0;

            OnPropertyChanged(nameof(FilteredSales));
            OnPropertyChanged(nameof(TotalRevenue));
            OnPropertyChanged(nameof(AverageDailySales));
        }

        private void PrintGrid()
        {
            MessageBox.Show("Printing current grid data...",
                "Print Report", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
