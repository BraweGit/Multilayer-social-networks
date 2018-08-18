using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultilayerNetworks.Components;

namespace MultilayerNetworks.Measures
{
/// <summary>
/// Class for random walk on multilayer network.
/// </summary>
    public class Walker
    {
        public MultilayerNetwork mnet;
        private Node current;
        private double jump;
        private double[][] transitions;
        private bool justJumped;
        private bool noAction;
        private Dictionary<int, int> layerIds;
        private MathUtils mathUtils;

        /// <summary>
        /// Returns initial node.
        /// </summary>
        /// <param name="initialNode">Initial node.</param>
        /// <returns>Initial node.</returns>
        public Node SetInitialNode(Node initialNode)
        {
            current = initialNode;
            return current;
        }

        public Walker(MultilayerNetwork multilayerNetwork, double jumpProbability, double[][] layerTransitions)
        {
            mathUtils = MathUtils.Instance;//new MathUtils();

            mnet = multilayerNetwork;
            jump = jumpProbability;
            transitions = layerTransitions;
            layerIds = new Dictionary<int, int>();

            justJumped = true;
            noAction = true;

            var i = 0;
            Layer flattened = mnet.GetLayer("flattenedNetwork");
            var layers = new HashSet<Layer>(mnet.GetLayers().Except(new HashSet<Layer>() { flattened }));
            foreach (var layer in layers)
            {
                layerIds.Add(layer.Id, i);
                i++;
            }
        }

        /// <summary>
        /// Current position of the walker.
        /// </summary>
        /// <returns>Current position of the walker.</returns>
        public Node Now()
        {
            return current;
        }

        /// <summary>
        /// Moves the walker
        /// </summary>
        /// <returns>Node where walker moved onto.</returns>
        public Node Next()
        {
            if (mathUtils.Test(jump))
            {
                current = Utils.Extensions.GetAtRandom(mnet.GetNodes());
                justJumped = true;
                noAction = false;
            }
            else
            {
                Layer flattened = mnet.GetLayer("flattenedNetwork");
                var layerId = layerIds[current.Layer.Id];
                var layerIdTest = mathUtils.Test(transitions, layerId);
                var newLayer = mnet.GetLayers().Except(new HashSet<Layer>() { flattened }).ElementAt(layerIdTest);

                if (current.Layer == newLayer)
                {
                    // Inside same layer.
                    var neigh = mnet.Neighbors(current, EdgeMode.Out);
                    if (neigh.Count == 0)
                    {
                        // Cant move.
                        // No action.
                        noAction = true;
                        return current;
                    }

                    var rand = mathUtils.GetRandomInt(neigh.Count);
                    current = neigh.ElementAt(rand);
                    justJumped = false;
                    noAction = false;
                }
                else
                {
                    // Changing to another node with this actor.
                    var nextNode = mnet.GetNode(current.Actor, newLayer);
                    if (nextNode == null)
                    {
                        // No other nodes with this actor.
                        noAction = true;
                        return current;
                    }

                    current = nextNode;
                    justJumped = false;
                    noAction = false;
                }
            }

            return current;
        }

        /// <summary>
        /// Returns true if walker just jumped.
        /// </summary>
        /// <returns>True if actor just jumped, false if not.</returns>
        public bool Jumped()
        {
            return justJumped;
        }

        /// <summary>
        /// Returns true if walker has no action to make.
        /// </summary>
        /// <returns>True if action is not possible, false if possible.</returns>
        public bool Action()
        {
            return !noAction;
        }
    }
}
