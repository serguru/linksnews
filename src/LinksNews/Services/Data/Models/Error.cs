using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.Data.Models
{
    public class Error
    {
        public long? Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTimeOffset DateCreated { get; set; }
    }
}
