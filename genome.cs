using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace graves {
    /// <summary>
    /// This class represents a sequence
    /// </summary>
    class genome : IComparable{        
        
        public List<int> sequence = new List<int>();  //the sequence of cemeteries to visit
        public double travelDist = 0;
        public double penalty = 0;
        public double fitness
        {
            get { return this.travelDist - this.penalty; }
        }

        /// <summary>
        /// Finding the distance from one cemetery to another
        /// </summary>
        public double distanceTo(int id1, int id2) {
            cemetery c1 = Program.cemsList[id1];
            cemetery c2 = Program.cemsList[id2];
            return distance(c1.lat, c1.lon, c2.lat, c2.lon, 'M');
        }

        /// <summary>
        /// Populating this genome with a random sequence
        /// </summary>
        public void randomize(){

            //Generating a list of integers to make the random list out of
            List<int> cemids = new List<int>();
            for (int i = 0; i < Program.cems.Count; i++) {
                cemids.Add(i);
            }

            //Generating a random sequence of cemeteries
            sequence.Clear();
            foreach (var i in cemids.AsRandom()) {
                sequence.Add(cemids[i]);
            }

            //Calculating the distance traveled
            this.travelDistance();
            this.calcPenalty();
        }

        /// <summary>
        /// The total distance it takes to travel to all the cemeteries in the list 
        /// </summary>
        /// <returns></returns>
        public double travelDistance() {

            double totalDist = 0;

            //Looping through the sequence, calculating numbers
            for (int i = 0; i < this.sequence.Count-1; i++ ) {
                int id1 = this.sequence[i];
                int id2 = this.sequence[i + 1];
                totalDist += this.distanceTo(id1, id2);
            }

            totalDist = Math.Ceiling(totalDist);

            this.travelDist = totalDist;
            return totalDist;
        }
        
        /// <summary>
        /// Calculating the penalty by taking the difference of the temperature on that day from the ideal
        /// </summary>
        public int calcPenalty() {
            int p = 0;

            int day = 0;
            foreach (int i in sequence) {
                cemetery c = Program.cemsList[i];
                int temp = c.temps[day];
                int difference = Math.Abs(Program.idealTemp - temp);
                p += difference;
                day++;
            }

            this.penalty = p;
            return p;
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

        public int CompareTo(genome genomeToCompare) {

            return this.fitness.CompareTo(genomeToCompare.fitness);
        }
    }

    /// <summary>
    /// This holds the list randomizer
    /// </summary>
    public static class MyExtensions {
        public static IEnumerable<T> AsRandom<T>(this IList<T> list) {
            int[] indexes = Enumerable.Range(0, list.Count).ToArray();
            Random generator = new Random((int)System.DateTime.UtcNow.Ticks);

            for (int i = 0; i < list.Count; ++i) {
                int position = generator.Next(i, list.Count);

                yield return list[indexes[position]];

                indexes[position] = indexes[i];
            }
        }
    }
}
