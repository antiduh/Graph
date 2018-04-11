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
            public TNode EndNode { get; set; }

            public TLink LinkData { get; set; }
        }
    }
}
