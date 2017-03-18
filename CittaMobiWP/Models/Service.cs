using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CittaMobiWP.Models
{
    public class Service : IComparable <Service>
    {
        private const string MSG_NO_PREDICTION = "Sem previsão.";
        private const string MSG_ARRIVING = "Próximo";
        private const string MSG_ARRIVING_IN = "Chegando em ";
        private const string MSG_SCHEDULED_IN = "Programado para daqui a ";
        private const string MSG_ARRIVING_UNIT = " minuto(s).";

        private const string DIRECTION_OUTWARD = "OUTWARD";
        private const string DIRECTION_RETURN = "RETURN";


        public string RouteCode { get; set; }
        public string RouteMnemonic { get; set; }
        private string direction;
        public string Direction
        {
            get
            {
                return direction;
            }

            set
            {
                if (value.Equals(DIRECTION_OUTWARD, StringComparison.OrdinalIgnoreCase))
                {
                    direction = "Ida";
                }
                else if (value.Equals(DIRECTION_RETURN, StringComparison.OrdinalIgnoreCase))
                {
                    direction = "Volta";
                }
                else
                {
                    direction = value;
                }
            }
        }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public bool CompanyAuthorized { get; set; }
        public int Id { get; set; }
        public int ServiceId
        {
            get
            {
                return Id;
            }
            set
            {
                Id = value;
            }
        }
        public string ServiceMnemonic { get; set; }
        public string PredictionType { get; set; }
        public int ActiveVehicles { get; set; }
        public int ScheduledVehicles { get; set; }
        public List<object> Messages { get; set; }
        public List<Vehicle> Vehicles { get; set; }

        public string NextArrival
        {
            get
            {
                if (Vehicles != null && Vehicles.Count > 0)
                {
                    int time = Vehicles.First().Prediction / 60;

                    if (time == 0)
                    {
                        return MSG_ARRIVING;
                    }
                    else
                    {
                        if (NextArrivalType.Equals(Vehicle.TYPE_MESSAGE, StringComparison.OrdinalIgnoreCase))
                        {
                            return MSG_ARRIVING_IN + time + MSG_ARRIVING_UNIT;
                        }
                        else if (NextArrivalType.Equals(Vehicle.TYPE_SCHEDULE, StringComparison.OrdinalIgnoreCase))
                        {
                            return MSG_SCHEDULED_IN + time + MSG_ARRIVING_UNIT;
                        }
                    }
                }
                
                return MSG_NO_PREDICTION;

            }
        }

        public string NextArrivalType
        {
            get
            {
                if (Vehicles != null && Vehicles.Count > 0)
                {
                    return Vehicles.First().Type;
                }

                return null;
            }
        }

        public int CompareTo(Service other)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                int thisTime = int.MaxValue;
                int otherTime = int.MaxValue;

                if (this.Vehicles != null && this.Vehicles.Count > 0)
                {
                    thisTime = this.Vehicles.First().Prediction;
                }

                if (other.Vehicles != null && other.Vehicles.Count > 0)
                {
                    otherTime = other.Vehicles.First().Prediction;
                }

                return thisTime - otherTime;
                
            }
        }

        public Service Copy()
        {
            Service copy = new Service();

            copy.RouteCode = RouteCode;
            copy.RouteMnemonic = RouteMnemonic;
            copy.CompanyId = CompanyId;
            copy.ServiceId = ServiceId;
            copy.ServiceMnemonic = ServiceMnemonic;
            copy.PredictionType = PredictionType;
            copy.ActiveVehicles = ActiveVehicles;
            copy.ScheduledVehicles = ScheduledVehicles;
            
            copy.Messages = new List<object>();
            copy.Vehicles = new List<Vehicle>();

            foreach (object o in Messages)
            {
                copy.Messages.Add(o);
            }

            foreach(Vehicle v in Vehicles)
            {
                copy.Vehicles.Add(v);
            }

            return copy;
        }

    }
}
