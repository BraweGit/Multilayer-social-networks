using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using MultilayerNetworks.Components;
using System.Numerics;
using System.Collections.Concurrent;

namespace MultilayerNetworks.Measures
{
    /// <summary>
    /// Class implements measures for networks.
    /// </summary>
    public class Measures
    {
        // Unreachable node in the path matrices.
        private int unreachable = 99999;

        public Measures() {}

        private MathUtils mathUtils = MathUtils.Instance; //new MathUtils();


        #region Degree

        /// <summary>
        /// Returns degree of the actor on provided layers.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor.</param>
        /// <param name="layers">HashSet of layers.</param>
        /// <param name="edgeMode">In / Out / All, mode of edge.</param>
        /// <returns>Degree of the Actor.</returns>
        public int Degree(MultilayerNetwork mnet, Actor actor, HashSet<Layer> layers, EdgeMode edgeMode)
        {
            var degree = 0;
            // All nodes with this ACTOR.
            foreach (var node in mnet.GetNodes(actor))
            {
                // All neighbors of this NODE, with the correct edgeMode (In / Out / Or both).
                foreach (var neighbor in mnet.Neighbors(node, edgeMode))
                {
                    // If there is a layer with this neighbor it means, that this NODE has this neighbor 
                    // on this layer, so we increase his degree by 1.
                    if (layers.Contains(neighbor.Layer))
                        degree += 1;
                }
            }

            return degree;
        }

        /// <summary>
        /// Returns degree on provided layer.
        /// </summary>
        /// <param name="mnet">Multlayer network model.</param>
        /// <param name="actor">Actor.</param>
        /// <param name="layer">Single Layer.</param>
        /// <param name="edgeMode">In / Out / All, mode of edge.</param>
        /// <returns>Degree of the Actor.</returns>
        public int Degree(MultilayerNetwork mnet, Actor actor, Layer layer, EdgeMode edgeMode)
        {
            var degree = 0;
            // All nodes with this ACTOR.
            foreach (var node in mnet.GetNodes(actor))
            {
                // All neighbors of this NODE, with the correct edgeMode (In / Out / Or both).
                foreach (var neighbor in mnet.Neighbors(node, edgeMode))
                {
                    // We go through all his neighbors, and if the neighbor is on the provided layer
                    // we increase his degree by 1.
                    if (neighbor.Layer == layer)
                        degree += 1;
                }
            }

            return degree;
        }

        /// <summary>
        /// Returns means of degrees on the provided layers.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor.</param>
        /// <param name="layers">HashSet of layers.</param>
        /// <param name="edgeMode">In / Out / All, mode of edge.</param>
        /// <returns>Mean of degrees.</returns>
        public double DegreeMean(MultilayerNetwork mnet, Actor actor, HashSet<Layer> layers, EdgeMode edgeMode)
        {
            var degrees = new List<double>();

            foreach (var layer in layers)
            {
                degrees.Add(Degree(mnet, actor, layer, edgeMode));
            }

            return degrees.Average();
        }

        /// <summary>
        /// Returns standard deviation of degrees on the provided layers.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor.</param>
        /// <param name="layers">HashSet of layers.</param>
        /// <param name="edgeMode">In / Out / All, mode of edge.</param>
        /// <returns>Standard deviation of degrees.</returns>
        public double DegreeDeviation(MultilayerNetwork mnet, Actor actor, HashSet<Layer> layers, EdgeMode edgeMode)
        {
            mathUtils = MathUtils.Instance; //new MathUtils();
            var degrees = new List<double>();

            foreach (var layer in layers)
            {
                degrees.Add(Degree(mnet, actor, layer, edgeMode));
            }

            return mathUtils.Stdev(degrees);
        }

        #endregion Degree

        #region Density
        /// <summary>
        /// Returns density of the layer in the network.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="layer">Layer to calculate density on.</param>
        /// <returns>Density of the given layer.</returns>
        public double Density(MultilayerNetwork mnet, Layer layer)
        {
            // Had to use long, because we will be calculating max. potential connections.
            // And integer wasn't enough.

            // Get all nodes on the layer.
            long nodesCount = mnet.NodesByLayer[layer.Id].Count;
            // Get all edges on the layer.
            long edgesCount = 0;
            // Check if there is this layer.
            if (mnet.EdgesByLayerPair.ContainsKey(layer.Id) && mnet.EdgesByLayerPair[layer.Id].ContainsKey(layer.Id))
            {
                edgesCount = mnet.EdgesByLayerPair[layer.Id][layer.Id].Count;
            }

            long potentialConnections = 0;
            if(mnet.IsDirected(layer, layer))
                potentialConnections = nodesCount * (nodesCount - 1);
            else
                potentialConnections = (nodesCount * (nodesCount - 1)) / 2;

            long actualConnections = edgesCount;

            if (actualConnections == 0) return 0;

            return (double)actualConnections / potentialConnections;
        }

