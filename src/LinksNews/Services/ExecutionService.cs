using LinksNews.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services
{
    public class ExecutionService
    {
        private readonly IHttpContextAccessor httpContext;

        public ExecutionService(
            IHttpContextAccessor httpContext
            )
        {
            this.httpContext = httpContext;
        }

        public AccountService accountService { get; set; }

        public LogService log { get; set; }

        public void ThrowException(string message, params string[] values)
        {
            string mess = string.Format(message, values);

            LinksException ex = new LinksException(mess, ExceptionSeverity.Warning);

            // TODO: put a warning and up severity message into the log, send an email if error

            log.Log(ExceptionSeverity.Warning, mess);
            throw ex;
        }

        public void ThrowInfoException(string message, params string[] values)
        {
            string mess = values != null ? string.Format(message, values) : message;
            LinksException ex = new LinksException(mess, ExceptionSeverity.Info);
            throw ex;
        }

        public void ThrowNotLoggedInException()
        {
            LinksException ex = new LinksException(401, null, ExceptionSeverity.Info);
            throw ex;
        }

        public void ThrowException(string message, ExceptionSeverity severity = ExceptionSeverity.Warning, params string[] values)
        {
            string mess = string.Format(message, values);
            LinksException ex = new LinksException(mess, severity);
            // TODO: send an email if error
            throw ex;
        }

        public JsonResult Execute(Func<JsonResult> function, bool mustBeLoggedIn)
        {
            try
            {
                if (mustBeLoggedIn && accountService.Login == null)
                {
                    ThrowNotLoggedInException();
                }
                return function();
            }
            catch (LinksException e)
            {
                log.Log(e.Severity, e.Message + (e.InnerException == null ? "" : 
                    ", Inner Exception: " + e.InnerException.Message));

                if (e.Severity == ExceptionSeverity.Info)
                {
                    string mess = e.Code.HasValue ? string.Empty : e.Message;
                    return new JsonResult(new GenericResult(true, mess, e.Code));
                }
                throw;
            }
            catch (Exception e)
            {
                string message = e.InnerException == null ? e.Message : e.InnerException.Message;
                log.Log(ExceptionSeverity.Error, message);
                throw;
            }
        }

    }
}
