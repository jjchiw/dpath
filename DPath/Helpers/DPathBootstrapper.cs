using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using TinyIoC;
using Nancy.Bootstrapper;
using Nancy.Authentication.Forms;
using Raven.Client.Document;
using Raven.Client;
using DPath.Models.Repositories;
using Nancy.ViewEngines;
using System.Configuration;
using Raven.Abstractions.Data;

namespace DPath.Helpers
{
	public class DPathBootstrapper : DefaultNancyBootstrapper
	{

		byte[] favicon;

		protected override byte[] DefaultFavIcon
		{
			get
			{
				if (favicon == null)
				{
					//TODO: remember to replace 'AssemblyName' with the prefix of the resource
					using (var resourceStream = GetType().Assembly.GetManifestResourceStream("DPath.favicon.ico"))
					{
						var tempFavicon = new byte[resourceStream.Length];
						resourceStream.Read(tempFavicon, 0, (int)resourceStream.Length);
						favicon = tempFavicon;
					}
				}
				return favicon;
			}
		}


		protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer container)
		{
			// We don't call "base" here to prevent auto-discovery of
			// types/dependencies
		}

		protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
		{
			base.ConfigureRequestContainer(container, context);

		
			// Here we register our user mapper as a per-request singleton.
			// As this is now per-request we could inject a request scoped
			// database "context" or other request scoped services.

			var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionStringName("RavenDB");
			parser.Parse();

			var documentStore = new DocumentStore
			{
				ApiKey = parser.ConnectionStringOptions.ApiKey,
				Url = parser.ConnectionStringOptions.Url
			};
			
			documentStore.Initialize();

			container.Register<IDocumentStore>(documentStore);
			container.Register<IUserMapper, UserMapper>();

			context.Items["RavenDocumentStore"] = documentStore;
		}

		
		protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
		{
			// At request startup we modify the request pipelines to
			// include forms authentication - passing in our now request
			// scoped user name mapper.
			//
			// The pipelines passed in here are specific to this request,
			// so we can add/remove/update items in them as we please.
			var formsAuthConfiguration =
				new FormsAuthenticationConfiguration()
				{
					RedirectUrl = "/",
					UserMapper = requestContainer.Resolve<IUserMapper>(),
				};

			FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
		}
	}
}