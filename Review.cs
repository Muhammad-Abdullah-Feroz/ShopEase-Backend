using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopEase_Backend
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}