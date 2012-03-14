﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace graves {
    class Program {

        //All cemeteries read in from the data file
        static public Dictionary<int, cemetery> cems = new Dictionary<int, cemetery>();

        static void Main(string[] args) {

            //A friendly welcoming message
            Console.WriteLine("Welcome to the Cemetery Distance Calculator.  Reading the data");

            //Reading in the data
            readData();

            //Pausing after running
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

            }

            //Closing the parser
            parser.Close();

        }
      
        


    }
}
