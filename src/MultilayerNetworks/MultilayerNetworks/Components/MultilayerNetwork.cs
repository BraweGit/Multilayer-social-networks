using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MultilayerNetworks.Components
{
    /// <summary>
    /// Class for multilayer model representation.
    /// </summary>
    public class MultilayerNetwork
    {
        // Porperties, self explanatory by its name.
        #region Properties
        public string Name { get; set; }

        // Sets of components
        public HashSet<Actor> Actors { get; set; }
        public HashSet<Layer> Layers { get; set; }
        public HashSet<Node> Nodes { get; set; }
        public HashSet<Edge> Edges { get; set; }
        public HashSet<Node> EmptyNodes { get; set; }
        public HashSet<Edge> EmptyEdges { get; set; }

        // Directionality between layers. Layer1 -> Layer2 : Directionality
        public Dictionary<int, Dictionary<int, bool>> EdgeDirectionality;

        // For faster lookup
        // Indexes to components (Component IDX):
        public Dictionary<string, Layer> LayerByName;
        public Dictionary<string, Actor> ActorByName;
        // Actor Id, Layer Id -> Node
        public Dictionary<int, Dictionary<int, Node>> NodeByActorAndLayer;
        public Dictionary<int, Dictionary<int, Edge>> EdgeByNodes;

        // Indexes to sets of components(Set IDX):
        public Dictionary<int, HashSet<Node>> NodesByLayer;
        public Dictionary<int, HashSet<Node>> NodesByActor;
        public Dictionary<int, Dictionary<int, HashSet<Edge>>> EdgesByLayerPair;

        public Dictionary<int, HashSet<Node>> NeighborsOut;
        public Dictionary<int, HashSet<Node>> NeighborsIn;
        public Dictionary<int, HashSet<Node>> NeighborsAll;

        // objects storing the feature vectors of the different components
        public AttributeCollection<Attribute> ActorAttributes;
        public AttributeCollection<Attribute> LayerAttributes;
        // Layer ID : Attributes.
        public Dictionary<int, AttributeCollection<Attribute>> NodeAttributes;
        // Layer ID -> Layer Id : Attributes.
        public Dictionary<int, Dictionary<int, AttributeCollection<Attribute>>> EdgeAttributes;

        public EdgeDirectionality DefaultEdgeDirectionality;

        // Maximum IDs of components.
        private int maxActorId;
        private int maxLayerId;
        private int maxNodeId;
        private int maxEdgeId;

        #endregion Properties

        /// <summary>
        /// Creates multilayer network.
        /// </summary>
        /// <param name="name">Name of the network.</param>
        public MultilayerNetwork(string name)
        {
            // Initialize.
            #region Init
            Name = name;
            maxActorId = 0;
            maxEdgeId = 0;
            maxLayerId = 0;
            maxNodeId = 0;

            Actors = new HashSet<Actor>();
            Layers = new HashSet<Layer>();
            Nodes = new HashSet<Node>();
            Edges = new HashSet<Edge>();
            EmptyEdges = new HashSet<Edge>();
            EmptyNodes = new HashSet<Node>();

            EdgeDirectionality = new Dictionary<int, Dictionary<int, bool>>();

            LayerByName = new Dictionary<string, Layer>();
            ActorByName = new Dictionary<string, Actor>();
            NodeByActorAndLayer = new Dictionary<int, Dictionary<int, Node>>();
            EdgeByNodes = new Dictionary<int, Dictionary<int, Edge>>();

            NodesByLayer = new Dictionary<int, HashSet<Node>>();
            NodesByActor = new Dictionary<int, HashSet<Node>>();
            EdgesByLayerPair = new Dictionary<int, Dictionary<int, HashSet<Edge>>>();

            NeighborsOut = new Dictionary<int, HashSet<Node>>();
            NeighborsIn = new Dictionary<int, HashSet<Node>>();
            NeighborsAll = new Dictionary<int, HashSet<Node>>();

            ActorAttributes = new AttributeCollection<Attribute>();
            LayerAttributes = new AttributeCollection<Attribute>();
            // Layer ID : Attributes.
            NodeAttributes = new Dictionary<int, AttributeCollection<Attribute>>();
            // Layer ID -> Layer Id : Attributes.
            EdgeAttributes = new Dictionary<int, Dictionary<int, AttributeCollection<Attribute>>>();

            DefaultEdgeDirectionality = MultilayerNetworks.EdgeDirectionality.Undirected;
            #endregion Init
        }

        /// <summary>
        /// Creates adjacency matrix for given, or all layers.
        /// </summary>
        /// <param name="layer">Specific layer.</param>
        /// <returns>Dictionary where keys are layer Ids and values are adjacency matrices.</returns>
        public Dictionary<int, int[][]> ToAdjacencyMatrix(Layer layer = null)
        {
            var unreachable = 99999;
            var sizeN = 0;
            var layers = new HashSet<Layer>();
            var result = new Dictionary<int, int[][]>();

            if (layer == null)
            {
                sizeN = Actors.Count;
                layers = Layers;
            }
            else
            {
                sizeN = NodesByLayer.Count;
                layers.Add(layer);
            }


            var layerAdjMatrixList = new List<int[][]>();

            //Parallel.ForEach(layers, (l) =>
            foreach (var l in layers)
            {
                // Init matrix.
                var adjMatrix = new int[sizeN][];

                if (!result.ContainsKey(l.Id))
                {
                    result.Add(l.Id, adjMatrix);
                }

                for (int i = 0; i < sizeN; i++)
                {
                    adjMatrix[i] = new int[sizeN];
                }

                for (int i = 0; i < sizeN; i++)
                {
                    for (int j = 0; j < sizeN; j++)
                    {
                        // Node to self = 0.
                        if (i == j)
                        {
                            adjMatrix[i][j] = 0;
                        }
                        // Anyhing else is initialized as unreachable.
                        // Later on if there is edge, it will be overwritten.
                        else
                        {
                            adjMatrix[i][j] = unreachable;
                        }

                    }
                }

                foreach (var edge in GetEdges(l, l))
                {
                    var node1Id = edge.V1.Actor.Id;
                    var node2Id = edge.V2.Actor.Id;

                    // Put edge between Node1 and Node2.
                    adjMatrix[node1Id][node2Id] = 1;

                    // If edge is undirected, put edge in reverse as well.
                    if (edge.Directionality == MultilayerNetworks.EdgeDirectionality.Undirected)
                    {
                        adjMatrix[node2Id][node1Id] = 1;
                    }
                }

            }

            return result;
        }

        /// <summary>
        /// Adds actor to the network.
        /// </summary>
        /// <param name="actorName">Name of the actor.</param>
        /// <returns>Newly added actor.</returns>
        public Actor AddActor(string actorName)
        {
            Actor newActor = GetActor(actorName);
            if (newActor == null)
            {
                var actorId = maxActorId++;
                newActor = new Actor(actorId, actorName);
                Actors.Add(newActor);
                ActorByName.Add(newActor.Name, newActor);
            }
            return newActor;
        }

        /// <summary>
        /// Returns actor by its name or null if doesn't exist.
        /// </summary>
        /// <param name="actorName">Name of the actor we are looking for.</param>
        /// <returns>Actor with given name.</returns>
        public Actor GetActor(string actorName)
        {
            if (ActorByName.ContainsKey(actorName))
            {
                return ActorByName[actorName];
            }   
            return null;
        }

        /// <summary>
        /// Returns layer by its name or null if doesn't exist.
        /// </summary>
        /// <param name="layerName">Name of the layer we are looking for.</param>
        /// <returns>Layer with given name.</returns>
        public Layer GetLayer(string layerName)
        {
            if (LayerByName.ContainsKey(layerName))
            {
                return LayerByName[layerName];
            }
            return null;
        }

        /// <summary>
        /// Adds new layer to the network.
        /// </summary>
        /// <param name="layerName">Name of the new layer.</param>
        /// <param name="directionality">Directed or undirected edges on this layer.</param>
        /// <returns>Newly added layer.</returns>
        public Layer AddLayer(string layerName, EdgeDirectionality directionality)
        {
            Layer newLayer = GetLayer(layerName);
            if (newLayer == null)
            {
                var layerId = maxLayerId++;
                newLayer = new Layer(layerId, layerName);
                Layers.Add(newLayer);
                LayerByName.Add(newLayer.Name, newLayer);

                switch (directionality)
                {
                    case MultilayerNetworks.EdgeDirectionality.Directed: SetDirected(newLayer, newLayer, true); break;
                    case MultilayerNetworks.EdgeDirectionality.Undirected: SetDirected(newLayer, newLayer, false); break;
                }
            }
            return newLayer;
        }


        /// <summary>
        /// Sets edge directionality between two layers.
        /// </summary>
        /// <param name="layer1">First layer.</param>
        /// <param name="layer2">Second layer.</param>
        /// <param name="directed">Directed or undirected edges.</param>
        public void SetDirected(Layer layer1, Layer layer2, bool directed)
        {
            if (!EdgeDirectionality.ContainsKey(layer1.Id))
            {
                EdgeDirectionality.Add(layer1.Id, new Dictionary<int, bool> { { layer2.Id, directed } });
            }
            else if (!EdgeDirectionality[layer1.Id].ContainsKey(layer2.Id))
            {
                EdgeDirectionality[layer1.Id].Add(layer2.Id, directed);
            }

            if (!EdgeDirectionality.ContainsKey(layer2.Id))
            {
                EdgeDirectionality.Add(layer2.Id, new Dictionary<int, bool> { { layer1.Id, directed } });
            }
            else if (!EdgeDirectionality[layer2.Id].ContainsKey(layer1.Id))
            {
                EdgeDirectionality[layer2.Id].Add(layer1.Id, directed);
            }
        }

        /// <summary>
        /// Check if edges between these two layers are directed or not.
        /// </summary>
        /// <param name="layer1">First layer.</param>
        /// <param name="layer2">Second layer.</param>
        /// <returns>True for directed, false for undirected.</returns>
        public bool IsDirected(Layer layer1, Layer layer2)
        {
            if (EdgeDirectionality.ContainsKey(layer1.Id))
            {
                if (EdgeDirectionality[layer1.Id].ContainsKey(layer2.Id))
                {
                    return EdgeDirectionality[layer1.Id][layer2.Id];
                }
            }
            // Default is UNDIRECTED => return false.
            return false;
        }

        /// <summary>
        /// Adds a node on specific layer to the network.
        /// </summary>
        /// <param name="actor">Actor of the node.</param>
        /// <param name="layer">Layer of the node.</param>
        /// <returns>Newly added node.</returns>
        public Node AddNode(Actor actor, Layer layer)
        {
            Node newNode = GetNode(actor, layer);
            if (newNode == null)
            {
                var nodeId = maxNodeId++;
                newNode = new Node(nodeId, actor, layer);
                Nodes.Add(newNode);

                if (!NodesByLayer.ContainsKey(layer.Id))
                {
                    NodesByLayer.Add(layer.Id, new HashSet<Node>());
                }

                NodesByLayer[layer.Id].Add(newNode);

                if (!NodeByActorAndLayer.ContainsKey(actor.Id))
                {
                    NodeByActorAndLayer.Add(actor.Id, new Dictionary<int, Node> { {layer.Id, newNode} });
                }

                if (!NodeByActorAndLayer[actor.Id].ContainsKey(layer.Id))
                {
                    NodeByActorAndLayer[actor.Id].Add(layer.Id, newNode);
                }

                if (!NodesByActor.ContainsKey(actor.Id))
                {
                    NodesByActor.Add(actor.Id, new HashSet<Node> {newNode});
                }

                NodesByActor[actor.Id].Add(newNode);
            }
            return newNode;
        }
        // TODO: Check tryGetValue

        /// <summary>
        /// Node with specific actor on specific layer.
        /// </summary>
        /// <param name="actor">Actor of the node.</param>
        /// <param name="layer">Layer of the node.</param>
        /// <returns>Node if found, or null.</returns>
        public Node GetNode(Actor actor, Layer layer)
        {
            if (NodeByActorAndLayer.ContainsKey(actor.Id) && NodeByActorAndLayer[actor.Id].ContainsKey(layer.Id))
            {
                return NodeByActorAndLayer[actor.Id][layer.Id];
            }
            return null;
        }

        /// <summary>
        /// All nodes in the network.
        /// </summary>
        /// <returns>All nodes in the network.</returns>
        public HashSet<Node> GetNodes()
        {
            return Nodes;
        }

        /// <summary>
        /// Nodes on the specific layer.
        /// </summary>
        /// <param name="layer">Layer we want nodes from.</param>
        /// <returns>All nodes on the specific layer.</returns>
        public HashSet<Node> GetNodes(Layer layer)
        {
            if (!NodesByLayer.ContainsKey(layer.Id))
            {
                return EmptyNodes;
            }
            return NodesByLayer[layer.Id];
        }

        /// <summary>
        /// All nodes of the specific actor.
        /// </summary>
        /// <param name="actor">Actor we want nodes from.</param>
        /// <returns>All nodes with the specific actor.</returns>
        public HashSet<Node> GetNodes(Actor actor)
        {
            if (!NodesByActor.ContainsKey(actor.Id))
            {
                return EmptyNodes;
            }
            return NodesByActor[actor.Id];
        }

        /// <summary>
        /// All layers in the network.
        /// </summary>
        /// <returns>All layers in the network.</returns>
        public HashSet<Layer> GetLayers()
        {
            return Layers;
        }

        /// <summary>
        /// All actors in the network.
        /// </summary>
        /// <returns>All actors in the network.</returns>
        public HashSet<Actor> GetActors()
        {
            return Actors;
        }

        /// <summary>
        /// All edges in the network.
        /// </summary>
        /// <returns>All edges in the network.</returns>
        public HashSet<Edge> GetEdges()
        {
            return Edges;
        }

        /// <summary>
        /// All edges between two given layers.
        /// </summary>
        /// <param name="layer1">First layer.</param>
        /// <param name="layer2">Second layer.</param>
        /// <returns>All edges between two given layers.</returns>
        public HashSet<Edge> GetEdges(Layer layer1, Layer layer2)
        {
            if (!EdgesByLayerPair.ContainsKey(layer1.Id) || !EdgesByLayerPair[layer1.Id].ContainsKey(layer2.Id))
            {
                return EmptyEdges;
            }
            return EdgesByLayerPair[layer1.Id][layer2.Id];
        }

        /// <summary>
        /// Adds an edge between two given nodes.
        /// </summary>
        /// <param name="node1">First node.</param>
        /// <param name="node2">Second node.</param>
        /// <returns>Edge between two nodes if added, if not null.</returns>
        public Edge AddEdge(Node node1, Node node2)
        {
            // If any of the nodes on the edge doesn't exist, we can't add the edge.
            if (node1 == null || node2 == null)
            {
                return null;
            }

            // If edge doesn't exist, create it.
            // It if exists, return it.
            Edge newEdge = GetEdge(node1, node2);
            if (newEdge == null)
            {
                // Create new Id of the new edge, and set direction of the edge.
                var edgeId = maxEdgeId++;
                var edgeDirectionality = IsDirected(node1.Layer, node2.Layer)
                    ? MultilayerNetworks.EdgeDirectionality.Directed
                    : MultilayerNetworks.EdgeDirectionality.Undirected;

                newEdge = new Edge(edgeId, node1, node2, edgeDirectionality);

                // Cache newly added edge.

                if (!EdgeByNodes.ContainsKey(node1.Id))
                {
                    EdgeByNodes.Add(node1.Id, new Dictionary<int, Edge> { {node2.Id, newEdge} });
                }
                else if (!EdgeByNodes[node1.Id].ContainsKey(node2.Id))
                {
                    EdgeByNodes[node1.Id].Add(node2.Id, newEdge);
                }
                else
                {
                    EdgeByNodes[node1.Id][node2.Id] = newEdge;
                }

                if (!NeighborsOut.ContainsKey(node1.Id))
                {
                    NeighborsOut.Add(node1.Id, new HashSet<Node>());
                }

                NeighborsOut[node1.Id].Add(node2);

                if (!NeighborsIn.ContainsKey(node2.Id))
                {
                    NeighborsIn.Add(node2.Id, new HashSet<Node>());
                }

                NeighborsIn[node2.Id].Add(node1);

                if (!NeighborsAll.ContainsKey(node1.Id))
                {
                    NeighborsAll.Add(node1.Id, new HashSet<Node>());
                }

                NeighborsAll[node1.Id].Add(node2);

                if (!NeighborsAll.ContainsKey(node2.Id))
                {
                    NeighborsAll.Add(node2.Id, new HashSet<Node>());
                }

                NeighborsAll[node2.Id].Add(node1);

                if (!EdgesByLayerPair.ContainsKey(node1.Layer.Id))
                {
                    EdgesByLayerPair.Add(node1.Layer.Id,
                        new Dictionary<int, HashSet<Edge>> {{node2.Layer.Id, new HashSet<Edge>() {newEdge}}});
                }
                else if (!EdgesByLayerPair[node1.Layer.Id].ContainsKey(node2.Layer.Id))
                {
                    EdgesByLayerPair[node1.Layer.Id].Add(node2.Layer.Id, new HashSet<Edge>() { newEdge });
                }
                else
                {
                    EdgesByLayerPair[node1.Layer.Id][node2.Layer.Id].Add(newEdge);
                }

                Edges.Add(newEdge);


                if (edgeDirectionality == MultilayerNetworks.EdgeDirectionality.Undirected)
                {
                    if (!EdgeByNodes.ContainsKey(node2.Id))
                    {
                        EdgeByNodes.Add(node2.Id, new Dictionary<int, Edge>() { {node1.Id, newEdge} });
                    }
                    else if (!EdgeByNodes[node2.Id].ContainsKey(node1.Id))
                    {
                        EdgeByNodes[node2.Id].Add(node1.Id, newEdge);
                    }
                    else
                    {
                        EdgeByNodes[node2.Id][node1.Id] = newEdge;
                    }

                    if (!NeighborsOut.ContainsKey(node2.Id))
                    {
                        NeighborsOut.Add(node2.Id, new HashSet<Node>());
                    }

                    NeighborsOut[node2.Id].Add(node1);

                    if (!NeighborsIn.ContainsKey(node1.Id))
                    {
                        NeighborsIn.Add(node1.Id, new HashSet<Node>());
                    }

                    NeighborsIn[node1.Id].Add(node2);

                    if (node1.Layer.Id != node2.Layer.Id)
                    {
                        if (!EdgesByLayerPair.ContainsKey(node2.Layer.Id))
                        {
                            EdgesByLayerPair.Add(node2.Layer.Id,
                                new Dictionary<int, HashSet<Edge>> { { node1.Layer.Id, new HashSet<Edge>() { newEdge } } });
                        }
                        else if (!EdgesByLayerPair[node2.Layer.Id].ContainsKey(node1.Layer.Id))
                        {
                            EdgesByLayerPair[node2.Layer.Id].Add(node1.Layer.Id, new HashSet<Edge>() { newEdge });
                        }
                        else
                        {
                            EdgesByLayerPair[node2.Layer.Id][node1.Layer.Id].Add(newEdge);
                        }
                    }

                }

            }
            return newEdge;
        }

        /// <summary>
        /// Edge between two given nodes.
        /// </summary>
        /// <param name="node1">First node.</param>
        /// <param name="node2">Second node.</param>
        /// <returns>Edge between two given nodes if exists, if not null.</returns>
        public Edge GetEdge(Node node1, Node node2)
        {
            if (node1 == null || node2 == null)
            {
                return null;
            }

            if (EdgeByNodes.ContainsKey(node1.Id) && EdgeByNodes[node1.Id].ContainsKey(node2.Id))
            {
                return EdgeByNodes[node1.Id][node2.Id];
            }

            return null;
        }

        /// <summary>
        /// Erases an edge from the network.
        /// </summary>
        /// <param name="edge">Edge to erase.</param>
        /// <returns>True if erased, false if not.</returns>
        public bool Erase(Edge edge)
        {
            bool result = Edges.Remove(edge);
            if (EdgesByLayerPair.ContainsKey(edge.V1.Layer.Id) && EdgesByLayerPair[edge.V1.Layer.Id].ContainsKey(edge.V2.Layer.Id))
                EdgesByLayerPair[edge.V1.Layer.Id][edge.V2.Layer.Id].Remove(edge);

            if (EdgeByNodes.ContainsKey(edge.V1.Id))
                EdgeByNodes[edge.V1.Id].Remove(edge.V2.Id);

            if (NeighborsIn.ContainsKey(edge.V2.Id))
                NeighborsIn[edge.V1.Id].Remove(edge.V1);

            if (NeighborsOut.ContainsKey(edge.V1.Id))
                NeighborsOut[edge.V1.Id].Remove(edge.V2);

            if (edge.Directionality == MultilayerNetworks.EdgeDirectionality.Directed
                && GetEdge(edge.V2, edge.V1) == null)
            {
                // if the edge is directed, we remove neighbors only if there isn't
                // an edge in the other direction keeping them neighbors
                if (NeighborsAll.ContainsKey(edge.V2.Id))
                    NeighborsAll[edge.V2.Id].Remove(edge.V1);

                if (NeighborsAll.ContainsKey(edge.V1.Id))
                    NeighborsAll[edge.V1.Id].Remove(edge.V2);
            }

            if (edge.Directionality == MultilayerNetworks.EdgeDirectionality.Undirected)
            {
                if (edge.V1.Layer.Id != edge.V2.Layer.Id)
                {
                    if (EdgesByLayerPair.ContainsKey(edge.V2.Layer.Id) && EdgesByLayerPair[edge.V2.Layer.Id].ContainsKey(edge.V1.Layer.Id))
                        EdgesByLayerPair[edge.V2.Layer.Id][edge.V1.Layer.Id].Remove(edge);
                }

                if (EdgeByNodes.ContainsKey(edge.V2.Id))
                    EdgeByNodes[edge.V2.Id].Remove(edge.V1.Id);

                if (NeighborsIn.ContainsKey(edge.V1.Id))
                    NeighborsIn[edge.V1.Id].Remove(edge.V2);

                if (NeighborsOut.ContainsKey(edge.V2.Id))
                    NeighborsOut[edge.V2.Id].Remove(edge.V1);

                if (NeighborsAll.ContainsKey(edge.V1.Id))
                    NeighborsAll[edge.V1.Id].Remove(edge.V2);

                if (NeighborsAll.ContainsKey(edge.V2.Id))
                    NeighborsAll[edge.V2.Id].Remove(edge.V1);
            }

            EdgeFeatures(edge.V1.Layer, edge.V2.Layer).Reset(edge.Id);

            return result;
            //TODO: Check Edge features / attributes.
        }

        /// <summary>
        /// Erases a node from the network.
        /// </summary>
        /// <param name="node">Node to erase.</param>
        /// <returns>True if erased, false if not.</returns>
        public bool Erase(Node node)
        {
            // Removing the node.
            bool result = Nodes.Remove(node);

            if (NodesByLayer.ContainsKey(node.Layer.Id))
                NodesByLayer[node.Layer.Id].Remove(node);

            if (NodesByActor.ContainsKey(node.Actor.Id))
                NodesByActor[node.Layer.Id].Remove(node);          

            if (NodeByActorAndLayer.ContainsKey(node.Actor.Id))
                    NodeByActorAndLayer[node.Actor.Id].Remove(node.Layer.Id);

            // Removing adjacent edges.
            var neighborsIn = Neighbors(node, EdgeMode.In);
            var toRemoveIn = new List<Edge>(neighborsIn.Count);

            foreach (var nodeIn in neighborsIn)
            {
                toRemoveIn.Add(GetEdge(nodeIn, node));
            }

            foreach (var edge in toRemoveIn)
            {
                Erase(edge);
            }

            NodeFeatures(node.Layer).Reset(node.Id);

            return result;
        }      

        /// <summary>
        /// Erases an actor from the network.
        /// </summary>
        /// <param name="actor">Actor to erase.</param>
        /// <returns>True if erased, false if not.</returns>
        public bool Erase(Actor actor)
        {
            bool result = Actors.Remove(actor);

            ActorByName.Remove(actor.Name);
            NodeByActorAndLayer.Remove(actor.Id);

            if (NodesByActor.ContainsKey(actor.Id))
            {
                foreach (var node in NodesByActor[actor.Id])
                {
                    Erase(node);
                }
            }

            NodesByActor.Remove(actor.Id);

            ActorFeatures().Reset(actor.Id);

            return result;
        }

        /// <summary>
        /// Erases a layer from the network.
        /// </summary>
        /// <param name="layer">Layer to erase.</param>
        /// <returns>True if erased, false if not.</returns>
        public bool Erase(Layer layer)
        {
            bool result = Layers.Remove(layer);
            LayerByName.Remove(layer.Name);

            if (NodesByLayer.ContainsKey(layer.Id))
            {
                var toRemove = new List<Node>(NodesByActor[layer.Id].Count);
                foreach (var node in NodesByLayer[layer.Id])
                {
                    toRemove.Add(node);
                }

                foreach (var node in toRemove)
                {
                    Erase(node);
                }
            }

            LayerFeatures().Reset(layer.Id);

            return result;
        }

        /// <summary>
        /// Neighbors of the node.
        /// </summary>
        /// <param name="node">Node which neighbors we want to get.</param>
        /// <param name="edgeMode">Out/In/InOut neighbors.</param>
        /// <returns>Neighbors of the given node.</returns>
        public HashSet<Node> Neighbors(Node node, EdgeMode edgeMode)
        {
            if (edgeMode == EdgeMode.In)
            {
                if (!NeighborsIn.ContainsKey(node.Id))
                    return EmptyNodes;

                return NeighborsIn[node.Id];
            }

            if (edgeMode == EdgeMode.Out)
            {
                if (!NeighborsOut.ContainsKey(node.Id))
                    return EmptyNodes;

                return NeighborsOut[node.Id];
            }

            if (edgeMode == EdgeMode.InOut)
            {
                if (!NeighborsAll.ContainsKey(node.Id))
                    return EmptyNodes;

                return NeighborsAll[node.Id];
            }
            return null;
        }

        /// <summary>
        /// String representation of the network.
        /// </summary>
        /// <returns>String value of the network.</returns>
        public override string ToString()
        {
            var result = "Multilayer Network (" + Name + ", " + Layers.Count + " layers, " +
                         Actors.Count + " actors, " +
                         Nodes.Count + " nodes, " +
                         Edges.Count + " edges)";

            return result;
        }

        /// <summary>
        /// Sets weight of the edge between two given nodes.
        /// </summary>
        /// <param name="node1">First node.</param>
        /// <param name="node2">Second node.</param>
        /// <param name="weight">Weight of the edge.</param>
        public void SetWeight(Node node1, Node node2, double weight)
        {
            var edge = GetEdge(node1, node2);

            if (edge == null)
            {
                throw new KeyNotFoundException("edge between" + node1 + " and " + node2);
            }
            // TODO: edge_features(node1->layer,node2->layer)->setNumeric(edge->id,DEFAULT_WEIGHT_ATTR_NAME,weight);
        }

        /// <summary>
        /// Weight of the edge.
        /// </summary>
        /// <param name="node1">First node.</param>
        /// <param name="node2">Second node.</param>
        /// <returns>Weight of the edge.</returns>
        private double GetWeight(Node node1, Node node2)
        {
            var edge = GetEdge(node1, node2);

            if (edge == null)
            {
                throw new KeyNotFoundException("edge between" + node1 + " and " + node2);
            }

            //EdgeAttributes[node1.Layer.Id][node2.Layer.Id].FirstOrDefault(e=> e.)
            //EdgeFeatures(node1.Layer, node2.Layer).

            // TODO: return edge_features(node1->layer,node2->layer)->getNumeric(edge->id,DEFAULT_WEIGHT_ATTR_NAME);
            return 0f;
        }

        /// <summary>
        /// Attributes of actors.
        /// </summary>
        /// <returns>Actors attributes.</returns>
        public AttributeCollection<Attribute> ActorFeatures()
        {
            return ActorAttributes;
        }

        /// <summary>
        /// Attributes of the layers.
        /// </summary>
        /// <returns>Layers attributes.</returns>
        public AttributeCollection<Attribute> LayerFeatures()
        {
            return LayerAttributes;
        }

        /// <summary>
        /// Attributes of the nodes on specific layer.
        /// </summary>
        /// <param name="layer">Layer to get nodes attributes from.</param>
        /// <returns>Atrributes of nodes on specific layer.</returns>
        public AttributeCollection<Attribute> NodeFeatures(Layer layer)
        {
            if (!NodeAttributes.ContainsKey(layer.Id))
            {
                NodeAttributes[layer.Id] = new AttributeCollection<Attribute>();
            }

            return NodeAttributes[layer.Id];
        }

        /// <summary>
        /// Edges attributes.
        /// </summary>
        /// <param name="layer1">First layer.</param>
        /// <param name="layer2">Second layer.</param>
        /// <returns>Attributes of edges between given layers.</returns>
        public AttributeCollection<Attribute> EdgeFeatures(Layer layer1, Layer layer2)
        {
            if (!EdgeAttributes.ContainsKey(layer1.Id) || !EdgeAttributes[layer1.Id].ContainsKey(layer2.Id))
            {
                EdgeAttributes.Add(layer1.Id, new Dictionary<int, AttributeCollection<Attribute>> { { layer2.Id, new AttributeCollection<Attribute>() } });

                if (!IsDirected(layer1, layer2))
                {
                    EdgeAttributes[layer2.Id][layer1.Id] = EdgeAttributes[layer1.Id][layer2.Id];
                }
            }
            return EdgeAttributes[layer1.Id][layer2.Id];
        }


        // Old code.
        /*
        public string GetAttributeString(List<Attribute> attributes, string attributeName, AttributeType attributeType, int id)
        {
            var attribute = attributes.FirstOrDefault(a => a.Name == attributeName && a.AttributeType == attributeType);

            if (attribute != null)
                return AttributesValuesString[attributeName][id];

            return null;
        }

        public double GetAttributeNumeric(List<Attribute> attributes, string attributeName, AttributeType attributeType, int id)
        {
            var attribute = attributes.FirstOrDefault(a => a.Name == attributeName && a.AttributeType == attributeType);

            if (attribute != null)
                return AttributesValuesNumeric[attributeName][id];

            return -1;
        }

        public void SetNumeric(int objectId, string attributeName, double val)
        {
            if (!AttributesValuesNumeric.ContainsKey(attributeName))
                throw new KeyNotFoundException("Numeric attribute " + attributeName);

            AttributesValuesNumeric[attributeName][objectId] = val;
        }


        public void SetString(int objectId, string attributeName, string val)
        {
            if (!AttributesValuesString.ContainsKey(attributeName))
                throw new KeyNotFoundException("Numeric attribute " + attributeName);

            AttributesValuesString[attributeName][objectId] = val;
        }

        public void ResetAttributes(Actor actor)
        {
            // Numeric attributes.
            AttributesValuesNumeric.Remove(actor.Name);

            // String attributes.
            AttributesValuesString.Remove(actor.Name);
        }


        public void ResetAttributes(Node node)
        {
            // Numeric attributes.
            AttributesValuesNumeric.Remove(node.Name);

            // String attributes.
            AttributesValuesString.Remove(node.Name);
        }

        public void ResetAttributes(Edge edge)
        {
            // Nodes.
            // Numeric attributes.
            AttributesValuesNumeric.Remove(edge.V1.Name);
            AttributesValuesNumeric.Remove(edge.V2.Name);
            // String attributes.
            AttributesValuesString.Remove(edge.V1.Name);
            AttributesValuesString.Remove(edge.V2.Name);

            // Edge.
            // Numeric attributes.
            AttributesValuesNumeric.Remove(edge.Name);

            // String attributes.
            AttributesValuesString.Remove(edge.Name);
        }

        
        public void ResetAttributes(Layer layer)
        {
            // Nodes.
            var toResetNodes = new List<Node>();
            foreach (var edge in Edges)
            {
                if (edge.V1.Layer == layer || edge.V2.Layer == layer)
                {
                    toResetNodes.Add(edge.V1);
                    toResetNodes.Add(edge.V2);
                }
            }

            foreach (var node in toResetNodes)
            {
                ResetAttributes(node);
            }

            // Edges.
            var toResetEdges = new List<Edge>();
            foreach (var edge in Edges)
            {
                if (edge.V1.Layer == layer || edge.V2.Layer == layer)
                {
                    toResetEdges.Add(edge);
                    toResetEdges.Add(edge);
                }
            }

            foreach (var edge in toResetEdges)
            {
                ResetAttributes(edge);
            }

            // Layer.
            // Numeric attributes.
            AttributesValuesNumeric.Remove(layer.Name);
            // String attributes.
            AttributesValuesString.Remove(layer.Name);
        }
        */
        
    }
}
