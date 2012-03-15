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

        static void Main(string[] args) {

            //A friendly welcoming message
            Console.WriteLine("Welcome to the Cemetery Distance Calculator.  Reading the data");

            //Step 1: Reading in the data
            readData();

            //Step 2: Generate the first generation
            Console.WriteLine("Generating the most responsible parents in the world");
            List<genome> startgen = new List<genome>();
            for (int i = 0; i < 12; i++) {
                genome g = new genome();
                g.randomize();
                startgen.Add(g);               
                
                Console.WriteLine("Travel Miles: " + g.travelDist + " | Penalty: " + g.penalty);
            }

            // ---- Start Loop ---- //

            //Step 3: Hooking up parents, making them have babies
            // 3a: Do crossover
            // 3b: Do mutation
            // 3c: Do elitism           
            
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
      
        


    }
}
