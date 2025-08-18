using SalesTrackingSystem.Models;
using SalesTrackingSystem.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SalesTrackingSystem.ViewModels
{
    public class SalesViewModel : INotifyPropertyChanged
    {
        // notify system
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _ordersJustEatInput;
        public int OrdersJustEatInput
        {
            get => _ordersJustEatInput;
            set { _ordersJustEatInput = value; OnPropertyChanged(nameof(OrdersJustEatInput)); }
        }

        private int _ordersUberInput;
        public int OrdersUberInput
        {
            get => _ordersUberInput;
            set { _ordersUberInput = value; OnPropertyChanged(nameof(OrdersUberInput)); }
        }

        private int _ordersDeliverooInput;
        public int OrdersDeliverooInput
        {
            get => _ordersDeliverooInput;
            set { _ordersDeliverooInput = value; OnPropertyChanged(nameof(OrdersDeliverooInput)); }
        }

        private decimal _amountJustEatInput;
        public decimal AmountJustEatInput
        {
            get => _amountJustEatInput;
            set { _amountJustEatInput = value; OnPropertyChanged(nameof(AmountJustEatInput)); }
        }

        private decimal _amountUberInput;
        public decimal AmountUberInput
        {
            get => _amountUberInput;
            set { _amountUberInput = value; OnPropertyChanged(nameof(AmountUberInput)); }
        }

        private decimal _amountDeliverooInput;
        public decimal AmountDeliverooInput
        {
            get => _amountDeliverooInput;
            set { _amountDeliverooInput = value; OnPropertyChanged(nameof(AmountDeliverooInput)); }
        }

        public ObservableCollection<SaleRecord> Sales { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand OpenWeeklySalesCommand { get; }

        public SalesViewModel()
        {
            Sales = new ObservableCollection<SaleRecord>();

            SaveCommand = new RelayCommand(Save);
            DeleteCommand = new RelayCommand(Delete);
            OpenWeeklySalesCommand = new RelayCommand(OpenWeeklySales);

            // start with zeros
            OrdersJustEatInput = 0;
            OrdersUberInput = 0;
            OrdersDeliverooInput = 0;
            AmountJustEatInput = 0;
            AmountUberInput = 0;
            AmountDeliverooInput = 0;
        }

        private void Save()
        {
            var newSale = new SaleRecord
            {
                Date = DateTime.Now,
                OrdersJustEat = OrdersJustEatInput,
                AmountJustEat = AmountJustEatInput,
                OrdersUber = OrdersUberInput,
                AmountUber = AmountUberInput,
                OrdersDeliveroo = OrdersDeliverooInput,
                AmountDeliveroo = AmountDeliverooInput
            };

            Sales.Add(newSale);

            // reset values (UI updates now because of INotifyPropertyChanged)
            OrdersJustEatInput = 0;
            OrdersUberInput = 0;
            OrdersDeliverooInput = 0;
            AmountJustEatInput = 0;
            AmountUberInput = 0;
            AmountDeliverooInput = 0;

            MessageBox.Show("Sale saved successfully!", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Delete(object parameter)
        {
            if (parameter is SaleRecord record && Sales.Contains(record))
            {
                Sales.Remove(record);
                MessageBox.Show("Sale deleted successfully!", "Deleted",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OpenWeeklySales()
        {
            var window = new WeeklySalesWindow(new ObservableCollection<SaleRecord>(Sales));
            window.ShowDialog();
        }
    }
}
