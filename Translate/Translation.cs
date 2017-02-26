//Scaffold-DbContext "Data Source = PC\\SQLEXPRESS;Initial Catalog = Links; Persist Security Info=True;User ID = sa; Password=sql" Microsoft.EntityFrameworkCore.SqlServer
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace translate
{

    [Table("englishMessage")]
    public class EnglishMessage
    {
        public long? Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Translate> Translates { get; set; }
    }

    [Table("language")]
    public class Language
    {
        public long? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool SupportedByInterface { get; set; }
        public bool SupportedByNews { get; set; }
    }

    [Table("translate")]
    public class Translate
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public long? EnglishMessageId { get; set; }
        public long? LanguageId { get; set; }
        public bool Reference { get; set; }
        public EnglishMessage Message { get; set; }
        public Language Language { get; set; }

        public virtual ICollection<TranslateVersion> TranslateVersions { get; set; }
    }

    [Table("translateVersion")]
    public class TranslateVersion
    {
        public long? Id { get; set; }
        public long? TranslateId { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public bool Reference { get; set; }
        public Translate Translate { get; set; }
    }


}
