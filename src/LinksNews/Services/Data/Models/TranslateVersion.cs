using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("translateVersion")]
    public class TranslateVersion
    {
        public long? Id { get; set; }
        public long? TranslateId { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public bool Reference { get; set; }
        public Translate Translate { get; set; }
    }
}
