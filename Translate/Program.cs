using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace translate
{
    class Program
    {
        static readonly string[] firstParams = { "db2files", "files2db" };

        private static void loadFiles2Db(TranslateContext context, List<string> files)
        {
            context.TranslateVersions.RemoveRange(context.TranslateVersions);
            context.Translates.RemoveRange(context.Translates);
            context.EnglishMessages.RemoveRange(context.EnglishMessages);

            context.SaveChanges();

            JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            foreach (string file in files)
            {
                Language language = context.Languages.First(x => x.Code.Equals(Path.GetFileNameWithoutExtension(file),
                    StringComparison.InvariantCultureIgnoreCase));

                string json = File.ReadAllText(file);
                List<EnglishMessage> messages = JsonConvert.DeserializeObject<List<EnglishMessage>>(json, settings);
                foreach (EnglishMessage message in messages)
                {
                    if (message.Translates != null)
                    {
                        foreach (Translate t in message.Translates)
                        {
                            t.Language = language;
                            t.Message = message;
                        }
                    }

                    EnglishMessage dbmess = context.EnglishMessages.FirstOrDefault(x => x.Name.Equals(message.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (dbmess == null)
                    {
                        context.EnglishMessages.Add(message);
                        continue;
                    }

                    Translate translate;

                    if (message.Translates == null || !message.Translates.Any())
                    {
                        translate = dbmess.Translates.FirstOrDefault(x => x.LanguageId == language.Id);
                        if (translate != null)
                        {
                            dbmess.Translates.Remove(translate);
                        }
                        continue;
                    }

                    translate = message.Translates.First();

                    if (dbmess.Translates == null)
                    {
                        dbmess.Translates = new List<Translate>();
                    }

                    Translate t2r = dbmess.Translates.FirstOrDefault(x =>
                        x.Name.Equals(translate.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        x.LanguageId == language.Id);

                    if (t2r != null)
                    {
                        dbmess.Translates.Remove(t2r);
                    }

                    translate.Message = dbmess;
                    dbmess.Translates.Add(translate);
                }
                context.SaveChanges();
            }
        }

        private static void files2db(string[] args, Appsettings settings)
        {
            if (!args.Any())
            {
                Console.WriteLine("Usage: translate db2files or translate 'files2db' 'all or en ru fr...'");
                return;
            }

            string path = settings.Files2DbPath;
            Regex regex = new Regex("^[A-za-z]{2}$");

            string[] files = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly)
                .Where(file => regex.IsMatch(Path.GetFileNameWithoutExtension(file))).ToArray();

            if (!files.Any())
            {
                Console.WriteLine("No valid *.json files found. Files must match xx.json pattern where x is a letter.");
                return;
            }

            if (args[0] == "all")
            {
                args = files.Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
            }

            string missing;

            TranslateContext context = new TranslateContext(settings.ConnectionString);

            var l = context.Languages;

            missing = args.FirstOrDefault(x => !context.Languages.Any(
                y => y.Code.Equals(x, StringComparison.InvariantCultureIgnoreCase)));

            if (missing != null)
            {
                Console.WriteLine("Languge {0} was not found in db", missing);
                return;
            }

            missing = args.FirstOrDefault(x => !files.Any(y =>
                Path.GetFileNameWithoutExtension(y).Equals(x, StringComparison.InvariantCultureIgnoreCase)));

            if (missing != null)
            {
                Console.WriteLine("File {0}.json not found ", missing);
                return;
            }

            List<string> files2db = new List<string>();
            files2db.AddRange(files.Where(x => args.Any(
                y => Path.GetFileNameWithoutExtension(x).Equals(y, StringComparison.InvariantCultureIgnoreCase))));

            context.Database.BeginTransaction();

            try
            {
                loadFiles2Db(context, files2db);
                context.Database.CommitTransaction();
                Console.WriteLine("{0} file(s) processed", files2db.Count);
            }
            catch (Exception e)
            {
                context.Database.RollbackTransaction();

                string message = e.InnerException == null ? e.Message : e.InnerException.Message;

                Console.WriteLine("Translation failed with error message: {0}", message);
            }
        }

        private static void db2files(Appsettings settings)
        {
            string path = settings.Db2FilesPath;

            string[] files = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly);

            if (files.Any())
            {
                string mess = string.Format("All .json files in {0} will be deleted. Proceed? [Y/N] ", path);
                Console.Write(mess);
                string line = Console.ReadLine().ToLower();

                if (line.Last() != 'y')
                {
                    Console.WriteLine("Nothing has been changed");
                    Console.WriteLine("");
                    return;
                }

                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }

            TranslateContext context = new TranslateContext(settings.ConnectionString);

            List<Language> languages = context.Languages.ToList();
            List<EnglishMessage> englishMessages = context.EnglishMessages.ToList();
            List<Translate> translates = context.Translates.ToList();
            List<TranslateVersion> translateVersions = context.TranslateVersions.ToList();


            foreach (Language language in languages)
            {
                Dictionary<string, string> translatesDict = new Dictionary<string, string>();
                foreach (EnglishMessage message in englishMessages)
                {
                    foreach (Translate translate in translates.Where(x => 
                        x.EnglishMessageId == message.Id &&
                        x.LanguageId == language.Id && 
                        !x.Reference))
                    {
                        translatesDict.Add(message.Name, translate.Name);
                        foreach (TranslateVersion version in translateVersions.Where(x => 
                            x.TranslateId == translate.Id && !x.Reference))
                        {
                            translatesDict.Add(message.Name + "." + version.Version, version.Name);
                        }
                    }
                }
                string json = JsonConvert.SerializeObject(translatesDict, Formatting.Indented);
                File.WriteAllText(path + language.Code + ".json", json);
            }

            Console.WriteLine("{0} file(s) created", context.Languages.Count());
            Console.WriteLine("");
        }

        private static bool firstParamValid(string param)
        {
            return firstParams.Any(x => x == param);
        }

        static void Main(string[] args)
        {
            if (args == null || !args.Any() || !firstParamValid(args[0]))
            {
                Console.WriteLine("Usage: translate db2files or translate 'files2db' 'all or en ru fr...'");
                return;
            }

            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string appsettings = appPath + "appsettings.json";

            if (!File.Exists(appsettings))
            {
                Console.WriteLine("File appsettings.json not found");
                return;
            }

            string json = File.ReadAllText(appsettings);
            Appsettings settings = JsonConvert.DeserializeObject<Appsettings>(json);

            if (args[0] == firstParams[0])
            {
                db2files(settings);
                return;
            }

            List<string> ar = args.ToList();
            ar.RemoveAt(0);
            files2db(ar.ToArray(), settings);
        }
    }
}
