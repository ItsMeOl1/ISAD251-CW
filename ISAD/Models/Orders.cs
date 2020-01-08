using System;
using System.Collections.Generic;

namespace ISAD.Models
{
    public partial class Orders
    {
        public Orders()
        {
            OrderDetails = new HashSet<OrderDetails>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? TableNumber { get; set; }
        public int? TotalPrice { get; set; }

        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
