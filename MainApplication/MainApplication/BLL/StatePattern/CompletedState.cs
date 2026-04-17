using System;
using System.Collections.Generic;
using System.Text;
using MainApplication.Models;

namespace MainApplication.BLL.StatePattern
{
    class CompletedState : IOrderState
    {
        public void Process(ProductionOrder order)
        {
            order.StatusName = "Done";
         
        }
    }
}
