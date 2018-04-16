using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph
{
    public partial class Graph<TNode, TLink>
    {
        public class Link
        {
            public Link( TNode startNode, TNode endNode )
            {
                this.StartNode = startNode;
                this.EndNode = endNode;
            }

            public TNode StartNode { get; private set; }

            public TNode EndNode { get; private set; }

            public TLink LinkData { get; internal set; }

            /// <summary>
            /// Returns whether this link and the given link have the same start and end nodes.
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool EqualEndpoints( Link other )
            {
                if( other == null ) { return false; }

                return EqualEndpoints( other.StartNode, other.EndNode );
            }

            public bool EqualEndpoints( TNode start, TNode end )
            {
                return
                    this.StartNode.Equals( start ) &&
                    this.EndNode.Equals( end );
            }

            /// <summary>
            /// Returns whether this link and the given link have the same start and end nodes.
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equal( Link other )
            {
                return EqualEndpoints( other );
            }

            /// <summary>
            /// Returns whether the given object is equal to this link.
            /// </summary>
            /// <remarks>
            /// Equality for Links is defined as having the same start node and the same end node.
            /// </remarks>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals( object obj )
            {
                return Equal( obj as Link );
            }

            /// <summary>
            /// Returns the hash code for the Link.
            /// </summary>
            /// <remarks>
            /// The identity of a Link object depends on its start node and its end node. The link's
            /// identity does not include its link data.
            /// </remarks>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return this.StartNode.GetHashCode() * 19 + this.EndNode.GetHashCode() * 7703;
            }

            /// <summary>
            /// Returns a simple string representing the link.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format( "{0} ---{1}---> {2}", this.StartNode, this.LinkData, this.EndNode );
            }
        }
    }
}
