using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Security;
using DPath.Helpers;
using Raven.Client;
using DPath.Models;
using DPath.Models.ViewModels;

namespace DPath.Modules
{
	public class PathSecuredModule : NancyModule
	{
		private IDocumentStore _documentStore;
		public PathSecuredModule (IDocumentStore documentStore) : base("paths")
		{
			this.RequiresAuthentication();

			_documentStore = documentStore;

			Get["/create"] = parameters =>
			{
				var m = Context.Model("Create Path");
				return View["Path/Index", m];
			};

			Post["/create"] = _ =>
			{
				using(IDocumentSession session = _documentStore.OpenSession())
				{
					var path = new Path
					{
						Name = Request.Form.name,
						User = (Context.CurrentUser as User)
					};

					session.Store(path);
					session.SaveChanges();

					return Response.AsJson(new { name = path.Name, id = path.Id });
				}
			};

			Post["/{id}/add-goal"] = parameters =>
			{
				using (IDocumentSession session = _documentStore.OpenSession())
				{
					var pathId = String.Format("{0}/{1}", this.ModulePath, parameters.id.Value as string);
					var path = session.Load<Path>(pathId);
					
					if(path == null)
						return Response.AsJson(new { }, HttpStatusCode.NotFound);

					var goal = new Goal
					{
						Name = Request.Form.name
					};

					path.Goals.Add(goal);

					session.Store(path);
					session.SaveChanges();

					return Response.AsJson(new { name = goal.Name });
				}
			};

			Get["/my-paths"] = _ =>
			{
				using (IDocumentSession session = _documentStore.OpenSession())
				{
					
					//var paths = session.Query<Path>().Where(x => x.User.Id == (Context.CurrentUser as User).Id)
					//                                 .Select(x => ConvertToPathView(x));

					var userId = (Context.CurrentUser as User).Id;
					var pathsWithCustomer = session.Query<Path>().Customize(x => x.Include<User>(y => y.Id))
																 .Where(x => x.User.Id == userId);

					var results = new List<PathView>(); // Prepare our results list
					foreach (var path in pathsWithCustomer)
					{
						// For each user, load its associated alias based on that user Id
						results.Add(ConvertToPathView(path));
					}

					var m = Context.Model("My Paths");
					m.Paths = results;
					return View["Path/MyPaths", m];
				}
			};
		}

		private PathView ConvertToPathView(Path path)
		{ 
			return new PathView
			{
				Id = path.Id,
				Name = path.Name,
				TotalOncourse = path.Goals.Sum(y => y.OnCourse.Count),
				TotalAstray = path.Goals.Sum(y => y.Astray.Count)
			};
		}
		
	}
}