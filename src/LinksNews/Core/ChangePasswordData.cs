using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Core
{
    public class ChangePasswordData
    {
        public string Login { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        public bool Valid()
        {
            return
                !string.IsNullOrWhiteSpace(Login) &&
                !string.IsNullOrWhiteSpace(OldPassword) &&
                !string.IsNullOrWhiteSpace(NewPassword);
        }
    }
}
