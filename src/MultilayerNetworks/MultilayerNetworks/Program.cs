using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultilayerNetworks.Components;

namespace MultilayerNetworks
{
    class Program
    {
        static void Main(string[] args)
        {
            #region todelete
            // Test structures.
            /*
             * var dict = new Dictionary<int, Dictionary<int, int>>() { {1, new Dictionary<int, int> { {1,1} } },
                { 2, new Dictionary<int, int> { {2,2} } } };
            dict[1].Remove(2);
            dict.Remove(1);
            */

            //var attr = new Components.Attribute("Attribute Test Name", AttributeType.StringType);
            //Console.WriteLine(attr.TypeAsString());

            // TODO: Check IO on all data files.
            // TODO: Implement attribute reading, storing and retrieval.
            // TODO: Test all methods in MultilayerNetwork.cs
            // IO Test.
            //var filePath = @"../../../../Data/bankwiring.mpx";
            //var networkName = "bankwiring";

            //var readTest = File.ReadAllText(filePath);

            //var multiNet = IO.ReadMultilayer(filePath, networkName);

            /*
            // Basic components test.
            var actor = new Actor(1, "honza");
            var actor2 = new Actor(2, "sweet");
            var layer = new Layer(4, "facebook");

            var node = new Node(3, actor, layer);
            var node2 = new Node(8, actor2, layer);
            var edge = new Edge(6, node, node2, EdgeDirectionality.Undirected);

            Console.WriteLine(actor.ToString());
            Console.WriteLine(actor2.ToString());
            Console.WriteLine(layer.ToString());

            Console.WriteLine(node.ToString());
            Console.WriteLine(node2.ToString());
            Console.WriteLine(edge.ToString());

            
            // MultilayerNetwork structure test.
            var multiNet = new MultilayerNetwork("multiNet");
            multiNet.AddActor("Honza");
            multiNet.AddEdge(node, node2);
            multiNet.AddNode(actor2, layer);


            var test1 = multiNet.GetNode(actor, layer);
            */

            //Console.WriteLine(multiNet.ToString());
#endregion todelete

            // TODO: Try to have neighbors as an array for faster random pick.
            // TODO: Utils jedinacek
            var tests = new Tests();
            //tests.TransformationTest();
            //tests.MeasuresTest();
            //tests.CsvTest();
            //tests.RandomTest();
            //tests.CsvTest();
            tests.RandomWalkTest();

            Console.WriteLine("Program....Done!");
            Console.ReadLine();
        }
    }
}
