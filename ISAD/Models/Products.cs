﻿using System;
using System.Collections.Generic;

namespace ISAD.Models
{
    public partial class Products
    {
        public Products()
        {
            OrderDetails = new HashSet<OrderDetails>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ProductDetails { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }

        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
