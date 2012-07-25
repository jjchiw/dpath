using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Security;
using Raven.Client;
using DPath.Models;
using DPath.Helpers;
using DPath.Models.ViewModels;

namespace DPath.Modules
{
	public class PathModule : RavenModule
	{
		public PathModule() : base("paths")
		{
			Get["/{id}"] = parameters =>
			{
				var pathId = String.Format("{0}/{1}", this.ModulePath, parameters.id.Value as string);
				Path path = RavenSession.Load<Path>(pathId);
				if (path == null)
					return new Response { StatusCode = HttpStatusCode.NotFound };

				var m = Context.Model(path.Name);
				m.Path = path.ConvertToPathView();
				m.IsOwner = path.User.Email == Context.UserEmail();
				m.MemberOf = path.SubscribedUsers.Contains(Context.UserRavenIdString());
				

				if (!m.MemberOf)
					m.HomeActive = "active";
				else
					m.MyPathsActive = "active";

				return View["Path/View", m];
			};

			Get["/{id}/all-stats"] = parameters =>
			{
				var pathId = String.Format("{0}/{1}", this.ModulePath, parameters.id.Value as string);
				Path path = RavenSession.Load<Path>(pathId);
				if (path == null)
					return new Response { StatusCode = HttpStatusCode.NotFound };


				var m  = path.Goals.Select(x => new
				{
					Id = x.Id,
					Name = x.Name,
					TOnCourse = x.Achievements.Where(y => y.Resolution == Resolution.OnCourse).Count(),
					TAstray = x.Achievements.Where(y => y.Resolution == Resolution.Astray).Count()
				}).ToList();

				m.Add(new
				{
					Id = "0",
					Name = "0",
					TOnCourse = path.Goals.Sum(x => x.Achievements.Where(y => y.Resolution == Resolution.OnCourse).Count()),
					TAstray = path.Goals.Sum(x => x.Achievements.Where(y => y.Resolution == Resolution.Astray).Count())
				});

				return Response.AsJson(new { m }, HttpStatusCode.OK);
			};

			Get[@"/{id}/view-goal/{goalId}/(?<from>[\d*])"] = parameters =>
			{
				var pathId = String.Format("{0}/{1}", this.ModulePath, parameters.id.Value as string);

				var from = Int32.Parse(parameters.from);

				Path path = RavenSession.Load<Path>(pathId);
				if (path == null)
					return new Response { StatusCode = HttpStatusCode.NotFound };

				var pathView = path.ConvertToPathView();
				var m = Context.Model(path.Name);
				m.LoggedIn = Context.IsLoggedIn();
				m.Path = new { Name = pathView.Name, Id = pathView.Id };

				Goal goal = null;
				List<Achievement> achievements = null;
				if (parameters.goalId.Value == "0")
				{
					m.Goal = new { Name = "All", Id = 0 };
					achievements = path.Goals.SelectMany(x => x.Achievements).ToList();
					m.AcceptInLineAchievement = false;
				}
				else 
				{
					goal = path.Goals.FirstOrDefault(x => x.Id == parameters.goalId);
					m.Goal = new { Name = goal.Name, Id = goal.Id };
					achievements = goal.Achievements;
				}

				m.AllAchievements = SortAchievements(achievements, from);

				if (Context.IsLoggedIn())
				{
					m.MyAchievements = SortAchievements(achievements.Where(x => x.User.Email == Context.UserEmail()), from);
					if (parameters.goalId.Value != "0")
					m.AcceptInLineAchievement = true;
				}
					

				return Response.AsJson(new { m }, HttpStatusCode.OK);
			};
		}

		private IEnumerable<AchievementView> SortAchievements(IEnumerable<Achievement> achievements, int fromValue)
		{
			return achievements	.Select(x => x.ConverToAchievementView(40, true))
								.OrderByDescending(x => x.DateCreated)
								.Skip(fromValue * 20)
								.Take(20);
		}
	}
}