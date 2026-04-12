using System;
using System.Collections.Generic;
using System.Text;

namespace MainApplication.DTO
{
    public class DashboardData
    {
        public int ActiveOrderCount { get; set; }
        public int OfflineWorkstationsCount { get; set; }
        public List<string> ItemsBelowAlertThreshold { get; set; }
        public List<string> RecentActivityLogs { get; set; }
    }
}
