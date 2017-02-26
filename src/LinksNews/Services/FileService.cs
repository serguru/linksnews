using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using LinksNews.Services.Data;
using LinksNews.Services.Data.Models;
using Microsoft.Extensions.Options;
using LinksNews.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using ImageProcessor.Imaging.Formats;
using ImageProcessor;
using System.Drawing;

namespace LinksNews.Services
{
    public class FileService
    {
        private readonly IOptions<LinksOptions> options;
        private readonly IHostingEnvironment environment;
        private readonly ExecutionService es;
        private readonly UtilsService us;

        Tuple<string, string>[] imageTypes =
        {
            new Tuple<string,string>( "image/x-png", "png" ),
            new Tuple<string,string>( "image/pjpeg", "jpg" ),
            new Tuple<string,string>( "image/jpeg", "jpg" ),
            new Tuple<string,string>( "image/bmp", "bmp" ),
            new Tuple<string,string>( "image/png", "png" ),
            new Tuple<string,string>( "image/gif", "gif" )
        };

        public FileService(
            IOptions<LinksOptions> options, 
            IHostingEnvironment environment,
            UtilsService us,
            ExecutionService es
            )
        {
            this.options = options;
            this.environment = environment;
            this.us = us;
            this.es = es;
        }

        public Paths GetLoginImagePaths(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }

            string relativePath = Path.Combine(options.Value.AccountImagesPath, login);
            string absolutePath = Path.Combine(environment.WebRootPath, relativePath);

            if (!Directory.Exists(absolutePath))
            {
                return null;
            }

            return new Paths(absolutePath, relativePath);
        }

        private void reduceImageSize(IFormFile file, FileStream fileStream)
        {
            ISupportedImageFormat format = new JpegFormat { Quality = 70 };
            Size size = new Size(options.Value.Images.ConvertWidth, 0);

            using (var inStream = file.OpenReadStream())
            {
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                {
                    imageFactory.Load(inStream)
                                .Resize(size)
                                .Format(format)
                                .Save(fileStream);
                }
            }
        }

        // TODO: reduce login and page names down to reasonable minimum, because path length is limited wih 248 (?) characters
        public async Task<string> UploadElementImage(string login, PageElement pageElement, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                es.ThrowException("File is empty");
            }

            if (string.IsNullOrWhiteSpace(options.Value.AccountImagesPath))
            {
                es.ThrowException("No account images path in configuration");
            }

            if (string.IsNullOrWhiteSpace(login))
            {
                es.ThrowException("No login to save an image to");
            }

            Tuple<string, string> ext = imageTypes.FirstOrDefault(x => string.Equals(x.Item1, file.ContentType, StringComparison.OrdinalIgnoreCase));

            if (ext == null)
            {
                es.ThrowInfoException("File to upload is not an image");
            }

            if (file.Length > options.Value.Images.InputSizeLimit * 1024 * 1024)
            {
                es.ThrowInfoException("File to upload exceeds a limit {0} Mb", 
                    options.Value.Images.InputSizeLimit.ToString());
            }

            string relativePath = Path.Combine(options.Value.AccountImagesPath, login, pageElement.ToString());
            string path = Path.Combine(environment.WebRootPath, relativePath);

            Directory.CreateDirectory(path);

            List<string> files =
                Directory
                    .GetFiles(path)
                    .Where(x => Path.GetFileNameWithoutExtension(x) == file.FileName ||
                        Path.GetFileNameWithoutExtension(x) == file.FileName + "_")
                    .ToList();

            bool add_ = files.Count > 0 && !files.Any(x => Path.GetFileNameWithoutExtension(x) == file.FileName + "_");
                
            files.ForEach(x => File.Delete(x));

            string fileName = file.FileName + (add_ ? "_" : "") + "." + ext.Item2;

