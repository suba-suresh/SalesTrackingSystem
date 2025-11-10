using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace SalesTrackingSystem.Models
{
    [Table("Salaries")]
    public class Salary : INotifyPropertyChanged
    {
        private int _id;
        private int? _employeeId;
        private decimal _hourlyRate;
        private decimal _hoursWorked;
        private decimal _totalPay;
        private DateTime _payDate;
        private string _notes;
        

        [Key]
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        [ForeignKey("Employee")]
        public int? EmployeeId
        {
            get => _employeeId;
            set { _employeeId = value; OnPropertyChanged(nameof(EmployeeId)); }
        }

        public virtual Employee Employee { get; set; }

      

        [Column(TypeName = "decimal")]
        public decimal HourlyRate
        {
            get => _hourlyRate;
            set { _hourlyRate = value; OnPropertyChanged(); UpdateTotal(); }
        }

        [Column(TypeName = "decimal")]
        public decimal HoursWorked
        {
            get => _hoursWorked;
            set { _hoursWorked = value; OnPropertyChanged(); UpdateTotal(); }
        }

        [Column(TypeName = "decimal")]
        public decimal TotalPay
        {
            get => _totalPay;
            set { _totalPay = value; OnPropertyChanged(); }
        }

        public DateTime PayDate
        {
            get => _payDate;
            set { _payDate = value; OnPropertyChanged(); }
        }

        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(); }
        }

 

        private void UpdateTotal()
        {
            TotalPay = HourlyRate * HoursWorked;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}