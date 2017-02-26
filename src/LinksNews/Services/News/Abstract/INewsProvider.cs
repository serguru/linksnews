using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinksNews.Services.Data.Models;

namespace LinksNews.Services.News.Abstract
{
    public interface INewsProvider
    {
        NewsProviderDef ProviderDef { get; }
        IEnumerable<NewsSource> GetSources();
        IEnumerable<Link> GetLinks(string sourceId);
    }
}
