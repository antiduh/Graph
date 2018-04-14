using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph
{
    /// <summary>
    /// Stores a directed graph of objects using generics.
    /// </summary>
    /// <remarks>
    /// Relationships between nodes are stored as a map of links with the starting node as the key to
    /// find the list of links.
    ///
    /// Some operations in this class are classified as being 'open' or 'closed'. Open indicates that
    /// the relationships between the nodes involved in the operation are not symmetric. Closed
    /// indicates that the relationships between nodes involved in the operation are symmetric.
    /// </remarks>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TLink"></typeparam>
    public partial class Graph<TNode, TLink>
    {
        /// <summary>
        /// Stores the user-provided function that evaluates the cost of an edge.
        /// </summary>
        private Func<TLink, int> costFunc;

        /// <summary>
        /// Stores a map from a node to a list of that node's outgoing links.
        /// </summary>
        /// <remarks>
        /// Every node that is part of the graph is stored as a value in the outlinkMap, always with
        /// a non-null list.
        ///
        /// The outlinkMap and the inlinkMap objects shall always refer to the same set of nodes and
        /// links. If A's outlinks contains a link from A to B, then B's inlinks contains the same
        /// link object (from A, to B).
        /// </remarks>
        private Dictionary<TNode, List<Link>> outlinkMap;

        /// <summary>
        /// Stores a map from a node to a list of that node's incoming links.
        /// </summary>
        /// <remarks>
        /// Every node that is part of the graph is stored as a value in the inlinkMap, always with a
        /// non-null list.
        ///
        /// The outlinkMap and the inlinkMap objects shall always refer to the same set of nodes and
        /// links. If A's outlinks contains a link from A to B, then B's inlinks contains the same
        /// link object (from A, to B).
        /// </remarks>
        private Dictionary<TNode, List<Link>> inlinkMap;

        /// <summary>
        /// Initializes a new instance of the Graph class.
        /// </summary>
        /// <param name="costFunc">A delegate to translate from an edge to an edge's cost.</param>
        public Graph( Func<TLink, int> costFunc )
        {
            this.costFunc = costFunc;

            this.outlinkMap = new Dictionary<TNode, List<Link>>();
            this.inlinkMap = new Dictionary<TNode, List<Link>>();
        }

        /// <summary>
        /// Adds the given node to the graph as a disconnected node.
        /// </summary>
        /// <param name="node">The node to add to the graph.</param>
        public void AddNode( TNode node )
        {
            if( this.outlinkMap.ContainsKey( node ) )
            {
                throw new InvalidOperationException(
                    "The given node already exists in the graph."
                );
            }

            EnsureAdded( node, true );
        }

        /// <summary>
        /// Adds a directed link between the two nodes.
        /// </summary>
        /// <remarks>
        /// If either node does not already exist in the graph, then the nodes will be added to the graph
        /// as necessary.
        /// </remarks>
        /// <param name="start">The node that the link starts from.</param>
        /// <param name="end">The node that the link ends at.</param>
        /// <param name="linkData">The data for the link.</param>
        public void AddLink( TNode start, TNode end, TLink linkData )
        {
            List<Link> startOutLinks;
            List<Link> endInlinks;
            Link newLink = new Link( start, end );

            // First, make sure both nodes exist in the graph.
            EnsureAdded( start, false );
            EnsureAdded( end, false );

            startOutLinks = GetOutlinks( start );
            endInlinks = GetInLinks( end );

            // Next, verify the link does not already exist.
            // - Note, we don't bother checking `endInlinks` because it will be symmetric with `startOutLinks`.
            if( FindLink( startOutLinks, start, end ) >= 0 )
            {
                throw new InvalidOperationException( "Cannot add link; link already exists." );
            }
            
            // Good to add it, so fill in the data and off we go.
            newLink.LinkData = linkData;

            startOutLinks.Add( newLink );
            endInlinks.Add( newLink );
        }

        /// <summary>
        /// Adds a directed link from <paramref name="left"/> to <paramref name="right"/>, and
        /// a directed link from <paramref name="right"/> to <paramref name="left"/>,
        /// effectively creating a bidirectional link.
        /// </summary>
        /// <remarks>
        /// If either node does not already exist in the graph, then the nodes will be added to graph
        /// as necessary.
        /// </remarks>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="linkData"></param>
        public void AddDual( TNode left, TNode right, TLink linkData )
        {
            AddLink( left, right, linkData );
            AddLink( right, left, linkData );
        }

        /// <summary>
        /// Removes the link from the start node to the end node.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void RemoveLink( TNode start, TNode end )
        {
            List<Link> startOutlinks = GetOutlinks( start );
            List<Link> endInlinks = GetInLinks( end );

            RemoveLink_Prefetched( start, end, startOutlinks, endInlinks );
        }


        /// <summary>
        /// Modifies the data associated with the directed link from the start node to the end node.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="linkData"></param>
        public void SetLink( TNode start, TNode end, TLink linkData )
        {
            // The Link object stored in start's outlinks and in end's inlinks is the same object.
            // We only need to update the object once, so find it through the startOutlinks.

            List<Link> startOutlinks = GetOutlinks( start );

            int index = FindLink( startOutlinks, start, end );

            if( index < 0 )
            {
                throw new InvalidOperationException( "Cannot modify link; link does not exist." );
            }

            startOutlinks[index].LinkData = linkData;
        }

        /// <summary>
        /// Disconnects all incoming and outgoing edges to the given node. The node remains in the
        /// graph as an unconnected node.
        /// </summary>
        /// <param name="node"></param>
        public void Disconnect( TNode node )
        {
            // Find all outlinks from this node, remove them.
            // Find all inlinks to this node, remove them.

            List<Link> nodeOutlinks = GetOutlinks( node );
            List<Link> nodeInlinks = GetInLinks( node );
            List<Link> colinks;

            for( int i = nodeOutlinks.Count - 1; i >= 0; i-- )
            {
                var link = nodeOutlinks[i];

                colinks = GetInLinks( link.EndNode );

                RemoveLink_Prefetched( node, link.EndNode, nodeOutlinks, colinks );
            }

            for( int i = nodeInlinks.Count - 1; i >= 0; i-- )
            {
                var link = nodeInlinks[i];

                colinks = GetOutlinks( link.StartNode );

                RemoveLink_Prefetched( link.StartNode, node, colinks, nodeInlinks );
            }
        }

        /// <summary>
        /// Disconnects all nodes from every other node. The nodes remain in the graph as unconnected nodes.
        /// </summary>
        public void DisconnectAll()
        {
            foreach( var linkList in this.outlinkMap.Values )
            {
                linkList.Clear();
            }

            foreach( var linkList in this.inlinkMap.Values )
            {
                linkList.Clear();
            }
        }

        /// <summary>
        /// Disconnects and removes the given node from the graph.
        /// </summary>
        /// <param name="node"></param>
        public void Remove( TNode node )
        {
            Disconnect( node );

            this.outlinkMap.Remove( node );
            this.inlinkMap.Remove( node );
        }

        /// <summary>
        /// Disconnects and removes all nodes from the graph. The result is an empty graph.
        /// </summary>
        public void RemoveAll()
        {
            // Be nice to the GC.
            DisconnectAll();

            this.outlinkMap.Clear();
            this.inlinkMap.Clear();
        }

        /// <summary>
        /// Returns a list of all directed links that originate at the given node.
        /// </summary>
        public List<Link> GetOutlinks( TNode node )
        {
            List<Link> outLinks;

            if( this.outlinkMap.TryGetValue( node, out outLinks ) == false )
            {
                throw new InvalidOperationException( "The node is not part of the graph." );
            }

            return outLinks;
        }

        /// <summary>
        /// Returns a list of all directed links that terminate at the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<Link> GetInLinks( TNode node )
        {
            List<Link> inlinks;

            if( this.inlinkMap.TryGetValue( node, out inlinks ) == false )
            {
                throw new InvalidOperationException( "The node is not part of the graph." );
            }

            return inlinks;
        }

        public List<TNode> GetNodes()
        {
            return this.outlinkMap.Keys.ToList();
        }


        /// <summary>
        /// Returns a list of nodes, with link data, that the given node has a bidirectional link to.
        /// </summary>
        public void GetNeighbors( TNode node)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the list of nodes such that every node in the list can be reached from the given
        /// node by traversing only bidirectional links.
        /// </summary>
        public void GetNetwork( TNode node )
        {
            throw new NotImplementedException();
        }
        
        public void GetShortestPath()
        {
            throw new NotImplementedException();
        }

        public void Clone()
        {
            throw new NotImplementedException();
        }

        public void IsAcyclic()
        {
            throw new NotImplementedException();
        }

        public void MinimumSpanningTree()
        {
            throw new NotImplementedException();
        }

        public void MatchSubgraph()
        {
            throw new NotImplementedException();
        }

        public void GetSubgraph( IList<TNode> nodes )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resizes internal data structures in the graph in order to the minimum size necessary for
        /// storing the current graph.
        /// </summary>
        public void Compact()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ensures the given node exists in the graph, creating it if necessary.
        /// </summary>
        /// <param name="knownMissing">
        /// True to indicate that checks have already occured to determine that the node does not
        /// exist in the graph.
        /// </param>
        private void EnsureAdded( TNode node, bool knownMissing )
        {
            List<Link> outLinks;
            List<Link> inlinks;

            // If we know it's missing, or we can't find it, create a new one.
            if( knownMissing || this.outlinkMap.TryGetValue( node, out outLinks ) == false )
            {
                outLinks = new List<Link>();
                this.outlinkMap.Add( node, outLinks );

                inlinks = new List<Link>();
                this.inlinkMap.Add( node, inlinks );
            }
        }

        private int FindLink( List<Link> links, TNode start, TNode end )
        {
            for( int i = 0; i < links.Count; i++ )
            {
                if( links[i].EqualEndpoints( start, end ) )
                {
                    return i;
                }
            }

            return -1;
        }

        private void RemoveLink_Prefetched( TNode start, TNode end, List<Link> startOutlinks, List<Link> endInlinks )
        {
            int index = FindLink( startOutlinks, start, end );

            if( index < 0 )
            {
                throw new InvalidOperationException( "Cannot remove link; link does not exist." );
            }

            startOutlinks.RemoveAt( index );

            index = FindLink( endInlinks, start, end );

            if( index < 0 )
            {
                // This should never happen unless there is a bug in the class, the hardware has gone
                // sideways, or the class is being misued via threading.
                throw new InvalidOperationException( "Graph internal state is corrupted." );
            }

            endInlinks.RemoveAt( index );
        }
    }
}
