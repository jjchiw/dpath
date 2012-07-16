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

			Get[@"/{id}/view-goal/{goalId}/(?<from>[\d*])"] = parameters =>
			{
				var pathId = String.Format("{0}/{1}", this.ModulePath, parameters.id.Value as string);

				var from = Int32.Parse(parameters.from);

				Path path = RavenSession.Load<Path>(pathId);
				if (path == null)
					return new Response { StatusCode = HttpStatusCode.NotFound };

				var pathView = path.ConvertToPathView();
				var goal = path.Goals.FirstOrDefault(x => x.Id == parameters.goalId);
				var goalView = pathView.Goals.FirstOrDefault(x => x.Id == parameters.goalId);

				var m = Context.Model(path.Name);
				m.LoggedIn = Context.IsLoggedIn();
				m.Path = new { Name = pathView.Name, Id = pathView.Id };
				m.Goal = new { Name = goalView.Name, Id = goalView.Id };
				m.AllAchievements = SortAchievements(goal.Achievements, from);

				if(Context.IsLoggedIn())
					m.MyAchievements = SortAchievements(goal.Achievements.Where(x => x.User.Email == Context.UserEmail()), from);


				m.OnCourseAchievements = SortAchievements(goal.Achievements	.Where(x => x.Resolution == Resolution.OnCourse), from);

				m.AstrayAchievements = SortAchievements(goal.Achievements.Where(x => x.Resolution == Resolution.Astray), from);

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