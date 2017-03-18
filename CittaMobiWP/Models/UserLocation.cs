using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace CittaMobiWP.Models
{
    public class UserLocation
    {
        private Geocoordinate coordinate;

        public UserLocation(Geocoordinate coordinate)
        {
            this.coordinate = coordinate;
        }


        public Geopoint Geopoint
        {
            get
            {
                BasicGeoposition bg = new BasicGeoposition();
                bg.Latitude = coordinate.Latitude;
                bg.Longitude = coordinate.Longitude;
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
