using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.Data.Models
{
    [Table("contentCategoryMap")]
    public class ContentCategoryMap
    {
        public long? Id { get; set; }
        public long? ContentCategoryId { get; set; }
        public string Name { get; set; }
    }
}
