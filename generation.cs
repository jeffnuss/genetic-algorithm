using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace graves {
    class generation {
        public List<genome> genomes = new List<genome>();

        public generation(List<genome> g) {
            this.genomes = g;
        }

        /// <summary>
        /// Calculating the average fitness of the generation
        /// </summary>
        /// <returns></returns>
        public double fitness() {
            return genomes[0].fitness;
        }

        /// <summary>
        /// Calculating the average fitness of the generation
        /// </summary>
        /// <returns></returns>
        public double avgFitness(){
            int num = 0;
            double total = 0;
            foreach (genome g in genomes) {
                num++;
                total += g.fitness;
            }            

            return total / (float)num ;
        }


        //Returning the best fitness
        public double bestFitness() {
            int num = 0;
            double total = genomes[0].fitness;
            foreach (genome g in genomes) {
                num++;
                if (g.fitness < total)
                    total = g.fitness;
            }
            return total;
        }

        /// <summary>
        /// Returning the worst fitness
        /// </summary>
        /// <returns></returns>
        public double worstFitness() {
            int num = 0;
            double total = 1000000000;
            foreach (genome g in genomes) {
                num++;
                if (g.fitness < total)
                    total = g.fitness;
            }
            return total;
        }

        

    }
}
