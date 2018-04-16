using System;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_AddNode
    {
        [TestMethod]
        public void Graph_AddNodes_ManyNodes()
        {
            int count = 1000;
            var graph = new Graph<int, int>( x => x );

            for( int i = 0; i < count; i++ )
            {
                graph.AddNode( i );
            }

            Assert.AreEqual( count, graph.GetNodes().Count );
        }

        [TestMethod]
        public void Graph_AddNodes_Rejects_DuplicateNodes()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddNode( 0 );

            Assert2.Throws<InvalidOperationException>( () => graph.AddNode( 0 ) );
        }
    }
}