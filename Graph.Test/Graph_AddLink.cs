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
        }

        [TestMethod]
        public void Graph_AddLink_SetsLinkData()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 42 );

            Assert.AreEqual( 42, graph.GetOutlinks( 0 )[0].LinkData );
            Assert.AreEqual( 42, graph.GetInLinks( 1 )[0].LinkData );
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
        }
    }
}
