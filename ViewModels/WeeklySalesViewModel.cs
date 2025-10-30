using SalesTrackingSystem.Commands;
using SalesTrackingSystem.Data;  // ✅ Add this for AppDbContext
using SalesTrackingSystem.Models;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity; // ✅ Needed for DbFunctions
using System.Linq;
using System.Windows.Input;

namespace SalesTrackingSystem.ViewModels
{
    public class WeeklySalesViewModel : BaseViewModel
    {
        private readonly AppDbContext _context; // ✅ Add this

        public WeeklySalesViewModel()
        {
            _context = new AppDbContext(); // ✅ Initialize DB connection
            SelectedDate = DateTime.Today;
            LoadWeekCommand = new RelayCommand(o => LoadWeek());
            LoadWeek();
        }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set { _selectedDate = value; OnPropertyChanged(); }
        }

        private ObservableCollection<DaySummary> _weeklySummary = new ObservableCollection<DaySummary>();
        public ObservableCollection<DaySummary> WeeklySummary
        {
            get => _weeklySummary;
            set { _weeklySummary = value; OnPropertyChanged(); }
        }

        public ICommand LoadWeekCommand { get; }
        public DateTime WeekStart { get; private set; }

        public void LoadWeek()
        {
            if (!SelectedDate.HasValue) return;

            var selected = SelectedDate.Value.Date;
            int deltaToMonday = (7 + ((int)selected.DayOfWeek - (int)DayOfWeek.Monday)) % 7;
            WeekStart = selected.AddDays(-deltaToMonday);
            var weekEnd = WeekStart.AddDays(6);

            // ✅ Fetch data directly from database for the selected week
            var salesInWeek = _context.Sales
                .Where(s => DbFunctions.TruncateTime(s.Date) >= WeekStart &&
                            DbFunctions.TruncateTime(s.Date) <= weekEnd)
                .ToList();

            var list = new ObservableCollection<DaySummary>();
            for (int i = 0; i < 7; i++)
            {
                var d = WeekStart.AddDays(i);
                var salesOfDay = salesInWeek.Where(s => s.Date.Date == d).ToList();

                var totalOrders = salesOfDay.Sum(s => s.TotalOrders);
                var totalAmount = salesOfDay.Sum(s => s.TotalAmount);

                list.Add(new DaySummary
                {
                    Day = d.ToString("dddd"),
                    Date = d.ToString("dd/MM/yyyy"),
                    TotalOrders = (int)totalOrders,
                    TotalAmount = totalAmount
                });
            }

            WeeklySummary = list;
        }
    }

    public class DaySummary
    {
        public string Day { get; set; }
        public string Date { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
