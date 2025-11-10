using SalesTrackingSystem.Commands;
using SalesTrackingSystem.Data;
using SalesTrackingSystem.Models;
using SalesTrackingSystem.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SalesTrackingSystem.ViewModels
{
    public class SalesViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        public ObservableCollection<SaleRecord> Sales { get; set; } = new ObservableCollection<SaleRecord>();

        private SaleRecord _selectedSale;
        public SaleRecord SelectedSale
        {
            get => _selectedSale;
            set
            {
                _selectedSale = value;
                OnPropertyChanged();
                LoadSelectedSale();
                UpdateButtonStates();
            }
        }

       
        private DateTime _dateInput = DateTime.Today;
        public DateTime DateInput
        {
            get => _dateInput;
            set { _dateInput = value; OnPropertyChanged(); UpdateButtonStates(); }
        }

        private decimal _ordersJustEatInput;
        public decimal OrdersJustEatInput
        {
            get => _ordersJustEatInput;
            set { _ordersJustEatInput = value; OnPropertyChanged(); UpdateButtonStates(); }
        }

        private decimal _ordersUberInput;
        public decimal OrdersUberInput
        {
            get => _ordersUberInput;
            set { _ordersUberInput = value; OnPropertyChanged(); UpdateButtonStates(); }
        }

        private decimal _ordersDeliverooInput;
        public decimal OrdersDeliverooInput
        {
            get => _ordersDeliverooInput;
            set { _ordersDeliverooInput = value; OnPropertyChanged(); UpdateButtonStates(); }
        }

        private decimal _ordersInHouseInput;
        public decimal OrdersInHouseInput
        {
            get => _ordersInHouseInput;
            set { _ordersInHouseInput = value; OnPropertyChanged(); UpdateButtonStates(); }
        }

        private decimal _amountJustEatInput;
        public decimal AmountJustEatInput
        {
            get => _amountJustEatInput;
            set { _amountJustEatInput = value; OnPropertyChanged(); UpdateButtonStates(); }
        }

        private decimal _amountUberInput;
        public decimal AmountUberInput
        {
            get => _amountUberInput;
            set { _amountUberInput = value; OnPropertyChanged(); UpdateButtonStates(); }
        }

        private decimal _amountDeliverooInput;
        public decimal AmountDeliverooInput
        {
            get => _amountDeliverooInput;
            set { _amountDeliverooInput = value; OnPropertyChanged(); UpdateButtonStates(); }
        }

        private decimal _amountInHouseInput;
        public decimal AmountInHouseInput
        {
            get => _amountInHouseInput;
            set { _amountInHouseInput = value; OnPropertyChanged(); UpdateButtonStates(); }
        }

        
        private DateTime? _filterDate;
        public DateTime? FilterDate
        {
            get => _filterDate;
            set { _filterDate = value; OnPropertyChanged(); FilterSalesByDate(); }
        }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand EditRowCommand { get; }
        public ICommand ViewSalesCommand { get; }

        // Button state properties (bind to IsEnabled in XAML)
        private bool _canSave = true;
        public bool CanSave
        {
            get => _canSave;
            set { _canSave = value; OnPropertyChanged(); }
        }

        private bool _canUpdate;
        public bool CanUpdate
        {
            get => _canUpdate;
            set { _canUpdate = value; OnPropertyChanged(); }
        }

        private bool _canDelete;
        public bool CanDelete
        {
            get => _canDelete;
            set { _canDelete = value; OnPropertyChanged(); }
        }

       
        private List<SaleRecord> _allSales = new List<SaleRecord>();

        public SalesViewModel()
        {
            _context = new AppDbContext();

            // Commands
            SaveCommand = new RelayCommand(o => SaveRecord(), o => CanSave);
            UpdateCommand = new RelayCommand(o => UpdateRecord(), o => CanUpdate);
            DeleteCommand = new RelayCommand(o => DeleteRecord(o), o => CanDelete);
            ClearCommand = new RelayCommand(o => ClearForm());
            EditRowCommand = new RelayCommand(o => EditRow(o), o => o != null);

            ViewSalesCommand = new RelayCommand(o =>
            {
                var win = new ViewSalesWindow();
                win.Owner = System.Windows.Application.Current.MainWindow;
                win.ShowDialog();
            });

            // Load data
            LoadAllSalesFromDb();
            UpdateButtonStates();
        }

        private void LoadAllSalesFromDb()
        {
            _allSales = _context.Sales
                               .OrderByDescending(s => s.Date)
                               .ToList();

            Sales = new ObservableCollection<SaleRecord>(_allSales);
            OnPropertyChanged(nameof(Sales));
        }

        private void FilterSalesByDate()
        {
            if (Sales == null) return;
            Sales.Clear();
            IEnumerable<SaleRecord> filtered = _allSales;

            if (FilterDate.HasValue)
                filtered = _allSales.Where(s => s.Date.Date == FilterDate.Value.Date);

            foreach (var r in filtered.OrderByDescending(s => s.Date))
                Sales.Add(r);
        }

        private bool InputsHaveValues()
        {
            return (OrdersJustEatInput + OrdersUberInput + OrdersDeliverooInput + OrdersInHouseInput) > 0
                || (AmountJustEatInput + AmountUberInput + AmountDeliverooInput + AmountInHouseInput) > 0;
        }

        // Create
        private void SaveRecord()
        {
            if (!InputsHaveValues())
            {
                MessageBox.Show("Please enter some orders or amounts before saving.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newSale = new SaleRecord
            {
                Date = DateInput.Date,
                OrdersJustEat = OrdersJustEatInput,
                OrdersUber = OrdersUberInput,
                OrdersDeliveroo = OrdersDeliverooInput,
                OrdersInHouse = OrdersInHouseInput,
                AmountJustEat = AmountJustEatInput,
                AmountUber = AmountUberInput,
                AmountDeliveroo = AmountDeliverooInput,
                AmountInhouse = AmountInHouseInput
                // DO NOT assign TotalOrders/TotalAmount here if those are read-only computed properties
            };

            try
            {
                _context.Sales.Add(newSale);
                _context.SaveChanges();

                // update local collections
                _allSales.Insert(0, newSale);
                if (!FilterDate.HasValue || newSale.Date.Date == FilterDate.Value.Date)
                    Sales.Insert(0, newSale);

                MessageBox.Show("Sale saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Update
        private void UpdateRecord()
        {
            if (SelectedSale == null)
            {
                MessageBox.Show("Please select a sale to update.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                // copy inputs into SelectedSale
                SelectedSale.Date = DateInput.Date;
                SelectedSale.OrdersJustEat = OrdersJustEatInput;
                SelectedSale.OrdersUber = OrdersUberInput;
                SelectedSale.OrdersDeliveroo = OrdersDeliverooInput;
                SelectedSale.OrdersInHouse = OrdersInHouseInput;
                SelectedSale.AmountJustEat = AmountJustEatInput;
                SelectedSale.AmountUber = AmountUberInput;
                SelectedSale.AmountDeliveroo = AmountDeliverooInput;
                SelectedSale.AmountInhouse = AmountInHouseInput;

                // DO NOT set read-only computed properties (TotalOrders/TotalAmount)
                _context.Entry(SelectedSale).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();

                // refresh local lists
                var idx = _allSales.FindIndex(s => s.Id == SelectedSale.Id);
                if (idx >= 0) _allSales[idx] = SelectedSale;

                FilterSalesByDate();

                MessageBox.Show("Record updated successfully!", "Updated", MessageBoxButton.OK, MessageBoxImage.Information);

                
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Delete 
        private void DeleteRecord(object parameter)
        {
            SaleRecord toDelete = null;

            if (parameter is SaleRecord row) toDelete = row;
            else if (SelectedSale != null) toDelete = SelectedSale;

            if (toDelete == null) return;

            var confirm = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                _context.Sales.Remove(toDelete);
                _context.SaveChanges();

                _allSales.RemoveAll(s => s.Id == toDelete.Id);
                var existing = Sales.FirstOrDefault(s => s.Id == toDelete.Id);
                if (existing != null) Sales.Remove(existing);

                MessageBox.Show("Record deleted successfully!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);

                // clear selection/form if that record was selected
                if (SelectedSale != null && SelectedSale.Id == toDelete.Id)
                    ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Called when user clicks Edit row - sets the form inputs to that row
        private void EditRow(object parameter)
        {
            if (parameter is SaleRecord row)
            {
                SelectedSale = row;
            }
        }

        private void LoadSelectedSale()
        {
            if (SelectedSale == null) return;

            DateInput = SelectedSale.Date;
            OrdersJustEatInput = SelectedSale.OrdersJustEat;
            OrdersUberInput = SelectedSale.OrdersUber;
            OrdersDeliverooInput = SelectedSale.OrdersDeliveroo;
            OrdersInHouseInput = SelectedSale.OrdersInHouse;
            AmountJustEatInput = SelectedSale.AmountJustEat;
            AmountUberInput = SelectedSale.AmountUber;
            AmountDeliverooInput = SelectedSale.AmountDeliveroo;
            AmountInHouseInput = SelectedSale.AmountInhouse;

            OnPropertyChanged(nameof(DateInput));
            OnPropertyChanged(nameof(OrdersJustEatInput));
            OnPropertyChanged(nameof(OrdersUberInput));
            OnPropertyChanged(nameof(OrdersDeliverooInput));
            OnPropertyChanged(nameof(OrdersInHouseInput));
            OnPropertyChanged(nameof(AmountJustEatInput));
            OnPropertyChanged(nameof(AmountUberInput));
            OnPropertyChanged(nameof(AmountDeliverooInput));
            OnPropertyChanged(nameof(AmountInHouseInput));
        }

        private void ClearForm()
        {
            DateInput = DateTime.Today;
            OrdersJustEatInput = 0m;
            OrdersUberInput = 0m;
            OrdersDeliverooInput = 0m;
            OrdersInHouseInput = 0m;
            AmountJustEatInput = 0m;
            AmountUberInput = 0m;
            AmountDeliverooInput = 0m;
            AmountInHouseInput = 0m;
            SelectedSale = null;

            OnPropertyChanged(nameof(DateInput));
            OnPropertyChanged(nameof(OrdersJustEatInput));
            OnPropertyChanged(nameof(OrdersUberInput));
            OnPropertyChanged(nameof(OrdersDeliverooInput));
            OnPropertyChanged(nameof(OrdersInHouseInput));
            OnPropertyChanged(nameof(AmountJustEatInput));
            OnPropertyChanged(nameof(AmountUberInput));
            OnPropertyChanged(nameof(AmountDeliverooInput));
            OnPropertyChanged(nameof(AmountInHouseInput));

            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            if (SelectedSale != null)
            {
                CanSave = false;
                CanUpdate = true;
                CanDelete = true;
            }
            else
            {
                CanSave = true;
                CanUpdate = false;
                CanDelete = false;
            }
            // notify commands to re-query if they use command.CanExecute (optional)
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
