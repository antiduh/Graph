using System;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_GetLink
    {
        [TestMethod]
        public void Graph_GetLink_Rejects_WrongNodes()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 42 );

            // Wrong end node.
            Assert2.Throws<InvalidOperationException>( () => graph.GetLinkData( 0, 2 ) );

            // Wrong start node.
            Assert2.Throws<InvalidOperationException>( () => graph.GetLinkData( 2, 1 ) );

            // Wrong both nodes.
            Assert2.Throws<InvalidOperationException>( () => graph.GetLinkData( 1, 2 ) );

            // Right nodes, reversed.
            Assert2.Throws<InvalidOperationException>( () => graph.GetLinkData( 1, 0 ) );
        }

        [TestMethod]
        public void Graph_GetLink_CreatedByAddLink()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 42 );

            Assert.AreEqual( 42, graph.GetLinkData( 0, 1 ) );
        }

        [TestMethod]
        public void Graph_GetLink_MultipleLinks()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 10 );
            graph.AddLink( 1, 2, 20 );
            graph.AddLink( 2, 0, 30 );

            Assert.AreEqual( 10, graph.GetLinkData( 0, 1 ) );
            Assert.AreEqual( 20, graph.GetLinkData( 1, 2 ) );
            Assert.AreEqual( 30, graph.GetLinkData( 2, 0 ) );
        }

        [TestMethod]
        public void Graph_GetLink_CreatedByAddDual()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddDual( 0, 1, 42 );

            Assert.AreEqual( 42, graph.GetLinkData( 0, 1 ) );
            Assert.AreEqual( 42, graph.GetLinkData( 1, 0 ) );
        }

        [TestMethod]
        public void Graph_GetLink_AfterModification()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 0 );
            Assert.AreEqual( 0, graph.GetLinkData( 0, 1 ) );

            for( int i = 1; i < 100; i++ )
            {
                graph.SetLink( 0, 1, i );
                Assert.AreEqual( i, graph.GetLinkData( 0, 1 ) );
            }
        }
    }
}