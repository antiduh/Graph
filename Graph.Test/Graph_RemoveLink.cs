using System;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_RemoveLink
    {
        [TestMethod]
        public void Graph_RemoveLink_Rejects_UnknownNode()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 1, 2, 10 );

            Assert2.Throws<InvalidOperationException>( () => graph.RemoveLink( 0, 1 ) );
            Assert2.Throws<InvalidOperationException>( () => graph.RemoveLink( 1, 0 ) );
            Assert2.Throws<InvalidOperationException>( () => graph.RemoveLink( 0, 3 ) );
        }

        [TestMethod]
        public void Graph_RemoveLink_Rejects_UnknownLink()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddNode( 0 );
            graph.AddNode( 1 );

            Assert2.Throws<InvalidOperationException>( () => graph.RemoveLink( 1, 0 ) );

            graph.AddLink( 0, 1, 10 );
            Assert2.Throws<InvalidOperationException>( () => graph.RemoveLink( 1, 0 ) );
        }

        [TestMethod]
        public void Graph_RemoveLink_LinkExists()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddLink( 0, 1, 10 );

            graph.RemoveLink( 0, 1 );

            Assert.AreEqual( 0, graph.GetOutlinks( 0 ).Count );
            Assert.AreEqual( 0, graph.GetInLinks( 1 ).Count );
        }

        [TestMethod]
        public void Graph_RemoveLink_Retains_BidiSecondLink()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddDual( 0, 1, 10 );

            graph.RemoveLink( 0, 1 );

            Assert.AreEqual( 10, graph.GetLinkData( 1, 0 ) );
            Assert.AreEqual( 0, graph.GetOutlinks( 0 ).Count );
            Assert.AreEqual( 1, graph.GetOutlinks( 1 ).Count );
        }
    }
}
