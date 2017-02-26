using Autofac;
using Autofac.Extensions.DependencyInjection;
using LinksNews.Core;
using LinksNews.Services;
using LinksNews.Services.Data;
using LinksNews.Services.News;
using LinksNews.Services.News.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace LinksNews
{
    public class Startup
    {
        private static string applicationPath = string.Empty;
        private static string contentRootPath = string.Empty;
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            applicationPath = env.WebRootPath;
            contentRootPath = env.ContentRootPath;

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            //if (env.IsDevelopment())
            //{
            //    // This reads the configuration keys from the secret store.
            //    // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
            //  //  builder.AddUserSecrets();
            //}
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.AddMemoryCache();

            services.AddDbContext<LinksContext>(options =>
                options.UseSqlServer(Configuration["Data:LinksConnection:ConnectionString"]));

            //// Repositories
            //services.AddScoped<AccountRepository, AccountRepository>();
            //services.AddScoped<RoleRepository, RoleRepository>();
            //services.AddScoped<LoggingRepository, LoggingRepository>();
            //services.AddScoped<PagesRepository, PagesRepository>();
            //services.AddScoped<ColumnsRepository, ColumnsRepository>();
            //services.AddScoped<RowsRepository, RowsRepository>();
            //services.AddScoped<LinksRepository, LinksRepository>();
            //services.AddScoped<NewsProviderRepository, NewsProviderRepository>();
            

            //// Services
            //services.AddScoped<AccountService, AccountService>();
            //services.AddScoped<EncryptionService, EncryptionService>();
            //services.AddScoped<FileService, FileService>();
            //services.AddScoped<AdService, AdService>();
            //services.AddScoped<ExecutionService, ExecutionService>();
            //services.AddScoped<UtilsService, UtilsService>();


            //// News provider
            //services.AddScoped<NewsProviderFactory, NewsProviderFactory>();
            //services.AddScoped<NewsService, NewsService>();


            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();



            //services.AddAuthentication();

            // Polices
            //services.AddAuthorization();
            //services.AddAuthorization(options =>
            //{
            //    // inline policies
            //    options.AddPolicy("AdminOnly", policy =>
            //    {
            //        policy.RequireClaim(ClaimTypes.Role, "Admin");
            //    });

            //});

            services.AddOptions();
            services.Configure<LinksOptions>(Configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalOrigins",
                    builder =>
                    {
                        builder
                            //.AllowAnyOrigin()
                            .WithOrigins("http://localhost:8080", "https://localhost:8080")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });


            services.AddMvc();


            // Add Autofac
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<DefaultModule>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            // This solution has been taken from http://benjii.me/2016/01/angular2-routing-with-asp-net-core-1/
            // Using file server was excluded as unnecessary
            // Route all unknown requests to app root
            app.Use(async (context, next) =>
            {
                if (!context.Request.IsHttps)
                {
                    var httpsUrl = "https://" + context.Request.Host + context.Request.Path;
                    context.Response.Redirect(httpsUrl);
                    return;
                }

                await next();

                if (context.Response.StatusCode != 404)
                {
                    return;
                }


                if (!string.IsNullOrWhiteSpace(context.Request.Path.Value) && context.Request.Path.Value.StartsWith("/fake_image"))
                {
                    context.Response.StatusCode = 200;
                    try
                    {
                        string url = context.Request.Query["url"];
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            using (var client = new HttpClient())
                            {
                                var bytes = await client.GetByteArrayAsync(url);
                                await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                            }
                        }
                    }
                    catch
                    {
                    }
                    return;
                }

                // If there's no available file and the request doesn't contain an extension, we're probably trying to access a page.
                // Rewrite request to use app root
                if (!Path.HasExtension(context.Request.Path.Value))
                {
                    context.Request.Path = "/index.html"; // Put your Angular root page here 
                    context.Response.StatusCode = 200; // Make sure we update the status code, otherwise it returns 404
                    await next();
                    return;
                }

            });

            //// Serve wwwroot as root
            //app.UseFileServer();

            //// Serve /node_modules as a separate root (for packages that use other npm modules client side)
            //app.UseFileServer(new FileServerOptions()
            //{
            //    // Set root of file server
            //    FileProvider = new PhysicalFileProvider(Path.Combine(environment.WebRootPath, "js")),
            //    // Only react to requests that match this path
            //    RequestPath = "/js",
            //    // Don't expose file system
            //    EnableDirectoryBrowsing = false
            //});



            /*
            app.Use(async (context, next) =>
            {
                if (context.Request.IsHttps)
                {
                    await next();
                }
                else
                {
                    var withHttps = "https://" + context.Request.Host + context.Request.Path;
                    context.Response.Redirect(withHttps);
                }
            });
            */

            //app.UseExceptionHandler(errorApp =>
            //{
            //    errorApp.Run(async context =>
            //    {
            //        context.Response.StatusCode = 500; // or another Status accordingly to Exception Type
            //        context.Response.ContentType = "application/json";

            //        var error = context.Features.Get<IExceptionHandlerFeature>();
            //        if (error == null)
            //        {
            //            return;
            //        }
            //        var ex = error.Error;

            //        if (ex == null)
            //        {
            //            return;
            //        }

            //        LinksException exception = error.Error as LinksException;

            //        if (exception == null && error.Error.InnerException != null)
            //        {
            //            exception = error.Error.InnerException as LinksException;
            //        }

            //        if (exception == null || exception.Severity != ExceptionSeverity.Info)
            //        {
            //            return;
            //        }


            //        await context.Response.WriteAsync(JsonConvert.SerializeObject(new 
            //        {
            //            Message = ex.Message
            //        }), Encoding.UTF8);
            //    });
            //});


            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            //TODO: how to restrict access to site's folders?

            //TODO: what is antiforgery ?


            // TODO: configure CORS to accept own requests only
            //app.UseCors(config =>
            //    config.AllowAnyHeader()
            //        .AllowAnyMethod()
            //        .AllowAnyOrigin());

            //AutoMapperConfiguration.Configure();

            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AutomaticAuthenticate = true,
            //    AutomaticChallenge = false,
            //    CookieName = "LinksAuth",
            //    Events = new CookieAuthenticationEvents
            //    {
            //        OnValidatePrincipal = LoginValidator.ValidateAsync
            //    }
            //});

            //app.UseCookieAuthentication(options =>
            //{
            //    options.Events = new CookieAuthenticationEvents
            //    {
            //        // Set other options
            //        OnValidatePrincipal = LastChangedValidator.ValidateAsync
            //    };
            //});

            app.UseCors("AllowLocalOrigins");
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
