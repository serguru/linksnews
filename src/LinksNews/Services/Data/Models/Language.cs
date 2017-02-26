using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("language")]
    public class Language
    {
        public long? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool SupportedByInterface { get; set; }
        public bool SupportedByNews { get; set; }
    }
}
