using System;
using System.Collections.Generic;
using System.Text;
using MainApplication.Models;

namespace MainApplication.BLL.StatePattern
{
    class NewState : IOrderState
    {
        public void Process(ProductionOrder order)
        {
           
            order.StatusName = "New";
            
        }
    }
}
