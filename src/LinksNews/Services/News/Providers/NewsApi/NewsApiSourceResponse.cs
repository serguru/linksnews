using LinksNews.Services.News.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.News.Providers.NewsApi
{
    public class NewsApiSourceResponse
    {
        public NewsResponseStatus Status { get; set; }
        public NewsApiSource[] Sources { get; set; }
    }
}
