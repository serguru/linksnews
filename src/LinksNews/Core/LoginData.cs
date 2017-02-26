using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Core
{
    public class LoginData
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public bool Valid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);
            }
        }
    }
}
