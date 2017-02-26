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

namespace LinksNews.Services
{
    public class AdService
    {
        private readonly ExecutionService es;
        private readonly IOptions<LinksOptions> options;

        public AdService(
            ExecutionService es,
            IOptions<LinksOptions> options
            )
        {
            this.es = es;
            this.options = options;
        }

        public void FillPageWithAds(Page page, string login)
        {
            if (!options.Value.ShowAds)
            {
                return;
            }

            Random random = new Random();
            int randomNumber = random.Next(1, 4);

            if (randomNumber == 1)
            {
                page.ShowAd = true;
                page.AdContent = string.Format("<div class='links-ad'><a href='http://ibm.com'><h5>Go IBM {0}</h5></a></div>", login);
            }

            if (page.Columns != null)
            {
                foreach (Column column in page.Columns)
                {
                    FillColumnWithAds(column, login);
                }
            }
        }

        public void FillRowWithAds(Row row, string login)
        {
            if (!options.Value.ShowAds)
            {
                return;
            }
            Random random = new Random();
            int randomNumber = random.Next(1, 3);

            if (randomNumber == 1)
            {
                row.ShowAd = true;
                row.AdContent = string.Format("<div class='links-ad'><a href='http://rbc.com'><h5>Go RBC News {0}</h5></a></div>", login);
            }

            if (row.Columns != null)
            {
                foreach (Column column in row.Columns)
                {
                    FillColumnWithAds(column, login);
                }
            }
        }

        public void FillColumnWithAds(Column column, string login)
        {
            if (!options.Value.ShowAds)
            {
                return;
            }

            Random random = new Random();
            int randomNumber = random.Next(1, 3);

            if (randomNumber == 1)
            {
                column.ShowAd = true;
                column.AdContent = string.Format("<div class='links-ad'><a href='http://www.bbc.com/news'><h6>Go BBC News {0}</h65></a></div>", login);
            }

            if (column.Rows != null)
            {
                foreach (Row row in column.Rows)
                {
                    FillRowWithAds(row, login);
                }
            }

            if (column.Links != null)
            {
                foreach (Link link in column.Links)
                {
                    FillLinkWithAds(link, login);
                }
            }


        }

        public void FillLinkWithAds(Link link, string login)
        {
            if (!options.Value.ShowAds)
            {
                return;
            }
            Random random = new Random();
            int randomNumber = random.Next(1, 4);

            if (randomNumber == 1)
            {
                link.ShowAd = true;
                link.AdContent = string.Format("<div class='links-ad'><a href='http://google.com'><h6>Go Google {0}</h6></a></div>", login);
            }
        }
    }
}
