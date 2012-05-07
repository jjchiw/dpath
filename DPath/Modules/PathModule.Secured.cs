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
using DPath.Tasks;

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
				m.NewPathActive = "active";
				return View["Path/Create", m];
			};

			

			Post["/create"] = _ =>
			{
				using(IDocumentSession session = _documentStore.OpenSession())
				{
					var path = new Path
					{
						Name = Request.Form.name,
						Description = Request.Form.description,
						User = (Context.CurrentUser as User),
						DateCreated = DateTime.UtcNow,
						LastUpdated = DateTime.UtcNow,
						SubscribedUsers = new List<string> { (Context.CurrentUser as User).Id }
					};

					session.Store(path);
					session.SaveChanges();

					return Response.AsJson(new { name = path.Name, id = path.Id, description = path.Description });
				}
			};

			Post["/{id}/add-goal"] = parameters =>
			{
				using (IDocumentSession session = _documentStore.OpenSession())
				{
                    Path path = PathTasks.GetPath(this, parameters as DynamicDictionary, session);
					if (path == null)
						return new Response { StatusCode = HttpStatusCode.NotFound };
					
					if(path == null)
						return Response.AsJson(new { }, HttpStatusCode.NotFound);

					var goal = new Goal
					{
						Id = Guid.NewGuid().ToString(),
						Name = Request.Form.name,
						Order = path.Goals.Count
					};

					path.LastUpdated = DateTime.UtcNow;
					path.Goals.Add(goal);

					session.Store(path);
					session.SaveChanges();

					return Response.AsJson(new { Name = goal.Name, Id = goal.Id });
				}
			};

			Post["/{id}/goal/{goalId}/{resolution}"] = parameters =>
			{
				using (IDocumentSession session = _documentStore.OpenSession())
				{
                    var result = PathTasks.AddResolution(this, parameters, session, (Context.CurrentUser as User), Request.Form.comment);

                    if (result == null)
                        return HttpStatusCode.NotFound;

                    var achievement = result.Achievement as Achievement;
                    var goal = result.Goal as Goal;

					var achievementView = achievement.ConverToAchievementView();
					var astrayCount = goal.Achievements.Count(x => x.Resolution == Resolution.Astray);
					var onCourseCount = goal.Achievements.Count(x => x.Resolution == Resolution.OnCourse);

					return Response.AsJson(new { AchievementView = achievementView, AstrayCount = astrayCount, OnCourseCount = onCourseCount });
				}
			};

			Get["/my-paths"] = _ =>
			{
				using (IDocumentSession session = _documentStore.OpenSession())
				{
					var userId = (Context.CurrentUser as User).Id;

					var pathsWithCustomer = session.Advanced.LuceneQuery<Path>()
															.Where(string.Format("SubscribedUsers:({0})", userId));

					var results = new List<PathView>(); // Prepare our results list
					foreach (var path in pathsWithCustomer)
					{
						// For each user, load its associated alias based on that user Id
						results.Add(path.ConvertToPathView());
					}

					var m = Context.Model("My Paths");
					m.Paths = results;
					m.MyPathsActive = "active";
					return View["Path/MyPaths", m];
				}
			};
		}
	}
}