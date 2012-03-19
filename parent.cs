using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace graves {
    /// <summary>
    /// Contains the methods and variables for a parent, which consists of two different genomes
    /// </summary>
    class parent {
        
        public genome mother { get; private set; }
        public genome father { get; private set; }
        public double eta { get; set; }
        public double beta { get; set; }

        public parent(genome motherToSet, genome fatherToSet, double betaToSet) {
            
            this.mother = motherToSet;
            this.father = fatherToSet;
            this.beta = betaToSet;
        }

        /// <summary>
        /// This method performs crossover and mutation to create two children
        /// </summary>
        /// <param name="chanceOfCrossover">The chance of crossover occuring</param>
        /// <param name="chanceOfMutation">The chance of mutation occuring</param>
        /// <returns>A list containing the two children</returns>
        public List<genome> getChildren(double chanceOfCrossover, double chanceOfMutation, double alphaInner) {

            genome child1 = this.mother;
            genome child2 = this.father;
            crossover(chanceOfCrossover, eta, child1, child2);
            mutate(chanceOfMutation, beta, child1);
            mutate(chanceOfMutation, beta, child2);
            List<genome> childList = new List<genome>();
            childList.Add(child1);
            childList.Add(child2);
            return childList;
        }

        private bool crossover(double chanceOfCrossover, double eta, genome child1, genome child2) {

            Random crossoverRand = new Random();
            if (crossoverRand.NextDouble() > chanceOfCrossover)
                return false;

            else {
                for (int i = 0; i < mother.sequence.Count; i++) {

                    double r = crossoverRand.NextDouble();
                    double a = 0;
                    if (r <= 0.5)
                        a = Math.Pow((2 * r), 1 / eta) / 2;
                    else
                        a = 1 - (Math.Pow((2 - 2 * r), 1 / eta) / 2);

                    child1.sequence[i] = (int)Math.Ceiling(a * (double)mother.sequence[i] + (1.0 - a) * (double)father.sequence[i]);
                    child2.sequence[i] = (int)Math.Ceiling((1.0 - a) * (double)mother.sequence[i] + a * (double)father.sequence[i]);
                }
            }
            


            return true;
        }

        private bool mutate(double chanceOfMutation, double beta, genome childToMutate) {

            Random mutateRand = new Random();
            double xmin = 1;
            double xmax = 365;

            for (int i = 0; i < childToMutate.sequence.Count; i++) {
                if (mutateRand.NextDouble() > chanceOfMutation) {
                    continue;
                }
                else {
                    double mutationType = mutateRand.NextDouble() * xmax;
                    if (mutationType <= childToMutate.sequence[i]) {


                    }
                }
            }
            return true;
        }

    }
}
