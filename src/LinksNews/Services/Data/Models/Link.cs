using LinksNews.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("link")]
    public class Link
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? Id { get; set; }
        public long? ColumnId { get; set; }
        public long? LinkIndex { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Hint { get; set; }
        public string Href { get; set; }
        public bool ButtonAccess { get; set; }
        public string ButtonTitle { get; set; }
        public long? ButtonIndex { get; set; }
        public bool ShowImage { get; set; }
        public bool ShowDescription { get; set; }
        public bool NewsLink { get; set; }
        public ViewModes? ViewModeId { get; set; }

        [NotMapped]
        public string ButtonImageUrl { get; set; }

        [NotMapped]
        public string ImageUrl { get; set; }

        [NotMapped]
        public bool ShowAd { get; set; }

        [NotMapped]
        public string AdContent { get; set; }
    }
}
