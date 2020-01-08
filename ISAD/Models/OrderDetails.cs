using System;
using System.Collections.Generic;

namespace ISAD.Models
{
    public partial class OrderDetails
    {
        public int? ProductId { get; set; }
        public int? OrderId { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Products Product { get; set; }
    }
}
