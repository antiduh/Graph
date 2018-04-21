using System;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class GraphMaker_BidiLine
    {
        [TestMethod]
        public void GraphMaker_BidiLine_Rejects_InvalidArgs()
        {
            var graph = new Graph<int, int>( x => x );
            var shortNodes = new int[] { 0 };
            var sufficientNodes = new int[] { 0, 1 };

            // Null graph.
            Assert2.Throws<ArgumentNullException>( 
                () => GraphMaker.BidiLine( null, sufficientNodes, 10 )
            );

            // Null nodes
            Assert2.Throws<ArgumentNullException>(
              () => GraphMaker.BidiLine( graph, null, 10 )
            );

            // Not enough nodes.
            Assert2.Throws<InvalidOperationException>(
              () => GraphMaker.BidiLine( graph, shortNodes, 10 )
            );
        }

        [TestMethod]
        public void GraphMaker_BidiLine_Adds_Links()
        {
            var graph = new Graph<int, int>( x => x );
            var nodes = new int[] { 0, 1, 2, 3 };

            GraphMaker.BidiLine( graph, nodes, 10 );

            Assert.AreEqual( nodes.Length, graph.GetNodes().Count );

            for( int i = 0; i < nodes.Length - 1; i++ )
            {
                Assert.AreEqual( 10, graph.GetLinkData( i, i + 1 ) );
                Assert.AreEqual( 10, graph.GetLinkData( i + 1, i ) );
            }
        }
    }
}
