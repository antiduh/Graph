using System;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_Remove
    {
        [TestMethod]
        public void Graph_Remove_Rejects_UnknownNode_EmptyGraph()
        {
            var graph = new Graph<int, int>( x => x );

            Assert2.Throws<InvalidOperationException>( () => graph.Remove( 0 ) );
        }

        [TestMethod]
        public void Graph_Remove_Rejects_UnknownNode_Populated()
        {
            var graph = new Graph<int, int>( x => x );

            for( int i = 1; i <= 10; i++ )
            {
                graph.AddNode( i );
            }

            Assert2.Throws<InvalidOperationException>( () => graph.Remove( 0 ) );
        }

        [TestMethod]
        public void Graph_Remove_UnconnectedNode()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddNode( 0 );

            graph.Remove( 0 );

            Assert.IsFalse( graph.GetNodes().Contains( 0 ) );
            Assert.AreEqual( 0, graph.GetNodes().Count );
        }

        [TestMethod]
        public void Graph_Remove_Rejects_AlreadyRemoved()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddNode( 0 );
            graph.Remove( 0 );

            Assert2.Throws<InvalidOperationException>( () => graph.Remove( 0 ) );
        }

        [TestMethod]
        public void Graph_Remove_ConnectedNode()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddDual( 0, 1, 10 );

            graph.Remove( 0 );

            Assert.IsFalse( graph.GetNodes().Contains( 0 ) );
            Assert.AreEqual( 0, graph.GetOutlinks( 1 ).Count );
            Assert.AreEqual( 0, graph.GetInLinks( 1 ).Count );
        }
    }
}