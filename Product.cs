using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace ShopEase_Backend
{
    public class Product
    {
        public string name { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public Image image { get; set; }
        public int rating { get; set; }
        public int quantity { get; set; }
        public bool is_rentable { get; set; }
        public string seller { get; set; }
    }
}