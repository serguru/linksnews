using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using LinksNews.Services.Data;
using LinksNews.Services.Data.Models;
using Microsoft.Extensions.Options;
using LinksNews.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using MimeKit;
using MailKit.Net.Smtp;
/*
Mail Client Configuration

Zoho Mail can be accessed via POP/ IMAP from the standard email clients.The administrator can enable or disable the access via POP/ IMAP for the user accounts based on the organizations requirements. The POP/ IMAP configuaration details are given below:

IMAP
Incoming Server Name : imappro.zoho.com
Port : 993
Require SSL : Yes
Username : you @yourdomain.com
POP
Incoming Server Name : poppro.zoho.com
Port : 995
Require SSL : Yes
Username : you @yourdomain.com
Outgoing / SMTP
Outgoing Server Name: smtp.zoho.com
Port : 465 with SSL or 587 with TLS
Require Authentication: Yes
Configuration instructions for some popular email clients:
IMAP: Outlook | Thunderbird | Apple Mail | General

POP : Outlook | Thunderbird | Apple Mail | General

Once the new emails have started coming to your webmail, you can configure the email clients you use, to start receiving emails in them.

*/

//https://azure.microsoft.com/en-us/documentation/articles/sendgrid-dotnet-how-to-send-email/

namespace LinksNews.Services
{
    public class MailService
    {
        private readonly IOptions<LinksOptions> op;
        private readonly IHostingEnvironment environment;
        private readonly ExecutionService es;
        private readonly UtilsService us;


        public MailService(
            IOptions<LinksOptions> op, 
            IHostingEnvironment environment,
            UtilsService us,
            ExecutionService es
            )
        {
            this.op = op;
            this.environment = environment;
            this.us = us;
            this.es = es;
        }


        private void send(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect("smtp.zoho.com", 465, true);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(op.Value.ZohoAdminAccount, op.Value.ZohoAdminPassword);

                client.Send(message);
                client.Disconnect(true);
            }
        }

        public void NotifyAdminAccountRegistered(Account account)
        {
            if (account == null)
            {
                return;
            }

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Links And News", op.Value.Emails.Support));
            message.To.Add(new MailboxAddress("Links And News Admin", op.Value.Emails.Admin));
            message.Subject = "A new account registered";

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = string.Format(
                @"
                    <p>Hello Admin,</p>
                    <p>A new account with Login <q>{0}</q> has been registered.</p>
                    <br />
                    <p>Sincerely,</p>
                    <p>Links And News</p>
                ", account.Login);

            message.Body = bodyBuilder.ToMessageBody();
            send(message);
        }

        public void NotifySupportContactUs(long messageId, Message receivedMessage)
        {
            if (us.Empty(receivedMessage))
            {
                return;
            }

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Links And News", op.Value.Emails.Support));
            message.To.Add(new MailboxAddress("Links And News Support", op.Value.Emails.Support));
            message.Subject = "A contact us message received";

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = string.Format(
                @"
                    <p>Hello Support,</p>
                    <p>A new contact us message with Id = {0} has been received:</p>
                    <p><q style='font-style: italic'>{1}</q></p>
                    <p>Please respond.</p>
                    <br />
                    <p>Sincerely,</p>
                    <p>Links And News</p>
                ", messageId, receivedMessage.MessageText);

            message.Body = bodyBuilder.ToMessageBody();

            send(message);
        }



        public void SendRegisterConfirmation(Account account)
        {
            if (account == null || account.Locked || us.Empty(account.Email))
            {
                return;
            }

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Links And News", op.Value.Emails.Support));
            message.To.Add(new MailboxAddress(account.ToName, account.Email));
            message.Subject = "Links$News register confirmation";

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = string.Format(
                @"
                    <p>Hello {0},</p>
                    <p>Welcome to Links And News!</p>
                    <p>You nave successfully registered with ""{1}"" login name. Thank you for joining us.</p>
                    <p>Please <a href=""{2}/help"">learn</a> how to use our services.</p>
                    <p>If you have any questions please do not hesitate to <a href=""{2}/contactUs"">contact us</a></p>
                    <br />
                    <p>Sincerely,</p>
                    <p>Links And News</p>
                ", account.ToName, account.Login, op.Value.SiteAddress);

            message.Body = bodyBuilder.ToMessageBody();

            send(message);
        }

        public void SendContactUsConfirmation(Message message2send)
        {
            if (message2send == null || message2send.MessageGroupId != MessageGroups.ContactUs)
            {
                es.ThrowException("No message to confirm contact us");
            }

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Links And News", op.Value.Emails.Support));
            message.To.Add(new MailboxAddress(message2send.AuthorName, message2send.AuthorEmail));
            message.Subject = message2send.Subject;


            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = string.Format(
                @"
                    <p>Hello {0},</p>
                    <p>Thank you for contacting us!</p>
                    <p>Your message</p>
                    <p><q style='font-style: italic'>{1}</q></p>
                    <p>has been received.</p>
                    <p>We will respond as soon as possible.</p>
                    <br />
                    <p>Sincerely,</p>
                    <p>Links And News</p>
                ", message2send.AuthorName, message2send.MessageText);

            message.Body = bodyBuilder.ToMessageBody();

            send(message);
        }
    }
}
