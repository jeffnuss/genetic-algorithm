using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace graves {
    class Program {

        //All cemeteries read in from the data file
        static public Dictionary<int, cemetery> cems = new Dictionary<int, cemetery>();
        static public List<cemetery> cemsList = new List<cemetery>();
        static public int idealTemp = 65;
        static int generationSize = 12;
        static int tournamentSize = 3;
        static double chanceOfCrossover = 0.8;
        static double chanceOfMutation = 0.1;
        static double eta = 0.5;
        static double beta = 0.5;
        static int totalGenerations = 100;

        static void Main(string[] args) {

            //A friendly welcoming message
            Console.WriteLine("Welcome to the Cemetery Distance Calculator.  Reading the data");

            //Step 1: Reading in the data
            readData();

            //Step 2: Generate the first generation
            Console.WriteLine("Generating the most responsible parents in the world");
            List<genome> startgen = new List<genome>();
            for (int i = 0; i < generationSize; i++) {
                genome g = new genome();
                g.randomize();
                startgen.Add(g);               
                
                Console.WriteLine("Travel Miles: " + g.travelDist + " | Penalty: " + g.penalty);
            }

            // ---- Start Loop ---- //

            //Step 3: Hooking up parents, making them have babies
            // 3a-1: Figure out parents
            for (int i = 0; i < totalGenerations; i++)
            {
                List<parent> parents = getParents(startgen, tournamentSize);
                List<genome> children = new List<genome>();

                double alphaInner = 1 - ((i - 1) / totalGenerations);

                foreach (parent p in parents)
                {

                    children.AddRange(p.getChildren(chanceOfCrossover, chanceOfMutation, alphaInner));
                }

                List<genome> nextGeneration = elitismTest(parents, children);

                // 3a: Do crossover
                // 3b: Do mutation
                // 3c: Do elitism           
            }
            // ---- End Loop ------ //
            
            //Step 4: Generate Report
            

            //Pausing after running
            Console.WriteLine("Completed the Algorithm");
            Console.ReadLine();
        }

        /// <summary>
        /// Reading in the data from the two csv files
        /// </summary>
        public static void readData() {
            //==========================================
            //      Reading in the cemetery data
            //==========================================
            TextFieldParser parser = new TextFieldParser(@"data.csv");
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            int i = 0;

            //Looping through each line
            while (!parser.EndOfData) {

                //Reading the line
                string[] fields = parser.ReadFields();

                //Skipping the first row
                i++; if (i == 1) continue;

                //Creating a new cemetery object
                cemetery c = new cemetery();
                c.cemetery_id = int.Parse(fields[0]);
                c.name = fields[1];
                c.lat = float.Parse(fields[2]);
                c.lon = float.Parse(fields[3]);
                c.city = fields[4];
                c.state = fields[5];
                c.country = fields[6];

                //Reading in the temperature data
                List<int> temps = new List<int>();
                int j = 0;
                foreach (string f in fields) {
                    j++; if (j <= 7) continue;
                    temps.Add(int.Parse(f));
                }
                c.temps = temps;

                //Adding the cemetery to the list
                cems.Add(c.cemetery_id, c);
                cemsList.Add(c);
            }

            //Closing the parser
            parser.Close();

        }
      

        private static List<parent> getParents(List<genome> generation, int tournamentSize) {

            List<parent> parents = new List<parent>();
            Random randTourneyPlayer = new Random();
            for (int i = 0; i < generation.Count; i++) {
                
                List<genome> motherCandidates = new List<genome>();
                List<genome> fatherCandidates = new List<genome>();
                for (int j = 0; j < tournamentSize; j++) {

                    motherCandidates.Add(generation[randTourneyPlayer.Next(0, generation.Count + 1)]);
                    fatherCandidates.Add(generation[randTourneyPlayer.Next(0, generation.Count + 1)]);
                }
                parents.Add(new parent(motherCandidates.Max(), fatherCandidates.Max()));
            }

            return parents;
        }

        /// <summary>
        /// Have the children and the parents duke it out in an epic battle royale to see who emerges victorious
        /// </summary>
        /// <param name="parents">The parents</param>
        /// <param name="children">The kids</param>
        /// <returns>The strongest of the group</returns>
        private static List<genome> elitismTest(List<parent> parents, List<genome> children) {

            return null;
        }


    }
}
