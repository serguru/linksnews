using Autofac;
using LinksNews.Services;
using LinksNews.Services.News;
using LinksNews.Services.News.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // builder.RegisterType<CharacterRepository>().As<ICharacterRepository>();

            //services.AddScoped<AccountRepository, AccountRepository>();
            builder.RegisterType<DataService>()
              .InstancePerLifetimeScope()
              .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterType<PagesService>()
              .InstancePerLifetimeScope()
              .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            // Services
            //builder.RegisterType<AccountService>().As<AccountService>();


            builder.RegisterType<AccountService>()
              .InstancePerLifetimeScope();

            builder.RegisterType<LogService>()
              .InstancePerLifetimeScope();

            builder.RegisterType<MailService>();
            builder.RegisterType<MessagesService>();
            builder.RegisterType<EncryptionService>();
            builder.RegisterType<FileService>();
            builder.RegisterType<AdService>();

            builder.RegisterType<ExecutionService>()
              .InstancePerLifetimeScope()
              .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterType<UtilsService>();

            // News provider
            builder.RegisterType<NewsProviderFactory>();
            builder.RegisterType<NewsService>();

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
        }
    }
}
