using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Core
{
    public class LinksException: Exception
    {
        private readonly ExceptionSeverity severity;
        public ExceptionSeverity Severity
        {
            get { return severity; }
        }

        private readonly int? code;
        public int? Code
        {
            get { return code; }
        }

        public LinksException(string message, ExceptionSeverity severity) : base(message)
        {
            this.severity = severity;
        }

        public LinksException(int code, string message, ExceptionSeverity severity) : base(message)
        {
            this.severity = severity;
            this.code = code;
        }
    }
}


