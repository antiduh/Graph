﻿using System;
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
    /// the relationships between the nodes involved in the operation are not symmetric, where closed
    /// indicates that the relationship is symmetric.
    /// </remarks>
    /// <typeparam name="TNode">The type of nodes.</typeparam>
    /// <typeparam name="TLink">The type of links.</typeparam>
    public partial class Graph<TNode, TLink>
    {
        /// <summary>
        /// Stores the user-provided function that evaluates the cost of a link.
        /// </summary>
        private Func<TLink, int> costFunc;

        /// <summary>
        /// Stores a map from a node to a list of that node's incoming and outgoing links.
        /// </summary>
        /// <remarks>
        /// Every node that is part of the graph is stored as a value in the nodeMap.
        /// </remarks>
        private Dictionary<TNode, LinkList> nodeMap;
        
        /// <summary>
        /// Initializes a new instance of the Graph class.
        /// </summary>
        /// <param name="costFunc">A delegate to translate from an edge to an edge's cost.</param>
        public Graph( Func<TLink, int> costFunc )
        {
            this.costFunc = costFunc;

            this.nodeMap = new Dictionary<TNode, LinkList>();
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
        /// Adds a directed link starting from the start node, ending at the end node.
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
            Link newLink = new Link( start, end );
            newLink.LinkData = linkData;

            startOutLinks.Add( newLink );
            endInlinks.Add( newLink );
        }

        /// <summary>
        /// Adds a directed link from <paramref name="left"/> to <paramref name="right"/>, and a
        /// directed link from <paramref name="right"/> to <paramref name="left"/>, effectively
        /// creating a bidirectional link.
        /// </summary>
        /// <remarks>
        /// If either node does not already exist in the graph, then the nodes will be added to the
        /// graph as necessary.
        /// </remarks>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="linkData"></param>
        public void AddDual( TNode left, TNode right, TLink linkData )
        {
            if( left.Equals( right ) )
            {
                throw new InvalidOperationException( "Cannot add bidirectional loopback links." );
            }

            EnsureAdded( left, false );
            EnsureAdded( right, false );

            var leftOuts = GetOutlinks( left );
            var leftIns = GetInLinks( left );
            var rightOuts = GetOutlinks( right );
            var rightIns = GetInLinks( right );

            if( FindLink( leftOuts, left, right ) >= 0 ||
                FindLink( rightIns, left, right ) >= 0 ||
                FindLink( rightOuts, right, left ) >= 0 ||
                FindLink( rightIns, right, left ) >= 0  )
            {
                throw new InvalidOperationException( "Cannot add link; link already exists." );
            }

            Link leftToRightLink = new Link( left, right );
            leftToRightLink.LinkData = linkData;

            leftOuts.Add( leftToRightLink );
            rightIns.Add( leftToRightLink );

            Link rightToLeftLink = new Link( right, left );
            rightToLeftLink.LinkData = linkData;

            rightOuts.Add( rightToLeftLink );
            leftIns.Add( rightToLeftLink );
        }

        /// <summary>
        /// Removes the link from the graph identified by the <paramref name="start"/> node to the
        /// <paramref name="end"/> node.
        /// </summary>
        /// <param name="start">The node that the link begins at.</param>
        /// <param name="end">The node that the link ends at.</param>
        public void RemoveLink( TNode start, TNode end )
        {
            List<Link> startOutlinks = GetOutlinks( start );
            List<Link> endInlinks = GetInLinks( end );

            RemoveLink_Prefetched( start, end, startOutlinks, endInlinks );
        }

        /// <summary>
        /// Returns the data for the link that starts with the given <paramref name="start"/> node
        /// and ends with the given <paramref name="end"/> node.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the link does not exist.</exception>
        /// <param name="start">The node that the link begins at.</param>
        /// <param name="end">The node that the link ends at.</param>
        /// <returns>The link's data.</returns>
        public TLink GetLinkData( TNode start, TNode end )
        {
            var startOutlinks = GetOutlinks( start );
            var endInlinks = GetInLinks( end );

            var link = startOutlinks.Intersect( endInlinks ).FirstOrDefault();

            if( link == null )
            {
                throw new InvalidOperationException( "No such link exists." );
            }

            return link.LinkData;
        }

        /// <summary>
        /// Returns the data for the link that starts with the given <paramref name="start"/> node
        /// and ends with the given <paramref name="end"/> node.
        /// </summary>
        /// <param name="start">The node that the link begins at.</param>
        /// <param name="end">The node that the link ends at.</param>
        /// <param name="linkData">Returns the link's data, if found.</param>
        /// <returns>True if the link could be found.</returns>
        public bool TryGetLinkData( TNode start, TNode end, out TLink linkData )
        {
            var startOutlinks = GetOutlinks( start );
            var endInlinks = GetInLinks( end );

            var link = startOutlinks.Intersect( endInlinks ).FirstOrDefault();

            if( link == null )
            {
                linkData = default( TLink );
                return false;
            }

            linkData = link.LinkData;
            return true;
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
            foreach( var linkList in this.nodeMap.Values )
            {
                linkList.Outlinks.Clear();
                linkList.Inlinks.Clear();
            }
        }

        /// <summary>
        /// Disconnects and removes the given node from the graph.
        /// </summary>
        /// <param name="node"></param>
        public void Remove( TNode node )
        {
            Disconnect( node );

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
        /// Returns a list of all directed links that originate at the given node.
        /// </summary>
        public List<Link> GetOutlinks( TNode node )
        {
            LinkList linkList;

            if( this.nodeMap.TryGetValue( node, out linkList ) == false )
            {
                throw new InvalidOperationException( "The node is not part of the graph." );
            }

            return linkList.Outlinks;
        }

        /// <summary>
        /// Returns a list of all directed links that terminate at the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<Link> GetInLinks( TNode node )
        {
            LinkList linkList;

            if( this.nodeMap.TryGetValue( node, out linkList ) == false )
            {
                throw new InvalidOperationException( "The node is not part of the graph." );
            }

            return linkList.Inlinks;
        }

        /// <summary>
        /// Returns a list of the nodes that exist in the graph.
        /// </summary>
        /// <returns></returns>
        public List<TNode> GetNodes()
        {
            return this.nodeMap.Keys.ToList();
        }

        /// <summary>
        /// Returns a list of nodes that the given node has a bidirectional link to.
        /// </summary>
        public List<TNode> GetNeighbors( TNode node )
        {
            List<TNode> neighbors = new List<TNode>();

            LinkList links;

            if( this.nodeMap.TryGetValue( node, out links ) == false )
            {
                throw new InvalidOperationException( "The node is not part of the graph." );
            }

            foreach( var outlink in links.Outlinks )
            {
                var peer = outlink.EndNode;

                if( peer.Equals( node ) )
                {
                    // Ignore loopback links.
                    continue; 
                }

                foreach( var inlink in links.Inlinks )
                {
                    if( inlink.StartNode.Equals( peer ) )
                    {
                        neighbors.Add( peer );
                        break;
                    }
                }
            }

            return neighbors;
        }


        /// <summary>
        /// Returns the list of nodes such that every node in the list can be reached from the given
        /// starting node by traversing only bidirectional links. The network contains the starting node.
        /// </summary>
        /// <remarks>
        /// The word 'closed' here means that startingNode's network contain nodes, where those nodes
        /// must have startingNode in their own network, as well as every other node in
        /// startingNode's network. The result is that asking for the network from any such node
        /// returns the same list of nodes.
        ///
        /// For example, if GetClosedNetwork(Node1) returns {Node1, Node2, Node3}, then
        /// GetClosedNetwork(Node2) would return the same list, and again for Node3.
        ///
        /// Contrast with <see cref="GetOpenNetwork(TNode)"/>, where the only criteria for inclusion
        /// in startingNodes's open network is that there exists a path (and not necessarily a bidi
        /// path) from startingNode to every returned node.
        ///
        /// If NodeX's open network contained NodeY, but NodeY has only in-links and no out-links,
        /// then NodeY's open network will be empty, even though NodeY is in NodeX's open network.
        /// </remarks>
        public List<TNode> GetClosedNetwork( TNode startingNode )
        {
            HashSet<TNode> seen = new HashSet<TNode>();
            Queue<TNode> leads = new Queue<TNode>();

            if( this.nodeMap.ContainsKey( startingNode ) == false )
            {
                throw new InvalidOperationException( "The node is not part of the graph." );
            }

            leads.Enqueue( startingNode );
            seen.Add( startingNode );

            while( leads.Count > 0 )
            {
                var lead = leads.Dequeue();
                var leadLinks = this.nodeMap[lead];

                // - Find all outlinks from the lead.
                // - Trim the outlinks that contain seen nodes.
                // - For all remaining nodes, verify that lead is bidi with the remaining node.

                foreach( Link leadOutlink in leadLinks.Outlinks )
                {
                    TNode peer = leadOutlink.EndNode;

                    if( seen.Contains( peer ) )
                    {
                        continue; 
                    }

                    foreach( var inlinks in leadLinks.Inlinks )
                    {
                        if( inlinks.StartNode.Equals( peer ) )
                        {
                            seen.Add( peer );
                            leads.Enqueue( peer );
                            break;
                        }
                    }
                }
            }

            return seen.ToList();
        }

        /// <summary>
        /// Returns the list of nodes that can be reached from the given starting node by traversing
        /// any kind of link, including undirectional links. The network contains the starting node.
        /// </summary>
        /// <remarks>
        /// The word 'open' here means that startingNode's network may contain nodes, where those
        /// nodes might not have startingNode in their own open network.
        ///
        /// As an example, if there is a path from A to B, but no path from B to A, then A's open
        /// network will contain B, but B's open network will not contain A.
        /// </remarks>
        public List<TNode> GetOpenNetwork( TNode startingNode )
        {
            HashSet<TNode> seen = new HashSet<TNode>();
            Queue<TNode> leads = new Queue<TNode>();
            List<Link> outlinks;

            leads.Enqueue( startingNode );
            seen.Add( startingNode );

            while( leads.Count > 0 )
            {
                var lead = leads.Dequeue();

                outlinks = GetOutlinks( lead );

                foreach( var outlink in outlinks )
                {
                    if( seen.Add( outlink.EndNode ) )
                    {
                        leads.Enqueue( outlink.EndNode );
                    }
                }
            }

            return seen.ToList();
        }

        public int GetDiameter()
        {
            // Strategy: 
            // - Examine every possible shortest path.
            // - Select the shortest path that is longer than all other shortest paths.

            List<TNode> nodes = GetNodes();

            int diameter = 0;

            for( int startIndex = 0; startIndex < nodes.Count; startIndex++ )
            {
                for( int endIndex = startIndex + 1; endIndex < nodes.Count; endIndex++ )
                {
                    bool found;
                    List<TNode> path;
                    int cost;

                    found = GetShortestPath( nodes[startIndex], nodes[endIndex], out path, out cost );

                    if( found )
                    {
                        diameter = Math.Max( diameter, path.Count );
                    }
                }
            }

            return diameter;
        }

        public bool GetShortestPath( TNode start, TNode end, out List<TNode> path, out int cost )
        {
            // Nodes are referred to by their node index in this implementation.
            // Node i is the node returned by nodes[i].

            // The list of all known nodes.
            List<TNode> nodes = GetNodes();

            // The distance from the start node to node i. 
            int[] dist = new int[nodes.Count];

            // prev[i] stores the node index that is previous to node i in a potential shortest path.
            int[] prev = new int[nodes.Count];

            // Stores the nodes that have not been visited.
            HashSet<int> unvisited = new HashSet<int>();

            // The node index of the terminal nodes.
            int startIndex = nodes.IndexOf( start );
            int endIndex = nodes.IndexOf( end );

            bool found = false;

            if( startIndex == -1 )
            {
                throw new InvalidOperationException( "Node not in graph: " + start );
            }

            if( endIndex == -1 )
            {
                throw new InvalidOperationException( "Node not in graph: " + end );
            }

            // Initialization:
            // - Set distance to all ndoes to infinite, except starting node.
            // - The start node has zero cost to itself.

            for( int i = 0; i < nodes.Count; i++ )
            {
                if( nodes[i].Equals( start ) )
                {
                    dist[i] = 0;
                }
                else
                {
                    dist[i] = int.MaxValue;
                }

                prev[i] = -1;

                unvisited.Add( i );
            }

            while( unvisited.Count > 0 )
            {
                // Find the unvisted node with the lowest distance.

                int bestDistance = int.MaxValue;
                int currentNode = -1;

                foreach( int index in unvisited )
                {
                    if( dist[index] <= bestDistance )
                    {
                        bestDistance = dist[index];
                        currentNode = index;
                    }
                }

                if( currentNode == endIndex )
                {
                    // The lowest cost is the target.
                    // - Check to see if lowest cost is our infinite cost. If so, we failed.
                    found = ( bestDistance < int.MaxValue );
                    break;
                }

                unvisited.Remove( currentNode );

                var currentOutlinks = GetOutlinks( nodes[currentNode] );

                foreach( var outlink in currentOutlinks )
                {
                    // neighbor of currentNode.
                    int neighbor = nodes.IndexOf( outlink.EndNode );

                    if( unvisited.Contains( neighbor ) == false ) { continue; }

                    int newDistance = dist[currentNode] + this.costFunc( outlink.LinkData );

                    if( newDistance < dist[neighbor] )
                    {
                        dist[neighbor] = newDistance;
                        prev[neighbor] = currentNode;
                    }
                }
            }

            if( found == false )
            {
                path = null;
                cost = -1;
                return false;
            }
            else
            {
                // Let's say the path was 0 --> 1 --> 2. Then:
                // - prev[2] == 1
                // - prev[1] == 0
                // - prev[0] == -1 (has no predecessor).

                // So, we'll use a stack to unwind into a pretty list.

                Stack<int> stack = new Stack<int>();

                int tail = endIndex;

                while( tail != -1 )
                {
                    stack.Push( tail );
                    tail = prev[tail];
                }

                List<TNode> results = new List<TNode>( stack.Count );

                while( stack.Count > 0 )
                {
                    results.Add( nodes[stack.Pop()] );
                }

                path = results;
                cost = dist[endIndex];
                return true;
            }
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
            LinkList linkList;

            // If we know it's missing, or we can't find it, create a new one.
            if( knownMissing || this.nodeMap.TryGetValue( node, out linkList ) == false )
            {
                linkList = new LinkList();
                linkList.Init();

                this.nodeMap.Add( node, linkList );
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
