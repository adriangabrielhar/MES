using System;
using System.Collections.Generic;
using System.Text;

namespace MainApplication.Models
{
    public class Workstation
    {
        public int Id { get; set; }
        public string WorkstationName { get; set; }
        public bool IsOnline { get; set; }
        public string LineType { get; set; } = "Final Product"; // "Sub-Assembly" or "Final Product"
        public int? CycleTimeSeconds { get; set; }
        public DateTime? CurrentTaskStartTime { get; set; }
    }
}
