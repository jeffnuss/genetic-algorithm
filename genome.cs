using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace graves {
    /// <summary>
    /// This class represents a sequence
    /// </summary>
    class genome {        
        public List<int> sequence;  //the sequence of cemeteries to visit

        /// <summary>
        /// Finding the distance from one cemetery to another
        /// </summary>
        /// <param name="id1">cemetery_id for one cemetery</param>
        /// <param name="id2">cemetery_id for another cemetery</param>
        /// <returns>a distance value in miles</returns>
        public double distanceTo(int id1, int id2) {
            cemetery c1 = Program.cems[id1];
            cemetery c2 = Program.cems[id2];

            return distance(c1.lat, c1.lon, c2.lat, c2.lon, 'M');
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //:::  Definitions:                                                           :::
        //:::    South latitudes are negative, east longitudes are positive           :::
        //:::                                                                         :::
        //:::  Passed to function:                                                    :::
        //:::    lat1, lon1 = Latitude and Longitude of point 1 (in decimal degrees)  :::
        //:::    lat2, lon2 = Latitude and Longitude of point 2 (in decimal degrees)  :::
        //:::    unit = the unit you desire for results                               :::
        //:::           where: 'M' is statute miles                                   :::
        //:::                  'K' is kilometers (default)                            :::
        //:::                  'N' is nautical miles                                  :::
        //:::                                                                         :::
        //:::  United States ZIP Code/ Canadian Postal Code databases with latitude   :::
        //:::  & longitude are available at http://www.zipcodeworld.com               :::
        //:::                                                                         :::S
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        static private double distance(double lat1, double lon1, double lat2, double lon2, char unit) {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K') {
                dist = dist * 1.609344;
            } else if (unit == 'N') {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        static private double deg2rad(double deg) {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        static private double rad2deg(double rad) {
            return (rad / Math.PI * 180.0);
        }

    }
}