        #endregion Density

        #region Clustering Coefficient
        /// <summary>
        /// Returns clustering coefficient of the given node.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="node">Node to calculate clustering coefficient on.</param>
        /// <returns>Clustering coefficient of the given node.</returns>
        public double ClusteringCoefficient(MultilayerNetwork mnet, Node node)
        {
            // Edges between neighbors.
            // How well connected are they.
            double edgesCount = 0;
            // Get all neighbors of the node.
            var neighbors = mnet.Neighbors(node, EdgeMode.InOut);
            // Degree of the node.
            var neighborsCount = neighbors.Count;

            if (neighborsCount <= 1) return 0;

            // Go through all neighbors of our node.
            // And check how well connected they are between each other.
            foreach (var n1 in neighbors)
            {
                foreach (var n2 in neighbors)
                {
                    if (n1 == n2) continue;
                    if (mnet.GetEdge(n1, n2) != null)
                    {
                        edgesCount++;
                    }
                }
            }

            return (edgesCount) / (neighborsCount * (neighborsCount - 1));
        }

        /// <summary>
        /// Returns average clustering coefficient on the given layer.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="layer">Layer to calculate average clustering coefficient on.</param>
        /// <returns>Average clustering coefficient on the given layer.</returns>
        public double AverageClusteringCoefficient(MultilayerNetwork mnet, Layer layer)
        {
            double sumCc = 0;
            var nodes = mnet.NodesByLayer[layer.Id];
            foreach (var n in nodes)
            {
                sumCc += ClusteringCoefficient(mnet, n);
            }

            return (1.0 / nodes.Count) * sumCc;
        }

        // Possible to sort nodes.
        // and in each loop go only through nodes with higher degree and higher id.
        private double ClusteringCoefficient_TODO(MultilayerNetwork mnet, Layer layer)
        {
            // 3 x Number of triangles / number of connected triplets of vertices
            // number of closed triplets / number of connected triplets of vertices
            var triangleCount = 0;
            double tripletsCount = 0;

            var nodes = mnet.NodesByLayer[layer.Id];
            // Count the triangles.
            foreach (var v in nodes)
            {
                foreach (var u in nodes)
                {
                    if (v == u || mnet.GetEdge(v, u) == null) continue;
                    foreach (var w in nodes)
                    {
                        if (u == w || mnet.GetEdge(v, w) == null) continue;

                        // Check for the final path.
                        if (mnet.GetEdge(u, w) != null)
                            triangleCount++;
                    }
                }
            }


            var edgesCount = mnet.EdgesByLayerPair[layer.Id][layer.Id].Count;
            return (3 * triangleCount) / tripletsCount;
        }

        #endregion Clustering Coefficient

        #region Neighborhood Measures

