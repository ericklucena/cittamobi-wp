using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CittaMobiWP.Models
{
    public class Vehicle
    {
        public const string TYPE_MESSAGE = "MESSAGE";
        public const string TYPE_SCHEDULE = "SCHEDULE";

        private const string MSG_ARRIVING = "Próximo";
        private const string MSG_ARRIVING_IN = "Chegando em ";
        private const string MSG_SCHEDULED_IN = "Programado para daqui a ";
        private const string MSG_ARRIVING_UNIT = " minuto(s).";

        public string Plate { get; set; }
        public string Prefix { get; set; }
        public bool Wheelchair { get; set; }
        public bool Climatized { get; set; }
        public int Prediction { get; set; }
        public int Age { get; set; }
        public string Type { get; set; }

        public String StringPrediction        
        {
            get
            {
                int time = Prediction / 60;

                if (time == 0)
                {
                    return MSG_ARRIVING;
                }
                else
                {
                    if (Type.Equals(Vehicle.TYPE_MESSAGE, StringComparison.OrdinalIgnoreCase))
                    {
                        return MSG_ARRIVING_IN + time + MSG_ARRIVING_UNIT;
                    }
                    else if (Type.Equals(Vehicle.TYPE_SCHEDULE, StringComparison.OrdinalIgnoreCase))
                    {
                        return MSG_SCHEDULED_IN + time + MSG_ARRIVING_UNIT;
                    }
                }

                return null;
            }
        }

    }
}
