using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("newsSourcePriority")]
    public class NewsSourcePriority
    {
        public long? Id { get; set; }
        public long? NewsProviderId { get; set; }
        public long? ContentCategoryId { get; set; }
        public long? Priority { get; set; }
    }
}
