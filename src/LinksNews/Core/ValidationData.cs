using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Core
{
    public class ValidationData
    {
        public bool Valid { get; set; }
        public string Message { get; set; }
        public string[] Params { get; set; }
}
}
