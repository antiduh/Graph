﻿using System;
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
            ArgCheck( graph, nodes, linkData );

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
        /// <param name="linkData"></param>
        public static void BidiLine<TNode, TLink>( Graph<TNode, TLink> graph, IReadOnlyList<TNode> nodes, TLink linkData )
        {
            ArgCheck( graph, nodes, linkData );

            if( nodes.Count <= 1 )
            {
                throw new InvalidOperationException( "Cannot create a line topology with fewer than 2 nodes." );
            }

            for( int i = 0; i < nodes.Count - 1; i++ )
            {
                graph.AddDual( nodes[i], nodes[i + 1], linkData );
            }
        }

        /// <summary>
        /// Creates a graph where the sequentially connected to each other using directed links; the
        /// first node is connected to the second node, the second to the third, and so on.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <typeparam name="TLink"></typeparam>
        /// <param name="graph"></param>
        /// <param name="nodes"></param>
        /// <param name="linkData"></param>
        public static void DirectedLine<TNode, TLink>( Graph<TNode, TLink> graph, IReadOnlyList<TNode> nodes, TLink linkData )
        {
            ArgCheck( graph, nodes, linkData );

            for( int i = 0; i < nodes.Count - 1; i++ )
            {
                graph.AddLink( nodes[i], nodes[i + 1], linkData );
            }
        }

        private static void ArgCheck<TNode, TLink>( Graph<TNode, TLink> graph, IReadOnlyList<TNode> nodes, TLink linkData )
        {
            if( graph == null )
            {
                throw new ArgumentNullException( nameof( graph ) );
            }

            if( nodes == null )
            {
                throw new ArgumentNullException( nameof( nodes ) );
            }

            if( linkData == null )
            {
                throw new ArgumentNullException( nameof( linkData ) );
            }
        }
    }
}
