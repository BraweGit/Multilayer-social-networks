using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultilayerNetworks.Components;

namespace MultilayerNetworks
{
    /// <summary>
    /// Class for reading and writing multilayer networks.
    /// </summary>
    public class IO
    {
        // Singleton
        #region Singleton
        private static IO instance;
        private IO() { }

        public static IO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IO();
                }
                return instance;
            }
        }
        #endregion Singleton

        enum NetworkType { MULTIPLEX_NETWORK, MULTILAYER_NETWORK };
        enum Section { TYPE, LAYERS, ACTOR_ATTRS, EDGE_ATTRS, NODE_ATTRS, ACTORS, EDGES, NODES, DEFAULT };

        /// <summary>
        /// Function reads multilayer network file.
        /// </summary>
        /// <param name="path">Path to the multilayer network file.</param>
        /// <param name="networkName">Name of the multilayer network.</param>
        /// <param name="separator">Separator that is used in the file. Default separator is ','.</param>
        public async Task<Components.MultilayerNetwork> ReadMultilayerAsync(string path, string networkName, char separator = ',')
        {
            var multinet = new Components.MultilayerNetwork(networkName);

            List<AttributeType> actorAttributeTypes = new List<AttributeType>();
            List<string> actorAttributeNames = new List<string>();

            Dictionary<string, Dictionary<string, List<AttributeType>>> edgeAttributeTypes = new Dictionary<string, Dictionary<string, List<AttributeType>>>();
            Dictionary<string, Dictionary<string, List<string>>> edgeAttributeNames = new Dictionary<string, Dictionary<string, List<string>>>();
            Dictionary<string, List<AttributeType>> nodeAttributeTypes = new Dictionary<string, List<AttributeType>>();
            Dictionary<string, List<string>> nodeAttributeNames = new Dictionary<string, List<string>>();

            EdgeDirectionality DefaultEdgeDirectionality = EdgeDirectionality.Undirected;
            NetworkType networkType = NetworkType.MULTIPLEX_NETWORK;
            Section currentSection = Section.DEFAULT;
            EdgeDirectionality dir = DefaultEdgeDirectionality;
            var lineNumber = 0;

            using (var sr = File.OpenText(path))
            {
                var line = string.Empty;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    /*
                    // For debug purposes.
                    if (lineNumber % 10000 == 0)
                    {
                        Console.Clear();
                        Console.WriteLine(lineNumber);
                    }

                    lineNumber++;
                    line = line.TrimEnd();
                    */

                    // Skip empty lines
                    if (line.Equals(""))
                        continue;

                    // Section #TYPE
                    if (line.StartsWith("#TYPE"))
                    {
                        currentSection = Section.TYPE;
                        line = line.ToUpper();

                        if (line.Equals("#TYPE MULTILAYER"))
                        {
                            networkType = NetworkType.MULTILAYER_NETWORK;
                        }
                        else if (line.Equals("#TYPE MULTIPLEX"))
                        {
                            networkType = NetworkType.MULTIPLEX_NETWORK;
                        }
                        else throw new System.FormatException("Type of the network can be only multilayer or multiplex. Error occured on line: " + lineNumber);

                        continue;
                    }

                    if (line.StartsWith("#LAYERS"))
                    {
                        currentSection = Section.LAYERS;
                        continue;
                    }

                    if (line.StartsWith("#ACTOR ATTRIBUTES"))
                    {
                        currentSection = Section.ACTOR_ATTRS;
                        continue;
                    }

                    if (line.StartsWith("#NODE ATTRIBUTES"))
                    {
                        currentSection = Section.NODE_ATTRS;
                        continue;
                    }

                    if (line.StartsWith("#EDGE ATTRIBUTES"))
                    {
                        currentSection = Section.EDGE_ATTRS;
                        continue;
                    }

                    if (line.StartsWith("#EDGES"))
                    {
                        currentSection = Section.EDGES;
                        continue;
                    }

                    if (line.StartsWith("#NODES"))
                    {
                        currentSection = Section.NODES;
                        continue;
                    }

                    if (line.StartsWith("#ACTORS"))
                    {
                        currentSection = Section.ACTORS;
                        continue;
                    }

                    if (currentSection == Section.ACTOR_ATTRS)
                    {
                        var split = line.Split(',');
                        if (split.Length != 2)
                            throw new System.FormatException("For actor attributes, attribute name and type expected. Error occured on line: " + lineNumber);
                        string attrName = split[0];
                        attrName = attrName.ToUpper();
                        if (multinet.ActorFeatures().FirstOrDefault(a => a.Name == attrName) != null)
                            throw new System.FormatException("For actor attributes, attribute name and type expected. Error occured on line: " + lineNumber);
                        AttributeType attrType;
                        if (split[1].Trim() == "NUMERIC") attrType = AttributeType.NumericType;
                        else if (split[1].Trim() == "STRING") attrType = AttributeType.StringType;
                        else throw new System.FormatException("Unsupported attribute type - " + split[1].Trim());
                        //var actorAttribute = new Components.Attribute(attrName, attrType);
                        actorAttributeTypes.Add(attrType);
                        actorAttributeNames.Add(attrName);
                        multinet.ActorFeatures().Add(attrName, attrType);
                    }

                    if (currentSection == Section.NODE_ATTRS)
                    {
                        var split = line.Split(',');
                        if (split.Length != 3)
                            throw new System.FormatException("For node attributes, layer name, attribute name and attribute type expected. Error occured on line: " + lineNumber);
                        string layerName = split[0];
                        string attrName = split[1];
                        if (multinet.NodeFeatures(multinet.GetLayer(layerName)).FirstOrDefault(a => a.Name == attrName) != null)
                            throw new System.FormatException("For actor attributes, attribute name and type expected. Error occured on line: " + lineNumber);
                        AttributeType attrType;
                        split[2] = split[2].ToUpper();
                        if (split[2] == "NUMERIC") attrType = AttributeType.NumericType;
                        else if (split[2] == "STRING") attrType = AttributeType.StringType;
                        else throw new System.FormatException("Unsupported attribute type - " + split[1] + ". Error occured on line: " + lineNumber);
                        //var nodeAttribute = new Components.Attribute(attrName, attrType);

                        if (nodeAttributeTypes.ContainsKey(layerName))
                            nodeAttributeTypes[layerName].Add(attrType);
                        else
                            nodeAttributeTypes.Add(layerName, new List<AttributeType>() {attrType});

                        if (nodeAttributeNames.ContainsKey(layerName))
                            nodeAttributeNames[layerName].Add(attrName);
                        else
                            nodeAttributeNames.Add(layerName, new List<string>() { attrName });

                        multinet.NodeFeatures(multinet.GetLayer(layerName)).Add(attrName, attrType);
                    }

                    if (currentSection == Section.EDGE_ATTRS)
                    {
                        var split = line.Split(',');
                        if (networkType == NetworkType.MULTIPLEX_NETWORK)
                        {
                            if (split.Length != 3)
                                throw new System.FormatException("For edge attributes, layer name, attribute name and type expected. Error occured on line: " + lineNumber);
                            string layerName = split[0];
                            var layer = multinet.GetLayer(layerName);
                            string attrName = split[1];
                            if (multinet.EdgeFeatures(layer, layer).FirstOrDefault(e => e.Name == attrName) != null)
                                throw new System.FormatException("For actor attributes, attribute name and type expected. Error occured on line: " + lineNumber);
                            AttributeType attrType;
                            split[2] = split[2].ToUpper();
                            if (split[2] == "NUMERIC") attrType = AttributeType.NumericType;
                            else if (split[2] == "STRING") attrType = AttributeType.StringType;
                            else throw new System.FormatException("Unsupported attribute type - " + split[1] + ". Error occured on line: " + lineNumber);
                            //var edgeAttribute = new Components.Attribute(attrName, attrType);

                            if (edgeAttributeTypes.ContainsKey(layerName) && 
                                edgeAttributeTypes[layerName].ContainsKey(layerName))
                            {
                                edgeAttributeTypes[layerName][layerName].Add(attrType);
                            }
                            else if (edgeAttributeTypes.ContainsKey(layerName) && !edgeAttributeTypes[layerName].ContainsKey(layerName))
                            {
                                edgeAttributeTypes[layerName].Add(layerName, new List<AttributeType>() {attrType});
                            }
                            else
                            {
                                edgeAttributeTypes.Add(layerName, new Dictionary<string, List<AttributeType>>() { {layerName, new List<AttributeType>() { {attrType} } } });
                            }

                            if (edgeAttributeNames.ContainsKey(layerName) &&
                                edgeAttributeNames[layerName].ContainsKey(layerName))
                            {
                                edgeAttributeNames[layerName][layerName].Add(attrName);
                            }
                            else if (edgeAttributeNames.ContainsKey(layerName) && !edgeAttributeNames[layerName].ContainsKey(layerName))
                            {
                                edgeAttributeNames[layerName].Add(layerName, new List<string>() { attrName });
                            }
                            else
                            {
                                edgeAttributeNames.Add(layerName, new Dictionary<string, List<string>>() { { layerName, new List<string>() { { attrName } } } });
                            }

                            multinet.EdgeFeatures(layer, layer).Add(attrName, attrType);
                        }
                        else if (networkType == NetworkType.MULTILAYER_NETWORK)
                        {
                            if (split.Length != 4)
                                throw new System.FormatException("For edge attributes, layer names, attribute name and type expected. Error occured on line: " + lineNumber);
                            string layerName1 = split[0];
                            Layer layer1 = multinet.GetLayer(layerName1);
                            string layerName2 = split[1];
                            Layer layer2 = multinet.GetLayer(layerName2);
                            string attrName = split[2];
                            if (multinet.EdgeFeatures(layer1, layer2).FirstOrDefault(e => e.Name == attrName) != null)
                                throw new System.FormatException("For actor attributes, attribute name and type expected. Error occured on line: " + lineNumber);
                            AttributeType attrType;
                            split[3] = split[3].ToUpper();
                            if (split[3] == "NUMERIC") attrType = AttributeType.NumericType;
                            else if (split[3] == "STRING") attrType = AttributeType.StringType;
                            else throw new System.FormatException("unsupported attribute type - " + split[2] + ". Error occured on line: " + lineNumber);
                            //var edgeAttribute = new Components.Attribute(attrName, attrType);

                            if (edgeAttributeTypes.ContainsKey(layerName1) &&
                                edgeAttributeTypes[layerName1].ContainsKey(layerName2))
                            {
                                edgeAttributeTypes[layerName1][layerName2].Add(attrType);
                            }
                            else if (edgeAttributeTypes.ContainsKey(layerName1) && !edgeAttributeTypes[layerName1].ContainsKey(layerName2))
                            {
                                edgeAttributeTypes[layerName1].Add(layerName2, new List<AttributeType>() { attrType });
                            }
                            else
                            {
                                edgeAttributeTypes.Add(layerName1, new Dictionary<string, List<AttributeType>>() { { layerName2, new List<AttributeType>() { { attrType } } } });
                            }

                            if (edgeAttributeNames.ContainsKey(layerName1) &&
                                edgeAttributeNames[layerName1].ContainsKey(layerName2))
                            {
                                edgeAttributeNames[layerName1][layerName2].Add(attrName);
                            }
                            else if (edgeAttributeNames.ContainsKey(layerName1) && !edgeAttributeNames[layerName1].ContainsKey(layerName2))
                            {
                                edgeAttributeNames[layerName1].Add(layerName2, new List<string>() { attrName });
                            }
                            else
                            {
                                edgeAttributeNames.Add(layerName1, new Dictionary<string, List<string>>() { { layerName2, new List<string>() { { attrName } } } });
                            }

                            multinet.EdgeFeatures(layer1, layer2).Add(attrName, attrType);
                        }
                    }

                    // Section #LAYERS
                    if (currentSection == Section.LAYERS)
                    {
                        string layerName1 = null, layerName2 = null;
                        string directionality = null;

                        if (networkType == NetworkType.MULTIPLEX_NETWORK)
                        {
                            var split = line.Split(',');
                            if (split.Length < 2)
                            {
                                throw new System.FormatException("Expected Layer name followed by DIRECTED/UNDIRECTED. Error occured on line: " + lineNumber);
                            }

                            layerName1 = layerName2 = split[0];
                            directionality = split[1];
                        }
                        else if (networkType == NetworkType.MULTILAYER_NETWORK)
                        {
                            var split = line.Split(',');
                            if (split.Length < 3)
                            {
                                throw new System.FormatException("Expected two Layer names followed by DIRECTED/UNDIRECTED. Error occured on line: " + lineNumber);
                            }

                            layerName1 = split[0];
                            layerName2 = split[1];
                            directionality = split[2];
                        }

                        line = line.ToUpper();
                        if (directionality.Equals("DIRECTED")) dir = EdgeDirectionality.Directed;
                        else if (directionality.Equals("UNDIRECTED")) dir = EdgeDirectionality.Undirected;
                        else throw new System.FormatException("Either DIRECTED or UNDIRECTED must be specified. Error occured on line: " + lineNumber);

                        if (layerName1.Equals(layerName2))
                        {
                            multinet.AddLayer(layerName1, dir);
                        }
                        else
                        {
                            // TODO first single layers must be specified
                            //LayerSharedPtr layer1 = mnet->get_layer(layer_name1);
                            //LayerSharedPtr layer2 = mnet->get_layer(layer_name2);
                            //mnet->set_directed(layer1, layer2, ((dir == DIRECTED) ? true : false));
                        }

                    }

                    if (currentSection == Section.ACTORS)
                    {
                        var split = line.Split(',');
                        if (split.Length < 1)
                        {
                            throw new System.FormatException("Actor name must be specified. Error occured on line: " + lineNumber);
                        }

                        var actorName = split[0];
                        var actor = multinet.AddActor(actorName);
                        if (actor == null)
                        {
                            // Actor already present
                            actor = multinet.GetActor(actorName);
                        }

                        // TODO: Read attributes.
                        if (actorAttributeTypes.Count != 0 && actorAttributeNames.Count != 0)
                            ReadAttributes(multinet.ActorFeatures(), actor.Id, actorAttributeTypes, actorAttributeNames, split, 1, lineNumber);
                    }

                    if (currentSection == Section.NODES)
                    {
                        var split = line.Split(',');
                        if (split.Length < 2)
                        {
                            throw new System.FormatException("Node name and layer name must be specified. Error occured on line: " + lineNumber);
                        }

                        var nodeName = split[0];
                        var layerName = split[1];

                        var actor = multinet.GetActor(nodeName);
                        if (actor == null)
                        {
                            actor = multinet.AddActor(nodeName);
                        }

                        var layer = multinet.GetLayer(layerName);
                        if (layer == null)
                        {
                            layer = multinet.AddLayer(layerName, DefaultEdgeDirectionality);
                        }

                        var node = multinet.GetNode(actor, layer);
                        if (node == null)
                        {
                            node = multinet.AddNode(actor, layer);
                        }

                        // TODO: Read attributes.
                        if (nodeAttributeTypes.ContainsKey(layer.Name) && nodeAttributeNames.ContainsKey(layer.Name))
                            ReadAttributes(multinet.NodeFeatures(layer), node.Id, nodeAttributeTypes[layer.Name], nodeAttributeNames[layer.Name], split, 2, lineNumber);

                    }

                    if (currentSection == Section.EDGES || currentSection == Section.DEFAULT)
                    {
                        var split = line.Split(',');

                        if (split.Length < 3)
                        {
                            if (currentSection == Section.EDGES)
                            {
                                throw new System.FormatException("From and To node names and layers name must be specified for each edge. Error occured on line: " + lineNumber);
                            }
                            else if (currentSection == Section.DEFAULT)
                            {
                                throw new System.FormatException("Unrecognized statement. Error occured on line: " + lineNumber);
                            }
                        }

                        string fromNode = null;
                        string toNode = null;
                        string layerName1 = null;
                        string layerName2 = null;

                        if (networkType == NetworkType.MULTIPLEX_NETWORK)
                        {
                            fromNode = split[0];
                            toNode = split[1];
                            layerName2 = layerName1 = split[2];
                        }
                        else if (networkType == NetworkType.MULTILAYER_NETWORK)
                        {
                            fromNode = split[0];
                            layerName1 = split[1];
                            toNode = split[2];
                            layerName2 = split[3];
                        }

                        var actor1 = multinet.GetActor(fromNode);
                        if (actor1 == null)
                        {
                            actor1 = multinet.AddActor(fromNode);
                        }

                        var actor2 = multinet.GetActor(toNode);
                        if (actor2 == null)
                        {
                            actor2 = multinet.AddActor(toNode);
                        }

                        //if (actor1.Name == "U107")
                        //{
                        //    var st = 10;
                        //}

                        var layer1 = multinet.GetLayer(layerName1);
                        if (layer1 == null)
                        {
                            layer1 = multinet.AddLayer(layerName1, DefaultEdgeDirectionality);
                        }

                        var layer2 = multinet.GetLayer(layerName2);
                        if (layer2 == null)
                        {
                            layer2 = multinet.AddLayer(layerName2, DefaultEdgeDirectionality);
                        }

                        var node1 = multinet.GetNode(actor1, layer1);
                        if (node1 == null)
                        {
                            node1 = multinet.AddNode(actor1, layer1);
                        }

                        var node2 = multinet.GetNode(actor2, layer2);
                        if (node2 == null)
                        {
                            node2 = multinet.AddNode(actor2, layer2);
                        }

                        var edge = multinet.GetEdge(node1, node2);
                        if (edge == null)
                        {
                            edge = multinet.AddEdge(node1, node2);
                        }

                        // TODO: Read attributes.
                        if (networkType == NetworkType.MULTIPLEX_NETWORK && (edgeAttributeTypes.ContainsKey(layerName1) && edgeAttributeTypes[layerName1].ContainsKey(layerName2)) 
                            && (edgeAttributeNames.ContainsKey(layerName1) && edgeAttributeNames[layerName1].ContainsKey(layerName2)))
                            ReadAttributes(multinet.EdgeFeatures(layer1, layer2), edge.Id, edgeAttributeTypes[layerName1][layerName2], edgeAttributeNames[layerName1][layerName2], split, 3, lineNumber);
                        else if (networkType == NetworkType.MULTILAYER_NETWORK && (edgeAttributeTypes.ContainsKey(layerName1) && edgeAttributeTypes[layerName1].ContainsKey(layerName2))
                                 && (edgeAttributeNames.ContainsKey(layerName1) && edgeAttributeNames[layerName1].ContainsKey(layerName2)))
                            ReadAttributes(multinet.EdgeFeatures(layer1, layer2), edge.Id, edgeAttributeTypes[layerName1][layerName2], edgeAttributeNames[layerName1][layerName2], split, 4, lineNumber);
                    }

                    lineNumber++;
                }
            }
            return multinet;
        }

        /// <summary>
        /// Utility function for reading attribute values.
        /// </summary>
        /// <param name="attributes">Attribute store where the attribute values are kept for the input object.</param>
        /// <param name="objectId">Identifier of the object for which the attributes should be read.</param>
        /// <param name="attrTypes">List with the expected types of attributes.</param>
        /// <param name="attrNames">List with the expected names of attributes.</param>
        /// <param name="line">List of strings where the attribute values are stored.</param>
        /// <param name="idx">The index of the first attribute value in the line vector</param>
        /// <param name="lineNumber">The line of the file at which the line vector has been read, for error management.</param>
        public void ReadAttributes(AttributeCollection<Components.Attribute> attributes, int objectId, List<AttributeType> attrTypes, List<string> attrNames, string[] line, int idx, int lineNumber)
        {
            // Read node attributes
            if (idx + attrNames.Count > line.Length)
                throw new System.FormatException("Not enough attribute values. Error occured on line: " + lineNumber);

            // indexes of current attribute in the local vectors and in the csv file.
            int attrIndex = 0;
            int lineIndex = idx;
            foreach (var attrName in attrNames)
            {
                switch (attrTypes[attrIndex])
                {
                    case AttributeType.NumericType:
                        attributes.SetNumeric(objectId, attrName, Convert.ToDouble(line[lineIndex]));
                        break;
                    case AttributeType.StringType:
                        attributes.SetString(objectId, attrName, line[lineIndex]);
                        break;
                }
                attrIndex++;
                lineIndex++;
            }
        }

        /// <summary>
        /// Writes layer in csv format.
        /// </summary>
        /// <param name="mnet">Multilayer network.</param>
        /// <param name="layer">Layer to write.</param>
        /// <param name="path">Where to write the file.</param>
        public void ToCsv(MultilayerNetwork mnet, Layer layer, string path)
        {
            var csv = new StringBuilder();
            foreach (var edge in mnet.EdgesByLayerPair[layer.Id][layer.Id])
            {
                csv.AppendLine(edge.ToCsv());
            }

            File.WriteAllText(path, csv.ToString());
        }
    }
}