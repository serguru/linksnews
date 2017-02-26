using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("arow")]
    public class Row
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? Id { get; set; }
        public long? ColumnId { get; set; }
        public long? RowIndex { get; set; }
        public string Title { get; set; }
        public bool ShowTitle { get; set; }

        [NotMapped]
        public bool ShowAd { get; set; }

        [NotMapped]
        public string AdContent { get; set; }

        [NotMapped]
        public virtual List<Column> Columns { get; set; }
    }
}