using System.Runtime.InteropServices.WindowsRuntime;

namespace MultilayerNetworks.Components
{
    /// <summary>
    /// Basic ancestor class for all components in the multilayer network.
    /// </summary>
    public class BasicComponent
    {
        // Porperties, self explanatory by its name.
        public int Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Creates basic component.
        /// </summary>
        /// <param name="id">Id of the basic component.</param>
        /// <param name="name">Name of the basic component.</param>
        public BasicComponent(int id, string name = null)
        {
            Id = id;
            Name = name;
        }

        // Overriding operators, so we can easily compare components.
        public static bool operator ==(BasicComponent a, BasicComponent b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Id == b.Id;
        }

        public static bool operator !=(BasicComponent a, BasicComponent b)
        {
            return !(a == b);
        }

        public static bool operator<(BasicComponent a, BasicComponent b)
        {
            return a.Id < b.Id;
        }

        public static bool operator >(BasicComponent a, BasicComponent b)
        {
            return a.Id > b.Id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
