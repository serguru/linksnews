using LinksNews.Services.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using LinksNews.Core;
//using Microsoft.Framework.ConfigurationModel;
using Microsoft.Extensions.Options;
using LinksNews.Services.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace LinksNews.Services
{
    public class MessagesService
    {
        private readonly IOptions<LinksOptions> op;
        private readonly LinksContext db;
        private readonly ExecutionService es;
        private readonly IHttpContextAccessor httpContext;
        private readonly AccountService accountService;
        private readonly UtilsService us;
        private readonly MailService mas;

        public MessagesService(
            IOptions<LinksOptions> op,
            LinksContext db,
            ExecutionService es,
            IHttpContextAccessor httpContext,
            AccountService accountService,
            UtilsService us,
            MailService mas
            )
        {
            this.op = op;
            this.db = db;
            this.es = es;
            this.httpContext = httpContext;
            this.accountService = accountService;
            this.us = us;
            this.mas = mas;
        }

        public DateTimeOffset? GetLatestMessageDateByIp(string sentFromIp, MessageGroups messageGroup)
        {
            DateTimeOffset? result = db
                .Messages
                .AsNoTracking()
                .Where(x => x.SentFromIP == sentFromIp && x.MessageGroupId == messageGroup)
                .Max(x => x.SentDate);

            return result;

        }

        public ValidationData ValidateMessage(Message message)
        {
            ValidationData result = new ValidationData()
            {
                Valid = false
            };

            if (message == null)
            {
                result.Message = "No message";
                return result;
            }

            if (string.IsNullOrWhiteSpace(message.MessageText))
            {
                result.Message = "Message body is empty";
                return result;
            }

            if (!message.SentDate.HasValue)
            {
                result.Message = "No message sent date";
                return result;
            }

            if (string.IsNullOrWhiteSpace(message.SentFromIP))
            {
                result.Message = "Message IP is empty";
                return result;
            }

            if (message.AuthorAccountId.HasValue)
            {
                string currentLogin = accountService.Login;

                if (us.Empty(currentLogin))
                {
                    result.Message = "Message account is not logged in";
                    return result;
                }

                string messageLogin = accountService.GetLoginById(message.AuthorAccountId.Value);

                if (!us.StrsEqual(currentLogin, messageLogin))
                {
                    result.Message = "Message account is not logged in";
                    return result;
                }
            }

            result.Valid = true;
            return result;
        }

        public ValidationData ValidateContactUsMessage(Message message)
        {
            ValidationData result = ValidateMessage(message);

            if (!result.Valid)
            {
                return result;
            }

            DateTimeOffset? dt = GetLatestMessageDateByIp(message.SentFromIP, MessageGroups.ContactUs);

            if (!dt.HasValue)
            {
                return result;
            }

            int limit = op.Value.ContactUsMessagesInterval;
            TimeSpan ts = message.SentDate.Value - dt.Value;
            double interval = ts.TotalMinutes;

            if (interval < limit)
            {
                result.Message = "Messages frequency minimum interval has not been reached. Please try again in {0} minute" +
                    (limit == 1 ? "." : "s.");
                result.Params = new string[] { limit.ToString() };
                result.Valid = false;
                return result;
            }

            return result;
        }

        private void addMessage2Db(Message message)
        {
            db.Messages.Add(message);
            db.SaveChanges();
        }

        public long AddContactUsMessage(Message message)
        {
            message.MessageGroupId = MessageGroups.ContactUs;
            message.SentDate = DateTimeOffset.UtcNow;
            message.SentFromIP = httpContext.HttpContext.Connection.RemoteIpAddress.ToString();

            ValidationData vd = ValidateContactUsMessage(message);
            if (!vd.Valid)
            {
                es.ThrowInfoException(vd.Message, vd.Params);
            }

            addMessage2Db(message);

            if (op.Value.Emails.SendContactUsEmailConfirmaion)
            {
                mas.SendContactUsConfirmation(message);
            }

            if (op.Value.Notifications.NotifySupportContactUsReceived)
            {
                mas.NotifySupportContactUs(message.Id.Value, message);
            }

            return message.Id.Value;
        }
    }
}
