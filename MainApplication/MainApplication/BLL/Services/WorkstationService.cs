using System.Linq;
using MainApplication.Models;
using MainApplication.DAL;

namespace MainApplication.BLL.Services
{
    class WorkstationService
    {
        private readonly IRepository<Workstation> _workstationRepo;

        public WorkstationService(IRepository<Workstation> workstationRepo)
        {
            _workstationRepo = workstationRepo;
        }

        
        public bool AssignOrderToStation(int orderId, int stationId)
        {
            var station = _workstationRepo.GetById(stationId);

           
            if (station != null && station.IsOnline && station.CurrentOrderId == null)
            {
                station.CurrentOrderId = orderId;
                _workstationRepo.Update(station);
                return true;
            }
            return false;
        }

       
        public void MarkStationOffline(int stationId, string reason)
        {
            var station = _workstationRepo.GetById(stationId);
            if (station != null)
            {
                station.IsOnline = false;
                station.CurrentOrderId = null; 
                _workstationRepo.Update(station);

                
            }
        }
    }
}