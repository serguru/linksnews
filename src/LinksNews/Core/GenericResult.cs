using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Core
{
    public class GenericResult
    {
        public bool Error { get; set; }
        public int? Code { get; set; }
        public string Message { get; set; }
        public Object Data { get; set; }

        public GenericResult()
        {

        }

        public GenericResult(bool error, string message, int? code = null)
        {
            Error = error;
            Message = message;
            Code = code;
        }

        public GenericResult(Object data)
        {
            Data = data;
        }
    }
}
