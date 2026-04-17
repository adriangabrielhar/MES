using System;
using System.Linq;
using MainApplication.BLL.Services;
using MainApplication.DAL;
using MainApplication.DTO;
using MainApplication.Models;

namespace MainApplication.BLL
{
    public class ProductionSystemFacade
    {
        private readonly AuthService _authService;
        private readonly InventoryService _inventoryService;
        private readonly WorkstationService _workstationService;
        private readonly OrderFactory _factory;
        private readonly MESDbContext _context;

        public ProductionSystemFacade()
        {
            _context = new MESDbContext();
            _authService = new AuthService(_context);
            var inventoryRepo = new InventoryRepository(_context);
            _inventoryService = new InventoryService(inventoryRepo, _context);
            _factory = new OrderFactory();
        }

        public ResponseData LaunchOrder(string orderType, string product, int quantity, int userId)
        {
            var response = new ResponseData();

            try
            {
                var newOrder = _factory.CreateOrder(orderType, product, quantity);
                _context.ProductionOrders.Add(newOrder);
                _context.SaveChanges();

                if (_inventoryService.CheckAvailabilityForOrder(newOrder))
                {
                    response.IsSuccess = true;
                    response.Message = "Comanda a fost lansata cu succes!";
                    response.CreateOrderId = newOrder.Id;
                    LogAction($"Lansare comanda: {product}", userId);
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Stoc insuficient!";
                    response.CreateOrderId = newOrder.Id;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public void AssignAndStartAssembly(int orderId, int stationId, int userId)
        {
            var order = _context.ProductionOrders.Find(orderId);
            if (order != null)
            {
                var assemblyState = new StatePattern.InAssemblyState();
                assemblyState.Process(order);
                _context.ProductionOrders.Update(order);
                _inventoryService.DeductStockForOrder(order, userId);
                LogAction($"Comanda #{orderId} in asamblare pe statia #{stationId}", userId);
                _context.SaveChanges();
            }
        }

        public DashboardData GetDashboardMetrics()
        {
            return new DashboardData
            {
                ActiveOrderCount = _context.ProductionOrders.Count(o => o.StatusName == "InAssembly" || o.StatusName == "New"),
                OfflineWorkstationsCount = _context.Workstations.Count(w => !w.IsOnline),
                ItemsBelowAlertThreshold = _inventoryService.GetItemsBelowThreshold().Select(i => i.ItemName).ToList(),
                RecentActivityLogs = _context.ProductionLogs.OrderByDescending(l => l.Timestamp).Take(5).Select(l => l.ActionDescription).ToList()
            };
        }

        private void LogAction(string action, int userId)
        {
            var log = new ProductionLog
            {
                ActionDescription = action,
                Timestamp = DateTime.Now,
                UserId = userId
            };
            _context.ProductionLogs.Add(log);
            _context.SaveChanges();
        }
    }
}