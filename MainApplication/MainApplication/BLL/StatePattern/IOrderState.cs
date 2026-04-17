using System;
using System.Collections.Generic;
using System.Text;
using MainApplication.Models;

namespace MainApplication.BLL.StatePattern
{
    interface IOrderState
    {
        void Process(ProductionOrder order);
    }
}
