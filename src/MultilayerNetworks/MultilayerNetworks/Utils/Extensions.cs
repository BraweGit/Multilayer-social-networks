using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultilayerNetworks.Utils
{
    /// <summary>
    /// Utility class.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Extension for hashset, to get random item.
        /// </summary>
        /// <typeparam name="T">Type of object in the hashset.</typeparam>
        /// <param name="hashSet">Hashset.</param>
        /// <returns>Randomly picked item from the hashset.</returns>
        public static T GetAtRandom<T>(this HashSet<T> hashSet)
        {
            var mathUtils = MathUtils.Instance; //new MathUtils();
            var element = hashSet.ElementAt(mathUtils.GetRandomInt(hashSet.Count));

            return element;
        }
    }
}
