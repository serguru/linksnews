using LinksNews.Core;
using LinksNews.Services;
using LinksNews.Services.Data;
using LinksNews.Services.Data.Models;
using LinksNews.Services.News;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LinksNews.Services
{
    public class PagesService
    {
        private readonly LinksContext db;
        private readonly ExecutionService es;
        private readonly FileService fs;
        private readonly IOptions<LinksOptions> op;
        private readonly NewsService newsService;
        private readonly IMemoryCache cache;
        private readonly UtilsService us;
        private readonly AdService adService;
        private readonly IHttpContextAccessor httpContext;


        public PagesService(
            LinksContext db, 
            ExecutionService es,
            FileService fs,
            IOptions<LinksOptions> op,
            NewsService newsService,
            IMemoryCache cache,
            UtilsService us,
            IHttpContextAccessor httpContext,
            AdService adService
            )
        {
            this.db = db;
            this.es = es;
            this.fs = fs;
            this.op = op;
            this.newsService = newsService;
            this.cache = cache;
            this.us = us;
            this.adService = adService;
            this.httpContext = httpContext;
        }

        public AccountService accountService { get; set; }

        public IEnumerable<Page> GetAccountPages(string login, bool publicOnly)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }

            Account account = db.Accounts.AsNoTracking().Where(x => x.Login == login).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            List<Page> result = db.Pages.AsNoTracking()
                .Where(x => (publicOnly && x.PublicAccess || !publicOnly) && x.AccountId == account.Id)
                .ToList();

            result.ForEach(x => 
            {
                x.Login = account.Login;
                x.ImageUrl = fs.GetPageElementImageUrl(x.Id.Value, PageElement.Page, x.Login);
            });

            List<Tuple<AccountPage, Page>> ap = db
                .AccountPages.AsNoTracking()
                .Where(x => x.AccountId == account.Id)
                .Join
                (
                    db.Pages.AsNoTracking().Where(y => y.PublicAccess),
                    a => a.PageId,
                    b => b.Id,
                    (a, b) => new Tuple<AccountPage, Page>
                    (
                        a,
                        b
                    )
                )
                .ToList();

            ap.ForEach(x => x.Item2.PageIndex = x.Item1.PageIndex);

            ap
                .Select( x => x.Item2)
                .Join
                (
                    db.Accounts.AsNoTracking(),
                    a => a.AccountId,
                    b => b.Id,
                    (x, y) => new
                    {
                        page = x,
                        account = y
                    }
                )
                .ToList()
                .ForEach(x => 
                {
                    x.page.Login = x.account.Login;
                    x.page.ImageUrl = fs.GetPageElementImageUrl(x.page.Id.Value, PageElement.Page, x.page.Login);
                });


            result.AddRange(ap.Select(x => x.Item2));

            return result.OrderBy(x => x.PageIndex);
        }


        private void addNestedLinksColumns(long columnId, List<ColumnData> columns)
        {
            List<Row> rows = db
                .Rows
                .AsNoTracking()
                .Where(x => x.ColumnId == columnId)
                .ToList();

            foreach (Row row in rows)
            {
                List<Column> rowColumns = db
                    .Columns
                    .AsNoTracking()
                    .Where(x => x.RowId == row.Id && x.ColumnTypeId != ColumnTypes.News)
                    .ToList();

                foreach (Column column in rowColumns)
                {
                    if (column.ColumnTypeId == ColumnTypes.Links)
                    {
                        columns.Add(new ColumnData() { Id = column.Id.Value, Title = column.Title });
                        continue;
                    }

                    addNestedLinksColumns(column.Id.Value, columns);
                }
            }
        }

        public IEnumerable<PageData> GetAccountPagesColumns(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }

            Account account = db.Accounts.AsNoTracking().Where(x => x.Login == login).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            List<PageData> result = db.Pages.AsNoTracking()
                .Where(x => x.AccountId == account.Id)
                .OrderBy(x => x.Title)
                .Select(x => new PageData() { Id = x.Id.Value, Title = x.Title})
                .ToList();

            foreach (PageData pageData in result)
            {
                pageData.Columns = new List<ColumnData>();

                List<Column> pageColumns = db
                    .Columns
                    .AsNoTracking()
                    .Where(x => x.PageId == pageData.Id && x.ColumnTypeId != ColumnTypes.News)
                    .ToList();

                foreach (Column column in pageColumns)
                {
                    if (column.ColumnTypeId == ColumnTypes.Links)
                    {
                        pageData.Columns.Add(new ColumnData() { Id = column.Id.Value, Title = column.Title });
                        continue;
                    }

                    addNestedLinksColumns(column.Id.Value, pageData.Columns);
                }

                pageData.Columns = pageData.Columns.OrderBy(x => x.Title).ToList();
            }

            result = result
                .Where(x => x.Columns != null && x.Columns.Count > 0)
                .ToList();

            return result;
        }

        public IEnumerable<Page> GetPublicPagesByCategory(string categoryName)
        {
            ContentCategory category = db.ContentCategories.Where(x => x.Name == categoryName).FirstOrDefault();

            if (category == null)
            {
                return null;
            }

            long categoryId = category.Id.Value;

            List<long> pageIds = db.PageCategories.Where(x => x.ContentCategoryId == categoryId).Select(x => x.PageId.Value).Distinct().ToList();

            if (!pageIds.Any())
            {
                return new List<Page>();
            }

            // TODO: optimize code
            List<Page> result = db.Pages.Where(x => x.PublicAccess && pageIds.Any(y => y == x.Id)).ToList();

            result.ForEach(x => 
            {
                x.Login = db.Accounts.First(y => y.Id == x.AccountId).Login;
                x.ImageUrl = fs.GetPageElementImageUrl(x.Id.Value, PageElement.Page, x.Login);
            });

            return result;
        }

        public List<CategoryCount> GetCategoriesCounts()
        {
            List<CategoryCount> result = db.CategoryCounts.AsNoTracking().FromSql("spCategoryCount").ToList();
            return result;
        }

        public List<ContentCategory> GetContentCategories()
        {
            List<CategoryCount> counts = GetCategoriesCounts();
            List<NewsSource> sources =
            newsService.GetSources().ToList();
            counts.ForEach(x =>
            {
                x.NewsSources = sources.Where(y => x.Id == y.ContentCategoryId).Count();
            });

            List<ContentCategory> result = db
                .ContentCategories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToList();

            ContentCategory all = new ContentCategory();
            all.Id = -1;
            all.Name = "all";
            result.Insert(0, all);

            int allNewsCount = 0;

            result
                .ForEach(x =>
                {
                    CategoryCount count = counts.FirstOrDefault(y => y.Id == x.Id);

                    if (count != null)
                    {
                        x.PagesCount = count.Pages;
                        x.AuthorsCount = count.Authors;
                        x.NewsSourcesCount = count.NewsSources;
                        allNewsCount += count.NewsSources.HasValue ? count.NewsSources.Value : 0;
                    }
                    else
                    {
                        x.PagesCount = 0;
                        x.AuthorsCount = 0;
                        x.NewsSourcesCount = 0;
                    }
                });

            all.NewsSourcesCount = allNewsCount;
            return result;
        }

        public Page GetFullPage(string authLogin, string ownerLogin, string page, bool refreshCache)
        {
            if (us.Empty(ownerLogin) || us.Empty(page))
            {
                return null;
            }

            string key = op.Value.Cache.PageKey + ownerLogin + page;

            Page result;

            if (!refreshCache && cache.TryGetValue(key, out result))
            {
                return result;
            }

            Account owner = accountService.GetAccountByLogin(ownerLogin);
            if (owner == null)
            {
                return null;
            }

            bool checkPublished = !string.Equals(authLogin, ownerLogin, StringComparison.OrdinalIgnoreCase);

            result = db.Pages.FirstOrDefault(x =>
                    x.AccountId == owner.Id &&
                        string.Equals(x.Name, page, StringComparison.OrdinalIgnoreCase) &&
                        (x.PublicAccess || !checkPublished)

                );

            if (result == null)
            {
                return null;
            }

            result.Categories = db.ContentCategories.OrderBy(x => x.Name).ToList();
            List<PageCategory> pc = db.PageCategories.Where(x => x.PageId == result.Id).ToList();
            result.Categories.ForEach(x => x.Selected = pc.Any(y => y.ContentCategoryId == x.Id));

            result.Login = owner.Login;
            result.ImageUrl = fs.GetPageElementImageUrl(result.Id.Value, PageElement.Page, owner.Login);

            result.Columns = 
                GetAllColumnsByPage(result.Id.Value)
                .OrderBy(x => x.ColumnIndex)
                .ToList();
            if (result.Columns == null)
            {
                return result;
            }

            result.Columns.ForEach(x => FillColumn(x, owner.Login));

            newsService.FillPageWithNews(result);
            adService.FillPageWithAds(result, authLogin);

            cache.Set(key, result,
                new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(op.Value.Cache.PageRefreshInterval)));

            return result;
        }

        private void addColumnChildrenToDb(List<Row> rows, List<Link> links, long columnId)
        {
            if (rows != null)
            {
                foreach (Row row in rows)
                {
                    row.Id = null;
                    row.ColumnId = columnId;

                    List<Column> columns = row.Columns;
                    row.Columns = null;
                    db.Rows.Add(row);
                    db.SaveChanges();
                    if (columns == null)
                    {
                        continue;
                    }

                    foreach (Column col in columns)
                    {
                        col.PageId = null;
                        col.Id = null;
                        col.RowId = row.Id;

                        List<Row> colRows = col.Rows;
                        col.Rows = null;
                        List<Link> colLinks = col.Links;
                        col.Links = null;

                        db.Columns.Add(col);
                        db.SaveChanges();

                        addColumnChildrenToDb(colRows, colLinks, col.Id.Value);
                    }
                }
            }

            if (links == null)
            {
                return;
            }

            foreach (Link link in links)
            {
                link.Id = null;
                link.ColumnId = columnId;
                db.Links.Add(link);
            }
            db.SaveChanges();
        }

        private void addPageChildrenToDb(List<Column> columns, long pageId)
        {
            if (columns == null)
            {
                return;
            }
            foreach (Column column in columns)
            {
                column.PageId = pageId;
                column.Id = null;
                column.RowId = null;

                List<Row> rows = column.Rows;
                column.Rows = null;
                List<Link> links = column.Links;
                column.Links = null;

                db.Columns.Add(column);
                db.SaveChanges();
                addColumnChildrenToDb(rows, links, column.Id.Value);
            }
        }

        public Column GetFullColumnById(long columnId, string login)
        {
            // TODO: add check for column's owner
            Column column = db.Columns.FirstOrDefault(x => x.Id == columnId);
            FillColumn(column, login);

            newsService.FillColumnWithNews(column);
            adService.FillColumnWithAds(column, login);

            return column;
        }

        public Link GetFullLinkById(long linkId, string login)
        {
            // TODO: add check for column's owner
            Link link = db.Links.FirstOrDefault(x => x.Id == linkId);
            link.ImageUrl = fs.GetPageElementImageUrl(link.Id.Value, PageElement.Link, login);
            adService.FillLinkWithAds(link, login);
            return link;
        }

        public Row GetFullRowById(long rowId, string login)
        {
            // TODO: add check for row's owner
            Row row = db.Rows.FirstOrDefault(x => x.Id == rowId);

            if (row == null)
            {
                return null;
            }

            row.Columns = db.Columns
                    .Where(y => y.RowId == rowId)
                    .OrderBy(y => y.ColumnIndex)
                    .ToList();

            if (row.Columns == null)
            {
                row.Columns = new List<Column>();
            }

            row.Columns.ForEach(y => FillColumn(y, login));

            newsService.FillRowWithNews(row);
            adService.FillRowWithAds(row, login);

            return row;
        }

        public string CreatePage(CreatePageData data, string login)
        {
            if (data == null
                || string.IsNullOrWhiteSpace(data.PageTitle)
                || string.IsNullOrWhiteSpace(data.LinkTitle)
                || string.IsNullOrWhiteSpace(data.LinkAddress)
                )
            {
                es.ThrowException("Wrong data passed to create a page");
            }

            if (string.IsNullOrWhiteSpace(login))
            {
                es.ThrowException("No login to update page");
            }

            db.Database.BeginTransaction();
            try
            {
                Account account = db.Accounts.FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

                if (account == null)
                {
                    es.ThrowException("No account found for login {0}", login);
                }
                if (account.Locked)
                {
                    es.ThrowInfoException("Account {0} is locked", account.Login);
                }

                Page page = CreatePageInDb(account.Id.Value, int.MaxValue);
                page.Title = data.PageTitle;
                db.Pages.Update(page);
                db.SaveChanges();

                Column column = createColumnInDb(page.Id, null, 1, 12);

                Link link = createLinkInDb(column.Id.Value, 1);
                link.Title = data.LinkTitle;
                string tmp = data.LinkAddress.ToLower();
                if (!tmp.StartsWith("http://") && !tmp.StartsWith("https://") && !tmp.StartsWith("file://"))
                {
                    data.LinkAddress = "http://" + data.LinkAddress;
                }
                link.Href = data.LinkAddress;
                db.Links.Update(link);
                db.SaveChanges();

                db.Database.CommitTransaction();
                return page.Name;
            }
            catch (Exception)
            {
                db.Database.RollbackTransaction();
                throw;
            }

        }

        private long addPage(Page source)
        {
            source.Id = null;
            List<Column> columns = source.Columns;
            source.Columns = null;
            db.Pages.Add(source);
            db.SaveChanges();
            long id = source.Id.Value;
            addPageChildrenToDb(columns, id);
            return id;
        }

        private void deleteColumnFromDb(long columnId)
        {
            List<long> rowIds = db
                .Rows
                .Where(x => x.ColumnId == columnId)
                .Select(x => x.Id.Value)
                .ToList();

            foreach (long rowId in rowIds)
            {
                deleteRowFromDb(rowId);
            }

            List<Link> links = db.Links.Where(x => x.ColumnId == columnId).ToList();
            db.Links.RemoveRange(links);
            db.SaveChanges();

            Column column = db.Columns.Where(x => x.Id == columnId).FirstOrDefault();
            if (column != null)
            {
                db.Columns.Remove(column);
            }
            db.SaveChanges();

            Account account = GetAccountByColumnId(columnId);
            if (account == null)
            {
                return;
            }
            fs.DeleteElementImage(columnId, account.Login, PageElement.Column);
        }

        private void deleteRowFromDb(long rowId)
        {
            List<long> columnIds = db
                .Columns
                .Where(x => x.RowId == rowId && x.Id.HasValue)
                .Select(y => y.Id.Value)
                .ToList();

            foreach (long columnId in columnIds)
            {
                deleteColumnFromDb(columnId);
            }

            Row row = db.Rows.Where(x => x.Id == rowId).FirstOrDefault();
            if (row == null) {
                return;
            }
            db.Rows.Remove(row);
            db.SaveChanges();
        }

        private Row createRowInDb(long columnId, int index)
        {
            Row row = new Row();
            row.Id = null;
            row.RowIndex = index;
            row.ColumnId = columnId;
            row.Title = getNewRowTitle(columnId);
            row.ShowTitle = true;
            db.Rows.Add(row);
            db.SaveChanges();
            return row;
        }

        private Link createLinkInDb(long columnId, int index)
        {
            Link link = new Link();
            link.Id = null;
            link.ColumnId = columnId;
            link.Href = "http://#";
            link.LinkIndex = index;
            link.Title = getNewLinkTitle(columnId);
            link.ViewModeId = ViewModes.List;
            db.Links.Add(link);
            db.SaveChanges();
            return link;
        }

        public void AddExternalPage(long pageId, string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                es.ThrowException("No login passed to add an external page to");
            }

            db.Database.BeginTransaction();
            try
            {
                Account account = db
                    .Accounts
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

                if (account == null)
                {
                    es.ThrowException("Login to add an external page to not found in db");
                }
                if (account.Locked)
                {
                    es.ThrowInfoException("Account {0} is locked", account.Login);
                }

                Page externalPage = db.Pages.AsNoTracking().FirstOrDefault(x => x.Id == pageId);
                if (externalPage == null)
                {
                    es.ThrowException("A page to add not found in db");
                }

                if (!externalPage.PublicAccess)
                {
                    es.ThrowInfoException("Cannot add a page becauers it is not published");
                }

                if (externalPage.AccountId == account.Id)
                {
                    es.ThrowInfoException("Cannot add a page because it belongs to the same account");
                }

                bool alreadyAdded = db.AccountPages.AsNoTracking().Any(x => x.AccountId == account.Id && x.PageId == pageId);
                if (alreadyAdded)
                {
                    es.ThrowInfoException("Cannot add a page because it is already added");
                }

                AccountPage ap = new AccountPage();
                ap.PageId = pageId;
                ap.AccountId = account.Id;
                ap.PageIndex = int.MaxValue;

                db.AccountPages.Add(ap);
                db.SaveChanges();

                db.Database.CommitTransaction();
            }
            catch (Exception)
            {
                db.Database.RollbackTransaction();
                throw;
            }
        }

        private void deleteLinkFromDb(long linkId)
        {
            db.Links.Remove(db.Links.FirstOrDefault(x => x.Id == linkId));
            db.SaveChanges();
            Account account = GetAccountByLinkId(linkId);
            if (account == null)
            {
                return;
            }
            if (account.Locked)
            {
                es.ThrowInfoException("Account {0} is locked", account.Login);
            }
            fs.DeleteElementImage(linkId, account.Login, PageElement.Link);
        }

        private void updateColumnToDb(Column column, List<ListEntity> rowIds, List<ListEntity> linkIds)
        {
            long columnId = column.Id.Value;
            int i = 0;
            foreach (ListEntity entity in rowIds)
            {
                switch (entity.Action)
                {
                    case RowAction.Inserted:
                        createRowInDb(columnId, i);
                        i++;
                        break;
                    case RowAction.Deleted:
                        deleteRowFromDb(entity.Id.Value);
                        break;
                    default:
                        updateRowInDb(entity.Id.Value, i);
                        i++;
                        break;
                }
            }

            foreach (ListEntity entity in linkIds)
            {
                switch (entity.Action)
                {
                    case RowAction.Inserted:
                        createLinkInDb(columnId, i);
                        i++;
                        break;
                    case RowAction.Deleted:
                        deleteLinkFromDb(entity.Id.Value);
                        break;
                    default:
                        updateLinkInDb(entity.Id.Value, i);
                        i++;
                        break;
                }
            }

            column.Rows = null;
            column.Links = null;
            db.Columns.Update(column);
            
            db.SaveChanges();

            
            //List<Row> rows = column.Rows == null ? new List<Row>() : column.Rows;
            //column.Rows = null;

            //List<Link> links = column.Links == null ? new List<Link>() : column.Links;
            //column.Links = null;

            //if (newColumn)
            //{
            //    context.Columns.Add(column);
            //}
            //else
            //{
            //    context.Columns.Update(column);
            //}

            //context.SaveChanges();

            //long columnId = column.Id.Value;

            //if (!newColumn)
            //{
            //    List<long> rowIds2Delete = context.Rows.Where(x => x.ColumnId == columnId && 
            //        !rows.Any(y => y.Id == x.Id)).Select(x => x.Id.Value).ToList();

            //    foreach (long rowId in rowIds2Delete)
            //    {
            //        deleteRowFromDb(rowId);
            //    }
            //}


            //long i = 0;
            //foreach (Row row in rows)
            //{
            //    i++;
            //    row.Id = newColumn ? null : row.Id;
            //    row.ColumnId = columnId;
            //    row.RowIndex = i;
            //    updateRowToDb(row);
            //}

            //if (!newColumn)
            //{
            //    List<Link> linkIds2Delete = context.Links.Where(x => x.ColumnId == columnId &&
            //        (
            //            !links.Any(y => y.Id == x.Id)
            //        )).ToList();

            //    if (linkIds2Delete.Any())
            //    {
            //        context.Links.RemoveRange(linkIds2Delete);
            //    }
            //}

            //long j = 0;
            //links.Where(x => !x.NewsLink).ToList().ForEach(x => 
            //{

            //    j++;
            //    x.ColumnId = column.Id;
            //    x.LinkIndex = j;

            //    if (newColumn || !x.Id.HasValue)
            //    {
            //        x.Id = null;
            //        context.Links.Add(x);
            //    }
            //    else
            //    {
            //        context.Links.Update(x);
            //    }
            //});
            //context.SaveChanges();
        }

        private void updateRowToDb(Row row, List<ListEntity> columnIds)
        {
            long rowId = row.Id.Value;
            int i = 1;
            foreach (ListEntity entity in columnIds)
            {
                switch (entity.Action)
                {
                    case RowAction.Inserted:
                        createColumnInDb(null, rowId, i, (long)entity.Props[0]);
                        i++;
                        break;
                    case RowAction.Deleted:
                        deleteColumnFromDb(entity.Id.Value);
                        break;
                    default:
                        updateColumnInDb(entity.Id.Value, i, (long)entity.Props[0]);
                        i++;
                        break;
                }
            }

            row.Columns = null;
            db.Rows.Update(row);
            db.SaveChanges();
        }

        private void updateLinkToDb(Link link)
        {
            db.Links.Update(link);
            db.SaveChanges();
        }

        private Column createColumnInDb(long? pageId, long? rowId, int index, long columnWidth)
        {
            Column column = new Column();
            column.Id = null;
            column.ColumnIndex = index;
            column.ColumnWidth = columnWidth;
            column.ColumnTypeId = ColumnTypes.Links;
            column.PageId = pageId;
            column.RowId = rowId;
            column.Title = pageId.HasValue ? GetNewPageColumnTitle(pageId.Value) : GetNewRowColumnTitle(rowId.Value);
            column.ShowTitle = true;
            column.ShowImage = false;
            column.ShowDescription = false;
            column.ShowNewsImages = true;
            column.ShowNewsDescriptions = true;
            column.ViewModeId = ViewModes.List;
            column.ShowAd = false;
            db.Columns.Add(column);
            db.SaveChanges();
            return column;
        }

        private void updateColumnInDb(long columnId, int index, long columnWidth)
        {
            Column column = db.Columns.FirstOrDefault(x => x.Id == columnId);
            if (column == null)
            {
                return;
            }
            column.ColumnIndex = index;
            column.ColumnWidth = columnWidth;
            db.SaveChanges();
        }

        private void updateRowInDb(long rowId, int index)
        {
            Row row = db.Rows.FirstOrDefault(x => x.Id == rowId);
            if (row == null)
            {
                return;
            }
            row.RowIndex = index;
            db.SaveChanges();
        }

        private void updateLinkInDb(long linkId, int index)
        {
            Link link = db.Links.FirstOrDefault(x => x.Id == linkId);
            if (link == null)
            {
                return;
            }
            link.LinkIndex = index;
            db.SaveChanges();
        }

        private void updatePageToDb(Page page, List<ListEntity> columnIds, long accountId)
        {
            long pageId = page.Id.Value;
            int i = 1;
            foreach (ListEntity entity in columnIds)
            {
                switch (entity.Action)
                {
                    case RowAction.Inserted:
                        createColumnInDb(pageId, null, i, (long)entity.Props[0]);
                        i++;
                        break;
                    case RowAction.Deleted:
                        deleteColumnFromDb(entity.Id.Value);
                        break;
                    default:
                        updateColumnInDb(entity.Id.Value, i, (long)entity.Props[0]);
                        i++;
                        break;
                }
            }
            page.Columns = null;

            if (page.Categories != null)
            {
                foreach (ContentCategory category in page.Categories)
                {
                    PageCategory pc = db.PageCategories.FirstOrDefault(x => x.PageId == page.Id && x.ContentCategoryId == category.Id);

                    if (category.Selected)
                    {
                        if (pc == null)
                        {
                            pc = new PageCategory();
                            pc.PageId = page.Id;
                            pc.ContentCategoryId = category.Id;
                            db.PageCategories.Add(pc);
                        }
                    }
                    else
                    {
                        if (pc != null)
                        {
                            db.PageCategories.Remove(pc);
                        }
                    }
                }

                db.SaveChanges();

            }

            db.Pages.Update(page);
            db.SaveChanges();
        }

        // Adds new or updates existing page including ALL children changes
        //private void updateFullPageToDb(Page page)
        //{
        //    bool newPage = !page.Id.HasValue;

        //    if (!newPage)
        //    {
        //        if (!context.Pages.Any(x => x.Id == page.Id))
        //        {
        //            page.Id = null;
        //            newPage = true;
        //        }
        //    }

        //    List<Column> columns = page.Columns == null ? new List<Column>() : page.Columns;
        //    page.Columns = null;

        //    if (newPage)
        //    {
        //        Account account = context.Accounts.Where(x => x.Login == page.Login).FirstOrDefault();

        //        if (account == null)
        //        {
        //            es.ThrowException("Cannot add a page for unknown account");
        //        }

        //        page.AccountId = account.Id;
        //        context.Pages.Add(page);
        //    }
        //    else
        //    {
        //        context.Pages.Update(page);
        //        //context.Entry<Page>(page)
        //        //    .State = EntityState.Modified;
        //    }

        //    context.SaveChanges();

        //    long pageId = page.Id.Value;

        //    if (!newPage)
        //    {
        //        List<long> columnIdsToDelete = context.Columns.Where(x => x.PageId == pageId &&
        //            !columns.Any(y => x.Id == y.Id)).Select(x => x.Id.Value).ToList();

        //        foreach (long columnId in columnIdsToDelete)
        //        {
        //            deleteColumnFromDb(columnId);
        //        }
        //    }


        //    long i = 0;
        //    foreach (Column column in columns)
        //    {
        //        i++;
        //        column.RowId = null;
        //        column.PageId = page.Id;
        //        column.ColumnIndex = i;
        //        updateColumnToDb(column);
        //    }
        //}

        public Boolean PagesBelongToLogin(List<long> pageIds, long accountId)
        {
            List<long> existingPageIds =
                db
                    .Pages
                    .AsNoTracking()
                    .Where(x => x.AccountId == accountId).Select(y => y.Id.Value)
                    .ToList();

            List<long> externalPageIds =
                db
                    .AccountPages
                    .AsNoTracking()
                    .Where(x => x.AccountId == accountId)
                    .Join
                    (
                        db.Pages
                        .AsNoTracking()
                        .Where(x => x.PublicAccess),
                        x => x.PageId,
                        y => y.Id,
                        (x, y) => y
                    )
                    .Select(x => x.Id.Value)
                    .Distinct()
                    .ToList();

            existingPageIds.AddRange(externalPageIds);

            if (pageIds.Any(x => !existingPageIds.Any(y => x == y)) || existingPageIds.Any(x => !pageIds.Any(y => x == y)))
            {
                return false;
            }

            return true;
        }

        public Boolean ColumnsReferencingPage(List<long> columnIds, long pageId)
        {
            List<long> existingColumnIds = db.Columns.Where(x => x.PageId == pageId).Select(y => y.Id.Value).ToList();

            if (columnIds.Any(x => !existingColumnIds.Any(y => x == y)) || existingColumnIds.Any(x => !columnIds.Any(y => x == y)))
            {
                return false;
            }

            return true;
        }

        public Boolean ColumnsReferencingRow(List<long> columnIds, long rowId)
        {
            List<long> existingColumnIds = db.Columns.Where(x => x.RowId == rowId).Select(y => y.Id.Value).ToList();

            if (columnIds.Any(x => !existingColumnIds.Any(y => x == y)) || existingColumnIds.Any(x => !columnIds.Any(y => x == y)))
            {
                return false;
            }

            return true;
        }

        public Boolean RowsReferencingColumn(List<long> rowIds, long columnId)
        {
            List<long> existingRowIds = db.Rows.Where(x => x.ColumnId == columnId).Select(y => y.Id.Value).ToList();

            if (rowIds.Any(x => !existingRowIds.Any(y => x == y)) || existingRowIds.Any(x => !rowIds.Any(y => x == y)))
            {
                return false;
            }

            return true;
        }

        public Boolean LinksReferencingColumn(List<long> linkIds, long columnId)
        {
            List<long> existingLinkIds = db.Links.Where(x => x.ColumnId == columnId).Select(y => y.Id.Value).ToList();

            if (linkIds.Any(x => !existingLinkIds.Any(y => x == y)) || existingLinkIds.Any(x => !linkIds.Any(y => x == y)))
            {
                return false;
            }

            return true;
        }

        public string GetNewPageName(long accountId, string prefix)
        {
            string result = prefix;

            for (int i = 1; i < int.MaxValue; i++)
            {
                if (!db.Pages.Any(x => x.AccountId == accountId &&
                    x.Name.Equals(result, StringComparison.OrdinalIgnoreCase)))
                {
                    break;
                }

                result = prefix + i;
            }

            return result;
        }

        public string GetNewPageColumnTitle(long pageId)
        {
            string prefix = "New Column";
            string result = prefix;

            for (int i = 1; i < int.MaxValue; i++)
            {
                if (!db.Columns.Any(x => x.PageId == pageId &&
                    x.Title.Equals(result, StringComparison.OrdinalIgnoreCase)))
                {
                    break;
                }

                result = prefix + " " + i;
            }

            return result;
        }

        private string getNewRowTitle(long columnId)
        {
            string prefix = "New Row";
            string result = prefix;

            for (int i = 1; i < int.MaxValue; i++)
            {
                if (!db.Rows.Any(x => x.ColumnId == columnId &&
                    x.Title.Equals(result, StringComparison.OrdinalIgnoreCase)))
                {
                    break;
                }

                result = prefix + " " + i;
            }

            return result;
        }

        private string getNewLinkTitle(long columnId)
        {
            string prefix = "New Link";
            string result = prefix;

            for (int i = 1; i < int.MaxValue; i++)
            {
                if (!db.Links.Any(x => x.ColumnId == columnId &&
                    x.Title.Equals(result, StringComparison.OrdinalIgnoreCase)))
                {
                    break;
                }

                result = prefix + " " + i;
            }

            return result;
        }

        public string GetNewRowColumnTitle(long rowId)
        {
            string prefix = "New Column";
            string result = prefix;

            for (int i = 1; i < int.MaxValue; i++)
            {
                if (!db.Columns.Any(x => x.RowId == rowId &&
                    x.Title.Equals(result, StringComparison.OrdinalIgnoreCase)))
                {
                    break;
                }

                result = prefix + " " + i;
            }

            return result;
        }

        public Page CreatePageInDb(long accountId, int index)
        {
            Page page = new Page();
            page.Id = null;
            page.PageIndex = index;
            page.AccountId = accountId;
            page.Name = GetNewPageName(accountId, "NewPage");
            page.Title = page.Name;
            page.ShowTitle = true;
            db.Pages.Add(page);
            db.SaveChanges();
            return page;
        }

        private void updatePageIndex(long pageId, int index, long accountId)
        {
            bool ownPage = db.Pages.Any(x => x.AccountId == accountId && x.Id == pageId);

            if (!ownPage)
            {
                AccountPage accountPage = db.AccountPages.FirstOrDefault((x => x.PageId == pageId && x.AccountId == accountId));
                if (accountPage == null)
                {
                    return;
                }
                accountPage.PageIndex = index;
            }
            else
            {
                Page page = db.Pages.FirstOrDefault(x => x.Id == pageId);
                if (page == null)
                {
                    return;
                }
                page.PageIndex = index;
            }


            db.SaveChanges();
        }

        public void DeletePageFromDb(long pageId, long accountId)
        {
            bool ownPage = db.Pages.Any(x => x.AccountId == accountId && x.Id == pageId);

            if (!ownPage)
            {
                db.AccountPages.Remove(db.AccountPages.FirstOrDefault(x => x.PageId == pageId && x.AccountId == accountId));
                db.SaveChanges();
                return;
            }

            db.AccountPages.RemoveRange(db.AccountPages.Where(x => x.PageId == pageId));
            db.SaveChanges();

            db
                .PageCategories
                .RemoveRange(db
                                .PageCategories
                                .Where(x => x.PageId == pageId)
                            );

            db.SaveChanges();

            //delete messageRecipient
            //from messageRecipient a
            //     join message b on b.Id = a.MessageId and b.Id = 1

            db
                .MessageRecipients
                .RemoveRange(db
                                .Messages
                                .Where(x => x.PageId == pageId)
                                .Join(db.MessageRecipients,
                                    x => x.Id,
                                    y => y.MessageId,
                                    (x,y) =>  new { x, y }
                                    )
                                    .Select(x => x.y)
                            );

            db.SaveChanges();

            db
                .Messages
                .RemoveRange(db
                                .Messages
                                .Where(x => x.PageId == pageId)
                            );

            db.SaveChanges();

            db
                .Columns
                .Where(x => x.PageId == pageId)
                .Select(x => x.Id.Value)
                .ToList()
                .ForEach(x => deleteColumnFromDb(x));

            db.Pages.Remove(db.Pages.First(x => x.Id == pageId));

            db.SaveChanges();


            Account account = GetAccountByPageId(pageId);
            if (account == null)
            {
                return;
            }
            fs.DeleteElementImage(pageId, account.Login, PageElement.Page);
        }

        private void updatePagesListToDb(List<ListEntity> pageIds, long accountId)
        {
            int i = 0;
            foreach (ListEntity entity in pageIds)
            {
                switch (entity.Action)
                {
                    case RowAction.Inserted:
                        CreatePageInDb(accountId, i);
                        i++;
                        break;
                    case RowAction.Deleted:
                        DeletePageFromDb(entity.Id.Value, accountId);
                        break;
                    default:
                        updatePageIndex(entity.Id.Value, i, accountId);
                        i++;
                        break;
                }
            }
        }

        public Account GetAccountByPageId(long pageId)
        {
            Page page = db.Pages.AsNoTracking().FirstOrDefault(x => x.Id == pageId);

            if (page == null)
            {
                return null;
            }

            Account account = db.Accounts.AsNoTracking().FirstOrDefault(x => x.Id == page.AccountId);
            if (account == null)
            {
                return null;
            }

            if (account.Locked)
            {
                es.ThrowInfoException("Account {0} is locked", account.Login);
            }

            return account;
        }

        public Account GetAccountByColumnId(long columnId)
        {
            long? pageId = getColumnPageId(columnId);

            if (!pageId.HasValue)
            {
                return null;
            }

            return GetAccountByPageId(pageId.Value);
        }

        public Account GetAccountByLinkId(long linkId)
        {
            long? pageId = getLinkPageId(linkId);

            if (!pageId.HasValue)
            {
                return null;
            }

            return GetAccountByPageId(pageId.Value);
        }

        private long? getColumnPageId(long columnId)
        {
            Column column = db.Columns.AsNoTracking()
                .FirstOrDefault(x => x.Id == columnId);

            if (column == null) {
                return null;
            }

            if (column.PageId.HasValue) {
                return column.PageId;
            }

            return getRowPageId(column.RowId.Value);
        }

        private long? getRowPageId(long rowId)
        {

            Row row = db.Rows.AsNoTracking()
                .FirstOrDefault(x => x.Id == rowId);

            if (row == null)
            {
                return null;
            }

            return getColumnPageId(row.ColumnId.Value);
        }

        private long? getLinkPageId(long linkId)
        {

            Link link = db.Links.AsNoTracking()
                .FirstOrDefault(x => x.Id == linkId);

            if (link == null)
            {
                return null;
            }

            return getColumnPageId(link.ColumnId.Value);
        }

        public void SaveColumnToDb(Column column, List<ListEntity> rowIds, List<ListEntity> linkIds, string login)
        {
            if (column == null || !column.Id.HasValue)
            {
                es.ThrowException("Wrong column passed to update");
            }

            if (string.IsNullOrWhiteSpace(login))
            {
                es.ThrowException("No login to update column");
            }

            if (rowIds == null)
            {
                rowIds = new List<ListEntity>();
            }
            if (linkIds == null)
            {
                linkIds = new List<ListEntity>();
            }

            switch (column.ColumnTypeId)
            {
                case ColumnTypes.Links:
                    rowIds.RemoveAll(x => x.Action == RowAction.Inserted);
                    rowIds.ForEach(x => x.Action = RowAction.Deleted);
                    break;
                case ColumnTypes.News:
                    linkIds.RemoveAll(x => x.Action == RowAction.Inserted);
                    linkIds.RemoveAll(x => x.Action == RowAction.Inserted);
                    rowIds.ForEach(x => x.Action = RowAction.Deleted);
                    linkIds.ForEach(x => x.Action = RowAction.Deleted);
                    break;
                case ColumnTypes.Rows:
                    linkIds.RemoveAll(x => x.Action == RowAction.Inserted);
                    linkIds.ForEach(x => x.Action = RowAction.Deleted);
                    break;
            }

            if (!ListEntity.Valid(rowIds))
            {
                es.ThrowException("Rows ids list is not valid");
            }
            if (!ListEntity.Valid(linkIds))
            {
                es.ThrowException("Links ids list is not valid");
            }


            db.Database.BeginTransaction();
            try
            {
                Account account = db.Accounts.FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

                if (account == null)
                {
                    es.ThrowException("No account found for login {0}", login);
                }

                bool columnExists = db.Columns.Any(x => x.Id == column.Id);
                if (!columnExists)
                {
                    es.ThrowException("Column to update not found");
                }

                long? pageId = getColumnPageId(column.Id.Value);

                bool pageExists = db.Pages.Any(x => x.AccountId == account.Id && x.Id == pageId);

                if (!pageExists)
                {
                    es.ThrowException("Page not found for login {0}", login);
                }


                if (!RowsReferencingColumn(rowIds.Where(x => x.Id.HasValue).Select(y => y.Id.Value).ToList(), column.Id.Value))
                {
                    es.ThrowException("Rows ids list does not match to existing column's rows list");
                }

                if (!LinksReferencingColumn(linkIds.Where(x => x.Id.HasValue).Select(y => y.Id.Value).ToList(), column.Id.Value))
                {
                    es.ThrowException("Links ids list does not match to existing column's links list");
                }

                updateColumnToDb(column, rowIds, linkIds);
                db.Database.CommitTransaction();
            }
            catch (Exception)
            {
                db.Database.RollbackTransaction();
                throw;
            }
        }

        public void SaveLinkToDb(Link link, string login)
        {
            if (link == null || !link.Id.HasValue)
            {
                es.ThrowException("Wrong link passed to update");
            }

            if (string.IsNullOrWhiteSpace(login))
            {
                es.ThrowException("No login to update a row");
            }

            db.Database.BeginTransaction();
            try
            {
                Account account = db.Accounts.FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

                if (account == null)
                {
                    es.ThrowException("No account found for login {0}", login);
                }
                if (account.Locked)
                {
                    es.ThrowInfoException("Account {0} is locked", account.Login);
                }

                bool linkExists = db.Links.Any(x => x.Id == link.Id);
                if (!linkExists)
                {
                    es.ThrowException("Link to update not found");
                }

                updateLinkToDb(link);
                db.Database.CommitTransaction();
            }
            catch (Exception)
            {
                db.Database.RollbackTransaction();
                throw;
            }
        }

        public void AddLinkToDb(Link link, string login)
        {
            if (link == null || link.Id.HasValue)
            {
                es.ThrowException("Wrong link passed to add");
            }

            if (string.IsNullOrWhiteSpace(login))
            {
                es.ThrowException("No login to update a row");
            }

            db.Database.BeginTransaction();
            try
            {
                Account account = db.Accounts.FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

                if (account == null)
                {
                    es.ThrowException("No account found for login {0}", login);
                }
                if (account.Locked)
                {
                    es.ThrowInfoException("Account {0} is locked", account.Login);
                }

                db.Links.Add(link);
                db.SaveChanges();
                db.Database.CommitTransaction();
            }
            catch (Exception)
            {
                db.Database.RollbackTransaction();
                throw;
            }
        }


        public void SaveRowToDb(Row row, List<ListEntity> columnIds, string login)
        {
            if (row == null || !row.Id.HasValue)
            {
                es.ThrowException("Wrong row passed to update");
            }

            if (string.IsNullOrWhiteSpace(login))
            {
                es.ThrowException("No login to update a row");
            }

            if (columnIds == null)
            {
                columnIds = new List<ListEntity>();
            }

            if (!ListEntity.Valid(columnIds))
            {
                es.ThrowException("Columns ids list is not valid");
            }

            db.Database.BeginTransaction();
            try
            {
                Account account = db.Accounts.FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

                if (account == null)
                {
                    es.ThrowException("No account found for login {0}", login);
                }
                if (account.Locked)
                {
                    es.ThrowInfoException("Account {0} is locked", account.Login);
                }

                bool rowExists = db.Rows.Any(x => x.Id == row.Id);
                if (!rowExists)
                {
                    es.ThrowException("Row to update not found");
                }

                long? pageId = getRowPageId(row.Id.Value);

                bool pageExists = db.Pages.Any(x => x.AccountId == account.Id && x.Id == pageId);

                if (!pageExists)
                {
                    es.ThrowException("Page not found for login {0}", login);
                }

                if (!ColumnsReferencingRow(columnIds.Where(x => x.Id.HasValue).Select(y => y.Id.Value).ToList(), row.Id.Value))
                {
                    es.ThrowException("Columns ids list does not match to existing row's columns list");
                }

                updateRowToDb(row, columnIds);
                db.Database.CommitTransaction();
            }
            catch (Exception)
            {
                db.Database.RollbackTransaction();
                throw;
            }
        }

        public void SavePageToDb(Page page, List<ListEntity> columnIds, string login)
        {
            if (page == null || !page.Id.HasValue)
            {
                es.ThrowException("Wrong page passed to update");
            }

            if (columnIds == null)
            {
                columnIds = new List<ListEntity>();
            }

            if (string.IsNullOrWhiteSpace(login))
            {
                es.ThrowException("No login to update page");
            }

            if (!ListEntity.Valid(columnIds))
            {
                es.ThrowException("Column ids list is not valid");
            }


            db.Database.BeginTransaction();
            try
            {
                Account account = db.Accounts.FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

                if (account == null)
                {
                    es.ThrowException("No account found for login {0}", login);
                }
                if (account.Locked)
                {
                    es.ThrowInfoException("Account {0} is locked", account.Login);
                }

                bool pageExists = db.Pages.Any(x => x.AccountId == account.Id && x.Id == page.Id);

                if (!pageExists)
                {
                    es.ThrowException("Page not found for login {0}", login);
                }

                if (!ColumnsReferencingPage(columnIds.Where(x => x.Id.HasValue).Select(y => y.Id.Value).ToList(), page.Id.Value))
                {
                    es.ThrowException("Columns ids list does not match to existing page's columns list", login);
                }


                if (page.Categories != null && page.Categories.Count(x => x.Selected) > 3)
                {
                    es.ThrowInfoException("Page cannot belong to more than 3 categories");
                }

                bool pageNameNotUnique = db
                    .Pages
                    .AsNoTracking()
                    .Any(x => x.AccountId == account.Id && x.Id != page.Id && string.Equals(x.Name, page.Name, StringComparison.OrdinalIgnoreCase));

                if (pageNameNotUnique)
                {
                    es.ThrowInfoException("Page with name {0} already exists", page.Name);
                }

                updatePageToDb(page, columnIds, account.Id.Value);
                db.Database.CommitTransaction();
            }
            catch (Exception)
            {
                db.Database.RollbackTransaction();
                throw;
            }
        }

        public void SavePagesListToDb(List<ListEntity> pageIds, string login)
        {
            if (pageIds == null)
            {
                pageIds = new List<ListEntity>();
            }

            if (string.IsNullOrWhiteSpace(login))
            {
                es.ThrowException("No login to update page ids list");
            }

            if (!ListEntity.Valid(pageIds))
            {
                es.ThrowException("Page ids list is not valid");
            }

            db.Database.BeginTransaction();
            try
            {
                Account account = db.Accounts.FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

                if (account == null)
                {
                    es.ThrowInfoException("No account found for login {0}", login);
                }
                if (account.Locked)
                {
                    es.ThrowInfoException("Account {0} is locked", account.Login);
                }

                if (!PagesBelongToLogin(pageIds.Where(x => x.Id.HasValue).Select(y => y.Id.Value).ToList(), account.Id.Value)) 
                {
                    es.ThrowException("Pages igs list does not match existing acount's list", login);
                }

                updatePagesListToDb(pageIds, account.Id.Value);
                db.Database.CommitTransaction();
            }
            catch (Exception)
            {
                db.Database.RollbackTransaction();
                throw;
            }
        }

        public Account GetAccountByLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }

            Account result = db
                .Accounts
                .FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

            return result;
        }

        private long copyLinkToDb(Link link, long columnId)
        {
            link.Id = null;
            link.ColumnId = columnId;

            db.Links.Add(link);
            db.SaveChanges();

            return link.Id.Value;
        }

        private long copyRowToDb(Row row, long columnId)
        {
            long srcRowId = row.Id.Value;

            row.Id = null;
            row.ColumnId = columnId;

            db.Rows.Add(row);
            db.SaveChanges();

            db
                .Columns
                .AsNoTracking()
                .Where(x => x.RowId == srcRowId).ToList()
                .ToList()
                .ForEach(x => copyColumnToDb(x, null, row.Id.Value));

            return row.Id.Value;
        }

        private long copyColumnToDb(Column column, long? pageId, long? rowId)
        {
            long srcColumnId = column.Id.Value;

            column.Id = null;
            column.PageId = pageId;
            column.RowId = rowId;

            db.Columns.Add(column);
            db.SaveChanges();

            db
                .Rows
                .AsNoTracking()
                .Where(x => x.ColumnId == srcColumnId).ToList()
                .ToList()
                .ForEach(x => copyRowToDb(x, column.Id.Value));

            db
                .Links
                .AsNoTracking()
                .Where(x => x.ColumnId == srcColumnId).ToList()
                .ToList()
                .ForEach(x => copyLinkToDb(x, column.Id.Value));


            return column.Id.Value;
        }

        public void CopyPageToDb(long pageId, string login)
        {
            db.Database.BeginTransaction();
            try
            {
                Page page = db.Pages.AsNoTracking().FirstOrDefault(x => x.Id == pageId);

                if (page == null)
                {
                    es.ThrowInfoException("Page does not exist");
                }

                Account account = GetAccountByLogin(login);
                if (account == null)
                {
                    es.ThrowInfoException("Login does not exist");
                }

                long srcPageId = page.Id.Value;

                page.Id = null;
                page.AccountId = account.Id;
                page.Name = GetNewPageName(account.Id.Value, page.Name);

                db.Pages.Add(page);
                db.SaveChanges();

                db
                    .Columns
                    .AsNoTracking()
                    .Where(x => x.PageId == srcPageId).ToList()
                    .ToList()
                    .ForEach(x => copyColumnToDb(x, page.Id.Value, null));

                db.Database.CommitTransaction();
            }
            catch (Exception)
            {
                db.Database.RollbackTransaction();
                throw;
            }
        }

        public IEnumerable<Column> GetAllColumnsByPage(long pageId)
        {
            return db
                .Columns
                .Where(x => x.PageId == pageId)
                .OrderBy(x => x.ColumnIndex);
        }

        public void FillColumn(Column column, string login)
        {
            column.ImageUrl = fs.GetPageElementImageUrl(column.Id.Value, PageElement.Column, login);

            // Column rows
            column.Rows = db
                .Rows
                .Where(x => x.ColumnId.Value == column.Id)
                .OrderBy(x => x.RowIndex)
                .ToList();

            column.Rows.ForEach(x =>
            {
                x.Columns = db
                    .Columns
                    .Where(y => y.RowId == x.Id)
                    .OrderBy(y => y.ColumnIndex)
                    .ToList();

                if (x.Columns == null)
                {
                    return;
                }

                x.Columns.ForEach(y => FillColumn(y, login));
            });

            // Column links
            column.Links = db
                .Links
                .Where(x => x.ColumnId == column.Id)
                .OrderBy(x => x.LinkIndex)
                .ToList();

            column.Links.ForEach(x => 
            {
                x.ImageUrl = fs.GetPageElementImageUrl(x.Id.Value, PageElement.Link, login);
                x.ViewModeId = column.ViewModeId;
            });

        }

        public void DeleteColumn(Column column)
        {
            if (column.Rows != null)
            {
                foreach (Row row in column.Rows)
                {
                    if (row.Columns != null)
                    {
                        foreach (Column col in row.Columns)
                        {
                            DeleteColumn(col);
                        }
                    }

                    //rowsRepository.Delete(row);

                    db.Rows.Remove(row);
                }
                db.SaveChanges();
            }

            if (column.Links != null)
            {
                foreach (Link link in column.Links)
                {
                    //linksRepository.Delete(link);
                    db.Links.Remove(link);
                }
                db.SaveChanges();
            }

            //Delete(column);
            db.Columns.Remove(column);
            db.SaveChanges();
        }

        public void RemovePageFromCache(long pageId, string login)
        {
            Page page = db.Pages.AsNoTracking().Where(x => x.Id == pageId).FirstOrDefault();
            if (page == null)
            {
                return;
            }
            RemovePageFromCache(login + page.Name);
        }

        public void RemovePageFromCache(string key)
        {
            string fullKey = op.Value.Cache.PageKey + key;
            cache.Remove(fullKey);
        }


        public LinkData GetExtensionDataFromCookies()
        {
            string data = httpContext.HttpContext.Request.Cookies["extensionPageColumn"];

            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }

            try
            {
                LinkData result = JsonConvert.DeserializeObject<LinkData>(data);
                return result;
            }
            catch
            {
                return null;
            }
        }

        public void PutExtensionDataToCookies(LinkData linkData)
        {
            if (linkData == null || linkData.PageId <= 0 || linkData.ColumnId <= 0)
            {
                es.ThrowException("Wrong extension data");
            }

            linkData.Href = null;
            linkData.Title = null;

            string json = JsonConvert.SerializeObject(linkData);
            httpContext.HttpContext.Response.Cookies.Append("extensionPageColumn", json,
                new CookieOptions()
                {
                    Path = "/",
                    HttpOnly = false,
                    Secure = false,
                    Expires = DateTimeOffset.MaxValue
                });
        }


    }
}
