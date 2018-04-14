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
    /// Relationships between nodes are stored as a map of links with the starting node as the key to find the list of links.
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
        /// Stores a map from a node to a list of that node's outgoing edges.
        /// </summary>
        /// <remarks>
        /// Every node that is part of the graph is stored as a value in the nodeMap. Every node
        /// always has a non-null link list. A node that has an empty link list is a disconnected node.
        /// </remarks>
        private Dictionary<TNode, List<Link>> nodeMap;

        /// <summary>
        /// Initializes a new instance of the Graph class.
        /// </summary>
        /// <param name="costFunc">A delegate to translate from an edge to an edge's cost.</param>
        public Graph( Func<TLink, int> costFunc )
        {
            this.costFunc = costFunc;

            this.nodeMap = new Dictionary<TNode, List<Link>>();
        }

        /// <summary>
        /// Adds the given node to the graph as a disconnected node.
        /// </summary>
        /// <param name="node">The node to add to the graph.</param>
        public void AddNode( TNode node )
        {
            if( this.nodeMap.ContainsKey( node ) )
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

            // First, make sure both nodes exist in the graph.
            startOutLinks = EnsureAdded( start, false );

            EnsureAdded( end, false );

            // Next, verify the link does not already exist.

            foreach( Link outLink in startOutLinks )
            {
                if( outLink.LinkData.Equals( linkData ) )
                {
                    throw new InvalidOperationException(
                        "Cannot add link between given nodes, a link already exists."
                    );
                }
            }

            startOutLinks.Add( new Link() { EndNode = end, LinkData = linkData } );
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
            List<Link> outLinks = GetOutlinks( start );
            int foundIndex = -1;

            for( int i = 0; i < outLinks.Count; i++ )
            {
                if( outLinks[i].EndNode.Equals( end ) )
                {
                    foundIndex = -1;
                    break;
                }
            }

            if( foundIndex < 0 )
            {
                throw new InvalidOperationException( "Link not found." );
            }

            outLinks.RemoveAt( foundIndex );
        }

        /// <summary>
        /// Modifies the data associated with the directed link from the start node to the end node.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="linkData"></param>
        public void SetLink( TNode start, TNode end, TLink linkData )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disconnects all incoming and outgoing edges to the given node. The node remains in the
        /// graph as an unconnected node.
        /// </summary>
        /// <param name="node"></param>
        public void Disconnect( TNode node )
        {
            List<Link> outLinks = GetOutlinks( node );
            
            // Clear the outlinks from the node.
            outLinks.Clear();

            // Clear the inlinks to the node.

            foreach( var otherNode in this.nodeMap.Keys )
            {
                if( otherNode.Equals( node ) )
                {
                    continue;
                }

                outLinks = this.nodeMap[otherNode];

                for( int i = 0; i < outLinks.Count; /* conditional increment */ )
                {
                    if( outLinks[i].EndNode.Equals( node ) )
                    {
                        outLinks.RemoveAt( i );
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// Disconnects all nodes from every other node. The nodes remain in the graph as unconnected nodes.
        /// </summary>
        public void DisconnectAll()
        {
            foreach( var links in this.nodeMap.Values )
            {
                links.Clear();
            }
        }

        /// <summary>
        /// Disconnects and removes the given node from the graph.
        /// </summary>
        /// <param name="node"></param>
        public void Remove( TNode node )
        {
            List<Link> outLinks = GetOutlinks( node );
            
            // Clear the links to be nice to the GC.
            outLinks.Clear();
            this.nodeMap.Remove( node );
        }

        /// <summary>
        /// Disconnects and removes all nodes from the graph. The result is an empty graph.
        /// </summary>
        public void RemoveAll()
        {
            // Be nice to the GC.
            DisconnectAll();

            this.nodeMap.Clear();
        }

        /// <summary>
        /// Returns a list of nodes that the given node has a bidirectional link to.
        /// </summary>
        public void GetNeighbors( TNode node)
        {
            throw new NotImplementedException();
        }

        public List<Link> GetOutlinks( TNode node )
        {
            List<Link> outLinks;

            if( this.nodeMap.TryGetValue( node, out outLinks ) == false )
            {
                throw new InvalidOperationException( "Node not found." );
            }

            return outLinks;
        }

        public void GetInLinks( TNode node )
        {
        }

        /// <summary>
        /// Returns the list of nodes such that every node in the list can be reached from the given
        /// node by traversing only bidirectional links.
        /// </summary>
        public void GetNetwork( TNode node )
        {
            throw new NotImplementedException();
        }

        public void GetNodes()
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
        private List<Link> EnsureAdded( TNode node, bool knownMissing )
        {
            List<Link> outLinks;

            // If we know it's missing, or we can't find it, create a new one.
            if( knownMissing || this.nodeMap.TryGetValue( node, out outLinks ) == false )
            {
                outLinks = new List<Link>();
                this.nodeMap.Add( node, outLinks );
            }

            return outLinks;
        }
    }
}
