using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Core
{
    public class ListEntity
    {
        public long? Id { get; set; }
        public RowAction Action { get; set; }
        public List<Object> Props { get; set; }

        public static Boolean Valid(IEnumerable<ListEntity> list) 
        {
            if (list == null)
            {
                return false;
            }

            if (!list.Any())
            {
                return true;
            }

            foreach (ListEntity item in list)
            {
                switch (item.Action)
                {
                    case RowAction.Inserted:
                        if (item.Id.HasValue)
                        {
                            return false;
                        }
                        break;
                    case RowAction.Updated:
                        if (!item.Id.HasValue)
                        {
                            return false;
                        }
                        break;
                    default:
                        break;

                }
            }

            return true;
        }
    }
}
