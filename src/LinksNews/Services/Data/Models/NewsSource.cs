using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("vwNewsSource")]
    public class NewsSource
    {
        public long? Id { get; set; }

        public long? NewsProviderId { get; set; }
        public string NewsProvider { get; set; }

        public string NewsSourceId { get; set; }
        public string NewsSourceDescription { get; set; }
        public string NewsSourceUrl { get; set; }

        public string NewsSourceSmallLogoUrl { get; set; }
        public string NewsSourceMediumLogoUrl { get; set; }
        public string NewsSourceLargeLogoUrl { get; set; }

        public long? ContentCategoryId { get; set; }
        public string ContentCategory { get; set; }

        public long? CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }

        public long? LanguageId { get; set; }
        public string LanguageCode { get; set; }
        public string LanguageName { get; set; }
    }
}
