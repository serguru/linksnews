using LinksNews.Services.News.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.News.Providers.NewsApi
{
    public class NewsApiSource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public UrlsToLogos UrlsToLogos { get; set; }
        public NewsSortBy[] sortBysAvailable { get; set; }
    }
}
