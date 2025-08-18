using System;

namespace SalesTrackingSystem.Models
{
    public class SaleRecord
    {
        public DateTime Date { get; set; }
        public int OrdersJustEat { get; set; }
        public int OrdersUber { get; set; }
        public int OrdersDeliveroo { get; set; }

        public decimal AmountJustEat { get; set; }
        public decimal AmountUber { get; set; }
        public decimal AmountDeliveroo { get; set; }

        public int TotalOrders => OrdersJustEat + OrdersUber + OrdersDeliveroo;
        public decimal TotalAmount => AmountJustEat + AmountUber + AmountDeliveroo;
    }
}
