using System;
using System.Collections.Generic;
using System.Text;

namespace MainApplication.Models
{
    class ProductionOrder
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int RequiredQuantity { get; set; }
        public DateTime LaunchDate { get; set; }
        public string StatusName { get; set; }

    }
}
