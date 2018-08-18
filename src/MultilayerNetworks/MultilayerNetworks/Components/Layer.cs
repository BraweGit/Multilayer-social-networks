namespace MultilayerNetworks.Components
{
    /// <summary>
    /// Class for layer representation.
    /// </summary>
    public class Layer : BasicComponent
    {
        // Porperties, self explanatory by its name.
        #region Properties
        public int MinDegree { get; set; }
        public int MaxDegree { get; set; }
        public double AvgDegree { get; set; }
        public double AvgDistance { get; set; }
        public double Average { get; set; }
        public int LargestComponent { get; set; }
        public int ConnectedComponentsCount { get; set; }
        public int NodesCount { get; set; }
        public int EdgesCount { get; set; }
        public double Density { get; set; }
        public double AverageClusteringCoefficient { get; set; }
        public double AverageDegreeCentrality { get; set; }
        #endregion Properties

        /// <summary>
        /// Creates layer.
        /// </summary>
        /// <param name="id">Id of the layer.</param>
        /// <param name="name">Name of the layer.</param>
        public Layer(int id, string name = null) : base(id, name){}

        /// <summary>
        /// String representation of the layer.
        /// </summary>
        /// <returns>String value of the layer.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
