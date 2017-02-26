using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.News.Providers.NewsApi
{
    public class NewsApiArticle
    {
        public string Author { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string UrlToImage { get; set; }
        public DateTimeOffset? PublishedAt  { get; set; }
    }
}
