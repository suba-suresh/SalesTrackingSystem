using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesTrackingSystem.Models
{
    [Table("Expenses")]
    public class Expense
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string PaymentType { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Amount { get; set; }
    }
}
