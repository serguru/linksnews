using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("accountPage")]
    public class AccountPage
    {
        public long? Id { get; set; }
        public long? PageId { get; set; }
        public long? AccountId { get; set; }
        public long? PageIndex { get; set; }

    }
}
