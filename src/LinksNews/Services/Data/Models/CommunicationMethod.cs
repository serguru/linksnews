using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.Data.Models
{
    [Table("communicationMethod")]
    public class CommunicationMethod
    {
        public long? Id { get; set; }
        public string Name { get; set; }
    }
}
