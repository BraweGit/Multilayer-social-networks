using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultilayerNetworks.Components;
using MultilayerNetworks.Measures;

namespace MultilayerNetworks
{
    public class Tests
    {
        private IO io = IO.Instance; //new IO();
        public void TransformationTest()
        {
            var transform = new Transformation.Transformation();

            Console.WriteLine("Transformation test begin...");
            Console.WriteLine("Creating the network...");

            var mnet = new MultilayerNetwork("Transformation Test");
            var a1 = mnet.AddActor("a1");
            var a2 = mnet.AddActor("a2");
            var a3 = mnet.AddActor("a3");
            var a4 = mnet.AddActor("a4");
            var a5 = mnet.AddActor("a5");
            var a6 = mnet.AddActor("a6");
            var i1 = mnet.AddActor("I1");
            var i2 = mnet.AddActor("I2");
            var i3 = mnet.AddActor("I3");
            var L1 = mnet.AddLayer("People 1", EdgeDirectionality.Undirected);
            var L2 = mnet.AddLayer("People 2", EdgeDirectionality.Undirected);
            var people = new HashSet<Layer> {L1, L2};
            var L3 = mnet.AddLayer("Institutions", EdgeDirectionality.Undirected);
            var a1l1 = mnet.AddNode(a1, L1);
            var a2l1 = mnet.AddNode(a2, L1);
            var a3l1 = mnet.AddNode(a3, L1);
            var a4l1 = mnet.AddNode(a4, L1);
            var a5l1 = mnet.AddNode(a5, L1);
            var a1l2 = mnet.AddNode(a1, L2);
            var a2l2 = mnet.AddNode(a2, L2);
            var a3l2 = mnet.AddNode(a3, L2);
            var a4l2 = mnet.AddNode(a4, L2);
            var a5l2 = mnet.AddNode(a5, L2);
            var i1l3 = mnet.AddNode(i1, L3);
            var i2l3 = mnet.AddNode(i2, L3);
            var i3l3 = mnet.AddNode(i3, L3);
            mnet.AddEdge(a1l1, a2l1);
            mnet.AddEdge(a1l1, a3l1);
            mnet.AddEdge(a1l2, a2l2);
            mnet.AddEdge(a3l2, a4l2);
            mnet.AddEdge(a1l2, i1l3);
            mnet.AddEdge(a2l2, i1l3);
            mnet.AddEdge(a2l2, i2l3);
            mnet.AddEdge(a3l2, i2l3);
            mnet.AddEdge(a4l2, i2l3);
            mnet.AddEdge(a5l2, i3l3);
            Console.WriteLine("Creating network end.");
            

            Console.WriteLine("Flattening L1 and L2 (unweighted, only existing actors)...");
            var f1 = transform.FlattenUnweighted(mnet, "flattened1", people, false, false);
            if (mnet.IsDirected(f1, f1)) Console.WriteLine("Layer should be undirected");
            if (mnet.GetNodes(f1).Count != 5) Console.WriteLine("Wrong number of nodes");
            if (mnet.GetEdges(f1, f1).Count != 3) Console.WriteLine("Wrong number of edges");
            Console.WriteLine("Done! " + mnet.ToString());
            Console.WriteLine();

            Console.WriteLine("Flattening L1 and L2 (unweighted, all actors)...");
            var f2 = transform.FlattenUnweighted(mnet, "flattened2", people, false, true);
            if (mnet.IsDirected(f2, f2)) Console.WriteLine("Layer should be undirected");
            if (mnet.GetNodes(f2).Count != 9) Console.WriteLine("Wrong number of nodes, {0}\t != {1}", mnet.GetNodes(f2).Count, 9);
            if (mnet.GetEdges(f2, f2).Count != 3) Console.WriteLine("Wrong number of edges");
            Console.WriteLine("Done! " + mnet.ToString());
            Console.WriteLine();



            Console.WriteLine("Transformation test end.");
            Console.ReadLine();
        }

