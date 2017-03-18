using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CittaMobiWP.Models
{
    class VehiclesPayload
    {
        public int StopId { get; set; }
        public Service Service { get; set; }

        public VehiclesPayload(int stopId, Service service)
        {
            StopId = stopId;
            Service = service;
        }
        
    }
}
