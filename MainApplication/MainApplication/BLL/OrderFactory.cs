using System;
using MainApplication.Models;

namespace MainApplication.BLL
{
    class OrderFactory
    {
        public ProductionOrder CreateOrder(string orderType, string productName, int quantity)
        {
            return new ProductionOrder
            {
                ProductName = productName,
                RequiredQuantity = quantity,
                LaunchDate = DateTime.Now,
                StatusName = "New"
            };
        }
    }
}