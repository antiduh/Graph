using System;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_GetOpenNetwork
    {
        [TestMethod]
        public void Graph_GetOpenNetwork_Rejects_UnknownNodes()
        {
            var graph = new Graph<int, int>( x => x );

            Assert2.Throws<InvalidOperationException>( () => graph.GetOpenNetwork( 0 ) );
        }

        [TestMethod]
        public void Graph_GetOpenNetwork_DisconnectedNode()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;
            var nodes = MakeNodes( count );

            GraphMaker.SimpleComplete( graph, nodes, 10 );

            graph.Disconnect( 0 );

            var network = graph.GetOpenNetwork( 0 );

            Assert.AreEqual( 1, network.Count );
            Assert.AreEqual( 0, network[0] );
        }

        [TestMethod]
        public void Graph_GetOpenNetwork_InlinksOnly()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;
            var nodes = MakeNodes( count );

            GraphMaker.SimpleComplete( graph, nodes, 10 );

            graph.Disconnect( 0 );

            for( int i = 1; i < count; i++ )
            {
                graph.AddLink( i, 0, 10 );
            }

            var network = graph.GetOpenNetwork( 0 );

            Assert.AreEqual( 1, network.Count );
            Assert.AreEqual( 0, network[0] );
        }

        [TestMethod]
        public void Graph_GetOpenNetwork_OutlinksOnly()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;
            var nodes = MakeNodes( count );

            GraphMaker.SimpleComplete( graph, nodes, 10 );

            graph.Disconnect( 0 );

            for( int i = 1; i < count; i++ )
            {
                graph.AddLink( 0, i, 10 );
            }

            var network = graph.GetOpenNetwork( 0 );

            Assert.AreEqual( 100, network.Count );

            for( int i = 0; i < count; i++ )
            {
                Assert.IsTrue( network.Contains( i ) );
            }
        }

        [TestMethod]
        public void Graph_GetOpenNetwork_DisconnectedPeer()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;
            var nodes = MakeNodes( count );

            GraphMaker.SimpleComplete( graph, nodes, 10 );

            graph.Disconnect( 1 );

            var network = graph.GetOpenNetwork( 0 );

            Assert.AreEqual( 99, network.Count );

            for( int i = 0; i < count; i++ )
            {
                if( i == 1 ) { continue; }
                Assert.IsTrue( network.Contains( i ) );
            }
        }

        [TestMethod]
        public void Graph_GetOpenNetwork_UnreachablePeer()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;
            var nodes = MakeNodes( count );

            GraphMaker.SimpleComplete( graph, nodes, 10 );

            // Disconnect 1, and then make it so that it has only outlinks to other nodes.
            graph.Disconnect( 1 );

            for( int i = 0; i < count; i++ )
            {
                if( i == 1 ) { continue; }
                graph.AddLink( 1, i, 10 );
            }

            var network = graph.GetOpenNetwork( 0 );

            Assert.AreEqual( 99, network.Count );

            for( int i = 0; i < count; i++ )
            {
                if( i == 1 ) { continue; }
                Assert.IsTrue( network.Contains( i ) );
            }
        }

        [TestMethod]
        public void Graph_GetOpenNetwork_DirectedLineAsymmetry()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;
            var nodes = MakeNodes( count );

            // 0 --> 1 --> 2 ....
            GraphMaker.DirectedLine( graph, nodes, 10 );

            var firstNetwork = graph.GetOpenNetwork( 0 );
            var lastNetwork = graph.GetOpenNetwork( count - 1 );

            Assert.AreEqual( count, firstNetwork.Count );
            for( int i = 0; i < count; i++ )
            {
                Assert.IsTrue( firstNetwork.Contains( i ) );
            }

            Assert.AreEqual( 1, lastNetwork.Count );
        }

        private int[] MakeNodes( int count )
        {
            int[] nodes = new int[count];

            for( int i = 0; i < count; i++ )
            {
                nodes[i] = i;
            }

            return nodes;
        }
    }
}
