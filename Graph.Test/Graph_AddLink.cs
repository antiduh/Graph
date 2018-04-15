﻿using System;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_AddLink
    {
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
