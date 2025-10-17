using SalesTrackingSystem.Models;
using SalesTrackingSystem.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace SalesTrackingSystem.ViewModels
{
    public class ExpenseViewModel : BaseViewModel
    {
        public ObservableCollection<Expense> Expenses { get; set; } = new ObservableCollection<Expense>();

        public ObservableCollection<PaymentMethod> PaymentMethods { get; } =
            new ObservableCollection<PaymentMethod>((PaymentMethod[])Enum.GetValues(typeof(PaymentMethod)));

        private Expense _newExpense = new Expense { Date = DateTime.Now.Date, PaymentType = PaymentMethod.Cash };
        public Expense NewExpense
        {
            get => _newExpense;
            set { _newExpense = value; OnPropertyChanged(); }
        }

        public ICommand AddExpenseCommand { get; }
        public ICommand DeleteExpenseCommand { get; }

        // Remove the unused field '_nextId' to fix CS0414 and avoid unnecessary code.
        // private int _nextId = 1;

        public ExpenseViewModel()
        {
            AddExpenseCommand = new RelayCommand(o => AddExpense(), o => CanAddExpense());
            DeleteExpenseCommand = new RelayCommand(o => DeleteExpense(o), o => o != null);

            // Sample data (optional)
            Expenses.Add(new Expense { Date = DateTime.Now.Date, Category = "Food", Amount = 25.00m, PaymentType = PaymentMethod.Cash });
        }

        private bool CanAddExpense()
        {
            return !string.IsNullOrWhiteSpace(NewExpense.Category) && NewExpense.Amount > 0;
        }

        private void AddExpense()
        {
            var e = new Expense
            {
               
                Date = NewExpense.Date,
                Category = NewExpense.Category,
                Amount = NewExpense.Amount,
                PaymentType = NewExpense.PaymentType
            };

            Expenses.Add(e);

            // Reset new expense
            NewExpense = new Expense { Date = DateTime.Now.Date, PaymentType = PaymentMethod.Cash };

            OnPropertyChanged(nameof(Expenses));
        }

        private void DeleteExpense(object parameter)
        {
            if (parameter is Expense ex && Expenses.Contains(ex))
            {
                Expenses.Remove(ex);
                OnPropertyChanged(nameof(Expenses));
            }
        }

        public decimal TotalExpenses => Expenses.Sum(x => x.Amount);
    }
}
