using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ShopEase_Backend
{
    [DataContract]
    public class cart_item
    {
        [DataMember]
        public int ProductId { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public decimal ProductPrice { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public byte[] Image { get; set; }
    }
}