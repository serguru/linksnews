using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinksNews.Services.Data.Models
{
    [Table("account")]
    public class Account
    {
        public long? Id { get; set; }
        public long RoleId { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? LanguageId { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Website { get; set; }
        public string Comment { get; set; }
        public long? ThemeId { get; set; }
        public bool Locked { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public long? NewsRefreshInterval { get; set; }

        [NotMapped]
        public string Password { get; set; }
        [NotMapped]
        public string ImageUrl { get; set; }

        public string ToName
        {
            get
            {
                string result = null;

                if (!string.IsNullOrEmpty(FirstName))
                {
                    result = FirstName.Trim();
                    if (!string.IsNullOrEmpty(LastName))
                    {
                        result += " " + LastName.Trim();
                    }
                }

                return result ?? Login;
            }
        }

    }
}
