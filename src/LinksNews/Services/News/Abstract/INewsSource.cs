using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.News.Abstract
{
    public interface INewsSource
    {
        string Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string Url { get; set; }
        string Category { get; set; }
        string Language { get; set; }
        string Country { get; set; }
        UrlsToLogos UrlsToLogos { get; set; }
        NewsSortBy[] SortBysAvailable { get; set; }

        IEnumerable<INewsArticle> Articles { get; set; }
    }
}
/*
{
"id": "abc-news-au",
"name": "ABC News (AUS)",
"description": "Australia's most trusted source of local, national and world news. Comprehensive, independent, in-depth analysis, the latest business, sport, weather and more.",
"url": "http://www.abc.net.au/news",
"category": "general",
"language": "en",
"country": "au",
-"urlsToLogos": {
"small": "http://i.newsapi.org/abc-news-au-s.png",
"medium": "http://i.newsapi.org/abc-news-au-m.png",
"large": "http://i.newsapi.org/abc-news-au-l.png"
},
-"sortBysAvailable": [
"top"
]
},
*/