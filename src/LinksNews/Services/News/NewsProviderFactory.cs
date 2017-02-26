using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinksNews.Core;
using LinksNews.Services.News.Providers.NewsApi;
using LinksNews.Services.Data;
using LinksNews.Services.News.Abstract;
using LinksNews.Services.Data.Models;
using LinksNews.Services.News.Providers.RSS;

namespace LinksNews.Services.News
{
    public class NewsProviderFactory
    {
        private readonly DataService ds;
        private readonly ExecutionService es;
        private readonly UtilsService us;
        private readonly LinksContext db;

        public NewsProviderFactory(
            DataService ds,
            ExecutionService es,
            UtilsService us,
            LinksContext db
            ) {
            this.ds = ds;
            this.es = es;
            this.us = us;
            this.db = db;
        }

        public INewsProvider GetProvider(NewsProviderDef providerDef)
        {
            INewsProvider provider = null;
            if (us.StrsEqual(providerDef.Name, "newsapi.org"))
            {
                provider = new NewsApiProvider(providerDef, ds, us);
                return provider;
            }

            provider = new RssProvider(providerDef, db, us);
            return provider;
        }
    }
}
