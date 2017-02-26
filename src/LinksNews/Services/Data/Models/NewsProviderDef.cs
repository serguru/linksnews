using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.Data.Models
{
    [Table("newsProviderDef")]
    public class NewsProviderDef
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public bool Locked { get; set; }
        public int? NewsProviderTypeId { get; set; }
    }
}
