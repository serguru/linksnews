using LinksNews.Core;
using LinksNews.Services;
using LinksNews.Services.Data.Models;
using LinksNews.Services.News;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinksNews.Controllers
{
    [Route("[controller]")]
    public class PagesController : Controller
    {
        private IHostingEnvironment environment;

        private readonly PagesService ps;
        private readonly NewsService newsService;
        private readonly FileService fs;
        private readonly AdService adService;
        private readonly ExecutionService es;
        private readonly AccountService accountService;

        public PagesController(
            PagesService ps,
            NewsService newsService,
            FileService fs,
            IHostingEnvironment environment,
            AdService adService,
            ExecutionService es,
            AccountService accountService
            )
        {
            this.ps = ps;
            this.newsService = newsService;
            this.fs = fs;
            this.environment = environment;
            this.adService = adService;
            this.es = es;
            this.accountService = accountService;
        }

        [HttpPost]
        [Route("getPage")]
        public IActionResult GetPage([FromBody] JObject data)
        {
            if (data == null)
            {
                return new JsonResult(new GenericResult());
            }

            string login = data["login"].ToString();
            string page = data["page"].ToString();

            bool refreshCache = false;
            bool tmp = false;

            if (bool.TryParse(data["refreshCache"].ToString(), out tmp))
            {
                refreshCache = tmp;
            }

            return es.Execute(() =>
            {
                Page result = ps.GetFullPage(accountService.Login, login, page, refreshCache);

                if (result == null)
                {
                    // A user will be redirected to 404
                    return new JsonResult(new GenericResult());
                }

                return new JsonResult(new GenericResult(result));
            }, false);
        }

        [HttpPost]
        [Route("newsSources")]
        public IActionResult GetNewsSources()
        {
            return es.Execute(() =>
            {
                IEnumerable<NewsSource> result = newsService.GetSources();
                return new JsonResult(new GenericResult(result));
            }, false);
        }

        [HttpPost]
        [Route("linksBySource")]
        public IActionResult GetLinksBySource([FromBody] NewsSource source)
        {
            return es.Execute(() =>
            {
                if (source == null)
                {
                    return new JsonResult(new GenericResult());
                }

                IEnumerable<Link> result = newsService.GetArticles(source.NewsProviderId, source.NewsSourceId);
                return new JsonResult(new GenericResult(result));
            }, false);
        }

        [HttpPost]
        [Route("category")]
        public IActionResult GetPublicPagesByCategory([FromBody] string categoryName)
        {
            return es.Execute(() =>
            {
                IEnumerable<Page> result = ps.GetPublicPagesByCategory(categoryName);
                return new JsonResult(new GenericResult(result));
            }, false);
        }

        [HttpPost]
        [Route("myPages")]
        public IActionResult GetMyPages()
        {
            return es.Execute(() =>
            {
                IEnumerable<Page> result = ps.GetAccountPages(accountService.Login, false);
                return new JsonResult(new GenericResult(result));
            }, true);
        }

        [HttpPost]
        [Route("myPagesColumns")]
        public IActionResult GetMyPagesColumns()
        {
            return es.Execute(() =>
            {
                IEnumerable<PageData> result = ps.GetAccountPagesColumns(accountService.Login);

                LinkData linkData = ps.GetExtensionDataFromCookies();

                if (linkData != null)
                {
                    foreach (PageData pageData in result)
                    {
                        if (linkData.PageId == pageData.Id)
                        {
                            pageData.Default = true;
                            foreach (ColumnData columnData in pageData.Columns)
                            {
                                if (columnData.Id == linkData.ColumnId)
                                {
                                    columnData.Default = true;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }

                return new JsonResult(new GenericResult(result));
            }, true);
        }

        [HttpPost("getLinkById")]
        public IActionResult GetFullLinkById([FromBody]long id)
        {
            return es.Execute(() =>
            {
                Link link = ps.GetFullLinkById(id, accountService.Login);
                return new JsonResult(new GenericResult(link));
            }, true);
        }

        [HttpPost("getColumnById")]
        public IActionResult GetFullColumnById([FromBody]long id)
        {
            return es.Execute(() =>
            {
                Column column = ps.GetFullColumnById(id, accountService.Login);
                return new JsonResult(new GenericResult(column));
            }, true);
        }

        [HttpPost("getRowById")]
        public IActionResult GetFullRowById([FromBody]long id)
        {
            return es.Execute(() =>
            {
                Row row = ps.GetFullRowById(id, accountService.Login);
                return new JsonResult(new GenericResult(row));
            }, true);
        }

        [HttpPost("saveColumn")]
        public IActionResult SaveColumn([FromBody]JObject data)
        {
            return es.Execute(() =>
            {
                Column column = data["column"].ToObject<Column>();

                List<ListEntity> rows2save = new List<ListEntity>();
                data["rows2save"].ToList().ForEach(x => rows2save.Add(x.ToObject<ListEntity>()));

                List<ListEntity> links2save = new List<ListEntity>();
                data["links2save"].ToList().ForEach(x => links2save.Add(x.ToObject<ListEntity>()));

                ps.SaveColumnToDb(column, rows2save, links2save, accountService.Login);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [HttpPost("saveLink")]
        public IActionResult SaveLink([FromBody] Link link)
        {
            return es.Execute(() =>
            {
                ps.SaveLinkToDb(link, accountService.Login);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [HttpPost("addLink")]
        public IActionResult AddLink([FromBody] LinkData linkData)
        {
            return es.Execute(() =>
            {
                Link link = new Link()
                {
                    ColumnId = linkData.ColumnId,
                    Href = linkData.Href,
                    Title = linkData.Title,
                    LinkIndex = int.MaxValue,
                    ViewModeId = ViewModes.List
                };

                ps.AddLinkToDb(link, accountService.Login);
                ps.RemovePageFromCache(linkData.PageId, accountService.Login);

                ps.PutExtensionDataToCookies(linkData);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [HttpPost("saveRow")]
        public IActionResult SaveRow([FromBody]JObject data)
        {
            return es.Execute(() =>
            {
                Row row = data["row"].ToObject<Row>();

                List<ListEntity> columns2save = new List<ListEntity>();
                data["columns2save"].ToList().ForEach(x => columns2save.Add(x.ToObject<ListEntity>()));

                ps.SaveRowToDb(row, columns2save, accountService.Login);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [HttpPost("savePage")]
        public IActionResult SavePage([FromBody]JObject data)
        {
            return es.Execute(() =>
            {
                Page page = data["page"].ToObject<Page>();
                List<ListEntity> columns2save = new List<ListEntity>();
                data["columns2save"].ToList().ForEach(x => columns2save.Add(x.ToObject<ListEntity>()));

                // TODO: check if the current account is a page's owner
                ps.SavePageToDb(page, columns2save, accountService.Login);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [HttpPost("savePagesList")]
        public IActionResult SavePagesList([FromBody] List<ListEntity> pageIds)
        {
            return es.Execute(() =>
            {
                // TODO: check if the current account is a pages' owner
                ps.SavePagesListToDb(pageIds, accountService.Login);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [HttpPost]
        [Route("uploadImage4Link")]
        public IActionResult UploadImage4Link(IFormFile file)
        {
            return es.Execute(() =>
            {
                string path = fs.UploadElementImage(accountService.Login, PageElement.Link, file).Result;
                return new JsonResult(new GenericResult(new { imageUrl = path }));
            }, true);
        }

        [HttpPost]
        [Route("deleteImage4Link")]
        public IActionResult DeleteImage4Link([FromBody] long linkId)
        {
            return es.Execute(() =>
            {
                fs.DeleteElementImage(linkId, accountService.Login, PageElement.Link);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [HttpPost]
        [Route("uploadImage4Page")]
        public IActionResult UploadImage4Page(IFormFile file)
        {
            return es.Execute(() =>
            {
                string path = fs.UploadElementImage(accountService.Login, PageElement.Page, file).Result;
                return new JsonResult(new GenericResult(new { imageUrl = path }));
            }, true);
        }

        [HttpPost]
        [Route("deleteImage4Page")]
        public IActionResult DeleteImage4Page([FromBody] long pageId)
        {
            return es.Execute(() =>
            {
                fs.DeleteElementImage(pageId, accountService.Login, PageElement.Page);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [HttpPost]
        [Route("deleteImage4Column")]
        public IActionResult DeleteImage4Column([FromBody] long columnId)
        {
            return es.Execute(() =>
            {
                fs.DeleteElementImage(columnId, accountService.Login, PageElement.Column);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [HttpPost]
        [Route("uploadImage4Column")]
        public IActionResult UploadImage4Column(IFormFile file)
        {
            return es.Execute(() =>
            {
                string path = fs.UploadElementImage(accountService.Login, PageElement.Column, file).Result;
                return new JsonResult(new GenericResult(new { imageUrl = path }));
            }, true);
        }

        [HttpPost]
        [Route("contentCategories")]
        public IActionResult GetContentCategories()
        {
            return es.Execute(() =>
            {
                IEnumerable<ContentCategory> result = ps.GetContentCategories();
                return new JsonResult(new GenericResult(result));
            }, false);
        }

        [HttpPost]
        [Route("addExternalPage")]
        public IActionResult AddExternalPage([FromBody] long pageId)
        {
            return es.Execute(() =>
            {
                ps.AddExternalPage(pageId, accountService.Login);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [HttpPost]
        [Route("copyPage")]
        public IActionResult CopyPage([FromBody] long pageId)
        {
            return es.Execute(() =>
            {
                ps.CopyPageToDb(pageId, accountService.Login);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [HttpPost]
        [Route("createPage")]
        public IActionResult CopyPage([FromBody] CreatePageData data)
        {
            return es.Execute(() =>
            {
                string pageName = ps.CreatePage(data, accountService.Login);
                return new JsonResult(new GenericResult(pageName));
            }, true);
        }

        [HttpPost]
        [Route("removePageFromCache")]
        public IActionResult RemovePageFromCache([FromBody] string key)
        {
            return es.Execute(() =>
            {
                ps.RemovePageFromCache(key);
                return new JsonResult(new GenericResult());
            }, true);
        }
    }
}