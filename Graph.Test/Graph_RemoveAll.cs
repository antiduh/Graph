using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_RemoveAll
    {
        [TestMethod]
        public void Graph_RemoveAll_Empty()
        {
            var graph = new Graph<int, int>( x => x );

            graph.RemoveAll();

            Assert.AreEqual( 0, graph.GetNodes().Count );
        }

        [TestMethod]
        public void Graph_RemoveAll_Filled()
        {
            var graph = new Graph<int, int>( x => x );
            int count = 100;

            for( int i = 0; i < count; i++ )
            {
                graph.AddLink( i, i + 1, 10 );
            }

            graph.RemoveAll();

            Assert.AreEqual( 0, graph.GetNodes().Count );
        }
    }
}
