﻿using System.Net.Http.Formatting;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Thinktecture.IdentityServer.v3.Admin.WebApi.Filters;
using Thinktecture.IdentityServer.v3.Admin.WebApi.Storage;

namespace Thinktecture.IdentityServer.v3.Admin.WebApi
{
	public static class ThinktectureIdentityServerAdminExtension
	{
		public static void UseThinktectureIdentityServerAdmin(this IAppBuilder app, StorageOptions storageOptions)
		{
		    var httpConfiguration = new HttpConfiguration();
			var container = RegisterServices(httpConfiguration, storageOptions);
			
			SetupHttpConfiguration(httpConfiguration, container);

            ConfigureJson(httpConfiguration);
			ConfigureRoutes(httpConfiguration);

			app.UseWebApi(httpConfiguration);
		}

		private static void SetupHttpConfiguration(HttpConfiguration configuration, IContainer container)
		{
		    configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
		    configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
		}

		private static IContainer RegisterServices(HttpConfiguration configuration, StorageOptions storageOptions)
		{
			var builder = new ContainerBuilder();

		    builder.RegisterApiControllers(typeof (ThinktectureIdentityServerAdminExtension).Assembly);
		    builder.RegisterInstance(storageOptions).AsSelf();
		    builder.RegisterModule(new StorageModule(storageOptions));

            builder.RegisterWebApiFilterProvider(configuration);
		    builder.RegisterType<ExceptionFilter>()
                .AsWebApiExceptionFilterFor<ApiController>()
		        .InstancePerRequest();

			return builder.Build();
		}

		private static void ConfigureJson(HttpConfiguration config)
		{
			config.Formatters.Clear();
			config.Formatters.Add(new JsonMediaTypeFormatter());

			var jsonFormatter = config.Formatters.JsonFormatter;
			jsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
			jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
		}

		private static void ConfigureRoutes(HttpConfiguration config)
		{
			config.Routes.MapHttpRoute("Default", "api/{controller}/{action}");
		}
	}
}
