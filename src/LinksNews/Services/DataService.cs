using LinksNews.Services.Data;
using LinksNews.Services.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinksNews.Core;
using System.Data.SqlClient;

namespace LinksNews.Services
{
    public class DataService
    {
        const string defaultLanguageCode = "en";

        LinksContext db;
        UtilsService us;
        ExecutionService es;

        public DataService(
            LinksContext db,
            UtilsService us,
            ExecutionService es
            )
        {
            this.db = db;
            this.us = us;
            this.es = es;
        }

        public LogService log { get; set; }

        public List<NewsProviderDef> GetNewsProviderDefs()
        {
            List<NewsProviderDef> result = db.NewsProviderDefs.AsNoTracking().ToList();
            return result;
        }

        public ContentCategory GetContentCategoryByName(string categoryName)
        {
            ContentCategory result = db
                .ContentCategories
                .AsNoTracking()
                .FromSql("spGetContentCategory {0}", categoryName)
                .FirstOrDefault();

            if (result == null)
            {
                log.LogWarning("Requested content category {0} was not found in db", categoryName);

                result = db.ContentCategories.AsNoTracking().FromSql("spGetContentCategory", "General").FirstOrDefault();
                if (result == null)
                {
                    es.ThrowException("Requested content category {0} was not found in db", ExceptionSeverity.Error, "General");
                }                
            }
            return result;
        }

        public void RegisterVisit(Visit visit)
        {
            db.Visits.Add(visit);
            db.SaveChanges();
        }

        public LogMessage SaveLogMessage2Db(ExceptionSeverity severity, string message)
        {
            LogMessage log = new LogMessage();

            log.ExceptionSeverityId = severity;
            log.Message = message;

            db.Logs.Add(log);
            db.SaveChanges();

            return log;
        }

        public Language GetLanguageByCode(string code)
        {
            Language result = db.Languages.AsNoTracking().FirstOrDefault(x => us.StrsEqual(code, x.Code));
            return result;
        }

        public Country GetCountryByCode(string code)
        {
            Country result = db.Countries.AsNoTracking().FirstOrDefault(x => us.StrsEqual(code, x.Code));
            return result;
        }

        public List<Language> GetInterfaceLanguages()
        {
            return db.Languages.Where(x => x.SupportedByInterface).OrderBy(x => x.Name).ToList();
        }

        public string GetInterfaceLanguageCode(string languageCode)
        {
            return GetInterfaceLanguages()
                .Any(x => string.Equals(x.Code, languageCode, StringComparison.OrdinalIgnoreCase)) ?
                languageCode : defaultLanguageCode;
        }
    }
}
