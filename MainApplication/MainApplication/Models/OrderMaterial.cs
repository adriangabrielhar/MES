using System;
using System.Collections.Generic;
using System.Text;

namespace MainApplication.Models
{
    class OrderMaterial
    {
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int QuantityNeeded { get; set; }
    }
}
