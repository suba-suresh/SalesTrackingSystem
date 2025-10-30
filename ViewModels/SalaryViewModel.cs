using SalesTrackingSystem.Commands;
using SalesTrackingSystem.Data;
using SalesTrackingSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace SalesTrackingSystem.ViewModels
{
    public class SalaryViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        public ObservableCollection<Salary> Salaries { get; set; } = new ObservableCollection<Salary>();

        private Salary _selectedSalary;
        public Salary SelectedSalary
        {
            get => _selectedSalary;
            set { _selectedSalary = value; OnPropertyChanged(); LoadSelectedSalary(); }
        }

        private string _employeeName;
        public string EmployeeName
        {
            get => _employeeName;
            set { _employeeName = value; OnPropertyChanged(); }
        }

        private decimal _hourlyRate;
        public decimal HourlyRate
        {
            get => _hourlyRate;
            set { _hourlyRate = value; OnPropertyChanged(); UpdateTotal(); }
        }

        private decimal _hoursWorked;
        public decimal HoursWorked
        {
            get => _hoursWorked;
            set { _hoursWorked = value; OnPropertyChanged(); UpdateTotal(); }
        }

        private decimal _totalPay;
        public decimal TotalPay
        {
            get => _totalPay;
            set { _totalPay = value; OnPropertyChanged(); }
        }

        private DateTime _payDate = DateTime.Now;
        public DateTime PayDate
        {
            get => _payDate;
            set { _payDate = value; OnPropertyChanged(); }
        }

        private string _notes;
        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand PrintCommand { get; }

        public SalaryViewModel()
        {
            _context = new AppDbContext();
            LoadAllSalaries();

            AddCommand = new RelayCommand(_ => AddSalary());
            UpdateCommand = new RelayCommand(_ => UpdateSalary(), _ => SelectedSalary != null);
            DeleteCommand = new RelayCommand(_ => DeleteSalary(), _ => SelectedSalary != null);
            ClearCommand = new RelayCommand(_ => ClearForm());
            PrintCommand = new RelayCommand(_ => PrintSalaryReport());
        }

        // ===============================
        // LOAD / REFRESH
        // ===============================
        private void LoadAllSalaries()
        {
            Salaries.Clear();
            var data = _context.Salaries
                .Include("Employee")
                .OrderByDescending(s => s.PayDate)
                .ToList();

            foreach (var s in data)
                Salaries.Add(s);
        }

        // ===============================
        // ADD
        // ===============================
        private void AddSalary()
        {
            if (string.IsNullOrWhiteSpace(EmployeeName))
            {
                MessageBox.Show("Please enter an employee name.", "Missing Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_context.Employees.Any(e => e.Name.ToLower() == EmployeeName.ToLower()))
            {
                MessageBox.Show($"Employee name '{EmployeeName}' already exists. Please use a different name.", "Duplicate Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (HourlyRate <= 0 || HoursWorked <= 0)
            {
                MessageBox.Show("Hourly rate and hours worked must be greater than zero.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var employee = new Employee
            {
                Name = EmployeeName,
                HourlyRate = HourlyRate,
                Date = DateTime.Now,
                HoursWorked = HoursWorked
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();

            var newSalary = new Salary
            {
                EmployeeId = employee.Id,
                HourlyRate = HourlyRate,
                HoursWorked = HoursWorked,
                TotalPay = TotalPay,
                PayDate = PayDate,
                Notes = Notes
            };

            _context.Salaries.Add(newSalary);
            _context.SaveChanges();

            newSalary.Employee = employee;
            Salaries.Insert(0, newSalary);

            MessageBox.Show("Salary added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            ClearForm();
        }

        // ===============================
        // UPDATE
        // ===============================
        private void UpdateSalary()
        {
            if (SelectedSalary == null)
            {
                MessageBox.Show("Please select a record to update.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedSalary.HourlyRate = HourlyRate;
            SelectedSalary.HoursWorked = HoursWorked;
            SelectedSalary.TotalPay = TotalPay;
            SelectedSalary.PayDate = PayDate;
            SelectedSalary.Notes = Notes;

            _context.Entry(SelectedSalary).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();

            MessageBox.Show("Salary updated successfully!", "Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadAllSalaries();
        }

        // ===============================
        // DELETE
        // ===============================
        private void DeleteSalary()
        {
            if (SelectedSalary == null) return;

            var confirm = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            _context.Salaries.Remove(SelectedSalary);
            _context.SaveChanges();
            Salaries.Remove(SelectedSalary);
            ClearForm();

            MessageBox.Show("Salary deleted successfully!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ===============================
        // FORM / CALCULATIONS
        // ===============================
        private void ClearForm()
        {
            EmployeeName = "";
            HourlyRate = 0;
            HoursWorked = 0;
            TotalPay = 0;
            PayDate = DateTime.Now;
            Notes = "";
            SelectedSalary = null;
        }

        private void LoadSelectedSalary()
        {
            if (SelectedSalary == null) return;

            EmployeeName = SelectedSalary.Employee?.Name;
            HourlyRate = SelectedSalary.HourlyRate;
            HoursWorked = SelectedSalary.HoursWorked;
            TotalPay = SelectedSalary.TotalPay;
            PayDate = SelectedSalary.PayDate;
            Notes = SelectedSalary.Notes;
        }

        private void UpdateTotal()
        {
            TotalPay = HourlyRate * HoursWorked;
        }

        // ===============================
        // PRINT FUNCTION
        // ===============================
        private void PrintSalaryReport()
        {
            if (Salaries == null || !Salaries.Any())
            {
                MessageBox.Show("No salary data to print.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            FlowDocument fd = new FlowDocument();

            // Title
            Paragraph title = new Paragraph(new Run("Salary Report"))
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            fd.Blocks.Add(title);

            Paragraph date = new Paragraph(new Run($"Printed on: {DateTime.Now:dd MMM yyyy HH:mm}"))
            {
                FontSize = 12,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            fd.Blocks.Add(date);

            // Table
            Table table = new Table();
            fd.Blocks.Add(table);
            table.CellSpacing = 0;
            table.BorderBrush = Brushes.Black;
            table.BorderThickness = new Thickness(1);

            string[] headers = { "Employee", "Hourly Rate (£)", "Hours Worked", "Total Pay (£)", "Pay Date", "Notes" };
            foreach (string _ in headers)
                table.Columns.Add(new TableColumn());

            // Header row
            TableRow headerRow = new TableRow();
            foreach (var h in headers)
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run(h)))
                {
                    FontWeight = FontWeights.Bold,
                    Background = Brushes.LightGray,
                    Padding = new Thickness(4),
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.5)
                });

            TableRowGroup trg = new TableRowGroup();
            table.RowGroups.Add(trg);
            trg.Rows.Add(headerRow);

            // Data rows
            foreach (var s in Salaries)
            {
                TableRow row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(s.Employee?.Name ?? ""))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(s.HourlyRate.ToString("F2")))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(s.HoursWorked.ToString("F2")))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(s.TotalPay.ToString("F2")))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(s.PayDate.ToString("dd/MM/yyyy")))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(s.Notes ?? ""))));
                foreach (var c in row.Cells)
                {
                    c.Padding = new Thickness(4);
                    c.BorderBrush = Brushes.Black;
                    c.BorderThickness = new Thickness(0.3);
                }
                trg.Rows.Add(row);
            }

            // Total row
            decimal totalAll = Salaries.Sum(s => s.TotalPay);
            TableRow totalRow = new TableRow();
            totalRow.Cells.Add(new TableCell(new Paragraph(new Run("TOTAL")))
            {
                FontWeight = FontWeights.Bold,
                Padding = new Thickness(4),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0.3),
                ColumnSpan = 3
            });
            totalRow.Cells.Add(new TableCell(new Paragraph(new Run(totalAll.ToString("F2"))))
            {
                FontWeight = FontWeights.Bold,
                Padding = new Thickness(4),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0.3)
            });
            trg.Rows.Add(totalRow);

            fd.FontFamily = new FontFamily("Segoe UI");
            fd.FontSize = 12;
            fd.PagePadding = new Thickness(50);

            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
                pd.PrintDocument(((IDocumentPaginatorSource)fd).DocumentPaginator, "Salary Report");
        }
    }
}
