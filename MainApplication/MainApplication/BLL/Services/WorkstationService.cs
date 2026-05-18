using System.Linq;
using MainApplication.Models;

namespace MainApplication.BLL.Services
{
    class WorkstationService
    {
        private readonly MESDbContext _context;

        public WorkstationService(MESDbContext context)
        {
            _context = context;
        }

        public bool AssignOrderToStation(int orderId, int stationId)
        {
            var station = _context.Workstations.Find(stationId);

            // Modificarea 1 (Linia 20): Verificăm starea text a liniei
            if (station != null && station.CurrentStatus == "ONLINE")
            {
                _context.Workstations.Update(station);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public void MarkStationOffline(int stationId, string reason)
        {
            var station = _context.Workstations.Find(stationId);
            if (station != null)
            {
                // Modificarea 2 (Linia 34): Setăm statusul pe OFFLINE sau EMERGENCY
                station.CurrentStatus = "OFFLINE";

                _context.Workstations.Update(station);
                _context.SaveChanges();
            }
        }
    }
}