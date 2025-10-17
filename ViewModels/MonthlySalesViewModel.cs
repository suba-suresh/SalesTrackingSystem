using SalesTrackingSystem.Commands;
using SalesTrackingSystem.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SalesTrackingSystem.ViewModels
{
    public class MonthlySalesViewModel : BaseViewModel
    {
        private readonly ObservableCollection<SaleRecord> _allSales;

        public MonthlySalesViewModel(ObservableCollection<SaleRecord> sales)
        {
            _allSales = sales ?? new ObservableCollection<SaleRecord>();
            SelectedDate = DateTime.Today;
            LoadMonthCommand = new RelayCommand(o => LoadMonth());
            LoadMonth();
        }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set { _selectedDate = value; OnPropertyChanged(); }
        }

        private ObservableCollection<WeeklyBreakdownItem> _weeklyBreakdown = new ObservableCollection<WeeklyBreakdownItem>();
        public ObservableCollection<WeeklyBreakdownItem> WeeklyBreakdown
        {
            get => _weeklyBreakdown;
            set { _weeklyBreakdown = value; OnPropertyChanged(); }
        }

        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageDailySales { get; set; }
        public string HighestDayDisplay { get; set; }

        public ICommand LoadMonthCommand { get; }

        public void LoadMonth()
        {
            if (!SelectedDate.HasValue) return;

            var year = SelectedDate.Value.Year;
            var month = SelectedDate.Value.Month;
            var firstDay = new DateTime(year, month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            // Filter only current month sales
            var monthSales = _allSales.Where(s => s.Date.Year == year && s.Date.Month == month).ToList();

            // --- Aggregate totals ---
            TotalOrders = (int)monthSales.Sum(s => s.TotalOrders);
            TotalRevenue = monthSales.Sum(s => s.TotalAmount);
            AverageDailySales = monthSales.GroupBy(s => s.Date.Date).Any()
                ? monthSales.GroupBy(s => s.Date.Date).Average(g => g.Sum(s => s.TotalAmount))
                : 0;

            var bestDay = monthSales
                .GroupBy(s => s.Date.Date)
                .Select(g => new { Date = g.Key, Amount = g.Sum(s => s.TotalAmount) })
                .OrderByDescending(x => x.Amount)
                .FirstOrDefault();
            HighestDayDisplay = bestDay == null ? "—" : $"{bestDay.Date:dd MMM}: £{bestDay.Amount:F2}";

            // --- Group by week (1–4) ---
            var list = new ObservableCollection<WeeklyBreakdownItem>();
            int weekNumber = 1;
            DateTime weekStart = firstDay;
            while (weekStart <= lastDay)
            {
                var weekEnd = weekStart.AddDays(6);
                if (weekEnd > lastDay) weekEnd = lastDay;

                var salesOfWeek = monthSales.Where(s => s.Date.Date >= weekStart && s.Date.Date <= weekEnd).ToList();
                var weekOrders = (int)salesOfWeek.Sum(s => s.TotalOrders);
                var weekAmount = salesOfWeek.Sum(s => s.TotalAmount);

                list.Add(new WeeklyBreakdownItem
                {
                    WeekName = $"Week {weekNumber}",
                    StartDate = weekStart.ToString("dd MMM"),
                    EndDate = weekEnd.ToString("dd MMM"),
                    TotalOrders = weekOrders,
                    TotalAmount = weekAmount
                });

                weekNumber++;
                weekStart = weekEnd.AddDays(1);
            }

            WeeklyBreakdown = list;
            OnPropertyChanged(nameof(TotalOrders));
            OnPropertyChanged(nameof(TotalRevenue));
            OnPropertyChanged(nameof(AverageDailySales));
            OnPropertyChanged(nameof(HighestDayDisplay));
        }
    }

    public class WeeklyBreakdownItem
    {
        public string WeekName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
