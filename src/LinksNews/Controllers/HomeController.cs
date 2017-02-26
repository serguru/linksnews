using LinksNews.Core;
using LinksNews.Services;
using LinksNews.Services.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LinksNews.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly DataService ds;
        private readonly ExecutionService es;
        private readonly UtilsService us;
        private readonly IHttpContextAccessor httpContext;

        public HomeController(
           DataService ds,
           ExecutionService es,
           UtilsService us,
           IHttpContextAccessor httpContext
            )
        {
            this.ds = ds;
            this.es = es;
            this.us = us;
            this.httpContext = httpContext;
        }

        [HttpPost("registerVisit")]
        public IActionResult RegisterVisit([FromBody] Visit visit)
        {
            return es.Execute(
                () =>
                {
                    if (visit == null || us.Empty(visit.Route))
                    {
                        es.ThrowException("Wrong visit data");
                    }
                    visit.RequestIp = httpContext.HttpContext.Connection.RemoteIpAddress.ToString();
                    visit.RequestTime = DateTimeOffset.UtcNow;
                    ds.RegisterVisit(visit);

                    return new JsonResult(new GenericResult());
                },
                false
            );
        }

        [HttpPost]
        [Route("setCookie")]
        public IActionResult SetCookie([FromBody]JObject data)
        {
            return es.Execute(() =>
            {
                if (data == null)
                {
                    return new JsonResult(new GenericResult());
                }
                object name = data["name"];
                object value = data["value"];

                if (us.Empty(name) || us.Empty(value))
                {
                    return new JsonResult(new GenericResult());
                }

                Response.Cookies.Delete(name.ToString());
                Response.Cookies.Append(
                    name.ToString(),
                    value.ToString(),
                    new CookieOptions()
                    {
                        Path = "/",
                        HttpOnly = false,
                        Secure = false,
                        Expires = DateTimeOffset.MaxValue
                    }
                );

                return new JsonResult(new GenericResult());
            }, false);
        }
    }
}