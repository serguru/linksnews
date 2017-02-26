using LinksNews.Core;
using LinksNews.Services.Data;
using LinksNews.Services.Data.Models;
using LinksNews.Services.News.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.News
{
    public class NewsService
    {
        private readonly NewsProviderFactory factory;
        private readonly DataService ds;
        private readonly IMemoryCache cache;
        private readonly IOptions<LinksOptions> op;
        private readonly UtilsService us;
        private readonly LinksContext db;

        List<NewsProviderDef> providerDefs;

        public List<NewsProviderDef> ProviderDefs {
            get {
                if (providerDefs != null) {
                    return providerDefs;
                }

                providerDefs = new List<NewsProviderDef>();
                providerDefs.AddRange(ds.GetNewsProviderDefs());

                return providerDefs;
            }
        }

        public NewsService(
            NewsProviderFactory factory, 
            DataService ds, 
            IMemoryCache cache,
            IOptions<LinksOptions> op,
            UtilsService us,
            LinksContext db
            )
        {
            this.factory = factory;
            this.ds = ds;
            this.cache = cache;
            this.op = op;
            this.us = us;
            this.db = db;
        }

        private void fillColumnWithNews(Column column)
        {
            if (column.ColumnTypeId == ColumnTypes.Rows)
            {
                foreach (Row row in column.Rows)
                {
                    foreach (Column col in row.Columns)
                    {
                        fillColumnWithNews(col);
                    }
                }
            }

            if (column.ColumnTypeId == ColumnTypes.News)
            {
                // TODO make a right title
                //   column.Title = column.NewsProviderSourceId;
                //column.Links = provider.GetLinks(column.NewsProviderSourceId, column).ToList();
                column.Links = GetArticles(column.NewsProviderId, column.NewsProviderSourceId);

                if (column.Links != null)
                {
                    column.Links.ForEach(x =>
                    {
                        x.ColumnId = column.Id;
                        x.ShowImage = column.ShowNewsImages;
                        x.ShowDescription = column.ShowNewsDescriptions;
                        x.ViewModeId = column.ViewModeId;
                    });
                }
            }
        }

        public void FillColumnWithNews(Column column)
        {
            if (column == null)
            {
                return;
            }
            fillColumnWithNews(column);
        }

        public void FillRowWithNews(Row row)
        {
            if (row.Columns == null)
            {
                return;
            }
            foreach (Column col in row.Columns)
            {
                fillColumnWithNews(col);
            }
        }

        public void FillPageWithNews(Page page)
        {
            foreach (Column column in page.Columns)
            {
                fillColumnWithNews(column);
            }
        }

        public IEnumerable<NewsSource> GetSources()
        {
            string key = op.Value.Cache.NewsSourcesKey;
            List<NewsSource> sources;

            if (cache.TryGetValue(key, out sources))
            {
                return sources;
            }

            sources = new List<NewsSource>();

            List<NewsProviderDef> providers = db.NewsProviderDefs.AsNoTracking().Where(x => !x.Locked).ToList();

            providers
                .ForEach(x =>
                {
                    INewsProvider provider = factory.GetProvider(x);
                    sources.AddRange(provider.GetSources());
                });

            sources = sources.OrderBy(x => x.NewsSourceId).ToList();


            cache.Set(key, sources,
                new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(op.Value.Cache.NewsSourcesRefreshInterval)));

            return sources;
        }

        public List<Link> GetArticles(long? newsProviderId, string newsSourceId)
        {
            if (us.Empty(newsSourceId) || !newsProviderId.HasValue)
            {
                return null;
            }

            NewsProviderDef providerDef = ProviderDefs.FirstOrDefault(x => x.Id == newsProviderId.Value);

            if (providerDef == null)
            {
                return null;
            }

            INewsProvider provider = factory.GetProvider(providerDef);

            if (provider == null)
            {
                return null;
            }

            string key = op.Value.Cache.NewsSourceKey + newsProviderId.Value + newsSourceId;

            List<Link> result;

            if (cache.TryGetValue(key, out result))
            {
                return result;
            }


            result = provider.GetLinks(newsSourceId).ToList();

            cache.Set(key, result,
                new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(op.Value.Cache.NewsArticlesRefreshInterval)));

            return result;
        }
    }
}