        public void CsvTest()
        {
            //Console.WriteLine("Csv test begin...");

            //var filePath = @"../../../../Data/aucs.mpx";
            
            //var networkName = "AUCS";
            //var transform = new Transformation.Transformation();

            //var task = io.ReadMultilayer(filePath, networkName);
            //var measures = new Measures.Measures();
            //var deg = new List<int>();
            //foreach (var a in mnet.Actors)
            //{
            //    var dd = measures.Degree(mnet, a, mnet.Layers, EdgeMode.InOut);
            //    if (dd == 49)
            //    {
            //        var st = 10;
            //    }
            //    deg.Add(dd);
            //}
            //var max = deg.Max();
            //transform.FlattenUnweighted(mnet, "Flattened", mnet.Layers, false, false);
            //var layer = mnet.Layers.Where(l => l.Name == "Flattened").FirstOrDefault();
            //var csvPath = @"../../../../Data/" + layer.Name +".csv";

            //io.ToCsv(mnet, layer, csvPath);

            Console.WriteLine("Done!");
            Console.WriteLine("Csv test end.");
        }

        public void RandomTest()
        {
            Console.WriteLine("Random test begin...");

            var utils = MathUtils.Instance; //new MathUtils();
            Console.WriteLine("Random int in range 0 to 10 .... 1000 iterations...");
            var occurences = new int[10];
            for (int i = 0; i < 10; i++)
            {
                occurences[i] = 0;
            }


            for (int i = 0; i < 1000; i++)
            {
                var value = utils.GetRandomInt(10);
                if (value < 0 || value >= 10)
                {
                    Console.WriteLine("Random integer out of range...");
                    break;
                }

                occurences[value]++;
            }

            Console.WriteLine("Done! Occurences per value:");

            for (int i = 0; i < occurences.Length; i++)
            {
                Console.WriteLine("Value: {0}, Occurence: {1}", i, occurences[i]);
            }

            Console.WriteLine("Random double in range 0 to 1 .... 1000 iterations...");
            for (int i = 0; i < 1000; i++)
            {
                var value = utils.GetRandomDouble();
                if (value < 0 || value >= 1)
                {
                    Console.WriteLine("Random double out of range...");
                    break;
                }
            }

            Console.WriteLine("Done!");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(utils.GetRandomDouble());
            }

            Console.WriteLine("Random choice, p = 0.75... 1000 iterations...");
            var outcome = new int[2];
            for (int i = 0; i < outcome.Length; i++)
            {
                outcome[i] = 0;
            }
            for (int i = 0; i < 1000; i++)
            {
                bool value = utils.Test(0.75);
                if (value)
                {
                    outcome[0]++;
                }
                else
                {
                    outcome[1]++;
                }
            }

            Console.WriteLine("Done! Positive and negative outcomes:");
            for (int i = 0; i < outcome.Length; i++)
            {
                Console.WriteLine(outcome[i]);
            }

            Console.WriteLine("Index selection test, prob: 0:0.25, 1:0.25, 2:0.5, 1000 iterations...");
            var options = new double[] {0.25, 0.25, 0.5};
            var hits = new int[] {0, 0, 0};

            for (int i = 0; i < 1000; i++)
            {
                hits[utils.Test(options)]++;
            }

            Console.WriteLine("Done! Hits per value:");

            for (int i = 0; i < hits.Length; i++)
            {
                Console.WriteLine(hits[i]);
            }

        }

