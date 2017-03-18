using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace CittaMobiWP.Models
{
    public class ServiceVehicle
    {
        public string Plate { get; set; }
        public string Prefix { get; set; }
        public long Ts { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public int Bearing { get; set; }

        public Geopoint Geopoint
        {
            get
            {
                BasicGeoposition bg = new BasicGeoposition();
                bg.Latitude = Lat;
                bg.Longitude = Lng;
                Geopoint g = new Geopoint(bg);
                return g;
            }
        }
        public Point AnchorPoint
        {
            get
            {
                return new Point(1, 1);
            }
        }
    }
}
