using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultilayerNetworks.Components;

namespace MultilayerNetworks.Transformation
{
    /// <summary>
    /// Class for transformation of multilayer networks (flattening).
    /// </summary>
    public class Transformation
    {
        /// <summary>
        /// Utility method, for creating new layer in the multilayer network.
        /// </summary>
        /// <param name="mnet">Multilayer network.</param>
        /// <param name="newLayerName">Name of the new layer.</param>
        /// <param name="layers">Layers of the network.</param>
        /// <param name="forceDirected">True if new edges should be directed, false if not.</param>
        /// <param name="forceActors">True if all actors should be on the new layer, false if not.</param>
        /// <returns>Newly created layer.</returns>
        private Layer createLayer(MultilayerNetwork mnet, string newLayerName, HashSet<Layer> layers,
            bool forceDirected, bool forceActors)
        {
            var directed = forceDirected;

            // Check if there are any directed layers, if YES, then new layer will be directed as well.
            if (!directed)
            {
                foreach (var layer1 in layers)
                {
                    foreach (var layer2 in layers)
                    {
                        if (mnet.IsDirected(layer1, layer2))
                        {
                            directed = true;
                            break;
                        }                
                    }

                    if (directed) break;
                }
            }

            var dir = directed ? EdgeDirectionality.Directed : EdgeDirectionality.Undirected;
            var newLayer = mnet.AddLayer(newLayerName, dir);
            if (newLayer == null) throw new ArgumentException("Layer " + newLayerName + " already exists.");

            if (forceActors)
            {
                foreach (var actor in mnet.GetActors())
                {
                    mnet.AddNode(actor, newLayer);
                }
            }
            else
            {
                foreach (var layer in layers)
                {
                    foreach (var node in mnet.GetNodes(layer))
                    {
                        mnet.AddNode(node.Actor, newLayer);
                    }              
                }
            }

            return newLayer;
        }

        /// <summary>
        /// Flatten layers into single layer.
        /// </summary>
        /// <param name="mnet">Multilayer network.</param>
        /// <param name="newLayerName">Name of the new layer.</param>
        /// <param name="layers">Layers to flatten.</param>
        /// <param name="forceDirected">True if new edges should be directed, false if not.</param>
        /// <param name="forceActors">True if all actors should be on the new layer, false if not.</param>
        /// <returns>Flattened layer.</returns>
        public Layer FlattenUnweighted(MultilayerNetwork mnet, string newLayerName, HashSet<Layer> layers,
            bool forceDirected = false, bool forceActors = false)
        {
            var newLayer = createLayer(mnet, newLayerName, layers, forceDirected, forceActors);
            var directed = mnet.IsDirected(newLayer, newLayer);

            foreach (var layer1 in layers)
            {
                foreach (var layer2 in layers)
                {
                    foreach (var edge in mnet.GetEdges(layer1, layer2))
                    {
                        var node1 = mnet.GetNode(edge.V1.Actor, newLayer);
                        var node2 = mnet.GetNode(edge.V2.Actor, newLayer);
                        var newEdge = mnet.GetEdge(node1, node2);

                        if (newEdge == null)
                        {
                            newEdge = mnet.AddEdge(node1, node2);
                        }

                        // If new layer is directed, undirected edges must be added twice in both directions.
                        if (directed && edge.Directionality == EdgeDirectionality.Undirected)
                        {
                            newEdge = mnet.GetEdge(node2, node1);
                            if (newEdge == null)
                            {
                                newEdge = mnet.AddEdge(node2, node1);
                            }
                        }
                    }
                }
            }

            return newLayer;
        }
    }
}
