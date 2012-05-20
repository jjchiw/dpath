using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Raven.Client;
using Raven.Client.Document;
using TinyIoC;

namespace DPath.Modules
{
	public class RavenModule  : NancyModule
	{
		protected IDocumentSession RavenSession;

		protected IDocumentStore RavenDocumentStore
		{
			get
			{
				return Context.Items["RavenDocumentStore"] as IDocumentStore;
			}
		}

		public RavenModule(string path) : base(path)
		{
			this.Before.AddItemToEndOfPipeline(ctx =>
			{
				RavenSession = RavenDocumentStore.OpenSession();
				return null;
			});

			this.After.AddItemToEndOfPipeline(ctx =>
			{
				if (RavenSession != null)
				{
					RavenSession.SaveChanges();
					RavenSession.Dispose();
				}

			});

			
		}
		
		public RavenModule() : this("")
		{
		}




	}
}