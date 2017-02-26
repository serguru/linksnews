using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services
{
    public class UtilsService
    {
        public bool IsDigitsOnly(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }

            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if both strings are not null/empty and equal ignore case
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>

        public bool StrsEqual(string str1, string str2)
        {
            bool result =
                !string.IsNullOrWhiteSpace(str1) &&
                !string.IsNullOrWhiteSpace(str2) &&
                string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);

                return result;
        }

        public bool Empty(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is string && string.IsNullOrWhiteSpace(value as string))
            {
                return true;
            }

            return false;
        }

        public string ProcessImageUrl(string imageUrl)
        {
            if (!string.IsNullOrWhiteSpace(imageUrl) &&
                imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                )
            {
                return "/fake_image?url=" + imageUrl;
            }

            return imageUrl;
        }

    }
}
