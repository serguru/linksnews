using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.News.Abstract
{
    public interface INewsSourceResponse
    {
        NewsResponseStatus Status { get; set; }
        IEnumerable<INewsSource> Sources { get; set; }
    }
}
