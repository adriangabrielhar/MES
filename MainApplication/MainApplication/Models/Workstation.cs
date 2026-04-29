using System;
using System.Collections.Generic;
using System.Text;

namespace MainApplication.Models
{
    class Workstation
    {
        public int Id { get; set; }
        public string WorkstationName { get; set; }
        public bool IsOnline { get; set; }
        public int? CurrentOrderId { get; set; }
        public string? Expertiza { get; set; }
    }
}
