using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("theme")]
    public class Theme
    {
        public long? Id { get; set; }
        public string Name { get; set; }
    }
}
