using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using LinksNews.Services.Data;
using LinksNews.Services.Data.Models;
using Microsoft.Extensions.Options;
using LinksNews.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace LinksNews.Services
{
    public class LogService
    {
        private readonly DataService ds;

        public LogService(DataService ds)
        {
            this.ds = ds;
        }

        public LogMessage Log(ExceptionSeverity severity, string message, params string[] values)
        {
            string mess = string.Format(message, values);
            return ds.SaveLogMessage2Db(severity, mess);
        }

        public LogMessage LogError(string message, params string[] values)
        {
            return Log(ExceptionSeverity.Error, message, values);
        }

        public LogMessage LogWarning(string message, params string[] values)
        {
            return Log(ExceptionSeverity.Warning, message, values);
        }

    }
}
