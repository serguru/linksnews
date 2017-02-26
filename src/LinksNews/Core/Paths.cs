using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Core
{
    public class Paths
    {
        public string Absolute { get; set; }
        public string Relative { get; set; }

        public Paths(string absolute, string relative)
        {
            Absolute = absolute;
            Relative = relative;
        }
    }
}
