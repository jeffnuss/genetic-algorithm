using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;

namespace graves {
    class Program {

        //Start time
        static public double start = DateTime.UtcNow.TimeOfDay.TotalSeconds;
        static public string reportname = String.Format("{0:h-mm-ss-tt}", DateTime.Now) + ".html";
        static public Random rand = new Random((int)System.DateTime.UtcNow.Ticks);

        //All cemeteries read in from the data file
        static public Dictionary<int, cemetery> cems = new Dictionary<int, cemetery>();
        static public List<cemetery> cemsList = new List<cemetery>();
        static public List<generation> genealogy = new List<generation>();  //keeps track of all the generations

        //Algoritm parameters
        static public int idealTemp = 76;
        static int generationSize = 24;
        static int tournamentSize = 3;
        static double chanceOfCrossover = .85;
        static double chanceOfMutation = 0.4;
        static public double penaltyMultiplier = 10.0;
        static int maxCrossoverSize = 80;
        static int totalGenerations = 40000;

        static void Main(string[] args) {
            
            //Getting command line parameters
            if (args.Length > 0) {
                Program.chanceOfMutation  = Double.Parse(args[0]);
                Program.chanceOfCrossover = Double.Parse(args[1]);
                Program.totalGenerations = int.Parse(args[2]);
                reportname = "mute=" + Program.chanceOfMutation + "-cross=" + Program.chanceOfCrossover + "-runs=" + Program.totalGenerations + ".html";
            }


            //A friendly welcoming message
            Console.WriteLine("Welcome to the Cemetery Distance Calculator.  Reading the data");

            //Step 1: Reading in the data
            readData();

            //Step 2: Generate the first generation
            Console.WriteLine("Generating the most responsible parents in the world");
            List<genome> startgen = new List<genome>();
            for (int i = 0; i < generationSize; i++) {

                genome g = new genome(maxCrossoverSize);
                g.randomize();
                startgen.Add(g);               
                
                Console.WriteLine("Travel Miles: " + g.travelDist + " | Penalty: " + g.penalty);
            }

            // ---- Start Loop ---- //

            // 3: Figure out children
            List<genome> nextGeneration = startgen;
            for (int i = 0; i < totalGenerations; i++) {
                
                List<genome> parents = nextGeneration;
                List<genome> children = new List<genome>();

                foreach (genome g in parents) {                    
                    genome childToAdd = g.getChild(chanceOfCrossover, chanceOfMutation);
                    children.Add(childToAdd);
                }

                nextGeneration = new List<genome>();
                nextGeneration = elitismTest(parents, children);

                //Keeping track of each generation for reporting purposes
                generation gen = new generation(nextGeneration);
                genealogy.Add(gen);

                if (i % 100 == 0)
                {
                    Console.WriteLine(" Generation " + i + " | Average Fitness:" + gen.avgFitness());
                    Console.WriteLine("                  Best Fitness:" + gen.bestFitness());
                }     
            }
            // ---- End Loop ------ //
            
            //Step 4: Generate Reports
            //Generating the maps
            Console.WriteLine("Writing maps...");
            mapReport(nextGeneration);
            Console.WriteLine("Writing progress report...");
            progressReport(genealogy);

            System.Diagnostics.Process.Start(Program.reportname);
        }

        /// <summary>
        /// Reading in the data from the csv file
        /// </summary>
        public static void readData() {
            //==========================================
            //      Reading in the cemetery data
            //==========================================
            TextFieldParser parser = new TextFieldParser(@"data3.csv");
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            int i = 0;

            //Looping through each line
            while (!parser.EndOfData) {

                //Reading the line
                string[] fields = parser.ReadFields();

                //Skipping the first and second row
                i++; if (i == 1 || i == 2) continue;

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
                List<double> temps = new List<double>();
                int j = 0;
                foreach (string f in fields) {
                    j++; if (j <= 7) continue;
                    temps.Add(double.Parse(f));
                }
                c.temps = temps;

                //Adding the cemetery to the list
                cems.Add(c.cemetery_id, c);
                cemsList.Add(c);
            }

            //Closing the parser
            parser.Close();

        }

        /// <summary>
        /// Have the children and the parents duke it out in an epic battle royale to see who emerges victorious
        /// </summary>
        /// <param name="parents">The parents</param>
        /// <param name="children">The kids</param>
        /// <returns>The strongest of the group</returns>
        private static List<genome> elitismTest(List<genome> parents, List<genome> children) {

            //HashSet<genome> parentsAndChildrenList = new HashSet<genome>();
            List<genome> parentsAndChildrenList = new List<genome>(parents);
            parentsAndChildrenList.AddRange(children);
            parentsAndChildrenList.Sort();
            HashSet<genome> nextGeneration = new HashSet<genome>();

            children.Sort();
            parents.Sort();

            int i = 0;
            while (nextGeneration.Count < parents.Count) {

                nextGeneration.Add(parentsAndChildrenList[i]);
                i++;
            }

            return nextGeneration.ToList();
        }

