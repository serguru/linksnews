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

namespace LinksNews.Services.News.Providers.NewsApi
{
    public class NewsApiProvider : INewsProvider
    {
        private readonly NewsProviderDef providerDef;
        public NewsProviderDef ProviderDef
        {
            get
                {
                    return providerDef;
                }
        }

        DataService ds;
        UtilsService us;

        public NewsApiProvider(NewsProviderDef providerDef, DataService ds, UtilsService us)
        {
            this.ds = ds;
            this.us = us;
            this.providerDef = providerDef;
        }

        private async Task<NewsApiArticleResponse> getArticlesResponse(string sourceId)
        {
            using (var client = new HttpClient())
            {

                string url = string.Format("https://newsapi.org/v1/articles?source={0}&apiKey=49c60107132542058df6c2fa3b27eb91", sourceId);

                HttpResponseMessage responseMessage = await client.GetAsync(url);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                    NewsApiArticleResponse result = JsonConvert.DeserializeObject<NewsApiArticleResponse>(responseData);

                    foreach (NewsApiArticle article in result.Articles)
                    {
                        article.UrlToImage = us.ProcessImageUrl(article.UrlToImage);
                    }

                    return result;
                }

            }

            return null;
        }

        private async Task<NewsApiSourceResponse> getSourcesResponse()
        {
            using (var client = new HttpClient())
            {

                string url = "https://newsapi.org/v1/sources";

                HttpResponseMessage responseMessage = await client.GetAsync(url);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    NewsApiSourceResponse result = JsonConvert.DeserializeObject<NewsApiSourceResponse>(responseData);

                    if (result.Sources != null)
                    {
                        foreach (NewsApiSource source in result.Sources)
                        {
                            source.UrlsToLogos.Large = us.ProcessImageUrl(source.UrlsToLogos.Large);
                            source.UrlsToLogos.Medium = us.ProcessImageUrl(source.UrlsToLogos.Medium);
                            source.UrlsToLogos.Small = us.ProcessImageUrl(source.UrlsToLogos.Small);
                        }
                    }

                    return result;
                }

            }

            return null;
        }

        private Link convertArticle2Link(NewsApiArticle article, int index)
        {
            Link result = new Link();

            result.Id = null;
            result.ColumnId = null;
            result.LinkIndex = index;
            result.Title = article.Title;
            result.Description = article.Description;
            result.Href = article.Url;
            result.ImageUrl = article.UrlToImage;
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

        private NewsSource convertApiSource2Source(NewsApiSource source, int index)
        {
            ContentCategory category = ds.GetContentCategoryByName(source.Category);
            Language language = ds.GetLanguageByCode(source.Language);
            Country country = ds.GetCountryByCode(source.Country);

            NewsSource result = new NewsSource();

            result.Id = index;
            result.NewsProviderId = ProviderDef.Id;
            result.NewsProvider = ProviderDef.Name;
            result.NewsSourceId = source.Id;
            result.NewsSourceDescription = source.Description;
            result.NewsSourceUrl = source.Url;
            result.NewsSourceSmallLogoUrl = source.UrlsToLogos.Small;
            result.NewsSourceMediumLogoUrl = source.UrlsToLogos.Medium;
            result.NewsSourceLargeLogoUrl = source.UrlsToLogos.Large;
            result.ContentCategoryId = category.Id;
            result.ContentCategory = category.Name;

            result.CountryId = country != null ? country.Id : null;
            result.CountryCode = source.Country;
            result.CountryName = country != null ? country.Name : null;

            result.LanguageId = language != null ? language.Id : null;
            result.LanguageCode = source.Language;
            result.LanguageName = language != null ? language.Name : null;

            return result;
        }

        public IEnumerable<Link> GetLinks(string sourceId)
        {
            Task<NewsApiArticleResponse> task = getArticlesResponse(sourceId);

            List<Link> result = new List<Link>();

            if (task == null)
            {
                return result;
            }

            NewsApiArticleResponse response = task.Result;

            // TODO process errors and response == null

            if (response == null)
            {
                return result;
            }

            if (response.Status != NewsResponseStatus.OK)
            {
                return result;
            }

            int i = 0;
            foreach (NewsApiArticle article in response.Articles)
            {
                i++;
                Link link = convertArticle2Link(article, i);
                result.Add(link);
            }

            return result;
        }

        public IEnumerable<NewsSource> GetSources()
        {
            Task<NewsApiSourceResponse> task = getSourcesResponse();

            List<NewsSource> result = new List<NewsSource>();

            if (task == null)
            {
                return result;
            }

            NewsApiSourceResponse response = task.Result;

            // TODO process errors

            if (response == null)
            {
                return result;
            }

            if (response.Status != NewsResponseStatus.OK)
            {
                return result;
            }

            int i = 0;
            foreach (NewsApiSource apiSource in response.Sources)
            {
                i++;
                NewsSource source = convertApiSource2Source(apiSource, i);
                result.Add(source);
            }

            return result;
        }
    }
}
