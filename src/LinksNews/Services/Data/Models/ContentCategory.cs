using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("contentCategory")]
    public class ContentCategory
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [NotMapped]
        public long? PagesCount { get; set; }

        [NotMapped]
        public long? AuthorsCount { get; set; }

        [NotMapped]
        public long? NewsSourcesCount { get; set; }

        [NotMapped]
        public Boolean Selected { get; set; }
    }
}