            using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                if (file.Length > options.Value.Images.ConvertSizeLimit * 1024)
                {
                    reduceImageSize(file, fileStream);
                }
                else
                {
                    await file.CopyToAsync(fileStream);
                }
            }

            return Path.Combine(relativePath, fileName).Replace('\\','/');
        }

        public void DeleteElementImage(long id, string login, PageElement pageElement)
        {
            string path = Path.Combine(environment.WebRootPath, options.Value.AccountImagesPath, login, pageElement.ToString());

            if (!Directory.Exists(path))
            {
                return;
            }

            Directory
                .GetFiles(path)
                .Where(x =>
                    Path.GetFileNameWithoutExtension(x) == id.ToString() ||
                    Path.GetFileNameWithoutExtension(x) == id.ToString() + "_"
                    )
                .ToList()
                .ForEach(x => File.Delete(x));
        }

        public async Task<string> UploadAccountImage(string login, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                es.ThrowException("File is empty");
            }

            if (string.IsNullOrWhiteSpace(options.Value.AccountImagesPath))
            {
                es.ThrowException("No account images path in configuration");
            }

            if (string.IsNullOrWhiteSpace(login))
            {
                es.ThrowException("No login to save an image to");
            }

            Tuple<string, string> ext = imageTypes.FirstOrDefault(x => string.Equals(x.Item1, file.ContentType, StringComparison.OrdinalIgnoreCase));

            if (ext == null)
            {
                es.ThrowInfoException("File to upload is not an image");
            }

            if (file.Length > options.Value.Images.InputSizeLimit * 1024 * 1024)
            {
                es.ThrowInfoException("File to upload exceeds a limit {0} Mb",
                    options.Value.Images.InputSizeLimit.ToString());
            }

            string relativePath = Path.Combine(options.Value.AccountImagesPath, login);
            string path = Path.Combine(environment.WebRootPath, relativePath);

            Directory.CreateDirectory(path);

            List<string> files =
                Directory
                    .GetFiles(path)
                    .Where(x => Path.GetFileNameWithoutExtension(x) == file.FileName ||
                        Path.GetFileNameWithoutExtension(x) == file.FileName + "_")
                    .ToList();

            bool add_ = files.Count > 0 && !files.Any(x => Path.GetFileNameWithoutExtension(x) == file.FileName + "_");

            files.ForEach(x => File.Delete(x));

            string fileName = file.FileName + (add_ ? "_" : "") + "." + ext.Item2;

            using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                if (file.Length > options.Value.Images.ConvertSizeLimit * 1024)
                {
                    reduceImageSize(file, fileStream);
                }
                else
                {
                    await file.CopyToAsync(fileStream);
                }
            }

            return Path.Combine(relativePath, fileName).Replace('\\', '/');
        }

        public void DeleteAccountImage(string login)
        {
            string path = Path.Combine(environment.WebRootPath, options.Value.AccountImagesPath, login);

            if (!Directory.Exists(path))
            {
                return;
            }

            Directory
                .GetFiles(path)
                .Where(x => 
                    string.Equals(Path.GetFileNameWithoutExtension(x), login, StringComparison.OrdinalIgnoreCase)
                    ||
                    string.Equals(Path.GetFileNameWithoutExtension(x), login + "_", StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(x => File.Delete(x));
        }

        public string GetAccountImageUrl(string login)
        {
            Paths paths = GetLoginImagePaths(login);

            if (paths == null)
            {
                return null;
            }

            List<string> files = Directory.GetFiles(paths.Absolute, login + ".*", SearchOption.TopDirectoryOnly).ToList();
            files.AddRange(Directory.GetFiles(paths.Absolute, login + "_.*", SearchOption.TopDirectoryOnly));

            if (!files.Any())
            {
                return null;
            }

            string result = Path.Combine(paths.Relative, Path.GetFileName(files[0]));
            result = result.Replace('\\', '/');
            return result;
        }

        public void DeleteAccountFolder(string login)
        {
            Paths paths = GetLoginImagePaths(login);

            if (paths == null)
            {
                return;
            }

            Directory.Delete(paths.Absolute, true);
        }

        public string GetPageElementImageUrl(long Id, PageElement element, string login)
        {
            Paths paths = GetLoginImagePaths(login);

            if (paths == null)
            {
                return null;
            }

            paths.Absolute = Path.Combine(paths.Absolute, element.ToString());

            if (!Directory.Exists(paths.Absolute))
            {
                return null;
            }

            List<string> files = Directory.GetFiles(paths.Absolute, Id.ToString() + ".*", SearchOption.TopDirectoryOnly).ToList();

            files.AddRange(Directory.GetFiles(paths.Absolute, Id.ToString() + "_.*", SearchOption.TopDirectoryOnly));

            if (!files.Any())
            {
                return null;
            }

            paths.Relative = Path.Combine(paths.Relative, element.ToString());

            string result = Path.Combine(paths.Relative, Path.GetFileName(files[0]));
            result = result.Replace('\\', '/');
            return result;
        }
    }
}
