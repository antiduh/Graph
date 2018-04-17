using System;
using System.Collections.Generic;

namespace Graph
{
    public static class GraphMaker
    {
        /// <summary>
        /// Creates a simple-complete graph. Every node is connected to every other node using a
        /// bidirectional link, without loopbacks.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <typeparam name="TLink"></typeparam>
        /// <param name="graph"></param>
        /// <param name="nodes"></param>
        /// <param name="linkData"></param>
        public static void SimpleComplete<TNode, TLink>( Graph<TNode, TLink> graph, IReadOnlyList<TNode> nodes, TLink linkData )
        {
            for( int start = 0; start < nodes.Count; start++ )
            {
                for( int end = 0; end < nodes.Count; end++ )
                {
                    if( start.Equals( end ) ) { continue; }

                    graph.AddLink( nodes[start], nodes[end], linkData );
                }
            }
        }

        /// <summary>
        /// Creates a graph where the given list of nodes are sequentially connected to each other
        /// with bidirectional links.
        /// </summary>
        /// <remarks>
        /// For example, if the given list of nodes was {0, 1, 2, 3}, then the graph would consist of
        /// bidi links between (0,1), (1,2), and (2,3).
        /// </remarks>
        /// <param name="graph"></param>
        /// <param name="nodes"></param>
        /// <param name="v"></param>
        public static void BidiLine( Graph<int, int> graph, IReadOnlyList<int> nodes, int v )
        {
            for( int i = 0; i < nodes.Count - 1; i++ )
            {
                graph.AddDual( i, i + 1, 10 );
            }
        }
    }
}
