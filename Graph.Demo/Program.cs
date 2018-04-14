using System;

namespace Graph.Demo
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Graph<int, int> graph = new Graph<int, int>( null );

            graph.AddDual( 0, 1, 42 );
            graph.AddDual( 1, 2, 42 );

            DumpGraph( graph );

            Console.WriteLine( "Disconnecting..." );

            graph.Disconnect( 0 );

            DumpGraph( graph );
        }

        private static void DumpGraph<TNode, TLink>( Graph<TNode, TLink> graph )
        {
            var nodes = graph.GetNodes();

            foreach( var node in nodes )
            {
                Console.WriteLine( "Node {0} =======", node );

                foreach( var outlink in graph.GetOutlinks( node ) )
                {
                    Console.WriteLine( "{0} --> {1}", node, outlink.EndNode );
                }

                foreach( var inlink in graph.GetInLinks( node ) )
                {
                    Console.WriteLine( "{0} <-- {1}", node, inlink.StartNode );
                }

                Console.WriteLine();
            }
        }
    }
}
