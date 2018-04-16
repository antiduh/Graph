using System;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_DisconnectAll
    {
        [TestMethod]
        public void Graph_DisconnectAll_Empty()
        {
            var graph = new Graph<int, int>( x => x );

            graph.DisconnectAll();

            Assert.AreEqual( 0, graph.GetNodes().Count );
        }

        [TestMethod]
        public void Graph_DisconnectAll_Filled()
        {
            var graph = new Graph<int, int>( x => x );
            int numNodes = 100;

            for( int i = 0; i < numNodes - 1; i++ )
            {
                graph.AddLink( i, i + 1, 10 );
            }

            graph.DisconnectAll();

            Assert.AreEqual( numNodes, graph.GetNodes().Count );

            for( int i = 0; i < numNodes; i++ )
            {
                Assert.AreEqual( 0, graph.GetOutlinks( i ).Count );
                Assert.AreEqual( 0, graph.GetInLinks( i ).Count );
            }
        }
    }
}
