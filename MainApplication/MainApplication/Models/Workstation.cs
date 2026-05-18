using System;
using System.Collections.Generic;
using System.Text;

namespace MainApplication.Models
{
    public class Workstation
    {
        public int Id { get; set; }
        public int? CycleTimeSeconds { get; set; }
        public DateTime? CurrentTaskStartTime { get; set; }
        public string CurrentStatus { get; set; } = "ONLINE";
        public string? CurrentProduct { get; set; }
    }
}
