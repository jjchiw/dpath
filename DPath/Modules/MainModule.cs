using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using DPath.Helpers;
using Raven.Client;
using DPath.Models;

namespace DPath.Modules
{
	public class MainModule : NancyModule
	{
		IDocumentStore _documentStore;

		public MainModule(IDocumentStore documentStore)
		{
			_documentStore = documentStore;
			Get["/"] = parameters =>
			{
				using (IDocumentSession session = _documentStore.OpenSession())
				{
					var m = Context.Model("Home");
					m.HomeActive = "active";
					m.Paths = session.Query<Path>()
									 .ToList()
									 .Select(x => x.ConvertToPathView())
									 .OrderByDescending(x => x.DateCreated)
									 .ToList();

					m.RecentPaths = session.Query<Path>()
									 .ToList()
									 .Select(x => x.ConvertToPathView())
									 .OrderByDescending(x => x.LastUpdated)
									 .ToList();

					return View["Views/Home", m];
				}
				
			};

			Get["about"] = parameters =>
			{
				var m = Context.Model("Home");
				m.AboutActive = "active";
				return View["Views/About", m];

			};
		}
	}
}