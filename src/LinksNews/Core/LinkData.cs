using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Core
{
    public class LinkData
    {
        public long PageId { get; set; }
        public long ColumnId { get; set; }
        public string Href { get; set; }
        public string Title { get; set; }
    }
}
