using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Core
{
    public enum ColumnTypes { Links = 1, News = 2, Rows = 3 };

    public enum RowAction { Unchanged, Inserted, Updated, Deleted};

    public enum PageElement { Page, Column, Row, Link};

    public enum ExceptionSeverity { Info = 1, Warning = 2, Error = 3 }

    public enum MessageGroups { Site = 1, ContactUs = 2, Forum = 3 }

    public enum ViewModes { List = 1, Tile = 2 }

    public enum NewsProviderTypes { Api = 1, Rss = 2 }
}
