namespace MultilayerNetworks.Components
{
    /// <summary>
    /// Class for Actor representation.
    /// </summary>
    public class Actor : BasicComponent
    {
        // Porperties, self explanatory by its name.
        #region Properties
        public int Degree { get; set; }
        public double Closeness { get; set; }
        public double ClusteringCoefficient { get; set; }
        public int NeighborhoodCentrality { get; set; }
        public double DegreeDeviation { get; set; }
        public double ConnectiveRedundancy { get; set; }
        public double Relevance { get; set; }
        public int Occupation { get; set; }
        public int SelectedDegree { get; set; }
        public double SelectedClusteringCoefficient { get; set; }
        public int SelectedNeighborhoodCentrality { get; set; }
        public double SelectedConnectiveRedundancy { get; set; }
        public double SelectedRelevance { get; set; }
        #endregion Properties

        /// <summary>
        /// Creates an actor.
        /// </summary>
        /// <param name="id">Id of the actor.</param>
        /// <param name="name">Name of the actor.</param>
        public Actor(int id, string name = null) : base(id, name)
        {
            Degree = 0;
            Closeness = 0;
            ClusteringCoefficient = 0;
        }
        /// <summary>
        /// String representation of the actor. (its name)
        /// </summary>
        /// <returns>Name of the actor.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
