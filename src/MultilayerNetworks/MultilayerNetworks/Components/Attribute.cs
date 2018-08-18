using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultilayerNetworks.Components
{
    /// <summary>
    /// Class for attribute/parameter representation.
    /// </summary>
    public class Attribute
    {
        // Porperties, self explanatory by its name.
        public string Name { get; set; }
        public AttributeType AttributeType { get; set; }

        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="type">Type of the attribute string/numeric.</param>
        public Attribute(string name, AttributeType type)
        {
            Name = name;
            AttributeType = type;
        }


        /// <summary>
        /// String representation of the attribute.
        /// </summary>
        /// <returns>Type of the attribute.</returns>
        public string TypeAsString()
        {
            return Enum.GetName(typeof(AttributeType), (int)AttributeType);
        }
    }
}
