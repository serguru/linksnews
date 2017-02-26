using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinksNews.Services.News;
using LinksNews.Services.Data.Models;
using System.Net.Http;
using Newtonsoft.Json;
using LinksNews.Services.News.Abstract;
using LinksNews.Services.Data;
using System.IO;
using Microsoft.EntityFrameworkCore;
using LinksNews.Core;
using System.Xml;
using System.ServiceModel.Syndication;

namespace LinksNews.Services.News.Providers.RSS
{
    public class RssProvider : INewsProvider
    {
        private readonly NewsProviderDef providerDef;
        public NewsProviderDef ProviderDef
        {
            get
            {
                return providerDef;
            }
        }

        LinksContext db;
        UtilsService us;

        public RssProvider(NewsProviderDef providerDef, LinksContext db, UtilsService us)
        {
            this.db = db;
            this.us = us;
            this.providerDef = providerDef;
        }

        public IEnumerable<NewsSource> GetSources()
        {
            NewsProviderDef def = db
                .NewsProviderDefs
                .AsNoTracking()
                .Where(x => x.Id == providerDef.Id)
                .FirstOrDefault();

            if (def == null)
            {
                return null;
            }

            List<NewsSource> result =
                db
                    .NewsSources
                    .Where(x => x.NewsProviderId == providerDef.Id)
                    .AsNoTracking()
                    .ToList();

                result
                    .ForEach(x =>
                    {
                        x.NewsProvider = def.Name;
                        x.NewsSourceUrl = def.Website;
                    });

            return result;
        }

        public IEnumerable<Link> GetLinks(string newsSourceId)
        {
            List<Link> result = new List<Link>();

            if (string.IsNullOrEmpty(newsSourceId)) {
                return result;
            }

            NewsSource source = db.NewsSources.AsNoTracking().FirstOrDefault(x => x.NewsSourceId == newsSourceId);

            if (source == null)
            {
                return result;
            }


            string url = source.NewsSourceUrl;
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();
            int i = 0;
            foreach (SyndicationItem item in feed.Items)
            {
                i++;
                result.Add(convertXml2Link(item,i));
            }

            return result;
        }

        private Link convertXml2Link(SyndicationItem item, int index)
        {
            Link result = new Link();
            if (item == null)
            {
                return result;
            }

            result.Id = null;
            result.ColumnId = null;
            result.LinkIndex = index;
            result.Title = item.Title != null ? item.Title.Text : "";
            result.Description = item.Summary != null ? item.Summary.Text : "";
            result.Href = item.Id != null ? item.Id : "";

            string url = item.Links != null && item.Links.Count > 1 ? item.Links[1].Uri.AbsoluteUri : "";

            result.ImageUrl = us.ProcessImageUrl(url);
            result.ButtonAccess = false;
            result.ButtonTitle = null;
            result.ButtonIndex = null;
            result.ButtonImageUrl = null;
            result.ShowImage = true;
            result.ShowDescription = true;
            result.NewsLink = true;
            result.ViewModeId = ViewModes.List;

            return result;
        }
    }
}
