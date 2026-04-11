using System;
using System.Collections.Generic;
using System.Text;

namespace MainApplication.Models
{
    class ProductionLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string ActionDescription { get; set; }
        public int UserId { get; set; }

    }
}
