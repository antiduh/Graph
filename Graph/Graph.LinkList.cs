using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph
{
    partial class Graph<TNode, TLink>
    {
        private struct LinkList
        {
            public List<Link> Outlinks;

            public List<Link> Inlinks;

            public void Init()
            {
                Outlinks = new List<Link>();
                Inlinks = new List<Link>();
            }
        }
    }
}
