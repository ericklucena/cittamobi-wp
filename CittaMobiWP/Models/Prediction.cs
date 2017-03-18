using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CittaMobiWP.Models
{
    class Prediction
    {
        public List<object> Messages { get; set; }
        public List<Service> Services { get; set; }
        public bool FeedbackEnabled { get; set; }   
    }
}
