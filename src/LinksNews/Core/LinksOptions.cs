using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Core
{
    public class LinksConnection
    {
        public string ConnectionString { get; set; }
    }

    public class Data
    {
        public LinksConnection LinksConnection { get; set; }
        public string DefaultRoleName { get; set; }
    }

    public class Emails
    {
        public string Support { get; set; }
        public string SiteNews { get; set; }
        public string Admin { get; set; }
        public bool SendContactUsEmailConfirmaion { get; set; }
        public bool SendRegisterEmailConfirmaion { get; set; }
    }

    public class Images
    {
        // Mb
        public int InputSizeLimit { get; set; }
        // kb
        public int ConvertSizeLimit { get; set; }
        // px
        public int ConvertWidth { get; set; }
    }


    public class Cache
    {
        public string NewsSourcesKey { get; set; }
        public string NewsSourceKey { get; set; }
        public string PagesKey { get; set; }
        public string PageKey { get; set; }
        public int PageRefreshInterval { get; set; } // in minutes
        public int NewsArticlesRefreshInterval { get; set; } // in minutes
        public int NewsSourcesRefreshInterval { get; set; } // in minutes
    }

    public class Notifications
    {
        public bool NotifyAdminAccountRegistered { get; set; }
        public bool NotifySupportContactUsReceived { get; set; }
    }

    public class LinksOptions
    {
        public string SiteAddress { get; set; }
        public Data Data { get; set; }
        public string AccountImagesPath { get; set; }
        public bool ShowAds { get; set; }
        public string EncryptKey { get; set; }
        public int ContactUsMessagesInterval { get; set; }
        public Emails Emails { get; set; }
        public Images Images { get; set; }
        public Notifications Notifications { get; set; }

        public string ZohoAdminAccount { get; set; }
        public string ZohoAdminPassword { get; set; }
        public Cache Cache { get; set; }
    }
}
