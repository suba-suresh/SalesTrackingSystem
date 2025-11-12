using SalesTrackingSystem.Commands;
using SalesTrackingSystem.Data;
using SalesTrackingSystem.Models;
using SalesTrackingSystem.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        // ===== NEW: Loading Indicator =====
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { SetProperty(ref _isLoading, value); }
        }

        private DateTime _dateInput = DateTime.Today;
        public DateTime DateInput
        {
            get => _dateInput;
            set { _dateInput = value; OnPropertyChanged(); UpdateButtonStates(); }
        }

        // ===== Order Inputs (Hidden in UI, but kept for DB compatibility) =====
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

        // ===== Amount Inputs (Visible in UI) =====
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

      

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand EditRowCommand { get; }
        public ICommand ViewSalesCommand { get; }
        public ICommand BackCommand { get; } // NEW: Back navigation

        // Button state properties
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

            // NEW: Back Command - Navigate to HomeWindow
            BackCommand = new RelayCommand(o => NavigateBack());

            // Load data ASYNCHRONOUSLY (CR2: Performance improvement)
            LoadAllSalesFromDbAsync();
            UpdateButtonStates();
        }

      
        private async void LoadAllSalesFromDbAsync()
        {
            IsLoading = true;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Perform database query on background thread
                await Task.Run(() =>
                {
                    _allSales = _context.Sales
                                       .OrderByDescending(s => s.Date)
                                       .Take(1000) // Limit to last 1000 records for performance
                                       .ToList();
                });

                stopwatch.Stop();
                Debug.WriteLine($"[PERFORMANCE] Sales data loaded in {stopwatch.ElapsedMilliseconds}ms ({_allSales.Count} records)");

                // Update UI on UI thread
                Sales = new ObservableCollection<SaleRecord>(_allSales);
                OnPropertyChanged(nameof(Sales));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sales: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

      
        private bool InputsHaveValues()
        {
            // Only check amounts since orders are hidden from UI
            return (AmountJustEatInput + AmountUberInput + AmountDeliverooInput + AmountInHouseInput) > 0;
        }

       
        private void SaveRecord()
        {
            if (!InputsHaveValues())
            {
                MessageBox.Show("Please enter amounts before saving.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newSale = new SaleRecord
            {
                Date = DateInput.Date,
                // Orders kept at 0 since they're hidden from UI (CR4)
                OrdersJustEat = 0,
                OrdersUber = 0,
                OrdersDeliveroo = 0,
                OrdersInHouse = 0,
                // Amounts from user input
                AmountJustEat = AmountJustEatInput,
                AmountUber = AmountUberInput,
                AmountDeliveroo = AmountDeliverooInput,
                AmountInhouse = AmountInHouseInput
            };

            try
            {
                _context.Sales.Add(newSale);
                _context.SaveChanges();

                // update local collections
                _allSales.Insert(0, newSale);
                Sales.Insert(0, newSale);

                MessageBox.Show($"Sale saved successfully for {DateInput.Date:dd-MM-yyyy}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void UpdateRecord()
        {
            if (SelectedSale == null)
            {
                MessageBox.Show("Please select a sale to update.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var updatedDate = DateInput.Date;
                var saleId = SelectedSale.Id;

                // Update sale
                SelectedSale.Date = DateInput.Date;
                SelectedSale.OrdersJustEat = 0;
                SelectedSale.OrdersUber = 0;
                SelectedSale.OrdersDeliveroo = 0;
                SelectedSale.OrdersInHouse = 0;
                SelectedSale.AmountJustEat = AmountJustEatInput;
                SelectedSale.AmountUber = AmountUberInput;
                SelectedSale.AmountDeliveroo = AmountDeliverooInput;
                SelectedSale.AmountInhouse = AmountInHouseInput;

                _context.Entry(SelectedSale).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();

                // ✅ Re-query to refresh left and right side
                LoadAllSales();
                SelectedSale = _context.Sales.FirstOrDefault(s => s.Id == saleId);
                OnPropertyChanged(nameof(SelectedSale));

                MessageBox.Show($"Record updated successfully for {updatedDate:dd-MM-yyyy}",
                    "Updated", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void DeleteRecord(object parameter)
        {
            SaleRecord toDelete = null;

            if (parameter is SaleRecord row)
                toDelete = row;
            else if (SelectedSale != null)
                toDelete = SelectedSale;

            if (toDelete == null) return;

            // ✅ Store values BEFORE deletion
            var deletedDate = toDelete.Date;
            var deletedId = toDelete.Id;

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete this record?\n\nDate: {deletedDate:dd-MM-yyyy}\nTotal: £{toDelete.TotalAmount:F2}",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                _context.Sales.Remove(toDelete);
                _context.SaveChanges();

                // Remove from local collections
                _allSales.RemoveAll(s => s.Id == deletedId);
                var existing = Sales.FirstOrDefault(s => s.Id == deletedId);
                if (existing != null)
                    Sales.Remove(existing);

                // ✅ Show success message
                MessageBox.Show($"Record deleted successfully for {deletedDate:dd-MM-yyyy}", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);

                // ✅ ALWAYS clear form after deletion regardless of which record was deleted
                ClearForm();

                // ✅ Ensure SelectedSale is null
                SelectedSale = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Delete failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
            // Orders hidden from UI, so we don't load them
            OrdersJustEatInput = 0;
            OrdersUberInput = 0;
            OrdersDeliverooInput = 0;
            OrdersInHouseInput = 0;
            // Load amounts
            AmountJustEatInput = SelectedSale.AmountJustEat;
            AmountUberInput = SelectedSale.AmountUber;
            AmountDeliverooInput = SelectedSale.AmountDeliveroo;
            AmountInHouseInput = SelectedSale.AmountInhouse;

            OnPropertyChanged(nameof(DateInput));
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
            CommandManager.InvalidateRequerySuggested();
        }

       
        private void NavigateBack()
        {
            var currentWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (currentWindow != null)
            {
                var homeWindow = new HomeWindow();
                homeWindow.Show();
                currentWindow.Close();
            }
        }

        // Add this method to fix CS0103: The name 'LoadAllSales' does not exist in the current context
        private void LoadAllSales()
        {
            // Load sales from _allSales into Sales ObservableCollection
            Sales = new ObservableCollection<SaleRecord>(_allSales);
            OnPropertyChanged(nameof(Sales));
        }
    }
}