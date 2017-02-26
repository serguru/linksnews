using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.Data.Models
{
    [Table("messageRecipient")]
    public class MessageRecipient
    {
        public long? Id { get; set; }
        public long? MessageId { get; set; }
        public long? RecipientAccountId { get; set; }
        public string RecipientAddress { get; set; }
        public DateTimeOffset? ReceiveDate { get; set; }
        public long? CommunicationMethodId { get; set; }
    }
}