        public void MeasuresTest()
        {
            Console.WriteLine("Measures test begin...");

            var measures = new Measures.Measures();
            var transform = new Transformation.Transformation();
            //var filePath = @"../../../../Data/aucs.mpx";
            //var networkName = "AUCS";

            //var mnet = IO.ReadMultilayer(filePath, networkName);
            //var f1 = transform.FlattenUnweighted(mnet, "Flattened", mnet.Layers, false, false);
            //var edges = mnet.EdgesByLayerPair[f1.Id][f1.Id];

            Console.WriteLine("Creating the network...");

            var mnet = new MultilayerNetwork("Transformation Test");
            var byactor = mnet.NodesByActor;
            var a1 = mnet.AddActor("a1");
            var a2 = mnet.AddActor("a2");
            var a3 = mnet.AddActor("a3");
            var a4 = mnet.AddActor("a4");
            var a5 = mnet.AddActor("a5");
            var a6 = mnet.AddActor("a6");
            var i1 = mnet.AddActor("I1");
            var i2 = mnet.AddActor("I2");
            var i3 = mnet.AddActor("I3");
            var L1 = mnet.AddLayer("People 1", EdgeDirectionality.Undirected);
            var L2 = mnet.AddLayer("People 2", EdgeDirectionality.Undirected);
            var people = new HashSet<Layer> { L1, L2 };
            var L3 = mnet.AddLayer("Institutions", EdgeDirectionality.Undirected);
            var a1l1 = mnet.AddNode(a1, L1);
            var a2l1 = mnet.AddNode(a2, L1);
            var a3l1 = mnet.AddNode(a3, L1);
            var a4l1 = mnet.AddNode(a4, L1);
            var a5l1 = mnet.AddNode(a5, L1);
            var a1l2 = mnet.AddNode(a1, L2);
            var a2l2 = mnet.AddNode(a2, L2);
            var a3l2 = mnet.AddNode(a3, L2);
            var a4l2 = mnet.AddNode(a4, L2);
            var a5l2 = mnet.AddNode(a5, L2);
            var i1l3 = mnet.AddNode(i1, L3);
            var i2l3 = mnet.AddNode(i2, L3);
            var i3l3 = mnet.AddNode(i3, L3);
            mnet.AddEdge(a1l1, a2l1);
            mnet.AddEdge(a1l1, a3l1);
            mnet.AddEdge(a1l2, a2l2);
            mnet.AddEdge(a3l2, a4l2);
            mnet.AddEdge(a1l2, i1l3);
            mnet.AddEdge(a2l2, i1l3);
            mnet.AddEdge(a2l2, i2l3);
            mnet.AddEdge(a3l2, i2l3);
            mnet.AddEdge(a4l2, i2l3);
            mnet.AddEdge(a5l2, i3l3);
            Console.WriteLine("Creating network end.");

            Console.WriteLine("Adjaceny matrices test start...");

            var adj = mnet.ToAdjacencyMatrix();

            Console.WriteLine("Adjancency matrices test end.");


            //foreach (var layer in mnet.Layers)
            //{
            //    var nodes = mnet.NodesByLayer[layer.Id];
            //    //Console.WriteLine("{0}\t{1}\t{2}\t{3}\t", layer.Name, nodes.Count, measures.Density(mnet, layer), measures.AverageClusteringCoefficient(mnet, layer));
            //    Console.WriteLine("{0}\t{1}\t{2}\t{3}\t", layer.Name, nodes.Count, measures., measures.AverageClusteringCoefficient(mnet, layer));
            //}



            //var actor = mnet.GetActor("a1");
            //var nodes = mnet.GetNodes(actor);
            //Console.WriteLine("Actor: {0}\t Degree: {1},\t Neighborhood: {2},\t Connective Redundancy: {3},\t Exclusive Neighborhood: {4}", actor.Name,measures.Degree(mnet, actor, mnet.GetLayers(), EdgeMode.InOut),
            //    measures.NeighborhoodCentrality(mnet, actor, mnet.GetLayers(), EdgeMode.InOut),
            //    measures.ConnectiveRedundancy(mnet, actor, mnet.GetLayers(), EdgeMode.InOut),
            //    measures.ExclusiveNeighborhood(mnet, actor, mnet.GetLayers(), EdgeMode.InOut));




            Console.WriteLine("Done! " + mnet.ToString());
            Console.WriteLine("Measures test end.");
        }

        public void RandomWalkTest()
        {
            Console.WriteLine("Random Walk test begin...");

            var filePath = @"../../../../Data/aucs.mpx";

            var networkName = "AUCS";
            var mnet = io.ReadMultilayerAsync(filePath, networkName);
            var measures = new Measures.Measures();


            double[][] transitions = new double[5][];
            transitions[0] = new double[] { 0.40, 0.15, 0.15, 0.15, 0.15 };
            transitions[1] = new double[] { 0.15, 0.40, 0.15, 0.15, 0.15 };
            transitions[2] = new double[] { 0.15, 0.15, 0.40, 0.15, 0.15 };
            transitions[3] = new double[] { 0.15, 0.15, 0.15, 0.40, 0.15 };
            transitions[4] = new double[] { 0.15, 0.15, 0.15, 0.15, 0.40 };


            //var closeness = measures.RandomWalkCloseness(mnet, 0.2, transitions);
            //var betweenness = measures.RandomWalkBetweenness(mnet, 0.2, transitions);
            var st = 10;
        }
    }
}
