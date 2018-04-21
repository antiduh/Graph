using System;
using System.Collections.Generic;
using System.Linq;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_GetShortestPath
    {
        [TestMethod]
        public void Graph_GetShortestPath_Rejects_UnknownNodes()
        {
            var graph = new Graph<int, int>( x => x );

            List<int> path;
            int cost;

            Assert2.Throws<InvalidOperationException>( () => graph.GetShortestPath( 0, 1, out path, out cost ) );
        }

        [TestMethod]
        public void Graph_GetShortestPath_PathDoesNotExist_DisconnectedNodes()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddNode( 0 );
            graph.AddNode( 1 );

            List<int> path;
            int cost;
            bool result;

            result = graph.GetShortestPath( 0, 1, out path, out cost );

            Assert.IsFalse( result );
        }

        [TestMethod]
        public void Graph_GetShortestPath_PathExists_OneWayLine()
        {
            var graph = new Graph<int, int>( x => x );
            int lineLength = 100;

            for( int i = 0; i < lineLength - 1; i++ )
            {
                graph.AddLink( i, i + 1, 1 );
            }
            
            List<int> path;
            int cost;
            bool result;

            result = graph.GetShortestPath( 0, lineLength -1, out path, out cost );

            Assert.IsTrue( result );
            Assert.AreEqual( lineLength - 1, cost );
            Assert.AreEqual( lineLength, path.Count );

            for( int i = 0; i < lineLength; i++ )
            {
                Assert.AreEqual( i, path[i] );
            }
        }

        [TestMethod]
        public void Graph_GetShortestPath_PathExists_2Truss()
        {
            // Constructs a graph similar to the following, where all edges are cost-1:

            /*
             |------- Len = 5 -------|
             |                       |
             0 --> 1 --> 2 --> 3 --> 4
             v     v     v     v     v
             5 --> 6 --> 7 --> 8 --> 9
            */

            // The path is a manhattan path; there are several possible paths, but all are equal cost
            // and equal length. For the sake of discussion, assume we traverse {0, 1, 2, 3, 4, 9}.
            // - The total cost should be 5. There are 5 links between 6 nodes.
            // - The path length should be 6. There are 6 nodes.
            // - The first element in the path should be 0, the start node.
            // - The last element in the path should be 9, the end node.
            //
            // To generalize, for a given Len:
            // - The cost should be Len.
            // - The path should be Len + 1 nodes.
            // - The first node should be 0.
            // - The last node should be `2*Len - 1`, as in `9 = 2*5 - 1`.

            var graph = new Graph<int, int>( x => x );

            int lineLength = 100;

            for( int i = 0; i < lineLength - 1; i++ )
            {
                // Top: 0 --> 1 --> 2, ...
                graph.AddLink( i, i + 1, 1 );

                // Top to bottom: 0 --> 5, 1 --> 6, ...
                graph.AddLink( i, lineLength + i, 1 );
            }

            for( int i = lineLength; i < 2 * lineLength - 1; i++ )
            {
                // Bottom: 5 --> 6 --> 7, ...
                graph.AddLink( i, i + 1, 1 );
            }

            List<int> path;
            int cost;
            bool result;

            result = graph.GetShortestPath( 0, 2 * lineLength - 1, out path, out cost );

            Assert.IsTrue( result );
            Assert.AreEqual( lineLength, cost );
            Assert.AreEqual( lineLength + 1, path.Count );
            Assert.AreEqual( 0, path.First() );
            Assert.AreEqual( 2 * lineLength - 1, path.Last() );
        }

        [TestMethod]
        public void Graph_GetShortestPath_PathExists_HairyLine()
        {
            // Constructs a graph similar to the following, where all edges are cost-1:

            /*
               100   101   102      ...        199
                ^     ^     ^                   ^
                0 --> 1 --> 2 --> 3 --> ... --> 99
                v     v     v                   v
               200   201   202      ...        299
             */

            // This gives the algorithm many dead ends it has to search out.

            var graph = new Graph<int, int>( x => x );

            int lineLength = 100;

            for( int i = 0; i < lineLength - 1; i++ )
            {
                // Backbone, basic link topo.
                graph.AddLink( i, i + 1, 1 );
            }

            for( int i = 0; i < lineLength; i++ )
            {
                // Top hair
                graph.AddLink( i, lineLength + i, 1 );

                // Bottom hair
                graph.AddLink( i, 2 * lineLength + i, 1 );
            }

            List<int> path;
            int cost;
            bool result;

            result = graph.GetShortestPath( 0, lineLength - 1, out path, out cost );

            Assert.IsTrue( result );
            Assert.AreEqual( lineLength - 1, cost );
            Assert.AreEqual( lineLength, path.Count );

            for( int i = 0; i < lineLength; i++ )
            {
                Assert.AreEqual( i, path[i] );
            }
        }
    }
}