        /// <summary>
        /// Writes information to a javascript file that will generate a map of the path
        /// </summary>
        public static void mapReport(List<genome> genomes) {

            //Initializing the file
            System.IO.StreamWriter file = new System.IO.StreamWriter("genome.js");
                                    
            file.WriteLine("var genomes = new Array();");
            file.WriteLine("var markers");

            //Writing the data for each visit in the genome
            foreach (genome g in genomes) {
                file.WriteLine("markers = new Array();");
                foreach (int i in g.sequence) {
                    cemetery c = cemsList[i];
                    file.WriteLine("markers.push({title:\"" + c.name + "\",lat:" + c.lat + ",lon:" + c.lon + ",temp:"+c.temps[i]+"});");
                }
                file.WriteLine("genomes.push(markers);");
            }
            
            file.Close();
        }

        /// <summary>
        /// Gets map data for a single genome
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static string mapdata(genome g) {

            //Initializing the file
            string data = "var genomes = new Array();\n";
            data += "var markers;\n";
            data += "markers = new Array();";
            foreach (int i in g.sequence) {
                cemetery c = cemsList[i];
                data += "markers.push({title:\"" + c.name + "\",lat:" + c.lat + ",lon:" + c.lon + ",temp:" + c.temps[i] + "});";
            }
            data += "genomes.push(markers);";

            return data;
        }

        /// <summary>
        /// Makes data for the google chart
        /// </summary>
        /// <param name="gens"></param>
        /// <returns></returns>
        public static string chartData(List<generation> gens) {
            string data = "chartdata = new Array();\n";
            int i = 0;
            foreach (generation g in gens) {
                i++;
                if (i % 2 == 0) {   //Only writing every few points
                    double f = g.bestFitness();
                    data += "chartdata.push([" + i + "," + f + "]);\n";                   
                }
            }
            return data;
        }

        /// <summary>
        /// Generating a report about the progress of the algorithm
        /// </summary>
        /// <param name="gens"></param>
        public static void progressReport(List<generation> gens){
            string report = "<html><head>";

            //Adding javascript
            report += "<title>Genetic Optimization Path</title>";
            report += "<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js\"></script>";
            report += "<script type=\"text/javascript\" src=\"http://maps.google.com/maps/api/js?sensor=false\"></script>";
            report += "<script type=\"text/javascript\" >"+mapdata(gens[gens.Count-1].genomes[0])+" \n "+chartData(gens)+" </script>";            
            report += "<script type=\"text/javascript\" src=\"https://www.google.com/jsapi\"></script>";
            report += "<script type=\"text/javascript\" src=\"mapgenome.js\"></script>";
            report += "<style> *{margin:0; padding:0} .map{height:600px; width:100%; } #chart_div{width:100%; height:600px} </style>";
            report += "</head><body>";
            report += "<div id='maps'></div>";
            report += "<div id='chart_div'></div>\n";

            //Parameters
            report += "<table border='1' cellpadding='1'>";
            report += "<tr><td colspan='2'><b>Report Generated "+DateTime.Now+"</b></td></tr>";
            report += "<tr><td>Ideal Temp</td><td>" + Program.idealTemp + "</td></tr>";
            report += "<tr><td>generationSize</td><td>" + Program.generationSize + "</td></tr>";
            report += "<tr><td>tournamentSize</td><td>" + Program.tournamentSize + "</td></tr>";
            report += "<tr><td>chanceOfCrossover</td><td>" + Program.chanceOfCrossover + "</td></tr>";
            report += "<tr><td>chanceOfMutation</td><td>" + Program.chanceOfMutation + "</td></tr>";
            report += "<tr><td>maxCrossoverSize</td><td>" + Program.maxCrossoverSize + "</td></tr>";
            report += "<tr><td>totalGenerations</td><td>" + Program.totalGenerations + "</td></tr>";
            report += "<tr><td>Secods to Run</td><td>" + (DateTime.UtcNow.TimeOfDay.TotalSeconds - Program.start) + "</td></tr>";
            report += "<tr><td>Best Fitness</td><td style='color:#068d5c; font-weight:bold;'>" + gens[gens.Count - 1].avgFitness() + "</td></tr>";
            report += "</table><hr>";

            string csv = "";
            report += "</table></body></html>";

            //Making the file names
            string name = gens[gens.Count - 1].avgFitness() + "-" + Program.chanceOfMutation + "-" + Program.chanceOfCrossover + "-" + Program.totalGenerations;
            Program.reportname = "reports\\" + name + ".html";
            string csvname = "reports\\" + name + ".csv";

            //Writing the html
            System.IO.StreamWriter file = new System.IO.StreamWriter(Program.reportname);
            file.Write(report);
            file.Close();

            //Writing the csv file
            System.IO.StreamWriter csvfile = new System.IO.StreamWriter(csvname);
            csvfile.Write(csv);
            csvfile.Close();

            //Writing to the log
            using (StreamWriter w = File.AppendText("log.csv")) {
                w.WriteLine(gens[gens.Count - 1].avgFitness() + "," + Program.chanceOfMutation + "," + Program.chanceOfCrossover + "," + Program.totalGenerations + "," + Program.generationSize + "," + Program.tournamentSize + "," + Program.maxCrossoverSize);
                w.Close();
            }
        }
    }
}
