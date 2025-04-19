using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopEase_Backend
{
    public class Product
    {
        public string name { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public string price { get; set; }
        public string image { get; set; }
        public string seller { get; set; }
        public string rating { get; set; }
    }
}