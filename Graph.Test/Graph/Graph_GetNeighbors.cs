using System;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_GetNeighbors
    {
        [TestMethod]
        public void Graph_GetNeighbors_Rejects_UnknownNode()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 1, 2, 10 );

            Assert2.Throws<InvalidOperationException>( () => graph.GetNeighbors( 0 ) );
        }

        /// <summary>
        /// Verifies that a known node has no neighbors when it has no links, in either direction, to
        /// any node.
        /// </summary>
        [TestMethod]
        public void Graph_GetNeighbors_NoNeighbors_NoLinks()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddNode( 0 );

            graph.AddDual( 1, 2, 10 );

            Assert.AreEqual( 0, graph.GetNeighbors( 0 ).Count );
        }

        /// <summary>
        /// Verifies that a known node has no neighbors when the only links it has to other nodes are
        /// outlinks from itself to other nodes.
        /// </summary>
        [TestMethod]
        public void Graph_GetNeighbors_NoNeighbors_OnlyOutlinks()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 10 );
            graph.AddLink( 0, 2, 10 );
            graph.AddLink( 0, 3, 10 );

            Assert.AreEqual( 0, graph.GetNeighbors( 0 ).Count );
        }

        /// <summary>
        /// Verifies that a known node has no neighbors when the only links it has to other nodes are 
        /// inlinks from other nodes to itself.
        /// </summary>
        [TestMethod]
        public void Graph_GetNeighbors_NoNeighbors_OnlyInlinks()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 1, 0, 10 );
            graph.AddLink( 2, 0, 10 );
            graph.AddLink( 3, 0, 10 );

            Assert.AreEqual( 0, graph.GetNeighbors( 0 ).Count );
        }

        /// <summary>
        /// Verifies GetNeibhors returns neighbors when a known node has several bidirectional links.
        /// </summary>
        [TestMethod]
        public void Graph_GetNeighbors_ReturnsNeighbors()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddDual( 0, 1, 10 );
            graph.AddDual( 0, 2, 10 );
            graph.AddDual( 0, 3, 10 );

            Assert.AreEqual( 3, graph.GetNeighbors( 0 ).Count );
            Assert.AreEqual( 1, graph.GetNeighbors( 1 ).Count );
            Assert.AreEqual( 1, graph.GetNeighbors( 2 ).Count );
            Assert.AreEqual( 1, graph.GetNeighbors( 3 ).Count );
        }
    }
}
