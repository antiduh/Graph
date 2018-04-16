using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_Constructor
    {
        [TestMethod]
        public void Graph_Accepts_ValidCostFunc()
        {
            var graph = new Graph<int, int>( x => x );

            Assert.AreEqual( 0, graph.GetNodes().Count );
        }

        [TestMethod]
        public void Graph_RejectsNullCostFunc()
        {
            var graph = new Graph<int, int>( null );
        }
    }
}