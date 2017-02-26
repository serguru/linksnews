using LinksNews.Core;
using LinksNews.Services;
using LinksNews.Services.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LinksNews.Controllers
{
    [Route("[controller]")]
    public class TranslateController : Controller
    {
        private readonly DataService dataService;
        private readonly ExecutionService es;

        public TranslateController(ExecutionService es, DataService dataService)
        {
            this.dataService = dataService;
            this.es = es;
        }

        [HttpPost]
        [Route("getInterfaceLanguages")]
        public IActionResult GetInterfaceLanguages()
        {
            return es.Execute(() =>
            {
                List<Language> result = dataService.GetInterfaceLanguages();
                return new JsonResult(new GenericResult(result));
            }, false);
        }

        [HttpPost]
        [Route("setLanguage")]
        public IActionResult SetLanguage([FromBody]string language)
        {
            return es.Execute(() =>
            {
                language = dataService.GetInterfaceLanguageCode(language);

                Response.Cookies.Delete("language");
                Response.Cookies.Append(
                    "language",
                    language,
                    new CookieOptions()
                    {
                        Path = "/",
                        HttpOnly = false,
                        Secure = false,
                        Expires = DateTimeOffset.MaxValue
                    }
                );

                return new JsonResult(new GenericResult(language));
            }, false);
        }
    }
}