        /// <summary>
        /// Returns neighbors of given actor on given set of layers with specific edgemode.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor to get neighbors of.</param>
        /// <param name="layers">Layers to get neighbors on.</param>
        /// <param name="edgeMode">In/Out/InOut edges.</param>
        /// <returns>Neighbors of given actor on given set of layers with specific edgemode.</returns>
        private HashSet<Actor> Neighbors(MultilayerNetwork mnet, Actor actor, HashSet<Layer> layers, EdgeMode edgeMode = EdgeMode.InOut)
        {
            // Empty hashset to add checked actors.
            var neighbors = new HashSet<Actor>();

            // All nodes with this ACTOR.
            foreach (var node in mnet.GetNodes(actor))
            {
                // All neighbors of this NODE, with the correct edgeMode (In / Out / Or both).
                foreach (var neighbor in mnet.Neighbors(node, edgeMode))
                {
                    // If there is a layer with this neighbor it means, that this NODE has this neighbor 
                    // on this layer, so we increase his degree by 1 IF actor on this node is still available.
                    if (layers.Contains(neighbor.Layer))
                    {
                        neighbors.Add(neighbor.Actor);
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Returns neighbors of given actor on given layer with specific edgemode.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor to get neighbors of.</param>
        /// <param name="layer">Layer to get neighbors on.</param>
        /// <param name="edgeMode">In/Out/InOut edges.</param>
        /// <returns>Neighbors of given actor on given layer with specific edgemode.</returns>
        private HashSet<Actor> Neighbors(MultilayerNetwork mnet, Actor actor, Layer layer, EdgeMode edgeMode = EdgeMode.InOut)
        {
            // Empty hashset to add checked actors.
            var neighbors = new HashSet<Actor>();

            // All nodes with this ACTOR.
            foreach (var node in mnet.GetNodes(actor))
            {
                // All neighbors of this NODE, with the correct edgeMode (In / Out / Or both).
                foreach (var neighbor in mnet.Neighbors(node, edgeMode))
                {
                    // If there is a layer with this neighbor it means, that this NODE has this neighbor 
                    // on this layer, so we increase his degree by 1 IF actor on this node is still available.
                    if (layer == neighbor.Layer)
                    {
                        neighbors.Add(neighbor.Actor);
                    }
                }
            }

            return neighbors;
        }

        // Old implementation.
        private int NeighborhoodCentrality_(MultilayerNetwork mnet, Actor actor, HashSet<Layer> layers, EdgeMode edgeMode = EdgeMode.InOut)
        {
            // Empty hashset to add checked actors.
            var actors = new HashSet<Actor>();
            var neighborhood = 0;

            // All nodes with this ACTOR.
            foreach (var node in mnet.GetNodes(actor))
            {
                // All neighbors of this NODE, with the correct edgeMode (In / Out / Or both).
                foreach (var neighbor in mnet.Neighbors(node, edgeMode))
                {
                    // If there is a layer with this neighbor it means, that this NODE has this neighbor 
                    // on this layer, so we increase his degree by 1 IF actor on this node is still available.
                    if (layers.Contains(neighbor.Layer) && actors.Add(neighbor.Actor))
                    {
                        neighborhood += 1;
                    }
                }
            }

            return neighborhood;
        }

        /// <summary>
        /// Returns neighborhood centrality of given actor on given set of layers with specific edgemode.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor to get neighborhood of.</param>
        /// <param name="layers">Layers to get neighborhood on.</param>
        /// <param name="edgeMode">In/Out/InOut edges.</param>
        /// <returns>Neighborhood centrality of given actor on given set of layers with specific edgemode.</returns>
        public int NeighborhoodCentrality(MultilayerNetwork mnet, Actor actor, HashSet<Layer> layers, EdgeMode edgeMode = EdgeMode.InOut)
        {
            return Neighbors(mnet, actor, layers, edgeMode).Count;
        }

        /// <summary>
        /// Returns neighborhood centrality of given actor on given layer with specific edgemode.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor to get neighborhood of.</param>
        /// <param name="layer">Layer to get neighborhood on.</param>
        /// <param name="edgeMode">In/Out/InOut edges.</param>
        /// <returns>Neighborhood centrality of given actor on given layer with specific edgemode.</returns>
        public int NeighborhoodCentrality(MultilayerNetwork mnet, Actor actor, Layer layer, EdgeMode edgeMode = EdgeMode.InOut)
        {
            return Neighbors(mnet, actor, layer, edgeMode).Count;
        }

        /// <summary>
        /// Returns connective redundancy of given actor on given set of layers with specific edgemode.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor to get connective redundancy of.</param>
        /// <param name="layers">Layers to get connective redundancy on.</param>
        /// <param name="edgeMode">In/Out/InOut edges.</param>
        /// <returns>Connective redundancy of given actor on given set of layers with specific edgemode.</returns>
        public double ConnectiveRedundancy(MultilayerNetwork mnet, Actor actor, HashSet<Layer> layers, EdgeMode edgeMode = EdgeMode.InOut)
        {
            return 1 - Math.Max((NeighborhoodCentrality(mnet, actor, layers, edgeMode) / Degree(mnet, actor, layers, edgeMode)), 0);
        }

        /// <summary>
        /// Returns connective redundancy of given actor.
        /// </summary>
        /// <param name="actor">Actor to get connective redundancy of.</param>
        /// <returns>Connective redundancy of given actor.</returns>
        public double ConnectiveRedundancy(Actor actor)
        {
            return 1 - ((double)actor.NeighborhoodCentrality / actor.Degree);
        }

        /// <summary>
        /// Returns exclusive neighborhood of given actor on given set of layers with specific edgemode.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor to get exclusive neighborhood of.</param>
        /// <param name="layers">Layers to exclusive neighborhood on.</param>
        /// <param name="edgeMode">In/Out/InOut edges.</param>
        /// <returns>Exclusive neighborhood of given actor on given set of layers with specific edgemode.</returns>
        public int ExclusiveNeighborhood(MultilayerNetwork mnet, Actor actor, HashSet<Layer> layers, EdgeMode edgeMode = EdgeMode.InOut)
        {
            // All layers except the layers in parameter.
            var layerSupplement = new HashSet<Layer>(mnet.GetLayers().Except(layers));
            var a = Neighbors(mnet, actor, layers, edgeMode);
            var b = Neighbors(mnet, actor, layerSupplement, edgeMode);

            var aMinusB = new HashSet<Actor>(a.Except(b));

            return aMinusB.Count;
        }

        /// <summary>
        /// Returns exclusive neighborhood of given actor on given layer with specific edgemode.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor to get exclusive neighborhood of.</param>
        /// <param name="layer">Layer to calculate exclusive neighborhood on.</param>
        /// <param name="edgeMode">In/Out/InOut edges.</param>
        /// <returns>Exclusive neighborhood of given actor on given layer with specific edgemode.</returns>
        public int ExclusiveNeighborhood(MultilayerNetwork mnet, Actor actor, Layer layer, EdgeMode edgeMode = EdgeMode.InOut)
        {
            Layer flattened = mnet.GetLayer("flattened");
            // All layers except the layers in parameter.
            var layers = new HashSet<Layer> { layer };
            var layerSupplement = new HashSet<Layer>(mnet.GetLayers().Except(new HashSet<Layer>() { flattened }).Except(layers));
            var a = Neighbors(mnet, actor, layers, edgeMode);
            var b = Neighbors(mnet, actor, layerSupplement, edgeMode);

            var aMinusB = new HashSet<Actor>(a.Except(b));

            return aMinusB.Count;
        }

        /// <summary>
        /// Returns relevance of the actor on given layer with specific edgemode.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor to get relevance of.</param>
        /// <param name="layer">Layer to calculate relevance on.</param>
        /// <param name="edgeMode">In/Out/InOut edges.</param>
        /// <returns>Relevance of the actor on given layer with specific edgemode.</returns>
        public double Relevance(MultilayerNetwork mnet, Actor actor, Layer layer, EdgeMode edgeMode = EdgeMode.InOut)
        {
            double allLayers = NeighborhoodCentrality(mnet, actor, mnet.Layers, edgeMode);
            double selectedLayers = NeighborhoodCentrality(mnet, actor, layer, edgeMode);

            if (allLayers == 0)
                return 0;

            return selectedLayers / allLayers;
        }

        /// <summary>
        /// Returns relevance of the actor on given set of layers.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor to get relevance of.</param>
        /// <param name="layers">Layers to calculate relevance on.</param>
        /// <param name="edgeMode">In/Out/InOut edges.</param>
        /// <returns>Relevance of the actor on given set of layers with specific edgemode.</returns>
        public double Relevance(MultilayerNetwork mnet, Actor actor, HashSet<Layer> layers, EdgeMode edgeMode = EdgeMode.InOut)
        {
            double allLayers = NeighborhoodCentrality(mnet, actor, mnet.Layers, edgeMode);
            double selectedLayers = NeighborhoodCentrality(mnet, actor, layers, edgeMode);

            if (allLayers == 0)
                return 0;

            return selectedLayers / allLayers;
        }


        /// <summary>
        /// Returns exclusive relevance of the actor on given layer with specific edgemode.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor to get exclusive relevance of.</param>
        /// <param name="layer">Layer to calculate exclusive relevance on.</param>
        /// <param name="edgeMode">In/Out/InOut edges.</param>
        /// <returns>Exclusive relevance of the actor on given layer with specific edgemode.</returns>
        public double ExclusiveRelevance(MultilayerNetwork mnet, Actor actor, Layer layer, EdgeMode edgeMode = EdgeMode.InOut)
        {
            Layer flattened = mnet.GetLayer("flattened");
            var test = new HashSet<Layer>(mnet.GetLayers().Except(new HashSet<Layer>() { flattened }));
            double allLayers = NeighborhoodCentrality(mnet, actor, new HashSet<Layer>(mnet.GetLayers().Except(new HashSet<Layer>() { flattened })), edgeMode);
            double selectedLayers = ExclusiveNeighborhood(mnet, actor, layer, edgeMode);

            if (allLayers == 0)
                return 0;

            return selectedLayers / allLayers;
        }

        /// <summary>
        /// Returns exclusive relevance of the actor on given set of layers with specific edgemode.
        /// </summary>
        /// <param name="mnet">Multilayer network model.</param>
        /// <param name="actor">Actor to get exclusive relevance of.</param>
        /// <param name="layers">Layers to calculate exclusive relevance on.</param>
        /// <param name="edgeMode">In/Out/InOut edges.</param>
        /// <returns>Exclusive relevance of the actor on given layer with specific edgemode.</returns>
        public double ExclusiveRelevance(MultilayerNetwork mnet, Actor actor, HashSet<Layer> layers, EdgeMode edgeMode = EdgeMode.InOut)
        {
            double allLayers = NeighborhoodCentrality(mnet, actor, mnet.Layers, edgeMode);
            double selectedLayers = ExclusiveNeighborhood(mnet, actor, layers, edgeMode);

            if (allLayers == 0)
                return 0;

            return selectedLayers / allLayers;
        }



        #endregion Neighborhood Measures


        #region RandomWalk Measures

        /// <summary>
        /// Returns dictionary, where actors are keys and their occupation centrality is value.
        /// </summary>
        /// <param name="mnet">Multilayer network.</param>
        /// <param name="jump">Jump probability for random walker, default value is 0.2.</param>
        /// <param name="transitions">Matrix of transition probabilities between layers. Default is uniform for each pair of layers.</param>
        /// <param name="numSteps">Maximum number of steps, if not specified it is number of edges times 100.</param>
        /// <param name="initialNode">Starting node for the random walker.</param>
        /// <returns>Dictionary, where actors are keys and their occupation centrality is value.</returns>
        public Dictionary<Actor, int> Occupation(MultilayerNetwork mnet, double jump = 0.2, double[][] transitions = null, int numSteps = 0, Node initialNode = null)
        {
            // If not set number of steps, set default number of steps to 100 times number of edges.
            // This was used in the R library.
            if (numSteps == 0)
                numSteps = mnet.Edges.Count * 100;

            // If not set transitions, use uniform probability for every layer.
            transitions = new double[mnet.Layers.Count][];

            for (int i = 0; i < mnet.Layers.Count; i++)
            {
                transitions[i] = new double[mnet.Layers.Count];
                for (int j = 0; j < transitions[i].Length; j++)
                {
                    transitions[i][j] = 1.0 / transitions[i].Length;
                }
            }

            var rw = new Walker(mnet, jump, transitions);

            if (initialNode == null)
            {
                rw.SetInitialNode(mnet.GetNodes().FirstOrDefault());
            }
            else
            {
                rw.SetInitialNode(initialNode);
            }

            var occupation = new Dictionary<Actor, int>();

            var node = rw.Now();
            var flag = true;

            while (numSteps-- > 0)
            {
                if (flag)
                {
                    // Initial node.
                    flag = false;
                }
                else
                {
                    node = rw.Next();
                    
                }
                
                if (!rw.Action())
                {
                    continue;
                }

                if (rw.Jumped())
                {
                    // Jumped to a new node - not counting.
                }
                else
                {
                    // If actor is not present yet.
                    if (!occupation.ContainsKey(node.Actor))
                    {
                        occupation.Add(node.Actor, 0);
                    }
                    // Then increase his occupancy.
                    occupation[node.Actor]++;
                }
            }

            return occupation;
        }

        /// <summary>
        /// Utility method, to count number of times random walker crossed the actor.
        /// </summary>
        /// <param name="rw">Random walker.</param>
        /// <param name="a">From node.</param>
        /// <param name="b">To node.</param>
        /// <param name="actorRandomWalkCount">Dictionary, where actors are keys, and number of walkes passed through them is their value.</param>
        private void RandomWalkActors(Walker rw, Node a, Node b, ref ConcurrentDictionary<Actor, int> actorRandomWalkCount)
        {
            int walkLen = 1;
            var passed = new HashSet<Actor>();

            // From same node to the same node.
            if (a.Actor == b.Actor)
            {
                //actorRandomWalkCount[a.Actor.Id] += 1;
                //actorRandomWalkCount[b.Actor.Id] += 1;
                passed.Add(a.Actor);
            }

            var node = rw.SetInitialNode(a);
            int numSteps = rw.mnet.GetEdges().Count;
            

            while (numSteps-- > 0)
            {
                node = rw.Next();
                if (!rw.Action())
                {
                    continue;
                }

                if (rw.Jumped())
                {
                    // Jumped to a new node - not counting.
                }
                else
                {
                    // Add node.
                    //actorRandomWalkCount[node.Actor.Id] += 1;
                    passed.Add(node.Actor);
                }

                // Check if we are at the node we are looking for.
                if (node.Actor == b.Actor)
                {
                    foreach (var actor in passed)
                    {
                        actorRandomWalkCount[actor] += 1;
                    }

                    return;
                }
            }


        }

        /// <summary>
        /// Utility method, to count steps of random walker.
        /// </summary>
        /// <param name="rw">Random walker.</param>
        /// <param name="a">From node.</param>
        /// <param name="b">To node.</param>
        /// <returns>Number of steps it took to reach from node a to node b.</returns>
        private int RandomWalkSteps(Walker rw, Node a, Node b)
        {
            int walkLen = 1;

            // From same node to the same node.
            if (a.Actor == b.Actor)
                return walkLen;

            var node = rw.SetInitialNode(a);
            int numSteps = rw.mnet.GetEdges().Count;

            while (numSteps-- > 0)
            {
                node = rw.Next();
                if (!rw.Action())
                {
                    continue;
                }

                if (rw.Jumped())
                {
                    // Jumped to a new node - not counting.
                }
                else
                {
                    // Increase the length of the walk.
                    walkLen++;
                }

                // Check if we are at the node we are looking for.
                if(node.Actor == b.Actor)
                {
                    return walkLen;
                }
            }

            return walkLen;
        }

        /// <summary>
        /// Returns dictionary, where actors are keys and their random walk closeness centrality is value.
        /// </summary>
        /// <param name="mnet">Multilayer network.</param>
        /// <param name="jump">Jump probability, default value is 0.2.</param>
        /// <param name="transitions">Matrix of transition probabilities between layers. Default is uniform for each pair of layers.</param>
        /// <returns>Dictionary, where actors are keys and their random walk closeness centrality is value.</returns>
        public ConcurrentDictionary<Actor, double> RandomWalkCloseness(MultilayerNetwork mnet, double jump = 0.2, double[][] transitions = null)
        {
            // If not set transitions, use uniform probability for every layer.
            transitions = new double[mnet.Layers.Count][];

            for (int i = 0; i < mnet.Layers.Count; i++)
            {
                transitions[i] = new double[mnet.Layers.Count];
                for (int j = 0; j < transitions[i].Length; j++)
                {
                    transitions[i][j] = 1.0 / transitions[i].Length;
                }
            }

            var rw = new Walker(mnet, jump, transitions);

            // Create adjacency matrix for actors.
            // Every actor has its Id.
            // First Id starts at 0.
            // Last Id is actors count - 1.
            var adjActors = new int[mnet.GetActors().Count][];
            for (int i = 0; i < mnet.Actors.Count; i++)
            {
                adjActors[i] = new int[mnet.Actors.Count];
            }

            // Traverse the matrix for every pair of nodes.
            var nodes = mnet.GetNodes().ToArray();
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                //for (int j = i + 1; j < nodes.Length; j++)
                Parallel.For(i + 1, nodes.Length, (j) =>
                {
                    //Console.WriteLine(i + ":" + j);
                    // Do random walk from i to j.
                    // And save only the first walk length.
                    var walkLen = RandomWalkSteps(rw, nodes[i], nodes[j]);

                    // Use only first walk.
                    if (adjActors[nodes[i].Actor.Id][nodes[j].Actor.Id] == 0)
                    {
                        adjActors[nodes[i].Actor.Id][nodes[j].Actor.Id] = walkLen;
                        adjActors[nodes[j].Actor.Id][nodes[i].Actor.Id] = walkLen;
                    }

                });
            }

            var actorById = new ConcurrentDictionary<int, Actor>();
            //foreach (var actor in mnet.GetActors())
            Parallel.ForEach(mnet.GetActors(), (a) =>
            {
                actorById.TryAdd(a.Id, a);
            });

            // Actual Clonesess centrality.
            // Harmonic mean distance.
            var closenessByActor = new ConcurrentDictionary<Actor, double>();
            for (int i = 0; i < adjActors[0].Length; i++)
            {
                double sum = 0;
                //for (int j = 0; j < adjActors[0].Length; j++)
                Parallel.For(0, adjActors[0].Length, (j) =>
                {
                    if (i == j)
                    {
                        adjActors[i][j] = 0;
                    }
                    else
                    {
                        if (adjActors[i][j] != 0)
                        {
                            sum += 1.0 / adjActors[i][j];
                        }
                        
                    }
                });

                double closeness = (1.0 / (adjActors[0].Length - 1)) * sum;
                closenessByActor[actorById[i]] = closeness;
            }

            return closenessByActor;
        }

        // Do for each layer random walk for every pair of nodes on the layer.
        // Average the value over all possible starting layers for the actor.

        /// <summary>
        /// Returns dictionary, where actors are keys and random walk betweenness centrality is their value.
        /// </summary>
        /// <param name="mnet">Multilayer network.</param>
        /// <param name="jump">Jump probability for random walker, default value is 0.2.</param>
        /// <param name="transitions">Matrix of transition probabilities between layers. Default is uniform for each pair of layers.</param>
        /// <returns>Dictionary, where actors are keys and random walk betweenness centrality is their value.</returns>
        public ConcurrentDictionary<Actor, int> RandomWalkBetweenness(MultilayerNetwork mnet, double jump = 0.2, double[][] transitions = null)
        {
            // If not set transitions, use uniform probability for every layer.
            transitions = new double[mnet.Layers.Count][];

            for (int i = 0; i < mnet.Layers.Count; i++)
            {
                transitions[i] = new double[mnet.Layers.Count];
                for (int j = 0; j < transitions[i].Length; j++)
                {
                    transitions[i][j] = 1.0 / transitions[i].Length;
                }
            }

            var rw = new Walker(mnet, jump, transitions);

            // Number of random walks passing through given actor.
            var actorRandomWalkCount = new ConcurrentDictionary<Actor, int>();
            //foreach (var a in mnet.GetActors())
            Parallel.ForEach(mnet.GetActors(), (a) =>
            {
                actorRandomWalkCount.TryAdd(a, 0);
            });

            // Do random walk between every pair of nodes.
            // Average it over the layers.
            var nodes = mnet.GetNodes().ToArray();
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                //for (int j = i + 1; j < nodes.Length; j++)
                Parallel.For(i + 1, nodes.Length, j =>
                {
                    RandomWalkActors(rw, nodes[i], nodes[j], ref actorRandomWalkCount);
                });
            }


            var temp = new ConcurrentDictionary<Actor, int>(actorRandomWalkCount);
            // Average it - divide by number of nodes with the corresponding actor.
            //foreach (var a in temp.Keys)
            Parallel.ForEach(temp.Keys, (a) =>
            {
                if (mnet.NodesByActor.ContainsKey(a.Id))
                {
                    var nodesCount = mnet.NodesByActor[a.Id].Count;
                    actorRandomWalkCount[a] = temp[a] / nodesCount;
                }

            });

            return actorRandomWalkCount;
        }

        #endregion RandomWalk Measures

        #region Other

        public int[][] FloydWarshall(int[][] matrix)
        {
            //var unreachable = 99999;
            int[][] dist = new int[matrix.Length][];

            // Initilize distance matrix.

            for (int i = 0; i < matrix.Length; i++)
            {
                dist[i] = new int[matrix.Length];

                for (int j = 0; j < matrix.Length; j++)
                {
                    dist[i][j] = matrix[i][j];
                }

            }

            for (int k = 0; k < matrix.Length; k++)
            {
                //for (int i = 0; i < matrix.Length; i++)
                Parallel.For(0, matrix.Length, i =>
                {
                    for (int j = 0; j < matrix.Length; j++)
                    {
                        if (dist[i][k] + dist[k][j] < dist[i][j])
                            dist[i][j] = dist[i][k] + dist[k][j];
                    }

                });
            }

            return dist;
        }

        public double ClosenessCentrality(Actor actor, Layer layer, Dictionary<int, int[][]> floydWarshallMatrixByLayerId, MultilayerNetwork mnet)
        {
            var distanceMatrix = floydWarshallMatrixByLayerId[layer.Id];
            var sumDist = 0.0;

            for (int i = 0; i < distanceMatrix.Length; i++)
            {
                if (distanceMatrix[actor.Id][i] < 9999)
                {
                    sumDist += distanceMatrix[actor.Id][i];
                }  
            }

            long n = mnet.NodesByLayer[layer.Id].Count;

            return 1.0 / ((1.0 / (n - 1)) * sumDist);
        }

        public double GetAverageDistance(MultilayerNetwork mnet, Layer layer, Dictionary<int, int[][]> floydWarshallMatrixByLayerId)
        {
            var distanceMatrix = floydWarshallMatrixByLayerId[layer.Id];
            var length = distanceMatrix.Length;

            var total = 0.0;
            var nodesCount = mnet.NodesByLayer[layer.Id].Count;
            for (int i = 0; i < length; i++)
                for (int j = i + 1; j < length; j++)
                {
                    if (distanceMatrix[i][j] < unreachable)
                    {
                        total += distanceMatrix[i][j];
                    }

                }


            var result = (2d / (nodesCount * (nodesCount - 1))) * total;

            return result;
        }

        public int GetAverage(Layer layer, Dictionary<int, int[][]> floydWarshallMatrixByLayerId)
        {
            var distanceMatrix = floydWarshallMatrixByLayerId[layer.Id];
            var length = distanceMatrix.Length;

            int result = 0;
            for (int i = 0; i < length; i++)
            {
                for (int j = i + 1; j < length; j++)
                {
                    if (distanceMatrix[i][j] > result && distanceMatrix[i][j] < unreachable)
                        result = distanceMatrix[i][j];
                }

            }       

            return result;
        }

        public HashSet<Node> DFS(MultilayerNetwork multilayerNetwork, Node initialNode, Layer layer)
        {
            var visited = new HashSet<Node>();

            //var nodes = multilayerNetwork.NodesByLayer[layer.Id];
            //var initialNode = nodes.FirstOrDefault();

            var stack = new Stack<Node>();
            stack.Push(initialNode);

            while (stack.Count > 0)
            {
                var node = stack.Pop();

                if (visited.Contains(node))
                    continue;

                visited.Add(node);

                foreach (var neighbor in multilayerNetwork.NeighborsAll[node.Id])
                {
                    if (!visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                    }
                }

            }

            return visited;
        }

        public int[] GetConnectedComponents(MultilayerNetwork multilayerNetwork, Layer layer)
        {
            var connectedComponents = new List<HashSet<Node>>();
            var visited = new HashSet<Node>();
            var nodes = multilayerNetwork.NodesByLayer[layer.Id];

            var componentsCount = 0;
            var largestComponent = 0;

            foreach (var n in nodes)
            {
                if (!visited.Contains(n))
                {
                    // DFS.
                    var newComponent = new HashSet<Node>();
                    var stack = new Stack<Node>();
                    stack.Push(n);

                    while (stack.Count > 0)
                    {
                        var node = stack.Pop();
                        
                        if (newComponent.Contains(node))
                            continue;

                        newComponent.Add(node);
                        visited.Add(node);

                        foreach (var neighbor in multilayerNetwork.NeighborsAll[node.Id])
                        {
                            if (!newComponent.Contains(neighbor))
                            {
                                stack.Push(neighbor);
                            }
                        }
                    }

                    if (newComponent.Count > largestComponent)
                        largestComponent = newComponent.Count;

                    componentsCount++;

                    //connectedComponents.Add(newComponent);
                    newComponent = new HashSet<Node>();
                }
            }

            return new int[] { componentsCount, largestComponent };
        }

        #endregion

        // Old recursive implementation
        #region Old
        public List<List<Node>> GetConnectedComponents_(MultilayerNetwork multilayerNetwork, Layer layer)
        {
            var connectedComponents = new List<List<Node>>();

            var visited = new Dictionary<int, bool>();
            var nodes = multilayerNetwork.NodesByLayer[layer.Id];
            foreach (var node in nodes)
            {
                visited.Add(node.Id, false);
            }

            var newComponent = new List<Node>();

            for (int i = 0; i < visited.Count; i++)
            {
                var node = visited.ElementAt(i);
                if (!node.Value)
                {
                    DfsUtil(node.Key, visited, newComponent, nodes, multilayerNetwork);
                    connectedComponents.Add(newComponent);
                    newComponent = new List<Node>();
                }
            }

            return connectedComponents;
        }

        public void DfsUtil(int nodeId, Dictionary<int, bool> visited, List<Node> component, HashSet<Node> nodes, MultilayerNetwork multilayerNetwork)
        {
            visited[nodeId] = true;
            var theNode = nodes.First(n => n.Id == nodeId);

            component.Add(theNode);

            foreach (var neighbors in multilayerNetwork.NeighborsAll[theNode.Id])
            {
                var node = visited.First(x => x.Key == neighbors.Id);
                if (!node.Value)
                    DfsUtil(node.Key, visited, component, nodes, multilayerNetwork);
            }
        }

        #endregion Old
    }
}
