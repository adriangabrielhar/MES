using System.Linq;
using MainApplication.Models;

namespace MainApplication.BLL.Services
{
    class WorkstationService
    {
        // Folosim contextul direct în loc de IRepository
        private readonly MESDbContext _context;

        public WorkstationService(MESDbContext context)
        {
            _context = context;
        }

        public bool AssignOrderToStation(int orderId, int stationId)
        {
            var station = _context.Workstations.Find(stationId);

            if (station != null && station.IsOnline && station.CurrentOrderId == null)
            {
                station.CurrentOrderId = orderId;
                _context.Workstations.Update(station);
                _context.SaveChanges(); // Salvăm direct în baza de date
                return true;
            }
            return false;
        }

        public void MarkStationOffline(int stationId, string reason)
        {
            var station = _context.Workstations.Find(stationId);
            if (station != null)
            {
                station.IsOnline = false;
                station.CurrentOrderId = null;
                _context.Workstations.Update(station);
                _context.SaveChanges(); // Salvăm direct în baza de date
            }
        }
    }
}