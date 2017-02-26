using LinksNews.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.Data.Models
{
    [Table("message")]
    public class Message
    {
        public long? Id { get; set; }
        public MessageGroups? MessageGroupId { get; set; }
        public long? ParentMessageId { get; set; }
        public DateTimeOffset? SentDate { get; set; }
        public string SentFromIP { get; set; }
        public long? PageId { get; set; }
        public long? AuthorAccountId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public string Subject { get; set; }
        public string MessageText { get; set; }
    }
}


