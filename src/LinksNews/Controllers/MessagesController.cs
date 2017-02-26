using LinksNews.Core;
using LinksNews.Services;
using LinksNews.Services.Data.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LinksNews.Controllers
{
    [Route("[controller]")]
    public class MessagesController : Controller
    {
        private readonly ExecutionService es;
        private readonly MessagesService messagesService;

        public MessagesController(
            ExecutionService es,
            MessagesService messagesService
            )
        {
            this.es = es;
            this.messagesService = messagesService;
        }

        [HttpPost]
        [Route("sendContactUs")]
        public IActionResult AddContactUsMessage([FromBody] Message message)
        {
            return es.Execute(() =>
            {
                long id = messagesService.AddContactUsMessage(message);
                return new JsonResult(new GenericResult(id));
            }, false);
        }
    }
}