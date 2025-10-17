using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesTrackingSystem.Models
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Column(TypeName = "decimal")]
        public decimal HourlyRate { get; set; }

        public DateTime Date { get; set; }

        [Column(TypeName = "decimal")]
        public decimal HoursWorked { get; set; }

        // Optional — just for convenience
        [NotMapped]
        public decimal TotalPay => HourlyRate * HoursWorked;
    }
}
