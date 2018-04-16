using System;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_SetLink
    {
        [TestMethod]
        public void Graph_SetLink_Rejects_UnknownNodes_Empty()
        {
            var graph = new Graph<int, int>( x => x );

            Assert2.Throws<InvalidOperationException>( () => graph.SetLink( 1, 2, 10 ) );
        }

        [TestMethod]
        public void Graph_SetLink_Rejects_UnknownNodes_Filled()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddDual( 0, 1, 10 );
            graph.AddDual( 0, 2, 10 );
            graph.AddLink( 2, 1, 10 );

            Assert2.Throws<InvalidOperationException>( () => graph.SetLink( 1, 2, 10 ) );
        }

        [TestMethod]
        public void Graph_SetLink_SetsLinkData()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddDual( 0, 1, 10 );

            graph.SetLink( 0, 1, 20 );
            graph.SetLink( 1, 0, 30 );

            Assert.AreEqual( 20, graph.GetLinkData( 0, 1 ) );
            Assert.AreEqual( 30, graph.GetLinkData( 1, 0 ) );
        }
    }
}
