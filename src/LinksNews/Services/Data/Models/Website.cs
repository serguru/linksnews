using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("website")]
    public class Website
    {
        public long? Id { get; set; }
        public string Name { get; set; }
    }
}
