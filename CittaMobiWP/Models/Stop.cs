using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace CittaMobiWP.Models
{
    public class Stop
    {
        public int Id { get; set; }
        public string Mnemonic { get; set; }
        public Location Location { get; set; }
        public int Bearing { get; set; }
        public string Status { get; set; }
        public List<object> Services { get; set; }
        public int Kind { get; set; }

        public Geopoint Geopoint
        {
            get
            {
                BasicGeoposition bg = new BasicGeoposition();
                bg.Latitude = Location.Lat;
                bg.Longitude = Location.Lng;
                Geopoint g = new Geopoint(bg);
                return g;
            }
        }

        public Point AnchorPoint
        {
            get
            {
                return new Point(0.5, 0.5);
            }
        }
    }
}
