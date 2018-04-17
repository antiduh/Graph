using System;
using System.Collections.Generic;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_GetClosedNetwork
    {
        /// <summary>
        /// Verifies that GetClosedNetwork throws an exception when given an unknown node.
        /// </summary>
        [TestMethod]
        public void Graph_GetClosedNetwork_Rejects_UnknownNode()
        {
            var graph = new Graph<int, int>( x => x );

            Assert2.Throws<InvalidOperationException>( () => graph.GetClosedNetwork( 0 ) );
        }

        /// <summary>
        /// Verifies GetClosedNetwork returns only the given node when the graph contains only one
        /// node, the given node.
        /// </summary>
        [TestMethod]
        public void Graph_GetClosedNetwork_IsolatedNode_SingleNodeGraph()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddNode( 0 );

            var network = graph.GetClosedNetwork( 0 );

            Assert.AreEqual( 1, network.Count );
            Assert.IsTrue( network.Contains( 0 ) );
        }

        /// <summary>
        /// Verifies the closed network of a disconnected node in a graph that contains many
        /// connected nodes contains only itself.
        /// </summary>
        [TestMethod]
        public void Graph_GetClosedNetwork_IsolatedNode_Disconnected_InLargeGraph()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;

            MakeSimpleComplete( graph, count );

            graph.Disconnect( 0 );

            var network = graph.GetClosedNetwork( 0 );

            Assert.AreEqual( 1, network.Count );
            Assert.IsTrue( network.Contains( 0 ) );
        }

        /// <summary>
        /// Verifies that a node that is connected to a larger graph through only outlinks has a
        /// closed network of only itself.
        /// </summary>
        [TestMethod]
        public void Graph_GetClosedNetwork_IsolatedNode_OutlinksOnly_InLargeGraph()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;

            MakeSimpleComplete( graph, count );

            graph.Disconnect( 0 );

            foreach( var node in graph.GetNodes() )
            {
                if( node == 0 ) { continue; }

                graph.AddLink( 0, node, 10 );
            }

            var network = graph.GetClosedNetwork( 0 );

            Assert.AreEqual( 1, network.Count );
            Assert.IsTrue( network.Contains( 0 ) );
        }

        /// <summary>
        /// Verifies that a node that is connected to a larger graph through only inlinks has a
        /// closed network of only itself.
        /// </summary>
        [TestMethod]
        public void Graph_GetClosedNetwork_IsolatedNode_InlinksOnly_InLargeGraph()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;

            MakeSimpleComplete( graph, count );

            graph.Disconnect( 0 );

            foreach( var node in graph.GetNodes() )
            {
                if( node == 0 ) { continue; }

                graph.AddLink( node, 0, 10 );
            }

            var network = graph.GetClosedNetwork( 0 );

            Assert.AreEqual( 1, network.Count );
            Assert.IsTrue( network.Contains( 0 ) );
        }

        /// <summary>
        /// Verifies that a node that is in a simple-complete graph (every node bidi with every other
        /// node) has a closed network that contains the entire graph.
        /// </summary>
        [TestMethod]
        public void Graph_GetClosedNetwork_ConnectedNode_InSimpleCompleteGraph()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;

            MakeSimpleComplete( graph, count );

            var network = graph.GetClosedNetwork( 0 );

            Assert.AreEqual( count, network.Count );

            for( int i = 0; i < count; i++ )
            {
                Assert.IsTrue( network.Contains( i ) );
            }
        }

        /// <summary>
        /// Verifies that a node that has a large network, but is part of a graph that contains
        /// disconnected nodes, has a closed network that contains the whole graph except for the
        /// disconnected nodes.
        /// </summary>
        [TestMethod]
        public void Graph_GetClosedNetwork_ConnectedNode_InLargeGraphWithDisconnectedNodes()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;

            MakeSimpleComplete( graph, count );

            for( int i = 1; i <= 10; i++ )
            {
                graph.Disconnect( i );
            }

            var network = graph.GetClosedNetwork( 0 );

            Assert.AreEqual( count - 10, network.Count );
            Assert.IsTrue( network.Contains( 0 ) );

            for( int i = 11; i < count; i++ )
            {
                Assert.IsTrue( network.Contains( i ) );
            }
        }

        /// <summary>
        /// Verifies that a node that has a large network, but is part of a graph that contains that
        /// are not bidirectional with any other nodes, has a closed network that contains the whole
        /// graph except for the non-bidirectional nodes.
        /// </summary>
        /// <remarks>
        /// A node can have both outlinks and inlinks and still not be bidirectional with any node in
        /// the graph, since some node A must have both an inlink and outlink to the same other node
        /// B in order for node A to have a bidirectional link.
        ///
        /// This test sets up node 0 to be the 'act' node. 0 is part of a bidi simple-complete
        /// subgraph containing most other nodes, excepting the one-way nodes.
        ///
        /// The one-way nodes are connected to node 0, and to node 11. Each oneway node has either
        /// - an outlink to 0 and an inlink from 11, or
        /// - an inlink from 0 and an outlink to 11.
        ///
        /// This shows that even though the nodes have in-links and out-links, and are connected to
        /// several other nodes in the network, they don't count in 0's closed network because none
        /// of these nodes are bidi with any node.
        /// </remarks>
        [TestMethod]
        public void Graph_GetClosedNetwork_ConnectedNode_InLargeGraphWithOneWayNodes()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;

            MakeSimpleComplete( graph, count );

            for( int i = 1; i <= 10; i++ )
            {
                graph.Disconnect( i );
            }

            // Make some of them inlinks going to 0.
            // Make some of them outlinks coming from 0.
            // Make some of them inlinks to 11 (a node bidi with 0).
            // Make some of them outlinks to 11 (a node bidi with 0).

            for( int i = 1; i <= 10; i++ )
            {
                if( i <= 5 )
                {
                    graph.AddLink( i, 0, 10 );
                    graph.AddLink( 11, i, 10 );
                }
                else
                {
                    graph.AddLink( 0, i, 10 );
                    graph.AddLink( i, 11, 10 );
                }
            }

            // At this point, node 0 should be bidi with nodes 11-99.
            // It should be oneway with nodes 1-10, either as inlinks or as outlinks.
            // Node 0's neighbor, 11, should also be oneway (inlinks or outlinks) to nodes 1-10.

            var network = graph.GetClosedNetwork( 0 );

            Assert.AreEqual( count - 10, network.Count );
            Assert.IsTrue( network.Contains( 0 ) );

            for( int i = 11; i < count; i++ )
            {
                Assert.IsTrue( network.Contains( i ) );
            }
        }

        /// <summary>
        /// Verifies that a node's network contains the entire graph, when the graph is a
        /// bidirectional line topology.
        /// </summary>
        [TestMethod]
        public void Graph_GetClosedNetwork_ConnectedNode_LineTopo()
        {
        }

        private void MakeSimpleComplete( Graph<int, int> graph, int numNodes )
        {
            List<int> nodes = new List<int>( numNodes );

            for( int i = 0; i < numNodes; i++ )
            {
                nodes.Add( i );
            }

            GraphMaker.SimpleComplete( graph, nodes, 10 );
        }

        private void MakeBidiLine( Graph<int, int> graph, int numNodes )
        {
            List<int> nodes = new List<int>( numNodes );

            for( int i = 0; i < numNodes; i++ )
            {
                nodes.Add( i );
            }

            GraphMaker.BidiLine( graph, nodes, 10 );
        }
    }
}
