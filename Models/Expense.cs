using System;

namespace SalesTrackingSystem.Models
{
    public enum PaymentMethod
    {
        Cash,
        Card
    }

    public class Expense
    {
        public DateTime Date { get; set; } = DateTime.Today;
        public string Category { get; set; } = string.Empty;
        public PaymentMethod PaymentType { get; set; } = PaymentMethod.Cash;
        public decimal Amount { get; set; }
    }
}
