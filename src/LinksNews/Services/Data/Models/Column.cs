using LinksNews.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("acolumn")]
    public class Column
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? Id { get; set; }
        public ColumnTypes ColumnTypeId { get; set; }
        public long? RowId { get; set; }
        public long? PageId { get; set; }
        public long? ColumnIndex { get; set; }
        public long? ColumnWidth { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long? NewsProviderId { get; set; }
        public string NewsProviderSourceId { get; set; }
        public bool ShowTitle { get; set; }
        public bool ShowImage { get; set; }
        public bool ShowDescription { get; set; }
        public bool ShowNewsImages { get; set; }
        public bool ShowNewsDescriptions { get; set; }
        public ViewModes ViewModeId { get; set; }

        [NotMapped]
        public string ImageUrl { get; set; }

        [NotMapped]
        public bool ShowAd { get; set; }

        [NotMapped]
        public string AdContent { get; set; }


        [NotMapped]
        public virtual List<Row> Rows { get; set; }
        [NotMapped]
        public virtual List<Link> Links { get; set; }
    }
}
