using System;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    /// <summary>
    /// Tests the behavior of the graph for scenarios related to loopback behavior.
    /// </summary>
    [TestClass]
    public class Graph_LoopbackScenarios
    {
        /// <summary>
        /// Verifies that Graph.AddLink will refuse to add more than one loopback link.
        /// </summary>
        [TestMethod]
        public void Graph_AddLink_Rejects_DuplicateLoopback()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 0, 10 );

            Assert2.Throws<InvalidOperationException>( () => graph.AddLink( 0, 0, 20 ) );
        }

        /// <summary>
        /// Verifies that Graph.AddDual will refuse to add a bidi loopback link.
        /// </summary>
        [TestMethod]
        public void Graph_AddDual_Rejects_AnyLoopback()
        {
            var graph = new Graph<int, int>( x => x );

            Assert2.Throws<InvalidOperationException>( () => graph.AddDual( 0, 0, 10 ) );
        }

        /// <summary>
        /// Verifies that Graph.AddLink can be used to create loopback links.
        /// </summary>
        [TestMethod]
        public void Graph_AddLink_Loopback()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 0, 10 );

            Assert.AreEqual( 10, graph.GetLinkData( 0, 0 ) );
            Assert.AreEqual( 1, graph.GetNodes().Count );
        }

        /// <summary>
        /// Verifies that Graph.RemoveLink can be used to remove loopback links.
        /// </summary>
        [TestMethod]
        public void Graph_RemoveLink_Loopback()
        {
            var graph = new Graph<int, int>( x => x );
            int dummy;

            graph.AddLink( 0, 0, 10 );

            graph.RemoveLink( 0, 0 );

            Assert.AreEqual( 1, graph.GetNodes().Count );
            Assert.AreEqual( 0, graph.GetNodes()[0] );
            Assert.IsFalse( graph.TryGetLinkData( 0, 0, out dummy ) );
        }

        /// <summary>
        /// Verifies that a loopback link's data can be modified.
        /// </summary>
        [TestMethod]
        public void Graph_SetLink_Loopback()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 0, 10 );

            graph.SetLink( 0, 0, 20 );

            Assert.AreEqual( 20, graph.GetLinkData( 0, 0 ) );
        }

        /// <summary>
        /// Verifies that GetLink rejects operations trying to fetch link data for non-existant
        /// loopback links.
        /// </summary>
        [TestMethod]
        public void Graph_GetLink_Rejects_LoopbackDoesntExist()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 10 );

            Assert2.Throws<InvalidOperationException>( () => graph.GetLinkData( 0, 0 ) );
        }

        /// <summary>
        /// Verifies the graph can fetch the link data for existant loopback links.
        /// </summary>
        [TestMethod]
        public void Graph_GetLink_Loopback_Exists()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 0, 10 );

            Assert.AreEqual( 10, graph.GetLinkData( 0, 0 ) );
        }

        /// <summary>
        /// Verifies that the graph supports querying for loopback links when such a loopback does
        /// not exist.
        /// </summary>
        [TestMethod]
        public void Graph_TryGetLink_Loopback_NotExists()
        {
            var graph = new Graph<int, int>( x => x );
            int dummy;

            graph.AddLink( 0, 1, 10 );

            Assert.IsFalse( graph.TryGetLinkData( 0, 0, out dummy ) );
        }

        /// <summary>
        /// Verifies that the graph can try to fetch the link data for a loopback link.
        /// </summary>
        [TestMethod]
        public void Graph_TryGetLink_Loopback_Exists()
        {
            var graph = new Graph<int, int>( x => x );
            int linkData;
            bool result;

            graph.AddLink( 0, 0, 10 );

            result = graph.TryGetLinkData( 0, 0, out linkData );

            Assert.IsTrue( result );
            Assert.AreEqual( 10, linkData );
        }

        /// <summary>
        /// Verifies the graph can disconnect nodes that have loopback links.
        /// </summary>
        [TestMethod]
        public void Graph_Disconnect_NodeWithLoopback()
        {
            var graph = new Graph<int, int>( x => x );
            int dummy;

            graph.AddLink( 0, 0, 10 );
            graph.AddLink( 0, 1, 20 );

            graph.Disconnect( 0 );

            Assert.AreEqual( 2, graph.GetNodes().Count );
            Assert.IsFalse( graph.TryGetLinkData( 0, 0, out dummy ) );
            Assert.AreEqual( 0, graph.GetOutlinks( 0 ).Count );
            Assert.AreEqual( 0, graph.GetInLinks( 0 ).Count );
        }

        /// <summary>
        /// Verifies the graph can disconnect all nodes in a graph when some nodes have loopback links.
        /// </summary>
        [TestMethod]
        public void Graph_DisconnectAll_GraphWithLoopbacks()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;

            for( int i = 0; i < count - 1; i++ )
            {
                graph.AddLink( i, i, 10 );
                graph.AddLink( i, i + 1, 20 );
            }

            graph.DisconnectAll();

            Assert.AreEqual( count, graph.GetNodes().Count );

            for( int i = 0; i < count; i++ )
            {
                Assert.AreEqual( 0, graph.GetOutlinks( i ).Count );
                Assert.AreEqual( 0, graph.GetInLinks( i ).Count );
            }
        }

        /// <summary>
        /// Verifies the graph can remove a node that has loopback links.
        /// </summary>
        [TestMethod]
        public void Graph_Remove_NodeWithLoopback()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 0, 10 );
            graph.AddLink( 0, 1, 20 );

            graph.Remove( 0 );

            Assert.AreEqual( 1, graph.GetNodes().Count );
            Assert.AreEqual( 1, graph.GetNodes()[0] );
        }

        /// <summary>
        /// Verifies the graph can remove all nodes from the graph when some nodes have loopback links.
        /// </summary>
        [TestMethod]
        public void Graph_RemoveAll_GraphWithLoopbacks()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;

            for( int i = 0; i < count - 1; i++ )
            {
                graph.AddLink( i, i, 10 );
                graph.AddLink( i, i + 1, 20 );
            }

            graph.RemoveAll();

            Assert.AreEqual( 0, graph.GetNodes().Count );
        }

        /// <summary>
        /// Verifies that the outlinks list for a node with loopbacks contains itself.
        /// </summary>
        [TestMethod]
        public void Graph_InLinks_NodeWithLoopbacks()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 0, 10 );

            var inlinks = graph.GetInLinks( 0 );

            Assert.AreEqual( 1, inlinks.Count );
            Assert.AreEqual( 0, inlinks[0].StartNode );
            Assert.AreEqual( 0, inlinks[0].EndNode );
            Assert.AreEqual( 10, inlinks[0].LinkData );
        }

        /// <summary>
        /// Verifies that the inlinks list for a node with loopbacks contains itself.
        /// </summary>
        [TestMethod]
        public void Graph_OutLinks_NodeWithLoopbacks()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 0, 10 );

            var outlinks = graph.GetOutlinks( 0 );

            Assert.AreEqual( 1, outlinks.Count );
            Assert.AreEqual( 0, outlinks[0].StartNode );
            Assert.AreEqual( 0, outlinks[0].EndNode );
            Assert.AreEqual( 10, outlinks[0].LinkData );
        }

        /// <summary>
        /// Verifies that GetNeighbors does not count a node as its own neighbor when such a node has
        /// a loopback link.
        /// </summary>
        [TestMethod]
        public void Graph_GetNeighbors_IgnoresLoopbacks()
        {
            throw new NotImplementedException();
        }
    }
}
