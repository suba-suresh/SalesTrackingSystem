using SalesTrackingSystem.Data;
using SalesTrackingSystem.Models;
using SalesTrackingSystem.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SalesTrackingSystem.ViewModels
{
    public class ExpenseViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        private ObservableCollection<Expense> _expenses;
        public ObservableCollection<Expense> Expenses
        {
            get => _expenses;
            set { _expenses = value; OnPropertyChanged(); RefreshTotals(); }
        }

        private Expense _newExpense;
        public Expense NewExpense
        {
            get => _newExpense;
            set { _newExpense = value; OnPropertyChanged(); }
        }

        private Expense _selectedExpense;
        public Expense SelectedExpense
        {
            get => _selectedExpense;
            set { _selectedExpense = value; OnPropertyChanged(); }
        }

        // Totals
        public decimal TotalCash => Expenses?.Where(x => x.PaymentType == "Cash").Sum(x => x.Amount) ?? 0;
        public decimal TotalCard => Expenses?.Where(x => x.PaymentType == "Card").Sum(x => x.Amount) ?? 0;
        public decimal TotalAmount => Expenses?.Sum(x => x.Amount) ?? 0;

        // Commands
        public ICommand AddExpenseCommand { get; }
        public ICommand DeleteExpenseCommand { get; }
        public ICommand ClearCommand { get; }

        public ExpenseViewModel()
        {
            _context = new AppDbContext();
            LoadExpenses();

            NewExpense = new Expense { Date = DateTime.Today, PaymentType = "Cash" };

            AddExpenseCommand = new RelayCommand(_ => AddExpense(), _ => CanAddExpense());
            DeleteExpenseCommand = new RelayCommand(_ => DeleteExpense(), _ => SelectedExpense != null);
            ClearCommand = new RelayCommand(_ => ClearForm());
        }

        private void LoadExpenses()
        {
            try
            {
                var expenses = _context.Expenses.OrderByDescending(e => e.Date).ToList();
                Expenses = new ObservableCollection<Expense>(expenses);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading expenses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Expenses = new ObservableCollection<Expense>();
            }
        }

        private bool CanAddExpense()
        {
            return NewExpense != null
                   && !string.IsNullOrWhiteSpace(NewExpense.Category)
                   && !string.IsNullOrWhiteSpace(NewExpense.PaymentType)
                   && NewExpense.Amount > 0;
        }

        private void AddExpense()
        {
            try
            {
                var expense = new Expense
                {
                    Date = NewExpense.Date,
                    Category = NewExpense.Category.Trim(),
                    PaymentType = NewExpense.PaymentType,
                    Amount = NewExpense.Amount
                };

                _context.Expenses.Add(expense);
                _context.SaveChanges();

                Expenses.Insert(0, expense);
                RefreshTotals();

                MessageBox.Show("Expense added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding expense: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteExpense()
        {
            if (SelectedExpense == null)
            {
                MessageBox.Show("Please select an expense to delete.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete this expense?\n\nCategory: {SelectedExpense.Category}\nAmount: £{SelectedExpense.Amount:F2}",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                _context.Expenses.Remove(SelectedExpense);
                _context.SaveChanges();

                Expenses.Remove(SelectedExpense);
                RefreshTotals();

                MessageBox.Show("Expense deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                SelectedExpense = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting expense: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            NewExpense = new Expense { Date = DateTime.Today, PaymentType = "Cash", Amount = 0 };
            SelectedExpense = null;
        }

        private void RefreshTotals()
        {
            OnPropertyChanged(nameof(TotalCash));
            OnPropertyChanged(nameof(TotalCard));
            OnPropertyChanged(nameof(TotalAmount));
        }
    }
}