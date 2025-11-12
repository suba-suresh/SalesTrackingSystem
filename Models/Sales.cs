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
        public int TotalOrders => (int)OrdersJustEat + (int)OrdersUber + (int)OrdersDeliveroo + (int)OrdersInHouse;

        [NotMapped]
        public decimal TotalAmount => AmountJustEat + AmountUber + AmountDeliveroo + AmountInhouse;
    }
}