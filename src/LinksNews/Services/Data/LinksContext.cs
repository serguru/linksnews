using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LinksNews.Services.Data.Models;
using LinksNews.Core;

namespace LinksNews.Services.Data
{
    public class LinksContext: DbContext
    {
        public virtual DbSet<LogMessage> Logs { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Column> Columns { get; set; }
        public virtual DbSet<ColumnType> ColumnTypes { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Link> Links { get; set; }
        public virtual DbSet<EnglishMessage> EnglishMessages { get; set; }
        public virtual DbSet<NewsProviderDef> NewsProviderDefs { get; set; }
        public virtual DbSet<ContentCategory> ContentCategories { get; set; }
        public virtual DbSet<ContentCategoryMap> ContentCategoryMaps { get; set; }
        public virtual DbSet<NewsSourcePriority> SourcePriorities { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Row> Rows { get; set; }
        public virtual DbSet<Theme> Themes { get; set; }
        public virtual DbSet<Translate> Translates { get; set; }
        public virtual DbSet<TranslateVersion> TranslateVersions { get; set; }
        public virtual DbSet<Visit> Visits { get; set; }
        public virtual DbSet<Website> Websites { get; set; }
        public virtual DbSet<PageCategory> PageCategories { get; set; }
        public virtual DbSet<AccountPage> AccountPages { get; set; }
        public virtual DbSet<CategoryCount> CategoryCounts { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<MessageGroup> MessageGroups { get; set; }
        public virtual DbSet<CommunicationMethod> CommunicationMethods { get; set; }
        public virtual DbSet<MessageRecipient> MessageRecipients { get; set; }
        public virtual DbSet<ViewMode> ViewModes { get; set; }
        public virtual DbSet<NewsProviderType> NewsProviderTypes { get; set; }
        public virtual DbSet<NewsSource> NewsSources { get; set; }

        public LinksContext(DbContextOptions<LinksContext> options): base(options)
        {
//            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
    }
}
/*
Scaffold-DbContext "'Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;'" Microsoft.EntityFrameworkCore.SqlServer -Verbose
Scaffold-DbContext "'Data Source=PC\SQLEXPRESS;Initial Catalog=Links;Persist Security Info=True;User ID=sa;Password=sql'" Microsoft.EntityFrameworkCore.SqlServer -Verbose
*/
