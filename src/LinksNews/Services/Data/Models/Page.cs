using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("page")]
    public class Page
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? Id { get; set; }
        public long? AccountId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool PublicAccess { get; set; }
        public string Description { get; set; }
        public long? PageIndex { get; set; }
        public bool ShowTitle { get; set; }
        public bool ShowDescription { get; set; }
        public bool ShowImage { get; set; }

        [NotMapped]
        public string ImageUrl { get; set; }

        [NotMapped]
        public bool ShowAd { get; set; }

        [NotMapped]
        public string AdContent { get; set; }

        [NotMapped]
        public string Login { get; set; }

        [NotMapped]
        public virtual List<Column> Columns { get; set; }

        [NotMapped]
        public virtual List<ContentCategory> Categories { get; set; }
    }
}
