using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Core
{
    [Table("log")]
    public class LogMessage
    {
        public long? Id { get; set; }
        public ExceptionSeverity ExceptionSeverityId { get; set; }
        public DateTimeOffset? LogDate { get; set; }
        public string Message { get; set; }

        public LogMessage()
        {
            LogDate = DateTimeOffset.UtcNow;
        }


    }
}
