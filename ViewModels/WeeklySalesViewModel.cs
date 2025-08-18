using SalesTrackingSystem.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SalesTrackingSystem.ViewModels
{
    public class WeeklySalesViewModel : BaseViewModel
    {
        public DateTime? SelectedDate { get; set; } = DateTime.Today;

        private ObservableCollection<DaySummary> _weeklySummary;
        public ObservableCollection<DaySummary> WeeklySummary
        {
            get => _weeklySummary;
            set { _weeklySummary = value; OnPropertyChanged(nameof(WeeklySummary)); }
        }

        public ICommand LoadWeekCommand { get; }

        private readonly ObservableCollection<SaleRecord> _allSales;

        public WeeklySalesViewModel(ObservableCollection<SaleRecord> sales)
        {
            _allSales = sales;
            LoadWeekCommand = new RelayCommand(LoadWeek);
            LoadWeek();
        }

        private void LoadWeek()
        {
            if (!SelectedDate.HasValue) return;

            var monday = SelectedDate.Value.AddDays(-(int)SelectedDate.Value.DayOfWeek + (int)DayOfWeek.Monday);
            WeeklySummary = new ObservableCollection<DaySummary>();

            for (int i = 0; i < 7; i++)
            {
                var date = monday.AddDays(i);
                var salesOfDay = _allSales.Where(s => s.Date.Date == date.Date).ToList();

                WeeklySummary.Add(new DaySummary
                {
                    Day = date.ToString("dddd"),
                    Date = date.ToString("dd/MM/yyyy"),
                    TotalOrders = salesOfDay.Sum(s => s.TotalOrders),
                    TotalAmount = salesOfDay.Sum(s => s.TotalAmount)
                });
            }
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
