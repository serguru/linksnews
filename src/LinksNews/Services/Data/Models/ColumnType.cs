using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("columnType")]
    public class ColumnType
    {
        public long? Id { get; set; }
        public string Name { get; set; }
    }
}
