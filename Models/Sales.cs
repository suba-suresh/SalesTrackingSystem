// File: Models/SaleRecord.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesTrackingSystem.Models
{
    [Table("Sales", Schema = "dbo")]
    public class SaleRecord
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Today;

        // Orders - ALL CHANGED TO DECIMAL to match database DECIMAL(18,2)
        [Column("Inhouse")]
        public decimal OrdersInHouse { get; set; }

        [Column("JustEat")]
        public decimal OrdersJustEat { get; set; }

        [Column("UberEats")]
        public decimal OrdersUber { get; set; }

        [Column("Deliveroo")]
        public decimal OrdersDeliveroo { get; set; }

        // Amounts
        public decimal AmountInhouse { get; set; }
        public decimal AmountJustEat { get; set; }

        [Column("AmountUberEats")]
        public decimal AmountUber { get; set; }

        public decimal AmountDeliveroo { get; set; }

        // Computed read-only properties used in UI
        [NotMapped]
        public decimal TotalOrders => OrdersJustEat + OrdersUber + OrdersDeliveroo + OrdersInHouse;

        [NotMapped]
        public decimal TotalAmount => AmountJustEat + AmountUber + AmountDeliveroo + AmountInhouse;
    }
}