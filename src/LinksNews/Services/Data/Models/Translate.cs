using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("translate")]
    public class Translate
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public long? MessageId { get; set; }
        public long? LanguageId { get; set; }
        public bool Reference { get; set; }
    }
}
