using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("visit")]
    public class Visit
    {
        public long? Id { get; set; }
        public long? AccountId { get; set; }
        public string RequestIp { get; set; }
        public string Route { get; set; }
        public DateTimeOffset RequestTime { get; set; }
    }
}
