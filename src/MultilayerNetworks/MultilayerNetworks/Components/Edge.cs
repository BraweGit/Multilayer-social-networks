namespace MultilayerNetworks.Components
{
    /// <summary>
    /// Class for edge representation.
    /// </summary>
    public class Edge : BasicComponent
    {
        // Porperties, self explanatory by its name.
        public Node V1 { get; set; }
        public Node V2 { get; set; }
        public EdgeDirectionality Directionality { get; set; }

        /// <summary>
        /// Creates edge.
        /// </summary>
        /// <param name="id">Id of the edge.</param>
        /// <param name="node1">First node on the edge.</param>
        /// <param name="node2">Second node on the edge.</param>
        /// <param name="directionality">Direction of the edge.</param>
        /// <param name="name">Name of the edge.</param>
        public Edge(int id, Node node1, Node node2, EdgeDirectionality directionality, string name = null) : base(id, name)
        {
            V1 = node1;
            V2 = node2;
            Directionality = directionality;
        }

        /// <summary>
        /// String representation of the edge.
        /// </summary>
        /// <returns>String value of the edge. </returns>
        public override string ToString()
        {
            switch (Directionality)
            {
                   case EdgeDirectionality.Directed: return "Edge: " + V1 + " -> " + V2;
                   case EdgeDirectionality.Undirected: return "Edge: " + V1 + " -- " + V2;
            }

            return "";
        }

        /// <summary>
        /// Csv representation of the edge. Used for export to csv.
        /// </summary>
        /// <returns>Node_1 and Node_2 in csv format.</returns>
        public string ToCsv()
        {
            switch (Directionality)
            {
                case EdgeDirectionality.Directed: return V1 + ";" + V2;
                case EdgeDirectionality.Undirected: return V1 + ";" + V2;
            }

            return "";
        }


    }
}
