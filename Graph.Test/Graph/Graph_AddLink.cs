using System;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_AddLink
    {
        [TestMethod]
        public void Graph_AddLink_CreatesOneLink()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 42 );

            Assert.AreEqual( 1, graph.GetOutlinks( 0 ).Count );
            Assert.AreEqual( 0, graph.GetInLinks( 0 ).Count );
            Assert.AreEqual( 0, graph.GetOutlinks( 1 ).Count );
            Assert.AreEqual( 1, graph.GetInLinks( 1 ).Count );

            Assert.AreEqual( 42, graph.GetLinkData( 0, 1 ) );
        }

        [TestMethod]
        public void Graph_AddLink_SetsLinkData()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 42 );

            Assert.AreEqual( 42, graph.GetLinkData( 0, 1 ) );
        }

        /// <summary>
        /// Verifies that adding a link with unknown nodes causes the nodes to be added.
        /// </summary>
        [TestMethod]
        public void Graph_AddLink_CreatesNodes()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 42 );

            var nodes = graph.GetNodes();

            Assert.AreEqual( 2, nodes.Count );
            Assert.IsTrue( nodes.Contains( 0 ) );
            Assert.IsTrue( nodes.Contains( 1 ) );
        }

        /// <summary>
        /// Verifies that adding a link with unknown nodes causes the nodes to be added.
        /// </summary>
        [TestMethod]
        public void Graph_AddLink_CreatesNodes_Then_ReusesNodes()
        {
            var graph = new Graph<int, int>( x => x );

            // Create the nodes using the first AddLink
            graph.AddLink( 0, 1, 42 );

            // Create another link involving the same nodes
            graph.AddLink( 1, 0, 42 );

            // Verify we still have two nodes.
            var nodes = graph.GetNodes();

            Assert.AreEqual( 2, nodes.Count );
            Assert.IsTrue( nodes.Contains( 0 ) );
            Assert.IsTrue( nodes.Contains( 1 ) );
        }

        /// <summary>
        /// Verifies that duplicate links are not allowed.
        /// </summary>
        [TestMethod]
        public void Graph_AddLink_Rejects_DuplicateLinks_SameCost()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 1 );

            Assert2.Throws<InvalidOperationException>( () => graph.AddLink( 0, 1, 1 ) );
            Assert2.Throws<InvalidOperationException>( () => graph.AddLink( 0, 1, 2 ) );

            Assert.AreEqual( 1, graph.GetLinkData( 0, 1 ) );
        }

        /// <summary>
        /// Verifies that adding a link to the graph after explicitly declaring the nodes, instead of
        /// letting AddLink create them, works.
        /// </summary>
        [TestMethod]
        public void Graph_AddLink_AfterExplictAddNode()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddNode( 0 );
            graph.AddNode( 1 );

            // Both nodes exist
            graph.AddLink( 0, 1, 10 );
            Assert.AreEqual(10, graph.GetLinkData( 0, 1 ) );

            // Only the start node exists
            graph.AddLink( 1, 2, 20 );
            Assert.AreEqual( 20, graph.GetLinkData( 1, 2 ) );

            // Only the end node exists.
            graph.AddLink( 3, 1, 30 );
            Assert.AreEqual( 30, graph.GetLinkData( 3, 1 ) );
        }
    }
}