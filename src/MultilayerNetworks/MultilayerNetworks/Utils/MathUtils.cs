using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

namespace MultilayerNetworks
{
    public class MathUtils
    {
        // Singleton
        #region Singleton
        private static MathUtils instance;
        private MathUtils() { }

        public static MathUtils Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MathUtils();
                }
                return instance;
            }
        }
        #endregion Singleton

        /// <summary>
        /// Standard deviation.
        /// </summary>
        /// <param name="values">Values.</param>
        /// <returns>Standard deviation of the values.</returns>
        public double Stdev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }

        /// <summary>
        /// Factorial.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int Factorial(int n)
        {
            if (n >= 2) return n * Factorial(n - 1);
            return 1;
        }

        /// <summary>
        /// Return new random source.
        /// </summary>
        /// <returns></returns>
        private MersenneTwister GetRandomSource()
        {
            // Recommended by library.
            // Might be default, not sure yet.
            return new MersenneTwister(RandomSeed.Robust());
        }

        /// <summary>
        /// Utility method for testing probability.
        /// </summary>
        /// <param name="probability">Probability of occurence.</param>
        /// <returns>True of in prob, false if out.</returns>
        public bool Test(double probability)
        {
            var bernoulliDistribution = new Bernoulli(probability, GetRandomSource());

            return bernoulliDistribution.Sample() == 1 ? true : false;
        }

        /// <summary>
        /// Utility method for probability testing.
        /// </summary>
        /// <param name="options">Probabilities.</param>
        /// <returns>Index of item where test passed.</returns>
        public int Test(double[] options)
        {
            double probFailingPreviousTests = 1;

            for (int i = 0; i < options.Length - 1; i++)
            {
                double adjustedProb = options[i] / probFailingPreviousTests;
                if (Test(adjustedProb))
                {
                    return i;
                }

                probFailingPreviousTests *= (1 - adjustedProb);
            }

            return options.Length - 1;
        }

        /// <summary>
        /// Utility method for probability testing.
        /// </summary>
        /// <param name="options">Probabilities.</param>
        /// <returns>Index of item where test passed.</returns>
        public int Test(double[][] options, int rowNum)
        {
            double probFailingPreviousTests = 1;

            for (int i = 0; i < options[rowNum].Length - 1; i++)
            {
                double adjustedProb = options[rowNum][i] / probFailingPreviousTests;
                if (Test(adjustedProb))
                {
                    return i;
                }

                probFailingPreviousTests *= (1 - adjustedProb);
            }

            return options[rowNum].Length - 1;
        }

        /// <summary>
        /// Returns random integer.
        /// </summary>
        /// <param name="max">Max value.</param>
        /// <returns>Random integer value.</returns>
        public int GetRandomInt(int max)
        {
            return GetRandomSource().Next(0, max);
        }

        /// <summary>
        /// Returns random double.
        /// </summary>
        /// <returns>Random double value.</returns>
        public double GetRandomDouble()
        {
            return GetRandomSource().NextDouble();
        }
    }
}
