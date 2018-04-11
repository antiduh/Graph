using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Dictionary<int, string> dict = new Dictionary<int, string>();

            dict.Add( 0, "hello" );
            dict.Add( 0, "hello" );
            dict.Add( 0, "hello" );
        }
    }
}
