using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ShopEase_Backend
{
    [DataContract]
    public class UserOrderHistory
    {
        [DataMember]
        public int OrderId { get; set; }
        [DataMember]
        public string ProductName { get; set; }
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Type { get; set; } // "Rented" or "Bought"
    }
}