namespace MultilayerNetworks.Components
{
    /// <summary>
    /// Class for node representation.
    /// </summary>
    public class Node : BasicComponent
    {
        // Porperties, self explanatory by its name.
        #region Properties
        public Actor Actor { get; set; }
        public Layer Layer { get; set; }
        public int Degree { get; set; }
        public double ClosenessCentrality { get; set; }
        public double ClusteringCoefficient { get; set; }
        #endregion Properties
        /// <summary>
        /// Creates node.
        /// </summary>
        /// <param name="id">Id of the node.</param>
        /// <param name="actor">Actor of the node.</param>
        /// <param name="layer">Layer of the node.</param>
        /// <param name="name">Name of the node.</param>
        public Node(int id, Actor actor, Layer layer, string name = null) : base(id, name)
        {
            Actor = actor;
            Layer = layer;
            Degree = 0;
            ClosenessCentrality = 0;
            Name = this.ToString();
        }

        /// <summary>
        /// String representation of the node.
        /// </summary>
        /// <returns>String value of the node.</returns>
        public override string ToString()
        {
            return Actor + "::" + Layer;
        }
    }
}
