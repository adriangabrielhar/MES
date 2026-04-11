using System;
using System.Collections.Generic;
using System.Text;

namespace MainApplication.Models
{
    class InventoryItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int AvailableQuantity { get; set; }
        public int AlertThreshold { get; set; }

    }
}
