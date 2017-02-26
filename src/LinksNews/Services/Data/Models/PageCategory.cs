using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("pageCategory")]
    public class PageCategory
    {
        public long? Id { get; set; }
        public long? PageId { get; set; }
        public long? ContentCategoryId { get; set; }

    }
}
