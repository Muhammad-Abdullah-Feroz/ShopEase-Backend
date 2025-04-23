using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopEase_Backend
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int BuyerId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string CourierTrackingId { get; set; }
    }

}