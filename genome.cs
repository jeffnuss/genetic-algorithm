using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace graves {

    /// <summary>
    /// This class represents a sequence
    /// </summary>
    
    [Serializable]
    class genome : IComparable, ICloneable {        
        
        //the sequence of cemeteries to visit
        public List<int> sequence { get; private set; }
        public double travelDist { get; private set; }
        public double penalty { get; private set; }
        public double fitness {
            get { return (this.travelDist/* + this.penalty*/); }
            private set { this.fitness = value; }
        }

        public double eta { get; private set; }
        public double beta { get; private set; }
        public int maxCrossoverSize { get; private set; }

        /// <summary>
        /// Genome constructor
        /// </summary>
        /// <param name="eta">Crossover coefficient</param>
        /// <param name="beta">Mutation coefficient</param>
        public genome(double eta, double beta, int crossoverSize)
        {
            this.eta = eta;
            this.beta = beta;
            this.maxCrossoverSize = crossoverSize;
            penalty = 0;
            travelDist = 0;
            sequence = new List<int>();
        }

        /// <summary>
        /// Finding the distance from one cemetery to another
        /// </summary>
        public double distanceTo(int id1, int id2) {
            cemetery c1 = Program.cemsList[id1];
            cemetery c2 = Program.cemsList[id2];
            //double d = distance(c1.lat, c1.lon, c2.lat, c2.lon, 'M');
            double d = euclidiandistance(c1.lat, c1.lon, c2.lat, c2.lon);
            return d;
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

        static private double euclidiandistance(double x0, double y0, double x1, double y1) {
            // Cities are points x0,y0 and x1,y1 in kilometers or miles or Smoots[1]
            double dx = (x1 - x0) * 69.1;
            double dy = (y1 - y0) * 53.0;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            return dist;
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

        int IComparable.CompareTo(object genomeToCompare) {

            return this.fitness.CompareTo(((genome)genomeToCompare).fitness);
        }

        /// <summary>
        /// This method performs crossover and mutation to create two children
        /// </summary>
        /// <param name="chanceOfCrossover">The chance of crossover occuring</param>
        /// <param name="chanceOfMutation">The chance of mutation occuring</param>
        /// <returns>A list containing the two children</returns>
        public genome getChild(double chanceOfCrossover, double chanceOfMutation)
        {
            genome child = (genome)this.Clone();
            crossover(child, chanceOfCrossover);
            mutate(child, chanceOfMutation);
            child.travelDistance();
            child.calcPenalty();
            return child;
        }

        /// <summary>
        /// Use serialization to make a deep copy
        /// </summary>
        /// <returns>A clone of the object</returns>
        public object Clone()
        {
            BinaryFormatter BF = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            BF.Serialize(memStream, this);
            memStream.Position = 0;

            return (BF.Deserialize(memStream));
        }

        public override bool Equals(object obj)
        {
            genome other = (genome)obj;
            if (other == null) {
                return false;
            }
            bool test = this.fitness == other.fitness;
            return (this.fitness == other.fitness);
        }

        public override int GetHashCode()
        {
            return this.fitness.GetHashCode();
        }

        private bool crossover(genome child, double chanceOfCrossover)
        {

            Random crossoverRand = new Random((int)System.DateTime.UtcNow.Ticks);
            if (Program.rand.NextDouble() > chanceOfCrossover)
                return false;

            else
            {

                Random rand = new Random((int)System.DateTime.UtcNow.Ticks);
                int crossoverSize = Program.rand.Next(1, maxCrossoverSize);
                int crossoverIndex = Program.rand.Next(0, child.sequence.Count - crossoverSize);
                child.sequence.Reverse(crossoverIndex, crossoverSize);
            }
            return true;
        }

        private bool mutate(genome child, double chanceOfMutation)
        {

            bool wasThereAnyMutation = false;

            for (int i = 0; i < child.sequence.Count; i++)
            {
                Random mutateRand = new Random((int)System.DateTime.UtcNow.Ticks);
                if (mutateRand.NextDouble() > chanceOfMutation)
                {
                    continue;
                }
                else
                {
                    int swapIndex = Program.rand.Next(0, child.sequence.Count);
                    int geneToSwap = child.sequence[swapIndex];
                    child.sequence[swapIndex] = child.sequence[i];
                    child.sequence[i] = geneToSwap;
                    wasThereAnyMutation = true;
                }
            }
            return wasThereAnyMutation;
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
                int position = Program.rand.Next(i, list.Count);

                yield return list[indexes[position]];

                indexes[position] = indexes[i];
            }
        }
    }
}
