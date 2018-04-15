using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graph.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graph.Tests
{
    [TestClass]
    public class Graph_AddDual
    {
        [TestMethod]
        public void Graph_AddDual_AddsTwoLinks()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddDual( 0, 1, 42 );

            Assert.AreEqual( 1, graph.GetOutlinks( 0 ).Count );
            Assert.AreEqual( 1, graph.GetInLinks( 0 ).Count );

            Assert.AreEqual( 1, graph.GetOutlinks( 1 ).Count );
            Assert.AreEqual( 1, graph.GetInLinks( 1 ).Count );
        }

        [TestMethod]
        public void Graph_AddDual_SetsLinkData()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddDual( 0, 1, 42 );

            Assert.AreEqual( 42, graph.GetOutlinks( 0 )[0].LinkData );
            Assert.AreEqual( 42, graph.GetInLinks( 0 )[0].LinkData );
            Assert.AreEqual( 42, graph.GetOutlinks( 1 )[0].LinkData );
            Assert.AreEqual( 42, graph.GetInLinks( 1 )[0].LinkData );
        }

        /// <summary>
        /// Verifies that adding a bidi link with unknown nodes causes the nodes to be added.
        /// </summary>
        [TestMethod]
        public void Graph_AddDual_CreatesNodes()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddDual( 0, 1, 42 );

            var nodes = graph.GetNodes();

            Assert.AreEqual( 2, nodes.Count );
            Assert.IsTrue( nodes.Contains( 0 ) );
            Assert.IsTrue( nodes.Contains( 1 ) );
        }

        /// <summary>
        /// Verifies that duplicate links are not allowed.
        /// </summary>
        [TestMethod]
        public void Graph_AddDual_Rejects_DuplicateLinks_SameCost()
        {
            var graph = new Graph<int, int>( x => x );

            graph.AddDual( 0, 1, 1 );

            // Verify that we can't add the duplicate unidirectional or bidirectional links,
            // specifying start and end in any order.
            Assert2.Throws<InvalidOperationException>( () => graph.AddLink( 0, 1, 1 ) );
            Assert2.Throws<InvalidOperationException>( () => graph.AddLink( 0, 1, 2 ) );

            Assert2.Throws<InvalidOperationException>( () => graph.AddLink( 1, 0, 1 ) );
            Assert2.Throws<InvalidOperationException>( () => graph.AddLink( 1, 0, 2 ) );

            Assert2.Throws<InvalidOperationException>( () => graph.AddDual( 0, 1, 1 ) );
            Assert2.Throws<InvalidOperationException>( () => graph.AddDual( 0, 1, 2 ) );

            Assert2.Throws<InvalidOperationException>( () => graph.AddDual( 1, 0, 1 ) );
            Assert2.Throws<InvalidOperationException>( () => graph.AddDual( 1, 0, 2 ) );
        }

        /// <summary>
        /// Verifies that AddDual will not add any links until it has verified that both links will
        /// be added sucessfully. Tests the case where the conflicting link is the forward direction
        /// of the bidi link.
        /// </summary>
        [TestMethod]
        public void Graph_AddDual_ValidatesBeforeAdding_ForwardOrder()
        {
            var graph = new Graph<int, int>( x => x );

            Action validation = () =>
            {
                Assert.AreEqual( 2, graph.GetNodes().Count );

                Assert.AreEqual( 1, graph.GetOutlinks( 0 ).Count );
                Assert.AreEqual( 0, graph.GetInLinks( 0 ).Count );
                Assert.AreEqual( 0, graph.GetOutlinks( 1 ).Count );
                Assert.AreEqual( 1, graph.GetInLinks( 1 ).Count );
            };

            // Create one directed link that'll conflict with the bidi links in the next steps.
            graph.AddLink( 0, 1, 1 );

            validation();

            // Verify that we get an exception during add
            Assert2.Throws<InvalidOperationException>( () => graph.AddDual( 0, 1, 1 ) );

            // Verify that the graph is completely unchanged.
            validation();
        }


        /// <summary>
        /// Verifies that AddDual will not add any links until it has verified that both links will
        /// be added sucessfully. Tests the case where the conflicting link is the reverse direction
        /// of the bidi link.
        /// </summary>
        [TestMethod]
        public void Graph_AddDual_ValidatesBeforeAdding_ReverseOrder()
        {
            var graph = new Graph<int, int>( x => x );

            Action validation = () =>
            {
                Assert.AreEqual( 2, graph.GetNodes().Count );

                Assert.AreEqual( 1, graph.GetOutlinks( 1 ).Count );
                Assert.AreEqual( 0, graph.GetInLinks( 1 ).Count );
                Assert.AreEqual( 0, graph.GetOutlinks( 0 ).Count );
                Assert.AreEqual( 1, graph.GetInLinks( 0 ).Count );
            };

            // Create one directed link that'll conflict with the bidi links in the next steps.
            graph.AddLink( 1, 0, 1 );

            validation();

            // Verify that we get an exception during add
            Assert2.Throws<InvalidOperationException>( () => graph.AddDual( 0, 1, 1 ) );

            // Verify that the graph is completely unchanged.
            validation();
        }
    }
}
