using System;
using System.Collections.Generic;
using System.Linq;
using MainApplication.Models;
using MainApplication.DAL;

namespace MainApplication.BLL.Services
{
    class InventoryService
    {
        private readonly IRepository<InventoryItem> _inventoryRepo;
        private readonly MESDbContext _context; 

        public InventoryService(IRepository<InventoryItem> inventoryRepo, MESDbContext context)
        {
            _inventoryRepo = inventoryRepo;
            _context = context;
        }

        
        public bool CheckAvailabilityForOrder(ProductionOrder order)
        {
            var requiredMaterials = _context.OrderMaterials.Where(om => om.OrderId == order.Id).ToList();

            foreach (var material in requiredMaterials)
            {
                var item = _inventoryRepo.GetById(material.ItemId);
                
                if (item == null || item.AvailableQuantity < (material.QuantityNeeded * order.RequiredQuantity))
                {
                    return false;
                }
            }
            return true;
        }

       
        public void DeductStockForOrder(ProductionOrder order, int userId)
        {
            var requiredMaterials = _context.OrderMaterials.Where(om => om.OrderId == order.Id).ToList();

            foreach (var material in requiredMaterials)
            {
                var item = _inventoryRepo.GetById(material.ItemId);
                if (item != null)
                {
                    int totalDeduction = material.QuantityNeeded * order.RequiredQuantity;
                    item.AvailableQuantity -= totalDeduction;
                    _inventoryRepo.Update(item);

                  
                    var log = new ProductionLog
                    {
                        Timestamp = DateTime.Now,
                        ActionDescription = $"Stoc scazut cu {totalDeduction} unitati (Piesa ID: {item.Id}) pentru Comanda #{order.Id}",
                        UserId = userId
                    };
                    _context.ProductionLogs.Add(log);
                }
            }
            _context.SaveChanges(); 
        }

        public List<InventoryItem> GetItemsBelowThreshold()
        {
            return _inventoryRepo.GetAll().Where(i => i.AvailableQuantity <= i.AlertThreshold).ToList();
        }
    }
}