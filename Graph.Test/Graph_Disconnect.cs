using System;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_Disconnect
    {
        [TestMethod]
        public void Graph_Disconnect_Rejects_UnknownNode()
        {
            var graph = new Graph<int, int>( x => x );

            Assert2.Throws<InvalidOperationException>( () => graph.Disconnect( 0 ) );
        }

        [TestMethod]
        public void Graph_Disconnect_AlreadyDisconnected()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddNode( 0 );
            graph.Disconnect( 0 );

            Assert.AreEqual( 0, graph.GetOutlinks( 0 ).Count );
            Assert.AreEqual( 0, graph.GetInLinks( 0 ).Count );
        }

        [TestMethod]
        public void Graph_Disconnect_RemovesLinks()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 42 );
            graph.AddLink( 1, 2, 84 );

            graph.Disconnect( 0 );

            Assert.AreEqual( 0, graph.GetOutlinks( 0 ).Count );
            Assert.AreEqual( 0, graph.GetInLinks( 0 ).Count );
        }

        /// <summary>
        /// Verifies that a node that transitions from connected to disconnected can be
        /// disconnected multiple times without changing the state of the graph.
        /// </summary>
        [TestMethod]
        public void Graph_Disconnect_RepeatedDisconnect()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 42 );
            graph.AddLink( 1, 2, 84 );

            for( int i = 0; i < 10; i++ )
            {
                graph.Disconnect( 0 );
            }

            Assert.AreEqual( 0, graph.GetOutlinks( 0 ).Count );
            Assert.AreEqual( 0, graph.GetInLinks( 0 ).Count );
        }

        /// <summary>
        /// Verifies that disconnecting a node connected affects the in and outlinks of its former peers.
        /// </summary>
        [TestMethod]
        public void Graph_Disconnect_PeersAreAffected()
        {
            var graph = new Graph<int, int>( x => x );

            // 1 has an in-link from 0.
            graph.AddLink( 0, 1, 42 );

            // 2 has an outlink to 0.
            graph.AddLink( 2, 0, 84 );

            // Add more links to 1 and 2, so that they don't end up alone.
            graph.AddDual( 1, 3, 42 );
            graph.AddDual( 2, 3, 42 );

            // Check the counts before we disconnect. We'll show that after they disconnect, they go down.
            Assert.AreEqual( 2, graph.GetInLinks( 1 ).Count );
            Assert.AreEqual( 2, graph.GetOutlinks( 2 ).Count );

            graph.Disconnect( 0 );

            Assert.AreEqual( 1, graph.GetInLinks( 1 ).Count );
            Assert.AreEqual( 1, graph.GetOutlinks( 2 ).Count );
        }

        /// <summary>
        /// Verifies that nodes that are not connected to some given node are unaffected when that
        /// given node is disconnected.
        /// </summary>
        [TestMethod]
        public void Graph_Disconnect_UninvolvedUnaffected()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 10 );
            graph.AddLink( 2, 0, 20 );

            graph.AddDual( 3, 1, 31 );
            graph.AddDual( 3, 2, 32 );

            Assert.AreEqual( 2, graph.GetOutlinks( 3 ).Count );
            Assert.AreEqual( 2, graph.GetInLinks( 3 ).Count );

            graph.Disconnect( 0 );

            Assert.AreEqual( 2, graph.GetOutlinks( 3 ).Count );
            Assert.AreEqual( 2, graph.GetInLinks( 3 ).Count );
        }
    }
}