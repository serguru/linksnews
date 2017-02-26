using LinksNews.Services.News.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.News.Providers.NewsApi
{
    public class NewsApiArticleResponse
    {
        public NewsResponseStatus Status { get; set; }
        public string Source { get; set; }
        public NewsSortBy SortBy { get; set; }
        public IEnumerable<NewsApiArticle> Articles { get; set; }
    }
}
